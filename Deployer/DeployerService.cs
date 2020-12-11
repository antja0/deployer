using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;
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

        public Version BuildNewVersion(Application application)
        {
            _logger.LogInformation($"Started building application '{application.Id}'");

            var oldScriptPath = Path.Combine(_options.ScriptsFolderPath, application.Script);
            if (!File.Exists(oldScriptPath))
            {
                _logger.LogError($"Script not found: {oldScriptPath}");
                return null;
            }

            var buildDir = Path.Combine(_options.TempBuildsFolderPath, application.Id, Guid.NewGuid().ToString());

            Directory.CreateDirectory(buildDir);

            var newScriptPath = Path.Combine(buildDir, $"{Guid.NewGuid()}.ps1");
            File.Copy(oldScriptPath, newScriptPath);

            RunScript(newScriptPath);

            var buildOutputDir = string.IsNullOrWhiteSpace(application.OutputFolder) ? buildDir : Path.Combine(buildDir, application.OutputFolder);
            if (!Directory.Exists(buildOutputDir))
            {
                _logger.LogError($"Output directory not found: {buildOutputDir}");
                return null;
            }

            var versionGuid = Guid.NewGuid().ToString();
            var finalOutputDir = Path.Combine(_options.VersionFolderPath, application.Id, $"{versionGuid}.zip");

            _logger.LogInformation($"Zipping output from build directory into ${finalOutputDir}");
            ZipFile.CreateFromDirectory(buildOutputDir, finalOutputDir, CompressionLevel.Optimal, false, Encoding.UTF8);
            
            Directory.Delete(buildDir, true);

            return new Version
            {
                Id = versionGuid,
                Date = DateTime.Now,
                Application = application,
            };
        }

        private void RunScript(string scriptPath)
        {
            try
            {
                using var ps = PowerShell.Create();
                ps.AddScript(scriptPath);
                ps.Invoke();

                if (ps.HadErrors)
                {
                    var errors = ps.Streams.Error.ReadAll();

                    var stringBuilder = new StringBuilder();
                    foreach (var error in errors)
                    {
                        stringBuilder.AppendLine(error.ToString());
                    }

                    var output = stringBuilder.ToString();
                    _logger.LogWarning("Script had errors: " + output);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
