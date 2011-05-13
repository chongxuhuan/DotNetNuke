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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

using DataCache = DotNetNuke.Common.Utilities.DataCache;
using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Users
{
    public partial class User : UserUserControlBase
    {
        public bool IsValid
        {
            get
            {
                return Validate();
            }
        }

        public bool ShowPassword
        {
            get
            {
                return Password.Visible;
            }
            set
            {
                Password.Visible = value;
            }
        }

        public bool ShowUpdate
        {
            get
            {
                return actionsRow.Visible;
            }
            set
            {
                actionsRow.Visible = value;
            }
        }

        private void UpdateDisplayName()
        {
            object setting = GetSetting(UserPortalID, "Security_DisplayNameFormat");
            if ((setting != null) && (!string.IsNullOrEmpty(Convert.ToString(setting))))
            {
                User.UpdateDisplayName(Convert.ToString(setting));
            }
        }

        private bool Validate()
        {
            bool _IsValid = userForm.IsValid;

            if (AddUser && ShowPassword)
            {
                UserCreateStatus createStatus = UserCreateStatus.AddUser;
                if (!chkRandom.Checked)
                {
                    if (txtPassword.Text != txtConfirm.Text)
                    {
                        createStatus = UserCreateStatus.PasswordMismatch;
                    }
                    if (createStatus == UserCreateStatus.AddUser && !UserController.ValidatePassword(txtPassword.Text))
                    {
                        createStatus = UserCreateStatus.InvalidPassword;
                    }
                    if (createStatus == UserCreateStatus.AddUser)
                    {
                        User.Membership.Password = txtPassword.Text;
                    }
                }
                else
                {
                    User.Membership.Password = UserController.GeneratePassword();
                }
                if (createStatus == UserCreateStatus.AddUser && MembershipProviderConfig.RequiresQuestionAndAnswer)
                {
                    if (string.IsNullOrEmpty(txtQuestion.Text))
                    {
                        createStatus = UserCreateStatus.InvalidQuestion;
                    }
                    else
                    {
                        User.Membership.PasswordQuestion = txtQuestion.Text;
                    }
                    if (createStatus == UserCreateStatus.AddUser)
                    {
                        if (string.IsNullOrEmpty(txtAnswer.Text))
                        {
                            createStatus = UserCreateStatus.InvalidAnswer;
                        }
                        else
                        {
                            User.Membership.PasswordAnswer = txtAnswer.Text;
                        }
                    }
                }
                if (createStatus != UserCreateStatus.AddUser)
                {
                    _IsValid = false;
                    valPassword.ErrorMessage = UserController.GetUserCreateStatus(createStatus);
                    valPassword.IsValid = false;
                }
            }
            return _IsValid;
        }

        public void CreateUser()
        {
            UpdateDisplayName();
            if (IsRegister)
            {
                User.Membership.Approved = PortalSettings.UserRegistration == (int) Globals.PortalRegistrationType.PublicRegistration;
            }
            else
            {
                User.Membership.Approved = chkAuthorize.Checked;
            }
            UserInfo user = User;
            UserCreateStatus createStatus = UserController.CreateUser(ref user);
            UserCreatedEventArgs args = (createStatus == UserCreateStatus.Success)
                                            ? new UserCreatedEventArgs(User) {Notify = chkNotify.Checked} 
                                            : new UserCreatedEventArgs(null);
            args.CreateStatus = createStatus;
            OnUserCreated(args);
            OnUserCreateCompleted(args);
        }

        public override void DataBind()
        {
            if (Page.IsPostBack == false)
            {
                string confirmString = Localization.GetString("DeleteItem");
                if (IsUser)
                {
                    confirmString = Localization.GetString("ConfirmUnRegister", LocalResourceFile);
                }
                ClientAPI.AddButtonConfirm(cmdDelete, confirmString);
                chkRandom.Checked = false;
            }

            cmdDelete.Visible = false;
            cmdRemove.Visible = false;
            cmdRestore.Visible = false;
            if (!AddUser)
            {
                var deletePermitted = (User.UserID != PortalSettings.AdministratorId) && !(IsUser && User.IsSuperUser);
                if ((deletePermitted))
                {
                    if ((User.IsDeleted))
                    {
                        cmdRemove.Visible = true;
                        cmdRestore.Visible = true;
                    }
                    else
                    {
                        cmdDelete.Visible = true;
                    }
                }
            }

            cmdUpdate.Text = Localization.GetString(IsUser ? "Register" : "CreateUser", LocalResourceFile);
            cmdDelete.Text = Localization.GetString(IsUser ? "UnRegister" : "Delete", LocalResourceFile);
            if (AddUser)
            {
                pnlAddUser.Visible = true;
                if (IsRegister)
                {
                    AuthorizeNotify.Visible = false;
                    randomRow.Visible = false;
                    if (ShowPassword)
                    {
                        questionRow.Visible = MembershipProviderConfig.RequiresQuestionAndAnswer;
                        answerRow.Visible = MembershipProviderConfig.RequiresQuestionAndAnswer;
                        lblPasswordHelp.Text = Localization.GetString("PasswordHelpUser", LocalResourceFile);
                    }
                }
                else
                {
                    lblPasswordHelp.Text = Localization.GetString("PasswordHelpAdmin", LocalResourceFile);
                }
                txtConfirm.Attributes.Add("value", txtConfirm.Text);
                txtPassword.Attributes.Add("value", txtPassword.Text);
            }

            userNameReadOnly.Visible = !AddUser;
            userName.Visible = AddUser;

            var setting = GetSetting(UserPortalID, "Security_EmailValidation");
            if ((setting != null) && (!string.IsNullOrEmpty(Convert.ToString(setting))))
            {
                email.ValidationExpression = Convert.ToString(setting);
            }

            setting = GetSetting(UserPortalID, "Security_DisplayNameFormat");
            if ((setting != null) && (!string.IsNullOrEmpty(Convert.ToString(setting))))
            {
                if (AddUser)
                {
                    displayNameReadOnly.Visible = false;
                    displayName.Visible = false;
                }
                else
                {
                    displayNameReadOnly.Visible = true;
                    displayName.Visible = false;
                }
            }
            else
            {
                displayNameReadOnly.Visible = false;
                displayName.Visible = true;
            }


            userForm.DataSource = User;
			if (!Page.IsPostBack)
			{
				userForm.DataBind();
			}
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cmdDelete.Click += cmdDelete_Click;
            cmdUpdate.Click += cmdUpdate_Click;
            cmdRemove.Click += cmdRemove_Click;
            cmdRestore.Click += cmdRestore_Click;
        }

        private void cmdDelete_Click(Object sender, EventArgs e)
        {
            string name = User.Username;
            int id = UserId;
            UserInfo user = User;
            if (UserController.DeleteUser(ref user, true, false))
            {
                OnUserDeleted(new UserDeletedEventArgs(id, name));
            }
            else
            {
                OnUserDeleteError(new UserUpdateErrorArgs(id, name, "UserDeleteError"));
            }
        }

        private void cmdRestore_Click(Object sender, EventArgs e)
        {
            var name = User.Username;
            var id = UserId;

            var userInfo = User;
            if (UserController.RestoreUser(ref userInfo))
            {
                OnUserRestored(new UserRestoredEventArgs(id, name));
            }
            else
            {
                OnUserRestoreError(new UserUpdateErrorArgs(id, name, "UserRestoreError"));
            }
        }

        private void cmdRemove_Click(Object sender, EventArgs e)
        {
            var name = User.Username;
            var id = UserId;

            if (UserController.RemoveUser(User))
            {
                OnUserRemoved(new UserRemovedEventArgs(id, name));
            }
            else
            {
                OnUserRemoveError(new UserUpdateErrorArgs(id, name, "UserRemoveError"));
            }
        }

        private void cmdUpdate_Click(Object sender, EventArgs e)
        {
            if (AddUser)
            {
                if (IsValid)
                {
                    CreateUser();
                }
            }
            else
            {
                if (userForm.IsValid && (User != null))
                {
                    if (User.UserID == PortalSettings.AdministratorId)
                    {
                        DataCache.ClearPortalCache(UserPortalID, false);
                    }
                    try
                    {
                        UpdateDisplayName();
                        UserController.UpdateUser(UserPortalID, User);
                        OnUserUpdated(EventArgs.Empty);
                        OnUserUpdateCompleted(EventArgs.Empty);
                    }
                    catch (Exception exc)
                    {
                        DnnLog.Error(exc);

                        var args = new UserUpdateErrorArgs(User.UserID, User.Username, "EmailError");
                        OnUserUpdateError(args);
                    }
                }
            }
        }

    }
}