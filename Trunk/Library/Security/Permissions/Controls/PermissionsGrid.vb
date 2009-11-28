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
Imports System.Collections.Generic

Namespace DotNetNuke.Security.Permissions.Controls

    Public MustInherit Class PermissionsGrid
        Inherits Control
        Implements INamingContainer

        Private pnlPermissions As Panel
        Private lblGroups As Label
        Private WithEvents cboRoleGroups As DropDownList
        Private dgRolePermissions As DataGrid
        Private lblUser As Label
        Private txtUser As TextBox
        Private WithEvents cmdUser As CommandButton
        Private dgUserPermissions As DataGrid

#Region "Enums"

        Protected Const PermissionTypeGrant As String = "True"
        Protected Const PermissionTypeDeny As String = "False"
        Protected Const PermissionTypeNull As String = "Null"

#End Region

#Region "Private Members"

        Private _dtRolePermissions As New DataTable
        Private _dtUserPermissions As New DataTable
        Private _roles As ArrayList
        Private _users As ArrayList
        Private _permissions As ArrayList
        Private _resourceFile As String

#End Region

#Region "Protected Properties"

        Protected Overridable ReadOnly Property PermissionsList() As List(Of PermissionInfoBase)
            Get
                Return Nothing
            End Get
        End Property

        Protected Overridable ReadOnly Property RefreshGrid() As Boolean
            Get
                Return False
            End Get
        End Property

#End Region

#Region "Public Properties"

#Region "DataGrid Properties"

        Public ReadOnly Property AlternatingItemStyle() As TableItemStyle
            Get
                Return dgRolePermissions.AlternatingItemStyle
            End Get
        End Property

        Public Property AutoGenerateColumns() As Boolean
            Get
                Return dgRolePermissions.AutoGenerateColumns
            End Get
            Set(ByVal Value As Boolean)
                dgRolePermissions.AutoGenerateColumns = Value
                dgUserPermissions.AutoGenerateColumns = Value
            End Set
        End Property

        Public Property CellSpacing() As Integer
            Get
                Return dgRolePermissions.CellSpacing
            End Get
            Set(ByVal Value As Integer)
                dgRolePermissions.CellSpacing = Value
                dgUserPermissions.CellSpacing = Value
            End Set
        End Property

        Public ReadOnly Property Columns() As DataGridColumnCollection
            Get
                Return dgRolePermissions.Columns()
            End Get
        End Property

        Public ReadOnly Property FooterStyle() As TableItemStyle
            Get
                Return dgRolePermissions.FooterStyle
            End Get
        End Property

        Public Property GridLines() As GridLines
            Get
                Return dgRolePermissions.GridLines
            End Get
            Set(ByVal Value As GridLines)
                dgRolePermissions.GridLines = Value
                dgUserPermissions.GridLines = Value
            End Set
        End Property

        Public ReadOnly Property HeaderStyle() As TableItemStyle
            Get
                Return dgRolePermissions.HeaderStyle
            End Get
        End Property

        Public ReadOnly Property ItemStyle() As TableItemStyle
            Get
                Return dgRolePermissions.ItemStyle
            End Get
        End Property

        Public ReadOnly Property Items() As DataGridItemCollection
            Get
                Return dgRolePermissions.Items()
            End Get
        End Property

        Public ReadOnly Property SelectedItemStyle() As TableItemStyle
            Get
                Return dgRolePermissions.SelectedItemStyle
            End Get
        End Property

