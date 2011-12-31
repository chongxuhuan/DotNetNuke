﻿#region Copyright

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
using System.ComponentModel;

using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;

namespace DotNetNuke.Services.FileSystem
{
    /// <summary>
    /// This interface has been created only with testing purposes. Do not implement it in your code as it is subject to change.
    /// </summary>
    public interface IFolderManager
    {
        /// <summary>
        /// Adds read permissions for all users to the specified folder.
        /// </summary>
        /// <param name="folder">The folder to add the permission to.</param>
        /// <param name="permission">Used as base class for FolderPermissionInfo when there is no read permission already defined.</param>
        void AddAllUserReadPermission(IFolderInfo folder, PermissionInfo permission);

        /// <summary>
        /// Creates a new folder using the provided folder path and mapping.
        /// </summary>
        /// <param name="folderMapping">The folder mapping to use.</param>
        /// <param name="folderPath">The path of the new folder.</param>
        /// <returns>The added folder.</returns>
        IFolderInfo AddFolder(FolderMappingInfo folderMapping, string folderPath);

        /// <summary>
        /// Creates a new folder in the given portal using the provided folder path.
        /// The same mapping than the parent folder will be used to create this folder. So this method have to be used only to create subfolders.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <param name="folderPath">The path of the new folder.</param>
        /// <returns>The added folder.</returns>
        IFolderInfo AddFolder(int portalID, string folderPath);

        /// <summary>
        /// Sets folder permissions to the given folder by copying parent folder permissions.
        /// </summary>
        void CopyParentFolderPermissions(IFolderInfo folder);

        /// <summary>
        /// Deletes the specified folder.
        /// </summary>
        /// <param name="folder">The folder to delete.</param>
        void DeleteFolder(IFolderInfo folder);

        /// <summary>
        /// Deletes the specified folder.
        /// </summary>
        /// <param name="folderID">The folder identifier.</param>
        void DeleteFolder(int folderID);

        /// <summary>
        /// Checks the existence of the specified folder in the specified portal.
        /// </summary>
        /// <param name="portalID">The portal where to check the existence of the folder.</param>
        /// <param name="folderPath">The path of folder to check the existence of.</param>
        /// <returns>A bool value indicating whether the folder exists or not in the specified portal.</returns>
        bool FolderExists(int portalID, string folderPath);

        /// <summary>
        /// Gets the files contained in the specified folder.
        /// </summary>
        /// <param name="folder">The folder from which to retrieve the files.</param>
        /// <returns>The list of files contained in the specified folder.</returns>
        IEnumerable<IFileInfo> GetFiles(IFolderInfo folder);

        /// <summary>
        /// Gets the files contained in the specified folder.
        /// </summary>
        /// <param name="folder">The folder from which to retrieve the files.</param>
        /// <param name="recursive">Whether or not to include all the subfolders</param>
        /// <returns>The list of files contained in the specified folder.</returns>
        IEnumerable<IFileInfo> GetFiles(IFolderInfo folder, bool recursive);

        /// <summary>
        /// Gets the list of Standard folders the specified user has the provided permissions.
        /// </summary>
        /// <param name="user">The user info</param>
        /// <param name="permissions">The permissions the folders have to met.</param>
        /// <returns>The list of Standard folders the specified user has the provided permissions.</returns>
        /// <remarks>This method is used to support legacy behaviours and situations where we know the file/folder is in the file system.</remarks>
        IEnumerable<IFolderInfo> GetFileSystemFolders(UserInfo user, string permissions);

        /// <summary>
        /// Gets a folder entity by providing a portal identifier and folder identifier.
        /// </summary>
        /// <param name="folderID">The identifier of the folder.</param>
        /// <returns>The folder entity or null if the folder cannot be located.</returns>
        IFolderInfo GetFolder(int folderID);

        /// <summary>
        /// Gets a folder entity by providing a portal identifier and folder path.
        /// </summary>
        /// <param name="portalID">The portal where the folder exists.</param>
        /// <param name="folderPath">The path of the folder.</param>
        /// <returns>The folder entity or null if the folder cannot be located.</returns>
        IFolderInfo GetFolder(int portalID, string folderPath);

