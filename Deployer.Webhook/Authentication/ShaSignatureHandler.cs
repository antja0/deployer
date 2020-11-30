using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deployer.Webhook.Authentication
{
    public class ShaSignatureHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string Sha1Prefix = "sha1=";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly WebhookOptions _options;

        public ShaSignatureHandler(IHttpContextAccessor httpContextAccessor, IOptions<WebhookOptions> options, IOptionsMonitor<AuthenticationSchemeOptions> authOptions, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(authOptions, logger, encoder, clock)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options.Value;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            httpContext.Request.Headers.TryGetValue("X-Hub-Signature", out var signatureWithPrefix);

            if (string.IsNullOrWhiteSpace(signatureWithPrefix))
            {
                return AuthenticateResult.Fail("X-Hub-Signature header not present or empty.");
            }

            // Verify SHA1 signature.
            var signature = (string)signatureWithPrefix;
            if (signature.StartsWith(Sha1Prefix, StringComparison.OrdinalIgnoreCase))
            {
                signature = signature.Substring(Sha1Prefix.Length);

                var body = httpContext.Request.BodyReader.AsStream();
                using var reader = new StreamReader(body);
                var bodyAsBytes = Encoding.ASCII.GetBytes(await reader.ReadToEndAsync());
                httpContext.Request.Body = new MemoryStream(bodyAsBytes); // Without this body stream is already red in Controller and cannot be used.

                using var sha1 = new HMACSHA1(Encoding.ASCII.GetBytes(_options.Secret));
                var hash = sha1.ComputeHash(bodyAsBytes);

                var hashString = ToHexString(hash);
                if (hashString.Equals(signature))
                {
                    var identity = new ClaimsIdentity(nameof(ShaSignatureHandler));
                    var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
            }

            return AuthenticateResult.Fail("Invalid X-Hub-Signature.");
        }

        public static string ToHexString(IReadOnlyCollection<byte> bytes)
        {
            var builder = new StringBuilder(bytes.Count * 2);

            foreach (var b in bytes)
            {
                builder.AppendFormat("{0:x2}", b);
            }

            return builder.ToString();
        }
    }
}
