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

#endregion

namespace DotNetNuke.Modules.Admin.Users
{
    public partial class Membership : UserModuleBase
    {
        public UserMembership UserMembership
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

        public event EventHandler MembershipAuthorized;
        public event EventHandler MembershipPasswordUpdateChanged;
        public event EventHandler MembershipUnAuthorized;
        public event EventHandler MembershipUnLocked;

        public void OnMembershipAuthorized(EventArgs e)
        {
            if (MembershipAuthorized != null)
            {
                MembershipAuthorized(this, e);
            }
        }

        public void OnMembershipPasswordUpdateChanged(EventArgs e)
        {
            if (MembershipPasswordUpdateChanged != null)
            {
                MembershipPasswordUpdateChanged(this, e);
            }
        }

        public void OnMembershipUnAuthorized(EventArgs e)
        {
            if (MembershipUnAuthorized != null)
            {
                MembershipUnAuthorized(this, e);
            }
        }

        public void OnMembershipUnLocked(EventArgs e)
        {
            if (MembershipUnLocked != null)
            {
                MembershipUnLocked(this, e);
            }
        }

        public override void DataBind()
        {
            if (UserInfo.UserID == User.UserID)
            {
                cmdAuthorize.Visible = false;
                cmdUnAuthorize.Visible = false;
                cmdUnLock.Visible = false;
                cmdPassword.Visible = false;
            }
            else
            {
                cmdUnLock.Visible = UserMembership.LockedOut;
                cmdUnAuthorize.Visible = UserMembership.Approved;
                cmdAuthorize.Visible = !UserMembership.Approved;
                cmdPassword.Visible = !UserMembership.UpdatePassword;
            }
            membershipForm.DataSource = UserMembership;
            membershipForm.DataBind();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdAuthorize.Click += cmdAuthorize_Click;
            cmdPassword.Click += cmdPassword_Click;
            cmdUnAuthorize.Click += cmdUnAuthorize_Click;
            cmdUnLock.Click += cmdUnLock_Click;
        }

        private void cmdAuthorize_Click(object sender, EventArgs e)
        {
            User.Membership = (UserMembership)membershipForm.DataSource;
            User.Membership.Approved = true;
            UserController.UpdateUser(PortalId, User);
            OnMembershipAuthorized(EventArgs.Empty);
        }

        private void cmdPassword_Click(object sender, EventArgs e)
        {
            User.Membership = (UserMembership)membershipForm.DataSource;
            User.Membership.UpdatePassword = true;
            UserController.UpdateUser(PortalId, User);
            OnMembershipPasswordUpdateChanged(EventArgs.Empty);
        }

        private void cmdUnAuthorize_Click(object sender, EventArgs e)
        {
            User.Membership = (UserMembership)membershipForm.DataSource;
            User.Membership.Approved = false;
            UserController.UpdateUser(PortalId, User);
            OnMembershipUnAuthorized(EventArgs.Empty);
        }

        private void cmdUnLock_Click(Object sender, EventArgs e)
        {
            bool isUnLocked = UserController.UnLockUser(User);
            if (isUnLocked)
            {
                User.Membership.LockedOut = false;
                OnMembershipUnLocked(EventArgs.Empty);
            }
        }
    }
}