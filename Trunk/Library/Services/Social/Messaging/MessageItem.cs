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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Services.Social.Messaging
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Messaging
    /// Class:      MessageRecipient
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The MessageItem class contains combined details of Message and Message Recipient
    /// </summary>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class MessageItem : MessageRecipient, IHydratable
    {
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
        /// messageID of the message -allows determination of reply rules and whether a message was sent to multiple recipients
        /// </summary>
        [XmlAttribute]
        public int ParentMessageID { get; set; }

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
        /// RowNumber of the message in a set
        /// </summary>
        [XmlAttribute]
        public int RowNumber { get; set; }

        /// <summary>
        /// Does the message contain attachments
        /// </summary>
        [XmlAttribute]
        public int AttachmentCount { get; set; }

        /// <summary>
        /// Fill the object with data from database.
        /// </summary>
        /// <param name="dr">the data reader.</param>
        public new void Fill(IDataReader dr)
        {
            this.To = Null.SetNullString(dr["To"]);
            this.From = Null.SetNullString(dr["From"]);
            this.Subject = Null.SetNullString(dr["Subject"]);
            this.Body = Null.SetNullString(dr["Body"]);
            this.ParentMessageID = Convert.ToInt32(dr["ParentMessageID"]);            
            this.ReplyAllAllowed = Null.SetNullBoolean(dr["ReplyAllAllowed"]);
            this.SenderUserID = Convert.ToInt32(dr["SenderUserID"]);
            this.RowNumber = Convert.ToInt32(dr["RowNumber"]);
            this.AttachmentCount = Convert.ToInt32(dr["AttachmentCount"]);

            base.Fill(dr);            
        }
    }
}
