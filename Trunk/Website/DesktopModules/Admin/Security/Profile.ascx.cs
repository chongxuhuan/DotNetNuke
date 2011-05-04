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
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Modules.Admin.Users
{
    public partial class DNNProfile : ProfileUserControlBase
    {
        protected bool ShowVisibility
        {
            get
            {
                object setting = GetSetting(PortalId, "Profile_DisplayVisibility");
                return Convert.ToBoolean(setting) && IsUser;
            }
        }

        public PropertyEditorMode EditorMode
        {
            get
            {
                return ProfileProperties.EditMode;
            }
            set
            {
                ProfileProperties.EditMode = value;
            }
        }

        public bool IsValid
        {
            get
            {
                bool _IsValid = false;
                if (ProfileProperties.IsValid || IsAdmin)
                {
                    _IsValid = true;
                }
                return _IsValid;
            }
        }

        public bool ShowUpdate
        {
            get
            {
                return cmdUpdate.Visible;
            }
            set
            {
                cmdUpdate.Visible = value;
            }
        }

        public UserProfile UserProfile
        {
            get
            {
                UserProfile _Profile = null;
                if (User != null)
                {
                    _Profile = User.Profile;
                }
                return _Profile;
            }
        }

        public override void DataBind()
        {
            if (IsAdmin)
            {
                lblTitle.Text = string.Format(Localization.GetString("ProfileTitle.Text", LocalResourceFile), User.Username, User.UserID);
            }
            else
            {
                trTitle.Visible = false;
            }
            ProfilePropertyDefinitionCollection properties = UserProfile.ProfileProperties;
            foreach (ProfilePropertyDefinition profProperty in properties)
            {
                if (IsAdmin && !IsProfile)
                {
                    profProperty.Visible = true;
                }
            }
            ProfileProperties.ShowVisibility = ShowVisibility;
            ProfileProperties.DataSource = UserProfile.ProfileProperties;
            ProfileProperties.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var basePage = Page as PageBase;
            if (basePage != null)
            {
                if (basePage.PageCulture.TextInfo.IsRightToLeft)
                {
                    ProfileProperties.LabelMode = LabelMode.Right;
                }
                else
                {
                    ProfileProperties.LabelMode = LabelMode.Left;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ProfileProperties.LocalResourceFile = LocalResourceFile;
            cmdUpdate.Click += cmdUpdate_Click;
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                var properties = (ProfilePropertyDefinitionCollection) ProfileProperties.DataSource;
                User = ProfileController.UpdateUserProfile(User, properties);
                OnProfileUpdated(EventArgs.Empty);
                OnProfileUpdateCompleted(EventArgs.Empty);
            }
        }
    }
}