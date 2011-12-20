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
        public void SaveRelationshipType(RelationshipType relationshipType)
		{
            Requires.NotNull("The relationshipType can't be null", relationshipType);

            int relationshipTypeId = DataProvider.Instance().SaveRelationshipType(relationshipType.RelationshipTypeID,
                                                        (int)relationshipType.Direction,
                                                        relationshipType.Name,
                                                        relationshipType.Description,														
														UserController.GetCurrentUserInfo().UserID);

            var logContent = string.Format("'{0}' RelationshipType '{1}'", relationshipType.RelationshipTypeID == Null.NullInteger ? "Added" : "Updated", relationshipType.Name);
			AddLog(logContent);

            relationshipType.RelationshipTypeID = relationshipTypeId;

            DataCache.RemoveCache(DataCache.RelationshipTypesCacheKey);
		}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete RelationshipType
        /// </summary>
        /// <param name="relationshipTypeID">RelationshipTypeID</param>        
        /// -----------------------------------------------------------------------------
        public void DeleteRelationshipType(int relationshipTypeID)
        {
            var relationshipType = GetRelationshipTypeByID(relationshipTypeID);

            if (relationshipType != null)
            {
                DataProvider.Instance().DeleteRelationshipType(relationshipTypeID);

                var logContent = string.Format("'{0}', ID {1} RelationshipType 'Deleted'", relationshipType.Name, relationshipType.RelationshipTypeID);
                AddLog(logContent);

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
        public RelationshipType GetRelationshipTypeByID(int relationshipTypeID)
        {
            return GetAllRelationshipTypes().Where(r => r.RelationshipTypeID == relationshipTypeID).FirstOrDefault();
        }


		#endregion

		#region "Private Methods"

        private IList<RelationshipType> GetAllRelationshipTypesCallBack(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillCollection<RelationshipType>(DataProvider.Instance().GetAllRelationshipTypes()).ToList();
        }

		



		private void AddLog(string logContent)
		{
			var objEventLog = new EventLogController();
			objEventLog.AddLog("Message", logContent, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.ADMIN_ALERT);
		}


		#endregion
	}
}
