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
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Entities.Users
{
	/// <summary>
	/// Business Layer to manage Relationships. Also contains CRUD methods.
	/// </summary>
    public class RelationshipController : IRelationshipController
	{
		#region "Public Methods"

        #region RelationshipType CRUD

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Save RelationshipType
        /// </summary>
        /// <param name="relationshipType">RelationshipType object</param>        
        /// <remarks>
        /// If RelationshipTypeID is -1 (Null.NullIntger), then a new RelationshipType is created, 
        /// else existing RelationshipType is updated
        /// </remarks>
        /// -----------------------------------------------------------------------------
        public void SaveRelationshipType(RelationshipType relationshipType)
		{
            Requires.NotNull("The relationshipType can't be null", relationshipType);

            int relationshipTypeID = DataProvider.Instance().SaveRelationshipType(relationshipType.RelationshipTypeID,
                                                        (int)relationshipType.Direction,
                                                        relationshipType.Name,
                                                        relationshipType.Description,														
														UserController.GetCurrentUserInfo().UserID);

            //log event
            var logContent = string.Format("'{0}' RelationshipType '{1}'", relationshipType.RelationshipTypeID == Null.NullInteger ? "Added" : "Updated", relationshipType.Name);
			AddLog(logContent);

            relationshipType.RelationshipTypeID = relationshipTypeID;

            //clear cache
            DataCache.RemoveCache(DataCache.RelationshipTypesCacheKey);
		}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete RelationshipType By RelationshipTypeID
        /// </summary>
        /// <param name="relationshipTypeID">RelationshipTypeID</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteRelationshipType(int relationshipTypeID)
        {
            var relationshipType = GetRelationshipType(relationshipTypeID);

            if (relationshipType != null)
            {
                DataProvider.Instance().DeleteRelationshipType(relationshipTypeID);

                //log event
                var logContent = string.Format("'{0}', ID {1} RelationshipType 'Deleted'", relationshipType.Name, relationshipType.RelationshipTypeID);
                AddLog(logContent);

                //clear cache
                DataCache.RemoveCache(DataCache.RelationshipTypesCacheKey);
            }
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get list of All RelationshipTypes defined in system
        /// </summary>        
        /// -----------------------------------------------------------------------------
        public IList<RelationshipType> GetAllRelationshipTypes()
        {
            var cacheArg = new CacheItemArgs(DataCache.RelationshipTypesCacheKey, DataCache.RelationshipTypesCacheTimeOut, DataCache.RelationshipTypesCachePriority, "");
            return CBO.GetCachedObject<IList<RelationshipType>>(cacheArg, GetAllRelationshipTypesCallBack);
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get RelationshipType By RelationshipTypeID
        /// </summary>        
        /// <param name="relationshipTypeID">RelationshipTypeID</param>        
        /// -----------------------------------------------------------------------------
        public RelationshipType GetRelationshipType(int relationshipTypeID)
        {
            return GetAllRelationshipTypes().Where(r => r.RelationshipTypeID == relationshipTypeID).FirstOrDefault();
        }

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
        public void SaveRelationship(Relationship relationship)
        {
            Requires.NotNull("The relationship can't be null", relationship);

            int relationshipID = DataProvider.Instance().SaveRelationship(relationship.RelationshipID,
                                                        relationship.RelationshipTypeID,
                                                        relationship.Name,
                                                        relationship.Description,
                                                        relationship.UserID,
                                                        relationship.PortalID,
                                                        UserController.GetCurrentUserInfo().UserID);

            //log event
            var logContent = string.Format("'{0}' Relationship '{1}'", relationship.RelationshipID == Null.NullInteger ? "Added" : "Updated", relationship.Name);
            AddLog(logContent);

            relationship.RelationshipID = relationshipID;

            //clear cache
            ClearRelationshipCache(relationship);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By RelationshipID
        /// </summary>
        /// <param name="relationshipID">RelationshipID</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteRelationship(int relationshipID)
        {
            var relationship = GetRelationship(relationshipID);

            if (relationship != null)
            {
                DataProvider.Instance().DeleteRelationship(relationshipID);

                //log event
                var logContent = string.Format("'{0}', ID {1} Relationship 'Deleted'", relationship.Name, relationship.RelationshipID);
                AddLog(logContent);

                //clear cache
                ClearRelationshipCache(relationship);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By UserID
        /// </summary>
        /// <param name="userID">UserID</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteRelationshipByUserID(int userID)
        {
            DataProvider.Instance().DeleteRelationshipByUserID(userID);

            //log event
            var logContent = string.Format("All Relationships for UserID '{0}' 'Deleted'", userID);
            AddLog(logContent);
            
            //cache clear
            DataCache.RemoveCache(string.Format(DataCache.RelationshipByUserIDCacheKey, userID));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By PortalID
        /// </summary>
        /// <param name="portalID">PortalID</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteRelationshipByPortalID(int portalID)
        {
            DataProvider.Instance().DeleteRelationshipByPortalID(portalID);

            //log event
            var logContent = string.Format("All Relationships for PortalID '{0}' 'Deleted'", portalID);
            AddLog(logContent);

            //cache clear            
            DataCache.RemoveCache(string.Format(DataCache.RelationshipByPortalIDCacheKey, portalID));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get Relationship By RelationshipID
        /// </summary>        
        /// <param name="relationshipID">RelationshipID</param>        
        /// -----------------------------------------------------------------------------
        public Relationship GetRelationship(int relationshipID)
        {
            return CBO.FillCollection<Relationship>(DataProvider.Instance().GetRelationship(relationshipID)).FirstOrDefault();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get Relationships By UserID
        /// </summary>        
        /// <param name="userID">UserID</param>        
        /// -----------------------------------------------------------------------------
        public IList<Relationship> GetRelationshipsByUserID(int userID)
        {
            var cacheArg = new CacheItemArgs(string.Format(DataCache.RelationshipByUserIDCacheKey, userID), DataCache.RelationshipByUserIDCacheTimeOut, DataCache.RelationshipByUserIDCachePriority, "");
            return CBO.GetCachedObject<IList<Relationship>>(cacheArg, GetRelationshipsByUserIDCallBack);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get Relationships By PortalID
        /// </summary>        
        /// <param name="portalID">PortalID</param>        
        /// -----------------------------------------------------------------------------
        public IList<Relationship> GetRelationshipsByPortalID(int portalID)
        {
            var cacheArg = new CacheItemArgs(string.Format(DataCache.RelationshipByPortalIDCacheKey, portalID), DataCache.RelationshipByPortalIDCacheTimeOut, DataCache.RelationshipByPortalIDCachePriority, "");
            return CBO.GetCachedObject<IList<Relationship>>(cacheArg, GetRelationshipsByPortalIDCallBack);            
        }

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
        public void SaveUserRelationship(UserRelationship userRelationship)
        {
            Requires.NotNull("The user relationship can't be null", userRelationship);

            int userRelationshipID = DataProvider.Instance().SaveUserRelationship(userRelationship.UserRelationshipID,
                                                        userRelationship.UserID,
                                                        userRelationship.RelatedUserID,
                                                        userRelationship.RelationshipID,
                                                        (int)userRelationship.Status,                                                        
                                                        UserController.GetCurrentUserInfo().UserID);

            //log event            
            var logContent = string.Format("UserRelationshipID for ID '{0}', UserID '{1}', RelatedUserID '{2}' '{3}'", userRelationship.UserRelationshipID, userRelationship.UserID, userRelationship.RelatedUserID, userRelationship.RelationshipID == Null.NullInteger ? "Added" : "Updated");
            AddLog(logContent);

            userRelationship.UserRelationshipID = userRelationshipID;

            //clear cache
            DataCache.RemoveCache(string.Format(DataCache.UserRelationshipByUserIDCacheKey, userRelationship.UserID));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By UserRelationshipID
        /// </summary>
        /// <param name="userRelationshipID">UserRelationshipID</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteUserRelationship(int userRelationshipID)
        {
            var userRelationship = GetUserRelationship(userRelationshipID);
            DeleteUserRelationship(userRelationship);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete USerRelationship By UserID
        /// </summary>
        /// <param name="userID">UserID</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteUserRelationshipByUserID(int userID)
        {
            var userRelationships = GetUserRelationshipsByUserID(userID);

            foreach (var userRelationship in userRelationships)
            {
                DeleteUserRelationship(userRelationship);
            } 
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By RelatedUserlID
        /// </summary>
        /// <param name="relatedUserID">RelatedUserID</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteRelationshipByRelatedID(int relatedUserID)
        {
            var userRelationships = GetUserRelationshipsByRelatedUserID(relatedUserID);

            foreach (var userRelationship in userRelationships)
            {
                DeleteUserRelationship(userRelationship);
            }                        
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete Relationship By RelationshipID
        /// </summary>
        /// <param name="relationshipID">RelationshipID</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteUserRelationshipByRelationshipID(int relationshipID)
        {
            var userRelationships = GetUserRelationshipsByRelationshipID(relationshipID);

            foreach(var userRelationship in userRelationships)
            {
                DeleteUserRelationship(userRelationship);                
            }            
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationship By UserRelationshipID
        /// </summary>        
        /// <param name="userRelationshipID">UserRelationshipID</param>        
        /// -----------------------------------------------------------------------------
        public UserRelationship GetUserRelationship(int userRelationshipID)
        {
            return CBO.FillCollection<UserRelationship>(DataProvider.Instance().GetUserRelationship(userRelationshipID)).FirstOrDefault();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationship By UserID
        /// </summary>        
        /// <param name="userID">UserID</param>        
        /// -----------------------------------------------------------------------------
        public IList<UserRelationship> GetUserRelationshipsByUserID(int userID)
        {
            var cacheArg = new CacheItemArgs(string.Format(DataCache.UserRelationshipByUserIDCacheKey, userID), DataCache.UserRelationshipByUserIDCacheTimeOut, DataCache.UserRelationshipByUserIDCachePriority, "");
            return CBO.GetCachedObject<IList<UserRelationship>>(cacheArg, GetUserRelationshipsByUserIDCallBack);            
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationship By RelatedUserID
        /// </summary>        
        /// <param name="relatedUserID">RelatedUserID</param>        
        /// -----------------------------------------------------------------------------
        public IList<UserRelationship> GetUserRelationshipsByRelatedUserID(int relatedUserID)
        {
            return CBO.FillCollection<UserRelationship>(DataProvider.Instance().GetUserRelationshipsByRelatedUserID(relatedUserID)).ToList();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get UserRelationship By RelationshipID
        /// </summary>        
        /// <param name="relationshipID">RelationshipID</param>        
        /// -----------------------------------------------------------------------------
        public IList<UserRelationship> GetUserRelationshipsByRelationshipID(int relationshipID)
	    {
            return CBO.FillCollection<UserRelationship>(DataProvider.Instance().GetUserRelationshipsByRelationshipID(relationshipID)).ToList();
	    }

        #endregion

        #endregion

        #region "Private Methods"

        private IList<RelationshipType> GetAllRelationshipTypesCallBack(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillCollection<RelationshipType>(DataProvider.Instance().GetAllRelationshipTypes()).ToList();
        }

        private IList<Relationship> GetRelationshipsByUserIDCallBack(CacheItemArgs cacheItemArgs)
        {
            int userID = (int)cacheItemArgs.ParamList[0];
            return CBO.FillCollection<Relationship>(DataProvider.Instance().GetRelationshipsByUserID(userID)).ToList();
        }

        private IList<Relationship> GetUserRelationshipsByUserIDCallBack(CacheItemArgs cacheItemArgs)
        {
            int userID = (int)cacheItemArgs.ParamList[0];
            return CBO.FillCollection<Relationship>(DataProvider.Instance().GetUserRelationshipsByUserID(userID)).ToList();
        }

        private IList<Relationship> GetRelationshipsByPortalIDCallBack(CacheItemArgs cacheItemArgs)
        {
            int portalID = (int)cacheItemArgs.ParamList[0];
            return CBO.FillCollection<Relationship>(DataProvider.Instance().GetRelationshipsByPortalID(portalID)).ToList();
        }

		private void AddLog(string logContent)
		{
			var objEventLog = new EventLogController();
			objEventLog.AddLog("Message", logContent, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.ADMIN_ALERT);
		}

        private void DeleteUserRelationship(UserRelationship userRelationship)
        {
            DataProvider.Instance().DeleteUserRelationship(userRelationship.UserRelationshipID);

            //log event
            var logContent = string.Format("UserRelationshipID for ID '{0}', UserID '{1}', RelatedUserID '{2}' 'Deleted'", userRelationship.UserRelationshipID, userRelationship.UserID, userRelationship.RelatedUserID);
            AddLog(logContent);

            //cache clear
            DataCache.RemoveCache(string.Format(DataCache.UserRelationshipByUserIDCacheKey, userRelationship.UserID));
        }


        private void ClearRelationshipCache(Relationship relationship)
        {
            DataCache.RemoveCache(relationship.UserID != Null.NullInteger
                                      ? string.Format(DataCache.RelationshipByUserIDCacheKey, relationship.UserID)
                                      : string.Format(DataCache.RelationshipByPortalIDCacheKey, relationship.PortalID));
            
        }

		#endregion
	}
}
