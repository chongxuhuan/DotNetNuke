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

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Common;

namespace DotNetNuke.Services.FileSystem
{
    /// <summary>
    ///   Base class that provides common functionallity to work with files and folders.
    /// </summary>
    public abstract class FolderProvider
    {
        #region Constants

        public const string SettingsControlID = "Settings.ascx";

        #endregion

        #region Static Provider Methods

        /// <summary>
        ///   Get the list of all the folder providers.
        /// </summary>
        public static Dictionary<string, FolderProvider> GetProviderList()
        {
            return ComponentFactory.GetComponents<FolderProvider>();
        }

        /// <summary>
        ///   Gets an instance of a specific FolderProvider of a given name.
        /// </summary>
        public static FolderProvider Instance(string friendlyName)
        {
            return ComponentFactory.GetComponent<FolderProvider>(friendlyName);
        }

        /// <summary>
        ///   Gets the list of default folder providers.
        /// </summary>
        public static List<string> GetDefaultProviders()
        {
            return new List<string> { "StandardFolderProvider", "SecureFolderProvider", "DatabaseFolderProvider" };
        }

        /// <summary>
        ///   Gets the virtual path of the control file used to display and update specific folder mapping settings. By convention, the control name must be Settings.ascx.
        /// </summary>
        /// <returns>
        ///   If the folder provider has special settings, this method returns the virtual path of the control that allows to display and set those settings.
        /// </returns>
        /// <remarks>
        ///   The returned control must inherit from FolderMappingSettingsControlBase.
        /// </remarks>
        public static string GetSettingsControlVirtualPath(string friendlyName)
        {
            Requires.NotNullOrEmpty("friendlyName", friendlyName);

            var provider = Config.GetProvider("folder", friendlyName);

            if (provider != null)
            {
                var virtualPath = provider.Attributes["providerPath"] + SettingsControlID;

                if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(virtualPath)))
                {
                    return virtualPath;
                }
            }

            return string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating if the provider ensures the files/folders it manages are secure from outside access.
        /// </summary>
        /// <remarks>
        /// Some providers (e.g. Standard) store their files/folders in a way that allows for anonymous access that bypasses DotNetNuke.
        /// These providers cannot guarantee that files are only accessed by authorized users and must return false.
        /// </remarks>
        public virtual bool IsStorageSecure
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        ///   Adds a new file to the specified folder.
        /// </summary>
        /// <remarks>
        ///   Do not close content Stream.
        /// </remarks>
        public abstract void AddFile(IFolderInfo folder, string fileName, Stream content);
        
        /// <summary>
        ///   Adds a new folder to a specified parent folder.
        /// </summary>
        public abstract void AddFolder(string folderPath, FolderMappingInfo folderMapping);
        
        /// <summary>
        ///   Deletes the specified file.
        /// </summary>
        public abstract void DeleteFile(IFileInfo file);
        
        /// <summary>
        ///   Deletes the specified folder.
        /// </summary>
        public abstract void DeleteFolder(IFolderInfo folder);

        /// <summary>
        ///   Checks the existence of the specified file in the underlying system.
        /// </summary>
        public abstract bool ExistsFile(IFolderInfo folder, string fileName);

        /// <summary>
        ///   Checks the existence of the specified folder in the underlying system.
        /// </summary>
        public abstract bool ExistsFolder(string folderPath, FolderMappingInfo folderMapping);

        /// <summary>
        ///   Gets the content of the specified file.
        /// </summary>
        public abstract byte[] GetFile(IFileInfo file);

        /// <summary>
        ///   Gets the file attributes of the specified file.
        /// </summary>
        /// <remarks>
        ///   Because some Providers don't support file attributes, this methods returns a nullable type to allow them to return null.
        /// </remarks>
        public abstract FileAttributes? GetFileAttributes(IFileInfo file);

        /// <summary>
        /// Gets the file length.
        /// </summary>
        public abstract long GetFileLength(IFileInfo file);

        /// <summary>
        ///   Gets the list of file names contained in the specified folder.
        /// </summary>
        public abstract string[] GetFiles(IFolderInfo folder);

        /// <summary>
        ///   Gets a file Stream of the specified file.
        /// </summary>
        public abstract Stream GetFileStream(IFolderInfo folder, string fileName);

        /// <summary>
        /// Gets the direct Url to the file.
        /// </summary>
        public abstract string GetFileUrl(IFileInfo file);
        
        /// <summary>
        ///   Gets the URL of the image to display in FileManager tree.
        /// </summary>
        public abstract string GetImageUrl();

        /// <summary>
        ///   Gets the time when the specified file was last modified.
        /// </summary>
        public abstract DateTime GetLastModificationTime(IFileInfo file);

        /// <summary>
        ///   Gets the list of subfolders for the specified folder.
        /// </summary>
        public abstract IEnumerable<string> GetSubFolders(string folderPath, FolderMappingInfo folderMapping);

        /// <summary>
        ///   Indicates if the specified file is synchronized.
        /// </summary>
        public abstract bool IsInSync(IFileInfo file);

        /// <summary>
        ///   Renames the specified file using the new filename.
        /// </summary>
        public abstract void RenameFile(IFileInfo file, string newFileName);

        /// <summary>
        ///   Renames the specified folder using the new foldername.
        /// </summary>
        public abstract void RenameFolder(IFolderInfo folder, string newFolderName);

        /// <summary>
        ///   Sets the specified attributes to the specified file.
        /// </summary>
        public abstract void SetFileAttributes(IFileInfo file, FileAttributes fileAttributes);

        /// <summary>
        ///   Gets a value indicating if the underlying system supports file attributes.
        /// </summary>
        public abstract bool SupportsFileAttributes();

        /// <summary>
        ///   Updates the content of the specified file.
        /// </summary>
        /// <remarks>
        ///   Do not close content Stream.
        /// </remarks>
        public abstract void UpdateFile(IFolderInfo folder, string fileName, Stream content);

        #endregion
    }
}