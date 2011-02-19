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

Namespace DotNetNuke.Security.Membership.Data

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Security.Membership
    ''' Class:      DataProvider
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DataProvider provides the abstract Data Access Layer for the project
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	03/28/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class DataProvider

#Region "Shared/Static Methods"

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of DataProvider)()
        End Function

#End Region

#Region "Abstract Methods"

        'Login/Security
        Public MustOverride Function UserLogin(ByVal Username As String, ByVal Password As String) As IDataReader
        Public MustOverride Function GetAuthRoles(ByVal PortalId As Integer, ByVal ModuleId As Integer) As IDataReader

        'Users
        Public MustOverride Function AddUser(ByVal PortalID As Integer, ByVal Username As String, ByVal FirstName As String, ByVal LastName As String, ByVal AffiliateId As Integer, ByVal IsSuperUser As Boolean, ByVal Email As String, ByVal DisplayName As String, ByVal UpdatePassword As Boolean, ByVal IsApproved As Boolean, ByVal createdByUserID As Integer) As Integer
        Public MustOverride Sub DeleteUserPortal(ByVal UserId As Integer, ByVal PortalId As Integer)
        Public MustOverride Sub RestoreUser(ByVal UserId As Integer, ByVal PortalId As Integer)
        Public MustOverride Sub RemoveUser(ByVal UserId As Integer, ByVal PortalId As Integer)
        Public MustOverride Function GetAllUsers(ByVal PortalID As Integer, ByVal pageIndex As Integer, ByVal pageSize As Integer) As IDataReader
        Public MustOverride Function GetUnAuthorizedUsers(ByVal portalId As Integer) As IDataReader
        Public MustOverride Function GetDeletedUsers(ByVal portalId As Integer) As IDataReader
        Public MustOverride Function GetUser(ByVal PortalId As Integer, ByVal UserId As Integer) As IDataReader
        Public MustOverride Function GetUserByAuthToken(ByVal PortalID As Integer, ByVal UserToken As String, ByVal AuthType As String) As IDataReader
        Public MustOverride Function GetUserByUsername(ByVal PortalID As Integer, ByVal Username As String) As IDataReader
        Public MustOverride Function GetUserCountByPortal(ByVal portalId As Integer) As Integer
        Public MustOverride Function GetUsersByEmail(ByVal PortalID As Integer, ByVal Email As String, ByVal pageIndex As Integer, ByVal pageSize As Integer) As IDataReader        
        Public MustOverride Function GetUsersByProfileProperty(ByVal PortalID As Integer, ByVal propertyName As String, ByVal propertyValue As String, ByVal pageIndex As Integer, ByVal pageSize As Integer) As IDataReader        
        Public MustOverride Function GetUsersByRolename(ByVal PortalID As Integer, ByVal Rolename As String) As IDataReader
        Public MustOverride Function GetUsersByUsername(ByVal PortalID As Integer, ByVal Username As String, ByVal pageIndex As Integer, ByVal pageSize As Integer) As IDataReader        
        Public MustOverride Function GetSuperUsers() As IDataReader
        Public MustOverride Sub UpdateUser(ByVal UserId As Integer, ByVal PortalID As Integer, ByVal FirstName As String, ByVal LastName As String, ByVal Email As String, ByVal DisplayName As String, ByVal UpdatePassword As Boolean, ByVal IsApproved As Boolean, ByVal RefreshRoles As Boolean, ByVal LastIPAddress As String, ByVal IsDeleted As Boolean, ByVal lastModifiedByUserID As Integer)

        'Roles
        Public MustOverride Function GetPortalRoles(ByVal PortalId As Integer) As IDataReader
        Public MustOverride Function GetRoles() As IDataReader
        Public MustOverride Function GetRole(ByVal RoleID As Integer, ByVal PortalID As Integer) As IDataReader
        Public MustOverride Function GetRoleByName(ByVal PortalId As Integer, ByVal RoleName As String) As IDataReader
        Public MustOverride Function AddRole(ByVal PortalId As Integer, ByVal RoleGroupId As Integer, ByVal RoleName As String, ByVal Description As String, ByVal ServiceFee As Single, ByVal BillingPeriod As String, ByVal BillingFrequency As String, ByVal TrialFee As Single, ByVal TrialPeriod As Integer, ByVal TrialFrequency As String, ByVal IsPublic As Boolean, ByVal AutoAssignment As Boolean, ByVal RSVPCode As String, ByVal IconFile As String, ByVal CreatedByUserID As Integer) As Integer
        Public MustOverride Sub DeleteRole(ByVal RoleId As Integer)
        Public MustOverride Sub UpdateRole(ByVal RoleId As Integer, ByVal RoleGroupId As Integer, ByVal Description As String, ByVal ServiceFee As Single, ByVal BillingPeriod As String, ByVal BillingFrequency As String, ByVal TrialFee As Single, ByVal TrialPeriod As Integer, ByVal TrialFrequency As String, ByVal IsPublic As Boolean, ByVal AutoAssignment As Boolean, ByVal RSVPCode As String, ByVal IconFile As String, ByVal LastModifiedByUserID As Integer)
        Public MustOverride Function GetRolesByUser(ByVal UserId As Integer, ByVal PortalId As Integer) As IDataReader

        'RoleGroups
        Public MustOverride Function AddRoleGroup(ByVal PortalId As Integer, ByVal GroupName As String, ByVal Description As String, ByVal CreatedByUserID As Integer) As Integer
        Public MustOverride Sub DeleteRoleGroup(ByVal RoleGroupId As Integer)
        Public MustOverride Function GetRoleGroup(ByVal portalId As Integer, ByVal roleGroupId As Integer) As IDataReader
        Public MustOverride Function GetRoleGroupByName(ByVal PortalID As Integer, ByVal RoleGroupName As String) As IDataReader
        Public MustOverride Function GetRoleGroups(ByVal portalId As Integer) As IDataReader
        Public MustOverride Function GetRolesByGroup(ByVal RoleGroupId As Integer, ByVal PortalId As Integer) As IDataReader
        Public MustOverride Sub UpdateRoleGroup(ByVal RoleGroupId As Integer, ByVal GroupName As String, ByVal Description As String, ByVal LastModifiedByUserID As Integer)

        'User Roles
        Public MustOverride Function GetUserRole(ByVal PortalID As Integer, ByVal UserId As Integer, ByVal RoleId As Integer) As IDataReader
        Public MustOverride Function GetUserRoles(ByVal PortalID As Integer, ByVal UserId As Integer) As IDataReader
        Public MustOverride Function GetUserRolesByUsername(ByVal PortalID As Integer, ByVal Username As String, ByVal Rolename As String) As IDataReader
        Public MustOverride Function AddUserRole(ByVal PortalID As Integer, ByVal UserId As Integer, ByVal RoleId As Integer, ByVal EffectiveDate As Date, ByVal ExpiryDate As Date, ByVal createdByUserID As Integer) As Integer
        Public MustOverride Sub UpdateUserRole(ByVal UserRoleId As Integer, ByVal EffectiveDate As Date, ByVal ExpiryDate As Date, ByVal lastModifiedByUserID As Integer)
        Public MustOverride Sub DeleteUserRole(ByVal UserId As Integer, ByVal RoleId As Integer)
        Public MustOverride Function GetServices(ByVal PortalId As Integer, ByVal UserId As Integer) As IDataReader

        'Profile
        Public MustOverride Function GetUserProfile(ByVal UserId As Integer) As IDataReader
        Public MustOverride Sub UpdateProfileProperty(ByVal ProfileId As Integer, ByVal UserId As Integer, ByVal PropertyDefinitionID As Integer, ByVal PropertyValue As String, ByVal Visibility As Integer, ByVal LastUpdatedDate As DateTime)

        ' users online
        Public MustOverride Sub UpdateUsersOnline(ByVal UserList As Hashtable)
        Public MustOverride Sub DeleteUsersOnline(ByVal TimeWindow As Integer)
        Public MustOverride Function GetOnlineUser(ByVal UserId As Integer) As IDataReader
        Public MustOverride Function GetOnlineUsers(ByVal PortalId As Integer) As IDataReader

        ' legacy
        Public MustOverride Function GetUsers(ByVal PortalId As Integer) As IDataReader

#End Region

#Region "Virtual Methods"
        Public Overridable Function GetAllUsers(ByVal PortalID As Integer, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByVal includeDeleted As Boolean, ByVal superUsersOnly As Boolean) As IDataReader
            Throw New NotImplementedException()
        End Function

        Public Overridable Function GetUsersByEmail(ByVal PortalID As Integer, ByVal Email As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByVal includeDeleted As Boolean, ByVal superUsersOnly As Boolean) As IDataReader
            Throw New NotImplementedException()
        End Function

        Public Overridable Function GetUsersByProfileProperty(ByVal PortalID As Integer, ByVal propertyName As String, ByVal propertyValue As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByVal includeDeleted As Boolean, ByVal superUsersOnly As Boolean) As IDataReader
            Throw New NotImplementedException()
        End Function

        Public Overridable Function GetUsersByUsername(ByVal PortalID As Integer, ByVal Username As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByVal includeDeleted As Boolean, ByVal superUsersOnly As Boolean) As IDataReader
            Throw New NotImplementedException()
        End Function

        Public Overridable Function GetUnAuthorizedUsers(ByVal portalId As Integer, ByVal includeDeleted As Boolean, ByVal superUsersOnly As Boolean) As IDataReader
            Throw New NotImplementedException()
        End Function

#End Region

    End Class

End Namespace
