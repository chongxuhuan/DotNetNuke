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

    Public Class FolderPermissionsGrid
        Inherits PermissionsGrid

#Region "Private Members"

        Private _FolderPath As String = ""
        Private _FolderPermissions As FolderPermissionCollection
        Private _PermissionsList As List(Of PermissionInfoBase)
        Private _RefreshGrid As Boolean = Null.NullBoolean

#End Region

#Region "Protected Properties"

        Protected Overrides ReadOnly Property PermissionsList() As List(Of PermissionInfoBase)
            Get
                If _PermissionsList Is Nothing AndAlso _FolderPermissions IsNot Nothing Then
                    _PermissionsList = _FolderPermissions.ToList()
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
        ''' Gets and Sets the path of the Folder
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FolderPath() As String
            Get
                Return _FolderPath
            End Get
            Set(ByVal Value As String)
                _FolderPath = Value
                _RefreshGrid = True
                GetFolderPermissions()
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Permission Collection
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/09/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Permissions() As Security.Permissions.FolderPermissionCollection
            Get
                'First Update Permissions in case they have been changed
                UpdatePermissions()

                'Return the FolderPermissions
                Return _FolderPermissions
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the TabPermissions from the Data Store
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GetFolderPermissions()
            _FolderPermissions = New FolderPermissionCollection(FolderPermissionController.GetFolderPermissionsCollectionByFolder(PortalId, Me.FolderPath))
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
        Private Function ParseKeys(ByVal Settings As String()) As FolderPermissionInfo
            Dim objFolderPermission As New FolderPermissionInfo

            'Call base class to load base properties
            MyBase.ParsePermissionKeys(objFolderPermission, Settings)

            If Settings(2) = "" Then
                objFolderPermission.FolderPermissionID = -1
            Else
                objFolderPermission.FolderPermissionID = Convert.ToInt32(Settings(2))
            End If
            objFolderPermission.FolderPath = FolderPath

            Return objFolderPermission
        End Function

#End Region

#Region "Protected Methods"

        Protected Overrides Sub AddPermission(ByVal permission As PermissionInfo, ByVal roleId As Integer, ByVal roleName As String, ByVal userId As Integer, ByVal displayName As String, ByVal allowAccess As Boolean)
            Dim objPermission As New FolderPermissionInfo(permission)
            objPermission.FolderPath = FolderPath
            objPermission.RoleID = roleId
            objPermission.RoleName = roleName
            objPermission.AllowAccess = allowAccess
            objPermission.UserID = userId
            objPermission.DisplayName = displayName
            _FolderPermissions.Add(objPermission, True)

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
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub AddPermission(ByVal permissions As ArrayList, ByVal user As UserInfo)
            'Search TabPermission Collection for the user 
            Dim isMatch As Boolean = False
            For Each objFolderPermission As FolderPermissionInfo In _FolderPermissions
                If objFolderPermission.UserID = user.UserID Then
                    isMatch = True
                    Exit For
                End If
            Next

            'user not found so add new
            If Not isMatch Then
                For Each objPermission As PermissionInfo In permissions
                    If objPermission.PermissionKey = "READ" Then
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
        '''     [cnurse]    01/13/2006  Created
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
        '''     [cnurse]    01/09/2006  Created
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
        '''     [cnurse]    01/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetPermissions() As ArrayList
            Return PermissionController.GetPermissionsByFolder()
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

                'Load Base Controls ViewStte
                If Not (myState(0) Is Nothing) Then
                    MyBase.LoadViewState(myState(0))
                End If

                'Load FolderPath
                If Not (myState(1) Is Nothing) Then
                    _FolderPath = CStr(myState(1))
                End If

                'Load FolderPermissions
                If Not (myState(2) Is Nothing) Then
                    _FolderPermissions = New FolderPermissionCollection()
                    Dim state As String = CStr(myState(2))
                    If state <> "" Then
                        'First Break the String into individual Keys
                        Dim permissionKeys As String() = Split(state, "##")
                        For Each key As String In permissionKeys
                            Dim Settings As String() = Split(key, "|")
                            _FolderPermissions.Add(ParseKeys(Settings))
                        Next
                    End If
                End If
            End If

        End Sub

        Protected Overrides Sub RemovePermission(ByVal permissionID As Integer, ByVal roleID As Integer, ByVal userID As Integer)
            _FolderPermissions.Remove(permissionID, roleID, userID)
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
            Dim allStates(2) As Object

            ' Save the Base Controls ViewState
            allStates(0) = MyBase.SaveViewState()

            'Save the Tab Id
            allStates(1) = FolderPath

            'Persist the TabPermisisons
            Dim sb As New StringBuilder
            If _FolderPermissions IsNot Nothing Then
                Dim addDelimiter As Boolean = False
                For Each objFolderPermission As FolderPermissionInfo In _FolderPermissions
                    If addDelimiter Then
                        sb.Append("##")
                    Else
                        addDelimiter = True
                    End If
                    sb.Append(BuildKey(objFolderPermission.AllowAccess, objFolderPermission.PermissionID, objFolderPermission.FolderPermissionID, objFolderPermission.RoleID, objFolderPermission.RoleName, objFolderPermission.UserID, objFolderPermission.DisplayName))
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