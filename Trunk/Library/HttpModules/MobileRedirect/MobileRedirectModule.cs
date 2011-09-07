#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
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

#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.HttpModules.Config;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.EventQueue;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mobile;

#endregion

namespace DotNetNuke.HttpModules
{
    public class MobileRedirectModule : IHttpModule
    {
        private IRedirectController _redirectionController;
        public string ModuleName
        {
            get
            {
                return "MobileRedirectModule";
            }
        }

        #region IHttpModule Members

        public void Init(HttpApplication application)
        {
            _redirectionController = new RedirectionController();
            application.BeginRequest += OnBeginRequest;
        }

        public void Dispose()
        {
        }

        #endregion

        public void OnBeginRequest(object s, EventArgs e)
        {
            if (_redirectionController != null)
            {
                var portalSettings = PortalController.GetCurrentPortalSettings();
                if (portalSettings != null && portalSettings.ActiveTab != null)
                {
                    var app = (HttpApplication)s;
                    if (app != null && app.Request != null && !string.IsNullOrEmpty(app.Request.UserAgent))
                    {
                        string redirectUrl = _redirectionController.GetRedirectUrl(app.Request.UserAgent, portalSettings.PortalId, portalSettings.ActiveTab.TabID);
                        if (!string.IsNullOrEmpty(redirectUrl) && string.Compare(redirectUrl, portalSettings.ActiveTab.FullUrl, true, CultureInfo.InvariantCulture) != 0)
                        {
                            app.Response.Redirect(redirectUrl);
                        }
                    }
                }
            }
        }


    }
}