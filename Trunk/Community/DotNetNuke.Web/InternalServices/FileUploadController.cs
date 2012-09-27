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
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Internal;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;
using DotNetNuke.Web.UI;

namespace DotNetNuke.Web.InternalServices
{
    public class FileUploadController: DnnApiController
    {
        public class FolderItemDTO
        {
            public string FolderPath { get; set; }
            public string FileFilter { get; set; }
            public bool Required { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage LoadFiles(FolderItemDTO folderItem)
        {
            int effectivePortalId = PortalSettings.PortalId;
            if (string.IsNullOrEmpty(folderItem.FolderPath)) folderItem.FolderPath = "";
            if (IsUserFolder(folderItem.FolderPath))
            {
                effectivePortalId = PortalController.GetEffectivePortalId(PortalSettings.PortalId);
            }

            var list = Globals.GetFileList(effectivePortalId, folderItem.FileFilter, !folderItem.Required, folderItem.FolderPath);
            var fileItems = new List<FileItem>();

            foreach(var item in list)
            {
                FileItem fileItem = item as FileItem;
                if (fileItem != null) fileItems.Add(fileItem);
            }

            return Request.CreateResponse(HttpStatusCode.OK, fileItems);
        }

        [HttpGet]
        public HttpResponseMessage LoadImage(string fileId)
        {
            if(!string.IsNullOrEmpty(fileId))
            {
                int file = 0;
                if(int.TryParse(fileId, out file))
                {
                    var imageUrl = ShowImage(file);
                    return Request.CreateResponse(HttpStatusCode.OK, imageUrl);
                }
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        public Task<HttpResponseMessage> PostFile()
        {
            HttpRequestMessage request = this.Request;

            if(!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType); 
            }

            var provider = new MultipartMemoryStreamProvider();

            var domainName = TestableGlobals.Instance.GetDomainName(ControllerContext.Request.RequestUri);
            var alias = TestablePortalAliasController.Instance.GetPortalAliasInfo(domainName);
            int tabId;
            ValidateTabAndModuleContext(alias.PortalID, out tabId);

            var portalSettings = new PortalSettings(tabId, alias);
            var userInfo = UserInfo;

            var task = request.Content.ReadAsMultipartAsync(provider).
                 ContinueWith<HttpResponseMessage>(o =>
                    {
                        string folder = string.Empty;
                        string filter = string.Empty;
                        string returnFilename = string.Empty;

                        foreach(var item in provider.Contents)
                        {
                            var name = item.Headers.ContentDisposition.Name;
                            switch(name.ToUpper())
                            {
                                case "\"FOLDER\"":
                                    folder = item.ReadAsStringAsync().Result;
                                    break;

                                case "\"FILTER\"":
                                    filter = item.ReadAsStringAsync().Result;
                                    break;


                                case "\"POSTFILE\"":
                                    var fileName = item.Headers.ContentDisposition.FileName.Replace("\"", "");
                                    Stream stream = item.ReadAsStreamAsync().Result;

                                    returnFilename = SaveFile(stream, portalSettings, userInfo, folder, filter, fileName);
                                    break;
                            }
                        }

                        if(!string.IsNullOrEmpty(returnFilename))
                        {
                            var root = System.AppDomain.CurrentDomain.BaseDirectory;
                            returnFilename = returnFilename.Replace(root, "~/");
                            returnFilename = System.Web.VirtualPathUtility.ToAbsolute(returnFilename);
                        }

                        return new HttpResponseMessage()
                        {
                            Content = new StringContent(returnFilename)
                        };
                    }
             );

            return task; 
        }

        private string SaveFile(Stream stream, PortalSettings portalSettings, UserInfo userInfo, string folder, string filter, string fileName)
        {
            try
            {

                if(!string.IsNullOrEmpty(fileName))
                {
                    var extension = Path.GetExtension(fileName).Replace(".", "");
                    if (!string.IsNullOrEmpty(filter) && !filter.ToLower().Contains(extension.ToLower()))
				    {
					    // trying to upload a file not allowed for current filter
				        return string.Empty;
				    }
                    else
                    { 
                        var folderManager = FolderManager.Instance;
                        var effectivePortalId = portalSettings.PortalId;
                        if (string.IsNullOrEmpty(folder)) folder = "";

                        //Check if this is a User Folder
                        IFolderInfo folderInfo;
                        if (folder.ToLowerInvariant().StartsWith("users/") && folder.EndsWith(string.Format("/{0}/", userInfo.UserID)))
                        {
                            effectivePortalId = PortalController.GetEffectivePortalId(portalSettings.PortalId);

                            //Make sure the user folder exists
                            folderInfo = folderManager.GetFolder(effectivePortalId, folder);
					        if (folderInfo == null)
					        {
						        //Add User folder
                                //fix user's portal id
						        userInfo.PortalID = portalSettings.PortalId;
                                folderInfo = ((FolderManager)folderManager).AddUserFolder(userInfo);
					        }
                        }
                        else
                        {
                            folderInfo = folderManager.GetFolder(portalSettings.PortalId, folder);
                        }

                        FileManager.Instance.AddFile(folderInfo, fileName, stream, true);
                        return Path.Combine(folderInfo.PhysicalPath, fileName);

                    }

                }

                return string.Empty;

            }
            catch(Exception exe)
            {
                DnnLog.Error(exe.Message);
                return string.Empty;
            }
        }

        private bool IsUserFolder(string folderPath)
        {
            return (folderPath.ToLowerInvariant().StartsWith("users/") && folderPath.EndsWith(string.Format("/{0}/", UserInfo.UserID)));
        }

        private string ShowImage(int fileId)
        {
            var image = (DotNetNuke.Services.FileSystem.FileInfo)FileManager.Instance.GetFile(fileId);

            if (image != null)
            {
                var imageUrl = FileManager.Instance.GetUrl(image);
                return imageUrl;
            }

            return null;

        }
    }
}
