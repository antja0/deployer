using System;
using System.Threading.Tasks;
using Deployer.Data;
using Deployer.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Deployer.Api.Nodes
{
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly ILogger<NodeController> _logger;
        private readonly DeployerContext _context;

        public NodeController(ILogger<NodeController> logger, DeployerContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Register a new node to deployer.
        /// </summary>
        [Authorize(AuthenticationSchemes = "Nodes")]
        [HttpPost("/api/Nodes")]
        public async Task<IActionResult> Register([FromBody] Node node)
        {
            _logger.LogInformation("Registering new node...");

            node.Id = Guid.NewGuid().ToString();

            await _context.Nodes.AddAsync(node);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Node '{node.Name}' registration done.");

            return Ok(node);
        }
    }
}
