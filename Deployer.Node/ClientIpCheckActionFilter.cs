using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Deployer.Node
{
    /// <remarks>
    /// Refactored from: https://docs.microsoft.com/en-us/aspnet/core/security/ip-safelist?view=aspnetcore-5.0
    /// TODO move to own nuget / use some other already made nuget.
    /// </remarks>
    public class ClientIpCheckActionFilter : ActionFilterAttribute
    {
        private readonly ILogger<ClientIpCheckActionFilter> _logger;
        private readonly string _whiteList;

        public ClientIpCheckActionFilter(string whiteList, ILogger<ClientIpCheckActionFilter> logger)
        {
            _whiteList = whiteList;
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            _logger.LogDebug($"Checking remote IpAddress: {remoteIp}");
            
            if (remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }

            if (_whiteList == "*")
            {
                _logger.LogWarning("All remote IP addresses are allowed - you can set IP white list in appsettings.json");
            }
            else
            {
                var whiteList = _whiteList.Split(';');
                if (!whiteList.Select(IPAddress.Parse).Contains(remoteIp))
                {
                    _logger.LogWarning($"Forbidden Request from IP: {remoteIp}");
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
