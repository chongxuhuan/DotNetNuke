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
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.UI.Skins

    Public Class SkinControl
        Inherits Framework.UserControlBase

#Region "Controls"
        Protected WithEvents optHost As System.Web.UI.WebControls.RadioButton
        Protected WithEvents optSite As System.Web.UI.WebControls.RadioButton
        Protected WithEvents cboSkin As System.Web.UI.WebControls.DropDownList
        Protected WithEvents cmdPreview As CommandButton
#End Region

#Region "Private Members"
        Private _Width As String = ""
        Private _SkinRoot As String
        Private _SkinSrc As String
        Private _localResourceFile As String
        Private _objPortal As PortalInfo
        Private _DefaultKey As String = "System"
#End Region

#Region "Public Properties"

        Public Property DefaultKey() As String
            Get
                Return _DefaultKey
            End Get
            Set(ByVal Value As String)
                _DefaultKey = Value
            End Set
        End Property

        Public Property Width() As String
            Get
                Width = Convert.ToString(ViewState("SkinControlWidth"))
            End Get
            Set(ByVal Value As String)
                _Width = Value
            End Set
        End Property

        Public Property SkinRoot() As String
            Get
                SkinRoot = Convert.ToString(ViewState("SkinRoot"))
            End Get
            Set(ByVal Value As String)
                _SkinRoot = Value
            End Set
        End Property

        Public Property SkinSrc() As String
            Get
                If Not cboSkin.SelectedItem Is Nothing Then
                    SkinSrc = cboSkin.SelectedItem.Value
                Else
                    SkinSrc = ""
                End If
            End Get
            Set(ByVal Value As String)
                _SkinSrc = Value
            End Set
        End Property

        Public Property LocalResourceFile() As String
            Get
                Dim fileRoot As String

                If _localResourceFile = "" Then
                    fileRoot = Me.TemplateSourceDirectory & "/" & Services.Localization.Localization.LocalResourceDirectory & "/SkinControl.ascx"
                Else
                    fileRoot = _localResourceFile
                End If
                Return fileRoot
            End Get
            Set(ByVal Value As String)
                _localResourceFile = Value
            End Set
        End Property
#End Region

