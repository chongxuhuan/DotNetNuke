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

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Membership.Data;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Security.Profile
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Profile
    /// Class:      DNNProfileProvider
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The DNNProfileProvider overrides the default ProfileProvider to provide
    /// a purely DotNetNuke implementation
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	03/29/2006	Created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class DNNProfileProvider : ProfileProvider
    {
        private readonly DataProvider dataProvider;

        public DNNProfileProvider()
        {
            dataProvider = DataProvider.Instance();
            if (dataProvider == null)
            {
				//get the provider configuration based on the type
                string defaultprovider = Data.DataProvider.Instance().DefaultProviderName;
                string dataProviderNamespace = "DotNetNuke.Security.Membership.Data";
                if (defaultprovider == "SqlDataProvider")
                {
                    dataProvider = new SqlDataProvider();
                }
                else
                {
                    string providerType = dataProviderNamespace + "." + defaultprovider;
                    dataProvider = (DataProvider) Reflection.CreateObject(providerType, providerType, true);
                }
                ComponentFactory.RegisterComponentInstance<DataProvider>(dataProvider);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the Provider Properties can be edited
        /// </summary>
        /// <returns>A Boolean</returns>
        /// <history>
        /// 	[cnurse]	03/29/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override bool CanEditProviderProperties
        {
            get
            {
                return true;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetUserProfile retrieves the UserProfile information from the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user whose Profile information we are retrieving.</param>
        /// <history>
        /// 	[cnurse]	03/29/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void GetUserProfile(ref UserInfo user)
        {
            int portalId;
            int definitionId;
            string oldTimeZone = string.Empty;
            string newTimeZone = string.Empty;
            int oldTimeZoneDefinitionId = Null.NullInteger;
            int newTimeZoneDefinitionId = Null.NullInteger;

            ProfilePropertyDefinition profProperty;
            ProfilePropertyDefinitionCollection properties;
            if (user.IsSuperUser)
            {
                portalId = Globals.glbSuperUserAppName;
            }
            else
            {
                portalId = user.PortalID;
            }
            properties = ProfileController.GetPropertyDefinitionsByPortal(portalId, true);

            //Load the Profile properties
            if (user.UserID > Null.NullInteger)
            {
                IDataReader dr = dataProvider.GetUserProfile(user.UserID);
                try
                {
                    while (dr.Read())
                    {
						//Ensure the data reader returned is valid
                        if (!string.Equals(dr.GetName(0), "ProfileID", StringComparison.InvariantCultureIgnoreCase))
                        {
                            break;
                        }
                        definitionId = Convert.ToInt32(dr["PropertyDefinitionId"]);
                        profProperty = properties.GetById(definitionId);
                        if (profProperty != null)
                        {
                            profProperty.PropertyValue = Convert.ToString(dr["PropertyValue"]);
                            profProperty.Visibility = (UserVisibilityMode) dr["Visibility"];

                            if(profProperty.PropertyName == "TimeZone")
                            {
                                oldTimeZone = profProperty.PropertyValue;
                                oldTimeZoneDefinitionId = definitionId;
                            }
                            if (profProperty.PropertyName == "PreferredTimeZone")
                            {
                                newTimeZone = profProperty.PropertyValue;
                                newTimeZoneDefinitionId = definitionId;
                            }
                        }
                    }
                    //lazy load time zone info...this is an anti-pattern
                    //old timezone is present but new is not...we will set that up.
                    if(!string.IsNullOrEmpty(oldTimeZone) && string.IsNullOrEmpty(newTimeZone))
                    {
                        ProfilePropertyDefinition oldTimeZoneProfProperty = properties.GetById(oldTimeZoneDefinitionId);
                        ProfilePropertyDefinition newTimeZoneProfProperty = properties.GetById(newTimeZoneDefinitionId);
                        if (oldTimeZoneProfProperty != null && newTimeZoneProfProperty != null) //redundant check
                        {
                            int oldOffset;
                            int.TryParse(oldTimeZoneProfProperty.PropertyValue, out oldOffset);
                            TimeZoneInfo timeZoneInfo = Localization.ConvertLegacyTimeZoneOffsetToTimeZoneInfo(oldOffset);
                            newTimeZoneProfProperty.PropertyValue = timeZoneInfo.Id;
                            UpdateUserProfile(user);
                        }
                    }
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
            }
			
            //Clear the profile
            user.Profile.ProfileProperties.Clear();
            
			//Add the properties to the profile
			foreach (ProfilePropertyDefinition property in properties)
            {
                profProperty = property;
                if (string.IsNullOrEmpty(profProperty.PropertyValue) && !string.IsNullOrEmpty(profProperty.DefaultValue))
                {
                    profProperty.PropertyValue = profProperty.DefaultValue;
                }
                user.Profile.ProfileProperties.Add(profProperty);
            }
			
            //Clear IsDirty Flag
            user.Profile.ClearIsDirty();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateUserProfile persists a user's Profile to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to persist to the Data Store.</param>
        /// <history>
        /// 	[cnurse]	03/29/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void UpdateUserProfile(UserInfo user)
        {
            ProfilePropertyDefinitionCollection properties = user.Profile.ProfileProperties;
            foreach (ProfilePropertyDefinition profProperty in properties)
            {
                if ((profProperty.PropertyValue != null) && (profProperty.IsDirty))
                {
                    var objSecurity = new PortalSecurity();
                    string propertyValue = objSecurity.InputFilter(profProperty.PropertyValue, PortalSecurity.FilterFlag.NoScripting);
                    dataProvider.UpdateProfileProperty(Null.NullInteger, user.UserID, profProperty.PropertyDefinitionId, propertyValue, (int) profProperty.Visibility, DateTime.Now);
                    var objEventLog = new EventLogController();
                    objEventLog.AddLog(user, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", "USERPROFILE_UPDATED");
                }
            }
        }
    }
}
