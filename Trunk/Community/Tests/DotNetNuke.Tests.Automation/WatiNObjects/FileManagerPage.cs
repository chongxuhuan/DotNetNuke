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
    /// The file manager page object.
    /// </summary>
    public class FileManagerPage : WatiNBase
    {
        #region Constructors

        public FileManagerPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public FileManagerPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region FileUploads
        public FileUpload SelectFileUpload
        {
            get { return IEInstance.FileUpload(Find.ById(s => s.EndsWith("WebUpload_cmdBrowse"))); }
        }

        #endregion

        #region Spans
        /// <summary>
        /// The span containing the path of the current folder.
        /// Found in the bottom left corner of the module.
        /// </summary>
        public Span CurrentFolderSpan
        {
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("FileManager_lblCurFolder"))); }
        }
        /// <summary>
        /// The outer span of the Folders section of the module.
        /// </summary>
        public Span FoldersSpan
        {
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("FileManager_pnlFolders"))); }
        }        
        /// <summary>
        /// The span for the Portal Root folder in the Folders section.
        /// </summary>
        public Span PortalRootSpan
        {
            get { return FoldersSpan.Span(Find.ByTitle("Site Root")); }
        }
        [Obsolete("Element no longer exists. The upload successful message can be viewed by examining the ModuleMessageSpan element.")]
        public Span UploadMessage
        {
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("WebUpload_lblMessage"))); }
        }
        public Span SyncFolderSpan
        {
            get { return ContentPaneDiv.Span(Find.ById(s => s.EndsWith("syncFolder"))); }
        }

        #endregion

        #region Divs
        public Div PermissionsGridDiv
        {
            get
            {
                return IEInstance.Div(Find.ById("dnnPermissionsGrid"));
            }
        }
        #endregion

        #region SelectLists
        /// <summary>
        /// The folder type select list.
        /// </summary>
        public SelectList StorageLocationSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("FileManager_ddlStorageLocation"))); }
        }
        public SelectList UploadDestinationSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("WebUpload_ddlFolders"))); }
        }

        public SelectList FolderProviderSelectList
        {
            get
            {
                return FindElement<SelectList>(Find.ById(s => s.EndsWith("EditFolderMapping_cboFolderProviders")));
            }
        }
        #endregion

        #region TextFields
        public TextField FolderNameTextField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("FileManager_txtNewFolder"))); }
        }

        public TextField FolderProviderNameField
        {
            get
            {
                return FindElement<TextField>(Find.ById(s => s.EndsWith("EditFolderMapping_txtName")));
            }
        }

        public TextField UNCPathField
        {
            get
            {
                return FindElement<TextField>(Find.ById(s => s.EndsWith("EditFolderMapping_Settings.ascx_tbUNCPath")));
            }
        }
        #endregion

        #region Images and Image Collections
        public Image AddFolderButton
        {
            get { return IEInstance.Image(Find.ByTitle("Add Folder")); }
        }
        /// <summary>
        /// The first folder expand image in the folders section of the module.
        /// </summary>
        public Image FolderExpandImage
        {
            get { return FoldersSpan.Image(Find.BySrc(s => s.Contains("/Max_12X12_Standard.png"))); }
        }
        public Image MoveFileImage
        {
            get { return IEInstance.Image(Find.ByTitle("Move Files")); }
        }
        public Image RefreshImage
        {
            get { return IEInstance.Image(Find.ByTitle("Refresh")); }
        }
        /// <summary>
        /// All of the Secure Database icons found in the folders section of the module.
        /// </summary>
        public ImageCollection SecureDatabaseIcons
        {
            get { return FoldersSpan.Images.Filter(Find.BySrc(s => s.Contains("Sql_16X16_Standard"))); }
        }
        /// <summary>
        /// All of the Secure Folder icons found in the foldesr section of the module.
        /// </summary>
        public ImageCollection SecureFolderIcons
        {
            get { return FoldersSpan.Images.Filter(Find.BySrc(s => s.Contains("SecurityRoles"))); }
        }
        public Image UploadImage
        {
            get { return IEInstance.Image(Find.ByTitle("Upload")); }
        }
        #endregion

        #region Links
        public Link ReturnLink
        {
            get { return ContentPaneDiv.Link(Find.ByTitle("Return")); }
        }

        public Link UploadSelectedFileLink
        {
            get { return ContentPaneDiv.Link(Find.ByText("Upload File")); }
        }

        public new Link UpdateLink
        {
            get { return ContentPaneDiv.Link(Find.ById(s => s.EndsWith("FileManager_cmdUpdate"))); }
        }

        public Link ManageFolderTypesLink
        {
            get
            {
                return ContentPaneDiv.Link(Find.ByText(s => s.Contains("Manage Folder Types")));
            }
        }

        public Link AddNewTypeLink
        {
            get
            {
                return FindElement<Link>(Find.ByTitle("Add New Type"));
            }
        }

        #endregion

        #region Tables
        /// <summary>
        /// The outer table containing the File Manager module.
        /// </summary>
        public Table FileManagerTable
        {
            get { return IEInstance.Table(Find.ByClass("FileManager")); }
        }
        public Table FolderContentsTable
        {
            get { return ContentPaneDiv.Table(Find.ByClass("FileManager_FileList")); }
        }
        public Table RolePermissionsTable
        {
            get { return ContentPaneDiv.Table(Find.ByClass("dnnPermissionsGrid")); }
        }

        #endregion

        #region CheckBoxes
        /// <summary>
        /// The select file checkbox of the first item in the current folder.
        /// </summary>
        public CheckBox SelectFileCheckbox
        {
            get { return FolderContentsTable.TableRow(Find.ByClass(new Regex(@"FileManager_\w*Item"))).CheckBox(Find.ById(s => s.Contains("chkFile"))); }
        }

        public CheckBox RecursiveSyncCheckbox
        {
            get { return ContentPaneDiv.CheckBox(Find.ById(s => s.EndsWith("chkRecursive"))); }
        }

        #endregion

        #region Buttons
        /// <summary>
        /// The OK button that appears in the folder contents section after a destination folder has been selected. 
        /// </summary>
        public Button MoveFileOKButton
        {
            get { return FileManagerTable.Button(Find.ById("btnMoveOK")); }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the file path for the Select Package Dialog.
        /// </summary>
        /// <param name="filePath">The path for the file that will be added.</param>
        public void SelectFile(string filePath)
        {
            SelectFileUpload.Set(filePath);
        }

        /// <summary>
        /// Uploads the file to the file manager to the default folder (the folder that was selected before clicking Upload).
        /// To upload the file to a specific folder make sure that the folder is selected before using this method.
        /// </summary>
        /// <param name="FilePath">The path for the file that will be added.</param>
        public void UploadFile(string FilePath)
        {
            UploadImage.ClickNoWait();
            SelectFile(FilePath);
            UploadSelectedFileLink.ClickNoWait();
        }

        /// <summary>
        /// Clicks the view permissions checkbox for the role.
        /// </summary>
        /// <param name="permission"> </param>
        /// <param name="roleName">The role name.</param>
        /// <param name="setting"> </param>
        public void SetPermissionForRole(string setting, string permission, string roleName)
        {
            Image result = null;
            foreach (TableRow row in RolePermissionsTable.TableRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(roleName))
                {
                    switch (permission)
                    {
                        case "Browse Folder":
                            result = row.TableCells[1].Image(Find.Any);
                            break;
                        case "View":
                            result = row.TableCells[2].Image(Find.Any);
                            break;
                        case "Add":
                            result = row.TableCells[3].Image(Find.Any);
                            break;
                        case "Copy":
                            result = row.TableCells[4].Image(Find.Any);
                            break;
                        case "Delete":
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
