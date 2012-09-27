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
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Common;
using DotNetNuke.Common.Internal;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Internal;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Tabs.Internal;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Localization.Internal;

namespace DotNetNuke.Web.Api
{
    public abstract class DnnApiController : ApiController
    {
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            LoadDnnContext();
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

        protected virtual void LoadDnnContext()
        {
            var domainName = TestableGlobals.Instance.GetDomainName(ControllerContext.Request.RequestUri);
            var alias = TestablePortalAliasController.Instance.GetPortalAliasInfo(domainName);

            int tabId;
            ValidateTabAndModuleContext(alias.PortalID, out tabId);

            var portalSettings = new PortalSettings(tabId, alias);

            HttpContextSource.Current.Items["PortalSettings"] = portalSettings;
        }

        protected void ValidateTabAndModuleContext(int portalId, out int tabId)
        {
            tabId = FindTabId();
            
            if (tabId != Null.NullInteger)
            {
                if (!TabIsInPortal(tabId, portalId))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Localization.GetString("TabNotInPortal", Localization.ExceptionsResourceFile)));
                }

                int moduleId = FindModuleId();

                if (moduleId != Null.NullInteger)
                {
                    var module = TestableModuleController.Instance.GetModule(moduleId, tabId);
                    if (module != null)
                    {
                        ActiveModule = module;
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Localization.GetString("TabModuleNotExist", Localization.ExceptionsResourceFile)));
                    }
                }
            }
        }

        /// <summary>
        /// Extracts the tabid from the request
        /// </summary>
        /// <returns>TabId</returns>
        protected virtual int FindTabId()
        {
            return Request.FindTabId();
        }

        /// <summary>
        /// Extracts the moduleId from the request
        /// </summary>
        /// <returns>ModuleId</returns>
        protected virtual int FindModuleId()
        {
            return Request.FindModuleId();
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
                return TestablePortalController.Instance.GetCurrentPortalSettings();
            }
        }

        /// <summary>
        /// UserInfo for the current user
        /// </summary>
        public UserInfo UserInfo { get { return PortalSettings.UserInfo; } }

        /// <summary>
        /// ModuleInfo for the current module
        /// <remarks>Will be null unless a valid pair of module and tab ids were provided in the request</remarks>
        /// </summary>
        public ModuleInfo ActiveModule { get; private set; }
    }
}