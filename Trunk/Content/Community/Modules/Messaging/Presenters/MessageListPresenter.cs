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
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Modules.Messaging.Views;
using DotNetNuke.Modules.Messaging.Views.Models;
using DotNetNuke.Services.Messaging;
using DotNetNuke.Services.Messaging.Data;
using DotNetNuke.Web.Mvp;
using DotNetNuke.Web.UI.WebControls;
using Telerik.Web.UI;

#endregion

namespace DotNetNuke.Modules.Messaging.Presenters
{

    public class MessageListPresenter : ModulePresenter<IMessageListView, MessageListModel>
    {

        #region Private Fields

        private readonly IMessagingController _MessagingController;

        #endregion

        #region Constructors

        public MessageListPresenter(IMessageListView listView) : this(listView, new MessagingController(new MessagingDataService()))
        {
        }

        public MessageListPresenter(IMessageListView listView, IMessagingController messagingController) : base(listView)
        {
            Requires.NotNull("messagingController", messagingController);
            _MessagingController = messagingController;

            View.DeleteSelectedMessages += DeleteSelectedMessages;
            View.MarkSelectedMessagesRead += MarkSelectedMessagesRead;
            View.MarkSelectedMessagesUnread += MarkSelectedMessagesUnread;
            View.MessageDataBound += MessageDataBound;
            View.MessagesNeedDataSource += MessagesNeedDataSource;
        }

        #endregion

        protected override void OnInit()
        {
            base.OnInit();

            View.Model.Messages = _MessagingController.GetUserInbox(PortalId, UserId, 1, 999);
            View.Model.ComposeMsgUrl = Globals.NavigateURL(TabId, "EditMessage", string.Format("mid={0}", ModuleId));
            View.Refresh();
        }

        public void DeleteSelectedMessages(object sender, DnnGridItemSelectedEventArgs e)
        {
            foreach (GridItem c in e.SelectedItems)
            {
                var messageID = Convert.ToInt32(c.OwnerTableView.DataKeyValues[c.ItemIndex]["MessageID"]);
                var message = _MessagingController.GetMessageByID(PortalId, UserId, messageID);
                message.Status = MessageStatusType.Deleted;
                _MessagingController.UpdateMessage(message);
            }
        }

        public void MarkSelectedMessagesRead(object sender, DnnGridItemSelectedEventArgs e)
        {
            foreach (GridItem c in e.SelectedItems)
            {
                var messageID = Convert.ToInt32(c.OwnerTableView.DataKeyValues[c.ItemIndex]["MessageID"]);
                var message = _MessagingController.GetMessageByID(PortalId, UserId, messageID);

                if ((message.Status == MessageStatusType.Unread))
                {
                    message.Status = MessageStatusType.Read;
                    _MessagingController.UpdateMessage(message);
                }
            }
        }

        public void MarkSelectedMessagesUnread(object sender, DnnGridItemSelectedEventArgs e)
        {
            foreach (GridItem c in e.SelectedItems)
            {
                var messageID = Convert.ToInt32(c.OwnerTableView.DataKeyValues[c.ItemIndex]["MessageID"]);
                var message = _MessagingController.GetMessageByID(PortalId, UserId, messageID);

                if ((message.Status == MessageStatusType.Read))
                {
                    message.Status = MessageStatusType.Unread;
                    _MessagingController.UpdateMessage(message);
                }
            }
        }

        public void MessageDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.Item)
            {
                var message = e.Item.DataItem as Message;

                var item = (GridDataItem) e.Item;

                var hyperLinkColumn = item.Controls[4].Controls[0] as HyperLink;

                if (hyperLinkColumn != null)
                {
                    if (message.Status == MessageStatusType.Draft)
                    {
                        //Message is from me
                        hyperLinkColumn.NavigateUrl = Globals.NavigateURL(TabId, "EditMessage", string.Format("mid={0}", ModuleId), string.Format("MessageId={0}", message.MessageID));
                        hyperLinkColumn.Text = string.Format("[Draft] {0}", message.Subject);
                    }
                    else
                    {
                        //Message is to me
                        hyperLinkColumn.NavigateUrl = Globals.NavigateURL(TabId, "ViewMessage", string.Format("mid={0}", ModuleId), string.Format("MessageId={0}", message.MessageID));
                        hyperLinkColumn.Text = message.Subject;
                    }
                }

                if ((message.Status == MessageStatusType.Unread))
                {
                    hyperLinkColumn.Font.Bold = true;
                }
            }
        }

        public void MessagesNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var mGrid = (DnnGrid) sender;
            mGrid.PagerStyle.AlwaysVisible = true;
            mGrid.VirtualItemCount = _MessagingController.GetInboxCount(PortalId, UserId);
            mGrid.DataSource = _MessagingController.GetUserInbox(PortalId, UserId, mGrid.CurrentPageIndex + 1, mGrid.PageSize);
        }

    }
}