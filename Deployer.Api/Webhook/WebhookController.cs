using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Deployer.Api.Webhook.Models;
using Deployer.Data;
using Deployer.Data.Models;

namespace Deployer.Api.Webhook
{
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly DeployerContext _context;

        public WebhookController(ILogger<WebhookController> logger, DeployerContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Webhook (mainly) for Github.
        /// Informs Deployer that new release is available for deployment.
        /// </summary>
        /// <param name="applicationId">Name/ID of the application that is being released.</param>
        /// <param name="payload">Contains various information about sending repository.</param>
        [Authorize(AuthenticationSchemes = "Webhook")]
        [HttpPost("/api/release/{applicationId}")]
        public async Task<IActionResult> ReleaseWebhook(string applicationId, [FromBody] WebhookPayload payload)
        {
            if (string.IsNullOrWhiteSpace(applicationId) || payload.Repository == null)
            {
                return BadRequest($"Malicious payload - application not defined or '{nameof(WebhookPayload.Repository)}' not included");
            }

            _logger.LogDebug($"Webhook received release request from '{applicationId}'...");

            var application = await _context.Applications.FindAsync(applicationId);
            if (application == null)
            {
                _logger.LogInformation($"Adding new application '{applicationId}'...");

                // TODO Try get latest version here.

                application = new Application
                {
                    Id = applicationId,
                    Name = payload.Repository.Name,
                    Versions = new List<ApplicationVersion>(),
                    Deleted = false,
                };

                await _context.Applications.AddAsync(application);
            }
            
            if (application.Deleted)
            {
                _logger.LogWarning($"Tried to release already deleted application '{applicationId}'");
                return BadRequest();
            }

            // Create new release logic

            await _context.SaveChangesAsync();

            return Ok(application);
        }
    }
}
