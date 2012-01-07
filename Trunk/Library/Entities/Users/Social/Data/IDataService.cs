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

#endregion

namespace DotNetNuke.Entities.Users.Social.Data
{
    public interface IDataService
    {
        #region RelationshipType CRUD

        IDataReader GetAllRelationshipTypes();

        IDataReader GetRelationshipType(int relationshipTypeID);

        void DeleteRelationshipType(int relationshipTypeID);

        int SaveRelationshipType(RelationshipType relationshipType, int createUpdateUserID);

        #endregion

        #region Relationship CRUD

        IDataReader GetRelationship(int relationshipID);

        IDataReader GetRelationshipsByUserID(int userID);

        IDataReader GetRelationshipsByPortalID(int portalID);

        void DeleteRelationship(int relationshipID);

        void DeleteRelationshipByUserID(int userID);

        void DeleteRelationshipByPortalID(int portalID);

        int SaveRelationship(Relationship relationship, int createUpdateUserID);

        #endregion

        #region UserRelationship CRUD

        IDataReader GetUserRelationship(int userRelationshipID);

        IDataReader GetUserRelationship(int userID, int relatedUserID, RelationshipDirection relationshipDirection, RelationshipStatus status);

        IDataReader GetUserRelationship(int userID, int relatedUserID, int relationshipID, RelationshipDirection relationshipDirection);

        IDataReader GetUserRelationshipsByInitiatingUserID(int userID);

        IDataReader GetUserRelationshipsByRelatedUserID(int relatedUserID);

        IDataReader GetUserRelationshipsByRelatedUser(int relatedUserID, int relationshipID, RelationshipStatus status);

        IDataReader GetUserRelationshipsByRelationshipID(int relationshipID);

        void DeleteUserRelationship(int userRelationshipID);

        void DeleteUserRelationshipByUserID(int userID);

        void DeleteUserRelationshipByRelatedUserID(int relatedUserID);

        void DeleteUserRelationshipByRelationshipID(int relationshipID);

        int SaveUserRelationship(UserRelationship userRelationship, int createUpdateUserID);

        #endregion

        #region UserRelationshipPreference CRUD

        IDataReader GetUserRelationshipPreferenceByID(int preferenceID);

        IDataReader GetUserRelationshipPreference(int userID, int relationshipID);

        void DeleteUserRelationshipPreference(int preferenceID);

        int SaveUserRelationshipPreference(UserRelationshipPreference userRelationshipPreference, int createUpdateUserID);

        #endregion
    }
}