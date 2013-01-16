#region Copyright
// 
// DotNetNukeŽ - http://www.dotnetnuke.com
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;

#endregion

namespace DotNetNuke.Entities.Modules.Definitions
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Namespace: DotNetNuke.Entities.Modules.Definitions
    /// Class	 : ModuleDefinitionController
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// ModuleDefinitionController provides the Business Layer for Module Definitions
    /// </summary>
    /// <history>
    /// 	[cnurse]	01/11/2008   Documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class ModuleDefinitionController
    {
        private const string key = "ModuleDefID";
        private static readonly DataProvider dataProvider = DataProvider.Instance();

        #region Private Members

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModuleDefinitionsCallBack gets a Dictionary of Module Definitions from
        /// the Database.
        /// </summary>
        /// <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        /// needed for the database call</param>
        /// <history>
        /// 	[cnurse]	01/13/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static object GetModuleDefinitionsCallBack(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillDictionary(key, dataProvider.GetModuleDefinitions(), new Dictionary<int, ModuleDefinitionInfo>());
        }

        #endregion

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModuleDefinitionByID gets a Module Definition by its ID
        /// </summary>
		/// <param name="objModuleDefinition">The object of the Module Definition</param>
        /// <history>
        /// 	[cnurse]	01/14/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void DeleteModuleDefinition(ModuleDefinitionInfo objModuleDefinition)
        {
            DeleteModuleDefinition(objModuleDefinition.ModuleDefID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// DeleteModuleDefinition deletes a Module Definition By ID
        /// </summary>
        /// <param name="moduleDefinitionId">The ID of the Module Definition to delete</param>
        /// <history>
        /// 	[cnurse]	01/11/2008   Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public void DeleteModuleDefinition(int moduleDefinitionId)
        {
			//Delete associated permissions
            var permissionController = new PermissionController();
            foreach (PermissionInfo permission in permissionController.GetPermissionsByModuleDefID(moduleDefinitionId))
            {
                permissionController.DeletePermission(permission.PermissionID);
            }
            dataProvider.DeleteModuleDefinition(moduleDefinitionId);
            DataCache.ClearHostCache(true);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModuleDefinitionByID gets a Module Definition by its ID
        /// </summary>
        /// <param name="moduleDefID">The ID of the Module Definition</param>
        /// <history>
        /// 	[cnurse]	01/14/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static ModuleDefinitionInfo GetModuleDefinitionByID(int moduleDefID)
        {
            return (from kvp in GetModuleDefinitions()
                    where kvp.Value.ModuleDefID == moduleDefID
                    select kvp.Value)
                   .FirstOrDefault();
        }

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// GetModuleDefinitionByFriendlyName gets a Module Definition by its Friendly
		/// Name (and DesktopModuleID)
		/// </summary>
		/// <param name="friendlyName">The friendly name</param>
		/// <history>
		/// 	[cnurse]	01/14/2008   Created
		/// </history>
		/// -----------------------------------------------------------------------------
		public static ModuleDefinitionInfo GetModuleDefinitionByFriendlyName(string friendlyName)
        {
            Requires.NotNullOrEmpty("friendlyName", friendlyName);

            return (from kvp in GetModuleDefinitions()
                    where kvp.Value.FriendlyName == friendlyName
                    select kvp.Value)
                   .FirstOrDefault();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModuleDefinitionByFriendlyName gets a Module Definition by its Friendly
        /// Name (and DesktopModuleID)
        /// </summary>
        /// <param name="friendlyName">The friendly name</param>
        /// <param name="desktopModuleID">The ID of the Dekstop Module</param>
        /// <history>
        /// 	[cnurse]	01/14/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static ModuleDefinitionInfo GetModuleDefinitionByFriendlyName(string friendlyName, int desktopModuleID)
        {
            Requires.NotNullOrEmpty("friendlyName", friendlyName);
            Requires.NotNegative("desktopModuleID", desktopModuleID);

            return (from kvp in GetModuleDefinitions()
                    where kvp.Value.FriendlyName == friendlyName && kvp.Value.DesktopModuleID == desktopModuleID
                    select kvp.Value)
                   .FirstOrDefault();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModuleDefinitions gets a Dictionary of Module Definitions.
        /// </summary>
        /// <history>
        /// 	[cnurse]	01/14/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static Dictionary<int, ModuleDefinitionInfo> GetModuleDefinitions()
        {
            return CBO.GetCachedObject<Dictionary<int, ModuleDefinitionInfo>>(new CacheItemArgs(DataCache.ModuleDefinitionCacheKey, 
                                                                                        DataCache.ModuleDefinitionCachePriority),
                                                                              GetModuleDefinitionsCallBack);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModuleDefinitionsByDesktopModuleID gets a Dictionary of Module Definitions
        /// with a particular DesktopModuleID, keyed by the FriendlyName.
        /// </summary>
        /// <param name="desktopModuleID">The ID of the Desktop Module</param>
        /// <history>
        /// 	[cnurse]	01/14/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static Dictionary<string, ModuleDefinitionInfo> GetModuleDefinitionsByDesktopModuleID(int desktopModuleID)
        {
            //Iterate through cached Dictionary to get all Module Definitions with the correct DesktopModuleID
            return GetModuleDefinitions().Where(kvp => kvp.Value.DesktopModuleID == desktopModuleID)
                    .ToDictionary(kvp => kvp.Value.FriendlyName, kvp => kvp.Value);
        }

        /// <summary>
        /// Get ModuleDefinition by DefinitionName
        /// </summary>
        /// <param name="definitionName">The defintion name</param>
        /// <param name="desktopModuleID">The ID of the Dekstop Module</param>
        /// <returns>A ModuleDefinition or null if not found</returns>
        public static ModuleDefinitionInfo GetModuleDefinitionByDefinitionName(string definitionName, int desktopModuleID)
        {
            Requires.NotNullOrEmpty("definitionName", definitionName);
            Requires.NotNegative("desktopModuleID", desktopModuleID);

            return (from kvp in GetModuleDefinitions()
                    where kvp.Value.DefinitionName == definitionName && kvp.Value.DesktopModuleID == desktopModuleID
                    select kvp.Value)
                   .FirstOrDefault();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// SaveModuleDefinition saves the Module Definition to the database
        /// </summary>
        /// <param name="moduleDefinition">The Module Definition to save</param>
        /// <param name="saveChildren">A flag that determines whether the child objects are also saved</param>
        /// <param name="clearCache">A flag that determines whether to clear the host cache</param>
        /// <history>
        /// 	[cnurse]	01/14/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int SaveModuleDefinition(ModuleDefinitionInfo moduleDefinition, bool saveChildren, bool clearCache)
        {
            int moduleDefinitionID = moduleDefinition.ModuleDefID;
            if (moduleDefinitionID == Null.NullInteger)
            {
				//Add new Module Definition
                moduleDefinitionID = dataProvider.AddModuleDefinition(moduleDefinition.DesktopModuleID,
                                                                      moduleDefinition.FriendlyName,
                                                                      moduleDefinition.DefinitionName,
                                                                      moduleDefinition.DefaultCacheTime,
                                                                      UserController.GetCurrentUserInfo().UserID);
            }
            else
            {
				//Upgrade Module Definition
                dataProvider.UpdateModuleDefinition(moduleDefinition.ModuleDefID, moduleDefinition.FriendlyName, moduleDefinition.DefinitionName, moduleDefinition.DefaultCacheTime, UserController.GetCurrentUserInfo().UserID);
            }
            if (saveChildren)
            {
                foreach (KeyValuePair<string, PermissionInfo> kvp in moduleDefinition.Permissions)
                {
                    kvp.Value.ModuleDefID = moduleDefinitionID;

                    //check if permission exists
                    var permissionController = new PermissionController();
                    ArrayList permissions = permissionController.GetPermissionByCodeAndKey(kvp.Value.PermissionCode, kvp.Value.PermissionKey);
                    if (permissions != null && permissions.Count == 1)
                    {
                        var permission = (PermissionInfo) permissions[0];
                        kvp.Value.PermissionID = permission.PermissionID;
                        permissionController.UpdatePermission(kvp.Value);
                    }
                    else
                    {
                        permissionController.AddPermission(kvp.Value);
                    }
                }
                foreach (KeyValuePair<string, ModuleControlInfo> kvp in moduleDefinition.ModuleControls)
                {
                    kvp.Value.ModuleDefID = moduleDefinitionID;

                    //check if definition exists
                    ModuleControlInfo moduleControl = ModuleControlController.GetModuleControlByControlKey(kvp.Value.ControlKey, kvp.Value.ModuleDefID);
                    if (moduleControl != null)
                    {
                        kvp.Value.ModuleControlID = moduleControl.ModuleControlID;
                    }
                    ModuleControlController.SaveModuleControl(kvp.Value, clearCache);
                }
            }
            if (clearCache)
            {
                DataCache.ClearHostCache(true);
            }
            return moduleDefinitionID;
        }

        #region Obsolete Members

        [Obsolete("Deprecated in DotNetNuke 6.0.  Replaced by SaveModuleDefinition")]
        public void UpdateModuleDefinition(ModuleDefinitionInfo objModuleDefinition)
        {
            SaveModuleDefinition(objModuleDefinition, false, true);
        }

        [Obsolete("Deprecated in DotNetNuke 6.0.  Replaced by SaveModuleDefinition")]
        public int AddModuleDefinition(ModuleDefinitionInfo objModuleDefinition)
        {
            return SaveModuleDefinition(objModuleDefinition, false, true);
        }

        [Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetModuleDefinitionByID(Integer)")]
        public ModuleDefinitionInfo GetModuleDefinition(int moduleDefID)
        {
            return GetModuleDefinitionByID(moduleDefID);
        }

        [Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetModuleDefinitionByFriendlyName(String,Integer)")]
        public ModuleDefinitionInfo GetModuleDefinitionByName(int desktopModuleID, string friendlyName)
        {
            return GetModuleDefinitionByFriendlyName(friendlyName, desktopModuleID);
        }

        [Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetModuleDefinitionsByDesktopModuleID(Integer)")]
        public ArrayList GetModuleDefinitions(int DesktopModuleId)
        {
            var arrDefinitions = new ArrayList();
            arrDefinitions.AddRange(GetModuleDefinitionsByDesktopModuleID(DesktopModuleId).Values);
            return arrDefinitions;
        }

        #endregion
    }
}
