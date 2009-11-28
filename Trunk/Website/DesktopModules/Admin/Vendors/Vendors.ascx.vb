'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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

Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Services.Vendors
Imports DotNetNuke.UI.Utilities

Namespace DotNetNuke.Modules.Admin.Vendors

	''' -----------------------------------------------------------------------------
	''' <summary>
	''' The Vendors PortalModuleBase is used to manage the Vendors of a portal
	''' </summary>
    ''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
	'''                       and localisation
	''' </history>
	''' -----------------------------------------------------------------------------
	Partial  Class Vendors
        Inherits Entities.Modules.PortalModuleBase
		Implements Entities.Modules.IActionable

#Region "Controls"

		Protected WithEvents lblMessage As System.Web.UI.WebControls.Label

#End Region

#Region "Protected Members"

        Protected CurrentPage As Integer = -1
        Protected TotalPages As Integer = -1

#End Region

#Region "Private Members"

        Dim strFilter As String

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindData gets the vendors from the Database and binds them to the DataGrid
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindData()
            BindData(Nothing, Nothing)
        End Sub

        Private Sub BindData(ByVal SearchText As String, ByVal SearchField As String)

            CreateLetterSearch()

            'Localize the Headers
            Services.Localization.Localization.LocalizeDataGrid(grdVendors, Me.LocalResourceFile)

            Select Case SearchText
                Case Services.Localization.Localization.GetString("All")
                    strFilter = ""
                Case Services.Localization.Localization.GetString("Unauthorized")
                    strFilter = ""
                Case Else
                    strFilter = SearchText
            End Select

            ' Get the list of vendors from the database
            Dim PageSize As Integer = Convert.ToInt32(ddlRecordsPerPage.SelectedItem.Value)
            Dim TotalRecords As Integer
            Dim objVendors As New Services.Vendors.VendorController
            Dim Portal As Integer
            If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                Portal = Null.NullInteger
            Else
                Portal = PortalId
            End If


            If strFilter = "" Then
                If SearchText = Services.Localization.Localization.GetString("Unauthorized") Then
                    grdVendors.DataSource = objVendors.GetVendors(Portal, True, CurrentPage - 1, PageSize, TotalRecords)
                Else
                    grdVendors.DataSource = objVendors.GetVendors(Portal, False, CurrentPage - 1, PageSize, TotalRecords)
                End If
            Else
                If SearchField = "email" Then
                    grdVendors.DataSource = objVendors.GetVendorsByEmail(strFilter, Portal, CurrentPage - 1, PageSize, TotalRecords)
                Else
                    grdVendors.DataSource = objVendors.GetVendorsByName(strFilter, Portal, CurrentPage - 1, PageSize, TotalRecords)
                End If
            End If

            grdVendors.DataBind()

            ctlPagingControl.TotalRecords = TotalRecords
            ctlPagingControl.PageSize = PageSize
            ctlPagingControl.CurrentPage = CurrentPage
            Dim strQuerystring As String = ""
            If ddlRecordsPerPage.SelectedIndex <> 0 Then
                strQuerystring = "PageRecords=" + ddlRecordsPerPage.SelectedValue
            End If
            If strFilter <> "" Then
                strQuerystring += "&filter=" + strFilter
            End If
            ctlPagingControl.QuerystringParams = strQuerystring
            ctlPagingControl.TabID = TabId

        End Sub

        Private Sub CreateLetterSearch()

            Dim strAlphabet As String() = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", Services.Localization.Localization.GetString("All"), Services.Localization.Localization.GetString("Unauthorized")}
            rptLetterSearch.DataSource = strAlphabet
            rptLetterSearch.DataBind()

        End Sub
#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DisplayAddress correctly formats an Address
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function DisplayAddress(ByVal Unit As Object, ByVal Street As Object, ByVal City As Object, ByVal Region As Object, ByVal Country As Object, ByVal PostalCode As Object) As String
            Return FormatAddress(Unit, Street, City, Region, Country, PostalCode)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DisplayEmail correctly formats an Email Address
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function DisplayEmail(ByVal Email As String) As String
            Return HtmlUtils.FormatEmail(Email)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatURL correctly formats the Url for the Edit Vendor Link
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FormatURL(ByVal strKeyName As String, ByVal strKeyValue As String) As String
            If strFilter <> "" Then
                Return EditUrl(strKeyName, strKeyValue, "", "filter=" & strFilter)
            Else
                Return EditUrl(strKeyName, strKeyValue)
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FilterURL correctly formats the Url for filter by first letter and paging
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function FilterURL(ByVal Filter As String, ByVal CurrentPage As String) As String
            If Filter <> "" Then
                If CurrentPage <> "" Then
                    Return Common.Globals.NavigateURL(TabId, "", "filter=" & Filter, "currentpage=" & CurrentPage, "PageRecords=" & ddlRecordsPerPage.SelectedValue)
                Else
                    Return Common.Globals.NavigateURL(TabId, "", "filter=" & Filter, "PageRecords=" & ddlRecordsPerPage.SelectedValue)
                End If
            Else
                If CurrentPage <> "" Then
                    Return Common.Globals.NavigateURL(TabId, "", "currentpage=" & CurrentPage, "PageRecords=" & ddlRecordsPerPage.SelectedValue)
                Else
                    Return Common.Globals.NavigateURL(TabId, "", "PageRecords=" & ddlRecordsPerPage.SelectedValue)
                End If
            End If
        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Request.QueryString("CurrentPage") Is Nothing Then
                    CurrentPage = CType(Request.QueryString("CurrentPage"), Integer)
                Else
                    CurrentPage = 1
                End If

                If Not Request.QueryString("filter") Is Nothing Then
                    strFilter = Request.QueryString("filter")
                Else
                    strFilter = ""
                End If

                If Not Page.IsPostBack Then
                    ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItems"))

                    If Not Request.QueryString("PageRecords") Is Nothing Then
                        ddlRecordsPerPage.SelectedValue = Request.QueryString("PageRecords")
                    End If

                    BindData(strFilter, "username")
                End If

                cmdDelete.Visible = Me.IsEditable

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' grdVendors_ItemCommand runs when a command button in the grid is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub grdVendors_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdVendors.ItemCommand
            Try
                If e.CommandName = "filter" Then
                    strFilter = e.CommandArgument.ToString
                    CurrentPage = 1
                    txtSearch.Text = ""
                    BindData(strFilter, "username")
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDelete_Click runs when the Delete Unauthorized Vendors button is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
            Try
                Dim objVendors As New VendorController
                If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                    objVendors.DeleteVendors()
                Else
                    objVendors.DeleteVendors(PortalId)
                End If
                BindData()

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ddlRecordsPerPage_SelectedIndexChanged runs when the user selects a new
        ''' Records Per Page value from the dropdown.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[dancaron]	10/28/2004	Intial Version
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ddlRecordsPerPage_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlRecordsPerPage.SelectedIndexChanged
            CurrentPage = 1
            BindData()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' btnSearch_Click runs when the user searches for accounts by username or email
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[dancaron]	10/28/2004	Intial Version
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
            CurrentPage = 1
            BindData(txtSearch.Text, ddlSearchType.SelectedItem.Value)
        End Sub

#End Region

#Region "Optional Interfaces"

        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                Actions.Add(GetNextActionID, Services.Localization.Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl(), False, SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property

#End Region

    End Class

End Namespace