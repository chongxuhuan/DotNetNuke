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
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Services.FileSystem

Namespace DotNetNuke.Security.Permissions

    Public Class FolderPermissionController

#Region "Private Members"

        Private Shared provider As PermissionProvider = PermissionProvider.Instance()

#End Region

#Region "Private Shared Methods"

        Private Shared Sub ClearPermissionCache(ByVal PortalID As Integer)
            DataCache.ClearFolderPermissionsCache(PortalID)
        End Sub

#End Region

#Region "Public Shared Methods"

        Public Shared Function CanAddFolder(ByVal folder As FolderInfo) As Boolean
            Return provider.CanAddFolder(folder) OrElse CanAdminFolder(folder)
        End Function

        Public Shared Function CanAdminFolder(ByVal folder As FolderInfo) As Boolean
            Return provider.CanAdminFolder(folder)
        End Function

        Public Shared Function CanCopyFolder(ByVal folder As FolderInfo) As Boolean
            Return provider.CanCopyFolder(folder) OrElse CanAdminFolder(folder)
        End Function

        Public Shared Function CanDeleteFolder(ByVal folder As FolderInfo) As Boolean
            Return provider.CanDeleteFolder(folder) OrElse CanAdminFolder(folder)
        End Function

        Public Shared Function CanManageFolder(ByVal folder As FolderInfo) As Boolean
            Return provider.CanManageFolder(folder) OrElse CanAdminFolder(folder)
        End Function

        Public Shared Function CanViewFolder(ByVal folder As FolderInfo) As Boolean
            Return provider.CanViewFolder(folder) OrElse CanAdminFolder(folder)
        End Function

        Public Shared Sub DeleteFolderPermissionsByUser(ByVal objUser As UserInfo)
            provider.DeleteFolderPermissionsByUser(objUser)
            ClearPermissionCache(objUser.PortalID)
        End Sub

        Public Shared Function GetFolderPermissionsCollectionByFolder(ByVal PortalID As Integer, ByVal Folder As String) As FolderPermissionCollection
            Return provider.GetFolderPermissionsCollectionByFolder(PortalID, Folder)
        End Function

        Public Shared Function HasFolderPermission(ByVal portalId As Integer, ByVal folderPath As String, ByVal permissionKey As String) As Boolean
            Return HasFolderPermission(FolderPermissionController.GetFolderPermissionsCollectionByFolder(portalId, folderPath), _
                                       permissionKey)
        End Function

        Public Shared Function HasFolderPermission(ByVal objFolderPermissions As FolderPermissionCollection, ByVal PermissionKey As String) As Boolean
            Dim hasPermission As Boolean = provider.HasFolderPermission(objFolderPermissions, "WRITE")
            If Not hasPermission Then
                If PermissionKey.Contains(",") Then
                    For Each permission As String In PermissionKey.Split(","c)
                        If provider.HasFolderPermission(objFolderPermissions, permission) Then
                            hasPermission = True
                            Exit For
                        End If
                    Next
                Else
                    hasPermission = provider.HasFolderPermission(objFolderPermissions, PermissionKey)
                End If
            End If
            Return hasPermission
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SaveFolderPermissions updates a Folder's permissions
        ''' </summary>
        ''' <param name="folder">The Folder to update</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SaveFolderPermissions(ByVal folder As FolderInfo)
            provider.SaveFolderPermissions(folder)
            DataCache.ClearFolderPermissionsCache(folder.PortalID)
        End Sub

#End Region

#Region "Obsolete Methods"

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Function AddFolderPermission(ByVal objFolderPermission As FolderPermissionInfo) As Integer
            ClearPermissionCache(objFolderPermission.PortalID)
            Return CType(DataProvider.Instance().AddFolderPermission(objFolderPermission.FolderID, objFolderPermission.PermissionID, objFolderPermission.RoleID, objFolderPermission.AllowAccess, objFolderPermission.UserID, UserController.GetCurrentUserInfo.UserID), Integer)
        End Function

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Sub DeleteFolderPermission(ByVal FolderPermissionID As Integer)
            DataProvider.Instance().DeleteFolderPermission(FolderPermissionID)
        End Sub

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Sub DeleteFolderPermissionsByFolder(ByVal PortalID As Integer, ByVal FolderPath As String)
            DataProvider.Instance().DeleteFolderPermissionsByFolderPath(PortalID, FolderPath)
            ClearPermissionCache(PortalID)
        End Sub

        <Obsolete("Deprecated in DNN 5.0.  Use DeleteFolderPermissionsByUser(UserInfo) ")> _
        Public Sub DeleteFolderPermissionsByUserID(ByVal objUser As UserInfo)
            DataProvider.Instance().DeleteFolderPermissionsByUserID(objUser.PortalID, objUser.UserID)
            ClearPermissionCache(objUser.PortalID)
        End Sub

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Function GetFolderPermission(ByVal FolderPermissionID As Integer) As FolderPermissionInfo
            Return CBO.FillObject(Of FolderPermissionInfo)(DataProvider.Instance().GetFolderPermission(FolderPermissionID), True)
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use GetFolderPermissionsCollectionByFolderPath(PortalId, Folder)")> _
        Public Function GetFolderPermissionsByFolder(ByVal PortalID As Integer, ByVal Folder As String) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetFolderPermissionsByFolderPath(PortalID, Folder, -1), GetType(FolderPermissionInfo))
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use GetFolderPermissionsCollectionByFolderPath(PortalId, Folder)")> _
        Public Function GetFolderPermissionsByFolder(ByVal arrFolderPermissions As ArrayList, ByVal FolderPath As String) As FolderPermissionCollection
            Return New FolderPermissionCollection(arrFolderPermissions, FolderPath)
        End Function

        <Obsolete("Deprecated in DNN 5.1.  GetModulePermissions(ModulePermissionCollection, String) ")> _
        Public Function GetFolderPermissionsByFolderPath(ByVal arrFolderPermissions As ArrayList, ByVal FolderPath As String, ByVal PermissionKey As String) As String
            'Create a Folder Permission Collection from the ArrayList
            Dim folderPermissions As FolderPermissionCollection = New FolderPermissionCollection(arrFolderPermissions, FolderPath)

            'Return the permission string for permissions with specified FolderPath
            Return folderPermissions.ToString(PermissionKey)
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use GetFolderPermissionsCollectionByFolder(PortalId, Folder)")> _
        Public Function GetFolderPermissionsCollectionByFolderPath(ByVal PortalID As Integer, ByVal Folder As String) As FolderPermissionCollection
            Return GetFolderPermissionsCollectionByFolder(PortalID, Folder)
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use GetFolderPermissionsCollectionByFolder(PortalId, Folder)")> _
        Public Function GetFolderPermissionsCollectionByFolderPath(ByVal arrFolderPermissions As ArrayList, ByVal FolderPath As String) As FolderPermissionCollection
            Dim objFolderPermissionCollection As New FolderPermissionCollection(arrFolderPermissions, FolderPath)
            Return objFolderPermissionCollection
        End Function

        <Obsolete("Deprecated in DNN 5.1.")> _
        Public Sub UpdateFolderPermission(ByVal objFolderPermission As FolderPermissionInfo)
            DataProvider.Instance().UpdateFolderPermission(objFolderPermission.FolderPermissionID, objFolderPermission.FolderID, objFolderPermission.PermissionID, objFolderPermission.RoleID, objFolderPermission.AllowAccess, objFolderPermission.UserID,UserController.GetCurrentUserInfo.UserID)
            ClearPermissionCache(objFolderPermission.PortalID)
        End Sub

#End Region

    End Class

End Namespace
