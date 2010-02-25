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

Imports System
Imports System.Drawing
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Collections
Imports System.Data

Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.UI.WebControls

    Public Class RolesSelectionGrid
        Inherits Control
        Implements INamingContainer

        Private pnlRoleSlections As Panel
        Private lblGroups As Label
        Private WithEvents cboRoleGroups As DropDownList
        Private dgRoleSelection As DataGrid

#Region "Private Members"

        Private _dtRoleSelections As New DataTable
        Private _roles As ArrayList
        Private _resourceFile As String
        Private _selectedRoles As ArrayList

#End Region

#Region "Public Properties"

#Region "DataGrid Properties"

        Public ReadOnly Property AlternatingItemStyle() As TableItemStyle
            Get
                Return dgRoleSelection.AlternatingItemStyle
            End Get
        End Property

        Public Property AutoGenerateColumns() As Boolean
            Get
                Return dgRoleSelection.AutoGenerateColumns
            End Get
            Set(ByVal Value As Boolean)
                dgRoleSelection.AutoGenerateColumns = Value
            End Set
        End Property

        Public Property CellSpacing() As Integer
            Get
                Return dgRoleSelection.CellSpacing
            End Get
            Set(ByVal Value As Integer)
                dgRoleSelection.CellSpacing = Value
            End Set
        End Property

        Public ReadOnly Property Columns() As DataGridColumnCollection
            Get
                Return dgRoleSelection.Columns()
            End Get
        End Property

        Public ReadOnly Property FooterStyle() As TableItemStyle
            Get
                Return dgRoleSelection.FooterStyle
            End Get
        End Property

        Public Property GridLines() As GridLines
            Get
                Return dgRoleSelection.GridLines
            End Get
            Set(ByVal Value As GridLines)
                dgRoleSelection.GridLines = Value
            End Set
        End Property

        Public ReadOnly Property HeaderStyle() As TableItemStyle
            Get
                Return dgRoleSelection.HeaderStyle
            End Get
        End Property

        Public ReadOnly Property ItemStyle() As TableItemStyle
            Get
                Return dgRoleSelection.ItemStyle
            End Get
        End Property

        Public ReadOnly Property Items() As DataGridItemCollection
            Get
                Return dgRoleSelection.Items()
            End Get
        End Property

        Public ReadOnly Property SelectedItemStyle() As TableItemStyle
            Get
                Return dgRoleSelection.SelectedItemStyle
            End Get
        End Property

#End Region

        ''' <summary>
        ''' List of the Names of the selected Roles
        ''' </summary>
        Public Property SelectedRoleNames() As ArrayList
            Get
                UpdateRoleSelections()
                Return (CurrentRoleSelection)
            End Get
            Set(ByVal value As ArrayList)
                CurrentRoleSelection = value
            End Set
        End Property

        ''' <summary>
        ''' Gets and Sets the ResourceFile to localize permissions
        ''' </summary>
        Public Property ResourceFile() As String
            Get
                Return _resourceFile
            End Get
            Set(ByVal Value As String)
                _resourceFile = Value
            End Set
        End Property

        ''' <summary>
        ''' Enable ShowAllUsers to display the virtuell "Unauthenticated Users" role 
        ''' </summary>
        Public Property ShowUnauthenticatedUsers() As Boolean
            Get
                If ViewState("ShowUnauthenticatedUsers") Is Nothing Then
                    Return False
                Else
                    Return CBool(ViewState("ShowUnauthenticatedUsers"))
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("ShowUnauthenticatedUsers") = value
            End Set
        End Property

        ''' <summary>
        ''' Enable ShowAllUsers to display the virtuell "All Users" role 
        ''' </summary>
        Public Property ShowAllUsers() As Boolean
            Get
                If ViewState("ShowAllUsers") Is Nothing Then
                    Return False
                Else
                    Return CBool(ViewState("ShowAllUsers"))
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("ShowAllUsers") = value
            End Set
        End Property

#End Region

#Region "Private Properties"
        Private ReadOnly Property dtRolesSelection() As DataTable
            Get
                Return _dtRoleSelections
            End Get
        End Property

        Private Property Roles() As ArrayList
            Get
                Return _roles
            End Get
            Set(ByVal Value As ArrayList)
                _roles = Value
            End Set
        End Property

        Private Property CurrentRoleSelection() As ArrayList
            Get
                If _selectedRoles Is Nothing Then _selectedRoles = New ArrayList
                Return _selectedRoles
            End Get
            Set(ByVal value As ArrayList)
                _selectedRoles = value
            End Set
        End Property


