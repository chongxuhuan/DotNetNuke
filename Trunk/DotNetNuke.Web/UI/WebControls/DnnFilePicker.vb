'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports System
Imports System.Collections

Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Web.UI
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Framework
Imports Telerik.Web.UI
Imports DotNetNuke.Entities.Users

Namespace DotNetNuke.Web.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The FilePicker Class provides a File Picker Control for DotNetNuke
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	07/31/2006
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DnnFilePicker
        Inherits CompositeControl
        Implements ILocalizable

#Region "Public Enums"

        ''' <summary>
        ''' Represents a possible mode for the File Control
        ''' </summary>
        Protected Enum FileControlMode
            ''' <summary>
            ''' The File Control is in its Normal mode
            ''' </summary>
            Normal

            ''' <summary>
            ''' The File Control is in the Upload File mode
            ''' </summary>
            UpLoadFile

            ''' <summary>
            ''' The File Control is in the Preview mode
            ''' </summary>
            Preview
        End Enum

#End Region

#Region "Controls"

        Private fileTable As HtmlTable

        'Files
        Private folderRow As HtmlTableRow
        Private folderCell As HtmlTableCell
        Private preViewCell As HtmlTableCell
        Private lblFolder As Label
        Private cboFolders As DropDownList

        Private fileRow As HtmlTableRow
        Private fileCell As HtmlTableCell
        Private lblFile As Label
        Private imgPreview As Image
        Private cboFiles As DropDownList
        Private txtFile As HtmlInputFile

        'Command Row
        Private commandRow As HtmlTableRow
        Private commandCell As HtmlTableCell
        Private cmdUpload As CommandButton
        Private cmdSave As CommandButton
        Private cmdCancel As CommandButton

        'essages
        Private messageRow As HtmlTableRow
        Private messageCell As HtmlTableCell
        Private lblMessage As Label

#End Region

