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
using DotNetNuke.Instrumentation;

namespace DotNetNuke.Services.FileSystem
{
    public class StandardFolderProvider : FolderProvider
    {
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

            if (FileWrapper.Instance.Exists(file.PhysicalPath))
            {
                FileWrapper.Instance.SetAttributes(file.PhysicalPath, FileAttributes.Normal);
                FileWrapper.Instance.Delete(file.PhysicalPath);
            }
        }

        /// <remarks>
        ///   No implementation needed
        /// </remarks>
        public override void DeleteFolder(IFolderInfo folder)
        {
        }

        public override bool ExistsFile(IFolderInfo folder, string fileName)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNull("fileName", fileName);

            return FileWrapper.Instance.Exists(Path.Combine(folder.PhysicalPath, fileName));
        }

        public override bool ExistsFolder(string folderPath, FolderMappingInfo folderMapping)
        {
            Requires.NotNull("folderPath", folderPath);
            Requires.NotNull("folderMapping", folderMapping);

            return DirectoryWrapper.Instance.Exists(PathUtils.Instance.GetPhysicalPath(folderMapping.PortalID, folderPath));
        }

        public override byte[] GetFile(IFileInfo file)
        {
            Requires.NotNull("file", file);

            byte[] fileContent = null;

            try
            {
                fileContent = FileWrapper.Instance.ReadAllBytes(file.PhysicalPath);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
            }

            return fileContent;
        }

        public override FileAttributes? GetFileAttributes(IFileInfo file)
        {
            Requires.NotNull("file", file);

            FileAttributes? fileAttributes = null;

            try
            {
                fileAttributes = FileWrapper.Instance.GetAttributes(file.PhysicalPath);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
            }

            return fileAttributes;
        }

        public override long GetFileLength(IFileInfo file)
        {
            Requires.NotNull("file", file);

            var physicalFile = new System.IO.FileInfo(file.PhysicalPath);

            return physicalFile.Length;
        }

        public override string[] GetFiles(IFolderInfo folder)
        {
            Requires.NotNull("folder", folder);

            var fileNames = DirectoryWrapper.Instance.GetFiles(folder.PhysicalPath);

            for (var i = 0; i < fileNames.Length; i++)
            {
                fileNames[i] = Path.GetFileName(fileNames[i]);
            }

            return fileNames;
        }

        public override Stream GetFileStream(IFolderInfo folder, string fileName)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("fileName", fileName);

            Stream stream = null;

            try
            {
                stream = FileWrapper.Instance.OpenRead(Path.Combine(folder.PhysicalPath, fileName));
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
            }

            return stream;
        }

        public override string GetImageUrl()
        {
            return GlobalsWrapper.Instance.ResolveUrl("~/images/folder.gif");
        }

        public override DateTime GetLastModificationTime(IFileInfo file)
        {
            Requires.NotNull("file", file);

            var lastModificationTime = Null.NullDate;

            try
            {
                lastModificationTime = FileWrapper.Instance.GetLastWriteTime(file.PhysicalPath);
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

            return DirectoryWrapper.Instance.GetDirectories(PathUtils.Instance.GetPhysicalPath(folderMapping.PortalID, folderPath))
                .Select(directory => PathUtils.Instance.GetRelativePath(folderMapping.PortalID, directory));
        }

        public override bool IsInSync(IFileInfo file)
        {
            Requires.NotNull("file", file);

            return GetHash(file) == file.SHA1Hash;
        }

        public override void RenameFile(IFileInfo file, string newFileName)
        {
            Requires.NotNull("file", file);
            Requires.NotNullOrEmpty("newFileName", newFileName);

            if (file.FileName != newFileName)
            {
                FileWrapper.Instance.Move(file.PhysicalPath, Path.Combine(Path.GetDirectoryName(file.PhysicalPath), newFileName));
            }
        }

        public override void RenameFolder(IFolderInfo folder, string newFolderName)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("newFolderName", newFolderName);

            if (folder.FolderName != newFolderName)
            {
                var newFolderPath = Path.Combine(folder.PhysicalPath.Substring(0, folder.PhysicalPath.LastIndexOf(folder.FolderName)), newFolderName);
                DirectoryWrapper.Instance.Move(folder.PhysicalPath, newFolderPath);
            }
        }

        public override void SetFileAttributes(IFileInfo file, FileAttributes fileAttributes)
        {
            Requires.NotNull("file", file);

            FileWrapper.Instance.SetAttributes(file.PhysicalPath, fileAttributes);
        }

        public override bool SupportsFileAttributes()
        {
            return true;
        }

        public override void UpdateFile(IFolderInfo folder, string fileName, Stream content)
        {
            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("fileName", fileName);
            Requires.NotNull("content", content);

            var arrData = new byte[2048];
            var filePath = Path.Combine(folder.PhysicalPath, fileName);

            if (FileWrapper.Instance.Exists(filePath))
            {
                FileWrapper.Instance.Delete(filePath);
            }

            using (var outStream = FileWrapper.Instance.Create(filePath))
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

        #endregion
    }
}