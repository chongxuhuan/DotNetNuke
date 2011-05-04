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
using System.Globalization;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Tokens;

#endregion

namespace DotNetNuke.Entities.Users
{
    public class ProfilePropertyAccess : IPropertyAccess
    {
        private readonly UserInfo objUser;
        private string strAdministratorRoleName;

        public ProfilePropertyAccess(UserInfo user)
        {
            objUser = user;
        }

        #region IPropertyAccess Members

        public string GetProperty(string propertyName, string format, CultureInfo formatProvider, UserInfo AccessingUser, Scope currentScope, ref bool PropertyNotFound)
        {
            if (currentScope >= Scope.DefaultSettings && objUser != null && objUser.Profile != null)
            {
                UserProfile objProfile = objUser.Profile;
                foreach (ProfilePropertyDefinition prop in objProfile.ProfileProperties)
                {
                    if (prop.PropertyName.ToLower() == propertyName.ToLower())
                    {
                        if (CheckAccessLevel(prop.Visibility, AccessingUser))
                        {
                            return GetRichValue(prop, format, formatProvider);
                        }
                        else
                        {
                            PropertyNotFound = true;
                            return PropertyAccess.ContentLocked;
                        }
                        //break;
                    }
                }
            }
            PropertyNotFound = true;
            return string.Empty;
        }

        public CacheLevel Cacheability
        {
            get
            {
                return CacheLevel.notCacheable;
            }
        }

        #endregion

        public static string GetRichValue(ProfilePropertyDefinition prop, string strFormat, CultureInfo formatProvider)
        {
            string result = "";
            if (!String.IsNullOrEmpty(prop.PropertyValue) || DisplayDataType(prop).ToLower() == "image")
            {
                switch (DisplayDataType(prop).ToLower())
                {
                    case "truefalse":
                        result = PropertyAccess.Boolean2LocalizedYesNo(Convert.ToBoolean(prop.PropertyValue), formatProvider);
                        break;
                    case "date":
                    case "datetime":
                        if (strFormat == string.Empty)
                        {
                            strFormat = "g";
                        }
                        result = DateTime.Parse(prop.PropertyValue, CultureInfo.InvariantCulture).ToString(strFormat, formatProvider);
                        break;
                    case "integer":
                        if (strFormat == string.Empty)
                        {
                            strFormat = "g";
                        }
                        result = int.Parse(prop.PropertyValue).ToString(strFormat, formatProvider);
                        break;
                    case "page":
                        var TabCtrl = new TabController();
                        int tabid;
                        if (int.TryParse(prop.PropertyValue, out tabid))
                        {
                            TabInfo Tab = TabCtrl.GetTab(tabid, Null.NullInteger, false);
                            if (Tab != null)
                            {
                                result = string.Format("<a href='{0}'>{1}</a>", Globals.NavigateURL(tabid), Tab.LocalizedTabName);
                            }
                        }
                        break;
                    case "image":
                        //File is stored as a FileID
                        int fileID;
                        if (Int32.TryParse(prop.PropertyValue, out fileID) && fileID > 0)
                        {
                            result = Globals.LinkClick(String.Format("fileid={0}", fileID), Null.NullInteger, Null.NullInteger);
                        }
                        else
                        {
                            result = Globals.ResolveUrl("~/images/spacer.gif");
                        }
                        break;
                    case "richtext":
                        var objSecurity = new PortalSecurity();
                        result = PropertyAccess.FormatString(objSecurity.InputFilter(HttpUtility.HtmlDecode(prop.PropertyValue), PortalSecurity.FilterFlag.NoScripting), strFormat);
                        break;
                    default:
                        result = HttpUtility.HtmlEncode(PropertyAccess.FormatString(prop.PropertyValue, strFormat));
                        break;
                }
            }
            return result;
        }

        private static string DisplayDataType(ProfilePropertyDefinition definition)
        {
            string CacheKey = string.Format("DisplayDataType:{0}", definition.DataType);
            string strDataType = Convert.ToString(DataCache.GetCache(CacheKey)) + "";
            if (strDataType == string.Empty)
            {
                var objListController = new ListController();
                strDataType = objListController.GetListEntryInfo(definition.DataType).Value;
                DataCache.SetCache(CacheKey, strDataType);
            }
            return strDataType;
        }

        private bool CheckAccessLevel(UserVisibilityMode VisibilityMode, UserInfo AccessingUser)
        {
            if (String.IsNullOrEmpty(strAdministratorRoleName) && !AccessingUser.IsSuperUser)
            {
                PortalInfo ps = new PortalController().GetPortal(objUser.PortalID);
                strAdministratorRoleName = ps.AdministratorRoleName;
            }
            return VisibilityMode == UserVisibilityMode.AllUsers || (VisibilityMode == UserVisibilityMode.MembersOnly && AccessingUser != null && AccessingUser.UserID != -1) ||
                   (AccessingUser.IsSuperUser || objUser.UserID == AccessingUser.UserID || AccessingUser.IsInRole(strAdministratorRoleName));
        }
    }
}