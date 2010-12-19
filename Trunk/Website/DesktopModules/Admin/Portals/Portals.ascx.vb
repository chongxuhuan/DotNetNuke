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

Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.UI.Skins.Controls.ModuleMessage
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.Modules.Admin.Portals

	''' -----------------------------------------------------------------------------
	''' <summary>
	''' The Portals PortalModuleBase is used to manage the portlas.
	''' </summary>
    ''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
	'''                       and localisation
	''' </history>
	''' -----------------------------------------------------------------------------
	Partial  Class Portals

		Inherits Entities.Modules.PortalModuleBase
		Implements Entities.Modules.IActionable

#Region "Private Members"

        Private _Filter As String = ""
        Private _CurrentPage As Integer = 1
        Private _Portals As ArrayList = New ArrayList

#End Region

#Region "Protected Members"

        Protected TotalPages As Integer = -1
        Protected TotalRecords As Integer

        Protected Property CurrentPage() As Integer
            Get
                Return _CurrentPage
            End Get
            Set(ByVal Value As Integer)
                _CurrentPage = Value
            End Set
        End Property

        Protected Property Filter() As String
            Get
                Return _Filter
            End Get
            Set(ByVal Value As String)
                _Filter = Value
            End Set
        End Property

        Protected Property Portals() As ArrayList
            Get
                Return _Portals
            End Get
            Set(ByVal Value As ArrayList)
                _Portals = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Page Size for the Grid
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/02/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property PageSize() As Integer
            Get
                'Dim setting As Object = UserModuleBase.GetSetting(UsersPortalId, "Records_PerPage")
                'Return CType(setting, Integer)
                Return 20
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a flag that determines whether to suppress the Pager (when not required)
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/10/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property SuppressPager() As Boolean
            Get
                'Dim setting As Object = UserModuleBase.GetSetting(UsersPortalId, "Display_SuppressPager")
                'Return CType(setting, Boolean)
                Return True
            End Get
        End Property

#End Region

#Region "Private Methods"

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' BindData fetches the data from the database and updates the controls
		''' </summary>
        ''' <remarks>
		''' </remarks>
        ''' <history>
		''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
		'''                       and localisation
		''' </history>
		''' -----------------------------------------------------------------------------
        Private Sub BindData()
            CreateLetterSearch()
            Dim strQuerystring As String = Null.NullString
            If Filter <> "" Then
                strQuerystring += "filter=" + Filter
            End If

            If Filter = Localization.GetString("Expired", LocalResourceFile) Then
                Portals = PortalController.GetExpiredPortals()
                ctlPagingControl.Visible = False
            Else
                Portals = PortalController.GetPortalsByName(Filter + "%", CurrentPage - 1, PageSize, TotalRecords)
            End If

            grdPortals.DataSource = Portals
            grdPortals.DataBind()

            ctlPagingControl.TotalRecords = TotalRecords
            ctlPagingControl.PageSize = PageSize
            ctlPagingControl.CurrentPage = CurrentPage

            ctlPagingControl.QuerystringParams = strQuerystring
            ctlPagingControl.TabID = TabId

            If SuppressPager And ctlPagingControl.Visible Then
                ctlPagingControl.Visible = (PageSize < TotalRecords)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds the letter filter
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/17/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CreateLetterSearch()
            Dim filters As String = Localization.GetString("Filter.Text", Me.LocalResourceFile)

            filters += "," + Localization.GetString("All")
            filters += "," + Localization.GetString("Expired", LocalResourceFile)

            Dim strAlphabet As String() = filters.Split(","c)
            rptLetterSearch.DataSource = strAlphabet
            rptLetterSearch.DataBind()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes all expired portals
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	11/17/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteExpiredPortals()
            Try
                CheckSecurity()
                PortalController.DeleteExpiredPortals(GetAbsoluteServerPath(Request))

                BindData()

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub CheckSecurity()
            ' Verify that the current user has access to access this page
            If Not UserInfo.IsSuperUser Then
                Response.Redirect(NavigateURL("Access Denied"), True)
            End If
        End Sub
#End Region

