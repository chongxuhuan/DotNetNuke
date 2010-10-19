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

Imports ICSharpCode.SharpZipLib.Zip

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.UI.Skins

Namespace DotNetNuke.Modules.Admin.FileManager

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : WebUpload
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Supplies the functionality for uploading files to the Portal
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''   [cnurse] 16/9/2004  Updated for localization, Help and 508
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class WebUpload
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Members"

        Private _FileTypeName As String
        Private _FileType As UploadType
        Private _DestinationFolder As String     ' content files
        Private _UploadRoles As String
        Private _RootFolder As String

#End Region

#Region "Public Properties"

        Public ReadOnly Property DestinationFolder() As String
            Get
                If _DestinationFolder Is Nothing Then
                    _DestinationFolder = String.Empty
                    If Not (Request.QueryString("dest") Is Nothing) Then
                        _DestinationFolder = QueryStringDecode(Request.QueryString("dest"))
                    End If
                End If
                Return FileSystemUtils.RemoveTrailingSlash(_DestinationFolder.Replace("\", "/"))
            End Get
        End Property

        Public ReadOnly Property FileType() As UploadType
            Get
                _FileType = UploadType.File
                If Not (Request.QueryString("ftype") Is Nothing) Then
                    'The select statement ensures that the parameter can be converted to UploadType
                    Select Case Request.QueryString("ftype").ToLower
                        Case "file", "container", "skin"
                            _FileType = DirectCast(System.Enum.Parse(GetType(UploadType), Request.QueryString("ftype")), UploadType)
                    End Select
                End If
                Return _FileType
            End Get
        End Property


        Public ReadOnly Property FileTypeName() As String
            Get
                If _FileTypeName Is Nothing Then
                    _FileTypeName = Localization.GetString(FileType.ToString, Me.LocalResourceFile)
                End If
                Return _FileTypeName
            End Get
        End Property

        Public ReadOnly Property FolderPortalID() As Integer
            Get
                If IsHostMenu Then
                    Return Null.NullInteger
                Else
                    Return PortalId
                End If
            End Get
        End Property

        Public ReadOnly Property RootFolder() As String
            Get
                If _RootFolder Is Nothing Then
                    If IsHostMenu Then
                        _RootFolder = Common.Globals.HostMapPath
                    Else
                        _RootFolder = PortalSettings.HomeDirectoryMapPath
                    End If
                End If
                Return _RootFolder
            End Get
        End Property

        Public ReadOnly Property UploadRoles() As String
            Get
                If _UploadRoles Is Nothing Then
                    _UploadRoles = String.Empty

                    Dim objModules As New ModuleController
                    'TODO:  Should replace this with a finder method in PortalSettings to look in the cached modules of the activetab - jmb 11/25/2004
                    Dim ModInfo As ModuleInfo

                    If IsHostMenu Then
                        ModInfo = objModules.GetModuleByDefinition(Null.NullInteger, "File Manager")
                    Else
                        ModInfo = objModules.GetModuleByDefinition(PortalId, "File Manager")
                    End If

                    Dim settings As Hashtable = New ModuleController().GetModuleSettings(ModInfo.ModuleID)
                    If Not CType(settings("uploadroles"), String) Is Nothing Then
                        _UploadRoles = CType(settings("uploadroles"), String)
                    End If
                End If

                Return _UploadRoles
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine checks the Access Security
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 1/21/2005  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CheckSecurity()
            If Not IsEditable Then
                Response.Redirect(NavigateURL("Access Denied"), True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine populates the Folder List Drop Down
        ''' There is no reference to permissions here as all folders should be available to the admin.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [Philip Beadle]     5/10/2004  Added
        '''     [cnurse]            04/24/2006  Converted to use Database as folder source
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LoadFolders()

            ddlFolders.Items.Clear()

            Dim folders As ArrayList = FileSystemUtils.GetFoldersByUser(FolderPortalID, True, True, "ADD")
            For Each folder As FolderInfo In folders
                Dim FolderItem As New ListItem
                If folder.FolderPath = Null.NullString Then
                    If IsHostMenu Then
                        FolderItem.Text = Localization.GetString("HostRoot", Me.LocalResourceFile)
                    Else
                        FolderItem.Text = Localization.GetString("PortalRoot", Me.LocalResourceFile)
                    End If
                Else
                    FolderItem.Text = FileSystemUtils.RemoveTrailingSlash(folder.DisplayPath)
                End If
                FolderItem.Value = folder.FolderPath
                ddlFolders.Items.Add(FolderItem)
            Next

            If DestinationFolder.Length > 0 Then
                If Not ddlFolders.Items.FindByText(DestinationFolder) Is Nothing Then
                    ddlFolders.Items.FindByText(DestinationFolder).Selected = True
                End If
            End If
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine determines the Return Url
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 1/21/2005  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ReturnURL() As String
            Dim TabID As Integer = PortalSettings.HomeTabId

            If Not Request.Params("rtab") Is Nothing Then
                TabID = Integer.Parse(Request.Params("rtab"))
            End If
            Return NavigateURL(TabID)
        End Function

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.

            'Customise the Control Title
            Me.ModuleConfiguration.ModuleTitle = Services.Localization.Localization.GetString("UploadType" & FileType.ToString, Me.LocalResourceFile)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Page_Load runs when the page loads
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 16/9/2004  Updated for localization, Help and 508
        '''   [VMasanas]  9/28/2004   Changed redirect to Access Denied
        '''   [Philip Beadle]  5/10/2004  Added folder population section.
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                CheckSecurity()

                'Get localized Strings
                Dim strHost As String = Localization.GetString("HostRoot", Me.LocalResourceFile)
                Dim strPortal As String = Localization.GetString("PortalRoot", Me.LocalResourceFile)

                If Not Page.IsPostBack Then
                    cmdAdd.Text = Services.Localization.Localization.GetString("UploadType" & FileType.ToString, Me.LocalResourceFile)
                    If FileType = UploadType.File Then
                        trFolders.Visible = True
                        trRoot.Visible = True
                        trUnzip.Visible = True

                        If IsHostMenu Then
                            lblRootType.Text = strHost & ":"
                            lblRootFolder.Text = RootFolder
                        Else
                            lblRootType.Text = strPortal & ":"
                            lblRootFolder.Text = RootFolder
                        End If
                        LoadFolders()
                    End If

                    chkUnzip.Checked = False
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The cmdAdd_Click runs when the Add Button is clicked
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 16/9/2004  Updated for localization, Help and 508
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAdd.Click
            Try
                Dim strFileName As String
                Dim strExtension As String = ""
                Dim strMessage As String = ""

                Dim postedFile As HttpPostedFile = cmdBrowse.PostedFile

                'Get localized Strings
                Dim strInvalid As String = Services.Localization.Localization.GetString("InvalidExt", Me.LocalResourceFile)

                strFileName = System.IO.Path.GetFileName(postedFile.FileName)
                strExtension = Path.GetExtension(strFileName)

                If postedFile.FileName <> "" Then
                    Select Case FileType
                        Case UploadType.File        ' content files
                            strMessage += FileSystemUtils.UploadFile(RootFolder & ddlFolders.SelectedItem.Value.Replace("/", "\"), postedFile, chkUnzip.Checked)
                        Case UploadType.Skin        ' skin package
                            If strExtension.ToLower = ".zip" Then
                                Dim objSkins As New UI.Skins.SkinController
                                Dim objLbl As New Label
                                objLbl.CssClass = "Normal"
                                objLbl.Text = Skins.SkinController.UploadLegacySkin(RootFolder, SkinController.RootSkin, Path.GetFileNameWithoutExtension(postedFile.FileName), postedFile.InputStream)
                                phPaLogs.Controls.Add(objLbl)
                            Else
                                strMessage += strInvalid & " " & FileTypeName & " " & strFileName
                            End If
                        Case UploadType.Container        ' container package
                            If strExtension.ToLower = ".zip" Then
                                Dim objSkins As New UI.Skins.SkinController
                                Dim objLbl As New Label
                                objLbl.CssClass = "Normal"
                                objLbl.Text = Skins.SkinController.UploadLegacySkin(RootFolder, SkinController.RootContainer, Path.GetFileNameWithoutExtension(postedFile.FileName), postedFile.InputStream)
                                phPaLogs.Controls.Add(objLbl)
                            Else
                                strMessage += strInvalid & " " & FileTypeName & " " & strFileName
                            End If
                    End Select
                Else
                    strMessage = Services.Localization.Localization.GetString("NoFile", Me.LocalResourceFile)
                End If

                If phPaLogs.Controls.Count > 0 Then
                    tblLogs.Visible = True
                ElseIf strMessage = "" Then
                    Response.Redirect(ReturnURL(), True)
                Else
                    lblMessage.Text = strMessage
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The cmdReturn_Click runs when the Return Button is clicked
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 16/9/2004  Updated for localization, Help and 508
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdReturn1.Click, cmdReturn2.Click
            Response.Redirect(ReturnURL(), True)
        End Sub

#End Region

    End Class

End Namespace
