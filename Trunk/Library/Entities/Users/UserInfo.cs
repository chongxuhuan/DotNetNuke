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
using DotNetNuke.Services.Tokens;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Entities.Users
{
    [Serializable]
    public class UserInfo : BaseEntityInfo, IPropertyAccess
    {
        private string _FullName;
        private UserMembership _Membership;
        private UserProfile _Profile;
        private string[] _Roles;
        private bool _RolesHydrated = Null.NullBoolean;
        private string strAdministratorRoleName;

        public UserInfo()
        {
            RefreshRoles = Null.NullBoolean;
            IsDeleted = Null.NullBoolean;
            UserID = Null.NullInteger;
            PortalID = Null.NullInteger;
            IsSuperUser = Null.NullBoolean;
            AffiliateID = Null.NullInteger;
            _Roles = new string[] {};
        }

        [Browsable(false)]
        public int AffiliateID { get; set; }

        [SortOrder(3), Required(true), MaxLength(128)]
        public string DisplayName { get; set; }

        [SortOrder(4), MaxLength(256), Required(true), RegularExpressionValidator(Globals.glbEmailRegEx)]
        public string Email { get; set; }

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

        [Browsable(false)]
        public bool IsDeleted { get; set; }

        [Browsable(false)]
        public bool IsSuperUser { get; set; }

        [Browsable(false)]
        public string LastIPAddress { get; set; }

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

        [Browsable(false)]
        public UserMembership Membership
        {
            get
            {
                if (_Membership == null)
                {
                    _Membership = new UserMembership(this);
                    if ((Username != null) && (!String.IsNullOrEmpty(Username)))
                    {
                        UserController.GetUserMembership(this);
                    }
                }
                return _Membership;
            }
            set
            {
                _Membership = value;
            }
        }

        [Browsable(false)]
        public int PortalID { get; set; }

        [Browsable(false)]
        public UserProfile Profile
        {
            get
            {
                if (_Profile == null)
                {
                    _Profile = new UserProfile();
                    UserInfo userInfo = this;
                    ProfileController.GetUserProfile(ref userInfo);
                }
                return _Profile;
            }
            set
            {
                _Profile = value;
            }
        }

        [Browsable(false)]
        public bool RefreshRoles { get; set; }

        [Browsable(false)]
        public string[] Roles
        {
            get
            {
                if (!_RolesHydrated)
                {
                    if (UserID > Null.NullInteger)
                    {
                        var controller = new RoleController();
                        _Roles = controller.GetRolesByUser(UserID, PortalID);
                        _RolesHydrated = true;
                    }
                }
                return _Roles;
            }
            set
            {
                _Roles = value;
                _RolesHydrated = true;
            }
        }

        [Browsable(false)]
        public int UserID { get; set; }

        [SortOrder(0), MaxLength(100), IsReadOnly(true), Required(true)]
        public string Username { get; set; }

        #region IPropertyAccess Members

        public string GetProperty(string propertyName, string format, CultureInfo formatProvider, UserInfo AccessingUser, Scope CurrentScope, ref bool PropertyNotFound)
        {
            Scope internScope;
            if (UserID == -1 && CurrentScope > Scope.Configuration)
            {
                internScope = Scope.Configuration;
            }
            else if (UserID != AccessingUser.UserID && !isAdminUser(ref AccessingUser) && CurrentScope > Scope.DefaultSettings)
            {
                internScope = Scope.DefaultSettings;
            }
            else
            {
                internScope = CurrentScope;
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
                case "firstname":
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
                case "lastname":
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
                case "fullname":
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

        public void UpdateDisplayName(string format)
        {
            format = format.Replace("[USERID]", UserID.ToString());
            format = format.Replace("[FIRSTNAME]", FirstName);
            format = format.Replace("[LASTNAME]", LastName);
            format = format.Replace("[USERNAME]", Username);
            DisplayName = format;
        }

        private bool isAdminUser(ref UserInfo AccessingUser)
        {
            if (AccessingUser == null || AccessingUser.UserID == -1)
            {
                return false;
            }
            else if (String.IsNullOrEmpty(strAdministratorRoleName))
            {
                PortalInfo ps = new PortalController().GetPortal(AccessingUser.PortalID);
                strAdministratorRoleName = ps.AdministratorRoleName;
            }
            return AccessingUser.IsInRole(strAdministratorRoleName) || AccessingUser.IsSuperUser;
        }

        [Browsable(false), Obsolete("Deprecated in DNN 5.1. This property has been deprecated in favour of Display Name")]
        public string FullName
        {
            get
            {
                if (String.IsNullOrEmpty(_FullName))
                {
                    _FullName = FirstName + " " + LastName;
                }
                return _FullName;
            }
            set
            {
                _FullName = value;
            }
        }
    }
}
