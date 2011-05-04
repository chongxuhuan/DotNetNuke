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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Services.FileSystem
{
    public class FolderMappingController : ComponentBase<IFolderMappingController, FolderMappingController>, IFolderMappingController
    {
        #region Constructor

        internal FolderMappingController()
        {
        }

        #endregion

        #region Private Variables

        private static readonly DataProvider dataProvider = DataProvider.Instance();
        private static readonly string cacheKeyPrefix = "GetFolderMappingSettings";

        #endregion

        #region Public Methods

        public FolderMappingInfo GetDefaultFolderMapping(int portalID)
        {
            foreach (var folderMapping in GetFolderMappings(portalID))
            {
                if (folderMapping.FolderProviderType == Config.GetDefaultProvider("folder").Name)
                {
                    return folderMapping;
                }
            }

            return null;
        }

        public int AddFolderMapping(FolderMappingInfo objFolderMapping)
        {
            objFolderMapping.FolderMappingID = dataProvider.AddFolderMapping(objFolderMapping.PortalID,
                                                                             objFolderMapping.MappingName,
                                                                             objFolderMapping.FolderProviderType,
                                                                             objFolderMapping.IsEnabled,
                                                                             UserController.GetCurrentUserInfo().UserID);

            UpdateFolderMappingSettings(objFolderMapping);

            return objFolderMapping.FolderMappingID;
        }

        public void DeleteFolderMapping(int folderMappingID)
        {
            dataProvider.DeleteFolderMapping(folderMappingID);
            DataCache.RemoveCache(cacheKeyPrefix + folderMappingID);
        }

        public void UpdateFolderMapping(FolderMappingInfo objFolderMapping)
        {
            dataProvider.UpdateFolderMapping(objFolderMapping.FolderMappingID,
                                             objFolderMapping.MappingName,
                                             objFolderMapping.IsEnabled,
                                             objFolderMapping.Priority,
                                             UserController.GetCurrentUserInfo().UserID);

            UpdateFolderMappingSettings(objFolderMapping);
        }

        public FolderMappingInfo GetFolderMapping(int folderMappingID)
        {
            return CBO.FillObject<FolderMappingInfo>(dataProvider.GetFolderMapping(folderMappingID));
        }

        public FolderMappingInfo GetFolderMapping(int portalID, string mappingName)
        {
            return CBO.FillObject<FolderMappingInfo>(dataProvider.GetFolderMappingByMappingName(portalID, mappingName));
        }

        public List<FolderMappingInfo> GetFolderMappings(int portalID)
        {
            return CBO.FillCollection<FolderMappingInfo>(dataProvider.GetFolderMappings(portalID));
        }

        public void AddDefaultFolderTypes(int portalID)
        {
            dataProvider.AddDefaultFolderTypes(portalID);
        }

        public Hashtable GetFolderMappingSettings(int folderMappingID)
        {
            Hashtable objSettings;
            string strCacheKey = cacheKeyPrefix + folderMappingID;
            objSettings = (Hashtable)DataCache.GetCache(strCacheKey);
            if (objSettings == null)
            {
                objSettings = new Hashtable();
                IDataReader dr = null;
                try
                {
                    dr = dataProvider.GetFolderMappingSettings(folderMappingID);
                    while (dr.Read())
                    {
                        if (!dr.IsDBNull(1))
                        {
                            objSettings[dr.GetString(0)] = dr.GetString(1);
                        }
                        else
                        {
                            objSettings[dr.GetString(0)] = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Exceptions.Exceptions.LogException(ex);
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
                int intCacheTimeout = 20 * Convert.ToInt32(Host.PerformanceSetting);
                DataCache.SetCache(strCacheKey, objSettings, TimeSpan.FromMinutes(intCacheTimeout));
            }
            return objSettings;
        }

        #endregion

        #region Private Methods

        private void UpdateFolderMappingSettings(FolderMappingInfo objFolderMapping)
        {
            foreach (string sKey in objFolderMapping.FolderMappingSettings.Keys)
            {
                UpdateFolderMappingSetting(objFolderMapping.FolderMappingID, sKey, Convert.ToString(objFolderMapping.FolderMappingSettings[sKey]));
            }
        }

        private void UpdateFolderMappingSetting(int folderMappingID, string settingName, string settingValue)
        {
            IDataReader dr = null;
            try
            {
                dr = dataProvider.GetFolderMappingSetting(folderMappingID, settingName);
                if (dr.Read())
                {
                    dataProvider.UpdateFolderMappingSetting(folderMappingID, settingName, settingValue, UserController.GetCurrentUserInfo().UserID);
                }
                else
                {
                    dataProvider.AddFolderMappingSetting(folderMappingID, settingName, settingValue, UserController.GetCurrentUserInfo().UserID);
                }
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
            }
            finally
            {
                // Ensure DataReader is closed
                CBO.CloseDataReader(dr, true);
            }

            DataCache.RemoveCache(cacheKeyPrefix + folderMappingID);
        }

        #endregion
    }
}