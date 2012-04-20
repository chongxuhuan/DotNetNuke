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
using System.Web.Mvc;

using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Users.Social;
using DotNetNuke.Services.Social.Messaging;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Web.Services;

namespace DotNetNuke.Web.CoreServices
{
    [DnnAuthorize]
    public class RelationshipServiceController : DnnController
    {
        #region Friend APIs

        public ActionResult AcceptFriend(int notificationId)
        {
            try
            {
                var recipient = MessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient != null)
                {
                    var action = NotificationsController.Instance.GetNotificationAction(notificationId, "Accept");
                    var key = action.Key;
                    int initiatingUserId;
                    if (int.TryParse(key, out initiatingUserId))
                    {
                        RelationshipController.Instance.AcceptUserRelationship(GetFriendUserRelationship(initiatingUserId).UserRelationshipId);
                        NotificationsController.Instance.DeleteNotificationRecipient(notificationId, UserInfo.UserID);

                        return Json(new { Result = "success" });
                    }
                }

                return Json(new { Result = "error" });
            }
            catch (Exception)
            {
                return Json(new { Result = "error" });
            }
        }

        public ActionResult RejectFriend(int notificationId)
        {
            try
            {
                var recipient = MessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient != null)
                {
                    var action = NotificationsController.Instance.GetNotificationAction(notificationId, "Reject");
                    var key = action.Key;
                    int initiatingUserId;
                    if (int.TryParse(key, out initiatingUserId))
                    {
                        RelationshipController.Instance.RejectUserRelationship(GetFriendUserRelationship(initiatingUserId).UserRelationshipId);
                        NotificationsController.Instance.DeleteNotificationRecipient(notificationId, UserInfo.UserID);

                        return Json(new { Result = "success" });
                    }
                }

                return Json(new { Result = "error" });
            }
            catch (Exception)
            {
                return Json(new { Result = "error" });
            }
        }

        public ActionResult IgnoreFriend(int notificationId)
        {
            try
            {
                var recipient = MessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient != null)
                {
                    var action = NotificationsController.Instance.GetNotificationAction(notificationId, "Ignore");
                    var key = action.Key;
                    int initiatingUserId;
                    if (int.TryParse(key, out initiatingUserId))
                    {
                        RelationshipController.Instance.IgnoreUserRelationship(GetFriendUserRelationship(initiatingUserId).UserRelationshipId);
                        NotificationsController.Instance.DeleteNotificationRecipient(notificationId, UserInfo.UserID);

                        return Json(new { Result = "success" });
                    }
                }

                return Json(new { Result = "error" });
            }
            catch (Exception)
            {
                return Json(new { Result = "error" });
            }
        }

        public ActionResult ReportFriend(int notificationId)
        {
            try
            {
                var recipient = MessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient != null)
                {
                    var action = NotificationsController.Instance.GetNotificationAction(notificationId, "Report");
                    var key = action.Key;
                    int initiatingUserId;
                    if (int.TryParse(key, out initiatingUserId))
                    {
                        RelationshipController.Instance.ReportUserRelationship(GetFriendUserRelationship(initiatingUserId).UserRelationshipId);
                        NotificationsController.Instance.DeleteNotificationRecipient(notificationId, UserInfo.UserID);

                        return Json(new { Result = "success" });
                    }
                }

                return Json(new { Result = "error" });
            }
            catch (Exception)
            {
                return Json(new { Result = "error" });
            }
        }

        public ActionResult BlockFriend(int notificationId)
        {
            try
            {
                var recipient = MessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient != null)
                {
                    var action = NotificationsController.Instance.GetNotificationAction(notificationId, "Block");
                    var key = action.Key;
                    int initiatingUserId;
                    if (int.TryParse(key, out initiatingUserId))
                    {
                        RelationshipController.Instance.BlockUserRelationship(GetFriendUserRelationship(initiatingUserId).UserRelationshipId);
                        NotificationsController.Instance.DeleteNotificationRecipient(notificationId, UserInfo.UserID);

                        return Json(new { Result = "success" });
                    }
                }

                return Json(new { Result = "error" });
            }
            catch (Exception)
            {
                return Json(new { Result = "error" });
            }
        }

        #endregion

        #region Follower APIs

        public ActionResult ReportFollower(int notificationId)
        {
            try
            {
                var recipient = MessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient != null)
                {
                    var action = NotificationsController.Instance.GetNotificationAction(notificationId, "Report");
                    var key = action.Key;
                    int initiatingUserId;
                    if (int.TryParse(key, out initiatingUserId))
                    {
                        RelationshipController.Instance.ReportUserRelationship(GetFollowerUserRelationship(initiatingUserId).UserRelationshipId);
                        NotificationsController.Instance.DeleteNotificationRecipient(notificationId, UserInfo.UserID);

                        return Json(new { Result = "success" });
                    }
                }

                return Json(new { Result = "error" });
            }
            catch (Exception)
            {
                return Json(new { Result = "error" });
            }
        }

        public ActionResult BlockFollower(int notificationId)
        {
            try
            {
                var recipient = MessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient != null)
                {
                    var action = NotificationsController.Instance.GetNotificationAction(notificationId, "Block");
                    var key = action.Key;
                    int initiatingUserId;
                    if (int.TryParse(key, out initiatingUserId))
                    {
                        RelationshipController.Instance.BlockUserRelationship(GetFollowerUserRelationship(initiatingUserId).UserRelationshipId);
                        NotificationsController.Instance.DeleteNotificationRecipient(notificationId, UserInfo.UserID);

                        return Json(new { Result = "success" });
                    }
                }

                return Json(new { Result = "error" });
            }
            catch (Exception)
            {
                return Json(new { Result = "error" });
            }
        }

        #endregion

        #region FollowBack APIS

        public ActionResult FollowBack(int notificationId)
        {
            try
            {
                var recipient = MessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient != null)
                {
                    var action = NotificationsController.Instance.GetNotificationAction(notificationId, "FollowBack");
                    var key = action.Key;
                    int targetUserId;
                    if (int.TryParse(key, out targetUserId))
                    {
                        var targetUser = UserController.GetUserById(PortalSettings.PortalId, targetUserId);

                        RelationshipController.Instance.FollowUser(targetUser);
                        NotificationsController.Instance.DeleteNotificationRecipient(notificationId, UserInfo.UserID);

                        return Json(new {Result = "success"});
                    }
                }

                return Json(new { Result = "error" });
            }
            catch (Exception)
            {
                return Json(new { Result = "error" });
            }
        }

        #endregion

        #region Private helpers

        private UserRelationship GetFriendUserRelationship(int initiatingUserId)
        {
            var initiatingUser = UserController.GetUserById(PortalSettings.PortalId, initiatingUserId);

            return RelationshipController.Instance.GetFriendRelationship(initiatingUser);
        }

        private UserRelationship GetFollowerUserRelationship(int initiatingUserId)
        {
            var initiatingUser = UserController.GetUserById(PortalSettings.PortalId, initiatingUserId);

            return RelationshipController.Instance.GetFollowerRelationship(initiatingUser);
        }

        #endregion
    }
}