#Region "Protected Methods"

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
            Dim _URL As String = Null.NullString
            If Filter <> "" Then
                If CurrentPage <> "" Then
                    _URL = Common.Globals.NavigateURL(TabId, "", "filter=" & Filter, "currentpage=" & CurrentPage)
                Else
                    _URL = Common.Globals.NavigateURL(TabId, "", "filter=" & Filter)
                End If
            Else
                If CurrentPage <> "" Then
                    _URL = Common.Globals.NavigateURL(TabId, "", "currentpage=" & CurrentPage)
                Else
                    _URL = Common.Globals.NavigateURL(TabId, "")
                End If
            End If
            Return _URL

        End Function

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatExpiryDate formats the expiry date and filter out null-dates
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FormatExpiryDate(ByVal DateTime As Date) As String
            Dim strDate As String = String.Empty
            Try
                If Not Null.IsNull(DateTime) Then
                    strDate = DateTime.ToShortDateString
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return strDate
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatExpiryDate formats the format name as an a tag
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FormatPortalAliases(ByVal PortalID As Integer) As String
            Dim str As New System.Text.StringBuilder
            Try
                Dim objPortalAliasController As New PortalAliasController
                Dim arr As ArrayList = objPortalAliasController.GetPortalAliasArrayByPortalID(PortalID)
                Dim objPortalAliasInfo As PortalAliasInfo
                Dim i As Integer
                For i = 0 To arr.Count - 1
                    objPortalAliasInfo = CType(arr(i), PortalAliasInfo)

                    Dim httpAlias As String = AddHTTP(objPortalAliasInfo.HTTPAlias)
                    Dim originalUrl As String = HttpContext.Current.Items("UrlRewrite:OriginalUrl").ToString().ToLowerInvariant()

                    httpAlias = AddPort(httpAlias, originalUrl)

                    str.Append("<a href=""" + httpAlias + """>" + objPortalAliasInfo.HTTPAlias + "</a>" + "<br/>")
                Next
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return str.ToString
        End Function

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            For Each column As DataGridColumn In grdPortals.Columns
                If column.GetType Is GetType(ImageCommandColumn) Then
                    'Manage Delete Confirm JS
                    Dim imageColumn As ImageCommandColumn = CType(column, ImageCommandColumn)
                    If imageColumn.CommandName = "Delete" Then
                        imageColumn.OnClickJS = Localization.GetString("DeleteItem")
                    End If
                    'Manage Edit Column NavigateURLFormatString
                    If imageColumn.CommandName = "Edit" Then
                        'The Friendly URL parser does not like non-alphanumeric characters
                        'so first create the format string with a dummy value and then
                        'replace the dummy value with the FormatString place holder
                        Dim formatString As String = EditUrl("pid", "KEYFIELD", "Edit")
                        formatString = formatString.Replace("KEYFIELD", "{0}")
                        imageColumn.NavigateURLFormatString = formatString
                    End If
                    'Localize Image Column Text
                    If imageColumn.CommandName <> "" Then
                        imageColumn.Text = Localization.GetString(imageColumn.CommandName, Me.LocalResourceFile)
                    End If
                End If
            Next
        End Sub

       
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        '''     [VMasanas]  9/28/2004   Changed redirect to Access Denied
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                'Add an Action Event Handler to the Skin
                AddActionHandler(AddressOf ModuleAction_Click)

                CheckSecurity()

                If Not Request.QueryString("CurrentPage") Is Nothing Then
                    CurrentPage = CType(Request.QueryString("CurrentPage"), Integer)
                End If

                If Not Request.QueryString("filter") Is Nothing Then
                    Filter = Request.QueryString("filter")
                End If

                If Filter = Localization.GetString("All") Then
                    Filter = ""
                End If

                If Not Page.IsPostBack Then
                    'Localize the Headers
                    Localization.LocalizeDataGrid(grdPortals, Me.LocalResourceFile)
                    BindData()
                End If

            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ModuleAction_Click handles all ModuleAction events raised from the skin
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="sender"> The object that triggers the event</param>
        ''' <param name="e">An ActionEventArgs object</param>
        ''' <history>
        ''' 	[cnurse]	11/17/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ModuleAction_Click(ByVal sender As Object, ByVal e As ActionEventArgs)

            Select Case e.Action.CommandArgument
                Case "Delete"
                    DeleteExpiredPortals()
            End Select

        End Sub

        Private Sub grdPortals_DeleteCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdPortals.DeleteCommand
            Try
                CheckSecurity()
                Dim objPortalController As New PortalController
                Dim portal As PortalInfo = objPortalController.GetPortal(Int32.Parse(e.CommandArgument.ToString))

                If Not portal Is Nothing Then
                    Dim strMessage As String = PortalController.DeletePortal(portal, GetAbsoluteServerPath(Request))
                    If String.IsNullOrEmpty(strMessage) Then
                        Dim objEventLog As New Services.Log.EventLog.EventLogController
                        objEventLog.AddLog("PortalName", portal.PortalName, PortalSettings, UserId, Services.Log.EventLog.EventLogController.EventLogType.PORTAL_DELETED)
                        UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("PortalDeleted", Me.LocalResourceFile), ModuleMessageType.GreenSuccess)
                    Else
                        UI.Skins.Skin.AddModuleMessage(Me, strMessage, ModuleMessageType.RedError)
                    End If
                End If

                BindData()

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub grdPortals_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdPortals.ItemDataBound
            Dim item As DataGridItem = e.Item

            If item.ItemType = ListItemType.Item Or _
                    item.ItemType = ListItemType.AlternatingItem Or _
                    item.ItemType = ListItemType.SelectedItem Then

                Dim imgColumnControl As Control = item.Controls(1).Controls(0)
                If TypeOf imgColumnControl Is ImageButton Then
                    Dim delImage As ImageButton = CType(imgColumnControl, ImageButton)
                    Dim portal As PortalInfo = CType(item.DataItem, PortalInfo)

                    delImage.Visible = Not (portal.PortalID = PortalSettings.PortalId)
                End If
            End If
        End Sub

#End Region

#Region "Optional Interfaces"

        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                Actions.Add(GetNextActionID, Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", EditUrl("Signup"), False, SecurityAccessLevel.Host, True, False)
                Actions.Add(GetNextActionID, Localization.GetString("ExportTemplate.Action", LocalResourceFile), ModuleActionType.AddContent, "", "lt.gif", EditUrl("Template"), False, SecurityAccessLevel.Admin, True, False)
                Actions.Add(GetNextActionID, Localization.GetString("DeleteExpired.Action", LocalResourceFile), ModuleActionType.AddContent, "Delete", "delete.gif", "", "confirm('" + ClientAPI.GetSafeJSString(Localization.GetString("DeleteItems.Confirm")) + "')", True, SecurityAccessLevel.Admin, True, False)
                Return Actions
            End Get
        End Property

#End Region

    End Class

End Namespace
