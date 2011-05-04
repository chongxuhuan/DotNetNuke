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
        private readonly IMessagingController _MessagingController;

        #region "Constructors"

        public EditMessagePresenter(IEditMessageView editView) : this(editView, new MessagingController(new MessagingDataService()))
        {
        }

        public EditMessagePresenter(IEditMessageView editView, IMessagingController messagingController) : base(editView)
        {
            Requires.NotNull("messagingController", messagingController);

            _MessagingController = messagingController;

            View.Cancel += Cancel;
            View.Delete += DeleteMessage;
            View.Load += Load;
            View.SaveDraft += SaveDraft;
            View.SendMessage += SendMessage;
            View.ValidateUser += ValidateUser;
        }

        #endregion

        #region "Public Properties"

        public long MessageId
        {
            get
            {
                long _IndexId = Null.NullInteger;
                if (!string.IsNullOrEmpty(Request.Params["MessageId"]))
                {
                    _IndexId = Int32.Parse(Request.Params["MessageId"]);
                }
                return _IndexId;
            }
        }


        public bool IsReplyMode
        {
            get
            {
                bool _isReply = false;
                if (!string.IsNullOrEmpty(Request.Params["IsReply"]))
                {
                    bool.TryParse(Request.Params["IsReply"], out _isReply);
                }
                return _isReply;
            }
        }

        #endregion

        #region "Private Methods"

        private string GetInboxUrl()
        {
            return Globals.NavigateURL(TabId, "", string.Format("userId={0}", UserId));
        }

        #endregion

        #region "Public Methods"

        public void Cancel(object sender, EventArgs e)
        {
            Response.Redirect(GetInboxUrl());
        }


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
                if ((MessageId > 0))
                {
                    Message orgMessage = _MessagingController.GetMessageByID(PortalId, UserId, (int) MessageId);
                    AuthorizeUser(orgMessage);
                    if (IsReplyMode)
                    {
                        View.Model.Message = _MessagingController.GetMessageByID(PortalId, UserId, (int) MessageId).GetReplyMessage();
                        View.HideDeleteButton();
                    }
                    else
                    {
                        View.Model.Message = _MessagingController.GetMessageByID(PortalId, UserId, (int) MessageId);
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
                    _MessagingController.SaveMessage(View.Model.Message);
                }
                else
                {
                    _MessagingController.UpdateMessage(View.Model.Message);
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
            int userId = Null.NullInteger;
            if (!string.IsNullOrEmpty(userName))
            {
                // validate username
                UserInfo objUser = UserController.GetUserByName(PortalId, userName);
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