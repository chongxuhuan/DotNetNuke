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
using System.IO;
using System.Web.Caching;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Entities.Modules
{
    public enum DisplayMode
    {
        All = 0,
        FirstLetter = 1,
        None = 2
    }

    public enum UsersControl
    {
        Combo = 0,
        TextBox = 1
    }

    /// -----------------------------------------------------------------------------
    /// Project	 :  DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Modules
    /// Class	 :  UserModuleBase
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The UserModuleBase class defines a custom base class inherited by all
    /// desktop portal modules within the Portal that manage Users.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	03/20/2006
    /// </history>
    /// -----------------------------------------------------------------------------
    public class UserModuleBase : PortalModuleBase
    {
        private UserInfo _User;

        protected bool AddUser
        {
            get
            {
                return (UserId == Null.NullInteger);
            }
        }

        protected bool IsAdmin
        {
            get
            {
                return IsEditable;
            }
        }

        protected bool IsHostTab
        {
            get
            {
                return (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId);
            }
        }

        protected bool IsEdit
        {
            get
            {
                bool _IsEdit = false;
                if (Request.QueryString["ctl"] != null)
                {
                    string ctl = Request.QueryString["ctl"];
                    if (ctl == "Edit")
                    {
                        _IsEdit = true;
                    }
                }
                return _IsEdit;
            }
        }

        protected bool IsProfile
        {
            get
            {
                bool _IsProfile = false;
                if (IsUser)
                {
                    if (PortalSettings.UserTabId != -1)
                    {
                        if (PortalSettings.ActiveTab.TabID == PortalSettings.UserTabId)
                        {
                            _IsProfile = true;
                        }
                    }
                    else
                    {
                        if (Request.QueryString["ctl"] != null)
                        {
                            string ctl = Request.QueryString["ctl"];
                            if (ctl == "Profile")
                            {
                                _IsProfile = true;
                            }
                        }
                    }
                }
                return _IsProfile;
            }
        }

        protected bool IsRegister
        {
            get
            {
                bool _IsRegister = false;
                if (!IsAdmin && !IsUser)
                {
                    _IsRegister = true;
                }
                return _IsRegister;
            }
        }

        protected bool IsUser
        {
            get
            {
                bool _IsUser = false;
                if (Request.IsAuthenticated)
                {
                    _IsUser = (User.UserID == UserInfo.UserID);
                }
                return _IsUser;
            }
        }

        protected int UserPortalID
        {
            get
            {
                if (IsHostTab)
                {
                    return Null.NullInteger;
                }
                else
                {
                    return PortalId;
                }
            }
        }

        public UserInfo User
        {
            get
            {
                if (_User == null)
                {
                    if (AddUser)
                    {
                        _User = InitialiseUser();
                    }
                    else
                    {
                        _User = UserController.GetUserById(UserPortalID, UserId);
                    }
                }
                return _User;
            }
            set
            {
                _User = value;
                if (_User != null)
                {
                    UserId = _User.UserID;
                }
            }
        }

        public new int UserId
        {
            get
            {
                int _UserId = Null.NullInteger;
                if (ViewState["UserId"] == null)
                {
                    if (Request.QueryString["userid"] != null)
                    {
                        _UserId = Int32.Parse(Request.QueryString["userid"]);
                        ViewState["UserId"] = _UserId;
                    }
                }
                else
                {
                    _UserId = Convert.ToInt32(ViewState["UserId"]);
                }
                return _UserId;
            }
            set
            {
                ViewState["UserId"] = value;
            }
        }

        [Obsolete("In DotNetNuke 5.0 there is no longer the concept of an Admin Page.  All pages are controlled by Permissions")]
        protected bool IsAdminTab
        {
            get
            {
                return false;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Setting for the Module
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	05/01/2006  Created
        ///     [cnurse]    02/07/2008  DNN-7003 Fixed GetSetting() in UserModuleBase so it handles situation where one or more settings are missing.
        /// </history>
        /// -----------------------------------------------------------------------------
        public static object GetSetting(int portalId, string settingKey)
        {
            Hashtable settings = UserController.GetUserSettings(portalId);
            if (settings[settingKey] == null)
            {
                settings = UserController.GetUserSettings(settings);
            }
            return settings[settingKey];
        }

        public static void UpdateSetting(int portalId, string key, string setting)
        {
            if (portalId == Null.NullInteger)
            {
                HostController.Instance.Update(new ConfigurationSetting {Value = setting, Key = key});
            }
            else
            {
                PortalController.UpdatePortalSetting(portalId, key, setting);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates the Settings for the Module
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	06/27/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void UpdateSettings(int portalId, Hashtable settings)
        {
            string key;
            string setting;
            IDictionaryEnumerator settingsEnumerator = settings.GetEnumerator();
            while (settingsEnumerator.MoveNext())
            {
                key = Convert.ToString(settingsEnumerator.Key);
                setting = Convert.ToString(settingsEnumerator.Value);
                UpdateSetting(portalId, key, setting);
            }
        }

        private UserInfo InitialiseUser()
        {
            var newUser = new UserInfo();
            if (IsHostMenu)
            {
                newUser.IsSuperUser = true;
            }
            else
            {
                newUser.PortalID = PortalId;
            }

            string lc = new Localization().CurrentUICulture;

            newUser.Profile.InitialiseProfile(PortalId);
            newUser.Profile.PreferredTimeZone = PortalSettings.TimeZone;

            newUser.Profile.PreferredLocale = lc;
            string country = Null.NullString;
            country = LookupCountry();
            if (!String.IsNullOrEmpty(country))
            {
                newUser.Profile.Country = country;
            }
            int AffiliateId = Null.NullInteger;
            if (Request.Cookies["AffiliateId"] != null)
            {
                AffiliateId = int.Parse(Request.Cookies["AffiliateId"].Value);
            }
            newUser.AffiliateID = AffiliateId;
            return newUser;
        }

        private string LookupCountry()
        {
            string IP;
            bool IsLocal = false;
            bool _CacheGeoIPData = true;
            string _GeoIPFile;
            _GeoIPFile = "controls/CountryListBox/Data/GeoIP.dat";
            if (Page.Request.UserHostAddress == "127.0.0.1")
            {
                IsLocal = true;
                IP = Page.Request.UserHostAddress;
            }
            else
            {
                IP = Page.Request.UserHostAddress;
            }
            if (Context.Cache.Get("GeoIPData") == null && _CacheGeoIPData)
            {
                Context.Cache.Insert("GeoIPData", CountryLookup.FileToMemory(Context.Server.MapPath(_GeoIPFile)), new CacheDependency(Context.Server.MapPath(_GeoIPFile)));
            }
            if (IsLocal)
            {
                return Null.NullString;
            }
            CountryLookup _CountryLookup;
            if (_CacheGeoIPData)
            {
                _CountryLookup = new CountryLookup((MemoryStream) Context.Cache.Get("GeoIPData"));
            }
            else
            {
                _CountryLookup = new CountryLookup(Context.Server.MapPath(_GeoIPFile));
            }
            string country = Null.NullString;
            try
            {
                country = _CountryLookup.LookupCountryName(IP);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            return country;
        }

        protected void AddLocalizedModuleMessage(string message, ModuleMessage.ModuleMessageType type, bool display)
        {
            if (display)
            {
                UI.Skins.Skin.AddModuleMessage(this, message, type);
            }
        }

        protected void AddModuleMessage(string message, ModuleMessage.ModuleMessageType type, bool display)
        {
            AddLocalizedModuleMessage(Localization.GetString(message, LocalResourceFile), type, display);
        }

        protected string CompleteUserCreation(UserCreateStatus createStatus, UserInfo newUser, bool notify, bool register)
        {
            string strMessage = "";
            ModuleMessage.ModuleMessageType message = ModuleMessage.ModuleMessageType.RedError;
            if (register)
            {
                strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationAdmin, PortalSettings);
                switch (PortalSettings.UserRegistration)
                {
                    case (int) Globals.PortalRegistrationType.PrivateRegistration:
                        strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationPrivate, PortalSettings);
                        if (string.IsNullOrEmpty(strMessage))
                        {
                            strMessage += string.Format(Localization.GetString("PrivateConfirmationMessage", Localization.SharedResourceFile), newUser.Email);
                            message = ModuleMessage.ModuleMessageType.GreenSuccess;
                        }
                        break;
                    case (int) Globals.PortalRegistrationType.PublicRegistration:
                        Mail.SendMail(newUser, MessageType.UserRegistrationPublic, PortalSettings);
                        UserLoginStatus loginStatus = UserLoginStatus.LOGIN_FAILURE;
                        UserController.UserLogin(PortalSettings.PortalId, newUser.Username, newUser.Membership.Password, "", PortalSettings.PortalName, "", ref loginStatus, false);
                        break;
                    case (int) Globals.PortalRegistrationType.VerifiedRegistration:
                        strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationVerified, PortalSettings);
                        if (string.IsNullOrEmpty(strMessage))
                        {
                            strMessage += string.Format(Localization.GetString("VerifiedConfirmationMessage", Localization.SharedResourceFile), newUser.Email);
                            message = ModuleMessage.ModuleMessageType.GreenSuccess;
                        }
                        break;
                }
                if (!Null.IsNull(User.AffiliateID))
                {
                    var objAffiliates = new AffiliateController();
                    objAffiliates.UpdateAffiliateStats(newUser.AffiliateID, 0, 1);
                }
                Localization.SetLanguage(newUser.Profile.PreferredLocale);
                if (IsRegister && message == ModuleMessage.ModuleMessageType.RedError)
                {
                    AddLocalizedModuleMessage(string.Format(Localization.GetString("SendMail.Error", Localization.SharedResourceFile), strMessage), message, (!String.IsNullOrEmpty(strMessage)));
                }
                else
                {
                    AddLocalizedModuleMessage(strMessage, message, (!String.IsNullOrEmpty(strMessage)));
                }
            }
            else
            {
                if (notify)
                {
                    if (PortalSettings.UserRegistration == (int) Globals.PortalRegistrationType.VerifiedRegistration)
                    {
                        strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationVerified, PortalSettings);
                    }
                    else
                    {
                        strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationPublic, PortalSettings);
                    }
                }
            }
            var objEventLog = new EventLogController();
            objEventLog.AddLog(newUser, PortalSettings, UserId, newUser.Username, EventLogController.EventLogType.USER_CREATED);
            return strMessage;
        }

        [Obsolete("In DotNetNuke 5.2 replaced by UserController.GetDefaultUserSettings().")]
        public static Hashtable GetDefaultSettings()
        {
            return UserController.GetDefaultUserSettings();
        }

        [Obsolete("In DotNetNuke 5.2 replaced by UserController.GetUserSettings(settings).")]
        public static Hashtable GetSettings(Hashtable settings)
        {
            return UserController.GetUserSettings(settings);
        }
    }
}
