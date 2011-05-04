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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Authentication;
using DotNetNuke.UI.Utilities;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Authentication
{
    public partial class Login : AuthenticationLoginBase
    {
        protected bool UseCaptcha
        {
            get
            {
                return AuthenticationConfig.GetConfig(PortalId).UseCaptcha;
            }
        }

        public override bool Enabled
        {
            get
            {
                return AuthenticationConfig.GetConfig(PortalId).Enabled;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdLogin.Click += cmdLogin_Click;

            ClientAPI.RegisterKeyCapture(Parent, cmdLogin, 13);
            if (!Request.IsAuthenticated)
            {
                if (Page.IsPostBack == false)
                {
                    try
                    {
                        if (Request.QueryString["username"] != null)
                        {
                            txtUsername.Text = Request.QueryString["username"];
                        }
                        if (Request.QueryString["verificationcode"] != null)
                        {
                            if (PortalSettings.UserRegistration == (int) Globals.PortalRegistrationType.VerifiedRegistration)
                            {
                                rowVerification1.Visible = true;
                                rowVerification2.Visible = true;
                                txtVerification.Text = Request.QueryString["verificationcode"];
                            }
                        }
                    }
					catch (Exception ex)
					{
						DnnLog.Error(ex);
					}
                }
                try
                {
                    if (string.IsNullOrEmpty(txtUsername.Text))
                    {
                        Globals.SetFormFocus(txtUsername);
                    }
                    else
                    {
                        Globals.SetFormFocus(txtPassword);
                    }
                }
				catch (Exception ex)
				{
					DnnLog.Error(ex);
				}
            }
            trCaptcha1.Visible = UseCaptcha;
            trCaptcha2.Visible = UseCaptcha;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            txtPassword.Attributes.Add("value", txtPassword.Text);
        }

        private void cmdLogin_Click(object sender, EventArgs e)
        {
            if ((UseCaptcha && ctlCaptcha.IsValid) || (!UseCaptcha))
            {
                UserLoginStatus loginStatus = UserLoginStatus.LOGIN_FAILURE;
                UserInfo objUser = UserController.ValidateUser(PortalId, txtUsername.Text, txtPassword.Text, "DNN", txtVerification.Text, PortalSettings.PortalName, IPAddress, ref loginStatus);
                bool authenticated = Null.NullBoolean;
                string message = Null.NullString;
                if (loginStatus == UserLoginStatus.LOGIN_USERNOTAPPROVED)
                {
                    if (PortalSettings.UserRegistration == (int) Globals.PortalRegistrationType.VerifiedRegistration)
                    {
                        if (!rowVerification1.Visible)
                        {
                            rowVerification1.Visible = true;
                            rowVerification2.Visible = true;
                            message = "EnterCode";
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(txtVerification.Text))
                            {
                                message = "InvalidCode";
                            }
                            else
                            {
                                message = "EnterCode";
                            }
                        }
                    }
                    else
                    {
                        message = "UserNotAuthorized";
                    }
                }
                else
                {
                    authenticated = (loginStatus != UserLoginStatus.LOGIN_FAILURE);
                }
                var eventArgs = new UserAuthenticatedEventArgs(objUser, txtUsername.Text, loginStatus, "DNN");
                eventArgs.Authenticated = authenticated;
                eventArgs.Message = message;
                OnUserAuthenticated(eventArgs);
            }
        }
    }
}