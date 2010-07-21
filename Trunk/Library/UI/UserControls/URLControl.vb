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
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.UI.UserControls

    Public MustInherit Class UrlControl

        Inherits Framework.UserControlBase

#Region "Controls"

        Protected WithEvents TypeRow As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents optType As System.Web.UI.WebControls.RadioButtonList

        Protected WithEvents URLRow As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents lblURL As System.Web.UI.WebControls.Label
        Protected WithEvents cboUrls As System.Web.UI.WebControls.DropDownList
        Protected WithEvents txtUrl As System.Web.UI.WebControls.TextBox
        Protected WithEvents cmdSelect As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdAdd As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdDelete As System.Web.UI.WebControls.LinkButton
        Protected WithEvents lblURLType As System.Web.UI.WebControls.Label

        Protected WithEvents ImagesRow As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents lblImages As System.Web.UI.WebControls.Label
        Protected WithEvents cboImages As System.Web.UI.WebControls.DropDownList

        Protected WithEvents TabRow As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents lblTab As System.Web.UI.WebControls.Label
        Protected WithEvents cboTabs As System.Web.UI.WebControls.DropDownList

        Protected WithEvents FileRow As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents lblFolder As System.Web.UI.WebControls.Label
        Protected WithEvents cboFolders As System.Web.UI.WebControls.DropDownList
        Protected WithEvents lblFile As System.Web.UI.WebControls.Label
        Protected WithEvents cboFiles As System.Web.UI.WebControls.DropDownList
        Protected WithEvents txtFile As System.Web.UI.HtmlControls.HtmlInputFile
        Protected WithEvents cmdUpload As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdSave As System.Web.UI.WebControls.LinkButton
        Protected WithEvents cmdCancel As System.Web.UI.WebControls.LinkButton
        Protected WithEvents imgStorageLocationType As System.Web.UI.WebControls.Image

        Protected WithEvents UserRow As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents lblUser As System.Web.UI.WebControls.Label
        Protected WithEvents txtUser As System.Web.UI.WebControls.TextBox

        Protected WithEvents ErrorRow As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents lblMessage As System.Web.UI.WebControls.Label

        Protected WithEvents chkNewWindow As System.Web.UI.WebControls.CheckBox
        Protected WithEvents chkTrack As System.Web.UI.WebControls.CheckBox
        Protected WithEvents chkLog As System.Web.UI.WebControls.CheckBox

#End Region

#Region "Private Members"

        Private _localResourceFile As String
        Private _objPortal As PortalInfo

        ' The following flags are defined to alow the control to flow
        Private _doChangeURL As Boolean = False
        Private _doRenderTypes As Boolean = False
        Private _doRenderTypeControls As Boolean = False
        Private _doRenderTrackingOptions As Boolean = False

        Private _doReloadFolders As Boolean = False
        Private _doReloadFiles As Boolean = False

#End Region

