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
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Instrumentation;
using DotNetNuke.Modules.Journal.Components;
using DotNetNuke.Security;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Journal;
using DotNetNuke.Services.Journal.Internal;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.Journal
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [SupportedModules("Journal")]
    [DnnAuthorize]
    [ValidateAntiForgeryToken]
    public class ServicesController : DnnApiController
    {
        public class CreateDTO
        {
            public string Text { get; set; }
            public int ProfileId { get; set; }
            public string JournalType { get; set; }
            public string ItemData { get; set; }
            public string SecuritySet { get; set; }
            public int GroupId { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage Create(CreateDTO postData)
        {
            try
            {
                var journalTypeId = 1;
                switch (postData.JournalType)
                {
                    case "link":
                        journalTypeId = 2;
                        break;
                    case "photo":
                        journalTypeId = 3;
                        break;
                    case "file":
                        journalTypeId = 4;
                        break;
                }

                if (postData.ProfileId == -1)
                {
                    postData.ProfileId = UserInfo.UserID;
                }

                if (postData.GroupId > 0)
                {
                    postData.ProfileId = -1;
                }
                
                var ji = new JournalItem
                {
                    JournalId = -1,
                    JournalTypeId = journalTypeId,
                    PortalId = PortalSettings.PortalId,
                    UserId = UserInfo.UserID,
                    SocialGroupId = postData.GroupId,
                    ProfileId = postData.ProfileId,
                    Summary = postData.Text,
                    SecuritySet = postData.SecuritySet
                };
                ji.Title = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(ji.Title));
                ji.Summary = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(ji.Summary));

                var ps = new PortalSecurity();

                ji.Title = ps.InputFilter(ji.Title, PortalSecurity.FilterFlag.NoScripting);
                ji.Title = Utilities.RemoveHTML(ji.Title);
                ji.Title = ps.InputFilter(ji.Title, PortalSecurity.FilterFlag.NoMarkup);

                ji.Summary = ps.InputFilter(ji.Summary, PortalSecurity.FilterFlag.NoScripting);
                ji.Summary = Utilities.RemoveHTML(ji.Summary);
                ji.Summary = ps.InputFilter(ji.Summary, PortalSecurity.FilterFlag.NoMarkup);

                if (ji.Summary.Length > 2000)
                {
                    ji.Body = ji.Summary;
                    ji.Summary = null;
                }

                if (!string.IsNullOrEmpty(postData.ItemData))
                {
                    ji.ItemData = postData.ItemData.FromJson<ItemData>();
                    ji.ItemData.Description = HttpUtility.UrlDecode(ji.ItemData.Description);

                    if (!string.IsNullOrEmpty(ji.ItemData.Url) && ji.ItemData.Url.StartsWith("fileid="))
                    {
                        var fileId = Convert.ToInt32(ji.ItemData.Url.Replace("fileid=", string.Empty).Trim());
                        var file = FileManager.Instance.GetFile(fileId);
                        ji.ItemData.Title = file.FileName;
                        ji.ItemData.Url = string.Format("{0}/LinkClick.aspx?fileticket={1}", Globals.ApplicationPath, UrlUtils.EncryptParameter(UrlUtils.GetParameterValue(ji.ItemData.Url)));
                    }
                }

                JournalController.Instance.SaveJournalItem(ji, 1);

                return Request.CreateResponse(HttpStatusCode.OK, ji);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }
        }

//        public class ListDTO
//        {
//            public int ProfileId { get; set; }
//        }

//        [DnnAuthorize]
//        [HttpGet]
//        public HttpResponseMessage List(ListDTO postData)
//        {
//            try
//            {
//                return Request.CreateResponse(HttpStatusCode.OK, InternalJournalController.Instance.GetJournalItemsByProfile(PortalSettings.PortalId, ActiveModule.ModuleID, UserInfo.UserID, postData.ProfileId, 0, 20));
//            }
//            catch (Exception exc)
//            {
//                DnnLog.Error(exc);
//                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
//            }
//        }

        public class JournalIdDTO
        {
            public int JournalId { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage Delete(JournalIdDTO postData)
        {
            try
            {
                var jc = JournalController.Instance;
                var ji = jc.GetJournalItem(PortalSettings.PortalId, UserInfo.UserID, postData.JournalId);

                if (ji == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "invalide request");
                }

                if (ji.UserId == UserInfo.UserID || UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    jc.DeleteJournalItem(PortalSettings.PortalId, UserInfo.UserID, postData.JournalId);
                    return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
                }

                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "access denied");
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }
        }

//        public class SearchStringDTO
//        {
//            public string Search { get; set; }
//        }
//
//
//        [HttpPost]
//        public HttpResponseMessage Users(SearchStringDTO postData)
//        {
//            try
//            {
//                var totalRecords = 0;
//                var list = new List<object>();
//                foreach (UserInfo u in UserController.GetUsersByUserName(PortalSettings.PortalId, postData.Search + '%', 0, 100, ref totalRecords))
//                {
//                    list.Add(new { label = u.DisplayName, value = u.UserID });
//                }
//                return Request.CreateResponse(HttpStatusCode.OK, list);
//            }
//            catch (Exception exc)
//            {
//                DnnLog.Error(exc);
//                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
//            }
//        }
//
//        [HttpPost]
//        public HttpResponseMessage Tags(SearchStringDTO postData)
//        {
//            try
//            {
//                var terms = Util.GetTermController().GetTermsByVocabulary(1).Where(t => t.Name.ToLower().Contains(postData.Search.ToLower())).Select(term => term.Name);
//
//                var list = new List<object>();
//                foreach (var t in terms)
//                {
//                    list.Add(new { label = t, value = t });
//                }
//
//                return Request.CreateResponse(HttpStatusCode.OK, terms);
//            }
//            catch (Exception exc)
//            {
//                DnnLog.Error(exc);
//                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
//            }
//        }

        public class PreviewDTO
        {
            public string Url { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage PreviewUrl(PreviewDTO postData)
        {
            try
            {
                var link = Utilities.GetLinkData(postData.Url);
                return Request.CreateResponse(HttpStatusCode.OK, link);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }
        }

//        [DnnAuthorize]
//        public HttpResponseMessage Upload()
//        {
//            var sourceFile = Request.Files[0];
//            if (sourceFile == null)
//            {
//                throw new HttpException(400, Localization.GetString("SaveFileError", "''"));
//            }
//
//            var userFolder = FolderManager.Instance.GetUserFolder(UserInfo);
//
//            var message = string.Empty;
//            IFileInfo fi = null;
//            try
//            {
//                fi = FileManager.Instance.AddFile(userFolder, sourceFile.FileName, sourceFile.InputStream, true);
//            }
//            catch (PermissionsNotMetException)
//            {
//                message = string.Format(Localization.GetString("InsufficientFolderPermission"), userFolder.FolderPath);
//            }
//            catch (NoSpaceAvailableException)
//            {
//                message = string.Format(Localization.GetString("DiskSpaceExceeded"), sourceFile.FileName);
//            }
//            catch (InvalidFileExtensionException)
//            {
//                message = string.Format(Localization.GetString("RestrictedFileType"), sourceFile.FileName, Host.AllowedExtensionWhitelist.ToDisplayString());
//            }
//            catch
//            {
//                message = string.Format(Localization.GetString("SaveFileError"), sourceFile.FileName);
//            }
//            if (String.IsNullOrEmpty(message) && fi != null)
//            {
//                return Json(fi);
//            }
//
//            return Json(new { Result = message });
//        }

        public class GetListForProfileDTO
        {
            public int ProfileId { get; set; }
            public int GroupId { get; set; }
            public int RowIndex { get; set; }
            public int MaxRows { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage GetListForProfile(GetListForProfileDTO postData)
        {
            try
            {
                var jp = new JournalParser(PortalSettings, ActiveModule.ModuleID, postData.ProfileId, postData.GroupId, UserInfo);
                return Request.CreateResponse(HttpStatusCode.OK, jp.GetList(postData.RowIndex, postData.MaxRows), "text/html");
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                throw new HttpException(500, exc.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage Like(JournalIdDTO postData)
        {
            try
            {
                InternalJournalController.Instance.LikeJournalItem(postData.JournalId, UserInfo.UserID, UserInfo.DisplayName);
                var ji = JournalController.Instance.GetJournalItem(PortalSettings.PortalId, UserInfo.UserID, postData.JournalId);
                var jp = new JournalParser(PortalSettings, ActiveModule.ModuleID, ji.ProfileId, -1, UserInfo);
                var isLiked = false;
                var likeList = jp.GetLikeListHTML(ji, ref isLiked);
                likeList = Utilities.LocalizeControl(likeList);
                return Request.CreateResponse(HttpStatusCode.OK, new { LikeList = likeList, Liked = isLiked });
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }
        }

        public class CommentSaveDTO
        {
            public int JournalId { get; set; }
            public string Comment { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage CommentSave(CommentSaveDTO postData)
        {
            try
            {
                var ci = new CommentInfo { JournalId = postData.JournalId, Comment = HttpUtility.UrlDecode(postData.Comment) };
                if (ci.Comment.Length > 2000)
                {
                    ci.Comment = ci.Comment.Substring(0, 1999);
                    ci.Comment = Utilities.RemoveHTML(ci.Comment);
                }
                ci.UserId = UserInfo.UserID;
                InternalJournalController.Instance.SaveComment(ci);

                var ji = JournalController.Instance.GetJournalItem(PortalSettings.PortalId, UserInfo.UserID, postData.JournalId);
                var jp = new JournalParser(PortalSettings, ActiveModule.ModuleID, ji.ProfileId, -1, UserInfo);

                return Request.CreateResponse(HttpStatusCode.OK, jp.GetCommentRow(ci), "text/html");
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }
        }

        public class CommentDeleteDTO
        {
            public int JournalId { get; set; }
            public int CommentId { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage CommentDelete(CommentDeleteDTO postData)
        {
            try
            {
                var ci = InternalJournalController.Instance.GetComment(postData.CommentId);
                if (ci == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "delete failed");
                }
                if (ci.UserId == UserInfo.UserID || UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    InternalJournalController.Instance.DeleteComment(postData.JournalId, postData.CommentId);
                    return Request.CreateResponse(HttpStatusCode.OK, new { Result = "success" });
                }

                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "access denied");
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }
        }
    }
}