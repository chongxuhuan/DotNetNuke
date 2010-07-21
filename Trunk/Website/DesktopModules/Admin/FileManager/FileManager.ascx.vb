'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging

Imports ICSharpCode.SharpZipLib.Zip

Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.UI.WebControls
Imports DNNTreeNode = DotNetNuke.UI.WebControls.TreeNode
Imports DNNTreeNodeCollection = DotNetNuke.UI.WebControls.TreeNodeCollection
Imports DotNetNuke.Security.Permissions


Namespace DotNetNuke.Modules.Admin.FileManager

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : FileManager
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Supplies the functionality for uploading files to the Portal
    ''' Synchronizing Files within the folder and the database
    ''' and Provides status of available disk space for the portal
    ''' as well as limiting uploads to the restricted allocated file space
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[DYNST]	        2/1/2004	Created
    '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
    '''     [cnurse]        12/2/2004   Database Synchronization added
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class FileManager
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Enums"

        Private Enum eImageType
            Folder = 0
            SecureFolder = 1
            DatabaseFolder = 2
            Page = 3
        End Enum

#End Region

#Region "Private Members"

        Private _DisplayingMessage As Boolean
        Private m_strParentFolderName As String
        Private imageDirectory As String = "~/images/FileManager/Icons/"
        Private _ErrorMessage As String = "<TABLE><TR><TD height=100% class=NormalRed>{0}</TD></TR><TR valign=bottom><TD align=center><INPUT id=btnClearError onclick=clearErrorMessage(); type=button value=OK></TD></TR></TABLE>"

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property IsAdminRole() As Boolean
            Get
                Return PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName)
            End Get
        End Property

        Protected ReadOnly Property HasPermission(ByVal permissionKey As String) As Boolean
            Get
                Dim _HasPermision As Boolean = Null.NullBoolean
                Dim strSourcePath As String = UnMaskPath(DestPath).Replace(RootFolderPath, "").Replace("\", "/")

                Dim objFolders As New FolderController
                Dim objFolder As FolderInfo = objFolders.GetFolder(FolderPortalID, strSourcePath, False)

                If objFolder IsNot Nothing Then
                    _HasPermision = IsEditable AndAlso FolderPermissionController.HasFolderPermission(objFolder.FolderPermissions, permissionKey)
                End If

                Return _HasPermision
            End Get
        End Property

#End Region

#Region "Public Properties"

        Public ReadOnly Property FolderPortalID() As Integer
            Get
                If IsHostMenu Then
                    Return Null.NullInteger
                Else
                    Return PortalId
                End If
            End Get
        End Property

        Public Property RootFolderName() As String
            Get
                If Not ViewState("RootFolderName") Is Nothing Then
                    Return ViewState("RootFolderName").ToString
                Else
                    Return ""
                End If
            End Get
            Set(ByVal Value As String)
                ViewState("RootFolderName") = Value
            End Set
        End Property

        Public ReadOnly Property RootFolderPath() As String
            Get
                Dim _CurRootFolder As String
                If IsHostMenu Then
                    _CurRootFolder = Common.Globals.HostMapPath
                Else
                    _CurRootFolder = PortalSettings.HomeDirectoryMapPath
                End If
                Return _CurRootFolder
            End Get
        End Property

        Public Property Sort() As String
            Get
                Return ViewState("strSort").ToString()
            End Get
            Set(ByVal Value As String)
                ViewState.Add("strSort", Value)
            End Set
        End Property

        Public Property LastSort() As String
            Get
                Return ViewState("strLastSort").ToString()
            End Get
            Set(ByVal Value As String)
                ViewState.Add("strLastSort", Value)
            End Set
        End Property

        Public Property FilterFiles() As String
            Get
                Return ViewState("strFilterFiles").ToString()
            End Get
            Set(ByVal Value As String)
                ViewState.Add("strFilterFiles", Value)
            End Set
        End Property

        Public Property LastPath() As String
            Get
                Return UnMaskPath(ClientAPI.GetClientVariable(Page, "LastPath"))
            End Get
            Set(ByVal Value As String)
                Value = MaskPath(Value)
                ClientAPI.RegisterClientVariable(Page, "LastPath", Value, True)
            End Set
        End Property

        Public Property DestPath() As String
            Get
                Return ClientAPI.GetClientVariable(Page, "DestPath")
            End Get
            Set(ByVal Value As String)
                ClientAPI.RegisterClientVariable(Page, "DestPath", Value, True)
            End Set
        End Property

        Public Property SourcePath() As String
            Get
                Return ClientAPI.GetClientVariable(Page, "SourcePath")
            End Get
            Set(ByVal Value As String)
                ClientAPI.RegisterClientVariable(Page, "SourcePath", Value, True)
            End Set
        End Property

        Public Property MoveFiles() As String
            Get
                Return ClientAPI.GetClientVariable(Page, "MoveFiles")
            End Get
            Set(ByVal Value As String)
                ClientAPI.RegisterClientVariable(Page, "MoveFiles", Value, True)
            End Set
        End Property

        Public Property IsRefresh() As Boolean
            Get
                Return CBool(ClientAPI.GetClientVariable(Page, "IsRefresh"))
            End Get
            Set(ByVal Value As Boolean)
                ClientAPI.RegisterClientVariable(Page, "IsRefresh", CStr(CInt(Value)), True)
            End Set
        End Property

        Public Property DisabledButtons() As Boolean
            Get
                Return CBool(ClientAPI.GetClientVariable(Page, "DisabledButtons"))
            End Get
            Set(ByVal Value As Boolean)
                ClientAPI.RegisterClientVariable(Page, "DisabledButtons", CStr(CInt(Value)), True)
            End Set
        End Property

        Public Property MoveStatus() As String
            Get
                Return ClientAPI.GetClientVariable(Page, "MoveStatus")
            End Get
            Set(ByVal Value As String)
                ClientAPI.RegisterClientVariable(Page, "MoveStatus", Value, True)
            End Set
        End Property

        Public Property LastFolderPath() As String
            Get
                If Not ViewState("LastFolderPath") Is Nothing Then
                    Return ViewState("LastFolderPath").ToString
                Else
                    Return ""
                End If
            End Get
            Set(ByVal Value As String)
                ViewState("LastFolderPath") = Value
            End Set
        End Property

        Public ReadOnly Property PageSize() As Integer
            Get
                Return CInt(selPageSize.SelectedValue)
            End Get
        End Property

        Public Property PageIndex() As Integer
            Get
                If Not ViewState("PageIndex") Is Nothing Then
                    Return CInt(ViewState("PageIndex"))
                End If
            End Get
            Set(ByVal Value As Integer)
                If Value >= 0 AndAlso Value < dgFileList.PageCount Then ViewState("PageIndex") = Value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a File to the DataTable used for the File List grid
        ''' </summary>
        ''' <param name="tblFiles">The DataTable</param>
        ''' <param name="objFile">The FileInfo object to add</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	    12/3/2004	documented
        '''     [cnurse]        04/24/2006  Updated to use new Secure Storage
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddFileToTable(ByVal tblFiles As DataTable, ByVal objFile As DotNetNuke.Services.FileSystem.FileInfo)
            Dim dRow As DataRow
            dRow = tblFiles.NewRow
            dRow("FileType") = "File"
            dRow("FileId") = objFile.FileId
            dRow("FileName") = objFile.FileName
            dRow("FileSize") = objFile.Size.ToString("##,##0")
            dRow("IntFileSize") = objFile.Size
            If Not objFile.Extension = "" Or Not objFile.Extension Is Nothing Then
                dRow("Extension") = objFile.Extension
            Else
                dRow("Extension") = "none"
            End If
            dRow("StorageLocation") = objFile.StorageLocation

            Select Case objFile.StorageLocation
                Case FolderController.StorageLocationTypes.InsecureFileSystem
                    Dim strSourcePath As String = UnMaskPath(DestPath)
                    Dim fsFile As System.IO.FileInfo = New System.IO.FileInfo(strSourcePath & objFile.FileName)
                    dRow("DateModified") = fsFile.LastWriteTime
                    dRow("Archive") = fsFile.Attributes And FileAttributes.Archive
                    dRow("ReadOnly") = fsFile.Attributes And FileAttributes.ReadOnly
                    dRow("Hidden") = fsFile.Attributes And FileAttributes.Hidden
                    dRow("System") = fsFile.Attributes And FileAttributes.System
                    dRow("AttributeString") = GetAttributeString(fsFile.Attributes)
                Case FolderController.StorageLocationTypes.SecureFileSystem
                    Dim strSourcePath As String = UnMaskPath(DestPath)
                    Dim fsFile As System.IO.FileInfo = New System.IO.FileInfo(strSourcePath & objFile.FileName & glbProtectedExtension)
                    dRow("DateModified") = fsFile.LastWriteTime
                    dRow("Archive") = fsFile.Attributes And FileAttributes.Archive
                    dRow("ReadOnly") = fsFile.Attributes And FileAttributes.ReadOnly
                    dRow("Hidden") = fsFile.Attributes And FileAttributes.Hidden
                    dRow("System") = fsFile.Attributes And FileAttributes.System
                    dRow("AttributeString") = GetAttributeString(fsFile.Attributes)
                Case FolderController.StorageLocationTypes.DatabaseSecure
                    dRow("Archive") = False
                    dRow("ReadOnly") = False
                    dRow("Hidden") = False
                    dRow("System") = False
                    dRow("AttributeString") = ""
            End Select
            tblFiles.Rows.Add(dRow)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds node to tree
        ''' </summary>
        ''' <param name="strName">Name of folder to display</param>
        ''' <param name="strKey">Masked Key of folder location</param>
        ''' <param name="eImage">Type of image</param>
        ''' <param name="objNodes">Node collection to add to</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	10/26/2004	Created
        ''' 	[Jon Henning]	8/24/2005	Added Populate on Demand (POD) logic
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function AddNode(ByVal strName As String, ByVal strKey As String, ByVal eImage As eImageType, ByVal objNodes As DNNTreeNodeCollection) As DNNTreeNode
            Dim objNode As DNNTreeNode
            objNode = New DNNTreeNode(strName)
            objNode.Key = strKey
            objNode.ToolTip = strName
            objNode.ImageIndex = eImage
            objNode.CssClass = "FileManagerTreeNode"
            objNodes.Add(objNode)

            If objNode.Key = DestPath Then
                objNode.Selected = True
                objNode.MakeNodeVisible()
            End If

            Return objNode
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds node to tree
        ''' </summary>
        ''' <param name="folder">The FolderInfo object to add</param>
        ''' <param name="objNodes">Node collection to add to</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	04/24/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function AddNode(ByVal folder As FolderInfo, ByVal objNodes As DNNTreeNodeCollection) As DNNTreeNode

            Dim objNode As DNNTreeNode
            Dim strName As String = folder.FolderName
            Dim strKey As String = MaskPath(RootFolderPath & folder.FolderPath)
            Dim subFolders As ArrayList = FileSystemUtils.GetFoldersByParentFolder(FolderPortalID, folder.FolderPath)
            Dim image As eImageType = eImageType.Folder
            Select Case folder.StorageLocation
                Case FolderController.StorageLocationTypes.InsecureFileSystem
                    image = eImageType.Folder
                Case FolderController.StorageLocationTypes.SecureFileSystem
                    image = eImageType.SecureFolder
                Case FolderController.StorageLocationTypes.DatabaseSecure
                    image = eImageType.DatabaseFolder
            End Select
            objNode = AddNode(strName, strKey, image, objNodes)
            objNode.HasNodes = subFolders.Count > 0

            Return objNode

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindFileList 
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindFileList()
            Dim strCurPage As String

            LastPath = FileSystemUtils.RemoveTrailingSlash(UnMaskPath(DestPath))
            dgFileList.PageSize = PageSize
            dgFileList.CurrentPageIndex = PageIndex

            GetFilesByFolder(FileSystemUtils.StripFolderPath(DestPath).Replace("\", "/"))

            If dgFileList.PageCount > 1 Then
                tblMessagePager.Visible = True
                strCurPage = Localization.GetString("Pages")
                lblCurPage.Text = String.Format(strCurPage, (dgFileList.CurrentPageIndex + 1), (dgFileList.PageCount))
                lnkMoveFirst.Text = "<img border=0 Alt='" & Localization.GetString("First") & "' src='" & ResolveUrl("~/images/FileManager/movefirst.gif") & "'>"
                lnkMovePrevious.Text = "<img border=0 Alt='" & Localization.GetString("Previous") & "' src='" & ResolveUrl("~/images/FileManager/moveprevious.gif") & "'>"
                lnkMoveNext.Text = "<img border=0 Alt='" & Localization.GetString("Next") & "' src='" & ResolveUrl("~/images/FileManager/movenext.gif") & "'>"
                lnkMoveLast.Text = "<img border=0 Alt='" & Localization.GetString("Last") & "' src='" & ResolveUrl("~/images/FileManager/movelast.gif") & "'>"
            Else
                tblMessagePager.Visible = False
            End If

            lblCurFolder.Text = Regex.Replace(DestPath, "^0\\", RootFolderName & "\")
            MoveFiles = ""

            UpdateSpaceUsed()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cpaterra]	4/6/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindStorageLocationTypes()
            ddlStorageLocation.Items.Add(New ListItem(Localization.GetString("InsecureFileSystem", Me.LocalResourceFile), "0"))
            ddlStorageLocation.Items.Add(New ListItem(Localization.GetString("SecureFileSystem", Me.LocalResourceFile), "1"))
            ddlStorageLocation.Items.Add(New ListItem(Localization.GetString("SecureDatabase", Me.LocalResourceFile), "2"))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The BindFolderTree helper method is used to bind the list of
        ''' files for this portal or for the hostfolder, to an asp:DATAGRID server control
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	        2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        '''     [cnurse]        12/2/2004   Updated to use Localization for Root
        ''' 	[Jon Henning]	8/24/2005	Added Populate on Demand (POD) logic
        '''     [cnurse]        04/24/2006  Updated to use new Secure Storage
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindFolderTree()

            Dim objNode As DNNTreeNode

            'Clear the Tree Nodes Collection
            DNNTree.TreeNodes.Clear()

            objNode = AddNode(RootFolderName, MaskPath(RootFolderPath), eImageType.Folder, DNNTree.TreeNodes)

            Dim arrFolders As ArrayList = FileSystemUtils.GetFolders(FolderPortalID)
            objNode.HasNodes = arrFolders.Count > 1
            If Me.DNNTree.PopulateNodesFromClient = False OrElse Me.DNNTree.IsDownLevel Then
                PopulateTree(objNode.TreeNodes, RootFolderPath)
            End If

            If DNNTree.SelectedTreeNodes.Count = 0 Then objNode.Selected = True

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetCheckAllString 
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetCheckAllString() As String
            Dim intCount As Integer = dgFileList.Items.Count

            Dim chkFile As CheckBox
            Dim i As Integer
            Dim strResult As String
            strResult = "setMoveFiles('');" & vbCrLf
            For i = 0 To intCount - 1
                chkFile = CType(dgFileList.Items(i).FindControl("chkFile"), CheckBox)
                If Not (chkFile) Is Nothing Then
                    strResult = strResult & "var chk1 = dnn.dom.getById('" & chkFile.ClientID & "');"
                    strResult = strResult & "chk1.checked = blValue;" & vbCrLf
                    strResult = strResult & "if (!chk1.onclick) {chk1.parentElement.onclick();}else{chk1.onclick();}" & vbCrLf
                End If
            Next
            strResult = "function CheckAllFiles(blValue) {" & strResult & "}" & vbCrLf

            strResult = "<script language=javascript>" & strResult & "</script>"

            Return strResult
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GeneratePermissionsGrid generates the permissions grid for the folder
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]        12/2/2004   documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GeneratePermissionsGrid()

            Dim folderPath As String = FileSystemUtils.StripFolderPath(DestPath).Replace("\", "/")

            dgPermissions.FolderPath = folderPath

            Dim objFolderInfo As New FolderInfo
            Dim objFolderController As New FolderController
            objFolderInfo = objFolderController.GetFolder(FolderPortalID, folderPath, False)

            If Not objFolderInfo Is Nothing Then
                ddlStorageLocation.SelectedValue = CType(objFolderInfo.StorageLocation, String)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetAttributeString generates the attributes string from the FileAttributes
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]        12/2/2004   documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetAttributeString(ByVal attributes As System.IO.FileAttributes) As String
            Dim strResult As String = ""
            If (attributes And FileAttributes.Archive) = FileAttributes.Archive Then
                strResult += "A"
            End If
            If (attributes And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                strResult += "R"
            End If
            If (attributes And FileAttributes.Hidden) = FileAttributes.Hidden Then
                strResult += "H"
            End If
            If (attributes And FileAttributes.System) = FileAttributes.System Then
                strResult += "S"
            End If
            Return strResult
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetFilesByFolder gets the Files/Folders to display
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]        12/2/2004   documented and modified to display Folders in 
        '''                                 the grid
        '''     [cnurse]        04/24/2006  Updated to use new Secure Storage
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GetFilesByFolder(ByVal strFolderName As String)

            Dim tblFiles As DataTable = GetFileTable()

            Dim objFolder As FolderInfo = FileSystemUtils.GetFolder(FolderPortalID, strFolderName)
            If Not objFolder Is Nothing Then
                Dim arrFiles As ArrayList = FileSystemUtils.GetFilesByFolder(FolderPortalID, objFolder.FolderID)
                For Each objFile As DotNetNuke.Services.FileSystem.FileInfo In arrFiles
                    AddFileToTable(tblFiles, objFile)
                Next
            End If

            Dim dv As DataView = New DataView
            dv.Table = tblFiles
            dv.Sort = Sort
            If FilterFiles <> "" Then
                dv.RowFilter = "FileName like '%" & Me.FilterFiles & "%'"
            End If

            dgFileList.DataSource = dv
            dgFileList.DataBind()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetFileTable creates the DataTable used to store the list of files and folders
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]        12/3/2004   documented and modified to display Folders in 
        '''                                 the grid
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetFileTable() As DataTable

            Dim tblFiles As New DataTable("Files")

            Dim myColumns As New DataColumn
            myColumns.DataType = System.Type.GetType("System.String")
            myColumns.ColumnName = "FileType"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.Int32")
            myColumns.ColumnName = "FileId"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.String")
            myColumns.ColumnName = "FileName"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.String")
            myColumns.ColumnName = "FileSize"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.Int32")
            myColumns.ColumnName = "IntFileSize"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.Int32")
            myColumns.ColumnName = "StorageLocation"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.DateTime")
            myColumns.ColumnName = "DateModified"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.Boolean")
            myColumns.ColumnName = "ReadOnly"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.Boolean")
            myColumns.ColumnName = "Hidden"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.Boolean")
            myColumns.ColumnName = "System"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.Boolean")
            myColumns.ColumnName = "Archive"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.String")
            myColumns.ColumnName = "AttributeString"
            tblFiles.Columns.Add(myColumns)

            myColumns = New DataColumn
            myColumns.DataType = System.Type.GetType("System.String")
            myColumns.ColumnName = "Extension"
            tblFiles.Columns.Add(myColumns)

            Return tblFiles

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the size of the all the files in the zip file
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/4/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetZipFileExtractSize(ByVal strFileName As String) As Long
            Dim objZipEntry As ZipEntry
            Dim objZipInputStream As ZipInputStream
            Try
                objZipInputStream = New ZipInputStream(File.OpenRead(strFileName))
            Catch ex As Exception
                ShowErrorMessage(MaskString(ex.Message))
                Return -1
            End Try

            objZipEntry = objZipInputStream.GetNextEntry
            Dim iTemp As Long

            While Not objZipEntry Is Nothing
                iTemp = iTemp + objZipEntry.Size
                objZipEntry = objZipInputStream.GetNextEntry
            End While
            objZipInputStream.Close()
            Return iTemp

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets common properties on DNNTree control
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' 	[Jon Henning]	8/24/2005	Added Populate on Demand (POD) logic
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub InitializeTree()
            DNNTree.SystemImagesPath = ResolveUrl("~/images/")
            DNNTree.ImageList.Add(ResolveUrl("~/images/folder.gif"))
            DNNTree.ImageList.Add(ResolveUrl("~/images/icon_securityroles_16px.gif"))
            DNNTree.ImageList.Add(ResolveUrl("~/images/icon_sql_16px.gif"))
            DNNTree.ImageList.Add(ResolveUrl("~/images/file.gif"))
            DNNTree.IndentWidth = 10
            DNNTree.CollapsedNodeImage = ResolveUrl("~/images/max.gif")
            DNNTree.ExpandedNodeImage = ResolveUrl("~/images/min.gif")
            DNNTree.PopulateNodesFromClient = True
            DNNTree.JSFunction = "nodeSelected();"
        End Sub

        Private Sub ManageToolbarButton(ByVal wrapperControl As HtmlGenericControl, ByVal imageControl As System.Web.UI.WebControls.Image, ByVal js As String, ByVal imageRootName As String, ByVal enableButton As Boolean)
            If enableButton Then
                wrapperControl.Attributes.Add("style", "cursor: pointer")
                wrapperControl.Attributes.Add("onclick", js)
                imageControl.ImageUrl = "~/images/FileManager/ToolBar" + imageRootName + "Enabled.gif"
            Else
                wrapperControl.Attributes.Remove("style")
                wrapperControl.Attributes.Remove("onclick")
                imageControl.ImageUrl = "~/images/FileManager/ToolBar" + imageRootName + "Disabled.gif"
            End If
        End Sub

        Private Sub ManageSecurity()
            ManageToolbarButton(addFolder, lnkAddFolderIMG, "return canAddFolder();", "AddFolder", HasPermission("ADD"))
            ManageToolbarButton(deleteFolder, lnkDelFolderIMG, "return deleteFolder();", "DelFolder", HasPermission("DELETE"))
            ManageToolbarButton(syncFolder, lnkSyncFolderIMG, "__doPostBack(m_sUCPrefixName + 'lnkSyncFolder', '');", "Synchronize", HasPermission("MANAGE"))
            chkRecursive.Enabled = HasPermission("MANAGE")

            ManageToolbarButton(refresh, lnkRefreshIMG, "__doPostBack(m_sUCPrefixName + 'lnkRefresh', '');", "Refresh", True)
            ManageToolbarButton(copy, lnkCopy, "copyCheckedFiles();", "Copy", HasPermission("COPY"))
            ManageToolbarButton(move, lnkMove, "moveFiles();", "Move", HasPermission("COPY"))
            ManageToolbarButton(upload, lnkUploadIMG, "__doPostBack(m_sUCPrefixName + 'lnkUpload', '');", "Upload", HasPermission("ADD"))
            ManageToolbarButton(delete, lnkDelete, "deleteCheckedFiles();", "Delete", HasPermission("DELETE"))
            ManageToolbarButton(filter, lnkFilterIMG, "__doPostBack(m_sUCPrefixName + 'lnkFilter', '');", "Filter", True)

            lnkCopy.Enabled = IsEditable
            lnkMove.Enabled = IsEditable
            lnkUpload.Enabled = IsEditable
            lnkDelete.Enabled = IsEditable

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Masks the path
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function MaskPath(ByVal strOrigPath As String) As String
            Return Replace(strOrigPath, FileSystemUtils.RemoveTrailingSlash(RootFolderPath), "0").Replace("/", "\")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Masks a string 
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function MaskString(ByVal strSource As String) As String
            Return FileManagerFunctions.CReplace(strSource, FileSystemUtils.RemoveTrailingSlash(RootFolderPath), Localization.GetString("PortalRoot", Me.LocalResourceFile), 1)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Populates DNNTree control with folder hiearachy
        ''' </summary>
        ''' <param name="objNodes">Node collection to add children to</param>
        ''' <param name="strPath">Path of parent node</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	10/26/2004	Created
        ''' 	[Jon Henning]	8/24/2005	Added Populate on Demand (POD) logic
        '''     [cnurse]        04/24/2006  Updated to use new Secure Storage
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PopulateTree(ByVal objNodes As DNNTreeNodeCollection, ByVal strPath As String)
            Dim folderPath As String = strPath.Replace(RootFolderPath, "").Replace("\", "/")
            Dim folders As ArrayList = FileSystemUtils.GetFoldersByParentFolder(FolderPortalID, folderPath)
            Dim objNode As DNNTreeNode

            'Iterate through the SubFolders
            For Each folder As FolderInfo In folders
                If FolderPermissionController.CanViewFolder(folder) Then
                    objNode = AddNode(folder, objNodes)
                    If Me.DNNTree.PopulateNodesFromClient = False Then
                        PopulateTree(objNode.TreeNodes, folder.FolderPath)
                    End If
                End If
            Next
        End Sub

        Private Sub SetFolder(ByVal node As DNNTreeNode)
            dgFileList.EditItemIndex = -1
            If DNNTree.IsDownLevel Then
                DestPath = node.Key
                LastPath = node.Key
            End If
            ManageSecurity()

            BindFileList()
            GeneratePermissionsGrid()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets up the file manager for Edit Mode
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SetEditMode()

            If dgFileList.EditItemIndex > -1 Then
                'In Edit Mode
                Dim intCount As Integer = dgFileList.Items.Count
                Dim chkFile2 As CheckBox
                Dim chkFile As CheckBox
                Dim lnkDeleteFile As ImageButton
                Dim lnkEditFile As ImageButton
                Dim i As Integer
                For i = 0 To intCount - 1
                    If i <> dgFileList.EditItemIndex Then
                        chkFile2 = CType(dgFileList.Items(i).FindControl("chkFile2"), CheckBox)
                        chkFile = CType(dgFileList.Items(i).FindControl("chkFile"), CheckBox)
                        lnkDeleteFile = CType(dgFileList.Items(i).FindControl("lnkDeleteFile"), ImageButton)
                        lnkEditFile = CType(dgFileList.Items(i).FindControl("lnkEditFile"), ImageButton)
                        If Not (chkFile2) Is Nothing Then chkFile2.Enabled = False
                        If Not (chkFile) Is Nothing Then chkFile.Enabled = False
                        If Not (lnkDeleteFile) Is Nothing Then
                            lnkDeleteFile.Enabled = False
                            lnkDeleteFile.ImageUrl = "~/images/FileManager/DNNExplorer_trash_disabled.gif"
                            lnkDeleteFile.AlternateText = ""
                        End If
                        If Not (lnkEditFile) Is Nothing Then
                            lnkEditFile.Enabled = False
                            lnkEditFile.ImageUrl = "~/images/FileManager/DNNExplorer_Edit_disabled.gif"
                            lnkEditFile.AlternateText = ""
                        End If
                        chkFile2 = Nothing
                        chkFile = Nothing
                        lnkDeleteFile = Nothing
                        lnkEditFile = Nothing
                    End If
                Next
                Me.DisabledButtons = True
            Else
            End If
            dgFileList.Columns(0).HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(5)
            dgFileList.Columns(1).HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(25)
            dgFileList.Columns(2).HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(25)
            dgFileList.Columns(3).HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(7)
            dgFileList.Columns(4).HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(15)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets up the Error Message
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ShowErrorMessage(ByVal strMessage As String)
            strMessage = Replace(strMessage, "\", "\\")
            strMessage = Replace(strMessage, "'", "\'")
            strMessage = Replace(strMessage, vbCrLf, "\n")
            strMessage = String.Format(_ErrorMessage, strMessage)
            _DisplayingMessage = True
            ClientAPI.RegisterClientVariable(Me.Page, "ErrorMessage", strMessage, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Synchronizes the complete File System
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Synchronize()
            If IsHostMenu Then
                FileSystemUtils.Synchronize(Null.NullInteger, Null.NullInteger, Common.Globals.HostMapPath, False)
            Else
                FileSystemUtils.Synchronize(PortalId, PortalSettings.AdministratorRoleId, PortalSettings.HomeDirectoryMapPath, PortalSettings.HideFoldersEnabled)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Unmasks the path
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function UnMaskPath(ByVal strOrigPath As String) As String
            strOrigPath = FileSystemUtils.AddTrailingSlash(RootFolderPath) & FileSystemUtils.StripFolderPath(strOrigPath)
            Return strOrigPath.Replace("/", "\")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates the space Used label
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UpdateSpaceUsed()
            Dim strDestFolder As String = FileSystemUtils.AddTrailingSlash(UnMaskPath(DestPath))
            Dim objPortalController As New PortalController
            Dim strUsed As String
            Dim strQuota As String

            If PortalSettings.HostSpace = 0 Then
                strQuota = Localization.GetString("UnlimitedSpace", Me.LocalResourceFile)
            Else
                strQuota = PortalSettings.HostSpace.ToString() + "MB"
            End If

            If IsHostMenu Then
                lblFileSpace.Text = "&nbsp;"
            Else
                Dim spaceUsed As Long = objPortalController.GetPortalSpaceUsedBytes(FolderPortalID)
                If spaceUsed < 1024 Then
                    strUsed = spaceUsed.ToString("0.00") + "B"
                ElseIf spaceUsed < (1024 * 1024) Then
                    strUsed = (spaceUsed / 1024).ToString("0.00") + "KB"
                Else
                    strUsed = (spaceUsed / (1024 * 1024)).ToString("0.00") + "MB"
                End If

                lblFileSpace.Text = String.Format(Localization.GetString("SpaceUsed", Me.LocalResourceFile), strUsed, strQuota)
            End If
        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DeleteFiles helper method is used to delete the files in the list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strFiles">The list of files to delete</param>
        ''' <history>
        ''' 	[DYNST]	        2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub DeleteFiles(ByVal strFiles As String)
            Dim arFiles() As String = Split(strFiles, ";")
            Dim i As Integer
            If arFiles.Length = 0 Then
                Exit Sub
            End If
            Dim strSourcePath As String
            Dim strErrorMessage As String = ""
            Dim strCurError As String
            strSourcePath = FileSystemUtils.AddTrailingSlash(LastPath)

            For i = 0 To arFiles.Length - 1
                If arFiles(i) <> "" Then
                    strCurError = FileSystemUtils.DeleteFile(strSourcePath & arFiles(i), PortalSettings, False)
                    If strCurError <> "" Then
                        strErrorMessage = strErrorMessage & Localization.GetString("ErrorDeletingFile", Me.LocalResourceFile) & FileSystemUtils.AddTrailingSlash(UnMaskPath(DestPath)) & arFiles(i) & "<BR>&nbsp;&nbsp;&nbsp;" & strCurError & "<BR>"
                    End If
                End If
            Next

            If strErrorMessage <> "" Then
                strErrorMessage = MaskString(strErrorMessage)
                ShowErrorMessage(strErrorMessage)
            End If

            BindFileList()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Renders the page output
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub Render(ByVal output As System.Web.UI.HtmlTextWriter)
            'mark various controls as valid for event validation
            Page.ClientScript.RegisterForEventValidation(lnkAddFolder.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkDeleteFolder.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkDeleteAllCheckedFiles.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkRefresh.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkSelectFolder.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkSyncFolder.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkFilter.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkCopy.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkUpload.UniqueID)

            Page.ClientScript.RegisterForEventValidation(lnkMove.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkMoveFirst.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkMoveLast.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkMoveNext.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkMovePrevious.UniqueID)
            Page.ClientScript.RegisterForEventValidation(lnkMoveFiles.UniqueID)

            Dim strTemp As String = GetCheckAllString()

            pnlScripts2.Controls.Add(New LiteralControl(strTemp))
            If dgFileList.Items.Count <= 10 And dgFileList.PageCount = 1 Then
                dgFileList.PagerStyle.Visible = False
            End If

            MyBase.Render(output)
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The CheckDestFolderAccess helper method Checks to make sure file copy/move 
        ''' operation will not exceed portal available space
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	        2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        '''     [cnurse]        12/2/2004   Updated to use Localization
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CheckDestFolderAccess(ByVal intSize As Long) As String
            If Request.IsAuthenticated Then
                Dim strDestFolder As String = FileSystemUtils.AddTrailingSlash(UnMaskPath(DestPath))
                Dim objPortalController As New PortalController

                If objPortalController.HasSpaceAvailable(FolderPortalID, intSize) Or (PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId) Then
                    Return ""
                Else
                    Return Localization.GetString("NotEnoughSpace", Me.LocalResourceFile)
                End If
            Else
                Return Localization.GetString("PleaseLogin", Me.LocalResourceFile)
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Image associated with the File/Folder
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/4/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetImageUrl(ByVal type As String) As String
            Dim url As String = ""

            Try
                If type = "folder" Then
                    url = imageDirectory & "ClosedFolder.gif"
                Else
                    If type <> "" AndAlso File.Exists(Server.MapPath(imageDirectory & type & ".gif")) Then
                        url = imageDirectory + type + ".gif"
                    Else
                        url = imageDirectory & "File.gif"
                    End If
                End If
            Catch exc As Exception   'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return url

        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Page_Load server event handler on this user control is used
        ''' to populate the current files from the appropriate PortalUpload Directory or the HostFolder
        ''' and binds this list to the Datagrid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                'FileManager requires at a bare minimum the dnn namespace, so regardless of wheter the ClientAPI is disabled of not we 
                'need to register it.
                ClientAPI.RegisterClientReference(Me.Page, ClientAPI.ClientNamespaceReferences.dnn)

                DNNClientAPI.AddBodyOnloadEventHandler(Me.Page, "initFileManager();")
                ClientAPI.RegisterClientVariable(Me.Page, "UCPrefixID", DNNTree.ClientID.Replace(DNNTree.ID, ""), True)
                ClientAPI.RegisterClientVariable(Me.Page, "UCPrefixName", DNNTree.UniqueID.Replace(DNNTree.ID, ""), True)

                If DNNTree.IsDownLevel Then
                    Me.DisabledButtons = True
                Else
                    Me.DisabledButtons = False
                End If

                If IsHostMenu Then
                    RootFolderName = Localization.GetString("HostRoot", Me.LocalResourceFile)
                    pnlSecurity.Visible = False
                Else
                    RootFolderName = Localization.GetString("PortalRoot", Me.LocalResourceFile)
                    'Only Administrators can manage Folder Security
                    pnlSecurity.Visible = HasPermission("WRITE")
                End If

                If Page.IsPostBack = False Then
                    Common.Utilities.DataCache.ClearFolderCache(FolderPortalID)
                    Localization.LocalizeDataGrid(dgFileList, Me.LocalResourceFile)
                    InitializeTree()
                    BindFolderTree()
                    IsRefresh = True
                    PageIndex = 0
                    Sort = "FileType ASC, FileName ASC"
                    LastSort = "FileType ASC, FileName ASC"
                    MoveStatus = ""
                    FilterFiles = ""
                    DestPath = "0\"
                    BindFileList()
                    BindStorageLocationTypes()

                    ManageSecurity()
                Else
                    FilterFiles = txtFilter.Text
                End If

                If LastFolderPath <> DestPath Then
                    PageIndex = 0
                    GeneratePermissionsGrid()
                End If
                LastFolderPath = DestPath

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The cmdUpdate_Click server event handler on this user control runs when the
        ''' Update button is clicked
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	        2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        '''     [Jon Henning]	4/21/2004	Rebind grid after update to reflect update - DNN-178
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Dim strFolderPath As String = FileSystemUtils.StripFolderPath(Me.LastFolderPath).Replace("\", "/")
            Dim objFolderController As New FolderController
            Dim objFolderInfo As FolderInfo = objFolderController.GetFolder(FolderPortalID, strFolderPath, False)
            If objFolderInfo Is Nothing Then
                'file system needs synchronizing
                'with database...this folder is new.
                Synchronize()
                objFolderInfo = objFolderController.GetFolder(FolderPortalID, strFolderPath, True)
            End If

            objFolderInfo.FolderPermissions.Clear()
            objFolderInfo.FolderPermissions.AddRange(dgPermissions.Permissions)

            Try
                FolderPermissionController.SaveFolderPermissions(objFolderInfo)
                Skins.Skin.AddModuleMessage(Me, Localization.GetString("PermissionsUpdated", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            Catch ex As Exception
                LogException(ex)
                Skins.Skin.AddModuleMessage(Me, Localization.GetString("PermissionsError", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End Try


            GeneratePermissionsGrid()    'rebind the grid to reflect updated values - it is possible for the grid controls and the database to become out of sync
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The dgFileList_ItemDataBound server event handler on this user control runs when a
        ''' File or Folder is added to the Files Table
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        '''     [cnurse]        12/3/2004   modified to handle folders and to use
        '''                                 custom images
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub dgFileList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgFileList.ItemDataBound
            Dim lnkEditFile As ImageButton
            Dim chkFile As CheckBox
            Dim lnkDeleteFile As ImageButton
            Dim lnkUnzip As System.Web.UI.WebControls.Image
            Dim lnkOkRename As ImageButton
            Dim blEnabled As Boolean = True
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.EditItem Or e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.SelectedItem Then
                chkFile = CType(e.Item.FindControl("chkFile"), CheckBox)
                If Not chkFile Is Nothing Then
                    Dim sDefCssClass As String = dgFileList.ItemStyle.CssClass
                    If e.Item.ItemType = ListItemType.AlternatingItem Then sDefCssClass = dgFileList.AlternatingItemStyle.CssClass

                    chkFile.Attributes.Add("onclick", "addFileToMoveList('" & ClientAPI.GetSafeJSString(chkFile.Attributes("filename")) & _
                    "', this, '" & dgFileList.SelectedItemStyle.CssClass & _
                    "', '" & sDefCssClass & _
                    "');")
                End If

                lnkEditFile = CType(e.Item.FindControl("lnkEditFile"), System.Web.UI.WebControls.ImageButton)
                If Not lnkEditFile Is Nothing Then
                    lnkEditFile.CommandName = e.Item.ItemIndex.ToString
                End If

                lnkUnzip = CType(e.Item.FindControl("lnkUnzip"), System.Web.UI.WebControls.Image)
                If Not lnkUnzip Is Nothing Then
                    If lnkUnzip.Attributes("extension") <> "zip" Then
                        lnkUnzip.Visible = False
                    Else
                        If e.Item.ItemType = ListItemType.EditItem Then
                            lnkUnzip.Visible = False
                        Else
                            lnkUnzip.Attributes.Add("onclick", "return unzipFile('" & ClientAPI.GetSafeJSString(lnkUnzip.Attributes("filename")) & "');")
                        End If
                    End If
                End If

                lnkDeleteFile = CType(e.Item.FindControl("lnkDeleteFile"), System.Web.UI.WebControls.ImageButton)
                If Not lnkDeleteFile Is Nothing Then
                    If dgFileList.EditItemIndex = -1 Then
                        ClientAPI.AddButtonConfirm(lnkDeleteFile, String.Format(Localization.GetString("EnsureDeleteFile", Me.LocalResourceFile), lnkDeleteFile.CommandName))
                    End If
                End If

                lnkOkRename = CType(e.Item.FindControl("lnkOkRename"), System.Web.UI.WebControls.ImageButton)
                If Not lnkOkRename Is Nothing Then
                    lnkOkRename.CommandName = e.Item.ItemIndex.ToString
                End If

            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The dgFileList_SortCommand server event handler on this user control runs when one
        ''' of the Column Header Links is clicked
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/12/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub dgFileList_SortCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridSortCommandEventArgs) Handles dgFileList.SortCommand
            BindFolderTree()
            Me.IsRefresh = True
            LastSort = Sort
            If Sort.Replace(" ASC", "").Replace(" DESC", "") = e.SortExpression Then
                'Switch order
                If Sort.Contains("ASC") Then
                    Sort = Sort.Replace("ASC", "DESC")
                Else
                    Sort = Sort.Replace("DESC", "ASC")
                End If
            Else
                Sort = e.SortExpression + " ASC"
            End If
            MoveStatus = ""
            FilterFiles = ""
            BindFileList()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DNNTree_NodeClick server event handler on this user control runs when a
        ''' Node (Folder in the) in the TreeView is clicked
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        '''     [cnurse]        12/3/2004   modified to handle folders and to use
        '''                                 custom images
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DNNTree_NodeClick(ByVal source As Object, ByVal e As DotNetNuke.UI.WebControls.DNNTreeNodeClickEventArgs) Handles DNNTree.NodeClick
            SetFolder(e.Node)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This method is called from the client to populate send new nodes down to the client
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	8/24/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DNNTree_PopulateOnDemand(ByVal source As Object, ByVal e As UI.WebControls.DNNTreeEventArgs) Handles DNNTree.PopulateOnDemand
            DestPath = e.Node.Key
            PopulateTree(e.Node.TreeNodes, UnMaskPath(e.Node.Key.Replace("\\", "\")))
            GeneratePermissionsGrid()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkAddFolder_Command server event handler on this user control runs when the
        ''' Add Folder button is clicked
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lnkAddFolder_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkAddFolder.Command

            If Me.txtNewFolder.Text = "" Then
                Exit Sub
            End If
            Dim strSourcePath As String

            strSourcePath = UnMaskPath(DestPath)

            Try

                If (DNNTree.TreeNodes(0).DNNNodes.Count = 0) Then
                    PopulateTree(DNNTree.TreeNodes(0).TreeNodes, RootFolderPath)
                End If

                Dim colNodes As Collection = DNNTree.SelectedTreeNodes()
                If colNodes.Count > 0 Then
                    Dim parentNode As DNNTreeNode = CType(colNodes(1), DNNTreeNode)

                    Dim filterFolderName As String
                    filterFolderName = txtNewFolder.Text.Replace(".", "_")
                    'Add Folder to Database
                    FileSystemUtils.AddFolder(PortalSettings, strSourcePath, filterFolderName, Integer.Parse(ddlStorageLocation.SelectedValue))

                    DestPath = MaskPath(FileSystemUtils.AddTrailingSlash(strSourcePath) + filterFolderName + "\")
                    LastFolderPath = DestPath

                    'Add new folder to folders tree
                    parentNode.Selected = False
                    Dim image As eImageType = eImageType.Folder
                    Select Case Integer.Parse(ddlStorageLocation.SelectedValue)
                        Case FolderController.StorageLocationTypes.InsecureFileSystem
                            image = eImageType.Folder
                        Case FolderController.StorageLocationTypes.SecureFileSystem
                            image = eImageType.SecureFolder
                        Case FolderController.StorageLocationTypes.DatabaseSecure
                            image = eImageType.DatabaseFolder
                    End Select
                    Dim objNode As DNNTreeNode = AddNode(filterFolderName, parentNode.Key.Replace("\\", "\") + filterFolderName + "\", image, parentNode.TreeNodes)
                    objNode.HasNodes = False
                    objNode.MakeNodeVisible()
                    objNode.Selected = True

                    SetFolder(objNode)
                End If
            Catch ex As Exception
                Dim strErrorMessage As String = MaskString(ex.Message)
                ShowErrorMessage(strErrorMessage)
            End Try

            txtNewFolder.Text = ""
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkDeleteFolder_Command server event handler on this user control runs when the
        ''' Add Folder ibutton is clicked
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lnkDeleteFolder_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkDeleteFolder.Command
            Dim strSourcePath As String
            Dim ctrlError As New LiteralControl

            If DestPath = DNNTree.TreeNodes(0).Key Then
                'Delete Root Node?  Then what? :/
                ShowErrorMessage(Localization.GetString("NotAllowedToDeleteRootFolder", Me.LocalResourceFile))
                BindFileList()
                Exit Sub
            Else
                strSourcePath = UnMaskPath(DestPath)
            End If

            Dim dinfo As New System.IO.DirectoryInfo(strSourcePath)
            If dinfo.Exists = False Then
                'ODD...
                ShowErrorMessage(Localization.GetString("FolderAlreadyRemoved", Me.LocalResourceFile))
                BindFileList()
                Exit Sub
            End If

            If (System.IO.Directory.GetDirectories(strSourcePath).Length > 0) Or (dgFileList.Items.Count > 0) Then
                'Files and/or folders exist in directory..
                'Files in current folder, make them delete first
                'Recursive Folder-delete can be enabled by adjusting this Sub
                ShowErrorMessage(Localization.GetString("PleaseRemoveFilesBeforeDeleting", Me.LocalResourceFile))
                BindFileList()
                Exit Sub
            End If

            Try
                'Delete Folder
                Dim folderName As String = FileSystemUtils.StripFolderPath(DestPath)
                FileSystemUtils.DeleteFolder(FolderPortalID, dinfo, folderName)

                Dim intEnd As Integer
                If DestPath.EndsWith("\") Then DestPath = DestPath.Substring(0, DestPath.Length - 1)
                intEnd = InStrRev(DestPath, "\")
                DestPath = DestPath.Substring(0, intEnd)

                'since we removed folder, we will select parent folder
                Dim colNodes As Collection = DNNTree.SelectedTreeNodes()
                If colNodes.Count > 0 Then
                    Dim objNode As DNNTreeNode = CType(colNodes(1), DNNTreeNode)
                    objNode.Selected = False
                    objNode.Parent.Selected = True
                    objNode.Parent.DNNNodes.Remove(objNode)
                End If

                BindFileList()
                GeneratePermissionsGrid()

            Catch ex As Exception
                ShowErrorMessage(Localization.GetString("ErrorDeletingFolder", Me.LocalResourceFile) & ex.Message)
            End Try


        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkDLFile_Command server event handler on this user control runs when the
        ''' Download File button is clicked
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' The method calls the FileSystemUtils DownLoad method
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub lnkDLFile_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)
            FileSystemUtils.DownloadFile(PortalSettings, CType(e.CommandArgument, Integer), False, True)
            BindFolderTree()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkEditFile_Command server event handler on this user control runs when the
        ''' Edit File button is clicked
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' The DataGrid is switched to Edit Mode
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub lnkEditFile_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)
            dgFileList.EditItemIndex = CType(e.CommandName, Integer)
            BindFileList()
            SetEditMode()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkCancelRename_Command server event handler on this user control runs when the
        ''' Cancel Edit button is clicked when in Edit Mode
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub lnkCancelRename_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)
            dgFileList.EditItemIndex = -1
            BindFileList()
            SetEditMode()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkDeleteAllCheckedFiles_Command server event handler on this user control runs when the
        ''' Javascript in the page triggers the event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lnkDeleteAllCheckedFiles_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkDeleteAllCheckedFiles.Command
            If Me.MoveFiles <> "" Then
                DeleteFiles(MoveFiles)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkDeleteFile_Command server event handler on this user control runs when the
        ''' Javascript in the page triggers the event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub lnkDeleteFile_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)
            DeleteFiles(e.CommandName)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkFilter_Command server event handler on this user control runs when the
        ''' Filter Files button is clicked.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' The method calls the relevant FileSystemUtils method
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lnkFilter_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkFilter.Command
            Me.dgFileList.CurrentPageIndex = 0
            'FilterFiles = txtFilter.Text
            BindFileList()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkMoveFiles_Command server event handler on this user control runs when the
        ''' Move Files button is clicked.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' The method calls the relevant FileSystemUtils method
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lnkMoveFiles_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkMoveFiles.Command
            Dim arFiles() As String
            arFiles = Split(MoveFiles, ";")
            Dim i As Integer
            Dim strSourceFile As String
            Dim strDestFile As String
            Dim strErrorMessages As String = ""
            Dim strCurErrorMessage As String = ""

            Dim strSourcePath As String
            Dim strDestPath As String
            strDestPath = FileSystemUtils.AddTrailingSlash(UnMaskPath(DestPath))
            strSourcePath = FileSystemUtils.AddTrailingSlash(UnMaskPath(SourcePath))

            'Check that the user has write permssion on the dest folder
            If Not HasPermission("ADD") Then
                strErrorMessages = Localization.GetString("NoWritePermission", Me.LocalResourceFile)
            End If

            If strErrorMessages = "" Then
                For i = 0 To arFiles.Length - 1
                    If arFiles(i) <> "" Then
                        strSourceFile = strSourcePath & arFiles(i)
                        strDestFile = strDestPath & arFiles(i)

                        strCurErrorMessage = ""
                        Select Case MoveStatus
                            Case "copy"
                                strCurErrorMessage = FileSystemUtils.CopyFile(strSourceFile, strDestFile, PortalSettings)
                            Case "move"
                                strCurErrorMessage = FileSystemUtils.MoveFile(strSourceFile, strDestFile, PortalSettings)
                            Case "unzip"
                                strCurErrorMessage = FileSystemUtils.UnzipFile(strSourceFile, strDestPath, PortalSettings)
                                BindFolderTree()
                        End Select

                        If strCurErrorMessage <> "" Then
                            'Unmask paths here, remask with title before showining error message
                            If MoveStatus = "copy" Then
                                strErrorMessages = strErrorMessages & Localization.GetString("ErrorCopyingFile", Me.LocalResourceFile) & FileSystemUtils.AddTrailingSlash(UnMaskPath(SourcePath)) & arFiles(i) & "&nbsp;&nbsp; to " & FileSystemUtils.AddTrailingSlash(UnMaskPath(DestPath)) & "<BR>&nbsp;&nbsp;&nbsp;" & strCurErrorMessage & "<BR>"
                            Else
                                strErrorMessages = strErrorMessages & Localization.GetString("ErrorMovingFile", Me.LocalResourceFile) & FileSystemUtils.AddTrailingSlash(UnMaskPath(SourcePath)) & arFiles(i) & "&nbsp;&nbsp; to " & FileSystemUtils.AddTrailingSlash(UnMaskPath(DestPath)) & "<BR>&nbsp;&nbsp;&nbsp;" & strCurErrorMessage & "<BR>"
                            End If

                        End If
                    End If
                Next
            End If

            If strErrorMessages = "" Then
                LastPath = FileSystemUtils.RemoveTrailingSlash(DestPath)
            Else
                strErrorMessages = MaskString(strErrorMessages)
                strErrorMessages = MaskString(strErrorMessages)
                ShowErrorMessage(strErrorMessages)
            End If

            ManageSecurity()
            BindFileList()
            MoveStatus = ""
            SourcePath = ""

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkMoveFirst_Command server event handler on this user control runs when the
        ''' Move First Page button is clicked.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' The method calls the relevant FileSystemUtils method
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub lnkMoveFirst_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkMoveFirst.Command
            PageIndex = 0
            BindFileList()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkMoveLast_Command server event handler on this user control runs when the
        ''' Move Last Page button is clicked.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' The method calls the relevant FileSystemUtils method
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub lnkMoveLast_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkMoveLast.Command
            Me.PageIndex = Me.dgFileList.PageCount - 1
            BindFileList()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkMoveNext_Command server event handler on this user control runs when the
        ''' Move Next Page button is clicked.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' The method calls the relevant FileSystemUtils method
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub lnkMoveNext_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkMoveNext.Command
            PageIndex += 1
            If PageIndex > dgFileList.PageCount - 1 Then
                PageIndex = dgFileList.PageCount - 1
            End If
            BindFileList()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkMoveNext_Command server event handler on this user control runs when the
        ''' Move Previous Page button is clicked.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' The method calls the relevant FileSystemUtils method
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub lnkMovePrevious_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkMovePrevious.Command
            PageIndex -= 1
            If PageIndex < 0 Then
                PageIndex = 0
            End If
            BindFileList()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkOkRename_Command server event handler on this user control runs when the
        ''' Save Changes (Ok) button is clicked when in Edit Mode
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub lnkOkRename_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)
            Dim strSourcePath As String
            Dim intItemID As Integer = CType(e.CommandName, Integer)
            strSourcePath = FileSystemUtils.AddTrailingSlash(UnMaskPath(DestPath))

            Dim strFileName As String = e.CommandArgument.ToString()
            Dim txtEdit As TextBox
            txtEdit = CType(dgFileList.Items(intItemID).FindControl("txtEditFileName"), TextBox)

            Dim strSourceFile As String

            strSourceFile = strSourcePath & e.CommandArgument.ToString()

            Dim strDestFile As String = strSourcePath & txtEdit.Text
            Dim strReturn As String = ""
            Dim strError As String = ""

            If strSourceFile <> strDestFile Then
                ' move(rename) file
                strError = FileSystemUtils.MoveFile(strSourceFile, strDestFile, PortalSettings)
                If strError <> "" Then
                    strError = Localization.GetString("Rename.Error", Me.LocalResourceFile) & strError
                Else
                    strSourceFile = strDestFile
                End If
            End If

            If strError = "" Then
                Dim chkReadOnly As CheckBox = CType(dgFileList.Items(intItemID).FindControl("chkReadOnly"), CheckBox)
                Dim chkHidden As CheckBox = CType(dgFileList.Items(intItemID).FindControl("chkHidden"), CheckBox)
                Dim chkSystem As CheckBox = CType(dgFileList.Items(intItemID).FindControl("chkSystem"), CheckBox)
                Dim chkArchive As CheckBox = CType(dgFileList.Items(intItemID).FindControl("chkArchive"), CheckBox)
                If (chkReadOnly.Attributes("original") <> chkReadOnly.Checked.ToString) Or (chkHidden.Attributes("original") <> chkHidden.Checked.ToString) Or (chkSystem.Attributes("original") <> chkSystem.Checked.ToString) Or (chkArchive.Attributes("original") <> chkArchive.Checked.ToString) Then
                    '  attributes were changed
                    Dim iAttr As Integer
                    If chkReadOnly.Checked Then iAttr += FileAttributes.ReadOnly
                    If chkHidden.Checked Then iAttr += FileAttributes.Hidden
                    If chkSystem.Checked Then iAttr += FileAttributes.System
                    If chkArchive.Checked Then iAttr += FileAttributes.Archive

                    Try
                        FileSystemUtils.SetFileAttributes(strSourceFile, iAttr)
                    Catch ex As Exception
                        strError = ex.Message
                    End Try

                    If strError <> "" Then
                        strError = Localization.GetString("SetAttrubute.Error", Me.LocalResourceFile) & strError
                    End If
                End If
            End If

            If strError <> "" Then
                ShowErrorMessage(MaskString(strError))
            End If

            dgFileList.EditItemIndex = -1
            BindFileList()
            SetEditMode()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkRefresh_Command server event handler on this user control runs when the
        ''' Refresh button is clicked.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lnkRefresh_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkRefresh.Command
            BindFolderTree()
            Me.IsRefresh = True
            Sort = "FileType ASC, FileName ASC"
            LastSort = "FileType ASC, FileName ASC"
            MoveStatus = ""
            FilterFiles = ""
            BindFileList()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkSelectFolder_Command server event handler on this user control runs when a
        ''' Folder is selected.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lnkSelectFolder_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkSelectFolder.Command

            Dim strSourcePath As String = DestPath
            Dim strFriendlyPath As String = Regex.Replace(strSourcePath, "^0\\", "Portal Root\")

            dgFileList.CurrentPageIndex = 0
            UI.Utilities.ClientAPI.AddButtonConfirm(lnkDeleteFolder, String.Format(Localization.GetString("EnsureDeleteFolder", Me.LocalResourceFile), strFriendlyPath))
            strSourcePath = UnMaskPath(strSourcePath.Replace("\\", "\"))
            LastPath = strSourcePath
            GetFilesByFolder(FileSystemUtils.AddTrailingSlash(strSourcePath))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkSyncFolder_Command server event handler on this user control runs when the
        ''' Synchronize Folder button is clicked.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	04/24/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lnkSyncFolder_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkSyncFolder.Command
            Dim syncFolderPath As String = UnMaskPath(DestPath)
            Dim isRecursive As Boolean = chkRecursive.Checked
            Dim relPath As String = syncFolderPath.Replace(RootFolderPath, "").Replace("\", "/")

            FileSystemUtils.SynchronizeFolder(FolderPortalID, syncFolderPath, relPath, isRecursive, PortalSettings.HideFoldersEnabled)

            BindFolderTree()
            BindFileList()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkSyncFolders_Click server event handler on this user control runs when the
        ''' Synchronize Folders button is clicked.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub lnkSyncFolders_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles lnkSyncFolders.Click
            If IsHostMenu Then
                FileSystemUtils.SynchronizeFolder(Null.NullInteger, Common.Globals.HostMapPath, "", True, False, True)
            Else
                FileSystemUtils.SynchronizeFolder(PortalId, PortalSettings.HomeDirectoryMapPath, "", True, False, True)
            End If

            BindFolderTree()
            BindFileList()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The lnkUpload_Command server event handler on this user control runs when the
        ''' Upload button is clicked
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lnkUpload_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles lnkUpload.Command
            Dim strDestPath As String = Regex.Replace(DestPath, "^0\\", "")
            Dim WebUploadParam As String = "ftype=" & UploadType.File.ToString()
            Dim returnTab As String = "rtab=" & TabId
            Dim destUrl As String = EditUrl("dest", QueryStringEncode(strDestPath), "Edit", WebUploadParam, returnTab)
            Response.Redirect(destUrl)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The selPageSize_SelectedIndexChanged server event handler on this user control 
        ''' runs when the Page Size combo's index/value is changed
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        '''     [Jon Henning]	11/1/2004	Updated to use ClientAPI/DNNTree
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub selPageSize_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles selPageSize.SelectedIndexChanged
            PageIndex = 0
            BindFileList()
        End Sub

#End Region

    End Class

End Namespace
