using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using Deployer.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Version = Deployer.Data.Models.Version;

namespace Deployer
{
    public class DeployerService : IDeployerService
    {
        private readonly DeployerOptions _options;
        private readonly ILogger<DeployerService> _logger;

        public DeployerService(IOptions<DeployerOptions> options, ILogger<DeployerService> logger)
        {
            _options = options.Value;
            _logger = logger;

            if (!Directory.Exists(_options.TempBuildsFolderPath))
            {
                _logger.LogError($"Builds directory not found: {_options.TempBuildsFolderPath}");
            }
        }

        public async Task<Version> BuildNewVersionAsync(Application application)
        {
            _logger.LogInformation($"Started building application '{application.Id.Trim()}'");

            var scriptPath = Path.Combine(_options.ScriptsFolderPath, application.Script);
            if (!File.Exists(scriptPath))
            {
                _logger.LogError($"Script not found: {scriptPath}");
                return null;
            }

            var script = await File.ReadAllTextAsync(scriptPath);

            var buildsAppDir = Path.Combine(_options.TempBuildsFolderPath, application.Id.Trim());
            Directory.CreateDirectory(buildsAppDir);

            var versionGuid = Guid.NewGuid().ToString();
            var buildDir = Path.Combine(buildsAppDir, versionGuid);

            Directory.CreateDirectory(buildDir);

            RunScript(buildDir, script);

            var buildOutputDir = string.IsNullOrWhiteSpace(application.OutputFolder) ? buildDir : Path.Combine(buildDir, application.OutputFolder);
            if (!Directory.Exists(buildOutputDir))
            {
                _logger.LogError($"Output directory not found: {buildOutputDir}");
                return null;
            }

            var finalOutPutDir = Path.Combine(_options.VersionFolderPath, application.Id.Trim());
            Directory.CreateDirectory(finalOutPutDir);
            var finalOutputFile = Path.Combine(finalOutPutDir, $"{versionGuid}.zip");

            _logger.LogInformation($"Zipping output from build directory into ${finalOutputFile}");
            ZipFile.CreateFromDirectory(buildOutputDir, finalOutputFile, CompressionLevel.Optimal, false, Encoding.UTF8);

            try
            {
                Directory.Delete(buildDir, true);
            }
            catch (UnauthorizedAccessException ex)
            {
                // TODO counter "System.UnauthorizedAccessException: Access to the path 'pack-.idx' is denied." if cloning with GIT.
                _logger.LogWarning(ex, "Error cleaning temp build folder: " + ex.Message);
            }

            return new Version
            {
                Id = versionGuid,
                Date = DateTime.Now,
                Application = application,
            };
        }

        private void RunScript(string scriptFolder, string script)
        {
            using var runSpace = RunspaceFactory.CreateRunspace();
            try
            {
                runSpace.Open();
                runSpace.SessionStateProxy.Path.SetLocation(scriptFolder);
                using var ps = runSpace.CreatePipeline();
                ps.Commands.AddScript(script);
                ps.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            finally
            {
                runSpace.Close();
            }
        }
    }
}