#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Bind the data to the controls
        ''' </summary>
        Private Sub BindData()

            Me.EnsureChildControls()

            BindRolesGrid()

        End Sub

        ''' <summary>
        ''' Bind the Roles data to the Grid
        ''' </summary>
        Private Sub BindRolesGrid()

            dtRolesSelection.Columns.Clear()
            dtRolesSelection.Rows.Clear()

            Dim col As DataColumn

            'Add Roles Column
            col = New DataColumn("RoleId", GetType(String))
            dtRolesSelection.Columns.Add(col)

            'Add Roles Column
            col = New DataColumn("RoleName", GetType(String))
            dtRolesSelection.Columns.Add(col)

            'Add Selected Column
            col = New DataColumn("Selected", GetType(Boolean))
            dtRolesSelection.Columns.Add(col)

            GetRoles()

            UpdateRoleSelections()
            Dim row As DataRow
            For i As Integer = 0 To Roles.Count - 1
                Dim role As RoleInfo = DirectCast(Roles(i), RoleInfo)
                row = dtRolesSelection.NewRow
                row("RoleId") = role.RoleID
                row("RoleName") = Localization.LocalizeRole(role.RoleName)
                row("Selected") = GetSelection(role.RoleName)

                dtRolesSelection.Rows.Add(row)
            Next

            dgRoleSelection.DataSource = dtRolesSelection
            dgRoleSelection.DataBind()

        End Sub

        Private Function GetSelection(ByVal roleName As String) As Boolean
            For Each r As String In CurrentRoleSelection
                If r = roleName Then Return True
            Next
            Return False
        End Function

        ''' <summary>
        ''' Gets the roles from the Database and loads them into the Roles property
        ''' </summary>
        Private Sub GetRoles()
            Dim objRoleController As New RoleController
            Dim RoleGroupId As Integer = -2
            If (Not cboRoleGroups Is Nothing) AndAlso (Not cboRoleGroups.SelectedValue Is Nothing) Then
                RoleGroupId = Integer.Parse(cboRoleGroups.SelectedValue)
            End If

            If RoleGroupId > -2 Then
                _roles = objRoleController.GetRolesByGroup(PortalController.GetCurrentPortalSettings.PortalId, RoleGroupId)
            Else
                _roles = objRoleController.GetPortalRoles(PortalController.GetCurrentPortalSettings.PortalId)
            End If

            If RoleGroupId < 0 Then
                Dim r As New RoleInfo
                If ShowUnauthenticatedUsers Then
                    r.RoleID = Integer.Parse(glbRoleUnauthUser)
                    r.RoleName = glbRoleUnauthUserName
                    _roles.Add(r)
                End If
                If ShowAllUsers Then
                    r = New RoleInfo
                    r.RoleID = Integer.Parse(glbRoleAllUsers)
                    r.RoleName = glbRoleAllUsersName
                    _roles.Add(r)
                End If
            End If
            _roles.Reverse()
            _roles.Sort(New RoleComparer)
        End Sub

        ''' <summary>
        ''' Sets up the columns for the Grid
        ''' </summary>
        Private Sub SetUpRolesGrid()

            dgRoleSelection.Columns.Clear()

            Dim textCol As New BoundColumn
            textCol.HeaderText = "&nbsp;"
            textCol.DataField = "RoleName"
            textCol.ItemStyle.Width = Unit.Parse("100px")
            dgRoleSelection.Columns.Add(textCol)

            Dim idCol As New BoundColumn
            idCol.HeaderText = ""
            idCol.DataField = "roleid"
            idCol.Visible = False
            dgRoleSelection.Columns.Add(idCol)

            Dim checkCol As TemplateColumn
            checkCol = New TemplateColumn
            Dim columnTemplate As New CheckBoxColumnTemplate
            columnTemplate.DataField = "Selected"
            checkCol.ItemTemplate = columnTemplate

            checkCol.HeaderText = Localization.GetString("SelectedRole")
            checkCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center
            checkCol.HeaderStyle.Wrap = True
            dgRoleSelection.Columns.Add(checkCol)
        End Sub
#End Region

