'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
' by DotNetNuke Corporation
'

Imports System
Imports System.Text
Imports System.Collections
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Common
Imports System.Collections.Generic
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users

Namespace DotNetNuke.Security.Permissions.Controls

    Public Class WorkflowStatePermissionsGrid
        Inherits PermissionsGrid

#Region "Private Members"

        Private _StateID As Integer = -1
        Private _WorkflowStatePermissions As WorkflowStatePermissionCollection
        Private _PermissionsList As List(Of PermissionInfoBase)
        Private _RefreshGrid As Boolean = Null.NullBoolean

#End Region

#Region "Protected Properties"

        Protected Overrides ReadOnly Property PermissionsList() As List(Of PermissionInfoBase)
            Get
                If _PermissionsList Is Nothing AndAlso _WorkflowStatePermissions IsNot Nothing Then
                    _PermissionsList = _WorkflowStatePermissions.ToList()
                End If
                Return _PermissionsList
            End Get
        End Property

        Protected Overrides ReadOnly Property RefreshGrid() As Boolean
            Get
                Return _RefreshGrid
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the StateID
        ''' </summary>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property StateID() As Integer
            Get
                Return _StateID
            End Get
            Set(ByVal Value As Integer)
                _StateID = Value
                _RefreshGrid = True
                GetWorkflowStatePermissions()
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Permission Collection
        ''' </summary>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Permissions() As Security.Permissions.WorkflowStatePermissionCollection
            Get
                'First Update Permissions in case they have been changed
                UpdatePermissions()

                'Return the WorkflowStatePermissions
                Return _WorkflowStatePermissions
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the WorkflowStatePermissions from the Data Store
        ''' </summary>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GetWorkflowStatePermissions()
            _WorkflowStatePermissions = New WorkflowStatePermissionCollection(WorkflowStatePermissionController.GetWorkflowStatePermissions(Me.StateID))
            _PermissionsList = Nothing
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Parse the Permission Keys used to persist the Permissions in the ViewState
        ''' </summary>
        ''' <param name="Settings">A string array of settings</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ParseKeys(ByVal Settings As String()) As WorkflowStatePermissionInfo
            Dim objWorkflowStatePermission As New WorkflowStatePermissionInfo

            'Call base class to load base properties
            MyBase.ParsePermissionKeys(objWorkflowStatePermission, Settings)

            If Settings(2) = "" Then
                objWorkflowStatePermission.WorkflowStatePermissionID = -1
            Else
                objWorkflowStatePermission.WorkflowStatePermissionID = Convert.ToInt32(Settings(2))
            End If
            objWorkflowStatePermission.StateID = StateID

            Return objWorkflowStatePermission
        End Function

#End Region

#Region "Protected Methods"

        Protected Overrides Sub AddPermission(ByVal permission As PermissionInfo, ByVal roleId As Integer, ByVal roleName As String, ByVal userId As Integer, ByVal displayName As String, ByVal allowAccess As Boolean)
            Dim objPermission As New WorkflowStatePermissionInfo(permission)
            objPermission.StateID = StateID
            objPermission.RoleID = roleId
            objPermission.RoleName = roleName
            objPermission.AllowAccess = allowAccess
            objPermission.UserID = userId
            objPermission.DisplayName = displayName
            _WorkflowStatePermissions.Add(objPermission, True)

            'Clear Permission List
            _PermissionsList = Nothing
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a Permission
        ''' </summary>
        ''' <param name="permissions">The permissions collection</param>
        ''' <param name="user">The user to add</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub AddPermission(ByVal permissions As ArrayList, ByVal user As UserInfo)
            'Search TabPermission Collection for the user 
            Dim isMatch As Boolean = False
            For Each objWorkflowStatePermission As WorkflowStatePermissionInfo In _WorkflowStatePermissions
                If objWorkflowStatePermission.UserID = user.UserID Then
                    isMatch = True
                    Exit For
                End If
            Next

            'user not found so add new
            If Not isMatch Then
                For Each objPermission As PermissionInfo In permissions
                    If objPermission.PermissionKey = "REVIEW" Then
                        AddPermission(objPermission, Integer.Parse(glbRoleNothing), Null.NullString, user.UserID, user.DisplayName, True)
                    End If
                Next
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Enabled status of the permission
        ''' </summary>
        ''' <param name="objPerm">The permission being loaded</param>
        ''' <param name="role">The role</param>
        ''' <param name="column">The column of the Grid</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetEnabled(ByVal objPerm As PermissionInfo, ByVal role As RoleInfo, ByVal column As Integer) As Boolean
            Return Not (role.RoleID = AdministratorRoleId)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Value of the permission
        ''' </summary>
        ''' <param name="objPerm">The permission being loaded</param>
        ''' <param name="role">The role</param>
        ''' <param name="column">The column of the Grid</param>
        ''' <returns>A Boolean (True or False)</returns>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetPermission(ByVal objPerm As PermissionInfo, ByVal role As RoleInfo, ByVal column As Integer, ByVal defaultState As String) As String
            Dim permission As String

            If role.RoleID = AdministratorRoleId Then
                permission = PermissionTypeGrant
            Else
                'Call base class method to handle standard permissions
                permission = MyBase.GetPermission(objPerm, role, column, defaultState)
            End If

            Return permission
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the permissions from the Database
        ''' </summary>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetPermissions() As ArrayList
            Return WorkflowStatePermissionController.GetPermissionsByStateID(StateID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Load the ViewState
        ''' </summary>
        ''' <param name="savedState">The saved state</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub LoadViewState(ByVal savedState As Object)

            If Not (savedState Is Nothing) Then
                ' Load State from the array of objects that was saved with SaveViewState.

                Dim myState As Object() = CType(savedState, Object())

                'Load Base Controls ViewStte
                If Not (myState(0) Is Nothing) Then
                    MyBase.LoadViewState(myState(0))
                End If

                'Load StateID
                If Not (myState(1) Is Nothing) Then
                    _StateID = CStr(myState(1))
                End If

                'Load WorkflowStatePermissions
                If Not (myState(2) Is Nothing) Then
                    _WorkflowStatePermissions = New WorkflowStatePermissionCollection()
                    Dim state As String = CStr(myState(2))
                    If state <> "" Then
                        'First Break the String into individual Keys
                        Dim permissionKeys As String() = Split(state, "##")
                        For Each key As String In permissionKeys
                            Dim Settings As String() = Split(key, "|")
                            _WorkflowStatePermissions.Add(ParseKeys(Settings))
                        Next
                    End If
                End If
            End If

        End Sub

        Protected Overrides Sub RemovePermission(ByVal permissionID As Integer, ByVal roleID As Integer, ByVal userID As Integer)
            _WorkflowStatePermissions.Remove(permissionID, roleID, userID)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Saves the ViewState
        ''' </summary>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function SaveViewState() As Object
            Dim allStates(2) As Object

            ' Save the Base Controls ViewState
            allStates(0) = MyBase.SaveViewState()

            'Save the State Id
            allStates(1) = StateID

            'Persist the TabPermisisons
            Dim sb As New StringBuilder
            If _WorkflowStatePermissions IsNot Nothing Then
                Dim addDelimiter As Boolean = False
                For Each objWorkflowStatePermission As WorkflowStatePermissionInfo In _WorkflowStatePermissions
                    If addDelimiter Then
                        sb.Append("##")
                    Else
                        addDelimiter = True
                    End If
                    sb.Append(BuildKey(objWorkflowStatePermission.AllowAccess, objWorkflowStatePermission.PermissionID, objWorkflowStatePermission.WorkflowStatePermissionID, objWorkflowStatePermission.RoleID, objWorkflowStatePermission.RoleName, objWorkflowStatePermission.UserID, objWorkflowStatePermission.DisplayName))
                Next
            End If
            allStates(2) = sb.ToString()

            Return allStates
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' returns whether or not the derived grid supports Deny permissions
        ''' </summary>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function SupportsDenyPermissions() As Boolean
            Return True
        End Function

#End Region

#Region "Public Methods"

        Public Sub ResetPermissions()
            GetWorkflowStatePermissions()
            _PermissionsList = Nothing
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Overrides the Base method to Generate the Data Grid
        ''' </summary>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub GenerateDataGrid()

        End Sub

#End Region

    End Class


End Namespace