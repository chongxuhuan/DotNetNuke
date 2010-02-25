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
Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.Modules.Admin.Users

	''' -----------------------------------------------------------------------------
	''' <summary>
    ''' The ProfileDefinitions PortalModuleBase is used to manage the Profile Properties
    ''' for a portal
	''' </summary>
    ''' <remarks>
	''' </remarks>
	''' <history>
    ''' 	[cnurse]	02/16/2006  Created
	''' </history>
	''' -----------------------------------------------------------------------------
    Partial Class ProfileDefinitions
        Inherits Entities.Modules.PortalModuleBase
        Implements Entities.Modules.IActionable

#Region "Private Members"

        Private _ProfileProperties As ProfilePropertyDefinitionCollection

#End Region

#Region "Constants"

        Const COLUMN_REQUIRED As Integer = 10
        Const COLUMN_VISIBLE As Integer = 11
        Const COLUMN_MOVE_DOWN As Integer = 2
        Const COLUMN_MOVE_UP As Integer = 3

#End Region

#Region "Protected Members"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether we are dealing with SuperUsers
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/11/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property IsSuperUser() As Boolean
            Get
                If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the collection of Profile Proeprties
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/03/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ProfileProperties() As ProfilePropertyDefinitionCollection
            Get
                If _ProfileProperties Is Nothing Then
                    _ProfileProperties = ProfileController.GetPropertyDefinitionsByPortal(UsersPortalId, False)
                End If
                Return _ProfileProperties
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Return Url for the page
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ReturnUrl() As String
            Get
                Dim _ReturnURL As String
                Dim FilterParams(IIf(Request.QueryString("filterproperty") = "", 1, 2)) As String

                If (Request.QueryString("filterProperty") = "") Then
                    FilterParams.SetValue("filter=" & Request.QueryString("filter"), 0)
                Else
                    FilterParams.SetValue("filter=" & Request.QueryString("filter"), 0)
                    FilterParams.SetValue("filterProperty=" & Request.QueryString("filterProperty"), 1)
                End If

                If String.IsNullOrEmpty(Request.QueryString("filter")) Then
                    _ReturnURL = NavigateURL(TabId)
                Else
                    _ReturnURL = NavigateURL(TabId, "", FilterParams)
                End If

                Return _ReturnURL
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Portal Id whose Users we are managing
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/11/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property UsersPortalId() As Integer
            Get
                Dim intPortalId As Integer = PortalId
                If IsSuperUser Then
                    intPortalId = Null.NullInteger
                End If
                Return intPortalId
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Helper function that determines whether the client-side functionality is possible
        ''' </summary>
        ''' <history>
        '''     [Jon Henning]	03/12/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function SupportsRichClient() As Boolean
            Return DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a property
        ''' </summary>
        ''' <param name="index">The index of the Property to delete</param>
        ''' <history>
        '''     [cnurse]	02/23/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteProperty(ByVal index As Integer)
            Dim objProperty As ProfilePropertyDefinition = ProfileProperties(index)

            ProfileController.DeletePropertyDefinition(objProperty)

            RefreshGrid()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Moves a property
        ''' </summary>
        ''' <param name="index">The index of the Property to move</param>
        ''' <param name="destIndex">The new index of the Property</param>
        ''' <history>
        '''     [cnurse]	02/23/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub MoveProperty(ByVal index As Integer, ByVal destIndex As Integer)

            Dim objProperty As ProfilePropertyDefinition = ProfileProperties(index)
            Dim objNext As ProfilePropertyDefinition = ProfileProperties(destIndex)

            Dim currentOrder As Integer = objProperty.ViewOrder
            Dim nextOrder As Integer = objNext.ViewOrder

            'Swap ViewOrders
            objProperty.ViewOrder = nextOrder
            objNext.ViewOrder = currentOrder

            ''Refresh Grid
            ProfileProperties.Sort()
            BindGrid()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Moves a property down in the ViewOrder
        ''' </summary>
        ''' <param name="index">The index of the Property to move</param>
        ''' <history>
        '''     [cnurse]	02/23/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub MovePropertyDown(ByVal index As Integer)

            MoveProperty(index, index + 1)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Moves a property up in the ViewOrder
        ''' </summary>
        ''' <param name="index">The index of the Property to move</param>
        ''' <history>
        '''     [cnurse]	02/23/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub MovePropertyUp(ByVal index As Integer)

            MoveProperty(index, index - 1)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Binds the Property Collection to the Grid
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/23/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindGrid()
            Dim allRequired As Boolean = True
            Dim allVisible As Boolean = True

            'Check whether the checkbox column headers are true or false
            For Each profProperty As ProfilePropertyDefinition In ProfileProperties
                If profProperty.Required = False Then
                    allRequired = False
                End If
                If profProperty.Visible = False Then
                    allVisible = False
                End If

                If Not allRequired And Not allVisible Then
                    Exit For
                End If
            Next

            For Each column As DataGridColumn In grdProfileProperties.Columns
                If column.GetType Is GetType(CheckBoxColumn) Then
                    'Manage CheckBox column events
                    Dim cbColumn As CheckBoxColumn = CType(column, CheckBoxColumn)
                    If cbColumn.DataField = "Required" Then
                        cbColumn.Checked = allRequired
                    End If
                    If cbColumn.DataField = "Visible" Then
                        cbColumn.Checked = allVisible
                    End If
                End If
            Next
            grdProfileProperties.DataSource = ProfileProperties
            grdProfileProperties.DataBind()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the Property Collection to the Grid
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/23/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub RefreshGrid()
            _ProfileProperties = Nothing
            BindGrid()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates any "dirty" properties
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/23/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UpdateProperties()
            ProcessPostBack()
            For Each objProperty As ProfilePropertyDefinition In ProfileProperties
                If objProperty.IsDirty Then
                    If UsersPortalId = Null.NullInteger Then
                        objProperty.Required = False
                    End If
                    ProfileController.UpdatePropertyDefinition(objProperty)
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This method is responsible for taking in posted information from the grid and
        ''' persisting it to the property definition collection
        ''' </summary>
        ''' <history>
        '''     [Jon Henning]	03/12/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProcessPostBack()
            Try
                Dim aryNewOrder() As String = DotNetNuke.UI.Utilities.ClientAPI.GetClientSideReorder(Me.grdProfileProperties.ClientID, Me.Page)
                Dim objProperty As ProfilePropertyDefinition
                Dim objItem As DataGridItem
                Dim chk As CheckBox
                For i As Integer = 0 To Me.grdProfileProperties.Items.Count - 1
                    objItem = Me.grdProfileProperties.Items(i)
                    objProperty = ProfileProperties(i)
                    chk = CType(objItem.Cells(COLUMN_REQUIRED).Controls(0), CheckBox)
                    objProperty.Required = chk.Checked
                    chk = CType(objItem.Cells(COLUMN_VISIBLE).Controls(0), CheckBox)
                    objProperty.Visible = chk.Checked
                Next
                'assign vieworder
                For i As Integer = 0 To aryNewOrder.Length - 1
                    ProfileProperties(CInt(aryNewOrder(i))).ViewOrder = i
                Next
                ProfileProperties.Sort()
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region

#Region "Protected Methods"

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            If Not (savedState Is Nothing) Then
                ' Load State from the array of objects that was saved with SaveViewState.

                Dim myState As Object() = CType(savedState, Object())

                'Load Base Controls ViewState
                If Not (myState(0) Is Nothing) Then
                    MyBase.LoadViewState(myState(0))
                End If

                'Load ModuleID
                If Not (myState(1) Is Nothing) Then
                    _ProfileProperties = CType(myState(1), ProfilePropertyDefinitionCollection)
                End If
            End If
        End Sub

        Protected Overrides Function SaveViewState() As Object
            Dim allStates(1) As Object

            ' Save the Base Controls ViewState
            allStates(0) = MyBase.SaveViewState()

            'Save the Profile Properties
            allStates(1) = ProfileProperties

            Return allStates
        End Function

#End Region

#Region "Public Methods"

        Public Function DisplayDataType(ByVal definition As ProfilePropertyDefinition) As String

            Dim retValue As String = Null.NullString
            Dim objListController As New ListController
            Dim definitionEntry As ListEntryInfo = objListController.GetListEntryInfo(definition.DataType)

            If Not definitionEntry Is Nothing Then
                retValue = definitionEntry.Value
            End If

            Return retValue

        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/16/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            For Each column As DataGridColumn In grdProfileProperties.Columns
                If column.GetType Is GetType(CheckBoxColumn) Then
                    Dim cbColumn As CheckBoxColumn = CType(column, CheckBoxColumn)
                    If cbColumn.DataField = "Required" And UsersPortalId = Null.NullInteger Then
                        cbColumn.Visible = False
                    End If
                    If SupportsRichClient() = False Then
                        AddHandler cbColumn.CheckedChanged, AddressOf grdProfileProperties_ItemCheckedChanged
                    End If
                ElseIf column.GetType Is GetType(ImageCommandColumn) Then
                    'Manage Delete Confirm JS
                    Dim imageColumn As ImageCommandColumn = CType(column, ImageCommandColumn)
                    Select Case imageColumn.CommandName
                        Case "Delete"
                            imageColumn.OnClickJS = Localization.GetString("DeleteItem")
                            imageColumn.Text = Localization.GetString("Delete", Me.LocalResourceFile)
                        Case "Edit"
                            'The Friendly URL parser does not like non-alphanumeric characters
                            'so first create the format string with a dummy value and then
                            'replace the dummy value with the FormatString place holder
                            Dim formatString As String = EditUrl("PropertyDefinitionID", "KEYFIELD", "EditProfileProperty")
                            formatString = formatString.Replace("KEYFIELD", "{0}")
                            imageColumn.NavigateURLFormatString = formatString
                        Case "MoveUp"
                            imageColumn.Text = Localization.GetString("MoveUp", Me.LocalResourceFile)
                        Case "MoveDown"
                            imageColumn.Text = Localization.GetString("MoveDown", Me.LocalResourceFile)
                    End Select
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/16/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    Localization.LocalizeDataGrid(grdProfileProperties, Me.LocalResourceFile)
                    BindGrid()
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdRefresh_Click runs when the refresh button is clciked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/23/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRefresh.Click
            RefreshGrid()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the update button is clciked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/23/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Try
                UpdateProperties()

                'Redirect to upadte page
                Response.Redirect(Request.RawUrl, True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' grdProfileProperties_ItemCheckedChanged runs when a checkbox in the grid
        ''' is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/23/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub grdProfileProperties_ItemCheckedChanged(ByVal sender As Object, ByVal e As UI.WebControls.DNNDataGridCheckChangedEventArgs)
            Dim propertyName As String = e.Field
            Dim propertyValue As Boolean = e.Checked
            Dim isAll As Boolean = e.IsAll
            Dim index As Integer = e.Item.ItemIndex

            Dim profProperty As ProfilePropertyDefinition

            If isAll Then
                'Update All the properties
                For Each profProperty In ProfileProperties
                    Select Case propertyName
                        Case "Required"
                            profProperty.Required = propertyValue
                        Case "Visible"
                            profProperty.Visible = propertyValue
                    End Select
                Next
            Else
                'Update the indexed property
                profProperty = ProfileProperties(index)
                Select Case propertyName
                    Case "Required"
                        profProperty.Required = propertyValue
                    Case "Visible"
                        profProperty.Visible = propertyValue
                End Select
            End If

            BindGrid()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' grdProfileProperties_ItemCommand runs when a Command event is raised in the
        ''' Grid
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/23/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub grdProfileProperties_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdProfileProperties.ItemCommand
            Dim commandName As String = e.CommandName
            Dim commandArgument As Integer = CType(e.CommandArgument, Integer)
            Dim index As Integer = e.Item.ItemIndex

            Select Case commandName
                Case "Delete"
                    DeleteProperty(index)
                Case "MoveUp"
                    MovePropertyUp(index)
                Case "MoveDown"
                    MovePropertyDown(index)
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' When it is determined that the client supports a rich interactivity the grdProfileProperties_ItemCreated 
        ''' event is responsible for disabling all the unneeded AutoPostBacks, along with assiging the appropriate
        '''	client-side script for each event handler
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [Jon Henning]	03/12/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub grdProfileProperties_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdProfileProperties.ItemCreated
            If SupportsRichClient() Then
                Select Case e.Item.ItemType
                    Case ListItemType.Header
                        'we combined the header label and checkbox in same place, so it is control 1 instead of 0
                        CType(e.Item.Cells(COLUMN_REQUIRED).Controls(1), WebControl).Attributes.Add("onclick", "dnn.util.checkallChecked(this," & COLUMN_REQUIRED & ");")
                        CType(e.Item.Cells(COLUMN_REQUIRED).Controls(1), CheckBox).AutoPostBack = False
                        CType(e.Item.Cells(COLUMN_VISIBLE).Controls(1), WebControl).Attributes.Add("onclick", "dnn.util.checkallChecked(this," & COLUMN_VISIBLE & ");")
                        CType(e.Item.Cells(COLUMN_VISIBLE).Controls(1), CheckBox).AutoPostBack = False
                    Case ListItemType.AlternatingItem, ListItemType.Item
                        CType(e.Item.Cells(COLUMN_REQUIRED).Controls(0), CheckBox).AutoPostBack = False
                        CType(e.Item.Cells(COLUMN_VISIBLE).Controls(0), CheckBox).AutoPostBack = False

                        DotNetNuke.UI.Utilities.ClientAPI.EnableClientSideReorder(e.Item.Cells(COLUMN_MOVE_DOWN).Controls(0), Me.Page, False, Me.grdProfileProperties.ClientID)
                        DotNetNuke.UI.Utilities.ClientAPI.EnableClientSideReorder(e.Item.Cells(COLUMN_MOVE_UP).Controls(0), Me.Page, True, Me.grdProfileProperties.ClientID)
                End Select
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' grdProfileProperties_ItemDataBound runs when a row in the grid is bound to its data source
        ''' Grid
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/06/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub grdProfileProperties_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdProfileProperties.ItemDataBound
            Dim item As DataGridItem = e.Item

            If item.ItemType = ListItemType.Item Or _
                    item.ItemType = ListItemType.AlternatingItem Or _
                    item.ItemType = ListItemType.SelectedItem Then

                Dim imgColumnControl As Control = item.Controls(1).Controls(0)
                If TypeOf imgColumnControl Is ImageButton Then
                    Dim delImage As ImageButton = CType(imgColumnControl, ImageButton)
                    Dim profProperty As ProfilePropertyDefinition = CType(item.DataItem, ProfilePropertyDefinition)

                    Select Case profProperty.PropertyName.ToLower
                        Case "lastname", "firstname", "timezone", "preferredlocale"
                            delImage.Visible = False
                        Case Else
                            delImage.Visible = True
                    End Select
                End If
            End If
        End Sub

#End Region

#Region "Optional Interfaces"

        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                Actions.Add(GetNextActionID, Services.Localization.Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", EditUrl("EditProfileProperty"), False, SecurityAccessLevel.Admin, True, False)

                Actions.Add(GetNextActionID, Localization.GetString("Cancel.Action", LocalResourceFile), ModuleActionType.AddContent, "", "lt.gif", ReturnUrl, False, SecurityAccessLevel.Admin, True, False)
                Return Actions
            End Get
        End Property

#End Region

    End Class

End Namespace
