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
using System.Linq;
using System.Web;
using System.Xml;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Entities.Tabs
{
    /// <summary>
    /// TabController provides all operation to tabinfo.
    /// </summary>
    /// <remarks>
    /// Tab is equal to page in DotNetNuke.
    /// Tabs will be a sitemap for a poatal, and every request at first need to check whether there is valid tab information
    /// include in the url, if not it will use default tab to display information.
    /// </remarks>
    public class TabController
    {
        private static readonly DataProvider Provider = DataProvider.Instance();

        /// <summary>
        /// Gets the current page in current http request.
        /// </summary>
        /// <value>Current Page Info.</value>
        public static TabInfo CurrentPage
        {
            get
            {
                TabInfo tab = null;
                if (PortalController.GetCurrentPortalSettings() != null)
                {
                    tab = PortalController.GetCurrentPortalSettings().ActiveTab;
                }
                return tab;
            }
        }

        #region Private Methods

        private static void AddAllTabsModules(TabInfo tab)
        {
            var objmodules = new ModuleController();
            foreach (ModuleInfo allTabsModule in objmodules.GetAllTabsModules(tab.PortalID, true))
            {
                //[DNN-6276]We need to check that the Module is not implicitly deleted.  ie If all instances are on Pages
                //that are all "deleted" then even if the Module itself is not deleted, we would not expect the 
                //Module to be added
                var canAdd =
                (from ModuleInfo allTabsInstance in objmodules.GetModuleTabs(allTabsModule.ModuleID) select new TabController().GetTab(allTabsInstance.TabID, tab.PortalID, false)).Any(
                    t => !t.IsDeleted);
                if (canAdd)
                {
                    objmodules.CopyModule(allTabsModule, tab, Null.NullString, true);
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a tab to the Datastore
        /// </summary>
        /// <param name="tab">The tab to be added</param>
        /// <param name="includeAllTabsModules">Include all Tab Modules</param>
        /// <history>
        /// 	[cnurse]	04/30/2008	Created
        /// 	[jlucarino]	02/26/2009	added CreatedByUserID
        /// </history>
        /// -----------------------------------------------------------------------------
        private int AddTabInternal(TabInfo tab, bool includeAllTabsModules)
        {
            tab.TabPath = Globals.GenerateTabPath(tab.ParentId, tab.TabName);
            var tabId = GetTabByTabPath(tab.PortalID, tab.TabPath, tab.CultureCode);
            if (tabId > Null.NullInteger)
            {
                //Tab exists so Throw
                throw new TabExistsException(tabId, string.Format("Page Exists in portal: {0}, path: {1}, culture: {2}", tab.PortalID, tab.TabPath, tab.CultureCode));
            }

            //First create ContentItem as we need the ContentItemID
            var typeController = new ContentTypeController();
            var contentType = (from t in typeController.GetContentTypes() where t.ContentType == "Tab" select t).SingleOrDefault();

            var contentController = Util.GetContentController();
            tab.Content = String.IsNullOrEmpty(tab.Title) ? tab.TabName : tab.Title;
            tab.ContentTypeId = contentType.ContentTypeId;
            tab.Indexed = false;
            int contentItemID = contentController.AddContentItem(tab);

            //Add Module
            tabId = Provider.AddTab(contentItemID,
                                    tab.PortalID,
                                    tab.UniqueId,
                                    tab.VersionGuid,
                                    tab.DefaultLanguageGuid,
                                    tab.LocalizedVersionGuid,
                                    tab.TabName,
                                    tab.IsVisible,
                                    tab.DisableLink,
                                    tab.ParentId,
                                    tab.IconFile,
                                    tab.IconFileLarge,
                                    tab.Title,
                                    tab.Description,
                                    tab.KeyWords,
                                    tab.Url,
                                    tab.SkinSrc,
                                    tab.ContainerSrc,
                                    tab.TabPath,
                                    tab.StartDate,
                                    tab.EndDate,
                                    tab.RefreshInterval,
                                    tab.PageHeadText,
                                    tab.IsSecure,
                                    tab.PermanentRedirect,
                                    tab.SiteMapPriority,
                                    UserController.GetCurrentUserInfo().UserID,
                                    tab.CultureCode);
            tab.TabID = tabId;

            //Clear the Cache
            ClearCache(tab.PortalID);


            //Now we have the TabID - update the contentItem
            contentController.UpdateContentItem(tab);

            var termController = Util.GetTermController();
            termController.RemoveTermsFromContent(tab);
            foreach (Term term in tab.Terms)
            {
                termController.AddTermToContent(term, tab);
            }

            var eventLog = new EventLogController();
            eventLog.AddLog(tab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.TAB_CREATED);

            //Add Tab Permissions
            TabPermissionController.SaveTabPermissions(tab);

            //Add TabSettings - use Try/catch as tabs are added during upgrade ptocess and the sproc may not exist
            try
            {
                UpdateTabSettings(ref tab);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }

            //Add AllTabs Modules
            if (includeAllTabsModules)
            {
                AddAllTabsModules(tab);
            }

            return tab.TabID;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a tab to the end of a List
        /// </summary>
        /// <param name="tab">The tab to be added</param>
        /// <param name="updateTabPath">A flag that indicates whether the TabPath is updated.</param>
        /// <history>
        /// 	[cnurse]	04/30/2008	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void AddTabToEndOfList(TabInfo tab, bool updateTabPath)
        {
            //Get the List of tabs with the same parent
            var siblingTabs = GetSiblingTabs(tab);

            //Get the Parent Tab
			if (!Null.IsNull(tab.ParentId))
			{
				var parentTab = GetTab(tab.ParentId, tab.PortalID, false);
				if (parentTab == null)
				{
					tab.Level = 0;
				}
				else
				{
					tab.Level = parentTab.Level + 1;
				}
			}

        	//Update the TabOrder for the Siblings
            UpdateTabOrder(siblingTabs, tab.CultureCode, tab.PortalID, 2);

            //UpdateOrder for the new tab
            tab.TabOrder = 2 * siblingTabs.Count + 1;
            UpdateTabOrder(tab, updateTabPath);
        }

        private void DeleteTabInternal(int tabId, int portalId)
        {
            //Get the tab from the Cache
            TabInfo tab = GetTab(tabId, portalId, false);

            //Delete ant tabModule Instances
            var moduleController = new ModuleController();
            foreach (var m in moduleController.GetTabModules(tabId).Values)
            {
                moduleController.DeleteTabModule(m.TabID, m.ModuleID, false);
            }

            //Delete Tab
            Provider.DeleteTab(tabId);

            //Remove the Content Item
            if (tab.ContentItemId > Null.NullInteger)
            {
                var contentController = Util.GetContentController();
                contentController.DeleteContentItem(tab);
            }

            //Log deletion
            var eventLog = new EventLogController();
            eventLog.AddLog("TabID", tabId.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.TAB_DELETED);
        }

        private void DemoteTab(TabInfo objTab, List<TabInfo> siblingTabs)
        {
            int siblingCount = siblingTabs.Count;

            //Get Tab's Index position in the Sibling List
            int tabIndex = GetIndexOfTab(objTab, siblingTabs);

            //Cannot demote tab that is the first sibling
            if (tabIndex > 0)
            {
                //All the siblings move up in the order
                UpdateTabOrder(siblingTabs, tabIndex + 1, siblingCount - 1, -2);

                //New parent is tab with index of tabIndex -1
                TabInfo parentTab = siblingTabs[tabIndex - 1];

                //Get the descendents now before the parent is updated
                var descendantTabs = GetTabsByPortal(objTab.PortalID).DescendentsOf(objTab.TabID);

                //Update the current tab and add to the end of the new parents list of children
                objTab.ParentId = parentTab.TabID;
                AddTabToEndOfList(objTab, true);

                //Update the Descendents of this tab
                UpdateDescendantLevel(descendantTabs, 1);
            }
        }

        private static bool DeleteChildTabs(int intTabid, PortalSettings PortalSettings, int UserId)
        {
            TabController objtabs = new TabController();
            bool bDeleted = true;
            foreach (TabInfo objtab in GetTabsByParent(intTabid, PortalSettings.PortalId))
            {
                bDeleted = DeleteTab(objtab, PortalSettings, UserId);
                if (!bDeleted)
                {
                    break;
                }
            }
            return bDeleted;
        }

        private static bool DeleteTab(TabInfo tabToDelete, PortalSettings PortalSettings, int UserId)
        {
            TabController objtabs = new TabController();
            bool bDeleted = true;
            if (!IsSpecialTab(tabToDelete.TabID, PortalSettings))
            {
                if (DeleteChildTabs(tabToDelete.TabID, PortalSettings, UserId))
                {
                    tabToDelete.IsDeleted = true;
                    objtabs.UpdateTab(tabToDelete);

                    ModuleController moduleCtrl = new ModuleController();
                    foreach (ModuleInfo m in moduleCtrl.GetTabModules(tabToDelete.TabID).Values)
                    {
                        moduleCtrl.DeleteTabModule(m.TabID, m.ModuleID, true);
                    }

                    EventLogController objEventLog = new EventLogController();
                    objEventLog.AddLog(tabToDelete, PortalSettings, UserId, "", EventLogController.EventLogType.TAB_SENT_TO_RECYCLE_BIN);
                }
                else
                {
                    bDeleted = false;
                }
            }
            else
            {
                bDeleted = false;
            }
            return bDeleted;
        }

        private static void DeserializeTabSettings(XmlNodeList nodeTabSettings, TabInfo objTab)
        {
            string sKey;
            string sValue;
            foreach (XmlNode oTabSettingNode in nodeTabSettings)
            {
                sKey = XmlUtils.GetNodeValue(oTabSettingNode.CreateNavigator(), "settingname");
                sValue = XmlUtils.GetNodeValue(oTabSettingNode.CreateNavigator(), "settingvalue");
                objTab.TabSettings[sKey] = sValue;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deserializes tab permissions
        /// </summary>
        /// <param name="nodeTabPermissions">Node for tab permissions</param>
        /// <param name="objTab">Tab being processed</param>
        /// <param name="IsAdminTemplate">Flag to indicate if we are parsing admin template</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[Vicenç]	15/10/2004	Created
        ///     [cnurse]    10/02/2007  Moved from PortalController
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void DeserializeTabPermissions(XmlNodeList nodeTabPermissions, TabInfo objTab, bool IsAdminTemplate)
        {
            PermissionController objPermissionController = new PermissionController();
            PermissionInfo objPermission;
            TabPermissionInfo objTabPermission;
            RoleController objRoleController = new RoleController();
            RoleInfo objRole;
            int RoleID;
            int PermissionID = 0;
            string PermissionKey;
            string PermissionCode;
            string RoleName;
            bool AllowAccess;
            ArrayList arrPermissions;
            int i;
            foreach (XmlNode xmlTabPermission in nodeTabPermissions)
            {
                PermissionKey = XmlUtils.GetNodeValue(xmlTabPermission.CreateNavigator(), "permissionkey");
                PermissionCode = XmlUtils.GetNodeValue(xmlTabPermission.CreateNavigator(), "permissioncode");
                RoleName = XmlUtils.GetNodeValue(xmlTabPermission.CreateNavigator(), "rolename");
                AllowAccess = XmlUtils.GetNodeValueBoolean(xmlTabPermission, "allowaccess");
                arrPermissions = objPermissionController.GetPermissionByCodeAndKey(PermissionCode, PermissionKey);
                for (i = 0; i <= arrPermissions.Count - 1; i++)
                {
                    objPermission = (PermissionInfo)arrPermissions[i];
                    PermissionID = objPermission.PermissionID;
                }
                RoleID = int.MinValue;
                switch (RoleName)
                {
                    case Globals.glbRoleAllUsersName:
                        RoleID = Convert.ToInt32(Globals.glbRoleAllUsers);
                        break;
                    case Globals.glbRoleUnauthUserName:
                        RoleID = Convert.ToInt32(Globals.glbRoleUnauthUser);
                        break;
                    default:
                        PortalController objPortals = new PortalController();
                        PortalInfo objPortal = objPortals.GetPortal(objTab.PortalID);
                        objRole = objRoleController.GetRoleByName(objPortal.PortalID, RoleName);
                        if (objRole != null)
                        {
                            RoleID = objRole.RoleID;
                        }
                        else
                        {
                            if (IsAdminTemplate && RoleName.ToLower() == "administrators")
                            {
                                RoleID = objPortal.AdministratorRoleId;
                            }
                        }
                        break;
                }
                if (RoleID != int.MinValue)
                {
                    objTabPermission = new TabPermissionInfo();
                    objTabPermission.TabID = objTab.TabID;
                    objTabPermission.PermissionID = PermissionID;
                    objTabPermission.RoleID = RoleID;
                    objTabPermission.AllowAccess = AllowAccess;
                    objTab.TabPermissions.Add(objTabPermission);
                }
            }
        }

        private static int GetIndexOfTab(TabInfo objTab, List<TabInfo> tabs)
        {
            int tabIndex = Null.NullInteger;
            for (int index = 0; index <= tabs.Count - 1; index++)
            {
                if (tabs[index].TabID == objTab.TabID)
                {
                    tabIndex = index;
                    break;
                }
            }
            return tabIndex;
        }

        private static int GetPortalId(int TabId, int PortalId)
        {
            if (Null.IsNull(PortalId))
            {
                var portalDic = PortalController.GetPortalDictionary();
                if (portalDic != null && portalDic.ContainsKey(TabId))
                {
                    PortalId = portalDic[TabId];
                }
            }
            return PortalId;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetTabsByPortalCallBack gets a Dictionary of Tabs by 
        /// Portal from the the Database.
        /// </summary>
        /// <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        /// needed for the database call</param>
        /// <history>
        /// 	[cnurse]	01/15/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static object GetTabsByPortalCallBack(CacheItemArgs cacheItemArgs)
        {
            int portalID = (int)cacheItemArgs.ParamList[0];
            var tabs = CBO.FillCollection<TabInfo>(Provider.GetTabs(portalID));
            return new TabCollection(tabs);
        }

        private static object GetTabPathDictionaryCallback(CacheItemArgs cacheItemArgs)
        {
            string cultureCode = Convert.ToString(cacheItemArgs.ParamList[0]);
            int portalID = (int)cacheItemArgs.ParamList[1];
            var tabpathDic = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            IDataReader dr = DataProvider.Instance().GetTabPaths(portalID, cultureCode);
            try
            {
                while (dr.Read())
                {
                    tabpathDic[Null.SetNullString(dr["TabPath"])] = Null.SetNullInteger(dr["TabID"]);
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
            return tabpathDic;
        }

        private List<TabInfo> GetSiblingTabs(TabInfo objTab)
        {
            return GetTabsByPortal(objTab.PortalID).WithCulture(objTab.CultureCode, true).WithParentId(objTab.ParentId);
        }

        private void PromoteTab(TabInfo objTab, List<TabInfo> siblingTabs)
        {
            int siblingCount = siblingTabs.Count;

            //Get the Parent Tab (we need to know the current position of the parent in)
            TabInfo parentTab = GetTab(objTab.ParentId, objTab.PortalID, false);
            if (parentTab != null)
            {
                //Get Tab's Index position in the Sibling List
                int tabIndex = GetIndexOfTab(objTab, siblingTabs);

                //All the siblings move up in the order
                UpdateTabOrder(siblingTabs, tabIndex + 1, siblingCount - 1, -2);

                //Get the siblings of the Parent
                siblingTabs = GetSiblingTabs(parentTab);
                siblingCount = siblingTabs.Count;

                //First make sure the list is sorted and spaced
                UpdateTabOrder(siblingTabs, objTab.CultureCode, objTab.PortalID, 2);

                //Get Parents Index position in the Sibling List
                int parentIndex = GetIndexOfTab(parentTab, siblingTabs);

                //We need to update the taborder for items that were after the parent
                UpdateTabOrder(siblingTabs, parentIndex + 1, siblingCount - 1, 2);

                //Get the descendents now before the parent is updated
                var descendantTabs = GetTabsByPortal(objTab.PortalID).DescendentsOf(objTab.TabID);

                //Update the current Tab
                objTab.ParentId = parentTab.ParentId;
                objTab.TabOrder = parentTab.TabOrder + 2;
                UpdateTab(objTab);

                //Update the current tabs level and tabpath
                if (objTab.Level < 0)
                {
                    objTab.Level = 0;
                }
                UpdateTabOrder(objTab, true);

                //Update the Descendents of this tab
                UpdateDescendantLevel(descendantTabs, -1);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Removes a tab from its current siblings
        /// </summary>
        /// <param name="objTab">Tab to remove</param>
        /// <history>
        /// 	[cnurse]	04/30/2008	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void RemoveTab(TabInfo objTab)
        {
            //Tab is being moved from the original list of siblings, so update the Taborder for the remaining tabs
            var siblingTabs = GetSiblingTabs(objTab).Where(t => t.TabID < objTab.TabID).ToList();

            UpdateTabOrder(siblingTabs, objTab.CultureCode, objTab.PortalID, 2);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates child tabs TabPath field
        /// </summary>
        /// <param name="intTabid">ID of the parent tab</param>
        /// <param name="portalId">Portal ID</param>
        /// <remarks>
        /// When a ParentTab is updated this method should be called to
        /// ensure that the TabPath of the Child Tabs is consistent with the Parent
        /// </remarks>
        /// <history>
        /// 	[JWhite]	16/11/2004	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void UpdateChildTabPath(int intTabid, int portalId)
        {
            foreach (TabInfo childTab in GetTabsByPortal(portalId).DescendentsOf(intTabid))
            {
                string oldTabPath = childTab.TabPath;
                childTab.TabPath = Globals.GenerateTabPath(childTab.ParentId, childTab.TabName);
                if (oldTabPath != childTab.TabPath)
                {
                    if (childTab.ContentItemId == Null.NullInteger && childTab.TabID != Null.NullInteger)
                    {
                        CreateContentItem(childTab);
                    }

                    Provider.UpdateTab(childTab.TabID,
                                       childTab.ContentItemId,
                                       childTab.PortalID,
                                       childTab.VersionGuid,
                                       childTab.DefaultLanguageGuid,
                                       childTab.LocalizedVersionGuid,
                                       childTab.TabName,
                                       childTab.IsVisible,
                                       childTab.DisableLink,
                                       childTab.ParentId,
                                       childTab.IconFile,
                                       childTab.IconFileLarge,
                                       childTab.Title,
                                       childTab.Description,
                                       childTab.KeyWords,
                                       childTab.IsDeleted,
                                       childTab.Url,
                                       childTab.SkinSrc,
                                       childTab.ContainerSrc,
                                       childTab.TabPath,
                                       childTab.StartDate,
                                       childTab.EndDate,
                                       childTab.RefreshInterval,
                                       childTab.PageHeadText,
                                       childTab.IsSecure,
                                       childTab.PermanentRedirect,
                                       childTab.SiteMapPriority,
                                       UserController.GetCurrentUserInfo().UserID,
                                       childTab.CultureCode);
                    UpdateTabVersion(childTab.TabID);
                    EventLogController objEventLog = new EventLogController();
                    objEventLog.AddLog("TabID",
                                       intTabid.ToString(),
                                       PortalController.GetCurrentPortalSettings(),
                                       UserController.GetCurrentUserInfo().UserID,
                                       EventLogController.EventLogType.TAB_UPDATED);
                }
            }
        }

        private void UpdateDescendantLevel(IEnumerable<TabInfo> descendantTabs, int levelDelta)
        {
            //Update the Descendents of this tab
            foreach (TabInfo descendent in descendantTabs)
            {
                descendent.Level = descendent.Level + levelDelta;
                UpdateTabOrder(descendent, true);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates the TabOrder for a single Tab
        /// </summary>
        /// <param name="objTab">The tab to be updated</param>
        /// <param name="updateTabPath">A flag that indicates whether the TabPath is updated.</param>
        /// <history>
        /// 	[cnurse]	04/30/2008	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void UpdateTabOrder(TabInfo objTab, bool updateTabPath)
        {
            if (updateTabPath)
            {
                objTab.TabPath = Globals.GenerateTabPath(objTab.ParentId, objTab.TabName);
            }
            Provider.UpdateTabOrder(objTab.TabID, objTab.TabOrder, objTab.Level, objTab.ParentId, objTab.TabPath, UserController.GetCurrentUserInfo().UserID);
            UpdateTabVersion(objTab.TabID);
            EventLogController objEventLog = new EventLogController();
            objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.TAB_ORDER_UPDATED);
            ClearCache(objTab.PortalID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates the TabOrder for a list of Tabs
        /// </summary>
        /// <param name="tabs">The List of tabs to be updated</param>
        /// <param name="startIndex">The index to start updating from</param>
        /// <param name="endIndex">The index to end updating to</param>
        /// <param name="increment">The increment to update each tabs TabOrder</param>
        /// <history>
        /// 	[cnurse]	04/30/2008	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void UpdateTabOrder(List<TabInfo> tabs, int startIndex, int endIndex, int increment)
        {
            for (int index = startIndex; index <= endIndex; index++)
            {
                TabInfo objTab = tabs[index];
                objTab.TabOrder += increment;

                //UpdateOrder - Parent hasn't changed so we don't need to regenerate TabPath
                UpdateTabOrder(objTab, false);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates the TabOrder for a list of Tabs
        /// </summary>
        /// <param name="tabs">The List of tabs to be updated</param>
        /// <param name="culture">culture code</param>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="increment">The increment to update each tabs TabOrder</param>
        /// <history>
        /// 	[cnurse]	06/09/2009	Created
        /// 	[kbeigi]	14/06/2010	Updated to only update the tab order only when needed
        /// </history>
        /// -----------------------------------------------------------------------------
        private void UpdateTabOrder(IEnumerable<TabInfo> tabs, string culture, int portalId, int increment)
        {
            PortalSettings _PortalSettings = PortalController.GetCurrentPortalSettings();
            if (_PortalSettings != null && _PortalSettings.ContentLocalizationEnabled)
            {
                if (string.IsNullOrEmpty(culture))
                {
                    UpdateTabOrderInternal(tabs.Where(t => t.CultureCode == _PortalSettings.DefaultLanguage || string.IsNullOrEmpty(t.CultureCode)), increment);
                }
                else
                {
                    UpdateTabOrderInternal(tabs, increment);
                }
            }
            else
            {
                UpdateTabOrderInternal(tabs, increment);
            }
        }

        private void UpdateTabOrderInternal(IEnumerable<TabInfo> tabs, int increment)
        {
            int tabOrder = 1;
            foreach (TabInfo objTab in tabs.OrderBy(t => t.TabOrder))
            {
                if (objTab.IsDeleted)
                {
                    objTab.TabOrder = -1;
                    UpdateTabOrder(objTab, false);

                    //Update the tab order of all child languages
                    foreach (TabInfo localizedtab in objTab.LocalizedTabs.Values)
                    {
                        localizedtab.TabOrder = -1;
                        UpdateTabOrder(localizedtab, false);
                    }
                }
                else
                {
                    //Only update the tabOrder if it actually needs to be updated
                    if ((objTab.TabOrder != tabOrder))
                    {
                        objTab.TabOrder = tabOrder;
                        UpdateTabOrder(objTab, false);

                        //Update the tab order of all child languages
                        foreach (TabInfo localizedtab in objTab.LocalizedTabs.Values)
                        {
                            if (localizedtab.TabOrder != tabOrder)
                            {
                                localizedtab.TabOrder = tabOrder;
                                UpdateTabOrder(localizedtab, false);
                            }
                        }
                    }

                    tabOrder += increment;
                }
            }
        }

        /// <summary>
        /// Updates the VersionGuid
        /// </summary>
        /// <param name="tabId"></param>
        /// <remarks></remarks>
        private static void UpdateTabVersion(int tabId)
        {
            Provider.UpdateTabVersion(tabId, Guid.NewGuid());
        }

        #endregion

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a tab
        /// </summary>
        /// <param name="objTab">The tab to be added</param>
        /// <remarks>The tab is added to the end of the current Level.</remarks>
        /// <history>
        /// 	[cnurse]	04/30/2008	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public int AddTab(TabInfo objTab)
        {
            return AddTab(objTab, true);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a tab
        /// </summary>
        /// <param name="objTab">The tab to be added</param>
        /// <param name="includeAllTabsModules">Flag that indicates whether to add the "AllTabs"
        /// Modules</param>
        /// <remarks>The tab is added to the end of the current Level.</remarks>
        /// <history>
        /// 	[cnurse]	04/30/2008	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public int AddTab(TabInfo objTab, bool includeAllTabsModules)
        {
            int tabID = AddTabInternal(objTab, includeAllTabsModules);
            AddTabToEndOfList(objTab, false);
            ClearCache(objTab.PortalID);
            return tabID;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a tab after the specified tab
        /// </summary>
        /// <param name="objTab">The tab to be added</param>
        /// <param name="afterTabId">Id of the tab after which this tab is added</param>
        /// <history>
        /// 	[cnurse]	04/30/2008	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int AddTabAfter(TabInfo objTab, int afterTabId)
        {
            //Get the List of tabs with the same parent
            var siblingTabs = GetSiblingTabs(objTab);

            //Add tab to store
            int tabID = AddTabInternal(objTab, true);

            //New tab is to be inserted into the siblings List after TabId=afterTabId
            TabInfo afterTab = siblingTabs.Find(t => t.TabID == afterTabId);
            if (afterTab == null)
            {
                //AfterTabId probably relates to a Tab in the current culture (objTab is in a different culture)
                afterTab = GetTab(afterTabId, objTab.PortalID, false);
            }
            objTab.Level = afterTab.Level;
            objTab.TabOrder = afterTab.TabOrder + 1; // tabs will be 1,3(afterTabId),4(newTab),5;

            siblingTabs.Add(objTab);

            //Sort and Update siblings
            UpdateTabOrder(siblingTabs, objTab.CultureCode, objTab.PortalID, 2);

            //Clear the Cache
            ClearCache(objTab.PortalID);
            return tabID;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a tab before the specified tab
        /// </summary>
        /// <param name="objTab">The tab to be added</param>
        /// <param name="beforeTabId">Id of the tab before which this tab is added</param>
        /// <history>
        /// 	[cnurse]	04/30/2008	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int AddTabBefore(TabInfo objTab, int beforeTabId)
        {
            //Get the List of tabs with the same parent
            var siblingTabs = GetSiblingTabs(objTab);

            //Add tab to store
            int tabID = AddTabInternal(objTab, true);

            //New tab is to be inserted into the siblings List before TabId=beforeTabId
            TabInfo beforeTab = siblingTabs.Find(t => t.TabID == beforeTabId);
            if (beforeTab == null)
            {
                //beforeTabId probably relates to a Tab in the current culture (objTab is in a different culture)
                beforeTab = GetTab(beforeTabId, objTab.PortalID, false);
            }
            objTab.Level = beforeTab.Level;
            objTab.TabOrder = beforeTab.TabOrder - 1; //tabs will be 1,3,4(newTab),5(beforeTabid)
            siblingTabs.Add(objTab);

            //Sort and Update siblings
            UpdateTabOrder(siblingTabs, objTab.CultureCode, objTab.PortalID, 2);

            //Clear the Cache
            ClearCache(objTab.PortalID);

            return tabID;
        }

        /// <summary>
        /// Clears tabs and portal cache for the specific portal.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        public void ClearCache(int portalId)
        {
            DataCache.ClearTabsCache(portalId);

            //Clear the Portal cache so the Pages count is correct
            DataCache.ClearPortalCache(portalId, false);

            DataCache.RemoveCache(DataCache.PortalDictionaryCacheKey);
        }

        /// <summary>
        /// Creates content item for the tab..
        /// </summary>
        /// <param name="updatedTab">The updated tab.</param>
        public void CreateContentItem(TabInfo updatedTab)
        {
            IContentTypeController typeController = new ContentTypeController();
            ContentType contentType = (from t in typeController.GetContentTypes()
                                       where t.ContentType == "Tab"
                                       select t).SingleOrDefault();

            //This tab does not have a valid ContentItem
            //create ContentItem
            IContentController contentController = Util.GetContentController();
            updatedTab.Content = string.IsNullOrEmpty(updatedTab.Title) ? updatedTab.TabName : updatedTab.Title;
            updatedTab.ContentTypeId = contentType.ContentTypeId;
            updatedTab.Indexed = false;
            updatedTab.ContentItemId = contentController.AddContentItem(updatedTab);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deletes a tab premanently from the database
        /// </summary>
        /// <param name="TabId">TabId of the tab to be deleted</param>
        /// <param name="PortalId">PortalId of the portal</param>
        /// <remarks>
        /// The tab will not delete if it has child tab(s).
        /// </remarks>
        /// <history>
        /// 	[Vicenç]	19/09/2004	Added skin deassignment before deleting the tab.
        /// </history>
        /// -----------------------------------------------------------------------------
        public void DeleteTab(int TabId, int PortalId)
        {
            //parent tabs can not be deleted
            if (GetTabsByPortal(PortalId).WithParentId(TabId).Count == 0)
            {
                DeleteTabInternal(TabId, PortalId);
                ClearCache(PortalId);
            }
        }

        /// <summary>
        /// Deletes a tab premanently from the database
        /// </summary>
        /// <param name="TabId">The tab id.</param>
        /// <param name="PortalId">The portal id.</param>
        /// <param name="deleteDescendants">if set to <c>true</c> will delete all child tabs.</param>
        /// <remarks>
        /// 
        /// </remarks>
        public void DeleteTab(int TabId, int PortalId, bool deleteDescendants)
        {
            var descendantList = GetTabsByPortal(PortalId).DescendentsOf(TabId);
            if (deleteDescendants && descendantList.Count > 0)
            {
                //Iterate through descendants from bottom - which will remove children first
                for (int i = descendantList.Count - 1; i >= 0; i += -1)
                {
                    DeleteTabInternal(descendantList[i].TabID, PortalId);
                }
                ClearCache(PortalId);
            }
            DeleteTab(TabId, PortalId);
        }

        /// <summary>
        /// Gets all tabs.
        /// </summary>
        /// <param name="CheckLegacyFields">do no effec to this method.</param>
        /// <returns>tab collection</returns>
        public ArrayList GetAllTabs(bool CheckLegacyFields)
        {
            return CBO.FillCollection(Provider.GetAllTabs(), typeof(TabInfo));
        }

        /// <summary>
        /// Gets all tabs.
        /// </summary>
        /// <returns>tab collection</returns>
        public ArrayList GetAllTabs()
        {
            return GetAllTabs(true);
        }

        /// <summary>
        /// Gets the tab.
        /// </summary>
        /// <param name="TabId">The tab id.</param>
        /// <param name="PortalId">The portal id.</param>
        /// <param name="ignoreCache">if set to <c>true</c> will get tab info directly from database.</param>
        /// <returns>tab info.</returns>
        public TabInfo GetTab(int TabId, int PortalId, bool ignoreCache)
        {
            TabInfo tab = null;
            //if we are using the cache
            if (ignoreCache || Host.Host.PerformanceSetting == Globals.PerformanceSettings.NoCaching) 
            {
                tab = CBO.FillObject<TabInfo>(Provider.GetTab(TabId));
            }
            else
            {
                //if we do not know the PortalId then try to find it in the Portals Dictionary using the TabId
                PortalId = GetPortalId(TabId, PortalId);

                //if we have the PortalId then try to get the TabInfo object
                tab = GetTabsByPortal(PortalId).WithTabId(TabId);
                if(tab==null)
                {
                    //Maybe we have the wrong PortalId - try host
                    tab = GetTabsByPortal(GetPortalId(TabId, Null.NullInteger)).WithTabId(TabId);                   
                }
                if (tab == null)
                {
                    DnnLog.Warn("Unable to find tabId {0} of portal {1}", TabId, PortalId);
                }
            }

            return tab;
        }

        /// <summary>
        /// Gets the tab by unique ID.
        /// </summary>
        /// <param name="UniqueID">The unique ID.</param>
        /// <returns>tab info.</returns>
        public TabInfo GetTabByUniqueID(Guid UniqueID)
        {
            TabInfo tab = null;
            tab = CBO.FillObject<TabInfo>(Provider.GetTabByUniqueID(UniqueID));
            return tab;
        }

        /// <summary>
        /// Gets the tab by culture.
        /// </summary>
        /// <param name="tabId">The tab id.</param>
        /// <param name="portalId">The portal id.</param>
        /// <param name="locale">The locale.</param>
        /// <returns>tab info.</returns>
        public TabInfo GetTabByCulture(int tabId, int portalId, Locale locale)
        {
            TabInfo originalTab = null;
            TabInfo localizedTab = null;
            TabCollection tabs = GetTabsByPortal(portalId);

            //Get Tab specified by Id
            originalTab = tabs.WithTabId(tabId);

            if (locale != null && originalTab != null)
            {
                //Check if tab is in the requested culture
                if (string.IsNullOrEmpty(originalTab.CultureCode) || originalTab.CultureCode == locale.Code)
                {
                    localizedTab = originalTab;
                }
                else
                {
                    //See if tab exists for culture
                    if (originalTab.IsDefaultLanguage)
                    {
                        originalTab.LocalizedTabs.TryGetValue(locale.Code, out localizedTab);
                    }
                    else
                    {
                        if (originalTab.DefaultLanguageTab != null)
                        {
                            if (originalTab.DefaultLanguageTab.CultureCode == locale.Code)
                            {
                                localizedTab = originalTab.DefaultLanguageTab;
                            }
                            else
                            {
                                if (!originalTab.DefaultLanguageTab.LocalizedTabs.TryGetValue(locale.Code, out localizedTab))
                                {
                                    localizedTab = originalTab.DefaultLanguageTab;
                                }
                            }
                        }
                    }
                }
            }
            return localizedTab;
        }

        /// <summary>
        /// Gets the name of the tab by name.
        /// </summary>
        /// <param name="TabName">Name of the tab.</param>
        /// <param name="PortalId">The portal id.</param>
        /// <returns>tab info.</returns>
        public TabInfo GetTabByName(string TabName, int PortalId)
        {
            return GetTabsByPortal(PortalId).WithTabName(TabName);
        }

        /// <summary>
        /// Gets the name of the tab by name and parent id.
        /// </summary>
        /// <param name="TabName">Name of the tab.</param>
        /// <param name="PortalId">The portal id.</param>
        /// <param name="ParentId">The parent id.</param>
        /// <returns>tab info</returns>
        public TabInfo GetTabByName(string TabName, int PortalId, int ParentId)
        {
            return GetTabsByPortal(PortalId).WithTabNameAndParentId(TabName, ParentId);
        }

        /// <summary>
        /// Gets the tab count in portal.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <returns>tab's count.</returns>
        public int GetTabCount(int portalId)
        {
            return GetTabsByPortal(portalId).Count;
        }

        /// <summary>
        /// Gets the tabs by portal.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <returns>tab collection</returns>
        public TabCollection GetTabsByPortal(int portalId)
        {
            string cacheKey = string.Format(DataCache.TabCacheKey, portalId);
            return CBO.GetCachedObject<TabCollection>(new CacheItemArgs(cacheKey, DataCache.TabCacheTimeOut, DataCache.TabCachePriority, portalId), GetTabsByPortalCallBack);
        }

        /// <summary>
        /// Gets the tabs which use the module.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <returns>tab collection</returns>
        public IDictionary<int, TabInfo> GetTabsByModuleID(int moduleID)
        {
            return CBO.FillDictionary<int, TabInfo>("TabID", Provider.GetTabsByModuleID(moduleID));
        }

        /// <summary>
        /// Gets the tabs which use the package.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="packageID">The package ID.</param>
        /// <param name="forHost">if set to <c>true</c> [for host].</param>
        /// <returns>tab collection</returns>
        public IDictionary<int, TabInfo> GetTabsByPackageID(int portalID, int packageID, bool forHost)
        {
            return CBO.FillDictionary<int, TabInfo>("TabID", Provider.GetTabsByPackageID(portalID, packageID, forHost));
        }

        /// <summary>
        /// Moves the tab by the tab move type.
        /// </summary>
        /// <param name="objTab">The obj tab.</param>
        /// <param name="type">The type.</param>
        /// <seealso cref="TabMoveType"/>
        /// <example>
        /// <code lang="C#">
        /// TabController tabCtrl = new TabController();
        /// tabCtrl.MoveTab(tab, TabMoveType.Up);
        /// </code>
        /// </example>
        public void MoveTab(TabInfo objTab, TabMoveType type)
        {
            //Get the List of tabs with the same parent
            var siblingTabs = GetSiblingTabs(objTab);
            int siblingCount = siblingTabs.Count;
            switch (type)
            {
                case TabMoveType.Top:
                    //Tab is being moved to the top of the current level - Set TabOrder = -1 to ensure it gets put at the top
                    objTab.TabOrder = 1;

                    //Sort and Update siblings
                    UpdateTabOrder(siblingTabs, objTab.CultureCode, objTab.PortalID, 2);
                    break;
                case TabMoveType.Bottom:
                    //Tab is being moved to the bottom of the current level - Set TabOrder = 2*TabCount - 1
                    objTab.TabOrder = siblingCount * 2 - 1;
                    //Sort and Update siblings
                    UpdateTabOrder(siblingTabs, objTab.CultureCode, objTab.PortalID, 2);
                    break;
                case TabMoveType.Up:
                    //Tab is being moved up one position in the current level
                    objTab.TabOrder -= 3;

                    //Sort and Update siblings
                    UpdateTabOrder(siblingTabs, objTab.CultureCode, objTab.PortalID, 2);
                    break;
                case TabMoveType.Down:
                    //Tab is being moved down one position in the current level
                    objTab.TabOrder += 3;

                    //Sort and Update siblings
                    UpdateTabOrder(siblingTabs, objTab.CultureCode, objTab.PortalID, 2);
                    break;
                case TabMoveType.Promote:
                    //Tab is being promoted to the next level up in the heirarchy
                    PromoteTab(objTab, siblingTabs);
                    break;
                case TabMoveType.Demote:
                    //Tab is being demoted to the next level down in the heirarchy
                    DemoteTab(objTab, siblingTabs);
                    break;
            }
            ClearCache(objTab.PortalID);
        }

        /// <summary>
        /// Moves the tab after a specific tab.
        /// </summary>
        /// <param name="objTab">The tab want to move.</param>
        /// <param name="afterTabId">will move objTab after this tab.</param>
        public void MoveTabAfter(TabInfo objTab, int afterTabId)
        {
            if ((objTab.TabID < 0))
            {
                return;
            }

            //Get the List of tabs with the same parent
            var siblingTabs = GetSiblingTabs(objTab);

            //tab is to be moved after TabId=afterTabId
            TabInfo afterTab = siblingTabs.Find(t => t.TabID == afterTabId);
            objTab.Level = afterTab.Level;
            objTab.TabOrder = afterTab.TabOrder + 1;
            // tabs will be 1,3(afterTabId),4(moveTab),5

            if (siblingTabs.Find(t => t.TabID == objTab.TabID) != null)
            {
                siblingTabs.Remove(siblingTabs.Find(t => t.TabID == objTab.TabID));
            }

            siblingTabs.Add(objTab);

            //Sort and Update siblings
            UpdateTabOrder(siblingTabs, objTab.CultureCode, objTab.PortalID, 2);

            //Clear the Cache
            ClearCache(objTab.PortalID);
        }

        /// <summary>
        /// Moves the tab before a specific tab.
        /// </summary>
        /// <param name="objTab">The tab want to move.</param>
        /// <param name="beforeTabId">will move objTab before this tab.</param>
        public void MoveTabBefore(TabInfo objTab, int beforeTabId)
        {
            if ((objTab.TabID < 0))
            {
                return;
            }

            //Get the List of tabs with the same parent
            var siblingTabs = GetSiblingTabs(objTab);

            //tab is to be moved after TabId=afterTabId
            TabInfo beforeTab = siblingTabs.Find(t => t.TabID == beforeTabId);
            objTab.Level = beforeTab.Level;
            objTab.TabOrder = beforeTab.TabOrder - 1;
            // tabs will be 1,3(afterTabId),4(moveTab),5

            if (siblingTabs.Find(t => t.TabID == objTab.TabID) != null)
            {
                siblingTabs.Remove(siblingTabs.Find(t => t.TabID == objTab.TabID));
            }

            siblingTabs.Add(objTab);

            //Sort and Update siblings
            UpdateTabOrder(siblingTabs, objTab.CultureCode, objTab.PortalID, 2);

            //Clear the Cache
            ClearCache(objTab.PortalID);
        }

        /// <summary>
        /// Populates the bread crumbs.
        /// </summary>
        /// <param name="tab">The tab.</param>
        public void PopulateBreadCrumbs(ref TabInfo tab)
        {
            if ((tab.BreadCrumbs == null))
            {
                ArrayList crumbs = new ArrayList();
                PopulateBreadCrumbs(tab.PortalID, ref crumbs, tab.TabID);
                tab.BreadCrumbs = crumbs;
            }
        }

        /// <summary>
        /// Populates the bread crumbs.
        /// </summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="breadCrumbs">The bread crumbs.</param>
        /// <param name="tabID">The tab ID.</param>
        public void PopulateBreadCrumbs(int portalID, ref ArrayList breadCrumbs, int tabID)
        {
            //find the tab in the tabs collection
            TabInfo objTab = null;
            TabController objTabController = new TabController();
            TabCollection portalTabs = objTabController.GetTabsByPortal(portalID);
            TabCollection hostTabs = objTabController.GetTabsByPortal(Null.NullInteger);
            bool blnFound = portalTabs.TryGetValue(tabID, out objTab);
            if (!blnFound)
            {
                blnFound = hostTabs.TryGetValue(tabID, out objTab);
            }
            //if tab was found
            if (blnFound)
            {
                breadCrumbs.Insert(0, objTab.Clone());

                //get the tab parent
                if (!Null.IsNull(objTab.ParentId))
                {
                    PopulateBreadCrumbs(portalID, ref breadCrumbs, objTab.ParentId);
                }
            }
        }

        /// <summary>
        /// Updates the tab to databse.
        /// </summary>
        /// <param name="updatedTab">The updated tab.</param>
        public void UpdateTab(TabInfo updatedTab)
        {
            TabInfo originalTab = GetTab(updatedTab.TabID, updatedTab.PortalID, true);
            bool updateOrder = (originalTab.ParentId != updatedTab.ParentId);
            int levelDelta = (updatedTab.Level - originalTab.Level);
            bool updateChildren = (originalTab.TabName != updatedTab.TabName || updateOrder);

            //Update ContentItem If neccessary
            if (updatedTab.ContentItemId == Null.NullInteger && updatedTab.TabID != Null.NullInteger)
            {
                CreateContentItem(updatedTab);
            }

            //Update Tab to DataStore
            Provider.UpdateTab(updatedTab.TabID,
                               updatedTab.ContentItemId,
                               updatedTab.PortalID,
                               updatedTab.VersionGuid,
                               updatedTab.DefaultLanguageGuid,
                               updatedTab.LocalizedVersionGuid,
                               updatedTab.TabName,
                               updatedTab.IsVisible,
                               updatedTab.DisableLink,
                               updatedTab.ParentId,
                               updatedTab.IconFile,
                               updatedTab.IconFileLarge,
                               updatedTab.Title,
                               updatedTab.Description,
                               updatedTab.KeyWords,
                               updatedTab.IsDeleted,
                               updatedTab.Url,
                               updatedTab.SkinSrc,
                               updatedTab.ContainerSrc,
                               updatedTab.TabPath,
                               updatedTab.StartDate,
                               updatedTab.EndDate,
                               updatedTab.RefreshInterval,
                               updatedTab.PageHeadText,
                               updatedTab.IsSecure,
                               updatedTab.PermanentRedirect,
                               updatedTab.SiteMapPriority,
                               UserController.GetCurrentUserInfo().UserID,
                               updatedTab.CultureCode);

            //Update Tags
            ITermController termController = Util.GetTermController();
            termController.RemoveTermsFromContent(updatedTab);
            foreach (Term _Term in updatedTab.Terms)
            {
                termController.AddTermToContent(_Term, updatedTab);
            }

            EventLogController objEventLog = new EventLogController();
            objEventLog.AddLog(updatedTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.TAB_UPDATED);

            //Update Tab permissions
            TabPermissionController.SaveTabPermissions(updatedTab);

            //Update TabSettings - use Try/catch as tabs are added during upgrade ptocess and the sproc may not exist
            try
            {
                UpdateTabSettings(ref updatedTab);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }

            //Updated Tab Level
            if (levelDelta != 0)
            {
                //Get the descendents
                var descendantTabs = GetTabsByPortal(updatedTab.PortalID).DescendentsOf(updatedTab.TabID, originalTab.Level);

                //Update the Descendents of this tab
                UpdateDescendantLevel(descendantTabs, levelDelta);
            }

            //Update Tab Order
            if (updateOrder)
            {
                //Tab is being moved from the original list of siblings, so update the Taborder for the remaining tabs
                RemoveTab(originalTab);

                //UpdateOrder - Parent has changed so we need to regenerate TabPath
                AddTabToEndOfList(updatedTab, true);
            }

            //Update Tab Path for descendents
            if (updateChildren)
            {
                //Clear Tab Cache to ensure that previous updates are picked up
                ClearCache(updatedTab.PortalID);
                UpdateChildTabPath(updatedTab.TabID, updatedTab.PortalID);
            }

            //Update Tab Version
            UpdateTabVersion(updatedTab.TabID);

            //Clear Tab Caches
            ClearCache(updatedTab.PortalID);
            if (updatedTab.PortalID != originalTab.PortalID)
            {
                ClearCache(originalTab.PortalID);
            }
        }

        /// <summary>
        /// Updates the tab settings.
        /// </summary>
        /// <param name="updatedTab">The updated tab.</param>
        private void UpdateTabSettings(ref TabInfo updatedTab)
        {
            string sKey = null;
            foreach (string sKey_loopVariable in updatedTab.TabSettings.Keys)
            {
                sKey = sKey_loopVariable;
                UpdateTabSetting(updatedTab.TabID, sKey, Convert.ToString(updatedTab.TabSettings[sKey]));
            }
        }

        /// <summary>
        /// Updates the tab order.
        /// </summary>
        /// <param name="objTab">The obj tab.</param>
        public void UpdateTabOrder(TabInfo objTab)
        {
            UpdateTabOrder(objTab, true);
        }

        /// <summary>
        /// Updates the translation status.
        /// </summary>
        /// <param name="localizedTab">The localized tab.</param>
        /// <param name="isTranslated">if set to <c>true</c> means the tab has already been translated.</param>
        public void UpdateTranslationStatus(TabInfo localizedTab, bool isTranslated)
        {
            if (isTranslated && (localizedTab.DefaultLanguageTab != null))
            {
                localizedTab.LocalizedVersionGuid = localizedTab.DefaultLanguageTab.LocalizedVersionGuid;
            }
            else
            {
                localizedTab.LocalizedVersionGuid = Guid.NewGuid();
            }
            DataProvider.Instance().UpdateTabTranslationStatus(localizedTab.TabID, localizedTab.LocalizedVersionGuid, UserController.GetCurrentUserInfo().UserID);

            //Clear Tab Caches
            ClearCache(localizedTab.PortalID);
        }

        /// <summary>
        /// read all settings for a tab from TabSettings table
        /// </summary>
        /// <param name="TabId">ID of the Tab to query</param>
        /// <returns>
        /// (cached) hashtable containing all settings
        /// </returns>
        /// <history>
        /// [jlucarino] 2009-08-31 Created
        ///   </history>
        public Hashtable GetTabSettings(int TabId)
        {
            Hashtable objSettings;
            string strCacheKey = "GetTabSettings" + TabId;
            objSettings = (Hashtable)DataCache.GetCache(strCacheKey);
            if (objSettings == null)
            {
                objSettings = new Hashtable();
                IDataReader dr = Provider.GetTabSettings(TabId);
                while (dr.Read())
                {
                    if (!dr.IsDBNull(1))
                    {
                        objSettings[dr.GetString(0)] = dr.GetString(1);
                    }
                    else
                    {
                        objSettings[dr.GetString(0)] = "";
                    }
                }
                dr.Close();

                //cache data
                int intCacheTimeout = 20 * Convert.ToInt32(Host.Host.PerformanceSetting);
                DataCache.SetCache(strCacheKey, objSettings, TimeSpan.FromMinutes(intCacheTimeout));
            }
            return objSettings;
        }

        /// <summary>
        /// Adds or updates a tab's setting value
        /// </summary>
        /// <param name="TabId">ID of the tab to update</param>
        /// <param name="SettingName">name of the setting property</param>
        /// <param name="SettingValue">value of the setting (String).</param>
        /// <remarks>empty SettingValue will remove the setting, if not preserveIfEmpty is true</remarks>
        /// <history>
        ///    [jlucarino] 2009-10-01 Created
        /// </history>
        public void UpdateTabSetting(int TabId, string SettingName, string SettingValue)
        {
            EventLogController objEventLog = new EventLogController();
            LogInfo objEventLogInfo = new LogInfo();
            objEventLogInfo.LogProperties.Add(new LogDetailInfo("TabId", TabId.ToString()));
            objEventLogInfo.LogProperties.Add(new LogDetailInfo("SettingName", SettingName));
            objEventLogInfo.LogProperties.Add(new LogDetailInfo("SettingValue", SettingValue));
            IDataReader dr = Provider.GetTabSetting(TabId, SettingName);
            if (dr.Read())
            {
                Provider.UpdateTabSetting(TabId, SettingName, SettingValue, UserController.GetCurrentUserInfo().UserID);
                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.TAB_SETTING_UPDATED.ToString();
                objEventLog.AddLog(objEventLogInfo);
            }
            else
            {
                Provider.AddTabSetting(TabId, SettingName, SettingValue, UserController.GetCurrentUserInfo().UserID);
                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.TAB_SETTING_CREATED.ToString();
                objEventLog.AddLog(objEventLogInfo);
            }
            dr.Close();

            UpdateTabVersion(TabId);
            DataCache.RemoveCache("GetTabSettings" + TabId);
        }

        /// <summary>
        /// Delete a Setting of a tab instance
        /// </summary>
        /// <param name="TabId">ID of the affected tab</param>
        /// <param name="SettingName">Name of the setting to be deleted</param>
        /// <history>
        ///    [jlucarino] 2009-10-01 Created
        /// </history>
        public void DeleteTabSetting(int TabId, string SettingName)
        {
            Provider.DeleteTabSetting(TabId, SettingName);
            EventLogController objEventLog = new EventLogController();
            LogInfo objEventLogInfo = new LogInfo();
            objEventLogInfo.LogProperties.Add(new LogDetailInfo("TabID", TabId.ToString()));
            objEventLogInfo.LogProperties.Add(new LogDetailInfo("SettingName", SettingName));
            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.TAB_SETTING_DELETED.ToString();
            objEventLog.AddLog(objEventLogInfo);

            UpdateTabVersion(TabId);
            DataCache.RemoveCache("GetTabSettings" + TabId);
        }

        /// <summary>
        /// Delete all Settings of a tab instance
        /// </summary>
        /// <param name="TabId">ID of the affected tab</param>
        /// <history>
        ///    [jlucarino] 2009-10-01 Created
        /// </history>
        public void DeleteTabSettings(int TabId)
        {
            Provider.DeleteTabSettings(TabId);
            EventLogController objEventLog = new EventLogController();
            LogInfo objEventLogInfo = new LogInfo();
            objEventLogInfo.LogProperties.Add(new LogDetailInfo("TabId", TabId.ToString()));
            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.TAB_SETTING_DELETED.ToString();
            objEventLog.AddLog(objEventLogInfo);
            UpdateTabVersion(TabId);
            DataCache.RemoveCache("GetTabSettings" + TabId);
        }

        /// <summary>
        /// Copies the design to children.
        /// </summary>
        /// <param name="parentTab">The parent tab.</param>
        /// <param name="skinSrc">The skin SRC.</param>
        /// <param name="containerSrc">The container SRC.</param>
        public static void CopyDesignToChildren(TabInfo parentTab, string skinSrc, string containerSrc)
        {
            CopyDesignToChildren(parentTab, skinSrc, containerSrc, PortalController.GetActivePortalLanguage(parentTab.PortalID));
        }

        /// <summary>
        /// Copies the design to children.
        /// </summary>
        /// <param name="parentTab">The parent tab.</param>
        /// <param name="skinSrc">The skin SRC.</param>
        /// <param name="containerSrc">The container SRC.</param>
        /// <param name="CultureCode">The culture code.</param>
        public static void CopyDesignToChildren(TabInfo parentTab, string skinSrc, string containerSrc, string CultureCode)
        {
            bool clearCache = Null.NullBoolean;
            var childTabs = new TabController().GetTabsByPortal(parentTab.PortalID).DescendentsOf(parentTab.TabID);
            TabController objTabController = new TabController();
            foreach (TabInfo objTab in childTabs)
            {
                if (TabPermissionController.CanAdminPage(objTab))
                {
                    Provider.UpdateTab(objTab.TabID,
                                       objTab.ContentItemId,
                                       objTab.PortalID,
                                       objTab.VersionGuid,
                                       objTab.DefaultLanguageGuid,
                                       objTab.LocalizedVersionGuid,
                                       objTab.TabName,
                                       objTab.IsVisible,
                                       objTab.DisableLink,
                                       objTab.ParentId,
                                       objTab.IconFile,
                                       objTab.IconFileLarge,
                                       objTab.Title,
                                       objTab.Description,
                                       objTab.KeyWords,
                                       objTab.IsDeleted,
                                       objTab.Url,
                                       skinSrc,
                                       containerSrc,
                                       objTab.TabPath,
                                       objTab.StartDate,
                                       objTab.EndDate,
                                       objTab.RefreshInterval,
                                       objTab.PageHeadText,
                                       objTab.IsSecure,
                                       objTab.PermanentRedirect,
                                       objTab.SiteMapPriority,
                                       UserController.GetCurrentUserInfo().UserID,
                                       objTab.CultureCode);

                    UpdateTabVersion(objTab.TabID);

                    EventLogController objEventLog = new EventLogController();
                    objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.TAB_UPDATED);
                    clearCache = true;
                }
            }
            if (clearCache)
            {
                DataCache.ClearTabsCache(childTabs[0].PortalID);
            }
        }

        /// <summary>
        /// Copies the permissions to children.
        /// </summary>
        /// <param name="parentTab">The parent tab.</param>
        /// <param name="newPermissions">The new permissions.</param>
        public static void CopyPermissionsToChildren(TabInfo parentTab, TabPermissionCollection newPermissions)
        {
            TabPermissionController objTabPermissionController = new TabPermissionController();
            bool clearCache = Null.NullBoolean;
            TabController objTabController = new TabController();
            var childTabs = new TabController().GetTabsByPortal(parentTab.PortalID).DescendentsOf(parentTab.TabID);
            foreach (TabInfo objTab in childTabs)
            {
                if (TabPermissionController.CanAdminPage(objTab))
                {
                    objTab.TabPermissions.Clear();
                    objTab.TabPermissions.AddRange(newPermissions);
                    TabPermissionController.SaveTabPermissions(objTab);
                    UpdateTabVersion(objTab.TabID);
                    clearCache = true;
                }
            }
            if (clearCache)
            {
                DataCache.ClearTabsCache(childTabs[0].PortalID);
            }
        }

        /// <summary>
        /// Deletes the tab.
        /// </summary>
        /// <param name="tabId">The tab id.</param>
        /// <param name="PortalSettings">The portal settings.</param>
        /// <param name="UserId">The user id.</param>
        /// <returns></returns>
        public static bool DeleteTab(int tabId, PortalSettings PortalSettings, int UserId)
        {
            bool bDeleted = true;
            TabController objController = new TabController();
            TabInfo objTab = objController.GetTab(tabId, PortalSettings.PortalId, false);
            if (objTab != null)
            {
                if (objTab.DefaultLanguageTab != null)
                {
                    //We are trying to delete the child, so recall this function with the master language's tab id
                    return DeleteTab(objTab.DefaultLanguageTab.TabID, PortalSettings, UserId);
                }
                //Delete Tab
                bDeleted = DeleteTab(objTab, PortalSettings, UserId);

                //Delete any localized children
                if (bDeleted)
                {
                    foreach (TabInfo localizedtab in objTab.LocalizedTabs.Values)
                    {
                        DeleteTab(localizedtab, PortalSettings, UserId);
                    }
                }

                //Get the List of tabs with the same parent
                var siblingTabs = objController.GetSiblingTabs(objTab);

                //Update TabOrder
                objController.UpdateTabOrder(siblingTabs, objTab.CultureCode, PortalSettings.PortalId, 2);
            }
            else
            {
                bDeleted = false;
            }
            return bDeleted;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Processes all panes and modules in the template file
        /// </summary>
        /// <param name="nodePanes">Template file node for the panes is current tab</param>
        /// <param name="PortalId">PortalId of the new portal</param>
        /// <param name="TabId">Tab being processed</param>
        /// <param name="mergeTabs">Tabs need to merge.</param>
        /// <param name="hModules">Modules Hashtable.</param>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	03/09/2004	Created
        /// 	[VMasanas]	15/10/2004	Modified for new skin structure
        ///		[cnurse]	15/10/2004	Modified to allow for merging template
        ///								with existing pages
        ///     [cnurse]    10/02/2007  Moved from PortalController
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeserializePanes(XmlNode nodePanes, int PortalId, int TabId, PortalTemplateModuleAction mergeTabs, Hashtable hModules)
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule;

            var dicModules = objModules.GetTabModules(TabId);

            //If Mode is Replace remove all the modules already on this Tab
            if (mergeTabs == PortalTemplateModuleAction.Replace)
            {
                foreach (KeyValuePair<int, ModuleInfo> kvp in dicModules)
                {
                    objModule = kvp.Value;
                    objModules.DeleteTabModule(TabId, objModule.ModuleID, false);
                }
            }

            //iterate through the panes
            foreach (XmlNode nodePane in nodePanes.ChildNodes)
            {
                //iterate through the modules
                if (nodePane.SelectSingleNode("modules") != null)
                {
                    foreach (XmlNode nodeModule in nodePane.SelectSingleNode("modules"))
                    {
                        ModuleController.DeserializeModule(nodeModule, nodePane, PortalId, TabId, mergeTabs, hModules);
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes the tab.
        /// </summary>
        /// <param name="nodeTab">The node tab.</param>
        /// <param name="objTab">The obj tab.</param>
        /// <param name="PortalId">The portal id.</param>
        /// <param name="mergeTabs">The merge tabs.</param>
        /// <returns></returns>
        public static TabInfo DeserializeTab(XmlNode nodeTab, TabInfo objTab, int PortalId, PortalTemplateModuleAction mergeTabs)
        {
            return DeserializeTab(nodeTab, objTab, new Hashtable(), PortalId, false, mergeTabs, new Hashtable());
        }

        /// <summary>
        /// Deserializes the tab.
        /// </summary>
        /// <param name="nodeTab">The node tab.</param>
        /// <param name="objTab">The obj tab.</param>
        /// <param name="hTabs">The h tabs.</param>
        /// <param name="PortalId">The portal id.</param>
        /// <param name="IsAdminTemplate">if set to <c>true</c> [is admin template].</param>
        /// <param name="mergeTabs">The merge tabs.</param>
        /// <param name="hModules">The h modules.</param>
        /// <returns></returns>
        public static TabInfo DeserializeTab(XmlNode nodeTab, TabInfo objTab, Hashtable hTabs, int PortalId, bool IsAdminTemplate, PortalTemplateModuleAction mergeTabs, Hashtable hModules)
        {
            TabController objTabs = new TabController();
            string tabName = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "name");
            if (!String.IsNullOrEmpty(tabName))
            {
                if (objTab == null)
                {
                    objTab = new TabInfo();
                    objTab.TabID = Null.NullInteger;
                    objTab.ParentId = Null.NullInteger;
                    objTab.TabName = tabName;
                }
                objTab.PortalID = PortalId;
                objTab.Title = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "title");
                objTab.Description = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "description");
                objTab.KeyWords = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "keywords");
                objTab.IsVisible = XmlUtils.GetNodeValueBoolean(nodeTab, "visible", true);
                objTab.DisableLink = XmlUtils.GetNodeValueBoolean(nodeTab, "disabled");
                objTab.IconFile = Globals.ImportFile(PortalId, XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "iconfile"));
                objTab.IconFileLarge = Globals.ImportFile(PortalId, XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "iconfilelarge"));
                objTab.Url = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "url");
                objTab.StartDate = XmlUtils.GetNodeValueDate(nodeTab, "startdate", Null.NullDate);
                objTab.EndDate = XmlUtils.GetNodeValueDate(nodeTab, "enddate", Null.NullDate);
                objTab.RefreshInterval = XmlUtils.GetNodeValueInt(nodeTab, "refreshinterval", Null.NullInteger);
                objTab.PageHeadText = XmlUtils.GetNodeValue(nodeTab, "pageheadtext", Null.NullString);
                objTab.IsSecure = XmlUtils.GetNodeValueBoolean(nodeTab, "issecure", false);
                objTab.SiteMapPriority = XmlUtils.GetNodeValueSingle(nodeTab, "sitemappriority", (float)0.5);
                //objTab.UniqueId = New Guid(XmlUtils.GetNodeValue(nodeTab, "guid", Guid.NewGuid.ToString()));
                //objTab.VersionGuid = New Guid(XmlUtils.GetNodeValue(nodeTab, "versionGuid", Guid.NewGuid.ToString()));
                objTab.TabPermissions.Clear();
                DeserializeTabPermissions(nodeTab.SelectNodes("tabpermissions/permission"), objTab, IsAdminTemplate);

                DeserializeTabSettings(nodeTab.SelectNodes("tabsettings/tabsetting"), objTab);

                //set tab skin and container
                if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeTab, "skinsrc", "")))
                {
                    objTab.SkinSrc = XmlUtils.GetNodeValue(nodeTab, "skinsrc", "");
                }
                if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeTab, "containersrc", "")))
                {
                    objTab.ContainerSrc = XmlUtils.GetNodeValue(nodeTab, "containersrc", "");
                }
                tabName = objTab.TabName;
                if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent")))
                {
                    if (hTabs[XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent")] != null)
                    {
                        //parent node specifies the path (tab1/tab2/tab3), use saved tabid
                        objTab.ParentId = Convert.ToInt32(hTabs[XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent")]);
                        tabName = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent") + "/" + objTab.TabName;
                    }
                    else
                    {
                        //Parent node doesn't spcecify the path, search by name.
                        //Possible incoherence if tabname not unique
                        TabInfo objParent = objTabs.GetTabByName(XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent"), PortalId);
                        if (objParent != null)
                        {
                            objTab.ParentId = objParent.TabID;
                            tabName = objParent.TabName + "/" + objTab.TabName;
                        }
                        else
                        {
                            //parent tab not found!
                            objTab.ParentId = Null.NullInteger;
                            tabName = objTab.TabName;
                        }
                    }
                }

                //create/update tab
                if (objTab.TabID == Null.NullInteger)
                {
                    objTab.TabID = objTabs.AddTab(objTab);
                }
                else
                {
                    objTabs.UpdateTab(objTab);
                }

                //extra check for duplicate tabs in same level
                if (hTabs[tabName] == null)
                {
                    hTabs.Add(tabName, objTab.TabID);
                }
            }

            //Parse Panes
            if (nodeTab.SelectSingleNode("panes") != null)
            {
                DeserializePanes(nodeTab.SelectSingleNode("panes"), PortalId, objTab.TabID, mergeTabs, hModules);
            }

            //Finally add "tabid" to node
            nodeTab.AppendChild(XmlUtils.CreateElement(nodeTab.OwnerDocument, "tabid", objTab.TabID.ToString()));
            return objTab;
        }

        /// <summary>
        /// Gets the portal tabs.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <param name="excludeTabId">The exclude tab id.</param>
        /// <param name="includeNoneSpecified">if set to <c>true</c> [include none specified].</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        public static List<TabInfo> GetPortalTabs(int portalId, int excludeTabId, bool includeNoneSpecified, bool includeHidden)
        {
            return GetPortalTabs(GetTabsBySortOrder(portalId, PortalController.GetActivePortalLanguage(portalId), true),
                                 excludeTabId,
                                 includeNoneSpecified,
                                 "<" + Localization.GetString("None_Specified") + ">",
                                 includeHidden,
                                 false,
                                 false,
                                 false,
                                 false);
        }

        /// <summary>
        /// Gets the portal tabs.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <param name="excludeTabId">The exclude tab id.</param>
        /// <param name="includeNoneSpecified">if set to <c>true</c> [include none specified].</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
        /// <param name="includeURL">if set to <c>true</c> [include URL].</param>
        /// <returns></returns>
        public static List<TabInfo> GetPortalTabs(int portalId, int excludeTabId, bool includeNoneSpecified, bool includeHidden, bool includeDeleted, bool includeURL)
        {
            return GetPortalTabs(GetTabsBySortOrder(portalId, PortalController.GetActivePortalLanguage(portalId), true),
                                 excludeTabId,
                                 includeNoneSpecified,
                                 "<" + Localization.GetString("None_Specified") + ">",
                                 includeHidden,
                                 includeDeleted,
                                 includeURL,
                                 false,
                                 false);
        }

        /// <summary>
        /// Gets the portal tabs.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <param name="excludeTabId">The exclude tab id.</param>
        /// <param name="includeNoneSpecified">if set to <c>true</c> [include none specified].</param>
        /// <param name="NoneSpecifiedText">The none specified text.</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
        /// <param name="includeURL">if set to <c>true</c> [include URL].</param>
        /// <param name="checkViewPermisison">if set to <c>true</c> [check view permisison].</param>
        /// <param name="checkEditPermission">if set to <c>true</c> [check edit permission].</param>
        /// <returns></returns>
        public static List<TabInfo> GetPortalTabs(int portalId, int excludeTabId, bool includeNoneSpecified, string NoneSpecifiedText, bool includeHidden, bool includeDeleted, bool includeURL,
                                                  bool checkViewPermisison, bool checkEditPermission)
        {
            return GetPortalTabs(GetTabsBySortOrder(portalId, PortalController.GetActivePortalLanguage(portalId), true),
                                 excludeTabId,
                                 includeNoneSpecified,
                                 NoneSpecifiedText,
                                 includeHidden,
                                 includeDeleted,
                                 includeURL,
                                 checkViewPermisison,
                                 checkEditPermission);
        }

        /// <summary>
        /// Gets the portal tabs.
        /// </summary>
        /// <param name="tabs">The tabs.</param>
        /// <param name="excludeTabId">The exclude tab id.</param>
        /// <param name="includeNoneSpecified">if set to <c>true</c> [include none specified].</param>
        /// <param name="NoneSpecifiedText">The none specified text.</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
        /// <param name="includeURL">if set to <c>true</c> [include URL].</param>
        /// <param name="checkViewPermisison">if set to <c>true</c> [check view permisison].</param>
        /// <param name="checkEditPermission">if set to <c>true</c> [check edit permission].</param>
        /// <returns></returns>
        public static List<TabInfo> GetPortalTabs(List<TabInfo> tabs, int excludeTabId, bool includeNoneSpecified, string NoneSpecifiedText, bool includeHidden, bool includeDeleted, bool includeURL,
                                                  bool checkViewPermisison, bool checkEditPermission)
        {
            var listTabs = new List<TabInfo>();
            if (includeNoneSpecified)
            {
                TabInfo objTab = new TabInfo();
                objTab.TabID = -1;
                objTab.TabName = NoneSpecifiedText;
                objTab.TabOrder = 0;
                objTab.ParentId = -2;
                listTabs.Add(objTab);
            }
            foreach (TabInfo objTab in tabs)
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                if (((excludeTabId < 0) || (objTab.TabID != excludeTabId)) && (!objTab.IsSuperTab || objUserInfo.IsSuperUser))
                {
                    if ((objTab.IsVisible || includeHidden) && (objTab.IsDeleted == false || includeDeleted) && (objTab.TabType == TabType.Normal || includeURL))
                    {
                        //Check if User has View/Edit Permission for this tab
                        if (checkEditPermission || checkViewPermisison)
                        {
                            string permissionList = "ADD,COPY,EDIT,MANAGE";
                            if (checkEditPermission && TabPermissionController.HasTabPermission(objTab.TabPermissions, permissionList))
                            {
                                listTabs.Add(objTab);
                            }
                            else if (checkViewPermisison && TabPermissionController.CanViewPage(objTab))
                            {
                                listTabs.Add(objTab);
                            }
                        }
                        else
                        {
                            //Add Tab to List
                            listTabs.Add(objTab);
                        }
                    }
                }
            }
            return listTabs;
        }

        /// <summary>
        /// Gets the tab by tab path.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <param name="tabPath">The tab path.</param>
        /// <param name="cultureCode">The culture code.</param>
        /// <returns></returns>
        public static int GetTabByTabPath(int portalId, string tabPath, string cultureCode)
        {
            var tabpathDic = GetTabPathDictionary(portalId, cultureCode);
            if (tabpathDic.ContainsKey(tabPath))
            {
                return tabpathDic[tabPath];
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the tab path dictionary.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <param name="cultureCode">The culture code.</param>
        /// <returns></returns>
        public static Dictionary<string, int> GetTabPathDictionary(int portalId, string cultureCode)
        {
            string cacheKey = string.Format(DataCache.TabPathCacheKey, cultureCode, portalId);
            return CBO.GetCachedObject<Dictionary<string, int>>(new CacheItemArgs(cacheKey, DataCache.TabPathCacheTimeOut, DataCache.TabPathCachePriority, cultureCode, portalId),
                                                                GetTabPathDictionaryCallback);
        }

        /// <summary>
        /// Gets the tabs by parent.
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <param name="portalId">The portal id.</param>
        /// <returns></returns>
        public static List<TabInfo> GetTabsByParent(int parentId, int portalId)
        {
            return new TabController().GetTabsByPortal(portalId).WithParentId(parentId);
        }

        /// <summary>
        /// Gets the tabs by sort order.
        /// </summary>
        /// <param name="portalId">The portal id.</param>
        /// <param name="cultureCode">The culture code.</param>
        /// <param name="includeNeutral">if set to <c>true</c> [include neutral].</param>
        /// <returns></returns>
        public static List<TabInfo> GetTabsBySortOrder(int portalId, string cultureCode, bool includeNeutral)
        {
            return new TabController().GetTabsByPortal(portalId).WithCulture(cultureCode, includeNeutral).AsList();
        }

        /// <summary>
        /// Determines whether is special tab.
        /// </summary>
        /// <param name="tabId">The tab id.</param>
        /// <param name="PortalSettings">The portal settings.</param>
        /// <returns>
        ///   <c>true</c> if is special tab; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSpecialTab(int tabId, PortalSettings PortalSettings)
        {
            return tabId == PortalSettings.SplashTabId || tabId == PortalSettings.HomeTabId || tabId == PortalSettings.LoginTabId || tabId == PortalSettings.UserTabId ||
                   tabId == PortalSettings.AdminTabId || tabId == PortalSettings.SuperTabId;
        }

        /// <summary>
        /// Restores the tab.
        /// </summary>
        /// <param name="objTab">The obj tab.</param>
        /// <param name="PortalSettings">The portal settings.</param>
        /// <param name="UserId">The user id.</param>
        public static void RestoreTab(TabInfo objTab, PortalSettings PortalSettings, int UserId)
        {
            EventLogController objEventLog = new EventLogController();
            TabController objController = new TabController();

            if (objTab.DefaultLanguageTab != null)
            {
                //We are trying to restore the child, so recall this function with the master language's tab id
                RestoreTab(objTab.DefaultLanguageTab, PortalSettings, UserId);
                return;
            }

            objTab.IsDeleted = false;
            objController.UpdateTab(objTab);

            //Restore any localized children
            foreach (TabInfo localizedtab in objTab.LocalizedTabs.Values)
            {
                localizedtab.IsDeleted = false;
                objController.UpdateTab(localizedtab);
            }

            var siblingTabs = objController.GetSiblingTabs(objTab);

            objController.UpdateTabOrder(siblingTabs, objTab.CultureCode, PortalSettings.PortalId, 2);

            objEventLog.AddLog(objTab, PortalSettings, UserId, "", EventLogController.EventLogType.TAB_RESTORED);
            ModuleController objmodules = new ModuleController();
            ArrayList arrMods = objmodules.GetAllTabsModules(objTab.PortalID, true);
            foreach (ModuleInfo objModule in arrMods)
            {
                objmodules.CopyModule(objModule, objTab, Null.NullString, true);
            }
            objController.ClearCache(objTab.PortalID);
        }

        /// <summary>
        /// SerializeTab
        /// </summary>
        /// <param name="xmlTab">The Xml Document to use for the Tab</param>
        /// <param name="objTab">The TabInfo object to serialize</param>
        /// <param name="includeContent">A flag used to determine if the Module content is included</param>
        public static XmlNode SerializeTab(XmlDocument xmlTab, TabInfo objTab, bool includeContent)
        {
            return SerializeTab(xmlTab, null, objTab, null, includeContent);
        }

        /// <summary>
        /// SerializeTab
        /// </summary>
        /// <param name="xmlTab">The Xml Document to use for the Tab</param>
        /// <param name="hTabs">A Hashtable used to store the names of the tabs</param>
        /// <param name="objTab">The TabInfo object to serialize</param>
        /// <param name="objPortal">The Portal object to which the tab belongs</param>
        /// <param name="includeContent">A flag used to determine if the Module content is included</param>
        public static XmlNode SerializeTab(XmlDocument xmlTab, Hashtable hTabs, TabInfo objTab, PortalInfo objPortal, bool includeContent)
        {
            XmlNode nodeTab;
            XmlNode urlNode;
            XmlNode newnode;
            CBO.SerializeObject(objTab, xmlTab);

            nodeTab = xmlTab.SelectSingleNode("tab");
            nodeTab.Attributes.Remove(nodeTab.Attributes["xmlns:xsd"]);
            nodeTab.Attributes.Remove(nodeTab.Attributes["xmlns:xsi"]);

            //remove unwanted elements
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("tabid"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("moduleID"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("taborder"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("portalid"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("parentid"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("isdeleted"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("tabpath"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("haschildren"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("skindoctype"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("uniqueid"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("versionguid"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("cultureCode"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("defaultLanguageGuid"));
            nodeTab.RemoveChild(nodeTab.SelectSingleNode("localizedVersionGuid"));
            foreach (XmlNode nodePermission in nodeTab.SelectNodes("tabpermissions/permission"))
            {
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("tabpermissionid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("permissionid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("tabid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("roleid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("userid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("username"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("displayname"));
            }

            //Manage Url
            urlNode = xmlTab.SelectSingleNode("tab/url");
            switch (objTab.TabType)
            {
                case TabType.Normal:
                    urlNode.Attributes.Append(XmlUtils.CreateAttribute(xmlTab, "type", "Normal"));
                    break;
                case TabType.Tab:
                    urlNode.Attributes.Append(XmlUtils.CreateAttribute(xmlTab, "type", "Tab"));
                    //Get the tab being linked to
                    TabInfo tab = new TabController().GetTab(Int32.Parse(objTab.Url), objTab.PortalID, false);
                    urlNode.InnerXml = tab.TabPath;
                    break;
                case TabType.File:
                    urlNode.Attributes.Append(XmlUtils.CreateAttribute(xmlTab, "type", "File"));
                    var file = FileManager.Instance.GetFile(Int32.Parse(objTab.Url.Substring(7)));
                    urlNode.InnerXml = file.RelativePath;
                    break;
                case TabType.Url:
                    urlNode.Attributes.Append(XmlUtils.CreateAttribute(xmlTab, "type", "Url"));
                    break;
            }

            //serialize TabSettings
            XmlUtils.SerializeHashtable(objTab.TabSettings, xmlTab, nodeTab, "tabsetting", "settingname", "settingvalue");
            if (objPortal != null)
            {
                if (objTab.TabID == objPortal.SplashTabId)
                {
                    newnode = xmlTab.CreateElement("tabtype");
                    newnode.InnerXml = "splashtab";
                    nodeTab.AppendChild(newnode);
                }
                else if (objTab.TabID == objPortal.HomeTabId)
                {
                    newnode = xmlTab.CreateElement("tabtype");
                    newnode.InnerXml = "hometab";
                    nodeTab.AppendChild(newnode);
                }
                else if (objTab.TabID == objPortal.UserTabId)
                {
                    newnode = xmlTab.CreateElement("tabtype");
                    newnode.InnerXml = "usertab";
                    nodeTab.AppendChild(newnode);
                }
                else if (objTab.TabID == objPortal.LoginTabId)
                {
                    newnode = xmlTab.CreateElement("tabtype");
                    newnode.InnerXml = "logintab";
                    nodeTab.AppendChild(newnode);
                }
            }
            if (hTabs != null)
            {
                //Manage Parent Tab
                if (!Null.IsNull(objTab.ParentId))
                {
                    newnode = xmlTab.CreateElement("parent");
                    newnode.InnerXml = HttpContext.Current.Server.HtmlEncode(hTabs[objTab.ParentId].ToString());
                    nodeTab.AppendChild(newnode);

                    //save tab as: ParentTabName/CurrentTabName
                    hTabs.Add(objTab.TabID, hTabs[objTab.ParentId] + "/" + objTab.TabName);
                }
                else
                {
                    //save tab as: CurrentTabName
                    hTabs.Add(objTab.TabID, objTab.TabName);
                }
            }
            XmlNode nodePanes;
            XmlNode nodePane;
            XmlNode nodeName;
            XmlNode nodeModules;
            XmlNode nodeModule;
            XmlDocument xmlModule;
            ModuleInfo objmodule;
            ModuleController objmodules = new ModuleController();

            //Serialize modules
            nodePanes = nodeTab.AppendChild(xmlTab.CreateElement("panes"));
            foreach (KeyValuePair<int, ModuleInfo> kvp in objmodules.GetTabModules(objTab.TabID))
            {
                objmodule = kvp.Value;
                if (!objmodule.IsDeleted)
                {
                    xmlModule = new XmlDocument();
                    nodeModule = ModuleController.SerializeModule(xmlModule, objmodule, includeContent);
                    if (nodePanes.SelectSingleNode("descendant::pane[name='" + objmodule.PaneName + "']") == null)
                    {
                        //new pane found
                        nodePane = xmlModule.CreateElement("pane");
                        nodeName = nodePane.AppendChild(xmlModule.CreateElement("name"));
                        nodeName.InnerText = objmodule.PaneName;
                        nodePane.AppendChild(xmlModule.CreateElement("modules"));
                        nodePanes.AppendChild(xmlTab.ImportNode(nodePane, true));
                    }
                    nodeModules = nodePanes.SelectSingleNode("descendant::pane[name='" + objmodule.PaneName + "']/modules");
                    nodeModules.AppendChild(xmlTab.ImportNode(nodeModule, true));
                }
            }
            return nodeTab;
        }

        [Obsolete("Deprecated in DotNetNuke 5.5.Replaced by ModuleController.CopyModules")]
        public void CopyTab(int PortalId, int FromTabId, int ToTabId, bool asReference)
        {
            ModuleController objModules = new ModuleController();
            TabInfo sourceTab = GetTab(FromTabId, PortalId, false);
            TabInfo destinationTab = GetTab(FromTabId, ToTabId, false);

            if (sourceTab != null && destinationTab != null)
            {
                objModules.CopyModules(sourceTab, destinationTab, asReference);
            }
        }

        [Obsolete("This method has replaced in DotNetNuke 5.0 by DeserializeTab(ByVal nodeTab As XmlNode, ByVal objTab As TabInfo, ByVal PortalId As Integer, ByVal mergeTabs As PortalTemplateModuleAction)")]
        public static TabInfo DeserializeTab(string tabName, XmlNode nodeTab, int PortalId)
        {
            return DeserializeTab(nodeTab, null, new Hashtable(), PortalId, false, PortalTemplateModuleAction.Ignore, new Hashtable());
        }

        [Obsolete("This method has replaced in DotNetNuke 5.0 by DeserializeTab(ByVal nodeTab As XmlNode, ByVal objTab As TabInfo, ByVal PortalId As Integer, ByVal mergeTabs As PortalTemplateModuleAction)")]
        public static TabInfo DeserializeTab(XmlNode nodeTab, TabInfo objTab, int PortalId)
        {
            return DeserializeTab(nodeTab, objTab, new Hashtable(), PortalId, false, PortalTemplateModuleAction.Ignore, new Hashtable());
        }

        [Obsolete("This method has replaced in DotNetNuke 5.0 by DeserializeTab(ByVal nodeTab As XmlNode, ByVal objTab As TabInfo, ByVal hTabs As Hashtable, ByVal PortalId As Integer, ByVal IsAdminTemplate As Boolean, ByVal mergeTabs As PortalTemplateModuleAction, ByVal hModules As Hashtable)")]
        public static TabInfo DeserializeTab(string tabName, XmlNode nodeTab, TabInfo objTab, Hashtable hTabs, int PortalId, bool IsAdminTemplate, PortalTemplateModuleAction mergeTabs,
                                             Hashtable hModules)
        {
            return DeserializeTab(nodeTab, objTab, hTabs, PortalId, IsAdminTemplate, mergeTabs, hModules);
        }

        [Obsolete("This method has replaced in DotNetNuke 5.0 by CopyDesignToChildren(TabInfo,String, String)")]
        public void CopyDesignToChildren(ArrayList tabs, string skinSrc, string containerSrc)
        {
            foreach (TabInfo objTab in tabs)
            {
                Provider.UpdateTab(objTab.TabID,
                                   objTab.ContentItemId,
                                   objTab.PortalID,
                                   objTab.VersionGuid,
                                   objTab.DefaultLanguageGuid,
                                   objTab.LocalizedVersionGuid,
                                   objTab.TabName,
                                   objTab.IsVisible,
                                   objTab.DisableLink,
                                   objTab.ParentId,
                                   objTab.IconFile,
                                   objTab.IconFileLarge,
                                   objTab.Title,
                                   objTab.Description,
                                   objTab.KeyWords,
                                   objTab.IsDeleted,
                                   objTab.Url,
                                   skinSrc,
                                   containerSrc,
                                   objTab.TabPath,
                                   objTab.StartDate,
                                   objTab.EndDate,
                                   objTab.RefreshInterval,
                                   objTab.PageHeadText,
                                   objTab.IsSecure,
                                   objTab.PermanentRedirect,
                                   objTab.SiteMapPriority,
                                   UserController.GetCurrentUserInfo().UserID,
                                   objTab.CultureCode);
                EventLogController objEventLog = new EventLogController();
                objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.TAB_UPDATED);
            }
            if (tabs.Count > 0)
            {
                DataCache.ClearTabsCache(((TabInfo)tabs[0]).PortalID);
            }
        }

        [Obsolete("Deprecated in DotNetNuke 5.0. Replaced by CopyPermissionsToChildren(TabInfo, TabPermissionCollection)")]
        public void CopyPermissionsToChildren(ArrayList tabs, TabPermissionCollection newPermissions)
        {
            TabPermissionController objTabPermissionController = new TabPermissionController();
            foreach (TabInfo objTab in tabs)
            {
                objTab.TabPermissions.Clear();
                objTab.TabPermissions.AddRange(newPermissions);
                TabPermissionController.SaveTabPermissions(objTab);
            }
            if (tabs.Count > 0)
            {
                DataCache.ClearTabsCache(((TabInfo)tabs[0]).PortalID);
            }
        }

        [Obsolete("This method is obsolete.  It has been replaced by GetTab(ByVal TabId As Integer, ByVal PortalId As Integer, ByVal ignoreCache As Boolean) ")]
        public TabInfo GetTab(int TabId)
        {
            return CBO.FillObject<TabInfo>(DataProvider.Instance().GetTab(TabId));
        }

        [Obsolete("Deprecated in DNN 5.5. Replaced by GetTabByTabPath(portalId, tabPath, cultureCode) ")]
        public static int GetTabByTabPath(int portalId, string tabPath)
        {
            return GetTabByTabPath(portalId, tabPath, Null.NullString);
        }

        [Obsolete("Deprecated in DNN 5.5. Replaced by GetTabPathDictionary(portalId, cultureCode) ")]
        public static Dictionary<string, int> GetTabPathDictionary(int portalId)
        {
            return GetTabPathDictionary(portalId, Null.NullString);
        }

        [Obsolete("This method has been replaced in 5.0 by GetTabPathDictionary(ByVal portalId As Integer) As Dictionary(Of String, Integer) ")]
        public static Dictionary<string, int> GetTabPathDictionary()
        {
            var tabpathDic = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            IDataReader dr = DataProvider.Instance().GetTabPaths(Null.NullInteger, Null.NullString);
            try
            {
                while (dr.Read())
                {
                    string strKey = "//" + Null.SetNullInteger(dr["PortalID"]) + Null.SetNullString(dr["TabPath"]);
                    tabpathDic[strKey] = Null.SetNullInteger(dr["TabID"]);
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
            return tabpathDic;
        }

        [Obsolete("This method has replaced in DotNetNuke 5.0 by GetTabsByPortal()")]
        public ArrayList GetTabs(int PortalId)
        {
            return GetTabsByPortal(PortalId).ToArrayList();
        }

        [Obsolete("This method is obsolete.  It has been replaced by GetTabsByParent(ByVal ParentId As Integer, ByVal PortalId As Integer) ")]
        public ArrayList GetTabsByParentId(int ParentId)
        {
            return CBO.FillCollection((DataProvider.Instance().GetTabsByParentId(ParentId)), typeof(TabInfo));
        }

        [Obsolete("This method has replaced in DotNetNuke 5.0 by GetTabsByParent(ByVal ParentId As Integer, ByVal PortalId As Integer)")]
        public ArrayList GetTabsByParentId(int ParentId, int PortalId)
        {
            ArrayList arrTabs = new ArrayList();
            foreach (TabInfo objTab in GetTabsByParent(ParentId, PortalId))
            {
                arrTabs.Add(objTab);
            }
            return arrTabs;
        }

        /// <summary>
        /// Get all TabInfo for the current culture in SortOrder
        /// </summary>
        /// <param name="portalId">The portalid to load tabs for</param>
        /// <returns>
        /// List of TabInfo oredered by default SortOrder
        /// </returns>
        /// <remarks>
        /// This method uses the Active culture.  There is an overload <seealso cref="TabController.GetTabsBySortOrder(int, string, bool)"/>
        /// which allows the culture information to be specified.
        /// </remarks>
        public static List<TabInfo> GetTabsBySortOrder(int portalId)
        {
            return GetTabsBySortOrder(portalId, PortalController.GetActivePortalLanguage(portalId), true);
        }

        [Obsolete("Deprecated in DNN 5.5. Replaced by UpdateTab(updatedTab)")]
        public void UpdateTab(TabInfo updatedTab, string CultureCode)
        {
            updatedTab.CultureCode = CultureCode;
            UpdateTab(updatedTab);
        }


        [Obsolete("This method is obsolete.  It has been replaced by UpdateTabOrder(ByVal objTab As TabInfo) ")]
        public void UpdateTabOrder(int PortalID, int TabId, int TabOrder, int Level, int ParentId)
        {
            TabInfo objTab = GetTab(TabId, PortalID, false);
            objTab.TabOrder = TabOrder;
            objTab.Level = Level;
            objTab.ParentId = ParentId;
            UpdateTabOrder(objTab);
        }

        #region "Content Localization"

        /// <summary>
        /// Creates the localized copies.
        /// </summary>
        /// <param name="originalTab">The original tab.</param>
        public void CreateLocalizedCopies(TabInfo originalTab)
        {
            Locale defaultLocale = LocaleController.Instance.GetDefaultLocale(originalTab.PortalID);
            foreach (Locale subLocale in LocaleController.Instance.GetLocales(originalTab.PortalID).Values)
            {
                if (!(subLocale.Code == defaultLocale.Code))
                {
                    CreateLocalizedCopy(originalTab, subLocale);
                }
            }
        }

        /// <summary>
        /// Creates the localized copy.
        /// </summary>
        /// <param name="tabs">The tabs.</param>
        /// <param name="locale">The locale.</param>
        public void CreateLocalizedCopy(List<TabInfo> tabs, Locale locale)
        {
            foreach (TabInfo t in tabs)
            {
                CreateLocalizedCopy(t, locale);
            }
        }

        /// <summary>
        /// Creates the localized copy.
        /// </summary>
        /// <param name="originalTab">The original tab.</param>
        /// <param name="locale">The locale.</param>
        public void CreateLocalizedCopy(TabInfo originalTab, Locale locale)
        {
            //First Clone the Tab
            TabInfo localizedCopy = originalTab.Clone();
            localizedCopy.TabID = Null.NullInteger;

            //Set Guids and Culture Code
            localizedCopy.UniqueId = Guid.NewGuid();
            localizedCopy.VersionGuid = Guid.NewGuid();
            localizedCopy.DefaultLanguageGuid = originalTab.UniqueId;
            localizedCopy.LocalizedVersionGuid = Guid.NewGuid();
            localizedCopy.CultureCode = locale.Code;
            localizedCopy.TabName = localizedCopy.TabName + " (" + locale.Code + ")";

            //Copy Permissions from original Tab for Admins only
            PortalController portalCtrl = new PortalController();
            PortalInfo portal = portalCtrl.GetPortal(originalTab.PortalID);
            localizedCopy.TabPermissions.AddRange(originalTab.TabPermissions.Where(p => p.RoleID == portal.AdministratorRoleId));

            //Get the original Tabs Parent
			//check the original whether have parent.
			if (!Null.IsNull(originalTab.ParentId))
			{
				TabInfo originalParent = GetTab(originalTab.ParentId, originalTab.PortalID, false);

				if (originalParent != null)
				{
					//Get the localized parent
					TabInfo localizedParent = GetTabByCulture(originalParent.TabID, originalParent.PortalID, locale);

					localizedCopy.ParentId = localizedParent.TabID;
				}
			}

        	//Save Tab
            int localizedTabId = AddTabInternal(localizedCopy, true);

            //Update TabOrder for this tab
            UpdateTabOrder(localizedCopy, false);

            //Make shallow copies of all modules
            ModuleController moduleCtrl = new ModuleController();
            moduleCtrl.CopyModules(originalTab, localizedCopy, true);

            //Convert these shallow copies to deep copies
            foreach (KeyValuePair<int, ModuleInfo> kvp in moduleCtrl.GetTabModules(localizedCopy.TabID))
            {
                moduleCtrl.LocalizeModule(kvp.Value, locale);
            }

            //Add Translator Role
            GiveTranslatorRoleEditRights(localizedCopy, null);

            //Clear the Cache
            ClearCache(originalTab.PortalID);
        }

        /// <summary>
        /// Gets the default culture tab list.
        /// </summary>
        /// <param name="portalid">The portalid.</param>
        /// <returns></returns>
        public List<TabInfo> GetDefaultCultureTabList(int portalid)
        {
            return (from kvp in GetTabsByPortal(portalid)
                    where !kvp.Value.TabPath.StartsWith("//Admin")
                        && !kvp.Value.IsDeleted
                    select kvp.Value).ToList();
        }

        /// <summary>
        /// Gets the culture tab list.
        /// </summary>
        /// <param name="portalid">The portalid.</param>
        /// <returns></returns>
        public List<TabInfo> GetCultureTabList(int portalid)
        {
            return (from kvp in GetTabsByPortal(portalid)
                    where !kvp.Value.TabPath.StartsWith("//Admin")
                        && kvp.Value.CultureCode == PortalController.GetCurrentPortalSettings().DefaultLanguage
                        && !kvp.Value.IsDeleted
                    select kvp.Value).ToList();
        }

        /// <summary>
        /// Gives the translator role edit rights.
        /// </summary>
        /// <param name="localizedTab">The localized tab.</param>
        /// <param name="users">The users.</param>
        public void GiveTranslatorRoleEditRights(TabInfo localizedTab, Dictionary<int, UserInfo> users)
        {
            RoleController roleCtrl = new RoleController();
            PermissionController permissionCtrl = new PermissionController();
            ArrayList permissionsList = permissionCtrl.GetPermissionByCodeAndKey("SYSTEM_TAB", "EDIT");

            string translatorRoles = PortalController.GetPortalSetting(string.Format("DefaultTranslatorRoles-{0}", localizedTab.CultureCode), localizedTab.PortalID, "");
            foreach (string translatorRole in translatorRoles.Split(';'))
            {
                if (users != null)
                {
                    foreach (UserInfo translator in roleCtrl.GetUsersByRoleName(localizedTab.PortalID, translatorRole))
                    {
                        users[translator.UserID] = translator;
                    }
                }

                if (permissionsList != null && permissionsList.Count > 0)
                {
                    PermissionInfo translatePermisison = (PermissionInfo)permissionsList[0];
                    string roleName = translatorRole;
                    RoleInfo role = new RoleController().GetRoleByName(localizedTab.PortalID, roleName);
                    if (role != null)
                    {
                        TabPermissionInfo perm = localizedTab.TabPermissions.Where(tp => tp.RoleID == role.RoleID && tp.PermissionKey == "EDIT").SingleOrDefault();
                        if (perm == null)
                        {
                            //Create Permission
                            TabPermissionInfo tabTranslatePermission = new TabPermissionInfo(translatePermisison);
                            tabTranslatePermission.RoleID = role.RoleID;
                            tabTranslatePermission.AllowAccess = true;
                            tabTranslatePermission.RoleName = roleName;
                            localizedTab.TabPermissions.Add(tabTranslatePermission);
                            UpdateTab(localizedTab);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Localizes the tab.
        /// </summary>
        /// <param name="originalTab">The original tab.</param>
        /// <param name="locale">The locale.</param>
        public void LocalizeTab(TabInfo originalTab, Locale locale)
        {
            originalTab.CultureCode = locale.Code;

            UpdateTab(originalTab);

            //Update culture of modules on this page
            ModuleController moduleCtl = new ModuleController();
            foreach (KeyValuePair<int, ModuleInfo> kvp in moduleCtl.GetTabModules(originalTab.TabID))
            {
                kvp.Value.CultureCode = locale.Code;
                moduleCtl.UpdateModule(kvp.Value);
            }
        }

        /// <summary>
        /// Publishes the tab.
        /// </summary>
        /// <param name="publishTab">The publish tab.</param>
        public void PublishTab(TabInfo publishTab)
        {
            //To publish a subsidiary language tab we need to enable the View Permissions
            if (publishTab != null && publishTab.DefaultLanguageTab != null)
            {
                foreach (TabPermissionInfo perm in
                    publishTab.DefaultLanguageTab.TabPermissions.Where(p => p.PermissionKey == "VIEW"))
                {
                    TabPermissionInfo sourcePerm = perm;
                    TabPermissionInfo targetPerm =
                        publishTab.TabPermissions.Where(p => p.PermissionKey == sourcePerm.PermissionKey && p.RoleID == sourcePerm.RoleID && p.UserID == sourcePerm.UserID).SingleOrDefault();

                    if (targetPerm == null)
                    {
                        publishTab.TabPermissions.Add(sourcePerm);
                    }

                    TabPermissionController.SaveTabPermissions(publishTab);
                }
            }
        }

        /// <summary>
        /// Publishes the tabs.
        /// </summary>
        /// <param name="tabs">The tabs.</param>
        public void PublishTabs(List<TabInfo> tabs)
        {
            foreach (TabInfo t in tabs)
            {
                if (t.IsTranslated)
                {
                    PublishTab(t);
                }
            }
        }

        #endregion
    }
}
