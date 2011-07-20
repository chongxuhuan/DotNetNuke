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
using System.Data;
using System.IO;
using System.Linq;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Services.FileSystem
{
    public class DatabaseFolderProvider : FolderProvider
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating if the provider ensures the files/folders it manages are secure from outside access.
        /// </summary>
        public override bool IsStorageSecure
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Abstract Methods

        public override void AddFile(IFolderInfo folder, string fileName, Stream content)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("fileName", fileName);

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

            DataProvider.Instance().UpdateFileContent(file.FileId, null);
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

            return DataProvider.Instance().GetFile(fileName, folder.FolderID).Read();
        }

        public override bool FolderExists(string folderPath, FolderMappingInfo folderMapping)
        {
            Requires.NotNull("folderPath", folderPath);
            Requires.NotNull("folderMapping", folderMapping);

            return DataProvider.Instance().GetFolder(folderMapping.PortalID, folderPath).Read();
        }

        public override FileAttributes? GetFileAttributes(IFileInfo file)
        {
            return null;
        }

        public override string[] GetFiles(IFolderInfo folder)
        {
            Requires.NotNull("folder", folder);

            var files = FolderManager.Instance.GetFiles(folder).ToList();

            var fileNames = new string[files.Count];

            for (var i = 0; i < files.Count; i++)
            {
                fileNames[i] = files[i].FileName;
            }

            return fileNames;
        }

        public override long GetFileSize(IFileInfo file)
        {
            Requires.NotNull("file", file);

            return file.Size;
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

            var file = FileManager.Instance.GetFile(folder, fileName);
            
            if (file != null)
            {
                byte[] bytes = null;
                IDataReader dr = null;
                try
                {
                    dr = DataProvider.Instance().GetFileContent(file.FileId);
                    if (dr.Read())
                    {
                        bytes = (byte[])dr["Content"];
                    }
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
                
                if (bytes != null)
                {
                    return new MemoryStream(bytes);
                }
            }

            return null;
        }

        public override string GetFileUrl(IFileInfo file)
        {
            Requires.NotNull("file", file);

            return GlobalsWrapper.Instance.LinkClick(String.Format("fileid={0}", file.FileId), Null.NullInteger, Null.NullInteger);
        }

        public override string GetFolderProviderIconPath()
        {
            return IconControllerWrapper.Instance.IconURL("Sql");
        }

        public override DateTime GetLastModificationTime(IFileInfo file)
        {
            return Null.NullDate;
        }

        public override IEnumerable<string> GetSubFolders(string folderPath, FolderMappingInfo folderMapping)
        {
            Requires.NotNull("folderPath", folderPath);
            Requires.NotNull("folderMapping", folderMapping);

            var folderManager = FolderManager.Instance;

            var folder = folderManager.GetFolder(folderMapping.PortalID, folderPath);

            return folderManager.GetFolders(folder).Select(subfolder => subfolder.FolderPath);
        }

        public override bool IsInSync(IFileInfo file)
        {
            return true;
        }

        /// <remarks>
        ///   No implementation needed
        /// </remarks>
        public override void RenameFile(IFileInfo file, string newFileName)
        {
        }

        public override void RenameFolder(IFolderInfo folder, string newFolderName)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("newFolderName", newFolderName);

            if (folder.FolderName != newFolderName)
            {
                var newFolderPath = folder.FolderPath.Substring(0, folder.FolderPath.LastIndexOf(folder.FolderName)) +
                    (newFolderName.EndsWith("/") ? newFolderName : newFolderName + "/");

                DataProvider.Instance().UpdateFolder(
                    folder.PortalID,
                    folder.VersionGuid,
                    folder.FolderID,
                    newFolderPath,
                    folder.StorageLocation,
                    folder.IsProtected,
                    folder.IsCached,
                    DateTime.Now,
                    UserControllerWrapper.Instance.GetCurrentUserInfo().UserID,
                    folder.FolderMappingID);
            }
        }

        public override void SetFileAttributes(IFileInfo file, FileAttributes fileAttributes)
        {
        }

        public override bool SupportsFileAttributes()
        {
            return false;
        }

        public override void UpdateFile(IFileInfo file, Stream content)
        {
            Requires.NotNull("file", file);

            byte[] fileContent = null;

            if (content != null)
            {
                var originalPosition = content.Position;
                content.Position = 0;

                var buffer = new byte[16 * 1024];

                using (var ms = new MemoryStream())
                {
                    int read;

                    while ((read = content.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }

                    fileContent = ms.ToArray();
                }

                content.Position = originalPosition;
            }

            DataProvider.Instance().UpdateFileContent(file.FileId, fileContent);
        }

        public override void UpdateFile(IFolderInfo folder, string fileName, Stream content)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("fileName", fileName);

            var file = FileManager.Instance.GetFile(folder, fileName);

            if (file == null) return;
            
            byte[] fileContent = null;

            if (content != null)
            {
                var originalPosition = content.Position;
                content.Position = 0;

                var buffer = new byte[16 * 1024];
                    
                using (var ms = new MemoryStream())
                {
                    int read;
                        
                    while ((read = content.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                        
                    fileContent = ms.ToArray();
                }

                content.Position = originalPosition;
            }

            DataProvider.Instance().UpdateFileContent(file.FileId, fileContent);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Clears the content of the file in the database.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        public static void ClearFileContent(int fileId)
        {
            DataProvider.Instance().UpdateFileContent(fileId, null);
            DataProvider.Instance().UpdateFileVersion(fileId, Guid.NewGuid());
        }

        /// <summary>
        /// Updates the content of the file in the database.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="content">The new content.</param>
        public static void UpdateFileContent(int fileId, Stream content)
        {
            if (content != null)
            {
                byte[] fileContent;
                var buffer = new byte[16 * 1024];
                using (var ms = new MemoryStream())
                {
                    int read;
                    while ((read = content.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    fileContent = ms.ToArray();
                }

                DataProvider.Instance().UpdateFileContent(fileId, fileContent);
            }
            else
            {
                DataProvider.Instance().UpdateFileContent(fileId, null);
            }

            DataProvider.Instance().UpdateFileVersion(fileId, Guid.NewGuid());
        }

        /// <summary>
        /// Updates the content of the file in the database.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="content">The new content.</param>
        public static void UpdateFileContent(int fileId, byte[] content)
        {
            DataProvider.Instance().UpdateFileContent(fileId, content);
            DataProvider.Instance().UpdateFileVersion(fileId, Guid.NewGuid());
        }

        #endregion
    }
}