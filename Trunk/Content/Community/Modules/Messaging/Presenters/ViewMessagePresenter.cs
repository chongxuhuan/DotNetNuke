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

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Modules.Messaging.Views;
using DotNetNuke.Modules.Messaging.Views.Models;
using DotNetNuke.Services.Messaging;
using DotNetNuke.Services.Messaging.Data;
using DotNetNuke.Web.Mvp;


#endregion

namespace DotNetNuke.Modules.Messaging.Presenters
{
    public class ViewMessagePresenter : ModulePresenter<IViewMessageView, ViewMessageModel>
    {

        private readonly IMessagingController _MessagingController;

        #region Constructors

        public ViewMessagePresenter(IViewMessageView viewView) : this(viewView, new MessagingController(new MessagingDataService()))
        {
        }

        public ViewMessagePresenter(IViewMessageView viewView, IMessagingController messagingController) : base(viewView)
        {
            Requires.NotNull("messagingController", messagingController);

            _MessagingController = messagingController;

            View.Delete += DeleteMessage;
            View.Load += Load;
        }

        #endregion

        #region Public Properties

        public int IndexId
        {
            get
            {
                var indexId = Null.NullInteger;
                if (!string.IsNullOrEmpty(Request.Params["MessageId"]))
                {
                    indexId = Int32.Parse(Request.Params["MessageId"]);
                }
                return indexId;
            }
        }

        #endregion

        #region Private Methods

        private string GetInboxUrl()
        {
            return Globals.NavigateURL(TabId, "", string.Format("userId={0}", UserId));
        }

        private string GetReplyUrl()
        {
            return Globals.NavigateURL(TabId, "EditMessage", string.Format("mid={0}", ModuleId), string.Format("MessageId={0}", View.Model.Message.MessageID), "IsReply=true");
        }

        #endregion

        #region Public Methods

        public void DeleteMessage(object sender, EventArgs e)
        {
            View.BindMessage(View.Model.Message);

            View.Model.Message.Status = MessageStatusType.Deleted;
            _MessagingController.UpdateMessage(View.Model.Message);

            //Redirect to List
            Response.Redirect(GetInboxUrl());
        }

        public void Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                View.Model.Message = _MessagingController.GetMessageByID(PortalId, UserId, IndexId);
                if (View.Model.Message == null || View.Model.Message.ToUserID != UserId)
                {
                    //Redirect - message does not belong to user
                    Response.Redirect(GetInboxUrl());
                }
                if (View.Model.Message.Status == MessageStatusType.Unread)
                {
                    View.Model.Message.Status = MessageStatusType.Read;
                    _MessagingController.UpdateMessage(View.Model.Message);
                }
                View.Model.ReplyUrl = GetReplyUrl();
                View.Model.InboxUrl = GetInboxUrl();

                View.BindMessage(View.Model.Message);
            }
        }

        #endregion

    }
}