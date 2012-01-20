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
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using DotNetNuke.Entities.Users;

namespace DotNetNuke.Entities.Users
{
    internal interface IRelationshipController
    {             
        #region Relationship Business APIs

        UserRelationship InitiateUserRelationship(UserInfo initiatingUser, UserInfo targetUser, Relationship relationship);
       
        void AcceptUserRelationship(int userRelationshipID);

        void RejectUserRelationship(int userRelationshipID);

        void IgnoreUserRelationship(int userRelationshipID);

        void ReportUserRelationship(int userRelationshipID);

        void BlockUserRelationship(int userRelationshipID);

        void RemoveUserRelationship(int userRelationshipID);
                      
        #endregion

        #region Easy Wrapper APIs

        bool IsFriend(UserInfo targetUser);
        bool AreFriends(UserInfo initiatingUser, UserInfo targetUser);
        UserRelationship GetFriendRelationship(UserInfo targetUser);
        UserRelationship GetFriendRelationship(UserInfo initiatingUser, UserInfo targetUser);

        UserRelationship AddFriend(UserInfo targetUser);
        UserRelationship AddFriend(UserInfo initiatingUser, UserInfo targetUser);
        IList<UserRelationship> GetFriends(UserInfo initiatingUser);

        UserRelationship AddFollower(UserInfo targetUser);
        UserRelationship AddFollower(UserInfo initiatingUser, UserInfo targetUser);
        IList<UserRelationship> GetFollowers(UserInfo initiatingUser);
        IList<UserRelationship> GetFollowing(UserInfo initiatingUser);

        Relationship AddUserList(string listName, string listDescription);
        Relationship AddUserList(UserInfo owningUser, string listName, string listDescription, RelationshipStatus defaultStatus);
        Relationship AddPortalRelationship(int portalID, string listName, string listDescription, RelationshipStatus defaultStatus, int relationshipTypeID);

        #endregion
    }
}