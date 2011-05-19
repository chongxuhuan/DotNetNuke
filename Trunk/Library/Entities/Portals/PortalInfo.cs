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
using System.Xml.Serialization;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.FileSystem;

#endregion

namespace DotNetNuke.Entities.Portals
{
    [XmlRoot("settings", IsNullable = false)]
    [Serializable]
    public class PortalInfo : BaseEntityInfo, IHydratable
    {
        private string _administratorRoleName;
        private int _pages = Null.NullInteger;
        private string _registeredRoleName;

        #region Constructors

        public PortalInfo()
        {
            Users = Null.NullInteger;
        }

        #endregion

        #region Auto_Properties

        [XmlElement("administratorid")]
        public int AdministratorId { get; set; }

        [XmlElement("administratorroleid")]
        public int AdministratorRoleId { get; set; }

        [XmlElement("admintabid")]
        public int AdminTabId { get; set; }

        [XmlElement("backgroundfile")]
        public string BackgroundFile { get; set; }

        [XmlElement("banneradvertising")]
        public int BannerAdvertising { get; set; }

        [XmlElement("cultureCode")]
        public string CultureCode { get; set; }

        [XmlElement("currency")]
        public string Currency { get; set; }

        [XmlElement("defaultlanguage")]
        public string DefaultLanguage { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("email")]
        public string Email { get; set; }

        [XmlElement("expirydate")]
        public DateTime ExpiryDate { get; set; }

        [XmlElement("footertext")]
        public string FooterText { get; set; }

        [XmlIgnore]
        public Guid GUID { get; set; }

        [XmlElement("homedirectory")]
        public string HomeDirectory { get; set; }

        [XmlElement("hometabid")]
        public int HomeTabId { get; set; }

        [XmlElement("hostfee")]
        public float HostFee { get; set; }

        [XmlElement("hostspace")]
        public int HostSpace { get; set; }

        [XmlElement("keywords")]
        public string KeyWords { get; set; }

        [XmlElement("logintabid")]
        public int LoginTabId { get; set; }

        [XmlElement("logofile")]
        public string LogoFile { get; set; }

        [XmlElement("pagequota")]
        public int PageQuota { get; set; }

        [XmlElement("paymentprocessor")]
        public string PaymentProcessor { get; set; }

        [XmlElement("portalid")]
        public int PortalID { get; set; }

        [XmlElement("portalname")]
        public string PortalName { get; set; }

        [XmlElement("processorpassword")]
        public string ProcessorPassword { get; set; }

        [XmlElement("processoruserid")]
        public string ProcessorUserId { get; set; }

        [XmlElement("registeredroleid")]
        public int RegisteredRoleId { get; set; }

        /// <summary>
        ///   Tabid of the Registration page
        /// </summary>
        /// <value>TabId of the Registration page</value>
        /// <returns>TabId of the Registration page</returns>
        /// <remarks>
        /// </remarks>
        [XmlElement("registertabid")]
        public int RegisterTabId { get; set; }

        /// <summary>
        ///   Tabid of the Search profile page
        /// </summary>
        /// <value>TabdId of the Search Results page</value>
        /// <returns>TabdId of the Search Results page</returns>
        /// <remarks>
        /// </remarks>
        [XmlElement("searchtabid")]
        public int SearchTabId { get; set; }

        [XmlElement("siteloghistory")]
        public int SiteLogHistory { get; set; }

        [XmlElement("splashtabid")]
        public int SplashTabId { get; set; }

        [XmlElement("supertabid")]
        public int SuperTabId { get; set; }

        [XmlElement("userquota")]
        public int UserQuota { get; set; }

        [XmlElement("userregistration")]
        public int UserRegistration { get; set; }

        [XmlElement("usertabid")]
        public int UserTabId { get; set; }