#Region "Protected Methods"

        ''' <summary>
        ''' Load the ViewState
        ''' </summary>
        ''' <param name="savedState">The saved state</param>
        Protected Overrides Sub LoadViewState(ByVal savedState As Object)

            If Not (savedState Is Nothing) Then
                ' Load State from the array of objects that was saved with SaveViewState.

                Dim myState As Object() = CType(savedState, Object())

                'Load Base Controls ViewState
                If Not (myState(0) Is Nothing) Then
                    MyBase.LoadViewState(myState(0))
                End If
                'Load TabPermissions
                If Not (myState(1) Is Nothing) Then
                    Dim state As String = CStr(myState(1))
                    If state <> String.Empty Then
                        CurrentRoleSelection = New ArrayList(Split(state, "##"))
                    Else
                        CurrentRoleSelection = New ArrayList()
                    End If
                End If
            End If

        End Sub

        ''' <summary>
        ''' Saves the ViewState
        ''' </summary>
        Protected Overrides Function SaveViewState() As Object

            Dim allStates(1) As Object

            ' Save the Base Controls ViewState
            allStates(0) = MyBase.SaveViewState()
            'Persist the TabPermisisons
            Dim sb As New System.Text.StringBuilder
            Dim addDelimiter As Boolean = False
            For Each role As String In CurrentRoleSelection
                If addDelimiter Then
                    sb.Append("##")
                Else
                    addDelimiter = True
                End If
                sb.Append(role)
            Next
            allStates(1) = sb.ToString()
            Return allStates

        End Function

        ''' <summary>
        ''' Creates the Child Controls
        ''' </summary>
        Protected Overrides Sub CreateChildControls()
            pnlRoleSlections = New Panel
            pnlRoleSlections.CssClass = "DataGrid_Container"

            'Optionally Add Role Group Filter
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim arrGroups As ArrayList = RoleController.GetRoleGroups(_portalSettings.PortalId)
            If arrGroups.Count > 0 Then
                lblGroups = New Label
                lblGroups.Text = Localization.GetString("RoleGroupFilter")
                lblGroups.CssClass = "SubHead"
                pnlRoleSlections.Controls.Add(lblGroups)

                pnlRoleSlections.Controls.Add(New LiteralControl("&nbsp;&nbsp;"))

                cboRoleGroups = New DropDownList
                cboRoleGroups.AutoPostBack = True

                cboRoleGroups.Items.Add(New ListItem(Localization.GetString("AllRoles"), "-2"))
                Dim liItem As ListItem = New ListItem(Localization.GetString("GlobalRoles"), "-1")
                liItem.Selected = True
                cboRoleGroups.Items.Add(liItem)
                For Each roleGroup As RoleGroupInfo In arrGroups
                    cboRoleGroups.Items.Add(New ListItem(roleGroup.RoleGroupName, roleGroup.RoleGroupID.ToString))
                Next
                pnlRoleSlections.Controls.Add(cboRoleGroups)

                pnlRoleSlections.Controls.Add(New LiteralControl("<br/><br/>"))
            End If

            dgRoleSelection = New DataGrid
            dgRoleSelection.AutoGenerateColumns = False
            dgRoleSelection.CellSpacing = 0
            dgRoleSelection.GridLines = GridLines.None
            dgRoleSelection.FooterStyle.CssClass = "DataGrid_Footer"
            dgRoleSelection.HeaderStyle.CssClass = "DataGrid_Header"
            dgRoleSelection.ItemStyle.CssClass = "DataGrid_Item"
            dgRoleSelection.AlternatingItemStyle.CssClass = "DataGrid_AlternatingItem"
            SetUpRolesGrid()
            pnlRoleSlections.Controls.Add(dgRoleSelection)

            Me.Controls.Add(pnlRoleSlections)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Overrides the base OnPreRender method to Bind the Grid to the Permissions
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            BindData()
        End Sub

        ''' <summary>
        ''' Updates a Selection
        ''' </summary>
        ''' <param name="roleName">The name of the role</param>
        ''' <param name="Selected"></param>
        Protected Overridable Sub UpdateSelection(ByVal roleName As String, ByVal Selected As Boolean)
            Dim isMatch As Boolean = False

            For Each currentRoleName As String In CurrentRoleSelection
                If currentRoleName = roleName Then
                    'role is in collection
                    If Not Selected Then
                        'Remove from collection as we only keep selected roles
                        CurrentRoleSelection.Remove(currentRoleName)
                    End If
                    isMatch = True
                    Exit For
                End If
            Next

            'Rolename not found so add new
            If Not isMatch And Selected Then
                CurrentRoleSelection.Add(roleName)
            End If
        End Sub

        ''' <summary>
        ''' Updates the Selections
        ''' </summary>
        Protected Sub UpdateSelections()

            Me.EnsureChildControls()

            UpdateRoleSelections()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates the permissions
        ''' </summary>
        Protected Sub UpdateRoleSelections()

            If Not dgRoleSelection Is Nothing Then
                Dim dgi As DataGridItem
                For Each dgi In dgRoleSelection.Items
                    Dim i As Integer = 2

                    If dgi.Cells(i).Controls.Count > 0 Then
                        Dim cb As CheckBox = CType(dgi.Cells(i).Controls(0), CheckBox)
                        UpdateSelection(dgi.Cells(0).Text, cb.Checked)
                    End If
                Next
            End If

        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RoleGroupsSelectedIndexChanged runs when the Role Group is changed
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/06/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub RoleGroupsSelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboRoleGroups.SelectedIndexChanged

            UpdateSelections()

        End Sub


#End Region

    End Class


End Namespace