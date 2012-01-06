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
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Entities.Users
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Users
    /// Class:      UserSocial
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The UserSocial is a high-level class describing social details of a user. 
    /// As an example, this class contains Friends, Followers, Follows lists.
    /// </summary>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class UserSocial
    {
        #region Private

        private UserInfo _userInfo;

        #endregion

        #region Constructor

        public UserSocial(UserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// List of Friends with UserRelationship Status as Accepted.
        /// </summary>
        [XmlAttribute]
        public IList<UserRelationship> Friends
        {
            get
            {
                var controller = new RelationshipController();
                return controller.GetFriends(_userInfo);
            }
        }

        /// <summary>
        /// List of Followerss with UserRelationship Status as Accepted.
        /// </summary>
        [XmlAttribute]
        public IList<UserRelationship> Followers
        {
            get
            {
                var controller = new RelationshipController();
                return controller.GetFollowers(_userInfo);
            }
        }

        #endregion
    }
}
