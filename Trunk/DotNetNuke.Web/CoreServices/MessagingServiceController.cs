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

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DotNetNuke.Entities.Users;
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
        public ActionResult Create(string subject, string body, IList<int> roleIds, IList<int> userIds, IList<int> fileIds)
        {
            var userController = new UserController();

            var roles = roleIds != null && roleIds.Count > 0
                ? roleIds.Select(id => TestableRoleController.Instance.GetRole(PortalSettings.PortalId, r => r.RoleID == id)).Where(role => role != null).ToList()
                : null;
            var users = userIds.Select(id => userController.GetUser(PortalSettings.PortalId, id)).Where(user => user != null).ToList();

            var message = MessagingController.Instance.CreateMessage(subject, body, roles, users, fileIds);

            return Json(message.MessageID);
        }
    }
}

