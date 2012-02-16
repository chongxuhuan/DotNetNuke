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

using System.Collections.Generic;
using System.Data;
using System.Linq;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users.Social.Data;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Entities.Users.Social
{
	/// <summary>
	/// Business Layer to manage Relationships. Also contains CRUD methods.
	/// </summary>
    public class RelationshipController : ComponentBase<IRelationshipController, RelationshipController>, IRelationshipController
    {
        #region Private Members

        private readonly IDataService _dataService;
	    private readonly IEventLogController _eventLogController;

        #endregion

        #region Constructors

        public RelationshipController() : this(DataService.Instance, new EventLogController())
        {
        }

        public RelationshipController(IDataService dataService, IEventLogController eventLogController)
        {
            //Argument Contract
            Requires.NotNull("dataService", dataService);
            Requires.NotNull("eventLogController", eventLogController);

            _dataService = dataService;
            _eventLogController = eventLogController;
        }

        #endregion

		#region Public Methods

        #region RelationshipType CRUD

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete RelationshipType
        /// </summary>
        /// <param name="relationshipType">RelationshipType</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteRelationshipType(RelationshipType relationshipType)
        {
            Requires.NotNull("relationshipType", relationshipType);

            _dataService.DeleteRelationshipType(relationshipType.RelationshipTypeId);

            //log event
            var logContent = string.Format(Localization.GetString("RelationshipType_Deleted", Localization.GlobalResourceFile), relationshipType.Name, relationshipType.RelationshipTypeId);
            AddLog(logContent);

            //clear cache
            DataCache.RemoveCache(DataCache.RelationshipTypesCacheKey);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get list of All RelationshipTypes defined in system
        /// </summary>        
        /// -----------------------------------------------------------------------------
        public IList<RelationshipType> GetAllRelationshipTypes()
        {
            var cacheArgs = new CacheItemArgs(DataCache.RelationshipTypesCacheKey, 
                                                DataCache.RelationshipTypesCacheTimeOut, 
                                                DataCache.RelationshipTypesCachePriority);
            return CBO.GetCachedObject<IList<RelationshipType>>(cacheArgs, c => CBO.FillCollection<RelationshipType>(_dataService.GetAllRelationshipTypes()));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get RelationshipType By RelationshipTypeId
        /// </summary>        
        /// <param name="relationshipTypeId">RelationshipTypeId</param>        
        /// -----------------------------------------------------------------------------
        public RelationshipType GetRelationshipType(int relationshipTypeId)
        {
            return GetAllRelationshipTypes().FirstOrDefault(r => r.RelationshipTypeId == relationshipTypeId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Save RelationshipType
        /// </summary>
        /// <param name="relationshipType">RelationshipType object</param>        
        /// <remarks>
        /// If RelationshipTypeId is -1 (Null.NullIntger), then a new RelationshipType is created, 
        /// else existing RelationshipType is updated
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public void SaveRelationshipType(RelationshipType relationshipType)
        {
            Requires.NotNull("relationshipType", relationshipType);

            var localizationKey = (relationshipType.RelationshipTypeId == Null.NullInteger) ? "RelationshipType_Added" : "RelationshipType_Updated";

            relationshipType.RelationshipTypeId = _dataService.SaveRelationshipType(relationshipType, UserController.GetCurrentUserInfo().UserID);

            //log event
            var logContent = string.Format(Localization.GetString(localizationKey, Localization.GlobalResourceFile), relationshipType.Name);
            AddLog(logContent);

            //clear cache
            DataCache.RemoveCache(DataCache.RelationshipTypesCacheKey);
        }

        #endregion

        #region Relationship CRUD

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship
        /// </summary>
        /// <param name="relationship">Relationship</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteRelationship(Relationship relationship)
        {
            Requires.NotNull("relationship", relationship);

            _dataService.DeleteRelationship(relationship.RelationshipId);

            //log event
            var logContent = string.Format(Localization.GetString("Relationship_Deleted", Localization.GlobalResourceFile), relationship.Name, relationship.RelationshipId);
            AddLog(logContent);

            //clear cache
            ClearRelationshipCache(relationship);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get Relationship By RelationshipId
        /// </summary>        
        /// <param name="relationshipId">RelationshipId</param>        
        /// -----------------------------------------------------------------------------
        public Relationship GetRelationship(int relationshipId)
        {
            return CBO.FillCollection<Relationship>(_dataService.GetRelationship(relationshipId)).FirstOrDefault();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get Relationships By UserId
        /// </summary>        
        /// <param name="userId">UserId</param>        
        /// -----------------------------------------------------------------------------
        public IList<Relationship> GetRelationshipsByUserId(int userId)
        {
            return CBO.FillCollection<Relationship>(_dataService.GetRelationshipsByUserId(userId));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get Relationships By PortalId
        /// </summary>        
        /// <param name="portalId">PortalId</param>        
        /// -----------------------------------------------------------------------------
        public IList<Relationship> GetRelationshipsByPortalId(int portalId)
        {
            var cacheArgs = new CacheItemArgs(string.Format(DataCache.RelationshipByPortalIDCacheKey, portalId), 
                                                DataCache.RelationshipByPortalIDCacheTimeOut, 
                                                DataCache.RelationshipByPortalIDCachePriority, 
                                                portalId);
            return CBO.GetCachedObject<IList<Relationship>>(cacheArgs, c => CBO.FillCollection<Relationship>(_dataService.GetRelationshipsByPortalId((int) c.ParamList[0])));            
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Save Relationship
        /// </summary>
        /// <param name="relationship">Relationship object</param>        
        /// <remarks>
        /// If RelationshipId is -1 (Null.NullIntger), then a new Relationship is created, 
        /// else existing Relationship is updated
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public void SaveRelationship(Relationship relationship)
        {
            Requires.NotNull("relationship", relationship);

            var localizationKey = (relationship.RelationshipId == Null.NullInteger) ? "Relationship_Added" : "Relationship_Updated";

            relationship.RelationshipId = _dataService.SaveRelationship(relationship, UserController.GetCurrentUserInfo().UserID);

            //log event
            var logContent = string.Format(Localization.GetString(localizationKey, Localization.GlobalResourceFile), relationship.Name);
            AddLog(logContent);

            //clear cache
            ClearRelationshipCache(relationship);
        }

        #endregion

        #region UserRelationship CRUD

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete UserRelationship
        /// </summary>
        /// <param name="userRelationship">UserRelationship to delete</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteUserRelationship(UserRelationship userRelationship)
        {
            Requires.NotNull("userRelationship", userRelationship);

            _dataService.DeleteUserRelationship(userRelationship.UserRelationshipId);

            //log event
            var logContent = string.Format(Localization.GetString("UserRelationship_Deleted", Localization.GlobalResourceFile), userRelationship.UserRelationshipId, userRelationship.UserId, userRelationship.RelatedUserId);
            AddLog(logContent);

            //cache clear
            ClearUserCache(userRelationship);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationship By UserRelationshipId
        /// </summary>        
        /// <param name="userRelationshipId">UserRelationshipId</param>        
        /// -----------------------------------------------------------------------------
        public UserRelationship GetUserRelationship(int userRelationshipId)
        {
            return CBO.FillObject<UserRelationship>(_dataService.GetUserRelationship(userRelationshipId));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationship by its members
        /// </summary>        
        /// <param name="user">User</param>        
        /// <param name="relatedUser">Related User</param>   
        /// <param name="relationship">Relationship Object</param>             
        /// -----------------------------------------------------------------------------
        public UserRelationship GetUserRelationship(UserInfo user, UserInfo relatedUser, Relationship relationship)
        {
            return CBO.FillObject<UserRelationship>(_dataService.GetUserRelationship(user.UserID, relatedUser.UserID, relationship.RelationshipId, GetRelationshipType(relationship.RelationshipTypeId).Direction));
        }

        /// <summary>
        /// This method gets a Dictionary of all the relationships that a User belongs to and the members of thase relationships.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>A Dictionary of Lists of UserRelationship, keyed by the Relationship</returns>
        public IList<UserRelationship> GetUserRelationships(UserInfo user)
        {
            return CBO.FillCollection<UserRelationship>(_dataService.GetUserRelationships(user.UserID));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Save UserRelationship
        /// </summary>
        /// <param name="userRelationship">UserRelationship object</param>        
        /// <remarks>
        /// If UserRelationshipId is -1 (Null.NullIntger), then a new UserRelationship is created, 
        /// else existing UserRelationship is updated
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public void SaveUserRelationship(UserRelationship userRelationship)
        {
            Requires.NotNull("userRelationship", userRelationship);

            var localizationKey = (userRelationship.UserRelationshipId == Null.NullInteger) ? "UserRelationship_Added" : "UserRelationship_Updated";

            userRelationship.UserRelationshipId = _dataService.SaveUserRelationship(userRelationship, UserController.GetCurrentUserInfo().UserID);

            //log event            
            var logContent = string.Format(Localization.GetString(localizationKey, Localization.GlobalResourceFile), userRelationship.UserRelationshipId, userRelationship.UserId, userRelationship.RelatedUserId);
            AddLog(logContent);

            //cache clear
            ClearUserCache(userRelationship);
        }

        #endregion

        #region UserRelationshipPreference CRUD

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete UserRelationshipPreference
        /// </summary>
        /// <param name="userRelationshipPreference">UserRelationshipPreference to delete</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteUserRelationshipPreference(UserRelationshipPreference userRelationshipPreference)
        {
            Requires.NotNull("userRelationshipPreference", userRelationshipPreference); 
            
            _dataService.DeleteUserRelationshipPreference(userRelationshipPreference.PreferenceId);

            //log event
            var logContent = string.Format(Localization.GetString("UserRelationshipPreference_Deleted", Localization.GlobalResourceFile), userRelationshipPreference.PreferenceId, userRelationshipPreference.UserId, userRelationshipPreference.RelationshipId);
            AddLog(logContent);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationshipPreference By RelationshipTypeId
        /// </summary>        
        /// <param name="preferenceId">PreferenceId</param>        
        /// -----------------------------------------------------------------------------
        public UserRelationshipPreference GetUserRelationshipPreference(int preferenceId)
        {
            return CBO.FillObject<UserRelationshipPreference>(_dataService.GetUserRelationshipPreferenceById(preferenceId));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationshipPreference By UserId and RelationshipId
        /// </summary>        
        /// <param name="userId">UserId</param>        
        /// <param name="relationshipId">RelationshipId</param>        
        /// -----------------------------------------------------------------------------
        public UserRelationshipPreference GetUserRelationshipPreference(int userId, int relationshipId)
        {
            return CBO.FillObject<UserRelationshipPreference>(_dataService.GetUserRelationshipPreference(userId, relationshipId));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Save UserRelationshipPreference
        /// </summary>
        /// <param name="userRelationshipPreference">UserRelationshipPreference object</param>        
        /// <remarks>
        /// If PreferenceId is -1 (Null.NullIntger), then a new UserRelationshipPreference is created, 
        /// else existing UserRelationshipPreference is updated
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public void SaveUserRelationshipPreference(UserRelationshipPreference userRelationshipPreference)
        {
            Requires.NotNull("userRelationshipPreference", userRelationshipPreference);

            var localizationKey = (userRelationshipPreference.PreferenceId == Null.NullInteger) ? "UserRelationshipPreference_Added" : "UserRelationshipPreference_Updated";

            userRelationshipPreference.PreferenceId = _dataService.SaveUserRelationshipPreference(userRelationshipPreference, UserController.GetCurrentUserInfo().UserID);

            //log event            
            var logContent = string.Format(Localization.GetString(localizationKey, Localization.GlobalResourceFile), userRelationshipPreference.PreferenceId, userRelationshipPreference.UserId, userRelationshipPreference.RelationshipId);
            AddLog(logContent);
        }

        #endregion

        #region Relationship Business APIs

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Initiate an UserRelationship Request
        /// </summary>
        /// <param name="initiatingUser">UserInfo of the user initiating the request</param>        
        /// <param name="targetUser">UserInfo of the user being solicited for initiating the request</param>        
        /// <param name="relationship">Relationship to associate this request to (Portal-Level Relationship or User-Level Relationship)</param>        
        /// <remarks>
        /// If all conditions are met UserRelationship object belonging to Initiating User is returned.
        /// </remarks>
        /// <returns>
        /// Relationship object belonging to the initiating user
        /// </returns>
        /// <exception cref="UserRelationshipBlockedException">Target user has Blocked any relationship request from Initiating user</exception>
        /// <exception cref="InvalidRelationshipTypeException">Relationship type does not exist</exception>
        /// -----------------------------------------------------------------------------
        public UserRelationship InitiateUserRelationship(UserInfo initiatingUser, UserInfo targetUser, Relationship relationship)
        {
            Requires.NotNull("initiatingUser", initiatingUser);
            Requires.NotNull("targetUser", targetUser);
            Requires.NotNull("relationship", relationship);

            Requires.PropertyNotNegative("initiatingUser", "UserID", initiatingUser.UserID);
            Requires.PropertyNotNegative("targetUser", "UserID", targetUser.UserID);

            Requires.PropertyNotNegative("initiatingUser", "PortalID", initiatingUser.PortalID);
            Requires.PropertyNotNegative("targetUser", "PortalID", targetUser.PortalID);

            Requires.PropertyNotNegative("relationship", "RelationshipId", relationship.RelationshipId);

            //cannot be same user
            if (initiatingUser.UserID == targetUser.UserID)
            {
                throw new UserRelationshipForSameUsersException(Localization.GetExceptionMessage("UserRelationshipForSameUsersError", "Initiating and Target Users cannot have same UserID '{0}'.", initiatingUser.UserID));
            }

            //users must be from same portal
            if (initiatingUser.PortalID != targetUser.PortalID)
            {
                throw new UserRelationshipForDifferentPortalException(Localization.GetExceptionMessage("UserRelationshipForDifferentPortalError", "Portal ID '{0}' of Initiating User is different from Portal ID '{1}' of Target  User.", initiatingUser.PortalID, targetUser.PortalID));
            }

            //check for existing UserRelationship record
            var existingRelationship = GetUserRelationship(initiatingUser, targetUser, relationship);
            if(existingRelationship != null) 
            {                                    
                throw new UserRelationshipExistsException(Localization.GetExceptionMessage("UserRelationshipExistsError", "Relationship already exists for Initiating User '{0}' Target User '{1}' RelationshipID '{2}'.", initiatingUser.UserID, targetUser.UserID, relationship.RelationshipId));
            }

            //no existing UserRelationship record found 

            
            //use Relationship DefaultResponse as status
            var status = relationship.DefaultResponse;

            //check if there is a custom relationship status setting for the user. 
            //TODO - Is this check only applicable for portal or host list
            //if (relationship.IsPortalList || relationship.IsHostList)
            {
                var preference = GetUserRelationshipPreference(targetUser.UserID, relationship.RelationshipId);
                if (preference != null) 
                {
                    status = preference.DefaultResponse;
                }                
            }

            if(status == RelationshipStatus.None)
                status = RelationshipStatus.Initiated;

            var userRelationship = new UserRelationship { UserRelationshipId = Null.NullInteger, UserId = initiatingUser.UserID, RelatedUserId = targetUser.UserID, RelationshipId = relationship.RelationshipId, Status = status };

            SaveUserRelationship(userRelationship);

            return userRelationship;            
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Accept an existing UserRelationship Request
        /// </summary>
        /// <param name="userRelationshipId">UserRelationshipId of the UserRelationship</param>        
        /// <remarks>
        /// Method updates the status of the UserRelationship to Accepted.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public void AcceptUserRelationship(int userRelationshipId)
        {
            ManageUserRelationshipStatus(userRelationshipId, RelationshipStatus.Accepted);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Reject an existing UserRelationship Request
        /// </summary>
        /// <param name="userRelationshipId">UserRelationshipId of the UserRelationship</param>        
        /// <remarks>
        /// Method updates the status of the UserRelationship to Rejected.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public void RejectUserRelationship(int userRelationshipId)
        {
            ManageUserRelationshipStatus(userRelationshipId, RelationshipStatus.Rejected);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Report an existing UserRelationship Request
        /// </summary>
        /// <param name="userRelationshipId">UserRelationshipId of the UserRelationship</param>        
        /// <remarks>
        /// Method updates the status of the UserRelationship to Reported.
        /// </remarks>
        /// -----------------------------------------------------------------------------        
        public void ReportUserRelationship(int userRelationshipId)
        {
            ManageUserRelationshipStatus(userRelationshipId, RelationshipStatus.Reported);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Ignore an existing UserRelationship Request
        /// </summary>
        /// <param name="userRelationshipId">UserRelationshipId of the UserRelationship</param>        
        /// <remarks>
        /// Method updates the status of the UserRelationship to Ignored.
        /// </remarks>
        /// ----------------------------------------------------------------------------- 
        public void IgnoreUserRelationship(int userRelationshipId)
        {
            ManageUserRelationshipStatus(userRelationshipId, RelationshipStatus.Ignored);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Block an existing UserRelationship Request
        /// </summary>
        /// <param name="userRelationshipId">UserRelationshipId of the UserRelationship</param>        
        /// <remarks>
        /// Method updates the status of the UserRelationship to Blocked.
        /// </remarks>
        /// ----------------------------------------------------------------------------- 
        public void BlockUserRelationship(int userRelationshipId)
        {
            ManageUserRelationshipStatus(userRelationshipId, RelationshipStatus.Blocked);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Remove an existing UserRelationship Request
        /// </summary>
        /// <param name="userRelationshipId">UserRelationshipId of the UserRelationship</param>        
        /// <remarks>
        /// UserRelationship record is physically removed.
        /// </remarks>
        /// -----------------------------------------------------------------------------  
        public void RemoveUserRelationship(int userRelationshipId)
        {
            var userRelationship = VerifyUserRelationshipExist(userRelationshipId);
            if (userRelationship != null)
            {
                DeleteUserRelationship(userRelationship);
            }
        }       
        

        #endregion

        #region Easy Wrapper APIs

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// IsFriend - Are the Current User and the Target Users in Friend Relationship
        /// </summary>        
        /// <param name="targetUser">UserInfo for Target User</param>        
        /// <returns>True or False</returns>
        /// <remarks>True is returned only if a Friend Relationship exists between the two Users. 
        /// The relation status must be Accepted. Friend Relationship can be initited by either of the Users.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public bool IsFriend(UserInfo targetUser)
        {
            return AreFriends(UserController.GetCurrentUserInfo(), targetUser);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AreFriends - Are the Initiating User and the Target Users in Friend Relationship
        /// </summary>        
        /// <param name="initiatingUser">UserInfo for Initiating User</param>                
        /// <param name="targetUser">UserInfo for Target User</param>        
        /// <returns>True or False</returns>
        /// <remarks>True is returned only if a Friend Relationship exists between the two Users. 
        /// The relation status must be Accepted. Friend Relationship can be initited by either of the Users.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public bool AreFriends(UserInfo initiatingUser, UserInfo targetUser)
        {
            Requires.NotNull("initiatingUser", initiatingUser);
            Requires.NotNull("targetUser", targetUser);

            Relationship friend = GetFriendsRelatioshipByPortal(initiatingUser.PortalID);

            UserRelationship userRelationship = GetUserRelationship(initiatingUser, targetUser, friend);

            return (userRelationship != null && userRelationship.Status == RelationshipStatus.Accepted);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetFriendRelationship - Get the UserRelationship between Current User and the Target Users in Friend Relationship
        /// </summary>        
        /// <param name="targetUser">UserInfo for Target User</param>        
        /// <returns>UserRelationship</returns>
        /// <remarks>UserRelationship object is returned if a Friend Relationship exists between the two Users. 
        /// The relation status can be Any (Initiated / Accepted / Blocked). Friend Relationship can be initited by either of the Users.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public UserRelationship GetFriendRelationship(UserInfo targetUser)
        {
            return GetFriendRelationship(UserController.GetCurrentUserInfo(), targetUser);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetFriendRelationship - Get the UserRelationship between InitiatingUser User and the Target Users in Friend Relationship
        /// </summary>        
        /// <param name="initiatingUser">UserInfo for Initiating User</param>        
        /// <param name="targetUser">UserInfo for Target User</param>        
        /// <returns>UserRelationship</returns>
        /// <remarks>UserRelationship object is returned if a Friend Relationship exists between the two Users. 
        /// The relation status can be Any (Initiated / Accepted / Blocked). Friend Relationship can be initited by either of the Users.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public UserRelationship GetFriendRelationship(UserInfo initiatingUser, UserInfo targetUser)
        {
            Requires.NotNull("initiatingUser", initiatingUser);
            Requires.NotNull("targetUser", targetUser);

            return GetUserRelationship(initiatingUser, targetUser, GetFriendsRelatioshipByPortal(initiatingUser.PortalID));
        }

	    /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddFriend - Current User initiates a Friend Request to the Target User
        /// </summary>                
        /// <param name="targetUser">UserInfo for Target User</param>        
        /// <returns>UserRelationship object</returns>
        /// <remarks>If the Friend Relationship is setup for auto-acceptance at the Portal level, the UserRelationship
        /// status is set as Accepted, otherwise it is set as Initiated.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public UserRelationship AddFriend(UserInfo targetUser)
        {
            return AddFriend(UserController.GetCurrentUserInfo(), targetUser);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddFriend - Initiating User initiates a Friend Request to the Target User
        /// </summary>        
        /// <param name="initiatingUser">UserInfo for Initiating User</param>        
        /// <param name="targetUser">UserInfo for Target User</param>        
        /// <returns>UserRelationship object</returns>
        /// <remarks>If the Friend Relationship is setup for auto-acceptance at the Portal level, the UserRelationship
        /// status is set as Accepted, otherwise it is set as Initiated.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public UserRelationship AddFriend(UserInfo initiatingUser, UserInfo targetUser)
        {
            Requires.NotNull("initiatingUser", initiatingUser);

            return InitiateUserRelationship(initiatingUser, targetUser, GetFriendsRelatioshipByPortal(initiatingUser.PortalID));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddFollower - Current User initiates a Follower Request to the Target User
        /// </summary>        
        /// <param name="targetUser">UserInfo for Target User</param>        
        /// <returns>UserRelationship object</returns>
        /// <remarks>If the Follower Relationship is setup for auto-acceptance (default) at the Portal level, the UserRelationship
        /// status is set as Accepted, otherwise it is set as Initiated.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public UserRelationship AddFollower(UserInfo targetUser)
        {
            return AddFollower(UserController.GetCurrentUserInfo(), targetUser);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddFollower - Initiating User initiates a Friend Request to the Target User
        /// </summary>        
        /// <param name="initiatingUser">UserInfo for Initiating User</param>        
        /// <param name="targetUser">UserInfo for Target User</param>        
        /// <returns>UserRelationship object</returns>
        /// <remarks>If the Follower Relationship is setup for auto-acceptance (default) at the Portal level, the UserRelationship
        /// status is set as Accepted, otherwise it is set as Initiated.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public UserRelationship AddFollower(UserInfo initiatingUser, UserInfo targetUser)
        {
            Requires.NotNull("initiatingUser", initiatingUser);

            return InitiateUserRelationship(initiatingUser, targetUser, GetFollowersRelatioshipByPortal(initiatingUser.PortalID));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetFriends - Get List of Friends (UserRelationship with status as Accepted) for an User
        /// </summary>        
        /// <param name="initiatingUser">UserInfo for Initiating User</param>        
        /// <returns>List of UserRelationship objects</returns>
        /// <remarks>This method can bring huge set of data.         
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public IList<UserRelationship> GetFriends(UserInfo initiatingUser)
        {
            Requires.NotNull("initiatingUser", initiatingUser);

            IList<UserRelationship> friends;
            var relationship = GetFriendsRelatioshipByPortal(initiatingUser.PortalID);
            initiatingUser.Social.UserRelationships.TryGetValue(relationship.RelationshipId, out friends);

            return friends;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetFollowers - Get List of Followers (UserRelationship with status as Accepted) for an User
        /// </summary>        
        /// <param name="initiatingUser">UserInfo for Initiating User</param>        
        /// <returns>List of UserRelationship objects</returns>
        /// <remarks>This method can bring huge set of data. Followers means the User is being Followed by x other Users        
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public IList<UserRelationship> GetFollowers(UserInfo initiatingUser)
        {
            Requires.NotNull("initiatingUser", initiatingUser);

            IList<UserRelationship> followers;
            var relationship = GetFollowersRelatioshipByPortal(initiatingUser.PortalID);
            initiatingUser.Social.UserRelationships.TryGetValue(relationship.RelationshipId, out followers);

            return followers;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetFriends - Get List of Following (UserRelationship with status as Accepted) for an User
        /// </summary>        
        /// <param name="initiatingUser">UserInfo for Initiating User</param>        
        /// <returns>List of UserRelationship objects</returns>
        /// <remarks>This method can bring huge set of data. Following means the User is Following x other Users        
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public IList<UserRelationship> GetFollowing(UserInfo initiatingUser)
        {
            Requires.NotNull("initiatingUser", initiatingUser);

            var relationship = GetFriendsRelatioshipByPortal(initiatingUser.PortalID);
            return new List<UserRelationship>();
        }

	    /// <summary>
	    /// GetUsersAdvancedSearch -- Gets a filtered list of users along with their profile properties.
	    /// </summary>
	    /// <param name="user"> User which could be used in conjunction with other parameters to filter the list in terms of relationship to.</param>
	    /// <param name="role">Filter the list of user based on their role.</param>
	    /// <param name="relationshipType">Filter the list of user based on their relationship to the current user [currUser parameter].</param>
	    /// <param name="propertyNamesValues"> A collection of Key Value pairs of property names and values to filter upon.</param>
	    /// <param name="additionalParameters"> A collection of Key Value pairs of more fields to filter upon</param>
	    /// <returns></returns>
	    public IDataReader GetUsersAdvancedSearch(UserInfo user, UserRoleInfo role, RelationshipType relationshipType, IDictionary<string, string> propertyNamesValues, Dictionary<string, string> additionalParameters)
        {
            if (additionalParameters == null) additionalParameters = new Dictionary<string, string>();

            var portalId = additionalParameters.ContainsKey("PortalId") && additionalParameters["PortalId"] != null ? int.Parse(additionalParameters["PortalId"].ToString()) : user.PortalID;
            var pageIndex = additionalParameters.ContainsKey("PageIndex") && additionalParameters["PageIndex"] != null ? int.Parse(additionalParameters["PageIndex"].ToString()) : 1;
            var records = additionalParameters.ContainsKey("Records") && additionalParameters["Records"] != null ? int.Parse(additionalParameters["Records"].ToString()) : 1;
            var sortByColumn = additionalParameters.ContainsKey("SortBy") && additionalParameters["SortBy"] != null ? additionalParameters["SortBy"].ToString() : string.Empty;
            var sortAscending = additionalParameters.ContainsKey("SortAscending") && additionalParameters["SortAscending"] != null ? bool.Parse(additionalParameters["SortAscending"].ToString()) : true;
            var propertyNamesFilter = propertyNamesValues.Aggregate("", (current, property) => current + "," + property.Key);
            var propertyValuesFilter = propertyNamesValues.Aggregate("", (current, property) => current + "," + property.Value);

            // Currently not applicable
            //var isAdmin = additionalParameters.ContainsKey("IsAdmin") && additionalParameters["IsAdmin"] != null ? bool.Parse(additionalParameters["IsAdmin"].ToString()) : true;
            //var roleId = role != null ? role.RoleID : -1;
            //var relationshipTypeId = relationshipType != null ? relationshipType.RelationshipTypeId : -1;

            return _dataService.GetUsersAdvancedSearch(portalId, records, pageIndex, sortByColumn, sortAscending, propertyNamesFilter, propertyValuesFilter);//, displayName);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddUserList - Add a new Relationship for the Current User, e.g. My Best Friends or My Inlaws.
        /// </summary>        
        /// <param name="listName">Name of the Relationship. It is also called as List Name, e.g. My School Friends.</param> 
        /// <param name="listDescription">Description of the Relationship. It is also called as List Description </param>         
        /// <returns>Relationship object</returns>
        /// <remarks>Each Portal contains default Relationships of Friends and Followers. 
        /// AddUserList can be used to create addiotional User-Level Relationships.
        /// A DefaultStatus of None is used in this call, indicating there will not be any auto-acceptance 
        /// when an UserRelationship request is made for this Relationship.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public Relationship AddUserList(string listName, string listDescription)
        {
            return AddUserList(UserController.GetCurrentUserInfo(), listName, listDescription, RelationshipStatus.None);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddUserList - Add a new Relationship for an User, e.g. My Best Friends or My Inlaws.
        /// </summary>        
        /// <param name="owningUser">UserInfo for the User to which the Relationship will belong. </param>        
        /// <param name="listName">Name of the Relationship. It is also called as List Name, e.g. My School Friends.</param> 
        /// <param name="listDescription">Description of the Relationship. It is also called as List Description </param>         
        /// <param name="defaultStatus">DefaultRelationshiStatus of the Relationship when a new UserRelationship is initiated. 
        /// E.g. Accepted, which means automatically accept all UserRelationship request. Default is None, meaning no automatic action.</param>         
        /// <returns>Relationship object</returns>
        /// <remarks>Each Portal contains default Relationships of Friends and Followers. 
        /// AddUserList can be used to create addiotional User-Level Relationships.
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public Relationship AddUserList(UserInfo owningUser, string listName, string listDescription, RelationshipStatus defaultStatus)
        {
            var relationship = new Relationship { RelationshipId = Null.NullInteger, Name = listName, Description = listDescription, PortalId = owningUser.PortalID, UserId = owningUser.UserID, DefaultResponse = defaultStatus, RelationshipTypeId = (int)DefaultRelationshipTypes.CustomList};

            SaveRelationship(relationship);

            return relationship;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddPortalRelationship - Add a new Relationship for a Portal, e.g. Co-workers
        /// </summary>        
        /// <param name="portalId">PortalId to which the Relationship will belong. Valid value includes Non-Negative number.</param>        
        /// <param name="listName">Name of the Relationship. It is also called as List Name, e.g. Co-Workers </param> 
        /// <param name="listDescription">Description of the Relationship. It is also called as List Description </param>         
        /// <param name="defaultStatus">DefaultRelationshiStatus of the Relationship when a new UserRelationship is initiated. 
        /// E.g. Accepted, which means automatically accept all UserRelationship request. Default is None, meaning no automatic action.</param> 
        /// <param name="relationshipTypeId">RelationshipTypeID. A new Relationship requires RelationshipType to be present in the system. </param> 
        /// <returns>Relationship object</returns>
        /// <remarks>Each Portal contains default Relationships of Friends and Followers. 
        /// AddPortalRelationship can be used to create addiotional Relationships
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public Relationship AddPortalRelationship(int portalId, string listName, string listDescription, RelationshipStatus defaultStatus, int relationshipTypeId)
        {
            var relationship = new Relationship { RelationshipId = Null.NullInteger, Name = listName, Description = listDescription, PortalId = portalId, UserId = Null.NullInteger, DefaultResponse = defaultStatus, RelationshipTypeId = relationshipTypeId};

            SaveRelationship(relationship);

            return relationship;            
        }

        #endregion

        #endregion

        #region Internal Methods

        internal void CreateDefaultRelationshipsForPortal(int portalId)
        {
            //create default Friend Relationship
            if (GetFriendsRelatioshipByPortal(portalId) == null)
            {
                var friendRelationship = new Relationship
                {
                    RelationshipId = Null.NullInteger,
                    Name = DefaultRelationshipTypes.Friends.ToString(),
                    Description = DefaultRelationshipTypes.Friends.ToString(),
                    PortalId = portalId,
                    UserId = Null.NullInteger,
                    DefaultResponse = RelationshipStatus.None, //default response is None
                    RelationshipTypeId = (int)DefaultRelationshipTypes.Friends
                };
                SaveRelationship(friendRelationship);
            }

            //create default Follower Relationship
            if (GetFollowersRelatioshipByPortal(portalId) == null)
            {

                var followerRelationship = new Relationship
                {
                    RelationshipId = Null.NullInteger,
                    Name = DefaultRelationshipTypes.Followers.ToString(),
                    Description = DefaultRelationshipTypes.Followers.ToString(),
                    PortalId = portalId,
                    UserId = Null.NullInteger,
                    DefaultResponse = RelationshipStatus.Accepted, //default response is Accepted
                    RelationshipTypeId = (int)DefaultRelationshipTypes.Followers
                };
                SaveRelationship(followerRelationship);
            }
        }

        #endregion

        #region Private Methods

        private void AddLog(string logContent)
        {
            _eventLogController.AddLog("Message", logContent, EventLogController.EventLogType.ADMIN_ALERT);
        }

        private void ClearRelationshipCache(Relationship relationship)
        {
            if (relationship.UserId == Null.NullInteger)
            {
                DataCache.RemoveCache(string.Format(DataCache.RelationshipByPortalIDCacheKey, relationship.PortalId));
            }
	    }

        private void ClearUserCache(UserRelationship userRelationship)
        {
            //Get Portal
            PortalSettings settings = PortalController.GetCurrentPortalSettings();

            if (settings != null)
            {
                //Get User
                UserInfo user = UserController.GetUserById(settings.PortalId, userRelationship.UserId);

                if (user != null)
                {
                    DataCache.ClearUserCache(settings.PortalId, user.Username);
                }
            }
        }

        private Relationship GetFriendsRelatioshipByPortal(int portalId)
        {
            return GetRelationshipsByPortalId(portalId).Where(re => re.RelationshipTypeId == (int) DefaultRelationshipTypes.Friends).FirstOrDefault();
        }

        private Relationship GetFollowersRelatioshipByPortal(int portalId)
        {
            return GetRelationshipsByPortalId(portalId).Where(re => re.RelationshipTypeId == (int)DefaultRelationshipTypes.Followers).FirstOrDefault();
        }

        private void ManageUserRelationshipStatus(int userRelationshipId, RelationshipStatus newStatus)
        {
            var userRelationship = VerifyUserRelationshipExist(userRelationshipId);
            if (userRelationship == null) return;

            //TODO - apply business rules - throw exception if newStatus requested is not allowed
            bool save = false;
            switch (newStatus)
            {
                case RelationshipStatus.None:
                    save = true;
                    break;
                case RelationshipStatus.Initiated:
                    save = true;
                    break;
                case RelationshipStatus.Accepted:
                    save = true;
                    break;
                case RelationshipStatus.Rejected:
                    save = true;
                    break;
                case RelationshipStatus.Ignored:
                    save = true;
                    break;
                case RelationshipStatus.Reported:
                    save = true;
                    break;
                case RelationshipStatus.Blocked:
                    save = true;
                    break;
            }

            if (save)
            {
                userRelationship.Status = newStatus;
                SaveUserRelationship(userRelationship);
            }
        }

        private UserRelationship VerifyUserRelationshipExist(int userRelationshipId)
        {
            var userRelationship = GetUserRelationship(userRelationshipId);
            if (userRelationship == null)
            {
                throw new UserRelationshipDoesNotExistException(Localization.GetExceptionMessage("UserRelationshipDoesNotExistError", "UserRelationshipID '{0}' does not exist.", userRelationshipId));
            }

            return userRelationship;
        }

        #endregion       
    }
}
