#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Messaging;
using DotNetNuke.Services.Social.Notifications;

#endregion

namespace DotNetNuke.UI.Skins.Controls
{
    /// -----------------------------------------------------------------------------
    /// <summary></summary>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    /// -----------------------------------------------------------------------------
    public partial class User : SkinObjectBase
    {
        private const string MyFileName = "User.ascx";

        public User()
        {
            ShowUnreadMessages = true;
        }

        public string CssClass { get; set; }

        public bool ShowUnreadMessages { get; set; }

        public string Text { get; set; }

        public string URL { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (!String.IsNullOrEmpty(CssClass))
                {
                    registerLink.CssClass = CssClass;
                }

                if (Request.IsAuthenticated == false)
                {
                    if (PortalSettings.UserRegistration != (int) Globals.PortalRegistrationType.NoRegistration)
                    {
                        if (!String.IsNullOrEmpty(Text))
                        {
                            if (Text.IndexOf("src=") != -1)
                            {
                                Text = Text.Replace("src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath);
                            }
                            registerLink.Text = Text;
                        }
                        else
                        {
                            registerLink.Text = Localization.GetString("Register", Localization.GetResourceFile(this, MyFileName));
                        }
                        if (PortalSettings.Users < PortalSettings.UserQuota || PortalSettings.UserQuota == 0)
                        {
                            registerLink.Visible = true;
                        }
                        else
                        {
                            registerLink.Visible = false;
                        }

                        registerLink.NavigateUrl = !String.IsNullOrEmpty(URL) 
                                            ? URL 
                                            : Globals.RegisterURL(HttpUtility.UrlEncode(Globals.NavigateURL()), Null.NullString);

                        //if (PortalSettings.EnablePopUps && PortalSettings.RegisterTabId == Null.NullInteger)
                        //{
                        //    registerLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(registerLink.NavigateUrl, this, PortalSettings, true, false, 600, 950));
                        //}

                    }
                    else
                    {
                        registerLink.Visible = false;
                    }
                }
                else
                {
                    var userInfo = UserController.GetCurrentUserInfo();
                    if (userInfo.UserID != -1)
                    {
                        var messagingController = new MessagingController();

                        int messageCount = messagingController.GetNewMessageCount(PortalSettings.PortalId, userInfo.UserID);

                        registerLink.Text = userInfo.DisplayName;

                        if ((ShowUnreadMessages && messageCount > 0))
                        {
                            registerLink.Text = registerLink.Text + string.Format(Localization.GetString("NewMessages", Localization.GetResourceFile(this, MyFileName)), messageCount);
                        }
                        if (ShowUnreadMessages && messageCount > 0)
                        {
                            registerLink.ToolTip = String.Format(Localization.GetString("ToolTipNewMessages", Localization.GetResourceFile(this, MyFileName)), messageCount);
                        }
                        else
                        {
                            registerLink.ToolTip = Localization.GetString("ToolTip", Localization.GetResourceFile(this, MyFileName));
                        }

                        if (userInfo.UserID != -1)
                        {
                            registerLink.NavigateUrl =Globals.UserProfileURL(userInfo.UserID);
                        }

                        var unreadMessages = Services.Social.Messaging.MessagingController.Instance.CountUnreadMessages(userInfo.UserID, userInfo.PortalID);
                        var unreadAlerts = NotificationsController.Instance.CountNotifications(userInfo.UserID, userInfo.PortalID);

                        if (ShowUnreadMessages)
                        {                         
                            messageLink.Text = string.Format("Inbox ({0})", unreadMessages);
                            notificationLink.Text = string.Format("Alerts ({0})", unreadAlerts);
                            //TODO - Tooltip and Localize the texts

                            var messageTabUrl = Globals.NavigateURL(GetMessageTab());
                            messageLink.NavigateUrl = messageTabUrl;
                            notificationLink.NavigateUrl = messageTabUrl;

                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private int GetMessageTab()
        {            
            var tabController = new TabController();            
            var moduleController = new ModuleController();

            //On brand new install the new Message Center Module is on the child page of User Profile Page 
            //On Upgrade to 6.2.0, the Message Center module is on the User Profile Page
            var profileTab = tabController.GetTab(PortalSettings.UserTabId, PortalSettings.PortalId, true);
            if (profileTab != null)
            {
                var childTabs = tabController.GetTabsByPortal(profileTab.PortalID).DescendentsOf(profileTab.TabID);
                foreach (TabInfo tab in childTabs)
                {
                    foreach (KeyValuePair<int, ModuleInfo> kvp in moduleController.GetTabModules(tab.TabID))
                    {
                        var module = kvp.Value;
                        if (module.DesktopModule.FriendlyName == "Message Center")
                        {
                            return tab.TabID;
                        }
                    }
                }                  
            }                        

            //still can't find, just hookup with the User Profile Page
            return PortalSettings.UserTabId;
        }
    }
}