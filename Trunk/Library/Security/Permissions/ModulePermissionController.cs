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
using System.Text.RegularExpressions;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;

#endregion

namespace DotNetNuke.Security.Permissions
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Namespace: DotNetNuke.Security.Permissions
    /// Class	 : ModulePermissionController
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// ModulePermissionController provides the Business Layer for Module Permissions
    /// </summary>
    /// <history>
    /// 	[cnurse]	01/14/2008   Documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class ModulePermissionController
    {
        private static readonly PermissionProvider provider = PermissionProvider.Instance();

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ClearPermissionCache clears the Module Permission Cache
        /// </summary>
        /// <param name="moduleId">The ID of the Module</param>
        /// <history>
        /// 	[cnurse]	01/15/2008   Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void ClearPermissionCache(int moduleId)
        {
            var objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule(moduleId, Null.NullInteger, false);
            DataCache.ClearModulePermissionsCache(objModule.TabID);
        }

        private static bool CanAddContentToPage(ModuleInfo objModule)
        {
            bool canManage = Null.NullBoolean;
            TabInfo objTab = new TabController().GetTab(objModule.TabID, objModule.PortalID, false);
            canManage = TabPermissionController.CanAddContentToPage(objTab);
            return canManage;
        }

        public static bool CanAdminModule(ModuleInfo objModule)
        {
            return provider.CanAdminModule(objModule);
        }

        public static bool CanDeleteModule(ModuleInfo objModule)
        {
            return CanAddContentToPage(objModule) || provider.CanDeleteModule(objModule);
        }

        public static bool CanEditModuleContent(ModuleInfo objModule)
        {
            return CanAddContentToPage(objModule) || provider.CanEditModuleContent(objModule);
        }

        public static bool CanExportModule(ModuleInfo objModule)
        {
            return provider.CanExportModule(objModule);
        }

        public static bool CanImportModule(ModuleInfo objModule)
        {
            return provider.CanImportModule(objModule);
        }

        public static bool CanManageModule(ModuleInfo objModule)
        {
            return CanAddContentToPage(objModule) || provider.CanManageModule(objModule);
        }

        public static bool CanViewModule(ModuleInfo objModule)
        {
            bool canView = Null.NullBoolean;
            if (objModule.InheritViewPermissions)
            {
                TabInfo objTab = new TabController().GetTab(objModule.TabID, objModule.PortalID, false);
                canView = TabPermissionController.CanViewPage(objTab);
            }
            else
            {
                canView = provider.CanViewModule(objModule);
            }
            return canView;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// DeleteModulePermissionsByUser deletes a user's Module Permission in the Database
        /// </summary>
        /// <param name="objUser">The user</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeleteModulePermissionsByUser(UserInfo objUser)
        {
            provider.DeleteModulePermissionsByUser(objUser);
            DataCache.ClearModulePermissionsCachesByPortal(objUser.PortalID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModulePermissions gets a ModulePermissionCollection
        /// </summary>
        /// <param name="moduleID">The ID of the module</param>
        /// <param name="tabID">The ID of the tab</param>
        /// <history>
        /// 	[cnurse]	01/14/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static ModulePermissionCollection GetModulePermissions(int moduleID, int tabID)
        {
            return provider.GetModulePermissions(moduleID, tabID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// HasModulePermission checks whether the current user has a specific Module Permission
        /// </summary>
        /// <remarks>If you pass in a comma delimited list of permissions (eg "ADD,DELETE", this will return
        /// true if the user has any one of the permissions.</remarks>
        /// <param name="objModulePermissions">The Permissions for the Module</param>
        /// <param name="permissionKey">The Permission to check</param>
        /// <history>
        /// 	[cnurse]	01/15/2008   Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool HasModulePermission(ModulePermissionCollection objModulePermissions, string permissionKey)
        {
            bool hasPermission = Null.NullBoolean;
            if (permissionKey.Contains(","))
            {
                foreach (string permission in permissionKey.Split(','))
                {
                    if (provider.HasModulePermission(objModulePermissions, permission))
                    {
                        hasPermission = true;
                        break;
                    }
                }
            }
            else
            {
                hasPermission = provider.HasModulePermission(objModulePermissions, permissionKey);
            }
            return hasPermission;
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Determines if user has the necessary permissions to access an item with the
        /// designated AccessLevel.
        /// </summary>
        /// <param name="AccessLevel">The SecurityAccessLevel required to access a portal module or module action.</param>
        /// <param name="permissionKey">If Security Access is Edit the permissionKey is the actual "edit" permisison required.</param>
        /// <param name="ModuleConfiguration">The ModuleInfo object for the associated module.</param>
        /// <returns>A boolean value indicating if the user has the necessary permissions</returns>
        /// <remarks>Every module control and module action has an associated permission level.  This
        /// function determines whether the user represented by UserName has sufficient permissions, as
        /// determined by the PortalSettings and ModuleSettings, to access a resource with the
        /// designated AccessLevel.</remarks>
        /// <history>
        ///     [cnurse]        02/27/2007  New overload
        ///     [cnurse]        02/27/2007  Moved from PortalSecurity
        /// </history>
        ///-----------------------------------------------------------------------------
        public static bool HasModuleAccess(SecurityAccessLevel AccessLevel, string permissionKey, ModuleInfo ModuleConfiguration)
        {
            bool blnAuthorized = false;
            UserInfo objUser = UserController.GetCurrentUserInfo();
            if (objUser != null && objUser.IsSuperUser)
            {
                blnAuthorized = true;
            }
            else
            {
                switch (AccessLevel)
                {
                    case SecurityAccessLevel.Anonymous:
                        blnAuthorized = true;
                        break;
                    case SecurityAccessLevel.View:
                        if (TabPermissionController.CanViewPage() || CanViewModule(ModuleConfiguration))
                        {
                            blnAuthorized = true;
                        }
                        break;
                    case SecurityAccessLevel.Edit:
                        if (TabPermissionController.CanAddContentToPage())
                        {
                            blnAuthorized = true;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(permissionKey))
                            {
                                permissionKey = "CONTENT,DELETE,EDIT,EXPORT,IMPORT,MANAGE";
                            }
                            if (ModuleConfiguration != null && CanViewModule(ModuleConfiguration) &&
                                (HasModulePermission(ModuleConfiguration.ModulePermissions, permissionKey) || HasModulePermission(ModuleConfiguration.ModulePermissions, "EDIT")))
                            {
                                blnAuthorized = true;
                            }
                        }
                        break;
                    case SecurityAccessLevel.Admin:
                        if (TabPermissionController.CanAdminPage())
                        {
                            blnAuthorized = true;
                        }
						else if (ModuleConfiguration != null && CanEditModuleContent(ModuleConfiguration))
                        {
                            if (String.IsNullOrEmpty(permissionKey))
                            {
                                permissionKey = "CONTENT,DELETE,EDIT,EXPORT,IMPORT,MANAGE";
                            }
                            if (CanViewModule(ModuleConfiguration) && HasModulePermission(ModuleConfiguration.ModulePermissions, permissionKey))
                            {
                                blnAuthorized = true;
                            }
                        }
                        break;
                    case SecurityAccessLevel.Host:
                        break;
                }
            }
            return blnAuthorized;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// SaveModulePermissions updates a Module's permissions
        /// </summary>
        /// <param name="objModule">The Module to update</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void SaveModulePermissions(ModuleInfo objModule)
        {
            provider.SaveModulePermissions(objModule);
            DataCache.ClearModulePermissionsCache(objModule.TabID);
        }

        #region "Obsolete Methods"

        [Obsolete("Deprecated in DNN 5.1.")]
        public int AddModulePermission(ModulePermissionInfo objModulePermission, int tabId)
        {
            int Id = DataProvider.Instance().AddModulePermission(objModulePermission.ModuleID,
                                                                 objModulePermission.PermissionID,
                                                                 objModulePermission.RoleID,
                                                                 objModulePermission.AllowAccess,
                                                                 objModulePermission.UserID,
                                                                 UserController.GetCurrentUserInfo().UserID);
            DataCache.ClearModulePermissionsCache(tabId);
            return Id;
        }

        [Obsolete("Deprecated in DNN 5.0.")]
        public int AddModulePermission(ModulePermissionInfo objModulePermission)
        {
            int Id = DataProvider.Instance().AddModulePermission(objModulePermission.ModuleID,
                                                                 objModulePermission.PermissionID,
                                                                 objModulePermission.RoleID,
                                                                 objModulePermission.AllowAccess,
                                                                 objModulePermission.UserID,
                                                                 UserController.GetCurrentUserInfo().UserID);
            ClearPermissionCache(objModulePermission.ModuleID);
            return Id;
        }

        [Obsolete("Deprecated in DNN 5.1.")]
        public void DeleteModulePermission(int modulePermissionID)
        {
            DataProvider.Instance().DeleteModulePermission(modulePermissionID);
        }

        [Obsolete("Deprecated in DNN 5.1.")]
        public void DeleteModulePermissionsByModuleID(int moduleID)
        {
            DataProvider.Instance().DeleteModulePermissionsByModuleID(moduleID);
            ClearPermissionCache(moduleID);
        }

        [Obsolete("Deprecated in DNN 5.0.  Use DeleteModulePermissionsByUser(UserInfo) ")]
        public void DeleteModulePermissionsByUserID(UserInfo objUser)
        {
            DataProvider.Instance().DeleteModulePermissionsByUserID(objUser.PortalID, objUser.UserID);
            DataCache.ClearModulePermissionsCachesByPortal(objUser.PortalID);
        }

        [Obsolete("Deprecated in DNN 5.1.")]
        public ModulePermissionInfo GetModulePermission(int modulePermissionID)
        {
            return CBO.FillObject<ModulePermissionInfo>(DataProvider.Instance().GetModulePermission(modulePermissionID), true);
        }

        [Obsolete("Deprecated in DNN 5.0. Replaced by ModulePermissionColelction.ToString(String)")]
        public string GetModulePermissions(ModulePermissionCollection modulePermissions, string permissionKey)
        {
            return modulePermissions.ToString(permissionKey);
        }

        [Obsolete("Deprecated in DNN 5.0.  This should have been declared as Friend as it was never meant to be used outside of the core.")]
        public ArrayList GetModulePermissionsByPortal(int PortalID)
        {
            return CBO.FillCollection(DataProvider.Instance().GetModulePermissionsByPortal(PortalID), typeof (ModulePermissionInfo));
        }

        [Obsolete("Deprecated in DNN 5.0.  This should have been declared as Friend as it was never meant to be used outside of the core.")]
        public ArrayList GetModulePermissionsByTabID(int TabID)
        {
            return CBO.FillCollection(DataProvider.Instance().GetModulePermissionsByTabID(TabID), typeof (ModulePermissionInfo));
        }

        [Obsolete("Deprecated in DNN 5.0.  Use GetModulePermissions(ModulePermissionCollection, String) ")]
        public string GetModulePermissionsByModuleID(ModuleInfo objModule, string PermissionKey)
        {
            //Create a Module Permission Collection from the ArrayList
            var modulePermissions = new ModulePermissionCollection(objModule);

            //Return the permission string for permissions with specified TabId
            return modulePermissions.ToString(PermissionKey);
        }

        [Obsolete("Deprecated in DNN 5.1.  GetModulePermissions(integer, integer) ")]
        public ModulePermissionCollection GetModulePermissionsCollectionByModuleID(int moduleID, int tabID)
        {
            return GetModulePermissions(moduleID, tabID);
        }

        [Obsolete("Deprecated in DNN 5.0.  Use GetModulePermissions(integer, integer) ")]
        public ModulePermissionCollection GetModulePermissionsCollectionByModuleID(int moduleID)
        {
            return new ModulePermissionCollection(CBO.FillCollection(DataProvider.Instance().GetModulePermissionsByModuleID(moduleID, -1), typeof (ModulePermissionInfo)));
        }

        [Obsolete("Deprecated in DNN 5.0.  Use GetModulePermissions(integer, integer) ")]
        public ModulePermissionCollection GetModulePermissionsCollectionByModuleID(ArrayList arrModulePermissions, int moduleID)
        {
            return new ModulePermissionCollection(arrModulePermissions, moduleID);
        }

        [Obsolete("Deprecated in DNN 5.0.  It was used to replace lists of RoleIds by lists of RoleNames.")]
        public string GetRoleNamesFromRoleIDs(string Roles)
        {
            string strRoles = "";
            if (Roles.IndexOf(";") > 0)
            {
                string[] arrRoles = Roles.Split(';');
                int i = 0;
                for (i = 0; i <= arrRoles.Length - 1; i++)
                {
                    if (Regex.IsMatch(arrRoles[i], "^\\d+$"))
                    {
                        strRoles += Globals.GetRoleName(Convert.ToInt32(arrRoles[i])) + ";";
                    }
                }
            }
            else if (Roles.Trim().Length > 0)
            {
                strRoles = Globals.GetRoleName(Convert.ToInt32(Roles.Trim()));
            }
            if (!strRoles.StartsWith(";"))
            {
                strRoles += ";";
            }
            return strRoles;
        }

        [Obsolete("Deprecated in DNN 5.0.  Use HasModulePermission(ModulePermissionCollection, string)")]
        public static bool HasModulePermission(int moduleID, string PermissionKey)
        {
            var objModulePermissions = new ModulePermissionCollection(CBO.FillCollection(DataProvider.Instance().GetModulePermissionsByModuleID(moduleID, -1), typeof (ModulePermissionInfo)));
            return HasModulePermission(objModulePermissions, PermissionKey);
        }

        [Obsolete("Deprecated in DNN 5.1.  Use HasModulePermission(ModulePermissionCollection, string)")]
        public static bool HasModulePermission(int moduleID, int tabID, string PermissionKey)
        {
            return HasModulePermission(GetModulePermissions(moduleID, tabID), PermissionKey);
        }

        [Obsolete("Deprecated in DNN 5.1.")]
        public void UpdateModulePermission(ModulePermissionInfo objModulePermission)
        {
            DataProvider.Instance().UpdateModulePermission(objModulePermission.ModulePermissionID,
                                                           objModulePermission.ModuleID,
                                                           objModulePermission.PermissionID,
                                                           objModulePermission.RoleID,
                                                           objModulePermission.AllowAccess,
                                                           objModulePermission.UserID,
                                                           UserController.GetCurrentUserInfo().UserID);
            ClearPermissionCache(objModulePermission.ModuleID);
        }

        #endregion
    }
}
