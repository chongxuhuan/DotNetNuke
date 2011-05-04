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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Instrumentation;
using DotNetNuke.Modules.Admin.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Messaging;
using DotNetNuke.Services.Messaging.Data;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Modules.Admin.Authentication
{
    public partial class Login : UserModuleBase
    {
        private static MessagingController _messagingController = new MessagingController();
        private readonly List<AuthenticationLoginBase> loginControls = new List<AuthenticationLoginBase>();

        protected string AuthenticationType
        {
            get
            {
                string _AuthenticationType = Null.NullString;
                if (ViewState["AuthenticationType"] != null)
                {
                    _AuthenticationType = Convert.ToString(ViewState["AuthenticationType"]);
                }
                return _AuthenticationType;
            }
            set
            {
                ViewState["AuthenticationType"] = value;
            }
        }

        protected bool AutoRegister
        {
            get
            {
                bool _AutoRegister = Null.NullBoolean;
                if (ViewState["AutoRegister"] != null)
                {
                    _AutoRegister = Convert.ToBoolean(ViewState["AutoRegister"]);
                }
                return _AutoRegister;
            }
            set
            {
                ViewState["AutoRegister"] = value;
            }
        }

        protected NameValueCollection ProfileProperties
        {
            get
            {
                var _Profile = new NameValueCollection();
                if (ViewState["ProfileProperties"] != null)
                {
                    _Profile = (NameValueCollection) ViewState["ProfileProperties"];
                }
                return _Profile;
            }
            set
            {
                ViewState["ProfileProperties"] = value;
            }
        }

        protected int PageNo
        {
            get
            {
                int _PageNo = 0;
                if (ViewState["PageNo"] != null)
                {
                    _PageNo = Convert.ToInt32(ViewState["PageNo"]);
                }
                return _PageNo;
            }
            set
            {
                ViewState["PageNo"] = value;
            }
        }

        protected string RedirectURL
        {
            get
            {
                string _RedirectURL = "";
                object setting = GetSetting(PortalId, "Redirect_AfterLogin");
                if (Convert.ToInt32(setting) == Null.NullInteger)
                {
                    if (Request.QueryString["returnurl"] != null)
                    {
                        _RedirectURL = HttpUtility.UrlDecode(Request.QueryString["returnurl"]);
                        if (_RedirectURL.Contains("://"))
                        {
                            _RedirectURL = "";
                        }
                    }
                    if (Request.Params["appctx"] != null)
                    {
                        _RedirectURL = HttpUtility.UrlDecode(Request.Params["appctx"]);
                        if (_RedirectURL.Contains("://"))
                        {
                            _RedirectURL = "";
                        }
                    }
                    if (String.IsNullOrEmpty(_RedirectURL))
                    {
                        if (PortalSettings.LoginTabId != -1 && PortalSettings.HomeTabId != -1)
                        {
                            _RedirectURL = Globals.NavigateURL(PortalSettings.HomeTabId);
                        }
                        else
                        {
                            _RedirectURL = Globals.NavigateURL();
                        }
                    }
                }
                else
                {
                    _RedirectURL = Globals.NavigateURL(Convert.ToInt32(setting));
                }
                if (UserId != -1 && User != null)
                {
                    if (!String.IsNullOrEmpty(User.Profile.PreferredLocale) && User.Profile.PreferredLocale != CultureInfo.CurrentCulture.Name)
                    {
                        _RedirectURL = UrlUtils.ReplaceQSParam(_RedirectURL, "language", User.Profile.PreferredLocale);
                    }
                }
                string qsDelimiter = "?";
                if (_RedirectURL.Contains("?"))
                {
                    qsDelimiter = "&";
                }
                if (LoginStatus == UserLoginStatus.LOGIN_INSECUREADMINPASSWORD)
                {
                    _RedirectURL = _RedirectURL + qsDelimiter + "runningDefault=1";
                }
                else if (LoginStatus == UserLoginStatus.LOGIN_INSECUREHOSTPASSWORD)
                {
                    _RedirectURL = _RedirectURL + qsDelimiter + "runningDefault=2";
                }
                return _RedirectURL;
            }
        }

        protected bool UseCaptcha
        {
            get
            {
                object setting = GetSetting(PortalId, "Security_CaptchaLogin");
                return Convert.ToBoolean(setting);
            }
        }

        protected UserLoginStatus LoginStatus
        {
            get
            {
                UserLoginStatus _LoginStatus = UserLoginStatus.LOGIN_FAILURE;
                if (ViewState["LoginStatus"] != null)
                {
                    _LoginStatus = (UserLoginStatus) ViewState["LoginStatus"];
                }
                return _LoginStatus;
            }
            set
            {
                ViewState["LoginStatus"] = value;
            }
        }

        protected string UserToken
        {
            get
            {
                string _UserToken = "";
                if (ViewState["UserToken"] != null)
                {
                    _UserToken = Convert.ToString(ViewState["UserToken"]);
                }
                return _UserToken;
            }
            set
            {
                ViewState["UserToken"] = value;
            }
        }

        private void DisplayLoginControl(AuthenticationLoginBase authLoginControl, bool addHeader, bool addFooter)
        {
            var container = new HtmlGenericControl();
            container.TagName = "div";
            container.ID = authLoginControl.AuthenticationType;
            container.Controls.Add(authLoginControl);
            SectionHeadControl sectionHeadControl = null;
            if (addHeader)
            {
                sectionHeadControl = (SectionHeadControl) LoadControl("~/controls/SectionHeadControl.ascx");
                sectionHeadControl.IncludeRule = true;
                sectionHeadControl.CssClass = "Head";
                sectionHeadControl.Text = Localization.GetString("Title", authLoginControl.LocalResourceFile);
                sectionHeadControl.Section = container.ID;
                pnlLoginContainer.Controls.Add(sectionHeadControl);
            }
            pnlLoginContainer.Controls.Add(container);
            if (addFooter)
            {
                pnlLoginContainer.Controls.Add(new LiteralControl("<br/>"));
            }
            pnlLoginContainer.Visible = true;
        }

        private void DisplayTabbedLoginControl(AuthenticationLoginBase authLoginControl, TabStripTabCollection Tabs)
        {
            var tab = new DNNTab(Localization.GetString("Title", authLoginControl.LocalResourceFile));
            tab.ID = authLoginControl.AuthenticationType;
            tab.Controls.Add(authLoginControl);
            Tabs.Add(tab);
            tsLogin.Visible = true;
        }

        private void BindLoginControl(AuthenticationLoginBase authLoginControl, AuthenticationInfo authSystem)
        {
            authLoginControl.AuthenticationType = authSystem.AuthenticationType;
            authLoginControl.ID = Path.GetFileNameWithoutExtension(authSystem.LoginControlSrc) + "_" + authSystem.AuthenticationType;
            authLoginControl.LocalResourceFile = authLoginControl.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/" +
                                                 Path.GetFileNameWithoutExtension(authSystem.LoginControlSrc);
            authLoginControl.RedirectURL = RedirectURL;
            authLoginControl.ModuleConfiguration = ModuleConfiguration;
            AddLoginControlAttributes(authLoginControl);
            authLoginControl.UserAuthenticated += UserAuthenticated;
        }

        private void BindLogin()
        {
            if (PortalSettings.UserRegistration == (int) Globals.PortalRegistrationType.NoRegistration)
            {
                tdRegister.Visible = false;
            }
            lblLogin.Text = Localization.GetSystemMessage(PortalSettings, "MESSAGE_LOGIN_INSTRUCTIONS");
            List<AuthenticationInfo> authSystems = AuthenticationController.GetEnabledAuthenticationServices();
            AuthenticationLoginBase defaultLoginControl = null;
            foreach (AuthenticationInfo authSystem in authSystems)
            {
                try
                {
                    var authLoginControl = (AuthenticationLoginBase) LoadControl("~/" + authSystem.LoginControlSrc);
                    BindLoginControl(authLoginControl, authSystem);
                    if (authSystem.AuthenticationType == "DNN")
                    {
                        defaultLoginControl = authLoginControl;
                    }
                    if (authLoginControl.Enabled)
                    {
                        loginControls.Add(authLoginControl);
                    }
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(ex);
                }
            }
            int authCount = loginControls.Count;
            switch (authCount)
            {
                case 0:
                    if (defaultLoginControl == null)
                    {
                        AuthenticationInfo authSystem = AuthenticationController.GetAuthenticationServiceByType("DNN");
                        var authLoginControl = (AuthenticationLoginBase) LoadControl("~/" + authSystem.LoginControlSrc);
                        BindLoginControl(authLoginControl, authSystem);
                        DisplayLoginControl(authLoginControl, false, false);
                    }
                    else
                    {
                        DisplayLoginControl(defaultLoginControl, false, false);
                    }
                    break;
                case 1:
                    DisplayLoginControl(loginControls[0], false, false);
                    break;
                default:
                    foreach (AuthenticationLoginBase authLoginControl in loginControls)
                    {
                        DisplayTabbedLoginControl(authLoginControl, tsLogin.Tabs);
                    }

                    break;
            }
        }

        private void AddLoginControlAttributes(AuthenticationLoginBase loginControl)
        {
            var username = loginControl.FindControl("txtUsername") as WebControl;
            if (username != null)
            {
                username.Attributes.Add("AUTOCOMPLETE", "off");
            }
            var password = loginControl.FindControl("txtPassword") as WebControl;
            if (password != null)
            {
                password.Attributes.Add("AUTOCOMPLETE", "off");
            }
            var rememberme = (CheckBox) FindControl("chkCookie");
            rememberme.Visible = Host.RememberCheckbox;
        }

        private void BindRegister()
        {
            lblType.Text = AuthenticationType;
            lblToken.Text = UserToken;
            if (PortalSettings.UserRegistration == (int) Globals.PortalRegistrationType.NoRegistration && Request.IsAuthenticated == false)
            {
                Response.Redirect(Globals.NavigateURL("Access Denied"), true);
            }
            lblRegisterHelp.Text = Localization.GetSystemMessage(PortalSettings, "MESSAGE_REGISTRATION_INSTRUCTIONS");
            switch (PortalSettings.UserRegistration)
            {
                case (int) Globals.PortalRegistrationType.PrivateRegistration:
                    lblRegisterHelp.Text += Localization.GetString("PrivateMembership", Localization.SharedResourceFile);
                    break;
                case (int) Globals.PortalRegistrationType.PublicRegistration:
                    lblRegisterHelp.Text += Localization.GetString("PublicMembership", Localization.SharedResourceFile);
                    break;
                case (int) Globals.PortalRegistrationType.VerifiedRegistration:
                    lblRegisterHelp.Text += Localization.GetString("VerifiedMembership", Localization.SharedResourceFile);
                    break;
            }
            if (AutoRegister)
            {
                InitialiseUser();
            }
            bool UserValid = true;
            if (string.IsNullOrEmpty(User.Username) || string.IsNullOrEmpty(User.Email) || string.IsNullOrEmpty(User.FirstName) || string.IsNullOrEmpty(User.LastName))
            {
                UserValid = Null.NullBoolean;
            }
            if (AutoRegister && UserValid)
            {
                ctlUser.Visible = false;
                lblRegisterTitle.Text = Localization.GetString("CreateTitle", LocalResourceFile);
                cmdCreateUser.Text = Localization.GetString("cmdCreate", LocalResourceFile);
            }
            else
            {
                lblRegisterHelp.Text += Localization.GetString("Required", Localization.SharedResourceFile);
                lblRegisterTitle.Text = Localization.GetString("RegisterTitle", LocalResourceFile);
                cmdCreateUser.Text = Localization.GetString("cmdRegister", LocalResourceFile);
                ctlUser.ShowPassword = false;
                ctlUser.ShowUpdate = false;
                ctlUser.User = User;
                ctlUser.DataBind();
            }
        }

        private void InitialiseUser()
        {
            User.Username = UserToken.Replace("http://", "").TrimEnd('/');
            UpdateProfile(User, false);
            if (string.IsNullOrEmpty(User.DisplayName))
            {
                User.DisplayName = UserToken.Replace("http://", "").TrimEnd('/');
            }
            if (User.DisplayName.IndexOf(' ') > 0)
            {
                User.FirstName = User.DisplayName.Substring(0, User.DisplayName.IndexOf(' '));
                User.LastName = User.DisplayName.Substring(User.DisplayName.IndexOf(' ') + 1);
            }
            if (string.IsNullOrEmpty(User.FirstName))
            {
                User.FirstName = AuthenticationType;
            }
            if (string.IsNullOrEmpty(User.LastName))
            {
                User.LastName = "User";
            }
        }

        private void ShowPanel()
        {
            bool showLogin = (PageNo == 0);
            bool showRegister = (PageNo == 1);
            bool showPassword = (PageNo == 2);
            bool showProfile = (PageNo == 3);
            pnlProfile.Visible = showProfile;
            pnlPassword.Visible = showPassword;
            pnlLogin.Visible = showLogin;
            pnlRegister.Visible = showRegister;
            pnlAssociate.Visible = showRegister;
            switch (PageNo)
            {
                case 0:
                    BindLogin();
                    break;
                case 1:
                    BindRegister();
                    break;
                case 2:
                    ctlPassword.UserId = UserId;
                    ctlPassword.DataBind();
                    break;
                case 3:
                    ctlProfile.UserId = UserId;
                    ctlProfile.DataBind();
                    break;
            }
        }

        private void UpdateProfile(UserInfo objUser, bool update)
        {
            bool bUpdateUser = false;
            if (ProfileProperties.Count > 0)
            {
                foreach (string key in ProfileProperties)
                {
                    switch (key)
                    {
                        case "FirstName":
                            if (objUser.FirstName != ProfileProperties[key])
                            {
                                objUser.FirstName = ProfileProperties[key];
                                bUpdateUser = true;
                            }
                            break;
                        case "LastName":
                            if (objUser.LastName != ProfileProperties[key])
                            {
                                objUser.LastName = ProfileProperties[key];
                                bUpdateUser = true;
                            }
                            break;
                        case "Email":
                            if (objUser.Email != ProfileProperties[key])
                            {
                                objUser.Email = ProfileProperties[key];
                                bUpdateUser = true;
                            }
                            break;
                        case "DisplayName":
                            if (objUser.DisplayName != ProfileProperties[key])
                            {
                                objUser.DisplayName = ProfileProperties[key];
                                bUpdateUser = true;
                            }
                            break;
                        default:
                            objUser.Profile.SetProfileProperty(key, ProfileProperties[key]);
                            break;
                    }
                }
                if (update)
                {
                    if (bUpdateUser)
                    {
                        UserController.UpdateUser(PortalId, objUser);
                    }
                    ProfileController.UpdateUserProfile(objUser);
                }
            }
        }

        private void ValidateUser(UserInfo objUser, bool ignoreExpiring)
        {
            UserValidStatus validStatus = UserValidStatus.VALID;
            string strMessage = Null.NullString;
            DateTime expiryDate = Null.NullDate;
            if (!objUser.IsSuperUser)
            {
                validStatus = UserController.ValidateUser(objUser, PortalId, ignoreExpiring);
            }
            if (PasswordConfig.PasswordExpiry > 0)
            {
                expiryDate = objUser.Membership.LastPasswordChangeDate.AddDays(PasswordConfig.PasswordExpiry);
            }
            UserId = objUser.UserID;
            switch (validStatus)
            {
                case UserValidStatus.VALID:
                    if ((objUser.Profile != null) && (objUser.Profile.PreferredLocale != null))
                    {
                        Localization.SetLanguage(objUser.Profile.PreferredLocale);
                    }
                    else
                    {
                        Localization.SetLanguage(PortalSettings.DefaultLanguage);
                    }
                    AuthenticationController.SetAuthenticationType(AuthenticationType);
                    UserController.UserLogin(PortalId, objUser, PortalSettings.PortalName, AuthenticationLoginBase.GetIPAddress(), chkCookie.Checked);
                    Response.Redirect(RedirectURL, true);
                    break;
                case UserValidStatus.PASSWORDEXPIRED:
                    strMessage = string.Format(Localization.GetString("PasswordExpired", LocalResourceFile), expiryDate.ToLongDateString());
                    AddLocalizedModuleMessage(strMessage, ModuleMessage.ModuleMessageType.YellowWarning, true);
                    PageNo = 2;
                    pnlProceed.Visible = false;
                    break;
                case UserValidStatus.PASSWORDEXPIRING:
                    strMessage = string.Format(Localization.GetString("PasswordExpiring", LocalResourceFile), expiryDate.ToLongDateString());
                    AddLocalizedModuleMessage(strMessage, ModuleMessage.ModuleMessageType.YellowWarning, true);
                    PageNo = 2;
                    pnlProceed.Visible = true;
                    break;
                case UserValidStatus.UPDATEPASSWORD:
                    AddModuleMessage("PasswordUpdate", ModuleMessage.ModuleMessageType.YellowWarning, true);
                    PageNo = 2;
                    pnlProceed.Visible = false;
                    break;
                case UserValidStatus.UPDATEPROFILE:
                    AddModuleMessage("ProfileUpdate", ModuleMessage.ModuleMessageType.YellowWarning, true);
                    PageNo = 3;
                    break;
            }
            ShowPanel();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ctlPassword.PasswordUpdated += PasswordUpdated;
            ctlProfile.ProfileUpdated += ProfileUpdated;
            ctlUser.UserCreateCompleted += UserCreateCompleted;

            ctlUser.ID = "User";
            ctlPassword.ID = "Password";
            ctlProfile.ID = "Profile";
            if (Request.QueryString["ctl"] != null)
            {
                if (Request.QueryString["ctl"].ToLower() == "login")
                {
                    CDefault myPage;
                    myPage = (CDefault) Page;
                    if (myPage.PortalSettings.LoginTabId == TabId || myPage.PortalSettings.LoginTabId == -1)
                    {
                        myPage.Title = Localization.GetString("ControlTitle_login", LocalResourceFile);
                    }
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdAssociate.Click += cmdAssociate_Click;
            cmdCreateUser.Click += cmdCreateUser_Click;
            cmdProceed.Click += cmdProceed_Click;

            if (!Null.IsNull(PortalSettings.LoginTabId) && Globals.IsAdminControl())
            {
                if (Globals.ValidateLoginTabID(PortalSettings.LoginTabId))
                {
                    var parameters = new string[3];
                    if (!string.IsNullOrEmpty(Request.QueryString["returnUrl"]))
                    {
                        parameters[0] = "returnUrl=" + Request.QueryString["returnUrl"];
                    }
                    if (!string.IsNullOrEmpty(Request.QueryString["username"]))
                    {
                        parameters[1] = "username=" + Request.QueryString["username"];
                    }
                    if (!string.IsNullOrEmpty(Request.QueryString["verificationcode"]))
                    {
                        parameters[2] = "verificationcode=" + Request.QueryString["verificationcode"];
                    }
                    Response.Redirect(Globals.NavigateURL(PortalSettings.LoginTabId, "", parameters));
                }
            }
            if (Page.IsPostBack == false)
            {
                try
                {
                    PageNo = 0;
                }
				catch (Exception ex)
				{
					DnnLog.Error(ex);
				}
            }
            if (!Request.IsAuthenticated)
            {
                ShowPanel();
            }
            else
            {
                if (Globals.IsAdminControl())
                {
                    Response.Redirect(Globals.NavigateURL(), true);
                }
                else
                {
                    if (TabPermissionController.CanAdminPage())
                    {
                        ShowPanel();
                    }
                    else
                    {
                        ContainerControl.Visible = false;
                    }
                }
            }
            trCaptcha.Visible = UseCaptcha;
            if (UseCaptcha)
            {
                ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", Localization.SharedResourceFile);
                ctlCaptcha.Text = Localization.GetString("CaptchaText", Localization.SharedResourceFile);
            }

            string returnUrl = Globals.NavigateURL();
            var popUpSkinSrc = UrlUtils.GetPopupSkinSrc(this, PortalSettings);
            string delimiter;
            string url;
            string popUpUrl;
            if (PortalSettings.UserRegistration != (int)Globals.PortalRegistrationType.NoRegistration)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["returnurl"]))
                {
                    returnUrl = Request.QueryString["returnurl"];
                }
                returnUrl = HttpUtility.UrlEncode(returnUrl);
                url = Globals.RegisterURL(returnUrl, Null.NullString);
                delimiter = url.Contains("?") ? "&" : "?";

                if (PortalSettings.EnablePopUps && HttpContext.Current.Request.Url.ToString().Contains("popUp"))
                {
                    popUpUrl = String.Format("{0}{1}popUp=true&" + popUpSkinSrc, url, delimiter);
                    registerLink.NavigateUrl = popUpUrl;
                }
                else
                {
                    registerLink.NavigateUrl = url;
                }
            }
            else
            {
                registerLink.Visible = false;
            }

            url = Globals.NavigateURL("SendPassword", "returnurl=" + returnUrl);
            delimiter = url.Contains("?") ? "&" : "?";

            if (PortalSettings.EnablePopUps && HttpContext.Current.Request.Url.ToString().Contains("popUp"))
            {
                popUpUrl = String.Format("{0}{1}popUp=true&" + popUpSkinSrc, url, delimiter);
                passwordLink.NavigateUrl = popUpUrl;
            }
            else
            {
                passwordLink.NavigateUrl = url;
            }
        }

        protected void cmdAssociate_Click(object sender, EventArgs e)
        {
            if ((UseCaptcha && ctlCaptcha.IsValid) || (!UseCaptcha))
            {
                UserLoginStatus loginStatus = UserLoginStatus.LOGIN_FAILURE;
                UserInfo objUser = UserController.ValidateUser(PortalId,
                                                               txtUsername.Text,
                                                               txtPassword.Text,
                                                               "DNN",
                                                               "",
                                                               PortalSettings.PortalName,
                                                               AuthenticationLoginBase.GetIPAddress(),
                                                               ref loginStatus);
                if (loginStatus == UserLoginStatus.LOGIN_SUCCESS)
                {
                    AuthenticationController.AddUserAuthentication(objUser.UserID, AuthenticationType, UserToken);
                    if (objUser != null)
                    {
                        UpdateProfile(objUser, true);
                    }
                    ValidateUser(objUser, true);
                }
                else
                {
                    AddModuleMessage("AssociationFailed", ModuleMessage.ModuleMessageType.RedError, true);
                }
            }
        }

        protected void cmdCreateUser_Click(object sender, EventArgs e)
        {
            User.Membership.Password = UserController.GeneratePassword();
            if (AutoRegister)
            {
                ctlUser.User = User;
                ctlUser.CreateUser();
            }
            else
            {
                if (ctlUser.IsValid)
                {
                    ctlUser.CreateUser();
                }
            }
        }

        private void cmdProceed_Click(object sender, EventArgs e)
        {
            UserInfo _User = ctlPassword.User;
            ValidateUser(_User, true);
        }

        private void PasswordUpdated(object sender, Password.PasswordUpdatedEventArgs e)
        {
            PasswordUpdateStatus status = e.UpdateStatus;
            if (status == PasswordUpdateStatus.Success)
            {
                AddModuleMessage("PasswordChanged", ModuleMessage.ModuleMessageType.GreenSuccess, true);
                UserInfo _User = ctlPassword.User;
                _User.Membership.LastPasswordChangeDate = DateTime.Now;
                _User.Membership.UpdatePassword = false;
                if (_User.IsSuperUser)
                {
                    LoginStatus = UserLoginStatus.LOGIN_SUPERUSER;
                }
                else
                {
                    LoginStatus = UserLoginStatus.LOGIN_SUCCESS;
                }
                UserLoginStatus userstatus = UserLoginStatus.LOGIN_FAILURE;
                UserController.CheckInsecurePassword(_User.Username, _User.Membership.Password, ref userstatus);
                LoginStatus = userstatus;
                ValidateUser(_User, true);
            }
            else
            {
                AddModuleMessage(status.ToString(), ModuleMessage.ModuleMessageType.RedError, true);
            }
        }

        private void ProfileUpdated(object sender, EventArgs e)
        {
            ValidateUser(ctlProfile.User, true);
        }

        private void UserAuthenticated(object sender, UserAuthenticatedEventArgs e)
        {
            LoginStatus = e.LoginStatus;
            switch (LoginStatus)
            {
                case UserLoginStatus.LOGIN_USERNOTAPPROVED:
                    switch (e.Message)
                    {
                        case "EnterCode":
                            AddModuleMessage(e.Message, ModuleMessage.ModuleMessageType.YellowWarning, true);
                            break;
                        case "InvalidCode":
                        case "UserNotAuthorized":
                            AddModuleMessage(e.Message, ModuleMessage.ModuleMessageType.RedError, true);
                            break;
                        default:
                            AddLocalizedModuleMessage(e.Message, ModuleMessage.ModuleMessageType.RedError, true);
                            break;
                    }
                    break;
                case UserLoginStatus.LOGIN_USERLOCKEDOUT:
                    AddLocalizedModuleMessage(string.Format(Localization.GetString("UserLockedOut", LocalResourceFile), Host.AutoAccountUnlockDuration), ModuleMessage.ModuleMessageType.RedError, true);
                    var Custom = new ArrayList();
                    Custom.Add(e.UserToken);
                    var _message = new Message();
                    _message.FromUserID = PortalSettings.AdministratorId;
                    _message.ToUserID = PortalSettings.AdministratorId;
                    _message.Subject = Localization.GetSystemMessage(PortalSettings, "EMAIL_USER_LOCKOUT_SUBJECT", Localization.GlobalResourceFile, Custom);
                    _message.Body = Localization.GetSystemMessage(PortalSettings, "EMAIL_USER_LOCKOUT_BODY", Localization.GlobalResourceFile, Custom);
                    _message.Status = MessageStatusType.Unread;
                    //_messagingController.SaveMessage(_message);
                    Mail.SendEmail(PortalSettings.Email, PortalSettings.Email, _message.Subject, _message.Body);
                    break;
                case UserLoginStatus.LOGIN_FAILURE:
                    if (e.Authenticated)
                    {
                        PageNo = 1;
                        AuthenticationType = e.AuthenticationType;
                        AutoRegister = e.AutoRegister;
                        ProfileProperties = e.Profile;
                        UserToken = e.UserToken;
                        ShowPanel();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(e.Message))
                        {
                            AddModuleMessage("LoginFailed", ModuleMessage.ModuleMessageType.RedError, true);
                        }
                        else
                        {
                            AddLocalizedModuleMessage(e.Message, ModuleMessage.ModuleMessageType.RedError, true);
                        }
                    }
                    break;
                default:
                    if (e.User != null)
                    {
                        AuthenticationType = e.AuthenticationType;
                        ProfileProperties = e.Profile;
                        UpdateProfile(e.User, true);
                        ValidateUser(e.User, false);
                    }
                    break;
            }
        }

        private void UserCreateCompleted(object sender, UserUserControlBase.UserCreatedEventArgs e)
        {
            string strMessage = "";
            try
            {
                if (e.CreateStatus == UserCreateStatus.Success)
                {
                    AuthenticationController.AddUserAuthentication(e.NewUser.UserID, AuthenticationType, UserToken);
                    strMessage = CompleteUserCreation(e.CreateStatus, e.NewUser, e.Notify, true);
                    if ((string.IsNullOrEmpty(strMessage)))
                    {
                        UpdateProfile(e.NewUser, true);
                        ValidateUser(e.NewUser, true);
                    }
                }
                else
                {
                    AddLocalizedModuleMessage(UserController.GetUserCreateStatus(e.CreateStatus), ModuleMessage.ModuleMessageType.RedError, true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}