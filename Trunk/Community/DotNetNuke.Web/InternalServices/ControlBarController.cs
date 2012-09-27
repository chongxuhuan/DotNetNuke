#region Copyright

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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Web.InternalServices
{
    [DnnAuthorize]
    public class ControlBarController : DnnApiController
    {
        private const string DefaultExtensionImage = "icon_extensions_32px.gif";

        public class ModuleDefDTO
        {
            public int ModuleID { get; set; }
            public string ModuleName { get; set; }
            public string ModuleImage { get; set; }
        }

        public class PageDefDTO
        {
            public int TabID { get; set; }
            public string IndentedTabName { get; set; }
        }

        public class AddModuleDTO
        {
            public string Visibility { get; set; }
            public string Position { get; set; }
            public string Module { get; set; }
            public string Page { get; set; }
            public string Pane { get; set; }
            public string AddExistingModule { get; set; }
            public string CopyModule { get; set; }
            public string Sort { get; set; }
        }

        public class UserModeDTO
        {
            public string UserMode { get; set; }
        }

        public class SwitchSiteDTO
        {
            public string Site { get; set; }
        }


        [HttpGet]
        public HttpResponseMessage GetPortalDesktopModules(string category)
        {
            if (string.IsNullOrEmpty(category))
                category = "All";

            IOrderedEnumerable<KeyValuePair<string, PortalDesktopModuleInfo>> portalModulesList;

            Func<KeyValuePair<string, PortalDesktopModuleInfo>, bool> Filter = category == "All"
                                        ? (Func<KeyValuePair<string, PortalDesktopModuleInfo>, bool>)(kvp => true)
                                         : (Func<KeyValuePair<string, PortalDesktopModuleInfo>, bool>)(kvp => kvp.Value.DesktopModule.Category == category);
            
            
            portalModulesList = DesktopModuleController.GetPortalDesktopModules(PortalSettings.Current.PortalId)
                .Where(Filter)
                .OrderBy(c => c.Key);
            

            Dictionary<int, string> resultDict = portalModulesList.ToDictionary(portalModule => portalModule.Value.DesktopModuleID,
                                                    portalModule => portalModule.Key);

            List<ModuleDefDTO> result = new List<ModuleDefDTO>();
            foreach (var kvp in resultDict)
            {
                string imageUrl = GetDeskTopModuleImage(kvp.Key);
                result.Add(new ModuleDefDTO { ModuleID = kvp.Key, ModuleName = kvp.Value, ModuleImage = imageUrl });
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage GetPageList(string portal)
        {
            var portalSettings = GetPortalSettings(portal);

            List<TabInfo> tabList = null;
            if (PortalSettings.PortalId == portalSettings.PortalId)
            {
                tabList = TabController.GetPortalTabs(portalSettings.PortalId, portalSettings.ActiveTab.TabID, true, string.Empty, true, false, false, false, true);
            }
            else
            {
                tabList = TabController.GetPortalTabs(portalSettings.PortalId, Null.NullInteger, true, string.Empty, true, false, false, false, true);
            }

            List<PageDefDTO> result = new List<PageDefDTO>();
            foreach (var tab in tabList)
            {
                result.Add(new PageDefDTO { TabID = tab.TabID, IndentedTabName = tab.IndentedTabName });
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage GetTabModules(string tab)
        {
            var tabCtrl = new TabController();
            var moduleCtrl = new ModuleController();
            int tabID = 0;

            if (!string.IsNullOrEmpty(tab))
            {

                try
                {
                    tabID = int.Parse(tab);
                }
                catch
                {
                }
            }

            List<ModuleDefDTO> result = new List<ModuleDefDTO>();
            if (tabID > 0)
            {
                var tabModules = moduleCtrl.GetTabModules(tabID);

                // Is this tab from another site?
                var isRemote = tabCtrl.GetTab(tabID, Null.NullInteger, false).PortalID != PortalSettings.Current.PortalId;

                var pageModules = tabModules.Values.Where(m => !isRemote || ModuleSuportsSharing(m)).Where(m => ModulePermissionController.CanAdminModule(m) && m.IsDeleted == false).ToList();
                
                Dictionary<int, string> resultDict = pageModules.ToDictionary(module => module.ModuleID, module => module.ModuleTitle);
                foreach (var kvp in resultDict)
                {
                    string imageUrl = GetTabModuleImage(tabID, kvp.Key);
                    result.Add(new ModuleDefDTO { ModuleID = kvp.Key, ModuleName = kvp.Value, ModuleImage= imageUrl });
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        } 


        [HttpPost]
//        [ValidateAntiForgeryToken]
        public HttpResponseMessage AddModule(AddModuleDTO dto)
        {
            if (TabPermissionController.CanAddContentToPage() && CanAddModuleToPage())
            {
                int permissionType;
                try
                {
                    permissionType = int.Parse(dto.Visibility);
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);
                    permissionType = 0;
                }

                int positionID = -1;
                if (!string.IsNullOrEmpty(dto.Sort))
                {
                    int sortID = 0;
                    try
                    {
                        sortID = int.Parse(dto.Sort);
                        if(sortID >= 0)
                            positionID = GetPaneModuleOrder(dto.Pane, sortID);
                    }
                    catch(Exception exc)
                    {
                        DnnLog.Error(exc);
                    }
                }
                
                if(positionID == -1)
                {
                    switch (dto.Position)
                    {
                        case "TOP":
                            positionID = 0;
                            break;
                        case "BOTTOM":
                            positionID = -1;
                            break;
                    }
                }

                int moduleLstID;
                try
                {
                    moduleLstID = int.Parse(dto.Module);
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);
                    moduleLstID = -1;
                }

                try
                {
                    if ((moduleLstID > -1))
                    {
                        if ((dto.AddExistingModule == "true"))
                        {
                            int pageID;
                            try
                            {
                                pageID = int.Parse(dto.Page);
                            }
                            catch (Exception exc)
                            {
                                DnnLog.Error(exc);
                                pageID = -1;
                            }

                            if ((pageID > -1))
                            {
                                DoAddExistingModule(moduleLstID, pageID, dto.Pane, positionID, "", dto.CopyModule == "true");
                            }
                        }
                        else
                        {
                            DoAddNewModule("", moduleLstID, dto.Pane, positionID, permissionType, "");
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch
                {
                }                
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        [HttpPost]
//        [ValidateAntiForgeryToken]
        public HttpResponseMessage ClearHostCache()
        {
            if (UserController.GetCurrentUserInfo().IsSuperUser)           
            {
                DataCache.ClearCache();
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }


        [HttpPost]
//        [ValidateAntiForgeryToken]
        public HttpResponseMessage RecycleApplicationPool()
        {
            if (UserController.GetCurrentUserInfo().IsSuperUser)
            {
                var objEv = new EventLogController();
                var objEventLogInfo = new LogInfo { BypassBuffering = true, LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString() };
                objEventLogInfo.AddProperty("Message", "UserRestart");
                objEv.AddLog(objEventLogInfo);
                Config.Touch();
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

     

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public HttpResponseMessage SwitchSite(SwitchSiteDTO dto)
        {
            if (UserController.GetCurrentUserInfo().IsSuperUser)
            {
                try
                {
                    if ((!string.IsNullOrEmpty(dto.Site)))
                    {
                        int selectedPortalID = int.Parse(dto.Site);
                        var portalAliasCtrl = new PortalAliasController();
                        ArrayList portalAliases = portalAliasCtrl.GetPortalAliasArrayByPortalID(selectedPortalID);

                        if (((portalAliases != null) && portalAliases.Count > 0 && (portalAliases[0] != null)))
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new { RedirectURL = Globals.AddHTTP(((PortalAliasInfo)portalAliases[0]).HTTPAlias) });
                        }
                    }
                }
                catch (System.Threading.ThreadAbortException)
                {
                    //Do nothing we are not logging ThreadAbortxceptions caused by redirects      
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(ex);
                }
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        [HttpPost]
//        [ValidateAntiForgeryToken]
        public HttpResponseMessage ToggleUserMode(UserModeDTO userMode)
        {
            if (userMode == null)
                userMode = new UserModeDTO { UserMode = "VIEW" };

            ToggleUserMode(userMode.UserMode);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public class BookmarkDTO
        {
            public string Title { get; set; }
            public string Bookmark { get; set; }
        }

        [HttpPost]
//        [ValidateAntiForgeryToken]
        public HttpResponseMessage SaveBookmark(BookmarkDTO bookmark)
        {
            var personalizationController = new DotNetNuke.Services.Personalization.PersonalizationController();
            var personalization = personalizationController.LoadProfile(UserInfo.UserID, PortalSettings.PortalId);
            personalization.Profile["ControlBar:" + bookmark.Title + PortalSettings.PortalId] = bookmark.Bookmark;
            personalization.IsModified = true;
            personalizationController.SaveProfile(personalization);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private void ToggleUserMode(string mode)
        {
            var personalizationController = new DotNetNuke.Services.Personalization.PersonalizationController();
            var personalization = personalizationController.LoadProfile(UserInfo.UserID, PortalSettings.PortalId);
            personalization.Profile["Usability:UserMode" + PortalSettings.PortalId] = mode.ToUpper();
            personalization.IsModified = true;
            personalizationController.SaveProfile(personalization);
        }

        private PortalSettings GetPortalSettings(string portal)
        {
            var portalSettings = PortalSettings.Current;

            try
            {

                if (!string.IsNullOrEmpty(portal))
                {
                    var selectedPortalId = int.Parse(portal);
                    if (PortalSettings.PortalId != selectedPortalId)
                    {
                        portalSettings = new PortalSettings(selectedPortalId);
                    }
                }

            }
            catch (Exception)
            {
                portalSettings = PortalSettings.Current;
            }

            return portalSettings;
        }

        private bool ModuleSuportsSharing(ModuleInfo moduleInfo)
        {
            switch (moduleInfo.DesktopModule.Shareable)
            {
                case ModuleSharing.Supported:
                case ModuleSharing.Unknown:
                    return moduleInfo.IsShareable;
                default:
                    return false;
            }
        }

        private string GetDeskTopModuleImage(int moduleId)
        {
            var portalDesktopModules = DesktopModuleController.GetDesktopModules(PortalSettings.Current.PortalId);
            var packages = PackageController.GetPackages(PortalSettings.Current.PortalId);

            string imageUrl =
                    (from pkgs in packages
                     join portMods in portalDesktopModules on pkgs.PackageID equals portMods.Value.PackageID
                     where portMods.Value.DesktopModuleID == moduleId
                     select pkgs.IconFile).FirstOrDefault();

            imageUrl = String.IsNullOrEmpty(imageUrl) ? Globals.ImagePath + DefaultExtensionImage : imageUrl;
            return System.Web.VirtualPathUtility.ToAbsolute(imageUrl);
        }

        private string GetTabModuleImage(int tabId, int moduleId)
        {
            var tabModules = new ModuleController().GetTabModules(tabId);
            var portalDesktopModules = DesktopModuleController.GetDesktopModules(PortalSettings.Current.PortalId);
            var moduleDefnitions = ModuleDefinitionController.GetModuleDefinitions();
            var packages = PackageController.GetPackages(PortalSettings.Current.PortalId);

            string imageUrl = (from pkgs in packages
                               join portMods in portalDesktopModules on pkgs.PackageID equals portMods.Value.PackageID
                               join modDefs in moduleDefnitions on portMods.Value.DesktopModuleID equals modDefs.Value.DesktopModuleID
                               join tabMods in tabModules on modDefs.Value.DesktopModuleID equals tabMods.Value.DesktopModuleID
                               where tabMods.Value.ModuleID == moduleId
                               select pkgs.IconFile).FirstOrDefault();

            imageUrl = String.IsNullOrEmpty(imageUrl) ? Globals.ImagePath + DefaultExtensionImage : imageUrl; 
            return System.Web.VirtualPathUtility.ToAbsolute(imageUrl);
        }

        public bool CanAddModuleToPage()
        {
            return true;
            //If we are not in an edit page
            //return (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["mid"])) && (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ctl"]));
        }

        private void DoAddExistingModule(int moduleId, int tabId, string paneName, int position, string align, bool cloneModule)
        {
            var moduleCtrl = new ModuleController();
            ModuleInfo moduleInfo = moduleCtrl.GetModule(moduleId, tabId, false);

            int userID = -1;
          
            UserInfo user = UserController.GetCurrentUserInfo();
            if (((user != null)))
            {
                userID = user.UserID;
            }
            

            if ((moduleInfo != null))
            {
                // Is this from a site other than our own? (i.e., is the user requesting "module sharing"?)
                var remote = moduleInfo.PortalID != PortalSettings.Current.PortalId;
                if (remote)
                {
                    switch (moduleInfo.DesktopModule.Shareable)
                    {
                        case ModuleSharing.Unsupported:
                            // Should never happen since the module should not be listed in the first place.
                            throw new ApplicationException(string.Format("Module '{0}' does not support Shareable and should not be listed in Add Existing Module from a different source site",
                                                                         moduleInfo.DesktopModule.FriendlyName));
                        case ModuleSharing.Supported:
                            break;
                        default:
                        case ModuleSharing.Unknown:
                            break;
                    }
                }

                // clone the module object ( to avoid creating an object reference to the data cache )
                ModuleInfo newModule = moduleInfo.Clone();

                newModule.UniqueId = Guid.NewGuid(); // Cloned Module requires a different uniqueID

                newModule.TabID = PortalSettings.Current.ActiveTab.TabID;
                newModule.ModuleOrder = position;
                newModule.PaneName = paneName;
                newModule.Alignment = align;

                if ((cloneModule))
                {
                    newModule.ModuleID = Null.NullInteger;
                    //reset the module id
                    newModule.ModuleID = moduleCtrl.AddModule(newModule);

                    if (!string.IsNullOrEmpty(newModule.DesktopModule.BusinessControllerClass))
                    {
                        object objObject = DotNetNuke.Framework.Reflection.CreateObject(newModule.DesktopModule.BusinessControllerClass, newModule.DesktopModule.BusinessControllerClass);
                        if (objObject is IPortable)
                        {
                            string content = Convert.ToString(((IPortable)objObject).ExportModule(moduleId));
                            if (!string.IsNullOrEmpty(content))
                            {
                                ((IPortable)objObject).ImportModule(newModule.ModuleID, content, newModule.DesktopModule.Version, userID);
                            }
                        }
                    }
                }
                else
                {
                    moduleCtrl.AddModule(newModule);
                }

                if (remote)
                {
                    //Ensure the Portal Admin has View rights
                    var permissionController = new PermissionController();
                    ArrayList arrSystemModuleViewPermissions = permissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "VIEW");
                    AddModulePermission(newModule,
                                    (PermissionInfo)arrSystemModuleViewPermissions[0],
                                    PortalSettings.Current.AdministratorRoleId,
                                    Null.NullInteger,
                                    true);

                    //Set PortalID correctly
                    newModule.OwnerPortalID = newModule.PortalID;
                    newModule.PortalID = PortalSettings.Current.PortalId;
                    ModulePermissionController.SaveModulePermissions(newModule);
                }

                //Add Event Log
                var objEventLog = new EventLogController();
                objEventLog.AddLog(newModule, PortalSettings.Current, userID, "", EventLogController.EventLogType.MODULE_CREATED);
            }
        }

        private ModulePermissionInfo AddModulePermission(ModuleInfo objModule, PermissionInfo permission, int roleId, int userId, bool allowAccess)
        {
            var objModulePermission = new ModulePermissionInfo
            {
                ModuleID = objModule.ModuleID,
                PermissionID = permission.PermissionID,
                RoleID = roleId,
                UserID = userId,
                PermissionKey = permission.PermissionKey,
                AllowAccess = allowAccess
            };

            // add the permission to the collection
            if (!objModule.ModulePermissions.Contains(objModulePermission))
            {
                objModule.ModulePermissions.Add(objModulePermission);
            }

            return objModulePermission;
        }

        private int GetPaneModuleOrder(string pane, int sort)
        {
            var items = new List<int>();

            foreach (ModuleInfo m in PortalSettings.Current.ActiveTab.Modules)
            {
                //if user is allowed to view module and module is not deleted
                if (ModulePermissionController.CanViewModule(m) && !m.IsDeleted)
                {
                    //modules which are displayed on all tabs should not be displayed on the Admin or Super tabs
                    if (!m.AllTabs || !PortalSettings.Current.ActiveTab.IsSuperTab)
                    {
                        if (m.PaneName == pane)
                        {
                            int moduleOrder = m.ModuleOrder;

                            while (items.Contains(moduleOrder) || moduleOrder == 0)
                            {
                                moduleOrder++;
                            }

                            items.Add(moduleOrder);
                        }
                    }
                }
            }

            items.Sort();

            if(items.Count > sort)
            {
                var itemOrder = items[sort];
                return itemOrder - 1;
            }
            else if(items.Count > 0)
            {
                return items.Last() + 1;
            }

            return 0;
        }

        private void DoAddNewModule(string title, int desktopModuleId, string paneName, int position, int permissionType, string align)
        {
            TabPermissionCollection objTabPermissions = PortalSettings.Current.ActiveTab.TabPermissions;            
            var objPermissionController = new PermissionController();
            var objModules = new ModuleController();
            new EventLogController();

            try
            {
                DesktopModuleInfo desktopModule;
                if (!DesktopModuleController.GetDesktopModules(PortalSettings.Current.PortalId).TryGetValue(desktopModuleId, out desktopModule))
                {
                    throw new ArgumentException("desktopModuleId");
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }

            foreach (ModuleDefinitionInfo objModuleDefinition in
                ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(desktopModuleId).Values)
            {
                var objModule = new ModuleInfo();
                objModule.Initialize(PortalSettings.Current.ActiveTab.PortalID);

                objModule.PortalID = PortalSettings.Current.ActiveTab.PortalID;
                objModule.TabID = PortalSettings.Current.ActiveTab.TabID;
                objModule.ModuleOrder = position;
                objModule.ModuleTitle = string.IsNullOrEmpty(title) ? objModuleDefinition.FriendlyName : title;
                objModule.PaneName = paneName;
                objModule.ModuleDefID = objModuleDefinition.ModuleDefID;
                if (objModuleDefinition.DefaultCacheTime > 0)
                {
                    objModule.CacheTime = objModuleDefinition.DefaultCacheTime;
                    if (PortalSettings.Current.DefaultModuleId > Null.NullInteger && PortalSettings.Current.DefaultTabId > Null.NullInteger)
                    {
                        ModuleInfo defaultModule = objModules.GetModule(PortalSettings.Current.DefaultModuleId, PortalSettings.Current.DefaultTabId, true);
                        if ((defaultModule != null))
                        {
                            objModule.CacheTime = defaultModule.CacheTime;
                        }
                    }
                }

                switch (permissionType)
                {
                    case 0:
                        objModule.InheritViewPermissions = true;
                        break;
                    case 1:
                        objModule.InheritViewPermissions = false;
                        break;
                }

                // get the default module view permissions
                ArrayList arrSystemModuleViewPermissions = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "VIEW");

                // get the permissions from the page
                foreach (TabPermissionInfo objTabPermission in objTabPermissions)
                {
                    if (objTabPermission.PermissionKey == "VIEW" && permissionType == 0)
                    {
                        //Don't need to explicitly add View permisisons if "Same As Page"
                        continue;
                    }

                    // get the system module permissions for the permissionkey
                    ArrayList arrSystemModulePermissions = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", objTabPermission.PermissionKey);
                    // loop through the system module permissions
                    int j;
                    for (j = 0; j <= arrSystemModulePermissions.Count - 1; j++)
                    {
                        // create the module permission
                        var objSystemModulePermission = (PermissionInfo)arrSystemModulePermissions[j];
                        if (objSystemModulePermission.PermissionKey == "VIEW" && permissionType == 1 && objTabPermission.PermissionKey != "EDIT")
                        {
                            //Only Page Editors get View permissions if "Page Editors Only"
                            continue;
                        }

                        ModulePermissionInfo objModulePermission = AddModulePermission(objModule,
                                                                                       objSystemModulePermission,
                                                                                       objTabPermission.RoleID,
                                                                                       objTabPermission.UserID,
                                                                                       objTabPermission.AllowAccess);

                        // ensure that every EDIT permission which allows access also provides VIEW permission
                        if (objModulePermission.PermissionKey == "EDIT" && objModulePermission.AllowAccess)
                        {
                            AddModulePermission(objModule,
                                (PermissionInfo)arrSystemModuleViewPermissions[0],
                                objModulePermission.RoleID,
                                objModulePermission.UserID,
                                true);
                        }
                    }

                    //Get the custom Module Permissions,  Assume that roles with Edit Tab Permissions
                    //are automatically assigned to the Custom Module Permissions
                    if (objTabPermission.PermissionKey == "EDIT")
                    {
                        ArrayList arrCustomModulePermissions = objPermissionController.GetPermissionsByModuleDefID(objModule.ModuleDefID);

                        // loop through the custom module permissions
                        for (j = 0; j <= arrCustomModulePermissions.Count - 1; j++)
                        {
                            // create the module permission
                            var objCustomModulePermission = (PermissionInfo)arrCustomModulePermissions[j];

                            AddModulePermission(objModule, objCustomModulePermission, objTabPermission.RoleID, objTabPermission.UserID, objTabPermission.AllowAccess);
                        }
                    }
                }
                if (PortalSettings.Current.ContentLocalizationEnabled)
                {
                    Locale defaultLocale = LocaleController.Instance.GetDefaultLocale(PortalSettings.Current.PortalId);
                    //check whether original tab is exists, if true then set culture code to default language,
                    //otherwise set culture code to current.
                    if (new TabController().GetTabByCulture(objModule.TabID, PortalSettings.Current.PortalId, defaultLocale) != null)
                    {
                        objModule.CultureCode = defaultLocale.Code;
                    }
                    else
                    {
                        objModule.CultureCode = PortalSettings.Current.CultureCode;
                    }
                }
                else
                {
                    objModule.CultureCode = Null.NullString;
                }
                objModule.AllTabs = false;
                objModule.Alignment = align;

                objModules.AddModule(objModule);
            }
        }


    }
}
