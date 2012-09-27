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
using DotNetNuke.Services.Messaging.Data;
using DotNetNuke.Web.Mvp;
using WebFormsMvp;

#endregion

namespace DotNetNuke.Modules.Messaging.Views
{

    [PresenterBinding(typeof (ViewMessagePresenter))]
    public partial class ViewMessage : ModuleView<ViewMessageModel>, IViewMessageView
    {

        #region IViewMessageView Implementation

        public event EventHandler Delete;

        public void BindMessage(Message message)
        {
            fromLabel.Text = message.FromUserName;
            subjectLabel.Text = message.Subject;
            messageLabel.Text = HtmlUtils.ConvertToHtml(message.Body);

            hlReplyMessage.NavigateUrl = Model.ReplyUrl;
            hlCancel.NavigateUrl = Model.InboxUrl;
        }

        #endregion

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            jQuery.RequestDnnPluginsRegistration();

            deleteMessage.Click += OnDeleteMessageClick;
        }

        protected void OnDeleteMessageClick(object sender, EventArgs e)
        {
            if (Delete != null)
            {
                Delete(this, e);
            }
        }

        #endregion

    }
}