        /// <summary>
        /// Gets a folder entity by providing its unique id.
        /// </summary>
        /// <param name="uniqueId">The unique id of the folder.</param>
        /// <returns>The folder entity or null if the folder cannot be located.</returns>
        IFolderInfo GetFolder(Guid uniqueId);

        /// <summary>
        /// Gets the list of subfolders for the specified folder.
        /// </summary>
        /// <param name="parentFolder">The folder to get the list of subfolders.</param>
        /// <returns>The list of subfolders for the specified folder.</returns>
        IEnumerable<IFolderInfo> GetFolders(IFolderInfo parentFolder);

        /// <summary>
        /// Gets the sorted list of folders of the provided portal.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <returns>The sorted list of folders of the provided portal.</returns>
        IEnumerable<IFolderInfo> GetFolders(int portalID);

        /// <summary>
        /// Gets the sorted list of folders that match the provided permissions in the specified portal.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <param name="permissions">The permissions to match.</param>
        /// <param name="userID">The user identifier to be used to check permissions.</param>
        /// <returns>The list of folders that match the provided permissions in the specified portal.</returns>
        IEnumerable<IFolderInfo> GetFolders(int portalID, string permissions, int userID);

        /// <summary>
        /// Gets the list of folders the specified user has read permissions.
        /// </summary>
        /// <param name="user">The user info</param>
        /// <returns>The list of folders the specified user has read permissions.</returns>
        IEnumerable<IFolderInfo> GetFolders(UserInfo user);

        /// <summary>
        /// Gets the list of folders the specified user has the provided permissions.
        /// </summary>
        /// <param name="user">The user info</param>
        /// <param name="permissions">The permissions the folders have to met.</param>
        /// <returns>The list of folders the specified user has the provided permissions.</returns>
        IEnumerable<IFolderInfo> GetFolders(UserInfo user, string permissions);

        /// <summary>
        /// Moves the specified folder and its contents to a new location.
        /// </summary>
        /// <param name="folder">The folder to move.</param>
        /// <param name="newFolderPath">The new folder path.</param>
        /// <returns>The moved folder.</returns>
        IFolderInfo MoveFolder(IFolderInfo folder, string newFolderPath);

        /// <summary>
        /// Renames the specified folder by setting the new provided folder name.
        /// </summary>
        /// <param name="folder">The folder to rename.</param>
        /// <param name="newFolderName">The new name to apply to the folder.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Deprecated in DNN 6.2.  It has been replaced by FolderManager.Instance.MoveFolder(IFolderInfo folder, string newFolderPath) ")]
        void RenameFolder(IFolderInfo folder, string newFolderName);

        /// <summary>
        /// Sets specific folder permissions for the given role to the given folder.
        /// </summary>
        void SetFolderPermission(IFolderInfo folder, int permissionId, int roleId);

        /// <summary>
        /// Sets specific folder permissions for the given role/user to the given folder.
        /// </summary>
        void SetFolderPermission(IFolderInfo folder, int permissionId, int roleId, int userId);

        /// <summary>
        /// Sets folder permissions for administrator role to the given folder.
        /// </summary>
        void SetFolderPermissions(IFolderInfo folder, int administratorRoleId);

        /// <summary>
        /// Synchronizes the entire folder tree for the specified portal.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <returns>The number of folder collisions.</returns>
        int Synchronize(int portalID);

        /// <summary>
        /// Syncrhonizes the specified folder, its files and its subfolders.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <param name="relativePath">The relative path of the folder.</param>
        /// <returns>The number of folder collisions.</returns>
        int Synchronize(int portalID, string relativePath);

        /// <summary>
        /// Syncrhonizes the specified folder, its files and, optionally, its subfolders.
        /// </summary>
        /// <param name="portalID">The portal identifier.</param>
        /// <param name="relativePath">The relative path of the folder.</param>
        /// <param name="isRecursive">Indicates if the synchronization has to be recursive.</param>
        /// <param name="syncFiles">Indicates if files need to be synchronized.</param>
        /// <returns>The number of folder collisions.</returns>
        int Synchronize(int portalID, string relativePath, bool isRecursive, bool syncFiles);

        /// <summary>
        /// Updates metadata of the specified folder.
        /// </summary>
        /// <param name="folder">The folder to update.</param>
        IFolderInfo UpdateFolder(IFolderInfo folder);
    }
}
