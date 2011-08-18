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
using System.Data;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals.Data;
using DotNetNuke.Entities.Users;

#endregion

namespace DotNetNuke.Entities.Portals
{
    public class PortalGroupController : ComponentBase<IPortalGroupController, PortalGroupController>, IPortalGroupController
    {
        private readonly IDataService _dataService;

        #region Constructors

        public PortalGroupController() : this(DataService.Instance)
        {
        }

        public PortalGroupController(IDataService dataService)
        {
            _dataService = dataService;
        }

        #endregion

        private object GetPortalGroupsCallback(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillCollection<PortalGroupInfo>(_dataService.GetPortalGroups());
        }

        #region IPortalGroupController Members

        public int AddPortalGroup(PortalGroupInfo portalGroup)
        {
            //Argument Contract
            Requires.NotNull("portalGroup", portalGroup);

            portalGroup.PortalGroupId = _dataService.AddPortalGroup(portalGroup, UserController.GetCurrentUserInfo().UserID);

            return portalGroup.PortalGroupId;
        }

        public void DeletePortalGroup(PortalGroupInfo portalGroup)
        {
            //Argument Contract
            Requires.NotNull("portalGroup", portalGroup);
            Requires.PropertyNotNegative("portalGroup", "PortalGroupId", portalGroup.PortalGroupId);

            _dataService.DeletePortalGroup(portalGroup);
        }

        public IEnumerable<PortalGroupInfo> GetPortalGroups()
        {
            string cacheKey = string.Format(DataCache.PortalGroupsCacheKey);
            return CBO.GetCachedObject<IEnumerable<PortalGroupInfo>>(new CacheItemArgs(cacheKey, DataCache.PortalGroupsCacheTimeOut, DataCache.PortalGroupsCachePriority), GetPortalGroupsCallback);
        }

        public void UpdatePortalGroup(PortalGroupInfo portalGroup)
        {
            //Argument Contract
            Requires.NotNull("portalGroup", portalGroup);
            Requires.PropertyNotNegative("portalGroup", "PortalGroupId", portalGroup.PortalGroupId);

            _dataService.UpdatePortalGroup(portalGroup, UserController.GetCurrentUserInfo().UserID);
        }

        #endregion

        //private static Object GetNull(Object Field)
        //{
        //    return Null.GetNull(Field, DBNull.Value);
        //}

        //private static PortalGroupInfo FillPortalGroupInfo(IDataReader dr, bool checkForOpenDataReader)
        //{
        //    PortalGroupInfo obj = null;
        //    bool canContinue = true;
        //    if (checkForOpenDataReader)
        //    {
        //        canContinue = false;
        //        if (dr.Read())
        //        {
        //            canContinue = true;
        //        }
        //    }

        //    if (canContinue)
        //    {
        //        obj = new PortalGroupInfo();
        //        obj.PortalGroupId = Convert.ToInt32(Null.SetNull(dr["PortalGroupID"], obj.PortalGroupId));
        //        obj.PortalGroupName = Convert.ToString(Null.SetNull(dr["PortalGroupName"], obj.PortalGroupName));
        //        obj.PortalGroupDescription = Convert.ToString(Null.SetNull(dr["PortalGroupDescription"], obj.PortalGroupDescription));
        //    }

        //    return obj;
        //}

        //public static PortalGroupInfo STD_GetPortalGroup(Int32 PortalGroupID)
        //{
        //    PortalGroupInfo obj = null;
        //    using (IDataReader dr = DataProvider.Instance().ExecuteReader("STD_GetPortalGroup", PortalGroupID))
        //    {
        //        obj = FillPortalGroupInfo(dr, true);
        //    }

        //    return obj;
        //}

        //public static List<PortalGroupInfo> PortalGroup_DefaultList()
        //{
        //    var list = new List<PortalGroupInfo>();
        //    using (IDataReader dr = DataProvider.Instance().ExecuteReader("PortalGroup_DefaultList"))
        //    {
        //        while (dr.Read())
        //        {
        //            list.Add(FillPortalGroupInfo(dr, false));
        //        }
        //    }

        //    return list;
        //}

        //public static List<PortalGroupInfo> PortalGroup_DefaultList_Where(String Where, String OrderBy)
        //{
        //    var list = new List<PortalGroupInfo>();
        //    using (IDataReader dr = DataProvider.Instance().ExecuteReader("PortalGroup_DefaultList_Where", Where, OrderBy))
        //    {
        //        while (dr.Read())
        //        {
        //            list.Add(FillPortalGroupInfo(dr, false));
        //        }
        //    }

        //    return list;
        //}

        //public static Int32 InsertPortalGroup(String PortalGroupName, String PortalGroupDescription, Int32 CreatedByUserID, DateTime CreatedOnDate, Int32 LastModifiedByUserID,
        //                                      DateTime LastModifiedOnDate)
        //{
        //    return
        //        Convert.ToInt32(DataProvider.Instance().ExecuteScalar("InsertPortalGroup",
        //                                                              PortalGroupName,
        //                                                              PortalGroupDescription,
        //                                                              CreatedByUserID,
        //                                                              CreatedOnDate,
        //                                                              LastModifiedByUserID,
        //                                                              LastModifiedOnDate));
        //}

        //public static void UpdatePortalGroup(String PortalGroupName, String PortalGroupDescription, Int32 CreatedByUserID, DateTime CreatedOnDate, Int32 LastModifiedByUserID,
        //                                     DateTime LastModifiedOnDate, Int32 PortalGroupID)
        //{
        //    DataProvider.Instance().ExecuteNonQuery("UpdatePortalGroup",
        //                                            PortalGroupName,
        //                                            PortalGroupDescription,
        //                                            CreatedByUserID,
        //                                            CreatedOnDate,
        //                                            LastModifiedByUserID,
        //                                            LastModifiedOnDate,
        //                                            PortalGroupID);
        //}

        //public static void DeletePortalGroup(Int32 PortalGroupID)
        //{
        //    DataProvider.Instance().ExecuteNonQuery("DeletePortalGroup", PortalGroupID);
        //}

        //public static PortalGroupInfo Detail_SP_PortalGroup_DefaultView()
        //{
        //    PortalGroupInfo obj = null;
        //    using (IDataReader dr = DataProvider.Instance().ExecuteReader("Detail_SP_PortalGroup_DefaultView"))
        //    {
        //        obj = FillPortalGroupInfo(dr, true);
        //    }

        //    return obj;
        //}

        //public static PortalGroupInfo Detail_SP_PortalGroupDetail(Int32 PortalGroupID)
        //{
        //    PortalGroupInfo obj = null;
        //    using (IDataReader dr = DataProvider.Instance().ExecuteReader("Detail_SP_PortalGroupDetail", PortalGroupID))
        //    {
        //        obj = FillPortalGroupInfo(dr, true);
        //    }

        //    return obj;
        //}
    }
}