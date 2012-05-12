#region Copyright
// 
// DotNetNuke� - http://www.dotnetnuke.com
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
#region Usings

using System;
using System.Collections;
using System.Collections.Generic;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Messaging.Data;
using DotNetNuke.Services.Social.Messaging;
using DotNetNuke.Services.Social.Messaging.Internal;

using Message = DotNetNuke.Services.Messaging.Data.Message;

#endregion

namespace DotNetNuke.Services.Messaging
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   The Controller class for Messaging
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    [Obsolete("Deprecated in DNN 6.2.0, please use DotNetNuke.Services.Social.Messaging.MessagingController")]
    public class MessagingController : IMessagingController
    {
        private static TabInfo _MessagingPage;

        #region "Constructors"

        public MessagingController()
            : this(GetDataService())
        {
        }

        public MessagingController(IMessagingDataService dataService)
        {
            _DataService = dataService;
        }

        #endregion

        #region "Private Shared Methods"

        private static IMessagingDataService GetDataService()
        {
            var ds = ComponentFactory.GetComponent<IMessagingDataService>();

            if (ds == null)
            {
                ds = new MessagingDataService();
                ComponentFactory.RegisterComponentInstance<IMessagingDataService>(ds);
            }
            return ds;
        }

        #endregion

        #region "Obsolete Methods"

        [Obsolete("Deprecated in DNN 6.2.0")]
        public static string DefaultMessagingURL(string ModuleFriendlyName)
        {
            TabInfo page = MessagingPage(ModuleFriendlyName);
            if (page != null)
            {
                return MessagingPage(ModuleFriendlyName).FullUrl;
            }
            else
            {
                return null;
            }
        }

        [Obsolete("Deprecated in DNN 6.2.0")]
        public static TabInfo MessagingPage(string ModuleFriendlyName)
        {
            if (((_MessagingPage != null)))
            {
                return _MessagingPage;
            }

            var mc = new ModuleController();
            ModuleInfo md = mc.GetModuleByDefinition(PortalSettings.Current.PortalId, ModuleFriendlyName);
            if ((md != null))
            {
                ArrayList a = mc.GetModuleTabs(md.ModuleID);
                if ((a != null))
                {
                    var mi = a[0] as ModuleInfo;
                    if ((mi != null))
                    {
                        var tc = new TabController();
                        _MessagingPage = tc.GetTab(mi.TabID, PortalSettings.Current.PortalId, false);
                    }
                }
            }

            return _MessagingPage;
        }

        [Obsolete("Deprecated in 6.2.0 - use DotNetNuke.Services.Social.Messaging.MessagingController.Instance(messageId)")]
        public Message GetMessageByID(int PortalID, int UserID, int messageId)
        {
            var coreMessage = InternalMessagingController.Instance.GetMessage(messageId);
            var coreMessageRecipient = InternalMessagingController.Instance.GetMessageRecipient(messageId, UserID);
            return ConvertCoreMessageToServicesMessage(PortalID, UserID, coreMessageRecipient, coreMessage);
        }



        [Obsolete("Deprecated in 6.2.0")]
        public List<Message> GetUserInbox(int PortalID, int UserID, int PageNumber, int PageSize)
        {
            return CBO.FillCollection<Message>(_DataService.GetUserInbox(PortalID, UserID, PageNumber, PageSize));
        }

        [Obsolete("Deprecated in 6.2.0 - use DotNetNuke.Services.Social.Messaging.MessagingController.Instance.GetMessage(messageId)")]
        public int GetInboxCount(int PortalID, int UserID)
        {
            return InternalMessagingController.Instance.CountConversations(UserID, PortalID);
        }

        [Obsolete("Deprecated in 6.2.0 - use InternalMessagingController.Instance.GetMessage(messageId)")]
        public int GetNewMessageCount(int PortalID, int UserID)
        {
            return InternalMessagingController.Instance.CountUnreadMessages(UserID, PortalID);
        }

        [Obsolete("Deprecated in 6.2.0 - use InternalMessagingController.Instance.GetMessage(messageId)")]
        public Message GetNextMessageForDispatch(Guid SchedulerInstance)
        {
            //does not need to run as scheduled task name was updated 
            return null;
        }

        [Obsolete("Deprecated in 6.2.0 - use InternalMessagingController.Instance.GetMessage(messageId)")]
        public void SaveMessage(Message message)
        {
            if ((PortalSettings.Current != null))
            {
                message.PortalID = PortalSettings.Current.PortalId;
            }

            if ((message.Conversation == null || message.Conversation == Guid.Empty))
            {
                message.Conversation = Guid.NewGuid();
            }

            List<UserInfo> users = null;

            var userController = new UserController();
            users.Add(userController.GetUser(message.PortalID, message.ToUserID));

            List<RoleInfo> emptyRoles = null;
            List<int> files = null;
            
            var coremessage = new Social.Messaging.Message {Body = message.Body, Subject = message.Subject};


            Social.Messaging.MessagingController.Instance.SendMessage(coremessage, emptyRoles, users, files);
        }

        [Obsolete("Deprecated in 6.2.0 - use InternalMessagingController.Instance.GetMessage(messageId)")]
        public void UpdateMessage(Message message)
        {
            var user = UserController.GetCurrentUserInfo().UserID;
            switch (message.Status)
            {
                case MessageStatusType.Unread:
                    InternalMessagingController.Instance.MarkUnRead(message.MessageID, user);
                    break;
                case MessageStatusType.Draft:
                    //no equivalent
                    break;
                case MessageStatusType.Deleted:
                    InternalMessagingController.Instance.MarkArchived(message.MessageID, user);
                    break;
                case MessageStatusType.Read:
                    InternalMessagingController.Instance.MarkRead(message.MessageID, user);
                    break;
            }

            _DataService.UpdateMessage(message);
        }

        [Obsolete("Deprecated in 6.2.0 - use InternalMessagingController.Instance.GetMessage(messageId)")]
        public void MarkMessageAsDispatched(int MessageID)
        {
            //does not need to run as scheduled task name was updated
        }

        #endregion

        #region "functions to support obsolence"
        private static Message ConvertCoreMessageToServicesMessage(int PortalID, int UserID, MessageRecipient coreMessageRecipeint, Social.Messaging.Message coreMessage)
        {
            var message = new Message { AllowReply = true, Body = coreMessage.Body, FromUserID = coreMessage.SenderUserID, MessageDate = coreMessage.CreatedOnDate, PortalID = PortalID };

            switch (coreMessageRecipeint.Read)
            {
                case true:
                    message.Status = MessageStatusType.Read;
                    break;
                case false:
                    message.Status = MessageStatusType.Unread;
                    break;
            }

            message.ToUserID = UserID;
            return message;
        }
        #endregion

        private readonly IMessagingDataService _DataService;
    }
}
