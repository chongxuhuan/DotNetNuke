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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Initiate a Relationship Request
        /// </summary>
        /// <param name="initiatingUser">UserInfo of the user initiating the request</param>        
        /// <param name="targetUser">UserInfo of the user being solicited for initiating the request</param>        
        /// <param name="relationship">Relationship to associate this request to (Portal-Level Relationship or User-Level Relationship)</param>        
        /// <remarks>
        /// If all conditions are met, Relationship object belonging to initiating user (if not already present) and UserRelationship objects are created.
        /// </remarks>
        /// <returns>
        /// Relationship object belonging to the initiating user
        /// </returns>
        /// <exception cref="RelationshipBlockedException">Target user has Blocked any relationship request from Initiating user</exception>
        /// <exception cref="InvalidRelationshipTypeException">Relationship type does not exist</exception>
        /// -----------------------------------------------------------------------------
        UserRelationship InitiateRelationship(UserInfo initiatingUser, UserInfo targetUser, Relationship relationship);

        /*
        void AcceptRelationship(int userRelationshipID);

        void RejectRelationship(int userRelationshipID);

        void BlockRelationship(int userRelationshipID);

        void RemoveRelationship(int userRelationshipID);
        */
              
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