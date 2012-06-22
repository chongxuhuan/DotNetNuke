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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Users.Internal;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Roles.Internal;
using DotNetNuke.Web.Services;
using DotNetNuke.Entities.Users.Social;

namespace DotNetNuke.Modules.MemberDirectory.Services
{
    [SupportedModules("DotNetNuke.Modules.MemberDirectory")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class MemberDirectoryController : DnnController
    {
        #region Private Methods

        private static void AddSearchTerm(ref string propertyNames, ref string propertyValues, string name, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                propertyNames += name + ",";
                propertyValues += value + ",";
            }
        }

        private bool CanViewGroupMembers(int portalId, int groupId)
        {
            var group = TestableRoleController.Instance.GetRole(portalId, r => r.RoleID == groupId);
            var canView = (@group.SecurityMode == SecurityMode.SecurityRole)
                               ? (PortalSettings.UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                               : (PortalSettings.UserInfo.IsInRole(@group.RoleName));
            return canView;
        }

        private IList<Member> GetMembers(IEnumerable<UserInfo> users)
        {
            return users.Select(user => new Member(user, PortalSettings)).ToList();
        }

        private static string GetSetting(IDictionary settings, string key, string defaultValue)
        {
            var setting = defaultValue;
            if (settings[key] != null)
            {
                setting = Convert.ToString(settings[key]);
            }
            return setting;
        }

        private IEnumerable<UserInfo> GetUsers(int groupId, string searchTerm, int pageIndex, int pageSize)
        {
            var portalId = PortalSettings.PortalId;
            var userId = PortalSettings.UserId;
            var isAdmin = PortalSettings.UserInfo.IsInRole(PortalSettings.AdministratorRoleName);

            var filterBy = GetSetting(ActiveModule.ModuleSettings, "FilterBy", String.Empty);
            var filterValue = GetSetting(ActiveModule.ModuleSettings, "FilterValue", String.Empty);

            var sortField = GetSetting(ActiveModule.TabModuleSettings, "SortField", "DisplayName");
            var sortOrder = GetSetting(ActiveModule.TabModuleSettings, "SortOrder", "ASC");

            IList<UserInfo> users;
            switch (filterBy)
            {
                case "Group":
                    if (groupId == -1)
                    {
                        groupId = Int32.Parse(filterValue);
                    }
                    if (CanViewGroupMembers(portalId, groupId))
                    {
                        users = TestableUserController.Instance.GetUsersAdvancedSearch(portalId, userId, -1,
                                                                                       Int32.Parse(filterValue),
                                                                                       -1, isAdmin, pageIndex, pageSize,
                                                                                       sortField, (sortOrder == "ASC"),
                                                                                       "DisplayName", searchTerm);
                    }
                    else
                    {
                        users = new List<UserInfo>();
                    }
                    break;
                case "Relationship":
                    users = TestableUserController.Instance.GetUsersAdvancedSearch(portalId, userId, userId, -1,
                                                                           Int32.Parse(filterValue), isAdmin, pageIndex, pageSize,
                                                                           sortField, (sortOrder == "ASC"),
                                                                           "DisplayName", searchTerm);
                    break;
                case "ProfileProperty":
                    var propertyNames = "DisplayName,";
                    var propertyValues = searchTerm + ",";
                    var propertyValue = GetSetting(ActiveModule.ModuleSettings, "FilterPropertyValue", String.Empty);
                    AddSearchTerm(ref propertyNames, ref propertyValues, filterValue, propertyValue);

                    users = TestableUserController.Instance.GetUsersAdvancedSearch(portalId, userId, -1, -1,
                                                                           -1, isAdmin, pageIndex, pageSize,
                                                                           sortField, (sortOrder == "ASC"),
                                                                           propertyNames, propertyValues);
                    break;
                default:
                    users = TestableUserController.Instance.GetUsersBasicSearch(PortalSettings.PortalId, pageIndex, pageSize,
                                                                           sortField, (sortOrder == "ASC"),
                                                                           "DisplayName", searchTerm);
                    break;
            }
            return users;
        }

        #endregion

        #region Public Methods

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult AdvancedSearch(int userId, int groupId, int pageIndex, int pageSize, string searchTerm1, string searchTerm2, string searchTerm3, string searchTerm4)
        {
            try
            {
                var portalId = PortalSettings.PortalId;

                if (userId < 0) userId = PortalSettings.UserId;
                var isAdmin = PortalSettings.UserInfo.IsInRole(PortalSettings.AdministratorRoleName);

                var filterBy = GetSetting(ActiveModule.ModuleSettings, "FilterBy", String.Empty);
                var filterValue = GetSetting(ActiveModule.ModuleSettings, "FilterValue", String.Empty);
                var searchField1 = GetSetting(ActiveModule.TabModuleSettings, "SearchField1", "DisplayName");
                var searchField2 = GetSetting(ActiveModule.TabModuleSettings, "SearchField2", "Email");
                var searchField3 = GetSetting(ActiveModule.TabModuleSettings, "SearchField3", "City");
                var searchField4 = GetSetting(ActiveModule.TabModuleSettings, "SearchField4", "Country");

                var propertyNames = "";
                var propertyValues = "";

                AddSearchTerm(ref propertyNames, ref propertyValues, searchField1, searchTerm1);
                AddSearchTerm(ref propertyNames, ref propertyValues, searchField2, searchTerm2);
                AddSearchTerm(ref propertyNames, ref propertyValues, searchField3, searchTerm3);
                AddSearchTerm(ref propertyNames, ref propertyValues, searchField4, searchTerm4);

                if (filterBy == "ProfileProperty")
                {
                    var propertyValue = GetSetting(ActiveModule.ModuleSettings, "FilterPropertyValue", String.Empty);
                    AddSearchTerm(ref propertyNames, ref propertyValues, filterValue, propertyValue);
                }

                propertyNames = propertyNames.TrimEnd(',');
                propertyValues = propertyValues.TrimEnd(',');

                var sortField = GetSetting(ActiveModule.TabModuleSettings, "SortField", "DisplayName");
                var sortOrder = GetSetting(ActiveModule.TabModuleSettings, "SortOrder", "ASC");

                IList<UserInfo> users;
                switch (filterBy)
                {
                    case "User":
                        users = new List<UserInfo> { UserController.GetUserById(portalId, userId) };
                        break;
                    case "Group":
                        if (groupId == -1)
                        {
                            groupId = Int32.Parse(filterValue);
                        }
                        if (CanViewGroupMembers(portalId, groupId))
                        {
                            users = TestableUserController.Instance.GetUsersAdvancedSearch(portalId, PortalSettings.UserId, -1, groupId,
                                                                                   -1, isAdmin, pageIndex, pageSize,
                                                                                   sortField, (sortOrder == "ASC"),
                                                                                   propertyNames, propertyValues);
                        }
                        else
                        {
                            users = new List<UserInfo>();
                        }
                        break;
                    case "Relationship":
                        users = TestableUserController.Instance.GetUsersAdvancedSearch(portalId, PortalSettings.UserId, userId, -1,
                                                                               Int32.Parse(filterValue), isAdmin, pageIndex, pageSize,
                                                                               sortField, (sortOrder == "ASC"),
                                                                               propertyNames, propertyValues);
                        break;
                    default:
                        users = TestableUserController.Instance.GetUsersAdvancedSearch(portalId, PortalSettings.UserId, -1, -1,
                                                                               -1, isAdmin, pageIndex, pageSize,
                                                                               sortField, (sortOrder == "ASC"),
                                                                               propertyNames, propertyValues);
                        break;
                }
                return Json(GetMembers(users), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult BasicSearch(int groupId, string searchTerm, int pageIndex, int pageSize)
        {
            try
            {
                var users = GetUsers(groupId, searchTerm.Trim(), pageIndex, pageSize);
                var members = GetMembers(users);
                return Json(members, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetMember(string userName)
        {
            try
            {
                var users = new List<UserInfo>();
                var user = UserController.GetUserByName(PortalSettings.PortalId, userName);
                users.Add(user);

                return Json(GetMembers(users), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetSuggestions(int groupId, string displayName)
        {
            try
            {
                var names = (from UserInfo user in GetUsers(groupId, displayName.Trim(), 0, 10)
                             select new { label = user.DisplayName, value = user.DisplayName, userName = user.Username })
                                .ToList();

                return Json(names, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptFriend(int friendId)
        {
            try
            {
                var friend = UserController.GetUserById(PortalSettings.PortalId, friendId);
                FriendsController.Instance.AcceptFriend(friend);
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
        public ActionResult AddFriend(int friendId)
        {
            try
            {
                var friend = UserController.GetUserById(PortalSettings.PortalId, friendId);
                FriendsController.Instance.AddFriend(friend);
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
        public ActionResult Follow(int followId)
        {
            try
            {
                var follow = UserController.GetUserById(PortalSettings.PortalId, followId);
                FollowersController.Instance.FollowUser(follow);
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
        public ActionResult RemoveFriend(int friendId)
        {
            try
            {
                var friend = UserController.GetUserById(PortalSettings.PortalId, friendId);
                FriendsController.Instance.DeleteFriend(friend);
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
        public ActionResult UnFollow(int followId)
        {
            try
            {
                var follow = UserController.GetUserById(PortalSettings.PortalId, followId);
                FollowersController.Instance.UnFollowUser(follow);
                return Json(new { Result = "success" });
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(new { Result = "error" });
            }
        }

        #endregion
    }
}