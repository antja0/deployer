using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Deployer.Node.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NodeController : ControllerBase
    {
        private readonly ILogger<NodeController> _logger;

        public NodeController(ILogger<NodeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Deploy()
        {
            return Ok();
        }
    }
}
