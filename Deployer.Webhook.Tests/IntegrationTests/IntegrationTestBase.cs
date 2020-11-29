using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace Deployer.Webhook.Tests.IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        protected readonly HttpClient TestClient;

        protected IntegrationTestBase()
        {
            const string hostUrl = "https://localhost:1337";

            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseUrls(hostUrl);
                    builder.ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                    {
                    });
                    builder.ConfigureServices(services =>
                    {
                    });
                    builder.ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                    });
                });

            TestClient = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                // Note: Must specify HTTPS here because default is HTTP.
                BaseAddress = new Uri(hostUrl),
            });
        }
    }
}
