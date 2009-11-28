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
Imports System.Collections
Imports Microsoft.VisualBasic

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Mail
Imports System.Xml


Namespace DotNetNuke.Security.Roles

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Security.Roles
    ''' Class:      RoleController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The RoleController class provides Business Layer methods for Roles
    ''' </summary>
    ''' <history>
    '''     [cnurse]    05/23/2005  made compatible with .NET 2.0
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class RoleController

#Region "Private Shared Members"

        Private Enum UserRoleActions
            add = 0
            update = 1
            delete = 2
        End Enum

        Private Shared UserRoleActionsCaption() As String = {"ASSIGNMENT", "UPDATE", "UNASSIGNMENT"}
        Private Shared provider As DotNetNuke.Security.Roles.RoleProvider = DotNetNuke.Security.Roles.RoleProvider.Instance()

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Auto Assign existing users to a role
        ''' </summary>
        ''' <param name="objRoleInfo">The Role to Auto assign</param>
        ''' <history>
        ''' 	[cnurse]	05/23/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AutoAssignUsers(ByVal objRoleInfo As RoleInfo)

            If objRoleInfo.AutoAssignment Then

                ' loop through users for portal and add to role
                Dim arrUsers As ArrayList = UserController.GetUsers(objRoleInfo.PortalID)
                For Each objUser As UserInfo In arrUsers
                    Try
                        AddUserRole(objRoleInfo.PortalID, objUser.UserID, objRoleInfo.RoleID, Null.NullDate, Null.NullDate)
                    Catch ex As Exception
                        ' user already belongs to role
                    End Try
                Next
            End If

        End Sub

#End Region

