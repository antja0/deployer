using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Deployer.Data.Models;
using Deployer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Version = Deployer.Data.Models.Version;

namespace Deployer
{
    public class DeployerService : IDeployerService
    {
        private readonly DeployerOptions _options;
        private readonly ILogger<DeployerService> _logger;
        private readonly HttpClient _httpClient;

        public DeployerService(IOptions<DeployerOptions> options, ILogger<DeployerService> logger, HttpClient httpClient)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = httpClient;

            if (!Directory.Exists(_options.TempBuildsFolderPath))
            {
                _logger.LogError($"Builds directory not found: {_options.TempBuildsFolderPath}");
            }
        }

        public async Task DeployAsync(Version version, DeploymentGroup deploymentGroup)
        {
            var nodeProjectsDict = new Dictionary<Node, List<Project>>();
            foreach (var project in deploymentGroup.Projects)
            {
                if (project.Node.Deleted || !project.Node.Registered)
                {
                    _logger.LogDebug($"Projects '{project.Name}' Node '{project.Node.Name}' not registered or deleted, skipping...");
                    continue;
                }

                if (nodeProjectsDict.ContainsKey(project.Node))
                {
                    nodeProjectsDict[project.Node].Add(project);
                }
                else
                {
                    nodeProjectsDict.Add(project.Node, new List<Project> {project});
                }
            }

            foreach (var (node, projects) in nodeProjectsDict)
            {
                var request = new DeployRequest
                {
                    VersionGuid = version.Id,
                    Projects = projects.Select(i => i.Name).ToList(),
                };

                var response = await _httpClient.PostAsJsonAsync($"{node.ApiEndpoint}{Routes.Deploy}", request);

                response.EnsureSuccessStatusCode();
            }

            _logger.LogInformation("Uploading is complete.");
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

            var version = new Version
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationId = application.Id,
                Date = DateTime.Now
            };
            var buildDir = Path.Combine(buildsAppDir, version.Id);
            Directory.CreateDirectory(buildDir);

            RunScript(buildDir, script);

            var buildOutputDir = string.IsNullOrWhiteSpace(application.OutputFolder) ? buildDir : Path.Combine(buildDir, application.OutputFolder);
            if (!Directory.Exists(buildOutputDir))
            {
                _logger.LogError($"Output directory not found: {buildOutputDir}");
                return null;
            }

            var versionFilePath = GetVersionFilePath(version);
            _logger.LogInformation($"Zipping output from build directory into ${versionFilePath}");
            ZipFile.CreateFromDirectory(buildOutputDir, versionFilePath, CompressionLevel.Optimal, false, Encoding.UTF8);

            try
            {
                Directory.Delete(buildDir, true);
            }
            catch (UnauthorizedAccessException ex)
            {
                // TODO counter "System.UnauthorizedAccessException: Access to the path 'pack-.idx' is denied." if cloning with GIT.
                _logger.LogWarning(ex, "Error cleaning temp build folder: " + ex.Message);
            }

            return version;
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

        private string GetVersionFilePath(Version version)
        {
            var dir = Path.Combine(_options.VersionFolderPath, version.ApplicationId.Trim());
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, $"{version.Id}.zip");
        }
    }
}
