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
Imports System.Text
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Collections
Imports System.Data
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization
Imports System.Collections.Generic

Namespace DotNetNuke.Security.Permissions.Controls

    Public Class ModulePermissionsGrid
        Inherits PermissionsGrid

#Region "Private Members"

        Private _InheritViewPermissionsFromTab As Boolean = False
        Private _ModuleID As Integer = -1
        Private _ModulePermissions As ModulePermissionCollection
        Private _PermissionsList As List(Of PermissionInfoBase)
        Private _TabId As Integer = -1
        Private _ViewColumnIndex As Integer

#End Region

#Region "Protected Properties"

        Protected Overrides ReadOnly Property PermissionsList() As List(Of PermissionInfoBase)
            Get
                If _PermissionsList Is Nothing AndAlso _ModulePermissions IsNot Nothing Then
                    _PermissionsList = _ModulePermissions.ToList()
                End If
                Return _PermissionsList
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets whether the Module inherits the Page's(Tab's) permissions
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property InheritViewPermissionsFromTab() As Boolean
            Get
                Return _InheritViewPermissionsFromTab
            End Get
            Set(ByVal Value As Boolean)
                _InheritViewPermissionsFromTab = Value
                _PermissionsList = Nothing
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the Id of the Module
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ModuleID() As Integer
            Get
                Return _ModuleID
            End Get
            Set(ByVal Value As Integer)
                _ModuleID = Value
                If Not Page.IsPostBack Then
                    GetModulePermissions()
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the Id of the Tab associated with this module
        ''' </summary>
        ''' <history>
        '''     [cnurse]    24/11/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property TabId() As Integer
            Get
                Return _TabId
            End Get
            Set(ByVal Value As Integer)
                _TabId = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ModulePermission Collection
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Permissions() As Security.Permissions.ModulePermissionCollection
            Get
                'First Update Permissions in case they have been changed
                UpdatePermissions()

                'Return the ModulePermissions
                Return _ModulePermissions
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ModulePermissions from the Data Store
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GetModulePermissions()
            _ModulePermissions = New ModulePermissionCollection(ModulePermissionController.GetModulePermissions(Me.ModuleID, Me.TabId))
            _PermissionsList = Nothing
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Parse the Permission Keys used to persist the Permissions in the ViewState
        ''' </summary>
        ''' <param name="Settings">A string array of settings</param>
        ''' <history>
        '''     [cnurse]    01/09/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ParseKeys(ByVal Settings As String()) As ModulePermissionInfo
            Dim objModulePermission As New Security.Permissions.ModulePermissionInfo

            'Call base class to load base properties
            MyBase.ParsePermissionKeys(objModulePermission, Settings)

            If Settings(2) = "" Then
                objModulePermission.ModulePermissionID = -1
            Else
                objModulePermission.ModulePermissionID = Convert.ToInt32(Settings(2))
            End If

            objModulePermission.ModuleID = ModuleID

            Return objModulePermission
        End Function

#End Region

#Region "Protected Methods"

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
        Protected Overrides Sub AddPermission(ByVal permissions As ArrayList, ByVal user As UserInfo)
            'Search TabPermission Collection for the user 
            Dim isMatch As Boolean = False
            For Each objModulePermission As ModulePermissionInfo In _ModulePermissions
                If objModulePermission.UserID = user.UserID Then
                    isMatch = True
                    Exit For
                End If
            Next

            'user not found so add new
            If Not isMatch Then
                For Each objPermission As PermissionInfo In permissions
                    If objPermission.PermissionKey = "VIEW" Then
                        AddPermission(objPermission, Integer.Parse(glbRoleNothing), Null.NullString, user.UserID, user.DisplayName, True)
                    End If
                Next
            End If
        End Sub

        Protected Overrides Sub AddPermission(ByVal permission As PermissionInfo, ByVal roleId As Integer, ByVal roleName As String, ByVal userId As Integer, ByVal displayName As String, ByVal allowAccess As Boolean)
            Dim objPermission As New ModulePermissionInfo(permission)
            objPermission.ModuleID = ModuleID
            objPermission.RoleID = roleId
            objPermission.RoleName = roleName
            objPermission.AllowAccess = allowAccess
            objPermission.UserID = userId
            objPermission.DisplayName = displayName
            _ModulePermissions.Add(objPermission, True)

            'Clear Permission List
            _PermissionsList = Nothing
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
        Protected Overrides Function GetEnabled(ByVal objPerm As PermissionInfo, ByVal role As RoleInfo, ByVal column As Integer) As Boolean
            Dim enabled As Boolean

            If InheritViewPermissionsFromTab And column = _ViewColumnIndex Then
                enabled = False
            Else
                If role.RoleID = AdministratorRoleId Then
                    enabled = False
                Else
                    enabled = True
                End If
            End If

            Return enabled
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
        Protected Overrides Function GetEnabled(ByVal objPerm As PermissionInfo, ByVal user As UserInfo, ByVal column As Integer) As Boolean
            Return Not (InheritViewPermissionsFromTab And column = _ViewColumnIndex)
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
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetPermission(ByVal objPerm As PermissionInfo, ByVal role As RoleInfo, ByVal column As Integer, ByVal defaultState As String) As String
            Dim permission As String

            If InheritViewPermissionsFromTab And column = _ViewColumnIndex Then
                permission = PermissionTypeNull
            Else
                If role.RoleID = AdministratorRoleId Then
                    permission = PermissionTypeGrant
                Else
                    'Call base class method to handle standard permissions
                    permission = MyBase.GetPermission(objPerm, role, column, defaultState)
                End If
            End If

            Return permission
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Value of the permission
        ''' </summary>
        ''' <param name="objPerm">The permission being loaded</param>
        ''' <param name="user">The role</param>
        ''' <param name="column">The column of the Grid</param>
        ''' <returns>A Boolean (True or False)</returns>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetPermission(ByVal objPerm As PermissionInfo, ByVal user As UserInfo, ByVal column As Integer, ByVal defaultState As String) As String
            Dim permission As String

            If InheritViewPermissionsFromTab And column = _ViewColumnIndex Then
                permission = PermissionTypeNull
            Else
                'Call base class method to handle standard permissions
                permission = MyBase.GetPermission(objPerm, user, column, defaultState)
            End If

            Return permission
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Permissions from the Data Store
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetPermissions() As ArrayList

            Dim objPermissionController As New Security.Permissions.PermissionController
            Dim arrPermissions As ArrayList = objPermissionController.GetPermissionsByModuleID(Me.ModuleID)

            Dim i As Integer
            For i = 0 To arrPermissions.Count - 1
                Dim objPermission As Security.Permissions.PermissionInfo
                objPermission = CType(arrPermissions(i), Security.Permissions.PermissionInfo)
                If objPermission.PermissionKey = "VIEW" Then
                    _ViewColumnIndex = i + 1
                End If
            Next

            Return arrPermissions

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Load the ViewState
        ''' </summary>
        ''' <param name="savedState">The saved state</param>
        ''' <history>
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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
                    ModuleID = CInt(myState(1))
                End If

                'Load InheritViewPermissionsFromTab
                If Not (myState(2) Is Nothing) Then
                    InheritViewPermissionsFromTab = CBool(myState(2))
                End If

                'Load ModulePermissions
                If Not (myState(3) Is Nothing) Then
                    _ModulePermissions = New ModulePermissionCollection()
                    Dim state As String = CStr(myState(3))
                    If state <> "" Then
                        'First Break the String into individual Keys
                        Dim permissionKeys As String() = Split(state, "##")
                        For Each key As String In permissionKeys
                            Dim Settings As String() = Split(key, "|")
                            _ModulePermissions.Add(ParseKeys(Settings))
                        Next
                    End If
                End If
            End If

        End Sub

        Protected Overrides Sub RemovePermission(ByVal permissionID As Integer, ByVal roleID As Integer, ByVal userID As Integer)
            _ModulePermissions.Remove(permissionID, roleID, userID)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Saves the ViewState
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function SaveViewState() As Object
            Dim allStates(3) As Object

            ' Save the Base Controls ViewState
            allStates(0) = MyBase.SaveViewState()

            'Save the ModuleID
            allStates(1) = ModuleID

            'Save the InheritViewPermissionsFromTab
            allStates(2) = InheritViewPermissionsFromTab

            'Persist the ModulePermissions
            Dim sb As New StringBuilder
            If _ModulePermissions IsNot Nothing Then
                Dim addDelimiter As Boolean = False
                For Each objModulePermission As ModulePermissionInfo In _ModulePermissions
                    If addDelimiter Then
                        sb.Append("##")
                    Else
                        addDelimiter = True
                    End If
                    sb.Append(BuildKey(objModulePermission.AllowAccess, objModulePermission.PermissionID, objModulePermission.ModulePermissionID, objModulePermission.RoleID, objModulePermission.RoleName, objModulePermission.UserID, objModulePermission.DisplayName))
                Next
            End If
            allStates(3) = sb.ToString()

            Return allStates
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' returns whether or not the derived grid supports Deny permissions
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function SupportsDenyPermissions() As Boolean
            Return True
        End Function

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Overrides the Base method to Generate the Data Grid
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub GenerateDataGrid()

        End Sub

#End Region

    End Class

End Namespace