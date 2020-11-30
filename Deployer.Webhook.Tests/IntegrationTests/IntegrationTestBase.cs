using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Antja.Authentication.HMAC;
using Antja.Authentication.HMAC.Utilities;
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

        private string _secret;
        private string _header;
        private HMACHashFunctions _hashFunction;

        protected IntegrationTestBase()
        {
            const string hostUrl = "https://localhost:1337";

            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseUrls(hostUrl);
                    builder.ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                    {
                        var config = configurationBuilder.Build();

                        _secret = config.GetValue("Webhook:Secret", "secret");
                        _header = config.GetValue("Webhook:Header", "X-Hub-Signature-256");
                        _hashFunction = config.GetValue("Webhook:HashFunction", HMACHashFunctions.SHA256);
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

            var hash = HMACUtilities.ComputeHash(_hashFunction, _secret, bodyAsBytes);
            var hashString = HMACUtilities.ToHexString(hash);
            TestClient.DefaultRequestHeaders.Add(_header, new[] { HMACUtilities.GetSignaturePrefix(_hashFunction) + hashString });
        }
    }
}
