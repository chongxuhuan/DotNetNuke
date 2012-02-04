#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.HttpModules.Membership;

namespace DotNetNuke.Web.Services
{
    public class DnnController : Controller
    {
        private AuthenticatorBase _basicAuthenticator;
        private AuthenticatorBase _digestAuthenticator;

        public DnnController()
        {
            ActionInvoker = new DnnControllerActionInvoker();
            DefaultAuthLevel = ServiceAuthLevel.Host;
            _basicAuthenticator = new BasicAuthenticator();
            _digestAuthenticator = new DigetstAuthenticator();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            var portalSettings = LoadPortalSettings(requestContext.HttpContext);
            AuthenticateRequest(requestContext.HttpContext, portalSettings.PortalId);

            base.Initialize(requestContext);
        }

        protected virtual void AuthenticateRequest(HttpContextBase context, int portalId)
        {
            if (!context.Request.IsAuthenticated)
            {
                _basicAuthenticator.TryToAuthenticate(context, portalId);
            }

            if (!context.Request.IsAuthenticated)
            {
                _digestAuthenticator.TryToAuthenticate(context, portalId);
            }

            MembershipModule.AuthenticateRequest(context, true /*allowUnknownExtension*/);
        }

        protected virtual PortalSettings LoadPortalSettings(HttpContextBase context)
        {
            var domainName = Globals.GetDomainName(context.Request);
            var alias = PortalAliasController.GetPortalAliasInfo(domainName);

            int tabId = context.FindTabId();

            if(tabId != Null.NullInteger)
            {
                if(!TabIsInPortal(tabId, alias.PortalID))
                {
                    //todo localize error message
                    throw new HttpException(400, "Specified tab is not in this portal");
                }

                int moduleId = context.FindModuleId();

                if(moduleId != Null.NullInteger)
                {
                    var module = new ModuleController().GetModule(moduleId, tabId);
                    if (module != null)
                    {
                        ActiveModule = module;
                    }
                    else
                    {
                        //todo localize error message
                        throw new HttpException(400, "Specified tab module does not exist");
                    }
                }
            }
            
            var portalSettings = new PortalSettings(tabId, alias);
            
            context.Items["PortalSettings"] = portalSettings;
            return portalSettings;
        }

        private bool TabIsInPortal(int tabId, int portalId)
        {
            var tc = new TabController();
            var tab = tc.GetTab(tabId, portalId, /*ignoreCache*/ false);

            return tab != null;
        }

        /// <summary>
        /// PortalSettings for the current portal
        /// </summary>
        public PortalSettings PortalSettings{get { return PortalController.GetCurrentPortalSettings(); }}

        /// <summary>
        /// UserInfo for the current user
        /// </summary>
        public UserInfo UserInfo { get {return PortalSettings.UserInfo;}}

        /// <summary>
        /// ModuleInfo for the current module
        /// <remarks>Will be null unless a valid pair of module and tab ids were provided in the request</remarks>
        /// </summary>
        public ModuleInfo ActiveModule { get; private set; }

        /// <summary>
        /// Default Authorization level required to call access the methods of this controller
        /// </summary>
        public ServiceAuthLevel DefaultAuthLevel { get; set; }

        /// <summary>
        /// Injection point for BasicAuth Authenticator
        /// <remarks>Should be used for unit test purposes only</remarks> 
        /// </summary>
        public AuthenticatorBase BasicAuthenticator { set { _basicAuthenticator = value; } }

        /// <summary>
        /// Injection point for BasicAuth Authenticator
        /// <remarks>Should be used for unit test purposes only</remarks> 
        /// </summary>
        public AuthenticatorBase DigestAuthenticator { set { _digestAuthenticator = value; } }
    }
}