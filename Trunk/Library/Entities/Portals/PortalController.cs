#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
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
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.XPath;

using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;

using ICSharpCode.SharpZipLib.Zip;

using FileInfo = DotNetNuke.Services.FileSystem.FileInfo;

#endregion

namespace DotNetNuke.Entities.Portals
{
	/// <summary>
	/// PoralController provides business layer of poatal.
	/// </summary>
	/// <remarks>
	/// DotNetNuke supports the concept of virtualised sites in a single install. This means that multiple sites, 
	/// each potentially with multiple unique URL's, can exist in one instance of DotNetNuke i.e. one set of files and one database.
	/// </remarks>
    public class PortalController : IPortalController
	{
        #region Private Methods

        private void AddFolderPermissions(int portalId, int folderId)
        {
            var objPortal = GetPortal(portalId);
            FolderPermissionInfo objFolderPermission;
            var folderManager = FolderManager.Instance;
            var folder = folderManager.GetFolder(folderId);
            var objPermissionController = new PermissionController();
            foreach (PermissionInfo objpermission in objPermissionController.GetPermissionByCodeAndKey("SYSTEM_FOLDER", ""))
            {
                objFolderPermission = new FolderPermissionInfo(objpermission)
                {
                    FolderID = folder.FolderID,
                    RoleID = objPortal.AdministratorRoleId,
                    AllowAccess = true
                };

                folder.FolderPermissions.Add(objFolderPermission);
                if (objpermission.PermissionKey == "READ")
                {
                    //add READ permissions to the All Users Role
                    folderManager.AddAllUserReadPermission(folder, objpermission);
                }
            }
            FolderPermissionController.SaveFolderPermissions((FolderInfo)folder);
        }

        private static void CreateDefaultPortalRoles(int portalId, int administratorId, int administratorRoleId, int registeredRoleId, int subscriberRoleId)
        {
            RoleController controller = new RoleController();

            //create required roles if not already created
            if (administratorRoleId == -1)
            {
                administratorRoleId = CreateRole(portalId, "Administrators", "Portal Administrators", 0, 0, "M", 0, 0, "N", false, false);
            }
            if (registeredRoleId == -1)
            {
                registeredRoleId = CreateRole(portalId, "Registered Users", "Registered Users", 0, 0, "M", 0, 0, "N", false, true);
            }
            if (subscriberRoleId == -1)
            {
                subscriberRoleId = CreateRole(portalId, "Subscribers", "A public role for portal subscriptions", 0, 0, "M", 0, 0, "N", true, true);
            }
            controller.AddUserRole(portalId, administratorId, administratorRoleId, Null.NullDate, Null.NullDate);
            controller.AddUserRole(portalId, administratorId, registeredRoleId, Null.NullDate, Null.NullDate);
            controller.AddUserRole(portalId, administratorId, subscriberRoleId, Null.NullDate, Null.NullDate);
        }

        private static string CreateProfileDefinitions(int portalId, string templatePath, string templateFile)
        {
            string strMessage = Null.NullString;
            try
            {
                //add profile definitions
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode node;
                //open the XML template file
                try
                {
                    xmlDoc.Load(templatePath + templateFile);
                }
                catch (Exception ex)
                {
                    DnnLog.Error(ex);
                }
                //parse profile definitions if available
                node = xmlDoc.SelectSingleNode("//portal/profiledefinitions");
                if (node != null)
                {
                    ParseProfileDefinitions(node, portalId);
                }
                else //template does not contain profile definitions ( ie. was created prior to DNN 3.3.0 )
                {
                    ProfileController.AddDefaultDefinitions(portalId);
                }
            }
            catch (Exception ex)
            {
                strMessage = Localization.GetString("CreateProfileDefinitions.Error");
                Exceptions.LogException(ex);
            }
            return strMessage;
        }