#Region "Private Members"

        Private _Localize As Boolean = True
        Private _LocalResourceFile As String
        Private _MaxHeight As Integer = 100
        Private _MaxWidth As Integer = 135

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the control is on a Host or Portal Tab
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property IsHost() As Boolean
            Get
                Dim _IsHost As Boolean = Null.NullBoolean
                If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                    _IsHost = True
                End If
                Return _IsHost
            End Get
        End Property

        Public Property MaxHeight() As Integer
            Get
                Return _MaxHeight
            End Get
            Set(ByVal value As Integer)
                _MaxHeight = value
            End Set
        End Property

        Public Property MaxWidth() As Integer
            Get
                Return _MaxWidth
            End Get
            Set(ByVal value As Integer)
                _MaxWidth = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the current mode of the control
        ''' </summary>
        ''' <remarks>Defaults to FileControlMode.Normal</remarks>
        ''' <value>A FileControlMode enum</value>
        ''' <history>
        ''' 	[cnurse]	7/13/2005  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property Mode() As FileControlMode
            Get
                If ViewState("Mode") Is Nothing Then
                    Return FileControlMode.Normal
                Else
                    Return DirectCast(ViewState("Mode"), FileControlMode)
                End If
            End Get
            Set(ByVal value As FileControlMode)
                ViewState("Mode") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the root folder for the control
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ParentFolder() As String
            Get
                Dim _ParentFolder As String
                If IsHost Then
                    _ParentFolder = Globals.HostMapPath
                Else
                    _ParentFolder = PortalSettings.HomeDirectoryMapPath
                End If
                Return _ParentFolder
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the file PortalId to use
        ''' </summary>
        ''' <remarks>Defaults to PortalSettings.PortalId</remarks>
        ''' <value>An Integer</value>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property PortalId() As Integer
            Get
                Dim _PortalId As Integer = Null.NullInteger
                If Not IsHost Then
                    _PortalId = PortalSettings.PortalId
                End If
                Return _PortalId
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the current Portal Settings
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property PortalSettings() As PortalSettings
            Get
                Return PortalController.GetCurrentPortalSettings()
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the class to be used for the Labels
        ''' </summary>
        ''' <remarks>Defaults to 'CommandButton'</remarks>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	7/13/2005  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CommandCssClass() As String
            Get
                Dim _Class As String = Convert.ToString(ViewState("CommandCssClass"))
                If String.IsNullOrEmpty(_Class) Then
                    Return "CommandButton"
                Else
                    Return _Class
                End If
            End Get
            Set(ByVal value As String)
                ViewState("CommandCssClass") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the file Filter to use
        ''' </summary>
        ''' <remarks>Defaults to ''</remarks>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	7/13/2005  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FileFilter() As String
            Get
                If ViewState("FileFilter") IsNot Nothing Then
                    Return DirectCast(ViewState("FileFilter"), String)
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                ViewState("FileFilter") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the FileID for the control
        ''' </summary>
        ''' <value>An Integer</value>
        ''' <history>
        ''' 	[cnurse]	7/13/2005  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FileID() As Integer
            Get
                Me.EnsureChildControls()
                If ViewState("FileID") Is Nothing Then
                    'Get FileId from the file combo
                    Dim _FileId As Integer = Null.NullInteger
                    If cboFiles.SelectedItem IsNot Nothing Then
                        _FileId = Int32.Parse(cboFiles.SelectedItem.Value)
                    End If
                    ViewState("FileID") = _FileId
                End If
                Return Convert.ToInt32(ViewState("FileID"))
            End Get
            Set(ByVal value As Integer)
                Me.EnsureChildControls()
                ViewState("FileID") = value
                If String.IsNullOrEmpty(FilePath) Then
                    Dim fileController As New FileController()
                    Dim fileInfo As FileInfo = fileController.GetFileById(value, PortalId)
                    If fileInfo IsNot Nothing Then
                        SetFilePath(fileInfo.Folder + fileInfo.FileName)
                    End If
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the FilePath for the control
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FilePath() As String
            Get
                Return Convert.ToString(ViewState("FilePath"))
            End Get
            Set(ByVal value As String)
                ViewState("FilePath") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether to Use Personal Folder
        ''' </summary>
        ''' <remarks>Defaults to false</remarks>
        ''' <value>A Boolean</value>
        ''' -----------------------------------------------------------------------------
        Public Property UsePersonalFolder() As Boolean
            Get
                If ViewState("UsePersonalFolder") Is Nothing Then
                    Return False
                Else
                    Return Convert.ToBoolean(ViewState("UsePersonalFolder"))
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("UsePersonalFolder") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the class to be used for the Labels
        ''' </summary>
        ''' <remarks>Defaults to 'NormalBold'</remarks>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LabelCssClass() As String
            Get
                Dim _Class As String = Convert.ToString(ViewState("LabelCssClass"))
                If String.IsNullOrEmpty(_Class) Then
                    Return "NormalBold"
                Else
                    Return _Class
                End If
            End Get
            Set(ByVal value As String)
                ViewState("LabelCssClass") = value
            End Set
        End Property

        Public Property Permissions() As String
            Get
                Dim _Permissions As String = Convert.ToString(ViewState("Permissions"))
                If String.IsNullOrEmpty(_Permissions) Then
                    Return "BROWSE,ADD"
                Else
                    Return _Permissions
                End If
            End Get
            Set(ByVal value As String)
                ViewState("Permissions") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether the combos have a "Not Specified" option
        ''' </summary>
        ''' <remarks>Defaults to True (ie no "Not Specified")</remarks>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Required() As Boolean
            Get
                If ViewState("Required") Is Nothing Then
                    Return False
                Else
                    Return Convert.ToBoolean(ViewState("Required"))
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("Required") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether to Show Database Folders
        ''' </summary>
        ''' <remarks>Defaults to false</remarks>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	7/31/2005  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowDatabase() As Boolean
            Get
                If ViewState("ShowDatabase") Is Nothing Then
                    Return False
                Else
                    Return Convert.ToBoolean(ViewState("ShowDatabase"))
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("ShowDatabase") = value
            End Set
        End Property

        Public Property ShowFolders As Boolean
            Get
                If ViewState("ShowFolders") Is Nothing Then
                    Return True
                Else
                    Return Convert.ToBoolean(ViewState("ShowFolders"))
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("ShowFolders") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether to Show Secure Folders
        ''' </summary>
        ''' <remarks>Defaults to false</remarks>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	7/31/2005  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowSecure() As Boolean
            Get
                If ViewState("ShowSecure") Is Nothing Then
                    Return False
                Else
                    Return Convert.ToBoolean(ViewState("ShowSecure"))
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("ShowSecure") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether to Show the Upload Button
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	7/31/2005  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowUpLoad() As Boolean
            Get
                If ViewState("ShowUpLoad") Is Nothing Then
                    Return True
                Else
                    Return Convert.ToBoolean(ViewState("ShowUpLoad"))
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("ShowUpLoad") = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddButton adds a button to the Command Row
        ''' </summary>
        ''' <param name="button">The button to add to the Row</param>
        ''' <param name="imageUrl">The url to the image for the button</param>
        ''' <history>
        ''' 	[cnurse]	07/31/2006 created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddButton(ByRef button As CommandButton, ByVal imageUrl As String)
            button = New CommandButton()
            button.EnableViewState = False
            button.CausesValidation = False
            button.ControlStyle.CssClass = CommandCssClass
            If Not String.IsNullOrEmpty(imageUrl) Then
                button.DisplayIcon = True
                button.ImageUrl = imageUrl
            End If
            button.Visible = False
            commandCell.Controls.Add(button)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddCommandRow adds the Command Row
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddCommandRow()
            'Create Command Row
            commandRow = New HtmlTableRow()
            commandRow.Visible = False
            commandCell = New HtmlTableCell()
            AddButton(cmdUpload, "~/images/up.gif")
            AddHandler cmdUpload.Click, AddressOf UploadFile
            AddButton(cmdSave, "~/images/save.gif")
            AddHandler cmdSave.Click, AddressOf SaveFile

            'Add separator
            commandCell.Controls.Add(New LiteralControl("&nbsp;&nbsp;"))

            AddButton(cmdCancel, "~/images/lt.gif")
            AddHandler cmdCancel.Click, AddressOf CancelUpload

            commandRow.Cells.Add(commandCell)
            fileTable.Rows.Add(commandRow)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddFileRow adds the Files Row
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddFileRow()
            'Create Url Row
            fileRow = New HtmlTableRow()
            fileCell = New HtmlTableCell()

            'Create File Label
            lblFile = New Label()
            lblFile.EnableViewState = False
            fileCell.Controls.Add(lblFile)

            'Add <br>
            fileCell.Controls.Add(New LiteralControl("<br/>"))

            'Create Files Combo
            cboFiles = New DropDownList()
            cboFiles.ID = "File"
            cboFiles.DataTextField = "Text"
            cboFiles.DataValueField = "Value"
            cboFiles.AutoPostBack = True
            AddHandler cboFiles.SelectedIndexChanged, AddressOf FileChanged
            fileCell.Controls.Add(cboFiles)

            'Create Upload Box
            txtFile = New HtmlInputFile()
            fileCell.Controls.Add(txtFile)

            'Add Cell/row/table
            fileRow.Cells.Add(fileCell)
            fileTable.Rows.Add(fileRow)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddFolderRow adds the Folders Row
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddFolderRow()
            'Create Url Row
            folderRow = New HtmlTableRow()
            folderCell = New HtmlTableCell()

            'Create Folder Label
            lblFolder = New Label()
            lblFolder.EnableViewState = False
            folderCell.Controls.Add(lblFolder)

            'Add <br>
            folderCell.Controls.Add(New LiteralControl("<br/>"))

            'Create Folders Combo
            cboFolders = New DropDownList()
            cboFolders.ID = "Folder"
            cboFolders.AutoPostBack = True
            AddHandler cboFolders.SelectedIndexChanged, AddressOf FolderChanged
            folderCell.Controls.Add(cboFolders)

            'Load Folders
            LoadFolders()

            'Add Preview
            preViewCell = New HtmlTableCell()
            preViewCell.VAlign = "top"
            preViewCell.RowSpan = 3

            imgPreview = New Image()
            preViewCell.Controls.Add(imgPreview)

            'Add Cell/row/table
            folderRow.Cells.Add(folderCell)
            folderRow.Cells.Add(preViewCell)
            fileTable.Rows.Add(folderRow)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddMessageRow adds the Message Row
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddMessageRow()
            'Create Type Row
            messageRow = New HtmlTableRow()
            messageCell = New HtmlTableCell()
            messageCell.ColSpan = 2

            'Create Label
            lblMessage = New Label()
            lblMessage.EnableViewState = False
            lblMessage.CssClass = "NormalRed"
            lblMessage.Text = ""
            messageCell.Controls.Add(lblMessage)

            'Add to Table
            messageRow.Cells.Add(messageCell)
            fileTable.Rows.Add(messageRow)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetFileList fetches the list of files to display in the File combo
        ''' </summary>
        ''' <param name="NoneSpecified">A flag indicating whether the NoneSpecified item is 
        ''' shown in the list</param>
        ''' <param name="Folder">The folder to fetch the list of files</param>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetFileList(ByVal NoneSpecified As Boolean, ByVal Folder As String) As ArrayList
            Dim fileList As ArrayList

            If IsHost Then
                fileList = Globals.GetFileList(Null.NullInteger, FileFilter, NoneSpecified, cboFolders.SelectedItem.Value)
            Else
                fileList = Globals.GetFileList(PortalId, FileFilter, NoneSpecified, cboFolders.SelectedItem.Value)
            End If

            Return fileList
        End Function

        Private Function IsUserFolder(ByVal folderPath As String) As Boolean
            Return (folderPath.ToLowerInvariant().StartsWith("users/") AndAlso _
                                folderPath.EndsWith(String.Format("/{0}/", UserController.GetCurrentUserInfo.UserID.ToString())))
        End Function

        Private Sub LoadFiles()
            cboFiles.DataSource = GetFileList(Not Required, cboFolders.SelectedItem.Value)
            cboFiles.DataBind()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadFolders fetches the list of folders from the Database
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LoadFolders()
            cboFolders.Items.Clear()

            'Add Personal Folder
            If UsePersonalFolder Then
                Dim userFolder As String = FileSystemUtils.GetUserFolderPath(UserController.GetCurrentUserInfo().UserID)
                Dim userFolderItem As ListItem = cboFolders.Items.FindByValue(userFolder)
                If userFolderItem IsNot Nothing Then
                    userFolderItem.Text = Utilities.GetLocalizedString("MyFolder")
                Else
                    'Add Dummy Folder
                    cboFolders.Items.Add(New ListItem(Utilities.GetLocalizedString("MyFolder"), userFolder))
                End If
            Else
                Dim folders As ArrayList = FileSystemUtils.GetFoldersByUser(PortalId, ShowSecure, ShowDatabase, Permissions)
                For Each folder As FolderInfo In folders
                    Dim folderItem As New ListItem
                    If folder.FolderPath = Null.NullString Then
                        folderItem.Text = Utilities.GetLocalizedString("PortalRoot")
                    Else
                        folderItem.Text = folder.DisplayPath
                    End If
                    folderItem.Value = folder.FolderPath
                    cboFolders.Items.Add(folderItem)
                Next
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SetFilePath sets the FilePath property
        ''' </summary>
        ''' <remarks>This overload uses the selected item in the Folder combo</remarks>
        ''' <history>
        ''' 	[cnurse]	08/01/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SetFilePath()
            SetFilePath(cboFiles.SelectedItem.Text)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SetFilePath sets the FilePath property
        ''' </summary>
        ''' <remarks>This overload allows the caller to specify a file</remarks>
        ''' <param name="fileName">The filename to use in setting the property</param>
        ''' <history>
        ''' 	[cnurse]	08/01/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SetFilePath(ByVal fileName As String)
            If String.IsNullOrEmpty(cboFolders.SelectedItem.Value) Then
                FilePath = fileName
            Else
                FilePath = (cboFolders.SelectedItem.Value & "/") + fileName
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ShowButton configures and displays a button
        ''' </summary>
        ''' <param name="button">The button to configure</param>
        ''' <param name="command">The command name (amd key) of the button</param>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ShowButton(ByVal button As CommandButton, ByVal command As String)
            button.Visible = True
            If Not String.IsNullOrEmpty(command) Then
                button.Text = Utilities.GetLocalizedString(command)
            End If
            button.RegisterForPostback()
            commandRow.Visible = True
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ShowImage displays the Preview Image
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/01/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ShowImage()
            Dim objController As New FileController()
            Dim image As FileInfo = objController.GetFileById(FileID, PortalId)

            If image IsNot Nothing Then
                imgPreview.ImageUrl = Globals.LinkClick("fileid=" & FileID.ToString(), PortalSettings.ActiveTab.TabID, Null.NullInteger)

                Utilities.CreateThumbnail(image, imgPreview, MaxWidth, MaxHeight)

                imgPreview.Visible = True
            Else
                imgPreview.Visible = False
            End If
        End Sub

#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)
            LocalResourceFile = Utilities.GetLocalResourceFile(Me)
            Me.EnsureChildControls()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateChildControls overrides the Base class's method to correctly build the
        ''' control based on the configuration
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overloads Overrides Sub CreateChildControls()
            'First clear the controls collection
            Controls.Clear()

            'Create Table
            fileTable = New HtmlTable()

            AddFolderRow()
            AddFileRow()

            AddCommandRow()
            AddMessageRow()

            'Add table to Control
            Me.Controls.Add(fileTable)

            'Call base class's method

            MyBase.CreateChildControls()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnPreRender runs just before the control is rendered
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overloads Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)

            If cboFolders.Items.Count > 0 Then
                'Configure Labels
                lblFolder.Text = Utilities.GetLocalizedString("Folder")
                lblFolder.CssClass = LabelCssClass
                lblFile.Text = Utilities.GetLocalizedString("File")
                lblFile.CssClass = LabelCssClass

                'select folder
                Dim fileName As String
                Dim folderPath As String
                If Not String.IsNullOrEmpty(FilePath) Then
                    fileName = FilePath.Substring(FilePath.LastIndexOf("/") + 1)
                    If String.IsNullOrEmpty(fileName) Then
                        folderPath = FilePath
                    Else
                        folderPath = FilePath.Replace(fileName, "")
                    End If
                Else
                    fileName = FilePath
                    folderPath = String.Empty
                End If

                If cboFolders.Items.FindByValue(folderPath) IsNot Nothing Then
                    cboFolders.SelectedIndex = -1
                    cboFolders.Items.FindByValue(folderPath).Selected = True
                End If
                cboFolders.Width = Width

                'Get Files
                LoadFiles()
                If cboFiles.Items.FindByText(fileName) IsNot Nothing Then
                    cboFiles.Items.FindByText(fileName).Selected = True
                End If
                If cboFiles.SelectedItem Is Nothing OrElse String.IsNullOrEmpty(cboFiles.SelectedItem.Value) Then
                    FileID = -1
                Else
                    FileID = Int32.Parse(cboFiles.SelectedItem.Value)
                End If
                cboFiles.Width = Width

                If cboFolders.Items.Count > 1 AndAlso ShowFolders Then
                    cboFolders.Visible = True
                    lblFolder.Visible = True
                Else
                    cboFolders.Visible = False
                    lblFolder.Visible = False
                End If

                'Configure Mode
                Select Case Mode
                    Case FileControlMode.Normal
                        fileRow.Visible = True
                        cboFiles.Visible = True
                        ShowImage()
                        txtFile.Visible = False
                        If (FolderPermissionController.HasFolderPermission(PortalId, cboFolders.SelectedItem.Value, "ADD") OrElse _
                                IsUserFolder(cboFolders.SelectedItem.Value)) AndAlso _
                                ShowUpLoad Then
                            ShowButton(cmdUpload, "Upload")
                        End If
                        Exit Select
                    Case FileControlMode.UpLoadFile
                        cboFiles.Visible = False
                        txtFile.Visible = True
                        imgPreview.Visible = False
                        ShowButton(cmdSave, "Save")
                        ShowButton(cmdCancel, "Cancel")
                        Exit Select
                End Select
            Else
                lblMessage.Text = Utilities.GetLocalizedString("NoPermission")
            End If

            'Show message Row
            messageRow.Visible = (Not String.IsNullOrEmpty(lblMessage.Text))

        End Sub

