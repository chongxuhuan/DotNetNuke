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
using System.ComponentModel;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using System.Xml.Serialization;

#endregion

namespace DotNetNuke.Entities.Users
{
    [Serializable]
    public class UserProfile
    {
        private const string cPrefix = "Prefix";
        private const string cFirstName = "FirstName";
        private const string cMiddleName = "MiddleName";
        private const string cLastName = "LastName";
        private const string cSuffix = "Suffix";
        private const string cUnit = "Unit";
        private const string cStreet = "Street";
        private const string cCity = "City";
        private const string cRegion = "Region";
        private const string cCountry = "Country";
        private const string cPostalCode = "PostalCode";
        private const string cTelephone = "Telephone";
        private const string cCell = "Cell";
        private const string cFax = "Fax";
        private const string cWebsite = "Website";
        private const string cIM = "IM";
        private const string cPhoto = "Photo";
        private const string cTimeZone = "TimeZone";
        private const string cPreferredLocale = "PreferredLocale";
        private const String cPreferredTimeZone = "PreferredTimeZone";
        private bool _IsDirty;
        private ProfilePropertyDefinitionCollection _profileProperties;

        public string Cell
        {
            get
            {
                return GetPropertyValue(cCell);
            }
            set
            {
                SetProfileProperty(cCell, value);
            }
        }

        public string City
        {
            get
            {
                return GetPropertyValue(cCity);
            }
            set
            {
                SetProfileProperty(cCity, value);
            }
        }

        public string Country
        {
            get
            {
                return GetPropertyValue(cCountry);
            }
            set
            {
                SetProfileProperty(cCountry, value);
            }
        }

        public string Fax
        {
            get
            {
                return GetPropertyValue(cFax);
            }
            set
            {
                SetProfileProperty(cFax, value);
            }
        }

        public string FirstName
        {
            get
            {
                return GetPropertyValue(cFirstName);
            }
            set
            {
                SetProfileProperty(cFirstName, value);
            }
        }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public string IM
        {
            get
            {
                return GetPropertyValue(cIM);
            }
            set
            {
                SetProfileProperty(cIM, value);
            }
        }

        public bool IsDirty
        {
            get
            {
                return _IsDirty;
            }
        }

        public string LastName
        {
            get
            {
                return GetPropertyValue(cLastName);
            }
            set
            {
                SetProfileProperty(cLastName, value);
            }
        }

        public string Photo
        {
            get
            {
                return GetPropertyValue(cPhoto);
            }
            set
            {
                SetProfileProperty(cPhoto, value);
            }
        }

        public string PhotoURL
        {
            get
            {
                string strPhotoURL = Globals.ApplicationPath + "/images/no_avatar.gif";
                ProfilePropertyDefinition objProperty = GetProperty(cPhoto);
                if ((objProperty != null))
                {
                    if (!string.IsNullOrEmpty(objProperty.PropertyValue) && objProperty.Visibility == UserVisibilityMode.AllUsers)
                    {
                        var objFile = FileManager.Instance.GetFile(int.Parse(objProperty.PropertyValue));
                        if ((objFile != null))
                        {
                            PortalInfo objPortal = new PortalController().GetPortal(objFile.PortalId);
                            if (objPortal != null)
                            {
                                strPhotoURL = Globals.ApplicationPath + "/" + objFile.RelativePath;
                            }
                        }
                    }
                }
                return strPhotoURL;
            }
        }


        public string PostalCode
        {
            get
            {
                return GetPropertyValue(cPostalCode);
            }
            set
            {
                SetProfileProperty(cPostalCode, value);
            }
        }

        public string PreferredLocale
        {
            get
            {
                return GetPropertyValue(cPreferredLocale);
            }
            set
            {
                SetProfileProperty(cPreferredLocale, value);
            }
        }

