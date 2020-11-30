using System.Threading.Tasks;
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
        /// <param name="node">Name of the node that is registered.</param>
        [Authorize(AuthenticationSchemes = "Nodes")]
        [HttpPost("api/Nodes/{node}")]
        public async Task<IActionResult> RegisterNode(string node)
        {
            await _context.Nodes.AddAsync(new Node
            {
                Id = System.Guid.NewGuid().ToString(),
                Name = node,
                ApiEndpoint = "",
                Description = "",
                Registered = false
            });

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
