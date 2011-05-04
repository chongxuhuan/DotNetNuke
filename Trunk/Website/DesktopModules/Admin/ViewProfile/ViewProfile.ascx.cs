#region Copyright

// 
// DotNetNuke� - http://www.dotnetnuke.com
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

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Tokens;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.Skins.Controls;

#endregion

namespace DotNetNuke.Modules.Admin.Users
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   The ViewProfile ProfileModuleUserControlBase is used to view a Users Profile
    /// </summary>
    /// <history>
    ///   [jlucarino]	02/25/2010 created
    /// </history>
    /// -----------------------------------------------------------------------------
    public partial class ViewProfile : ProfileModuleUserControlBase
    {
        public override bool DisplayModule
        {
            get
            {
                return true;
            }
        }

        #region "Event Handlers"

        private bool IsAdmin
        {
            get
            {
                return PortalSecurity.IsInRole(ModuleContext.PortalSettings.AdministratorRoleName);
            }
        }

        private bool IsUser
        {
            get
            {
                return ProfileUserId == ModuleContext.PortalSettings.UserId;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [jlucarino]	02/25/2010 created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cmdEdit.Click += cmdEdit_Click;
            try
            {
                if (ModuleContext.TabId != ModuleContext.PortalSettings.UserTabId)
                {
                    UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("ModuleNotIntended", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                }
                else
                {
                    if (ProfileUserId == Null.NullInteger)
                    {
                        //Clicked on breadcrumb - don't know which user
                        if (Request.IsAuthenticated)
                        {
                            Response.Redirect(Globals.UserProfileURL(ModuleContext.PortalSettings.UserId), true);
                        }
                        else
                        {
                            Response.Redirect(Globals.NavigateURL(ModuleContext.PortalSettings.HomeTabId), true);
                        }
                    }
                    else
                    {
                        UserInfo oUser = UserController.GetUserById(ModuleContext.PortalId, ProfileUserId);

                        if (!IsUser)
                        {
                            cmdEdit.Visible = false;
                        }

                        ProfilePropertyDefinitionCollection properties = oUser.Profile.ProfileProperties;
                        int visibleCount = 0;

                        //loop through properties to see if any are set to visible
                        foreach (ProfilePropertyDefinition profProperty in properties)
                        {
                            if (profProperty.Visible)
                            {
                                //Check Visibility
                                if (profProperty.Visibility == UserVisibilityMode.AdminOnly)
                                {
                                    //Only Visible if Admin (or self)
                                    profProperty.Visible = (IsAdmin || IsUser);
                                }
                                else if (profProperty.Visibility == UserVisibilityMode.MembersOnly)
                                {
                                    //Only Visible if Is a Member (ie Authenticated)
                                    profProperty.Visible = Request.IsAuthenticated;
                                }
                            }
                            if (profProperty.Visible)
                            {
                                visibleCount += 1;
                            }
                        }

                        if (visibleCount == 0)
                        {
                            lblNoProperties.Visible = true;
                        }
                        else
                        {
                            string Template = "";
                            var oToken = new TokenReplace();

                            oToken.User = oUser;
                            //user in profile
                            oToken.AccessingUser = ModuleContext.PortalSettings.UserInfo;
                            //user browsing the site

                            if ((ModuleContext.Settings["ProfileTemplate"] != null))
                            {
                                Template = Convert.ToString(ModuleContext.Settings["ProfileTemplate"]);
                            }
                            else
                            {
                                Template = Localization.GetString("DefaultTemplate", LocalResourceFile);
                            }

                            ProfileOutput.Text = oToken.ReplaceEnvironmentTokens(Template);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                //Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(ModuleContext.PortalSettings.ActiveTab.TabID, "Profile", "userId=" + ProfileUserId, "pageno=3"), true);
        }

        #endregion
    }
}