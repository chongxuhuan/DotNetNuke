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
using System.Data;
using System.Xml.Serialization;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Services.Social.Messaging.Internal.Views
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Messaging.Views
    /// Class:      MessageConversationView
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The MessageConversationView class contains details of the latest message in a Conversation.
    /// </summary>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class MessageConversationView : IHydratable
    {
        private int _messageID = -1;
        private string _displayDate;
        private DateTime _createdOnDate;

        /// <summary>
        /// MessageID - The primary key
        /// </summary>
        [XmlAttribute]
        public int MessageID
        {
            get
            {
                return _messageID;
            }
            set
            {
                _messageID = value;
            }
        }

        /// <summary>
        /// portalID for the message
        /// </summary>
        [XmlAttribute]
        public int PortalID { get; set; }

        /// <summary>
        /// To list for the message. This information is saved for faster display of To list in the message
        /// </summary>
        [XmlAttribute]
        public string To { get; set; }

        /// <summary>
        /// Message From
        /// </summary>
        [XmlAttribute]
        public string From { get; set; }

        /// <summary>
        /// Message Subject
        /// </summary>
        [XmlAttribute]
        public string Subject { get; set; }

        /// <summary>
        /// Message body
        /// </summary>
        [XmlAttribute]
        public string Body { get; set; }

        /// <summary>
        /// Conversation ID of the Message. Each message has at least one ConversationId. Subsequent Replies to a Message get same ConversationId
        /// </summary>
        [XmlAttribute]
        public int ConversationId { get; set; }

        /// <summary>
        /// ReplyAllAllowed is a bit value to indicate if the reply to the message can be sent to all the recipients or just the sender
        /// </summary>
        [XmlAttribute]
        public bool ReplyAllAllowed { get; set; }

        /// <summary>
        /// The UserID of the sender of the message
        /// </summary>
        [XmlAttribute]
        public int SenderUserID { get; set; }

        /// <summary>
        /// A pretty printed string with the time since the message was created
        /// </summary>
        [XmlAttribute]
        public string DisplayDate
        {
            get
            {
                if (string.IsNullOrEmpty(_displayDate))
                {
                    _displayDate = DateUtils.CalculateDateForDisplay(_createdOnDate);
                }
                return _displayDate;
            }
        }

        /// <summary>
        /// IHydratable.KeyID.
        /// </summary>
        [XmlIgnore]
        public int KeyID
        {
            get
            {
                return MessageID;
            }
            set
            {
                MessageID = value;
            }
        }

        /// <summary>
        /// RowNumber of the message in a set
        /// </summary>
        [XmlAttribute]
        public int RowNumber { get; set; }

        /// <summary>
        /// Count of Total Attachments in a Conversation. It is calculated by adding attachments in all the threads for a given conversation.
        /// </summary>
        [XmlAttribute]
        public int AttachmentCount { get; set; }

        /// <summary>
        /// Count of Total New (Unread) Threads in a Conversation. It is calculated by inspecting all the threads in a conversation and counting the ones that are not read yet.
        /// </summary>
        [XmlAttribute]
        public int NewThreadCount { get; set; }

        /// <summary>
        /// Count of Total Threads in a Conversation.
        /// </summary>
        [XmlAttribute]
        public int ThreadCount { get; set; }

        /// <summary>
        /// The Sender User Profile URL
        /// </summary>
        [XmlIgnore]
        public string SenderProfileUrl
        {
            get
            {
                return Globals.UserProfileURL(SenderUserID);
            }
        }

        /// <summary>
        /// Fill the object with data from database.
        /// </summary>
        /// <param name="dr">the data reader.</param>
        public void Fill(IDataReader dr)
        {
            MessageID = Convert.ToInt32(dr["MessageID"]);
            To = Null.SetNullString(dr["To"]);
            From = Null.SetNullString(dr["From"]);
            Subject = Null.SetNullString(dr["Subject"]);
            Body = Null.SetNullString(dr["Body"]);
            ConversationId = Null.SetNullInteger(dr["ConversationID"]);
            ReplyAllAllowed = Null.SetNullBoolean(dr["ReplyAllAllowed"]);
            SenderUserID = Convert.ToInt32(dr["SenderUserID"]);
            RowNumber = Convert.ToInt32(dr["RowNumber"]);
            AttachmentCount = Convert.ToInt32(dr["AttachmentCount"]);
            NewThreadCount = Convert.ToInt32(dr["NewThreadCount"]);
            ThreadCount = Convert.ToInt32(dr["ThreadCount"]);
            _createdOnDate = Null.SetNullDateTime(dr["CreatedOnDate"]);
        }
    }
}
