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

#region Usings

using System;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Modules.Messaging.Presenters;
using DotNetNuke.Modules.Messaging.Views.Models;
using DotNetNuke.UI.Modules;
using DotNetNuke.Web.Mvp;
using DotNetNuke.Web.UI.WebControls;

using Telerik.Web.UI;

using WebFormsMvp;


#endregion

namespace DotNetNuke.Modules.Messaging.Views
{
    [PresenterBinding(typeof (MessageListPresenter))]
    public partial class MessageList : ModuleView<MessageListModel>, IMessageListView, IProfileModule
    {
        public MessageList()
        {
            AutoDataBind = false;
        }

        #region "IProfileModule Implementation"

        public bool DisplayModule
        {
            get
            {
                return Request.IsAuthenticated && (ProfileUserId == ModuleContext.PortalSettings.UserId);
            }
        }

        public int ProfileUserId
        {
            get
            {
                int _ProfileUserId = Null.NullInteger;
                if (!string.IsNullOrEmpty(Request.Params["UserId"]))
                {
                    _ProfileUserId = Int32.Parse(Request.Params["UserId"]);
                }
                return _ProfileUserId;
            }
        }

        #endregion

        #region "Protected Methods"

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            markAsRead.Click += markAsRead_Click;
            markAsUnread.Click += markAsUnread_Click;
            addMessageButton.Click += addMessageButton_Click;
            delete.Click += delete_Click;
            messagesGrid.ItemDataBound += messagesGrid_ItemDataBound;
            messagesGrid.NeedDataSource += messagesGrid_NeedDataSource;
        }

        #endregion

        #region "IMessageListView Implementation"

        public event EventHandler AddMessage;
        public event DnnGridItemSelectedEventHandler DeleteSelectedMessages;
        public event DnnGridItemSelectedEventHandler MarkSelectedMessagesRead;
        public event DnnGridItemSelectedEventHandler MarkSelectedMessagesUnread;
        public event GridItemEventHandler MessageDataBound;
        public event GridNeedDataSourceEventHandler MessagesNeedDataSource;

        #endregion

        #region "Event Handlers"

        private void RefreshInbox()
        {
            Response.Redirect(Request.RawUrl);
        }

        private void addMessageButton_Click(object sender, EventArgs e)
        {
            if (AddMessage != null)
            {
                AddMessage(this, e);
            }
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if (DeleteSelectedMessages != null)
            {
                DeleteSelectedMessages(this, new DnnGridItemSelectedEventArgs(messagesGrid.SelectedItems));
            }
            RefreshInbox();
        }

        private void markAsRead_Click(object sender, EventArgs e)
        {
            if (MarkSelectedMessagesRead != null)
            {
                MarkSelectedMessagesRead(this, new DnnGridItemSelectedEventArgs(messagesGrid.SelectedItems));
            }
            RefreshInbox();
        }

        private void markAsUnread_Click(object sender, EventArgs e)
        {
            if (MarkSelectedMessagesUnread != null)
            {
                MarkSelectedMessagesUnread(this, new DnnGridItemSelectedEventArgs(messagesGrid.SelectedItems));
            }
            RefreshInbox();
        }

        private void messagesGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (MessageDataBound != null)
            {
                MessageDataBound(messagesGrid, e);
            }
        }

        private void messagesGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (MessagesNeedDataSource != null)
            {
                MessagesNeedDataSource(messagesGrid, e);
            }
        }

        #endregion
    }
}