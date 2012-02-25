using System;
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
        //
        // GET: /Services/
        [DnnAuthorize(AllowAnonymous = true)]
        public string Index()
        {
            return "Hello World " + DateTime.Now;
        }

        [DnnAuthorize]
        public ActionResult Inbox(int pageIndex, int pageSize)
        {            
            return Json(MessagingController.Instance.GetRecentInbox(UserInfo.UserID, pageIndex, pageSize).Conversations, JsonRequestBehavior.AllowGet);
        }

        [DnnAuthorize]
        public ActionResult Sentbox(int pageIndex, int pageSize)
        {            
            return Json(MessagingController.Instance.GetRecentSentbox(UserInfo.UserID, pageIndex, pageSize).Conversations, JsonRequestBehavior.AllowGet);
        }

        [DnnAuthorize]
        public ActionResult Archived(int pageIndex, int pageSize)
        {            
            return Json(MessagingController.Instance.GetArchivedMessages(UserInfo.UserID, pageIndex, pageSize).Conversations, JsonRequestBehavior.AllowGet);
        }

        [DnnAuthorize]
        public ActionResult Thread(int conversationId, int pageIndex, int pageSize)
        {
            var totalRecords = 0;
            return Json(MessagingController.Instance.GetMessageThread(conversationId, UserInfo.UserID, pageIndex, pageSize, ref totalRecords), JsonRequestBehavior.AllowGet);
        }

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

        [DnnAuthorize]
        public ActionResult Reply(int conversationId, string body, IList<int> fileIds)
        {
            var messageId = MessagingController.Instance.ReplyMessage(conversationId, body, fileIds);
            return Json(messageId);
        }

        [DnnAuthorize]
        public ActionResult MarkArchived(int conversationId)
        {
            MessagingController.Instance.MarkArchived(conversationId, UserInfo.UserID);
            return Json(new { Result = "success" }, JsonRequestBehavior.AllowGet);
        }

        [DnnAuthorize]
        public ActionResult MarkUnArchived(int conversationId)
        {
            MessagingController.Instance.MarkUnArchived(conversationId, UserInfo.UserID);
            return Json(new { Result = "success" }, JsonRequestBehavior.AllowGet);
        }

        [DnnAuthorize]
        public ActionResult MarkRead(int conversationId)
        {
            MessagingController.Instance.MarkRead(conversationId, UserInfo.UserID);
            return Json(new { Result = "success" }, JsonRequestBehavior.AllowGet);
        }

        [DnnAuthorize]
        public ActionResult MarkUnRead(int conversationId)
        {
            MessagingController.Instance.MarkUnRead(conversationId, UserInfo.UserID);
            return Json(new { Result = "success" }, JsonRequestBehavior.AllowGet);
        }

        [DnnAuthorize]
        public ActionResult Auth()
        {
            return Json(new { Result = string.Format("Hello {0} you are Authorized!!", UserInfo.Username) }, JsonRequestBehavior.AllowGet);
        }

        [DnnAuthorize(Roles = "Registered Users")]
        public ActionResult Registered()
        {
            return Json(new { Output = string.Format("Hello {0} you are a registered user.", UserInfo.Username) }, JsonRequestBehavior.AllowGet);
        }

        [DnnAuthorize(Roles = "Custom Role")]
        public ActionResult CustomRole()
        {
            return Json(new { Output = string.Format("Hello {0} you are in a custom role.", UserInfo.Username) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HostsOnly()
        {
            return Json(new { Output = string.Format("Hello {0} you are a most excellent Host.", UserInfo.Username) }, JsonRequestBehavior.AllowGet);
        }
    }
}

