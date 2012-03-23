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
using System.Data;
using System.Xml.Serialization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Services.Social.Messaging
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Services.Social.Notifications
    /// Class:      MessageType
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The MessageType class describes a single message type that can be associated to a message.
    /// This message could be a notification or a standard message sent between users.
    /// </summary>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class MessageType : IHydratable
    {
        private int _messageTypeId = -1;
        private int _timeToLive = -1;

        /// <summary>
        /// The message type identifier.
        /// </summary>
        [XmlAttribute]
        public int MessageTypeId
        {
            get
            {
                return _messageTypeId;
            }
            set
            {
                _messageTypeId = value;
            }
        }

        /// <summary>
        /// The message type name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// The message type description.
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// The number of minutes to add to the creation date of the message to calculate the expiration date.
        /// </summary>
        [XmlAttribute]
        public int TimeToLive
        {
            get
            {
                return _timeToLive;
            }
            set
            {
                _timeToLive = value;
            }
        }

        #region Implementation of IHydratable

        /// <summary>
        /// IHydratable.KeyID.
        /// </summary>
        [XmlIgnore]
        public int KeyID
        {
            get { return MessageTypeId; }
            set { MessageTypeId = value; }
        }

        /// <summary>
        /// Fill the object with data from database.
        /// </summary>
        /// <param name="dr">the data reader.</param>
        public void Fill(IDataReader dr)
        {
            MessageTypeId = Convert.ToInt32(dr["MessageTypeID"]);
            Name = dr["Name"].ToString();
            Description = Null.SetNullString(dr["Description"]);
            TimeToLive = Null.SetNullInteger(dr["TTL"]);
        }

        #endregion
    }
}
