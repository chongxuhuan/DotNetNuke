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
Imports System.Collections.Generic
Imports System.Data
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Security.Permissions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Security.Permissions
    ''' Class	 : ModulePermissionController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModulePermissionController provides the Business Layer for Module Permissions
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ModulePermissionController

#Region "Private Members"

        Private Shared provider As PermissionProvider = PermissionProvider.Instance()

#End Region

#Region "Private Shared Methdos"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ClearPermissionCache clears the Module Permission Cache
        ''' </summary>
        ''' <param name="moduleID">The ID of the Module</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub ClearPermissionCache(ByVal moduleId As Integer)
            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo = objModules.GetModule(moduleId, Null.NullInteger, False)
            DataCache.ClearModulePermissionsCache(objModule.TabID)
        End Sub

        Private Shared Function CanAddContentToPage(ByVal objModule As ModuleInfo) As Boolean
            Dim canManage As Boolean = Null.NullBoolean
            Dim objTab As TabInfo = New TabController().GetTab(objModule.TabID, objModule.PortalID, False)
            canManage = TabPermissionController.CanAddContentToPage(objTab)
            Return canManage
        End Function

#End Region

#Region "Public Shared Methods"

        Public Shared Function CanAdminModule(ByVal objModule As ModuleInfo) As Boolean
            Return provider.CanAdminModule(objModule)
        End Function

        Public Shared Function CanDeleteModule(ByVal objModule As ModuleInfo) As Boolean
            Return CanAddContentToPage(objModule) OrElse provider.CanDeleteModule(objModule)
        End Function

        Public Shared Function CanEditModuleContent(ByVal objModule As ModuleInfo) As Boolean
            Return CanAddContentToPage(objModule) OrElse provider.CanEditModuleContent(objModule)
        End Function

        Public Shared Function CanExportModule(ByVal objModule As ModuleInfo) As Boolean
            Return provider.CanExportModule(objModule)
        End Function

        Public Shared Function CanImportModule(ByVal objModule As ModuleInfo) As Boolean
            Return provider.CanImportModule(objModule)
        End Function

        Public Shared Function CanManageModule(ByVal objModule As ModuleInfo) As Boolean
            Return CanAddContentToPage(objModule) OrElse provider.CanManageModule(objModule)
        End Function

        Public Shared Function CanViewModule(ByVal objModule As ModuleInfo) As Boolean
            Dim canView As Boolean = Null.NullBoolean
            If objModule.InheritViewPermissions Then
                Dim objTab As TabInfo = New TabController().GetTab(objModule.TabID, objModule.PortalID, False)
                canView = TabPermissionController.CanViewPage(objTab)
            Else
                canView = provider.CanViewModule(objModule)
            End If
            Return canView
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteModulePermissionsByUser deletes a user's Module Permission in the Database
        ''' </summary>
        ''' <param name="objUser">The user</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteModulePermissionsByUser(ByVal objUser As UserInfo)
            provider.DeleteModulePermissionsByUser(objUser)
            DataCache.ClearModulePermissionsCachesByPortal(objUser.PortalID)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModulePermissions gets a ModulePermissionCollection
        ''' </summary>
        ''' <param name="moduleID">The ID of the module</param>
        ''' <param name="tabID">The ID of the tab</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModulePermissions(ByVal moduleID As Integer, ByVal tabID As Integer) As ModulePermissionCollection
            Return provider.GetModulePermissions(moduleID, tabID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HasModulePermission checks whether the current user has a specific Module Permission
        ''' </summary>
        ''' <remarks>If you pass in a comma delimited list of permissions (eg "ADD,DELETE", this will return
        ''' true if the user has any one of the permissions.</remarks>
        ''' <param name="objModulePermissions">The Permissions for the Module</param>
        ''' <param name="permissionKey">The Permission to check</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function HasModulePermission(ByVal objModulePermissions As ModulePermissionCollection, ByVal permissionKey As String) As Boolean
            Dim hasPermission As Boolean = Null.NullBoolean
            If permissionKey.Contains(",") Then
                For Each permission As String In permissionKey.Split(","c)
                    If provider.HasModulePermission(objModulePermissions, permission) Then
                        hasPermission = True
                        Exit For
                    End If
                Next
            Else
                hasPermission = provider.HasModulePermission(objModulePermissions, permissionKey)
            End If
            Return hasPermission
        End Function

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Determines if user has the necessary permissions to access an item with the
        ''' designated AccessLevel.
        ''' </summary>
        ''' <param name="AccessLevel">The SecurityAccessLevel required to access a portal module or module action.</param>
        ''' <param name="permissionKey">If Security Access is Edit the permissionKey is the actual "edit" permisison required.</param>
        ''' <param name="ModuleConfiguration">The ModuleInfo object for the associated module.</param>
        ''' <returns>A boolean value indicating if the user has the necessary permissions</returns>
        ''' <remarks>Every module control and module action has an associated permission level.  This
        ''' function determines whether the user represented by UserName has sufficient permissions, as
        ''' determined by the PortalSettings and ModuleSettings, to access a resource with the 
        ''' designated AccessLevel.</remarks>
        ''' <history>
        '''     [cnurse]        02/27/2007  New overload
        '''     [cnurse]        02/27/2007  Moved from PortalSecurity
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Shared Function HasModuleAccess(ByVal AccessLevel As SecurityAccessLevel, ByVal permissionKey As String, ByVal ModuleConfiguration As ModuleInfo) As Boolean
            Dim blnAuthorized As Boolean = False
            Dim objUser As UserInfo = UserController.GetCurrentUserInfo

            If Not objUser Is Nothing AndAlso objUser.IsSuperUser Then
                blnAuthorized = True
            Else
                Select Case AccessLevel
                    Case SecurityAccessLevel.Anonymous
                        blnAuthorized = True
                    Case SecurityAccessLevel.View     ' view
                        If TabPermissionController.CanViewPage() OrElse CanViewModule(ModuleConfiguration) Then
                            blnAuthorized = True
                        End If
                    Case SecurityAccessLevel.Edit     ' edit
                        If TabPermissionController.CanAddContentToPage() Then
                            blnAuthorized = True
                        Else
                            If String.IsNullOrEmpty(permissionKey) Then
                                permissionKey = "CONTENT,DELETE,EDIT,EXPORT,IMPORT,MANAGE"
                            End If
                            If ModuleConfiguration IsNot Nothing AndAlso CanViewModule(ModuleConfiguration) AndAlso _
                                        (HasModulePermission(ModuleConfiguration.ModulePermissions, permissionKey) OrElse _
                                            HasModulePermission(ModuleConfiguration.ModulePermissions, "EDIT")) Then
                                blnAuthorized = True
                            End If
                        End If
                    Case SecurityAccessLevel.Admin     ' admin
                        If TabPermissionController.CanAdminPage Then
                            blnAuthorized = True
                        ElseIf TabPermissionController.CanAddContentToPage() Then
                            If String.IsNullOrEmpty(permissionKey) Then
                                permissionKey = "CONTENT,DELETE,EDIT,EXPORT,IMPORT,MANAGE"
                            End If
                            If ModuleConfiguration IsNot Nothing AndAlso CanViewModule(ModuleConfiguration) AndAlso _
                                        HasModulePermission(ModuleConfiguration.ModulePermissions, permissionKey) Then
                                blnAuthorized = True
                            End If
                        End If
                    Case SecurityAccessLevel.Host     ' host
                End Select
            End If

            Return blnAuthorized
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SaveModulePermissions updates a Module's permissions
        ''' </summary>
        ''' <param name="objModule">The Module to update</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SaveModulePermissions(ByVal objModule As ModuleInfo)
            provider.SaveModulePermissions(objModule)
            DataCache.ClearModulePermissionsCache(objModule.TabID)
        End Sub

#End Region

#Region "Obsolete Methods"

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Function AddModulePermission(ByVal objModulePermission As ModulePermissionInfo, ByVal tabId As Integer) As Integer
            Dim Id As Integer = DataProvider.Instance().AddModulePermission(objModulePermission.ModuleID, objModulePermission.PermissionID, objModulePermission.RoleID, objModulePermission.AllowAccess, objModulePermission.UserID, UserController.GetCurrentUserInfo.UserID)
            DataCache.ClearModulePermissionsCache(tabId)
            Return Id
        End Function

        <Obsolete("Deprecated in DNN 5.0.")> _
        Public Function AddModulePermission(ByVal objModulePermission As ModulePermissionInfo) As Integer
            Dim Id As Integer = DataProvider.Instance().AddModulePermission(objModulePermission.ModuleID, objModulePermission.PermissionID, objModulePermission.RoleID, objModulePermission.AllowAccess, objModulePermission.UserID, UserController.GetCurrentUserInfo.UserID)
            ClearPermissionCache(objModulePermission.ModuleID)
            Return Id
        End Function

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Sub DeleteModulePermission(ByVal modulePermissionID As Integer)
            DataProvider.Instance().DeleteModulePermission(modulePermissionID)
        End Sub

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Sub DeleteModulePermissionsByModuleID(ByVal moduleID As Integer)
            DataProvider.Instance().DeleteModulePermissionsByModuleID(moduleID)
            ClearPermissionCache(moduleID)
        End Sub

        <Obsolete("Deprecated in DNN 5.0.  Use DeleteModulePermissionsByUser(UserInfo) ")> _
        Public Sub DeleteModulePermissionsByUserID(ByVal objUser As UserInfo)
            DataProvider.Instance().DeleteModulePermissionsByUserID(objUser.PortalID, objUser.UserID)
            DataCache.ClearModulePermissionsCachesByPortal(objUser.PortalID)
        End Sub

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Function GetModulePermission(ByVal modulePermissionID As Integer) As ModulePermissionInfo
            Return CBO.FillObject(Of ModulePermissionInfo)(DataProvider.Instance().GetModulePermission(modulePermissionID), True)
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by ModulePermissionColelction.ToString(String)")> _
         Public Function GetModulePermissions(ByVal modulePermissions As ModulePermissionCollection, ByVal permissionKey As String) As String
            Return modulePermissions.ToString(permissionKey)
        End Function

        <Obsolete("Deprecated in DNN 5.0.  This should have been declared as Friend as it was never meant to be used outside of the core.")> _
        Public Function GetModulePermissionsByPortal(ByVal PortalID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetModulePermissionsByPortal(PortalID), GetType(ModulePermissionInfo))
        End Function

        <Obsolete("Deprecated in DNN 5.0.  This should have been declared as Friend as it was never meant to be used outside of the core.")> _
        Public Function GetModulePermissionsByTabID(ByVal TabID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetModulePermissionsByTabID(TabID), GetType(ModulePermissionInfo))
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Use GetModulePermissions(ModulePermissionCollection, String) ")> _
        Public Function GetModulePermissionsByModuleID(ByVal objModule As ModuleInfo, ByVal PermissionKey As String) As String
            'Create a Module Permission Collection from the ArrayList
            Dim modulePermissions As ModulePermissionCollection = New ModulePermissionCollection(objModule)

            'Return the permission string for permissions with specified TabId
            Return modulePermissions.ToString(PermissionKey)
        End Function

        <Obsolete("Deprecated in DNN 5.1.  GetModulePermissions(integer, integer) ")> _
        Public Function GetModulePermissionsCollectionByModuleID(ByVal moduleID As Integer, ByVal tabID As Integer) As ModulePermissionCollection
            Return GetModulePermissions(moduleID, tabID)
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Use GetModulePermissions(integer, integer) ")> _
        Public Function GetModulePermissionsCollectionByModuleID(ByVal moduleID As Integer) As Security.Permissions.ModulePermissionCollection
            Return New ModulePermissionCollection(CBO.FillCollection(DataProvider.Instance().GetModulePermissionsByModuleID(moduleID, -1), GetType(ModulePermissionInfo)))
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Use GetModulePermissions(integer, integer) ")> _
        Public Function GetModulePermissionsCollectionByModuleID(ByVal arrModulePermissions As ArrayList, ByVal moduleID As Integer) As Security.Permissions.ModulePermissionCollection
            Return New ModulePermissionCollection(arrModulePermissions, moduleID)
        End Function

        <Obsolete("Deprecated in DNN 5.0.  It was used to replace lists of RoleIds by lists of RoleNames.")> _
        Public Function GetRoleNamesFromRoleIDs(ByVal Roles As String) As String
            Dim strRoles As String = ""
            If Roles.IndexOf(";") > 0 Then
                Dim arrRoles As String() = Split(Roles, ";")
                Dim i As Integer
                For i = 0 To arrRoles.Length - 1
                    If IsNumeric(arrRoles(i)) Then
                        strRoles += GetRoleName(Convert.ToInt32(arrRoles(i))) + ";"
                    End If
                Next
            ElseIf Roles.Trim.Length > 0 Then
                strRoles = GetRoleName(Convert.ToInt32(Roles.Trim))
            End If
            If Not strRoles.StartsWith(";") Then
                strRoles += ";"
            End If
            Return strRoles
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Use HasModulePermission(ModulePermissionCollection, string)")> _
        Public Shared Function HasModulePermission(ByVal moduleID As Integer, ByVal PermissionKey As String) As Boolean
            Dim objModulePermissions As ModulePermissionCollection = New ModulePermissionCollection(CBO.FillCollection(DataProvider.Instance().GetModulePermissionsByModuleID(moduleID, -1), GetType(ModulePermissionInfo)))
            Return HasModulePermission(objModulePermissions, PermissionKey)
        End Function

        <Obsolete("Deprecated in DNN 5.1.  Use HasModulePermission(ModulePermissionCollection, string)")> _
        Public Shared Function HasModulePermission(ByVal moduleID As Integer, ByVal tabID As Integer, ByVal PermissionKey As String) As Boolean
            Return HasModulePermission(GetModulePermissions(moduleID, tabID), PermissionKey)
        End Function

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Sub UpdateModulePermission(ByVal objModulePermission As ModulePermissionInfo)
            DataProvider.Instance().UpdateModulePermission(objModulePermission.ModulePermissionID, objModulePermission.ModuleID, objModulePermission.PermissionID, objModulePermission.RoleID, objModulePermission.AllowAccess, objModulePermission.UserID, UserController.GetCurrentUserInfo.UserID)
            ClearPermissionCache(objModulePermission.ModuleID)
        End Sub

#End Region

    End Class
End Namespace
