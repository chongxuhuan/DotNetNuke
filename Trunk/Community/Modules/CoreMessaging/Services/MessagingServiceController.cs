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
using System.Web;
using System.Web.Mvc;

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Modules.CoreMessaging.ViewModels;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Social.Messaging.Internal;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Web.Services;

namespace DotNetNuke.Modules.CoreMessaging.Services
{
    [SupportedModules("DotNetNuke.Modules.CoreMessaging")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [DnnAuthorize]
    public class MessagingServiceController : DnnController
    {
        #region Public Methods

        public ActionResult Inbox(int afterMessageId, int numberOfRecords)
        {
            try
            {
                var messageBoxView = InternalMessagingController.Instance.GetRecentInbox(UserInfo.UserID, afterMessageId, numberOfRecords);

                messageBoxView.TotalNewThreads = InternalMessagingController.Instance.CountUnreadMessages(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID);
                messageBoxView.TotalConversations = InternalMessagingController.Instance.CountConversations(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID);

                return Json(messageBoxView, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Sentbox(int afterMessageId, int numberOfRecords)
        {
            try
            {
                var messageBoxView = InternalMessagingController.Instance.GetRecentSentbox(UserInfo.UserID, afterMessageId, numberOfRecords);

                messageBoxView.TotalNewThreads = InternalMessagingController.Instance.CountUnreadMessages(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID);
                messageBoxView.TotalConversations = InternalMessagingController.Instance.CountSentMessages(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID);

                return Json(messageBoxView, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Archived(int afterMessageId, int numberOfRecords)
        {
            try
            {
                var messageBoxView = InternalMessagingController.Instance.GetArchivedMessages(UserInfo.UserID, afterMessageId, numberOfRecords);

                messageBoxView.TotalNewThreads = InternalMessagingController.Instance.CountUnreadMessages(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID);
                messageBoxView.TotalConversations = InternalMessagingController.Instance.CountArchivedMessages(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID);

                return Json(messageBoxView, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Thread(int conversationId, int afterMessageId, int numberOfRecords)
        {
            try
            {
                var totalRecords = 0;
                var messageThreadsView = InternalMessagingController.Instance.GetMessageThread(conversationId, UserInfo.UserID, afterMessageId, numberOfRecords, ref totalRecords);

                messageThreadsView.TotalNewThreads = InternalMessagingController.Instance.CountUnreadMessages(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID);
                messageThreadsView.TotalThreads = InternalMessagingController.Instance.CountMessagesByConversation(conversationId);
                messageThreadsView.TotalArchivedThreads = InternalMessagingController.Instance.CountArchivedMessagesByConversation(conversationId);

                return Json(messageThreadsView, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(int conversationId, string body, IList<int> fileIds)
        {
            try
            {
                body = HttpUtility.UrlDecode(body);
                var messageId = InternalMessagingController.Instance.ReplyMessage(conversationId, body, fileIds);
                var message = InternalMessagingController.Instance.GetMessage(messageId);

                var totalNewThreads = InternalMessagingController.Instance.CountUnreadMessages(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID);
                var totalThreads = InternalMessagingController.Instance.CountMessagesByConversation(conversationId);
                var totalArchivedThreads = InternalMessagingController.Instance.CountArchivedMessagesByConversation(conversationId);

                return Json(new { Conversation = message, TotalNewThreads = totalNewThreads, TotalThreads = totalThreads, TotalArchivedThreads = totalArchivedThreads });
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(null);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult MarkArchived(int conversationId)
        {
            try
            {
                InternalMessagingController.Instance.MarkArchived(conversationId, UserInfo.UserID);
                return Json(new { Result = "success" });
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult MarkUnArchived(int conversationId)
        {
            try
            {
                InternalMessagingController.Instance.MarkUnArchived(conversationId, UserInfo.UserID);
                return Json(new { Result = "success" });
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult MarkRead(int conversationId)
        {
            try
            {
                InternalMessagingController.Instance.MarkRead(conversationId, UserInfo.UserID);
                return Json(new { Result = "success" });
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult MarkUnRead(int conversationId)
        {
            try
            {
                InternalMessagingController.Instance.MarkUnRead(conversationId, UserInfo.UserID);
                return Json(new { Result = "success" });
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        public ActionResult Notifications(int afterNotificationId, int numberOfRecords)
        {
            try
            {
                var notificationsDomainModel = NotificationsController.Instance.GetNotifications(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID, afterNotificationId, numberOfRecords);

                var notificationsViewModel = new NotificationsViewModel
                {
                    TotalNotifications = NotificationsController.Instance.CountNotifications(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID),
                    Notifications = new List<NotificationViewModel>(notificationsDomainModel.Count)
                };

                foreach (var notification in notificationsDomainModel)
                {
                    var notificationViewModel = new NotificationViewModel
                    {
                        NotificationId = notification.NotificationID,
                        Subject = notification.Subject,
                        From = notification.From,
                        Body = notification.Body,
                        DisplayDate = Common.Utilities.DateUtils.CalculateDateForDisplay(notification.CreatedOnDate),
                        SenderAvatar = string.Format(Globals.UserProfilePicFormattedUrl(), notification.SenderUserID, 32, 32),
                        SenderProfileUrl = Globals.UserProfileURL(notification.SenderUserID),
                        Actions = new List<NotificationActionViewModel>()
                    };

                    var notificationType = NotificationsController.Instance.GetNotificationType(notification.NotificationTypeID);
                    var notificationTypeActions = NotificationsController.Instance.GetNotificationTypeActions(notification.NotificationTypeID);

                    foreach (var notificationTypeAction in notificationTypeActions)
                    {
                        var notificationActionViewModel = new NotificationActionViewModel
                        {
                            Name = LocalizeActionString(notificationTypeAction.NameResourceKey, notificationType.DesktopModuleId),
                            Description = LocalizeActionString(notificationTypeAction.DescriptionResourceKey, notificationType.DesktopModuleId),
                            Confirm = LocalizeActionString(notificationTypeAction.ConfirmResourceKey, notificationType.DesktopModuleId),
                            APICall = notificationTypeAction.APICall
                        };

                        notificationViewModel.Actions.Add(notificationActionViewModel);
                    }

                    if (notification.IncludeDismissAction)
                    {
                        notificationViewModel.Actions.Add(new NotificationActionViewModel
                        {
                            Name = Localization.GetString("Dismiss.Text"),
                            Description = Localization.GetString("DismissNotification.Text"),
                            Confirm = "",
                            APICall = "DesktopModules/InternalServices/API/NotificationsService.ashx/Dismiss"
                        });
                    }

                    notificationsViewModel.Notifications.Add(notificationViewModel);
                }

                return Json(notificationsViewModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CountNotifications()
        {
            try
            {
                var notifications = NotificationsController.Instance.CountNotifications(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID);
                return Json(notifications, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CountUnreadMessages()
        {
            try
            {
                var unreadMessages = InternalMessagingController.Instance.CountUnreadMessages(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID);
                return Json(unreadMessages, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTotals()
        {
            try
            {
                var totalsViewModel = new TotalsViewModel
                {
                    TotalUnreadMessages = InternalMessagingController.Instance.CountUnreadMessages(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID),
                    TotalNotifications = NotificationsController.Instance.CountNotifications(UserInfo.UserID, UserController.GetCurrentUserInfo().PortalID)
                };

                return Json(totalsViewModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Private Methods

        private string LocalizeActionString(string key, int desktopModuleId)
        {
            if (string.IsNullOrEmpty(key)) return "";

            string actionString;

            if (desktopModuleId > 0)
            {
                var desktopModule = DesktopModuleController.GetDesktopModule(desktopModuleId, PortalSettings.PortalId);

                var resourceFile = string.Format("~/DesktopModules/{0}/{1}/{2}",
                    desktopModule.FolderName.Replace("\\", "/"),
                    Localization.LocalResourceDirectory,
                    Localization.LocalSharedResourceFile);

                actionString = Localization.GetString(key, resourceFile);
            }
            else
            {
                actionString = Localization.GetString(key);
            }

            return string.IsNullOrEmpty(actionString) ? key : actionString;
        }

        #endregion
    }
}
