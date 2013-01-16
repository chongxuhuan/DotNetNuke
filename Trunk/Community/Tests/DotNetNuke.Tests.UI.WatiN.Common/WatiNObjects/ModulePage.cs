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
using System.Threading;
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// Generic Module object.
    /// </summary>
    public class ModulePage : WatiNBase
    {
        #region Constructors

        public ModulePage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public ModulePage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region TextFields
        public TextField ModuleTitleTextField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ModuleSettings_txtTitle")));
                }
                return PageContentDiv.TextField(Find.ById(s => s.EndsWith("ModuleSettings_txtTitle")));
            }
        }
        /// <summary>
        /// The username field in the permissions tab of the module settings page.
        /// </summary>
        public TextField AddUserPermissionField
        {
            get { return UserPermissionsDiv.TextField(Find.ByName(s => s.Contains("ModuleSettings$dgPermissions$txtUser"))); }
        }
        /// <summary>
        /// The start date field on the module settings page.
        /// </summary>
        public TextField StartDateField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("ModuleSettings_txtStartDate"))); }
        }
        /// <summary>
        /// The end date field on the module settings page.
        /// </summary>
        public TextField EndDateField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("ModuleSettings_txtEndDate"))); }
        }
        #endregion

        #region Spans

        public Span ModuleOnlineHelpSpan
        {
            get { return IEInstance.Span(Find.ByText("Online Help")); }
        }
        #endregion

        #region Links
        /// <summary>
        /// The Save content link on the Edit Module content page.
        /// </summary>
       public Link SaveContentLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Save"));
                }
                return IEInstance.Link(Find.ByTitle("Save"));
            }
        }

        /// <summary>
        /// The Add link in the permissions tab of the module settings page.
        /// </summary>
        public Link AddUserPermissionButton
        {
            get
            {
                return UserPermissionsDiv.Link(Find.ByText(s => s.Contains("Add")));
            }
        }

        public Link PermissionsLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText(s => s.Contains("Permissions")));
                }
                return IEInstance.Link(Find.ByText(s => s.Contains("Permissions")));
            }
        }


        public Link DeleteLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Delete"));
                }
                return PageContentDiv.Link(Find.ByTitle("Delete"));
            }
        }
        public Link EditModuleContentLink
        {
            get { return PageContentDiv.Link(Find.ByTitle("Edit Content")); }
        }
        /// <summary>
        /// The Current Content tab link on the edit content page.
        /// </summary>
        public Link CurrentContentLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText(s => s.Contains("Current Content")));
                }
                return IEInstance.Link(Find.ByText(s => s.Contains("Current Content")));
            }
        }
        /// <summary>
        /// The main content section link on the edit content page.
        /// </summary>
        public Link MainContentSectionLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText(s => s.Contains("Main Content")));
                }
                return IEInstance.Link(Find.ByText(s => s.Contains("Main Content")));
            }
        }
        #endregion
        
        #region Images
        [Obsolete("Element is out of date. Use ModuleActionMenu.DeleteActionMenuLink instead.")]
        public Image DeleteModuleImage
        {
            get { return IEInstance.Image(Find.BySrc(s => s.EndsWith("action_delete.gif"))); }
        }
        [Obsolete("Element is out of date. Use ModuleActionMenu.PrintActionMenuLink instead.")]
        public Image PrintIcon
        {
            get { return ContainerDiv.Image(Find.ByAlt("Print")); }
        }
        [Obsolete("Element is out of date.")]
        public Image SyndicateIcon
        {
            get { return ContainerDiv.Image(Find.ByAlt("Syndicate")); }
        }
        #endregion

        #region Divs
        /// <summary>
        /// The div containing the user permissions grid.
        /// </summary>
        public Div UserPermissionsDiv
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Div(Find.ByClass("dnnGrid dnnPermissionsGrid"));
                }
                return IEInstance.Div(Find.ByClass("dnnGrid dnnPermissionsGrid"));
            }
        }
        /// <summary>
        /// The container div for the first module on the page.
        /// </summary>
        public Div ContainerDiv
        {
            get { return IEInstance.Div(Find.ByClass(s => s.StartsWith("c_container "))); }
        }
        #endregion

        #region CheckBoxes

        public CheckBox InheritPagePermissionsCheckBox
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById(s => s.EndsWith("ModuleSettings_chkInheritPermissions")));
                }
                return IEInstance.CheckBox(Find.ById(s => s.EndsWith("ModuleSettings_chkInheritPermissions")));
            }
        }
        public CheckBox DisplayOnAllPagesCheckbox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("ModuleSettings_chkAllTabs"))); }
        }
        public CheckBox AllowPrintCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("ModuleSettings_chkDisplayPrint"))); }
        }
        public CheckBox AllowSyndicateCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("ModuleSettings_chkDisplaySyndicate"))); }
        }
        public CheckBox HideAdminBorderCheckbox
        {
            get { return ContentPaneDiv.CheckBox(Find.ById(s => s.EndsWith("ModuleSettings_chkAdminBorder"))); }
        }
        #endregion

        #region Radio buttons
        /// <summary>
        /// The basic text box radio button on the edit content page.
        /// </summary>
        public RadioButton BasicTextBoxRadioButton
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ById(s => s.EndsWith("EditHTML_txtContent_OptView_0")));
                }
                return IEInstance.RadioButton(Find.ById(s => s.EndsWith("EditHTML_txtContent_OptView_0")));
            }
        }
        #endregion

        #region SelectLists
        public SelectList ModuleContainerSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("ModuleSettings_ctlModuleContainer_cboSkin"))); }
        }
        public SelectList MoveToPageSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("ModuleSettings_cboTab"))); }
        }
        #endregion

        #region Tables
        public Table RolePermissionsTable
        {
            get { return UserPermissionsDiv.Tables[0]; }
        }
        #endregion
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the action menu image for the module at the specified index.
        /// </summary>
        /// <param name="moduleNum">The index of the module on the page. To find the action menu image for the first module on the page use 0.</param>
        /// <returns>The action menu image.</returns>
        public Image GetActionMenuImageForModule(int moduleNum)
        {
            return IEInstance.Images.Filter(Find.ById(s => s.Contains("dnnACTIONS_ctldnnACTIONSicn")))[moduleNum];
        }

        /// <summary>
        /// Selects the text editor style for the Edit Content page of an HTML module.
        /// </summary>
        /// <param name="selectedValue">The value for the radio button that will be selected.</param>
        public void SelectTextEditorStyleByName(string selectedValue)
        {
            RadioButtonCollection rbCollection;
            if (PopUpFrame != null)
            {
                rbCollection = PopUpFrame.RadioButtons;
            }
            else
            {
                rbCollection = IEInstance.RadioButtons;
            }
            foreach (RadioButton rb in rbCollection)
            {
                if (selectedValue != rb.GetAttributeValue("value")) continue;
                //rb.Checked = true;
                rb.Click();
                Thread.Sleep(1500);
                break;
            }
        }

        /// <summary>
        /// Finds the edit module content link for the module specified.
        /// </summary>
        /// <param name="moduleNum">The index of the module on the page. To find the action menu image for the first module on the page use 0.</param>
        /// <returns>The edit module link for the module.</returns>
        public Link GetEditModuleContentLink(int moduleNum)
        {
            //Returns the edit module content link for the module specified by moduleNum
            //0 being the first module on the page
            DivCollection actionMenu = IEInstance.Divs.Filter(Find.ByClass("dnnActionMenu"));
            if (actionMenu.Count > moduleNum)
            {
                return actionMenu[moduleNum].Link(Find.ByText(s => s.Contains("Edit Content")));
            }
            return IEInstance.Link(Find.ByText(s => s.Contains("Edit Content")));
        }

        /// <summary>
        /// Finds the module settings link for the module.
        /// </summary>
        /// <param name="moduleNum">The index of the module on the page. To find the action menu image for the first module on the page use 0.</param>
        /// <returns>The module settings link.</returns>
        public Link GetModuleSettingsImage(int moduleNum)
        {
            //Returns the edit module settings link for the module specified by moduleNum
            return IEInstance.Divs.Filter(Find.ByClass(s => s.Contains("dnnActionMenu")))[moduleNum].Link(Find.ByText("Settings"));
        }

        /// <summary>
        /// Finds the module settings link for the module.
        /// </summary>
        /// <param name="moduleNum">The index of the module on the page. To find the action menu image for the first module on the page use 0.</param>
        /// <returns>The module Online help link.</returns>
        public Link GetModuleOnlineHelpImage(int moduleNum)
        {
            //Returns the edit module settings link for the module specified by moduleNum
            return IEInstance.Divs.Filter(Find.ByClass(s => s.Contains("dnnActionMenu")))[moduleNum].Link(Find.ByText("Online Help"));
        }

        /// <summary>
        /// Gives a user edit permissions for the module.
        /// The user must have already been given view permissions for the module.
        /// </summary>
        /// <param name="edition">The edition of the site. "Community", "Professional", "Enterprise"</param>
        public void ClickAddedUserEditCheckbox(string edition)
        {
            Image editCheckbox;
            Table table;
            if (edition.Equals("Community"))
            {
                table = UserPermissionsDiv.Tables[1];
                editCheckbox = table.TableRows.Filter(Find.ByClass("dnnGridItem"))[0].TableCells[2].Image(Find.Any);
            }
            else
            {
                editCheckbox = UserPermissionsDiv.Tables[1].TableRows.Filter(Find.ByClass("dnnGridItem"))[0].TableCells[7].Image(Find.Any);
            }
            editCheckbox.ClickNoWait();
        }

        /// <summary>
        /// Clicks the view permissions checkbox for the role.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        public void SetPermissionForRole(string setting, string permission, string roleName)
        {
            Image result = null;
            foreach (TableRow row in RolePermissionsTable.TableRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(roleName))
                {
                    switch (permission)
                    {
                        case "View":
                            result = row.TableCells[1].Image(Find.Any);
                            break;
                        case "Edit Content":
                            result = row.TableCells[2].Image(Find.Any);
                            break;
                        case "Edit":
                            result = row.TableCells[2].Image(Find.Any);
                            break;
                        case "Delete":
                            result = row.TableCells[3].Image(Find.Any);
                            break;
                        case "Export":
                            result = row.TableCells[4].Image(Find.Any);
                            break;
                        case "Import":
                            result = row.TableCells[5].Image(Find.Any);
                            break;
                        case "Manage Settings":
                            result = row.TableCells[6].Image(Find.Any);
                            break;
                        case "Full Control":
                            result = row.TableCells[7].Image(Find.Any);
                            break;
                    }
                    ClickPermission(setting, result);
                    return;
                }
            }
        }

        private void ClickPermission(string setting, Image result)
        {
            if (result.Src.Contains("Grant"))
            {
                switch (setting)
                {
                    case "Grant":
                        return;
                    case "Uncheck":
                        result.ClickNoWait();
                        Thread.Sleep(100);
                        result.ClickNoWait();
                        return;
                    case "Deny":
                        result.ClickNoWait();
                        return;
                }
            }
            if (result.Src.Contains("Deny"))
            {
                switch (setting)
                {
                    case "Grant":
                        result.ClickNoWait();
                        Thread.Sleep(100);
                        result.ClickNoWait();
                        return;
                    case "Uncheck":
                        result.ClickNoWait();
                        return;
                    case "Deny":
                        return;
                }
            }
            if (result.Src.Contains("Uncheck"))
            {
                switch (setting)
                {
                    case "Grant":
                        result.ClickNoWait();
                        return;
                    case "Uncheck":
                        return;
                    case "Deny":
                        result.ClickNoWait();
                        Thread.Sleep(100);
                        result.ClickNoWait();
                        return;
                }
            }

        }

        #endregion
    }
}
