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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.UI;
using Telerik.Web.UI;
using DataCache = DotNetNuke.Common.Utilities.DataCache;
using Globals = DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Admin.Pages
{

    public enum Position
    {
        Child,
        Below,
        Above
    }

    /// <summary>
    /// The Tabs PortalModuleBase is used to manage the Tabs/Pages for a 
    /// portal.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public partial class View : PortalModuleBase
    {

        #region Private Members

        private const string DefaultPageTemplate = "Default.page.template";

        #endregion

        #region Properties

        private string AllUsersIcon
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_Everyone.png";
            }
        }

        private string AdminOnlyIcon
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_UserAdmin.png";
            }
        }

        private string IconAdd
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_Add.png";
            }
        }

        private string IconDelete
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_Delete.png";
            }
        }

        private string IconDown
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_Down.png";
            }
        }

        private string IconEdit
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_Edit.png";
            }
        }

        private string IconHome
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_Home.png";
            }
        }

        private string IconPageDisabled
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_Disabled.png";
            }
        }

        private string IconPageHidden
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_Hidden.png";
            }
        }

        private string IconPortal
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_Portal.png";
            }
        }

        private string IconUp
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_Up.png";
            }
        }

        private string IconView
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_View.png";
            }
        }

        private string RegisteredUsersIcon
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_User.png";
            }
        }

        private string SecuredIcon
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_UserSecure.png";
            }
        }

        private string SelectedNode
        {
            get
            {
                return (string)ViewState["SelectedNode"];
            }
            set
            {
                ViewState["SelectedNode"] = value;
            }
        }

        protected List<TabInfo> Tabs
        {
            get
            {
                return TabController.GetPortalTabs(rblMode.SelectedValue == "H" ? Null.NullInteger : PortalId, Null.NullInteger, false, true, false, true);
            }
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cmdCopySkin.Click += CmdCopySkinClick;
            rblMode.SelectedIndexChanged += RblModeSelectedIndexChanged;
            ctlPages.NodeClick += CtlPagesNodeClick;
            ctlPages.ContextMenuItemClick += CtlPagesContextMenuItemClick;
            ctlPages.NodeDrop += CtlPagesNodeDrop;
            ctlPages.NodeEdit += CtlPagesNodeEdit;
            cmdExpandTree.Click += OnExpandTreeClick;
            grdModules.NeedDataSource += GrdModulesNeedDataSource;
            ctlPages.NodeExpand += CtlPagesNodeExpand;
            btnBulkCreate.Click += BtnBulkCreateClick;
            cmdUpdate.Click += CmdUpdateClick;

            jQuery.RequestDnnPluginsRegistration();

            ClientAPI.RegisterClientReference(Page, ClientAPI.ClientNamespaceReferences.dnn_dom);
            ClientAPI.RegisterClientScriptBlock(Page, "dnn.controls.js");
            dgPermissions.RegisterScriptsForAjaxPanel();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                CheckSecurity();
                pnlHost.Visible = UserInfo.IsSuperUser;

                // If this is the first visit to the page, bind the tab data to the page listbox
                if (Page.IsPostBack == false)
                {
                    LocalizeControl();
                    BindSkinsAndContainers();

                    if (!(string.IsNullOrEmpty(Request.QueryString["isHost"])))
                    {
                        if (bool.Parse(Request.QueryString["isHost"]))
                        {
                            rblMode.SelectedValue = "H";
                        }
                    }
                    BindTree();
                }

                //ctlPages.ContextMenus(0).Items(2).Attributes.Add("onclick", "return confirm('Are you sure?');")
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void CmdCopySkinClick(object sender, EventArgs e)
        {
            try
            {
                TabController.CopyDesignToChildren(new TabController().GetTab(Convert.ToInt32(ctlPages.SelectedNode.Value), PortalId, false), drpSkin.SelectedValue, drpContainer.SelectedValue);
                ShowSuccessMessage(Localization.GetString("DesignCopied", LocalResourceFile));
            }
            catch (Exception ex)
            {                
                ShowErrorMessage(Localization.GetString("DesignCopyError", LocalResourceFile));
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void RblModeSelectedIndexChanged(object sender, EventArgs e)
        {
            BindTree();
        }

        protected void CtlPagesNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (e.Node.Attributes["isPortalRoot"] != null && Boolean.Parse(e.Node.Attributes["isPortalRoot"]))
            {
                pnlDetails.Visible = false;
                pnlBulk.Visible = false;
            }
            else
            {
                var tabid = Convert.ToInt32(e.Node.Value);
                BindTab(tabid);
                pnlDetails.Visible = true;
                pnlBulk.Visible = false;
                grdModules.Rebind();
            }
        }

        protected void CtlPagesContextMenuItemClick(object sender, RadTreeViewContextMenuEventArgs e)
        {
            SelectedNode = e.Node.Value;

            var objTabController = new TabController();
            var objTab = objTabController.GetTab(int.Parse(e.Node.Value), PortalId, false);

            switch (e.MenuItem.Value.ToLower())
            {
                case "makehome":
                    PortalSettings.HomeTabId = objTab.TabID;
                    PortalController.UpdatePortalSetting(PortalId, "HomeTabId", objTab.TabID.ToString());
                    DataCache.ClearPortalCache(PortalId, false);
                    BindTreeAndShowTab(objTab.TabID);
                    ShowSuccessMessage(string.Format(Localization.GetString("TabMadeHome", LocalResourceFile), objTab.TabName));
                    break;
                case "view":
                    Response.Redirect(objTab.FullUrl);
                    break;
                case "edit":
                    Response.Redirect(Globals.NavigateURL(objTab.TabID, "Tab", "action=edit", "returntabid=" + TabId), true);                    
                    break;
                case "delete":
                    TabController.DeleteTab(objTab.TabID, PortalSettings, UserId);                    
                    BindTree();                    
                    //keep the parent tab selected
                    if (objTab.ParentId != Null.NullInteger)
                    {                        
                        SelectedNode = objTab.ParentId.ToString();
                        ctlPages.FindNodeByValue(SelectedNode).Selected = true;
                        ctlPages.FindNodeByValue(SelectedNode).ExpandParentNodes();
                        BindTab(objTab.ParentId);
                    }
                    else
                    {
                        pnlDetails.Visible = false;
                    }
                    ShowSuccessMessage(string.Format(Localization.GetString("TabDeleted", LocalResourceFile), objTab.TabName));
                    break;
                case "moveup":
                    objTabController.MoveTab(objTab, TabMoveType.Up);
                    BindTree();
                    break;
                case "movedown":
                    objTabController.MoveTab(objTab, TabMoveType.Down);
                    BindTree();
                    break;
                case "add":
                    pnlBulk.Visible = true;
                    btnBulkCreate.CommandArgument = e.Node.Value;
                    ctlPages.FindNodeByValue(e.Node.Value).Selected = true;
                    txtBulk.Focus();
                    pnlDetails.Visible = false;
                    //Response.Redirect(NavigateURL(objTab.TabID, "Tab", "action=add", "returntabid=" & TabId.ToString), True)
                    break;
                case "hide":
                    objTab.IsVisible = false;
                    objTabController.UpdateTab(objTab);
                    BindTreeAndShowTab(objTab.TabID);
                    ShowSuccessMessage(string.Format(Localization.GetString("TabHidden", LocalResourceFile), objTab.TabName));
                    break;
                case "show":
                    objTab.IsVisible = true;
                    objTabController.UpdateTab(objTab);
                    BindTreeAndShowTab(objTab.TabID);
                    ShowSuccessMessage(string.Format(Localization.GetString("TabShown", LocalResourceFile), objTab.TabName));
                    break;
                case "disable":
                    objTab.DisableLink = true;
                    objTabController.UpdateTab(objTab);
                    BindTreeAndShowTab(objTab.TabID);
                    ShowSuccessMessage(string.Format(Localization.GetString("TabDisabled", LocalResourceFile), objTab.TabName));
                    break;
                case "enable":
                    objTab.DisableLink = false;
                    objTabController.UpdateTab(objTab);
                    BindTreeAndShowTab(objTab.TabID);
                    ShowSuccessMessage(string.Format(Localization.GetString("TabEnabled", LocalResourceFile), objTab.TabName));
                    break;
            }
        }

        protected void CtlPagesNodeDrop(object sender, RadTreeNodeDragDropEventArgs e)
        {
            var sourceNode = e.SourceDragNode;
            var destNode = e.DestDragNode;
            var dropPosition = e.DropPosition;
            if (destNode != null)
            {
                if (sourceNode.TreeView.SelectedNodes.Count <= 1)
                {
                    PerformDragAndDrop(dropPosition, sourceNode, destNode);
                }
                else if (sourceNode.TreeView.SelectedNodes.Count > 1)
                {
                    foreach (var node in sourceNode.TreeView.SelectedNodes)
                    {
                        PerformDragAndDrop(dropPosition, node, destNode);
                    }
                }

                destNode.Expanded = true;

                foreach (var node in ctlPages.GetAllNodes())
                {
                    node.Selected = node.Value == e.SourceDragNode.Value;
                }
            }
        }

        protected void CtlPagesNodeEdit(object sender, RadTreeNodeEditEventArgs e)
        {
            var objTabController = new TabController();
            var objTab = objTabController.GetTab(int.Parse(e.Node.Value), PortalId, false);

            //Check for invalid 
            if (!IsValidTabName(e.Text))
            {                                                
                e.Node.Text = objTab.TabName;
                e.Text = objTab.TabName;                
            }
            else
            {
                objTab.TabName = e.Text;
                objTabController.UpdateTab(objTab);
            }

            BindTreeAndShowTab(objTab.TabID);
        }

        protected void OnExpandTreeClick(object sender, EventArgs e)
        {
            var btn = (LinkButton) sender;
            if (btn.CommandName.ToLower() == "expand")
            {
                ctlPages.ExpandAllNodes();
                btn.CommandName = "Collapse";
                cmdExpandTree.Text = LocalizeString("CollapseAll");
            }
            else
            {
                ctlPages.CollapseAllNodes();
                ctlPages.Nodes[0].Expanded = true;
                btn.CommandName = "Expand";
                cmdExpandTree.Text = LocalizeString("ExpandAll");
            }
        }

        protected void GrdModulesNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var lst = new List<ModuleInfo>();

            if (ctlPages.SelectedNode != null)
            {
                var tabid = Convert.ToInt32(ctlPages.SelectedNode.Value);
                var moduleController = new ModuleController();
                var dic = moduleController.GetTabModules(tabid);

                lst.AddRange(dic.Values.Where(objModule => objModule.IsDeleted == false));
            }

            grdModules.DataSource = lst;
        }

        protected void CtlPagesNodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            AddChildnodes(e.Node);
        }

        protected void BtnBulkCreateClick(object sender, EventArgs e)
        {
            var strValue = txtBulk.Text;
            strValue = strValue.Replace("\r", "\n");
            strValue = strValue.Replace(Environment.NewLine, "\n");
            strValue = strValue.Replace("\n" + "\n", "\n").Trim();

            if (!IsValidTabName(strValue))
            {                
                return;
            }

            var pages = strValue.Split(char.Parse("\n"));
            var parentId = Convert.ToInt32(((Button) sender).CommandArgument);
            var tabController = new TabController();
            var rootTab = tabController.GetTab(parentId, PortalId, true);
            var tabs = new List<TabInfo>();

            foreach (var strLine in pages)
            {
                var oTab = new TabInfo {TabName = strLine};
                if (strLine.StartsWith(">"))
                {
                    oTab.Level = strLine.LastIndexOf(">") + 1;
                }
                else
                {
                    oTab.Level = 0;
                }
                tabs.Add(oTab);
            }

            var currentIndex = -1;
            foreach (var oTab in tabs)
            {
                currentIndex += 1;

                try
                {
                    oTab.TabID = oTab.TabName.StartsWith(">") == false
                                     ? CreateTabFromParent(rootTab, oTab.TabName, parentId)
                                     : CreateTabFromParent(rootTab, oTab.TabName.Replace(">", ""), GetParentTabId(tabs, currentIndex, oTab.Level - 1));
                }
                catch(Exception ex)
                {                    
                    ShowErrorMessage(ex.ToString());
                    //Instrumentation.DnnLog.Error(ex); --this code shows unexpected results.
                }
            }

            BindTree();

            var tabId = Convert.ToInt32(tabs[0].TabID);
            if (tabId != Null.NullInteger)
            {
                BindTab(tabId);
                pnlDetails.Visible = true;
                pnlBulk.Visible = false;
                grdModules.Rebind();
            }
            else
            {
                tabId = parentId;
            }
            ctlPages.FindNodeByValue(tabId.ToString()).Selected = true;
            ctlPages.FindNodeByValue(tabId.ToString()).ExpandParentNodes();            
        }

        protected void CmdDeleteModuleClick(object sender, EventArgs e)
        {
            var moduleId = Convert.ToInt32(((ImageButton) sender).CommandArgument);
            var tabId = Convert.ToInt32(ctlPages.SelectedNode.Value);

            var moduleController = new ModuleController();
            moduleController.DeleteTabModule(tabId, moduleId, true);
            moduleController.ClearCache(tabId);

            grdModules.Rebind();
        }

        protected void CmdUpdateClick(object sender, EventArgs e)
        {
            //Often times grid stays but node is not selected (e.g. when node is deleted or update page is clicked)
            if (ctlPages.SelectedNode == null)
                return;

            var intTab = Convert.ToInt32(ctlPages.SelectedNode.Value);            
            var tabcontroller = new TabController();
            var tab = tabcontroller.GetTab(intTab, PortalId, true);
            if (tab != null)
            {
                tab.TabName = txtName.Text;
                tab.Title = txtTitle.Text;
                tab.Description = txtDescription.Text;
                tab.KeyWords = txtKeywords.Text;
                tab.IsVisible = chkVisible.Checked;
                tab.DisableLink = chkDisabled.Checked;

                tab.IsDeleted = false;
                tab.Url = ctlURL.Url;

                tab.SkinSrc = drpSkin.SelectedValue;
                tab.ContainerSrc = drpContainer.SelectedValue;
                tab.TabPath = Globals.GenerateTabPath(tab.ParentId, tab.TabName);

                tab.TabPermissions.Clear();
                if (tab.PortalID != Null.NullInteger)
                {
                    tab.TabPermissions.AddRange(dgPermissions.Permissions);
                }

                //Check for invalid 
                if (!IsValidTabName(tab.TabName))
                {                
                    return;
                }

                if (txtRefresh.Text.Length > 0 && IsNumeric(txtRefresh.Text))
                {
                    tab.RefreshInterval = Convert.ToInt32(txtRefresh.Text);
                }

                tab.SiteMapPriority = float.Parse(txtSitemapPriority.Text);
                tab.PageHeadText = txtMeta.Text;
                tab.IsSecure = chkSecure.Checked;
                tab.PermanentRedirect = chkPermanentRedirect.Checked;

                var iconFile = ctlIcon.Url;
                var iconFileLarge = ctlIconLarge.Url;

                tab.IconFile = iconFile;
                tab.IconFileLarge = iconFileLarge;

                tab.Terms.Clear();
                tab.Terms.AddRange(termsSelector.Terms);

                tabcontroller.UpdateTab(tab);

                BindTree();

                //keep the tab selected
                SelectedNode = intTab.ToString();
                ctlPages.FindNodeByValue(SelectedNode).Selected = true;
                ctlPages.FindNodeByValue(SelectedNode).ExpandParentNodes();
            }
        }

        #endregion

        #region Private Methods

        private void BindTab(int tabId)
        {
            var tabController = new TabController();
            var tab = tabController.GetTab(tabId, PortalId, true);

            if (tab != null)
            {
                SelectedNode = tabId.ToString();

                //Bind TabPermissionsGrid to TabId 
                dgPermissions.TabID = tab.TabID;
                dgPermissions.DataBind();

                cmdMore.NavigateUrl = ModuleContext.EditNavUrl(tabId, "",false, "ctl=Tab", "action=edit", "returntabid=" + TabId);

                txtTitle.Text = tab.Title;
                txtName.Text = tab.TabName;
                chkVisible.Checked = tab.IsVisible;

                txtSitemapPriority.Text = tab.SiteMapPriority.ToString();
                txtDescription.Text = tab.Description;
                txtKeywords.Text = tab.KeyWords;
                txtMeta.Text = tab.PageHeadText;
                txtRefresh.Text = tab.RefreshInterval.ToString();

                drpSkin.SelectedValue = tab.SkinSrc;
                drpContainer.SelectedValue = tab.ContainerSrc;

                if (tab.Url == "")
                {
                    ctlURL.UrlType = "N";
                }
                else
                {
                    ctlURL.Url = tab.Url;
                }

                chkPermanentRedirect.Checked = tab.PermanentRedirect;
                txtKeywords.Text = tab.KeyWords;
                txtDescription.Text = tab.Description;

                chkDisabled.Checked = tab.DisableLink;
                if (tab.TabID == PortalSettings.AdminTabId || tab.TabID == PortalSettings.SplashTabId || 
                    tab.TabID == PortalSettings.HomeTabId || tab.TabID == PortalSettings.LoginTabId ||
                    tab.TabID == PortalSettings.UserTabId || tab.TabID == PortalSettings.SuperTabId)
                {
                    chkDisabled.Enabled = false;
                }

                if (PortalSettings.SSLEnabled)
                {
                    chkSecure.Enabled = true;
                    chkSecure.Checked = tab.IsSecure;
                }
                else
                {
                    chkSecure.Enabled = false;
                    chkSecure.Checked = tab.IsSecure;
                }

                ctlIcon.Url = tab.IconFile;
                ctlIconLarge.Url = tab.IconFileLarge;

                ShowPermissions(!tab.IsSuperTab && TabPermissionController.CanAdminPage());

                termsSelector.PortalId = tab.PortalID;
                termsSelector.Terms = tab.Terms;
                termsSelector.DataBind();
            }
        }

        private void BindSkinsAndContainers()
        {
            var portalController = new PortalController();
            var portal = portalController.GetPortal(PortalSettings.PortalId);

            var skins = SkinController.GetSkins(portal, SkinController.RootSkin, SkinScope.All)
                                         .ToDictionary(skin => skin.Key, skin => skin.Value);
            var containers = SkinController.GetSkins(portal, SkinController.RootContainer, SkinScope.All)
                                                    .ToDictionary(skin => skin.Key, skin => skin.Value);

            drpSkin.Items.Clear();
            drpSkin.DataSource = skins;
            drpSkin.DataBind();
            drpSkin.Items.Insert(0, new ListItem(Localization.GetString("DefaultSkin", LocalResourceFile), ""));

            drpContainer.Items.Clear();
            drpContainer.DataSource = containers;
            drpContainer.DataBind();
            drpContainer.Items.Insert(0, new ListItem(Localization.GetString("DefaultContainer", LocalResourceFile), ""));
        }

        private void CheckSecurity()
        {
            if ((! (TabPermissionController.HasTabPermission("CONTENT"))) && ! (ModulePermissionController.HasModulePermission(ModuleConfiguration.ModulePermissions, "CONTENT, EDIT")))
            {
                Response.Redirect(Globals.NavigateURL("Access Denied"), true);
            }
        }

        private void LocalizeControl()
        {
            ctlIcon.ShowFiles = true;
            ctlIcon.ShowImages = true;
            ctlIcon.ShowTabs = false;
            ctlIcon.ShowUrls = false;
            ctlIcon.Required = false;

            ctlIcon.ShowLog = false;
            ctlIcon.ShowNewWindow = false;
            ctlIcon.ShowTrack = false;
            ctlIcon.FileFilter = Globals.glbImageFileTypes;
            ctlIcon.Width = "275px";

            ctlIconLarge.ShowFiles = ctlIcon.ShowFiles;
            ctlIconLarge.ShowImages = ctlIcon.ShowImages;
            ctlIconLarge.ShowTabs = ctlIcon.ShowTabs;
            ctlIconLarge.ShowUrls = ctlIcon.ShowUrls;
            ctlIconLarge.Required = ctlIcon.Required;

            ctlIconLarge.ShowLog = ctlIcon.ShowLog;
            ctlIconLarge.ShowNewWindow = ctlIcon.ShowNewWindow;
            ctlIconLarge.ShowTrack = ctlIcon.ShowTrack;
            ctlIconLarge.FileFilter = ctlIcon.FileFilter;
            ctlIconLarge.Width = ctlIcon.Width;

            ctlPages.ContextMenus[0].Items[0].Text = LocalizeString("ViewPage");
            ctlPages.ContextMenus[0].Items[1].Text = LocalizeString("EditPage");
            ctlPages.ContextMenus[0].Items[2].Text = LocalizeString("DeletePage");
            ctlPages.ContextMenus[0].Items[3].Text = LocalizeString("MovePageUp");
            ctlPages.ContextMenus[0].Items[4].Text = LocalizeString("MovePageDown");
            ctlPages.ContextMenus[0].Items[5].Text = LocalizeString("AddPage");
            ctlPages.ContextMenus[0].Items[6].Text = LocalizeString("HidePage");
            ctlPages.ContextMenus[0].Items[7].Text = LocalizeString("ShowPage");
            ctlPages.ContextMenus[0].Items[8].Text = LocalizeString("EnablePage");
            ctlPages.ContextMenus[0].Items[9].Text = LocalizeString("DisablePage");
            ctlPages.ContextMenus[0].Items[10].Text = LocalizeString("MakeHome");

            lblBulkIntro.Text = LocalizeString("BulkCreateIntro");
            btnBulkCreate.Text = LocalizeString("btnBulkCreate");

            ctlPages.ContextMenus[0].Items[0].ImageUrl = IconView;
            ctlPages.ContextMenus[0].Items[1].ImageUrl = IconEdit;
            ctlPages.ContextMenus[0].Items[2].ImageUrl = IconDelete;
            ctlPages.ContextMenus[0].Items[3].ImageUrl = IconUp;
            ctlPages.ContextMenus[0].Items[4].ImageUrl = IconDown;
            ctlPages.ContextMenus[0].Items[5].ImageUrl = IconAdd;
            ctlPages.ContextMenus[0].Items[6].ImageUrl = IconPageHidden;
            ctlPages.ContextMenus[0].Items[7].ImageUrl = IconPageHidden;
            ctlPages.ContextMenus[0].Items[8].ImageUrl = IconPageDisabled;
            ctlPages.ContextMenus[0].Items[9].ImageUrl = IconPageDisabled;
            ctlPages.ContextMenus[0].Items[10].ImageUrl = IconHome;

            rblMode.Items[0].Text = LocalizeString("ShowPortalTabs");
            rblMode.Items[1].Text = LocalizeString("ShowHostTabs");

            cmdExpandTree.Text = LocalizeString("ExpandAll");
            lblDisabled.Text = LocalizeString("lblDisabled");
            lblHidden.Text = LocalizeString("lblHidden");
            lblHome.Text = LocalizeString("lblHome");
            lblSecure.Text = LocalizeString("lblSecure");
            lblEveryone.Text = LocalizeString("lblEveryone");
            lblRegistered.Text = LocalizeString("lblRegistered");
            lblAdminOnly.Text = LocalizeString("lblAdminOnly");
        }

        private void AddAttributes(ref RadTreeNode node, TabInfo tab)
        {
            var canView = true;
            var canEdit = true;
            var canAdd = true;
            var canDelete = true;
            var canHide = true;
            var canMakeVisible = true;
            var canEnable = true;
            var canDisable = true;
            var canMakeHome = true;

            if (node.Attributes["isPortalRoot"] != null && Boolean.Parse(node.Attributes["isPortalRoot"]))
            {
                canView = false;
                canEdit = false;
                canDelete = false;
                canHide = false;
                canMakeVisible = false;
                canEnable = false;
                canDisable = false;
                canMakeHome = false;
            }
            else if (tab == null)
            {
                canView = false;
                canEdit = false;
                canAdd = false;
                canDelete = false;
                canHide = false;
                canMakeVisible = false;
                canEnable = false;
                canDisable = false;
                canMakeHome = false;
            }
            else
            {
                if (TabController.IsSpecialTab(tab.TabID, PortalSettings) || rblMode.SelectedValue == "H")
                {
                    canDelete = false;
                    canMakeHome = false;
                }

                if (tab.IsVisible)
                {
                    canMakeVisible = false;
                }
                else
                {
                    canHide = false;
                }

                if (tab.DisableLink)
                {
                    canDisable = false;
                }
                else
                {
                    canEnable = false;
                }
            }

            node.Attributes.Add("CanView", canView.ToString());
            node.Attributes.Add("CanEdit", canEdit.ToString());
            node.Attributes.Add("CanAdd", canAdd.ToString());
            node.Attributes.Add("CanDelete", canDelete.ToString());
            node.Attributes.Add("CanHide", canHide.ToString());
            node.Attributes.Add("CanMakeVisible", canMakeVisible.ToString());
            node.Attributes.Add("CanEnable", canEnable.ToString());
            node.Attributes.Add("CanDisable", canDisable.ToString());
            node.Attributes.Add("CanMakeHome", canMakeHome.ToString());
        }

        private void BindTree()
        {
            ctlPages.Nodes.Clear();

            var rootNode = new RadTreeNode();
            var strParent = "-1";

            if (Settings["ParentPageFilter"] != null)
            {
                strParent = Convert.ToString(Settings["ParentPageFilter"]);
            }

            if (strParent == "-1")
            {               
                rootNode.Text = PortalSettings.PortalName;
                rootNode.ImageUrl = IconPortal;
                rootNode.Value = Null.NullInteger.ToString();
                rootNode.Expanded = true;
                rootNode.AllowEdit = false;
                rootNode.EnableContextMenu = true;
                rootNode.Attributes.Add("isPortalRoot", "True");
                AddAttributes(ref rootNode, null);
            }
            else
            {
                var tabController = new TabController();
                var parent = tabController.GetTab(Convert.ToInt32(strParent), PortalId, false);
                if (parent != null)
                {
                    rootNode.Text = parent.TabName;
                    rootNode.ImageUrl = IconPortal;
                    rootNode.Value = parent.TabID.ToString();
                    rootNode.Expanded = true;
                    rootNode.EnableContextMenu = true;
                    rootNode.PostBack = false;
                }
            }


            foreach (var tab in Tabs)
            {
                if (strParent != "-1")
                {
                    if (tab.ParentId == Convert.ToInt32(strParent))
                    {
                        var node = new RadTreeNode {Text = string.Format("{0} {1}", tab.TabName, GetNodeStatusIcon(tab)), Value = tab.TabID.ToString(), AllowEdit = true, ImageUrl = GetNodeIcon(tab)};
                        AddAttributes(ref node, tab);

                        AddChildnodes(node);
                        rootNode.Nodes.Add(node);
                    }
                }
                else
                {
                    if (tab.Level == 0)
                    {
                        var node = new RadTreeNode {Text = string.Format("{0} {1}", tab.TabName, GetNodeStatusIcon(tab)), Value = tab.TabID.ToString(), AllowEdit = true, ImageUrl = GetNodeIcon(tab)};
                        AddAttributes(ref node, tab);

                        AddChildnodes(node);
                        rootNode.Nodes.Add(node);
                    }
                }
            }

            ctlPages.Nodes.Add(rootNode);
            //AttachContextMenu(ctlPages)

            if (SelectedNode != null)
            {
                if (! Page.IsPostBack)
                {
                    try
                    {
                        ctlPages.FindNodeByValue(SelectedNode).Selected = true;
                        ctlPages.FindNodeByValue(SelectedNode).ExpandParentNodes();
                        var tabid = Convert.ToInt32(SelectedNode);
                        BindTab(tabid);
                        pnlDetails.Visible = true;
                        pnlBulk.Visible = false;
                        grdModules.Rebind();
                    }
                    catch(Exception exc)
                    {
                        Exceptions.ProcessModuleLoadException(this, exc);
                    }
                }
            }
        }

        private void BindTreeAndShowTab(int tabId)
        {
            BindTree();
            ctlPages.FindNodeByValue(SelectedNode).Selected = true;
            ctlPages.FindNodeByValue(SelectedNode).ExpandParentNodes();
            pnlDetails.Visible = true;
            BindTab(tabId);
        }

        private void ShowPermissions(bool show)
        {
            PermissionsSection.Visible = show;
        }

        private void AddChildnodes(RadTreeNode parentNode)
        {
            parentNode.Nodes.Clear();

            var parentId = int.Parse(parentNode.Value);

            foreach (var objTab in Tabs)
            {
                if (objTab.ParentId == parentId)
                {
                    var node = new RadTreeNode {Text = string.Format("{0} {1}", objTab.TabName, GetNodeStatusIcon(objTab)), Value = objTab.TabID.ToString(), AllowEdit = true, ImageUrl = GetNodeIcon(objTab)};
                    AddAttributes(ref node, objTab);
                    //If objTab.HasChildren Then
                    //    node.ExpandMode = TreeNodeExpandMode.ServerSide
                    //End If

                    AddChildnodes(node);
                    parentNode.Nodes.Add(node);
                }
            }
        }

        private string GetNodeStatusIcon(TabInfo tab)
        {
            if (tab.DisableLink)
            {
                return "<img src=\"" + IconPageDisabled + "\" class=\"statusicon\" />";
            }

            if (tab.IsVisible == false)
            {
                return "<img src=\"" + IconPageHidden + "\" class=\"statusicon\" />";
            }

            return "";
        }

        private string GetNodeIcon(TabInfo tab)
        {
            if (PortalSettings.HomeTabId == tab.TabID)
            {
                return IconHome;
            }

            if (IsSecuredTab(tab))
            {
                if (IsAdminTab(tab))
                {
                    return AdminOnlyIcon;
                }

                if (IsRegisteredUserTab(tab))
                {
                    return RegisteredUsersIcon;
                }

                return SecuredIcon;
            }

            return AllUsersIcon;
        }

        private static bool IsNumeric(object expression)
        {
            if (expression == null)
                return false;

            double testDouble;
            if (double.TryParse(expression.ToString(), out testDouble))
                return true;

            //VB's 'IsNumeric' returns true for any boolean value:
            bool testBool;
            return bool.TryParse(expression.ToString(), out testBool);
        }

        private static bool IsSecuredTab(TabInfo tab)
        {
            var perms = tab.TabPermissions;
            return perms.Cast<TabPermissionInfo>().All(perm => perm.RoleName != "All Users" || !perm.AllowAccess);
        }

        private bool IsRegisteredUserTab(TabInfo tab)
        {
            var perms = tab.TabPermissions;
            return perms.Cast<TabPermissionInfo>().Any(perm => perm.RoleName == PortalSettings.RegisteredRoleName && perm.AllowAccess);
        }

        private bool IsAdminTab(TabInfo tab)
        {
            var perms = tab.TabPermissions;
            return perms.Cast<TabPermissionInfo>().All(perm => perm.RoleName == PortalSettings.AdministratorRoleName || !perm.AllowAccess);
        }

        private void PerformDragAndDrop(RadTreeViewDropPosition dropPosition, RadTreeNode sourceNode, RadTreeNode destNode)
        {
            var tabController = new TabController();
            var sourceTab = tabController.GetTab(int.Parse(sourceNode.Value), PortalId, false);
            var targetTab = tabController.GetTab(int.Parse(destNode.Value), PortalId, false);

            switch (dropPosition)
            {
                case RadTreeViewDropPosition.Over:
                    if (! (sourceNode.IsAncestorOf(destNode)))
                    {
                        sourceNode.Owner.Nodes.Remove(sourceNode);
                        destNode.Nodes.Add(sourceNode);
                        MoveTabToParent(sourceTab, targetTab);
                    }
                    break;
                case RadTreeViewDropPosition.Above:
                    sourceNode.Owner.Nodes.Remove(sourceNode);
                    destNode.InsertBefore(sourceNode);
                    MoveTab(sourceTab, targetTab, Position.Above);
                    break;
                case RadTreeViewDropPosition.Below:
                    sourceNode.Owner.Nodes.Remove(sourceNode);
                    destNode.InsertAfter(sourceNode);
                    MoveTab(sourceTab, targetTab, Position.Below);
                    break;
            }

            DataCache.ClearTabsCache(PortalId);
        }

        private void MoveTabToParent(TabInfo tab, TabInfo targetTab)
        {
            var tabController = new TabController();

            //get current siblings of moving tab
            var siblingTabs = GetSiblingTabs(tab);
            var siblingCount = siblingTabs.Count;

            //move all current siblings of moving tab one level up in order
            var tabIndex = GetIndexOfTab(tab, siblingTabs);
            UpdateTabOrder(siblingTabs, tabIndex + 1, siblingCount - 1, -2);

            tab.TabOrder = -1;
            tab.ParentId = targetTab.TabID;
            tabController.UpdateTab(tab);

            UpdateTabOrder(tab, true);            
            ShowSuccessMessage(string.Format(Localization.GetString("TabMoved", LocalResourceFile), tab.TabName));
        }

        private void MoveTab(TabInfo tab, TabInfo targetTab, Position position)
        {
            var tabController = new TabController();

            //get current siblings of moving tab
            var siblingTabs = GetSiblingTabs(tab);
            int siblingCount = siblingTabs.Count;

            //move all current siblings of moving tab one level up in order
            int tabIndex = GetIndexOfTab(tab, siblingTabs);
            UpdateTabOrder(siblingTabs, tabIndex + 1, siblingCount - 1, -2);

            //get siblings of new position
            siblingTabs = GetSiblingTabs(targetTab);
            siblingCount = siblingTabs.Count;

            //First make sure the list of siblings at the new position is sorted and spaced
            UpdateTabOrder(siblingTabs, tab.CultureCode, 2);

            //Find Index position of new positin in the Sibling List
            var targetIndex = GetIndexOfTab(targetTab, siblingTabs);

            switch (position)
            {
                case Position.Above:
                    targetIndex = targetIndex - 1;
                    break;
                case Position.Below:
                    break;
            }

            //We need to update the taborder for items that were after that position
            UpdateTabOrder(siblingTabs, targetIndex + 1, siblingCount - 1, 2);

            //Get the descendents of old tab position now before the new parentid is updated
            var descendantTabs = tabController.GetTabsByPortal(tab.PortalID).DescendentsOf(tab.TabID);

            //Update the current Tab to reflect new parent id and new taborder
            tab.ParentId = targetTab.ParentId;
            
            tabController.UpdateTab(tab);

            //Update the moving tabs level and tabpath
            tab.Level = targetTab.Level;

            //Update TabOrder as the previous UpdateTab call pushes the tab to the bottom
            switch (position)
            {
                case Position.Above:
                    tab.TabOrder = targetTab.TabOrder - 2;
                    break;
                case Position.Below:
                    tab.TabOrder = targetTab.TabOrder + 1;
                    break;
            }

            UpdateTabOrder(tab, true);

            //Update the Descendents of the moving tab
            UpdateDescendantLevel(descendantTabs, tab.Level + 1);
            ShowSuccessMessage(string.Format(Localization.GetString("TabMoved", LocalResourceFile), tab.TabName));
        }

        #endregion

        #region Public Methods

        public string ModuleEditUrl(int moduleId)
        {
            if (IsNumeric(moduleId))
            {
                var moduleController = new ModuleController();
                var module = moduleController.GetModule(moduleId);
                if (module != null)
                {
                    return ModuleContext.EditNavUrl(module.TabID, "", false,"ctl=Module", "ModuleId=" + moduleId);
                }
            }

            return "#";
        }

        public string GetConfirmString()
        {
            return Localization.GetString("ConfirmDelete", LocalResourceFile);
        }

        #endregion

        #region Tab Moving Helpers

        private void ShowWarningMessage(string message)
        {
            UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
        }

        private void ShowErrorMessage(string message)
        {
            UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.RedError);
        }

        private void ShowSuccessMessage(string message)
        {
            UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.GreenSuccess);
        }

        private void UpdateDescendantLevel(IEnumerable<TabInfo> descendantTabs, int levelDelta)
        {
            //Update the Descendents of this tab
            foreach (var descendent in descendantTabs)
            {
                descendent.Level = descendent.Level + levelDelta;
                UpdateTabOrder(descendent, true);
            }
        }

        private void UpdateTabOrderInternal(IEnumerable<TabInfo> tabs, int increment)
        {
            var tabOrder = 1;
            foreach (var objTab in tabs.OrderBy(t => t.TabOrder))
            {
                if (objTab.IsDeleted)
                {
                    objTab.TabOrder = -1;
                    UpdateTabOrder(objTab, false);

                    //Update the tab order of all child languages
                    foreach (var localizedtab in objTab.LocalizedTabs.Values)
                    {
                        localizedtab.TabOrder = -1;
                        UpdateTabOrder(localizedtab, false);
                    }
                }
                else
                {
                    //Only update the tabOrder if it actually needs to be updated
                    if (objTab.TabOrder != tabOrder)
                    {
                        objTab.TabOrder = tabOrder;
                        UpdateTabOrder(objTab, false);

                        //Update the tab order of all child languages
                        foreach (var localizedtab in objTab.LocalizedTabs.Values)
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

        private void UpdateTabOrder(IEnumerable<TabInfo> tabs, string culture, int increment)
        {
            var portalSettings = PortalController.GetCurrentPortalSettings();
            if (portalSettings != null && portalSettings.ContentLocalizationEnabled)
            {
                UpdateTabOrderInternal(string.IsNullOrEmpty(culture) ? tabs.Where(t => t.CultureCode == portalSettings.DefaultLanguage || string.IsNullOrEmpty(t.CultureCode)) : tabs, increment);
            }
            else
            {
                UpdateTabOrderInternal(tabs, increment);
            }
        }

        private void UpdateTabOrder(List<TabInfo> tabs, int startIndex, int endIndex, int increment)
        {
            for (var index = startIndex; index <= endIndex; index++)
            {
                var objTab = tabs[index];
                objTab.TabOrder += increment;

                //UpdateOrder - Parent hasn't changed so we don't need to regenerate TabPath
                UpdateTabOrder(objTab, false);
            }
        }

        private void UpdateTabOrder(TabInfo objTab, bool updateTabPath)
        {
            if (updateTabPath)
            {
                objTab.TabPath = Globals.GenerateTabPath(objTab.ParentId, objTab.TabName);
            }

            DataProvider.Instance().UpdateTabOrder(objTab.TabID, objTab.TabOrder, objTab.Level, objTab.ParentId, objTab.TabPath, UserController.GetCurrentUserInfo().UserID);
            DataProvider.Instance().UpdateTabVersion(TabId, Guid.NewGuid());

            var objEventLog = new EventLogController();
            objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.TAB_ORDER_UPDATED);
        }

        private static List<TabInfo> GetSiblingTabs(TabInfo objTab)
        {
            var objTabController = new TabController();
            return objTabController.GetTabsByPortal(objTab.PortalID).WithCulture(objTab.CultureCode, true).WithParentId(objTab.ParentId);
        }

        private static int GetIndexOfTab(TabInfo objTab, List<TabInfo> tabs)
        {
            var tabIndex = Null.NullInteger;
            for (var index = 0; index < tabs.Count; index++)
            {
                if (tabs[index].TabID == objTab.TabID)
                {
                    tabIndex = index;
                    break;
                }
            }
            return tabIndex;
        }

        private static int GetParentTabId(List<TabInfo> lstTabs, int currentIndex, int parentLevel)
        {
            var oParent = lstTabs[0];

            for (var i = 0; i < lstTabs.Count; i++)
            {
                if (i == currentIndex)
                {
                    return oParent.TabID;
                }
                if (lstTabs[i].Level == parentLevel)
                {
                    oParent = lstTabs[i];
                }
            }

            return Null.NullInteger;
        }

        private int CreateTabFromParent(TabInfo objRoot, string tabName, int parentId)
        {
            var tab = new TabInfo
                          {
                              PortalID = PortalId,
                              TabName = tabName,
                              ParentId = parentId,
                              Title = "",
                              Description = "",
                              KeyWords = "",
                              IsVisible = true,
                              DisableLink = false,
                              IconFile = "",
                              IconFileLarge = "",
                              IsDeleted = false,
                              Url = "",
                              SkinSrc = "",
                              ContainerSrc = ""
                          };

            if (objRoot != null)
            {
                tab.IsVisible = objRoot.IsVisible;
                tab.DisableLink = objRoot.DisableLink;
                tab.SkinSrc = objRoot.SkinSrc;
                tab.ContainerSrc = objRoot.ContainerSrc;
                tab.CultureCode = objRoot.CultureCode;
            }
            else
            {
                tab.CultureCode = PortalSettings.Current.DefaultLanguage;
            }

            var controller = new TabController();
            var parentTab = controller.GetTab(parentId, PortalId, false);

            if (parentTab != null)
            {
                tab.PortalID = parentTab.PortalID;
                tab.ParentId = parentTab.TabID;
                tab.Level = parentTab.Level + 1;
                if(parentTab.IsSuperTab)
                    ShowPermissions(false);
            }
            else
            {
                //return Null.NullInteger;
                tab.PortalID = PortalId;
                tab.ParentId = Null.NullInteger;
                tab.Level = 0;
            }

            tab.TabPath = Globals.GenerateTabPath(tab.ParentId, tab.TabName);

            //Check for invalid 
            if (!IsValidTabName(tab.TabName))
            {            
                return Null.NullInteger;
            }

            //Validate Tab Path
            var tabID = TabController.GetTabByTabPath(tab.PortalID, tab.TabPath, tab.CultureCode);
            if (tabID != Null.NullInteger)
            {
                var existingTab = controller.GetTab(tabID, tab.PortalID, false);
                if(existingTab != null && existingTab.IsDeleted)
                    ShowWarningMessage(Localization.GetString("TabRecycled", LocalResourceFile));
                else
                    ShowWarningMessage(Localization.GetString("TabExists", LocalResourceFile));

                return Null.NullInteger;
            }

            //Inherit from Parent
            if (objRoot != null)
            {
                tab.TabPermissions.Clear();
                if (tab.PortalID != Null.NullInteger)
                {
                    tab.TabPermissions.AddRange(objRoot.TabPermissions);
                }

                tab.Terms.Clear();
                tab.CultureCode = objRoot.CultureCode;
                tab.StartDate = objRoot.StartDate;
                tab.EndDate = objRoot.EndDate;
                tab.RefreshInterval = objRoot.RefreshInterval;
                tab.SiteMapPriority = objRoot.SiteMapPriority;
                tab.PageHeadText = objRoot.PageHeadText;
                tab.IsSecure = objRoot.IsSecure;
                tab.PermanentRedirect = objRoot.PermanentRedirect;
            }
            else
            {
                tab.CultureCode = PortalSettings.Current.DefaultLanguage;
            }

            var ctrl = new TabController();
            tab.TabID = ctrl.AddTab(tab);
            ApplyDefaultTabTemplate(tab);
            ShowSuccessMessage(string.Format(Localization.GetString("TabCreated", LocalResourceFile),tab.TabName));
            return tab.TabID;                          
        }

        private bool IsValidTabName(string tabName)
        {
            var valid = true;

            if (string.IsNullOrEmpty(tabName.Trim()))
            {
                ShowWarningMessage(Localization.GetString("EmptyTabName", LocalResourceFile));
                valid = false;
            }
            else if (Regex.IsMatch(tabName, "^AUX$|^CON$|^LPT[1-9]$|^CON$|^COM[1-9]$|^NUL$|^SITEMAP$|^LINKCLICK$|^KEEPALIVE$|^DEFAULT$|^ERRORPAGE$", RegexOptions.IgnoreCase))
            {
                valid = false;
                ShowWarningMessage(string.Format(Localization.GetString("InvalidTabName", LocalResourceFile), tabName));
            }

            return valid;
        }

        private void ApplyDefaultTabTemplate(TabInfo tab)
        {
            var templateFile = Path.Combine(PortalSettings.HomeDirectoryMapPath, "Templates\\" + DefaultPageTemplate);
            var xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(templateFile);
                TabController.DeserializePanes(xmlDoc.SelectSingleNode("//portal/tabs/tab/panes"), tab.PortalID, tab.TabID, PortalTemplateModuleAction.Ignore, new Hashtable());
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                throw new DotNetNukeException("Unable to process page template.", ex, DotNetNukeErrorCode.DeserializePanesFailed);
            }
        }

        #endregion

    }
}