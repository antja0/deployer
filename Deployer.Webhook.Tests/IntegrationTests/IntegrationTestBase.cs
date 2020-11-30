using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Deployer.Webhook.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Deployer.Webhook.Tests.IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        protected readonly HttpClient TestClient;
        private const string TestSecret = "TestSecret";

        protected IntegrationTestBase()
        {
            const string hostUrl = "https://localhost:1337";

            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseUrls(hostUrl);
                    builder.ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                    {
                        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            { "Webhook:Secret", TestSecret },
                        });
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

        protected void AddSignatureHeaders<TBody>(TBody body)
        {
            var bodyData = JsonConvert.SerializeObject(body);
            var bodyAsBytes = Encoding.ASCII.GetBytes(bodyData);

            using var sha = new HMACSHA256(Encoding.ASCII.GetBytes(TestSecret));
            var hash = sha.ComputeHash(bodyAsBytes);

            var hashString = ShaSignatureHandler.ToHexString(hash);
            TestClient.DefaultRequestHeaders.Add(ShaSignatureHandler.ShaSignatureHeader, new[] { ShaSignatureHandler.ShaPrefix + hashString });
        }
    }
}