#End Region

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Id of the Administrator Role
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/16/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property AdministratorRoleId() As Integer
            Get
                Return PortalController.GetCurrentPortalSettings.AdministratorRoleId
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Id of the Registered Users Role
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/16/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property RegisteredUsersRoleId() As Integer
            Get
                Return PortalController.GetCurrentPortalSettings.RegisteredRoleId
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets whether a Dynamic Column has been added
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DynamicColumnAdded() As Boolean
            Get
                If ViewState("ColumnAdded") Is Nothing Then
                    Return False
                Else
                    Return True
                End If
            End Get
            Set(ByVal Value As Boolean)
                ViewState("ColumnAdded") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the underlying Permissions Data Table
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property dtRolePermissions() As DataTable
            Get
                Return _dtRolePermissions
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the underlying Permissions Data Table
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property dtUserPermissions() As DataTable
            Get
                Return _dtUserPermissions
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Id of the Portal
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/16/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property PortalId() As Integer
            Get
                ' Obtain PortalSettings from Current Context
                Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                Dim intPortalID As Integer

                If _portalSettings.ActiveTab.ParentId = _portalSettings.SuperTabId Then 'if we are in host filemanager then we need to pass a null portal id
                    intPortalID = Null.NullInteger
                Else
                    intPortalID = _portalSettings.PortalId
                End If

                Return intPortalID
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the collection of Roles to display
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Roles() As ArrayList
            Get
                Return _roles
            End Get
            Set(ByVal Value As ArrayList)
                _roles = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the ResourceFile to localize permissions
        ''' </summary>
        ''' <history>
        '''     [vmasanas]    02/24/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ResourceFile() As String
            Get
                Return _resourceFile
            End Get
            Set(ByVal Value As String)
                _resourceFile = Value
            End Set
        End Property
#End Region

#Region "Abstract Members"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generate the Data Grid
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public MustOverride Sub GenerateDataGrid()

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Bind the data to the controls
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindData()

            Me.EnsureChildControls()

            BindRolesGrid()
            BindUsersGrid()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Bind the Roles data to the Grid
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindRolesGrid()

            dtRolePermissions.Columns.Clear()
            dtRolePermissions.Rows.Clear()

            Dim col As DataColumn

            'Add Roles Column
            col = New DataColumn("RoleId")
            dtRolePermissions.Columns.Add(col)

            'Add Roles Column
            col = New DataColumn("RoleName")
            dtRolePermissions.Columns.Add(col)

            Dim i As Integer
            For i = 0 To _permissions.Count - 1
                Dim objPerm As Security.Permissions.PermissionInfo
                objPerm = CType(_permissions(i), Security.Permissions.PermissionInfo)

                'Add Enabled Column
                col = New DataColumn(objPerm.PermissionName & "_Enabled")
                dtRolePermissions.Columns.Add(col)

                'Add Permission Column
                col = New DataColumn(objPerm.PermissionName)
                dtRolePermissions.Columns.Add(col)
            Next

            GetRoles()

            UpdateRolePermissions()
            Dim row As DataRow
            For i = 0 To Roles.Count - 1
                Dim role As RoleInfo = DirectCast(Roles(i), RoleInfo)
                row = dtRolePermissions.NewRow
                row("RoleId") = role.RoleID
                row("RoleName") = Localization.LocalizeRole(role.RoleName)

                Dim j As Integer
                For j = 0 To _permissions.Count - 1
                    Dim objPerm As Security.Permissions.PermissionInfo
                    objPerm = CType(_permissions(j), Security.Permissions.PermissionInfo)

                    row(objPerm.PermissionName & "_Enabled") = GetEnabled(objPerm, role, j + 1)
                    If SupportsDenyPermissions() Then
                        row(objPerm.PermissionName) = GetPermission(objPerm, role, j + 1, PermissionTypeNull)
                    Else
                        If GetPermission(objPerm, role, j + 1) Then
                            row(objPerm.PermissionName) = PermissionTypeGrant
                        Else
                            row(objPerm.PermissionName) = PermissionTypeNull
                        End If
                    End If
                Next
                dtRolePermissions.Rows.Add(row)
            Next

            dgRolePermissions.DataSource = dtRolePermissions
            dgRolePermissions.DataBind()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Bind the Users data to the Grid
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindUsersGrid()

            dtUserPermissions.Columns.Clear()
            dtUserPermissions.Rows.Clear()

            Dim col As DataColumn

            'Add Roles Column
            col = New DataColumn("UserId")
            dtUserPermissions.Columns.Add(col)

            'Add Roles Column
            col = New DataColumn("DisplayName")
            dtUserPermissions.Columns.Add(col)

            Dim i As Integer
            For i = 0 To _permissions.Count - 1
                Dim objPerm As Security.Permissions.PermissionInfo
                objPerm = CType(_permissions(i), Security.Permissions.PermissionInfo)

                'Add Enabled Column
                col = New DataColumn(objPerm.PermissionName & "_Enabled")
                dtUserPermissions.Columns.Add(col)

                'Add Permission Column
                col = New DataColumn(objPerm.PermissionName)
                dtUserPermissions.Columns.Add(col)
            Next

            If Not dgUserPermissions Is Nothing Then

                _users = GetUsers()

                If _users.Count <> 0 Then
                    dgUserPermissions.Visible = True

                    UpdateUserPermissions()

                    Dim row As DataRow
                    For i = 0 To _users.Count - 1
                        Dim user As UserInfo = DirectCast(_users(i), UserInfo)
                        row = dtUserPermissions.NewRow
                        row("UserId") = user.UserID
                        row("DisplayName") = user.DisplayName

                        Dim j As Integer
                        For j = 0 To _permissions.Count - 1
                            Dim objPerm As Security.Permissions.PermissionInfo
                            objPerm = CType(_permissions(j), Security.Permissions.PermissionInfo)

                            row(objPerm.PermissionName & "_Enabled") = GetEnabled(objPerm, user, j + 1)
                            If SupportsDenyPermissions() Then
                                row(objPerm.PermissionName) = GetPermission(objPerm, user, j + 1, PermissionTypeNull)
                            Else
                                If GetPermission(objPerm, user, j + 1) Then
                                    row(objPerm.PermissionName) = PermissionTypeGrant
                                Else
                                    row(objPerm.PermissionName) = PermissionTypeNull
                                End If
                            End If
                        Next
                        dtUserPermissions.Rows.Add(row)
                    Next

                    dgUserPermissions.DataSource = dtUserPermissions
                    dgUserPermissions.DataBind()
                Else
                    dgUserPermissions.Visible = False
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the roles from the Database and loads them into the Roles property
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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
                r.RoleID = Integer.Parse(glbRoleUnauthUser)
                r.RoleName = glbRoleUnauthUserName
                _roles.Add(r)
                r = New RoleInfo
                r.RoleID = Integer.Parse(glbRoleAllUsers)
                r.RoleName = glbRoleAllUsersName
                _roles.Add(r)
            End If
            _roles.Reverse()
            _roles.Sort(New RoleComparer)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets up the columns for the Grid
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SetUpRolesGrid()

            dgRolePermissions.Columns.Clear()

            Dim textCol As New BoundColumn
            textCol.HeaderText = "&nbsp;"
            textCol.DataField = "RoleName"
			textCol.ItemStyle.Width = Unit.Parse("150px")
			textCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right
            dgRolePermissions.Columns.Add(textCol)

            Dim idCol As New BoundColumn
            idCol.HeaderText = ""
            idCol.DataField = "roleid"
            idCol.Visible = False
            dgRolePermissions.Columns.Add(idCol)

            Dim templateCol As TemplateColumn

            Dim objPermission As Security.Permissions.PermissionInfo
            For Each objPermission In _permissions
                templateCol = New TemplateColumn

                Dim columnTemplate As New DNNMultiStateBoxColumnTemplate
                columnTemplate.DataField = objPermission.PermissionName
                columnTemplate.EnabledField = objPermission.PermissionName & "_Enabled"
                columnTemplate.ImagePath = "~/images/"
                columnTemplate.States.Add(New DNNMultiState(PermissionTypeGrant, "", "grant.gif", "lock.gif", Localization.GetString("PermissionTypeGrant")))
                If SupportsDenyPermissions() Then
                    columnTemplate.States.Add(New DNNMultiState(PermissionTypeDeny, "", "deny.gif", "lock.gif", Localization.GetString("PermissionTypeDeny")))
                End If
                columnTemplate.States.Add(New DNNMultiState(PermissionTypeNull, "", "unchecked.gif", "lock.gif", Localization.GetString("PermissionTypeNull")))

                templateCol.ItemTemplate = columnTemplate
                Dim locName As String = ""
                If objPermission.ModuleDefID > 0 Then
                    If ResourceFile <> "" Then
                        ' custom permission
                        locName = Services.Localization.Localization.GetString(objPermission.PermissionName + ".Permission", ResourceFile)
                    End If
                Else
                    ' system permission
                    locName = Services.Localization.Localization.GetString(objPermission.PermissionName + ".Permission", PermissionProvider.Instance.LocalResourceFile)
                End If
                templateCol.HeaderText = IIf(locName <> "", locName, objPermission.PermissionName).ToString
				templateCol.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
				templateCol.HeaderStyle.VerticalAlign = VerticalAlign.Bottom
				templateCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center
				templateCol.ItemStyle.Width = Unit.Parse("70px")
                templateCol.HeaderStyle.Wrap = True
                dgRolePermissions.Columns.Add(templateCol)
            Next

        End Sub

        Private Sub SetUpUsersGrid()

            If Not dgUserPermissions Is Nothing Then
                dgUserPermissions.Columns.Clear()

                Dim textCol As New BoundColumn
                textCol.HeaderText = "&nbsp;"
                textCol.DataField = "DisplayName"
				textCol.ItemStyle.Width = Unit.Parse("150px")
				textCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right
				dgUserPermissions.Columns.Add(textCol)

                Dim idCol As New BoundColumn
                idCol.HeaderText = ""
                idCol.DataField = "userid"
                idCol.Visible = False
                dgUserPermissions.Columns.Add(idCol)

                Dim templateCol As TemplateColumn

                Dim objPermission As Security.Permissions.PermissionInfo
                For Each objPermission In _permissions
                    templateCol = New TemplateColumn

                    Dim columnTemplate As New DNNMultiStateBoxColumnTemplate
                    columnTemplate.DataField = objPermission.PermissionName
                    columnTemplate.EnabledField = objPermission.PermissionName & "_Enabled"
                    columnTemplate.ImagePath = "~/images/"
                    columnTemplate.States.Add(New DNNMultiState(PermissionTypeGrant, "", "grant.gif", "lock.gif", Localization.GetString("PermissionTypeGrant")))
                    If SupportsDenyPermissions() Then
                        columnTemplate.States.Add(New DNNMultiState(PermissionTypeDeny, "", "deny.gif", "lock.gif", Localization.GetString("PermissionTypeDeny")))
                    End If
                    columnTemplate.States.Add(New DNNMultiState(PermissionTypeNull, "", "unchecked.gif", "lock.gif", Localization.GetString("PermissionTypeNull")))

                    templateCol.ItemTemplate = columnTemplate
                    Dim locName As String = ""
                    If objPermission.ModuleDefID > 0 Then
                        If ResourceFile <> "" Then
                            ' custom permission
                            locName = Services.Localization.Localization.GetString(objPermission.PermissionName + ".Permission", ResourceFile)
                        End If
                    Else
                        ' system permission
                        locName = Services.Localization.Localization.GetString(objPermission.PermissionName + ".Permission", PermissionProvider.Instance.LocalResourceFile)
                    End If
                    templateCol.HeaderText = IIf(locName <> "", locName, objPermission.PermissionName).ToString
					templateCol.HeaderStyle.VerticalAlign = VerticalAlign.Bottom
					templateCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center
					templateCol.ItemStyle.Width = Unit.Parse("70px")
                    templateCol.HeaderStyle.Wrap = True
                    dgUserPermissions.Columns.Add(templateCol)
                Next
            End If

        End Sub


#End Region

#Region "Protected Methods"

        Protected Overridable Sub AddPermission(ByVal permission As PermissionInfo, ByVal roleId As Integer, ByVal roleName As String, ByVal userId As Integer, ByVal displayName As String, ByVal allowAccess As Boolean)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a Permission
        ''' </summary>
        ''' <param name="permissions">The permissions collection</param>
        ''' <param name="user">The user to add</param>
        ''' <history>
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub AddPermission(ByVal permissions As ArrayList, ByVal user As UserInfo)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds the key used to store the "permission" information in the ViewState
        ''' </summary>
        ''' <param name="allowAccess">The type of permission ( grant / deny )</param>
        ''' <param name="permissionId">The Id of the permission</param>
        ''' <param name="objectPermissionId">The Id of the object permission</param>
        ''' <param name="roleId">The role id</param>
        ''' <param name="roleName">The role name</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function BuildKey(ByVal allowAccess As Boolean, ByVal permissionId As Integer, ByVal objectPermissionId As Integer, ByVal roleId As Integer, ByVal roleName As String) As String
            Return BuildKey(allowAccess, permissionId, objectPermissionId, roleId, roleName, Null.NullInteger, Null.NullString)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds the key used to store the "permission" information in the ViewState
        ''' </summary>
        ''' <param name="allowAccess">The type of permission ( grant / deny )</param>
        ''' <param name="permissionId">The Id of the permission</param>
        ''' <param name="objectPermissionId">The Id of the object permission</param>
        ''' <param name="roleId">The role id</param>
        ''' <param name="roleName">The role name</param>
        ''' <param name="userId">The user id</param>
        ''' <param name="displayName">The user display name</param>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function BuildKey(ByVal allowAccess As Boolean, ByVal permissionId As Integer, ByVal objectPermissionId As Integer, ByVal roleId As Integer, ByVal roleName As String, ByVal userID As Integer, ByVal displayName As String) As String

            Dim key As String

            If allowAccess Then
                key = "True"
            Else
                key = "False"
            End If

            key += "|" + Convert.ToString(permissionId)

            'Add objectPermissionId
            key += "|"
            If objectPermissionId > -1 Then
                key += Convert.ToString(objectPermissionId)
            End If

            key += "|" + roleName
            key += "|" + roleId.ToString
            key += "|" + userID.ToString
            key += "|" + displayName.ToString

            Return key

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates the Child Controls
        ''' </summary>
        ''' <history>
        '''     [cnurse]    02/23/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub CreateChildControls()

            ' get data
            _permissions = GetPermissions()

            pnlPermissions = New Panel
            pnlPermissions.CssClass = "DataGrid_Container"

            'Optionally Add Role Group Filter
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim arrGroups As ArrayList = RoleController.GetRoleGroups(_portalSettings.PortalId)
            If arrGroups.Count > 0 Then
                lblGroups = New Label
                lblGroups.Text = Localization.GetString("RoleGroupFilter")
				lblGroups.CssClass = "SubHead"
                pnlPermissions.Controls.Add(lblGroups)

                pnlPermissions.Controls.Add(New LiteralControl("&nbsp;&nbsp;"))

                cboRoleGroups = New DropDownList
                cboRoleGroups.AutoPostBack = True

                cboRoleGroups.Items.Add(New ListItem(Localization.GetString("AllRoles"), "-2"))
                Dim liItem As ListItem = New ListItem(Localization.GetString("GlobalRoles"), "-1")
                liItem.Selected = True
                cboRoleGroups.Items.Add(liItem)
                For Each roleGroup As RoleGroupInfo In arrGroups
                    cboRoleGroups.Items.Add(New ListItem(roleGroup.RoleGroupName, roleGroup.RoleGroupID.ToString))
                Next
                pnlPermissions.Controls.Add(cboRoleGroups)

                pnlPermissions.Controls.Add(New LiteralControl("<br/><br/>"))
            End If

            dgRolePermissions = New DataGrid
            dgRolePermissions.AutoGenerateColumns = False
			dgRolePermissions.CellSpacing = 0
			dgRolePermissions.CellPadding = 2
            dgRolePermissions.GridLines = GridLines.None
            dgRolePermissions.FooterStyle.CssClass = "DataGrid_Footer"
            dgRolePermissions.HeaderStyle.CssClass = "DataGrid_Header"
            dgRolePermissions.ItemStyle.CssClass = "DataGrid_Item"
            dgRolePermissions.AlternatingItemStyle.CssClass = "DataGrid_AlternatingItem"
            SetUpRolesGrid()
            pnlPermissions.Controls.Add(dgRolePermissions)

            _users = GetUsers()

            If Not _users Is Nothing Then
                dgUserPermissions = New DataGrid
                dgUserPermissions.AutoGenerateColumns = False
                dgUserPermissions.CellSpacing = 0
                dgUserPermissions.GridLines = GridLines.None
                dgUserPermissions.FooterStyle.CssClass = "DataGrid_Footer"
                dgUserPermissions.HeaderStyle.CssClass = "DataGrid_Header"
                dgUserPermissions.ItemStyle.CssClass = "DataGrid_Item"
                dgUserPermissions.AlternatingItemStyle.CssClass = "DataGrid_AlternatingItem"
                SetUpUsersGrid()
                pnlPermissions.Controls.Add(dgUserPermissions)

                pnlPermissions.Controls.Add(New LiteralControl("<br/>"))

                lblUser = New Label
                lblUser.Text = Localization.GetString("User")
                lblUser.CssClass = "SubHead"
                pnlPermissions.Controls.Add(lblUser)

                pnlPermissions.Controls.Add(New LiteralControl("&nbsp;&nbsp;"))

                txtUser = New TextBox
                txtUser.CssClass = "NormalTextBox"
                pnlPermissions.Controls.Add(txtUser)

                pnlPermissions.Controls.Add(New LiteralControl("&nbsp;&nbsp;"))

                cmdUser = New CommandButton
                cmdUser.Text = Localization.GetString("Add")
                cmdUser.CssClass = "CommandButton"
                cmdUser.ImageUrl = "~/images/add.gif"
                pnlPermissions.Controls.Add(cmdUser)
            End If

            Me.Controls.Add(pnlPermissions)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Enabled status of the permission
        ''' </summary>
        ''' <param name="objPerm">The permission being loaded</param>
        ''' <param name="role">The role</param>
        ''' <param name="column">The column of the Grid</param>
        ''' <history>
        '''     [cnurse]    01/13/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function GetEnabled(ByVal objPerm As PermissionInfo, ByVal role As RoleInfo, ByVal column As Integer) As Boolean
            Return True
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Enabled status of the permission
        ''' </summary>
        ''' <param name="objPerm">The permission being loaded</param>
        ''' <param name="user">The user</param>
        ''' <param name="column">The column of the Grid</param>
        ''' <history>
        '''     [cnurse]    01/13/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function GetEnabled(ByVal objPerm As PermissionInfo, ByVal user As UserInfo, ByVal column As Integer) As Boolean
            Return True
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Value of the permission
        ''' </summary>
        ''' <param name="objPerm">The permission being loaded</param>
        ''' <param name="role">The role</param>
        ''' <param name="column">The column of the Grid</param>
        ''' <history>
        '''     [cnurse]    01/13/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function GetPermission(ByVal objPerm As PermissionInfo, ByVal role As RoleInfo, ByVal column As Integer) As Boolean
            Return Convert.ToBoolean(GetPermission(objPerm, role, column, PermissionTypeDeny))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Value of the permission
        ''' </summary>
        ''' <param name="objPerm">The permission being loaded</param>
        ''' <param name="role">The role</param>
        ''' <param name="column">The column of the Grid</param>
        ''' <history>
        '''     [cnurse]    01/13/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function GetPermission(ByVal objPerm As PermissionInfo, ByVal role As RoleInfo, ByVal column As Integer, ByVal defaultState As String) As String
            Dim stateKey As String = defaultState
            If PermissionsList IsNot Nothing Then
                For Each permission As PermissionInfoBase In PermissionsList
                    If permission.PermissionID = objPerm.PermissionID AndAlso permission.RoleID = role.RoleID Then
                        If permission.AllowAccess Then
                            stateKey = PermissionTypeGrant
                        Else
                            stateKey = PermissionTypeDeny
                        End If
                        Exit For
                    End If
                Next
            End If
            Return stateKey
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Value of the permission
        ''' </summary>
        ''' <param name="objPerm">The permission being loaded</param>
        ''' <param name="user">The user</param>
        ''' <param name="column">The column of the Grid</param>
        ''' <history>
        '''     [cnurse]    01/13/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function GetPermission(ByVal objPerm As PermissionInfo, ByVal user As UserInfo, ByVal column As Integer) As Boolean
            Return Convert.ToBoolean(GetPermission(objPerm, user, column, PermissionTypeDeny))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Value of the permission
        ''' </summary>
        ''' <param name="objPerm">The permission being loaded</param>
        ''' <param name="user">The user</param>
        ''' <param name="column">The column of the Grid</param>
        ''' <history>
        '''     [cnurse]    01/13/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function GetPermission(ByVal objPerm As PermissionInfo, ByVal user As UserInfo, ByVal column As Integer, ByVal defaultState As String) As String
            Dim stateKey As String = defaultState
            If PermissionsList IsNot Nothing Then
                For Each permission As PermissionInfoBase In PermissionsList
                    If permission.PermissionID = objPerm.PermissionID AndAlso permission.UserID = user.UserID Then
                        If permission.AllowAccess Then
                            stateKey = PermissionTypeGrant
                        Else
                            stateKey = PermissionTypeDeny
                        End If
                        Exit For
                    End If
                Next
            End If
            Return stateKey
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the permissions from the Database
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function GetPermissions() As ArrayList
            Return Nothing
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the users from the Database
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function GetUsers() As ArrayList
            Dim arrUsers As New ArrayList
            Dim objUser As UserInfo
            Dim blnExists As Boolean

            If PermissionsList IsNot Nothing Then
                For Each permission As PermissionInfoBase In PermissionsList
                    If Not Null.IsNull(permission.UserID) Then
                        blnExists = False
                        For Each objUser In arrUsers
                            If permission.UserID = objUser.UserID Then
                                blnExists = True
                            End If
                        Next
                        If Not blnExists Then
                            objUser = New UserInfo
                            objUser.UserID = permission.UserID
                            objUser.Username = permission.Username
                            objUser.DisplayName = permission.DisplayName
                            arrUsers.Add(objUser)
                        End If
                    End If
                Next
            End If
            Return arrUsers
        End Function

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

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

        Protected Overridable Sub ParsePermissionKeys(ByVal permission As PermissionInfoBase, ByVal Settings As String())
            permission.PermissionID = Convert.ToInt32(Settings(1))
            permission.RoleID = Convert.ToInt32(Settings(4))
            permission.RoleName = Settings(3)
            permission.AllowAccess = Convert.ToBoolean(Settings(0))
            permission.UserID = Convert.ToInt32(Settings(5))
            permission.DisplayName = Settings(6)
        End Sub

        Protected Overridable Sub RemovePermission(ByVal permissionID As Integer, ByVal roleID As Integer, ByVal userID As Integer)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a Permission
        ''' </summary>
        ''' <param name="permission">The permission being updated</param>
        ''' <param name="roleName">The name of the role</param>
        ''' <param name="allowAccess">The value of the permission</param>
        ''' <history>
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub UpdatePermission(ByVal permission As PermissionInfo, ByVal roleId As Integer, ByVal roleName As String, ByVal allowAccess As Boolean)
            If allowAccess Then
                UpdatePermission(permission, roleId, roleName, PermissionTypeGrant)
            Else
                UpdatePermission(permission, roleId, roleName, PermissionTypeNull)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a Permission
        ''' </summary>
        ''' <param name="permission">The permission being updated</param>
        ''' <param name="roleName">The name of the role</param>
        ''' <param name="stateKey">The permission state</param>
        ''' <history>
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub UpdatePermission(ByVal permission As PermissionInfo, ByVal roleId As Integer, ByVal roleName As String, ByVal stateKey As String)
            RemovePermission(permission.PermissionID, roleId, Null.NullInteger)
            Select Case stateKey
                Case PermissionTypeGrant
                    AddPermission(permission, roleId, roleName, Null.NullInteger, Null.NullString, True)
                Case PermissionTypeDeny
                    AddPermission(permission, roleId, roleName, Null.NullInteger, Null.NullString, False)
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a Permission
        ''' </summary>
        ''' <param name="permission">The permission being updated</param>
        ''' <param name="displayName">The user's displayname</param>
        ''' <param name="userId">The user's id</param>
        ''' <param name="allowAccess">The value of the permission</param>
        ''' <history>
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub UpdatePermission(ByVal permission As PermissionInfo, ByVal displayName As String, ByVal userId As Integer, ByVal allowAccess As Boolean)
            If allowAccess Then
                UpdatePermission(permission, displayName, userId, PermissionTypeGrant)
            Else
                UpdatePermission(permission, displayName, userId, PermissionTypeNull)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a Permission
        ''' </summary>
        ''' <param name="permission">The permission being updated</param>
        ''' <param name="displayName">The user's displayname</param>
        ''' <param name="userId">The user's id</param>
        ''' <param name="stateKey">The permission state</param>
        ''' <history>
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub UpdatePermission(ByVal permission As PermissionInfo, ByVal displayName As String, ByVal userId As Integer, ByVal stateKey As String)
            RemovePermission(permission.PermissionID, Integer.Parse(glbRoleNothing), userId)
            Select Case stateKey
                Case PermissionTypeGrant
                    AddPermission(permission, Integer.Parse(glbRoleNothing), Null.NullString, userId, displayName, True)
                Case PermissionTypeDeny
                    AddPermission(permission, Integer.Parse(glbRoleNothing), Null.NullString, userId, displayName, False)
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' returns whether or not the derived grid supports Deny permissions
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function SupportsDenyPermissions() As Boolean
            Return False ' to support Deny permissions a derived grid typically needs to implement the new GetPermission and UpdatePermission overload methods which support StateKey
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates the permissions
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub UpdatePermissions()
            Me.EnsureChildControls()

            UpdateRolePermissions()
            UpdateUserPermissions()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates the permissions
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub UpdateRolePermissions()
            If dgRolePermissions IsNot Nothing AndAlso Not RefreshGrid Then
                Dim dgi As DataGridItem
                For Each dgi In dgRolePermissions.Items
                    Dim i As Integer
                    For i = 2 To dgi.Cells.Count - 1
                        'all except first two cells which is role names and role ids
                        If dgi.Cells(i).Controls.Count > 0 Then
                            Dim ms As DNNMultiStateBox = CType(dgi.Cells(i).Controls(0), DNNMultiStateBox)
                            If SupportsDenyPermissions() Then
                                UpdatePermission(CType(_permissions(i - 2), PermissionInfo), Integer.Parse(dgi.Cells(1).Text), dgi.Cells(0).Text, ms.SelectedStateKey)
                            Else
                                UpdatePermission(CType(_permissions(i - 2), PermissionInfo), Integer.Parse(dgi.Cells(1).Text), dgi.Cells(0).Text, ms.SelectedStateKey = PermissionTypeGrant)
                            End If
                        End If
                    Next
                Next
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates the permissions
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub UpdateUserPermissions()
            If dgUserPermissions IsNot Nothing AndAlso Not RefreshGrid Then
                Dim dgi As DataGridItem
                For Each dgi In dgUserPermissions.Items
                    Dim i As Integer
                    For i = 2 To dgi.Cells.Count - 1
                        'all except first two cells which is displayname and userid
                        If dgi.Cells(i).Controls.Count > 0 Then
                            Dim ms As DNNMultiStateBox = CType(dgi.Cells(i).Controls(0), DNNMultiStateBox)
                            If SupportsDenyPermissions() Then
                                UpdatePermission(CType(_permissions(i - 2), PermissionInfo), dgi.Cells(0).Text, Integer.Parse(dgi.Cells(1).Text), ms.SelectedStateKey)
                            Else
                                UpdatePermission(CType(_permissions(i - 2), PermissionInfo), dgi.Cells(0).Text, Integer.Parse(dgi.Cells(1).Text), ms.SelectedStateKey = PermissionTypeGrant)
                            End If
                        End If
                    Next
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

            UpdatePermissions()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddUser runs when the Add user linkbutton is clicked
        ''' </summary>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddUser(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUser.Click

            UpdatePermissions()

            If txtUser.Text <> "" Then
                ' verify username
                Dim objUser As UserInfo = UserController.GetCachedUser(PortalId, txtUser.Text)
                If Not objUser Is Nothing Then
                    AddPermission(_permissions, objUser)
                    BindData()
                Else
                    ' user does not exist
                    lblUser = New Label
                    lblUser.Text = "<br>" & Localization.GetString("InvalidUserName")
                    lblUser.CssClass = "NormalRed"
                    pnlPermissions.Controls.Add(lblUser)
                End If
            End If

        End Sub

#End Region

    End Class


End Namespace