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
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Users.Internal;
using DotNetNuke.Security.Roles.Internal;
using DotNetNuke.Services.Social.Messaging;
using DotNetNuke.Web.Services;

namespace DotNetNuke.Web.CoreServices
{
    public class MessagingServiceController : DnnController
    {
        [DnnAuthorize]
        public ActionResult WaitTimeForNextMessage()
        {
            return Json(MessagingController.Instance.WaitTimeForNextMessage(UserInfo), JsonRequestBehavior.AllowGet);
        }

        [DnnAuthorize]
        public ActionResult Create(string subject, string body, string roleIds, string userIds, string fileIds)
        {
            var roleIdsList = string.IsNullOrEmpty(roleIds) ? null : roleIds.FromJson<IList<int>>();
            var userIdsList = string.IsNullOrEmpty(userIds) ? null : userIds.FromJson<IList<int>>();
            var fileIdsList = string.IsNullOrEmpty(fileIds) ? null : fileIds.FromJson<IList<int>>();

            var roles = roleIdsList != null && roleIdsList.Count > 0
                ? roleIdsList.Select(id => TestableRoleController.Instance.GetRole(PortalSettings.PortalId, r => r.RoleID == id)).Where(role => role != null).ToList()
                : null;

            List<UserInfo> users = null;
            if (userIdsList != null)
            {
                var userController = new UserController();
                users = userIdsList.Select(id => userController.GetUser(PortalSettings.PortalId, id)).Where(user => user != null).ToList();
            }

            var message = MessagingController.Instance.CreateMessage(HttpUtility.UrlDecode(subject), HttpUtility.UrlDecode(body), roles, users, fileIdsList);

            return Json(message.MessageID);
        }

        [DnnAuthorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Search(string q)
        {
            var portalId = PortalController.GetEffectivePortalId(PortalSettings.PortalId);
            var isAdmin = UserInfo.IsSuperUser || UserInfo.IsInRole("Administrators");

            // GetUsersAdvancedSearch doesn't accept a comma or a single quote in the query so we have to remove them for now. See issue 20224.
            q = q.Replace(",", "").Replace("'", "");
            if (q.Length == 0) return Json(null, JsonRequestBehavior.AllowGet);

            var users = TestableUserController.Instance.GetUsersAdvancedSearch(portalId, UserInfo.UserID, -1, -1, -1, isAdmin, -1, int.MaxValue, "DisplayName", true, "DisplayName", q);
            var roles = TestableRoleController.Instance.GetRolesBasicSearch(portalId, 10, q);
            var rolesdata = roles.Select(role => new SearchResult {id = "role-" + role.RoleID, name = role.RoleName}).ToList();         

            var data = users.Select(user => new SearchResult { id = "user-" + user.UserID, name = user.DisplayName }).ToList();
            
            data.AddRange(rolesdata);

            var results = data.OrderBy(sr => sr.name);

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This class stores a single search result needed by jQuery Tokeninput
        /// </summary>
        private class SearchResult
        {
            // ReSharper disable InconsistentNaming
            // ReSharper disable NotAccessedField.Local
            public string id;
            // ReSharper restore NotAccessedField.Local
            public string name;
            // ReSharper restore InconsistentNaming
        }
    }
}

