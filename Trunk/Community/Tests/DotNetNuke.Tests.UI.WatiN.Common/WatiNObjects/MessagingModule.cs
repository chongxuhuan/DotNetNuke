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
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The messaging module object.
    /// </summary>
    public class MessagingModule : WatiNBase
    {
        private TelerikEditor telerikEditor;

        #region Constructors

        public MessagingModule(WatiNBase watinBase) : this(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public MessagingModule(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName)
        {
            telerikEditor = new TelerikEditor(ieInstance, siteUrl, dbName);
        }

        #endregion

       #region Public Properties

       public TelerikEditor TelerikEditor
        {
            get { return telerikEditor; }
        }

       #region Tables
       public Table InboxTable
        {
            get { return PageContentDiv.Table(Find.ById(s => s.Contains("MessageList_messagesGrid"))); }
        }
       /// <summary>
       /// The table containing the Message editor.
       /// </summary>
       public Table EditorTableWrap
       {
           get { return PageContentDiv.Table(Find.ById(s => s.EndsWith("EditMessage_messageEditor_messageEditorWrapper"))); }
       }
       #endregion

       #region Buttons
        public Button ConfirmDeleteButton
        {
            get { return IEInstance.Button(Find.ByText(s => s.Contains("Yes"))); }
        }
       #endregion

       #region TextFields
       public TextField ToTextField
        {
            get { return PageContentDiv.TextField(Find.ById(s => s.EndsWith("EditMessage_txtTo"))); }
        }
       public TextField SubjectTextField
       {
           get { return PageContentDiv.TextField(Find.ById(s => s.EndsWith("EditMessage_txtSubject"))); }
       }
       /// <summary>
       /// The textfield within the HTML frame.
       /// Use this to enter text into the telerik message editor
       /// </summary>
       public TextField htmlField
       {
           get { return HTMLFrame.TextField(Find.Any); }
       }
       #endregion

       #region Links
       public Link SendLink
       {
           get { return PageContentDiv.Link(Find.ByTitle("Send Message")); }
       }
       public Link ReplyLink
       {
           get { return PageContentDiv.Link(Find.ById(s => s.EndsWith("ReplyMessage"))); }
       }
       public Link ValidateUserLink
        {
            get { return PageContentDiv.Link(Find.ByTitle("Validate User")); }
        }
       public Link SaveMsgLink
       {
           get { return PageContentDiv.Link(Find.ByTitle("Save Draft")); }
       }
       public Link DeleteMsgLink
       {
           get { return PageContentDiv.Link(Find.ByTitle("Delete Message")); }
       }
       public Link CancelMsgLink
       {
           get { return PageContentDiv.Link(Find.ById(s => s.EndsWith("EditMessage_cancelEdit"))); }
       }
       public Link ComposeLink
       {
           get { return PageContentDiv.Link(Find.ById(s => s.EndsWith("MessageList_addMessageButton"))); }
       }
       #endregion

       #region Spans
        /// <summary>
        /// The span containing a link that switches the text editor mode to HTML. 
        /// </summary>
       public Span HTMLEditorModeSpan
        {
            get { return PageContentDiv.Span(Find.ByText("HTML"));}
        }
        /// <summary>
        /// A span containing the content of the message on the View Message page.
        /// </summary>
       public Span MsgContentSpan
       {
           get { return MsgContentDiv.Span(Find.ById(s => s.EndsWith("ViewMessage_messageLabel"))); }
       }
       #endregion

       #region Frames
        /// <summary>
        /// The design iFrame in the text editor.
        /// </summary>
       public Frame ContentFrame
        {
            get { return IEInstance.Frames.Filter(Find.ById(s => s.EndsWith("EditMessage_messageEditor_messageEditor_contentIframe")))[0]; }
        }
        /// <summary>
        /// The HTML iFrame in the text editor.
        /// </summary>
       public Frame HTMLFrame
        {
            get { return IEInstance.Frames[1]; }
        }
       #endregion

       #region Divs
       /// <summary>
       ///  The outer div for the messaging module
       /// </summary>
       public Div MsgContentDiv
        {
            get { return PageContentDiv.Div(Find.ByClass(s => s.Contains("ModDotNetNukeMessagingC"))); }
        }
       #endregion

       #endregion

        #region Public Methods

        /// <summary>
        /// Sends a message to a user.
        /// The test will also validate the username of the recipient.
        /// </summary>
        /// <param name="userName">The username of the message recipient.</param>
        /// <param name="subject">The subject for the message.</param>
        /// <param name="body">The body of the message.</param>
        public void SendMsgToUser(string userName, string subject, string body)
        {
            
            System.Threading.Thread.Sleep(1000);
            ComposeLink.Click();
            System.Threading.Thread.Sleep(2000);
            //Setting textfield.value does not appear to work properly 
            ToTextField.TypeText(userName);
            System.Threading.Thread.Sleep(3000);
            ValidateUserLink.Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(MessageSpan.InnerHtml.Contains("Username '" + userName + "' is valid"));
            SubjectTextField.TypeText(subject);

            HTMLEditorModeSpan.Click();
            System.Threading.Thread.Sleep(2000);
            htmlField.Value = body;
            SendLink.Click();

        }

        /// <summary>
        /// Composes a message to send to a user, then performs the message action.
        /// The test will also validate the username of the recipient.
        /// </summary>
        /// <param name="userName">The username of the recipient</param>
        /// <param name="subject">The subject for the message.</param>
        /// <param name="content">The body of the message.</param>
        /// <param name="msgAction">The action to perform after composing the message. 
        /// The current options are: "Send", "Save", "Delete" and "Cancel".</param>
        public void ComposeMessage(string userName, string subject, string content, string msgAction)
        {
            ComposeLink.ClickNoWait();
            ToTextField.TypeText(userName);
            System.Threading.Thread.Sleep(1000);
            ValidateUserLink.ClickNoWait();
            System.Threading.Thread.Sleep(2000);
            Assert.IsTrue(ModuleMessageSpan.InnerHtml.Contains("Username '" + userName + "' is valid"), MessageSpan.InnerHtml);
            SubjectTextField.TypeText(subject);
            HTMLEditorModeSpan.ClickNoWait();
            System.Threading.Thread.Sleep(1000);
            htmlField.TypeText(content);
            if (msgAction.CompareTo("Send") == 0)
            {
                SendLink.Click();
            }
            else if (msgAction.CompareTo("Save") == 0)
            {
                SaveMsgLink.Click();
            }
            else if (msgAction.CompareTo("Delete") == 0)
            {
                DeleteMsgLink.Click();
            }
            else if (msgAction.CompareTo("Cancel") == 0)
            {
                CancelMsgLink.Click();
            }
        }

        /// <summary>
        /// Finds the row for the message in the users messages table.
        /// </summary>
        /// <param name="subject">The subject of the message.</param>
        /// <returns>The row for the message in the table.</returns>
        public TableRow GetMsgRowBySubject(string subject)
        {
            //Returns the msg Row specified from the inbox
            TableRow result = null;
            TableRowCollection rows = InboxTable.TableRows.Filter(Find.ById(s => s.Contains("MessageList_messagesGrid")));
            foreach (TableRow row in rows)
            {
                if (row.TableCells[2].Link(Find.Any).Text.Contains(subject))
                {
                    result = row;
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the message link in the users messages table.
        /// </summary>
        /// <param name="subject">The subject for the message.</param>
        /// <returns>The link to the message.</returns>
        public Link GetMsgLinkBySubject(string subject)
        {
            //Returns the message link specified from the my messages table
            Link result = null;
            TableRowCollection rows = InboxTable.TableRows.Filter(Find.ById(s => s.Contains("MessageList_messagesGrid")));
            foreach (TableRow row in rows)
            {
                if (row.TableCells[2].Link(Find.Any).Text.Contains(subject))
                {
                    result = row.TableCells[2].Link(Find.Any);
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the table cell containing the message status.
        /// </summary>
        /// <param name="subject">The subject of the message.</param>
        /// <returns>The table cell containing the Status of the message.</returns>
        public TableCell GetMsgStatusBySubject(string subject)
        {
            //Returns the user Row specified from the user table
            TableCell result = null;
            TableRowCollection rows = InboxTable.TableRows.Filter(Find.ById(s => s.Contains("MessageList_messagesGrid")));
            foreach (TableRow row in rows)
            {
                if (row.TableCells[2].InnerHtml.Contains(subject))
                {
                    result = row.TableCells[4];
                    break;
                }
                continue;
            }
            return result;
        }

        #endregion
    }
}
