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
using System.IO;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Web.UI.WebControls;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Modules.Admin.Users
{
    public partial class Register : UserUserControlBase
    {
        private readonly List<AuthenticationLoginBase> _loginControls = new List<AuthenticationLoginBase>();

        #region Protected Properties

        protected UserCreateStatus CreateStatus { get; set; }

        protected string DisplayNameFormat
        {
            get
            {
                return GetSettingValue("Security_DisplayNameFormat");
            }
        }

        protected string EmailValidator
        {
            get
            {
                return GetSettingValue("Security_EmailValidation");
            }
        }

        protected bool IsValid
        {
            get
            {
                return Validate();
            }
        }

        protected bool RandomPassword
        {
            get
            {
                return Convert.ToBoolean(GetSetting(PortalId, "Security_RandomPassword"));
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
                        //return to the url passed to register
                        _RedirectURL = HttpUtility.UrlDecode(Request.QueryString["returnurl"]);
                        //redirect url should never contain a protocol ( if it does, it is likely a cross-site request forgery attempt )
                        if (_RedirectURL.Contains("://"))
                        {
                            _RedirectURL = "";
                        }
                        if (_RedirectURL.Contains("?returnurl"))
                        {
                            string baseURL = _RedirectURL.Substring(0, _RedirectURL.IndexOf("?returnurl", StringComparison.Ordinal));
                            string returnURL = _RedirectURL.Substring(_RedirectURL.IndexOf("?returnurl", StringComparison.Ordinal) + 11);

                            _RedirectURL = string.Concat(baseURL, "?returnurl", HttpUtility.UrlEncode(returnURL));
                        }
                    }
                    if (String.IsNullOrEmpty(_RedirectURL))
                    {
                        //redirect to current page 
                        _RedirectURL = Globals.NavigateURL();
                    }
                }
                else //redirect to after registration page
                {
                    _RedirectURL = Globals.NavigateURL(Convert.ToInt32(setting));
                }
                return _RedirectURL;
            }
        }

        protected bool RequirePasswordConfirm
        {
            get
            {
                return Convert.ToBoolean(GetSetting(PortalId, "Security_RequireConfirmPassword"));
            }
        }

        protected bool RequireValidProfile
        {
            get
            {
                return Convert.ToBoolean(GetSetting(PortalId, "Security_RequireValidProfile"));
            }
        }

        protected bool UseAuthProviders
        {
            get
            {
                return Convert.ToBoolean(GetSetting(PortalId, "Security_UseAuthProvidersForRegistration"));
            }
        }

        protected bool UseCaptcha
        {
            get
            {
                return Convert.ToBoolean(GetSetting(PortalId, "Security_CaptchaRegister"));
            }
        }

        protected bool UseEmailAsUserName
        {
            get
            {
                return Convert.ToBoolean(GetSetting(PortalId, "Security_UseEmailAsUserName"));
            }
        }

        protected string UserNameValidator
        {
            get
            {
                return GetSettingValue("Security_UserNameValidation");
            }
        }

        protected bool UseProfanityFilter
        {
            get
            {
                return Convert.ToBoolean(GetSetting(PortalId, "Security_UseProfanityFilter"));
            }
        }

        #endregion

        private void BindLoginControl(AuthenticationLoginBase authLoginControl, AuthenticationInfo authSystem)
        {
            //set the control ID to the resource file name ( ie. controlname.ascx = controlname )
            //this is necessary for the Localization in PageBase
            authLoginControl.AuthenticationType = authSystem.AuthenticationType;
            authLoginControl.ID = Path.GetFileNameWithoutExtension(authSystem.LoginControlSrc) + "_" + authSystem.AuthenticationType;
            authLoginControl.LocalResourceFile = authLoginControl.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/" +
                                                 Path.GetFileNameWithoutExtension(authSystem.LoginControlSrc);
            authLoginControl.RedirectURL = RedirectURL;
            authLoginControl.ModuleConfiguration = ModuleConfiguration;

            //attempt to inject control attributes
            //AddLoginControlAttributes(authLoginControl);
            //authLoginControl.UserAuthenticated += UserAuthenticated;
        }

        public UserInfo CreateUser()
        {
            //Update DisplayName to conform to Format
            UpdateDisplayName();

            User.Membership.Approved = PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.PublicRegistration;
            var user = User;
            CreateStatus = UserController.CreateUser(ref user);

            return user;
        }

        private string GetSettingValue(string key)
        {
            var value = String.Empty;
            var setting = GetSetting(UserPortalID, key);
            if ((setting != null) && (!String.IsNullOrEmpty(Convert.ToString(setting))))
            {
                value = Convert.ToString(setting);
            }
            return value;
           
        }

        private void UpdateDisplayName()
        {
            //Update DisplayName to conform to Format
            object setting = GetSetting(UserPortalID, "Security_DisplayNameFormat");
            if ((setting != null) && (!string.IsNullOrEmpty(Convert.ToString(setting))))
            {
                User.UpdateDisplayName(Convert.ToString(setting));
            }
        }

        private bool Validate()
        {
            CreateStatus = UserCreateStatus.AddUser;
            var portalSecurity = new PortalSecurity();

            //Check User Editor
            bool _IsValid = userForm.IsValid;

            //Update UserName
            if(UseEmailAsUserName)
            {
                User.Username = User.Email;
                if(String.IsNullOrEmpty(User.DisplayName))
                {
                    User.DisplayName = User.Email.Substring(0, User.Email.IndexOf("@", StringComparison.Ordinal));
                }
            }

            //Validate Profanity
            if (UseProfanityFilter)
            {
                if (!portalSecurity.ValidateInput(User.Username, PortalSecurity.FilterFlag.NoProfanity))
                {
                    CreateStatus = UserCreateStatus.InvalidUserName;
                }
                if (!String.IsNullOrEmpty(User.DisplayName))
                {
                    if (!portalSecurity.ValidateInput(User.DisplayName, PortalSecurity.FilterFlag.NoProfanity))
                    {
                        CreateStatus = UserCreateStatus.InvalidDisplayName;
                    }
                }
            }

            //Check Password is valid
            if (!RandomPassword)
            {
                //Check Password is Valid
                if (CreateStatus == UserCreateStatus.AddUser && !UserController.ValidatePassword(User.Membership.Password))
                {
                    CreateStatus = UserCreateStatus.InvalidPassword;
                }
            }
            else
            {
                //Generate a random password for the user
                User.Membership.Password = UserController.GeneratePassword();
            }

            if (RequirePasswordConfirm)
            {
                if (User.Membership.Password != User.Membership.PasswordConfirm)
                {
                    CreateStatus = UserCreateStatus.PasswordMismatch;
                }
            }

            //Check Question/Answer
            if (CreateStatus == UserCreateStatus.AddUser && MembershipProviderConfig.RequiresQuestionAndAnswer)
            {
                if (string.IsNullOrEmpty(User.Membership.PasswordQuestion))
                {
                    //Invalid Question
                    CreateStatus = UserCreateStatus.InvalidQuestion;
                }
                if (CreateStatus == UserCreateStatus.AddUser)
                {
                    if (string.IsNullOrEmpty(User.Membership.PasswordAnswer))
                    {
                        //Invalid Question
                        CreateStatus = UserCreateStatus.InvalidAnswer;
                    }
                }
            }
            if (CreateStatus != UserCreateStatus.AddUser)
            {
                _IsValid = false;
            }
            return _IsValid;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //Verify that the current user has access to this page
            if (PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.NoRegistration && Request.IsAuthenticated == false)
            {
                Response.Redirect(Globals.NavigateURL("Access Denied"), true);
            }

            cancelButton.Click += cancelButton_Click;
            registerButton.Click += registerButton_Click;

            if(UseAuthProviders)
            {
                List<AuthenticationInfo> authSystems = AuthenticationController.GetEnabledAuthenticationServices();
                foreach (AuthenticationInfo authSystem in authSystems)
                {
                    try
                    {
                        var authLoginControl = (AuthenticationLoginBase)LoadControl("~/" + authSystem.LoginControlSrc);
                        if (authSystem.AuthenticationType != "DNN")
                        {
                            BindLoginControl(authLoginControl, authSystem);
                            //Check if AuthSystem is Enabled
                            if (authLoginControl.Enabled)
                            {
                                //Add Login Control to List
                                _loginControls.Add(authLoginControl);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Exceptions.LogException(ex);
                    }
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (UseAuthProviders)
            {
                foreach (AuthenticationLoginBase authLoginControl in _loginControls)
                {
                    socialLoginControls.Controls.Add(authLoginControl);
                }
            }

            //Display relevant message
            userHelpLabel.Text = Localization.GetSystemMessage(PortalSettings, "MESSAGE_REGISTRATION_INSTRUCTIONS");
            switch (PortalSettings.UserRegistration)
            {
                case (int)Globals.PortalRegistrationType.PrivateRegistration:
                    userHelpLabel.Text += Localization.GetString("PrivateMembership", Localization.SharedResourceFile);
                    break;
                case (int)Globals.PortalRegistrationType.PublicRegistration:
                    userHelpLabel.Text += Localization.GetString("PublicMembership", Localization.SharedResourceFile);
                    break;
                case (int)Globals.PortalRegistrationType.VerifiedRegistration:
                    userHelpLabel.Text += Localization.GetString("VerifiedMembership", Localization.SharedResourceFile);
                    break;
            }
            userHelpLabel.Text += Localization.GetString("Required", LocalResourceFile);
            userHelpLabel.Text += Localization.GetString("RegisterWarning", LocalResourceFile);

            //Manage Email Address / userName
            userName.Visible = !UseEmailAsUserName;
            userName.Required = !UseEmailAsUserName;
            if (!UseEmailAsUserName && !String.IsNullOrEmpty(UserNameValidator))
            {
                userName.ValidationExpression = UserNameValidator;
            }
            if (!String.IsNullOrEmpty(EmailValidator))
            {
                email.ValidationExpression = EmailValidator;
            }

            //DisplayName
            if (!String.IsNullOrEmpty(DisplayNameFormat))
            {
                displayName.Visible = false;
            }

            //Manage Password
            password.Visible = !RandomPassword;
            password.Required = !RandomPassword;
            passwordConfirm.Visible = RequirePasswordConfirm;
            passwordConfirm.Required = RequirePasswordConfirm;

            passwordAnswer.Visible = MembershipProviderConfig.RequiresQuestionAndAnswer;
            passwordQuestion.Visible = MembershipProviderConfig.RequiresQuestionAndAnswer;

            foreach (ProfilePropertyDefinition property in User.Profile.ProfileProperties)
            {
                if (property.Required)
                {
                    DnnFormEditControlItem formItem = new DnnFormEditControlItem();
                    formItem.ResourceKey = String.Format("ProfileProperties_{0}", property.PropertyName);
                    formItem.LocalResourceFile = "~/DesktopModules/Admin/Security/App_LocalResources/Profile.ascx.resx";
                    formItem.ControlType = EditorInfo.GetEditor(property.DataType);
                    formItem.DataMember = "Profile";
                    formItem.DataField = property.PropertyName;
                    formItem.Visible = true;
                    formItem.Required = true;
                    userForm.Items.Add(formItem);
                }
            }

            userForm.DataSource = User;
            if (!Page.IsPostBack)
            {
                userForm.DataBind();
            }

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(RedirectURL, true);
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                UserInfo newUser = CreateUser();
                DataCache.ClearPortalCache(PortalId, true);

                try
                {
                    if (CreateStatus == UserCreateStatus.Success)
                    {
                        //hide the succesful captcha
                        //captchaRow.Visible = false;

                        string strMessage = CompleteUserCreation(CreateStatus, newUser, true, IsRegister);
                        if ((string.IsNullOrEmpty(strMessage)))
                        {
                            Response.Redirect(RedirectURL, true);
                        }
                    }
                    else
                    {
                        AddLocalizedModuleMessage(UserController.GetUserCreateStatus(CreateStatus), ModuleMessage.ModuleMessageType.RedError, true);
                    }
                }
                catch (Exception exc) //Module failed to load
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
            else
            {
                AddLocalizedModuleMessage(UserController.GetUserCreateStatus(CreateStatus), ModuleMessage.ModuleMessageType.RedError, true);
            }
        }
    }
}