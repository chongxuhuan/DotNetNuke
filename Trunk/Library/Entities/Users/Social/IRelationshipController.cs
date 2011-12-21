using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using DotNetNuke.Entities.Users;

namespace DotNetNuke.Entities.Users
{
    public interface IRelationshipController
    {
        #region RelationshipType CRUD

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Save RelationshipType By RelationshipTypeID
        /// </summary>
        /// <param name="relationshipType">RelationshipType object</param>        
        /// <remarks>
        /// If RelationshipTypeID is -1 (Null.NullIntger), then a new RelationshipType is created, 
        /// else existing RelationshipType is updated
        /// </remarks>
        /// -----------------------------------------------------------------------------
        void SaveRelationshipType(RelationshipType relationshipType);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete RelationshipType
        /// </summary>
        /// <param name="relationshipTypeID">RelationshipTypeID</param>        
        /// -----------------------------------------------------------------------------
        void DeleteRelationshipType(int relationshipTypeID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get list of All RelationshipTypes defined in system
        /// </summary>        
        /// -----------------------------------------------------------------------------
        IList<RelationshipType> GetAllRelationshipTypes();

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get RelationshipType By RelationshipTypeID
        /// </summary>        
        /// <param name="relationshipTypeID">RelationshipTypeID</param>        
        /// -----------------------------------------------------------------------------
        RelationshipType GetRelationshipType(int relationshipTypeID);
        
        #endregion

        #region Relationship CRUD

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Save Relationship
        /// </summary>
        /// <param name="relationship">Relationship object</param>        
        /// <remarks>
        /// If RelationshipID is -1 (Null.NullIntger), then a new Relationship is created, 
        /// else existing Relationship is updated
        /// </remarks>
        /// -----------------------------------------------------------------------------
        void SaveRelationship(Relationship relationship);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By RelationshipID
        /// </summary>
        /// <param name="relationshipID">RelationshipID</param>        
        /// -----------------------------------------------------------------------------
        void DeleteRelationship(int relationshipID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By UserID
        /// </summary>
        /// <param name="userID">UserID</param>        
        /// -----------------------------------------------------------------------------
        void DeleteRelationshipByUserID(int userID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By PortalID
        /// </summary>
        /// <param name="portalID">PortalID</param>        
        /// -----------------------------------------------------------------------------
        void DeleteRelationshipByPortalID(int portalID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get Relationship By RelationshipID
        /// </summary>        
        /// <param name="relationshipID">RelationshipID</param>        
        /// -----------------------------------------------------------------------------
        Relationship GetRelationship(int relationshipID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get Relationships By UserID
        /// </summary>        
        /// <param name="userID">UserID</param>        
        /// -----------------------------------------------------------------------------
        IList<Relationship> GetRelationshipsByUserID(int userID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get Relationships By PortalID
        /// </summary>        
        /// <param name="portalID">PortalID</param>        
        /// -----------------------------------------------------------------------------
        IList<Relationship> GetRelationshipsByPortalID(int portalID);


        #endregion
        
        #region UserRelationship CRUD

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Save UserRelationship
        /// </summary>
        /// <param name="userRelationship">UserRelationship object</param>        
        /// <remarks>
        /// If UserRelationshipID is -1 (Null.NullIntger), then a new UserRelationship is created, 
        /// else existing UserRelationship is updated
        /// </remarks>
        /// -----------------------------------------------------------------------------
        void SaveUserRelationship(UserRelationship userRelationship);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By UserRelationshipID
        /// </summary>
        /// <param name="userRelationshipID">UserRelationshipID</param>        
        /// -----------------------------------------------------------------------------
        void DeleteUserRelationship(int userRelationshipID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete USerRelationship By UserID
        /// </summary>
        /// <param name="userID">UserID</param>        
        /// -----------------------------------------------------------------------------
        void DeleteUserRelationshipByUserID(int userID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By RelatedUserlID
        /// </summary>
        /// <param name="relatedUserID">RelatedUserID</param>        
        /// -----------------------------------------------------------------------------
        void DeleteRelationshipByRelatedID(int relatedUserID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By RelationshipID
        /// </summary>
        /// <param name="relationshipID">RelationshipID</param>        
        /// -----------------------------------------------------------------------------
        void DeleteUserRelationshipByRelationshipID(int relationshipID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationship By UserRelationshipID
        /// </summary>        
        /// <param name="userRelationshipID">UserRelationshipID</param>        
        /// -----------------------------------------------------------------------------
        UserRelationship GetUserRelationship(int userRelationshipID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationship By UserID
        /// </summary>        
        /// <param name="userID">UserID</param>        
        /// -----------------------------------------------------------------------------
        IList<UserRelationship> GetUserRelationshipsByUserID(int userID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationship By RelatedUserID
        /// </summary>        
        /// <param name="relatedUserID">RelatedUserID</param>        
        /// -----------------------------------------------------------------------------
        IList<UserRelationship> GetUserRelationshipsByRelatedUserID(int relatedUserID);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationship By RelationshipID
        /// </summary>        
        /// <param name="relationshipID">RelationshipID</param>        
        /// -----------------------------------------------------------------------------
        IList<UserRelationship> GetUserRelationshipsByRelationshipID(int relationshipID);

        #endregion

        /*

        #region Relationship Business APIs

        Relationship InitiateRelationship(int initiatingUserID, int targetUserID, int relationshipTypeID );

        void AcceptRelationship(int userRelationshipID);

        void RejectRelationship(int userRelationshipID);

        void BlockRelationship(int userRelationshipID);

        void RemoveRelationship(int userRelationshipID);

        #endregion
         */
    }
}