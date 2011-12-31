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

        UserRelationship AddFriend(UserInfo targetUser);
        UserRelationship AddFriend(UserInfo initiatingUser, UserInfo targetUser);

        Relationship AddUserList(string listName, string listDescription);
        Relationship AddUserList(UserInfo owningUser, string listName, string listDescription, RelationshipStatus defaultStatus);
        Relationship AddPortalRelationship(int portalID, string listName, string listDescription, RelationshipStatus defaultStatus, int relationshipTypeID);

        #endregion
    }
}