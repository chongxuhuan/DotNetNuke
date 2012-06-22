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
using System.Web.Mvc;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Modules.Groups.Components;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Social.Messaging.Internal;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Web.Services;
using DotNetNuke.Security;

namespace DotNetNuke.Modules.Groups
{
    [DnnAuthorize]
    public class ModerationServiceController : DnnController
    {
        internal int TabId { get; set; }
        internal int ModuleId { get; set; }
        internal int RoleId { get; set; }
        internal int MemberId { get; set; }
        internal RoleInfo roleInfo { get; set; }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveGroup(int notificationId)
        {
            try
            {
                var recipient = InternalMessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient == null) return Json(new { Result = "error" });

                var notification = NotificationsController.Instance.GetNotification(notificationId);
                ParseKey(notification.Context);
                if (roleInfo == null)
                {
                    return Json(new { Result = "error" });
                }
                if (!IsMod())
                {
                    return Json(new { Result = "access denied" });
                }
                var roleController = new RoleController();
                roleInfo.Status = RoleStatus.Approved;
                roleController.UpdateRole(roleInfo);
                var roleCreator = UserController.GetUserById(PortalSettings.PortalId, roleInfo.CreatedByUserID);
                //Update the original creator's role
                roleController.UpdateUserRole(PortalSettings.PortalId, roleCreator.UserID, roleInfo.RoleID, RoleStatus.Approved, true, false);
                GroupUtilities.CreateJournalEntry(roleInfo, roleCreator);

                var notifications = new Notifications();
                var siteAdmin = UserController.GetUserById(PortalSettings.PortalId, PortalSettings.AdministratorId);
                notifications.AddGroupNotification(Constants.GroupApprovedNotification, TabId, ModuleId, roleInfo, siteAdmin, new List<RoleInfo> { roleInfo });
                NotificationsController.Instance.DeleteAllNotificationRecipients(notificationId);

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
        public ActionResult RejectGroup(int notificationId)
        {
            try
            {
                var recipient = InternalMessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient == null) return Json(new { Result = "error" });

                var notification = NotificationsController.Instance.GetNotification(notificationId);
                ParseKey(notification.Context);
                if (roleInfo == null)
                {
                    return Json(new { Result = "error" });
                }
                if (!IsMod())
                {
                    return Json(new { Result = "access denied" });
                }
                var notifications = new Notifications();
                var roleCreator = UserController.GetUserById(PortalSettings.PortalId, roleInfo.CreatedByUserID);
                var siteAdmin = UserController.GetUserById(PortalSettings.PortalId, PortalSettings.AdministratorId);
                notifications.AddGroupNotification(Constants.GroupRejectedNotification, TabId, ModuleId, roleInfo, siteAdmin, new List<RoleInfo> { roleInfo }, roleCreator);

                var roleController = new RoleController();
                roleController.DeleteRole(RoleId, PortalSettings.PortalId);
                NotificationsController.Instance.DeleteAllNotificationRecipients(notificationId);
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
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [SupportedModules("Social Groups")]
        public ActionResult JoinGroup(int roleId)
        {
            try
            {
                if (UserInfo.UserID >= 0 && roleId > 0)
                {
                    var roleController = new RoleController();
                    roleInfo = roleController.GetRole(roleId, PortalSettings.PortalId);
                    if (roleInfo != null)
                    {
                        var requireApproval = Convert.ToBoolean(roleInfo.Settings["ReviewMembers"].ToString());
                        if (roleInfo.IsPublic && !requireApproval)
                        {
                            roleController.AddUserRole(PortalSettings.PortalId, UserInfo.UserID, roleInfo.RoleID, Null.NullDate);
                            roleController.UpdateRole(roleInfo);
                            return Json(new { Result = "success", URL = roleInfo.Settings["URL"] });
                        }
                        if (roleInfo.IsPublic && requireApproval)
                        {
                            roleController.AddUserRole(PortalSettings.PortalId, UserInfo.UserID, roleInfo.RoleID, RoleStatus.Pending, false, Null.NullDate, Null.NullDate);
                            Components.Notifications notifications = new Components.Notifications();
                            notifications.AddGroupOwnerNotification(Constants.MemberPendingNotification, TabId, ModuleId, roleInfo, UserInfo);
                            return Json(new { Result = "success", URL = string.Empty });
                        }

                        
                    }
                }
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
            }

            return Json(new { Result = "error" });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [SupportedModules("Social Groups")]
        public ActionResult LeaveGroup(int roleId)
        {
            var success = false;

            try
            {
                if (UserInfo.UserID >= 0 && roleId > 0)
                {
                    var roleController = new RoleController();
                    roleInfo = roleController.GetRole(roleId, PortalSettings.PortalId);

                    if (roleInfo != null)
                    {
                        if (UserInfo.IsInRole(roleInfo.RoleName))
                        {
                            RoleController.DeleteUserRole(UserInfo, roleInfo, PortalSettings, false);
                        }
                        success = true;
                    }
                }
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
            }

            return Json(new { Result = success ? "success" : "error" });
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveMember(int notificationId)
        {
            try
            {
                var recipient = InternalMessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient == null) return Json(new { Result = "error" });

                var notification = NotificationsController.Instance.GetNotification(notificationId);
                ParseKey(notification.Context);
                if (MemberId <= 0) return Json(new { Result = "error" });

                if (roleInfo == null) return Json(new { Result = "error" });

                var member = UserController.GetUserById(PortalSettings.PortalId, MemberId);

                

                if (member != null)
                {
                    var roleController = new RoleController();
                    var memberRoleInfo = roleController.GetUserRole(PortalSettings.PortalId, MemberId, roleInfo.RoleID);
                    memberRoleInfo.Status = RoleStatus.Approved;
                    roleController.UpdateUserRole(PortalSettings.PortalId, MemberId, roleInfo.RoleID, RoleStatus.Approved, false, false);
                    
                    var notifications = new Notifications();
                    var groupOwner = UserController.GetUserById(PortalSettings.PortalId, roleInfo.CreatedByUserID);
                    notifications.AddMemberNotification(Constants.MemberApprovedNotification, TabId, ModuleId, roleInfo, groupOwner, member);
                    NotificationsController.Instance.DeleteAllNotificationRecipients(notificationId);

                    return Json(new { Result = "success" });
                }
            } catch (Exception exc)
            {
                DnnLog.Error(exc);
            }

            return Json(new { Result = "error" });
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult RejectMember(int notificationId)
        {
            try
            {
                var recipient = InternalMessagingController.Instance.GetMessageRecipient(notificationId, UserInfo.UserID);
                if (recipient == null) return Json(new { Result = "error" });

                var notification = NotificationsController.Instance.GetNotification(notificationId);
                ParseKey(notification.Context);
                if (MemberId <= 0) return Json(new { Result = "error" });

                if (roleInfo == null) return Json(new { Result = "error" });

                var member = UserController.GetUserById(PortalSettings.PortalId, MemberId);



                if (member != null)
                {
                    var roleController = new RoleController();
                    roleController.DeleteUserRole(PortalSettings.PortalId, MemberId, roleInfo.RoleID);
                    var notifications = new Notifications();
                    var groupOwner = UserController.GetUserById(PortalSettings.PortalId, roleInfo.CreatedByUserID);
                    notifications.AddMemberNotification(Constants.MemberRejectedNotification, TabId, ModuleId, roleInfo, groupOwner, member);
                    NotificationsController.Instance.DeleteAllNotificationRecipients(notificationId);

                    return Json(new { Result = "success" });
                }
            } catch (Exception exc)
            {
                DnnLog.Error(exc);
            }

            return Json(new { Result = "error" });
        }
        public void ParseKey(string key)
        {
            TabId = -1;
            ModuleId = -1;
            RoleId = -1;
            MemberId = -1;
            roleInfo = null;
            if (!String.IsNullOrEmpty(key))
            {
                string[] keys = key.Split(':');
                TabId = Convert.ToInt32(keys[0]);
                ModuleId = Convert.ToInt32(keys[1]);
                RoleId = Convert.ToInt32(keys[2]);
                if (keys.Length > 3)
                {
                    MemberId = Convert.ToInt32(keys[3]);
                }
            }
            if (RoleId > 0)
            {
                var roleController = new RoleController();
                roleInfo = roleController.GetRole(RoleId, PortalSettings.PortalId);
            }
        }

        public bool IsMod()
        {
            return ModulePermissionController.HasModulePermission(ModuleId, "MODGROUP");
        }
    }
}