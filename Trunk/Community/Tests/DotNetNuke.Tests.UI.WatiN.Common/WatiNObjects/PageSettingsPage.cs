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
using System.Text.RegularExpressions;
using System.Threading;

using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The page settings object.
    /// </summary>
    public class PageSettingsPage : WatiNBase
    {
        #region Constructors

        public PageSettingsPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public PageSettingsPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region TextFields
        public TextField PageNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ctr_ManageTabs_txtTabName")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ctr_ManageTabs_txtTabName")));
            }
        }
        public TextField PageTitleField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ctr_ManageTabs_txtTitle")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ctr_ManageTabs_txtTitle")));
            }
        }
        /// <summary>
        /// The username field in the permissions tab.
        /// </summary>
        public TextField UserPermissionField
        {
            get { return UserPermissionsDiv.TextField(Find.ById("dnn_ctr_ManageTabs_dgPermissions_txtUser")); }
        }
        /// <summary>
        /// The URL field in the Advanced Settings tab.
        /// Only exists if the URL link type is selected.
        /// </summary>
        public TextField UrlTextField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ctr_ManageTabs_ctlURL_txtUrl")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ctr_ManageTabs_ctlURL_txtUrl")));
            }
        }
        /// <summary>
        /// The start date field in the Advanced Settings tab.
        /// </summary>
        public TextField StartDateTextField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ctr_ManageTabs_datepickerStartDate_dateInput_text")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ctr_ManageTabs_datepickerStartDate_dateInput_text")));
            }
        }
        /// <summary>
        /// The end date field in the Advanced Settings tab.
        /// </summary>
        public TextField EndDateTextField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ctr_ManageTabs_datepickerEndDate_dateInput_text")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ctr_ManageTabs_datepickerEndDate_dateInput_text")));
            }
        }
        #endregion

        #region SelectLists
        public SelectList ParentPageSelectList
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("ctr_ManageTabs_cboParentTab")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("ctr_ManageTabs_cboParentTab")));
            }
        }
        public SelectList PageTemplateSelect
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("ctr_ManageTabs_cboTemplate")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("ctr_ManageTabs_cboTemplate")));
            }
        }
        public SelectList PageSkinSelect
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("ManageTabs_pageSkinCombo")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("ManageTabs_pageSkinCombo")));
            }
        }
        public SelectList PageContainerSelect
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("ManageTabs_pageContainerCombo")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("ManageTabs_pageContainerCombo")));
            }
        }
        public SelectList CopyPageSelectList
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("ctr_ManageTabs_cboCopyPage")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("ctr_ManageTabs_cboCopyPage")));
            }
        }
        /// <summary>
        /// The page selectlist in the Advanced Settings tab.
        /// Only exists if the Page link type is selected.
        /// </summary>
        public SelectList LinkedPageSelectList
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("ManageTabs_ctlURL_cboTabs")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("ManageTabs_ctlURL_cboTabs")));
            }
        }

        public SelectList InsertPageSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("dnn_ctr_ManageTabs_cboPositionTab"))); }
        }
        /// <summary>
        /// The File name selectlist in the Advanced Settings tab.
        /// Only exists if the File link type is selected.
        /// </summary>
        public SelectList FileNameSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("ctr_ManageTabs_ctlURL_cboFiles"))); }
        }
        public SelectList TemplateFolderSelectList
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("ctr_ManageTabs_cboFolders")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("ctr_ManageTabs_cboFolders")));
            }
        }
        #endregion

        #region CheckBoxes
        public CheckBox IncludeInMenuCheckbox
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById(s => s.EndsWith("ctr_ManageTabs_chkMenu")));
                }
                return IEInstance.CheckBox(Find.ById(s => s.EndsWith("ctr_ManageTabs_chkMenu")));
            }
        }
        #endregion

        #region Links
        public Link AddPageLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText(s => s.Contains("Add Page")));
                }
                return ContentPaneDiv.Link(Find.ByText(s => s.Contains("Add Page")));
            }
        }
        public Link UpdatePageLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText(s => s.Contains("Update Page")));
                }
                return ContentPaneDiv.Link(Find.ByText(s => s.Contains("Update Page")));
            }
        }
        public Link DeletePageLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Delete"));
                }
                return IEInstance.Link(Find.ByTitle("Delete"));
            }
        }
        /// <summary>
        /// The Add link in the permissions tab.
        /// </summary>
        public Link AddUserPermissionLink
        {
            get { return UserPermissionsDiv.Link(Find.ByText(s => s.Contains("Add"))); }
        }
        public Link PermissionsTabLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText("Permissions"));
                }
                return ContentPaneDiv.Link(Find.ByText("Permissions"));
            }
        }
        public Link AdvancedSettingsTabLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText("Advanced Settings"));
                }
                return ContentPaneDiv.Link(Find.ByText("Advanced Settings"));
            }
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
                    return PopUpFrame.Div(Find.ById("dnn_ctr_ManageTabs_permissionRow"));
                }
                return IEInstance.Div(Find.ById("dnn_ctr_ManageTabs_permissionRow"));
            }
        }
        #endregion

        #region Tables
        /// <summary>
        /// The table containing permissions information for Security Roles.
        /// </summary>
        public Table RolePermissionsTable
        {
            get { return UserPermissionsDiv.Tables[0]; }
        }
        /// <summary>
        /// The table containing permissions information for Users.
        /// </summary>
        public Table UserPermissionsTable
        {
            get { return UserPermissionsDiv.Tables[1]; }
        }
        #endregion

        #region RadioButtons
        public RadioButton LinkURLTypeRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ById(s => s.EndsWith("ctr_ManageTabs_ctlURL_optType_1")));
                }
                return IEInstance.RadioButton(Find.ById(s => s.EndsWith("ctr_ManageTabs_ctlURL_optType_1")));
            }
        }
        public RadioButton NewCopyRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ByValue("optNew"));
                }
                return IEInstance.RadioButton(Find.ByValue("optNew"));
            }
        }
        public RadioButton CopyRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ByValue("optCopy"));
                }
                return IEInstance.RadioButton(Find.ByValue("optCopy"));
            }
        }
        public RadioButton ReferencedCopyRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ByValue("optReference"));
                }
                return IEInstance.RadioButton(Find.ByValue("optReference"));
            }
        }
        public RadioButton LinkToPageRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ById(s => s.EndsWith("_ManageTabs_ctlURL_optType_2")));
                }
                return IEInstance.RadioButton(Find.ById(s => s.EndsWith("_ManageTabs_ctlURL_optType_2")));
            }
        }
        public RadioButton InsertAfterRadioButton
        {
            get { return IEInstance.RadioButton(Find.ByValue("After")); }
        }
        public RadioButton LinkToFileRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("ctr_ManageTabs_ctlURL_optType_3"))); }
        }
        #endregion

        #endregion

        #region Public Methods
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
                        case "Add":
                            result = row.TableCells[2].Image(Find.Any);
                            break;
                        case "Edit":
                            result = row.TableCells[2].Image(Find.Any);
                            break;
                        case "Add Content":
                            result = row.TableCells[3].Image(Find.Any);
                            break;
                        case "Copy":
                            result = row.TableCells[4].Image(Find.Any);
                            break;
                        case "Delete":
                            result = row.TableCells[5].Image(Find.Any);
                            break;
                        case "Export":
                            result = row.TableCells[6].Image(Find.Any);
                            break;
                        case "Import":
                            result = row.TableCells[7].Image(Find.Any);
                            break;
                        case "Manage Settings":
                            result = row.TableCells[8].Image(Find.Any);
                            break;
                        case "Navigate":
                            result = row.TableCells[9].Image(Find.Any);
                            break;
                        case "Full Control":
                            result = row.TableCells[10].Image(Find.Any);
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

        /// <summary>
        /// Clicks the view permissions checkbox for the role.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        public void ClickViewPermissionForRole(string roleName)
        {
            Image result = null;
            foreach (TableRow row in RolePermissionsTable.TableRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(roleName))
                {
                    result = row.TableCells[1].Image(Find.Any);
                    break;
                }
                continue;
            }
            result.ClickNoWait();
        }

        /// <summary>
        /// Clicks the edit permission checkbox for the role.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        public void ClickEditPermissionForRole(string roleName)
        {
            Image result = null;
            foreach (TableRow row in RolePermissionsTable.TableRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(roleName))
                {
                    result = row.TableCells[2].Image(Find.Any);
                    break;
                }
            }
            result.ClickNoWait();
        }

        /// <summary>
        /// Gives the user edit permissions for the page.
        /// </summary>
        /// <param name="userName">The users username.</param>
        /// <param name="displayName">The users display name.</param>
        public void GiveUserEditPermissionsForPage(string userName, string displayName)
        {
            UserPermissionField.Value = userName;
            AddUserPermissionLink.Click();
            System.Threading.Thread.Sleep(2000);
            Image result = null;
            foreach (TableRow row in UserPermissionsTable.TableRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(displayName))
                {
                    if (row.TableCells.Count == 11)
                    {
                        result = row.TableCells[10].Image(Find.Any);
                        break;
                    }
                    else
                    {
                        result = row.TableCells[2].Image(Find.Any);
                        break;
                    }
                }
                continue;
            }
            result.Click();
        }

        /// <summary>
        /// Determines if a role has view permissions for the page.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>Returns true if the role has view permissions, and false otherwise. </returns>
        public bool GetViewPermissionsForRole(string roleName)
        {
            TextField result = null;
            foreach (TableRow row in RolePermissionsTable.TableRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(roleName))
                {
                    result = row.TableCells[1].TextField(Find.Any);
                    break;
                }
                continue;
            }
            bool canView = result.Value.CompareTo("True") == 0;
            return canView;
        }

        /// <summary>
        /// Determines if a role has edit permissions for the page.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>Returns true if the role has edit permissions, and false otherwise. </returns>
        public bool GetEditPermissionsForRole(string roleName)
        {
            TextField result = null;
            foreach (TableRow row in RolePermissionsTable.TableRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(roleName))
                {
                    result = row.TableCells[2].TextField(Find.Any);
                    break;
                }
                continue;
            }
            bool canView = result.Value.CompareTo("True") == 0;
            return canView;
        }

        /// <summary>
        /// Creates a page with the default settings.
        /// Fills in the page name field and clicks the add page link.
        /// Will not work unless the test is already on the Add page form.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        public void AddPage(string pageName)
        {
            PageNameField.Value = pageName;
            AddPageLink.ClickNoWait();
            System.Threading.Thread.Sleep(3000);
        }

        /// <summary>
        /// Creates a page that is not included in the menu, with the default for all other settings.
        /// Will not work unless the test is already on the Add page form.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        public void AddHiddenPage(string pageName)
        {
            PageNameField.Value = pageName;
            IncludeInMenuCheckbox.Checked = false;
            AddPageLink.ClickNoWait();
            System.Threading.Thread.Sleep(3000);
        }

        /// <summary>
        /// Creates a page that is inserted after another page. All other settings will have the default value.
        /// Will not work unless the test is already on the Add page form.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="afterPage">The name of the page that the new page will be inserted afer.</param>
        public void AddPageAfter(string pageName, string afterPage)
        {
            PageNameField.Value = pageName;
            InsertAfterRadioButton.Checked = true;
            InsertPageSelectList.Select(afterPage);
            AddPageLink.ClickNoWait();
            System.Threading.Thread.Sleep(3000);
        }

        /// <summary>
        /// Creates a child page. All other settings will have the default value.
        /// Will not work unless the test is already on the Add Page form.
        /// This method can be used to make grandchild pages by entering the parent exactly as it would appear in the page drop down. 
        /// Ex. "...Test Page"
        /// </summary>
        /// <param name="childName">The name of the child page that will be created.</param>
        /// <param name="parentName">The name of the child's parent. The parent page must already exist. </param>
        public void AddChildPage(string childName, string parentName)
        {
            PageNameField.Value = childName;
            ParentPageSelectList.Select(parentName);
            AddPageLink.ClickNoWait();
            System.Threading.Thread.Sleep(3000);
        }

        /// <summary>
        /// Creates a page using a specific template. All other settings will have the default value.
        /// Will not work unless the test is already on the Add Page form.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="template">The name of the template.</param>
        public void AddPageSelectTemplate(string pageName, string template)
        {
            PageNameField.Value = pageName;
            if (TemplateFolderSelectList.Option("Templates/").Exists)
            {
                TemplateFolderSelectList.Select("Templates/");
            }
            else
            {
                TemplateFolderSelectList.Select("Root");
            }
            PageTemplateSelect.Select(template);
            AddPageLink.ClickNoWait();
            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// Creates a page using a specific template and gives All Users view permissions for the page. All other settings will have the default value.
        /// Will not work unless the test is already on the Add Page form.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="template">The name of the template.</param>
        public void AddPageSelectTemplateAllUsersView(string pageName, string template)
        {

            PageNameField.Value = pageName;
            if (TemplateFolderSelectList.Option("Templates/").Exists)
            {
                TemplateFolderSelectList.Select("Templates/");
            }
            else
            {
                TemplateFolderSelectList.Select("Root");
            }
            PageTemplateSelect.Select(template);
            SetPermissionForRole("Grant", "View", "All Users");
            AddPageLink.ClickNoWait();
            System.Threading.Thread.Sleep(1500);
        }

        /// <summary>
        /// Creates a page and gives a user view permissions for the page.
        /// Will not work unless the test is already on the Add Page form.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="userName">The username of the user.</param>
        public void AddPageGiveUserViewPermission(string pageName, string userName)
        {
            PageNameField.Value = pageName;
            UserPermissionField.Value = userName;
            AddUserPermissionLink.ClickNoWait();
            System.Threading.Thread.Sleep(1000);
            AddPageLink.ClickNoWait();
            System.Threading.Thread.Sleep(1500);
        }

        /// <summary>
        /// Creates a page using a specific template and gies a user view permissions for the page.
        /// Will not work unless the test is already on the Add Page form.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="userName">The username of the user.</param>
        /// <param name="templateName">The name of the template.</param>
        public void AddPageGiveUserViewPermissionSelectTemplate(string pageName, string userName, string templateName)
        {
            PageNameField.Value = pageName;
            PageTemplateSelect.Select(templateName);
            System.Threading.Thread.Sleep(1000);
            PermissionsTabLink.Click();
            UserPermissionField.Value = userName;
            AddUserPermissionLink.Click();
            System.Threading.Thread.Sleep(1000);
            AddPageLink.Click();
            System.Threading.Thread.Sleep(3000);
        }

        #endregion
    }
}
