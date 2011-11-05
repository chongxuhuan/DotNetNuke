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
using System.Web;

using DotNetNuke.Framework;
using DotNetNuke.Modules.Messaging.Presenters;
using DotNetNuke.Modules.Messaging.Views.Models;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Messaging.Data;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Web.Mvp;
using DotNetNuke.Security;
using WebFormsMvp;

#endregion

namespace DotNetNuke.Modules.Messaging.Views
{

    [PresenterBinding(typeof (EditMessagePresenter))]
    public partial class EditMessage : ModuleView<EditMessageModel>, IEditMessageView
    {

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            jQuery.RequestDnnPluginsRegistration();

            deleteMessage.Click += OnDeleteMessage;
            saveDraftButton.Click += OnSaveDraftClick;
            sendMessageButton.Click += OnSendMessageClick;
            validateUserButton.Click += OnValidateUserClick;
        }

        #endregion

        #region IEditMessageView Implementation

        public event EventHandler Delete;
        public event EventHandler SaveDraft;
        public event EventHandler SendMessage;
        public event EventHandler ValidateUser;

        public void BindMessage(Message message)
        {
            cancelEdit.NavigateUrl = Model.InboxUrl;

            if (IsPostBack)
            {
                message.Subject = EncodeContent(txtSubject.Text);
                PortalSecurity ps = new PortalSecurity();
                string filterValue = string.Empty;
                filterValue = ps.InputFilter(messageEditor.Text, PortalSecurity.FilterFlag.NoScripting);
                message.Body = filterValue;
            }
            else
            {
                txtTo.Text = message.ToUserName;
                txtTo.ToolTip = message.ToUserName;
                txtSubject.Text = message.Subject;
                messageEditor.Text = message.Body;
            }
        }

        public void ShowInvalidUserError()
        {
            //'toTextBox.Text = ""
            var toError = string.Format(Localization.GetString("Validation.Error.Message", LocalResourceFile), EncodeContent(txtTo.Text));

            UI.Skins.Skin.AddModuleMessage(this, toError, ModuleMessage.ModuleMessageType.RedError);

            //'Services.Localization.Localization.GetString("Validation.Error.Message", Me)
        }

        public void ShowValidUserMessage()
        {
            var toValid = string.Format(Localization.GetString("Validation.Success.Message", LocalResourceFile), EncodeContent(txtTo.Text));

            UI.Skins.Skin.AddModuleMessage(this, toValid, ModuleMessage.ModuleMessageType.GreenSuccess);
        }

        public void HideDeleteButton()
        {
            liDelete.Visible = false;
        }

        #endregion

        #region Event Handlers

        protected void OnDeleteMessage(object sender, EventArgs e)
        {
            if (Delete != null)
            {
                Delete(this, e);
            }
        }

        protected void OnSaveDraftClick(object sender, EventArgs e)
        {
            Model.UserName = EncodeContent(txtTo.Text);
            if (SaveDraft != null)
            {
                SaveDraft(this, e);
            }
        }

        protected void OnSendMessageClick(object sender, EventArgs e)
        {
            Model.UserName = EncodeContent(txtTo.Text);
            if (SendMessage != null)
            {
                SendMessage(this, e);
            }
        }

        protected void OnValidateUserClick(object sender, EventArgs e)
        {
            Model.UserName = EncodeContent(txtTo.Text);
            if (ValidateUser != null)
            {
                ValidateUser(this, e);
            }
        }

        private string EncodeContent(string content)
        {
            return HttpUtility.HtmlEncode(content.ToString());
        }

        #endregion

    }
}