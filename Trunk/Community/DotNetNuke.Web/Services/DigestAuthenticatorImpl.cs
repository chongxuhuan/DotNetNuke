using System;
using System.Web;
using DotNetNuke.HttpModules.Services.Internal;

namespace DotNetNuke.Web.Services
{
    internal class DigestAuthenticatorImpl : AuthenticatorBase
    {
        public override void TryToAuthenticate(HttpContextBase context, int portalId)
        {
            var authHeader = context.Request.Headers["Authorization"];
            if(String.IsNullOrEmpty(authHeader))
            {
                return;
            }

            var digestAuthentication =
                new DigestAuthentication(
                    new DigestAuthenticationRequest(authHeader, context.Request.HttpMethod),
                    portalId);
            if (digestAuthentication.IsValid)
            {
                context.User = digestAuthentication.User;
            }
            else if (digestAuthentication.IsNonceStale)
            {
                var sac = new ServicesContextWrapper(context);
                sac.DoA401 = true;
                sac.IsStale = true;
                context.Response.End();
            }
        }
    }
}