      [XmlIgnore]
        public TimeZoneInfo PreferredTimeZone
        {
            get
            {
                //First set to Server
                TimeZoneInfo _TimeZone = TimeZoneInfo.Local;

                //Next check if there is a Property Setting
                string _TimeZoneId = GetPropertyValue(cPreferredTimeZone);
                if (!string.IsNullOrEmpty(_TimeZoneId))
                {
                    _TimeZone = TimeZoneInfo.FindSystemTimeZoneById(_TimeZoneId);
                }
                //Check if old offset setting is still around. If yes, then use it and save that to new format..
                //this is an anti-pattern that we are setting in a getter, but we wanted to lazy-upgrade this setting.
                else if (LegacyTimeZone != Null.NullInteger)
                {
                    TimeZoneInfo timeZone = Localization.ConvertLegacyTimeZoneOffsetToTimeZoneInfo(LegacyTimeZone);
                    _TimeZone = timeZone;
                    PreferredTimeZone = timeZone;
                }
                //Next check if there is a Portal Setting
                else
                {
                    PortalSettings _PortalSettings = PortalController.GetCurrentPortalSettings();
                    if (_PortalSettings != null)
                    {
                        _TimeZone = _PortalSettings.TimeZone;
                    }
                }

                //still we can't find it or it's somehow set to null
                if (_TimeZone == null)
                {
                    _TimeZone = TimeZoneInfo.Local;                    
                }

                //return timezone
                return _TimeZone;
            }
            set
            {
                SetProfileProperty(cPreferredTimeZone, value.Id);
            }
        }

        public ProfilePropertyDefinitionCollection ProfileProperties
        {
            get
            {
                if (_profileProperties == null)
                {
                    _profileProperties = new ProfilePropertyDefinitionCollection();
                }
                return _profileProperties;
            }
        }

        public string Region
        {
            get
            {
                return GetPropertyValue(cRegion);
            }
            set
            {
                SetProfileProperty(cRegion, value);
            }
        }

        public string Street
        {
            get
            {
                return GetPropertyValue(cStreet);
            }
            set
            {
                SetProfileProperty(cStreet, value);
            }
        }

        public string Telephone
        {
            get
            {
                return GetPropertyValue(cTelephone);
            }
            set
            {
                SetProfileProperty(cTelephone, value);
            }
        }

        public string Unit
        {
            get
            {
                return GetPropertyValue(cUnit);
            }
            set
            {
                SetProfileProperty(cUnit, value);
            }
        }

        public string Website
        {
            get
            {
                return GetPropertyValue(cWebsite);
            }
            set
            {
                SetProfileProperty(cWebsite, value);
            }
        }

        public void ClearIsDirty()
        {
            _IsDirty = false;
            foreach (ProfilePropertyDefinition profProperty in ProfileProperties)
            {
                profProperty.ClearIsDirty();
            }
        }

        public ProfilePropertyDefinition GetProperty(string propName)
        {
            return ProfileProperties[propName];
        }

        public string GetPropertyValue(string propName)
        {
            string propValue = Null.NullString;
            ProfilePropertyDefinition profileProp = GetProperty(propName);
            if (profileProp != null)
            {
                propValue = profileProp.PropertyValue;
            }
            return propValue;
        }

        public void InitialiseProfile(int portalId)
        {
            InitialiseProfile(portalId, true);
        }

        public void InitialiseProfile(int portalId, bool useDefaults)
        {
            _profileProperties = ProfileController.GetPropertyDefinitionsByPortal(portalId, true);
            if (useDefaults)
            {
                foreach (ProfilePropertyDefinition ProfileProperty in _profileProperties)
                {
                    ProfileProperty.PropertyValue = ProfileProperty.DefaultValue;
                }
            }
        }

        public void SetProfileProperty(string propName, string propvalue)
        {
            ProfilePropertyDefinition profileProp = GetProperty(propName);
            if (profileProp != null)
            {
                profileProp.PropertyValue = propvalue;
                if (profileProp.IsDirty)
                {
                    _IsDirty = true;
                }
            }
        }

        #region Obsolete

        [Obsolete("Deprecated in DNN 6.0. Replaced by PreferredTimeZone.")]
        [Browsable(false)]
        public int TimeZone
        {
            get
            {
                return Convert.ToInt32(PreferredTimeZone.BaseUtcOffset.TotalMinutes);
            }
            set
            {
                PreferredTimeZone = Localization.ConvertLegacyTimeZoneOffsetToTimeZoneInfo(value);
            }
        }

        //this private property is created to simply remove obsolete warnings and clarity of code
        private int LegacyTimeZone
        {
            get
            {
                Int32 retValue = Null.NullInteger;
                string propValue = GetPropertyValue(cTimeZone);
                if (!string.IsNullOrEmpty(propValue))
                {
                    retValue = int.Parse(propValue);
                }
                return retValue;
            }
            set
            {
                SetProfileProperty(cTimeZone, value.ToString());
            }
        }

        #endregion
    }
}
