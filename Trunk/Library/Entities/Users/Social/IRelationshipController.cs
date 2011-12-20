using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using DotNetNuke.Entities.Users;

namespace DotNetNuke.Entities.Users
{
    public interface IRelationshipController
    {
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Save RelationshipType
        /// </summary>
        /// <param name="relationshipType">RelationshipType object</param>        
        /// <remarks>
        /// If RelationshipTypeID is -1 (Null.NullIntger), then a new RelationShipType is created, 
        /// else existing RelationShipType is updated
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
        RelationshipType GetRelationshipTypeByID(int relationshipTypeID);
    }
}