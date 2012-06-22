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

using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using DotNetNuke.Common.Internal;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Internal;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Tabs.Internal;
using DotNetNuke.Entities.Users;
using DotNetNuke.HttpModules.Membership;
using DotNetNuke.Services.Localization.Internal;

namespace DotNetNuke.Web.Services
{
    public class DnnController : Controller
    {
        public DnnController()
        {
            ActionInvoker = new DnnControllerActionInvoker();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            LoadDnnContext(requestContext.HttpContext);
            AuthenticateRequest(requestContext.HttpContext, PortalSettings.PortalId);
            SetupCulture();
        }

        private void SetupCulture()
        {
            if (PortalSettings == null) return;

            CultureInfo pageLocale = TestableLocalization.Instance.GetPageLocale(PortalSettings);
            if (pageLocale != null)
            {
                TestableLocalization.Instance.SetThreadCultures(pageLocale, PortalSettings);
            }
        }

        protected virtual void AuthenticateRequest(HttpContextBase context, int portalId)
        {
            if (!context.Request.IsAuthenticated)
            {
                BasicAuthenticator.Instance.TryToAuthenticate(context, portalId);
            }

            if (!context.Request.IsAuthenticated)
            {
                DigestAuthenticator.Instance.TryToAuthenticate(context, portalId);
            }

            MembershipModule.AuthenticateRequest(context, true /*allowUnknownExtension*/);
        }

        protected virtual void LoadDnnContext(HttpContextBase context)
        {
            var domainName = TestableGlobals.Instance.GetDomainName(context.Request);
            var alias = TestablePortalAliasController.Instance.GetPortalAliasInfo(domainName);

            int tabId;
            ValidateTabAndModuleContext(context, alias.PortalID, out tabId);

            var portalSettings = new PortalSettings(tabId, alias);
            
            context.Items["PortalSettings"] = portalSettings;
        }

        protected void ValidateTabAndModuleContext(HttpContextBase context, int portalId, out int tabId)
        {
            tabId = context.FindTabId();

            if (tabId != Null.NullInteger)
            {
                if (!TabIsInPortal(tabId, portalId))
                {
                    //todo localize error message
                    throw new HttpException(400, "Specified tab is not in this portal");
                }

                int moduleId = context.FindModuleId();

                if (moduleId != Null.NullInteger)
                {
                    var module = TestableModuleController.Instance.GetModule(moduleId, tabId);
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
        }

        private bool TabIsInPortal(int tabId, int portalId)
        {
            var tab = TestableTabController.Instance.GetTab(tabId, portalId);

            return tab != null;
        }

        /// <summary>
        /// PortalSettings for the current portal
        /// </summary>
        public PortalSettings PortalSettings
        {
            get
            {
                //avoid untestable static method PortalController.GetCurrentPortalSettings();
                if (ControllerContext.HttpContext != null)
                {
                    return (PortalSettings) ControllerContext.HttpContext.Items["PortalSettings"];
                }
                return null;
            }
        }

        /// <summary>
        /// UserInfo for the current user
        /// </summary>
        public UserInfo UserInfo { get {return PortalSettings.UserInfo;}}

        /// <summary>
        /// ModuleInfo for the current module
        /// <remarks>Will be null unless a valid pair of module and tab ids were provided in the request</remarks>
        /// </summary>
        public ModuleInfo ActiveModule { get; private set; }
    }
}