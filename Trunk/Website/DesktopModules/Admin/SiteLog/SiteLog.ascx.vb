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

Imports DotNetNuke.Services.Log.SiteLog
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Modules.Admin.SiteLog

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SiteLog PortalModuleBase is used to display Logs for the Site
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class SiteLog
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindData binds the controls to the Data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindData()

            Dim strPortalAlias As String

            strPortalAlias = GetPortalDomainName(PortalAlias.HTTPAlias, Request, False)
            If InStr(1, strPortalAlias, "/") <> 0 Then           ' child portal
                strPortalAlias = Left(strPortalAlias, InStrRev(strPortalAlias, "/") - 1)
            End If

            Dim strStartDate As String = txtStartDate.Text
            Dim dtStart As Date = Date.Parse(strStartDate)
            If strStartDate <> "" Then
                strStartDate = strStartDate & " 00:00"
            End If

            Dim strEndDate As String = txtEndDate.Text
            Dim dtEnd As Date = Date.Parse(strEndDate)
            If strEndDate <> "" Then
                strEndDate = strEndDate & " 23:59"
            End If

            Dim objUsers As New UserController
            Dim objUser As UserInfo
            Select Case cboReportType.SelectedItem.Value
                Case "10"  'User Registrations By Date
                    Dim arrUsers As ArrayList = UserController.GetUsers(PortalId)
                    Dim dt As New DataTable
                    Dim dr As DataRow

                    dt.Columns.Add(New DataColumn("Full Name", GetType(String)))
                    dt.Columns.Add(New DataColumn("User Name", GetType(String)))
                    dt.Columns.Add(New DataColumn("Date Registered", GetType(Date)))

                    For Each objUser In arrUsers
                        If objUser.Membership.CreatedDate >= dtStart And objUser.Membership.CreatedDate <= dtEnd And objUser.IsSuperUser = False Then
                            dr = dt.NewRow

                            dr("Date Registered") = objUser.Membership.CreatedDate
                            dr("Full Name") = objUser.Profile.FullName
                            dr("User Name") = objUser.Username

                            dt.Rows.Add(dr)
                        End If
                    Next

                    Dim dv As New DataView(dt)
                    dv.Sort = "Date Registered DESC"
                    grdLog.DataSource = dv
                    grdLog.DataBind()
                Case "11"  'User Registrations By Country
                    Dim arrUsers As ArrayList = UserController.GetUsers(PortalId)
                    Dim dt As New DataTable
                    Dim dr As DataRow

                    dt.Columns.Add(New DataColumn("Full Name", GetType(String)))
                    dt.Columns.Add(New DataColumn("User Name", GetType(String)))
                    dt.Columns.Add(New DataColumn("Country", GetType(String)))

                    For Each objUser In arrUsers
                        If objUser.Membership.CreatedDate >= dtStart And objUser.Membership.CreatedDate <= dtEnd And objUser.IsSuperUser = False Then
                            dr = dt.NewRow

                            dr("Country") = objUser.Profile.Country
                            dr("Full Name") = objUser.Profile.FullName
                            dr("User Name") = objUser.Username

                            dt.Rows.Add(dr)
                        End If
                    Next

                    Dim dv As New DataView(dt)
                    dv.Sort = "Country"
                    grdLog.DataSource = dv
                    grdLog.DataBind()
                Case Else
                    Dim objSiteLog As New SiteLogController
                    Dim dr As IDataReader = objSiteLog.GetSiteLog(PortalId, strPortalAlias, Convert.ToInt32(cboReportType.SelectedItem.Value), Convert.ToDateTime(strStartDate), Convert.ToDateTime(strEndDate))
                    grdLog.DataSource = dr    ' we are using a DataReader here because the resultset returned by GetSiteLog varies based on the report type selected and therefore does not conform to a static business object
                    grdLog.DataBind()
                    dr.Close()
            End Select

            If grdLog.Items.Count > 0 Then
                lblMessage.Visible = False
                grdLog.Visible = True
            Else
                lblMessage.Visible = True
                grdLog.Visible = False
            End If
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                'this needs to execute always to the client script code is registred in InvokePopupCal
                cmdStartCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtStartDate)
                cmdEndCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtEndDate)

                ' If this is the first visit to the page, bind the role data to the datalist
                If Page.IsPostBack = False Then

                    Dim strSiteLogStorage As String = "D"
                    If Not String.IsNullOrEmpty(Host.SiteLogStorage) Then
                        strSiteLogStorage = Host.SiteLogStorage
                    End If
                    If strSiteLogStorage = "F" Then
                        UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("FileSystem", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                        cmdDisplay.Visible = False
                    Else
                        Select Case PortalSettings.SiteLogHistory
                            Case -1       ' unlimited
                            Case 0
                                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("LogDisabled", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                            Case Else
                                UI.Skins.Skin.AddModuleMessage(Me, String.Format(Services.Localization.Localization.GetString("LogHistory", Me.LocalResourceFile), PortalSettings.SiteLogHistory.ToString), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                        End Select
                        cmdDisplay.Visible = True
                    End If

                    Dim ctlList As New Common.Lists.ListController
                    Dim colSiteLogReports As Common.Lists.ListEntryInfoCollection = ctlList.GetListEntryInfoCollection("Site Log Reports")

                    cboReportType.DataSource = colSiteLogReports
                    cboReportType.DataBind()
                    cboReportType.SelectedIndex = 0

                    txtStartDate.Text = DateAdd(DateInterval.Day, -6, Date.Today).ToShortDateString
                    txtEndDate.Text = DateAdd(DateInterval.Day, 1, Date.Today).ToShortDateString

                    ' Store URL Referrer to return to portal
                    If Not Request.UrlReferrer Is Nothing Then
                        If Request.UrlReferrer.AbsoluteUri = Request.Url.AbsoluteUri Then
                            ViewState("UrlReferrer") = ""
                        Else
                            ViewState("UrlReferrer") = Convert.ToString(Request.UrlReferrer)
                        End If
                    Else
                        ViewState("UrlReferrer") = ""
                    End If
                End If

                If Convert.ToString(ViewState("UrlReferrer")) = "" Then
                    cmdCancel.Visible = False
                Else
                    cmdCancel.Visible = True
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the Cancel Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(Convert.ToString(ViewState("UrlReferrer")))
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDisplay_Click runs when the Display Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDisplay_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDisplay.Click
            Try
                BindData()
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
        ''' <summary>
        ''' ensure sitelog grid data is htmlencoded
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Sub grdLog_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdLog.ItemDataBound
            Dim item As DataGridItem = DirectCast(e.Item, DataGridItem)
            Dim itemType As ListItemType = item.ItemType
            If ((itemType = ListItemType.AlternatingItem) OrElse (itemType = ListItemType.Item)) Then
                Dim cells As TableCellCollection = DirectCast(item.Cells, TableCellCollection)
                For Each cell As TableCell In cells
                    cell.Text = HttpUtility.HtmlEncode(cell.Text.ToString())
                Next
            End If
        End Sub

#End Region

    End Class

End Namespace
