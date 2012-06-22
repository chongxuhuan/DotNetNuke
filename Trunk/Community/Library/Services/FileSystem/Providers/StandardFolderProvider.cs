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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using DotNetNuke.Common;
using DotNetNuke.Common.Internal;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.FileSystem.Internal;

namespace DotNetNuke.Services.FileSystem
{
    public class StandardFolderProvider : FolderProvider
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating if the provider requires network connectivity to do its tasks.
        /// </summary>
        public override bool RequiresNetworkConnectivity
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Abstract Methods

        public override void AddFile(IFolderInfo folder, string fileName, Stream content)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("fileName", fileName);
            Requires.NotNull("content", content);

            UpdateFile(folder, fileName, content);
        }

        /// <remarks>
        ///   No implementation needed
        /// </remarks>
        public override void AddFolder(string folderPath, FolderMappingInfo folderMapping)
        {
        }

        public override void DeleteFile(IFileInfo file)
        {
            Requires.NotNull("file", file);

            var path = GetActualPath(file);

            if (FileWrapper.Instance.Exists(path))
            {
                FileWrapper.Instance.SetAttributes(path, FileAttributes.Normal);
                FileWrapper.Instance.Delete(path);
            }
        }

        /// <remarks>
        ///   No implementation needed
        /// </remarks>
        public override void DeleteFolder(IFolderInfo folder)
        {
        }

        public override bool FileExists(IFolderInfo folder, string fileName)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNull("fileName", fileName);

            return FileWrapper.Instance.Exists(GetActualPath(folder, fileName));
        }

        public override bool FolderExists(string folderPath, FolderMappingInfo folderMapping)
        {
            Requires.NotNull("folderPath", folderPath);
            Requires.NotNull("folderMapping", folderMapping);

            return DirectoryWrapper.Instance.Exists(GetActualPath(folderMapping, folderPath));
        }

        public override FileAttributes? GetFileAttributes(IFileInfo file)
        {
            Requires.NotNull("file", file);

            FileAttributes? fileAttributes = null;

            try
            {
                fileAttributes = FileWrapper.Instance.GetAttributes(GetActualPath(file));
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
            }

            return fileAttributes;
        }

        public override string[] GetFiles(IFolderInfo folder)
        {
            Requires.NotNull("folder", folder);

            var fileNames = DirectoryWrapper.Instance.GetFiles(GetActualPath(folder));

            for (var i = 0; i < fileNames.Length; i++)
            {
                fileNames[i] = Path.GetFileName(fileNames[i]);
            }

            return fileNames;
        }

        public override long GetFileSize(IFileInfo file)
        {
            Requires.NotNull("file", file);

            var physicalFile = new System.IO.FileInfo(GetActualPath(file));

            return physicalFile.Length;
        }

        public override Stream GetFileStream(IFileInfo file)
        {
            Requires.NotNull("file", file);

            var folder = FolderManager.Instance.GetFolder(file.FolderId);

            return GetFileStream(folder, file.FileName);
        }

        public override Stream GetFileStream(IFolderInfo folder, string fileName)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("fileName", fileName);

            Stream stream = null;

            try
            {
                stream = FileWrapper.Instance.OpenRead(GetActualPath(folder, fileName));
            }
            catch (IOException iex)
            {
                DnnLog.Warn(iex.Message);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
            }

            return stream;
        }

        public override string GetFileUrl(IFileInfo file)
        {
            Requires.NotNull("file", file);

            string rootFolder;
            if (file.PortalId == Null.NullInteger)
            {
                //Host
                rootFolder = Globals.HostPath;
            }
            else
            {
                //Portal
                var portalSettings = GetPortalSettings(file.PortalId);
                rootFolder = portalSettings.HomeDirectory;
            }

            return TestableGlobals.Instance.ResolveUrl(rootFolder + file.Folder + file.FileName);
        }

        public override string GetFolderProviderIconPath()
        {
            return IconControllerWrapper.Instance.IconURL("Folder");
        }

        public override DateTime GetLastModificationTime(IFileInfo file)
        {
            Requires.NotNull("file", file);

            var lastModificationTime = Null.NullDate;

            try
            {
                lastModificationTime = FileWrapper.Instance.GetLastWriteTime(GetActualPath(file));
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
            }

            return lastModificationTime;
        }

        public override IEnumerable<string> GetSubFolders(string folderPath, FolderMappingInfo folderMapping)
        {
            Requires.NotNull("folderPath", folderPath);
            Requires.NotNull("folderMapping", folderMapping);

            return DirectoryWrapper.Instance.GetDirectories(GetActualPath(folderMapping, folderPath))
                .Select(directory => GetRelativePath(folderMapping, directory));
        }

        public override bool IsInSync(IFileInfo file)
        {
            Requires.NotNull("file", file);

            return Convert.ToInt32((file.LastModificationTime - GetLastModificationTime(file)).TotalSeconds) == 0;                        
        }

        public override void RenameFile(IFileInfo file, string newFileName)
        {
            Requires.NotNull("file", file);
            Requires.NotNullOrEmpty("newFileName", newFileName);

            if (file.FileName != newFileName)
            {
                IFolderInfo folder = FolderManager.Instance.GetFolder(file.FolderId);
                string oldName = GetActualPath(file);
                string newName = GetActualPath(folder, newFileName);
                FileWrapper.Instance.Move(oldName, newName);
            }
        }

        [Obsolete("Deprecated in DNN 6.2.  It has been replaced by MoveFolder(string folderPath, string newFolderPath, FolderMappingInfo folderMapping) ")]
        public override void RenameFolder(IFolderInfo folder, string newFolderName)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("newFolderName", newFolderName);

            if (folder.FolderName != newFolderName)
            {
                var currentPath = GetActualPath(folder);
                var parentPath = currentPath.Substring(0, folder.PhysicalPath.LastIndexOf(folder.FolderName));
                var newFolderPath = Path.Combine(parentPath, newFolderName); //todo is this ok
                DirectoryWrapper.Instance.Move(currentPath, newFolderPath);
            }
        }

        public override void SetFileAttributes(IFileInfo file, FileAttributes fileAttributes)
        {
            Requires.NotNull("file", file);

            FileWrapper.Instance.SetAttributes(GetActualPath(file), fileAttributes);
        }

        public override bool SupportsFileAttributes()
        {
            return true;
        }

        public override void UpdateFile(IFileInfo file, Stream content)
        {
            Requires.NotNull("file", file);
            Requires.NotNull("content", content);

            UpdateFile(FolderManager.Instance.GetFolder(file.FolderId), file.FileName, content);
        }

        public override void UpdateFile(IFolderInfo folder, string fileName, Stream content)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("fileName", fileName);
            Requires.NotNull("content", content);

            var arrData = new byte[2048];
            var actualPath = GetActualPath(folder, fileName);

            if (FileWrapper.Instance.Exists(actualPath))
            {
                FileWrapper.Instance.Delete(actualPath);
            }

            using (var outStream = FileWrapper.Instance.Create(actualPath))
            {
                var originalPosition = content.Position;
                content.Position = 0;

                try
                {
                    var intLength = content.Read(arrData, 0, arrData.Length);

                    while (intLength > 0)
                    {
                        outStream.Write(arrData, 0, intLength);
                        intLength = content.Read(arrData, 0, arrData.Length);
                    }
                }
                finally
                {
                    content.Position = originalPosition;
                }
            }
        }

        #endregion

        #region Internal Methods

        internal virtual string GetHash(IFileInfo file)
        {
            var fileManager = new FileManager();
            return fileManager.GetHash(file);
        }

        internal virtual PortalSettings GetPortalSettings(int portalId)
        {
            return new PortalSettings(portalId);
        }

        #endregion

        /// <summary>
        /// Get the path relative to the root of the FolderMapping
        /// </summary>
        /// <param name="folderMapping">Path is relative to this</param>
        /// <param name="path">The path</param>
        /// <returns>A relative path</returns>
        protected virtual string GetRelativePath(FolderMappingInfo folderMapping, string path)
        {
            return PathUtils.Instance.GetRelativePath(folderMapping.PortalID, path);
        }

        /// <summary>
        /// Get actual path to an IFileInfo
        /// </summary>
        /// <param name="file">The file</param>
        /// <returns>A windows supported path to the file</returns>
        protected virtual string GetActualPath(IFileInfo file)
        {
            return file.PhysicalPath;
        }

        /// <summary>
        /// Get actual path to a file in specified folder
        /// </summary>
        /// <param name="folder">The folder that contains the file</param>
        /// <param name="fileName">The file name</param>
        /// <returns>A windows supported path to the file</returns>
        protected virtual string GetActualPath(IFolderInfo folder, string fileName)
        {
            return Path.Combine(folder.PhysicalPath, fileName);
        }

        /// <summary>
        /// Get actual path to a folder in the specified folder mapping
        /// </summary>
        /// <param name="folderMapping">The folder mapping</param>
        /// <param name="folderPath">The folder path</param>
        /// <returns>A windows supported path to the folder</returns>
        protected virtual string GetActualPath(FolderMappingInfo folderMapping, string folderPath)
        {
            return PathUtils.Instance.GetPhysicalPath(folderMapping.PortalID, folderPath);
        }

        /// <summary>
        /// Get actual path to a folder
        /// </summary>
        /// <param name="folder">The folder</param>
        /// <returns>A windows supported path to the folder</returns>
        protected virtual string GetActualPath(IFolderInfo folder)
        {
            return folder.PhysicalPath;
        }
    }
}