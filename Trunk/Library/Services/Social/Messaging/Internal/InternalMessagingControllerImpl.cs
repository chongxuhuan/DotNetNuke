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
#region Usings

using System;
using System.Collections.Generic;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Social.Messaging.Data;
using DotNetNuke.Services.Social.Messaging.Internal.Views;

#endregion

namespace DotNetNuke.Services.Social.Messaging.Internal
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   The Controller class for social Messaging
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    internal class InternalMessagingControllerImpl : IInternalMessagingController
    {
        #region Constants

        internal const int ConstMaxTo = 2000;
        internal const int ConstMaxSubject = 400;
        internal const int ConstDefaultPageIndex = 0;
        internal const int ConstDefaultPageSize = 10;
        internal const string ConstSortColumnDate = "CreatedOnDate";
        internal const string ConstSortColumnFrom = "From";
        internal const string ConstSortColumnSubject = "Subject";
        internal const bool ConstAscending = true;

        #endregion

        #region Private Variables

        private readonly IDataService _dataService;

        #endregion

        #region Constructors

        public InternalMessagingControllerImpl()
            : this(DataService.Instance)
        {
        }

        public InternalMessagingControllerImpl(IDataService dataService)
        {
            //Argument Contract
            Requires.NotNull("dataService", dataService);

            _dataService = dataService;
        }

        #endregion

        #region Get View APIs

        public virtual MessageBoxView GetArchivedMessages(int userId, int afterMessageId, int numberOfRecords)
        {
            var reader = _dataService.GetArchiveBoxView(userId, GetCurrentUserInfo().PortalID, afterMessageId, numberOfRecords, ConstSortColumnDate, !ConstAscending);
            return new MessageBoxView { Conversations = CBO.FillCollection<MessageConversationView>(reader) };
        }

        public virtual MessageBoxView GetInbox(int userId, int afterMessageId, int numberOfRecords, string sortColumn, bool sortAscending)
        {
            return GetInbox(userId, afterMessageId, numberOfRecords, sortColumn, sortAscending, MessageReadStatus.Any, MessageArchivedStatus.UnArchived);
        }

        public virtual MessageBoxView GetInbox(int userId, int afterMessageId, int numberOfRecords, string sortColumn, bool sortAscending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus)
        {
            var reader = _dataService.GetInBoxView(userId, GetCurrentUserInfo().PortalID, afterMessageId, numberOfRecords, sortColumn, sortAscending, readStatus, archivedStatus, MessageSentStatus.Received);
            return new MessageBoxView { Conversations = CBO.FillCollection<MessageConversationView>(reader) };         
        }

        public virtual MessageThreadsView GetMessageThread(int conversationId, int userId, int afterMessageId, int numberOfRecords, ref int totalRecords)
        {
            return GetMessageThread(conversationId, userId, afterMessageId, numberOfRecords, ConstSortColumnDate, !ConstAscending, ref totalRecords);
        }

        public virtual MessageThreadsView GetMessageThread(int conversationId, int userId, int afterMessageId, int numberOfRecords, string sortColumn, bool sortAscending, ref int totalRecords)
        {
            var messageThreadsView = new MessageThreadsView();

            var dr = _dataService.GetMessageThread(conversationId, userId, afterMessageId, numberOfRecords, sortColumn, sortAscending, ref totalRecords);

            try
            {
                while (dr.Read())
                {
                    var messageThreadView = new MessageThreadView { Conversation = new MessageConversationView() };
                    messageThreadView.Conversation.Fill(dr);

                    if (messageThreadView.Conversation.AttachmentCount > 0)
                    {
                        messageThreadView.Attachments = _dataService.GetMessageAttachmentsByMessage(messageThreadView.Conversation.MessageID);
                    }

                    if (messageThreadsView.Conversations == null) messageThreadsView.Conversations = new List<MessageThreadView>();

                    messageThreadsView.Conversations.Add(messageThreadView);
                }
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }

            return messageThreadsView;            
        }

        public virtual MessageBoxView GetRecentInbox(int userId)
        {
            return GetRecentInbox(userId, ConstDefaultPageIndex, ConstDefaultPageSize);
        }

        public virtual MessageBoxView GetRecentInbox(int userId, int afterMessageId, int numberOfRecords)
        {
            return GetInbox(userId, afterMessageId, numberOfRecords, ConstSortColumnDate, !ConstAscending);
        }

        public virtual MessageBoxView GetRecentSentbox(int userId)
        {
            return GetRecentSentbox(userId, ConstDefaultPageIndex, ConstDefaultPageSize);
        }

        public virtual MessageBoxView GetRecentSentbox(int userId, int afterMessageId, int numberOfRecords)
        {
            return GetSentbox(userId, afterMessageId, numberOfRecords, ConstSortColumnDate, !ConstAscending);
        }

        public virtual MessageBoxView GetSentbox(int userId, int afterMessageId, int numberOfRecords, string sortColumn, bool sortAscending)
        {
            return GetSentbox(userId, afterMessageId, numberOfRecords, sortColumn, sortAscending, MessageReadStatus.Any, MessageArchivedStatus.UnArchived);
        }

        public virtual MessageBoxView GetSentbox(int userId, int afterMessageId, int numberOfRecords, string sortColumn, bool sortAscending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus)
        {
            var reader = _dataService.GetSentBoxView(userId, GetCurrentUserInfo().PortalID, afterMessageId, numberOfRecords, sortColumn, sortAscending);
            return new MessageBoxView { Conversations = CBO.FillCollection<MessageConversationView>(reader) };
        }

        #endregion

        #region Counter APIs

        public virtual int CountArchivedMessagesByConversation(int conversationId)
        {
            return _dataService.CountArchivedMessagesByConversation(conversationId);
        }

        public virtual int CountMessagesByConversation(int conversationId)
        {
            return _dataService.CountMessagesByConversation(conversationId);
        }

        public virtual int CountConversations(int userId, int portalId)
        {
            return _dataService.CountTotalConversations(userId, portalId);
        }

        public virtual int CountUnreadMessages(int userId, int portalId)
        {
            return _dataService.CountNewThreads(userId, portalId);
        }

        public virtual int CountSentMessages(int userId, int portalId)
        {
            return _dataService.CountSentMessages(userId, portalId);
        }

        public virtual int CountArchivedMessages(int userId, int portalId)
        {
            return _dataService.CountArchivedMessages(userId, portalId);
        }
        
        #endregion

        #region Internal Methods

        internal virtual UserInfo GetCurrentUserInfo()
        {
            return UserController.GetCurrentUserInfo();
        }

        #endregion

        #region Upgrade APIs

        public void ConvertLegacyMessages(int pageIndex, int pageSize)
        {
            _dataService.ConvertLegacyMessages(pageIndex, pageSize);
        }

        public int CountLegacyMessages()
        {
            var totalRecords = 0;
            var dr = _dataService.CountLegacyMessages();

            try
            {
                while (dr.Read())
                {
                    totalRecords = Convert.ToInt32(dr["TotalRecords"]);
                }
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }

            return totalRecords;
        }

        #endregion

        #region Queued email API's

        public IList<MessageRecipient> GetNextMessagesForDispatch(Guid schedulerInstance, int batchSize)
        {
            return CBO.FillCollection<MessageRecipient>(_dataService.GetNextMessagesForDispatch(schedulerInstance, batchSize));
        }

        public void MarkMessageAsDispatched(int messageId, int recipientId)
        {
            _dataService.MarkMessageAsDispatched(messageId, recipientId);
        }


        #endregion
    }
}
