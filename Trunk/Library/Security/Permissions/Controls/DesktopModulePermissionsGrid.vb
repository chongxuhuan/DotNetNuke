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
' TO THE WARRANTIES OF MERCHANDesktopModuleILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
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

    Public Class DesktopModulePermissionsGrid
        Inherits PermissionsGrid

#Region "Private Members"

        Private _PortalDesktopModuleID As Integer = -1
        Private _DesktopModulePermissions As DesktopModulePermissionCollection
        Private _PermissionsList As List(Of PermissionInfoBase)

#End Region

#Region "Protected Properties"

        Protected Overrides ReadOnly Property PermissionsList() As List(Of PermissionInfoBase)
            Get
                If _PermissionsList Is Nothing AndAlso _DesktopModulePermissions IsNot Nothing Then
                    _PermissionsList = _DesktopModulePermissions.ToList()
                End If
                Return _PermissionsList
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Permissions Collection
        ''' </summary>
        ''' <history>
        '''     [cnurse]    02/22/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Permissions() As Security.Permissions.DesktopModulePermissionCollection
            Get
                'First Update Permissions in case they have been changed
                UpdatePermissions()

                'Return the DesktopModulePermissions
                Return _DesktopModulePermissions
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the Id of the PortalDesktopModule
        ''' </summary>
        ''' <history>
        '''     [cnurse]    02/22/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PortalDesktopModuleID() As Integer
            Get
                Return _PortalDesktopModuleID
            End Get
            Set(ByVal Value As Integer)
                Dim oldValue As Integer = _PortalDesktopModuleID
                _PortalDesktopModuleID = Value
                If _DesktopModulePermissions Is Nothing OrElse oldValue <> Value Then
                    GetDesktopModulePermissions()
                End If
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the DesktopModulePermissions from the Data Store
        ''' </summary>
        ''' <history>
        '''     [cnurse]    02/22/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GetDesktopModulePermissions()
            _DesktopModulePermissions = New DesktopModulePermissionCollection(DesktopModulePermissionController.GetDesktopModulePermissions(PortalDesktopModuleID))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Parse the Permission Keys used to persist the Permissions in the ViewState
        ''' </summary>
        ''' <param name="Settings">A string array of settings</param>
        ''' <history>
        '''     [cnurse]    02/22/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ParseKeys(ByVal Settings As String()) As DesktopModulePermissionInfo
            Dim objDesktopModulePermission As New Security.Permissions.DesktopModulePermissionInfo

            'Call base class to load base properties
            MyBase.ParsePermissionKeys(objDesktopModulePermission, Settings)

            If Settings(2) = "" Then
                objDesktopModulePermission.DesktopModulePermissionID = -1
            Else
                objDesktopModulePermission.DesktopModulePermissionID = Convert.ToInt32(Settings(2))
            End If
            objDesktopModulePermission.PortalDesktopModuleID = PortalDesktopModuleID

            Return objDesktopModulePermission
        End Function

#End Region

#Region "Protected Methods"

        Protected Overrides Sub AddPermission(ByVal permission As PermissionInfo, ByVal roleId As Integer, ByVal roleName As String, ByVal userId As Integer, ByVal displayName As String, ByVal allowAccess As Boolean)
            Dim objPermission As New DesktopModulePermissionInfo(permission)
            objPermission.PortalDesktopModuleID = PortalDesktopModuleID
            objPermission.RoleID = roleId
            objPermission.RoleName = roleName
            objPermission.AllowAccess = allowAccess
            objPermission.UserID = userId
            objPermission.DisplayName = displayName
            _DesktopModulePermissions.Add(objPermission, True)

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
        '''     [cnurse]    02/22/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub AddPermission(ByVal permissions As ArrayList, ByVal user As UserInfo)
            'Search DesktopModulePermission Collection for the user 
            Dim isMatch As Boolean = False
            For Each objDesktopModulePermission As DesktopModulePermissionInfo In _DesktopModulePermissions
                If objDesktopModulePermission.UserID = user.UserID Then
                    isMatch = True
                    Exit For
                End If
            Next

            'user not found so add new
            If Not isMatch Then
                For Each objPermission As PermissionInfo In permissions
                    If objPermission.PermissionKey = "DEPLOY" Then
                        AddPermission(objPermission, Integer.Parse(glbRoleNothing), Null.NullString, user.UserID, user.DisplayName, True)
                    End If
                Next
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the permissions from the Database
        ''' </summary>
        ''' <history>
        '''     [cnurse]    02/22/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetPermissions() As ArrayList
            Return PermissionController.GetPermissionsByPortalDesktopModule()
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Load the ViewState
        ''' </summary>
        ''' <param name="savedState">The saved state</param>
        ''' <history>
        '''     [cnurse]    02/22/2008  Created
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

                'Load DesktopModuleId
                If Not (myState(1) Is Nothing) Then
                    PortalDesktopModuleID = CInt(myState(1))
                End If

                'Load DesktopModulePermissions
                If Not (myState(2) Is Nothing) Then
                    _DesktopModulePermissions = New DesktopModulePermissionCollection()
                    Dim state As String = CStr(myState(2))
                    If state <> "" Then
                        'First Break the String into individual Keys
                        Dim permissionKeys As String() = Split(state, "##")
                        For Each key As String In permissionKeys
                            Dim Settings As String() = Split(key, "|")
                            _DesktopModulePermissions.Add(ParseKeys(Settings))
                        Next
                    End If
                End If
            End If

        End Sub

        Protected Overrides Sub RemovePermission(ByVal permissionID As Integer, ByVal roleID As Integer, ByVal userID As Integer)
            _DesktopModulePermissions.Remove(permissionID, roleID, userID)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Saves the ViewState
        ''' </summary>
        ''' <history>
        '''     [cnurse]    02/22/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function SaveViewState() As Object
            Dim allStates(2) As Object

            ' Save the Base Controls ViewState
            allStates(0) = MyBase.SaveViewState()

            'Save the DesktopModule Id
            allStates(1) = PortalDesktopModuleID

            'Persist the DesktopModulePermisisons
            Dim sb As New StringBuilder
            If _DesktopModulePermissions IsNot Nothing Then
                Dim addDelimiter As Boolean = False
                For Each objDesktopModulePermission As DesktopModulePermissionInfo In _DesktopModulePermissions
                    If addDelimiter Then
                        sb.Append("##")
                    Else
                        addDelimiter = True
                    End If
                    sb.Append(BuildKey(objDesktopModulePermission.AllowAccess, objDesktopModulePermission.PermissionID, objDesktopModulePermission.DesktopModulePermissionID, objDesktopModulePermission.RoleID, objDesktopModulePermission.RoleName, objDesktopModulePermission.UserID, objDesktopModulePermission.DisplayName))
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

        Public Sub ResetPermissions()
            GetDesktopModulePermissions()
            _PermissionsList = Nothing
        End Sub

        Public Overrides Sub GenerateDataGrid()

        End Sub

#End Region


    End Class

End Namespace
