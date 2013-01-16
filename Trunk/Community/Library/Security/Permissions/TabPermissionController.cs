#region Copyright
// 
// DotNetNuke� - http://www.dotnetnuke.com
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
using System.Collections;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
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
		#region "Private Shared Methods"

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// ClearPermissionCache clears the Tab Permission Cache
		/// </summary>
		/// <param name="tabId">The ID of the Tab</param>
		/// <history>
		/// 	[cnurse]	01/15/2008   Documented
		/// </history>
		/// -----------------------------------------------------------------------------
		private static void ClearPermissionCache(int tabId)
		{
			TabController objTabs = new TabController();
			TabInfo objTab = objTabs.GetTab(tabId, Null.NullInteger, false);
			DataCache.ClearTabPermissionsCache(objTab.PortalID);
		}

		#endregion
		
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

		#region "Obsolete Methods"

		[Obsolete("Deprecated in DNN 5.1.")]
		public int AddTabPermission(TabPermissionInfo objTabPermission)
		{
			int id = Convert.ToInt32(DataProvider.Instance().AddTabPermission(objTabPermission.TabID, objTabPermission.PermissionID, objTabPermission.RoleID, objTabPermission.AllowAccess, objTabPermission.UserID, UserController.GetCurrentUserInfo().UserID));
			ClearPermissionCache(objTabPermission.TabID);
			return id;
		}

		[Obsolete("Deprecated in DNN 5.1.")]
		public void DeleteTabPermission(int tabPermissionID)
		{
			DataProvider.Instance().DeleteTabPermission(tabPermissionID);
		}

		[Obsolete("Deprecated in DNN 5.1.")]
		public void DeleteTabPermissionsByTabID(int tabID)
		{
			DataProvider.Instance().DeleteTabPermissionsByTabID(tabID);
			ClearPermissionCache(tabID);
		}

		[Obsolete("Deprecated in DNN 5.0.  Use DeleteTabPermissionsByUser(UserInfo) ")]
		public void DeleteTabPermissionsByUserID(UserInfo objUser)
		{
			DataProvider.Instance().DeleteTabPermissionsByUserID(objUser.PortalID, objUser.UserID);
			DataCache.ClearTabPermissionsCache(objUser.PortalID);
		}

		[Obsolete("Deprecated in DNN 5.0. Please use TabPermissionCollection.ToString(String)")]
		public string GetTabPermissions(TabPermissionCollection tabPermissions, string permissionKey)
		{
			return tabPermissions.ToString(permissionKey);
		}

		[Obsolete("Deprecated in DNN 5.0.  This should have been declared as Friend as it was never meant to be used outside of the core.")]
		public ArrayList GetTabPermissionsByPortal(int PortalID)
		{
			return CBO.FillCollection(DataProvider.Instance().GetTabPermissionsByPortal(PortalID), typeof(TabPermissionInfo));
		}

		[Obsolete("Deprecated in DNN 5.0.  Please use GetTabPermissions(TabId, PortalId)")]
		public ArrayList GetTabPermissionsByTabID(int TabID)
		{
			return CBO.FillCollection(DataProvider.Instance().GetTabPermissionsByTabID(TabID, -1), typeof(TabPermissionInfo));
		}

		[Obsolete("Deprecated in DNN 5.0. Please use TabPermissionCollection.ToString(String)")]
		public string GetTabPermissionsByTabID(ArrayList arrTabPermissions, int TabID, string PermissionKey)
		{
			//Create a Tab Permission Collection from the ArrayList
			TabPermissionCollection tabPermissions = new TabPermissionCollection(arrTabPermissions, TabID);

			//Return the permission string for permissions with specified TabId
			return tabPermissions.ToString(PermissionKey);
		}

		[Obsolete("Deprecated in DNN 5.0.  Please use GetTabPermissions(TabId, PortalId)")]
		public TabPermissionCollection GetTabPermissionsByTabID(ArrayList arrTabPermissions, int TabID)
		{
			return new TabPermissionCollection(arrTabPermissions, TabID);
		}

		[Obsolete("Deprecated in DNN 5.0.  Please use GetTabPermissions(TabId, PortalId)")]
		public Security.Permissions.TabPermissionCollection GetTabPermissionsCollectionByTabID(int TabID)
		{
			return new TabPermissionCollection(CBO.FillCollection(DataProvider.Instance().GetTabPermissionsByTabID(TabID, -1), typeof(TabPermissionInfo)));
		}

		[Obsolete("Deprecated in DNN 5.0.  Please use GetTabPermissions(TabId, PortalId)")]
		public TabPermissionCollection GetTabPermissionsCollectionByTabID(ArrayList arrTabPermissions, int TabID)
		{
			return new TabPermissionCollection(arrTabPermissions, TabID);
		}

		[Obsolete("Deprecated in DNN 5.1.  Please use GetTabPermissions(TabId, PortalId)")]
		public TabPermissionCollection GetTabPermissionsCollectionByTabID(int tabID, int portalID)
		{
			return GetTabPermissions(tabID, portalID);
		}

		[Obsolete("Deprecated in DNN 5.1.")]
		public void UpdateTabPermission(TabPermissionInfo objTabPermission)
		{
			DataProvider.Instance().UpdateTabPermission(objTabPermission.TabPermissionID, objTabPermission.TabID, objTabPermission.PermissionID, objTabPermission.RoleID, objTabPermission.AllowAccess, objTabPermission.UserID, UserController.GetCurrentUserInfo().UserID);
			ClearPermissionCache(objTabPermission.TabID);
		}

		#endregion
    }
}
