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

using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Services.Social.Notifications
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Services.Social.Notifications
    /// Class:      NotificationAction
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The NotificationAction class describes the entity that stores the extra information to be sent in the API call when a notification action is performed.
    /// </summary>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class NotificationAction : BaseEntityInfo, IHydratable
    {
        private int _notificationActionId = -1;

        /// <summary>
        /// The notification action identifier.
        /// </summary>
        [XmlAttribute]
        public int NotificationActionId
        {
            get
            {
                return _notificationActionId;
            }
            set
            {
                _notificationActionId = value;
            }
        }

        /// <summary>
        /// The notification identifier.
        /// </summary>
        [XmlAttribute]
        public int NotificationId { get; set; }

        /// <summary>
        /// The notification type action identifier.
        /// </summary>
        [XmlAttribute]
        public int NotificationTypeActionId { get; set; }

        /// <summary>
        /// The extra information to be sent to the API call when the notification action is performed.
        /// </summary>
        [XmlAttribute]
        public string Key { get; set; }

        #region Implementation of IHydratable

        /// <summary>
        /// IHydratable.KeyID.
        /// </summary>
        [XmlIgnore]
        public int KeyID
        {
            get
            {
                return NotificationActionId;
            }
            set
            {
                NotificationActionId = value;
            }
        }

        /// <summary>
        /// Fill the object with data from database.
        /// </summary>
        /// <param name="dr">the data reader.</param>
        public void Fill(IDataReader dr)
        {
            NotificationActionId = Convert.ToInt32(dr["NotificationActionID"]);
            NotificationId = Convert.ToInt32(dr["MessageID"]);
            NotificationTypeActionId = Convert.ToInt32(dr["NotificationTypeActionID"]);
            Key = dr["Key"].ToString();

            //add audit column data
            FillInternal(dr);
        }

        #endregion
    }
}
