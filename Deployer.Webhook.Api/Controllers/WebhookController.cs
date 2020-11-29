using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Deployer.Webhook.Api.Controllers
{
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(ILogger<WebhookController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Webhook (mainly) for Github.
        /// Informs Deployer that new release is available for deployment.
        /// </summary>
        /// <param name="app">Name of the application that is released.</param>
        [HttpPost("api/release/{app}")]
        public async Task<IActionResult> GithubWebhook(string app)
        {
            return Ok();
        }
    }
}
