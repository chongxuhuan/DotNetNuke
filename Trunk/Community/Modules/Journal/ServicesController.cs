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
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Modules.Journal.Components;
using DotNetNuke.Security;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Journal;
using DotNetNuke.Services.Journal.Internal;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Services;

namespace DotNetNuke.Modules.Journal
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [SupportedModules("Journal")]
    [DnnAuthorize]
    [ValidateAntiForgeryToken]
    public class ServicesController : DnnController
    {
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string text, int profileId, string journalType, string itemData, string securitySet, int groupId)
        {
            try
            {
                var journalTypeId = 1;
                switch (journalType)
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

                if (profileId == -1)
                {
                    profileId = UserInfo.UserID;
                }

                if (groupId > 0)
                {
                    profileId = -1;
                }
                
                var ji = new JournalItem
                {
                    JournalId = -1,
                    JournalTypeId = journalTypeId,
                    PortalId = PortalSettings.PortalId,
                    UserId = UserInfo.UserID,
                    SocialGroupId = groupId,
                    ProfileId = profileId,
                    Summary = text,
                    SecuritySet = securitySet
                };
                ji.Title = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(ji.Title));
                ji.Summary = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(ji.Summary));

                Security.PortalSecurity ps = new Security.PortalSecurity();

                ji.Title = ps.InputFilter(ji.Title, Security.PortalSecurity.FilterFlag.NoScripting);
                ji.Title = Utilities.RemoveHTML(ji.Title);
                ji.Title = ps.InputFilter(ji.Title, Security.PortalSecurity.FilterFlag.NoMarkup);

                ji.Summary = ps.InputFilter(ji.Summary, Security.PortalSecurity.FilterFlag.NoScripting);
                ji.Summary = Utilities.RemoveHTML(ji.Summary);
                ji.Summary = ps.InputFilter(ji.Summary, Security.PortalSecurity.FilterFlag.NoMarkup);

                if (ji.Summary.Length > 2000)
                {
                    ji.Body = ji.Summary;
                    ji.Summary = null;
                }

                if (!string.IsNullOrEmpty(itemData))
                {
                    ji.ItemData = itemData.FromJson<ItemData>();
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

                return Json(ji);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [DnnAuthorize]
        public ActionResult List(int profileId)
        {
            try
            {
                return Json(InternalJournalController.Instance.GetJournalItemsByProfile(PortalSettings.PortalId, ActiveModule.ModuleID, UserInfo.UserID, profileId, 0, 20));
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int journalId)
        {
            try
            {
                var jc = JournalController.Instance;
                var ji = jc.GetJournalItem(PortalSettings.PortalId, UserInfo.UserID, journalId);

                if (ji == null)
                {
                    return Json(new { Result = "invalid request" }, JsonRequestBehavior.AllowGet);
                }

                if (ji.UserId == UserInfo.UserID || UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    jc.DeleteJournalItem(PortalSettings.PortalId, UserInfo.UserID, journalId);
                    return Json(new { Result = "success" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Result = "access denied" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Users(string search)
        {
            try
            {
                var totalRecords = 0;
                var list = new List<object>();
                foreach (UserInfo u in UserController.GetUsersByUserName(PortalSettings.PortalId, search + '%', 0, 100, ref totalRecords))
                {
                    list.Add(new { label = u.DisplayName, value = u.UserID });
                }
                return Json(list);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Tags(string search)
        {
            try
            {
                var terms = Util.GetTermController().GetTermsByVocabulary(1).Where(t => t.Name.ToLower().Contains(search.ToLower())).Select(term => term.Name);

                var list = new List<object>();
                foreach (var t in terms)
                {
                    list.Add(new { label = t, value = t });
                }

                return Json(terms);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PreviewURL(string url)
        {
            try
            {
                var link = Utilities.GetLinkData(url);
                return Json(link);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [DnnAuthorize]
        public ActionResult Upload()
        {
            var sourceFile = Request.Files[0];
            if (sourceFile == null)
            {
                throw new HttpException(400, Localization.GetString("SaveFileError", "''"));
            }

            var userFolder = FolderManager.Instance.GetUserFolder(UserInfo);

            var message = string.Empty;
            IFileInfo fi = null;
            try
            {
                fi = FileManager.Instance.AddFile(userFolder, sourceFile.FileName, sourceFile.InputStream, true);
            }
            catch (PermissionsNotMetException)
            {
                message = string.Format(Localization.GetString("InsufficientFolderPermission"), userFolder.FolderPath);
            }
            catch (NoSpaceAvailableException)
            {
                message = string.Format(Localization.GetString("DiskSpaceExceeded"), sourceFile.FileName);
            }
            catch (InvalidFileExtensionException)
            {
                message = string.Format(Localization.GetString("RestrictedFileType"), sourceFile.FileName, Host.AllowedExtensionWhitelist.ToDisplayString());
            }
            catch
            {
                message = string.Format(Localization.GetString("SaveFileError"), sourceFile.FileName);
            }
            if (String.IsNullOrEmpty(message) && fi != null)
            {
                return Json(fi);
            }

            return Json(new { Result = message }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string GetListForProfile(int profileId, int groupId, int rowIndex, int maxRows)
        {
            try
            {
                var jp = new JournalParser(PortalSettings, ActiveModule.ModuleID, profileId, groupId, UserInfo);
                return jp.GetList(rowIndex, maxRows);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return string.Empty;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Like(int journalId)
        {
            try
            {
                InternalJournalController.Instance.LikeJournalItem(journalId, UserInfo.UserID, UserInfo.DisplayName);
                var ji = JournalController.Instance.GetJournalItem(PortalSettings.PortalId, UserInfo.UserID, journalId);
                var jp = new JournalParser(PortalSettings, ActiveModule.ModuleID, ji.ProfileId, -1, UserInfo);
                var isLiked = false;
                var likeList = jp.GetLikeListHTML(ji, ref isLiked);
                likeList = Utilities.LocalizeControl(likeList);
                return Json(new { LikeList = likeList, Liked = isLiked }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string CommentSave(int journalId, string comment)
        {
            try
            {
                var ci = new CommentInfo { JournalId = journalId, Comment = HttpUtility.UrlDecode(comment) };
                if (ci.Comment.Length > 2000)
                {
                    ci.Comment = ci.Comment.Substring(0, 1999);
                    ci.Comment = Utilities.RemoveHTML(ci.Comment);
                }
                ci.UserId = UserInfo.UserID;
                InternalJournalController.Instance.SaveComment(ci);

                var ji = JournalController.Instance.GetJournalItem(PortalSettings.PortalId, UserInfo.UserID, journalId);
                var jp = new JournalParser(PortalSettings, ActiveModule.ModuleID, ji.ProfileId, -1, UserInfo);
                return jp.GetCommentRow(ci);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return string.Empty;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommentDelete(int journalId, int commentId)
        {
            try
            {
                var ci = InternalJournalController.Instance.GetComment(commentId);
                if (ci == null)
                {
                    return Json(new { Result = "delete failed" }, JsonRequestBehavior.AllowGet);
                }
                if (ci.UserId == UserInfo.UserID || UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    InternalJournalController.Instance.DeleteComment(journalId, commentId);
                    return Json(new { Result = "success" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Result = "access denied" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [DnnAuthorize]
        public ActionResult CommentLike(int journalId, int commentId)
        {
            try
            {
                InternalJournalController.Instance.LikeComment(journalId, commentId, UserInfo.UserID, UserInfo.DisplayName);
                var ji = JournalController.Instance.GetJournalItem(PortalSettings.PortalId, UserInfo.UserID, journalId);
                var jp = new JournalParser(PortalSettings, ActiveModule.ModuleID, ji.ProfileId, -1, UserInfo);
                var isLiked = false;
                var likeList = jp.GetLikeListHTML(ji, ref isLiked);
                likeList = Utilities.LocalizeControl(likeList);
                return Json(new { LikeList = likeList, Liked = isLiked }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }
    }
}