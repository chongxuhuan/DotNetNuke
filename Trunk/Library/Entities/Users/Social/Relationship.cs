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
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Entities.Users
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Users
    /// Class:      Relationship
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Relationship class describes the relationships that a user or portal owns.  
    /// A handful of default Portal-Level Relationships will be be present for every portal (e.g. Friends, Followers, Family).  
    /// Portal-Level Relationship will have a -1 in UserID field.
    /// Any custom User-Level Relationship created by user will also be defined by this class (e.g. My InLaws, Engineering Group).
    /// User-Relationship will always have an associcated PortalID. User-Level Relationship will always be tied to a specific Portal.    
    /// </summary>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class Relationship : BaseEntityInfo, IHydratable
    {
        private int _relationshipID = -1;

        /// <summary>
        /// RelationshipID - The primary key
        /// </summary>
        [XmlAttribute]
        public int RelationshipID
        {
            get
            {
                return _relationshipID;
            }
            set
            {
                _relationshipID = value;
            }
        }

        /// <summary>
        /// Relationship Name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Relationship Description.
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// UserID of the User that owns the Relationship. A value of -1 indicates that it's a Portal-Level Relationship
        /// </summary>
        [XmlAttribute]
        public int UserID { get; set; }

        /// <summary>
        /// PortalID of the User that owns the Relationship. A value of -1 in UserID field indicates that it's a Portal-Level Relationship
        /// </summary>
        [XmlAttribute]
        public int PortalID { get; set; }

        /// <summary>
        /// The ID of the Relationship to which this Relation belongs to (e.g. Friend List or Coworkers)
        /// </summary>
        [XmlAttribute]
        public int RelationshipTypeID { get; set; }

        /// <summary>
        /// Default Relationship Status to be provided to any new Relationship Request
        /// </summary>
        [XmlAttribute]
        public RelationshipStatus DefaultResponse { get; set; }

        /// <summary>
        /// Is this a Portal-Level Relationship
        /// </summary>
        [XmlIgnore]
        public bool IsPortalList 
        { 
            get
            {
                return UserID == Null.NullInteger && PortalID >= 0;
            }
        }

        /// <summary>
        /// Is this a Host-Level Relationship (very uncommon)
        /// </summary>
        [XmlIgnore]
        public bool IsHostList
        {
            get
            {
                return UserID == Null.NullInteger && PortalID == Null.NullInteger;
            }
        }

        /// <summary>
        /// Is this a USer-Level Relationship 
        /// </summary>
        [XmlIgnore]
        public bool IsUserList
        {
            get
            {
                return UserID > 0 && PortalID >= 0;
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
                return this.RelationshipID;
            }
            set
            {
                this.RelationshipID = value;
            }
        }

        /// <summary>
        /// Fill the object with data from database.
        /// </summary>
        /// <param name="dr">the data reader.</param>
        public void Fill(IDataReader dr)
        {
            this.RelationshipID = Convert.ToInt32(dr["RelationshipID"]);
            this.UserID = Null.SetNullInteger(dr["UserID"]);
            this.PortalID = Null.SetNullInteger(dr["PortalID"]);
            this.Name = dr["Name"].ToString();
            this.Description = dr["Description"].ToString();
            this.DefaultResponse = (RelationshipStatus)Convert.ToInt32(dr["DefaultResponse"]);
            this.RelationshipTypeID = Convert.ToInt32(dr["RelationshipTypeID"]);
        }
    }
}
