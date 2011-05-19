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
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Security.Permissions
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Namespace: DotNetNuke.Security.Permissions
    /// Class	 : TabPermissionController
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// TabPermissionController provides the Business Layer for Tab Permissions
    /// </summary>
    /// <history>
    /// 	[cnurse]	01/14/2008   Documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class TabPermissionController
    {
		#region Private Members
		
        private static readonly PermissionProvider provider = PermissionProvider.Instance();
		
		#endregion
		
		#region Public Shared Methods

        public static bool CanAddContentToPage()
        {
            return CanAddContentToPage(TabController.CurrentPage);
        }

        public static bool CanAddContentToPage(TabInfo objTab)
        {
            return provider.CanAddContentToPage(objTab) || CanAdminPage(objTab);
        }

        public static bool CanAddPage()
        {
            return CanAddPage(TabController.CurrentPage);
        }

        public static bool CanAddPage(TabInfo objTab)
        {
            return provider.CanAddPage(objTab) || CanAdminPage(objTab);
        }

        public static bool CanAdminPage()
        {
            return CanAdminPage(TabController.CurrentPage);
        }

        public static bool CanAdminPage(TabInfo objTab)
        {
            return provider.CanAdminPage(objTab);
        }

        public static bool CanCopyPage()
        {
            return CanCopyPage(TabController.CurrentPage);
        }

        public static bool CanCopyPage(TabInfo objTab)
        {
            return provider.CanCopyPage(objTab) || CanAdminPage(objTab);
        }

        public static bool CanDeletePage()
        {
            return CanDeletePage(TabController.CurrentPage);
        }

        public static bool CanDeletePage(TabInfo objTab)
        {
            return provider.CanDeletePage(objTab) || CanAdminPage(objTab);
        }

        public static bool CanExportPage()
        {
            return CanExportPage(TabController.CurrentPage);
        }

        public static bool CanExportPage(TabInfo objTab)
        {
            return provider.CanExportPage(objTab) || CanAdminPage(objTab);
        }

        public static bool CanImportPage()
        {
            return CanImportPage(TabController.CurrentPage);
        }

        public static bool CanImportPage(TabInfo objTab)
        {
            return provider.CanImportPage(objTab) || CanAdminPage(objTab);
        }

        public static bool CanManagePage()
        {
            return CanManagePage(TabController.CurrentPage);
        }

        public static bool CanManagePage(TabInfo objTab)
        {
            return provider.CanManagePage(objTab) || CanAdminPage(objTab);
        }

        public static bool CanNavigateToPage()
        {
            return CanNavigateToPage(TabController.CurrentPage);
        }

        public static bool CanNavigateToPage(TabInfo objTab)
        {
            return provider.CanNavigateToPage(objTab) || CanAdminPage(objTab);
        }

        public static bool CanViewPage()
        {
            return CanViewPage(TabController.CurrentPage);
        }

        public static bool CanViewPage(TabInfo objTab)
        {
            return provider.CanViewPage(objTab) || CanAdminPage(objTab);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// DeleteTabPermissionsByUser deletes a user's Tab Permissions in the Database
        /// </summary>
        /// <param name="objUser">The user</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeleteTabPermissionsByUser(UserInfo objUser)
        {
            provider.DeleteTabPermissionsByUser(objUser);
            var objEventLog = new EventLogController();
            objEventLog.AddLog(objUser, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.TABPERMISSION_DELETED);
            DataCache.ClearTabPermissionsCache(objUser.PortalID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetTabPermissions gets a TabPermissionCollection
        /// </summary>
        /// <param name="tabID">The ID of the tab</param>
        /// <param name="portalID">The ID of the portal</param>
        /// <history>
        /// 	[cnurse]	01/14/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static TabPermissionCollection GetTabPermissions(int tabID, int portalID)
        {
            return provider.GetTabPermissions(tabID, portalID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// HasTabPermission checks whether the current user has a specific Tab Permission
        /// </summary>
        /// <remarks>If you pass in a comma delimited list of permissions (eg "ADD,DELETE", this will return
        /// true if the user has any one of the permissions.</remarks>
        /// <param name="permissionKey">The Permission to check</param>
        /// <history>
        /// 	[cnurse]	01/15/2008   Documented
        /// 	[cnurse]	04/22/2009   Added multi-permisison support
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool HasTabPermission(string permissionKey)
        {
            return HasTabPermission(PortalController.GetCurrentPortalSettings().ActiveTab.TabPermissions, permissionKey);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// HasTabPermission checks whether the current user has a specific Tab Permission
        /// </summary>
        /// <remarks>If you pass in a comma delimited list of permissions (eg "ADD,DELETE", this will return
        /// true if the user has any one of the permissions.</remarks>
        /// <param name="objTabPermissions">The Permissions for the Tab</param>
        /// <param name="permissionKey">The Permission(s) to check</param>
        /// <history>
        /// 	[cnurse]	01/15/2008   Documented
        /// 	[cnurse]	04/22/2009   Added multi-permisison support
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool HasTabPermission(TabPermissionCollection objTabPermissions, string permissionKey)
        {
            bool hasPermission = provider.HasTabPermission(objTabPermissions, "EDIT");
            if (!hasPermission)
            {
                if (permissionKey.Contains(","))
                {
                    foreach (string permission in permissionKey.Split(','))
                    {
                        if (provider.HasTabPermission(objTabPermissions, permission))
                        {
                            hasPermission = true;
                            break;
                        }
                    }
                }
                else
                {
                    hasPermission = provider.HasTabPermission(objTabPermissions, permissionKey);
                }
            }
            return hasPermission;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// SaveTabPermissions saves a Tab's permissions
        /// </summary>
        /// <param name="tabInfo">The Tab to update</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void SaveTabPermissions(TabInfo tabInfo)
        {
            provider.SaveTabPermissions(tabInfo);
            new EventLogController().AddLog(tabInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.TABPERMISSION_UPDATED);
            DataCache.ClearTabPermissionsCache(tabInfo.PortalID);
        }
		
		#endregion
    }
}
