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
using System.Data;

using DotNetNuke.ComponentModel;
using DotNetNuke.Data;

#endregion

namespace DotNetNuke.Entities.Users.Social.Data
{
    internal class DataService : ComponentBase<IDataService, DataService>, IDataService        
    {
        private readonly DataProvider _provider = DataProvider.Instance();

        #region RelationshipType CRUD

        public IDataReader GetAllRelationshipTypes()
        {
            return _provider.ExecuteReader("GetAllRelationshipTypes");
        }

        public IDataReader GetRelationshipType(int relationshipTypeID)
        {
            return _provider.ExecuteReader("GetRelationshipType", relationshipTypeID);
        }

        public void DeleteRelationshipType(int relationshipTypeID)
        {            
            _provider.ExecuteNonQuery("DeleteRelationshipType", relationshipTypeID);
        }

        public int SaveRelationshipType(RelationshipType relationshipType, int createUpdateUserID)
        {
            return _provider.ExecuteScalar<int>("SaveRelationshipType", relationshipType.RelationshipTypeID, relationshipType.Direction, relationshipType.Name, relationshipType.Description, createUpdateUserID);
        }

        #endregion

        #region Relationship CRUD

        public IDataReader GetRelationship(int relationshipID)
        {
            return _provider.ExecuteReader("GetRelationship", relationshipID);
        }

        public IDataReader GetRelationshipsByUserID(int userID)
        {
            return _provider.ExecuteReader("GetRelationshipsByUserID", userID);
        }

        public IDataReader GetRelationshipsByPortalID(int portalID)
        {
            return _provider.ExecuteReader("GetRelationshipsByPortalID", portalID);
        }

        public void DeleteRelationship(int relationshipID)
        {
            _provider.ExecuteNonQuery("DeleteRelationship", relationshipID);
        }

        public void DeleteRelationshipByUserID(int userID)
        {
            _provider.ExecuteNonQuery("DeleteRelationshipByUserID", userID);
        }

        public void DeleteRelationshipByPortalID(int portalID)
        {
            _provider.ExecuteNonQuery("DeleteRelationshipByPortalID", portalID);
        }

        public int SaveRelationship(Relationship relationship, int createUpdateUserID)
        {
            return _provider.ExecuteScalar<int>("SaveRelationship", relationship.RelationshipID, relationship.RelationshipTypeID, relationship.Name, relationship.Description, _provider.GetNull(relationship.UserID), _provider.GetNull(relationship.PortalID), relationship.DefaultResponse, createUpdateUserID);
        }

        #endregion

        #region UserRelationship CRUD
        public IDataReader GetUserRelationship(int userRelationshipID)
        {
            return _provider.ExecuteReader("GetUserRelationship", userRelationshipID);
        }

        public IDataReader GetUserRelationship(int userID, int relationshipID, RelationshipDirection relationshipDirection, RelationshipStatus status)
        {
            return _provider.ExecuteReader("GetUserRelationshipsByUserRelationshipAndStatus", userID, relationshipID, relationshipDirection, status);
        }

        public IDataReader GetUserRelationship(int userID, int relatedUserID, int relationshipID, RelationshipDirection relationshipDirection)
        {
            return _provider.ExecuteReader("GetUserRelationshipsByMultipleIDs", userID, relatedUserID, relationshipID, relationshipDirection);
        }

        public IDataReader GetUserRelationshipsByInitiatingUserID(int userID)
        {
            return _provider.ExecuteReader("GetUserRelationshipsByUserID", userID);
        }

        public IDataReader GetUserRelationshipsByRelatedUserID(int relatedUserID)
        {
            return _provider.ExecuteReader("GetUserRelationshipsByRelatedUserID", relatedUserID);
        }

        public IDataReader GetUserRelationshipsByRelatedUser(int relatedUserID, int relationshipID, RelationshipStatus status)
        {
            return _provider.ExecuteReader("GetUserRelationshipsByRelatedUser", relatedUserID, relationshipID, status);
        }

        public IDataReader GetUserRelationshipsByRelationshipID(int relationshipID)
        {
            return _provider.ExecuteReader("GetUserRelationshipsByRelationshipID", relationshipID);
        }

        public void DeleteUserRelationship(int userRelationshipID)
        {
            _provider.ExecuteNonQuery("DeleteRelationship", userRelationshipID);
        }

        public void DeleteUserRelationshipByUserID(int userID)
        {
            _provider.ExecuteNonQuery("DeleteRelationshipByUserID", userID);
        }

        public void DeleteUserRelationshipByRelatedUserID(int relatedUserID)
        {
            _provider.ExecuteNonQuery("DeleteRelationshipByRelatedUserID", relatedUserID);
        }

        public void DeleteUserRelationshipByRelationshipID(int relationshipID)
        {
            _provider.ExecuteNonQuery("DeleteRelationshipByRelatationshipID", relationshipID);
        }

        public int SaveUserRelationship(UserRelationship userRelationship, int createUpdateUserID)
        {
            return _provider.ExecuteScalar<int>("SaveUserRelationship", userRelationship.UserRelationshipID, userRelationship.UserID, userRelationship.RelatedUserID, userRelationship.RelationshipID, userRelationship.Status, createUpdateUserID);
        }

        #endregion

        #region UserRelationshipPreference CRUD

        public IDataReader GetUserRelationshipPreferenceByID(int preferenceID)
        {
            return _provider.ExecuteReader("GetUserRelationshipPreferenceByID", preferenceID);
        }

        public IDataReader GetUserRelationshipPreference(int userID, int relationshipID)
        {
            return _provider.ExecuteReader("GetUserRelationshipPreference", userID, relationshipID);
        }

        public void DeleteUserRelationshipPreference(int preferenceID)
        {
            _provider.ExecuteNonQuery("DeleteUserRelationshipPreference", preferenceID);
        }

        public int SaveUserRelationshipPreference(UserRelationshipPreference userRelationshipPreference, int createUpdateUserID)
        {
            return _provider.ExecuteScalar<int>("SaveUserRelationshipPreference", userRelationshipPreference.PreferenceID, userRelationshipPreference.UserID, userRelationshipPreference.RelationshipID, userRelationshipPreference.DefaultResponse, createUpdateUserID);
        }

        #endregion
    }
}