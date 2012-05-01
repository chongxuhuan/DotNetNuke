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
using System.Globalization;
using System.IO;
using System.Web.Mvc;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Services;

namespace DotNetNuke.Modules.Journal
{

    public class UserFileController : DnnController
    {
        private readonly IFolderManager _folderManager = FolderManager.Instance;
        private readonly IPathUtils _pathUtils = PathUtils.Instance;

        [DnnAuthorize]
        [HttpGet]
        public ActionResult GetItems(string fileExtensions)
        {
            IFolderInfo userFolder = GetUserFolder();

            var extensions = new List<string>();
            if (!string.IsNullOrEmpty(fileExtensions))
            {
                fileExtensions = fileExtensions.ToLowerInvariant();
                extensions.AddRange(fileExtensions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            var folderStructure = new Item
            {
                children = GetChildren(userFolder, extensions),
                folder = true,
                id = userFolder.FolderID,
                name = Localization.GetString("UserFolderTitle.Text",Localization.SharedResourceFile)
            };

            return Json(new List<Item> { folderStructure }, JsonRequestBehavior.AllowGet);
        }

        // ReSharper disable LoopCanBeConvertedToQuery
        private List<Item> GetChildren(IFolderInfo folder, List<string> extensions)
        {
            var everything = new List<Item>();

            IEnumerable<IFolderInfo> folders = _folderManager.GetFolders(folder);

            foreach (IFolderInfo currentFolder in folders)
            {
                everything.Add(new Item
                {
                    id = currentFolder.FolderID,
                    name = currentFolder.DisplayName ?? currentFolder.FolderName,
                    folder = true,
                    parentId = folder.FolderID,
                    children = GetChildren(currentFolder, extensions)
                });
            }

            IEnumerable<IFileInfo> files = _folderManager.GetFiles(folder);

            foreach (IFileInfo file in files)
            {
                // list is empty or contains the file extension in question
                if (extensions.Count == 0 || extensions.Contains(file.Extension.ToLowerInvariant()))
                {
                    everything.Add(new Item
                    {
                        id = file.FileId,
                        name = file.FileName,
                        folder = false,
                        parentId = file.FolderId,
                        thumb_url = GetThumbUrl(file),
                        type = GetTypeName(file),
                        size = GetFileSize(file.Size),
                        modified = GetModifiedTime(file.LastModificationTime)
                    });
                }
            }

            return everything;
        }

        private static string GetModifiedTime(DateTime dateTime)
        {
            return string.Format("{0:MMM} {0:dd}, {0:yyyy} at {0:t}", dateTime);
        }

        // ReSharper restore LoopCanBeConvertedToQuery

        private string GetThumbUrl(IFileInfo file)
        {
            if (IsImageFile(file.RelativePath))
            {
                return Path.Combine(PortalSettings.HomeDirectory, file.RelativePath);
            }

            string fileIcon = IconController.IconURL("Ext" + file.Extension, "32x32");
            if (!System.IO.File.Exists(Server.MapPath(fileIcon)))
            {
                fileIcon = IconController.IconURL("File", "32x32");
            }
            return fileIcon;
        }

        private static string GetTypeName(IFileInfo file)
        {
            if (file.ContentType == null)
            {
                return string.Empty;
            }

            string name = file.ContentType;
            if (name.StartsWith("image/"))
            {
                name = file.ContentType.Replace("image/", string.Empty);
            }
            else
            {
                name = file.Extension != null ? file.Extension.ToLowerInvariant() : string.Empty;
            }

            return name;
        }

        private static bool IsImageFile(string relativePath)
        {
            var acceptedExtensions = new List<string> { "jpg", "png", "gif", "jpe", "jpeg", "tiff" };
            string extension = relativePath.Substring(relativePath.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower();
            return acceptedExtensions.Contains(extension);
        }

        private static string GetFileSize(int sizeInBytes)
        {
            int size = sizeInBytes / 1024;
            bool biggerThanAMegabyte = size > 1024;
            if (biggerThanAMegabyte)
            {
                size = (size / 1024);
            }
            return size.ToString(CultureInfo.InvariantCulture) + (biggerThanAMegabyte ? "Mb" : "k");
        }

        private IFolderInfo GetUserFolder()
        {
            string userFolderPath = _pathUtils.GetUserFolderPath(UserInfo);
            IFolderInfo folder = _folderManager.GetFolder(PortalSettings.PortalId, userFolderPath);
            if (folder == null)
            {
                // this line will take care of creating the user's folder
                _folderManager.GetFileSystemFolders(UserInfo, "READ");
            }
            return _folderManager.GetFolder(PortalSettings.PortalId, userFolderPath);
        }

        class Item
        {
            // ReSharper disable InconsistentNaming
            public int id { get; set; }
            public string name { get; set; }
            public bool folder { get; set; }
            public int parentId { get; set; }
            public string thumb_url { get; set; }
            public string type { get; set; }
            public string size { get; set; }
            public string modified { get; set; }
            public List<Item> children { get; set; }
            // ReSharper restore InconsistentNaming
        }
    }
}