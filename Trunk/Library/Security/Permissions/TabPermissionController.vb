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
Imports System.Collections.Generic
Imports System.Data
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Security.Permissions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Security.Permissions
    ''' Class	 : TabPermissionController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' TabPermissionController provides the Business Layer for Tab Permissions
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class TabPermissionController

#Region "Private Members"

        Private Shared provider As PermissionProvider = PermissionProvider.Instance()

#End Region

#Region "Private Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ClearPermissionCache clears the Tab Permission Cache
        ''' </summary>
        ''' <param name="tabId">The ID of the Tab</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub ClearPermissionCache(ByVal tabId As Integer)
            Dim objTabs As New TabController
            Dim objTab As TabInfo = objTabs.GetTab(tabId, Null.NullInteger, False)
            DataCache.ClearTabPermissionsCache(objTab.PortalID)
        End Sub

#End Region

#Region "Public Shared Methods"

        Public Shared Function CanAddContentToPage() As Boolean
            Return CanAddContentToPage(TabController.CurrentPage)
        End Function

        Public Shared Function CanAddContentToPage(ByVal objTab As TabInfo) As Boolean
            Return provider.CanAddContentToPage(objTab) OrElse CanAdminPage(objTab)
        End Function

        Public Shared Function CanAddPage() As Boolean
            Return CanAddPage(TabController.CurrentPage)
        End Function

        Public Shared Function CanAddPage(ByVal objTab As TabInfo) As Boolean
            Return provider.CanAddPage(objTab) OrElse CanAdminPage(objTab)
        End Function

        Public Shared Function CanAdminPage() As Boolean
            Return CanAdminPage(TabController.CurrentPage)
        End Function

        Public Shared Function CanAdminPage(ByVal objTab As TabInfo) As Boolean
            Return provider.CanAdminPage(objTab)
        End Function

        Public Shared Function CanCopyPage() As Boolean
            Return CanCopyPage(TabController.CurrentPage)
        End Function

        Public Shared Function CanCopyPage(ByVal objTab As TabInfo) As Boolean
            Return provider.CanCopyPage(objTab) OrElse CanAdminPage(objTab)
        End Function

        Public Shared Function CanDeletePage() As Boolean
            Return CanDeletePage(TabController.CurrentPage)
        End Function

        Public Shared Function CanDeletePage(ByVal objTab As TabInfo) As Boolean
            Return provider.CanDeletePage(objTab) OrElse CanAdminPage(objTab)
        End Function

        Public Shared Function CanExportPage() As Boolean
            Return CanExportPage(TabController.CurrentPage)
        End Function

        Public Shared Function CanExportPage(ByVal objTab As TabInfo) As Boolean
            Return provider.CanExportPage(objTab) OrElse CanAdminPage(objTab)
        End Function

        Public Shared Function CanImportPage() As Boolean
            Return CanImportPage(TabController.CurrentPage)
        End Function

        Public Shared Function CanImportPage(ByVal objTab As TabInfo) As Boolean
            Return provider.CanImportPage(objTab) OrElse CanAdminPage(objTab)
        End Function

        Public Shared Function CanManagePage() As Boolean
            Return CanManagePage(TabController.CurrentPage)
        End Function

        Public Shared Function CanManagePage(ByVal objTab As TabInfo) As Boolean
            Return provider.CanManagePage(objTab) OrElse CanAdminPage(objTab)
        End Function

        Public Shared Function CanNavigateToPage() As Boolean
            Return CanNavigateToPage(TabController.CurrentPage)
        End Function

        Public Shared Function CanNavigateToPage(ByVal objTab As TabInfo) As Boolean
            Return provider.CanNavigateToPage(objTab) OrElse CanAdminPage(objTab)
        End Function

        Public Shared Function CanViewPage() As Boolean
            Return CanViewPage(TabController.CurrentPage)
        End Function

        Public Shared Function CanViewPage(ByVal objTab As TabInfo) As Boolean
            Return provider.CanViewPage(objTab) OrElse CanAdminPage(objTab)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteTabPermissionsByUser deletes a user's Tab Permissions in the Database
        ''' </summary>
        ''' <param name="objUser">The user</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteTabPermissionsByUser(ByVal objUser As UserInfo)
            provider.DeleteTabPermissionsByUser(objUser)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objUser, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.TABPERMISSION_DELETED)
            DataCache.ClearTabPermissionsCache(objUser.PortalID)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetTabPermissions gets a TabPermissionCollection
        ''' </summary>
        ''' <param name="tabID">The ID of the tab</param>
        ''' <param name="portalID">The ID of the portal</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetTabPermissions(ByVal tabID As Integer, ByVal portalID As Integer) As TabPermissionCollection
            Return provider.GetTabPermissions(tabID, portalID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HasTabPermission checks whether the current user has a specific Tab Permission
        ''' </summary>
        ''' <remarks>If you pass in a comma delimited list of permissions (eg "ADD,DELETE", this will return
        ''' true if the user has any one of the permissions.</remarks>
        ''' <param name="permissionKey">The Permission to check</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Documented
        ''' 	[cnurse]	04/22/2009   Added multi-permisison support
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function HasTabPermission(ByVal permissionKey As String) As Boolean
            Return HasTabPermission(PortalController.GetCurrentPortalSettings.ActiveTab.TabPermissions, permissionKey)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HasTabPermission checks whether the current user has a specific Tab Permission
        ''' </summary>
        ''' <remarks>If you pass in a comma delimited list of permissions (eg "ADD,DELETE", this will return
        ''' true if the user has any one of the permissions.</remarks>
        ''' <param name="objTabPermissions">The Permissions for the Tab</param>
        ''' <param name="permissionKey">The Permission(s) to check</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Documented
        ''' 	[cnurse]	04/22/2009   Added multi-permisison support
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function HasTabPermission(ByVal objTabPermissions As TabPermissionCollection, ByVal permissionKey As String) As Boolean
            Dim hasPermission As Boolean = provider.HasTabPermission(objTabPermissions, "EDIT")
            If Not hasPermission Then
                If permissionKey.Contains(",") Then
                    For Each permission As String In permissionKey.Split(","c)
                        If provider.HasTabPermission(objTabPermissions, permission) Then
                            hasPermission = True
                            Exit For
                        End If
                    Next
                Else
                    hasPermission = provider.HasTabPermission(objTabPermissions, permissionKey)
                End If

            End If
            Return hasPermission
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SaveTabPermissions saves a Tab's permissions
        ''' </summary>
        ''' <param name="objTab">The Tab to update</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SaveTabPermissions(ByVal objTab As TabInfo)
            provider.SaveTabPermissions(objTab)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.TABPERMISSION_UPDATED)
            ClearPermissionCache(objTab.TabID)
        End Sub

#End Region

#Region "Obsolete Methods"

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Function AddTabPermission(ByVal objTabPermission As TabPermissionInfo) As Integer
            Dim Id As Integer = CType(DataProvider.Instance().AddTabPermission(objTabPermission.TabID, objTabPermission.PermissionID, objTabPermission.RoleID, objTabPermission.AllowAccess, objTabPermission.UserID, UserController.GetCurrentUserInfo.UserID), Integer)
            ClearPermissionCache(objTabPermission.TabID)
            Return Id
        End Function

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Sub DeleteTabPermission(ByVal tabPermissionID As Integer)
            DataProvider.Instance().DeleteTabPermission(tabPermissionID)
        End Sub

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Sub DeleteTabPermissionsByTabID(ByVal tabID As Integer)
            DataProvider.Instance().DeleteTabPermissionsByTabID(tabID)
            ClearPermissionCache(tabID)
        End Sub

        <Obsolete("Deprecated in DNN 5.0.  Use DeleteTabPermissionsByUser(UserInfo) ")> _
        Public Sub DeleteTabPermissionsByUserID(ByVal objUser As UserInfo)
            DataProvider.Instance().DeleteTabPermissionsByUserID(objUser.PortalID, objUser.UserID)
            DataCache.ClearTabPermissionsCache(objUser.PortalID)
        End Sub

        <Obsolete("Deprecated in DNN 5.0. Please use TabPermissionCollection.ToString(String)")> _
        Public Function GetTabPermissions(ByVal tabPermissions As TabPermissionCollection, ByVal permissionKey As String) As String
            Return tabPermissions.ToString(permissionKey)
        End Function

        <Obsolete("Deprecated in DNN 5.0.  This should have been declared as Friend as it was never meant to be used outside of the core.")> _
        Public Function GetTabPermissionsByPortal(ByVal PortalID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetTabPermissionsByPortal(PortalID), GetType(TabPermissionInfo))
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use GetTabPermissions(TabId, PortalId)")> _
        Public Function GetTabPermissionsByTabID(ByVal TabID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetTabPermissionsByTabID(TabID, -1), GetType(TabPermissionInfo))
        End Function

        <Obsolete("Deprecated in DNN 5.0. Please use TabPermissionCollection.ToString(String)")> _
        Public Function GetTabPermissionsByTabID(ByVal arrTabPermissions As ArrayList, ByVal TabID As Integer, ByVal PermissionKey As String) As String
            'Create a Tab Permission Collection from the ArrayList
            Dim tabPermissions As TabPermissionCollection = New TabPermissionCollection(arrTabPermissions, TabID)

            'Return the permission string for permissions with specified TabId
            Return tabPermissions.ToString(PermissionKey)
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use GetTabPermissions(TabId, PortalId)")> _
        Public Function GetTabPermissionsByTabID(ByVal arrTabPermissions As ArrayList, ByVal TabID As Integer) As TabPermissionCollection
            Return New TabPermissionCollection(arrTabPermissions, TabID)
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use GetTabPermissions(TabId, PortalId)")> _
        Public Function GetTabPermissionsCollectionByTabID(ByVal TabID As Integer) As Security.Permissions.TabPermissionCollection
            Return New TabPermissionCollection(CBO.FillCollection(DataProvider.Instance().GetTabPermissionsByTabID(TabID, -1), GetType(TabPermissionInfo)))
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use GetTabPermissions(TabId, PortalId)")> _
        Public Function GetTabPermissionsCollectionByTabID(ByVal arrTabPermissions As ArrayList, ByVal TabID As Integer) As TabPermissionCollection
            Return New TabPermissionCollection(arrTabPermissions, TabID)
        End Function

        <Obsolete("Deprecated in DNN 5.1.  Please use GetTabPermissions(TabId, PortalId)")> _
        Public Function GetTabPermissionsCollectionByTabID(ByVal tabID As Integer, ByVal portalID As Integer) As TabPermissionCollection
            Return GetTabPermissions(tabID, portalID)
        End Function

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Sub UpdateTabPermission(ByVal objTabPermission As TabPermissionInfo)
            DataProvider.Instance().UpdateTabPermission(objTabPermission.TabPermissionID, objTabPermission.TabID, objTabPermission.PermissionID, objTabPermission.RoleID, objTabPermission.AllowAccess, objTabPermission.UserID, UserController.GetCurrentUserInfo.UserID)
            ClearPermissionCache(objTabPermission.TabID)
        End Sub

#End Region

    End Class


End Namespace
