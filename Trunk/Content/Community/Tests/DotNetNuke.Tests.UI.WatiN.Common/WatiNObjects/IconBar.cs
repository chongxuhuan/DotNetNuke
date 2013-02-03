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
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The Icon bar styled Control Panel object.
    /// </summary>
    public class IconBar : WatiNBase
    {
        #region Constructors

        public IconBar(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public IconBar(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }
       
        #endregion

        #region Public Properties

        #region Images
        public new Image MaximiseControlPanelImage { get { return ControlPanelTable.Image(Find.ByTitle("Maximize")); } }
        public new Image MinimiseControlPanelImage { get { return ControlPanelTable.Image(Find.ByTitle("Minimize")); } }
        #endregion

        #region Links
        public Link AddModuleLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdAddModule"))); }
        }
        public Link AddPageLink
        {
            get { return ControlPanel.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdAddTab"))); }
        }
        public Link AddPageLinkAlt
        {
            get { return ControlPanelAlt.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdAddTab"))); }
        }
        public Link AdminLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdAdmin"))); }
        }
        public Link CopyPageLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdCopyTab"))); }
        }
        public Link DeletePageLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdDeleteTab"))); }
        }
        public Link ExportPageLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdExportTab"))); }
        }
        public Link ExtensionsLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdExtensions"))); }
        }
        public Link FilesLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdFiles"))); }
        }
        public Link HelpLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdHelp"))); }
        }
        public Link ImportPageLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdImportTab"))); }
        }
        public Link PageSettingsLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdEditTab"))); }
        }
        public Link RolesLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdRoles"))); }
        }
        public Link SiteSettingsLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdSite"))); }
        }

        public Link UsersLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("IconBar.ascx_cmdUsers"))); }
        }
        #endregion

        #region SelectLists
        public SelectList ExistingModulePageSelectList
        {
            get { return IEInstance.SelectList(Find.ById("dnn_IconBar.ascx_cboTabs")); }
        }
        public SelectList ExistingModuleSelectList
        {
            get { return IEInstance.SelectList(Find.ById("dnn_IconBar.ascx_cboModules")); }
        }
        public SelectList InsertRelativeSelectList
        {
            get { return IEInstance.SelectList(Find.ById("dnn_IconBar.ascx_cbpInstances")); }
        }
        public SelectList ModuleInsertSelect
        {
            get { return IEInstance.SelectList(Find.ById("dnn_IconBar.ascx_cboPosition")); }
        }
        public SelectList ModulePaneSelect
        {
            get { return IEInstance.SelectList(Find.ById("dnn_IconBar.ascx_cboPanes")); }
        }
        public SelectList ModuleSelect
        {
            get { return IEInstance.SelectList(Find.ById("dnn_IconBar.ascx_cboDesktopModules")); }
        }
        public SelectList ModuleVisibilitySelect
        {
            get { return IEInstance.SelectList(Find.ById("dnn_IconBar.ascx_cboPermission")); }
        }
        #endregion

        #region TextFields 
        public TextField ModuleTitleField
        {
            get { return IEInstance.TextField(Find.ById("dnn_IconBar.ascx_txtTitle")); }
        }
        #endregion

        #region RadioButtons
        public RadioButton AddExistingModuleRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById("dnn_IconBar.ascx_optModuleType_1")); }
        }
        public new RadioButton EditRadioButton { get { return ControlPanelTable.RadioButton(Find.ByValue("EDIT")); } }
        public new RadioButton ViewRadioButton { get { return ControlPanelTable.RadioButton(Find.ByValue("VIEW")); } }
        public new RadioButton LayoutRadioButton { get { return ControlPanelTable.RadioButton(Find.ByValue("LAYOUT")); } }
        #endregion

        #region Table
        /// <summary>
        /// The table containing the control panel
        /// </summary>
        public Table ControlPanelTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("IconBar.ascx_tblControlPanel"))); }
        }
        #endregion


        #endregion

        #region Public Methods
        /// <summary>
        /// Adds an existing module to a page.
        /// </summary>
        /// <param name="pageName">The name of the page containing the existing module.</param>
        /// <param name="moduleName">The name of the existing module.</param>
        public void AddExistingModuleToPage(string pageName, string moduleName)
        {
            AddExistingModuleRadioButton.Checked = true;
            System.Threading.Thread.Sleep(3000);
            ExistingModulePageSelectList.Select(pageName);
            System.Threading.Thread.Sleep(1000);
            ExistingModuleSelectList.Select(moduleName);
            AddModuleLink.Click();

        }

        /// <summary>
        /// Adds a module with the specified visibility to a page and adds content to the module.
        /// </summary>
        /// <param name="moduleName">The name that will be given to the module. Entering "" as a name will not fill in the module title field.</param>
        /// <param name="moduleType">The type of module that will be added. Ex. HTML, Links, Dashboard etc. </param>
        /// <param name="visibilityChoice">The visibility for the module. Must be either "Same As Page" or "Page Editors Only". </param>
        /// <param name="content">The content that will be added to the module.</param>
        /// <param name="moduleIndex">The index for the module on the page. Ex. if the module will be the first one on the page the index should be 0.</param>
        public void AddModuleToPageSelectVisibilityAddContent(string moduleName, string moduleType, string visibilityChoice, string content, int moduleIndex)
        {
            ModuleSelect.Select(moduleType);
            if (!moduleName.Equals(""))
            {
                ModuleTitleField.Value = moduleName;
            };
            ModuleVisibilitySelect.Select(visibilityChoice);
            AddModuleLink.Click();
            System.Threading.Thread.Sleep(2000);
            var htmlModule = new HTMLModule(this);
            htmlModule.AddContentToModule(content, moduleIndex);
        }

        /// <summary>
        /// Adds a module to a page and adds content to the module.
        /// </summary>
        /// <param name="moduleName">The name that will be given to the module. Entering "" as a name will not fill in the module title field.</param>
        /// <param name="moduleType">The type of module that will be added. Ex. HTML, Links, Dashboard etc.</param>
        /// <param name="content">The content that will be added to the module.</param>
        /// <param name="moduleIndex">The index for the module on the page. Ex. if the module will be the first one on the page the index should be 0.</param>
        public void AddHTMLModuleToPageWithContent(string moduleName, string moduleType, string content, int moduleIndex)
        {
            ModuleSelect.Select(moduleType);
            if (!moduleName.Equals(""))
            {
                ModuleTitleField.Value = moduleName;
            }
            AddModuleLink.Click();

            System.Threading.Thread.Sleep(2000);
            var htmlModule = new HTMLModule(this);
            htmlModule.AddContentToModule(content, moduleIndex);
        }
       
        /// <summary>
        /// Adds a module to a page.
        /// </summary>
        /// <param name="moduleName">The name that will be given to the module. Entering "" as a name will not fill in the module title field.</param>
        /// <param name="moduleType">The type of module that will be added. Ex. HTML, Links, Dashboard etc.</param>
        public void AddModuleToPage(string moduleName, string moduleType)
        {
            ModuleSelect.Select(moduleType);
            if (!moduleName.Equals(""))
            {
                ModuleTitleField.Value = moduleName;
            }
            AddModuleLink.ClickNoWait();
        }

        /// <summary>
        /// Adds a module to a page and inserts it above or below another module.
        /// </summary>
        /// <param name="moduleName">The name that will be given to the module. Entering "" as a name will not fill in the module title field.</param>
        /// <param name="moduleType">The type of module that will be added. Ex. HTML, Links, Dashboard etc.</param>
        /// <param name="insert">How the module shoudl be inserted. "Above" or "Below".</param>
        /// <param name="otherModule">The module that the new module will be inserted relative to.</param>
        public void AddModuleToPageInsert(string moduleName, string moduleType, string insert, string otherModule)
        {
            ModuleSelect.Select(moduleName);
            if (!moduleName.Equals(""))
            {
                ModuleTitleField.Value = moduleName;
            }
            ModuleInsertSelect.Select(insert);
            if (insert.ToLower().Equals("above") || insert.ToLower().Equals("below"))
            {
                InsertRelativeSelectList.Select(otherModule);
            }
            AddModuleLink.Click();

        }

        /// <summary>
        /// Select the display mode for pages.
        /// </summary>
        /// <param name="pageDisplayMode">The display mode. Either "View", "Edit" or "Layout".</param>
        public void SelectPageDisplayMode(string pageDisplayMode)
        {
            if (!string.IsNullOrEmpty(pageDisplayMode))
                WatiNUtil.SelectRadioButtonByName(IEInstance, "dnn$IconBar.ascx$optMode", pageDisplayMode);
        }
        
        #endregion
    }
}
