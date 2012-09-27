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
using System.Linq;
using System.Text;
using System.Web;

using DotNetNuke.Application;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Client.ClientResourceManagement;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.UI.ControlPanels
{
    public partial class ControlBar : ControlPanelBase
    {
        public override bool IsDockable { get; set; }

        public override bool IncludeInControlHierarchy
        {
            get
            {
                return base.IncludeInControlHierarchy && (IsPageAdmin() || IsModuleAdmin());
            }
        }
      
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {           
            base.OnInit(e);
            ID = "ControlBar";
        }

        protected override void OnLoad(EventArgs e)
        {           
            base.OnLoad(e);

            if (ControlPanel.Visible && IncludeInControlHierarchy)
            {
                ClientResourceManager.RegisterStyleSheet(this.Page, "~/admin/ControlPanel/ControlBar.css");
                jQuery.RequestHoverIntentRegistration();
            }

            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

            if (!IsPostBack)
            {
                LoadCategoryList();
                LoadSiteList();
                LoadVisibilityList();
                AutoSetUserMode();
            }  
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		}
	
		#endregion

		#region Protected Methods

		protected string PreviewPopup()
		{
			var previewUrl = string.Format("{0}/Default.aspx?ctl={1}&previewTab={2}", 
										Globals.AddHTTP(PortalSettings.PortalAlias.HTTPAlias), 
										"MobilePreview",
										PortalSettings.ActiveTab.TabID);

			if(PortalSettings.EnablePopUps)
			{
				return UrlUtils.PopUpUrl(previewUrl, this, PortalSettings, true, false, 660, 800);
			}
			else
			{
				return string.Format("location.href = \"{0}\"", previewUrl);
			}
		}

        protected IEnumerable<string[]> LoadPaneList()
        {
            ArrayList panes = PortalSettings.Current.ActiveTab.Panes;
            List<string[]> resultPanes = new List<string[]>();
            foreach (var p in panes)
            {
                string[] topPane = new string[]{
                    string.Format(GetString("Pane.AddTop.Text"), p),
                    p.ToString(),
                    "TOP"
                };

                string[] botPane = new string[]{
                    string.Format(GetString("Pane.AddBottom.Text"), p),
                    p.ToString(),
                    "BOTTOM"
                };
                resultPanes.Add(topPane);
                resultPanes.Add(botPane);
            }

            return resultPanes;
        }


        protected string GetString(string key)
        {
            return Localization.GetString(key, LocalResourceFile);
        }

        protected string BuildToolUrl(string toolName, bool isHostTool, string moduleFriendlyName, 
                                      string controlKey, string navigateUrl, bool showAsPopUp)
        {
            if ((isHostTool && !UserController.GetCurrentUserInfo().IsSuperUser))
            {
                return "javascript:void(0);";
            }

            if ((!string.IsNullOrEmpty(navigateUrl)))
            {
                return navigateUrl;
            }            

            string returnValue = "javascript:void(0);";
            switch (toolName)
            {
                case "PageSettings":

                    if(TabPermissionController.CanManagePage())
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=edit&activeTab=settingTab");

                    break;

                case "CopyPage":

                    if (TabPermissionController.CanCopyPage())
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=copy&activeTab=copyTab");

                    break;

                case "DeletePage":

                    if(TabPermissionController.CanDeletePage())
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=delete");                   
                       
                    break;

                case "PageTemplate":

                    if(TabPermissionController.CanManagePage())
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=edit&activeTab=advancedTab");                   
                       
                    break;

                case "PagePermission":

                    if (TabPermissionController.CanAdminPage())
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=edit&activeTab=permissionsTab");

                    break;

                case "ImportPage":

                    if (TabPermissionController.CanImportPage())
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "ImportTab");

                    break;

                case "ExportPage":

                    if (TabPermissionController.CanExportPage())
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "ExportTab");

                    break;

                case "NewPage":

                    if (DotNetNuke.Security.Permissions.TabPermissionController.CanAddPage())
                        returnValue = Globals.NavigateURL("Tab", "activeTab=settingTab");

                    break;

                default:
                    if ((!string.IsNullOrEmpty(moduleFriendlyName)))
                    {
                        var additionalParams = new List<string>();
                        if ((toolName == "UploadFile" || toolName == "HostUploadFile"))
                        {
                            additionalParams.Add("ftype=File");
                            additionalParams.Add("rtab=" + PortalSettings.ActiveTab.TabID);
                        }
                        returnValue = GetTabURL(additionalParams, toolName, isHostTool, 
                                                moduleFriendlyName, controlKey, showAsPopUp);
                    }
                    break;
            }
            return returnValue;
        }

        protected string GetTabURL(List<string> additionalParams, string toolName, bool isHostTool, 
                                   string moduleFriendlyName, string controlKey, bool showAsPopUp)
        {
            int portalId = (isHostTool) ? Null.NullInteger : PortalSettings.PortalId;

            string strURL = string.Empty;

            if (((additionalParams == null)))
            {
                additionalParams = new List<string>();
            }

            var moduleCtrl = new ModuleController();
            var moduleInfo = moduleCtrl.GetModuleByDefinition(portalId, moduleFriendlyName);

            if (((moduleInfo != null)))
            {
                bool isHostPage = (portalId == Null.NullInteger);
                if ((!string.IsNullOrEmpty(controlKey)))
                {
                    additionalParams.Insert(0, "mid=" + moduleInfo.ModuleID);
                    if (showAsPopUp && PortalSettings.EnablePopUps)
                    {
                        additionalParams.Add("popUp=true");
                    }
                }

                string currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                strURL = Globals.NavigateURL(moduleInfo.TabID, isHostPage, PortalSettings, controlKey, currentCulture, additionalParams.ToArray());
            }

            return strURL;
        }

        protected string GetTabURL(string tabName, bool isHostTool)
        {
            if ((isHostTool && !UserController.GetCurrentUserInfo().IsSuperUser))
            {
                return "javascript:void(0);";
            }

            int portalId = (isHostTool) ? Null.NullInteger : PortalSettings.PortalId;
            return GetTabURL(tabName, portalId);
        }

        protected string GetTabURL(string tabName, int portalId)
        {
            var tabController = new TabController();   
            var tab = tabController.GetTabByName(tabName, portalId);

            if(tab != null)
                return tab.FullUrl;

            return string.Empty;
        }

        protected string GetMenuItem(string tabName, bool isHostTool)
        {
            if ((isHostTool && !UserController.GetCurrentUserInfo().IsSuperUser))
            {
                return string.Empty;
            }

            List<TabInfo> tabList = null;
            if(isHostTool)
            {
                if(_hostTabs == null) _hostTabs = GetHostTabs();
                tabList = _hostTabs;
            }
            else
            {
                if(_adminTabs == null) _adminTabs = GetAdminTabs();
                tabList = _adminTabs;
            }

            var tab = tabList.SingleOrDefault(t => t.TabName == tabName);
            return GetMenuItem(tab);
        }

        protected string GetMenuItem(TabInfo tab)
        {
            if (tab == null) return string.Empty;
            if (tab.IsVisible && !tab.IsDeleted && !tab.DisableLink)
            {
                string name = string.IsNullOrEmpty(tab.Title) ? tab.TabName : tab.Title;

                return string.Format("<li data-tabname='{3}'><a href='{0}?editmode=true'>{1}</a><a href='javascript:void(0)' class='bookmark' title='{2}'><span></span></a></li>",
                                     tab.FullUrl,
                                     name,
                                     GetString("Tool.AddToBookmarks.ToolTip"),
                                     tab.TabName);
            }
            return string.Empty;
        }
    

        protected string GetAdminBaseMenu()
        {
            var tabs = AdminBaseTabs;
            var sb = new StringBuilder();
            foreach(var tab in tabs)
            {
                sb.Append(GetMenuItem(tab));
            }

            return sb.ToString();
        }

        protected string GetAdminAdvancedMenu()
        {
            var tabs = AdminAdvancedTabs;
            var sb = new StringBuilder();
            foreach (var tab in tabs)
            {
                sb.Append(GetMenuItem(tab));
            }

            return sb.ToString();
        }

        protected string GetHostBaseMenu()
        {
            var tabs = HostBaseTabs;
            var sb = new StringBuilder();
            foreach (var tab in tabs)
            {
                sb.Append(GetMenuItem(tab));
            }

            return sb.ToString();
        }

        protected string GetHostAdvancedMenu()
        {
            var tabs = HostAdvancedTabs;
            var sb = new StringBuilder();
            foreach (var tab in tabs)
            {
                sb.Append(GetMenuItem(tab));
            }

            return sb.ToString();
        }

        protected string GetBookmarkItems(string title)
        {
            var personalizationController = new DotNetNuke.Services.Personalization.PersonalizationController();
            var personalization = personalizationController.LoadProfile(UserController.GetCurrentUserInfo().UserID, PortalSettings.PortalId);
            var bookmarkItems = personalization.Profile["ControlBar:" + title + PortalSettings.PortalId];
            bool isHostTool = title == "host";

            if(bookmarkItems != null)
            {
                var items = bookmarkItems.ToString().Split(',');
                var sb = new StringBuilder();
                foreach(var itemKey in items)
                {
                    sb.Append(GetMenuItem(itemKey, isHostTool));
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        protected string GetButtonConfirmMessage(string toolName)
        {
            if (toolName == "DeletePage")
            {
                return ClientAPI.GetSafeJSString(Localization.GetString("Tool.DeletePage.Confirm", LocalResourceFile));
            }

            return string.Empty;
        }    

        protected IEnumerable<string[]> LoadPortalsList()
        {
            var portalCtrl = new PortalController();
            ArrayList portals = portalCtrl.GetPortals();

            List<string[]> result = new List<string[]>();
            foreach (var portal in portals)
            {
                PortalInfo pi = portal as PortalInfo;

                if (pi != null)
                {
                    string[] p = new string[]{
                        pi.PortalName,
                        pi.PortalID.ToString()
                    };

                    result.Add(p);
                }
            }

            return result;
        }

        protected string CheckedWhenInLayoutMode()
        {
            return UserMode == PortalSettings.Mode.Layout ? "checked='checked'" : string.Empty;
        }

        protected string SpecialClassWhenNotInViewMode()
        {
            return UserMode == PortalSettings.Mode.View ? string.Empty : "controlBar_editPageInEditMode";
        }

        protected string GetModeForAttribute()
        {
            return UserMode.ToString().ToUpper();
        }

        protected string GetEditButtonLabel()
        {
            return UserMode == PortalSettings.Mode.Edit ? GetString("Tool.CloseEditMode.Text") : GetString("Tool.EditThisPage.Text");
        }

        #endregion

        #region Private Methods

        private string LocalResourceFile
        {
            get
            {
                return string.Format("{0}/{1}/{2}.ascx.resx", TemplateSourceDirectory, Localization.LocalResourceDirectory, GetType().BaseType.Name);
            }
        }

        private void LoadCategoryList()
        {
            ITermController termController = Util.GetTermController();
            CategoryList.DataSource = termController.GetTermsByVocabulary("Module_Categories").OrderBy(t => t.Weight).Where(t => t.Name != "< None >").ToList();
            CategoryList.DataBind();
            CategoryList.AddItem(Localization.GetString("AllCategories", LocalResourceFile), "All");
            if (!IsPostBack)
            {
                CategoryList.Select("Common", false);
            }
        }

        private void LoadSiteList()
        {
            // Is there more than one site in this group?
            var multipleSites = GetCurrentPortalsGroup().Count() > 1;
            
            
            // Get a list of portals in this SiteGroup.
            var controller = new PortalController();

            var portals = controller.GetPortals().Cast<PortalInfo>().ToArray();

            SiteList.DataSource = portals.Select(
                x => new { Value = x.PortalID, Name = x.PortalName, GroupID = x.PortalGroupID }).ToList();
            SiteList.DataTextField = "Name";
            SiteList.DataValueField = "Value";
            SiteList.DataBind();
            
        }

        private void LoadVisibilityList()
        { 
            var items = new Dictionary<string, string> { { "0", GetString("PermissionView") }, { "1", GetString("PermissionEdit") } };

            VisibilityLst.Items.Clear();
            VisibilityLst.DataValueField = "key";
            VisibilityLst.DataTextField = "value";
            VisibilityLst.DataSource = items;
            VisibilityLst.DataBind();            
        }

        private IEnumerable<PortalInfo> GetCurrentPortalsGroup()
        {
            var groups = PortalGroupController.Instance.GetPortalGroups().ToArray();

            var result = (from @group in groups
                          select PortalGroupController.Instance.GetPortalsByGroup(@group.PortalGroupId)
                              into portals
                              where portals.Any(x => x.PortalID == PortalSettings.Current.PortalId)
                              select portals.ToArray()).FirstOrDefault();

            // Are we in a group of one?
            if (result == null || result.Length == 0)
            {
                var portalController = new PortalController();

                result = new[] { portalController.GetPortal(PortalSettings.Current.PortalId) };
            }

            return result;
        }

        private void AutoSetUserMode()
        {
            int tabId = PortalSettings.ActiveTab.TabID;
            int portalId = PortalSettings.Current.PortalId;
            string pageId = string.Format("{0}:{1}", portalId, tabId);

            string lastPageId = GetLastPageHistory();

            if (lastPageId != pageId)
            {
                // navigate between pages
                if (PortalSettings.Current.UserMode != Entities.Portals.PortalSettings.Mode.View)
                {
                    SetUserMode("VIEW");
                    SetLastPageHistory(pageId);
                    Response.Redirect(Request.RawUrl, true);
                }
            }

            SetLastPageHistory(pageId);            
        }

        private void SetLastPageHistory(string pageId)
        {
            HttpCookie newCookie = new HttpCookie("LastPageId", pageId);
            Response.Cookies.Add(newCookie);
        }

        private string GetLastPageHistory()
        {
            HttpCookie cookie = Request.Cookies["LastPageId"];
            if (cookie != null)
                return cookie.Value;

            return "NEW";
        }

        private List<TabInfo> GetHostTabs()
        {
            var tabController = new TabController();
            var hostTab = tabController.GetTabByName("Host", -1);
            var hosts = TabController.GetTabsByParent(hostTab.TabID, -1);


            var professionalTabs = new List<TabInfo>();
            if ((DotNetNukeContext.Current.Application.Name != "DNNCORP.CE"))
            {
                var professionalTab = tabController.GetTabByName("Professional Features", -1);
                professionalTabs = TabController.GetTabsByParent(professionalTab.TabID, -1);
            }


            var result = new List<TabInfo>();
            result.AddRange(hosts);
            result.AddRange(professionalTabs);
            result = result.OrderBy(t => t.TabName).ToList();
            return result;
        } 

        private List<TabInfo> GetAdminTabs()
        {
            var tabController = new TabController();
            var adminTab = tabController.GetTabByName("Admin", PortalSettings.PortalId);
            return TabController.GetTabsByParent(adminTab.TabID, PortalSettings.PortalId).OrderBy(t => t.TabName).ToList();
        }

        #endregion

        #region Menu Items Properties

        private List<TabInfo> _adminTabs;
        private List<TabInfo> _adminBaseTabs;
        private List<TabInfo> _adminAdvancedTabs;

        protected List<TabInfo> AdminBaseTabs
        {
            get
            {
                if (_adminBaseTabs == null)
                {
                    if (_adminTabs == null) _adminTabs = GetAdminTabs();

                    _adminBaseTabs = new List<TabInfo>();
                    _adminAdvancedTabs = new List<TabInfo>();

                    foreach(var tabInfo in _adminTabs)
                    {
                        switch(tabInfo.TabName)
                        {
                            case "Site Settings":
                            case "Pages":
                            case "Security Roles":
                            case "User Accounts":
                            case "File Manager":
                            case "Recycle Bin":
                            case "Log Viewer":
                                _adminBaseTabs.Add(tabInfo);
                                break;
                            default:
                                _adminAdvancedTabs.Add(tabInfo);
                                break;
                        }
                    }
                }
                return _adminBaseTabs;
            }
        }

        protected List<TabInfo> AdminAdvancedTabs
        {
            get
            {
                if (_adminAdvancedTabs == null)
                {
                    if (_adminTabs == null) _adminTabs = GetAdminTabs();

                    _adminBaseTabs = new List<TabInfo>();
                    _adminAdvancedTabs = new List<TabInfo>();

                    foreach (var tabInfo in _adminTabs)
                    {
                        switch (tabInfo.TabName)
                        {
                            case "Site Settings":
                            case "Pages":
                            case "Security Roles":
                            case "User Accounts":
                            case "File Manager":
                            case "Recycle Bin":
                            case "Log Viewer":
                                _adminBaseTabs.Add(tabInfo);
                                break;
                            default:
                                _adminAdvancedTabs.Add(tabInfo);
                                break;
                        }
                    }
                }
                return _adminAdvancedTabs;
            }
        }

        private List<TabInfo> _hostTabs;
        private List<TabInfo> _hostBaseTabs;
        private List<TabInfo> _hostAdvancedTabs;


        protected List<TabInfo> HostBaseTabs
        {
            get
            {
                if (_hostBaseTabs == null)
                {
                    if (_hostTabs == null) _hostTabs = GetHostTabs();

                    _hostBaseTabs = new List<TabInfo>();
                    _hostAdvancedTabs = new List<TabInfo>();

                    foreach (var tabInfo in _hostTabs)
                    {
                        switch (tabInfo.TabName)
                        {
                            case "Host Settings":
                            case "Site Management":
                            case "File Manager":
                            case "Extensions":
                            case "Dashboard":
                            case "Health Monitoring":
                            case "Technical Support":
                            case "Knowledge Base":
                            case "TSoftware and Documentation":
                                _hostBaseTabs.Add(tabInfo);
                                break;
                            default:
                                _hostAdvancedTabs.Add(tabInfo);
                                break;
                        }
                    }
                }
                return _hostBaseTabs;
            }
        }

        protected List<TabInfo> HostAdvancedTabs
        {
            get
            {
                if (_hostAdvancedTabs == null)
                {
                    if (_hostTabs == null) _hostTabs = GetHostTabs();

                    _hostBaseTabs = new List<TabInfo>();
                    _hostAdvancedTabs = new List<TabInfo>();

                    foreach (var tabInfo in _hostTabs)
                    {
                        switch (tabInfo.TabName)
                        {
                            case "Host Settings":
                            case "Site Management":
                            case "File Manager":
                            case "Extensions":
                            case "Dashboard":
                            case "Health Monitoring":
                            case "Technical Support":
                            case "Knowledge Base":
                            case "TSoftware and Documentation":
                                _hostBaseTabs.Add(tabInfo);
                                break;
                            default:
                                _hostAdvancedTabs.Add(tabInfo);
                                break;
                        }
                    }
                }
                return _hostAdvancedTabs;
            }
        }

        #endregion
    }
    
}
