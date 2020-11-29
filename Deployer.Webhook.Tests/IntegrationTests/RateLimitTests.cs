﻿using System.Net;
using System.Threading.Tasks;
using Deployer.Webhook.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Deployer.Webhook.Tests.IntegrationTests
{
    [TestFixture]
    public class RateLimitTests : IntegrationTestBase
    {
        [Test]
        public async Task PostToWebhook_Once_ReturnsOk()
        {
            // Arrange
            var content = new WebhookPayload
            {
                Repository = new Repository
                {
                    Name = "test-app",
                    FullName = "organization/test-app"
                }
            };

            // Act
            var response = await TestClient.PostAsJsonAsync("api/release/test-app", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task PostToWebhook_RapidlyTooManyTimes_RateLimitingShouldOccur()
        {
            // Arrange
            var content = new WebhookPayload
            {
                Repository = new Repository
                {
                    Name = "test-app",
                    FullName = "organization/test-app"
                }
            };

            // Act
            for (int i = 0; i < 5; i++)
            {
                await TestClient.PostAsJsonAsync("api/release/test-app", content);
            }

            var response = await TestClient.PostAsJsonAsync("api/release/test-app", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        }
    }
}
