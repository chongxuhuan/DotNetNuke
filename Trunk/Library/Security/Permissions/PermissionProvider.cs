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
using System.Collections.Generic;
using System.Data;
using System.Linq;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Security.Permissions
{
    public class PermissionProvider
    {
		#region "Private Members"

        //Module Permission Codes
        private const string AdminFolderPermissionCode = "WRITE";
        private const string AddFolderPermissionCode = "WRITE";
        private const string CopyFolderPermissionCode = "WRITE";
        private const string DeleteFolderPermissionCode = "WRITE";
        private const string ManageFolderPermissionCode = "WRITE";
        private const string ViewFolderPermissionCode = "READ";

        //Module Permission Codes
        private const string AdminModulePermissionCode = "EDIT";
        private const string ContentModulePermissionCode = "EDIT";
        private const string DeleteModulePermissionCode = "EDIT";
        private const string ExportModulePermissionCode = "EDIT";
        private const string ImportModulePermissionCode = "EDIT";
        private const string ManageModulePermissionCode = "EDIT";
        private const string ViewModulePermissionCode = "VIEW";

        //Page Permission Codes
        private const string AddPagePermissionCode = "EDIT";
        private const string AdminPagePermissionCode = "EDIT";
        private const string ContentPagePermissionCode = "EDIT";
        private const string CopyPagePermissionCode = "EDIT";
        private const string DeletePagePermissionCode = "EDIT";
        private const string ExportPagePermissionCode = "EDIT";
        private const string ImportPagePermissionCode = "EDIT";
        private const string ManagePagePermissionCode = "EDIT";
        private const string NavigatePagePermissionCode = "VIEW";
        private const string ViewPagePermissionCode = "VIEW";
        private readonly DataProvider dataProvider = DataProvider.Instance();

		#endregion

		#region "Shared/Static Methods"

        //return the provider
        public virtual string LocalResourceFile
        {
            get
            {
                return Localization.GlobalResourceFile;
            }
        }

        public static PermissionProvider Instance()
        {
            return ComponentFactory.GetComponent<PermissionProvider>();
        }
		
		#endregion
		
		#region "Private Methods"

        private object GetFolderPermissionsCallBack(CacheItemArgs cacheItemArgs)
        {
            var PortalID = (int) cacheItemArgs.ParamList[0];
            IDataReader dr = dataProvider.GetFolderPermissionsByPortal(PortalID);
            var dic = new Dictionary<string, FolderPermissionCollection>();
            try
            {
                FolderPermissionInfo obj;
                while (dr.Read())
                {
					//fill business object
                    obj = CBO.FillObject<FolderPermissionInfo>(dr, false);
                    string dictionaryKey = obj.FolderPath;
                    if (string.IsNullOrEmpty(dictionaryKey))
                    {
                        dictionaryKey = "[PortalRoot]";
                    }
					
                    //add Folder Permission to dictionary
                    if (dic.ContainsKey(dictionaryKey))
                    {
                        //Add FolderPermission to FolderPermission Collection already in dictionary for FolderPath
                        dic[dictionaryKey].Add(obj);
                    }
                    else
                    {
						//Create new FolderPermission Collection for TabId
                        var collection = new FolderPermissionCollection();

                        //Add Permission to Collection
                        collection.Add(obj);

                        //Add Collection to Dictionary
                        dic.Add(dictionaryKey, collection);
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return dic;
        }

        private Dictionary<string, FolderPermissionCollection> GetFolderPermissions(int PortalID)
        {
            string cacheKey = string.Format(DataCache.FolderPermissionCacheKey, PortalID);
            return
                CBO.GetCachedObject<Dictionary<string, FolderPermissionCollection>>(
                    new CacheItemArgs(cacheKey, DataCache.FolderPermissionCacheTimeOut, DataCache.FolderPermissionCachePriority, PortalID), GetFolderPermissionsCallBack);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModulePermissions gets a Dictionary of ModulePermissionCollections by
        /// Module.
        /// </summary>
        /// <param name="tabID">The ID of the tab</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private Dictionary<int, ModulePermissionCollection> GetModulePermissions(int tabID)
        {
            string cacheKey = string.Format(DataCache.ModulePermissionCacheKey, tabID);
            return CBO.GetCachedObject<Dictionary<int, ModulePermissionCollection>>(
                new CacheItemArgs(cacheKey, DataCache.ModulePermissionCacheTimeOut, DataCache.ModulePermissionCachePriority, tabID), GetModulePermissionsCallBack);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModulePermissionsCallBack gets a Dictionary of ModulePermissionCollections by
        /// Module from the the Database.
        /// </summary>
        /// <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        /// needed for the database call</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private object GetModulePermissionsCallBack(CacheItemArgs cacheItemArgs)
        {
            var tabID = (int) cacheItemArgs.ParamList[0];
            IDataReader dr = dataProvider.GetModulePermissionsByTabID(tabID);
            var dic = new Dictionary<int, ModulePermissionCollection>();
            try
            {
                ModulePermissionInfo obj;
                while (dr.Read())
                {
					//fill business object
                    obj = CBO.FillObject<ModulePermissionInfo>(dr, false);

                    //add Module Permission to dictionary
                    if (dic.ContainsKey(obj.ModuleID))
                    {
                        dic[obj.ModuleID].Add(obj);
                    }
                    else
                    {
						//Create new ModulePermission Collection for ModuleId
                        var collection = new ModulePermissionCollection();

                        //Add Permission to Collection
                        collection.Add(obj);

                        //Add Collection to Dictionary
                        dic.Add(obj.ModuleID, collection);
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
				//close datareader
                CBO.CloseDataReader(dr, true);
            }
            return dic;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetTabPermissions gets a Dictionary of TabPermissionCollections by
        /// Tab.
        /// </summary>
        /// <param name="portalID">The ID of the portal</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private Dictionary<int, TabPermissionCollection> GetTabPermissions(int portalID)
        {
            string cacheKey = string.Format(DataCache.TabPermissionCacheKey, portalID);
            return CBO.GetCachedObject<Dictionary<int, TabPermissionCollection>>(new CacheItemArgs(cacheKey, DataCache.TabPermissionCacheTimeOut, DataCache.TabPermissionCachePriority, portalID),
                                                                                 GetTabPermissionsCallBack);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetTabPermissionsCallBack gets a Dictionary of TabPermissionCollections by
        /// Tab from the the Database.
        /// </summary>
        /// <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        /// needed for the database call</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private object GetTabPermissionsCallBack(CacheItemArgs cacheItemArgs)
        {
            var portalID = (int) cacheItemArgs.ParamList[0];
            IDataReader dr = dataProvider.GetTabPermissionsByPortal(portalID);
            var dic = new Dictionary<int, TabPermissionCollection>();
            try
            {
                TabPermissionInfo obj;
                while (dr.Read())
                {
					//fill business object
                    obj = CBO.FillObject<TabPermissionInfo>(dr, false);

                    //add Tab Permission to dictionary
                    if (dic.ContainsKey(obj.TabID))
                    {
                        //Add TabPermission to TabPermission Collection already in dictionary for TabId
                        dic[obj.TabID].Add(obj);
                    }
                    else
                    {
						//Create new TabPermission Collection for TabId
                        var collection = new TabPermissionCollection();

                        //Add Permission to Collection
                        collection.Add(obj);

                        //Add Collection to Dictionary
                        dic.Add(obj.TabID, collection);
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
				//close datareader
                CBO.CloseDataReader(dr, true);
            }
            return dic;
        }
		
		#endregion
		
		#region "Public Methods"
		
		#region "FolderPermission Methods"

        public virtual bool CanAdminFolder(FolderInfo folder)
        {
            if (folder == null) return false;
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(AdminFolderPermissionCode));
        }

        public virtual bool CanAddFolder(FolderInfo folder)
        {
            if (folder == null) return false;
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(AddFolderPermissionCode));
        }

        public virtual bool CanCopyFolder(FolderInfo folder)
        {
            if (folder == null) return false;
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(CopyFolderPermissionCode));
        }

        public virtual bool CanDeleteFolder(FolderInfo folder)
        {
            if (folder == null) return false;
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(DeleteFolderPermissionCode));
        }

        public virtual bool CanManageFolder(FolderInfo folder)
        {
            if (folder == null) return false;
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(ManageFolderPermissionCode));
        }

        public virtual bool CanViewFolder(FolderInfo folder)
        {
            if (folder == null) return false;
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(ViewFolderPermissionCode));
        }

        public virtual void DeleteFolderPermissionsByUser(UserInfo objUser)
        {
            dataProvider.DeleteFolderPermissionsByUserID(objUser.PortalID, objUser.UserID);
        }

        public virtual FolderPermissionCollection GetFolderPermissionsCollectionByFolder(int PortalID, string Folder)
        {
			string dictionaryKey = Folder;
			if (string.IsNullOrEmpty(dictionaryKey))
			{
				dictionaryKey = "[PortalRoot]";
			}
			//Get the Portal FolderPermission Dictionary
			var dicFolderPermissions = GetFolderPermissions(PortalID);

			//Get the Collection from the Dictionary
			var folderPermissions =
				dicFolderPermissions.FirstOrDefault(kvp => kvp.Key.Equals(dictionaryKey, StringComparison.InvariantCultureIgnoreCase)).Value;

			if (folderPermissions == null)
			{
				//try the database
				folderPermissions = new FolderPermissionCollection(CBO.FillCollection(dataProvider.GetFolderPermissionsByFolderPath(PortalID, Folder, -1), typeof(FolderPermissionInfo)), Folder);
			}
			return folderPermissions;
        }

        public virtual bool HasFolderPermission(FolderPermissionCollection objFolderPermissions, string PermissionKey)
        {
            return PortalSecurity.IsInRoles(objFolderPermissions.ToString(PermissionKey));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// SaveFolderPermissions updates a Folder's permissions
        /// </summary>
        /// <param name="folder">The Folder to update</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public virtual void SaveFolderPermissions(FolderInfo folder)
        {
            if ((folder.FolderPermissions != null))
            {
                FolderPermissionCollection folderPermissions = GetFolderPermissionsCollectionByFolder(folder.PortalID, folder.FolderPath);

                //Ensure that if role/user has been given a permission that is not Read/Browse then they also need Read/Browse
                var permController = new PermissionController();
                ArrayList permArray = permController.GetPermissionByCodeAndKey("SYSTEM_FOLDER", "READ");
                PermissionInfo readPerm = null;
                if (permArray.Count == 1)
                {
                    readPerm = permArray[0] as PermissionInfo;
                }

                PermissionInfo browsePerm = null;
                permArray = permController.GetPermissionByCodeAndKey("SYSTEM_FOLDER", "BROWSE");
                if (permArray.Count == 1)
                {
                    browsePerm = permArray[0] as PermissionInfo;
                }

                var additionalPermissions = new FolderPermissionCollection();
                foreach (FolderPermissionInfo folderPermission in folder.FolderPermissions)
                {
                    if (folderPermission.PermissionKey != "BROWSE" && folderPermission.PermissionKey != "READ")
                    {
                        //Try to add Read permission
                        var newFolderPerm = new FolderPermissionInfo(readPerm);
                        newFolderPerm.FolderID = folderPermission.FolderID;
                        newFolderPerm.RoleID = folderPermission.RoleID;
                        newFolderPerm.UserID = folderPermission.UserID;
                        newFolderPerm.AllowAccess = folderPermission.AllowAccess;

                        additionalPermissions.Add(newFolderPerm);

                        //Try to add Browse permission
                        newFolderPerm = new FolderPermissionInfo(browsePerm);
                        newFolderPerm.FolderID = folderPermission.FolderID;
                        newFolderPerm.RoleID = folderPermission.RoleID;
                        newFolderPerm.UserID = folderPermission.UserID;
                        newFolderPerm.AllowAccess = folderPermission.AllowAccess;

                        additionalPermissions.Add(newFolderPerm);
                    }
                }

                foreach (FolderPermissionInfo folderPermission in additionalPermissions)
                {
                    folder.FolderPermissions.Add(folderPermission, true);
                }

                if (!folderPermissions.CompareTo(folder.FolderPermissions))
                {
                    dataProvider.DeleteFolderPermissionsByFolderPath(folder.PortalID, folder.FolderPath);

                    foreach (FolderPermissionInfo folderPermission in folder.FolderPermissions)
                    {
                        dataProvider.AddFolderPermission(folder.FolderID,
                                                         folderPermission.PermissionID,
                                                         folderPermission.RoleID,
                                                         folderPermission.AllowAccess,
                                                         folderPermission.UserID,
                                                         UserController.GetCurrentUserInfo().UserID);
                    }
                }
            }
        }
		
		#endregion
		
		#region "ModulePermission Methods"

        public virtual bool CanAdminModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(AdminModulePermissionCode));
        }

        public virtual bool CanDeleteModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(DeleteModulePermissionCode));
        }

        public virtual bool CanEditModuleContent(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ContentModulePermissionCode));
        }

        public virtual bool CanExportModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ExportModulePermissionCode));
        }

        public virtual bool CanImportModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ImportModulePermissionCode));
        }

        public virtual bool CanManageModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ManageModulePermissionCode));
        }

        public virtual bool CanViewModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ViewModulePermissionCode));
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
        public virtual void DeleteModulePermissionsByUser(UserInfo objUser)
        {
            dataProvider.DeleteModulePermissionsByUserID(objUser.PortalID, objUser.UserID);
            DataCache.ClearModulePermissionsCachesByPortal(objUser.PortalID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModulePermissions gets a ModulePermissionCollection
        /// </summary>
        /// <param name="moduleID">The ID of the module</param>
        /// <param name="tabID">The ID of the tab</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public virtual ModulePermissionCollection GetModulePermissions(int moduleID, int tabID)
        {
            bool bFound = false;

            //Get the Tab ModulePermission Dictionary
            Dictionary<int, ModulePermissionCollection> dicModulePermissions = GetModulePermissions(tabID);

            //Get the Collection from the Dictionary
            ModulePermissionCollection modulePermissions = null;
            bFound = dicModulePermissions.TryGetValue(moduleID, out modulePermissions);
            if (!bFound)
            {
				//try the database
                modulePermissions = new ModulePermissionCollection(CBO.FillCollection(dataProvider.GetModulePermissionsByModuleID(moduleID, -1), typeof (ModulePermissionInfo)), moduleID);
            }
            return modulePermissions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// HasModulePermission checks whether the current user has a specific Module Permission
        /// </summary>
        /// <param name="objModulePermissions">The Permissions for the Module</param>
        /// <param name="permissionKey">The Permission to check</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public virtual bool HasModulePermission(ModulePermissionCollection objModulePermissions, string permissionKey)
        {
            return PortalSecurity.IsInRoles(objModulePermissions.ToString(permissionKey));
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
        public virtual void SaveModulePermissions(ModuleInfo objModule)
        {
            if (objModule.ModulePermissions != null)
            {
                ModulePermissionCollection modulePermissions = ModulePermissionController.GetModulePermissions(objModule.ModuleID, objModule.TabID);
                if (!modulePermissions.CompareTo(objModule.ModulePermissions))
                {
                    dataProvider.DeleteModulePermissionsByModuleID(objModule.ModuleID);
                    foreach (ModulePermissionInfo objModulePermission in objModule.ModulePermissions)
                    {
                        if (objModule.InheritViewPermissions && objModulePermission.PermissionKey == "VIEW")
                        {
                            dataProvider.DeleteModulePermission(objModulePermission.ModulePermissionID);
                        }
                        else
                        {
                            dataProvider.AddModulePermission(objModule.ModuleID,
                                                             objModulePermission.PermissionID,
                                                             objModulePermission.RoleID,
                                                             objModulePermission.AllowAccess,
                                                             objModulePermission.UserID,
                                                             UserController.GetCurrentUserInfo().UserID);
                        }
                    }
                }
            }
        }
		
		#endregion
		
		#region "TabPermission Methods"

        public virtual bool CanAddContentToPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ContentPagePermissionCode));
        }

        public virtual bool CanAddPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(AddPagePermissionCode));
        }

        public virtual bool CanAdminPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(AdminPagePermissionCode));
        }

        public virtual bool CanCopyPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(CopyPagePermissionCode));
        }

        public virtual bool CanDeletePage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(DeletePagePermissionCode));
        }

        public virtual bool CanExportPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ExportPagePermissionCode));
        }

        public virtual bool CanImportPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ImportPagePermissionCode));
        }

        public virtual bool CanManagePage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ManagePagePermissionCode));
        }

        public virtual bool CanNavigateToPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(NavigatePagePermissionCode));
        }

        public virtual bool CanViewPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ViewPagePermissionCode));
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
        public virtual void DeleteTabPermissionsByUser(UserInfo objUser)
        {
            dataProvider.DeleteTabPermissionsByUserID(objUser.PortalID, objUser.UserID);
            DataCache.ClearTabPermissionsCache(objUser.PortalID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetTabPermissions gets a TabPermissionCollection
        /// </summary>
        /// <param name="tabID">The ID of the tab</param>
        /// <param name="portalID">The ID of the portal</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public virtual TabPermissionCollection GetTabPermissions(int tabID, int portalID)
        {
            bool bFound = false;

            //Get the Portal TabPermission Dictionary
            Dictionary<int, TabPermissionCollection> dicTabPermissions = GetTabPermissions(portalID);

            //Get the Collection from the Dictionary
            TabPermissionCollection tabPermissions = null;
            bFound = dicTabPermissions.TryGetValue(tabID, out tabPermissions);
            if (!bFound)
            {
                //try the database
                tabPermissions = new TabPermissionCollection(CBO.FillCollection(dataProvider.GetTabPermissionsByTabID(tabID, -1), typeof (TabPermissionInfo)), tabID);
            }
            return tabPermissions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// HasTabPermission checks whether the current user has a specific Tab Permission
        /// </summary>
        /// <param name="objTabPermissions">The Permissions for the Tab</param>
        /// <param name="permissionKey">The Permission to check</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public virtual bool HasTabPermission(TabPermissionCollection objTabPermissions, string permissionKey)
        {
            return PortalSecurity.IsInRoles(objTabPermissions.ToString(permissionKey));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// SaveTabPermissions saves a Tab's permissions
        /// </summary>
        /// <param name="objTab">The Tab to update</param>
        /// <history>
        /// 	[cnurse]	04/15/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public virtual void SaveTabPermissions(TabInfo objTab)
        {
            TabPermissionCollection objCurrentTabPermissions = GetTabPermissions(objTab.TabID, objTab.PortalID);
            var objEventLog = new EventLogController();
            if (!objCurrentTabPermissions.CompareTo(objTab.TabPermissions))
            {
                dataProvider.DeleteTabPermissionsByTabID(objTab.TabID);
                objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.TABPERMISSION_DELETED);
                if (objTab.TabPermissions != null)
                {
                    foreach (TabPermissionInfo objTabPermission in objTab.TabPermissions)
                    {
                        dataProvider.AddTabPermission(objTab.TabID,
                                                      objTabPermission.PermissionID,
                                                      objTabPermission.RoleID,
                                                      objTabPermission.AllowAccess,
                                                      objTabPermission.UserID,
                                                      UserController.GetCurrentUserInfo().UserID);
                        objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.TABPERMISSION_CREATED);
                    }
                }
            }
        }
		
		#endregion
		
		#endregion
    }
}
