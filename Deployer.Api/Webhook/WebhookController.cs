using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Deployer.Api.Webhook.Models;
using Deployer.Data;
using Deployer.Data.Models;
using Microsoft.EntityFrameworkCore;

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
        /// Informs Deployer that new event occurred, eg. new release or branch is available for deployment.
        /// </summary>
        /// <param name="eventId">ID/Name of event that triggers. eg. 'pull-request' or 'push'</param>
        /// <param name="payload">Contains various information about sending repository.</param>
        [Authorize(AuthenticationSchemes = "Webhook")]
        [HttpPost("/api/{eventId}")]
        public async Task<IActionResult> ReleaseWebhook(string eventId, [FromBody] WebhookPayload payload)
        {
            if (string.IsNullOrWhiteSpace(eventId)) return BadRequest("Invalid applicationId or eventId.");
            if (payload.Repository == null) return BadRequest($"Malicious payload - '{nameof(WebhookPayload.Repository)}' not included");

            var eventType = GetDeploymentType(eventId);
            if (eventType == DeploymentType.Undefined)
            {
                return BadRequest($"Event type '{eventId}' not defined");
            }

            var applicationId = payload.Repository.FullName;

            _logger.LogDebug($"Webhook received event '{eventType}' from '{applicationId}'...");

            var application = await _context.Applications.Include(i => i.DeploymentRules).FirstOrDefaultAsync(i => i.Id.Equals(applicationId));
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

            var deploymentRule = application.DeploymentRules?.FirstOrDefault(i => i.DeployAutomatically && i.Type == eventType);
            if (deploymentRule != null)
            {
                _logger.LogInformation($"Starting to deploy application '{applicationId}'...");
                // TODO deploy automatically
            }

            await _context.SaveChangesAsync();

            return Ok(application);
        }

        private static DeploymentType GetDeploymentType(string eventId)
        {
            switch (eventId)
            {
                case "push":
                    return DeploymentType.Push;
                case "pullrequest":
                case "pull-request":
                    return DeploymentType.PullRequest;
                case "release":
                    return DeploymentType.Release;
                default:
                    return DeploymentType.Undefined;
            }
        }
    }
}
