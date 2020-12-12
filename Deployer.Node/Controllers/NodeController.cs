using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Deployer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deployer.Node.Controllers
{
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly ILogger<NodeController> _logger;
        private readonly NodeOptions _options;
        private readonly HttpClient _httpClient;

        public NodeController(ILogger<NodeController> logger, IOptions<NodeOptions> options, HttpClient httpClient)
        {
            _logger = logger;
            _options = options.Value;
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }


        [HttpPost(Routes.Deploy)]
        public async Task<IActionResult> Deploy([FromBody] DeployRequest deployRequest)
        {
            _logger.LogInformation($"Downloading version with GUID '{deployRequest.VersionGuid}'...");

            var fileInfo = new FileInfo($"{deployRequest.VersionGuid}.zip");

            var response = await _httpClient.GetAsync($"{_options.DeployerUrl}/api/Versions?{deployRequest.VersionGuid}");
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = System.IO.File.Create(fileInfo.FullName);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fileStream);

            _logger.LogInformation($"Version saved temporarily as '{fileInfo.Name}'.");

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                DeployToIIS(fileInfo, deployRequest.Projects);
            }).Start();

            return Ok();
        }

        private void DeployToIIS(FileInfo file, List<string> projects)
        {
            _logger.LogInformation("Starting to deploy to IIS sites.");
            

            _logger.LogInformation("Done.");
        }
    }
}