        private static int CreatePortal(string portalName, string homeDirectory)
        {
            //add portal
            int PortalId = -1;
            try
            {
                //Use host settings as default values for these parameters
                //This can be overwritten on the portal template
                DateTime datExpiryDate;
                if (Host.Host.DemoPeriod > Null.NullInteger)
                {
                    datExpiryDate = Convert.ToDateTime(Globals.GetMediumDate(DateTime.Now.AddDays(Host.Host.DemoPeriod).ToString()));
                }
                else
                {
                    datExpiryDate = Null.NullDate;
                }
                PortalId = DataProvider.Instance().CreatePortal(portalName,
                                                                Host.Host.HostCurrency,
                                                                datExpiryDate,
                                                                Host.Host.HostFee,
                                                                Host.Host.HostSpace,
                                                                Host.Host.PageQuota,
                                                                Host.Host.UserQuota,
                                                                Host.Host.SiteLogHistory,
                                                                homeDirectory,
                                                                UserController.GetCurrentUserInfo().UserID);
                EventLogController objEventLog = new EventLogController();
                objEventLog.AddLog("PortalName", portalName, GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.PORTAL_CREATED);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            return PortalId;
        }

        private static int CreateRole(RoleInfo role)
        {
            RoleInfo objRoleInfo;
            RoleController objRoleController = new RoleController();
            int roleId = Null.NullInteger;

            //First check if the role exists
            objRoleInfo = objRoleController.GetRoleByName(role.PortalID, role.RoleName);
            if (objRoleInfo == null)
            {
                roleId = objRoleController.AddRole(role);
            }
            else
            {
                roleId = objRoleInfo.RoleID;
            }
            return roleId;
        }

        private static int CreateRole(int portalId, string roleName, string description, float serviceFee, int billingPeriod, string billingFrequency, float trialFee, int trialPeriod, string trialFrequency,
                               bool isPublic, bool isAuto)
        {
            RoleInfo objRoleInfo = new RoleInfo();
            objRoleInfo.PortalID = portalId;
            objRoleInfo.RoleName = roleName;
            objRoleInfo.RoleGroupID = Null.NullInteger;
            objRoleInfo.Description = description;
            objRoleInfo.ServiceFee = Convert.ToSingle(serviceFee < 0 ? 0 : serviceFee);
            objRoleInfo.BillingPeriod = billingPeriod;
            objRoleInfo.BillingFrequency = billingFrequency;
            objRoleInfo.TrialFee = Convert.ToSingle(trialFee < 0 ? 0 : trialFee);
            objRoleInfo.TrialPeriod = trialPeriod;
            objRoleInfo.TrialFrequency = trialFrequency;
            objRoleInfo.IsPublic = isPublic;
            objRoleInfo.AutoAssignment = isAuto;
            return CreateRole(objRoleInfo);
        }

        private static void CreateRoleGroup(RoleGroupInfo roleGroup)
        {
            RoleGroupInfo objRoleGroupInfo;
            RoleController objRoleController = new RoleController();
            int roleGroupId = Null.NullInteger;

            //First check if the role exists
            objRoleGroupInfo = RoleController.GetRoleGroupByName(roleGroup.PortalID, roleGroup.RoleGroupName);

            if (objRoleGroupInfo == null)
            {
                roleGroup.RoleGroupID = RoleController.AddRoleGroup(roleGroup);
            }
            else
            {
                roleGroup.RoleGroupID = objRoleGroupInfo.RoleGroupID;
            }
        }

        private static PortalInfo GetPortalInternal(int portalID, string cultureCode)
        {
            var controller = new PortalController();
            var portal = controller.GetPortalList(cultureCode).Where(p => p.PortalID == portalID).SingleOrDefault();
            if (portal == null)
            {
                portal = CBO.FillObject<PortalInfo>(DataProvider.Instance().GetPortal(portalID, cultureCode));
            }
            return portal;
        }

        private static object GetPortalDefaultLanguageCallBack(CacheItemArgs cacheItemArgs)
        {
            int portalID = (int)cacheItemArgs.ParamList[0];
            return DataProvider.Instance().GetPortalDefaultLanguage(portalID);
        }

        private static object GetPortalDictionaryCallback(CacheItemArgs cacheItemArgs)
        {
            var portalDic = new Dictionary<int, int>();
            if (Host.Host.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
            {
				//get all tabs
                int intField = 0;
                IDataReader dr = DataProvider.Instance().GetTabPaths(Null.NullInteger, Null.NullString);
                try
                {
                    while (dr.Read())
                    {
						//add to dictionary
                        portalDic[Convert.ToInt32(Null.SetNull(dr["TabID"], intField))] = Convert.ToInt32(Null.SetNull(dr["PortalID"], intField));
                    }
                }
                catch (Exception exc)
                {
                    Exceptions.LogException(exc);
                }
                finally
                {
					//close datareader
                    CBO.CloseDataReader(dr, true);
                }
            }
            return portalDic;
        }

        private static object GetPortalSettingsDictionaryCallback(CacheItemArgs cacheItemArgs)
        {
            int portalID = (int)cacheItemArgs.ParamList[0];
            var dicSettings = new Dictionary<string, string>();
            IDataReader dr = DataProvider.Instance().GetPortalSettings(portalID, GetActivePortalLanguage(portalID));
            try
            {
                while (dr.Read())
                {
                    if (!dr.IsDBNull(1))
                    {
                        dicSettings.Add(dr.GetString(0), dr.GetString(1));
                    }
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
            return dicSettings;
        }

        private static void ParseFiles(XmlNodeList nodeFiles, int portalId, FolderInfo objFolder)
        {
            var fileManager = FileManager.Instance;

            foreach (XmlNode node in nodeFiles)
            {
                var fileName = XmlUtils.GetNodeValue(node.CreateNavigator(), "filename");

                //First check if the file exists
                var objInfo = fileManager.GetFile(objFolder, fileName);

                if (objInfo != null) continue;

                objInfo = new FileInfo
                {
                    PortalId = portalId,
                    FileName = fileName,
                    Extension = XmlUtils.GetNodeValue(node.CreateNavigator(), "extension"),
                    Size = XmlUtils.GetNodeValueInt(node, "size"),
                    Width = XmlUtils.GetNodeValueInt(node, "width"),
                    Height = XmlUtils.GetNodeValueInt(node, "height"),
                    ContentType = XmlUtils.GetNodeValue(node.CreateNavigator(), "contenttype"),
                    SHA1Hash = XmlUtils.GetNodeValue(node.CreateNavigator(), "sha1hash"),
                    FolderId = objFolder.FolderID,
                    Folder = objFolder.FolderPath
                };

                //Save new File
                using (var fileContent = fileManager.GetFileContent(objInfo))
                {
                    objInfo.FileId = fileManager.AddFile(objFolder, fileName, fileContent, false).FileId;
                }

                fileManager.UpdateFile(objInfo);
            }
        }

        private static void ParseFolderPermissions(XmlNodeList nodeFolderPermissions, int PortalId, FolderInfo folder)
        {
            PermissionController objPermissionController = new PermissionController();
            RoleController objRoleController = new RoleController();
            int PermissionID = 0;

            //Clear the current folder permissions
            folder.FolderPermissions.Clear();
            foreach (XmlNode xmlFolderPermission in nodeFolderPermissions)
            {
                string PermissionKey = XmlUtils.GetNodeValue(xmlFolderPermission.CreateNavigator(), "permissionkey");
                string PermissionCode = XmlUtils.GetNodeValue(xmlFolderPermission.CreateNavigator(), "permissioncode");
                string RoleName = XmlUtils.GetNodeValue(xmlFolderPermission.CreateNavigator(), "rolename");
                bool AllowAccess = XmlUtils.GetNodeValueBoolean(xmlFolderPermission, "allowaccess");
                foreach (PermissionInfo objPermission in
                    objPermissionController.GetPermissionByCodeAndKey(PermissionCode, PermissionKey))
                {
                    PermissionID = objPermission.PermissionID;
                }
                int RoleID = int.MinValue;
                switch (RoleName)
                {
                    case Globals.glbRoleAllUsersName:
                        RoleID = Convert.ToInt32(Globals.glbRoleAllUsers);
                        break;
                    case Globals.glbRoleUnauthUserName:
                        RoleID = Convert.ToInt32(Globals.glbRoleUnauthUser);
                        break;
                    default:
                        RoleInfo objRole = objRoleController.GetRoleByName(PortalId, RoleName);
                        if (objRole != null)
                        {
                            RoleID = objRole.RoleID;
                        }
                        break;
                }

                //if role was found add, otherwise ignore
                if (RoleID != int.MinValue)
                {
                    FolderPermissionInfo objFolderPermission = new FolderPermissionInfo();
                    objFolderPermission.FolderID = folder.FolderID;
                    objFolderPermission.PermissionID = PermissionID;
                    objFolderPermission.RoleID = RoleID;
                    objFolderPermission.AllowAccess = AllowAccess;
                    folder.FolderPermissions.Add(objFolderPermission);
                }
            }
            FolderPermissionController.SaveFolderPermissions(folder);
        }

        private void ParseFolders(XmlNode nodeFolders, int PortalId)
        {
            IFolderInfo objInfo;
            string folderPath;
            int storageLocation;
            bool isProtected = false;
            var folderManager = FolderManager.Instance;
            var folderMappingController = FolderMappingController.Instance;
            FolderMappingInfo folderMapping = null;

            foreach (XmlNode node in nodeFolders.SelectNodes("//folder"))
            {
                folderPath = XmlUtils.GetNodeValue(node.CreateNavigator(), "folderpath");

                //First check if the folder exists
                objInfo = folderManager.GetFolder(PortalId, folderPath);

                if (objInfo == null)
                {
                    isProtected = PathUtils.Instance.IsDefaultProtectedPath(folderPath);

                    if (isProtected)
                    {
                        //protected folders must use insecure storage
                        folderMapping = folderMappingController.GetDefaultFolderMapping(PortalId);
                    }
                    else
                    {
                        storageLocation = Convert.ToInt32(XmlUtils.GetNodeValue(node, "storagelocation", "0"));

                        switch (storageLocation)
                        {
                            case (int)FolderController.StorageLocationTypes.InsecureFileSystem:
                                folderMapping = folderMappingController.GetDefaultFolderMapping(PortalId);
                                break;
                            case (int)FolderController.StorageLocationTypes.SecureFileSystem:
                                folderMapping = folderMappingController.GetFolderMapping(PortalId, "Secure");
                                break;
                            case (int)FolderController.StorageLocationTypes.DatabaseSecure:
                                folderMapping = folderMappingController.GetFolderMapping(PortalId, "Database");
                                break;
                            default:
                                break;
                        }

                        isProtected = XmlUtils.GetNodeValueBoolean(node, "isprotected");
                    }
                    //Save new folder
                    objInfo = folderManager.AddFolder(folderMapping, folderPath);
                    objInfo.IsProtected = isProtected;

                    folderManager.UpdateFolder(objInfo);
                }

                var nodeFolderPermissions = node.SelectNodes("folderpermissions/permission");
                ParseFolderPermissions(nodeFolderPermissions, PortalId, (FolderInfo)objInfo);

                var nodeFiles = node.SelectNodes("files/file");

                if (!String.IsNullOrEmpty(folderPath))
                {
                    folderPath += "/";
                }

                ParseFiles(nodeFiles, PortalId, (FolderInfo)objInfo);
            }
        }

        private static void ParsePortalDesktopModules(XPathNavigator nav, int portalID)
        {
            string friendlyName = Null.NullString;
            DesktopModuleInfo desktopModule = null;
            foreach (XPathNavigator desktopModuleNav in nav.Select("portalDesktopModule"))
            {
                friendlyName = XmlUtils.GetNodeValue(desktopModuleNav, "friendlyname");
                if (!string.IsNullOrEmpty(friendlyName))
                {
                    desktopModule = DesktopModuleController.GetDesktopModuleByFriendlyName(friendlyName);
                    if (desktopModule != null)
                    {
                        //Parse the permissions
                        DesktopModulePermissionCollection permissions = new DesktopModulePermissionCollection();
                        foreach (XPathNavigator permissionNav in
                            desktopModuleNav.Select("portalDesktopModulePermissions/portalDesktopModulePermission"))
                        {
                            string code = XmlUtils.GetNodeValue(permissionNav, "permissioncode");
                            string key = XmlUtils.GetNodeValue(permissionNav, "permissionkey");
                            DesktopModulePermissionInfo desktopModulePermission = null;
                            ArrayList arrPermissions = new PermissionController().GetPermissionByCodeAndKey(code, key);
                            if (arrPermissions.Count > 0)
                            {
                                PermissionInfo permission = arrPermissions[0] as PermissionInfo;
                                if (permission != null)
                                {
                                    desktopModulePermission = new DesktopModulePermissionInfo(permission);
                                }
                            }
                            desktopModulePermission.AllowAccess = bool.Parse(XmlUtils.GetNodeValue(permissionNav, "allowaccess"));
                            string rolename = XmlUtils.GetNodeValue(permissionNav, "rolename");
                            if (!string.IsNullOrEmpty(rolename))
                            {
                                RoleInfo role = new RoleController().GetRoleByName(portalID, rolename);
                                if (role != null)
                                {
                                    desktopModulePermission.RoleID = role.RoleID;
                                }
                            }
                            permissions.Add(desktopModulePermission);
                        }
                        DesktopModuleController.AddDesktopModuleToPortal(portalID, desktopModule, permissions, false);
                    }
                }
            }
        }

        private void ParsePortalSettings(XmlNode nodeSettings, int PortalId)
        {
            PortalInfo objPortal;
            objPortal = GetPortal(PortalId);
            objPortal.LogoFile = Globals.ImportFile(PortalId, XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "logofile"));
            objPortal.FooterText = XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "footertext");
            if (nodeSettings.SelectSingleNode("expirydate") != null)
            {
                objPortal.ExpiryDate = XmlUtils.GetNodeValueDate(nodeSettings, "expirydate", Null.NullDate);
            }
            objPortal.UserRegistration = XmlUtils.GetNodeValueInt(nodeSettings, "userregistration");
            objPortal.BannerAdvertising = XmlUtils.GetNodeValueInt(nodeSettings, "banneradvertising");
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "currency")))
            {
                objPortal.Currency = XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "currency");
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "hostfee")))
            {
                objPortal.HostFee = XmlUtils.GetNodeValueSingle(nodeSettings, "hostfee");
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "hostspace")))
            {
                objPortal.HostSpace = XmlUtils.GetNodeValueInt(nodeSettings, "hostspace");
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "pagequota")))
            {
                objPortal.PageQuota = XmlUtils.GetNodeValueInt(nodeSettings, "pagequota");
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "userquota")))
            {
                objPortal.UserQuota = XmlUtils.GetNodeValueInt(nodeSettings, "userquota");
            }
            objPortal.BackgroundFile = XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "backgroundfile");
            objPortal.PaymentProcessor = XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "paymentprocessor");
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "siteloghistory")))
            {
                objPortal.SiteLogHistory = XmlUtils.GetNodeValueInt(nodeSettings, "siteloghistory");
            }
            objPortal.DefaultLanguage = XmlUtils.GetNodeValue(nodeSettings, "defaultlanguage", "en-US");
            UpdatePortalInfo(objPortal);

            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "skinsrc", "")))
            {
                UpdatePortalSetting(PortalId, "DefaultPortalSkin", XmlUtils.GetNodeValue(nodeSettings, "skinsrc", ""));
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "skinsrcadmin", "")))
            {
                UpdatePortalSetting(PortalId, "DefaultAdminSkin", XmlUtils.GetNodeValue(nodeSettings, "skinsrcadmin", ""));
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "containersrc", "")))
            {
                UpdatePortalSetting(PortalId, "DefaultPortalContainer", XmlUtils.GetNodeValue(nodeSettings, "containersrc", ""));
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "containersrcadmin", "")))
            {
                UpdatePortalSetting(PortalId, "DefaultAdminContainer", XmlUtils.GetNodeValue(nodeSettings, "containersrcadmin", ""));
            }

            //Enable Skin Widgets Setting
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "enableskinwidgets", "")))
            {
                UpdatePortalSetting(PortalId, "EnableSkinWidgets", XmlUtils.GetNodeValue(nodeSettings, "enableskinwidgets", ""));
            }
            //Set Auto alias mapping

            if (!string.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "portalaliasmapping", "CANONICALURL")))
            {
                UpdatePortalSetting(PortalId, "PortalAliasMapping", XmlUtils.GetNodeValue(nodeSettings, "portalaliasmapping", "CANONICALURL").ToUpperInvariant());
            }
            //Set Time Zone maping
            if (!string.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "timezone", Localization.SystemTimeZone)))
            {
                UpdatePortalSetting(PortalId, "TimeZone", XmlUtils.GetNodeValue(nodeSettings, "timezone", Localization.SystemTimeZone));
            }

            if (!string.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "contentlocalizationenabled")))
            {
                UpdatePortalSetting(PortalId, "ContentLocalizationEnabled", XmlUtils.GetNodeValue(nodeSettings, "contentlocalizationenabled"));
            }
        }

        private static void ParseProfileDefinitions(XmlNode nodeProfileDefinitions, int PortalId)
        {
            var listController = new ListController();
            Dictionary<string, ListEntryInfo> colDataTypes = listController.GetListEntryInfoDictionary("DataType");

            int orderCounter = -1;
            ProfilePropertyDefinition objProfileDefinition;
            bool preferredTimeZoneFound = false;
            foreach (XmlNode node in nodeProfileDefinitions.SelectNodes("//profiledefinition"))
            {
                orderCounter += 2;
                ListEntryInfo typeInfo = colDataTypes["DataType:" + XmlUtils.GetNodeValue(node.CreateNavigator(), "datatype")];
                if (typeInfo == null)
                {
                    typeInfo = colDataTypes["DataType:Unknown"];
                }
                objProfileDefinition = new ProfilePropertyDefinition(PortalId);
                objProfileDefinition.DataType = typeInfo.EntryID;
                objProfileDefinition.DefaultValue = "";
                objProfileDefinition.ModuleDefId = Null.NullInteger;
                objProfileDefinition.PropertyCategory = XmlUtils.GetNodeValue(node.CreateNavigator(), "propertycategory");
                objProfileDefinition.PropertyName = XmlUtils.GetNodeValue(node.CreateNavigator(), "propertyname");
                objProfileDefinition.Required = false;
                objProfileDefinition.Visible = true;
                objProfileDefinition.ViewOrder = orderCounter;
                objProfileDefinition.Length = XmlUtils.GetNodeValueInt(node, "length");

                switch (XmlUtils.GetNodeValueInt(node, "defaultvisibility", 2))
                {
                    case 0:
                        objProfileDefinition.DefaultVisibility = UserVisibilityMode.AllUsers;
                        break;
                    case 1:
                        objProfileDefinition.DefaultVisibility = UserVisibilityMode.MembersOnly;
                        break;
                    case 2:
                        objProfileDefinition.DefaultVisibility = UserVisibilityMode.AdminOnly;
                        break;
                }

                if (objProfileDefinition.PropertyName == "PreferredTimeZone")
                {
                    preferredTimeZoneFound = true;
                }

                ProfileController.AddPropertyDefinition(objProfileDefinition);
            }

            //6.0 requires the old TimeZone property to be marked as Deleted
            ProfilePropertyDefinition pdf = ProfileController.GetPropertyDefinitionByName(PortalId, "TimeZone");
            if (pdf != null)
            {
                ProfileController.DeletePropertyDefinition(pdf);
            }

            // 6.0 introduced a new property called as PreferredTimeZone. If this property is not present in template
            // it should be added. Situation will mostly happen while using an older template file.
            if (!preferredTimeZoneFound)
            {
                orderCounter += 2;

                ListEntryInfo typeInfo = colDataTypes["DataType:TimeZoneInfo"];
                if (typeInfo == null)
                {
                    typeInfo = colDataTypes["DataType:Unknown"];
                }

                objProfileDefinition = new ProfilePropertyDefinition(PortalId);
                objProfileDefinition.DataType = typeInfo.EntryID;
                objProfileDefinition.DefaultValue = "";
                objProfileDefinition.ModuleDefId = Null.NullInteger;
                objProfileDefinition.PropertyCategory = "Preferences";
                objProfileDefinition.PropertyName = "PreferredTimeZone";
                objProfileDefinition.Required = false;
                objProfileDefinition.Visible = true;
                objProfileDefinition.ViewOrder = orderCounter;
                objProfileDefinition.Length = 0;
                objProfileDefinition.DefaultVisibility = UserVisibilityMode.AdminOnly;
                ProfileController.AddPropertyDefinition(objProfileDefinition);
            }
        }

        private void ParseRoleGroups(XPathNavigator nav, int portalID, int administratorId)
        {
            int administratorRoleId = -1;
            int registeredRoleId = -1;
            int subscriberRoleId = -1;
            RoleController controller = new RoleController();
            foreach (XPathNavigator roleGroupNav in nav.Select("rolegroup"))
            {
                RoleGroupInfo roleGroup = CBO.DeserializeObject<RoleGroupInfo>(new StringReader(roleGroupNav.OuterXml));
                if (roleGroup.RoleGroupName != "GlobalRoles")
                {
                    roleGroup.PortalID = portalID;
                    CreateRoleGroup(roleGroup);
                }
                foreach (RoleInfo role in roleGroup.Roles.Values)
                {
                    role.PortalID = portalID;
                    role.RoleGroupID = roleGroup.RoleGroupID;
                    switch (role.RoleType)
                    {
                        case RoleType.Administrator:
                            administratorRoleId = CreateRole(role);
                            break;
                        case RoleType.RegisteredUser:
                            registeredRoleId = CreateRole(role);
                            break;
                        case RoleType.Subscriber:
                            subscriberRoleId = CreateRole(role);
                            break;
                        case RoleType.None:
                            CreateRole(role);
                            break;
                    }
                }
            }
            CreateDefaultPortalRoles(portalID, administratorId, administratorRoleId, registeredRoleId, subscriberRoleId);

            //update portal setup
            PortalInfo objportal;
            objportal = GetPortal(portalID);
            UpdatePortalSetup(portalID,
                              administratorId,
                              administratorRoleId,
                              registeredRoleId,
                              objportal.SplashTabId,
                              objportal.HomeTabId,
                              objportal.LoginTabId,
                              objportal.RegisterTabId,
                              objportal.UserTabId,
                              objportal.SearchTabId,
                              objportal.AdminTabId,
                              GetActivePortalLanguage(portalID));
        }

        private void ParseRoles(XPathNavigator nav, int portalID, int administratorId)
        {
            int administratorRoleId = -1;
            int registeredRoleId = -1;
            int subscriberRoleId = -1;
            RoleController controller = new RoleController();
            foreach (XPathNavigator roleNav in nav.Select("role"))
            {
                RoleInfo role = CBO.DeserializeObject<RoleInfo>(new StringReader(roleNav.OuterXml));
                role.PortalID = portalID;
                role.RoleGroupID = Null.NullInteger;
                switch (role.RoleType)
                {
                    case RoleType.Administrator:
                        administratorRoleId = CreateRole(role);
                        break;
                    case RoleType.RegisteredUser:
                        registeredRoleId = CreateRole(role);
                        break;
                    case RoleType.Subscriber:
                        subscriberRoleId = CreateRole(role);
                        break;
                    case RoleType.None:
                        CreateRole(role);
                        break;
                }
            }

            //create required roles if not already created
            CreateDefaultPortalRoles(portalID, administratorId, administratorRoleId, registeredRoleId, subscriberRoleId);

            //update portal setup
            PortalInfo objportal;
            objportal = GetPortal(portalID);
            UpdatePortalSetup(portalID,
                              administratorId,
                              administratorRoleId,
                              registeredRoleId,
                              objportal.SplashTabId,
                              objportal.HomeTabId,
                              objportal.LoginTabId,
                              objportal.RegisterTabId,
                              objportal.UserTabId,
                              objportal.SearchTabId,
                              objportal.AdminTabId,
                              GetActivePortalLanguage(portalID));
        }

        private void ParseTab(XmlNode nodeTab, int PortalId, bool IsAdminTemplate, PortalTemplateModuleAction mergeTabs, ref Hashtable hModules, ref Hashtable hTabs, bool IsNewPortal)
        {
            TabInfo objTab = null;
            TabController objTabs = new TabController();
            string strName = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "name");
            PortalInfo objportal = GetPortal(PortalId);
            if (!String.IsNullOrEmpty(strName))
            {
                if (!IsNewPortal)  //running from wizard: try to find the tab by path
                {
                    string parenttabname = "";
                    if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent")))
                    {
                        parenttabname = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent") + "/";
                    }
                    if (hTabs[parenttabname + strName] != null)
                    {
                        objTab = objTabs.GetTab(Convert.ToInt32(hTabs[parenttabname + strName]), PortalId, false);
                    }
                }
                if (objTab == null || IsNewPortal)
                {
                    objTab = TabController.DeserializeTab(nodeTab, null, hTabs, PortalId, IsAdminTemplate, mergeTabs, hModules);
                }
                EventLogController objEventLog = new EventLogController();

                //when processing the template we should try and identify the Admin tab
                if (objTab.TabName == "Admin")
                {
                    objportal.AdminTabId = objTab.TabID;
                    UpdatePortalSetup(PortalId,
                                      objportal.AdministratorId,
                                      objportal.AdministratorRoleId,
                                      objportal.RegisteredRoleId,
                                      objportal.SplashTabId,
                                      objportal.HomeTabId,
                                      objportal.LoginTabId,
                                      objportal.RegisterTabId,
                                      objportal.UserTabId,
                                      objportal.SearchTabId,
                                      objportal.AdminTabId,
                                      GetActivePortalLanguage(PortalId));
                    objEventLog.AddLog("AdminTab",
                                       objTab.TabID.ToString(),
                                       GetCurrentPortalSettings(),
                                       UserController.GetCurrentUserInfo().UserID,
                                       EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
                }
                //when processing the template we can find: hometab, usertab, logintab
                switch (XmlUtils.GetNodeValue(nodeTab, "tabtype", ""))
                {
                    case "splashtab":
                        objportal.SplashTabId = objTab.TabID;
                        UpdatePortalSetup(PortalId,
                                          objportal.AdministratorId,
                                          objportal.AdministratorRoleId,
                                          objportal.RegisteredRoleId,
                                          objportal.SplashTabId,
                                          objportal.HomeTabId,
                                          objportal.LoginTabId,
                                          objportal.RegisterTabId,
                                          objportal.UserTabId,
                                          objportal.SearchTabId,
                                          objportal.AdminTabId,
                                          GetActivePortalLanguage(PortalId));
                        objEventLog.AddLog("SplashTab",
                                           objTab.TabID.ToString(),
                                           GetCurrentPortalSettings(),
                                           UserController.GetCurrentUserInfo().UserID,
                                           EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
                        break;
                    case "hometab":
                        objportal.HomeTabId = objTab.TabID;
                        UpdatePortalSetup(PortalId,
                                          objportal.AdministratorId,
                                          objportal.AdministratorRoleId,
                                          objportal.RegisteredRoleId,
                                          objportal.SplashTabId,
                                          objportal.HomeTabId,
                                          objportal.LoginTabId,
                                          objportal.RegisterTabId,
                                          objportal.UserTabId,
                                          objportal.SearchTabId,
                                          objportal.AdminTabId,
                                          GetActivePortalLanguage(PortalId));
                        objEventLog.AddLog("HomeTab",
                                           objTab.TabID.ToString(),
                                           GetCurrentPortalSettings(),
                                           UserController.GetCurrentUserInfo().UserID,
                                           EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
                        break;
                    case "logintab":
                        objportal.LoginTabId = objTab.TabID;
                        UpdatePortalSetup(PortalId,
                                          objportal.AdministratorId,
                                          objportal.AdministratorRoleId,
                                          objportal.RegisteredRoleId,
                                          objportal.SplashTabId,
                                          objportal.HomeTabId,
                                          objportal.LoginTabId,
                                          objportal.RegisterTabId,
                                          objportal.UserTabId,
                                          objportal.SearchTabId,
                                          objportal.AdminTabId,
                                          GetActivePortalLanguage(PortalId));
                        objEventLog.AddLog("LoginTab",
                                           objTab.TabID.ToString(),
                                           GetCurrentPortalSettings(),
                                           UserController.GetCurrentUserInfo().UserID,
                                           EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
                        break;
                    case "usertab":
                        objportal.UserTabId = objTab.TabID;
                        UpdatePortalSetup(PortalId,
                                          objportal.AdministratorId,
                                          objportal.AdministratorRoleId,
                                          objportal.RegisteredRoleId,
                                          objportal.SplashTabId,
                                          objportal.HomeTabId,
                                          objportal.LoginTabId,
                                          objportal.RegisterTabId,
                                          objportal.UserTabId,
                                          objportal.SearchTabId,
                                          objportal.AdminTabId,
                                          GetActivePortalLanguage(PortalId));
                        objEventLog.AddLog("UserTab",
                                           objTab.TabID.ToString(),
                                           GetCurrentPortalSettings(),
                                           UserController.GetCurrentUserInfo().UserID,
                                           EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
                        break;
                    case "searchtab":
                        objportal.SearchTabId = objTab.TabID;
                        UpdatePortalSetup(PortalId,
                                          objportal.AdministratorId,
                                          objportal.AdministratorRoleId,
                                          objportal.RegisteredRoleId,
                                          objportal.SplashTabId,
                                          objportal.HomeTabId,
                                          objportal.LoginTabId,
                                          objportal.RegisterTabId,
                                          objportal.UserTabId,
                                          objportal.SearchTabId,
                                          objportal.AdminTabId,
                                          GetActivePortalLanguage(PortalId));
                        objEventLog.AddLog("SearchTab",
                                           objTab.TabID.ToString(),
                                           GetCurrentPortalSettings(),
                                           UserController.GetCurrentUserInfo().UserID,
                                           EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
                        break;
                }
            }
        }

        private void ParseTabs(XmlNode nodeTabs, int PortalId, bool IsAdminTemplate, PortalTemplateModuleAction mergeTabs, bool IsNewPortal)
        {
            //used to control if modules are true modules or instances
            //will hold module ID from template / new module ID so new instances can reference right moduleid
            //only first one from the template will be create as a true module, 
            //others will be moduleinstances (tabmodules)
            Hashtable hModules = new Hashtable();
            Hashtable hTabs = new Hashtable();

            //if running from wizard we need to pre populate htabs with existing tabs so ParseTab 
            //can find all existing ones
            string tabname;
            if (!IsNewPortal)
            {
                Hashtable hTabNames = new Hashtable();
                TabController objTabs = new TabController();
                foreach (KeyValuePair<int, TabInfo> tabPair in objTabs.GetTabsByPortal(PortalId))
                {
                    TabInfo objTab = tabPair.Value;
                    if (!objTab.IsDeleted)
                    {
                        tabname = objTab.TabName;
                        if (!Null.IsNull(objTab.ParentId))
                        {
                            tabname = Convert.ToString(hTabNames[objTab.ParentId]) + "/" + objTab.TabName;
                        }
                        hTabNames.Add(objTab.TabID, tabname);
                    }
                }

                //when parsing tabs we will need tabid given tabname
                foreach (int i in hTabNames.Keys)
                {
                    if (hTabs[hTabNames[i]] == null)
                    {
                        hTabs.Add(hTabNames[i], i);
                    }
                }
                hTabNames = null;
            }
            foreach (XmlNode nodeTab in nodeTabs.SelectNodes("//tab"))
            {
                ParseTab(nodeTab, PortalId, IsAdminTemplate, mergeTabs, ref hModules, ref hTabs, IsNewPortal);
            }

            //Process tabs that are linked to tabs
            foreach (XmlNode nodeTab in nodeTabs.SelectNodes("//tab[url/@type = 'Tab']"))
            {
                int tabId = XmlUtils.GetNodeValueInt(nodeTab, "tabid", Null.NullInteger);
                string tabPath = XmlUtils.GetNodeValue(nodeTab, "url", Null.NullString);
                if (tabId > Null.NullInteger)
                {
                    TabController controller = new TabController();
                    TabInfo objTab = controller.GetTab(tabId, PortalId, false);
                    objTab.Url = TabController.GetTabByTabPath(PortalId, tabPath, Null.NullString).ToString();
                    controller.UpdateTab(objTab);
                }
            }
            var folderManager = FolderManager.Instance;
            var fileManager = FileManager.Instance;
            //Process tabs that are linked to files
            foreach (XmlNode nodeTab in nodeTabs.SelectNodes("//tab[url/@type = 'File']"))
            {
                var tabId = XmlUtils.GetNodeValueInt(nodeTab, "tabid", Null.NullInteger);
                var filePath = XmlUtils.GetNodeValue(nodeTab, "url", Null.NullString);
                if (tabId > Null.NullInteger)
                {
                    var controller = new TabController();
                    var objTab = controller.GetTab(tabId, PortalId, false);

                    var fileName = Path.GetFileName(filePath);

                    var folderPath = filePath.Substring(0, filePath.LastIndexOf(fileName));
                    var folder = folderManager.GetFolder(PortalId, folderPath);

                    var file = fileManager.GetFile(folder, fileName);

                    objTab.Url = "FileID=" + file.FileId;
                    controller.UpdateTab(objTab);
                }
            }
        }

        private void UpdatePortalSetup(int PortalId, int AdministratorId, int AdministratorRoleId, int RegisteredRoleId, int SplashTabId, int HomeTabId, int LoginTabId, int RegisterTabId,
                                       int UserTabId, int SearchTabId, int AdminTabId, string CultureCode)
        {
            DataProvider.Instance().UpdatePortalSetup(PortalId,
                                                      AdministratorId,
                                                      AdministratorRoleId,
                                                      RegisteredRoleId,
                                                      SplashTabId,
                                                      HomeTabId,
                                                      LoginTabId,
                                                      RegisterTabId,
                                                      UserTabId,
                                                      SearchTabId,
                                                      AdminTabId,
                                                      CultureCode);
            EventLogController objEventLog = new EventLogController();
            objEventLog.AddLog("PortalId", PortalId.ToString(), GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.PORTALINFO_UPDATED);
            DataCache.ClearHostCache(true);
        }

        #endregion

	    /// <summary>
        /// Loads the portal settings when not provided by "normal" DNN mechanisms
        /// </summary>
        /// <remarks>This method is for very special situations where the normal DNN page lifecycle does not run.
        /// Unless you know specifically why your are calling this method, you should use PortalController.GetCurrentPortalSettings
        /// </remarks>
        /// <param name="request">The current request</param> 
        /// <returns>PortaSettings</returns>
        public PortalSettings LoadPortalSettingsWhenOtherwiseUnavailable(HttpRequestBase request)
        {
            var domainName = Globals.GetDomainName(request);
            var alias = PortalAliasController.GetPortalAliasInfo(domainName);

            return new PortalSettings(-1, alias);
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Creates a new portal alias
        /// </summary>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="portalAlias">Portal Alias to be created</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]    01/11/2005  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void AddPortalAlias(int portalId, string portalAlias)
        {
            PortalAliasController portalAliasController = new PortalAliasController();

            //Check if the Alias exists
            PortalAliasInfo portalAliasInfo = portalAliasController.GetPortalAlias(portalAlias, portalId);

            //If alias does not exist add new
            if (portalAliasInfo == null)
            {
                portalAliasInfo = new PortalAliasInfo();
                portalAliasInfo.PortalID = portalId;
                portalAliasInfo.HTTPAlias = portalAlias;
                portalAliasController.AddPortalAlias(portalAliasInfo);
            }
        }

        /// <summary>
        /// Copies the page template.
        /// </summary>
        /// <param name="templateFile">The template file.</param>
        /// <param name="mappedHomeDirectory">The mapped home directory.</param>
        public void CopyPageTemplate(string templateFile, string mappedHomeDirectory)
        {
            string hostTemplateFile = string.Format("{0}Templates\\{1}", Globals.HostMapPath, templateFile);
            if (File.Exists(hostTemplateFile))
            {
                string portalTemplateFolder = string.Format("{0}Templates\\", mappedHomeDirectory);
                if (!Directory.Exists(portalTemplateFolder))
                {
                    //Create Portal Templates folder
                    Directory.CreateDirectory(portalTemplateFolder);
                }
                string portalTemplateFile = portalTemplateFolder + templateFile;
                if (!File.Exists(portalTemplateFile))
                {
                    File.Copy(hostTemplateFile, portalTemplateFile);
                }
            }
        }

        /// <summary>
        /// Creates the portal.
        /// </summary>
        /// <param name="portalName">Name of the portal.</param>
        /// <param name="adminUser">The obj admin user.</param>
        /// <param name="description">The description.</param>
        /// <param name="keyWords">The key words.</param>
        /// <param name="templatePath">The template path.</param>
        /// <param name="templateFile">The template file.</param>
        /// <param name="homeDirectory">The home directory.</param>
        /// <param name="portalAlias">The portal alias.</param>
        /// <param name="serverPath">The server path.</param>
        /// <param name="childPath">The child path.</param>
        /// <param name="isChildPortal">if set to <c>true</c> means the portal is child portal.</param>
        /// <returns>Portal id.</returns>
        public int CreatePortal(string portalName, UserInfo adminUser, string description, string keyWords, string templatePath, 
                                string templateFile, string homeDirectory, string portalAlias,
                                string serverPath, string childPath, bool isChildPortal)
        {
            string message = Null.NullString;
            int administratorId = Null.NullInteger;

            //Attempt to create a new portal
            int portalId = CreatePortal(portalName, homeDirectory);

            //Add Languages to Portal 
            Localization.AddLanguagesToPortal(portalId);

            if (portalId != -1)
            {
                if (String.IsNullOrEmpty(homeDirectory))
                {
                    homeDirectory = "Portals/" + portalId;
                }
                string mappedHomeDirectory = String.Format(Globals.ApplicationMapPath + "\\" + homeDirectory + "\\").Replace("/", "\\");
                message += CreateProfileDefinitions(portalId, templatePath, templateFile);
                if (message == Null.NullString)
                {
                    //add administrator
                    try
                    {
                        adminUser.PortalID = portalId;
                        UserCreateStatus createStatus = UserController.CreateUser(ref adminUser);
                        if (createStatus == UserCreateStatus.Success)
                        {
                            administratorId = adminUser.UserID;
                            //reload the UserInfo as when it was first created, it had no portal id and therefore
                            //used host profile definitions
                            adminUser = UserController.GetUserById(adminUser.PortalID, adminUser.UserID);
                        }
                        else
                        {
                            message += UserController.GetUserCreateStatus(createStatus);
                        }
                    }
                    catch (Exception Exc)
                    {
                        DnnLog.Error(Exc);
                        message += Localization.GetString("CreateAdminUser.Error") + Exc.Message + Exc.StackTrace;
                    }
                }
                else
                {
                    throw new Exception(message);
                }
                if (String.IsNullOrEmpty(message) && administratorId > 0)
                {
                    try
                    {
                        //the upload directory may already exist if this is a new DB working with a previously installed application
                        if (Directory.Exists(mappedHomeDirectory))
                        {
                            Globals.DeleteFolderRecursive(mappedHomeDirectory);
                        }
                    }
                    catch (Exception Exc)
                    {
                        DnnLog.Error(Exc);
                        message += Localization.GetString("DeleteUploadFolder.Error") + Exc.Message + Exc.StackTrace;
                    }

                    //Set up Child Portal
                    if (message == Null.NullString)
                    {
                        if (isChildPortal)
                        {
                            message = CreateChildPortalFolder(childPath);
                        }
                    }
                    else
                    {
                        throw new Exception(message);
                    }
                    if (message == Null.NullString)
                    {
                        try
                        {
                            //create the upload directory for the new portal
                            Directory.CreateDirectory(mappedHomeDirectory);
                            //ensure that the Templates folder exists
                            string templateFolder = String.Format("{0}Templates", mappedHomeDirectory);
                            if (!Directory.Exists(templateFolder))
                            {
                                Directory.CreateDirectory(templateFolder);
                            }

                            //ensure that the Users folder exists
                            string usersFolder = String.Format("{0}Users", mappedHomeDirectory);
                            if (!Directory.Exists(usersFolder))
                            {
                                Directory.CreateDirectory(usersFolder);
                            }

                            //copy the default page template
                            CopyPageTemplate("Default.page.template", mappedHomeDirectory);

                            // process zip resource file if present
                            if (File.Exists(templatePath + templateFile + ".resources"))
                            {
                                ProcessResourceFile(mappedHomeDirectory, templatePath + templateFile);
                            }
                        }
                        catch (Exception Exc)
                        {
                            DnnLog.Error(Exc);
                            message += Localization.GetString("ChildPortal.Error") + Exc.Message + Exc.StackTrace;
                        }
                    }
                    else
                    {
                        throw new Exception(message);
                    }
                    if (message == Null.NullString)
                    {
                        try
                        {
                            FolderMappingController.Instance.AddDefaultFolderTypes(portalId);
                        }
                        catch (Exception Exc)
                        {
                            DnnLog.Error(Exc);
                            message += Localization.GetString("DefaultFolderMappings.Error") + Exc.Message + Exc.StackTrace;
                        }
                    }
                    else
                    {
                        throw new Exception(message);
                    }
                    if (message == Null.NullString)
                    {
                        try
                        {
                            ParseTemplate(portalId, templatePath, templateFile, administratorId, PortalTemplateModuleAction.Replace, true);
                        }
                        catch (Exception Exc)
                        {
                            DnnLog.Error(Exc);
                            message += Localization.GetString("PortalTemplate.Error") + Exc.Message + Exc.StackTrace;
                        }
                    }
                    else
                    {
                        throw new Exception(message);
                    }
                    if (message == Null.NullString)
                    {
                        var portal = GetPortal(portalId);
						portal.Description = description;
						portal.KeyWords = keyWords;
						portal.UserTabId = TabController.GetTabByTabPath(portal.PortalID, "//UserProfile", portal.CultureCode);
						portal.SearchTabId = TabController.GetTabByTabPath(portal.PortalID, "//SearchResults", portal.CultureCode);
						UpdatePortalInfo(portal);
						adminUser.Profile.PreferredLocale = portal.DefaultLanguage;
						var portalSettings = new PortalSettings(portal);
						adminUser.Profile.PreferredTimeZone = portalSettings.TimeZone;
						UserController.UpdateUser(portal.PortalID, adminUser);
                        DesktopModuleController.AddDesktopModulesToPortal(portalId);
                        AddPortalAlias(portalId, portalAlias);
                        UpdatePortalSetting(portalId, "DefaultPortalAlias", portalAlias, true);

                        try
                        {
                            //create default portal relationships
                            var relationshipController = new RelationshipController();
                            relationshipController.CreateDefaultRelationshipsForPortal(portalId);
                        }
                        catch (Exception Exc)
                        {
                            DnnLog.Error(Exc);                            
                        }

                        try
                        {
                            var objEventLogInfo = new LogInfo();
                            objEventLogInfo.BypassBuffering = true;
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString();
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("Install Portal:", portalName));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("FirstName:", adminUser.FirstName));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("LastName:", adminUser.LastName));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("Username:", adminUser.Username));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("Email:", adminUser.Email));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("Description:", description));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("Keywords:", keyWords));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("TemplatePath:", templatePath));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("TemplateFile:", templateFile));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("HomeDirectory:", homeDirectory));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("PortalAlias:", portalAlias));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("ServerPath:", serverPath));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("ChildPath:", childPath));
                            objEventLogInfo.LogProperties.Add(new LogDetailInfo("IsChildPortal:", isChildPortal.ToString()));
                            var eventLog = new EventLogController();
                            eventLog.AddLog(objEventLogInfo);
                        }
                        catch (Exception exc)
                        {
                            DnnLog.Error(exc);
                        }
                    }
                    else
                    {
                        throw new Exception(message);
                    }
                }
                else
                {
                    DeletePortalInfo(portalId);
                    portalId = -1;
                    throw new Exception(message);
                }
            }
            else
            {
                message += Localization.GetString("CreatePortal.Error");
                throw new Exception(message);
            }
            return portalId;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Creates a new portal.
        /// </summary>
        /// <param name="portalName">Name of the portal to be created</param>
        /// <param name="firstName">Portal Administrator's first name</param>
        /// <param name="lastName">Portal Administrator's last name</param>
        /// <param name="username">Portal Administrator's username</param>
        /// <param name="password">Portal Administrator's password</param>
        /// <param name="email">Portal Administrator's email</param>
        /// <param name="description">Description for the new portal</param>
        /// <param name="keyWords">KeyWords for the new portal</param>
        /// <param name="templatePath">Path where the templates are stored</param>
        /// <param name="templateFile">Template file</param>
        /// <param name="homeDirectory">Home Directory</param>
        /// <param name="portalAlias">Portal Alias String</param>
        /// <param name="serverPath">The Path to the root of the Application</param>
        /// <param name="childPath">The Path to the Child Portal Folder</param>
        /// <param name="isChildPortal">True if this is a child portal</param>
        /// <returns>PortalId of the new portal if there are no errors, -1 otherwise.</returns>
        /// <remarks>
        /// After the selected portal template is parsed the admin template ("admin.template") will be
        /// also processed. The admin template should only contain the "Admin" menu since it's the same
        /// on all portals. The selected portal template can contain a <settings/> node to specify portal
        /// properties and a <roles/> node to define the roles that will be created on the portal by default.
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/08/2004	created (most of this code was moved from SignUp.ascx.vb)
        /// </history>
        /// -----------------------------------------------------------------------------
        public int CreatePortal(string portalName, string firstName, string lastName, string username, string password, string email, 
                                string description, string keyWords, string templatePath, string templateFile, string homeDirectory, 
                                string portalAlias, string serverPath, string childPath, bool isChildPortal)
        {
            UserInfo adminUser = new UserInfo();
            adminUser.FirstName = firstName;
            adminUser.LastName = lastName;
            adminUser.Username = username;
            adminUser.DisplayName = firstName + " " + lastName;
            adminUser.Membership.Password = password;
            adminUser.Email = email;
            adminUser.IsSuperUser = false;
            adminUser.Membership.Approved = true;
            adminUser.Profile.FirstName = firstName;
            adminUser.Profile.LastName = lastName;
            return CreatePortal(portalName, adminUser, description, keyWords, templatePath, templateFile, homeDirectory, portalAlias, serverPath, childPath, isChildPortal);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deletes a portal permanently
        /// </summary>
        /// <param name="portalId">PortalId of the portal to be deleted</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	03/09/2004	Created
        /// 	[VMasanas]	26/10/2004	Remove dependent data (skins, modules)
        ///     [cnurse]    24/11/2006  Removal of Modules moved to sproc
        /// </history>
        /// -----------------------------------------------------------------------------
        public void DeletePortalInfo(int portalId)
        {
            UserController.DeleteUsers(portalId, false, true);
            DataProvider.Instance().DeletePortalInfo(portalId);
            EventLogController eventLogController = new EventLogController();
            eventLogController.AddLog("PortalId", portalId.ToString(), GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.PORTALINFO_DELETED);
            DataCache.ClearHostCache(true);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets information of a portal
        /// </summary>
        /// <param name = "portalId">Id of the portal</param>
        /// <returns>PortalInfo object with portal definition</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public PortalInfo GetPortal(int portalId)
        {
            string defaultLanguage = GetActivePortalLanguage(portalId);
            PortalInfo portal = GetPortal(portalId, defaultLanguage);
            if (portal == null)
            {
                //Active language may not be valid, so fallback to default language
                defaultLanguage = GetPortalDefaultLanguage(portalId);
                portal = GetPortal(portalId, defaultLanguage);
            }
            return portal;
        }

        public PortalInfo GetPortal(int portalId, string cultureCode)
        {
            if (Localization.ActiveLanguagesByPortalID(portalId) == 1)
            {
                //only 1 language active, no need for fallback check
                return GetPortalInternal(portalId, cultureCode);
            }
            PortalInfo portal = GetPortalInternal(portalId, cultureCode);
            if (portal == null)
            {
                //Get Fallback language
                string fallbackLanguage = string.Empty;
                Locale userLocale = LocaleController.Instance.GetLocale(cultureCode);
                if (userLocale != null && !string.IsNullOrEmpty(userLocale.Fallback))
                {
                    fallbackLanguage = userLocale.Fallback;
                }
                portal = GetPortalInternal(portalId, fallbackLanguage);
                //if we cannot find any fallback, it mean's it's a non portal default langauge
                if (portal == null)
                {
                    DataProvider.Instance().EnsureLocalizationExists(portalId, GetActivePortalLanguage(portalId));
                    portal = GetPortalInternal(portalId, GetActivePortalLanguage(portalId));
                }
            }
            return portal;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets information from all portals
        /// </summary>
        /// <returns>ArrayList of PortalInfo objects</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public ArrayList GetPortals()
        {
            string cultureCode = Localization.SystemLocale;
            string cacheKey = String.Format(DataCache.PortalCacheKey, Null.NullInteger, cultureCode);
            var portals = CBO.GetCachedObject<List<PortalInfo>>(new CacheItemArgs(cacheKey, DataCache.PortalCacheTimeOut, DataCache.PortalCachePriority, cultureCode), 
                                                    GetPortalsCallBack);
            return new ArrayList(portals);
        }

        public List<PortalInfo> GetPortalList(string cultureCode)
        {
            string cacheKey = String.Format(DataCache.PortalCacheKey, Null.NullInteger, cultureCode);
            return CBO.GetCachedObject<List<PortalInfo>>(new CacheItemArgs(cacheKey, DataCache.PortalCacheTimeOut, DataCache.PortalCachePriority, cultureCode),
                                                    GetPortalsCallBack);
        }

        private object GetPortalsCallBack(CacheItemArgs cacheItemArgs)
	    {
            string cultureCode = (string)cacheItemArgs.ParamList[0];
            string cacheKey = String.Format(DataCache.PortalCacheKey, Null.NullInteger, cultureCode);
            List<PortalInfo> portals = CBO.FillCollection<PortalInfo>(DataProvider.Instance().GetPortals(cultureCode));
	        DataCache.SetCache(cacheKey, portals);
	        return portals;
	    }

	    /// <summary>
		/// Gets the portal.
		/// </summary>
		/// <param name="uniqueId">The unique id.</param>
		/// <returns>Portal info.</returns>
        public PortalInfo GetPortal(Guid uniqueId)
        {
            ArrayList portals = GetPortals();
            PortalInfo targetPortal = null;

            foreach (PortalInfo currentPortal in portals)
            {
                if (currentPortal.GUID == uniqueId)
                {
                    targetPortal = currentPortal;
                    break;
                }
            }
            return targetPortal;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the space used at the host level
        /// </summary>
        /// <returns>Space used in bytes</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	19/04/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public long GetPortalSpaceUsedBytes()
        {
            return GetPortalSpaceUsedBytes(-1);
        }

		/// <summary>
		/// Gets the portal space used bytes.
		/// </summary>
		/// <param name="portalId">The portal id.</param>
		/// <returns>Space used in bytes</returns>
        public long GetPortalSpaceUsedBytes(int portalId)
        {
            long size = 0;
            IDataReader dr = null;
            dr = DataProvider.Instance().GetPortalSpaceUsed(portalId);
            try
            {
                if (dr.Read())
                {
                    if (dr["SpaceUsed"] != DBNull.Value)
                    {
                        size = Convert.ToInt64(dr["SpaceUsed"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return size;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Verifies if there's enough space to upload a new file on the given portal
        /// </summary>
		/// <param name="portalId">Id of the portal</param>
        /// <param name="fileSizeBytes">Size of the file being uploaded</param>
        /// <returns>True if there's enough space available to upload the file</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	19/04/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool HasSpaceAvailable(int portalId, long fileSizeBytes)
        {
            int hostSpace;
            if (portalId == -1)
            {
                hostSpace = 0;
            }
            else
            {
                PortalSettings ps = GetCurrentPortalSettings();
                if (ps != null && ps.PortalId == portalId)
                {
                    hostSpace = ps.HostSpace;
                }
                else
                {
                    PortalInfo portal = GetPortal(portalId);
                    hostSpace = portal.HostSpace;
                }
            }
            return (((GetPortalSpaceUsedBytes(portalId) + fileSizeBytes) / Math.Pow(1024, 2)) <= hostSpace) || hostSpace == 0;
        }

        /// <summary>
        ///   Remaps the Special Pages such as Home, Profile, Search
        ///   to their localized versions
        /// </summary>
        /// <remarks>
        /// </remarks>
        public void MapLocalizedSpecialPages(int portalId, string cultureCode)
        {
            TabController tabCont = new TabController();

            DataCache.ClearHostCache(true);
            DataProvider.Instance().EnsureLocalizationExists(portalId, cultureCode);

            PortalInfo defaultPortal = GetPortal(portalId, GetPortalDefaultLanguage(portalId));
            PortalInfo targetPortal = GetPortal(portalId, cultureCode);

            Locale targetLocale = LocaleController.Instance.GetLocale(cultureCode);
            if ((defaultPortal.HomeTabId != Null.NullInteger))
            {
                targetPortal.HomeTabId = tabCont.GetTabByCulture(defaultPortal.HomeTabId, portalId, targetLocale).TabID;
            }
            if ((defaultPortal.LoginTabId != Null.NullInteger))
            {
                targetPortal.LoginTabId = tabCont.GetTabByCulture(defaultPortal.LoginTabId, portalId, targetLocale).TabID;
            }
            if ((defaultPortal.RegisterTabId != Null.NullInteger))
            {
                targetPortal.RegisterTabId = tabCont.GetTabByCulture(defaultPortal.RegisterTabId, portalId, targetLocale).TabID;
            }
            if ((defaultPortal.SplashTabId != Null.NullInteger))
            {
                targetPortal.SplashTabId = tabCont.GetTabByCulture(defaultPortal.SplashTabId, portalId, targetLocale).TabID;
            }
            if ((defaultPortal.UserTabId != Null.NullInteger))
            {
                targetPortal.UserTabId = tabCont.GetTabByCulture(defaultPortal.UserTabId, portalId, targetLocale).TabID;
            }
            if ((defaultPortal.SearchTabId != Null.NullInteger))
            {
                targetPortal.SearchTabId = tabCont.GetTabByCulture(defaultPortal.SearchTabId, portalId, targetLocale).TabID;
            }

            UpdatePortalInfo(targetPortal);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Processess a template file for the new portal. This method will be called twice: for the portal template and for the admin template
        /// </summary>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <param name="TemplatePath">Path for the folder where templates are stored</param>
        /// <param name="TemplateFile">Template file to process</param>
        /// <param name="AdministratorId">UserId for the portal administrator. This is used to assign roles to this user</param>
        /// <param name="mergeTabs">Flag to determine whether Module content is merged.</param>
        /// <param name="IsNewPortal">Flag to determine is the template is applied to an existing portal or a new one.</param>
        /// <remarks>
        /// The roles and settings nodes will only be processed on the portal template file.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	27/08/2004	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void ParseTemplate(int PortalId, string TemplatePath, string TemplateFile, int AdministratorId, PortalTemplateModuleAction mergeTabs, bool IsNewPortal)
        {
            XmlDocument xmlPortal = new XmlDocument();
            IFolderInfo objFolder;
            XmlNode node;
            try
            {
                xmlPortal.Load(TemplatePath + TemplateFile);
            }
            catch(Exception ex)
            {
				DnnLog.Error(ex);
            }
            node = xmlPortal.SelectSingleNode("//portal/settings");
            if (node != null && IsNewPortal)
            {
                ParsePortalSettings(node, PortalId);
            }
            node = xmlPortal.SelectSingleNode("//portal/rolegroups");
            if (node != null)
            {
                ParseRoleGroups(node.CreateNavigator(), PortalId, AdministratorId);
            }
            node = xmlPortal.SelectSingleNode("//portal/roles");
            if (node != null)
            {
                ParseRoles(node.CreateNavigator(), PortalId, AdministratorId);
            }
            node = xmlPortal.SelectSingleNode("//portal/portalDesktopModules");
            if (node != null)
            {
                ParsePortalDesktopModules(node.CreateNavigator(), PortalId);
            }
            node = xmlPortal.SelectSingleNode("//portal/folders");
            if (node != null)
            {
                ParseFolders(node, PortalId);
            }

            var defaultFolderMapping = FolderMappingController.Instance.GetDefaultFolderMapping(PortalId);

            if (FolderManager.Instance.GetFolder(PortalId, "") == null)
            {
                objFolder = FolderManager.Instance.AddFolder(defaultFolderMapping, "");
                objFolder.IsProtected = true;
                FolderManager.Instance.UpdateFolder(objFolder);

                AddFolderPermissions(PortalId, objFolder.FolderID);
            }

            if (FolderManager.Instance.GetFolder(PortalId, "Templates/") == null)
            {
                objFolder = FolderManager.Instance.AddFolder(defaultFolderMapping, "Templates/");
                objFolder.IsProtected = true;
                FolderManager.Instance.UpdateFolder(objFolder);

                //AddFolderPermissions(PortalId, objFolder.FolderID);
            }

            // force creation of users folder if not present on template
            if (FolderManager.Instance.GetFolder(PortalId, "Users/") == null)
            {
                objFolder = FolderManager.Instance.AddFolder(defaultFolderMapping, "Users/");
                objFolder.IsProtected = true;
                FolderManager.Instance.UpdateFolder(objFolder);

                //AddFolderPermissions(PortalId, objFolder.FolderID);
            }
            
            if (mergeTabs == PortalTemplateModuleAction.Replace)
            {
                TabController objTabs = new TabController();
                TabInfo objTab;
                foreach (KeyValuePair<int, TabInfo> tabPair in objTabs.GetTabsByPortal(PortalId))
                {
                    objTab = tabPair.Value;
                    objTab.TabName = objTab.TabName + "_old";
                    objTab.TabPath = Globals.GenerateTabPath(objTab.ParentId, objTab.TabName);
                    objTab.IsDeleted = true;
                    objTabs.UpdateTab(objTab);
                    ModuleController objModules = new ModuleController();
                    ModuleInfo objModule;
                    foreach (KeyValuePair<int, ModuleInfo> modulePair in objModules.GetTabModules(objTab.TabID))
                    {
                        objModule = modulePair.Value;
                        objModules.DeleteTabModule(objModule.TabID, objModule.ModuleID, false);
                    }
                }
            }
            node = xmlPortal.SelectSingleNode("//portal/tabs");
            if (node != null)
            {
                string version = xmlPortal.DocumentElement.GetAttribute("version");
                if (version != "5.0")
                {
                    XmlDocument xmlAdmin = new XmlDocument();
                    try
                    {
                        xmlAdmin.Load(TemplatePath + "admin.template");

						XmlNode adminNode = xmlAdmin.SelectSingleNode("//portal/tabs");
						foreach (XmlNode adminTabNode in adminNode.ChildNodes)
						{
							node.AppendChild(xmlPortal.ImportNode(adminTabNode, true));
						}
                    }
					catch (Exception ex)
					{
						DnnLog.Error(ex);
					}
                }
                ParseTabs(node, PortalId, false, mergeTabs, IsNewPortal);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Processes the resource file for the template file selected
        /// </summary>
        /// <param name="portalPath">New portal's folder</param>
        /// <param name="TemplateFile">Selected template file</param>
        /// <remarks>
        /// The resource file is a zip file with the same name as the selected template file and with
        /// an extension of .resources (to unable this file being downloaded).
        /// For example: for template file "portal.template" a resource file "portal.template.resources" can be defined.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	10/09/2004	Created
        ///     [cnurse]    11/08/2004  Moved from SignUp to PortalController
        ///     [cnurse]    03/04/2005  made Public
        ///     [cnurse]    05/20/2005  moved most of processing to new method in FileSystemUtils
        /// </history>
        /// -----------------------------------------------------------------------------
        public void ProcessResourceFile(string portalPath, string TemplateFile)
        {
            ZipInputStream objZipInputStream;
            try
            {
                objZipInputStream = new ZipInputStream(new FileStream(TemplateFile + ".resources", FileMode.Open, FileAccess.Read));
                FileSystemUtils.UnzipResources(objZipInputStream, portalPath);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
            }
        }

		/// <summary>
		/// Updates the portal expiry.
		/// </summary>
		/// <param name="PortalId">The portal id.</param>
        public void UpdatePortalExpiry(int PortalId)
        {
            UpdatePortalExpiry(PortalId, GetActivePortalLanguage(PortalId));
        }

		/// <summary>
		/// Updates the portal expiry.
		/// </summary>
		/// <param name="PortalId">The portal id.</param>
		/// <param name="CultureCode">The culture code.</param>
        public void UpdatePortalExpiry(int PortalId, string CultureCode)
        {
            DateTime ExpiryDate;
            IDataReader dr = null;
            try
            {
                dr = DataProvider.Instance().GetPortal(PortalId, CultureCode);
                if (dr.Read())
                {
                    if (dr["ExpiryDate"] != DBNull.Value)
                    {
                        ExpiryDate = Convert.ToDateTime(dr["ExpiryDate"]);
                    }
                    else
                    {
                        ExpiryDate = DateTime.Now;
                    }
                    DataProvider.Instance().UpdatePortalInfo(PortalId,
                                                            Convert.ToInt32(dr["PortalgroupID"]),
                                                            Convert.ToString(dr["PortalName"]),
                                                            Convert.ToString(dr["LogoFile"]),
                                                            Convert.ToString(dr["FooterText"]),
                                                            ExpiryDate.AddMonths(1),
                                                            Convert.ToInt32(dr["UserRegistration"]),
                                                            Convert.ToInt32(dr["BannerAdvertising"]),
                                                            Convert.ToString(dr["Currency"]),
                                                            Convert.ToInt32(dr["AdministratorId"]),
                                                            Convert.ToDouble(dr["HostFee"]),
                                                            Convert.ToDouble(dr["HostSpace"]),
                                                            Convert.ToInt32(dr["PageQuota"]),
                                                            Convert.ToInt32(dr["UserQuota"]),
                                                            Convert.ToString(dr["PaymentProcessor"]),
                                                            Convert.ToString(dr["ProcessorUserId"]),
                                                            Convert.ToString(dr["ProcessorPassword"]),
                                                            Convert.ToString(dr["Description"]),
                                                            Convert.ToString(dr["KeyWords"]),
                                                            Convert.ToString(dr["BackgroundFile"]),
                                                            Convert.ToInt32(dr["SiteLogHistory"]),
                                                            Convert.ToInt32(dr["SplashTabId"]),
                                                            Convert.ToInt32(dr["HomeTabId"]),
                                                            Convert.ToInt32(dr["LoginTabId"]),
                                                            Convert.ToInt32(dr["RegisterTabId"]),
                                                            Convert.ToInt32(dr["UserTabId"]),
                                                            Convert.ToInt32(dr["SearchTabId"]),
                                                            Convert.ToString(dr["DefaultLanguage"]),
                                                            Convert.ToString(dr["HomeDirectory"]),
                                                            UserController.GetCurrentUserInfo().UserID,
                                                            CultureCode);
                    var eventLogController = new EventLogController();
                    eventLogController.AddLog("ExpiryDate", ExpiryDate.ToString(), GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.PORTALINFO_UPDATED);
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates basic portal information
        /// </summary>
        /// <param name="portal"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/13/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void UpdatePortalInfo(PortalInfo portal)
        {
            DataProvider.Instance().UpdatePortalInfo(portal.PortalID,
                                            portal.PortalGroupID,
                                            portal.PortalName,
                                            portal.LogoFile,
                                            portal.FooterText,
                                            portal.ExpiryDate,
                                            portal.UserRegistration,
                                            portal.BannerAdvertising,
                                            portal.Currency,
                                            portal.AdministratorId,
                                            portal.HostFee,
                                            portal.HostSpace,
                                            portal.PageQuota,
                                            portal.UserQuota,
                                            portal.PaymentProcessor,
                                            portal.ProcessorUserId,
                                            portal.ProcessorPassword,
                                            portal.Description,
                                            portal.KeyWords,
                                            portal.BackgroundFile,
                                            portal.SiteLogHistory,
                                            portal.SplashTabId,
                                            portal.HomeTabId,
                                            portal.LoginTabId,
                                            portal.RegisterTabId,
                                            portal.UserTabId,
                                            portal.SearchTabId,
                                            portal.DefaultLanguage,
                                            portal.HomeDirectory,
                                            UserController.GetCurrentUserInfo().UserID,
                                            portal.CultureCode);
            
            var eventLogController = new EventLogController();
            eventLogController.AddLog("PortalId", portal.PortalID.ToString(), GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.PORTALINFO_UPDATED);
            
            //ensure a localization item exists (in case a new default language has been set)
            DataProvider.Instance().EnsureLocalizationExists(portal.PortalID, portal.DefaultLanguage);
            
            //clear portal cache
            DataCache.ClearHostCache(true);
        }

	    #region Public Static Methods

        /// <summary>
        /// Adds the portal dictionary.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <param name="tabId">The tab id.</param>
        public static void AddPortalDictionary(int portalId, int tabId)
        {
            var portalDic = GetPortalDictionary();
            portalDic[tabId] = portalId;
            DataCache.SetCache(DataCache.PortalDictionaryCacheKey, portalDic);
        }

        /// <summary>
        /// Creates the root folder for a child portal.
        /// </summary>
        /// <remarks>
        /// If call this method, it will create the specific folder if the folder doesn't exist;
        /// and will copy subhost.aspx to the folder if there is no 'Default.aspx';
        /// </remarks>
        /// <param name="ChildPath">The child path.</param>
        /// <returns>
        /// If the method executed successful, it will return NullString, otherwise return error message.
        /// </returns>
        /// <example>
        /// <code lang="C#">
        /// string childPhysicalPath = Server.MapPath(childPath);
        /// message = PortalController.CreateChildPortalFolder(childPhysicalPath);
        /// </code>
        /// </example>
        public static string CreateChildPortalFolder(string ChildPath)
        {
            string message = Null.NullString;

            //Set up Child Portal
            try
            {
                // create the subdirectory for the new portal
                if (!Directory.Exists(ChildPath))
                {
                    Directory.CreateDirectory(ChildPath);
                }

                // create the subhost default.aspx file
                if (!File.Exists(ChildPath + "\\" + Globals.glbDefaultPage))
                {
                    File.Copy(Globals.HostMapPath + "subhost.aspx", ChildPath + "\\" + Globals.glbDefaultPage);
                }
            }
            catch (Exception Exc)
            {
                DnnLog.Error(Exc);
                message += Localization.GetString("ChildPortal.Error") + Exc.Message + Exc.StackTrace;
            }

            return message;
        }

        /// <summary>
        /// Deletes all expired portals.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        public static void DeleteExpiredPortals(string serverPath)
        {
            foreach (PortalInfo portal in GetExpiredPortals())
            {
                DeletePortal(portal, serverPath);
            }
        }

        /// <summary>
        /// Deletes the portal.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="serverPath">The server path.</param>
        /// <returns>If the method executed successful, it will return NullString, otherwise return error message.</returns>
        public static string DeletePortal(PortalInfo portal, string serverPath)
        {
            string strPortalName;
            string strMessage = string.Empty;

            //check if this is the last portal
            int portalCount = DataProvider.Instance().GetPortalCount();
            if (portalCount > 1)
            {
                if (portal != null)
                {
                    //delete custom resource files
                    Globals.DeleteFilesRecursive(serverPath, ".Portal-" + portal.PortalID + ".resx");

                    //If child portal delete child folder
                    PortalAliasController objPortalAliasController = new PortalAliasController();
                    ArrayList arr = objPortalAliasController.GetPortalAliasArrayByPortalID(portal.PortalID);
                    if (arr.Count > 0)
                    {
                        PortalAliasInfo objPortalAliasInfo = (PortalAliasInfo)arr[0];
                        strPortalName = Globals.GetPortalDomainName(objPortalAliasInfo.HTTPAlias, null, true);
                        if (objPortalAliasInfo.HTTPAlias.IndexOf("/") > -1)
                        {
                            strPortalName = objPortalAliasInfo.HTTPAlias.Substring(objPortalAliasInfo.HTTPAlias.LastIndexOf("/") + 1);
                        }
                        if (!String.IsNullOrEmpty(strPortalName) && Directory.Exists(serverPath + strPortalName))
                        {
                            Globals.DeleteFolderRecursive(serverPath + strPortalName);
                        }
                    }
                    //delete upload directory
                    Globals.DeleteFolderRecursive(serverPath + "Portals\\" + portal.PortalID);
                    if (!string.IsNullOrEmpty(portal.HomeDirectory))
                    {
                        string HomeDirectory = portal.HomeDirectoryMapPath;
                        if (Directory.Exists(HomeDirectory))
                        {
                            Globals.DeleteFolderRecursive(HomeDirectory);
                        }
                    }
                    //remove database references
                    PortalController objPortalController = new PortalController();
                    objPortalController.DeletePortalInfo(portal.PortalID);
                }
            }
            else
            {
                strMessage = Localization.GetString("LastPortal");
            }
            return strMessage;
        }

        /// <summary>
        /// Gets the portal dictionary.
        /// </summary>
        /// <returns>portal dictionary. the dictionary's Key -> Value is: TabId -> PortalId.</returns>
        public static Dictionary<int, int> GetPortalDictionary()
        {
            string cacheKey = string.Format(DataCache.PortalDictionaryCacheKey);
            return CBO.GetCachedObject<Dictionary<int, int>>(new CacheItemArgs(cacheKey, DataCache.PortalDictionaryTimeOut, DataCache.PortalDictionaryCachePriority), GetPortalDictionaryCallback);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetPortalsByName gets all the portals whose name matches a provided filter expression
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="nameToMatch">The email address to use to find a match.</param>
        /// <param name="pageIndex">The page of records to return.</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        /// <returns>An ArrayList of PortalInfo objects.</returns>
        /// <history>
        ///     [cnurse]	11/17/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static ArrayList GetPortalsByName(string nameToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            if (pageIndex == -1)
            {
                pageIndex = 0;
                pageSize = int.MaxValue;
            }
            Type type = typeof(PortalInfo);
            return CBO.FillCollection(DataProvider.Instance().GetPortalsByName(nameToMatch, pageIndex, pageSize), ref type, ref totalRecords);
        }

        /// <summary>
        /// Gets the current portal settings.
        /// </summary>
        /// <returns>portal settings.</returns>
        public static PortalSettings GetCurrentPortalSettings()
        {
            PortalSettings objPortalSettings = null;
            if (HttpContext.Current != null)
            {
                objPortalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            }
            return objPortalSettings;
        }

        public static int GetEffectivePortalId(int portalId)
        {
            if (portalId > Null.NullInteger && Globals.Status != Globals.UpgradeStatus.Upgrade)
            {
                var portalController = new PortalController();
                var portal = portalController.GetPortal(portalId);
                var portalGroup = (from p in PortalGroupController.Instance.GetPortalGroups()
                                   where p.PortalGroupId == portal.PortalGroupID
                                   select p)
                                .SingleOrDefault();

                if (portalGroup != null)
                {
                    portalId = portalGroup.MasterPortalId;
                }
            }

            return portalId;
        }

        /// <summary>
        /// Gets all expired portals.
        /// </summary>
        /// <returns>all expired portals as array list.</returns>
        public static ArrayList GetExpiredPortals()
        {
            return CBO.FillCollection(DataProvider.Instance().GetExpiredPortals(), typeof(PortalInfo));
        }

        /// <summary>
        /// Determines whether the portal is child portal.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="serverPath">The server path.</param>
        /// <returns>
        ///   <c>true</c> if the portal is child portal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsChildPortal(PortalInfo portal, string serverPath)
        {
            bool isChild = Null.NullBoolean;
            string portalName;
            PortalAliasController aliasController = new PortalAliasController();
            ArrayList arr = aliasController.GetPortalAliasArrayByPortalID(portal.PortalID);
            if (arr.Count > 0)
            {
                PortalAliasInfo portalAlias = (PortalAliasInfo)arr[0];
                portalName = Globals.GetPortalDomainName(portalAlias.HTTPAlias, null, true);
                if (portalAlias.HTTPAlias.IndexOf("/") > -1)
                {
                    portalName = portalAlias.HTTPAlias.Substring(portalAlias.HTTPAlias.LastIndexOf("/") + 1);
                }
                if (!String.IsNullOrEmpty(portalName) && Directory.Exists(serverPath + portalName))
                {
                    isChild = true;
                }
            }
            return isChild;
        }

        public static bool IsMemberOfPortalGroup(int portalId)
        {
            var portalController = new PortalController();
            var portal = portalController.GetPortal(portalId);

            return (portal.PortalGroupID > Null.NullInteger);
        }

        /// <summary>
        /// Deletes the portal setting.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="settingName">Name of the setting.</param>
        public static void DeletePortalSetting(int portalID, string settingName)
        {
            DeletePortalSetting(portalID, settingName, GetActivePortalLanguage(portalID));
        }

        /// <summary>
        /// Deletes the portal setting.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="CultureCode">The culture code.</param>
        public static void DeletePortalSetting(int portalID, string settingName, string CultureCode)
        {
            DataProvider.Instance().DeletePortalSetting(portalID, settingName, CultureCode.ToLower());
            EventLogController objEventLog = new EventLogController();
            objEventLog.AddLog("SettingName", settingName, GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.PORTAL_SETTING_DELETED);
            DataCache.ClearHostCache(true);
        }

        /// <summary>
        /// Deletes all portal settings by portal id.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        public static void DeletePortalSettings(int portalID)
        {
            DataProvider.Instance().DeletePortalSettings(portalID);
            EventLogController objEventLog = new EventLogController();
            objEventLog.AddLog("PortalID", portalID.ToString(), GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.PORTAL_SETTING_DELETED);
            DataCache.ClearHostCache(true);
        }

        /// <summary>
        /// Gets the portal settings dictionary.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <returns>portal settings.</returns>
        public static Dictionary<string, string> GetPortalSettingsDictionary(int portalID)
        {
            string cacheKey = string.Format(DataCache.PortalSettingsCacheKey, portalID);
            return CBO.GetCachedObject<Dictionary<string, string>>(new CacheItemArgs(cacheKey, DataCache.PortalSettingsCacheTimeOut, DataCache.PortalSettingsCachePriority, portalID),
                                                                   GetPortalSettingsDictionaryCallback,
                                                                   true);
        }

        /// <summary>
        /// Gets the portal setting.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns setting's value if portal contains the specific setting, otherwise return defaultValue.</returns>
        public static string GetPortalSetting(string settingName, int portalID, string defaultValue)
        {
            string retValue = Null.NullString;
            try
            {
                string setting = Null.NullString;
                GetPortalSettingsDictionary(portalID).TryGetValue(settingName, out setting);
                if (string.IsNullOrEmpty(setting))
                {
                    retValue = defaultValue;
                }
                else
                {
                    retValue = setting;
                }
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
            }
            return retValue;
        }

        /// <summary>
        /// Gets the portal setting as boolean.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="defaultValue">default value.</param>
        /// <returns>Returns setting's value if portal contains the specific setting, otherwise return defaultValue.</returns>
        public static bool GetPortalSettingAsBoolean(string key, int portalID, bool defaultValue)
        {
            bool retValue = Null.NullBoolean;
            try
            {
                string setting = Null.NullString;
                GetPortalSettingsDictionary(portalID).TryGetValue(key, out setting);
                if (string.IsNullOrEmpty(setting))
                {
                    retValue = defaultValue;
                }
                else
                {
                    retValue = (setting.StartsWith("Y", StringComparison.InvariantCultureIgnoreCase) || setting.ToUpperInvariant() == "TRUE");
                }
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
            }
            return retValue;
        }

        /// <summary>
        /// Gets the portal setting as integer.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns setting's value if portal contains the specific setting, otherwise return defaultValue.</returns>
        public static int GetPortalSettingAsInteger(string key, int portalID, int defaultValue)
        {
            int retValue = Null.NullInteger;
            try
            {
                string setting = Null.NullString;
                GetPortalSettingsDictionary(portalID).TryGetValue(key, out setting);
                if (string.IsNullOrEmpty(setting))
                {
                    retValue = defaultValue;
                }
                else
                {
                    retValue = Convert.ToInt32(setting);
                }
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
            }
            return retValue;
        }

        /// <summary>
        /// Updates the portal setting.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        public static void UpdatePortalSetting(int portalID, string settingName, string settingValue)
        {
            UpdatePortalSetting(portalID, settingName, settingValue, true);
        }

        /// <summary>
        /// Updates the portal setting.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        /// <param name="clearCache">if set to <c>true</c> [clear cache].</param>
        public static void UpdatePortalSetting(int portalID, string settingName, string settingValue, bool clearCache)
        {
            string culture = Thread.CurrentThread.CurrentCulture.ToString().ToLower();
            if ((string.IsNullOrEmpty(culture)))
            {
                culture = GetPortalSetting("DefaultLanguage", portalID, "".ToLower());
            }
            if ((string.IsNullOrEmpty(culture)))
            {
                culture = Localization.SystemLocale.ToLower();
            }
            UpdatePortalSetting(portalID, settingName, settingValue, clearCache, culture);
        }

        /// <summary>
        /// Updates the portal setting.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        /// <param name="clearCache">if set to <c>true</c> [clear cache].</param>
        /// <param name="culturecode">The culturecode.</param>
        public static void UpdatePortalSetting(int portalID, string settingName, string settingValue, bool clearCache, string culturecode)
        {
            DataProvider.Instance().UpdatePortalSetting(portalID, settingName, settingValue, UserController.GetCurrentUserInfo().UserID, culturecode);
            EventLogController objEventLog = new EventLogController();
            objEventLog.AddLog(settingName, settingValue, GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
            if (clearCache)
            {
                DataCache.ClearHostCache(true);
            }
        }

        /// <summary>
        /// Checks the desktop modules whether is installed.
        /// </summary>
        /// <param name="nav">The nav.</param>
        /// <returns>Empty string if the module hasn't been installed, otherwise return the frind name.</returns>
        public static string CheckDesktopModulesInstalled(XPathNavigator nav)
        {
            string friendlyName = Null.NullString;
            DesktopModuleInfo desktopModule = null;
            StringBuilder modulesNotInstalled = new StringBuilder();

            foreach (XPathNavigator desktopModuleNav in nav.Select("portalDesktopModule"))
            {
                friendlyName = XmlUtils.GetNodeValue(desktopModuleNav, "friendlyname");

                if (!string.IsNullOrEmpty(friendlyName))
                {
                    desktopModule = DesktopModuleController.GetDesktopModuleByFriendlyName(friendlyName);
                    if (desktopModule == null)
                    {
                        //PE and EE templates have HTML as friendly name so check to make sure
                        //there is really no HTML module installed
                        if (friendlyName == "HTML")
                        {
                            desktopModule = DesktopModuleController.GetDesktopModuleByFriendlyName("HTML Pro");
                            if (desktopModule == null)
                            {
                                modulesNotInstalled.Append(friendlyName);
                                modulesNotInstalled.Append("<br/>");
                            }
                        }
                        else
                        {
                            modulesNotInstalled.Append(friendlyName);
                            modulesNotInstalled.Append("<br/>");
                        }
                    }
                }
            }
            return modulesNotInstalled.ToString();
        }

        /// <summary>
        ///   function provides the language for portalinfo requests
        ///   in case where language has not been installed yet, will return the core install default of en-us
        /// </summary>
        /// <param name = "portalID"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public static string GetActivePortalLanguage(int portalID)
        {
            // get Language
            string Language = Localization.SystemLocale;
            string tmpLanguage = GetPortalDefaultLanguage(portalID);
        	var isDefaultLanguage = false;
            if (!String.IsNullOrEmpty(tmpLanguage))
            {
                Language = tmpLanguage;
            	isDefaultLanguage = true;
            }
            //handles case where portalcontroller methods invoked before a language is installed
            if (portalID > Null.NullInteger && Globals.Status == Globals.UpgradeStatus.None && Localization.ActiveLanguagesByPortalID(portalID) == 1)
            {
                return Language;
            }
            if (HttpContext.Current != null && Globals.Status == Globals.UpgradeStatus.None)
            {
                if ((HttpContext.Current.Request.QueryString["language"] != null))
                {
                    Language = HttpContext.Current.Request.QueryString["language"];
					isDefaultLanguage = false;
                }
                else
                {
                    PortalSettings _PortalSettings = GetCurrentPortalSettings();
                    if (_PortalSettings != null && _PortalSettings.ActiveTab != null && !String.IsNullOrEmpty(_PortalSettings.ActiveTab.CultureCode))
                    {
                        Language = _PortalSettings.ActiveTab.CultureCode;
						isDefaultLanguage = false;
                    }
                    else
                    {
                        //PortalSettings IS Nothing - probably means we haven't set it yet (in Begin Request)
                        //so try detecting the user's cookie
                        if (HttpContext.Current.Request["language"] != null)
                        {
                            Language = HttpContext.Current.Request["language"];
							isDefaultLanguage = false;
                        }

                        //if no cookie - try detecting browser
						if (String.IsNullOrEmpty(Language) || isDefaultLanguage)
                        {
                            CultureInfo Culture = Localization.GetBrowserCulture(portalID);

                            if (Culture != null)
                            {
                                Language = Culture.Name;
                            }
                        }
                    }
                }
            }

            return Language;
        }

        /// <summary>
        ///   return the current DefaultLanguage value from the Portals table for the requested Portalid
        /// </summary>
        /// <param name = "portalID"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public static string GetPortalDefaultLanguage(int portalID)
        {
            //Return DataProvider.Instance().GetPortalDefaultLanguage(portalID)
            string cacheKey = String.Format("PortalDefaultLanguage_{0}", portalID);
            return CBO.GetCachedObject<string>(new CacheItemArgs(cacheKey, DataCache.PortalCacheTimeOut, DataCache.PortalCachePriority, portalID), GetPortalDefaultLanguageCallBack);
        }

        /// <summary>
        ///   set the required DefaultLanguage in the Portals table for a particular portal
        ///   saves having to update an entire PortalInfo object
        /// </summary>
        /// <param name = "portalID"></param>
        /// <param name = "CultureCode"></param>
        /// <remarks>
        /// </remarks>
        public static void UpdatePortalDefaultLanguage(int portalID, string CultureCode)
        {
            DataProvider.Instance().UpdatePortalDefaultLanguage(portalID, CultureCode);
            //ensure localization record exists as new portal default language may be relying on fallback chain
            //of which it is now the final part
            DataProvider.Instance().EnsureLocalizationExists(portalID, CultureCode);
        }

        #endregion

        #region "Obsolete Methods"

        [Obsolete("This function has been replaced by GetPortalSpaceUsedBytes")]
        public int GetPortalSpaceUsed(int portalId)
        {
            int size = 0;
            try
            {
                size = Convert.ToInt32(GetPortalSpaceUsedBytes(portalId));
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);

                size = int.MaxValue;
            }

            return size;
        }

        [Obsolete("This function has been replaced by TabController.DeserializePanes")]
        public void ParsePanes(XmlNode nodePanes, int portalId, int TabId, PortalTemplateModuleAction mergeTabs, Hashtable hModules)
        {
            TabController.DeserializePanes(nodePanes, portalId, TabId, mergeTabs, hModules);
        }

        [Obsolete("Deprecated in DotNetNuke 6.1. Replaced by UpdatePortalInfo(PortalInfo)")]
        public void UpdatePortalInfo(int portalId, string portalName, string logoFile, string footerText, DateTime expiryDate, int userRegistration, int bannerAdvertising, string currency,
                            int administratorId, double hostFee, double hostSpace, int pageQuota, int userQuota, string paymentProcessor, string processorUserId, string processorPassword,
                            string description, string KeyWords, string backgroundFile, int siteLogHistory, int splashTabId, int homeTabId, int loginTabId, int registerTabId, int userTabId,
                            int searchTabId, string defaultLanguage, string homeDirectory)
        {
            var portal = new PortalInfo()
            {
                PortalID = portalId,
                PortalName = portalName,
                LogoFile = logoFile,
                FooterText = footerText,
                ExpiryDate = expiryDate,
                UserRegistration = userRegistration,
                BannerAdvertising = bannerAdvertising,
                Currency = currency,
                AdministratorId = administratorId,
                HostFee = (float)hostFee,
                HostSpace = (int)hostSpace,
                PageQuota = pageQuota,
                UserQuota = userQuota,
                PaymentProcessor = paymentProcessor,
                ProcessorUserId = processorUserId,
                ProcessorPassword = processorPassword,
                Description = description,
                KeyWords = KeyWords,
                BackgroundFile = backgroundFile,
                SiteLogHistory = siteLogHistory,
                SplashTabId = splashTabId,
                HomeTabId = homeTabId,
                LoginTabId = loginTabId,
                RegisterTabId = registerTabId,
                UserTabId = userTabId,
                SearchTabId = searchTabId,
                DefaultLanguage = defaultLanguage,
                HomeDirectory = homeDirectory,
                CultureCode = PortalController.GetActivePortalLanguage(portalId)
            };

            UpdatePortalInfo(portal);
        }


        [Obsolete("Deprecated in DotNetNuke 6.1. Replaced by UpdatePortalInfo(PortalInfo)")]
        public void UpdatePortalInfo(int portalId, string portalName, string logoFile, string footerText, DateTime expiryDate, int userRegistration, int bannerAdvertising, string currency,
                            int administratorId, double hostFee, double hostSpace, int pageQuota, int userQuota, string paymentProcessor, string processorUserId, string processorPassword,
                            string description, string keyWords, string backgroundFile, int siteLogHistory, int splashTabId, int homeTabId, int loginTabId, int registerTabId, int userTabId,
                            int searchTabId, string defaultLanguage, string homeDirectory, string cultureCode)
{
            var portal = new PortalInfo()
            {
                PortalID = portalId,
                PortalName = portalName,
                LogoFile = logoFile,
                FooterText = footerText,
                ExpiryDate = expiryDate,
                UserRegistration = userRegistration,
                BannerAdvertising = bannerAdvertising,
                Currency = currency,
                AdministratorId = administratorId,
                HostFee = (float)hostFee,
                HostSpace = (int)hostSpace,
                PageQuota = pageQuota,
                UserQuota = userQuota,
                PaymentProcessor = paymentProcessor,
                ProcessorUserId = processorUserId,
                ProcessorPassword = processorPassword,
                Description = description,
                KeyWords = keyWords,
                BackgroundFile = backgroundFile,
                SiteLogHistory = siteLogHistory,
                SplashTabId = splashTabId,
                HomeTabId = homeTabId,
                LoginTabId = loginTabId,
                RegisterTabId = registerTabId,
                UserTabId = userTabId,
                SearchTabId = searchTabId,
                DefaultLanguage = defaultLanguage,
                HomeDirectory = homeDirectory,
                CultureCode = cultureCode
            };
            UpdatePortalInfo(portal);
        }


        #endregion
    }
}