#Region "Public Properties"

        Public Property FileFilter() As String
            Get
                If Not ViewState("FileFilter") Is Nothing Then
                    Return CType(ViewState("FileFilter"), String)
                Else
                    Return ""
                End If
            End Get
            Set(ByVal Value As String)
                ViewState("FileFilter") = Value
                If IsTrackingViewState Then
                    _doReloadFiles = True
                End If
            End Set
        End Property

        Public Property IncludeActiveTab() As Boolean
            Get
                If Not ViewState("IncludeActiveTab") Is Nothing Then
                    Return CType(ViewState("IncludeActiveTab"), Boolean)
                Else
                    Return False ' Set as default
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("IncludeActiveTab") = Value
                If IsTrackingViewState Then
                    _doRenderTypeControls = True
                End If
            End Set
        End Property

        Public Property LocalResourceFile() As String
            Get
                Dim fileRoot As String

                If _localResourceFile = "" Then
                    fileRoot = Me.TemplateSourceDirectory & "/" & Localization.LocalResourceDirectory & "/URLControl.ascx"
                Else
                    fileRoot = _localResourceFile
                End If
                Return fileRoot
            End Get
            Set(ByVal Value As String)
                _localResourceFile = Value
            End Set
        End Property

        Public ReadOnly Property Log() As Boolean
            Get
                If chkLog.Visible = True Then
                    Log = chkLog.Checked
                Else
                    Log = False
                End If
            End Get
        End Property

        Public Property ModuleID() As Integer
            Get
                Dim myMid As Integer = -2
                If Not ViewState("ModuleId") Is Nothing Then
                    myMid = Convert.ToInt32(ViewState("ModuleId"))
                ElseIf Not Request.QueryString("mid") Is Nothing Then
                    Integer.TryParse(Request.QueryString("mid"), myMid)
                End If
                Return myMid
            End Get
            Set(ByVal value As Integer)
                ViewState("ModuleId") = value
            End Set
        End Property

        Public ReadOnly Property NewWindow() As Boolean
            Get
                If chkNewWindow.Visible = True Then
                    Return chkNewWindow.Checked
                Else
                    Return False
                End If
            End Get
        End Property

        Public Property Required() As Boolean
            Get
                If Not ViewState("Required") Is Nothing Then
                    Return CType(ViewState("Required"), Boolean)
                Else
                    Return True ' Set as default in the old variable
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("Required") = Value
                If IsTrackingViewState Then
                    _doRenderTypeControls = True
                End If
            End Set
        End Property

        Public Property ShowFiles() As Boolean
            Get
                If Not ViewState("ShowFiles") Is Nothing Then
                    Return CType(ViewState("ShowFiles"), Boolean)
                Else
                    Return True ' Set as default in the old variable
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("ShowFiles") = Value
                If IsTrackingViewState Then
                    _doRenderTypes = True
                End If
            End Set
        End Property

        Public Property ShowImages() As Boolean
            Get
                If Not ViewState("ShowImages") Is Nothing Then
                    Return CType(ViewState("ShowImages"), Boolean)
                Else
                    Return False
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("ShowImages") = Value
                If IsTrackingViewState Then
                    _doRenderTypes = True
                End If
            End Set
        End Property

        Public Property ShowLog() As Boolean
            Get
                Return chkLog.Visible
            End Get
            Set(ByVal Value As Boolean)
                chkLog.Visible = Value
            End Set
        End Property

        Public Property ShowNewWindow() As Boolean
            Get
                Return chkNewWindow.Visible
            End Get
            Set(ByVal Value As Boolean)
                chkNewWindow.Visible = Value
            End Set
        End Property

        Public Property ShowNone() As Boolean
            Get
                If Not ViewState("ShowNone") Is Nothing Then
                    Return CType(ViewState("ShowNone"), Boolean)
                Else
                    Return False ' Set as default in the old variable
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("ShowNone") = Value
                If IsTrackingViewState Then
                    _doRenderTypes = True
                End If
            End Set
        End Property

        Public Property ShowSecure() As Boolean
            Get
                If Not ViewState("ShowSecure") Is Nothing Then
                    Return CType(ViewState("ShowSecure"), Boolean)
                Else
                    Return True ' Set as default in the old variable
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("ShowSecure") = Value
                If IsTrackingViewState Then
                    _doReloadFolders = True
                End If
            End Set
        End Property

        Public Property ShowDatabase() As Boolean
            Get
                If Not ViewState("ShowDatabase") Is Nothing Then
                    Return CType(ViewState("ShowDatabase"), Boolean)
                Else
                    Return True ' Set as default in the old variable
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("ShowDatabase") = Value
                If IsTrackingViewState Then
                    _doReloadFolders = True
                End If
            End Set
        End Property

        Public Property ShowTabs() As Boolean
            Get
                If Not ViewState("ShowTabs") Is Nothing Then
                    Return CType(ViewState("ShowTabs"), Boolean)
                Else
                    Return True ' Set as default in the old variable
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("ShowTabs") = Value
                If IsTrackingViewState Then
                    _doRenderTypes = True
                End If
            End Set
        End Property

        Public Property ShowTrack() As Boolean
            Get
                Return chkTrack.Visible
            End Get
            Set(ByVal Value As Boolean)
                chkTrack.Visible = Value
            End Set
        End Property

        Public Property ShowUpLoad() As Boolean
            Get
                If Not ViewState("ShowUpLoad") Is Nothing Then
                    Return CType(ViewState("ShowUpLoad"), Boolean)
                Else
                    Return True ' Set as default in the old variable
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("ShowUpLoad") = Value
                If IsTrackingViewState Then
                    _doRenderTypeControls = True
                End If
            End Set
        End Property

        Public Property ShowUrls() As Boolean
            Get
                If Not ViewState("ShowUrls") Is Nothing Then
                    Return CType(ViewState("ShowUrls"), Boolean)
                Else
                    Return True ' Set as default in the old variable
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("ShowUrls") = Value
                If IsTrackingViewState Then
                    _doRenderTypes = True
                End If
            End Set
        End Property

        Public Property ShowUsers() As Boolean
            Get
                If Not ViewState("ShowUsers") Is Nothing Then
                    Return CType(ViewState("ShowUsers"), Boolean)
                Else
                    Return False ' Set as default in the old variable
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("ShowUsers") = Value
                If IsTrackingViewState Then
                    _doRenderTypes = True
                End If
            End Set
        End Property

        Public ReadOnly Property Track() As Boolean
            Get
                If chkTrack.Visible = True Then
                    Track = chkTrack.Checked
                Else
                    Track = False
                End If
            End Get
        End Property

        Public Property Url() As String
            Get
                Dim r As String = ""
                Dim strCurrentType As String = ""
                If optType.Items.Count > 0 AndAlso optType.SelectedIndex >= 0 Then
                    strCurrentType = optType.SelectedItem.Value
                End If
                Select Case strCurrentType
                    Case "I"
                        If Not cboImages.SelectedItem Is Nothing Then
                            r = cboImages.SelectedItem.Value
                        End If
                    Case "U"
                        If cboUrls.Visible Then
                            If Not cboUrls.SelectedItem Is Nothing Then
                                r = cboUrls.SelectedItem.Value
                                txtUrl.Text = r
                            End If
                        Else
                            Dim mCustomUrl As String = txtUrl.Text
                            If mCustomUrl.ToLower = "http://" Then
                                r = ""
                            Else
                                r = AddHTTP(mCustomUrl)
                            End If
                        End If
                    Case "T"
                        Dim strTab As String = ""
                        If Not cboTabs.SelectedItem Is Nothing Then
                            strTab = cboTabs.SelectedItem.Value
                            If IsNumeric(strTab) AndAlso (CInt(strTab) >= 0) Then
                                r = strTab
                            End If
                        End If
                    Case "F"
                        If Not cboFiles.SelectedItem Is Nothing Then
                            If Not cboFiles.SelectedItem.Value = "" Then
                                r = "FileID=" & cboFiles.SelectedItem.Value
                            Else
                                r = ""
                            End If
                        End If
                    Case "M"
                        If txtUser.Text <> "" Then
                            Dim objUser As UserInfo = UserController.GetCachedUser(_objPortal.PortalID, txtUser.Text)
                            If Not objUser Is Nothing Then
                                r = "UserID=" & objUser.UserID.ToString
                            Else
                                lblMessage.Text = Localization.GetString("NoUser", Me.LocalResourceFile)
                                txtUser.Text = ""
                            End If
                        End If
                End Select
                Return r
            End Get
            Set(ByVal Value As String)
                ViewState("Url") = Value
                If IsTrackingViewState Then
                    _doChangeURL = True
                    _doReloadFiles = True
                End If
            End Set
        End Property

        Public Property UrlType() As String
            Get
                Return Convert.ToString(ViewState("UrlType"))
            End Get
            Set(ByVal Value As String)
                If Not Value Is Nothing AndAlso Trim(Value) <> "" Then
                    ViewState("UrlType") = Value
                    If IsTrackingViewState Then
                        _doChangeURL = True
                    End If
                End If
            End Set
        End Property

        Public Property Width() As String
            Get
                Width = Convert.ToString(ViewState("SkinControlWidth"))
            End Get
            Set(ByVal Value As String)
                If Value <> "" Then
                    cboUrls.Width = System.Web.UI.WebControls.Unit.Parse(Value)
                    txtUrl.Width = System.Web.UI.WebControls.Unit.Parse(Value)
                    cboImages.Width = System.Web.UI.WebControls.Unit.Parse(Value)
                    cboTabs.Width = System.Web.UI.WebControls.Unit.Parse(Value)
                    cboFolders.Width = System.Web.UI.WebControls.Unit.Parse(Value)
                    cboFiles.Width = System.Web.UI.WebControls.Unit.Parse(Value)
                    txtUser.Width = System.Web.UI.WebControls.Unit.Parse(Value)
                    ViewState("SkinControlWidth") = Value
                End If
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Function GetFileList(ByVal NoneSpecified As Boolean) As ArrayList
            Dim PortalId As Integer = Null.NullInteger

            If (Not IsHostMenu) OrElse (Request.QueryString("pid") IsNot Nothing) Then
                PortalId = _objPortal.PortalID
            End If

            Return Common.Globals.GetFileList(PortalId, FileFilter, NoneSpecified, cboFolders.SelectedItem.Value, False)
        End Function

        Private Sub LoadFolders(ByVal Permissions As String)
            Dim PortalId As Integer = Null.NullInteger
            cboFolders.Items.Clear()

            If (Not IsHostMenu) OrElse (Request.QueryString("pid") IsNot Nothing) Then
                PortalId = _objPortal.PortalID
            End If

            Dim folders As ArrayList = FileSystemUtils.GetFoldersByUser(PortalId, ShowSecure, ShowDatabase, Permissions)
            For Each folder As FolderInfo In folders
                Dim FolderItem As New ListItem
                If folder.FolderPath = Null.NullString Then
                    FolderItem.Text = Localization.GetString("Root", Me.LocalResourceFile)
                Else
                    FolderItem.Text = folder.FolderPath
                End If
                FolderItem.Value = folder.FolderPath
                cboFolders.Items.Add(FolderItem)
            Next
        End Sub

        Private Sub LoadUrls()
            Dim objUrls As New UrlController
            cboUrls.Items.Clear()
            cboUrls.DataSource = objUrls.GetUrls(_objPortal.PortalID)
            cboUrls.DataBind()
        End Sub

        Private Sub SetStorageLocationType()
            Dim objFolder As New FolderController
            Dim objFolderInfo As New FolderInfo
            Dim FolderName As String = cboFolders.SelectedValue

            ' Check to see if this is the 'Root' folder, if so we cannot rely on its text value because it is something and not an empty string that we need to lookup the 'root' folder
            If cboFolders.SelectedValue = String.Empty Then
                FolderName = ""
            End If

            objFolderInfo = objFolder.GetFolder(PortalSettings.PortalId, FolderName, False)

            If Not objFolderInfo Is Nothing Then
                Select Case objFolderInfo.StorageLocation
                    Case FolderController.StorageLocationTypes.InsecureFileSystem
                        imgStorageLocationType.Visible = False
                    Case FolderController.StorageLocationTypes.SecureFileSystem
                        imgStorageLocationType.ImageUrl = ResolveUrl("~/images/icon_securityroles_16px.gif")
                        imgStorageLocationType.Visible = True
                    Case FolderController.StorageLocationTypes.DatabaseSecure
                        imgStorageLocationType.ImageUrl = ResolveUrl("~/images/icon_sql_16px.gif")
                        imgStorageLocationType.Visible = True
                End Select
            End If

        End Sub

        Private Sub DoChangeURL()
            Dim _Url As String = Convert.ToString(ViewState("Url"))
            Dim _Urltype As String = Convert.ToString(ViewState("UrlType"))
            If _Url <> "" Then
                Dim objUrls As New UrlController
                Dim TrackingUrl As String = _Url

                _Urltype = GetURLType(_Url).ToString("g").Substring(0, 1)
                If _Urltype = "U" AndAlso (_Url.StartsWith("~/images/")) Then
                    _Urltype = "I"
                End If
                ViewState("UrlType") = _Urltype
                If _Urltype = "F" Then
                    Dim objFiles As New FileController
                    If _Url.ToLower.StartsWith("fileid=") Then
                        TrackingUrl = _Url
                        Dim objFile As DotNetNuke.Services.FileSystem.FileInfo = objFiles.GetFileById(Integer.Parse(_Url.Substring(7)), _objPortal.PortalID)
                        If Not objFile Is Nothing Then
                            _Url = objFile.Folder & objFile.FileName
                        End If
                    Else
                        ' to handle legacy scenarios before the introduction of the FileServerHandler
                        TrackingUrl = "FileID=" & objFiles.ConvertFilePathToFileId(_Url, _objPortal.PortalID).ToString
                    End If
                End If

                If _Urltype = "M" Then
                    If _Url.ToLower.StartsWith("userid=") Then
                        Dim objUser As UserInfo = UserController.GetUserById(_objPortal.PortalID, Integer.Parse(_Url.Substring(7)))
                        If Not objUser Is Nothing Then
                            _Url = objUser.Username
                        End If
                    End If
                End If

                Dim objUrlTracking As UrlTrackingInfo = objUrls.GetUrlTracking(_objPortal.PortalID, TrackingUrl, ModuleID)
                If Not objUrlTracking Is Nothing Then
                    chkNewWindow.Checked = objUrlTracking.NewWindow
                    chkTrack.Checked = objUrlTracking.TrackClicks
                    chkLog.Checked = objUrlTracking.LogActivity
                Else       ' the url does not exist in the tracking table
                    chkNewWindow.Checked = False 'Need check
                    chkTrack.Checked = False 'Need check
                    chkLog.Checked = False 'Need check
                End If
                ViewState("Url") = _Url
            Else
                If _Urltype <> "" Then
                    optType.ClearSelection()
                    If Not optType.Items.FindByValue(_Urltype) Is Nothing Then
                        optType.Items.FindByValue(_Urltype).Selected = True
                    Else
                        optType.Items(0).Selected = True
                    End If
                Else
                    If optType.Items.Count > 0 Then
                        optType.ClearSelection()
                        optType.Items(0).Selected = True
                    End If
                End If
                chkNewWindow.Checked = False 'Need check
                chkTrack.Checked = False 'Need check
                chkLog.Checked = False 'Need check
            End If

            'Url type changed, then we must draw the controlos for that type
            _doRenderTypeControls = True
        End Sub

        Private Sub DoRenderTypes()
            ' We must clear the list to keep the same item order
            Dim strCurrent As String = ""
            If optType.SelectedIndex >= 0 Then
                strCurrent = optType.SelectedItem.Value ' Save current selected value
            End If
            optType.Items.Clear()
            If ShowNone Then
                If optType.Items.FindByValue("N") Is Nothing Then
                    optType.Items.Add(New ListItem(Localization.GetString("NoneType", LocalResourceFile), "N"))
                End If
            Else
                If Not optType.Items.FindByValue("N") Is Nothing Then
                    optType.Items.Remove(optType.Items.FindByValue("N"))
                End If
            End If
            If ShowUrls Then
                If optType.Items.FindByValue("U") Is Nothing Then
                    optType.Items.Add(New ListItem(Localization.GetString("URLType", LocalResourceFile), "U"))
                End If
            Else
                If Not optType.Items.FindByValue("U") Is Nothing Then
                    optType.Items.Remove(optType.Items.FindByValue("U"))
                End If
            End If
            If ShowTabs Then
                If optType.Items.FindByValue("T") Is Nothing Then
                    optType.Items.Add(New ListItem(Localization.GetString("TabType", LocalResourceFile), "T"))
                End If
            Else
                If Not optType.Items.FindByValue("T") Is Nothing Then
                    optType.Items.Remove(optType.Items.FindByValue("T"))
                End If
            End If
            If ShowFiles Then
                If optType.Items.FindByValue("F") Is Nothing Then
                    optType.Items.Add(New ListItem(Localization.GetString("FileType", LocalResourceFile), "F"))
                End If
            Else
                If Not optType.Items.FindByValue("F") Is Nothing Then
                    optType.Items.Remove(optType.Items.FindByValue("F"))
                End If
            End If
            If ShowImages Then
                If optType.Items.FindByValue("I") Is Nothing Then
                    optType.Items.Add(New ListItem(Localization.GetString("ImageType", LocalResourceFile), "I"))
                End If
            Else
                If Not optType.Items.FindByValue("I") Is Nothing Then
                    optType.Items.Remove(optType.Items.FindByValue("I"))
                End If
            End If
            If ShowUsers Then
                If optType.Items.FindByValue("M") Is Nothing Then
                    optType.Items.Add(New ListItem(Localization.GetString("UserType", LocalResourceFile), "M"))
                End If
            Else
                If Not optType.Items.FindByValue("M") Is Nothing Then
                    optType.Items.Remove(optType.Items.FindByValue("M"))
                End If
            End If
            If optType.Items.Count > 0 Then
                If strCurrent <> "" Then
                    If Not optType.Items.FindByValue(strCurrent) Is Nothing Then
                        optType.Items.FindByValue(strCurrent).Selected = True
                    Else
                        optType.Items(0).Selected = True
                        _doRenderTypeControls = True 'Type changed, re-draw
                    End If
                Else
                    optType.Items(0).Selected = True
                    _doRenderTypeControls = True 'Type changed, re-draw
                End If
                TypeRow.Visible = optType.Items.Count > 1
            Else
                TypeRow.Visible = False
            End If
        End Sub

        Private Sub DoCorrectRadioButtonList()
            Dim _Urltype As String = Convert.ToString(ViewState("UrlType"))

            If optType.Items.Count > 0 Then
                optType.ClearSelection()
                If _Urltype <> "" Then
                    If Not optType.Items.FindByValue(_Urltype) Is Nothing Then
                        optType.Items.FindByValue(_Urltype).Selected = True
                    Else
                        optType.Items(0).Selected = True
                        _Urltype = optType.Items(0).Value
                        ViewState("UrlType") = _Urltype
                    End If
                Else
                    optType.Items(0).Selected = True
                    _Urltype = optType.Items(0).Value
                    ViewState("UrlType") = _Urltype
                End If
            End If
        End Sub

        Private Sub DoRenderTypeControls()
            Dim _Url As String = Convert.ToString(ViewState("Url"))
            Dim _Urltype As String = Convert.ToString(ViewState("UrlType"))
            Dim objUrls As New UrlController

            If _Urltype <> "" Then
                ' load listitems
                Select Case optType.SelectedItem.Value
                    Case "N"    ' None
                        URLRow.Visible = False
                        TabRow.Visible = False
                        FileRow.Visible = False
                        UserRow.Visible = False
                        ImagesRow.Visible = False
                    Case "I"    ' System Image
                        URLRow.Visible = False
                        TabRow.Visible = False
                        FileRow.Visible = False
                        UserRow.Visible = False
                        ImagesRow.Visible = True

                        cboImages.Items.Clear()

                        Dim strImagesFolder As String = Path.Combine(ApplicationMapPath, "images\")
                        For Each strImage As String In Directory.GetFiles(strImagesFolder)
                            strImage = strImage.Replace(strImagesFolder, "")
                            cboImages.Items.Add(New ListItem(strImage, String.Format("~/images/{0}", strImage)))
                        Next

                        If Not cboImages.Items.FindByValue(_Url) Is Nothing Then
                            cboImages.Items.FindByValue(_Url).Selected = True
                        End If
                    Case "U"    ' Url
                        URLRow.Visible = True
                        TabRow.Visible = False
                        FileRow.Visible = False
                        UserRow.Visible = False
                        ImagesRow.Visible = False

                        If txtUrl.Text = "" Then
                            txtUrl.Text = _Url
                        End If
                        If txtUrl.Text = "" Then
                            txtUrl.Text = "http://"
                        End If
                        txtUrl.Visible = True

                        cmdSelect.Visible = True

                        cboUrls.Visible = False
                        cmdAdd.Visible = False
                        cmdDelete.Visible = False
                    Case "T"    ' tab
                        URLRow.Visible = False
                        TabRow.Visible = True
                        FileRow.Visible = False
                        UserRow.Visible = False
                        ImagesRow.Visible = False

                        cboTabs.Items.Clear()

                        Dim _settings As PortalSettings = PortalController.GetCurrentPortalSettings()
                        Dim excludeTabId As Integer = Null.NullInteger
                        If Not IncludeActiveTab Then
                            excludeTabId = _settings.ActiveTab.TabID
                        End If
                        cboTabs.DataSource = TabController.GetPortalTabs(_settings.PortalId, excludeTabId, Not Required, "none available", True, False, False, True, False)
                        cboTabs.DataBind()
                        If Not cboTabs.Items.FindByValue(_Url) Is Nothing Then
                            cboTabs.Items.FindByValue(_Url).Selected = True
                        End If
                    Case "F"    ' file
                        URLRow.Visible = False
                        TabRow.Visible = False
                        FileRow.Visible = True
                        UserRow.Visible = False
                        ImagesRow.Visible = False

                        If ViewState("FoldersLoaded") Is Nothing Or Me._doReloadFolders Then
                            LoadFolders("BROWSE,ADD")
                            ViewState("FoldersLoaded") = "Y"
                        End If

                        If cboFolders.Items.Count = 0 Then
                            lblMessage.Text = Localization.GetString("NoPermission", Me.LocalResourceFile)
                            FileRow.Visible = False
                            Exit Sub
                        End If

                        ' select folder
                        ' We Must check if selected folder has changed because of a property change (Secure, Database)
                        Dim FileName As String = String.Empty
                        Dim FolderPath As String = String.Empty
                        Dim LastFileName As String = String.Empty
                        Dim LastFolderPath As String = String.Empty
                        Dim _MustRedrawFiles As Boolean = False
                        'Let's try to remember last selection
                        If Not ViewState("LastFolderPath") Is Nothing Then
                            LastFolderPath = Convert.ToString(ViewState("LastFolderPath"))
                        End If
                        If Not ViewState("LastFileName") Is Nothing Then
                            LastFileName = Convert.ToString(ViewState("LastFileName"))
                        End If
                        If Not _Url = String.Empty Then
                            'Let's use the new URL
                            FileName = _Url.Substring(_Url.LastIndexOf("/") + 1)
                            FolderPath = _Url.Replace(FileName, "")
                        Else
                            'Use last settings
                            FileName = LastFileName
                            FolderPath = LastFolderPath
                        End If

                        If Not cboFolders.Items.FindByValue(FolderPath) Is Nothing Then
                            cboFolders.ClearSelection()
                            cboFolders.Items.FindByValue(FolderPath).Selected = True
                        ElseIf cboFolders.Items.Count > 0 Then
                            cboFolders.ClearSelection()
                            cboFolders.Items(0).Selected = True
                            FolderPath = cboFolders.Items(0).Value
                        End If

                        If ViewState("FilesLoaded") Is Nothing Or FolderPath <> LastFolderPath Or Me._doReloadFiles Then
                            'Reload files only if property change or not same folder
                            _MustRedrawFiles = True
                            ViewState("FilesLoaded") = "Y"
                        Else
                            If cboFiles.Items.Count > 0 Then
                                If (Required And cboFiles.Items(0).Value = "") Or (Not Required And cboFiles.Items(0).Value <> "") Then
                                    'Required state has changed, so we need to reload files
                                    _MustRedrawFiles = True
                                End If
                            ElseIf Not Required Then
                                'Required state has changed, so we need to reload files
                                _MustRedrawFiles = True
                            End If
                        End If

                        If _MustRedrawFiles Then
                            cboFiles.DataSource = GetFileList(Not Required)
                            cboFiles.DataBind()
                            If Not cboFiles.Items.FindByText(FileName) Is Nothing Then
                                cboFiles.ClearSelection()
                                cboFiles.Items.FindByText(FileName).Selected = True
                            End If
                        End If

                        cboFiles.Visible = True
                        txtFile.Visible = False

                        Dim objFolders As New FolderController
                        Dim objFolder As FolderInfo = objFolders.GetFolder(_objPortal.PortalID, FolderPath, False)
                        cmdUpload.Visible = ShowUpLoad AndAlso FolderPermissionController.CanAddFolder(objFolder)

                        SetStorageLocationType()
                        txtUrl.Visible = False
                        cmdSave.Visible = False
                        cmdCancel.Visible = False

                        If cboFolders.SelectedIndex >= 0 Then
                            ViewState("LastFolderPath") = cboFolders.SelectedValue
                        Else
                            ViewState("LastFolderPath") = ""
                        End If
                        If cboFiles.SelectedIndex >= 0 Then
                            ViewState("LastFileName") = cboFiles.SelectedValue
                        Else
                            ViewState("LastFileName") = ""
                        End If

                    Case "M"    ' membership users
                        URLRow.Visible = False
                        TabRow.Visible = False
                        FileRow.Visible = False
                        UserRow.Visible = True
                        ImagesRow.Visible = False

                        If txtUser.Text = "" Then
                            txtUser.Text = _Url
                        End If
                End Select
            Else
                URLRow.Visible = False
                ImagesRow.Visible = False
                TabRow.Visible = False
                FileRow.Visible = False
                UserRow.Visible = False
            End If
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Framework.AJAX.RegisterPostBackControl(Me.FindControl("cmdSave"))

            ' prevent unauthorized access
            If Request.IsAuthenticated = False Then
                Me.Visible = False
            End If
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                Dim objPortals As New PortalController
                If Not (Request.QueryString("pid") Is Nothing) AndAlso (PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId OrElse UserController.GetCurrentUserInfo.IsSuperUser) Then
                    _objPortal = objPortals.GetPortal(Int32.Parse(Request.QueryString("pid")))
                Else
                    _objPortal = objPortals.GetPortal(PortalSettings.PortalId)
                End If

                If ViewState("IsUrlControlLoaded") Is Nothing Then
                    'If Not Page.IsPostBack Then
                    'let's make at least an initialization
                    'The type radio button must be initialized
                    'The url must be initialized no matter its value
                    _doRenderTypes = True
                    _doChangeURL = True

                    ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItem"))

                    'The following line was mover to the pre-render event to ensure render for the first time
                    'ViewState("IsUrlControlLoaded") = "Loaded"
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Try
                If _doRenderTypes Then
                    DoRenderTypes()
                End If
                If _doChangeURL Then
                    DoChangeURL()
                End If
                If _doReloadFolders Or _doReloadFiles Then
                    DoCorrectRadioButtonList()
                    _doRenderTypeControls = True
                End If

                If _doRenderTypeControls Then
                    If Not (_doReloadFolders Or _doReloadFiles) Then
                        DoCorrectRadioButtonList()
                    End If
                    DoRenderTypeControls()
                End If
                ViewState("Url") = Nothing
                ViewState("IsUrlControlLoaded") = "Loaded"
            Catch exc As Exception
                ' Let's detect possible problems
                LogException(New Exception("Error rendering URLControl subcontrols.", exc))
            End Try
        End Sub

        Private Sub cboFolders_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboFolders.SelectedIndexChanged
            Dim objFolders As New FolderController
            Dim PortalId As Integer = Null.NullInteger

            If (Not IsHostMenu) OrElse (Request.QueryString("pid") IsNot Nothing) Then
                PortalId = _objPortal.PortalID
            End If
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, cboFolders.SelectedValue, False)
            If FolderPermissionController.CanAddFolder(objFolder) Then
                If Not txtFile.Visible Then
                    cmdSave.Visible = False
                    ' only show if not already in upload mode and not disabled
                    cmdUpload.Visible = ShowUpLoad
                End If
            Else
                'reset controls
                cboFiles.Visible = True
                cmdUpload.Visible = False
                txtFile.Visible = False
                cmdSave.Visible = False
                cmdCancel.Visible = False
            End If

            cboFiles.Items.Clear()
            cboFiles.DataSource = GetFileList(Not Required)
            cboFiles.DataBind()

            SetStorageLocationType()

            If cboFolders.SelectedIndex >= 0 Then
                ViewState("LastFolderPath") = cboFolders.SelectedValue
            Else
                ViewState("LastFolderPath") = ""
            End If
            If cboFiles.SelectedIndex >= 0 Then
                ViewState("LastFileName") = cboFiles.SelectedValue
            Else
                ViewState("LastFileName") = ""
            End If


            _doRenderTypeControls = False 'Must not render on this postback
            _doRenderTypes = False
            _doChangeURL = False
            _doReloadFolders = False
            _doReloadFiles = False
        End Sub

        Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAdd.Click
            cboUrls.Visible = False
            cmdSelect.Visible = True
            txtUrl.Visible = True
            cmdAdd.Visible = False
            cmdDelete.Visible = False

            _doRenderTypeControls = False 'Must not render on this postback
            _doRenderTypes = False
            _doChangeURL = False
            _doReloadFolders = False
            _doReloadFiles = False
        End Sub

        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            cboFiles.Visible = True
            cmdUpload.Visible = True
            txtFile.Visible = False
            cmdSave.Visible = False
            cmdCancel.Visible = False

            _doRenderTypeControls = False 'Must not render on this postback
            _doRenderTypes = False
            _doChangeURL = False
            _doReloadFolders = False
            _doReloadFiles = False
        End Sub

        Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
            If Not cboUrls.SelectedItem Is Nothing Then

                Dim objUrls As New UrlController
                objUrls.DeleteUrl(_objPortal.PortalID, cboUrls.SelectedItem.Value)

                LoadUrls() 'we must reload the url list
            End If

            _doRenderTypeControls = False 'Must not render on this postback
            _doRenderTypes = False
            _doChangeURL = False
            _doReloadFolders = False
            _doReloadFiles = False
        End Sub

        Private Sub cmdSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSave.Click

            cmdUpload.Visible = False

            ' if no file is selected exit
            If txtFile.PostedFile.FileName = "" Then
                Exit Sub
            End If

            Dim ParentFolderName As String
            If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                ParentFolderName = Common.Globals.HostMapPath
            Else
                ParentFolderName = PortalSettings.HomeDirectoryMapPath
            End If
            ParentFolderName += cboFolders.SelectedItem.Value

            Dim strExtension As String = Replace(Path.GetExtension(txtFile.PostedFile.FileName), ".", "")
            If FileFilter <> "" And InStr("," & FileFilter.ToLower, "," & strExtension.ToLower) = 0 Then
                ' trying to upload a file not allowed for current filter
                lblMessage.Text = String.Format(Localization.GetString("UploadError", Me.LocalResourceFile), FileFilter, strExtension)
            Else
                lblMessage.Text = FileSystemUtils.UploadFile(ParentFolderName.Replace("/", "\"), txtFile.PostedFile, False)
            End If

            If lblMessage.Text = String.Empty Then
                cboFiles.Visible = True
                cmdUpload.Visible = ShowUpLoad
                txtFile.Visible = False
                cmdSave.Visible = False
                cmdCancel.Visible = False

                Dim Root As New DirectoryInfo(ParentFolderName)
                cboFiles.Items.Clear()
                cboFiles.DataSource = GetFileList(False)
                cboFiles.DataBind()

                Dim FileName As String = txtFile.PostedFile.FileName.Substring(txtFile.PostedFile.FileName.LastIndexOf("\") + 1)
                If Not cboFiles.Items.FindByText(FileName) Is Nothing Then
                    cboFiles.Items.FindByText(FileName).Selected = True
                End If

                If cboFiles.SelectedIndex >= 0 Then
                    ViewState("LastFileName") = cboFiles.SelectedValue
                Else
                    ViewState("LastFileName") = ""
                End If
            End If

            _doRenderTypeControls = False 'Must not render on this postback
            _doRenderTypes = False
            _doChangeURL = False
            _doReloadFolders = False
            _doReloadFiles = False
        End Sub

        Private Sub cmdSelect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSelect.Click
            cboUrls.Visible = True
            cmdSelect.Visible = False
            txtUrl.Visible = False
            cmdAdd.Visible = True
            cmdDelete.Visible = PortalSecurity.IsInRole(_objPortal.AdministratorRoleName)

            LoadUrls()

            If Not cboUrls.Items.FindByValue(txtUrl.Text) Is Nothing Then
                cboUrls.ClearSelection()
                cboUrls.Items.FindByValue(txtUrl.Text).Selected = True
            End If

            _doRenderTypeControls = False 'Must not render on this postback
            _doRenderTypes = False
            _doChangeURL = False
            _doReloadFolders = False
            _doReloadFiles = False
        End Sub

        Private Sub cmdUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpload.Click
            Dim strSaveFolder As String = cboFolders.SelectedValue
            LoadFolders("ADD")
            If Not cboFolders.Items.FindByValue(strSaveFolder) Is Nothing Then
                cboFolders.Items.FindByValue(strSaveFolder).Selected = True
                cboFiles.Visible = False
                cmdUpload.Visible = False
                txtFile.Visible = True
                cmdSave.Visible = True
                cmdCancel.Visible = True
            Else
                If cboFolders.Items.Count > 0 Then
                    cboFolders.Items(0).Selected = True
                    cboFiles.Visible = False
                    cmdUpload.Visible = False
                    txtFile.Visible = True
                    cmdSave.Visible = True
                    cmdCancel.Visible = True
                Else
                    'reset controls
                    LoadFolders("BROWSE,ADD")
                    cboFolders.Items.FindByValue(strSaveFolder).Selected = True
                    cboFiles.Visible = True
                    cmdUpload.Visible = False
                    txtFile.Visible = False
                    cmdSave.Visible = False
                    cmdCancel.Visible = False
                    lblMessage.Text = Localization.GetString("NoWritePermission", Me.LocalResourceFile)
                End If
            End If

            _doRenderTypeControls = False 'Must not render on this postback
            _doRenderTypes = False
            _doChangeURL = False
            _doReloadFolders = False
            _doReloadFiles = False
        End Sub

        Private Sub optType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optType.SelectedIndexChanged
            ' Type changed, render the correct control set
            ViewState("UrlType") = optType.SelectedItem.Value
            _doRenderTypeControls = True
        End Sub

#End Region

    End Class

End Namespace
