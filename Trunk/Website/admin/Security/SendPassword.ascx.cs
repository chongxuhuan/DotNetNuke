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
using System.Collections;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.Skins.Controls;

#endregion

namespace DotNetNuke.Modules.Admin.Security
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The SendPassword UserModuleBase is used to allow a user to retrieve their password
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	03/21/2006  Created
    /// </history>
    /// -----------------------------------------------------------------------------
    public partial class SendPassword : UserModuleBase
    {
		#region "Private Members"

        private UserInfo _User;
        private int _UserCount = Null.NullInteger;
        private string ipAddress;

		#endregion

		#region "Protected Properties"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Redirect URL (after successful sending of password)
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/11/2008  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected string RedirectURL
        {
            get
            {
                string _RedirectURL = "";

                if (Request.QueryString["returnurl"] != null)
                {
					//return to the url passed
                    _RedirectURL = HttpUtility.UrlDecode(Request.QueryString["returnurl"]);
                    //redirect url should never contain a protocol ( if it does, it is likely a cross-site request forgery attempt )
                    if (_RedirectURL.Contains("://"))
                    {
                        _RedirectURL = "";
                    }
                    if (_RedirectURL.Contains("?returnurl"))
                    {
                        string baseURL = _RedirectURL.Substring(0, _RedirectURL.IndexOf("?returnurl"));
                        string returnURL = _RedirectURL.Substring(_RedirectURL.IndexOf("?returnurl") + 11);

                        _RedirectURL = string.Concat(baseURL, "?returnurl", HttpUtility.UrlEncode(returnURL));
                    }
                }
                if (String.IsNullOrEmpty(_RedirectURL))
                {
					//redirect to current page 
                    _RedirectURL = Globals.NavigateURL();
                }
                return _RedirectURL;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the Captcha control is used to validate the login
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected bool UseCaptcha
        {
            get
            {
                object setting = GetSetting(PortalId, "Security_CaptchaLogin");
                return Convert.ToBoolean(setting);
            }
        }

		#endregion

		#region "Private Methods"
		
        private void GetUser()
        {
            ArrayList arrUsers;
            if (MembershipProviderConfig.RequiresUniqueEmail && !String.IsNullOrEmpty(txtEmail.Text.Trim()) && String.IsNullOrEmpty(txtUsername.Text.Trim()))
            {
                arrUsers = UserController.GetUsersByEmail(PortalSettings.PortalId, txtEmail.Text, 0, Int32.MaxValue, ref _UserCount);
                if (arrUsers != null && arrUsers.Count == 1)
                {
                    _User = (UserInfo) arrUsers[0];
                }
            }
            else
            {
                _User = UserController.GetUserByName(PortalSettings.PortalId, txtUsername.Text);
            }
        }

		#endregion

		#region "Event Handlers"

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            bool isEnabled = true;

            if (MembershipProviderConfig.PasswordRetrievalEnabled)
            {
                lblHelp.Text = Localization.GetString("SendPasswordHelp", LocalResourceFile);
                cmdSendPassword.Text = Localization.GetString("SendPassword", LocalResourceFile);
                cmdSendPassword.ImageUrl = "~/images/password.gif";
            }
            else if (MembershipProviderConfig.PasswordResetEnabled)
            {
                lblHelp.Text = Localization.GetString("ResetPasswordHelp", LocalResourceFile);
                cmdSendPassword.Text = Localization.GetString("ResetPassword", LocalResourceFile);
                cmdSendPassword.ImageUrl = "~/images/reset.gif";
            }
            else
            {
                isEnabled = false;
                lblHelp.Text = Localization.GetString("DisabledPasswordHelp", LocalResourceFile);
                tblSendPassword.Visible = false;
            }
            if (MembershipProviderConfig.RequiresUniqueEmail && isEnabled)
            {
                lblHelp.Text += Localization.GetString("RequiresUniqueEmail", LocalResourceFile);
            }
            if (MembershipProviderConfig.RequiresQuestionAndAnswer && isEnabled)
            {
                lblHelp.Text += Localization.GetString("RequiresQuestionAndAnswer", LocalResourceFile);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdSendPassword.Click += cmdSendPassword_Click;

            if (Request.UserHostAddress != null)
            {
                ipAddress = Request.UserHostAddress;
            }
            rowEmailLabel.Visible = MembershipProviderConfig.RequiresUniqueEmail;
            rowEmailText.Visible = MembershipProviderConfig.RequiresUniqueEmail;
            trCaptcha1.Visible = UseCaptcha;
            trCaptcha2.Visible = UseCaptcha;
            if (UseCaptcha)
            {
                ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", LocalResourceFile);
                ctlCaptcha.Text = Localization.GetString("CaptchaText", LocalResourceFile);
            }

            string returnUrl = "";
            if (returnUrl.IndexOf("?returnurl=") != -1)
            {
                returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?returnurl="));
            }
            returnUrl = HttpUtility.UrlEncode(returnUrl);

            var popUpSkinSrc = UrlUtils.GetPopupSkinSrc(this, PortalSettings);
            string url = Globals.LoginURL(returnUrl, (Request.QueryString["override"] != null));
            string delimiter = url.Contains("?") ? "&" : "?";
            string popUpUrl = String.Format("{0}{1}popUp=true&" + popUpSkinSrc, url, delimiter);
            loginLink.NavigateUrl = PortalSettings.EnablePopUps ? popUpUrl : url;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// cmdSendPassword_Click runs when the Password Reminder button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void cmdSendPassword_Click(Object sender, EventArgs e)
        {
            string strMessage = Null.NullString;
            string strLogMessage = Null.NullString;
            bool canSend = true;

            if (MembershipProviderConfig.RequiresQuestionAndAnswer && String.IsNullOrEmpty(txtAnswer.Text))
            {
                GetUser();
                if (_User != null)
                {
                    lblQuestion.Text = _User.Membership.PasswordQuestion;
                }
                tblQA.Visible = true;
                return;
            }
            if ((UseCaptcha && ctlCaptcha.IsValid) || (!UseCaptcha))
            {
                if (String.IsNullOrEmpty(txtUsername.Text.Trim()))
                {
					//No UserName provided
                    if (MembershipProviderConfig.RequiresUniqueEmail)
                    {
                        if (String.IsNullOrEmpty(txtEmail.Text.Trim()))
                        {
							//No email address either (cannot retrieve password)
                            canSend = false;
                            strMessage = Localization.GetString("EnterUsernameEmail", LocalResourceFile);
                        }
                    }
                    else
                    {
						//Cannot retrieve password
                        canSend = false;
                        strMessage = Localization.GetString("EnterUsername", LocalResourceFile);
                    }
                }
                if (canSend)
                {
                    var objSecurity = new PortalSecurity();
                    GetUser();
                    if (_User != null)
                    {
                        if (_User.IsDeleted)
                        {
                            canSend = false;
                            strMessage = Localization.GetString("UsernameError", LocalResourceFile);
                        }
                        else
                        {
                            if (MembershipProviderConfig.PasswordRetrievalEnabled)
                            {
                                try
                                {
                                    _User.Membership.Password = UserController.GetPassword(ref _User, txtAnswer.Text);
                                }
                                catch (Exception exc)
                                {
                                    DnnLog.Error(exc);

                                    canSend = false;
                                    strMessage = Localization.GetString("PasswordRetrievalError", LocalResourceFile);
                                }
                            }
                            else
                            {
                                try
                                {
                                    _User.Membership.Password = UserController.GeneratePassword();
                                    UserController.ResetPassword(_User, txtAnswer.Text);
                                }
                                catch (Exception exc)
                                {
                                    DnnLog.Error(exc);

                                    canSend = false;
                                    strMessage = Localization.GetString("PasswordResetError", LocalResourceFile);
                                }
                            }
                            if (canSend)
                            {
                                if (Mail.SendMail(_User, MessageType.PasswordReminder, PortalSettings) != string.Empty)
                                {
                                    strMessage = Localization.GetString("SendMailError", LocalResourceFile);
                                    canSend = false;
                                }
                                else
                                {
                                    strMessage = Localization.GetString("PasswordSent", LocalResourceFile);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_UserCount > 1)
                        {
                            strMessage = Localization.GetString("MultipleUsers", LocalResourceFile);
                        }
                        else if (MembershipProviderConfig.RequiresUniqueEmail && !String.IsNullOrEmpty(txtEmail.Text.Trim()) && String.IsNullOrEmpty(txtUsername.Text.Trim()))
                        {
                            strMessage = Localization.GetString("EmailError", LocalResourceFile);
                        }
                        else
                        {
                            strMessage = Localization.GetString("UsernameError", LocalResourceFile);
                        }
                        canSend = false;
                    }
                    if (canSend)
                    {
                        var objEventLog = new EventLogController();
                        var objEventLogInfo = new LogInfo();
                        objEventLogInfo.AddProperty("IP", ipAddress);
                        objEventLogInfo.LogPortalID = PortalSettings.PortalId;
                        objEventLogInfo.LogPortalName = PortalSettings.PortalName;
                        objEventLogInfo.LogUserID = UserId;
                        objEventLogInfo.LogUserName = objSecurity.InputFilter(txtUsername.Text,
                                                                              PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
                        objEventLogInfo.LogTypeKey = "PASSWORD_SENT_SUCCESS";
                        objEventLog.AddLog(objEventLogInfo);

                        UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess);
                    }
                    else
                    {
                        var objEventLog = new EventLogController();
                        var objEventLogInfo = new LogInfo();
                        objEventLogInfo.AddProperty("IP", ipAddress);
                        objEventLogInfo.LogPortalID = PortalSettings.PortalId;
                        objEventLogInfo.LogPortalName = PortalSettings.PortalName;
                        objEventLogInfo.LogUserID = UserId;
                        objEventLogInfo.LogUserName = objSecurity.InputFilter(txtUsername.Text,
                                                                              PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
                        objEventLogInfo.LogTypeKey = "PASSWORD_SENT_FAILURE";
                        objEventLogInfo.LogProperties.Add(new LogDetailInfo("Cause", strMessage));
                        objEventLog.AddLog(objEventLogInfo);

                        UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.RedError);
                    }
                    loginLink.Visible = true;
                }
                else
                {
                    UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.RedError);
                }
            }
        }
		
		#endregion
    }
}