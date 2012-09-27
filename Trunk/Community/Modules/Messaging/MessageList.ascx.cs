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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework;
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
    public partial class MessageList : ProfileModuleViewBase<MessageListModel>, IMessageListView
    {

        #region Constructor

        public MessageList()
        {
            AutoDataBind = false;
        }

        #endregion

        public override bool DisplayModule
        {
            get
            {
                return Request.IsAuthenticated && (ProfileUserId == ModuleContext.PortalSettings.UserId);
            }
        }

        #region IMessageListView Implementation

        public event DnnGridItemSelectedEventHandler DeleteSelectedMessages;
        public event DnnGridItemSelectedEventHandler MarkSelectedMessagesRead;
        public event DnnGridItemSelectedEventHandler MarkSelectedMessagesUnread;
        public event GridItemEventHandler MessageDataBound;
        public event GridNeedDataSourceEventHandler MessagesNeedDataSource;

        #endregion

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            jQuery.RequestDnnPluginsRegistration();

            markAsRead.Click += OnMarkAsReadClick;
            markAsUnread.Click += OnMarkAsUnreadClick;
            delete.Click += OnDeleteMessagesClick;
            messagesGrid.ItemDataBound += OnMessagesItemDataBound;
            messagesGrid.NeedDataSource += OnMessagesNeedDataSource;
        }

        public void Refresh()
        {
            addMessageButton.NavigateUrl = Model.ComposeMsgUrl;
        }

        private void RefreshInbox()
        {
            Response.Redirect(Request.RawUrl);
        }

        protected void OnDeleteMessagesClick(object sender, EventArgs e)
        {
            if (DeleteSelectedMessages != null)
            {
                DeleteSelectedMessages(this, new DnnGridItemSelectedEventArgs(messagesGrid.SelectedItems));
            }
            RefreshInbox();
        }

        protected void OnMarkAsReadClick(object sender, EventArgs e)
        {
            if (MarkSelectedMessagesRead != null)
            {
                MarkSelectedMessagesRead(this, new DnnGridItemSelectedEventArgs(messagesGrid.SelectedItems));
            }
            RefreshInbox();
        }

        protected void OnMarkAsUnreadClick(object sender, EventArgs e)
        {
            if (MarkSelectedMessagesUnread != null)
            {
                MarkSelectedMessagesUnread(this, new DnnGridItemSelectedEventArgs(messagesGrid.SelectedItems));
            }
            RefreshInbox();
        }

        protected void OnMessagesItemDataBound(object sender, GridItemEventArgs e)
        {
            if (MessageDataBound != null)
            {
                MessageDataBound(messagesGrid, e);
            }
        }

        protected void OnMessagesNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (MessagesNeedDataSource != null)
            {
                MessagesNeedDataSource(messagesGrid, e);
            }
        }

        #endregion

    }
}