#End Region

#Region "Event Handlers"

        Private Sub CancelUpload(ByVal sender As Object, ByVal e As EventArgs)
            Mode = FileControlMode.Normal
        End Sub

        Private Sub FileChanged(ByVal sender As Object, ByVal e As EventArgs)
            SetFilePath()
        End Sub

        Private Sub FolderChanged(ByVal sender As Object, ByVal e As EventArgs)
            LoadFiles()
            SetFilePath()
        End Sub

        Private Sub SaveFile(ByVal sender As Object, ByVal e As EventArgs)
            'if file is selected exit
            If Not String.IsNullOrEmpty(txtFile.PostedFile.FileName) Then
                Dim folderPath As String = ParentFolder + cboFolders.SelectedItem.Value

                Dim extension As String = System.IO.Path.GetExtension(txtFile.PostedFile.FileName).Replace(".", "")

                If Not String.IsNullOrEmpty(FileFilter) AndAlso Not FileFilter.ToLower().Contains(extension.ToLower()) Then
                    ' trying to upload a file not allowed for current filter
                    lblMessage.Text = String.Format(Localization.GetString("UploadError", Me.LocalResourceFile), FileFilter, extension)
                Else
                    'Check if this is a User Folder
                    If IsUserFolder(cboFolders.SelectedItem.Value) Then
                        'Make sure the user folder exists
                        Dim folder As FolderInfo = New FolderController().GetFolder(PortalId, folderPath, False)
                        If folder Is Nothing Then
                            'Add User folder
                            FileSystemUtils.AddUserFolder(PortalSettings, PortalSettings.HomeDirectoryMapPath, FolderController.StorageLocationTypes.InsecureFileSystem, Me.PortalSettings.UserId)
                        End If
                    End If

                    lblMessage.Text = FileSystemUtils.UploadFile(folderPath.Replace("/", "\"), txtFile.PostedFile, False)
                End If

                If String.IsNullOrEmpty(lblMessage.Text) Then
                    Dim fileName As String = txtFile.PostedFile.FileName.Substring(txtFile.PostedFile.FileName.LastIndexOf("\") + 1)
                    SetFilePath(fileName)
                End If
            End If
            Mode = FileControlMode.Normal
        End Sub

        Private Sub UploadFile(ByVal sender As Object, ByVal e As EventArgs)
            Mode = FileControlMode.UpLoadFile
        End Sub

#End Region

#Region "ILocalizable Implementation"

        Public Property Localize() As Boolean Implements ILocalizable.Localize
            Get
                Return _Localize
            End Get
            Set(ByVal value As Boolean)
                _Localize = value
            End Set
        End Property

        Public Property LocalResourceFile() As String Implements ILocalizable.LocalResourceFile
            Get
                Return _LocalResourceFile
            End Get
            Set(ByVal value As String)
                _LocalResourceFile = value
            End Set
        End Property

        Protected Overridable Sub LocalizeStrings() Implements ILocalizable.LocalizeStrings
        End Sub

#End Region

    End Class
End Namespace
