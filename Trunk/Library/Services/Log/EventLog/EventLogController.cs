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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

#endregion

namespace DotNetNuke.Services.Log.EventLog
{
    public partial class EventLogController : LogController
    {
        public void AddLog(PortalSettings portalSettings, int userID, EventLogType logType)
        {
            AddLog(new LogProperties(), portalSettings, userID, logType.ToString(), false);
        }

        public void AddLog(string propertyName, string propertyValue, PortalSettings portalSettings, int userID, EventLogType logType)
        {
            var properties = new LogProperties();
            var logDetailInfo = new LogDetailInfo {PropertyName = propertyName, PropertyValue = propertyValue};
            properties.Add(logDetailInfo);
            AddLog(properties, portalSettings, userID, logType.ToString(), false);
        }

        public void AddLog(string propertyName, string propertyValue, PortalSettings portalSettings, int userID, string logType)
        {
            var properties = new LogProperties();
            var logDetailInfo = new LogDetailInfo {PropertyName = propertyName, PropertyValue = propertyValue};
            properties.Add(logDetailInfo);
            AddLog(properties, portalSettings, userID, logType, false);
        }


        public void AddLog(LogProperties properties, PortalSettings portalSettings, int userID, string logTypeKey, bool bypassBuffering)
        {
            var logInfo = new LogInfo {LogUserID = userID, LogTypeKey = logTypeKey, LogProperties = properties, BypassBuffering = bypassBuffering};
            if (portalSettings != null)
            {
                logInfo.LogPortalID = portalSettings.PortalId;
                logInfo.LogPortalName = portalSettings.PortalName;
            }
            base.AddLog(logInfo);
        }

        public void AddLog(object businessObject, PortalSettings portalSettings, int userID, string userName, EventLogType logType)
        {
            AddLog(businessObject, portalSettings, userID, userName, logType.ToString());
        }

        public void AddLog(object businessObject, PortalSettings portalSettings, int userID, string userName, string logType)
        {
            var logInfo = new LogInfo {LogUserID = userID, LogTypeKey = logType};
            if (portalSettings != null)
            {
                logInfo.LogPortalID = portalSettings.PortalId;
                logInfo.LogPortalName = portalSettings.PortalName;
            }
            switch (businessObject.GetType().FullName)
            {
                case "DotNetNuke.Entities.Portals.PortalInfo":
                    var portal = (PortalInfo) businessObject;
                    logInfo.LogProperties.Add(new LogDetailInfo("PortalID", portal.PortalID.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("PortalName", portal.PortalName));
                    logInfo.LogProperties.Add(new LogDetailInfo("Description", portal.Description));
                    logInfo.LogProperties.Add(new LogDetailInfo("KeyWords", portal.KeyWords));
                    logInfo.LogProperties.Add(new LogDetailInfo("LogoFile", portal.LogoFile));
                    break;
                case "DotNetNuke.Entities.Tabs.TabInfo":
                    var tab = (TabInfo) businessObject;
                    logInfo.LogProperties.Add(new LogDetailInfo("TabID", tab.TabID.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("PortalID", tab.PortalID.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("TabName", tab.TabName));
                    logInfo.LogProperties.Add(new LogDetailInfo("Title", tab.Title));
                    logInfo.LogProperties.Add(new LogDetailInfo("Description", tab.Description));
                    logInfo.LogProperties.Add(new LogDetailInfo("KeyWords", tab.KeyWords));
                    logInfo.LogProperties.Add(new LogDetailInfo("Url", tab.Url));
                    logInfo.LogProperties.Add(new LogDetailInfo("ParentId", tab.ParentId.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("IconFile", tab.IconFile));
                    logInfo.LogProperties.Add(new LogDetailInfo("IsVisible", tab.IsVisible.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("SkinSrc", tab.SkinSrc));
                    logInfo.LogProperties.Add(new LogDetailInfo("ContainerSrc", tab.ContainerSrc));
                    break;
                case "DotNetNuke.Entities.Modules.ModuleInfo":
                    var module = (ModuleInfo) businessObject;
                    logInfo.LogProperties.Add(new LogDetailInfo("ModuleId", module.ModuleID.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("ModuleTitle", module.ModuleTitle));
                    logInfo.LogProperties.Add(new LogDetailInfo("TabModuleID", module.TabModuleID.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("TabID", module.TabID.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("PortalID", module.PortalID.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("ModuleDefId", module.ModuleDefID.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("FriendlyName", module.DesktopModule.FriendlyName));
                    logInfo.LogProperties.Add(new LogDetailInfo("IconFile", module.IconFile));
                    logInfo.LogProperties.Add(new LogDetailInfo("Visibility", module.Visibility.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("ContainerSrc", module.ContainerSrc));
                    break;
                case "DotNetNuke.Entities.Users.UserInfo":
                    var user = (UserInfo) businessObject;
                    logInfo.LogProperties.Add(new LogDetailInfo("UserID", user.UserID.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("FirstName", user.Profile.FirstName));
                    logInfo.LogProperties.Add(new LogDetailInfo("LastName", user.Profile.LastName));
                    logInfo.LogProperties.Add(new LogDetailInfo("UserName", user.Username));
                    logInfo.LogProperties.Add(new LogDetailInfo("Email", user.Email));
                    break;
                case "DotNetNuke.Security.Roles.RoleInfo":
                    var role = (RoleInfo) businessObject;
                    logInfo.LogProperties.Add(new LogDetailInfo("RoleID", role.RoleID.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("RoleName", role.RoleName));
                    logInfo.LogProperties.Add(new LogDetailInfo("PortalID", role.PortalID.ToString()));
                    logInfo.LogProperties.Add(new LogDetailInfo("Description", role.Description));
                    logInfo.LogProperties.Add(new LogDetailInfo("IsPublic", role.IsPublic.ToString()));
                    break;
                default:
                    logInfo.LogProperties.Add(new LogDetailInfo("logdetail", XmlUtils.Serialize(businessObject)));
                    break;
            }
            base.AddLog(logInfo);
        }
    }
}