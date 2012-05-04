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
    public class MessageSentView : Message, IHydratable
    {
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
        public new void Fill(IDataReader dr)
        {
            RowNumber = Convert.ToInt32(dr["RowNumber"]);
            AttachmentCount = Convert.ToInt32(dr["AttachmentCount"]);

            base.Fill(dr);            
        }
    }
}
