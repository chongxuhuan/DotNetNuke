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
using System.Globalization;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.SystemDateTime;
using DotNetNuke.Services.Tokens;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Entities.Users
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Users
    /// Class:      UserInfo
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The UserInfo class provides Business Layer model for Users
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	12/13/2005	documented
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class UserInfo : BaseEntityInfo, IPropertyAccess
    {
        #region Private Members

        private string _fullName;
        private UserMembership _membership;
        private UserProfile _profile;
        private UserSocial _social;
        private bool _refreshRoles;
        private string[] _roles;
        private bool _rolesHydrated = Null.NullBoolean;
        private string _administratorRoleName;

        #endregion

        #region Constructors

        public UserInfo()
        {
            _refreshRoles = Null.NullBoolean;
            IsDeleted = Null.NullBoolean;
            UserID = Null.NullInteger;
            PortalID = Null.NullInteger;
            IsSuperUser = Null.NullBoolean;
            AffiliateID = Null.NullInteger;
            _roles = new string[] {};
        }

        #endregion

        #region Public Properties

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the AffiliateId for this user
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public int AffiliateID { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Display Name
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [SortOrder(3), Required(true), MaxLength(128)]
        public string DisplayName { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Email Address
        /// </summary>
        /// <history>
        ///     [cnurse]	02/27/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [SortOrder(4), MaxLength(256), Required(true), RegularExpressionValidator(Globals.glbEmailRegEx)]
        public string Email { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the First Name
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [SortOrder(1), MaxLength(50), Required(true)]
        public string FirstName
        {
            get
            {
                return Profile.FirstName;
            }
            set
            {
                Profile.FirstName = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether the User is deleted
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public bool IsDeleted { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether the User is a SuperUser
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public bool IsSuperUser { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Last IP address used by user
        /// </summary>
        /// <history>
        ///     [cnurse]	02/13/2009	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public string LastIPAddress { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Last Name
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [SortOrder(2), MaxLength(50), Required(true)]
        public string LastName
        {
            get
            {
                return Profile.LastName;
            }
            set
            {
                Profile.LastName = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Membership object
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public UserMembership Membership
        {
            get
            {
				//implemented progressive hydration
                //this object will be hydrated on demand
                if (_membership == null)
                {
                    _membership = new UserMembership(this);
                    if ((Username != null) && (!String.IsNullOrEmpty(Username)))
                    {
                        UserController.GetUserMembership(this);
                    }
                }
                return _membership;
            }
            set
            {
                _membership = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the PortalId
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public int PortalID { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Profile Object
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public UserProfile Profile
        {
            get
            {
				//implemented progressive hydration
                //this object will be hydrated on demand
                if (_profile == null)
                {
                    _profile = new UserProfile();
                    UserInfo userInfo = this;
                    ProfileController.GetUserProfile(ref userInfo);
                }
                return _profile;
            }
            set
            {
                _profile = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Roles for this User
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        ///     [sleupold]  08/14/2007  auto hydration of roles added
        /// </history>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public string[] Roles
        {
            get
            {
                if (!_rolesHydrated)
                {
                    if (UserID > Null.NullInteger) //fill
                    {
                        var controller = new RoleController();
                        _roles = controller.GetRolesByUser(UserID, PortalID);
                        _rolesHydrated = true;
                    }
                }
                return _roles;
            }
            set
            {
                _roles = value;
                _rolesHydrated = true;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the User Id
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public int UserID { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the User Name
        /// </summary>
        /// <history>
        ///     [cnurse]	02/24/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [SortOrder(0), MaxLength(100), IsReadOnly(true), Required(true)]
        public string Username { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Scoial object
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        public UserSocial Social
        {
            get
            {
                return _social ?? (_social = new UserSocial(this));
            }
        }
    
        #region IPropertyAccess Members

        /// <summary>
        /// Property access, initially provided for TokenReplace
        /// </summary>
        /// <param name="propertyName">Name of the Property</param>
        /// <param name="format">format string</param>
        /// <param name="formatProvider">format provider for numbers, dates, currencies</param>
        /// <param name="AccessingUser">userinfo of the user, who queries the data (used to determine permissions)</param>
        /// <param name="CurrentScope">requested maximum access level, might be restricted due to user level</param>
        /// <param name="PropertyNotFound">out: flag, if property could be retrieved.</param>
        /// <returns>current value of the property for this userinfo object</returns>
        /// <history>
        ///    2007-10-20   [sleupold]   documented and extended with differenciated access permissions
        ///    2007-10-20   [sleupold]   role access added (for user himself or admin only).
        /// </history>
        public string GetProperty(string propertyName, string format, CultureInfo formatProvider, UserInfo AccessingUser, Scope CurrentScope, ref bool PropertyNotFound)
        {
            Scope internScope;
            if (UserID == -1 && CurrentScope > Scope.Configuration)
            {
                internScope = Scope.Configuration; //anonymous users only get access to displayname
            }
            else if (UserID != AccessingUser.UserID && !isAdminUser(ref AccessingUser) && CurrentScope > Scope.DefaultSettings)
            {
                internScope = Scope.DefaultSettings; //registerd users can access username and userID as well
            }
            else
            {
                internScope = CurrentScope; //admins and user himself can access all data
            }
            string OutputFormat = string.Empty;
            if (format == string.Empty)
            {
                OutputFormat = "g";
            }
            else
            {
                OutputFormat = format;
            }
            switch (propertyName.ToLower())
            {
                case "verificationcode":
                    if (internScope < Scope.SystemMessages)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return PortalID + "-" + UserID;
                case "affiliateid":
                    if (internScope < Scope.SystemMessages)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return (AffiliateID.ToString(OutputFormat, formatProvider));
                case "displayname":
                    if (internScope < Scope.Configuration)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return PropertyAccess.FormatString(DisplayName, format);
                case "email":
                    if (internScope < Scope.DefaultSettings)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return (PropertyAccess.FormatString(Email, format));
                case "firstname": //using profile property is recommended!
                    if (internScope < Scope.DefaultSettings)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return (PropertyAccess.FormatString(FirstName, format));
                case "issuperuser":
                    if (internScope < Scope.Debug)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return (IsSuperUser.ToString(formatProvider));
                case "lastname": //using profile property is recommended!
                    if (internScope < Scope.DefaultSettings)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return (PropertyAccess.FormatString(LastName, format));
                case "portalid":
                    if (internScope < Scope.Configuration)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return (PortalID.ToString(OutputFormat, formatProvider));
                case "userid":
                    if (internScope < Scope.DefaultSettings)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return (UserID.ToString(OutputFormat, formatProvider));
                case "username":
                    if (internScope < Scope.DefaultSettings)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return (PropertyAccess.FormatString(Username, format));
                case "fullname": //fullname is obsolete, it will return DisplayName
                    if (internScope < Scope.Configuration)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return (PropertyAccess.FormatString(DisplayName, format));
                case "roles":
                    if (CurrentScope < Scope.SystemMessages)
                    {
                        PropertyNotFound = true;
                        return PropertyAccess.ContentLocked;
                    }
                    return (PropertyAccess.FormatString(string.Join(", ", Roles), format));
            }
            PropertyNotFound = true;
            return string.Empty;
        }

        [Browsable(false)]
        public CacheLevel Cacheability
        {
            get
            {
                return CacheLevel.notCacheable;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Determine, if accessing user is Administrator
        /// </summary>
        /// <param name="AccessingUser">userinfo of the user to query</param>
        /// <returns>true, if user is portal administrator or superuser</returns>
        /// <history>
        ///    2007-10-20 [sleupold] added
        /// </history>
        private bool isAdminUser(ref UserInfo AccessingUser)
        {
            if (AccessingUser == null || AccessingUser.UserID == -1)
            {
                return false;
            }
            else if (String.IsNullOrEmpty(_administratorRoleName))
            {
                PortalInfo ps = new PortalController().GetPortal(AccessingUser.PortalID);
                _administratorRoleName = ps.AdministratorRoleName;
            }
            return AccessingUser.IsInRole(_administratorRoleName) || AccessingUser.IsSuperUser;
        }

        #endregion

        #region Public Methods

        public void ClearRoles()
        {
            _roles = new string[] {};
            _rolesHydrated = false;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// IsInRole determines whether the user is in the role passed
        /// </summary>
        /// <param name="role">The role to check</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        /// <history>
        ///     [cnurse]	12/13/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool IsInRole(string role)
        {
            if (IsSuperUser || role == Globals.glbRoleAllUsersName)
            {
                return true;
            }
            else
            {
                if ("[" + UserID + "]" == role)
                {
                    return true;
                }
                if (Roles != null)
                {
                    foreach (string strRole in Roles)
                    {
                        if (strRole == role)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets current time in User's timezone
        /// </summary>
        /// <history>
        ///     [aprasad]	07/19/2011	Added
        /// </history>
        /// -----------------------------------------------------------------------------        
        public DateTime LocalTime()
        {
            return LocalTime(SystemDateTime.GetCurrentTimeUtc());
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Convert utc time in User's timezone
        /// </summary>
        /// <param name="utcTime">Utc time to convert</param>
        /// <history>
        ///     [aprasad]	07/19/2011	Added
        /// </history>
        /// -----------------------------------------------------------------------------       
        public DateTime LocalTime(DateTime utcTime)
        {
            if (UserID > Null.NullInteger)
            {
                return TimeZoneInfo.ConvertTime(utcTime, TimeZoneInfo.Utc, this.Profile.PreferredTimeZone);
            }
            else
            {
                return TimeZoneInfo.ConvertTime(utcTime, TimeZoneInfo.Utc, PortalController.GetCurrentPortalSettings().TimeZone);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateDisplayName updates the displayname to the format provided
        /// </summary>
        /// <param name="format">The format to use</param>
        /// <history>
        ///     [cnurse]	02/21/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void UpdateDisplayName(string format)
        {
            //Replace Tokens
            format = format.Replace("[USERID]", UserID.ToString());
            format = format.Replace("[FIRSTNAME]", FirstName);
            format = format.Replace("[LASTNAME]", LastName);
            format = format.Replace("[USERNAME]", Username);
            DisplayName = format;
        }

        #endregion

        #region Obsolete

        [Browsable(false), Obsolete("Deprecated in DNN 5.1. This property has been deprecated in favour of Display Name")]
        public string FullName
        {
            get
            {
                if (String.IsNullOrEmpty(_fullName))
                {
					//Build from component names
                    _fullName = FirstName + " " + LastName;
                }
                return _fullName;
            }
            set
            {
                _fullName = value;
            }
        }

        [Browsable(false), Obsolete("Deprecated in DNN 6.2. Roles are no longer stored in a cookie so this property is no longer neccessary")]
        public bool RefreshRoles
        {
            get
            {
                return _refreshRoles;
            }
            set
            {
                _refreshRoles = value;
            }
        }

        #endregion
    }
}
