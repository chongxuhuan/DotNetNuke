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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
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

    public partial class View : PortalModuleBase
    {
        private const string DefaultPageTemplate = "Default.page.template";

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

        protected void CmdCopySkinClick(object sender, EventArgs e)
        {
            try
            {
                TabController.CopyDesignToChildren(new TabController().GetTab(Convert.ToInt32(ctlPages.SelectedNode.Value), PortalId, false), drpSkin.SelectedValue, drpContainer.SelectedValue);
                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("DesignCopied", LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess);
            }
            catch (Exception ex)
            {
                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("DesignCopyError", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
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
                int tabid = Convert.ToInt32(e.Node.Value);
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
                    BindTree();
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
                    BindTree();
                    break;
                case "show":
                    objTab.IsVisible = true;
                    objTabController.UpdateTab(objTab);
                    BindTree();
                    break;
                case "disable":
                    objTab.DisableLink = true;
                    objTabController.UpdateTab(objTab);
                    BindTree();
                    break;
                case "enable":
                    objTab.DisableLink = false;
                    objTabController.UpdateTab(objTab);
                    BindTree();
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

                foreach (RadTreeNode node in ctlPages.GetAllNodes())
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
            if (Regex.IsMatch(e.Text, "^AUX$|^CON$|^LPT[1-9]$|^CON$|^COM[1-9]$|^NUL$|^SITEMAP$|^LINKCLICK$|^KEEPALIVE$|^DEFAULT$|^ERRORPAGE$", RegexOptions.IgnoreCase))
            {
                string strInvalid = string.Format(Localization.GetString("InvalidTabName", LocalResourceFile), e.Text);
                e.Node.Text = objTab.TabName;
                e.Text = objTab.TabName;
                ctlAjax.ResponseScripts.Add("alert('" + strInvalid + "');return false;");
            }
            else
            {
                objTab.TabName = e.Text;
                objTabController.UpdateTab(objTab);
            }

            BindTree();
        }

        protected void BtnTreeCommandClick(object sender, ImageClickEventArgs e)
        {
            var btn = (ImageButton) sender;
            if (btn.CommandName.ToLower() == "expand")
            {
                ctlPages.ExpandAllNodes();
                btn.CommandName = "Collapse";
                btnTreeCommand.ImageUrl = TemplateSourceDirectory + "/images/Icon_Collapse.png";
                btnTreeCommand.ToolTip = Localization.GetString("Collapse", LocalResourceFile);
            }
            else
            {
                ctlPages.CollapseAllNodes();
                ctlPages.Nodes[0].Expanded = true;
                btn.CommandName = "Expand";
                btnTreeCommand.ImageUrl = TemplateSourceDirectory + "/images/Icon_Expand.png";
                btnTreeCommand.ToolTip = Localization.GetString("Expand", LocalResourceFile);
            }
        }

        protected void GrdModulesNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var lst = new List<ModuleInfo>();

            if (ctlPages.SelectedNode != null)
            {
                int tabid = Convert.ToInt32(ctlPages.SelectedNode.Value);
                var moduleController = new ModuleController();
                var dic = moduleController.GetTabModules(tabid);

                lst.AddRange(dic.Values.Where(objModule => objModule.IsDeleted == false));
            }

            grdModules.DataSource = lst;
        }

        protected void CtlAjaxAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            string cmd = e.Argument.Split(char.Parse("|"))[0];
            string arg = e.Argument.Split(char.Parse("|"))[1];

            switch (cmd.ToLower())
            {
                case "getmenuitems":
                    var tabController = new TabController();
                    var tab = tabController.GetTab(int.Parse(arg), PortalId, false);
                    if (tab != null)
                    {
                        var ctl = (RadContextMenu) (ctlPages.FindControl("ctlContext"));
                        if (ctl != null)
                        {
                            ctl.Items[1].Visible = false;
                        }
                    }
                    break;
            }
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

            if (strValue.Length == 0)
            {
                return;
            }

            var pages = strValue.Split(char.Parse("\n"));
            int parentId = Convert.ToInt32(((Button) sender).CommandArgument);
            var tabController = new TabController();
            var rootTab = tabController.GetTab(parentId, PortalId, true);
            var tabs = new List<TabInfo>();

            foreach (string strLine in pages)
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

                oTab.TabID = oTab.TabName.StartsWith(">") == false ? CreateTabFromParent(rootTab, oTab.TabName, parentId) : CreateTabFromParent(rootTab, oTab.TabName.Replace(">", ""), GetParentTabId(tabs, currentIndex, oTab.Level - 1));
            }

            BindTree();

            var tabId = Convert.ToInt32(tabs[0].TabID);
            ctlPages.FindNodeByValue(tabId.ToString()).Selected = true;
            ctlPages.FindNodeByValue(tabId.ToString()).ExpandParentNodes();

            BindTab(tabId);
            pnlDetails.Visible = true;
            pnlBulk.Visible = false;
            grdModules.Rebind();
        }

        public void CmdDeleteModuleClick(object sender, EventArgs e)
        {
            int moduleId = Convert.ToInt32(((ImageButton) sender).CommandArgument);
            int tabId = Convert.ToInt32(ctlPages.SelectedNode.Value);

            var moduleController = new ModuleController();
            moduleController.DeleteTabModule(tabId, moduleId, true);
            moduleController.ClearCache(tabId);

            grdModules.Rebind();
        }

        protected void CmdUpdateClick(object sender, EventArgs e)
        {
            int intTab = Convert.ToInt32(ctlPages.SelectedNode.Value);
            var tabcontroller = new TabController();
            TabInfo tab = tabcontroller.GetTab(intTab, PortalId, true);
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

                if (tab.TabName == "")
                {
                    UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("InvalidTabName", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                    return;
                }

                //Check for invalid 
                if (Regex.IsMatch(tab.TabName, "^AUX$|^CON$|^LPT[1-9]$|^CON$|^COM[1-9]$|^NUL$|^SITEMAP$|^LINKCLICK$|^KEEPALIVE$|^DEFAULT$|^ERRORPAGE$", RegexOptions.IgnoreCase))
                {
                    UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("InvalidTabName", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
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

                string iconFile = ctlIcon.Url;
                string iconFileLarge = ctlIconLarge.Url;

                tab.IconFile = iconFile;
                tab.IconFileLarge = iconFileLarge;

                tab.Terms.Clear();
                tab.Terms.AddRange(termsSelector.Terms);

                tabcontroller.UpdateTab(tab);

                BindTree();
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

                cmdMore.NavigateUrl = Globals.NavigateURL(tabId, "", "ctl=Tab", "action=edit", "returntabid=" + TabId);
                //cmdMore.Target = "_blank"

                txtTitle.Text = tab.Title;
                txtName.Text = tab.TabName;
                chkVisible.Checked = tab.IsVisible;

                txtSitemapPriority.Text = tab.SiteMapPriority.ToString();
                txtDescription.Text = tab.Description;
                txtKeywords.Text = tab.KeyWords;
                txtMeta.Text = tab.PageHeadText;
                txtRefresh.Text = tab.RefreshInterval.ToString();


                if (tab.SkinSrc != "")
                {
                    switch (tab.SkinSrc.Substring(0, 3))
                    {
                        case "[L]":
                            rblSkinMode.SelectedValue = "S";
                            break;
                        case "[G]":
                            rblSkinMode.SelectedValue = "H";
                            break;
                    }
                }
                BindSkins();

                drpSkin.SelectedValue = tab.SkinSrc;

                if (tab.ContainerSrc != "")
                {
                    switch (tab.ContainerSrc.Substring(0, 3))
                    {
                        case "[L]":
                            rblContainerMode.SelectedValue = "S";
                            break;
                        case "[G]":
                            rblContainerMode.SelectedValue = "H";
                            break;
                    }
                }
                BindContainers();

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

        private void BindSkins()
        {
            drpSkin.Items.Clear();

            var portalController = new PortalController();
            var portal = portalController.GetPortal(PortalSettings.PortalId);

            if (rblSkinMode.SelectedValue == "H")
            {
                foreach (var skin in SkinController.GetSkins(portal, SkinController.RootSkin, SkinScope.Host))
                {
                    drpSkin.Items.Add(new RadComboBoxItem(skin.Key, skin.Value));
                }
            }
            else
            {
                foreach (var skin in SkinController.GetSkins(portal, SkinController.RootSkin, SkinScope.Site))
                {
                    drpSkin.Items.Add(new RadComboBoxItem(skin.Key, skin.Value));
                }
            }

            drpSkin.Items.Insert(0, new RadComboBoxItem(Localization.GetString("DefaultSkin", LocalResourceFile), ""));
        }

        private void BindContainers()
        {
            drpContainer.Items.Clear();

            var portalController = new PortalController();
            var portal = portalController.GetPortal(PortalSettings.PortalId);

            if (rblContainerMode.SelectedValue == "H")
            {
                foreach (var container in SkinController.GetSkins(portal, SkinController.RootContainer, SkinScope.Host))
                {
                    drpContainer.Items.Add(new RadComboBoxItem(container.Key, container.Value));
                }
            }
            else
            {
                foreach (var container in SkinController.GetSkins(portal, SkinController.RootContainer, SkinScope.Site))
                {
                    drpContainer.Items.Add(new RadComboBoxItem(container.Key, container.Value));
                }
            }

            drpContainer.Items.Insert(0, new RadComboBoxItem(Localization.GetString("DefaultContainer", LocalResourceFile), ""));
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

            ctlPages.ContextMenus[0].Items[0].Text = Localization.GetString("ViewPage", LocalResourceFile);
            ctlPages.ContextMenus[0].Items[1].Text = Localization.GetString("EditPage", LocalResourceFile);
            ctlPages.ContextMenus[0].Items[2].Text = Localization.GetString("DeletePage", LocalResourceFile);
            ctlPages.ContextMenus[0].Items[3].Text = Localization.GetString("MovePageUp", LocalResourceFile);
            ctlPages.ContextMenus[0].Items[4].Text = Localization.GetString("MovePageDown", LocalResourceFile);
            ctlPages.ContextMenus[0].Items[5].Text = Localization.GetString("AddPage", LocalResourceFile);
            ctlPages.ContextMenus[0].Items[6].Text = Localization.GetString("HidePage", LocalResourceFile);
            ctlPages.ContextMenus[0].Items[7].Text = Localization.GetString("ShowPage", LocalResourceFile);
            ctlPages.ContextMenus[0].Items[8].Text = Localization.GetString("EnablePage", LocalResourceFile);
            ctlPages.ContextMenus[0].Items[9].Text = Localization.GetString("DisablePage", LocalResourceFile);
            ctlPages.ContextMenus[0].Items[10].Text = Localization.GetString("MakeHome", LocalResourceFile);

            lblBulkIntro.Text = Localization.GetString("BulkCreateIntro", LocalResourceFile);
            btnBulkCreate.Text = Localization.GetString("btnBulkCreate", LocalResourceFile);

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


            rblMode.Items[0].Text = Localization.GetString("ShowPortalTabs", LocalResourceFile);
            rblMode.Items[1].Text = Localization.GetString("ShowHostTabs", LocalResourceFile);
            lblHead.Text = Localization.GetString("lblHead", LocalResourceFile);

            lblDisabled.Text = Localization.GetString("lblDisabled", LocalResourceFile);
            lblHidden.Text = Localization.GetString("lblHidden", LocalResourceFile);
            lblHome.Text = Localization.GetString("lblHome", LocalResourceFile);

            lblSecure.Text = Localization.GetString("lblSecure", LocalResourceFile);
            lblEveryone.Text = Localization.GetString("lblEveryone", LocalResourceFile);
            lblRegistered.Text = Localization.GetString("lblRegistered", LocalResourceFile);
            lblAdminOnly.Text = Localization.GetString("lblAdminOnly", LocalResourceFile);
            btnTreeCommand.ToolTip = Localization.GetString("Expand", LocalResourceFile);

            foreach (RadTab rTab in ctlTabstrip.Tabs)
            {
                rTab.Text = Localization.GetString(rTab.Text + ".Tabname", LocalResourceFile);
            }

            foreach (GridColumn col in grdModules.Columns)
            {
                col.HeaderText = Localization.GetString(col.HeaderText + ".Header", LocalResourceFile);
            }
        }

        private void AddAttributes(ref RadTreeNode node, TabInfo tab)
        {
            bool canView = true;
            bool canEdit = true;
            bool canAdd = true;
            bool canDelete = true;
            bool canHide = true;
            bool canMakeVisible = true;
            bool canEnable = true;
            bool canDisable = true;
            bool canMakeHome = true;

            if (node.Attributes["isPortalRoot"] != null && Boolean.Parse(node.Attributes["isPortalRoot"]))
            {
                canView = false;
                canEdit = false;
                canAdd = true;
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
            string strParent = "-1";

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
                        int tabid = Convert.ToInt32(SelectedNode);
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

        private void ShowPermissions(bool show)
        {
            ctlPagePermissions.Visible = show;
        }

        private void AddChildnodes(RadTreeNode parentNode)
        {
            parentNode.Nodes.Clear();

            int parentid = int.Parse(parentNode.Value);

            foreach (TabInfo objTab in Tabs)
            {
                if (objTab.ParentId == parentid)
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

        //private void AttachContextMenu(ref RadTreeView tree)
        //{
        //    var menu = new RadTreeViewContextMenu {ID = "ctxt_PagesTree"};

        //    var item = new RadMenuItem {Text = Localization.GetString("ViewPage", LocalResourceFile), Value = "view", ImageUrl = IconView};
        //    menu.Items.Add(item);

        //    item = new RadMenuItem {Text = Localization.GetString("EditPage", LocalResourceFile), Value = "edit", ImageUrl = IconEdit};
        //    menu.Items.Add(item);

        //    item = new RadMenuItem {Text = Localization.GetString("DeletePage", LocalResourceFile), Value = "delete", ImageUrl = IconDelete};
        //    menu.Items.Add(item);

        //    item = new RadMenuItem {Text = Localization.GetString("MovePageUp", LocalResourceFile), Value = "moveup", ImageUrl = IconUp};
        //    menu.Items.Add(item);

        //    item = new RadMenuItem {Text = Localization.GetString("MovePageDown", LocalResourceFile), Value = "movedown", ImageUrl = IconDown};
        //    menu.Items.Add(item);

        //    item = new RadMenuItem {Text = Localization.GetString("AddPage", LocalResourceFile), Value = "add", ImageUrl = IconAdd};
        //    menu.Items.Add(item);

        //    item = new RadMenuItem {Text = Localization.GetString("UnHidePage", LocalResourceFile), Value = "unhide", ImageUrl = IconPageHidden};
        //    menu.Items.Add(item);

        //    item = new RadMenuItem {Text = Localization.GetString("DisablePage", LocalResourceFile), Value = "disable", ImageUrl = IconPageDisabled};
        //    menu.Items.Add(item);

        //    ctlPages.ContextMenus.Add(menu);
        //}

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
            if (bool.TryParse(expression.ToString(), out testBool))
                return true;

            return false;
        }

        private static bool IsSecuredTab(TabInfo tab)
        {
            var perms = tab.TabPermissions;
            return perms.Cast<TabPermissionInfo>().All(perm => perm.RoleName != "All Users" || !perm.AllowAccess);
        }

        private bool IsRegisteredUserTab(TabInfo tab)
        {
            TabPermissionCollection perms = tab.TabPermissions;
            return perms.Cast<TabPermissionInfo>().Any(perm => perm.RoleName == PortalSettings.RegisteredRoleName && perm.AllowAccess);
        }

        private bool IsAdminTab(TabInfo tab)
        {
            TabPermissionCollection perms = tab.TabPermissions;
            return perms.Cast<TabPermissionInfo>().All(perm => perm.RoleName == PortalSettings.AdministratorRoleName || !perm.AllowAccess);
        }

        private void PerformDragAndDrop(RadTreeViewDropPosition dropPosition, RadTreeNode sourceNode, RadTreeNode destNode)
        {
            var tabController = new TabController();
            TabInfo sourceTab = tabController.GetTab(int.Parse(sourceNode.Value), PortalId, false);
            TabInfo targetTab = tabController.GetTab(int.Parse(destNode.Value), PortalId, false);

            if (dropPosition == RadTreeViewDropPosition.Over)
            {
                // child
                if (! (sourceNode.IsAncestorOf(destNode)))
                {
                    sourceNode.Owner.Nodes.Remove(sourceNode);
                    destNode.Nodes.Add(sourceNode);
                    MoveTabToParent(sourceTab, targetTab);
                }
            }
            else if (dropPosition == RadTreeViewDropPosition.Above)
            {
                // sibling - above
                sourceNode.Owner.Nodes.Remove(sourceNode);
                destNode.InsertBefore(sourceNode);
                MoveTab(sourceTab, targetTab, Position.Above);
            }
            else if (dropPosition == RadTreeViewDropPosition.Below)
            {
                // sibling - below
                sourceNode.Owner.Nodes.Remove(sourceNode);
                destNode.InsertAfter(sourceNode);
                MoveTab(sourceTab, targetTab, Position.Below);
            }

            DataCache.ClearTabsCache(PortalId);
        }

        private void MoveTabToParent(TabInfo tab, TabInfo targetTab)
        {
            var tabController = new TabController();

            //get current siblings of moving tab
            var siblingTabs = GetSiblingTabs(tab);
            int siblingCount = siblingTabs.Count;

            //move all current siblings of moving tab one level up in order
            int tabIndex = GetIndexOfTab(tab, siblingTabs);
            UpdateTabOrder(siblingTabs, tabIndex + 1, siblingCount - 1, -2);

            tab.TabOrder = -1;
            tab.ParentId = targetTab.TabID;
            tabController.UpdateTab(tab);

            UpdateTabOrder(tab, true);
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
            int targetIndex = GetIndexOfTab(targetTab, siblingTabs);

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
            List<TabInfo> descendantTabs = tabController.GetTabsByPortal(tab.PortalID).DescendentsOf(tab.TabID);

            //Update the current Tab to reflect new parent id and new taborder
            tab.ParentId = targetTab.ParentId;

            switch (position)
            {
                case Position.Above:
                    tab.TabOrder = targetTab.TabOrder - 2;
                    break;
                case Position.Below:
                    tab.TabOrder = targetTab.TabOrder + 2;
                    break;
            }

            tabController.UpdateTab(tab);

            //Update the moving tabs level and tabpath
            if (tab.Level < 0)
            {
                tab.Level = 0;
            }
            UpdateTabOrder(tab, true);

            //Update the Descendents of the moving tab
            UpdateDescendantLevel(descendantTabs, tab.Level + 1);
        }

        #endregion

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cmdCopySkin.Click += CmdCopySkinClick;
            rblMode.SelectedIndexChanged += RblModeSelectedIndexChanged;
            ctlPages.NodeClick += CtlPagesNodeClick;
            ctlPages.ContextMenuItemClick += CtlPagesContextMenuItemClick;
            ctlPages.NodeDrop += CtlPagesNodeDrop;
            ctlPages.NodeEdit += CtlPagesNodeEdit;
            btnTreeCommand.Click += BtnTreeCommandClick;
            grdModules.NeedDataSource += GrdModulesNeedDataSource;
            ctlAjax.AjaxRequest += CtlAjaxAjaxRequest;
            ctlPages.NodeExpand += CtlPagesNodeExpand;
            btnBulkCreate.Click += BtnBulkCreateClick;
            cmdUpdate.Click += CmdUpdateClick;

            AJAX.RegisterScriptManager();

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
                    //ctlSkin.SkinRoot = DotNetNuke.UI.Skins.SkinController.RootSkin
                    //ctlContainer.SkinRoot = DotNetNuke.UI.Skins.SkinController.RootContainer

                    LocalizeControl();

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
                    return Globals.NavigateURL(module.TabID, "", "ctl=Module", "ModuleId=" + moduleId);
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
            int tabOrder = 1;
            foreach (var objTab in tabs.OrderBy(t => t.TabOrder))
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
                    if (objTab.TabOrder != tabOrder)
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

        private void UpdateTabOrder(IEnumerable<TabInfo> tabs, string culture, int increment)
        {
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
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
            int tabIndex = Null.NullInteger;
            for (int index = 0; index < tabs.Count; index++)
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

            for (int i = 0; i < lstTabs.Count; i++)
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
            if (Regex.IsMatch(tab.TabName, "^AUX$|^CON$|^LPT[1-9]$|^CON$|^COM[1-9]$|^NUL$|^SITEMAP$|^LINKCLICK$|^KEEPALIVE$|^DEFAULT$|^ERRORPAGE$", RegexOptions.IgnoreCase))
            {
                return Null.NullInteger;
            }

            //Validate Tab Path
            int tabID = TabController.GetTabByTabPath(tab.PortalID, tab.TabPath, tab.CultureCode);
            if (tabID != Null.NullInteger)
            {
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
            return tab.TabID;                          
        }

        private void ApplyDefaultTabTemplate(TabInfo tab)
        {
            string templateFile = Path.Combine(PortalSettings.HomeDirectoryMapPath, "Templates\\" + DefaultPageTemplate);
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