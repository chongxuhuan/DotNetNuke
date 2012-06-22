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

using System.Collections.Generic;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Social.Notifications;

namespace DotNetNuke.Modules.Groups.Components
{
    public class GroupsBusinessController : IUpgradeable
    {
        public string UpgradeModule(string version)
        {
            switch (version)
            {
                case "06.02.00":
                    AddNotificationTypes();
                    break;
            }

            return string.Empty;
        }

        private void AddNotificationTypes()
        {
            var actions = new List<NotificationTypeAction>();
            
            //DesktopModule should not be null
            var deskModuleId = DesktopModuleController.GetDesktopModuleByFriendlyName("Social Groups").DesktopModuleID;

            //GroupPendingNotification
            var type = new NotificationType { Name = "GroupPendingNotification", Description = "Group Pending Notification", DesktopModuleId = deskModuleId};
            if (NotificationsController.Instance.GetNotificationType(type.Name) == null)
            {
                actions.Add(new NotificationTypeAction
                                {
                                    NameResourceKey = "Approve",
                                    DescriptionResourceKey = "ApproveGroup",
                                    APICall = "DesktopModules/SocialGroups/API/ModerationService.ashx/ApproveGroup"
                                });
                actions.Add(new NotificationTypeAction
                                {
                                    NameResourceKey = "RejectGroup",
                                    DescriptionResourceKey = "RejectGroup",
                                    APICall = "DesktopModules/SocialGroups/API/ModerationService.ashx/RejectGroup"
                                });
                NotificationsController.Instance.CreateNotificationType(type);
                NotificationsController.Instance.SetNotificationTypeActions(actions, type.NotificationTypeId);
            }

            //GroupApprovedNotification
            type = new NotificationType { Name = "GroupApprovedNotification", Description = "Group Approved Notification", DesktopModuleId = deskModuleId };
            if (NotificationsController.Instance.GetNotificationType(type.Name) == null)
            {
                NotificationsController.Instance.CreateNotificationType(type);
            }

            //GroupCreatedNotification
            type = new NotificationType { Name = "GroupCreatedNotification", Description = "Group Created Notification", DesktopModuleId = deskModuleId };
            if (NotificationsController.Instance.GetNotificationType(type.Name) == null)
            {
                actions.Clear();
                actions.Add(new NotificationTypeAction
                                {
                                    NameResourceKey = "RejectGroup",
                                    DescriptionResourceKey = "RejectGroup",
                                    ConfirmResourceKey = "DeleteItem",
                                    APICall = "DesktopModules/SocialGroups/API/ModerationService.ashx/RejectGroup"
                                });
                NotificationsController.Instance.CreateNotificationType(type);
                NotificationsController.Instance.SetNotificationTypeActions(actions, type.NotificationTypeId);
            }

            //GroupRejectedNotification
            type = new NotificationType { Name = "GroupRejectedNotification", Description = "Group Rejected Notification", DesktopModuleId = deskModuleId };
            if (NotificationsController.Instance.GetNotificationType(type.Name) == null)
            {
                NotificationsController.Instance.CreateNotificationType(type);
            }

            //GroupMemberPendingNotification
            type = new NotificationType { Name = "GroupMemberPendingNotification", Description = "Group Member Pending Notification", DesktopModuleId = deskModuleId };
            if (NotificationsController.Instance.GetNotificationType(type.Name) == null)
            {
                actions.Clear();
                actions.Add(new NotificationTypeAction
                                {
                                    NameResourceKey = "Approve",
                                    DescriptionResourceKey = "ApproveGroupMember",
                                    ConfirmResourceKey = "",
                                    APICall = "DesktopModules/SocialGroups/API/ModerationService.ashx/ApproveMember"
                                });
                actions.Add(new NotificationTypeAction
                                {
                                    NameResourceKey = "RejectMember",
                                    DescriptionResourceKey = "RejectGroupMember",
                                    APICall = "DesktopModules/SocialGroups/API/ModerationService.ashx/RejectMember"
                                });
                NotificationsController.Instance.CreateNotificationType(type);
                NotificationsController.Instance.SetNotificationTypeActions(actions, type.NotificationTypeId);
            }

            //GroupMemberApprovedNotification
            type = new NotificationType { Name = "GroupMemberApprovedNotification", Description = "Group Member Notification", DesktopModuleId = deskModuleId };
            if (NotificationsController.Instance.GetNotificationType(type.Name) == null)
            {
                NotificationsController.Instance.CreateNotificationType(type);
            }

            //GroupMemberRejectedNotification
            type = new NotificationType { Name = "GroupMemberRejectedNotification", Description = "Group Rejected Notification", DesktopModuleId = deskModuleId };
            if (NotificationsController.Instance.GetNotificationType(type.Name) == null)
            {
                NotificationsController.Instance.CreateNotificationType(type);
            }
        }
    }
}