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
using System.Threading;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.UI.WebControls;

using DataCache = DotNetNuke.UI.Utilities.DataCache;

#endregion

namespace DotNetNuke.Modules.Admin.Users
{
    public partial class UserSettings : UserModuleBase
    {
        protected string ReturnURL
        {
            get
            {
                string _ReturnURL;
                var FilterParams = new string[String.IsNullOrEmpty(Request.QueryString["filterproperty"]) ? 1 : 2];
                if (String.IsNullOrEmpty(Request.QueryString["filterProperty"]))
                {
                    FilterParams.SetValue("filter=" + Request.QueryString["filter"], 0);
                }
                else
                {
                    FilterParams.SetValue("filter=" + Request.QueryString["filter"], 0);
                    FilterParams.SetValue("filterProperty=" + Request.QueryString["filterProperty"], 1);
                }
                if (string.IsNullOrEmpty(Request.QueryString["filter"]))
                {
                    _ReturnURL = Globals.NavigateURL(TabId);
                }
                else
                {
                    _ReturnURL = Globals.NavigateURL(TabId, "", FilterParams);
                }
                return _ReturnURL;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdCancel.Click += cmdCancel_Click;
            cmdUpdate.Click += cmdUpdate_Click;

            var config = new MembershipProviderConfig();
            if (MembershipProviderConfig.CanEditProviderProperties)
            {
                ProviderSettings.EditMode = PropertyEditorMode.Edit;
            }
            else
            {
                ProviderSettings.EditMode = PropertyEditorMode.View;
            }
            ProviderSettings.LocalResourceFile = LocalResourceFile;
            ProviderSettings.DataSource = config;
            ProviderSettings.DataBind();
            if (UserInfo.IsSuperUser)
            {
                PasswordSettings.EditMode = PropertyEditorMode.Edit;
            }
            else
            {
                PasswordSettings.EditMode = PropertyEditorMode.View;
            }
            PasswordSettings.LocalResourceFile = LocalResourceFile;
            PasswordSettings.DataSource = new PasswordConfig();
            PasswordSettings.DataBind();
            var editors = new Hashtable();
            editors["Redirect_AfterLogin"] = EditorInfo.GetEditor("Page");
            editors["Redirect_AfterLogout"] = EditorInfo.GetEditor("Page");
            editors["Redirect_AfterRegistration"] = EditorInfo.GetEditor("Page");
            var visibility = new Hashtable();
            if (IsHostMenu)
            {
                visibility["Profile_DefaultVisibility"] = false;
                visibility["Profile_DisplayVisibility"] = false;
                visibility["Profile_ManageServices"] = false;
                visibility["Redirect_AfterLogin"] = false;
                visibility["Redirect_AfterRegistration"] = false;
                visibility["Redirect_AfterLogout"] = false;
                visibility["Security_CaptchaLogin"] = false;
                visibility["Security_CaptchaRegister"] = false;
                visibility["Security_EmailValidation"] = false;
                visibility["Security_RequireValidProfile"] = false;
                visibility["Security_RequireValidProfileAtLogin"] = false;
                visibility["Security_UsersControl"] = false;
            }
            UserSettingsEditor.LocalResourceFile = LocalResourceFile;
            UserSettingsEditor.DataSource = UserController.GetUserSettings(UserPortalID);
            UserSettingsEditor.CustomEditors = editors;
            UserSettingsEditor.Visibility = visibility;
            UserSettingsEditor.DataBind();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ReturnURL, true);
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            string key = Null.NullString;
            string setting = Null.NullString;
            foreach (FieldEditorControl settingsEditor in UserSettingsEditor.Fields)
            {
                if (settingsEditor.IsDirty)
                {
                    key = settingsEditor.Editor.Name;
                    setting = Convert.ToString(settingsEditor.Editor.Value);
                    if (key == "Security_DisplayNameFormat")
                    {
                        var objUserController = new UserController();
                        objUserController.PortalId = UserPortalID;
                        objUserController.DisplayFormat = setting;
                        var objThread = new Thread(objUserController.UpdateDisplayNames);
                        objThread.Start();
                    }
                    UpdateSetting(UserPortalID, key, setting);
                }
            }
            DataCache.RemoveCache(UserController.SettingsKey(UserPortalID));
            Response.Redirect(ReturnURL, true);
        }
    }
}