        [XmlElement("users")]
        public int Users { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        #endregion

        #region Properties

        [XmlElement("administratorrolename")]
        public string AdministratorRoleName
        {
            get
            {
                if (_administratorRoleName == Null.NullString && AdministratorRoleId > Null.NullInteger)
                {
					//Get Role Name
                    RoleInfo adminRole = new RoleController().GetRole(AdministratorRoleId, PortalID);
                    if (adminRole != null)
                    {
                        _administratorRoleName = adminRole.RoleName;
                    }
                }
                return _administratorRoleName;
            }
            set
            {
                _administratorRoleName = value;
            }
        }

        [XmlIgnore]
        public string HomeDirectoryMapPath
        {
            get
            {
                return String.Format("{0}\\{1}\\", Globals.ApplicationMapPath, HomeDirectory.Replace("/", "\\"));
            }
        }

        [XmlElement("pages")]
        public int Pages
        {
            get
            {
                if (_pages < 0)
                {
                    var objTabController = new TabController();
                    _pages = objTabController.GetTabCount(PortalID);
                }
                return _pages;
            }
            set
            {
                _pages = value;
            }
        }

        [XmlElement("registeredrolename")]
        public string RegisteredRoleName
        {
            get
            {
                if (_registeredRoleName == Null.NullString && RegisteredRoleId > Null.NullInteger)
                {
                    RoleInfo regUsersRole = new RoleController().GetRole(RegisteredRoleId, PortalID);
                    if (regUsersRole != null)
                    {
                        _registeredRoleName = regUsersRole.RoleName;
                    }
                }
                return _registeredRoleName;
            }
            set
            {
                _registeredRoleName = value;
            }
        }

        #endregion

        [XmlIgnore, Obsolete("Deprecated in DNN 6.0.")]
        public int TimeZoneOffset { get; set; }

        #region IHydratable Members

        public void Fill(IDataReader dr)
        {
            PortalID = Null.SetNullInteger(dr["PortalID"]);
            PortalName = Null.SetNullString(dr["PortalName"]);
            LogoFile = Null.SetNullString(dr["LogoFile"]);
            FooterText = Null.SetNullString(dr["FooterText"]);
            ExpiryDate = Null.SetNullDateTime(dr["ExpiryDate"]);
            UserRegistration = Null.SetNullInteger(dr["UserRegistration"]);
            BannerAdvertising = Null.SetNullInteger(dr["BannerAdvertising"]);
            AdministratorId = Null.SetNullInteger(dr["AdministratorID"]);
            Email = Null.SetNullString(dr["Email"]);
            Currency = Null.SetNullString(dr["Currency"]);
            HostFee = Null.SetNullInteger(dr["HostFee"]);
            HostSpace = Null.SetNullInteger(dr["HostSpace"]);
            PageQuota = Null.SetNullInteger(dr["PageQuota"]);
            UserQuota = Null.SetNullInteger(dr["UserQuota"]);
            AdministratorRoleId = Null.SetNullInteger(dr["AdministratorRoleID"]);
            RegisteredRoleId = Null.SetNullInteger(dr["RegisteredRoleID"]);
            Description = Null.SetNullString(dr["Description"]);
            KeyWords = Null.SetNullString(dr["KeyWords"]);
            BackgroundFile = Null.SetNullString(dr["BackGroundFile"]);
            GUID = new Guid(Null.SetNullString(dr["GUID"]));
            PaymentProcessor = Null.SetNullString(dr["PaymentProcessor"]);
            ProcessorUserId = Null.SetNullString(dr["ProcessorUserId"]);
            ProcessorPassword = Null.SetNullString(dr["ProcessorPassword"]);
            SiteLogHistory = Null.SetNullInteger(dr["SiteLogHistory"]);
            SplashTabId = Null.SetNullInteger(dr["SplashTabID"]);
            HomeTabId = Null.SetNullInteger(dr["HomeTabID"]);
            LoginTabId = Null.SetNullInteger(dr["LoginTabID"]);
            RegisterTabId = Null.SetNullInteger(dr["RegisterTabID"]);
            UserTabId = Null.SetNullInteger(dr["UserTabID"]);
            SearchTabId = Null.SetNullInteger(dr["SearchTabID"]);
            DefaultLanguage = Null.SetNullString(dr["DefaultLanguage"]);
#pragma warning disable 612,618 //needed for upgrades and backwards compatibility
            TimeZoneOffset = Null.SetNullInteger(dr["TimeZoneOffset"]);
#pragma warning restore 612,618
            AdminTabId = Null.SetNullInteger(dr["AdminTabID"]);
            HomeDirectory = Null.SetNullString(dr["HomeDirectory"]);
            SuperTabId = Null.SetNullInteger(dr["SuperTabId"]);
            CultureCode = Null.SetNullString(dr["CultureCode"]);

            FillInternal(dr);
            AdministratorRoleName = Null.NullString;
            RegisteredRoleName = Null.NullString;

            //Aggressively load Users
            Users = UserController.GetUserCountByPortal(PortalID);
            Pages = Null.NullInteger;
        }

        public int KeyID
        {
            get
            {
                return PortalID;
            }
            set
            {
                PortalID = value;
            }
        }

        #endregion
    }
}
