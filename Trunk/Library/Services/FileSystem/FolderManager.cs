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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Log.EventLog;

namespace DotNetNuke.Services.FileSystem
{
    /// <summary>
    /// Exposes methods to manage folders.
    /// </summary>
    public class FolderManager : ComponentBase<IFolderManager, FolderManager>, IFolderManager
    {
        #region Constructor

        internal FolderManager()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new folder using the provided folder path and mapping.
        /// </summary>
        /// <param name="folderMapping">The folder mapping to use.</param>
        /// <param name="folderPath">The path of the new folder.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when folderPath or folderMapping are null.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.FolderProviderException">Thrown when the underlying system throw an exception.</exception>
        /// <returns>The added folder.</returns>
        public virtual IFolderInfo AddFolder(FolderMappingInfo folderMapping, string folderPath)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("folderPath", folderPath);
            Requires.NotNull("folderMapping", folderMapping);

            try
            {
                FolderProvider.Instance(folderMapping.FolderProviderType).AddFolder(folderPath, folderMapping);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);

                throw new FolderProviderException(Localization.Localization.GetExceptionMessage("AddFolderUnderlyingSystemError", "The underlying system threw an exception. The folder has not been added."), ex);
            }

            CreateFolderInFileSystem(PathUtils.Instance.GetPhysicalPath(folderMapping.PortalID, folderPath));
            var folderId = CreateFolderInDatabase(folderMapping.PortalID, folderPath, folderMapping.FolderMappingID);

