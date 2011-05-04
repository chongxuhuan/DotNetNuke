#region Copyright

// 
// DotNetNukeŽ - http://www.dotnetnuke.com
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
using System.Web.Security;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

#endregion

namespace DotNetNuke.Modules.Admin.Users
{
    public partial class Password : UserModuleBase
    {
        #region Delegates

        public delegate void PasswordUpdatedEventHandler(object sender, PasswordUpdatedEventArgs e);

        #endregion

        public UserMembership Membership
        {
            get
            {
                UserMembership _Membership = null;
                if (User != null)
                {
                    _Membership = User.Membership;
                }
                return _Membership;
            }
        }

        public event PasswordUpdatedEventHandler PasswordUpdated;
        public event PasswordUpdatedEventHandler PasswordQuestionAnswerUpdated;

        public void OnPasswordUpdated(PasswordUpdatedEventArgs e)
        {
            if (PasswordUpdated != null)
            {
                PasswordUpdated(this, e);
            }
        }

        public void OnPasswordQuestionAnswerUpdated(PasswordUpdatedEventArgs e)
        {
            if (PasswordQuestionAnswerUpdated != null)
            {
                PasswordQuestionAnswerUpdated(this, e);
            }
        }

        public override void DataBind()
        {
            if (IsAdmin)
            {
                lblTitle.Text = string.Format(Localization.GetString("PasswordTitle.Text", LocalResourceFile), User.Username, User.UserID);
            }
            else
            {
                titleRow.Visible = false;
            }
            lblLastChanged.Text = User.Membership.LastPasswordChangeDate.ToLongDateString();
            if (User.Membership.UpdatePassword)
            {
                lblExpires.Text = Localization.GetString("ForcedExpiry", LocalResourceFile);
            }
            else
            {
                lblExpires.Text = PasswordConfig.PasswordExpiry > 0 ? User.Membership.LastPasswordChangeDate.AddDays(PasswordConfig.PasswordExpiry).ToLongDateString() : Localization.GetString("NoExpiry", LocalResourceFile);
            }
            if (((!MembershipProviderConfig.PasswordRetrievalEnabled) && IsAdmin && (!IsUser)))
            {
                pnlChange.Visible = false;
            }
            else
            {
                pnlChange.Visible = true;
                if (IsAdmin && !IsUser)
                {
                    lblChangeHelp.Text = Localization.GetString("AdminChangeHelp", LocalResourceFile);
                    oldPasswordRow.Visible = false;
                }
                else
                {
                    lblChangeHelp.Text = Localization.GetString("UserChangeHelp", LocalResourceFile);
                }
            }
            if (!MembershipProviderConfig.PasswordResetEnabled)
            {
                pnlReset.Visible = false;
            }
            else
            {
                pnlReset.Visible = true;
                if (IsAdmin && !IsUser)
                {
                    if (MembershipProviderConfig.RequiresQuestionAndAnswer)
                    {
                        pnlReset.Visible = false;
                    }
                    else
                    {
                        lblResetHelp.Text = Localization.GetString("AdminResetHelp", LocalResourceFile);
                    }
                    questionRow.Visible = false;
                    answerRow.Visible = false;
                }
                else
                {
                    if (MembershipProviderConfig.RequiresQuestionAndAnswer && IsUser)
                    {
                        lblResetHelp.Text = Localization.GetString("UserResetHelp", LocalResourceFile);
                        lblQuestion.Text = User.Membership.PasswordQuestion;
                        questionRow.Visible = true;
                        answerRow.Visible = true;
                    }
                    else
                    {
                        pnlReset.Visible = false;
                    }
                }
            }
            if (MembershipProviderConfig.RequiresQuestionAndAnswer && IsUser)
            {
                pnlQA.Visible = true;
            }
            else
            {
                pnlQA.Visible = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //ClientAPI.RegisterKeyCapture(Parent, cmdUpdate.Controls[0], 13);
            //ClientAPI.RegisterKeyCapture(this, cmdUpdate.Controls[0], 13);
            cmdReset.Click += cmdReset_Click;
            cmdUpdate.Click += cmdUpdate_Click;
            cmdUpdateQA.Click += cmdUpdateQA_Click;
        }

        private void cmdReset_Click(object sender, EventArgs e)
        {
            string answer = "";
            if (MembershipProviderConfig.RequiresQuestionAndAnswer && !IsAdmin)
            {
                if (String.IsNullOrEmpty(txtAnswer.Text))
                {
                    OnPasswordUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.InvalidPasswordAnswer));
                    return;
                }
                answer = txtAnswer.Text;
            }
            try
            {
                UserController.ResetPassword(User, answer);
                OnPasswordUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.Success));
            }
            catch (ArgumentException exc)
            {
                DnnLog.Error(exc);
                OnPasswordUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.InvalidPasswordAnswer));
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                OnPasswordUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordResetFailed));
            }
        }

        private void cmdUpdate_Click(Object sender, EventArgs e)
        {
            if (txtNewPassword.Text != txtNewConfirm.Text)
            {
                OnPasswordUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordMismatch));
                return;
            }
            if (!UserController.ValidatePassword(txtNewPassword.Text))
            {
                OnPasswordUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordInvalid));
                return;
            }
            if (!IsAdmin && String.IsNullOrEmpty(txtOldPassword.Text))
            {
                OnPasswordUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordMissing));
                return;
            }
            if (!IsAdmin && txtNewPassword.Text == txtOldPassword.Text)
            {
                OnPasswordUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordNotDifferent));
                return;
            }
            try
            {
                OnPasswordUpdated(UserController.ChangePassword(User, txtOldPassword.Text, txtNewPassword.Text)
                                      ? new PasswordUpdatedEventArgs(PasswordUpdateStatus.Success)
                                      : new PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordResetFailed));
            }
            catch (MembershipPasswordException exc)
            {
                DnnLog.Error(exc);

                OnPasswordUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.InvalidPasswordAnswer));
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);

                OnPasswordUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordResetFailed));
            }
        }

        private void cmdUpdateQA_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtQAPassword.Text))
            {
                OnPasswordQuestionAnswerUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordInvalid));
                return;
            }
            if (String.IsNullOrEmpty(txtEditQuestion.Text))
            {
                OnPasswordQuestionAnswerUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.InvalidPasswordQuestion));
                return;
            }
            if (String.IsNullOrEmpty(txtEditAnswer.Text))
            {
                OnPasswordQuestionAnswerUpdated(new PasswordUpdatedEventArgs(PasswordUpdateStatus.InvalidPasswordAnswer));
                return;
            }
            UserInfo objUser = UserController.GetUserById(PortalId, UserId);
            OnPasswordQuestionAnswerUpdated(UserController.ChangePasswordQuestionAndAnswer(objUser, txtQAPassword.Text, txtEditQuestion.Text, txtEditAnswer.Text)
                                                ? new PasswordUpdatedEventArgs(PasswordUpdateStatus.Success)
                                                : new PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordResetFailed));
        }

        #region Nested type: PasswordUpdatedEventArgs

        public class PasswordUpdatedEventArgs
        {
            public PasswordUpdatedEventArgs(PasswordUpdateStatus status)
            {
                UpdateStatus = status;
            }

            public PasswordUpdateStatus UpdateStatus { get; set; }
        }

        #endregion
    }
}