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
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Modules.Admin.Security;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Profile;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.Skins.Controls;

#endregion

namespace DotNetNuke.Modules.Admin.Users
{
    public partial class ManageUsers : UserModuleBase, IActionable
    {
        protected bool DisplayServices
        {
            get
            {
                object setting = GetSetting(PortalId, "Profile_ManageServices");
                return Convert.ToBoolean(setting) && !(IsEdit || User.IsSuperUser);
            }
        }

        protected string RedirectURL
        {
            get
            {
                string _RedirectURL = "";
                object setting = GetSetting(PortalId, "Redirect_AfterRegistration");
                if (Convert.ToInt32(setting) == Null.NullInteger)
                {
                    if (Request.QueryString["returnurl"] != null)
                    {
                        _RedirectURL = HttpUtility.UrlDecode(Request.QueryString["returnurl"]);
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
                        _RedirectURL = Globals.NavigateURL();
                    }
                }
                else
                {
                    _RedirectURL = Globals.NavigateURL(Convert.ToInt32(setting));
                }
                return _RedirectURL;
            }
        }

        protected bool RequireProfile
        {
            get
            {
                object setting = GetSetting(PortalId, "Security_RequireValidProfile");
                return Convert.ToBoolean(setting) && IsRegister;
            }
        }

        protected string ReturnUrl
        {
            get
            {
                return Globals.NavigateURL(TabId, "", !String.IsNullOrEmpty(UserFilter) ? UserFilter : "");
            }
        }

        protected bool UseCaptcha
        {
            get
            {
                object setting = GetSetting(PortalId, "Security_CaptchaRegister");
                return Convert.ToBoolean(setting) && IsRegister;
            }
        }

        protected string UserFilter
        {
            get
            {
                string filterString = !string.IsNullOrEmpty(Request["filter"]) ? "filter=" + Request["filter"] : "";
                string filterProperty = !string.IsNullOrEmpty(Request["filterproperty"]) ? "filterproperty=" + Request["filterproperty"] : "";
                string page = !string.IsNullOrEmpty(Request["currentpage"]) ? "currentpage=" + Request["currentpage"] : "";
                if (!string.IsNullOrEmpty(filterString))
                {
                    filterString += "&";
                }
                if (!string.IsNullOrEmpty(filterProperty))
                {
                    filterString += filterProperty + "&";
                }
                if (!string.IsNullOrEmpty(page))
                {
                    filterString += page;
                }
                return filterString;
            }
        }