            return GetFolder(folderId);
        }

        /// <summary>
        /// Creates a new folder in the given portal using the provided folder path.
        /// The same mapping than the parent folder will be used to create this folder. So this method have to be used only to create subfolders.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <param name="folderPath">The path of the new folder.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when folderPath is null or empty.</exception>
        /// <returns>The added folder.</returns>
        public virtual IFolderInfo AddFolder(int portalID, string folderPath)
        {
            DnnLog.MethodEntry();

            Requires.NotNullOrEmpty("folderPath", folderPath);

            folderPath = PathUtils.Instance.FormatFolderPath(folderPath);

            var parentFolderPath = folderPath.Substring(0, folderPath.Substring(0, folderPath.Length - 1).LastIndexOf("/") + 1);
            var parentFolder = GetFolder(portalID, parentFolderPath) ?? AddFolder(portalID, parentFolderPath);

            var folderMapping = FolderMappingController.Instance.GetFolderMapping(parentFolder.FolderMappingID);

            return AddFolder(folderMapping, folderPath);
        }

        /// <summary>
        /// Deletes the specified folder.
        /// </summary>
        /// <param name="folder">The folder to delete.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when folder is null.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.FolderProviderException">Thrown when the underlying system throw an exception.</exception>
        public virtual void DeleteFolder(IFolderInfo folder)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("folder", folder);

            var folderMapping = FolderMappingController.Instance.GetFolderMapping(folder.FolderMappingID);

            try
            {
                FolderProvider.Instance(folderMapping.FolderProviderType).DeleteFolder(folder);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);

                throw new FolderProviderException(Localization.Localization.GetExceptionMessage("DeleteFolderUnderlyingSystemError", "The underlying system threw an exception. The folder has not been deleted."), ex);
            }

            DirectoryWrapper.Instance.Delete(folder.PhysicalPath, false);
            DeleteFolder(folder.PortalID, folder.FolderPath);
        }

        /// <summary>
        /// Deletes the specified folder.
        /// </summary>
        /// <param name="folderID">The folder identifier.</param>
        public virtual void DeleteFolder(int folderID)
        {
            DnnLog.MethodEntry();

            var folder = GetFolder(folderID);

            DeleteFolder(folder);
        }

        /// <summary>
        /// Checks the existence of the specified folder in the specified portal.
        /// </summary>
        /// <param name="portalID">The portal where to check the existence of the folder.</param>
        /// <param name="folderPath">The path of folder to check the existence of.</param>
        /// <returns>A bool value indicating whether the folder exists or not in the specified portal.</returns>
        public virtual bool FolderExists(int portalID, string folderPath)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("folderPath", folderPath);

            return GetFolder(portalID, folderPath) != null;
        }

        /// <summary>
        /// Gets the files contained in the specified folder.
        /// </summary>
        /// <param name="folder">The folder from which to retrieve the files.</param>
        /// <returns>The list of files contained in the specified folder.</returns>
        public virtual IList<IFileInfo> GetFiles(IFolderInfo folder)
        {
            DnnLog.MethodEntry();

            return GetFiles(folder, false);
        }

        /// <summary>
        /// Gets the files contained in the specified folder.
        /// </summary>
        /// <param name="folder">The folder from which to retrieve the files.</param>
        /// <param name="recursive">Whether or not to include all the subfolders</param>
        /// <returns>The list of files contained in the specified folder.</returns>
        public virtual IList<IFileInfo> GetFiles(IFolderInfo folder, bool recursive)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("folder", folder);

            var fileCollection = CBOWrapper.Instance.FillCollection<FileInfo>(DataProvider.Instance().GetFiles(folder.FolderID));

            var files = fileCollection.Cast<IFileInfo>().ToList();

            if (recursive)
            {
                foreach (var subFolder in GetFolders(folder))
                {
                    files.AddRange(GetFiles(subFolder, true));
                }
            }

            return files;
        }

        /// <summary>
        /// Gets a folder entity by providing a folder identifier.
        /// </summary>
        /// <param name="folderID">The identifier of the folder.</param>
        /// <returns>The folder entity or null if the folder cannot be located.</returns>
        public virtual IFolderInfo GetFolder(int folderID)
        {
            DnnLog.MethodEntry();

            return CBOWrapper.Instance.FillObject<FolderInfo>(DataProvider.Instance().GetFolder(folderID));
        }

        /// <summary>
        /// Gets a folder entity by providing a portal identifier and folder path.
        /// </summary>
        /// <param name="portalID">The portal where the folder exists.</param>
        /// <param name="folderPath">The path of the folder.</param>
        /// <returns>The folder entity or null if the folder cannot be located.</returns>
        public virtual IFolderInfo GetFolder(int portalID, string folderPath)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("folderPath", folderPath);

            folderPath = PathUtils.Instance.FormatFolderPath(folderPath);

            var dicFolders = GetFolders(portalID);
            return dicFolders.SingleOrDefault(f => f.FolderPath == folderPath) ?? CBOWrapper.Instance.FillObject<FolderInfo>(DataProvider.Instance().GetFolder(portalID, folderPath));
        }

        /// <summary>
        /// Gets a folder entity by providing its unique id.
        /// </summary>
        /// <param name="uniqueId">The unique id of the folder.</param>
        /// <returns>The folder entity or null if the folder cannot be located.</returns>
        public virtual IFolderInfo GetFolder(Guid uniqueId)
        {
            DnnLog.MethodEntry();

            return CBOWrapper.Instance.FillObject<FolderInfo>(DataProvider.Instance().GetFolderByUniqueID(uniqueId));
        }

        /// <summary>
        /// Gets the list of subfolders for the specified folder.
        /// </summary>
        /// <param name="parentFolder">The folder to get the list of subfolders.</param>
        /// <returns>The list of subfolders for the specified folder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when parentFolder is null.</exception>
        public virtual IList<IFolderInfo> GetFolders(IFolderInfo parentFolder)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("parentFolder", parentFolder);

            var folders = GetFolders(parentFolder.PortalID);
            var subFolders = new List<IFolderInfo>();

            foreach (var folder in folders)
            {
                var folderPath = folder.FolderPath;
                if (folderPath != Null.NullString && folderPath.Contains(parentFolder.FolderPath) && folder.FolderPath != parentFolder.FolderPath)
                {
                    if (parentFolder.FolderPath == Null.NullString)
                    {
                        if (folderPath.IndexOf("/") == folderPath.LastIndexOf("/"))
                        {
                            subFolders.Add(folder);
                        }
                    }
                    else if (folderPath.StartsWith(parentFolder.FolderPath))
                    {
                        folderPath = folderPath.Substring(parentFolder.FolderPath.Length + 1);
                        if (folderPath.IndexOf("/") == folderPath.LastIndexOf("/"))
                        {
                            subFolders.Add(folder);
                        }
                    }
                }
            }

            return subFolders;
        }

        /// <summary>
        /// Gets the sorted list of folders of the provided portal.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <returns>The sorted list of folders of the provided portal.</returns>
        public virtual IList<IFolderInfo> GetFolders(int portalID)
        {
            DnnLog.MethodEntry();

            var folders = new List<IFolderInfo>();

            var cacheKey = string.Format(DataCache.FolderCacheKey, portalID);
            CBOWrapper.Instance.GetCachedObject<List<FolderInfo>>(new CacheItemArgs(cacheKey, DataCache.FolderCacheTimeOut, DataCache.FolderCachePriority, portalID), GetFoldersSortedCallBack).ForEach(folders.Add);

            return folders;
        }

        /// <summary>
        /// Gets the sorted list of folders that match the provided permissions in the specified portal.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <param name="permissions">The permissions to match.</param>
        /// <param name="userID">The user identifier to be used to check permissions.</param>
        /// <returns>The list of folders that match the provided permissions in the specified portal.</returns>
        public virtual IList<IFolderInfo> GetFolders(int portalID, string permissions, int userID)
        {
            DnnLog.MethodEntry();

            var folders = new List<IFolderInfo>();

            var cacheKey = string.Format(DataCache.FolderCacheKey, portalID);
            var cacheItemArgs = new CacheItemArgs(cacheKey, DataCache.FolderCacheTimeOut, DataCache.FolderCachePriority, portalID, permissions, userID);
            CBOWrapper.Instance.GetCachedObject<List<FolderInfo>>(cacheItemArgs, GetFoldersByPermissionSortedCallBack).ForEach(folders.Add);

            return folders;
        }

        /// <summary>
        /// Gets the list of folders the specified user has read permissions
        /// </summary>
        /// <param name="user">The user info</param>
        /// <returns>The list of folders the specified user has read permissions.</returns>
        public virtual IList<IFolderInfo> GetFolders(UserInfo user)
        {
            DnnLog.MethodEntry();

            return GetFolders(user, "READ", true, true);
        }

        /// <summary>
        /// Gets the list of folders the specified user has the provided permissions
        /// </summary>
        /// <param name="user">The user info</param>
        /// <param name="permissions">The permissions the folders have to met.</param>
        /// <returns>The list of folders the specified user has the provided permissions.</returns>
        public virtual IList<IFolderInfo> GetFolders(UserInfo user, string permissions)
        {
            DnnLog.MethodEntry();

            return GetFolders(user, permissions, true, true);
        }

        /// <summary>
        /// Gets the list of folders the specified user has the provided permissions, including or not, secure and database folder types.
        /// </summary>
        /// <param name="user">The user info</param>
        /// <param name="permissions">The permissions the folders have to met.</param>
        /// <param name="includeSecure">Indicates if the result should include secure folders.</param>
        /// <param name="includeDatabase">Indicates if the result should include database folders.</param>
        /// <returns>The list of folders the specified user has the provided permissions, including or not, secure and database folder types.</returns>
        public virtual IList<IFolderInfo> GetFolders(UserInfo user, string permissions, bool includeSecure, bool includeDatabase)
        {
            DnnLog.MethodEntry();

            var userFolders = new List<IFolderInfo>();

            var portalID = user.PortalID;
            var userFolderPath = PathUtils.Instance.GetUserFolderPath(user);

            var userFolder = GetFolder(portalID, userFolderPath);
            if (userFolder == null)
            {
                AddUserFolder(user);
                userFolder = GetFolder(portalID, userFolderPath);
            }

            var databaseFolderMapping = FolderMappingController.Instance.GetFolderMapping(portalID, "Database");
            var secureFolderMapping = FolderMappingController.Instance.GetFolderMapping(portalID, "Secure");

            foreach (var folder in GetFolders(portalID, permissions, user.UserID))
            {
                if (folder.FolderPath == null)
                {
                    continue;
                }

                if (!includeDatabase && folder.FolderMappingID == databaseFolderMapping.FolderMappingID)
                {
                    continue;
                }

                if (!includeSecure && folder.FolderMappingID == secureFolderMapping.FolderMappingID)
                {
                    continue;
                }

                if (folder.FolderPath.StartsWith("users/", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (folder.FolderID == userFolder.FolderID)
                    {
                        folder.DisplayPath = "My Folder/";
                        folder.DisplayName = "My Folder";
                    }
                    else
                    {
                        continue;
                    }
                }

                userFolders.Add(folder);
            }

            return userFolders;
        }

        /// <summary>
        /// Renames the specified folder by setting the new provided folder name.
        /// </summary>
        /// <param name="folder">The folder to rename.</param>
        /// <param name="newFolderName">The new name to apply to the folder.</param>
        /// <exception cref="System.ArgumentException">Thrown when newFolderName is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when folder is null.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.FolderProviderException">Thrown when the underlying system throw an exception.</exception>
        public virtual void RenameFolder(IFolderInfo folder, string newFolderName)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("newFolderName", newFolderName);

            if (folder.FolderName.Equals(newFolderName)) return;
            
            var folderMapping = FolderMappingController.Instance.GetFolderMapping(folder.FolderMappingID);

            if (folderMapping == null) return;

            if (folderMapping.IsEditable)
            {
                try
                {
                    FolderProvider.Instance(folderMapping.FolderProviderType).RenameFolder(folder, newFolderName);
                }
                catch (Exception ex)
                {
                    DnnLog.Error(ex);

                    throw new FolderProviderException(
                        Localization.Localization.GetExceptionMessage("RenameFolderUnderlyingSystemError", "The underlying system threw an exception. The folder has not been renamed."), ex);
                }
            }

            var newFolderPath = folder.FolderPath.Substring(0, folder.FolderPath.LastIndexOf(folder.FolderName)) + PathUtils.Instance.FormatFolderPath(newFolderName);

            RenameFolderInFileSystem(folder, newFolderPath);
            RenameFolderInDatabase(folder, newFolderPath);
        }

        /// <summary>
        /// Synchronizes the entire folder tree for the specified portal.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <returns>The number of folder collisions.</returns>
        public virtual int Synchronize(int portalID)
        {
            DnnLog.MethodEntry();

            var folderCollisions = Synchronize(portalID, "", true, true);

            DataCache.ClearFolderCache(portalID);

            return folderCollisions;
        }

        /// <summary>
        /// Syncrhonizes the specified folder, its files and its subfolders.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <param name="relativePath">The relative path of the folder.</param>
        /// <returns>The number of folder collisions.</returns>
        public virtual int Synchronize(int portalID, string relativePath)
        {
            DnnLog.MethodEntry();

            return Synchronize(portalID, relativePath, true, true);
        }

        /// <summary>
        /// Syncrhonizes the specified folder, its files and, optionally, its subfolders.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <param name="relativePath">The relative path of the folder.</param>
        /// <param name="isRecursive">Indicates if the synchronization has to be recursive.</param>
        /// <param name="syncFiles">Indicates if files need to be synchronized.</param>
        /// <returns>The number of folder collisions.</returns>
        public virtual int Synchronize(int portalID, string relativePath, bool isRecursive, bool syncFiles)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("relativePath", relativePath);

            var collisionNotifications = new List<string>();
            var mergedTree = GetMergedTree(portalID, relativePath, isRecursive);

            for (var i = mergedTree.Count - 1; i >= 0; i--)
            {
                var item = mergedTree.Values[i];

                var collisionNotification = ProcessMergedTreeItem(item, i, mergedTree, portalID);

                if (collisionNotification != null)
                {
                    collisionNotifications.Add(collisionNotification);
                }

                if (syncFiles && NeedsSynchronization(item, mergedTree))
                {
                    SynchronizeFiles(item, portalID);
                }
            }

            if (collisionNotifications.Count > 0)
            {
                LogCollisions(portalID, collisionNotifications);
            }

            return collisionNotifications.Count;
        }

        /// <summary>
        /// Updates metadata of the specified folder.
        /// </summary>
        /// <param name="folder">The folder to update.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when folder is null.</exception>
        public virtual IFolderInfo UpdateFolder(IFolderInfo folder)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("folder", folder);

            DataProvider.Instance().UpdateFolder(folder.PortalID,
                                                    folder.VersionGuid,
                                                    folder.FolderID,
                                                    PathUtils.Instance.FormatFolderPath(folder.FolderPath),
                                                    folder.StorageLocation,
                                                    folder.IsProtected,
                                                    folder.IsCached,
                                                    folder.LastUpdated,
                                                    GetCurrentUserID(),
                                                    folder.FolderMappingID);

            AddLogEntry(folder, EventLogController.EventLogType.FOLDER_UPDATED);

            ClearFolderCache(folder.PortalID);

            SaveFolderPermissions(folder);

            return folder;
        }

        #endregion

        #region Permission Methods

        /// <summary>
        /// Adds read permissions for all users to the specified folder.
        /// </summary>
        /// <param name="folder">The folder to add the permission to.</param>
        /// <param name="permission">Used as base class for FolderPermissionInfo when there is no read permission already defined.</param>
        public virtual void AddAllUserReadPermission(IFolderInfo folder, PermissionInfo permission)
        {
            var roleId = Int32.Parse(Globals.glbRoleAllUsers);

            var folderPermission =
                (from FolderPermissionInfo p in folder.FolderPermissions
                 where p.PermissionKey == "READ" && p.FolderID == folder.FolderID && p.RoleID == roleId && p.UserID == Null.NullInteger
                 select p).SingleOrDefault();

            if (folderPermission != null)
            {
                folderPermission.AllowAccess = true;
            }
            else
            {
                folderPermission = new FolderPermissionInfo(permission)
                                       {
                                           FolderID = folder.FolderID,
                                           UserID = Null.NullInteger,
                                           RoleID = roleId,
                                           AllowAccess = true
                                       };

                folder.FolderPermissions.Add(folderPermission);
            }
        }

        /// <summary>
        /// Sets specific folder permissions for the given role to the given folder.
        /// </summary>
        public virtual void SetFolderPermission(IFolderInfo folder, int permissionId, int roleId)
        {
            SetFolderPermission(folder, permissionId, roleId, Null.NullInteger);
        }

        /// <summary>
        /// Sets specific folder permissions for the given role/user to the given folder.
        /// </summary>
        public virtual void SetFolderPermission(IFolderInfo folder, int permissionId, int roleId, int userId)
        {
            if (folder.FolderPermissions.Cast<FolderPermissionInfo>()
                .Any(fpi => fpi.FolderID == folder.FolderID && fpi.PermissionID == permissionId && fpi.RoleID == roleId && fpi.UserID == userId && fpi.AllowAccess))
            {
                return;
            }

            var objFolderPermissionInfo = new FolderPermissionInfo
            {
                FolderID = folder.FolderID,
                PermissionID = permissionId,
                RoleID = roleId,
                UserID = userId,
                AllowAccess = true
            };

            folder.FolderPermissions.Add(objFolderPermissionInfo, true);
            FolderPermissionController.SaveFolderPermissions((FolderInfo)folder);
        }

        /// <summary>
        /// Sets folder permissions to the given folder by copying parent folder permissions.
        /// </summary>
        public virtual void SetFolderPermissions(IFolderInfo folder)
        {
            Requires.NotNull("folder", folder);

            if (String.IsNullOrEmpty(folder.FolderPath)) return;
            
            var parentFolderPath = folder.FolderPath.Substring(0, folder.FolderPath.Substring(0, folder.FolderPath.Length - 1).LastIndexOf("/") + 1);

            foreach (FolderPermissionInfo objPermission in
                FolderPermissionController.GetFolderPermissionsCollectionByFolder(folder.PortalID, parentFolderPath))
            {
                var folderPermission = new FolderPermissionInfo(objPermission)
                                           {
                                               FolderID = folder.FolderID,
                                               RoleID = objPermission.RoleID,
                                               UserID = objPermission.UserID,
                                               AllowAccess = objPermission.AllowAccess
                                           };
                folder.FolderPermissions.Add(folderPermission, true);
            }

            FolderPermissionController.SaveFolderPermissions((FolderInfo)folder);
        }

        /// <summary>
        /// Sets folder permissions for administrator role to the given folder.
        /// </summary>
        public virtual void SetFolderPermissions(IFolderInfo folder, int administratorRoleId)
        {
            Requires.NotNull("folder", folder);

            foreach (PermissionInfo objPermission in PermissionController.GetPermissionsByFolder())
            {
                var folderPermission = new FolderPermissionInfo(objPermission)
                                           {
                                               FolderID = folder.FolderID,
                                               RoleID = administratorRoleId
                                           };

                folder.FolderPermissions.Add(folderPermission, true);
            }

            FolderPermissionController.SaveFolderPermissions((FolderInfo)folder);
        }

        #endregion

        #region Internal Methods

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual int AddFolder(IFolderInfo folder)
        {
            //Check this is not a duplicate
            var tmpfolder = GetFolder(folder.PortalID, folder.FolderPath);

            if (tmpfolder != null && folder.FolderID == Null.NullInteger)
            {
                folder.FolderID = tmpfolder.FolderID;
            }

            if (folder.FolderID == Null.NullInteger)
            {
                folder.FolderPath = PathUtils.Instance.FormatFolderPath(folder.FolderPath);
                folder.FolderID = DataProvider.Instance().AddFolder(folder.PortalID,
                                                                    folder.UniqueId,
                                                                    folder.VersionGuid,
                                                                    folder.FolderPath,
                                                                    folder.StorageLocation,
                                                                    folder.IsProtected,
                                                                    folder.IsCached,
                                                                    folder.LastUpdated,
                                                                    GetCurrentUserID(),
                                                                    folder.FolderMappingID);

                //Refetch folder for logging
                folder = GetFolder(folder.PortalID, folder.FolderPath);

                var objEventLog = new EventLogController();
                objEventLog.AddLog(folder, PortalController.GetCurrentPortalSettings(), GetCurrentUserID(), "", EventLogController.EventLogType.FOLDER_CREATED);
                UpdateParentFolder(folder.PortalID, folder.FolderPath);
            }
            else
            {
                DataProvider.Instance().UpdateFolder(folder.PortalID,
                                                     folder.VersionGuid,
                                                     folder.FolderID,
                                                     folder.FolderPath,
                                                     folder.StorageLocation,
                                                     folder.IsProtected,
                                                     folder.IsCached,
                                                     folder.LastUpdated,
                                                     GetCurrentUserID(),
                                                     folder.FolderMappingID);

                var objEventLog = new EventLogController();
                objEventLog.AddLog("FolderPath",
                                   folder.FolderPath,
                                   PortalController.GetCurrentPortalSettings(),
                                   GetCurrentUserID(),
                                   EventLogController.EventLogType.FOLDER_UPDATED);
            }

            //Invalidate Cache
            ClearFolderCache(folder.PortalID);

            return folder.FolderID;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void AddLogEntry(IFolderInfo folder, EventLogController.EventLogType eventLogType)
        {
            var objEventLog = new EventLogController();
            objEventLog.AddLog(folder, PortalController.GetCurrentPortalSettings(), GetCurrentUserID(), "", eventLogType);
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual IFolderInfo AddUserFolder(UserInfo user)
        {
            var portalID = user.PortalID;

            var defaultFolderMapping = FolderMappingController.Instance.GetDefaultFolderMapping(portalID);

            var rootFolder = PathUtils.Instance.GetUserFolderPathElement(user.UserID, PathUtils.UserFolderElement.Root);

            var folderPath = PathUtils.Instance.FormatFolderPath(String.Format("Users/{0}", rootFolder));

            if (!FolderExists(portalID, folderPath))
            {
                AddFolder(defaultFolderMapping, folderPath);
            }

            folderPath = PathUtils.Instance.FormatFolderPath(String.Concat(folderPath, PathUtils.Instance.GetUserFolderPathElement(user.UserID, PathUtils.UserFolderElement.SubFolder)));

            if (!FolderExists(portalID, folderPath))
            {
                AddFolder(defaultFolderMapping, folderPath);
            }

            folderPath = PathUtils.Instance.FormatFolderPath(String.Concat(folderPath, user.UserID.ToString()));

            if (!FolderExists(portalID, folderPath))
            {
                AddFolder(defaultFolderMapping, folderPath);

                var folder = GetFolder(portalID, folderPath);

                foreach (PermissionInfo permission in PermissionController.GetPermissionsByFolder())
                {
                    if (permission.PermissionKey.ToUpper() == "READ" || permission.PermissionKey.ToUpper() == "WRITE" || permission.PermissionKey.ToUpper() == "BROWSE")
                    {
                        var folderPermission = new FolderPermissionInfo(permission)
                        {
                            FolderID = folder.FolderID,
                            UserID = user.UserID,
                            RoleID = Null.NullInteger,
                            AllowAccess = true
                        };

                        folder.FolderPermissions.Add(folderPermission);

                        if (permission.PermissionKey.ToUpper() == "READ")
                        {
                            AddAllUserReadPermission(folder, permission);
                        }
                    }
                }

                FolderPermissionController.SaveFolderPermissions((FolderInfo)folder);
            }

            return GetFolder(portalID, folderPath);
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void ClearFolderCache(int portalId)
        {
            DataCache.ClearFolderCache(portalId);
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual int CreateFolderInDatabase(int portalID, string folderPath, int folderMappingID)
        {
            var isProtected = PathUtils.Instance.IsDefaultProtectedPath(folderPath);
            var folderMapping = FolderMappingController.Instance.GetFolderMapping(folderMappingID);
            var storageLocation = (int)FolderController.StorageLocationTypes.DatabaseSecure;
            if (!folderMapping.IsEditable)
            {
                switch (folderMapping.MappingName)
                {
                    case "Standard":
                        storageLocation = (int)FolderController.StorageLocationTypes.InsecureFileSystem;
                        break;
                    case "Secure":
                        storageLocation = (int)FolderController.StorageLocationTypes.SecureFileSystem;
                        break;
                    default:
                        storageLocation = (int)FolderController.StorageLocationTypes.DatabaseSecure;
                        break;
                }
            }
            var folder = new FolderInfo(portalID, folderPath, storageLocation, isProtected, false, Null.NullDate) { FolderMappingID = folderMappingID };
            folder.FolderID = AddFolder(folder);

            if (portalID != Null.NullInteger)
            {
                //Set Folder Permissions to inherit from parent
                SetFolderPermissions(folder);
            }

            return folder.FolderID;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void CreateFolderInFileSystem(string physicalPath)
        {
            var di = new DirectoryInfo(physicalPath);

            if (!di.Exists)
            {
                di.Create();
            }
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void DeleteFolder(int portalID, string folderPath)
        {
            DataProvider.Instance().DeleteFolder(portalID, PathUtils.Instance.FormatFolderPath(folderPath));
            var objEventLog = new EventLogController();
            objEventLog.AddLog("FolderPath", folderPath, PortalController.GetCurrentPortalSettings(), GetCurrentUserID(), EventLogController.EventLogType.FOLDER_DELETED);
            UpdateParentFolder(portalID, folderPath);
            DataCache.ClearFolderCache(portalID);
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual int GetCurrentUserID()
        {
            return UserController.GetCurrentUserInfo().UserID;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual SortedList<string, MergedTreeItem> GetDatabaseFolders(int portalID, string relativePath, bool isRecursive)
        {
            var databaseFolders = new SortedList<string, MergedTreeItem>();

            var folder = GetFolder(portalID, relativePath);

            if (folder != null)
            {
                if (!isRecursive)
                {
                    var item = new MergedTreeItem
                                   {
                                       FolderPath = relativePath,
                                       ExistsInDatabase = true,
                                       FolderMappingID = folder.FolderMappingID
                                   };

                    databaseFolders.Add(relativePath, item);
                }
                else
                {
                    databaseFolders = GetDatabaseFoldersRecursive(folder);
                }
            }

            return databaseFolders;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual SortedList<string, MergedTreeItem> GetDatabaseFoldersRecursive(IFolderInfo folder)
        {
            var result = new SortedList<string, MergedTreeItem>();
            var stack = new Stack<IFolderInfo>();

            stack.Push(folder);

            while (stack.Count > 0)
            {
                var folderInfo = stack.Pop();

                var item = new MergedTreeItem
                               {
                                   FolderPath = folderInfo.FolderPath,
                                   ExistsInDatabase = true,
                                   FolderMappingID = folderInfo.FolderMappingID
                               };

                result.Add(item.FolderPath, item);

                foreach (var subfolder in GetFolders(folderInfo))
                {
                    stack.Push(subfolder);
                }
            }

            return result;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual SortedList<string, MergedTreeItem> GetFileSystemFolders(int portalID, string relativePath, bool isRecursive)
        {
            var fileSystemFolders = new SortedList<string, MergedTreeItem>();

            var physicalPath = PathUtils.Instance.GetPhysicalPath(portalID, relativePath);

            if (DirectoryWrapper.Instance.Exists(physicalPath))
            {
                if (!isRecursive)
                {
                    var item = new MergedTreeItem
                                   {
                                       FolderPath = relativePath,
                                       ExistsInFileSystem = true
                                   };

                    fileSystemFolders.Add(relativePath, item);
                }
                else
                {
                    fileSystemFolders = GetFileSystemFoldersRecursive(portalID, physicalPath);
                }
            }

            return fileSystemFolders;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual SortedList<string, MergedTreeItem> GetFileSystemFoldersRecursive(int portalID, string physicalPath)
        {
            var result = new SortedList<string, MergedTreeItem>();
            var stack = new Stack<string>();

            stack.Push(physicalPath);

            while (stack.Count > 0)
            {
                var dir = stack.Pop();

                try
                {
                    var item = new MergedTreeItem
                                   {
                                       FolderPath = PathUtils.Instance.GetRelativePath(portalID, dir),
                                       ExistsInFileSystem = true
                                   };

                    result.Add(item.FolderPath, item);

                    foreach (var dn in DirectoryWrapper.Instance.GetDirectories(dir))
                    {
                        stack.Push(dn);
                    }
                }
                catch (Exception ex)
                {
                    DnnLog.Error(ex);
                }
            }

            return result;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual SortedList<string, MergedTreeItem> GetFolderMappingFolders(FolderMappingInfo folderMapping, string relativePath, bool isRecursive)
        {
            var folderMappingFolders = new SortedList<string, MergedTreeItem>();
            var folderProvider = FolderProvider.Instance(folderMapping.FolderProviderType);

            if (folderProvider.ExistsFolder(relativePath, folderMapping))
            {
                if (!isRecursive)
                {
                    var item = new MergedTreeItem { FolderPath = relativePath };

                    item.ExistsInFolderMappings.Add(folderMapping.FolderMappingID);

                    folderMappingFolders.Add(relativePath, item);
                }
                else
                {
                    folderMappingFolders = GetFolderMappingFoldersRecursive(folderMapping, relativePath);
                }
            }

            return folderMappingFolders;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual SortedList<string, MergedTreeItem> GetFolderMappingFoldersRecursive(FolderMappingInfo folderMapping, string relativePath)
        {
            var result = new SortedList<string, MergedTreeItem>();
            var stack = new Stack<string>();
            var folderProvider = FolderProvider.Instance(folderMapping.FolderProviderType);

            stack.Push(relativePath);

            while (stack.Count > 0)
            {
                var folderPath = stack.Pop();

                try
                {
                    var item = new MergedTreeItem { FolderPath = folderPath };

                    item.ExistsInFolderMappings.Add(folderMapping.FolderMappingID);

                    result.Add(item.FolderPath, item);

                    foreach (var subfolderPath in folderProvider.GetSubFolders(folderPath, folderMapping))
                    {
                        stack.Push(subfolderPath);
                    }
                }
                catch (Exception ex)
                {
                    DnnLog.Error(ex);
                }
            }

            return result;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual FolderMappingInfo GetFolderMappingWithHighestPriority(List<int> folderMappingIDs)
        {
            FolderMappingInfo folderMappingWithHighestPriority = null;

            foreach (var folderMappingID in folderMappingIDs)
            {
                var folderMapping = FolderMappingController.Instance.GetFolderMapping(folderMappingID);

                // Lower value in Priority property means higher priority
                if (folderMappingWithHighestPriority == null || folderMapping.Priority < folderMappingWithHighestPriority.Priority)
                {
                    folderMappingWithHighestPriority = folderMapping;
                }
            }

            return folderMappingWithHighestPriority;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual object GetFoldersByPermissionSortedCallBack(CacheItemArgs cacheItemArgs)
        {
            var portalID = (int)cacheItemArgs.ParamList[0];
            var permissions = (string)cacheItemArgs.ParamList[1];
            var userID = (int)cacheItemArgs.ParamList[2];
            return CBOWrapper.Instance.FillCollection<FolderInfo>(DataProvider.Instance().GetFoldersByPortalAndPermissions(portalID, permissions, userID));
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual object GetFoldersSortedCallBack(CacheItemArgs cacheItemArgs)
        {
            var portalID = (int)cacheItemArgs.ParamList[0];
            return CBOWrapper.Instance.FillCollection<FolderInfo>(DataProvider.Instance().GetFoldersByPortal(portalID));
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual SortedList<string, MergedTreeItem> GetMergedTree(int portalID, string relativePath, bool isRecursive)
        {
            var fileSystemFolders = GetFileSystemFolders(portalID, relativePath, isRecursive);
            var databaseFolders = GetDatabaseFolders(portalID, relativePath, isRecursive);

            var mergedTree = MergeFolderLists(fileSystemFolders, databaseFolders);

            return (from folderMapping in FolderMappingController.Instance.GetFolderMappings(portalID)
                    where folderMapping.IsEditable
                    select GetFolderMappingFolders(folderMapping, relativePath, isRecursive)).Aggregate(mergedTree, MergeFolderLists);
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void LogCollisions(int portalID, List<string> collisionNotifications)
        {
            var eventLogController = new EventLogController();
            var eventLogInfo = new LogInfo
                                   {
                                       BypassBuffering = true,
                                       LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString(),
                                       LogPortalID = portalID
                                   };

            eventLogInfo.AddProperty("Folder Synchronization", "Collisions detected");
            eventLogInfo.AddProperty("Collisions", String.Join("<br />", collisionNotifications.ToArray()));

            eventLogController.AddLog(eventLogInfo);
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual SortedList<string, MergedTreeItem> MergeFolderLists(SortedList<string, MergedTreeItem> list1, SortedList<string, MergedTreeItem> list2)
        {
            foreach (var item in list2.Values)
            {
                if (list1.ContainsKey(item.FolderPath))
                {
                    var existingItem = list1[item.FolderPath];

                    existingItem.ExistsInFileSystem = existingItem.ExistsInFileSystem || item.ExistsInFileSystem;
                    existingItem.ExistsInDatabase = existingItem.ExistsInDatabase || item.ExistsInDatabase;
                    existingItem.ExistsInFolderMappings.AddRange(item.ExistsInFolderMappings);

                    if (item.FolderMappingID != 0)
                    {
                        existingItem.FolderMappingID = item.FolderMappingID;
                    }
                }
                else
                {
                    list1.Add(item.FolderPath, item);
                }
            }

            return list1;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        /// <remarks>
        /// Return true for now but we need a way to know if a folder is in sync
        /// </remarks>
        internal virtual bool NeedsSynchronization(MergedTreeItem item, SortedList<string, MergedTreeItem> mergedTree)
        {
            return true;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual string ProcessMergedTreeItem(MergedTreeItem item, int itemIndex, SortedList<string, MergedTreeItem> mergedTree, int portalID)
        {
            const string collisionNotification = "Collision on path '{0}'. Resolved using '{1}' folder mapping.";

            if (item.ExistsInFileSystem)
            {
                if (item.ExistsInDatabase)
                {
                    var folderMapping = FolderMappingController.Instance.GetFolderMapping(item.FolderMappingID);

                    if (item.ExistsInFolderMappings.Count == 0)
                    {
                        if (folderMapping.IsEditable)
                        {
                            if (itemIndex < (mergedTree.Count - 1) && mergedTree.Values[itemIndex + 1].FolderPath.StartsWith(item.FolderPath))
                            {
                                UpdateFolderMappingID(portalID, item.FolderPath, FolderMappingController.Instance.GetDefaultFolderMapping(portalID).FolderMappingID);
                            }
                            else
                            {
                                DirectoryWrapper.Instance.Delete(PathUtils.Instance.GetPhysicalPath(portalID, item.FolderPath), false);
                                DeleteFolder(portalID, item.FolderPath);
                            }
                        }
                    }
                    else
                    {
                        var newFolderMapping = folderMapping;

                        if (!folderMapping.IsEditable)
                        {
                            var folder = GetFolder(portalID, item.FolderPath);

                            if (FolderProvider.Instance(folderMapping.FolderProviderType).GetFiles(folder).Length == 0)
                            {
                                newFolderMapping = GetFolderMappingWithHighestPriority(item.ExistsInFolderMappings);
                                UpdateFolderMappingID(portalID, item.FolderPath, newFolderMapping.FolderMappingID);
                            }

                            return string.Format(collisionNotification, item.FolderPath, newFolderMapping.MappingName);
                        }

                        newFolderMapping = GetFolderMappingWithHighestPriority(item.ExistsInFolderMappings);

                        if (item.FolderMappingID != newFolderMapping.FolderMappingID)
                        {
                            UpdateFolderMappingID(portalID, item.FolderPath, newFolderMapping.FolderMappingID);

                            return string.Format(collisionNotification, item.FolderPath, newFolderMapping.MappingName);
                        }

                        if (item.ExistsInFolderMappings.Count > 1)
                        {
                            return string.Format(collisionNotification, item.FolderPath, newFolderMapping.MappingName);
                        }
                    }
                }
                else
                {
                    if (item.ExistsInFolderMappings.Count == 0)
                    {
                        CreateFolderInDatabase(portalID, item.FolderPath, FolderMappingController.Instance.GetDefaultFolderMapping(portalID).FolderMappingID);
                    }
                    else
                    {
                        var newFolderMapping = DirectoryWrapper.Instance.GetFiles(PathUtils.Instance.GetPhysicalPath(portalID, item.FolderPath)).Length > 0 ?
                            FolderMappingController.Instance.GetDefaultFolderMapping(portalID) : GetFolderMappingWithHighestPriority(item.ExistsInFolderMappings);

                        CreateFolderInDatabase(portalID, item.FolderPath, newFolderMapping.FolderMappingID);

                        return string.Format(collisionNotification, item.FolderPath, newFolderMapping.MappingName);
                    }
                }
            }
            else
            {
                if (item.ExistsInDatabase)
                {
                    var folderMapping = FolderMappingController.Instance.GetFolderMapping(item.FolderMappingID);

                    if (item.ExistsInFolderMappings.Count == 0)
                    {
                        if (!folderMapping.IsEditable)
                        {
                            CreateFolderInFileSystem(PathUtils.Instance.GetPhysicalPath(portalID, item.FolderPath));
                        }
                        else
                        {
                            if (itemIndex < (mergedTree.Count - 1) && mergedTree.Values[itemIndex + 1].FolderPath.StartsWith(item.FolderPath))
                            {
                                CreateFolderInFileSystem(PathUtils.Instance.GetPhysicalPath(portalID, item.FolderPath));
                                UpdateFolderMappingID(portalID, item.FolderPath, FolderMappingController.Instance.GetDefaultFolderMapping(portalID).FolderMappingID);
                            }
                            else
                            {
                                DeleteFolder(portalID, item.FolderPath);
                            }
                        }
                    }
                    else
                    {
                        CreateFolderInFileSystem(PathUtils.Instance.GetPhysicalPath(portalID, item.FolderPath));

                        if (!folderMapping.IsEditable)
                        {
                            if (folderMapping.FolderProviderType != "DatabaseFolderProvider")
                            {
                                if (item.ExistsInFolderMappings.Count == 1)
                                {
                                    UpdateFolderMappingID(portalID, item.FolderPath, item.ExistsInFolderMappings[0]);
                                }
                                else
                                {
                                    var newFolderMapping = GetFolderMappingWithHighestPriority(item.ExistsInFolderMappings);

                                    UpdateFolderMappingID(portalID, item.FolderPath, newFolderMapping.FolderMappingID);

                                    return string.Format(collisionNotification, item.FolderPath, newFolderMapping.MappingName);
                                }
                            }
                            else
                            {
                                var newFolderMapping = folderMapping;
                                var folder = GetFolder(portalID, item.FolderPath);

                                if (GetFiles(folder).Count == 0)
                                {
                                    newFolderMapping = GetFolderMappingWithHighestPriority(item.ExistsInFolderMappings);
                                    UpdateFolderMappingID(portalID, item.FolderPath, newFolderMapping.FolderMappingID);
                                }

                                return string.Format(collisionNotification, item.FolderPath, newFolderMapping.MappingName);
                            }
                        }
                        else
                        {
                            var newFolderMapping = GetFolderMappingWithHighestPriority(item.ExistsInFolderMappings);

                            if (item.FolderMappingID != newFolderMapping.FolderMappingID)
                            {
                                UpdateFolderMappingID(portalID, item.FolderPath, newFolderMapping.FolderMappingID);
                            }

                            if (item.ExistsInFolderMappings.Count > 1)
                            {
                                return string.Format(collisionNotification, item.FolderPath, newFolderMapping.MappingName);
                            }
                        }
                    }
                }
                else
                {
                    CreateFolderInFileSystem(PathUtils.Instance.GetPhysicalPath(portalID, item.FolderPath));

                    var newFolderMapping = GetFolderMappingWithHighestPriority(item.ExistsInFolderMappings);

                    CreateFolderInDatabase(portalID, item.FolderPath, newFolderMapping.FolderMappingID);

                    if (item.ExistsInFolderMappings.Count > 1)
                    {
                        return string.Format(collisionNotification, item.FolderPath, newFolderMapping.MappingName);
                    }
                }
            }

            return null;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void RemoveOrphanedFiles(IFolderInfo folder)
        {
            var files = GetFiles(folder);

            var folderMapping = FolderMappingController.Instance.GetFolderMapping(folder.FolderMappingID);

            if (folderMapping != null)
            {
                var folderProvider = FolderProvider.Instance(folderMapping.FolderProviderType);

                foreach (var file in files)
                {
                    try
                    {
                        if (!folderProvider.ExistsFile(folder, file.FileName))
                        {
                            FileManager.Instance.DeleteFile(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        DnnLog.Error(ex);
                    }
                }
            }
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void RenameFiles(IFolderInfo folder, string newFolderPath)
        {
            var files = GetFiles(folder);

            foreach (var file in files)
            {
                file.Folder = newFolderPath;
                FileManager.Instance.UpdateFile(file);
            }
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void RenameFolderInDatabase(IFolderInfo folder, string newFolderPath)
        {
            RenameSubfolders(folder, newFolderPath);

            folder.FolderPath = newFolderPath;
            UpdateFolder(folder);
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void RenameFolderInFileSystem(IFolderInfo folder, string newFolderPath)
        {
            var di = new DirectoryInfo(folder.PhysicalPath);

            if (di.Exists)
            {
                di.MoveTo(newFolderPath);
            }
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void RenameSubfolders(IFolderInfo folder, string newFolderPath)
        {
            RenameFiles(folder, newFolderPath);

            var folders = GetFolders(folder.PortalID);

            foreach (var portalFolder in folders)
            {
                var portalFolderPath = portalFolder.FolderPath;

                if (!String.IsNullOrEmpty(portalFolderPath) &&
                    portalFolderPath.StartsWith(folder.FolderPath) &&
                    portalFolderPath != folder.FolderPath)
                {
                    portalFolderPath = portalFolderPath.Substring(folder.FolderPath.Length + 1);
                    portalFolder.FolderPath = newFolderPath + portalFolderPath;

                    UpdateFolder(portalFolder);

                    RenameFiles(portalFolder, portalFolder.FolderPath);
                }
            }
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void SaveFolderPermissions(IFolderInfo folder)
        {
            FolderPermissionController.SaveFolderPermissions((FolderInfo)folder);
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void SynchronizeFiles(MergedTreeItem item, int portalID)
        {
            var folder = GetFolder(portalID, item.FolderPath);

            if (folder == null)
            {
                return;
            }

            var folderMapping = FolderMappingController.Instance.GetFolderMapping(folder.FolderMappingID);

            if (folderMapping == null)
            {
                return;
            }

            try
            {
                var folderProvider = FolderProvider.Instance(folderMapping.FolderProviderType);
                var fileManager = FileManager.Instance;
                var files = folderProvider.GetFiles(folder);

                foreach (var fileName in files)
                {
                    try
                    {
                        var file = fileManager.GetFile(folder, fileName);

                        if (file == null)
                        {
                            using (var fileContent = folderProvider.GetFileStream(folder, fileName))
                            {
                                fileManager.AddFile(folder, fileName, fileContent, false);
                            }
                        }
                        else if (!folderProvider.IsInSync(file))
                        {
                            using (var fileContent = fileManager.GetFileContent(file))
                            {
                                fileManager.UpdateFile(file, fileContent);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DnnLog.Error(ex);
                    }
                }

                RemoveOrphanedFiles(folder);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
            }
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void UpdateParentFolder(int portalID, string folderPath)
        {
            if (!String.IsNullOrEmpty(folderPath))
            {
                var parentFolderPath = folderPath.Substring(0, folderPath.Substring(0, folderPath.Length - 1).LastIndexOf("/") + 1);
                var objFolder = GetFolder(portalID, parentFolderPath);
                if (objFolder != null)
                {
                    UpdateFolder(objFolder);
                }
            }
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void UpdateFolderMappingID(int portalID, string folderPath, int folderMappingID)
        {
            var folder = GetFolder(portalID, folderPath);
            folder.FolderMappingID = folderMappingID;
            UpdateFolder(folder);
        }

        #endregion

        #region Internal Classes

        /// <summary>
        /// This class and its members are reserved for internal use and are not intended to be used in your code.
        /// </summary>
        internal class MergedTreeItem
        {
            public string FolderPath { get; set; }
            public bool ExistsInFileSystem { get; set; }
            public bool ExistsInDatabase { get; set; }
            public List<int> ExistsInFolderMappings { get; set; }
            public int FolderMappingID { get; set; }

            public MergedTreeItem()
            {
                ExistsInFolderMappings = new List<int>();
            }
        }

        #endregion
    }
}
