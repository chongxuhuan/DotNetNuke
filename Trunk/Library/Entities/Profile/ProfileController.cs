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
using System.Collections;
using System.Collections.Generic;
using System.Data;

using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Profile;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Entities.Profile
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Profile
    /// Class:      ProfileController
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ProfileController class provides Business Layer methods for profiles and
    /// for profile property Definitions
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	01/31/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class ProfileController
    {
        private static readonly DataProvider provider = DataProvider.Instance();
        private static readonly ProfileProvider profileProvider = ProfileProvider.Instance();
        private static int _orderCounter;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a single default property definition
        /// </summary>
        /// <param name="portalId">Id of the Portal</param>
        /// <param name="category">Category of the Property</param>
        /// <param name="name">Name of the Property</param>
		/// <param name="strType">DataType</param>
		/// <param name="length">length</param>
		/// <param name="defaultVisibility">Default visibility</param>
		/// <param name="types">List of types</param>		
        /// <history>
        ///     [cnurse]	02/22/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void AddDefaultDefinition(int portalId, string category, string name, string strType, int length, UserVisibilityMode defaultVisibility, ListEntryInfoCollection types)
        {
            _orderCounter += 2;
            AddDefaultDefinition(portalId, category, name, strType, length, _orderCounter, defaultVisibility, types);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Adds a single default property definition
        /// </summary>
        /// <param name = "portalId">Id of the Portal</param>
        /// <param name = "category">Category of the Property</param>
        /// <param name = "name">Name of the Property</param>
        /// <param name="type">DataType</param>
        /// <param name="length">length</param>
        /// <param name="viewOrder">for sort order</param>
        /// <param name="defaultVisibility">Default visibility</param>
        /// <param name="types">List of types</param>
        /// <history>
        ///   [cnurse]	02/22/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        internal static void AddDefaultDefinition(int portalId, string category, string name, string type, int length, int viewOrder, UserVisibilityMode defaultVisibility,
                                                  ListEntryInfoCollection types)
        {
            ListEntryInfo typeInfo = types["DataType:" + type];
            if (typeInfo == null)
            {
                typeInfo = types["DataType:Unknown"];
            }
            var propertyDefinition = new ProfilePropertyDefinition(portalId);
            propertyDefinition.DataType = typeInfo.EntryID;
            propertyDefinition.DefaultValue = "";
            propertyDefinition.ModuleDefId = Null.NullInteger;
            propertyDefinition.PropertyCategory = category;
            propertyDefinition.PropertyName = name;
            propertyDefinition.Required = false;
            propertyDefinition.ViewOrder = viewOrder;
            propertyDefinition.Visible = true;
            propertyDefinition.Length = length;
            propertyDefinition.DefaultVisibility = defaultVisibility;
            AddPropertyDefinition(propertyDefinition);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fills a ProfilePropertyDefinitionCollection from a DataReader
        /// </summary>
        /// <param name="dr">An IDataReader object</param>
        /// <returns>The ProfilePropertyDefinitionCollection</returns>
        /// <history>
        ///     [cnurse]	02/01/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static ProfilePropertyDefinitionCollection FillCollection(IDataReader dr)
        {
            ArrayList arrDefinitions = CBO.FillCollection(dr, typeof (ProfilePropertyDefinition));
            var definitionsCollection = new ProfilePropertyDefinitionCollection();
            foreach (ProfilePropertyDefinition definition in arrDefinitions)
            {
                definition.ClearIsDirty();
                definition.Visibility = definition.DefaultVisibility;
                definitionsCollection.Add(definition);
            }
            return definitionsCollection;
        }

        private static ProfilePropertyDefinition FillPropertyDefinitionInfo(IDataReader dr)
        {
            ProfilePropertyDefinition definition = null;
            try
            {
                definition = FillPropertyDefinitionInfo(dr, false);
            }
			catch (Exception ex)
			{
				DnnLog.Error(ex);
			}
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return definition;
        }

        private static ProfilePropertyDefinition FillPropertyDefinitionInfo(IDataReader dr, bool checkForOpenDataReader)
        {
            ProfilePropertyDefinition definition = null;
            bool canContinue = true;
            if (checkForOpenDataReader)
            {
                canContinue = false;
                if (dr.Read())
                {
                    canContinue = true;
                }
            }
            if (canContinue)
            {
                int portalid = 0;
                portalid = Convert.ToInt32(Null.SetNull(dr["PortalId"], portalid));
                definition = new ProfilePropertyDefinition(portalid);
                definition.PropertyDefinitionId = Convert.ToInt32(Null.SetNull(dr["PropertyDefinitionId"], definition.PropertyDefinitionId));
                definition.ModuleDefId = Convert.ToInt32(Null.SetNull(dr["ModuleDefId"], definition.ModuleDefId));
                definition.DataType = Convert.ToInt32(Null.SetNull(dr["DataType"], definition.DataType));
                definition.DefaultValue = Convert.ToString(Null.SetNull(dr["DefaultValue"], definition.DefaultValue));
                definition.PropertyCategory = Convert.ToString(Null.SetNull(dr["PropertyCategory"], definition.PropertyCategory));
                definition.PropertyName = Convert.ToString(Null.SetNull(dr["PropertyName"], definition.PropertyName));
                definition.Length = Convert.ToInt32(Null.SetNull(dr["Length"], definition.Length));
                definition.Required = Convert.ToBoolean(Null.SetNull(dr["Required"], definition.Required));
                definition.ValidationExpression = Convert.ToString(Null.SetNull(dr["ValidationExpression"], definition.ValidationExpression));
                definition.ViewOrder = Convert.ToInt32(Null.SetNull(dr["ViewOrder"], definition.ViewOrder));
                definition.Visible = Convert.ToBoolean(Null.SetNull(dr["Visible"], definition.Visible));
                definition.DefaultVisibility = (UserVisibilityMode) Convert.ToInt32(Null.SetNull(dr["DefaultVisibility"], definition.DefaultVisibility));
                definition.Visibility = definition.DefaultVisibility;
            }
            return definition;
        }

        private static List<ProfilePropertyDefinition> FillPropertyDefinitionInfoCollection(IDataReader dr)
        {
            var arr = new List<ProfilePropertyDefinition>();
            try
            {
                ProfilePropertyDefinition obj;
                while (dr.Read())
                {
                    obj = FillPropertyDefinitionInfo(dr, false);
                    arr.Add(obj);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return arr;
        }

        private static List<ProfilePropertyDefinition> GetPropertyDefinitions(int portalId)
        {
            string key = string.Format(DataCache.ProfileDefinitionsCacheKey, portalId);
            var definitions = (List<ProfilePropertyDefinition>) DataCache.GetCache(key);
            if (definitions == null)
            {
                Int32 timeOut = DataCache.ProfileDefinitionsCacheTimeOut*Convert.ToInt32(Host.Host.PerformanceSetting);
                definitions = FillPropertyDefinitionInfoCollection(provider.GetPropertyDefinitionsByPortal(portalId));
                if (timeOut > 0)
                {
                    DataCache.SetCache(key, definitions, TimeSpan.FromMinutes(timeOut));
                }
            }
            return definitions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Profile Information for the User
        /// </summary>
        /// <remarks></remarks>
        /// <param name="objUser">The user whose Profile information we are retrieving.</param>
        /// <history>
        /// 	[cnurse]	12/13/2005	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void GetUserProfile(ref UserInfo objUser)
        {
            profileProvider.GetUserProfile(ref objUser);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates a User's Profile
        /// </summary>
        /// <param name="objUser">The use to update</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	02/18/2005	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void UpdateUserProfile(UserInfo objUser)
        {
            if (objUser.Profile.IsDirty)
            {
                profileProvider.UpdateUserProfile(objUser);
            }
            DataCache.ClearUserCache(objUser.PortalID, objUser.Username);
        }

        public static UserInfo UpdateUserProfile(UserInfo objUser, ProfilePropertyDefinitionCollection profileProperties)
        {
            bool updateUser = Null.NullBoolean;
            if (profileProperties != null)
            {
                foreach (ProfilePropertyDefinition propertyDefinition in profileProperties)
                {
                    string propertyName = propertyDefinition.PropertyName;
                    string propertyValue = propertyDefinition.PropertyValue;
                    if (propertyDefinition.IsDirty)
                    {
                        objUser.Profile.SetProfileProperty(propertyName, propertyValue);
                        if (propertyName.ToLower() == "firstname" || propertyName.ToLower() == "lastname")
                        {
                            updateUser = true;
                        }
                    }
                }
                UpdateUserProfile(objUser);
                if (updateUser)
                {
                    UserController.UpdateUser(objUser.PortalID, objUser);
                }
            }
            return objUser;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Validates the Profile properties for the User (determines if all required properties
        /// have been set)
        /// </summary>
        /// <param name="portalId">The Id of the portal.</param>
        /// <param name="objProfile">The profile.</param>
        /// <history>
        /// 	[cnurse]	03/13/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool ValidateProfile(int portalId, UserProfile objProfile)
        {
            bool isValid = true;
            foreach (ProfilePropertyDefinition propertyDefinition in objProfile.ProfileProperties)
            {
                if (propertyDefinition.Required && propertyDefinition.PropertyValue == Null.NullString)
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds the default property definitions for a portal
        /// </summary>
        /// <param name="PortalId">Id of the Portal</param>
        /// <history>
        ///     [cnurse]	02/22/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void AddDefaultDefinitions(int PortalId)
        {
            _orderCounter = 1;
            var objListController = new ListController();
            ListEntryInfoCollection dataTypes = objListController.GetListEntryInfoCollection("DataType");
            AddDefaultDefinition(PortalId, "Name", "Prefix", "Text", 50, UserVisibilityMode.AllUsers, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "FirstName", "Text", 50, UserVisibilityMode.AllUsers, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "MiddleName", "Text", 50, UserVisibilityMode.AllUsers, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "LastName", "Text", 50, UserVisibilityMode.AllUsers, dataTypes);
            AddDefaultDefinition(PortalId, "Name", "Suffix", "Text", 50, UserVisibilityMode.AllUsers, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Unit", "Text", 50, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Street", "Text", 50, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "City", "Text", 50, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Region", "Region", 0, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "Country", "Country", 0, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Address", "PostalCode", "Text", 50, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Telephone", "Text", 50, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Cell", "Text", 50, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Fax", "Text", 50, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "Website", "Text", 50, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Contact Info", "IM", "Text", 50, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Preferences", "Photo", "Image", 0, UserVisibilityMode.AllUsers, dataTypes);
            AddDefaultDefinition(PortalId, "Preferences", "Biography", "RichText", 0, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Preferences", "TimeZone", "TimeZone", 0, UserVisibilityMode.AdminOnly, dataTypes);
            AddDefaultDefinition(PortalId, "Preferences", "PreferredLocale", "Locale", 0, UserVisibilityMode.AdminOnly, dataTypes);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a Property Defintion to the Data Store
        /// </summary>
        /// <param name="definition">An ProfilePropertyDefinition object</param>
        /// <returns>The Id of the definition (or if negative the errorcode of the error)</returns>
        /// <history>
        ///     [cnurse]	02/01/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int AddPropertyDefinition(ProfilePropertyDefinition definition)
        {
            if (definition.Required)
            {
                definition.Visible = true;
            }
            int intDefinition = provider.AddPropertyDefinition(definition.PortalId,
                                                               definition.ModuleDefId,
                                                               definition.DataType,
                                                               definition.DefaultValue,
                                                               definition.PropertyCategory,
                                                               definition.PropertyName,
                                                               definition.Required,
                                                               definition.ValidationExpression,
                                                               definition.ViewOrder,
                                                               definition.Visible,
                                                               definition.Length,
                                                               (int) definition.DefaultVisibility,
                                                               UserController.GetCurrentUserInfo().UserID);
            var objEventLog = new EventLogController();
            objEventLog.AddLog(definition, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.PROFILEPROPERTY_CREATED);
            ClearProfileDefinitionCache(definition.PortalId);
            return intDefinition;
        }

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Clears the Profile Definitions Cache
		/// </summary>
		/// <param name="PortalId">Id of the Portal</param>
		/// <history>
		///     [cnurse]	02/22/2006	created
		/// </history>
		/// -----------------------------------------------------------------------------
        public static void ClearProfileDefinitionCache(int PortalId)
        {
            DataCache.ClearDefinitionsCache(PortalId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deletes a Property Defintion from the Data Store
        /// </summary>
        /// <param name="definition">The ProfilePropertyDefinition object to delete</param>
        /// <history>
        ///     [cnurse]	02/01/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeletePropertyDefinition(ProfilePropertyDefinition definition)
        {
            provider.DeletePropertyDefinition(definition.PropertyDefinitionId);
            var objEventLog = new EventLogController();
            objEventLog.AddLog(definition, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.PROFILEPROPERTY_DELETED);
            ClearProfileDefinitionCache(definition.PortalId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Property Defintion from the Data Store by id
        /// </summary>
        /// <param name="definitionId">The id of the ProfilePropertyDefinition object to retrieve</param>
        /// <param name="portalId">Portal Id.</param>
        /// <returns>The ProfilePropertyDefinition object</returns>
        /// <history>
        ///     [cnurse]	02/01/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static ProfilePropertyDefinition GetPropertyDefinition(int definitionId, int portalId)
        {
            bool bFound = Null.NullBoolean;
            ProfilePropertyDefinition definition = null;
            foreach (ProfilePropertyDefinition def in GetPropertyDefinitions(portalId))
            {
                if (def.PropertyDefinitionId == definitionId)
                {
                    definition = def;
                    bFound = true;
                    break;
                }
            }
            if (!bFound)
            {
                definition = FillPropertyDefinitionInfo(provider.GetPropertyDefinition(definitionId));
            }
            return definition;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Property Defintion from the Data Store by name
        /// </summary>
        /// <param name="portalId">The id of the Portal</param>
        /// <param name="name">The name of the ProfilePropertyDefinition object to retrieve</param>
        /// <returns>The ProfilePropertyDefinition object</returns>
        /// <history>
        ///     [cnurse]	02/01/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static ProfilePropertyDefinition GetPropertyDefinitionByName(int portalId, string name)
        {
            bool bFound = Null.NullBoolean;
            ProfilePropertyDefinition definition = null;
            foreach (ProfilePropertyDefinition def in GetPropertyDefinitions(portalId))
            {
                if (def.PropertyName == name)
                {
                    definition = def;
                    bFound = true;
                    break;
                }
            }
            if (!bFound)
            {
                definition = FillPropertyDefinitionInfo(provider.GetPropertyDefinitionByName(portalId, name));
            }
            return definition;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of Property Defintions from the Data Store by category
        /// </summary>
        /// <param name="portalId">The id of the Portal</param>
        /// <param name="category">The category of the Property Defintions to retrieve</param>
        /// <returns>A ProfilePropertyDefinitionCollection object</returns>
        /// <history>
        ///     [cnurse]	02/01/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static ProfilePropertyDefinitionCollection GetPropertyDefinitionsByCategory(int portalId, string category)
        {
            var definitions = new ProfilePropertyDefinitionCollection();
            foreach (ProfilePropertyDefinition definition in GetPropertyDefinitions(portalId))
            {
                if (definition.PropertyCategory == category)
                {
                    definitions.Add(definition);
                }
            }
            return definitions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of Property Defintions from the Data Store by portal
        /// </summary>
        /// <param name="portalId">The id of the Portal</param>
        /// <returns>A ProfilePropertyDefinitionCollection object</returns>
        /// <history>
        ///     [cnurse]	02/01/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static ProfilePropertyDefinitionCollection GetPropertyDefinitionsByPortal(int portalId)
        {
            return GetPropertyDefinitionsByPortal(portalId, true);
        }

        public static ProfilePropertyDefinitionCollection GetPropertyDefinitionsByPortal(int portalId, bool clone)
        {
            var definitions = new ProfilePropertyDefinitionCollection();
            foreach (ProfilePropertyDefinition definition in GetPropertyDefinitions(portalId))
            {
                if (clone)
                {
                    definitions.Add(definition.Clone());
                }
                else
                {
                    definitions.Add(definition);
                }
            }
            return definitions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates a Property Defintion in the Data Store
        /// </summary>
        /// <param name="definition">The ProfilePropertyDefinition object to update</param>
        /// <history>
        ///     [cnurse]	02/01/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void UpdatePropertyDefinition(ProfilePropertyDefinition definition)
        {
            if (definition.Required)
            {
                definition.Visible = true;
            }
            provider.UpdatePropertyDefinition(definition.PropertyDefinitionId,
                                              definition.DataType,
                                              definition.DefaultValue,
                                              definition.PropertyCategory,
                                              definition.PropertyName,
                                              definition.Required,
                                              definition.ValidationExpression,
                                              definition.ViewOrder,
                                              definition.Visible,
                                              definition.Length,
                                              (int) definition.DefaultVisibility,
                                              UserController.GetCurrentUserInfo().UserID);
            var objEventLog = new EventLogController();
            objEventLog.AddLog(definition, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.PROFILEPROPERTY_UPDATED);
            ClearProfileDefinitionCache(definition.PortalId);
        }

        [Obsolete("This method has been deprecated.  Please use GetPropertyDefinition(ByVal definitionId As Integer, ByVal portalId As Integer) instead")]
        public static ProfilePropertyDefinition GetPropertyDefinition(int definitionId)
        {
            return (ProfilePropertyDefinition) CBO.FillObject(provider.GetPropertyDefinition(definitionId), typeof (ProfilePropertyDefinition));
        }
    }
}
