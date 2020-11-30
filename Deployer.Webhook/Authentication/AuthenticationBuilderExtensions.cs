using Microsoft.AspNetCore.Authentication;

namespace Deployer.Webhook.Authentication
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddScheme<THandler>(this AuthenticationBuilder builder, string authenticationScheme)
            where THandler : AuthenticationHandler<AuthenticationSchemeOptions> =>
            builder.AddScheme<AuthenticationSchemeOptions, THandler>(authenticationScheme, o => { });
    }
}