        public int PageNo
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

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var Actions = new ModuleActionCollection();
                if (!IsProfile)
                {
                    if (!AddUser)
                    {
                        Actions.Add(GetNextActionID(),
                                    Localization.GetString(ModuleActionType.AddContent, LocalResourceFile),
                                    ModuleActionType.AddContent,
                                    "",
                                    "add.gif",
                                    EditUrl(),
                                    false,
                                    SecurityAccessLevel.Admin,
                                    true,
                                    false);
                        if (ProfileProviderConfig.CanEditProviderProperties)
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("ManageProfile.Action", LocalResourceFile),
                                        ModuleActionType.AddContent,
                                        "",
                                        "icon_profile_16px.gif",
                                        EditUrl("ManageProfile"),
                                        false,
                                        SecurityAccessLevel.Admin,
                                        true,
                                        false);
                        }
                        Actions.Add(GetNextActionID(),
                                    Localization.GetString("Cancel.Action", LocalResourceFile),
                                    ModuleActionType.AddContent,
                                    "",
                                    "lt.gif",
                                    ReturnUrl,
                                    false,
                                    SecurityAccessLevel.Admin,
                                    true,
                                    false);
                    }
                }
                return Actions;
            }
        }

        #endregion

        private void BindData()
        {
            if (User != null)
            {
                if (AddUser && IsHostMenu && !UserInfo.IsSuperUser)
                {
                    AddModuleMessage("NoUser", ModuleMessage.ModuleMessageType.YellowWarning, true);
                    DisableForm();
                    return;
                }
                if (User.PortalID != Null.NullInteger && User.PortalID != PortalId)
                {
                    AddModuleMessage("InvalidUser", ModuleMessage.ModuleMessageType.YellowWarning, true);
                    DisableForm();
                    return;
                }
                if (User.IsSuperUser && !UserInfo.IsSuperUser)
                {
                    AddModuleMessage("NoUser", ModuleMessage.ModuleMessageType.YellowWarning, true);
                    DisableForm();
                    return;
                }
                if (IsEdit)
                {
                    if (!IsAdmin || (User.IsInRole(PortalSettings.AdministratorRoleName) && !PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName)))
                    {
                        AddModuleMessage("NotAuthorized", ModuleMessage.ModuleMessageType.YellowWarning, true);
                        DisableForm();
                        return;
                    }
                }
                else
                {
                    if (!IsUser)
                    {
                        if (Request.IsAuthenticated)
                        {
                            if (!PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                            {
                                Response.Redirect(Globals.NavigateURL(PortalSettings.UserTabId, "", "UserID=" + UserInfo.UserID), true);
                            }
                        }
                        else
                        {
                            if ((User.UserID > Null.NullInteger))
                            {
                                AddModuleMessage("NotAuthorized", ModuleMessage.ModuleMessageType.YellowWarning, true);
                                DisableForm();
                                return;
                            }
                        }
                    }
                }
                if (AddUser)
                {
                    if (!Request.IsAuthenticated)
                    {
                        BindRegister();
                    }
                    else
                    {
                        cmdRegister.Text = Localization.GetString("AddUser", LocalResourceFile);
                        lblTitle.Text = Localization.GetString("AddUser", LocalResourceFile);
                    }
                }
                else
                {
                    if (!Request.IsAuthenticated)
                    {
                        titleRow.Visible = false;
                    }
                    else
                    {
                        if (IsUser && IsProfile)
                        {
                            titleRow.Visible = false;
                        }
                        else
                        {
                            lblTitle.Text = string.Format(Localization.GetString("UserTitle", LocalResourceFile), User.Username, User.UserID);
                        }
                    }
                }
                if (!Page.IsPostBack)
                {
                    if ((Request.QueryString["pageno"] != null))
                    {
                        PageNo = int.Parse(Request.QueryString["pageno"]);
                    }
                    else
                    {
                        PageNo = 0;
                    }
                }
                ShowPanel();
            }
            else
            {
                AddModuleMessage("NoUser", ModuleMessage.ModuleMessageType.YellowWarning, true);
                DisableForm();
            }
        }

        private void BindMembership()
        {
            ctlMembership.User = User;
            ctlMembership.DataBind();
            AddModuleMessage("UserLockedOut", ModuleMessage.ModuleMessageType.YellowWarning, ctlMembership.UserMembership.LockedOut && (!Page.IsPostBack));
            imgLockedOut.Visible = ctlMembership.UserMembership.LockedOut;
            imgOnline.Visible = ctlMembership.UserMembership.IsOnLine;
        }

        private void BindRegister()
        {
            if (UseCaptcha)
            {
                ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", LocalResourceFile);
                ctlCaptcha.Text = Localization.GetString("CaptchaText", LocalResourceFile);
            }
            if (PortalSettings.UserRegistration == (int) Globals.PortalRegistrationType.NoRegistration && Request.IsAuthenticated == false)
            {
                Response.Redirect(Globals.NavigateURL("Access Denied"), true);
            }
            lblTitle.Text = Localization.GetString("Register", LocalResourceFile);
            cmdRegister.Text = Localization.GetString("cmdRegister", LocalResourceFile);
            lblUserHelp.Text = Localization.GetSystemMessage(PortalSettings, "MESSAGE_REGISTRATION_INSTRUCTIONS");
            switch (PortalSettings.UserRegistration)
            {
                case (int) Globals.PortalRegistrationType.PrivateRegistration:
                    lblUserHelp.Text += Localization.GetString("PrivateMembership", Localization.SharedResourceFile);
                    break;
                case (int) Globals.PortalRegistrationType.PublicRegistration:
                    lblUserHelp.Text += Localization.GetString("PublicMembership", Localization.SharedResourceFile);
                    break;
                case (int) Globals.PortalRegistrationType.VerifiedRegistration:
                    lblUserHelp.Text += Localization.GetString("VerifiedMembership", Localization.SharedResourceFile);
                    break;
            }
            lblUserHelp.Text += Localization.GetString("Required", LocalResourceFile);
            lblUserHelp.Text += Localization.GetString("RegisterWarning", LocalResourceFile);
            helpRow.Visible = true;
            captchaRow.Visible = UseCaptcha;
        }

        private void BindUser()
        {
            if (AddUser)
            {
                ctlUser.ShowUpdate = false;
                CheckQuota();
            }
            ctlUser.User = User;
            ctlUser.DataBind();
            if (AddUser || (IsUser && !IsAdmin))
            {
				membershipRow.Visible = false;
            }
            else
            {
                BindMembership();
            }
        }

        private void CheckQuota()
        {
            if (PortalSettings.Users < PortalSettings.UserQuota || UserInfo.IsSuperUser || PortalSettings.UserQuota == 0)
            {
                cmdRegister.Enabled = true;
            }
            else
            {
                cmdRegister.Enabled = false;
                if (IsRegister)
                {
                    AddModuleMessage("ExceededRegisterQuota", ModuleMessage.ModuleMessageType.YellowWarning, true);
                }
                else
                {
                    AddModuleMessage("ExceededUserQuota", ModuleMessage.ModuleMessageType.YellowWarning, true);
                }
            }
        }

        private void DisableForm()
        {
            adminTabNav.Visible = false;

        }

        private void ShowPanel()
        {
            if (AddUser)
            {
                adminTabNav.Visible = false;
                if (Request.IsAuthenticated && MembershipProviderConfig.RequiresQuestionAndAnswer)
                {
                    //pnlUser.Visible = false;
                    actionsRow.Visible = false;
                    AddModuleMessage("CannotAddUser", ModuleMessage.ModuleMessageType.YellowWarning, true);
                }
                else
                {
                    //pnlUser.Visible = true;
                    actionsRow.Visible = true;
                }
                BindUser();
                if (RequireProfile)
                {
                    if (AddUser)
                    {
                        ctlProfile.ShowUpdate = false;
                    }
                    ctlProfile.User = User;
                    ctlProfile.DataBind();
                }
                dnnProfileDetails.Visible = RequireProfile;
            }
            else
            {
                //pnlUser.Visible = showUser;
                //pnlProfile.Visible = showProfile;
                if ((!IsAdmin && !IsUser))
                {
                    passwordTab.Visible = false;
                }
                else
                {
                    ctlPassword.User = User;
                    ctlPassword.DataBind();
                }
                if ((!IsEdit || User.IsSuperUser))
                {
                    rolesTab.Visible = false;
                }
                else
                {
                    ctlRoles.DataBind();
                }
                //cmdProfile.Enabled = !showProfile;
                if ((!DisplayServices))
                {
                    servicesTab.Visible = false;
                }
                else
                {
                    ctlServices.User = User;
                    ctlServices.DataBind();
                }

                BindUser();
                ctlProfile.User = User;
                ctlProfile.DataBind(); 
            }

            dnnRoleDetails.Visible = IsEdit && !User.IsSuperUser;
            dnnServicesDetails.Visible = DisplayServices && !IsRegister;
            dnnPasswordDetails.Visible = IsAdmin || IsUser;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            jQuery.RequestDnnPluginsRegistration();

            ctlMembership.ID = "Membership";
            ctlMembership.ModuleConfiguration = ModuleConfiguration;
            ctlMembership.UserId = UserId;
            ctlUser.ID = "User";
            ctlUser.ModuleConfiguration = ModuleConfiguration;
            ctlUser.UserId = UserId;
            ctlRoles.ID = "SecurityRoles";
            ctlRoles.ModuleConfiguration = ModuleConfiguration;
            ctlRoles.ParentModule = this;
            ctlPassword.ID = "Password";
            ctlPassword.ModuleConfiguration = ModuleConfiguration;
            ctlPassword.UserId = UserId;
            ctlProfile.ID = "Profile";
            ctlProfile.ModuleConfiguration = ModuleConfiguration;
            ctlProfile.UserId = UserId;
            ctlServices.ID = "MemberServices";
            ctlServices.ModuleConfiguration = ModuleConfiguration;
            ctlServices.UserId = UserId;
            if (AddUser)
            {
                if (!Request.IsAuthenticated)
                {
                    ModuleConfiguration.ModuleTitle = Localization.GetString("Register.Title", LocalResourceFile);
                }
                else
                {
                    ModuleConfiguration.ModuleTitle = Localization.GetString("AddUser.Title", LocalResourceFile);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdCancel.Click += cmdCancel_Click;
            cmdLogin.Click += cmdLogin_Click;
            cmdRegister.Click += cmdRegister_Click;


            ctlUser.UserCreateCompleted += UserCreateCompleted;
            ctlUser.UserDeleted += UserDeleted;
            ctlUser.UserRemoved += UserRemoved;
            ctlUser.UserRestored += UserRestored;
            ctlUser.UserUpdateCompleted += UserUpdateCompleted;
            ctlUser.UserUpdateError += UserUpdateError;

            ctlServices.SubscriptionUpdated += SubscriptionUpdated;
            ctlProfile.ProfileUpdateCompleted += ProfileUpdateCompleted;
            ctlPassword.PasswordUpdated += PasswordUpdated;
            ctlPassword.PasswordQuestionAnswerUpdated += PasswordQuestionAnswerUpdated;
            ctlMembership.MembershipAuthorized += MembershipAuthorized;
            ctlMembership.MembershipPasswordUpdateChanged += MembershipPasswordUpdateChanged;
            ctlMembership.MembershipUnAuthorized += MembershipUnAuthorized;
            ctlMembership.MembershipUnLocked += MembershipUnLocked;

            try
            {
                AddActionHandler(ModuleAction_Click);
                BindData();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(), true);
        }

        protected void cmdLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect(RedirectURL, true);
        }

        protected void cmdRegister_Click(object sender, EventArgs e)
        {
            if (((UseCaptcha && ctlCaptcha.IsValid) || (!UseCaptcha)) && ctlUser.IsValid && ((RequireProfile && ctlProfile.IsValid) || (!RequireProfile)))
            {
                ctlUser.CreateUser();
            }
        }

        private void ModuleAction_Click(object sender, ActionEventArgs e)
        {
            switch (e.Action.CommandArgument)
            {
                case "ManageRoles":
                    //pnlRoles.Visible = true;
                    //pnlUser.Visible = false;
                    break;
                case "Cancel":
                    break;
                case "Delete":
                    break;
                case "Edit":
                    break;
                case "Save":
                    break;
                default:
                    break;
            }
        }

        private void MembershipAuthorized(object sender, EventArgs e)
        {
            try
            {
                AddModuleMessage("UserAuthorized", ModuleMessage.ModuleMessageType.GreenSuccess, true);
                if (string.IsNullOrEmpty(User.Membership.Password) && !MembershipProviderConfig.RequiresQuestionAndAnswer && MembershipProviderConfig.PasswordRetrievalEnabled)
                {
                    UserInfo user = User;
                    User.Membership.Password = UserController.GetPassword(ref user, "");
                }
                Mail.SendMail(User, MessageType.UserRegistrationPublic, PortalSettings);
                BindMembership();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void MembershipPasswordUpdateChanged(object sender, EventArgs e)
        {
            try
            {
                AddModuleMessage("UserPasswordUpdateChanged", ModuleMessage.ModuleMessageType.GreenSuccess, true);
                BindMembership();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void MembershipUnAuthorized(object sender, EventArgs e)
        {
            try
            {
                AddModuleMessage("UserUnAuthorized", ModuleMessage.ModuleMessageType.GreenSuccess, true);
                BindMembership();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void MembershipUnLocked(object sender, EventArgs e)
        {
            try
            {
                AddModuleMessage("UserUnLocked", ModuleMessage.ModuleMessageType.GreenSuccess, true);
                BindMembership();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PasswordQuestionAnswerUpdated(object sender, Password.PasswordUpdatedEventArgs e)
        {
            PasswordUpdateStatus status = e.UpdateStatus;
            if (status == PasswordUpdateStatus.Success)
            {
                AddModuleMessage("PasswordQAChanged", ModuleMessage.ModuleMessageType.GreenSuccess, true);
            }
            else
            {
                AddModuleMessage(status.ToString(), ModuleMessage.ModuleMessageType.RedError, true);
            }
        }

        private void PasswordUpdated(object sender, Password.PasswordUpdatedEventArgs e)
        {
            PasswordUpdateStatus status = e.UpdateStatus;
            if (status == PasswordUpdateStatus.Success)
            {
                try
                {
                    var accessingUser = (UserInfo) HttpContext.Current.Items["UserInfo"];
                    if (accessingUser.UserID != User.UserID)
                    {
                        Mail.SendMail(User, MessageType.PasswordReminder, PortalSettings);
                    }
                    else
                    {
                        Mail.SendMail(User, MessageType.UserUpdatedOwnPassword, PortalSettings);
                    }
                    AddModuleMessage("PasswordChanged", ModuleMessage.ModuleMessageType.GreenSuccess, true);
                }
                catch (Exception ex)
                {
                    AddModuleMessage("PasswordMailError", ModuleMessage.ModuleMessageType.YellowWarning, true);
                    Exceptions.LogException(ex);
                }
            }
            else
            {
                AddModuleMessage(status.ToString(), ModuleMessage.ModuleMessageType.RedError, true);
            }
        }

        private void ProfileUpdateCompleted(object sender, EventArgs e)
        {
            if (IsUser)
            {
                Mail.SendMail(User, MessageType.ProfileUpdated, PortalSettings);
                ProfilePropertyDefinition localeProperty = User.Profile.GetProperty("PreferredLocale");
                if (localeProperty.IsDirty)
                {
                    if (User.Profile.PreferredLocale == string.Empty)
                    {
                        Localization.SetLanguage(PortalController.GetPortalDefaultLanguage(User.PortalID));
                    }
                    else
                    {
                        Localization.SetLanguage(User.Profile.PreferredLocale);
                    }
                }
            }
            Response.Redirect(Request.RawUrl, true);
        }

        private void SubscriptionUpdated(object sender, MemberServices.SubscriptionUpdatedEventArgs e)
        {
            string message = Null.NullString;
            if (e.Cancel)
            {
                message = string.Format(Localization.GetString("UserUnSubscribed", LocalResourceFile), e.RoleName);
            }
            else
            {
                message = string.Format(Localization.GetString("UserSubscribed", LocalResourceFile), e.RoleName);
            }
            PortalSecurity.ClearRoles();
            AddLocalizedModuleMessage(message, ModuleMessage.ModuleMessageType.GreenSuccess, true);
        }

        private void UserCreateCompleted(object sender, UserUserControlBase.UserCreatedEventArgs e)
        {
            string strMessage = "";
            try
            {
                if (e.CreateStatus == UserCreateStatus.Success)
                {
                    //hide the succesful captcha
                    captchaRow.Visible = false;

                    strMessage = CompleteUserCreation(e.CreateStatus, e.NewUser, e.Notify, IsRegister);
                    if (IsRegister)
                    {
                        if ((string.IsNullOrEmpty(strMessage)))
                        {
                            Response.Redirect(RedirectURL, true);
                        }
                        else
                        {
                            object setting = GetSetting(PortalId, "Redirect_AfterRegistration");
                            if (Convert.ToInt32(setting) == Null.NullInteger)
                            {
                                DisableForm();
                                cmdRegister.Visible = false;
                                cmdLogin.Visible = true;
                            }
                            else
                            {
                                Response.Redirect(RedirectURL, true);
                            }
                            DisableForm();
                            cmdRegister.Visible = false;
                            cmdLogin.Visible = true;
                        }
                    }
                    else
                    {
                        Response.Redirect(ReturnUrl, true);
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

        private void UserDeleted(object sender, UserUserControlBase.UserDeletedEventArgs e)
        {
            try
            {
                Response.Redirect(ReturnUrl, true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void UserRestored(object sender, UserUserControlBase.UserRestoredEventArgs e)
        {
            try
            {
                Response.Redirect(ReturnUrl, true);
                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void UserRemoved(object sender, UserUserControlBase.UserRemovedEventArgs e)
        {
            try
            {
                Response.Redirect(ReturnUrl, true);
                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void UserUpdateCompleted(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl, true);
        }

        private void UserUpdateError(object sender, UserUserControlBase.UserUpdateErrorArgs e)
        {
            AddModuleMessage(e.Message, ModuleMessage.ModuleMessageType.RedError, true);
        }
    }
}