#Region "Role Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This overload adds a role and optionally adds the info to the AspNet Roles
        ''' </summary>
        ''' <param name="objRoleInfo">The Role to Add</param>
        ''' <returns>The Id of the new role</returns>
        ''' <history>
        ''' 	[cnurse]	05/23/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function AddRole(ByVal objRoleInfo As RoleInfo) As Integer
            Dim roleId As Integer = -1
            Dim success As Boolean = provider.CreateRole(objRoleInfo.PortalID, objRoleInfo)

            If success Then
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                objEventLog.AddLog(objRoleInfo, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.ROLE_CREATED)
                AutoAssignUsers(objRoleInfo)
                roleId = objRoleInfo.RoleID
            End If



            Return roleId
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Delete a Role
        ''' </summary>
        ''' <param name="RoleId">The Id of the Role to delete</param>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <history>
        ''' 	[cnurse]	05/24/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteRole(ByVal RoleId As Integer, ByVal PortalId As Integer)

            Dim objRole As RoleInfo = GetRole(RoleId, PortalId)

            If Not objRole Is Nothing Then
                provider.DeleteRole(PortalId, objRole)
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                objEventLog.AddLog("RoleID", RoleId.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Services.Log.EventLog.EventLogController.EventLogType.ROLE_DELETED)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get a list of roles for the Portal
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <returns>An ArrayList of RoleInfo objects</returns>
        ''' <history>
        ''' 	[cnurse]	05/24/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetPortalRoles(ByVal PortalId As Integer) As ArrayList
            Return provider.GetRoles(PortalId)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fetch a single Role
        ''' </summary>
        ''' <param name="RoleID">The Id of the Role</param>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[cnurse]	05/24/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetRole(ByVal RoleID As Integer, ByVal PortalID As Integer) As RoleInfo
            Return provider.GetRole(PortalID, RoleID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Obtains a role given the role name
        ''' </summary>
        ''' <param name="PortalId">Portal indentifier</param>
        ''' <param name="RoleName">Name of the role to be found</param>
        ''' <returns>A RoleInfo object is the role is found</returns>
        ''' <history>
        ''' 	[VMasanas]	27/08/2004	Created
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetRoleByName(ByVal PortalId As Integer, ByVal RoleName As String) As RoleInfo
            Return provider.GetRole(PortalId, RoleName)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns an array of rolenames for a Portal
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <returns>A String Array of Role Names</returns>
        ''' <history>
        ''' 	[cnurse]	05/24/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetRoleNames(ByVal PortalID As Integer) As String()
            Return provider.GetRoleNames(PortalID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an ArrayList of Roles
        ''' </summary>
        ''' <returns>An ArrayList of Roles</returns>
        ''' <history>
        ''' 	[cnurse]	05/24/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetRoles() As ArrayList
            Return provider.GetRoles(Null.NullInteger)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the roles for a Role Group
        ''' </summary>
        ''' <param name="portalId">Id of the portal</param>
        ''' <param name="roleGroupId">Id of the Role Group (If -1 all roles for the portal are
        ''' retrieved).</param>
        ''' <returns>An ArrayList of RoleInfo objects</returns>
        ''' <history>
        '''     [cnurse]	01/03/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetRolesByGroup(ByVal portalId As Integer, ByVal roleGroupId As Integer) As ArrayList
            Return provider.GetRolesByGroup(portalId, roleGroupId)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a List of Roles for a given User
        ''' </summary>
        ''' <param name="UserId">The Id of the User</param>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <returns>A String Array of Role Names</returns>
        ''' <history>
        ''' 	[cnurse]	05/24/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetRolesByUser(ByVal UserId As Integer, ByVal PortalId As Integer) As String()
            Return provider.GetRoleNames(PortalId, UserId)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Persists a role to the Data Store
        ''' </summary>
        ''' <param name="objRoleInfo">The role to persist</param>
        ''' <history>
        ''' 	[cnurse]	05/24/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateRole(ByVal objRoleInfo As RoleInfo)
            provider.UpdateRole(objRoleInfo)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objRoleInfo, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.ROLE_UPDATED)
            AutoAssignUsers(objRoleInfo)
        End Sub

#End Region

#Region "User Role Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a User to a Role
        ''' </summary>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <param name="UserId">The Id of the User</param>
        ''' <param name="RoleId">The Id of the Role</param>
        ''' <param name="ExpiryDate">The expiry Date of the Role membership</param>
        ''' <history>
        ''' 	[cnurse]	05/24/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        '''     [cnurse]    05/12/2006  Now calls new overload with EffectiveDate = Now()
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddUserRole(ByVal PortalID As Integer, ByVal UserId As Integer, ByVal RoleId As Integer, ByVal ExpiryDate As Date)
            AddUserRole(PortalID, UserId, RoleId, Null.NullDate, ExpiryDate)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a User to a Role
        ''' </summary>
        ''' <remarks>Overload adds Effective Date</remarks>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <param name="UserId">The Id of the User</param>
        ''' <param name="RoleId">The Id of the Role</param>
        ''' <param name="EffectiveDate">The expiry Date of the Role membership</param>
        ''' <param name="ExpiryDate">The expiry Date of the Role membership</param>
        ''' <history>
        '''     [cnurse]    05/12/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddUserRole(ByVal PortalID As Integer, ByVal UserId As Integer, ByVal RoleId As Integer, ByVal EffectiveDate As Date, ByVal ExpiryDate As Date)

            Dim objUser As UserInfo = UserController.GetUserById(PortalID, UserId)
            Dim objUserRole As UserRoleInfo = GetUserRole(PortalID, UserId, RoleId)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            If objUserRole Is Nothing Then
                'Create new UserRole
                objUserRole = New UserRoleInfo
                objUserRole.UserID = UserId
                objUserRole.RoleID = RoleId
                objUserRole.PortalID = PortalID
                objUserRole.EffectiveDate = EffectiveDate
                objUserRole.ExpiryDate = ExpiryDate
                provider.AddUserToRole(PortalID, objUser, objUserRole)
                objEventLog.AddLog(objUserRole, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_CREATED)
            Else
                objUserRole.EffectiveDate = EffectiveDate
                objUserRole.ExpiryDate = ExpiryDate
                provider.UpdateUserRole(objUserRole)
                objEventLog.AddLog(objUserRole, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_UPDATED)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Delete/Remove a User from a Role
        ''' </summary>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <param name="UserId">The Id of the User</param>
        ''' <param name="RoleId">The Id of the Role</param>
        ''' <returns></returns>
        ''' <history>
        ''' 	[cnurse]	05/24/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function DeleteUserRole(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal RoleId As Integer) As Boolean
            Dim objUser As UserInfo = UserController.GetUserById(PortalId, UserId)
            Dim objUserRole As UserRoleInfo = GetUserRole(PortalId, UserId, RoleId)

            Dim objPortals As New PortalController
            Dim blnDelete As Boolean = True

            Dim objPortal As PortalInfo = objPortals.GetPortal(PortalId)
            If Not (objPortal Is Nothing OrElse objUserRole Is Nothing) Then
                If CanRemoveUserFromRole(objPortal, UserId, RoleId) Then
                    provider.RemoveUserFromRole(PortalId, objUser, objUserRole)
                    Dim objEventLog As New Services.Log.EventLog.EventLogController
                    objEventLog.AddLog(objUserRole, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.ROLE_UPDATED)

                Else
                    blnDelete = False
                End If
            End If

            Return blnDelete
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a User/Role
        ''' </summary>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <param name="UserId">The Id of the user</param>
        ''' <param name="RoleId">The Id of the Role</param>
        ''' <returns>A UserRoleInfo object</returns>
        ''' <history>
        ''' 	[cnurse]	05/24/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetUserRole(ByVal PortalID As Integer, ByVal UserId As Integer, ByVal RoleId As Integer) As UserRoleInfo
            Return provider.GetUserRole(PortalID, UserId, RoleId)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a list of UserRoles for a Portal
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <returns>An ArrayList of UserRoleInfo objects</returns>
        ''' <history>
        ''' 	[Nik Kalyani]	10/15/2004	Created multiple signatures to eliminate Optional parameters
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetUserRoles(ByVal PortalId As Integer) As ArrayList
            Return GetUserRoles(PortalId, -1)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a list of UserRoles for a User
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <param name="UserId">The Id of the User</param>
        ''' <returns>An ArrayList of UserRoleInfo objects</returns>
        ''' <history>
        ''' 	[Nik Kalyani]	10/15/2004	Created multiple signatures to eliminate Optional parameters
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetUserRoles(ByVal PortalId As Integer, ByVal UserId As Integer) As ArrayList
            Return provider.GetUserRoles(PortalId, UserId, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a list of UserRoles for a User
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <param name="UserId">The Id of the User</param>
        ''' <returns>An ArrayList of UserRoleInfo objects</returns>
        ''' <history>
        ''' 	[Nik Kalyani]	10/15/2004	Created multiple signatures to eliminate Optional parameters
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetUserRoles(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal includePrivate As Boolean) As ArrayList
            Return provider.GetUserRoles(PortalId, UserId, includePrivate)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a List of UserRoles by UserName and RoleName
        ''' </summary>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <param name="Username">The username of the user</param>
        ''' <param name="Rolename">The role name</param>
        ''' <returns>An ArrayList of UserRoleInfo objects</returns>
        ''' <history>
        ''' 	[cnurse]	05/24/2005	Documented
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetUserRolesByUsername(ByVal PortalID As Integer, ByVal Username As String, ByVal Rolename As String) As ArrayList
            Return provider.GetUserRoles(PortalID, Username, Rolename)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the users in a role (as UserRole objects)
        ''' </summary>
        ''' <param name="portalId">Id of the portal (If -1 all roles for all portals are 
        ''' retrieved.</param>
        ''' <param name="roleName">The role to fetch users for</param>
        ''' <returns>An ArrayList of UserRoleInfo objects</returns>
        ''' <history>
        '''     [cnurse]	01/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetUserRolesByRoleName(ByVal portalId As Integer, ByVal roleName As String) As ArrayList
            Return provider.GetUserRolesByRoleName(portalId, roleName)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the users in a role (as User objects)
        ''' </summary>
        ''' <param name="portalId">Id of the portal (If -1 all roles for all portals are 
        ''' retrieved.</param>
        ''' <param name="roleName">The role to fetch users for</param>
        ''' <returns>An ArrayList of UserInfo objects</returns>
        ''' <history>
        '''     [cnurse]	01/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetUsersByRoleName(ByVal PortalID As Integer, ByVal RoleName As String) As ArrayList
            Return provider.GetUsersByRoleName(PortalID, RoleName)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a Service (UserRole)
        ''' </summary>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <param name="UserId">The Id of the User</param>
        ''' <param name="RoleId">The Id of the Role</param>
        ''' <history>
        ''' 	[Nik Kalyani]	10/15/2004	Created multiple signatures to eliminate Optional parameters
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateUserRole(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal RoleId As Integer)
            UpdateUserRole(PortalId, UserId, RoleId, False)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a Service (UserRole)
        ''' </summary>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <param name="UserId">The Id of the User</param>
        ''' <param name="RoleId">The Id of the Role</param>
        ''' <param name="Cancel">A flag that indicates whether to cancel (delete) the userrole</param>
        ''' <history>
        ''' 	[Nik Kalyani]	10/15/2004	Created multiple signatures to eliminate Optional parameters
        '''     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateUserRole(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal RoleId As Integer, ByVal Cancel As Boolean)
            Dim userRole As UserRoleInfo
            userRole = GetUserRole(PortalId, UserId, RoleId)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            If Cancel Then
                If userRole IsNot Nothing AndAlso userRole.ServiceFee > 0.0 AndAlso userRole.IsTrialUsed Then
                    'Expire Role so we retain trial used data
                    userRole.ExpiryDate = DateAdd(DateInterval.Day, -1, Date.Today())
                    provider.UpdateUserRole(userRole)

                    objEventLog.AddLog(userRole, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_UPDATED)
                Else
                    'Delete Role
                    DeleteUserRole(PortalId, UserId, RoleId)
                    objEventLog.AddLog("UserId", UserId.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_DELETED)
                End If
            Else
                Dim UserRoleId As Integer = -1
                Dim role As RoleInfo
                Dim ExpiryDate As Date = Now
                Dim EffectiveDate As Date = Null.NullDate
                Dim IsTrialUsed As Boolean = False
                Dim Period As Integer
                Dim Frequency As String = ""

                If Not userRole Is Nothing Then
                    UserRoleId = userRole.UserRoleID
                    EffectiveDate = userRole.EffectiveDate
                    ExpiryDate = userRole.ExpiryDate
                    IsTrialUsed = userRole.IsTrialUsed
                End If

                role = GetRole(RoleId, PortalId)

                If Not role Is Nothing Then
                    If IsTrialUsed = False And role.TrialFrequency.ToString <> "N" Then
                        Period = role.TrialPeriod
                        Frequency = role.TrialFrequency
                    Else
                        Period = role.BillingPeriod
                        Frequency = role.BillingFrequency
                    End If
                End If

                If EffectiveDate < Now Then
                    EffectiveDate = Null.NullDate
                End If
                If ExpiryDate < Now Then
                    ExpiryDate = Now
                End If

                If Period = Null.NullInteger Then
                    ExpiryDate = Null.NullDate
                Else
                    Select Case Frequency
                        Case "N" : ExpiryDate = Null.NullDate
                        Case "O" : ExpiryDate = New System.DateTime(9999, 12, 31)
                        Case "D" : ExpiryDate = DateAdd(DateInterval.Day, Period, Convert.ToDateTime(ExpiryDate))
                        Case "W" : ExpiryDate = DateAdd(DateInterval.Day, (Period * 7), Convert.ToDateTime(ExpiryDate))
                        Case "M" : ExpiryDate = DateAdd(DateInterval.Month, Period, Convert.ToDateTime(ExpiryDate))
                        Case "Y" : ExpiryDate = DateAdd(DateInterval.Year, Period, Convert.ToDateTime(ExpiryDate))
                    End Select

                End If
                If UserRoleId <> -1 Then
                    userRole.ExpiryDate = ExpiryDate
                    provider.UpdateUserRole(userRole)
                    objEventLog.AddLog(userRole, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_UPDATED)
                Else
                    AddUserRole(PortalId, UserId, RoleId, EffectiveDate, ExpiryDate)
                End If
            End If
        End Sub

#End Region

#Region "Private SHared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SendNotification sends an email notification to the user of the change in his/her role
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="objUser">The User</param>
        ''' <param name="objRole">The Role</param>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        '''     [cnurse]    10/17/2007  Moved to RoleController
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub SendNotification(ByVal objUser As UserInfo, ByVal objRole As RoleInfo, ByVal PortalSettings As PortalSettings, ByVal Action As UserRoleActions)
            Dim objRoles As New RoleController

            Dim Custom As New ArrayList
            Custom.Add(objRole.RoleName)
            Custom.Add(objRole.Description)

            Select Case Action
                Case UserRoleActions.add, UserRoleActions.update
                    Dim preferredLocale As String = objUser.Profile.PreferredLocale
                    If String.IsNullOrEmpty(preferredLocale) Then
                        preferredLocale = PortalSettings.DefaultLanguage
                    End If
                    Dim ci As New CultureInfo(preferredLocale)
                    Dim objUserRole As UserRoleInfo = objRoles.GetUserRole(PortalSettings.PortalId, objUser.UserID, objRole.RoleID)
                    If Null.IsNull(objUserRole.EffectiveDate) Then
                        Custom.Add(DateTime.Today.ToString("g", ci))
                    Else
                        Custom.Add(objUserRole.EffectiveDate.ToString("g", ci))
                    End If
                    If Null.IsNull(objUserRole.ExpiryDate) Then
                        Custom.Add("-")
                    Else
                        Custom.Add(objUserRole.ExpiryDate.ToString("g", ci))
                    End If
                Case UserRoleActions.delete
                    Custom.Add("")
            End Select
            Mail.SendMail(PortalSettings.Email, objUser.Email, "", _
                Services.Localization.Localization.GetSystemMessage(objUser.Profile.PreferredLocale, PortalSettings, "EMAIL_ROLE_" & UserRoleActionsCaption(Action) & "_SUBJECT", objUser), _
                Services.Localization.Localization.GetSystemMessage(objUser.Profile.PreferredLocale, PortalSettings, "EMAIL_ROLE_" & UserRoleActionsCaption(Action) & "_BODY", objUser, Services.Localization.Localization.GlobalResourceFile, Custom), _
                "", "", "", "", "", "")

        End Sub

#End Region

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a Role Group
        ''' </summary>
        ''' <param name="objRoleGroupInfo">The RoleGroup to Add</param>
        ''' <returns>The Id of the new role</returns>
        ''' <history>
        ''' 	[cnurse]	01/03/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function AddRoleGroup(ByVal objRoleGroupInfo As RoleGroupInfo) As Integer
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objRoleGroupInfo, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_CREATED)
            Return provider.CreateRoleGroup(objRoleGroupInfo)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a User to a Role
        ''' </summary>
        ''' <param name="objUser">The user to assign</param>
        ''' <param name="objRole">The role to add</param>
        ''' <param name="PortalSettings">The PortalSettings of the Portal</param>
        ''' <param name="effDate">The expiry Date of the Role membership</param>
        ''' <param name="expDate">The expiry Date of the Role membership</param>
        ''' <param name="UserId">The Id of the User assigning the role</param>
        ''' <param name="notifyUser">A flag that indicates whether the user should be notified</param>
        ''' <history>
        '''     [cnurse]    10/17/2007  Created  (Refactored code from Security Roles user control)
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddUserRole(ByVal objUser As UserInfo, ByVal objRole As RoleInfo, ByVal PortalSettings As PortalSettings, ByVal effDate As Date, ByVal expDate As Date, ByVal userId As Integer, ByVal notifyUser As Boolean)
            Dim objRoleController As New RoleController
            Dim objUserRole As UserRoleInfo = objRoleController.GetUserRole(PortalSettings.PortalId, objUser.UserID, objRole.RoleID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController

            ' update assignment
            objRoleController.AddUserRole(PortalSettings.PortalId, objUser.UserID, objRole.RoleID, effDate, expDate)

            'Set flag on user to make sure its portal roles cookie is refreshed
            objUser.RefreshRoles = True
            UserController.UpdateUser(PortalSettings.PortalId, objUser)

            If objUserRole Is Nothing Then
                objEventLog.AddLog("Role", objRole.RoleName, PortalSettings, userId, Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_CREATED)

                ' send notification
                If notifyUser Then
                    SendNotification(objUser, objRole, PortalSettings, UserRoleActions.add)
                End If
            Else
                objEventLog.AddLog("Role", objRole.RoleName, PortalSettings, userId, Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_UPDATED)
                If notifyUser Then
                    objUserRole = objRoleController.GetUserRole(PortalSettings.PortalId, objUser.UserID, objRole.RoleID)
                    SendNotification(objUser, objRole, PortalSettings, UserRoleActions.update)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Removes a User from a Role
        ''' </summary>
        ''' <param name="userId">The Id of the user to remove</param>
        ''' <param name="objRole">The role to remove the use from</param>
        ''' <param name="PortalSettings">The PortalSettings of the Portal</param>
        ''' <param name="notifyUser">A flag that indicates whether the user should be notified</param>
        ''' <history>
        '''     [cnurse]    10/17/2007  Created  (Refactored code from Security Roles user control)
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function DeleteUserRole(ByVal userId As Integer, ByVal objRole As RoleInfo, ByVal PortalSettings As PortalSettings, ByVal notifyUser As Boolean) As Boolean
            Dim objUser As UserInfo = UserController.GetUserById(PortalSettings.PortalId, userId)

            Return DeleteUserRole(objUser, objRole, PortalSettings, notifyUser)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Removes a User from a Role
        ''' </summary>
        ''' <param name="roleId">The Id of the role to remove the user from</param>
        ''' <param name="objUser">The user to remove</param>
        ''' <param name="PortalSettings">The PortalSettings of the Portal</param>
        ''' <param name="notifyUser">A flag that indicates whether the user should be notified</param>
        ''' <history>
        '''     [cnurse]    10/17/2007  Created  (Refactored code from Security Roles user control)
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function DeleteUserRole(ByVal roleId As Integer, ByVal objUser As UserInfo, ByVal PortalSettings As PortalSettings, ByVal notifyUser As Boolean) As Boolean
            Dim objRoleController As New RoleController
            Dim objRole As RoleInfo = objRoleController.GetRole(roleId, PortalSettings.PortalId)

            Return DeleteUserRole(objUser, objRole, PortalSettings, notifyUser)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Removes a User from a Role
        ''' </summary>
        ''' <param name="objUser">The user to remove</param>
        ''' <param name="objRole">The role to remove the use from</param>
        ''' <param name="PortalSettings">The PortalSettings of the Portal</param>
        ''' <param name="notifyUser">A flag that indicates whether the user should be notified</param>
        ''' <history>
        '''     [cnurse]    10/17/2007  Created  (Refactored code from Security Roles user control)
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function DeleteUserRole(ByVal objUser As UserInfo, ByVal objRole As RoleInfo, ByVal PortalSettings As PortalSettings, ByVal notifyUser As Boolean) As Boolean
            Dim objRoleController As New RoleController
            Dim canDelete As Boolean = objRoleController.DeleteUserRole(PortalSettings.PortalId, objUser.UserID, objRole.RoleID)
            If canDelete Then
                If notifyUser Then
                    SendNotification(objUser, objRole, PortalSettings, UserRoleActions.delete)
                End If
            End If
            Return canDelete
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Determines if the specified user can be removed from a role
        ''' </summary>
        ''' <remarks>
        ''' Roles such as "Registered Users" and "Administrators" can only
        ''' be removed in certain circumstances
        ''' </remarks>
        ''' <param name="PortalSettings">A <see cref="PortalSettings">PortalSettings</see> structure representing the current portal settings</param>
        ''' <param name="UserId">The Id of the User that should be checked for role removability</param>
        ''' <param name="RoleId">The Id of the Role that should be checked for removability</param>
        ''' <returns></returns>
        ''' <history>
        ''' 	[anurse]	01/12/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CanRemoveUserFromRole(ByVal PortalSettings As PortalSettings, ByVal UserId As Integer, ByVal RoleId As Integer) As Boolean
            ' [DNN-4285] Refactored this check into a method for use in SecurityRoles.ascx.vb
            ' HACK: Duplicated in CanRemoveUserFromRole(PortalInfo, Integer, Integer) method below
            ' changes to this method should be reflected in the other method as well
            Return Not ((PortalSettings.AdministratorId = UserId AndAlso PortalSettings.AdministratorRoleId = RoleId) OrElse PortalSettings.RegisteredRoleId = RoleId)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Determines if the specified user can be removed from a role
        ''' </summary>
        ''' <remarks>
        ''' Roles such as "Registered Users" and "Administrators" can only
        ''' be removed in certain circumstances
        ''' </remarks>
        ''' <param name="PortalInfo">A <see cref="PortalInfo">PortalInfo</see> structure representing the current portal</param>
        ''' <param name="UserId">The Id of the User</param>
        ''' <param name="RoleId">The Id of the Role that should be checked for removability</param>
        ''' <returns></returns>
        ''' <history>
        ''' 	[anurse]	01/12/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CanRemoveUserFromRole(ByVal PortalInfo As PortalInfo, ByVal UserId As Integer, ByVal RoleId As Integer) As Boolean
            ' [DNN-4285] Refactored this check into a method for use in SecurityRoles.ascx.vb
            ' HACK: Duplicated in CanRemoveUserFromRole(PortalSettings, Integer, Integer) method above
            ' changes to this method should be reflected in the other method as well
            Return Not ((PortalInfo.AdministratorId = UserId And PortalInfo.AdministratorRoleId = RoleId) Or PortalInfo.RegisteredRoleId = RoleId)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a Role Group
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/03/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteRoleGroup(ByVal PortalID As Integer, ByVal RoleGroupId As Integer)

            DeleteRoleGroup(GetRoleGroup(PortalID, RoleGroupId))

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a Role Group
        ''' </summary>
        ''' <param name="objRoleGroupInfo">The RoleGroup to Delete</param>
        ''' <history>
        ''' 	[cnurse]	01/03/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteRoleGroup(ByVal objRoleGroupInfo As RoleGroupInfo)

            provider.DeleteRoleGroup(objRoleGroupInfo)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objRoleGroupInfo, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_DELETED)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fetch a single RoleGroup
        ''' </summary>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[cnurse]	01/03/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetRoleGroup(ByVal PortalID As Integer, ByVal RoleGroupID As Integer) As RoleGroupInfo
            Return provider.GetRoleGroup(PortalID, RoleGroupID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fetch a single RoleGroup by Name
        ''' </summary>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' 
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[cnurse]	01/03/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetRoleGroupByName(ByVal PortalID As Integer, ByVal RoleGroupName As String) As RoleGroupInfo
            Return provider.GetRoleGroupByName(PortalID, RoleGroupName)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an ArrayList of RoleGroups
        ''' </summary>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <returns>An ArrayList of RoleGroups</returns>
        ''' <history>
        ''' 	[cnurse]	01/03/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetRoleGroups(ByVal PortalID As Integer) As ArrayList
            Return provider.GetRoleGroups(PortalID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Serializes the role groups
        ''' </summary>
        ''' <param name="writer">An XmlWriter</param>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <history>
        ''' 	[cnurse]	03/18/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SerializeRoleGroups(ByVal writer As XmlWriter, ByVal portalID As Integer)
            'Serialize Role Groups
            writer.WriteStartElement("rolegroups")
            For Each objRoleGroup As RoleGroupInfo In GetRoleGroups(portalID)
                CBO.SerializeObject(objRoleGroup, writer)
            Next

            'Serialize Global Roles
            Dim globalRoleGroup As New RoleGroupInfo(Null.NullInteger, portalID, True)
            globalRoleGroup.RoleGroupName = "GlobalRoles"
            globalRoleGroup.Description = "A dummy role group that represents the Global roles"
            CBO.SerializeObject(globalRoleGroup, writer)

            writer.WriteEndElement()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Serializes the roles
        ''' </summary>
        ''' <param name="writer">An XmlWriter</param>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <history>
        ''' 	[cnurse]	03/17/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SerializeRoles(ByVal writer As XmlWriter, ByVal portalID As Integer)
            'Serialize Global Roles
            writer.WriteStartElement("roles")
            For Each objRole As RoleInfo In New RoleController().GetRolesByGroup(portalID, Null.NullInteger)
                CBO.SerializeObject(objRole, writer)
            Next
            writer.WriteEndElement()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a Role Group
        ''' </summary>
        ''' <param name="roleGroup">The RoleGroup to Update</param>
        ''' <history>
        ''' 	[cnurse]	01/03/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub UpdateRoleGroup(ByVal roleGroup As RoleGroupInfo)
            UpdateRoleGroup(roleGroup, False)
        End Sub

        Public Shared Sub UpdateRoleGroup(ByVal roleGroup As RoleGroupInfo, ByVal includeRoles As Boolean)
            provider.UpdateRoleGroup(roleGroup)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(roleGroup, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_UPDATED)
            If includeRoles Then
                Dim controller As New RoleController()
                For Each role As RoleInfo In roleGroup.Roles.Values
                    controller.UpdateRole(role)
                    objEventLog.AddLog(role, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.ROLE_UPDATED)
                Next
            End If
        End Sub

#End Region

#Region "Obsoleted Methods, retained for Binary Compatability"

        <Obsolete("This function has been replaced by AddRole(objRoleInfo)")> _
        Public Function AddRole(ByVal objRoleInfo As RoleInfo, ByVal SynchronizationMode As Boolean) As Integer
            AddRole(objRoleInfo)
        End Function

        <Obsolete("This function has been replaced by GetPortalRoles(PortalId)")> _
        Public Function GetPortalRoles(ByVal PortalId As Integer, ByVal SynchronizeRoles As Boolean) As ArrayList
            Return GetPortalRoles(PortalId)
        End Function

        <Obsolete("This function has been replaced by GetRolesByUser")> _
        Public Function GetPortalRolesByUser(ByVal UserId As Integer, ByVal PortalId As Integer) As String()
            Return GetRolesByUser(UserId, PortalId)
        End Function

        <Obsolete("This function has been replaced by GetUserRoles")> _
        Public Function GetServices(ByVal PortalId As Integer) As ArrayList
            Return GetUserRoles(PortalId, -1, False)
        End Function

        <Obsolete("This function has been replaced by GetUserRoles")> _
        Public Function GetServices(ByVal PortalId As Integer, ByVal UserId As Integer) As ArrayList
            Return GetUserRoles(PortalId, UserId, False)
        End Function

        <Obsolete("This function has been replaced by GetUserRolesByRoleName")> _
        Public Function GetUsersInRole(ByVal PortalID As Integer, ByVal RoleName As String) As ArrayList
            Return provider.GetUserRolesByRoleName(PortalID, RoleName)
        End Function

        <Obsolete("This function has been replaced by UpdateUserRole")> _
        Public Sub UpdateService(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal RoleId As Integer)
            UpdateUserRole(PortalId, UserId, RoleId, False)
        End Sub

        <Obsolete("This function has been replaced by UpdateUserRole")> _
        Public Sub UpdateService(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal RoleId As Integer, ByVal Cancel As Boolean)
            UpdateUserRole(PortalId, UserId, RoleId, Cancel)
        End Sub

#End Region

    End Class

End Namespace
