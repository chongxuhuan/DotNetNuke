using System.Web;

namespace DotNetNuke.Web.Services
{
    public class AuthenticatorBase
    {
        /// <summary>
        /// Attempts to authenticate the current request
        /// </summary>
        /// <param name="context">Context of the currect request</param>
        /// <param name="portalId">PortalID to authenticate in</param>
        public virtual void TryToAuthenticate(HttpContextBase context, int portalId) {}
    }
}