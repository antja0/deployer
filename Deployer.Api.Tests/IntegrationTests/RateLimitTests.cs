using System.Net;
using System.Threading.Tasks;
using Deployer.Api.Webhook.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Deployer.Api.Tests.IntegrationTests
{
    [TestFixture]
    public class RateLimitTests : IntegrationTestBase
    {
        [Test, Order(1)]
        public async Task PostToWebhook_WithoutHMAC_ReturnsUnauthorized()
        {
            // Arrange
            var request = new WebhookPayload
            {
                Repository = new Repository
                {
                    Name = "test-app",
                    FullName = "organization/test-app"
                }
            };

            // Act
            var response = await TestClient.PostAsJsonAsync("api/release/test-app", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test, Order(1)]
        public async Task PostToWebhook_Once_ReturnsOk()
        {
            // Arrange
            var request = new WebhookPayload
            {
                Repository = new Repository
                {
                    Name = "test-app",
                    FullName = "organization/test-app"
                }
            };

            var header = GetSignatureHeader(request, "Webhook");

            // Act
            var response = await TestClient.PostAsJsonAsync("api/release/test-app", request, header);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test, Order(10)]
        public async Task PostToWebhook_RapidlyTooManyTimes_RateLimitingShouldOccur()
        {
            // Arrange
            var request = new WebhookPayload
            {
                Repository = new Repository
                {
                    Name = "test-app",
                    FullName = "organization/test-app"
                }
            };

            var header = GetSignatureHeader(request, "Webhook");

            // Act
            for (int i = 0; i < 5; i++)
            {
                await TestClient.PostAsJsonAsync("api/release/test-app", request);
            }

            var response = await TestClient.PostAsJsonAsync("api/release/test-app", request, header);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        }
    }
}
