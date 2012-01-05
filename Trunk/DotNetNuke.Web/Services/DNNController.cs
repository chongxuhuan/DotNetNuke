using System.Web.Mvc;
using System.Web.Routing;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.HttpModules.Membership;

namespace DotNetNuke.Web.Services
{
    public class DnnController : Controller
    {
        public DnnController()
        {
            ActionInvoker = new DnnControllerActionInvoker();
            DefaultAuthLevel = ServiceAuthLevel.Host; 
        }

        protected override void Initialize(RequestContext requestContext)
        {
            var domainName = Globals.GetDomainName(requestContext.HttpContext.Request);
            var alias = PortalAliasController.GetPortalAliasInfo(domainName);

            //load the PortalSettings into current context
            var portalSettings = new PortalSettings(-1, alias);
            requestContext.HttpContext.Items["PortalSettings"] = portalSettings;

            MembershipModule.AuthenticateRequest(requestContext.HttpContext, true /*allowUnknownExtension*/);

            base.Initialize(requestContext);
        }

        public PortalSettings PortalSettings{get { return PortalController.GetCurrentPortalSettings(); }}

        public UserInfo UserInfo { get {return PortalSettings.UserInfo;}}

        public ServiceAuthLevel DefaultAuthLevel { get; set; }
    }
}