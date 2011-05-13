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
using System.IO;
using System.Threading;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.Modules.Admin.Authentication
{
    public partial class Logoff : UserModuleBase
    {
        private void Redirect()
        {
            Response.Redirect(AuthenticationController.GetLogoffRedirectURL(PortalSettings, Request), true);
        }

        private void DoLogoff()
        {
            try
            {
                if (User != null)
                {
                    DataCache.ClearUserCache(PortalSettings.PortalId, Context.User.Identity.Name);
                }
                var objPortalSecurity = new PortalSecurity();
                objPortalSecurity.SignOut();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessPageLoadException(exc);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                AuthenticationInfo authSystem = AuthenticationController.GetAuthenticationType();
                if (authSystem != null && !string.IsNullOrEmpty(authSystem.LogoffControlSrc))
                {
                    var authLogoffControl = (AuthenticationLogoffBase) LoadControl("~/" + authSystem.LogoffControlSrc);
                    authLogoffControl.AuthenticationType = authSystem.AuthenticationType;
                    authLogoffControl.ID = Path.GetFileNameWithoutExtension(authSystem.LogoffControlSrc) + "_" + authSystem.AuthenticationType;
                    authLogoffControl.LocalResourceFile = authLogoffControl.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/" +
                                                          Path.GetFileNameWithoutExtension(authSystem.LogoffControlSrc);
                    authLogoffControl.ModuleConfiguration = ModuleConfiguration;
                    authLogoffControl.LogOff += UserLogOff;
                    authLogoffControl.Redirect += UserRedirect;
                    pnlLogoffContainer.Controls.Add(authLogoffControl);
                }
                else
                {
                    DoLogoff();
                    Redirect();
                }
            }
            catch (ThreadAbortException texc)
            {
                //Do nothing Response.redirect
            }
            catch (Exception exc)
            {
                Exceptions.ProcessPageLoadException(exc);
            }
        }

        protected void UserLogOff(Object sender, EventArgs e)
        {
            DoLogoff();
        }

        protected void UserRedirect(Object sender, EventArgs e)
        {
            Redirect();
        }
    }
}