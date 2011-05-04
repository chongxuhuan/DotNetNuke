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

using DotNetNuke.Modules.Messaging.Presenters;
using DotNetNuke.Modules.Messaging.Views.Models;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Messaging.Data;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Web.Mvp;

using WebFormsMvp;

#endregion

namespace DotNetNuke.Modules.Messaging.Views
{
    [PresenterBinding(typeof (EditMessagePresenter))]
    public partial class EditMessage : ModuleView<EditMessageModel>, IEditMessageView
    {
        #region "Protected Methods"

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cancelEdit.Click += cancelEdit_Click;
            deleteMessage.Click += deleteMessage_Click;
            saveDraftButton.Click += saveDraftButton_Click;
            sendMessageButton.Click += sendMessageButton_Click;
            validateUserButton.Click += validateUserButton_Click;
        }

        #endregion

        #region "IEditMessageView Implementation"

        public event EventHandler Cancel;
        public event EventHandler Delete;
        public event EventHandler SaveDraft;
        public event EventHandler SendMessage;
        public event EventHandler ValidateUser;

        public void BindMessage(Message message)
        {
            if (IsPostBack)
            {
                message.Subject = subjectTextBox.Text;
                message.Body = messageEditor.Text;
            }
            else
            {
                toTextBox.Text = message.ToUserName;
                toTextBox.ToolTip = message.ToUserName;
                subjectTextBox.Text = message.Subject;
                messageEditor.Text = message.Body;
            }
        }

        public void ShowInvalidUserError()
        {
            //'toTextBox.Text = ""
            string toError = string.Format(Localization.GetString("Validation.Error.Message", LocalResourceFile), toTextBox.Text);

            UI.Skins.Skin.AddModuleMessage(this, toError, ModuleMessage.ModuleMessageType.RedError);

            //'Services.Localization.Localization.GetString("Validation.Error.Message", Me)
        }

        public void ShowValidUserMessage()
        {
            string toValid = string.Format(Localization.GetString("Validation.Success.Message", LocalResourceFile), toTextBox.Text);

            UI.Skins.Skin.AddModuleMessage(this, toValid, ModuleMessage.ModuleMessageType.GreenSuccess);
        }

        public void HideDeleteButton()
        {
            deleteHolder.Visible = false;
        }

        #endregion

        private void cancelEdit_Click(object sender, EventArgs e)
        {
            if (Cancel != null)
            {
                Cancel(this, e);
            }
        }

        private void deleteMessage_Click(object sender, EventArgs e)
        {
            if (Delete != null)
            {
                Delete(this, e);
            }
        }

        private void saveDraftButton_Click(object sender, EventArgs e)
        {
            Model.UserName = toTextBox.Text;
            if (SaveDraft != null)
            {
                SaveDraft(this, e);
            }
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            Model.UserName = toTextBox.Text;
            if (SendMessage != null)
            {
                SendMessage(this, e);
            }
        }

        private void validateUserButton_Click(object sender, EventArgs e)
        {
            Model.UserName = toTextBox.Text;
            if (ValidateUser != null)
            {
                ValidateUser(this, e);
            }
        }
    }
}