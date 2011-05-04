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
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;

using ICSharpCode.SharpZipLib.Zip;

using Microsoft.VisualBasic;

using DataCache = DotNetNuke.Common.Utilities.DataCache;
using DNNTreeNode = DotNetNuke.UI.WebControls.TreeNode;
using DNNTreeNodeCollection = DotNetNuke.UI.WebControls.TreeNodeCollection;
using FileInfo = DotNetNuke.Services.FileSystem.FileInfo;
using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.FileManager
{
    public partial class FileManager : PortalModuleBase, IActionable
    {
        private string _ErrorMessage =
            "<TABLE><TR><TD height=100% class=NormalRed>{0}</TD></TR><TR valign=bottom><TD align=center><INPUT id=btnClearError onclick=clearErrorMessage(); type=button value=OK></TD></TR></TABLE>";

        private string imageDirectory = "~/images/FileManager/Icons/";
        private SortedList<int, string> m_FolderMappings;

        protected bool IsAdminRole
        {
            get
            {
                return PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName);
            }
        }

        public int FolderPortalID
        {
            get
            {
                if (IsHostMenu)
                {
                    return Null.NullInteger;
                }
                else
                {
                    return PortalId;
                }
            }
        }

        public string RootFolderName
        {
            get
            {
                if (ViewState["RootFolderName"] != null)
                {
                    return ViewState["RootFolderName"].ToString();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ViewState["RootFolderName"] = value;
            }
        }

        public string RootFolderPath
        {
            get
            {
                string _CurRootFolder;
                if (IsHostMenu)
                {
                    _CurRootFolder = Globals.HostMapPath;
                }
                else
                {
                    _CurRootFolder = PortalSettings.HomeDirectoryMapPath;
                }
                return _CurRootFolder;
            }
        }

        public string Sort
        {
            get
            {
                return ViewState["strSort"].ToString();
            }
            set
            {
                ViewState.Add("strSort", value);
            }
        }

        public string LastSort
        {
            get
            {
                return ViewState["strLastSort"].ToString();
            }
            set
            {
                ViewState.Add("strLastSort", value);
            }
        }

        public string FilterFiles
        {
            get
            {
                return ViewState["strFilterFiles"].ToString();
            }
            set
            {
                ViewState.Add("strFilterFiles", value);
            }
        }

        public string LastPath
        {
            get
            {
                return UnMaskPath(ClientAPI.GetClientVariable(Page, "LastPath"));
            }
            set
            {
                value = MaskPath(value);
                ClientAPI.RegisterClientVariable(Page, "LastPath", value, true);
            }
        }

        public string DestPath
        {
            get
            {
                return ClientAPI.GetClientVariable(Page, "DestPath");
            }
            set
            {
                ClientAPI.RegisterClientVariable(Page, "DestPath", value, true);
            }
        }

        public string SourcePath
        {
            get
            {
                return ClientAPI.GetClientVariable(Page, "SourcePath");
            }
            set
            {
                ClientAPI.RegisterClientVariable(Page, "SourcePath", value, true);
            }
        }

        public string MoveFiles
        {
            get
            {
                return ClientAPI.GetClientVariable(Page, "MoveFiles");
            }
            set
            {
                ClientAPI.RegisterClientVariable(Page, "MoveFiles", value, true);
            }
        }

        public bool IsRefresh
        {
            get
            {
                return Convert.ToBoolean(ClientAPI.GetClientVariable(Page, "IsRefresh"));
            }
            set
            {
                ClientAPI.RegisterClientVariable(Page, "IsRefresh", Convert.ToString(Convert.ToInt32(value)), true);
            }
        }

        public bool DisabledButtons
        {
            get
            {
                return Convert.ToBoolean(ClientAPI.GetClientVariable(Page, "DisabledButtons"));
            }
            set
            {
                ClientAPI.RegisterClientVariable(Page, "DisabledButtons", Convert.ToString(Convert.ToInt32(value)), true);
            }
        }

        public string MoveStatus
        {
            get
            {
                return ClientAPI.GetClientVariable(Page, "MoveStatus");
            }
            set
            {
                ClientAPI.RegisterClientVariable(Page, "MoveStatus", value, true);
            }
        }

        public string LastFolderPath
        {
            get
            {
                if (ViewState["LastFolderPath"] != null)
                {
                    return ViewState["LastFolderPath"].ToString();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ViewState["LastFolderPath"] = value;
            }
        }

        public int PageSize
        {
            get
            {
                return Convert.ToInt32(selPageSize.SelectedValue);
            }
        }

        public int PageIndex
        {
            get
            {
                if (ViewState["PageIndex"] != null)
                {
                    return Convert.ToInt32(ViewState["PageIndex"]);
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (value >= 0 && value < dgFileList.PageCount)
                {
                    ViewState["PageIndex"] = value;
                }
            }
        }

        private SortedList<int, string> FolderMappings
        {
            get
            {
                if (m_FolderMappings == null)
                {
                    m_FolderMappings = new SortedList<int, string>();

                    var folderMappingController = FolderMappingController.Instance;
                    var folderMappings = folderMappingController.GetFolderMappings(FolderPortalID);
                    folderMappings.FindAll(f => f.IsEnabled).ForEach(f => m_FolderMappings.Add(f.FolderMappingID, f.MappingName));
                }

                return m_FolderMappings;
            }
        }

        protected bool HasPermission(string permissionKey)
        {
            var hasPermision = UserInfo.IsSuperUser;
            var strSourcePath = UnMaskPath(DestPath).Replace(RootFolderPath, "").Replace("\\", "/");
            var objFolder = FolderManager.Instance.GetFolder(FolderPortalID, strSourcePath);
            if (!hasPermision && objFolder != null)
            {
                hasPermision = IsEditable && FolderPermissionController.HasFolderPermission(objFolder.FolderPermissions, permissionKey);
            }
            return hasPermision;
        }

        private void AddFileToTable(DataTable tblFiles, FileInfo objFile)
        {
            DataRow dRow = tblFiles.NewRow();

            dRow["FileType"] = "File";
            dRow["FileId"] = objFile.FileId;
            dRow["FileName"] = objFile.FileName;
            dRow["FileSize"] = objFile.Size.ToString("##,##0");
            dRow["IntFileSize"] = objFile.Size;
            
            if (!String.IsNullOrEmpty(objFile.Extension) || objFile.Extension != null)
            {
                dRow["Extension"] = objFile.Extension;
            }
            else
            {
                dRow["Extension"] = "none";
            }

            if (objFile.SupportsFileAttributes)
            {
                dRow["SupportsFileAttributes"] = true;

                if (objFile.FileAttributes.HasValue)
                {
                    dRow["DateModified"] = objFile.LastModificationTime;
                    dRow["Archive"] = objFile.FileAttributes.Value & FileAttributes.Archive;
                    dRow["ReadOnly"] = objFile.FileAttributes.Value & FileAttributes.ReadOnly;
                    dRow["Hidden"] = objFile.FileAttributes.Value & FileAttributes.Hidden;
                    dRow["System"] = objFile.FileAttributes.Value & FileAttributes.System;
                    dRow["AttributeString"] = GetAttributeString(objFile.FileAttributes.Value);
                }
            }
            else
            {
                dRow["SupportsFileAttributes"] = false;
                dRow["Archive"] = false;
                dRow["ReadOnly"] = false;
                dRow["Hidden"] = false;
                dRow["System"] = false;
                dRow["AttributeString"] = "";
            }

            tblFiles.Rows.Add(dRow);
        }

        private DNNTreeNode AddNode(string strName, string strKey, int imageIndex, DNNTreeNodeCollection objNodes)
        {
            DNNTreeNode objNode;
            objNode = new DNNTreeNode(strName);
            objNode.Key = strKey;
            objNode.ToolTip = strName;
            objNode.ImageIndex = imageIndex;
            objNode.CssClass = "FileManagerTreeNode";
            objNodes.Add(objNode);
            if (objNode.Key == DestPath)
            {
                objNode.Selected = true;
                objNode.MakeNodeVisible();
            }
            return objNode;
        }

        private DNNTreeNode AddNode(FolderInfo folder, DNNTreeNodeCollection objNodes)
        {
            DNNTreeNode objNode;
            string strName = folder.FolderName;
            string strKey = MaskPath(RootFolderPath + folder.FolderPath);
            var subFolders = FolderManager.Instance.GetFolders(folder);

            var imageIndex = FolderMappings.IndexOfKey(folder.StorageLocation);

            objNode = AddNode(strName, strKey, imageIndex, objNodes);
            objNode.HasNodes = subFolders.Count > 0;
            return objNode;
        }

        private void BindFileList()
        {
            string strCurPage;
            LastPath = PathUtils.Instance.RemoveTrailingSlash(UnMaskPath(DestPath));
            dgFileList.PageSize = PageSize;
            dgFileList.CurrentPageIndex = PageIndex;
            GetFilesByFolder(PathUtils.Instance.StripFolderPath(DestPath).Replace("\\", "/"));
            if (dgFileList.PageCount > 1)
            {
                tblMessagePager.Visible = true;
                strCurPage = Localization.GetString("Pages");
                lblCurPage.Text = string.Format(strCurPage, (dgFileList.CurrentPageIndex + 1), (dgFileList.PageCount));
                lnkMoveFirst.Text = "<img border=0 Alt='" + Localization.GetString("First") + "' src='" + ResolveUrl("~/images/FileManager/movefirst.gif") + "'>";
                lnkMovePrevious.Text = "<img border=0 Alt='" + Localization.GetString("Previous") + "' src='" + ResolveUrl("~/images/FileManager/moveprevious.gif") + "'>";
                lnkMoveNext.Text = "<img border=0 Alt='" + Localization.GetString("Next") + "' src='" + ResolveUrl("~/images/FileManager/movenext.gif") + "'>";
                lnkMoveLast.Text = "<img border=0 Alt='" + Localization.GetString("Last") + "' src='" + ResolveUrl("~/images/FileManager/movelast.gif") + "'>";
            }
            else
            {
                tblMessagePager.Visible = false;
            }
            lblCurFolder.Text = Regex.Replace(DestPath, "^0\\\\", RootFolderName + "\\");
            MoveFiles = "";
            UpdateSpaceUsed();
        }

        private void BindStorageLocationTypes()
        {
            ddlStorageLocation.Items.Clear();

            foreach (var folderMapping in FolderMappings)
            {
                ddlStorageLocation.Items.Add(new ListItem(folderMapping.Value, folderMapping.Key.ToString()));
            }
        }

        private void BindFolderTree()
        {
            DNNTreeNode objNode;
            DNNTree.TreeNodes.Clear();
            objNode = AddNode(RootFolderName, MaskPath(RootFolderPath), FolderMappings.IndexOfValue("Standard"), DNNTree.TreeNodes);
            var folders = FolderManager.Instance.GetFolders(FolderPortalID);
            objNode.HasNodes = folders.Count > 1;
            if (DNNTree.PopulateNodesFromClient == false || DNNTree.IsDownLevel)
            {
                PopulateTree(objNode.TreeNodes, RootFolderPath);
            }
            if (DNNTree.SelectedTreeNodes.Count == 0)
            {
                objNode.Selected = true;
            }
        }

        private string GetCheckAllString()
        {
            int intCount = dgFileList.Items.Count;
            CheckBox chkFile;
            int i;
            string strResult;
            strResult = "setMoveFiles('');" + Environment.NewLine;
            for (i = 0; i <= intCount - 1; i++)
            {
                chkFile = (CheckBox) dgFileList.Items[i].FindControl("chkFile");
                if ((chkFile) != null)
                {
                    strResult = strResult + "var chk1 = dnn.dom.getById('" + chkFile.ClientID + "');";
                    strResult = strResult + "chk1.checked = blValue;" + Environment.NewLine;
                    strResult = strResult + "if (!chk1.onclick) {chk1.parentElement.onclick();}else{chk1.onclick();}" + Environment.NewLine;
                }
            }
            strResult = "function CheckAllFiles(blValue) {" + strResult + "}" + Environment.NewLine;
            strResult = "<script language=javascript>" + strResult + "</script>";
            return strResult;
        }

        private void GeneratePermissionsGrid()
        {
            var folderPath = PathUtils.Instance.StripFolderPath(DestPath).Replace("\\", "/");
            dgPermissions.FolderPath = folderPath;
            var objFolderInfo = FolderManager.Instance.GetFolder(FolderPortalID, folderPath);
            if (objFolderInfo != null)
            {
                ddlStorageLocation.SelectedValue = Convert.ToString(objFolderInfo.StorageLocation);
            }
        }

        private string GetAttributeString(FileAttributes attributes)
        {
            string strResult = "";
            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
            {
                strResult += "A";
            }
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                strResult += "R";
            }
            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                strResult += "H";
            }
            if ((attributes & FileAttributes.System) == FileAttributes.System)
            {
                strResult += "S";
            }
            return strResult;
        }

        private void GetFilesByFolder(string strFolderName)
        {
            DataTable tblFiles = GetFileTable();
            var objFolder = FolderManager.Instance.GetFolder(FolderPortalID, strFolderName);
            if (objFolder != null)
            {
                var arrFiles = FolderManager.Instance.GetFiles(objFolder);
                foreach (FileInfo objFile in arrFiles)
                {
                    AddFileToTable(tblFiles, objFile);
                }
            }
            var dv = new DataView();
            dv.Table = tblFiles;
            dv.Sort = Sort;
            if (!String.IsNullOrEmpty(FilterFiles))
            {
                dv.RowFilter = "FileName like '%" + FilterFiles + "%'";
            }
            dgFileList.DataSource = dv;
            dgFileList.DataBind();
        }

        private DataTable GetFileTable()
        {
            var tblFiles = new DataTable("Files");
            var myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.String");
            myColumns.ColumnName = "FileType";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.Int32");
            myColumns.ColumnName = "FileId";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.String");
            myColumns.ColumnName = "FileName";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.String");
            myColumns.ColumnName = "FileSize";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.Int32");
            myColumns.ColumnName = "IntFileSize";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.DateTime");
            myColumns.ColumnName = "DateModified";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.Boolean");
            myColumns.ColumnName = "ReadOnly";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.Boolean");
            myColumns.ColumnName = "Hidden";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.Boolean");
            myColumns.ColumnName = "System";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.Boolean");
            myColumns.ColumnName = "Archive";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.String");
            myColumns.ColumnName = "AttributeString";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.String");
            myColumns.ColumnName = "Extension";
            tblFiles.Columns.Add(myColumns);
            myColumns = new DataColumn();
            myColumns.DataType = Type.GetType("System.Boolean");
            myColumns.ColumnName = "SupportsFileAttributes";
            tblFiles.Columns.Add(myColumns);
            return tblFiles;
        }

        private long GetZipFileExtractSize(string strFileName)
        {
            ZipEntry objZipEntry;
            ZipInputStream objZipInputStream;
            try
            {
                objZipInputStream = new ZipInputStream(File.OpenRead(strFileName));
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
                ShowErrorMessage(MaskString(ex.Message));
                return -1;
            }
            objZipEntry = objZipInputStream.GetNextEntry();
            long iTemp = 0;
            while (objZipEntry != null)
            {
                iTemp = iTemp + objZipEntry.Size;
                objZipEntry = objZipInputStream.GetNextEntry();
            }
            objZipInputStream.Close();
            return iTemp;
        }

        private void InitializeTree()
        {
            DNNTree.SystemImagesPath = ResolveUrl("~/images/");

            PreloadFolderImages();

            DNNTree.ImageList.Add(ResolveUrl("~/images/file.gif"));
            DNNTree.IndentWidth = 10;
            DNNTree.CollapsedNodeImage = ResolveUrl("~/images/max.gif");
            DNNTree.ExpandedNodeImage = ResolveUrl("~/images/min.gif");
            DNNTree.PopulateNodesFromClient = true;
            DNNTree.JSFunction = "nodeSelected();";
        }

        private void PreloadFolderImages()
        {
            IFolderMappingController folderMappingController = FolderMappingController.Instance;
            FolderMappingInfo folderMappingInfo = null;
            string imageUrl = String.Empty;

            foreach (var folderMapping in FolderMappings)
            {
                folderMappingInfo = folderMappingController.GetFolderMapping(folderMapping.Key);
                imageUrl = folderMappingInfo.ImageUrl;
                DNNTree.ImageList.Add(imageUrl);
            }
        }

        private void ManageToolbarButton(HtmlGenericControl wrapperControl, Image imageControl, string js, string imageRootName, bool enableButton)
        {
            if (enableButton)
            {
                wrapperControl.Attributes.Add("style", "cursor: pointer");
                wrapperControl.Attributes.Add("onclick", js);
                imageControl.ImageUrl = "~/images/FileManager/ToolBar" + imageRootName + "Enabled.gif";
            }
            else
            {
                wrapperControl.Attributes.Remove("style");
                wrapperControl.Attributes.Remove("onclick");
                imageControl.ImageUrl = "~/images/FileManager/ToolBar" + imageRootName + "Disabled.gif";
            }
        }

        private void ManageSecurity()
        {
            ManageToolbarButton(addFolder, lnkAddFolderIMG, "return canAddFolder();", "AddFolder", HasPermission("ADD"));
            ManageToolbarButton(deleteFolder, lnkDelFolderIMG, "return deleteFolder();", "DelFolder", HasPermission("DELETE"));
            ManageToolbarButton(syncFolder, lnkSyncFolderIMG, "__doPostBack(m_sUCPrefixName + 'lnkSyncFolder', '');", "Synchronize", HasPermission("MANAGE"));
            chkRecursive.Enabled = HasPermission("MANAGE");
            ManageToolbarButton(refresh, lnkRefreshIMG, "__doPostBack(m_sUCPrefixName + 'lnkRefresh', '');", "Refresh", true);
            ManageToolbarButton(copy, lnkCopy, "copyCheckedFiles();", "Copy", HasPermission("COPY"));
            ManageToolbarButton(move, lnkMove, "moveFiles();", "Move", HasPermission("COPY"));
            ManageToolbarButton(upload, lnkUploadIMG, "__doPostBack(m_sUCPrefixName + 'lnkUpload', '');", "Upload", HasPermission("ADD"));
            ManageToolbarButton(delete, lnkDelete, "deleteCheckedFiles();", "Delete", HasPermission("DELETE"));
            ManageToolbarButton(filter, lnkFilterIMG, "__doPostBack(m_sUCPrefixName + 'lnkFilter', '');", "Filter", true);
            lnkCopy.Enabled = IsEditable;
            lnkMove.Enabled = IsEditable;
            lnkUpload.Enabled = IsEditable;
            lnkDelete.Enabled = IsEditable;
        }

        private string MaskPath(string strOrigPath)
        {
            return strOrigPath.Replace(PathUtils.Instance.RemoveTrailingSlash(RootFolderPath), "0").Replace("/", "\\");
        }

        private string MaskString(string strSource)
        {
            return FileManagerFunctions.CReplace(strSource, PathUtils.Instance.RemoveTrailingSlash(RootFolderPath), Localization.GetString("PortalRoot", LocalResourceFile), 1);
        }

        private void PopulateTree(DNNTreeNodeCollection objNodes, string strPath)
        {
            var folderPath = strPath.Replace(RootFolderPath, "").Replace("\\", "/");
            var parentFolder = FolderManager.Instance.GetFolder(FolderPortalID, folderPath);
            var folders = FolderManager.Instance.GetFolders(parentFolder);
            DNNTreeNode objNode;
            foreach (var folder in folders)
            {
                if (FolderPermissionController.CanViewFolder((FolderInfo)folder))
                {
                    objNode = AddNode((FolderInfo)folder, objNodes);
                    if (DNNTree.PopulateNodesFromClient == false)
                    {
                        PopulateTree(objNode.TreeNodes, folder.FolderPath);
                    }
                }
            }
        }

        private void SetFolder(DNNTreeNode node)
        {
            dgFileList.EditItemIndex = -1;
            if (DNNTree.IsDownLevel)
            {
                DestPath = node.Key;
                LastPath = node.Key;
            }
            ManageSecurity();
            BindFileList();
            GeneratePermissionsGrid();
        }

        private void SetEditMode()
        {
            if (dgFileList.EditItemIndex > -1)
            {
                int intCount = dgFileList.Items.Count;
                CheckBox chkFile2;
                CheckBox chkFile;
                ImageButton lnkDeleteFile;
                ImageButton lnkEditFile;
                int i;
                for (i = 0; i <= intCount - 1; i++)
                {
                    if (i != dgFileList.EditItemIndex)
                    {
                        chkFile2 = (CheckBox) dgFileList.Items[i].FindControl("chkFile2");
                        chkFile = (CheckBox) dgFileList.Items[i].FindControl("chkFile");
                        lnkDeleteFile = (ImageButton) dgFileList.Items[i].FindControl("lnkDeleteFile");
                        lnkEditFile = (ImageButton) dgFileList.Items[i].FindControl("lnkEditFile");
                        if ((chkFile2) != null)
                        {
                            chkFile2.Enabled = false;
                        }
                        if ((chkFile) != null)
                        {
                            chkFile.Enabled = false;
                        }
                        if ((lnkDeleteFile) != null)
                        {
                            lnkDeleteFile.Enabled = false;
                            lnkDeleteFile.ImageUrl = "~/images/FileManager/DNNExplorer_trash_disabled.gif";
                            lnkDeleteFile.AlternateText = "";
                        }
                        if ((lnkEditFile) != null)
                        {
                            lnkEditFile.Enabled = false;
                            lnkEditFile.ImageUrl = "~/images/FileManager/DNNExplorer_Edit_disabled.gif";
                            lnkEditFile.AlternateText = "";
                        }
                        chkFile2 = null;
                        chkFile = null;
                        lnkDeleteFile = null;
                        lnkEditFile = null;
                    }
                }
                DisabledButtons = true;
            }
            else
            {
            }
            dgFileList.Columns[0].HeaderStyle.Width = Unit.Percentage(5);
            dgFileList.Columns[1].HeaderStyle.Width = Unit.Percentage(25);
            dgFileList.Columns[2].HeaderStyle.Width = Unit.Percentage(25);
            dgFileList.Columns[3].HeaderStyle.Width = Unit.Percentage(7);
            dgFileList.Columns[4].HeaderStyle.Width = Unit.Percentage(15);
        }

        private void ShowErrorMessage(string strMessage)
        {
            strMessage = strMessage.Replace("\\", "\\\\");
            strMessage = strMessage.Replace("'", "\\'");
            strMessage = strMessage.Replace(Environment.NewLine, "\\n");
            strMessage = string.Format(_ErrorMessage, strMessage);
            ClientAPI.RegisterClientVariable(Page, "ErrorMessage", strMessage, true);
        }

        private void Synchronize()
        {
            if (IsHostMenu)
            {
                FolderManager.Instance.Synchronize(Null.NullInteger);
            }
            else
            {
                FolderManager.Instance.Synchronize(PortalId);
            }
        }

        private string UnMaskPath(string strOrigPath)
        {
            strOrigPath = PathUtils.Instance.AddTrailingSlash(RootFolderPath) + PathUtils.Instance.StripFolderPath(strOrigPath);
            return strOrigPath.Replace("/", "\\");
        }

        private void UpdateSpaceUsed()
        {
            string strDestFolder = PathUtils.Instance.AddTrailingSlash(UnMaskPath(DestPath));
            var objPortalController = new PortalController();
            string strUsed;
            string strQuota;
            if (PortalSettings.HostSpace == 0)
            {
                strQuota = Localization.GetString("UnlimitedSpace", LocalResourceFile);
            }
            else
            {
                strQuota = PortalSettings.HostSpace + "MB";
            }
            if (IsHostMenu)
            {
                lblFileSpace.Text = "&nbsp;";
            }
            else
            {
                long spaceUsed = objPortalController.GetPortalSpaceUsedBytes(FolderPortalID);
                if (spaceUsed < 1024)
                {
                    strUsed = spaceUsed.ToString("0.00") + "B";
                }
                else if (spaceUsed < (1024*1024))
                {
                    strUsed = (spaceUsed/1024).ToString("0.00") + "KB";
                }
                else
                {
                    strUsed = (spaceUsed/(1024*1024)).ToString("0.00") + "MB";
                }
                lblFileSpace.Text = string.Format(Localization.GetString("SpaceUsed", LocalResourceFile), strUsed, strQuota);
            }
        }

        protected void DeleteFiles(string strFiles)
        {
            var arFiles = strFiles.Split(';');

            if (arFiles.Length == 0)
            {
                return;
            }
                        
            var strErrorMessage = "";
            
            var strSourcePath = Globals.GetSubFolderPath(PathUtils.Instance.AddTrailingSlash(LastPath) + arFiles[0], FolderPortalID);
            var folder = FolderManager.Instance.GetFolder(FolderPortalID, strSourcePath);

            for (var i = 0; i <= arFiles.Length - 1; i++)
            {
                if (!String.IsNullOrEmpty(arFiles[i]))
                {
                    var file = Services.FileSystem.FileManager.Instance.GetFile(folder, arFiles[i]);

                    try
                    {
                        Services.FileSystem.FileManager.Instance.DeleteFile(file);
                    }
                    catch (Exception ex)
                    {
                        DnnLog.Error(ex);
                        strErrorMessage += Localization.GetString("ErrorDeletingFile", LocalResourceFile) +
                            PathUtils.Instance.AddTrailingSlash(UnMaskPath(DestPath)) +
                            arFiles[i] +
                            "<br/>&nbsp;&nbsp;&nbsp;" + ex.Message + "<br/>";
                    }
                }
            }
            
            if (!String.IsNullOrEmpty(strErrorMessage))
            {
                strErrorMessage = MaskString(strErrorMessage);
                ShowErrorMessage(strErrorMessage);
            }
            
            BindFileList();
        }

        protected override void Render(HtmlTextWriter output)
        {
            Page.ClientScript.RegisterForEventValidation(lnkAddFolder.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkDeleteFolder.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkDeleteAllCheckedFiles.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkRefresh.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkSelectFolder.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkSyncFolder.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkFilter.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkCopy.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkUpload.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkMove.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkMoveFirst.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkMoveLast.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkMoveNext.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkMovePrevious.UniqueID);
            Page.ClientScript.RegisterForEventValidation(lnkMoveFiles.UniqueID);
            
            string strTemp = GetCheckAllString();
            pnlScripts2.Controls.Add(new LiteralControl(strTemp));
            if (dgFileList.Items.Count <= 10 && dgFileList.PageCount == 1)
            {
                dgFileList.PagerStyle.Visible = false;
            }
            base.Render(output);
        }

        public string CheckDestFolderAccess(long intSize)
        {
            if (Request.IsAuthenticated)
            {
                string strDestFolder = PathUtils.Instance.AddTrailingSlash(UnMaskPath(DestPath));
                var objPortalController = new PortalController();
                if (objPortalController.HasSpaceAvailable(FolderPortalID, intSize) || (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId))
                {
                    return "";
                }
                else
                {
                    return Localization.GetString("NotEnoughSpace", LocalResourceFile);
                }
            }
            else
            {
                return Localization.GetString("PleaseLogin", LocalResourceFile);
            }
        }

        public string GetImageUrl(string type)
        {
            string url = "";
            try
            {
                if (type == "folder")
                {
                    url = imageDirectory + "ClosedFolder.gif";
                }
                else
                {
                    if (!String.IsNullOrEmpty(type) && File.Exists(Server.MapPath(imageDirectory + type + ".gif")))
                    {
                        url = imageDirectory + type + ".gif";
                    }
                    else
                    {
                        url = imageDirectory + "File.gif";
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return url;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cmdUpdate.Click += cmdUpdate_Click;
            dgFileList.ItemDataBound += dgFileList_ItemDataBound;
            dgFileList.SortCommand += dgFileList_SortCommand;
            DNNTree.NodeClick += DNNTree_NodeClick;
            DNNTree.PopulateOnDemand += DNNTree_PopulateOnDemand;
            lnkAddFolder.Command += lnkAddFolder_Command;
            lnkDeleteFolder.Command += lnkDeleteFolder_Command;
            lnkFilter.Command += lnkFilter_Command;
            lnkDeleteAllCheckedFiles.Command += lnkDeleteAllCheckedFiles_Command;
            lnkMoveFiles.Command += lnkMoveFiles_Command;
            lnkMoveFirst.Command += lnkMoveFirst_Command;
            lnkMoveLast.Command += lnkMoveLast_Command;
            lnkMoveNext.Command += lnkMoveNext_Command;
            lnkMovePrevious.Command += lnkMovePrevious_Command;
            lnkRefresh.Command += lnkRefresh_Command;
            lnkSelectFolder.Command += lnkSelectFolder_Command;
            lnkSyncFolder.Command += lnkSyncFolder_Command;
            lnkSyncFolders.Click += lnkSyncFolders_Click;
            lnkUpload.Command += lnkUpload_Command;
            selPageSize.SelectedIndexChanged += selPageSize_SelectedIndexChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                ClientAPI.RegisterClientReference(Page, ClientAPI.ClientNamespaceReferences.dnn);
                DNNClientAPI.AddBodyOnloadEventHandler(Page, "initFileManager();");
                ClientAPI.RegisterClientVariable(Page, "UCPrefixID", DNNTree.ClientID.Replace(DNNTree.ID, ""), true);
                ClientAPI.RegisterClientVariable(Page, "UCPrefixName", DNNTree.UniqueID.Replace(DNNTree.ID, ""), true);

                DisabledButtons = DNNTree.IsDownLevel;

                if (IsHostMenu)
                {
                    RootFolderName = Localization.GetString("HostRoot", LocalResourceFile);
                    pnlSecurity.Visible = false;
                }
                else
                {
                    RootFolderName = Localization.GetString("PortalRoot", LocalResourceFile);
                    pnlSecurity.Visible = HasPermission("WRITE");
                }

                if (!Page.IsPostBack)
                {
                    DataCache.ClearFolderCache(FolderPortalID);
                    Localization.LocalizeDataGrid(ref dgFileList, LocalResourceFile);
                    InitializeTree();
                    BindFolderTree();
                    IsRefresh = true;
                    PageIndex = 0;
                    Sort = "FileType ASC, FileName ASC";
                    LastSort = "FileType ASC, FileName ASC";
                    MoveStatus = "";
                    FilterFiles = "";
                    DestPath = "0\\";
                    BindFileList();
                    BindStorageLocationTypes();
                    ManageSecurity();
                }
                else
                {
                    FilterFiles = txtFilter.Text;
                }

                if (LastFolderPath != DestPath)
                {
                    PageIndex = 0;
                    GeneratePermissionsGrid();
                }

                LastFolderPath = DestPath;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdUpdate_Click(Object sender, EventArgs e)
        {
            var strFolderPath = PathUtils.Instance.StripFolderPath(LastFolderPath).Replace("\\", "/");
            var objFolderInfo = FolderManager.Instance.GetFolder(FolderPortalID, strFolderPath);
            if (objFolderInfo == null)
            {
                Synchronize();
                objFolderInfo = FolderManager.Instance.GetFolder(FolderPortalID, strFolderPath);
            }
            objFolderInfo.FolderPermissions.Clear();
            objFolderInfo.FolderPermissions.AddRange(dgPermissions.Permissions);
            try
            {
                FolderPermissionController.SaveFolderPermissions((FolderInfo)objFolderInfo);
                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("PermissionsUpdated", LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("PermissionsError", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
            }
            GeneratePermissionsGrid();
        }

        private void dgFileList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            ImageButton lnkEditFile;
            CheckBox chkFile;
            ImageButton lnkDeleteFile;
            Image lnkUnzip;
            ImageButton lnkOkRename;
            
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.EditItem || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem)
            {
                chkFile = (CheckBox) e.Item.FindControl("chkFile");
                if (chkFile != null)
                {
                    string sDefCssClass = dgFileList.ItemStyle.CssClass;
                    if (e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        sDefCssClass = dgFileList.AlternatingItemStyle.CssClass;
                    }
                    chkFile.Attributes.Add("onclick",
                                           "addFileToMoveList('" + ClientAPI.GetSafeJSString(chkFile.Attributes["filename"]) + "', this, '" + dgFileList.SelectedItemStyle.CssClass + "', '" +
                                           sDefCssClass + "');");
                }
                lnkEditFile = (ImageButton) e.Item.FindControl("lnkEditFile");
                if (lnkEditFile != null)
                {
                    lnkEditFile.CommandName = e.Item.ItemIndex.ToString();
                }
                lnkUnzip = (Image) e.Item.FindControl("lnkUnzip");
                if (lnkUnzip != null)
                {
                    if (lnkUnzip.Attributes["extension"] != "zip")
                    {
                        lnkUnzip.Visible = false;
                    }
                    else
                    {
                        if (e.Item.ItemType == ListItemType.EditItem)
                        {
                            lnkUnzip.Visible = false;
                        }
                        else
                        {
                            lnkUnzip.Attributes.Add("onclick", "return unzipFile('" + ClientAPI.GetSafeJSString(lnkUnzip.Attributes["filename"]) + "');");
                        }
                    }
                }
                lnkDeleteFile = (ImageButton) e.Item.FindControl("lnkDeleteFile");
                if (lnkDeleteFile != null)
                {
                    if (dgFileList.EditItemIndex == -1)
                    {
                        ClientAPI.AddButtonConfirm(lnkDeleteFile, string.Format(Localization.GetString("EnsureDeleteFile", LocalResourceFile), lnkDeleteFile.CommandName));
                    }
                }
                lnkOkRename = (ImageButton) e.Item.FindControl("lnkOkRename");
                if (lnkOkRename != null)
                {
                    lnkOkRename.CommandName = e.Item.ItemIndex.ToString();
                }
            }
        }

        protected void dgFileList_SortCommand(object source, DataGridSortCommandEventArgs e)
        {
            BindFolderTree();
            IsRefresh = true;
            LastSort = Sort;
            if (Sort.Replace(" ASC", "").Replace(" DESC", "") == e.SortExpression)
            {
                if (Sort.Contains("ASC"))
                {
                    Sort = Sort.Replace("ASC", "DESC");
                }
                else
                {
                    Sort = Sort.Replace("DESC", "ASC");
                }
            }
            else
            {
                Sort = e.SortExpression + " ASC";
            }
            MoveStatus = "";
            FilterFiles = "";
            BindFileList();
        }

        private void DNNTree_NodeClick(object source, DNNTreeNodeClickEventArgs e)
        {
            SetFolder(e.Node);
        }

        private void DNNTree_PopulateOnDemand(object source, DNNTreeEventArgs e)
        {
            DestPath = e.Node.Key;
            PopulateTree(e.Node.TreeNodes, UnMaskPath(e.Node.Key.Replace("\\\\", "\\")));
            GeneratePermissionsGrid();
        }

        private void lnkAddFolder_Command(object sender, CommandEventArgs e)
        {
            if (String.IsNullOrEmpty(txtNewFolder.Text))
            {
                return;
            }
            var strSourcePath = UnMaskPath(DestPath);
            try
            {
                if (DNNTree.TreeNodes[0].DNNNodes.Count == 0)
                {
                    PopulateTree(DNNTree.TreeNodes[0].TreeNodes, RootFolderPath);
                }

                var colNodes = DNNTree.SelectedTreeNodes;
                if (colNodes.Count > 0)
                {
                    var parentNode = (DNNTreeNode) colNodes[1];
                    var filterFolderName = txtNewFolder.Text.Replace(".", "_");

                    var folderPath = PathUtils.Instance.FormatFolderPath(
                        PathUtils.Instance.FormatFolderPath(
                        PathUtils.Instance.StripFolderPath(DestPath).Replace("\\", "/")) + filterFolderName);
                    FolderManager.Instance.AddFolder(FolderMappingController.Instance.GetFolderMapping(int.Parse(ddlStorageLocation.SelectedValue)), folderPath);

                    DestPath = MaskPath(PathUtils.Instance.AddTrailingSlash(strSourcePath) + filterFolderName + "\\");
                    LastFolderPath = DestPath;
                    parentNode.Selected = false;

                    var imageIndex = FolderMappings.IndexOfKey(int.Parse(ddlStorageLocation.SelectedValue));

                    var objNode = AddNode(filterFolderName, parentNode.Key.Replace("\\\\", "\\") + filterFolderName + "\\", imageIndex, parentNode.TreeNodes);
                    objNode.HasNodes = false;
                    objNode.MakeNodeVisible();
                    objNode.Selected = true;
                    SetFolder(objNode);
                }
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
                var strErrorMessage = MaskString(ex.Message);
                ShowErrorMessage(strErrorMessage);
            }
            txtNewFolder.Text = "";
        }

        private void lnkDeleteFolder_Command(object sender, CommandEventArgs e)
        {
            string strSourcePath;
            var ctrlError = new LiteralControl();
            if (DestPath == DNNTree.TreeNodes[0].Key)
            {
                ShowErrorMessage(Localization.GetString("NotAllowedToDeleteRootFolder", LocalResourceFile));
                BindFileList();
                return;
            }
            else
            {
                strSourcePath = UnMaskPath(DestPath);
            }
            var dinfo = new DirectoryInfo(strSourcePath);
            if (dinfo.Exists == false)
            {
                ShowErrorMessage(Localization.GetString("FolderAlreadyRemoved", LocalResourceFile));
                BindFileList();
                return;
            }
            if ((Directory.GetDirectories(strSourcePath).Length > 0) || (dgFileList.Items.Count > 0))
            {
                ShowErrorMessage(Localization.GetString("PleaseRemoveFilesBeforeDeleting", LocalResourceFile));
                BindFileList();
                return;
            }
            try
            {
                var folder = FolderManager.Instance.GetFolder(FolderPortalID, PathUtils.Instance.StripFolderPath(DestPath).Replace("\\", "/"));
                FolderManager.Instance.DeleteFolder(folder);

                if (DestPath.EndsWith("\\"))
                {
                    DestPath = DestPath.Substring(0, DestPath.Length - 1);
                }
                var intEnd = DestPath.LastIndexOf("\\");
                DestPath = DestPath.Substring(0, intEnd);
                var colNodes = DNNTree.SelectedTreeNodes;
                if (colNodes.Count > 0)
                {
                    var objNode = (DNNTreeNode) colNodes[1];
                    objNode.Selected = false;
                    objNode.Parent.Selected = true;
                    objNode.Parent.DNNNodes.Remove(objNode);
                }
                BindFileList();
                GeneratePermissionsGrid();
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);

                ShowErrorMessage(Localization.GetString("ErrorDeletingFolder", LocalResourceFile) + ex.Message);
            }
        }

        protected void lnkDLFile_Command(object sender, CommandEventArgs e)
        {
            var fileManager = Services.FileSystem.FileManager.Instance;
            var file = fileManager.GetFile(Convert.ToInt32(e.CommandArgument));
            fileManager.WriteFileToResponse(file, ContentDisposition.Inline);

            BindFolderTree();
        }

        protected void lnkEditFile_Command(object sender, CommandEventArgs e)
        {
            dgFileList.EditItemIndex = Convert.ToInt32(e.CommandName);
            BindFileList();
            SetEditMode();
        }

        protected void lnkCancelRename_Command(object sender, CommandEventArgs e)
        {
            dgFileList.EditItemIndex = -1;
            BindFileList();
            SetEditMode();
        }

        private void lnkDeleteAllCheckedFiles_Command(object sender, CommandEventArgs e)
        {
            if (!String.IsNullOrEmpty(MoveFiles))
            {
                DeleteFiles(MoveFiles);
            }
        }

        protected void lnkDeleteFile_Command(object sender, CommandEventArgs e)
        {
            DeleteFiles(e.CommandName);
        }

        private void lnkFilter_Command(object sender, CommandEventArgs e)
        {
            dgFileList.CurrentPageIndex = 0;
            BindFileList();
        }

        private void lnkMoveFiles_Command(object sender, CommandEventArgs e)
        {
            var arFiles = MoveFiles.Split(';');

            var strErrorMessages = "";

            if (!HasPermission("ADD"))
            {
                strErrorMessages = Localization.GetString("NoWritePermission", LocalResourceFile);
            }
            else
            {
                var sourceFolder = FolderManager.Instance.GetFolder(FolderPortalID, UnMaskPath(SourcePath).Replace(RootFolderPath, "").Replace("\\", "/"));
                var destinationFolder = FolderManager.Instance.GetFolder(FolderPortalID, UnMaskPath(DestPath).Replace(RootFolderPath, "").Replace("\\", "/"));
                IFileInfo sourceFile;

                for (var i = 0; i <= arFiles.Length - 1; i++)
                {
                    if (!String.IsNullOrEmpty(arFiles[i]))
                    {
                        sourceFile = Services.FileSystem.FileManager.Instance.GetFile(sourceFolder, arFiles[i]);

                        try
                        {
                            switch (MoveStatus)
                            {
                                case "copy":
                                    Services.FileSystem.FileManager.Instance.CopyFile(sourceFile, destinationFolder);
                                    break;
                                case "move":
                                    Services.FileSystem.FileManager.Instance.MoveFile(sourceFile, destinationFolder);
                                    break;
                                case "unzip":
                                    Services.FileSystem.FileManager.Instance.UnzipFile(sourceFile, destinationFolder);
                                    BindFolderTree();
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            DnnLog.Error(ex);
                            if (MoveStatus == "copy")
                            {
                                strErrorMessages += Localization.GetString("ErrorCopyingFile", LocalResourceFile) +
                                    PathUtils.Instance.AddTrailingSlash(UnMaskPath(SourcePath)) +
                                    arFiles[i] + "&nbsp;&nbsp; to " + PathUtils.Instance.AddTrailingSlash(UnMaskPath(DestPath)) +
                                    "<br />&nbsp;&nbsp;&nbsp;" + ex.Message + "<br />";
                            }
                            else
                            {
                                strErrorMessages += Localization.GetString("ErrorMovingFile", LocalResourceFile) +
                                    PathUtils.Instance.AddTrailingSlash(UnMaskPath(SourcePath)) +
                                    arFiles[i] + "&nbsp;&nbsp; to " + PathUtils.Instance.AddTrailingSlash(UnMaskPath(DestPath)) +
                                    "<br />&nbsp;&nbsp;&nbsp;" + ex.Message + "<br />";
                            }
                        }
                    }
                }
            }

            if (String.IsNullOrEmpty(strErrorMessages))
            {
                LastPath = PathUtils.Instance.RemoveTrailingSlash(DestPath);
            }
            else
            {
                strErrorMessages = MaskString(strErrorMessages);
                strErrorMessages = MaskString(strErrorMessages);
                ShowErrorMessage(strErrorMessages);
            }

            ManageSecurity();
            BindFileList();
            MoveStatus = "";
            SourcePath = "";
        }

        protected void lnkMoveFirst_Command(object sender, CommandEventArgs e)
        {
            PageIndex = 0;
            BindFileList();
        }

        protected void lnkMoveLast_Command(object sender, CommandEventArgs e)
        {
            PageIndex = dgFileList.PageCount - 1;
            BindFileList();
        }

        protected void lnkMoveNext_Command(object sender, CommandEventArgs e)
        {
            PageIndex += 1;
            if (PageIndex > dgFileList.PageCount - 1)
            {
                PageIndex = dgFileList.PageCount - 1;
            }
            BindFileList();
        }

        protected void lnkMovePrevious_Command(object sender, CommandEventArgs e)
        {
            PageIndex -= 1;
            if (PageIndex < 0)
            {
                PageIndex = 0;
            }
            BindFileList();
        }

        protected void lnkOkRename_Command(object sender, CommandEventArgs e)
        {
            var strSourcePath = PathUtils.Instance.AddTrailingSlash(UnMaskPath(DestPath));
            var intItemID = Convert.ToInt32(e.CommandName);
            var strFileName = e.CommandArgument.ToString();
            var txtEdit = (TextBox)dgFileList.Items[intItemID].FindControl("txtEditFileName");
            var strSourceFile = strSourcePath + e.CommandArgument;
            var strDestFile = strSourcePath + txtEdit.Text;
            var strError = "";

            var sourceFolder = FolderManager.Instance.GetFolder(FolderPortalID, UnMaskPath(DestPath).Replace(RootFolderPath, "").Replace("\\", "/"));
            var file = Services.FileSystem.FileManager.Instance.GetFile(sourceFolder, strFileName);

            if (strSourceFile != strDestFile)
            {
                try
                {
                    Services.FileSystem.FileManager.Instance.RenameFile(file, txtEdit.Text);
                }
                catch (Exception ex)
                {
                    DnnLog.Error(ex);
                    strError = ex.Message;
                }

                if (!String.IsNullOrEmpty(strError))
                {
                    strError = Localization.GetString("Rename.Error", LocalResourceFile) + strError;
                }
                else
                {
                    strSourceFile = strDestFile;
                }
            }
            
            if (String.IsNullOrEmpty(strError))
            {
                var chkReadOnly = (CheckBox) dgFileList.Items[intItemID].FindControl("chkReadOnly");
                var chkHidden = (CheckBox) dgFileList.Items[intItemID].FindControl("chkHidden");
                var chkSystem = (CheckBox) dgFileList.Items[intItemID].FindControl("chkSystem");
                var chkArchive = (CheckBox) dgFileList.Items[intItemID].FindControl("chkArchive");
                
                if ((chkReadOnly.Attributes["original"] != chkReadOnly.Checked.ToString()) || (chkHidden.Attributes["original"] != chkHidden.Checked.ToString()) ||
                    (chkSystem.Attributes["original"] != chkSystem.Checked.ToString()) || (chkArchive.Attributes["original"] != chkArchive.Checked.ToString()))
                {
                    int iAttr = 0;
                    
                    if (chkReadOnly.Checked)
                    {
                        iAttr += (int) FileAttributes.ReadOnly;
                    }
                    
                    if (chkHidden.Checked)
                    {
                        iAttr += (int) FileAttributes.Hidden;
                    }
                    
                    if (chkSystem.Checked)
                    {
                        iAttr += (int) FileAttributes.System;
                    }
                    
                    if (chkArchive.Checked)
                    {
                        iAttr += (int) FileAttributes.Archive;
                    }
                    
                    try
                    {
                        file.FileAttributes = (FileAttributes) iAttr;
                    }
                    catch (Exception ex)
                    {
                        DnnLog.Error(ex);
                        strError = ex.Message;
                    }
                    if (!String.IsNullOrEmpty(strError))
                    {
                        strError = Localization.GetString("SetAttrubute.Error", LocalResourceFile) + strError;
                    }
                }
            }
            
            if (!String.IsNullOrEmpty(strError))
            {
                ShowErrorMessage(MaskString(strError));
            }
            
            dgFileList.EditItemIndex = -1;
            BindFileList();
            SetEditMode();
        }

        private void lnkRefresh_Command(object sender, CommandEventArgs e)
        {
            BindFolderTree();
            IsRefresh = true;
            Sort = "FileType ASC, FileName ASC";
            LastSort = "FileType ASC, FileName ASC";
            MoveStatus = "";
            FilterFiles = "";
            BindFileList();
        }

        private void lnkSelectFolder_Command(object sender, CommandEventArgs e)
        {
            string strSourcePath = DestPath;
            string strFriendlyPath = Regex.Replace(strSourcePath, "^0\\\\", "Portal Root\\");
            dgFileList.CurrentPageIndex = 0;
            ClientAPI.AddButtonConfirm(lnkDeleteFolder, string.Format(Localization.GetString("EnsureDeleteFolder", LocalResourceFile), strFriendlyPath));
            strSourcePath = UnMaskPath(strSourcePath.Replace("\\\\", "\\"));
            LastPath = strSourcePath;
            GetFilesByFolder(PathUtils.Instance.AddTrailingSlash(strSourcePath));
        }

        private void lnkSyncFolder_Command(object sender, CommandEventArgs e)
        {
            string syncFolderPath = UnMaskPath(DestPath);
            bool isRecursive = chkRecursive.Checked;
            string relPath = syncFolderPath.Replace(RootFolderPath, "").Replace("\\", "/");
            FolderManager.Instance.Synchronize(FolderPortalID, relPath, isRecursive, true);
            BindFolderTree();
            BindFileList();
        }

        protected void lnkSyncFolders_Click(object sender, ImageClickEventArgs e)
        {
            if (IsHostMenu)
            {
                FolderManager.Instance.Synchronize(Null.NullInteger, "", true, false);
            }
            else
            {
                FolderManager.Instance.Synchronize(PortalId, "", true, false);
            }
            BindFolderTree();
            BindFileList();
        }

        private void lnkUpload_Command(object sender, CommandEventArgs e)
        {
            string strDestPath = Regex.Replace(DestPath, "^0\\\\", "");
            string WebUploadParam = "ftype=" + UploadType.File;
            string returnTab = "rtab=" + TabId;
            string destUrl = EditUrl("dest", Globals.QueryStringEncode(strDestPath), "Edit", WebUploadParam, returnTab);
            Response.Redirect(destUrl);
        }

        private void selPageSize_SelectedIndexChanged(Object sender, EventArgs e)
        {
            PageIndex = 0;
            BindFileList();
        }

        #region Nested type: eImageType

        private enum eImageType
        {
            Folder = 0,
            SecureFolder = 1,
            DatabaseFolder = 2,
            Page = 3
        }

        #endregion

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var Actions = new ModuleActionCollection();
                Actions.Add(GetNextActionID(),
                            Localization.GetString("ManageFolderTypes.Action", LocalResourceFile),
                            ModuleActionType.EditContent,
                            "",
                            "icon_profile_16px.gif",
                            EditUrl("FolderMappings"),
                            false,
                            SecurityAccessLevel.Edit,
                            true,
                            false);
                return Actions;
            }
        }
    }
}