#Region "Private Methods"

        Private Sub LoadSkins()

            Dim strRoot As String
            Dim strFolder As String
            Dim arrFolders As String()
            Dim strFile As String
            Dim arrFiles As String()
            Dim strLastFolder As String
            Dim strSeparator As String = "----------------------------------------"

            cboSkin.Items.Clear()

            If optHost.Checked Then
                ' load host skins
                strLastFolder = ""
                strRoot = Common.Globals.HostMapPath & SkinRoot
                If Directory.Exists(strRoot) Then
                    arrFolders = Directory.GetDirectories(strRoot)
                    For Each strFolder In arrFolders
                        If Not strFolder.EndsWith(glbHostSkinFolder) Then
                            arrFiles = Directory.GetFiles(strFolder, "*.ascx")
                            For Each strFile In arrFiles
                                strFolder = Mid(strFolder, InStrRev(strFolder, "\") + 1)
                                If strLastFolder <> strFolder Then
                                    If strLastFolder <> "" Then
                                        cboSkin.Items.Add(New ListItem(strSeparator, ""))
                                    End If
                                    strLastFolder = strFolder
                                End If
                                cboSkin.Items.Add(New ListItem(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)), "[G]" & SkinRoot & "/" & strFolder & "/" & Path.GetFileName(strFile)))
                            Next
                        End If
                    Next
                End If
            End If

            If optSite.Checked Then
                ' load portal skins
                strLastFolder = ""
                strRoot = _objPortal.HomeDirectoryMapPath & SkinRoot
                If Directory.Exists(strRoot) Then
                    arrFolders = Directory.GetDirectories(strRoot)
                    For Each strFolder In arrFolders
                        arrFiles = Directory.GetFiles(strFolder, "*.ascx")
                        For Each strFile In arrFiles
                            strFolder = Mid(strFolder, InStrRev(strFolder, "\") + 1)
                            If strLastFolder <> strFolder Then
                                If strLastFolder <> "" Then
                                    cboSkin.Items.Add(New ListItem(strSeparator, ""))
                                End If
                                strLastFolder = strFolder
                            End If
                            cboSkin.Items.Add(New ListItem(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)), "[L]" & SkinRoot & "/" & strFolder & "/" & Path.GetFileName(strFile)))
                        Next
                    Next
                End If
            End If

            ' default value
            If cboSkin.Items.Count > 0 Then
                cboSkin.Items.Insert(0, New ListItem(strSeparator, ""))
            End If
            cboSkin.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString(DefaultKey, LocalResourceFile) + ">", ""))

            ' select current skin
            Dim intIndex As Integer
            For intIndex = 0 To cboSkin.Items.Count - 1
                If cboSkin.Items(intIndex).Value.ToLower = Convert.ToString(ViewState("SkinSrc")).ToLower Then
                    cboSkin.Items(intIndex).Selected = True
                    Exit For
                End If
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' format skin name
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strSkinFolder">The Folder Name</param>
        ''' <param name="strSkinFile">The File Name without extension</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function FormatSkinName(ByVal strSkinFolder As String, ByVal strSkinFile As String) As String
            If strSkinFolder.ToLower = "_default" Then
                ' host folder
                Return strSkinFile
            Else ' portal folder
                Select Case strSkinFile.ToLower
                    Case "skin", "container", "default"
                        Return strSkinFolder
                    Case Else
                        Return strSkinFolder & " - " & strSkinFile
                End Select
            End If
        End Function

#End Region

#Region "Event Handlers"
        '*******************************************************
        '
        ' The Page_Load server event handler on this page is used
        ' to populate the role information for the page
        '
        '*******************************************************
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                Dim objPortals As New PortalController
                If Not (Request.QueryString("pid") Is Nothing) And (PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Or UserController.GetCurrentUserInfo.IsSuperUser) Then
                    _objPortal = objPortals.GetPortal(Int32.Parse(Request.QueryString("pid")))
                Else
                    _objPortal = objPortals.GetPortal(PortalSettings.PortalId)
                End If

                If Not Page.IsPostBack Then

                    ' save persistent values
                    ViewState("SkinControlWidth") = _Width
                    ViewState("SkinRoot") = _SkinRoot
                    ViewState("SkinSrc") = _SkinSrc

                    ' set width of control
                    If _Width <> "" Then
                        cboSkin.Width = System.Web.UI.WebControls.Unit.Parse(_Width)
                    End If

                    ' set selected skin
                    If _SkinSrc <> "" Then
                        Select Case _SkinSrc.Substring(0, 3)
                            Case "[L]"
                                optHost.Checked = False
                                optSite.Checked = True
                            Case "[G]"
                                optSite.Checked = False
                                optHost.Checked = True
                        End Select
                    Else
                        ' no skin selected, initialized to site skin if any exists
                        Dim strRoot As String = _objPortal.HomeDirectoryMapPath & SkinRoot
                        If Directory.Exists(strRoot) AndAlso Directory.GetDirectories(strRoot).Length > 0 Then
                            optHost.Checked = False
                            optSite.Checked = True
                        End If
                    End If

                    LoadSkins()

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub optHost_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optHost.CheckedChanged

            LoadSkins()

        End Sub

        Private Sub optSite_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optSite.CheckedChanged

            LoadSkins()

        End Sub

        Private Sub cmdPreview_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdPreview.Click

            If SkinSrc <> "" Then

                Dim strType As String = SkinRoot.Substring(0, SkinRoot.Length - 1)

                Dim strURL As String = ApplicationURL() & "&" & strType & "Src=" & QueryStringEncode(SkinSrc.Replace(".ascx", ""))

                If SkinRoot = SkinController.RootContainer Then
                    If Not Request.QueryString("ModuleId") Is Nothing Then
                        strURL += "&ModuleId=" & Request.QueryString("ModuleId").ToString
                    End If
                End If

                Response.Redirect(strURL, True)
            End If

        End Sub
#End Region

    End Class

End Namespace
