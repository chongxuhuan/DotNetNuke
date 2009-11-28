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
Imports DotNetNuke
Imports DotNetNuke.Security.Roles

Namespace DotNetNuke.Security.Roles

    Public MustInherit Class RoleProvider

#Region "Shared/Static Methods"

        ' return the provider
        Public Shared Shadows Function Instance() As RoleProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of RoleProvider)()
        End Function

#End Region

#Region "Abstract Methods"

        'Roles
        Public MustOverride Function CreateRole(ByVal portalId As Integer, ByRef role As RoleInfo) As Boolean
        Public MustOverride Sub DeleteRole(ByVal portalId As Integer, ByRef role As RoleInfo)
        Public MustOverride Function GetRole(ByVal portalId As Integer, ByVal roleId As Integer) As RoleInfo
        Public MustOverride Function GetRole(ByVal portalId As Integer, ByVal roleName As String) As RoleInfo
        Public MustOverride Function GetRoleNames(ByVal portalId As Integer) As String()
        Public MustOverride Function GetRoleNames(ByVal portalId As Integer, ByVal userId As Integer) As String()
        Public MustOverride Function GetRoles(ByVal portalId As Integer) As ArrayList
        Public MustOverride Function GetRolesByGroup(ByVal portalId As Integer, ByVal roleGroupId As Integer) As ArrayList
        Public MustOverride Sub UpdateRole(ByVal role As RoleInfo)

        'Role Groups
        Public MustOverride Function CreateRoleGroup(ByVal roleGroup As RoleGroupInfo) As Integer
        Public MustOverride Sub DeleteRoleGroup(ByVal roleGroup As RoleGroupInfo)
        Public MustOverride Function GetRoleGroup(ByVal portalId As Integer, ByVal roleGroupId As Integer) As RoleGroupInfo
        Public MustOverride Function GetRoleGroups(ByVal portalId As Integer) As ArrayList
        Public MustOverride Sub UpdateRoleGroup(ByVal roleGroup As RoleGroupInfo)

        'User Roles
        Public MustOverride Function AddUserToRole(ByVal portalId As Integer, ByVal user As UserInfo, ByVal userRole As UserRoleInfo) As Boolean
        Public MustOverride Function GetUserRole(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal RoleId As Integer) As UserRoleInfo
        Public MustOverride Function GetUserRoles(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal includePrivate As Boolean) As ArrayList
        Public MustOverride Function GetUserRoles(ByVal PortalId As Integer, ByVal Username As String, ByVal Rolename As String) As ArrayList
        Public MustOverride Function GetUsersByRoleName(ByVal portalId As Integer, ByVal roleName As String) As ArrayList
        Public MustOverride Function GetUserRolesByRoleName(ByVal portalId As Integer, ByVal roleName As String) As ArrayList
        Public MustOverride Sub RemoveUserFromRole(ByVal portalId As Integer, ByVal user As UserInfo, ByVal userRole As UserRoleInfo)
        Public MustOverride Sub UpdateUserRole(ByVal userRole As UserRoleInfo)

        Public Overridable Function GetRoleGroupByName(ByVal PortalID As Integer, ByVal RoleGroupName As String) As RoleGroupInfo
            Return Nothing
        End Function


#End Region

    End Class

End Namespace

