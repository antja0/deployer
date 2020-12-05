using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Deployer.Api.Webhook.Models;

namespace Deployer.Api.Webhook
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
        [Authorize(AuthenticationSchemes = "Webhook")]
        [HttpPost("/api/release/{app}")]
        public async Task<IActionResult> ReleaseWebhook(string app, [FromBody] WebhookPayload payload)
        {
            if (payload.Repository == null)
            {
                return BadRequest($"Malicious payload - '{nameof(WebhookPayload.Repository)}' not included");
            }

            return Ok();
        }
    }
}
