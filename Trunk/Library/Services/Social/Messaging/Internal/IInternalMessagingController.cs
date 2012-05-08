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

using DotNetNuke.Services.Social.Messaging.Internal.Views;

namespace DotNetNuke.Services.Social.Messaging.Internal
{
    public interface IInternalMessagingController
    {
        #region Upgrade APIs

        void ConvertLegacyMessages(int pageIndex, int pageSize);
        int CountLegacyMessages();

        #endregion

        #region Get View APIs

        MessageBoxView GetInbox(int userId, int afterMessageId, int numberOfRecords, string sortColumn, bool @ascending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus);

        MessageBoxView GetInbox(int userId, int afterMessageId, int numberOfRecords, string sortColumn, bool sortAscending);

        MessageBoxView GetRecentInbox(int userId);

        MessageBoxView GetRecentInbox(int userId, int afterMessageId, int numberOfRecords);

        MessageBoxView GetSentbox(int userId, int afterMessageId, int numberOfRecords, string sortColumn, bool ascending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus);

        MessageBoxView GetSentbox(int userId, int afterMessageId, int numberOfRecords, string sortColumn, bool sortAscending);

        MessageBoxView GetRecentSentbox(int userId);

        MessageBoxView GetRecentSentbox(int userId, int afterMessageId, int numberOfRecords);

        MessageBoxView GetArchivedMessages(int userId, int afterMessageId, int numberOfRecords);

        MessageThreadsView GetMessageThread(int conversationId, int userId, int afterMessageId, int numberOfRecords, string sortColumn, bool ascending, ref int totalRecords);

        MessageThreadsView GetMessageThread(int conversationId, int userId, int afterMessageId, int numberOfRecords, ref int totalRecords);

        #endregion

        #region Queued email API's

        IList<MessageRecipient> GetNextMessagesForDispatch(Guid schedulerInstance,int batchSize);
        void MarkMessageAsDispatched(int messageId, int recipientId);

        #endregion

        #region Counter APIs

        int CountUnreadMessages(int userId, int portalId);

        int CountConversations(int userId, int portalId, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus, MessageSentStatus sentStatus);

        int CountMessagesByConversation(int conversationId);

        int CountArchivedMessagesByConversation(int conversationId);

        #endregion
    }
}