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
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.Messaging.Views;
using DotNetNuke.Modules.Messaging.Views.Models;
using DotNetNuke.Services.Messaging;
using DotNetNuke.Services.Messaging.Data;
using DotNetNuke.Web.Mvp;


#endregion

namespace DotNetNuke.Modules.Messaging.Presenters
{

    public class EditMessagePresenter : ModulePresenter<IEditMessageView, EditMessageModel>
    {

        private readonly IMessagingController _messagingController;

        #region Constructors

        public EditMessagePresenter(IEditMessageView editView) : this(editView, new MessagingController(new MessagingDataService()))
        {
        }

        public EditMessagePresenter(IEditMessageView editView, IMessagingController messagingController) : base(editView)
        {
            Requires.NotNull("messagingController", messagingController);

            _messagingController = messagingController;

            View.Delete += DeleteMessage;
            View.Load += Load;
            View.SaveDraft += SaveDraft;
            View.SendMessage += SendMessage;
            View.ValidateUser += ValidateUser;
        }

        #endregion

        #region Public Properties

        public long MessageId
        {
            get
            {
                long indexId = Null.NullInteger;
                if (!string.IsNullOrEmpty(Request.Params["MessageId"]))
                {
                    indexId = Int32.Parse(Request.Params["MessageId"]);
                }
                return indexId;
            }
        }


        public bool IsReplyMode
        {
            get
            {
                var isReply = false;
                if (!string.IsNullOrEmpty(Request.Params["IsReply"]))
                {
                    bool.TryParse(Request.Params["IsReply"], out isReply);
                }
                return isReply;
            }
        }

        #endregion

        #region Private Methods

        private string GetInboxUrl()
        {
            return Globals.NavigateURL(TabId, "", string.Format("userId={0}", UserId));
        }

        #endregion

        #region Public Methods

        public void DeleteMessage(object sender, EventArgs e)
        {
            View.BindMessage(View.Model.Message);

            View.Model.Message.Status = MessageStatusType.Deleted;
            _messagingController.UpdateMessage(View.Model.Message);

            //Redirect to List
            Response.Redirect(GetInboxUrl());
        }

        public void Load(object sender, EventArgs e)
        {
            View.Model.InboxUrl = GetInboxUrl();

            if (!IsPostBack)
            {
                if ((MessageId > 0))
                {
                    var orgMessage = _messagingController.GetMessageByID(PortalId, UserId, (int) MessageId);
                    AuthorizeUser(orgMessage);
                    if (IsReplyMode)
                    {
                        View.Model.Message = _messagingController.GetMessageByID(PortalId, UserId, (int) MessageId).GetReplyMessage();
                        View.HideDeleteButton();
                    }
                    else
                    {
                        View.Model.Message = _messagingController.GetMessageByID(PortalId, UserId, (int) MessageId);
                    }
                }
                else
                {
                    View.Model.Message = new Message();
                }

                View.BindMessage(View.Model.Message);
            }
        }

        private void AuthorizeUser(Message message)
        {
            switch (message.Status)
            {
                case MessageStatusType.Deleted:
                    Response.Redirect(GetInboxUrl());
                    break;
                case MessageStatusType.Draft:
                    if (message.FromUserID != UserId)
                    {
                        Response.Redirect(GetInboxUrl());
                    }
                    break;
                case MessageStatusType.Read:
                    if (message.ToUserID != UserId)
                    {
                        Response.Redirect(GetInboxUrl());
                    }
                    break;
                case MessageStatusType.Unread:
                    if (message.ToUserID != UserId)
                    {
                        Response.Redirect(GetInboxUrl());
                    }
                    break;
            }
        }

        public void SaveDraft(object sender, EventArgs e)
        {
            SubmitMessage(MessageStatusType.Draft);
        }

        public void SendMessage(object sender, EventArgs e)
        {
            SubmitMessage(MessageStatusType.Unread);
        }

        private void SubmitMessage(MessageStatusType status)
        {
            View.BindMessage(View.Model.Message);

            View.Model.Message.ToUserID = ValidateUserName(View.Model.UserName);


            if (View.Model.Message.ToUserID > Null.NullInteger)
            {
                View.Model.Message.FromUserID = UserId;
                View.Model.Message.MessageDate = DateTime.Now;


                View.Model.Message.Status = status;

                //Save Message
                if ((View.Model.Message.MessageID == 0))
                {
                    _messagingController.SaveMessage(View.Model.Message);
                }
                else
                {
                    _messagingController.UpdateMessage(View.Model.Message);
                }

                //Redirect to Message List
                Response.Redirect(GetInboxUrl());
            }
        }

        public void ValidateUser(object sender, EventArgs e)
        {
            // validate username
            if ((ValidateUserName(View.Model.UserName) > 0))
            {
                View.ShowValidUserMessage();
            }
        }

        #endregion

        private int ValidateUserName(string userName)
        {
            var userId = Null.NullInteger;
            if (!string.IsNullOrEmpty(userName))
            {
                // validate username
                var objUser = UserController.GetUserByName(PortalId, userName);
                if (objUser != null)
                {
                    userId = objUser.UserID;
                }
            }

            if ((userId == Null.NullInteger))
            {
                View.ShowInvalidUserError();
            }

            return userId;
        }

    }
}