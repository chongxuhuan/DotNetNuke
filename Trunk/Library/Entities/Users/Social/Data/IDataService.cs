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

#endregion

namespace DotNetNuke.Entities.Users.Social.Data
{
    public interface IDataService
    {
        #region RelationshipType CRUD

        void DeleteRelationshipType(int relationshipTypeID);
        IDataReader GetAllRelationshipTypes();
        IDataReader GetRelationshipType(int relationshipTypeID);
        int SaveRelationshipType(RelationshipType relationshipType, int createUpdateUserID);

        #endregion

        #region Relationship CRUD

        void DeleteRelationship(int relationshipID);
        IDataReader GetRelationship(int relationshipID);
        IDataReader GetRelationshipsByUserID(int userID);
        IDataReader GetRelationshipsByPortalID(int portalID);
        int SaveRelationship(Relationship relationship, int createUpdateUserID);

        #endregion

        #region UserRelationship CRUD

        void DeleteUserRelationship(int userRelationshipID);
        IDataReader GetUserRelationship(int userRelationshipID);
        IDataReader GetUserRelationship(int userID, int relatedUserID, int relationshipID, RelationshipDirection relationshipDirection);
        IDataReader GetUserRelationships(int userID);
        IDataReader GetUserRelationshipsByRelationshipID(int relationshipID);
        IDataReader GetUsersByFilters(int portalId, int currUserId, int numberOfRecords, int roleId, int relationshipType, string profileProperty, string profilePropertyValue, string sortColumn, bool sortAcending, bool isAdmin);
        int SaveUserRelationship(UserRelationship userRelationship, int createUpdateUserID);

        #endregion

        #region UserRelationshipPreference CRUD

        void DeleteUserRelationshipPreference(int preferenceID);
        IDataReader GetUserRelationshipPreferenceByID(int preferenceID);
        IDataReader GetUserRelationshipPreference(int userID, int relationshipID);
        int SaveUserRelationshipPreference(UserRelationshipPreference userRelationshipPreference, int createUpdateUserID);

        #endregion
    }
}