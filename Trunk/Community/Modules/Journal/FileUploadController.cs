using System;

using DotNetNuke.Instrumentation;

namespace DotNetNuke.Modules.Journal
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;

    using Entities.Host;
    using Services.FileSystem;
    using Web.Services;

    public class FileUploadController : DnnController
    {
        private readonly IFileManager _fileManager = FileManager.Instance;
        private readonly IFolderManager _folderManager = FolderManager.Instance;

        [DnnAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile()
        {
            var statuses = new List<FilesStatus>();
            try
            {
                UploadWholeFile(HttpContext, statuses);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
            }
            return IframeSafeJson(Json(statuses));
        }

        private JsonResult IframeSafeJson(JsonResult jsonResult)
        {
            string httpAccept = null;
            try
            {
                httpAccept = Request["HTTP_ACCEPT"];
            }
            catch (HttpRequestValidationException)
            { //swallow error and assume json not supported
            }

            if (httpAccept == null || !httpAccept.Contains("application.json"))
            {
                jsonResult.ContentType = "text/plain";
            }

            return jsonResult;
        }

        private static bool IsAllowedExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            //regex matches a dot followed by 1 or more chars followed by a semi-colon
            //regex is meant to block files like "foo.asp;.png" which can take advantage
            //of a vulnerability in IIS6 which treasts such files as .asp, not .png
            return !string.IsNullOrEmpty(extension)
                   && Host.AllowedExtensionWhitelist.IsAllowedExtension(extension)
                   && !Regex.IsMatch(fileName, @"\..+;");
        }

        // Upload entire file
        private void UploadWholeFile(HttpContextBase context, ICollection<FilesStatus> statuses)
        {
            for (var i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];
                if (file == null) continue;

                var fileName = Path.GetFileName(file.FileName);

                if (IsAllowedExtension(fileName))
                {
                    var userFolder = _folderManager.GetUserFolder(UserInfo);

                    //todo: deal with the case where the exact file name already exists.
                    var fileInfo = _fileManager.AddFile(userFolder, fileName, file.InputStream, true);
                    var fileIcon = Entities.Icons.IconController.IconURL("Ext" + fileInfo.Extension, "32x32");
                    if (!System.IO.File.Exists(Server.MapPath(fileIcon)))
                    {
                        fileIcon = Entities.Icons.IconController.IconURL("File", "32x32");
                    }
                    statuses.Add(new FilesStatus
                    {
                        success = true,
                        name = fileName,
                        extension = fileInfo.Extension,
                        type = fileInfo.ContentType,
                        size = file.ContentLength,
                        progress = "1.0",
                        url = FileManager.Instance.GetUrl(fileInfo),
                        thumbnail_url = fileIcon,
                        message = "success",
                        file_id = fileInfo.FileId,
                    });
                }
                else
                {
                    statuses.Add(new FilesStatus
                    {
                        success = false,
                        name = fileName,
                        message = "File type not allowed."
                    });
                }
            }
        }
    }
}