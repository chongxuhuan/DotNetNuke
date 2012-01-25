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
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Entities.Users
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Users
    /// Class:      UserRelationship
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The UserRelationship class defines the membership of the relationship. 
    /// The user initiating the relationship is UserID. 
    /// The target of the relationship is RelatedUserID.  
    /// Status tracks relationship status as Initiated, Approved, Rejected etc.
    /// </summary>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class UserRelationship : BaseEntityInfo, IHydratable
    {
        private int _userRelationshipID = -1;

        /// <summary>
        /// UserRelationshipID - The primary key
        /// </summary>
        [XmlAttribute]
        public int UserRelationshipID
        {
            get
            {
                return _userRelationshipID;
            }
            set
            {
                _userRelationshipID = value;
            }
        }

        /// <summary>
        /// UserID of the User that owns the relationship
        /// </summary>
        [XmlAttribute]
        public int UserID { get; set; }

        /// <summary>
        /// The UserID of the Related User 
        /// </summary>
        [XmlAttribute]
        public int RelatedUserID { get; set; }

        /// <summary>
        /// The ID of the Relationship to which this Relation belongs to (e.g. Friend List or Coworkers)
        /// </summary>
        [XmlAttribute]
        public int RelationshipID { get; set; }

        /// <summary>
        /// The Status of the Relationship (e.g. Initiated, Accepted, Rejected)
        /// </summary>
        [XmlAttribute]
        public RelationshipStatus Status { get; set; }


        /// <summary>
        /// IHydratable.KeyID.
        /// </summary>
        [XmlIgnore]
        public int KeyID
        {
            get
            {
                return this.UserRelationshipID;
            }
            set
            {
                this.UserRelationshipID = value;
            }
        }

        /// <summary>
        /// Fill the object with data from database.
        /// </summary>
        /// <param name="dr">the data reader.</param>
        public void Fill(IDataReader dr)
        {
            this.UserRelationshipID = Convert.ToInt32(dr["UserRelationshipID"]);
            this.UserID = Convert.ToInt32(dr["UserID"]);
            this.RelatedUserID = Convert.ToInt32(dr["RelatedUserID"]);
            this.RelationshipID = Convert.ToInt32(dr["RelationshipID"]);
            this.Status = (RelationshipStatus)Convert.ToInt32(dr["Status"]);

            //add audit column data
            FillInternal(dr);
        }
    }
}
