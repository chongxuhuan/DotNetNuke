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
using System;
using System.Text.RegularExpressions;
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;
using WatiN.Core.Comparers;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The ribbon bar style control panel object.
    /// </summary>
    public class RibbonBar : WatiNBase
    {
        #region Constructors

        public RibbonBar(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public RibbonBar(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }


        #endregion

        #region Public Properties

        #region Divs
        public Div RibbonBarWrapper
        {
            get { return IEInstance.Div(Find.ById("dnnCPWrapRight")); }
        }
        public Div HeaderDiv
        {
            get { return IEInstance.Div(Find.ByClass("dnnCPHeader dnnClear")); }
        }
        /// <summary>
        /// The div containing the menus in the ribbon bar.
        /// </summary>
        public Div AdminMenusDiv
        {
            get { return IEInstance.Div(Find.ById(s => s.EndsWith("_RibbonBar_adminMenus"))); }
        }
        public new Div RibbonBarDiv
        {
            get { return IEInstance.Div(Find.ById(new Regex("dnn_(cp_)?RibbonBar_BodyPanel"))); }
        }
        /// <summary>
        /// The div containing elements in the module menu.
        /// </summary>
        public Div CommonTasksDiv
        {
            get { return IEInstance.Div(Find.ById(s => s.EndsWith("RibbonBar_CommonTasksPanel"))); }
        }
        /// <summary>
        /// The div containing elements in the pages menu. 
        /// </summary>
        public Div CurrentPageDiv
        {
            get { return IEInstance.Div(Find.ById(s => s.EndsWith("_RibbonBar_CurrentPagePanel"))); }
        }
        /// <summary>
        /// The div containing the options in the module telerik drop down.
        /// </summary>
        public Div ModuleSelectDiv
        {
            get { return IEInstance.Div(Find.ById(new Regex(@"dnn_(cp_)?RibbonBar_AddMod_ctl\d\d_DropDown"))); }
        }
        [Obsolete("Element no longer exists in 6.x")]
        public Div InsertSelectDiv
        {
            get { return IEInstance.Div(Find.ById("dnn_RibbonBar_AddMod_PositionLst_DropDown")); }
        }
        [Obsolete("Element no longer exists in 6.x")]
        public Div VisibilitySelectDiv
        {
            get { return IEInstance.Div(Find.ById("dnn_RibbonBar_AddMod_VisibilityLst_DropDown")); }
        }
        [Obsolete("Element no longer exists in 6.x")]
        public Div ExistingPageSelectDiv
        {
            get { return IEInstance.Div(Find.ById("dnn_RibbonBar_AddMod_PageLst_DropDown")); }
        }
        /// <summary>
        /// The div containing the options in the existing module telerik drop down.
        /// </summary>
        public Div ExistingModuleSelectDiv
        {
            get { return IEInstance.Div(Find.ById(new Regex(@"dnn_(cp_)?RibbonBar_AddMod_ctl\d\d_DropDown"))); }
        }
        /// <summary>
        /// The div containing the options in the skin telerik drop down.
        /// </summary>
        public Div SkinSelectDiv
        {
            get { return IEInstance.Div(Find.ById(s => s.EndsWith("RibbonBar_EditPage_SkinLst_DropDown"))); }
        }
        #endregion

        #region Spans
        public Span HostTabLink
        {
            get { return AdminMenusDiv.Span(Find.ByText("Host")); }
        }
        public Span AdminTabLink
        {
            get { return AdminMenusDiv.Span(Find.ByText("Admin")); }
        }
        #endregion

        #region TabLinks
        [Obsolete("Element no longer exists in 6.x")]
        public Link CurrentPageTabLink
        {
            get { return RibbonBarDiv.Link(Find.ByText(s => s.Contains("Current Page"))); }
        }
        [Obsolete("Element no longer exists in 6.x")]
        public Link CommonTasksTabLink
        {
            get { return RibbonBarDiv.Link(Find.ByText("Common Tasks")); }
        }
        [Obsolete("Element no longer exists in 6.x")]
        public Link OtherToolsTabLink
        {
            get { return RibbonBarDiv.Link(Find.ByText("Other Tools")); }
        }
        #endregion

        #region Links
        public Link AddModuleButton
        {
            get { return CommonTasksDiv.Link(Find.ById(s => s.EndsWith("RibbonBar_AddMod_cmdAddModule"))); }
        }
        [Obsolete("Element no longer exists in 6.x")]
        public Link SiteSettingsLink
        {
            get { return IEInstance.Div(Find.ById("dnn_RibbonBar_PageSite")).Link(Find.ByTitle("Site Settings")); }
        }
        [Obsolete("Element no longer exists in 6.x")]
        public Link UsersLink
        {
            get { return IEInstance.Div(Find.ById("dnn_RibbonBar_PageSite")).Link(Find.ByTitle("Users")); }
        }
        [Obsolete("Element no longer exists in 6.x")]
        public Link AddNewPageLinkAdminTab
        {
            get { return IEInstance.Div(Find.ById("dnn_RibbonBar_PageSite")).Link(Find.ByTitle("Create New Page")); }
        }
        /// <summary>
        /// The link for the modules menu drop down.
        /// </summary>
        public Link ModulesSectionLink
        {
            get { return AdminMenusDiv.Link(Find.ByText(s => s.Contains("Modules"))); }
        }
        /// <summary>
        /// The link for the pages menu drop down.
        /// </summary>
        public Link PagesSectionLink
        {
            get { return AdminMenusDiv.Link(Find.ByText(s => s.Contains("Pages"))); }
        }
        /// <summary>
        /// The link for the tools menu drop down.
        /// </summary>
        public Link ToolsSectionLink
        {
            get { return AdminMenusDiv.Link(Find.ByText(s => s.Contains("Tools"))); }
        }
        [Obsolete("Element no longer exists in 6.x")]
        public Link HostFileManagerLink
        {
            get { return IEInstance.Div(Find.ById("dnn_RibbonBar_PageHostSystem")).Link(Find.ByTitle("File Manager")); }
        }
        [Obsolete("Element no longer exists in 6.x")]
        public Link HostExtensionsLink
        {
            get { return IEInstance.Div(Find.ById("dnn_RibbonBar_PageHostSystem")).Link(Find.ByTitle("Extensions")); }
        }
        public Link NewPageLink
        {
            get { return CurrentPageDiv.Link(Find.ByTitle("Create New Page")); }
        }
        public Link HelpLink
        {
            get { return AdminMenusDiv.Link(Find.ByTitle("Online Help")); }
        }
        public Link DeletePageLink
        {
            get { return CurrentPageDiv.Link(Find.ByTitle("Delete Page")); }
        }
        public Link CopyPageLink
        {
            get { return CurrentPageDiv.Link(Find.ByTitle("Copy Page")); }
        }
        public Link ExportPageLink
        {
            get { return CurrentPageDiv.Link(Find.ByTitle("Export Page")); }
        }
        public Link ImportPageLink
        {
            get { return CurrentPageDiv.Link(Find.ByTitle("Import Page")); }
        }
        /// <summary>
        /// The link to expand the module telerik drop down.
        /// </summary>
        public Link ModuleComboBoxLink
        {
            get { return IEInstance.Div(Find.ById(s => s.EndsWith("RibbonBar_AddMod_UpdateAddModule"))).Link(Find.ById(new Regex(@"dnn_(cp_)?RibbonBar_AddMod_ctl\d\d_Arrow"))); }
        }
        public Link EditCurrentPageLink
        {
            get { return CurrentPageDiv.Link(Find.ByTitle("Page Settings")); }
        }
        /// <summary>
        /// The link to expand the existing module telerik drop down.
        /// </summary>
        public Link AddExistingModuleComboBoxLink
        {
            get { return CommonTasksDiv.Link(Find.ById(new Regex(@"dnn_(cp_)?RibbonBar_AddMod_ctl\d\d_Arrow"))); }
        }
        /// <summary>
        /// The link to expand the skin telerik drop down.
        /// </summary>
        public Link PageSkinComboBox
        {
            get { return ControlPanel.Link(Find.ById(s => s.EndsWith("RibbonBar_EditPage_SkinLst_Arrow"))); }
        }
        public Link CopyPermissionsToChildrenLink
        {
            get { return ControlPanel.Link(Find.ByTitle("Copy Permissions to Children")); }
        }
        public Link CopyDesignToChildrenLink
        {
            get { return CurrentPageDiv.Link(Find.ByTitle("Copy Skin and Container Settings to Children")); }
        }
        public Link SupportLink
        {
            get { return AdminMenusDiv.Link(Find.ByTitle("Support Tickets")); }
        }
        public Link AddPageLink
        {
            get { return AdminMenusDiv.Link(Find.ByTitle("Add Page")); }
        }
        public Link UpdatePageLink
        {
            get { return AdminMenusDiv.Link(Find.ByTitle("Update Page")); }
        }
        public Link ClearCacheLink
        {
            get { return ControlPanel.Link(Find.ById(s => s.EndsWith("RibbonBar_ClearCache_CPCommandBtn"))); }
        }
        #endregion

        #region TextField
        public TextField ModuleTitleField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("_RibbonBar_AddMod_Title"))); }
        }
        public TextField NewPageNameField
        {
            get { return ControlPanel.TextField(Find.ById(s => s.EndsWith("_RibbonBar_AddPage_Name"))); }
        }
        public TextField EditPageNameField
        {
            get { return ControlPanel.TextField(Find.ById(s => s.EndsWith("_RibbonBar_EditPage_Name"))); }
        }
        #endregion

        #region Buttons
        public Button SwitchSiteButton
        {
            get { return ControlPanel.Button(Find.ById(new Regex("dnn(_cp)?_RibbonBar_SwitchSite_cmdSwitch"))); }
        }
        public Button PopUpConfirmation
        {
            get { return IEInstance.Div(Find.ByClass("ui-dialog-buttonset")).Button(Find.ByText("Yes")); }
        }
        public Button PopUpCancelation
        {
            get { return IEInstance.Div(Find.ByClass("ui-dialog-buttonset")).Button(Find.ByText("No"));  }
        }
        #endregion

        #region RadioButtons
        public RadioButton AddExistingModuleRadioButton
        {
            get { return CommonTasksDiv.RadioButton(Find.ByValue("AddExistingModule")); }
        }
        #endregion

        #region CheckBoxes
        public CheckBox CopyExistingModuleCheckBox
        {
            get { return CommonTasksDiv.CheckBox(Find.ById(s => s.EndsWith("RibbonBar_AddMod_chkCopyModule"))); }
        }
        #endregion

        #region SelectLists
        public SelectList VisibilitySelectList
        {
            get { return CommonTasksDiv.SelectList(Find.ById(s => s.EndsWith("RibbonBar_AddMod_VisibilityLst"))); }
        }
        public SelectList ModuleCategorySelectList
        {
            get { return CommonTasksDiv.SelectList(Find.ById(s => s.EndsWith("_RibbonBar_AddMod_CategoryList"))); }
        }
        public SelectList ModulePositionSelectList
        {
            get { return CommonTasksDiv.SelectList(Find.ById(s => s.EndsWith("_RibbonBar_AddMod_PositionLst"))); }
        }
        public SelectList AddExistingModulePageSelectList
        {
            get { return CommonTasksDiv.SelectList(Find.ById(s => s.EndsWith("RibbonBar_AddMod_PageLst"))); }
        }
        public SelectList EditPageLocationSelectList
        {
            get { return CurrentPageDiv.SelectList(Find.ById(s => s.EndsWith("RibbonBar_EditPage_LocationLst"))); }
        }
        public SelectList EditPageLocationPageSelectList
        {
            get { return CurrentPageDiv.SelectList(Find.ById(s => s.EndsWith("RibbonBar_EditPage_PageLst"))); }
        }
        #endregion
        
        #endregion

        #region Public Methods
        /// <summary>
        /// Finds a link within the ribbon bar that contains the link text.
        /// </summary>
        /// <param name="linkText">The text within the link object.</param>
        /// <returns>The link object.</returns>
        public Link GetRibbonBarLinkByText(string linkText)
        {
            return AdminMenusDiv.Link(Find.ByText(s => s.Contains(linkText)));
        }

        /// <summary>
        /// Returns a item from a telerik "select list" element
        /// </summary>
        /// <param name="comboBoxLink">The link object that expands the drop down. Ex. ModuleComboBox</param>
        /// <param name="comboBoxDiv">The div object containing all of the list items for the drop down. Ex. ModuleSelectDiv</param>
        /// <param name="selectListClass">The class to filter all of the items within the comboBoxDiv by. 
        /// The currently selected item will have the class "rcbHovered ", all others will have the class "rcbItem ".</param>
        /// <param name="ItemText">The text of the item in the drop down.</param>
        /// <returns>The list item from the drop down, considered an Element object instead of a list item.</returns>
        public Element GetItemFromTelerikComboBox(Link comboBoxLink, Div comboBoxDiv, string selectListClass, string ItemText)
        {
            //Click Drop down
            comboBoxLink.Click();

            System.Threading.Thread.Sleep(1500);
            //Find Item to Select
            //Find all List Item elements that match the class
            ElementCollection selectListElements = comboBoxDiv.Elements.Filter(Find.ByClass(selectListClass));
            Element result = null;
            //Search for the desired Element
            foreach (Element e in selectListElements)
            {
                if (e.InnerHtml.Contains(ItemText))
                {
                    //Found the Element
                    result = e;
                    break;
                }
                continue;
            }
            return result;
        }
        
        /// <summary>
        /// Adds a module to the current page from the ribbon bar.
        /// </summary>
        /// <param name="moduleName">The title for the module.</param>
        /// <param name="moduleType">The type of module to add.</param>
        /// <param name="moduleCategory">The category the module is in.</param>
        public void AddModuleToPage(string moduleName, string moduleType, string moduleCategory)
        {
            //Select the module category
            ModuleCategorySelectList.Select(moduleCategory);
            System.Threading.Thread.Sleep(1500);
            Element item = null;
            //Try finding the module element in the combo box
            item = GetItemFromTelerikComboBox(ModuleComboBoxLink, ModuleSelectDiv, "rcbItem ", moduleType);
            if(item == null)
            {
                //The module element wasn't found
                //Try finding the module element using the class "rcbHovered " instead
                item = GetItemFromTelerikComboBox(ModuleComboBoxLink, ModuleSelectDiv, "rcbHovered ", moduleType);
            }
            //Select the module type
            item.FireEvent("onmouseover");
            System.Threading.Thread.Sleep(1000);
            item.ClickNoWait();
            if (!moduleName.Equals(""))
            {
                //Enter a title for the module
                ModuleTitleField.Value = moduleName;
            }
            System.Threading.Thread.Sleep(2000);
            //Click the add module button
            AddModuleButton.ClickNoWait();
        }

        /// <summary>
        /// Adds an HTML module to the current page from the ribbon bar.
        /// </summary>
        /// <param name="moduleName">The title for the module.</param>
        /// <param name="moduleType">The type of module to add. Either HTML or HTML Pro</param>
        public void AddHTMLModuleToPage(string moduleName, string moduleType)
        {
            ModuleCategorySelectList.Select("All Categories");
            System.Threading.Thread.Sleep(1500);
            Element item = null;
            //Class will be "rcbHovered ", instead of "rcbList "
            item = GetItemFromTelerikComboBox(ModuleComboBoxLink, ModuleSelectDiv, "rcbHovered ", moduleType);
            item.FireEvent("onmouseover");
            item.ClickNoWait();
            if (!moduleName.Equals(""))
            {
                ModuleTitleField.Value = moduleName;
            }
            System.Threading.Thread.Sleep(1000);
       
            AddModuleButton.ClickNoWait();
        }

        /// <summary>
        /// Adds an HTML module to the current page from the ribbon bar, with the visibility set.
        /// </summary>
        /// <param name="moduleName">The title for the module.</param>
        /// <param name="moduleType">The type of module to add. Either HTML or HTML Pro</param>
        /// <param name="visibility">The visibility for the module. Either "Same As Page" or "Page Editors Only".</param>
        public void AddHTMLModuleToPage(string moduleName, string moduleType, string visibility)
        {
            ModuleCategorySelectList.Select("Common");
            System.Threading.Thread.Sleep(1500);
            Element item = null;
            //Class will be "rcbHovered ", instead of "rcbList "
            item = GetItemFromTelerikComboBox(ModuleComboBoxLink, ModuleSelectDiv, "rcbHovered ", moduleType);
            item.FireEvent("onmouseover");
            item.ClickNoWait();
            if (!moduleName.Equals(""))
            {
                ModuleTitleField.Value = moduleName;
            }
            System.Threading.Thread.Sleep(1000);
            VisibilitySelectList.Select(visibility);
            AddModuleButton.ClickNoWait();
        }

        /// <summary>
        /// Adds an HTML module to the current page from the ribbon bar, with the visibility set and the position of the module set.
        /// </summary>
        /// <param name="moduleName">The title for the module.</param>
        /// <param name="moduleType">The type of module to add. Either HTML or HTML Pro.</param>
        /// <param name="visibility">The visibility for the module. Either "Same As Page" or "Page Editors Only".</param>
        /// <param name="modulePostion">The position the module should be inserted into. Either "Bottom" "Top" "Above" or "Below".</param>
        public void AddHTMLModuleToPage(string moduleName, string moduleType, string visibility, string modulePostion)
        {
            ModuleCategorySelectList.Select("All Categories");
            System.Threading.Thread.Sleep(1500);
            Element item = null;
            //Class will be "rcbHovered ", instead of "rcbList "
            item = GetItemFromTelerikComboBox(ModuleComboBoxLink, ModuleSelectDiv, "rcbHovered ", moduleType);
            item.FireEvent("onmouseover");
            item.ClickNoWait();
            if (!moduleName.Equals(""))
            {
                ModuleTitleField.Value = moduleName;
            }
            System.Threading.Thread.Sleep(1000);
            ModulePositionSelectList.Select(modulePostion);
            AddModuleButton.ClickNoWait();
        }

        /// <summary>
        /// Adds an HTML module to the current page from the ribbon bar, then adds content to the module.
        /// </summary>
        /// <param name="moduleName">The title for the module</param>
        /// <param name="moduleType">The type of module to add. Either HTML or HTML Pro.</param>
        /// <param name="content">The content to add to the module.</param>
        /// <param name="moduleIndex">The index of the module on the page. To find the action menu image for the first module on the page use 0.</param>
        public void AddHTMLModuleToPageWithContent(string moduleName, string moduleType, string content, int moduleIndex)
        {
            AddHTMLModuleToPage(moduleName, moduleType);
            var htmlModule = new HTMLModule(this);
            htmlModule.AddContentToModule(content, moduleIndex);
        }

        /// <summary>
        /// Adds an HTML module to the current page from the ribbon bar, with the visibility set, then adds content to the module.
        /// </summary>
        /// <param name="content">The content to add to the module.</param>
        /// <param name="moduleIndex">The index of the module on the page. To find the action menu image for the first module on the page use 0.</param>
        public void UpdateHTMLModuleContent(string content, int moduleIndex)
        {
            var htmlModule = new HTMLModule(this);
            htmlModule.AddContentToModule(content, moduleIndex);
        }

        public void AddHtmlModuleToPageSelectVisibility(string moduleName, string visibility,string moduleType, string content, int moduleIndex)
        {
            AddHTMLModuleToPage(moduleName, moduleType, visibility);
            var htmlModule = new HTMLModule(this);
            htmlModule.AddContentToModule(content, moduleIndex); ;
        }

        /// <summary>
        /// Adds an HTML module to the current page from the ribbon bar, with the visibility set to Page Editors Only, then adds content to the module.
        /// </summary>
        /// <param name="moduleName">The title for the module.</param>
        /// <param name="moduleType">The type of module to add. Either HTML or HTML Pro.</param>
        /// <param name="content">The content to add to the module.</param>
        /// <param name="moduleIndex">The index of the module on the page. To find the action menu image for the first module on the page use 0.</param>
        [Obsolete("Use more generic AddHtmlModuleToPageSelectVisibility instead.")]
        public void AddHtmlModuleToPagePageEditorsOnlyVisibility(string moduleName, string moduleType, string content, int moduleIndex)
        {
            AddHTMLModuleToPage(moduleName, moduleType, "Page Editors Only");
            var htmlModule = new HTMLModule(this);
            htmlModule.AddContentToModule(content, moduleIndex);
        }

        /// <summary>
        /// Adds an existing module to the current page from the ribbon bar.
        /// </summary>
        /// <param name="pageName">The name of the page that contains the existing module.</param>
        /// <param name="moduleName">The name of the existing module.</param>
        /// <param name="moduleListItemClass">The class to filter all of the items within the existing module drop down by. 
        /// The currently selected item will have the class "rcbHovered ", all others will have the class "rcbItem ".</param>
        public void AddExistingModuleToPage(string pageName, string moduleName, string moduleListItemClass)
        {
            AddExistingModuleRadioButton.ClickNoWait();
            AddExistingModulePageSelectList.Select(pageName);
            System.Threading.Thread.Sleep(2000);

            Element item = GetItemFromTelerikComboBox(AddExistingModuleComboBoxLink, ExistingModuleSelectDiv, moduleListItemClass, moduleName);
            item.FireEvent("onmouseover");
            item.Click();
            System.Threading.Thread.Sleep(1000);
            AddModuleButton.ClickNoWait();
        }

        #endregion
    
    }
}
