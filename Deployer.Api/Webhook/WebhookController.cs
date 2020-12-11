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
        private readonly IDeployerService _deployer;

        public WebhookController(ILogger<WebhookController> logger, DeployerContext context, IDeployerService deployer)
        {
            _logger = logger;
            _context = context;
            _deployer = deployer;
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
            if (string.IsNullOrWhiteSpace(eventId)) return BadRequest("Invalid eventId");
            if (payload.Repository == null) return BadRequest($"Malicious payload - '{nameof(WebhookPayload.Repository)}' not included");
            if (string.IsNullOrWhiteSpace(payload.Repository.Name)) return BadRequest("Invalid repository name");

            var deployEvent = await _context.Events.FirstOrDefaultAsync(i => i.EventId.Equals(eventId));
            if (deployEvent == null)
            {
                return NotFound($"Event '{eventId}' not found");
            }

            var applicationId = payload.Repository.Name;
            // Remove all illegal chars before adding, applicationId is used in build folder paths etc.
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                applicationId = applicationId.Replace(c.ToString(), "");
            }

            _logger.LogDebug($"Webhook received event '{eventId}' from '{applicationId}'...");

            var application = await _context.Applications.Include(i => i.DeploymentRules).FirstOrDefaultAsync(i => i.Id.Equals(applicationId));
            if (application == null)
            {
                _logger.LogInformation($"Adding new application '{applicationId}'...");

                application = new Application
                {
                    Id = applicationId,
                    Name = payload.Repository.Name.Replace("-", " "),
                    Deleted = false,
                    Versions = new List<Version>(),
                    DeploymentRules = new List<DeploymentRule>()
                };

                await _context.Applications.AddAsync(application);
                await _context.SaveChangesAsync();
                return Ok("Application created");
            }
            
            if (application.Deleted)
            {
                _logger.LogWarning($"Tried to release already deleted application '{applicationId}'");
                return BadRequest();
            }

            // Build and zip the new version.
            var version = await _deployer.BuildNewVersionAsync(application);
            version.UnListed = !deployEvent.ListNewVersions;
            await _context.Versions.AddAsync(version);

            // Create new release logic

            var deploymentRule = application.DeploymentRules?.FirstOrDefault(i => i.DeployAutomatically && i.Event.EventId == eventId);
            if (deploymentRule != null)
            {
                _logger.LogInformation($"Starting to deploy application '{applicationId}'...");
                // TODO deploy automatically
            }

            await _context.SaveChangesAsync();

            return Ok($"{eventId} event successful");
        }
    }
}
