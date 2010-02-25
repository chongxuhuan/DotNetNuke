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
Imports DotNetNuke.Services.FileSystem

Namespace DotNetNuke.UI.UserControls

    Public MustInherit Class URLTrackingControl
        Inherits Framework.UserControlBase

#Region "Controls"
        Protected WithEvents lblURL As System.Web.UI.WebControls.Label
        Protected WithEvents lblCreatedDate As System.Web.UI.WebControls.Label

        Protected WithEvents pnlTrack As System.Web.UI.WebControls.Panel
        Protected WithEvents lblTrackingURL As System.Web.UI.WebControls.Label
        Protected WithEvents lblClicks As System.Web.UI.WebControls.Label
        Protected WithEvents lblLastClick As System.Web.UI.WebControls.Label

        Protected WithEvents pnlLog As System.Web.UI.WebControls.Panel
        Protected WithEvents txtStartDate As System.Web.UI.WebControls.TextBox
        Protected WithEvents txtEndDate As System.Web.UI.WebControls.TextBox
        Protected WithEvents cmdStartCalendar As System.Web.UI.WebControls.HyperLink
        Protected WithEvents cmdEndCalendar As System.Web.UI.WebControls.HyperLink
        Protected WithEvents valStartDate As System.Web.UI.WebControls.CompareValidator
        Protected WithEvents valEndDate As System.Web.UI.WebControls.CompareValidator
        Protected WithEvents cmdDisplay As System.Web.UI.WebControls.LinkButton
        Protected WithEvents grdLog As System.Web.UI.WebControls.DataGrid
        Protected WithEvents lblLogURL As System.Web.UI.WebControls.Label

        Protected Label1 As System.Web.UI.WebControls.Label
        Protected Label2 As System.Web.UI.WebControls.Label
        Protected Label3 As System.Web.UI.WebControls.Label
        Protected Label4 As System.Web.UI.WebControls.Label
        Protected Label5 As System.Web.UI.WebControls.Label
        Protected Label6 As System.Web.UI.WebControls.Label
        Protected Label7 As System.Web.UI.WebControls.Label
#End Region

#Region "Private Members"
        Private _URL As String = ""
        Private _FormattedURL As String = ""
        Private _TrackingURL As String = ""
        Private _ModuleID As Integer = -2
        Private _localResourceFile As String
#End Region

#Region "Public Properties"
        Public Property FormattedURL() As String
            Get
                FormattedURL = _FormattedURL
            End Get
            Set(ByVal Value As String)
                _FormattedURL = Value
            End Set
        End Property

        Public Property TrackingURL() As String
            Get
                TrackingURL = _TrackingURL
            End Get
            Set(ByVal Value As String)
                _TrackingURL = Value
            End Set
        End Property

        Public Property URL() As String
            Get
                URL = _URL
            End Get
            Set(ByVal Value As String)
                _URL = Value
            End Set
        End Property

        Public Property ModuleID() As Integer
            Get
                ModuleID = _ModuleID
                If ModuleID = -2 Then
                    If Not Request.QueryString("mid") Is Nothing Then
                        ModuleID = Int32.Parse(Request.QueryString("mid"))
                    End If
                End If
            End Get
            Set(ByVal Value As Integer)
                _ModuleID = Value
            End Set
        End Property

        Public Property LocalResourceFile() As String
            Get
                Dim fileRoot As String

                If _localResourceFile = "" Then
                    fileRoot = Me.TemplateSourceDirectory & "/" & Services.Localization.Localization.LocalResourceDirectory & "/URLTrackingControl.ascx"
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

#Region "Event Handlers"
        '*******************************************************
        '
        ' The Page_Load server event handler on this page is used
        ' to populate the role information for the page
        '
        '*******************************************************

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                'this needs to execute always to the client script code is registred in InvokePopupCal
                cmdStartCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtStartDate)
                cmdEndCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtEndDate)

                If Not Page.IsPostBack Then

                    If _URL <> "" Then

                        lblLogURL.Text = URL ' saved for loading Log grid

                        Dim URLType As TabType = GetURLType(_URL)
                        If URLType = TabType.File And _URL.ToLower.StartsWith("fileid=") = False Then
                            ' to handle legacy scenarios before the introduction of the FileServerHandler
                            Dim objFiles As New FileController
                            lblLogURL.Text = "FileID=" & objFiles.ConvertFilePathToFileId(_URL, PortalSettings.PortalId).ToString
                        End If


                        Dim objUrls As New UrlController
                        Dim objUrlTracking As UrlTrackingInfo = objUrls.GetUrlTracking(PortalSettings.PortalId, lblLogURL.Text, ModuleID)
                        If Not objUrlTracking Is Nothing Then
                            If _FormattedURL = "" Then
                                If Not URL.StartsWith("http") And Not URL.StartsWith("mailto") Then
                                    lblURL.Text = AddHTTP(Request.Url.Host)
                                End If
                                lblURL.Text += Common.Globals.LinkClick(URL, PortalSettings.ActiveTab.TabID, ModuleID, False)
                            Else
                                lblURL.Text = _FormattedURL
                            End If
                            lblCreatedDate.Text = objUrlTracking.CreatedDate.ToString

                            If objUrlTracking.TrackClicks Then
                                pnlTrack.Visible = True
                                If _TrackingURL = "" Then
                                    If Not URL.StartsWith("http") Then
                                        lblTrackingURL.Text = AddHTTP(Request.Url.Host)
                                    End If
                                    lblTrackingURL.Text += Common.Globals.LinkClick(URL, PortalSettings.ActiveTab.TabID, ModuleID, objUrlTracking.TrackClicks)
                                Else
                                    lblTrackingURL.Text = _TrackingURL
                                End If
                                lblClicks.Text = objUrlTracking.Clicks.ToString
                                If Not Null.IsNull(objUrlTracking.LastClick) Then
                                    lblLastClick.Text = objUrlTracking.LastClick.ToString
                                End If
                            End If

                            If objUrlTracking.LogActivity Then
                                pnlLog.Visible = True

                                txtStartDate.Text = DateAdd(DateInterval.Day, -6, Date.Today).ToShortDateString
                                txtEndDate.Text = DateAdd(DateInterval.Day, 1, Date.Today).ToShortDateString
                            End If
                        End If
                    Else
                        Me.Visible = False
                    End If

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdDisplay_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDisplay.Click
            Try

                Dim strStartDate As String = txtStartDate.Text
                If strStartDate <> "" Then
                    strStartDate = strStartDate & " 00:00"
                End If

                Dim strEndDate As String = txtEndDate.Text
                If strEndDate <> "" Then
                    strEndDate = strEndDate & " 23:59"
                End If

                Dim objUrls As New UrlController
                'localize datagrid
                Services.Localization.Localization.LocalizeDataGrid(grdLog, Me.LocalResourceFile)
                grdLog.DataSource = objUrls.GetUrlLog(PortalSettings.PortalId, lblLogURL.Text, ModuleID, Convert.ToDateTime(strStartDate), Convert.ToDateTime(strEndDate))
                grdLog.DataBind()

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
