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

Imports System.Collections.Generic

Imports DotNetNuke.ComponentModel
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Services.FileSystem


Namespace DotNetNuke.Security.Permissions

    Public Class PermissionProvider

#Region "Private Members"

        'Module Permission Codes
        Private Const AdminFolderPermissionCode As String = "WRITE"
        Private Const AddFolderPermissionCode As String = "WRITE"
        Private Const CopyFolderPermissionCode As String = "WRITE"
        Private Const DeleteFolderPermissionCode As String = "WRITE"
        Private Const ManageFolderPermissionCode As String = "WRITE"
        Private Const ViewFolderPermissionCode As String = "READ"

        'Module Permission Codes
        Private Const AdminModulePermissionCode As String = "EDIT"
        Private Const ContentModulePermissionCode As String = "EDIT"
        Private Const DeleteModulePermissionCode As String = "EDIT"
        Private Const ExportModulePermissionCode As String = "EDIT"
        Private Const ImportModulePermissionCode As String = "EDIT"
        Private Const ManageModulePermissionCode As String = "EDIT"
        Private Const ViewModulePermissionCode As String = "VIEW"

        'Page Permission Codes
        Private Const AddPagePermissionCode As String = "EDIT"
        Private Const AdminPagePermissionCode As String = "EDIT"
        Private Const ContentPagePermissionCode As String = "EDIT"
        Private Const CopyPagePermissionCode As String = "EDIT"
        Private Const DeletePagePermissionCode As String = "EDIT"
        Private Const ExportPagePermissionCode As String = "EDIT"
        Private Const ImportPagePermissionCode As String = "EDIT"
        Private Const ManagePagePermissionCode As String = "EDIT"
        Private Const NavigatePagePermissionCode As String = "VIEW"
        Private Const ViewPagePermissionCode As String = "VIEW"

        Private dataProvider As DataProvider = Data.DataProvider.Instance()

#End Region

#Region "Shared/Static Methods"

        ' return the provider
        Public Shared Function Instance() As PermissionProvider
            Return ComponentFactory.GetComponent(Of PermissionProvider)()
        End Function

#End Region

#Region "Public Properties"

        Public Overridable ReadOnly Property LocalResourceFile() As String
            Get
                Return Services.Localization.Localization.GlobalResourceFile
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Function GetFolderPermissionsCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim PortalID As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Dim dr As IDataReader = dataProvider.GetFolderPermissionsByPortal(PortalID)
            Dim dic As New Dictionary(Of String, FolderPermissionCollection)
            Try
                Dim obj As FolderPermissionInfo
                While dr.Read
                    ' fill business object
                    obj = CBO.FillObject(Of FolderPermissionInfo)(dr, False)

                    Dim dictionaryKey As String = obj.FolderPath
                    If String.IsNullOrEmpty(dictionaryKey) Then
                        dictionaryKey = "[PortalRoot]"
                    End If

                    ' add Folder Permission to dictionary
                    If dic.ContainsKey(dictionaryKey) Then
                        'Add FolderPermission to FolderPermission Collection already in dictionary for FolderPath
                        dic(dictionaryKey).Add(obj)
                    Else
                        'Create new FolderPermission Collection for TabId
                        Dim collection As New FolderPermissionCollection

                        'Add Permission to Collection
                        collection.Add(obj)

                        'Add Collection to Dictionary
                        dic.Add(dictionaryKey, collection)
                    End If
                End While
            Catch exc As Exception
                LogException(exc)
            Finally
                ' close datareader
                CBO.CloseDataReader(dr, True)
            End Try
            Return dic
        End Function

        Private Function GetFolderPermissions(ByVal PortalID As Integer) As Dictionary(Of String, FolderPermissionCollection)
            Dim cacheKey As String = String.Format(DataCache.FolderPermissionCacheKey, PortalID.ToString())
            Return CBO.GetCachedObject(Of Dictionary(Of String, FolderPermissionCollection))(New CacheItemArgs(cacheKey, DataCache.FolderPermissionCacheTimeOut, DataCache.FolderPermissionCachePriority, PortalID), _
                                                                                                        AddressOf GetFolderPermissionsCallBack)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModulePermissions gets a Dictionary of ModulePermissionCollections by 
        ''' Module.
        ''' </summary>
        ''' <param name="tabID">The ID of the tab</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetModulePermissions(ByVal tabID As Integer) As Dictionary(Of Integer, ModulePermissionCollection)
            Dim cacheKey As String = String.Format(DataCache.ModulePermissionCacheKey, tabID.ToString())
            Return CBO.GetCachedObject(Of Dictionary(Of Integer, ModulePermissionCollection))(New CacheItemArgs(cacheKey, DataCache.ModulePermissionCacheTimeOut, DataCache.ModulePermissionCachePriority, tabID), _
                                                                                                        AddressOf GetModulePermissionsCallBack)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModulePermissionsCallBack gets a Dictionary of ModulePermissionCollections by 
        ''' Module from the the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetModulePermissionsCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim tabID As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Dim dr As IDataReader = dataProvider.GetModulePermissionsByTabID(tabID)
            Dim dic As New Dictionary(Of Integer, ModulePermissionCollection)
            Try
                Dim obj As ModulePermissionInfo
                While dr.Read
                    ' fill business object
                    obj = CBO.FillObject(Of ModulePermissionInfo)(dr, False)

                    ' add Module Permission to dictionary
                    If dic.ContainsKey(obj.ModuleID) Then
                        'Add ModulePermission to ModulePermission Collection already in dictionary for TabId
                        dic(obj.ModuleID).Add(obj)
                    Else
                        'Create new ModulePermission Collection for ModuleId
                        Dim collection As New ModulePermissionCollection

                        'Add Permission to Collection
                        collection.Add(obj)

                        'Add Collection to Dictionary
                        dic.Add(obj.ModuleID, collection)
                    End If
                End While
            Catch exc As Exception
                LogException(exc)
            Finally
                ' close datareader
                CBO.CloseDataReader(dr, True)
            End Try
            Return dic
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetTabPermissions gets a Dictionary of TabPermissionCollections by 
        ''' Tab.
        ''' </summary>
        ''' <param name="portalID">The ID of the portal</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetTabPermissions(ByVal portalID As Integer) As Dictionary(Of Integer, TabPermissionCollection)
            Dim cacheKey As String = String.Format(DataCache.TabPermissionCacheKey, portalID.ToString())
            Return CBO.GetCachedObject(Of Dictionary(Of Integer, TabPermissionCollection))(New CacheItemArgs(cacheKey, DataCache.TabPermissionCacheTimeOut, DataCache.TabPermissionCachePriority, portalID), _
                                                                                                     AddressOf GetTabPermissionsCallBack)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetTabPermissionsCallBack gets a Dictionary of TabPermissionCollections by 
        ''' Tab from the the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetTabPermissionsCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim portalID As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Dim dr As IDataReader = dataProvider.GetTabPermissionsByPortal(portalID)
            Dim dic As New Dictionary(Of Integer, TabPermissionCollection)
            Try
                Dim obj As TabPermissionInfo
                While dr.Read
                    ' fill business object
                    obj = CBO.FillObject(Of TabPermissionInfo)(dr, False)

                    ' add Tab Permission to dictionary
                    If dic.ContainsKey(obj.TabID) Then
                        'Add TabPermission to TabPermission Collection already in dictionary for TabId
                        dic(obj.TabID).Add(obj)
                    Else
                        'Create new TabPermission Collection for TabId
                        Dim collection As New TabPermissionCollection

                        'Add Permission to Collection
                        collection.Add(obj)

                        'Add Collection to Dictionary
                        dic.Add(obj.TabID, collection)
                    End If
                End While
            Catch exc As Exception
                LogException(exc)
            Finally
                ' close datareader
                CBO.CloseDataReader(dr, True)
            End Try
            Return dic
        End Function

#End Region

#Region "Public Methods"

#Region "FolderPermission Methods"

        Public Overridable Function CanAdminFolder(ByVal folder As FolderInfo) As Boolean
            Return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(AdminFolderPermissionCode))
        End Function

        Public Overridable Function CanAddFolder(ByVal folder As FolderInfo) As Boolean
            Return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(AddFolderPermissionCode))
        End Function

        Public Overridable Function CanCopyFolder(ByVal folder As FolderInfo) As Boolean
            Return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(CopyFolderPermissionCode))
        End Function

        Public Overridable Function CanDeleteFolder(ByVal folder As FolderInfo) As Boolean
            Return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(DeleteFolderPermissionCode))
        End Function

        Public Overridable Function CanManageFolder(ByVal folder As FolderInfo) As Boolean
            Return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(ManageFolderPermissionCode))
        End Function

        Public Overridable Function CanViewFolder(ByVal folder As FolderInfo) As Boolean
            Return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(ViewFolderPermissionCode))
        End Function

        Public Overridable Sub DeleteFolderPermissionsByUser(ByVal objUser As UserInfo)
            dataProvider.DeleteFolderPermissionsByUserID(objUser.PortalID, objUser.UserID)
        End Sub

        Public Overridable Function GetFolderPermissionsCollectionByFolder(ByVal PortalID As Integer, ByVal Folder As String) As FolderPermissionCollection
            Dim bFound As Boolean = False
            Dim dictionaryKey As String = Folder
            If String.IsNullOrEmpty(dictionaryKey) Then
                dictionaryKey = "[PortalRoot]"
            End If
            'Get the Portal FolderPermission Dictionary
            Dim dicFolderPermissions As Dictionary(Of String, FolderPermissionCollection) = GetFolderPermissions(PortalID)

            'Get the Collection from the Dictionary
            Dim folderPermissions As FolderPermissionCollection = Nothing
            bFound = dicFolderPermissions.TryGetValue(dictionaryKey, folderPermissions)

            If Not bFound Then
                'try the database
                folderPermissions = New FolderPermissionCollection(CBO.FillCollection(dataProvider.GetFolderPermissionsByFolderPath(PortalID, Folder, -1), GetType(FolderPermissionInfo)), Folder)
            End If

            Return folderPermissions
        End Function

        Public Overridable Function HasFolderPermission(ByVal objFolderPermissions As FolderPermissionCollection, ByVal PermissionKey As String) As Boolean
            Return PortalSecurity.IsInRoles(objFolderPermissions.ToString(PermissionKey))
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
        Public Overridable Sub SaveFolderPermissions(ByVal folder As FolderInfo)
            If Not folder.FolderPermissions Is Nothing Then
                Dim folderPermissions As FolderPermissionCollection = GetFolderPermissionsCollectionByFolder(folder.PortalID, folder.FolderPath)

                'Ensure that if role/user has been given a permission that is not Read/Browse then they also need Read/Browse
                Dim permController As New PermissionController()
                Dim permArray As ArrayList = permController.GetPermissionByCodeAndKey("SYSTEM_FOLDER", "READ")
                Dim readPerm As PermissionInfo = Nothing
                If permArray.Count = 1 Then
                    readPerm = TryCast(permArray(0), PermissionInfo)
                End If

                Dim browsePerm As PermissionInfo = Nothing
                permArray = permController.GetPermissionByCodeAndKey("SYSTEM_FOLDER", "BROWSE")
                If permArray.Count = 1 Then
                    browsePerm = TryCast(permArray(0), PermissionInfo)
                End If

                Dim additionalPermissions As New FolderPermissionCollection
                For Each folderPermission As FolderPermissionInfo In folder.FolderPermissions
                    If folderPermission.PermissionKey <> "BROWSE" AndAlso folderPermission.PermissionKey <> "READ" Then
                        'Try to add Read permission
                        Dim newFolderPerm As New FolderPermissionInfo(readPerm)
                        newFolderPerm.FolderID = folderPermission.FolderID
                        newFolderPerm.RoleID = folderPermission.RoleID
                        newFolderPerm.UserID = folderPermission.UserID
                        newFolderPerm.AllowAccess = folderPermission.AllowAccess

                        additionalPermissions.Add(newFolderPerm)

                        'Try to add Browse permission
                        newFolderPerm = New FolderPermissionInfo(browsePerm)
                        newFolderPerm.FolderID = folderPermission.FolderID
                        newFolderPerm.RoleID = folderPermission.RoleID
                        newFolderPerm.UserID = folderPermission.UserID
                        newFolderPerm.AllowAccess = folderPermission.AllowAccess

                        additionalPermissions.Add(newFolderPerm)
                    End If
                Next

                For Each folderPermission As FolderPermissionInfo In additionalPermissions
                    folder.FolderPermissions.Add(folderPermission, True)
                Next

                If Not folderPermissions.CompareTo(folder.FolderPermissions) Then
                    dataProvider.DeleteFolderPermissionsByFolderPath(folder.PortalID, folder.FolderPath)

                    For Each folderPermission As FolderPermissionInfo In folder.FolderPermissions
                        dataProvider.AddFolderPermission(folder.FolderID, folderPermission.PermissionID, _
                                                     folderPermission.RoleID, folderPermission.AllowAccess, _
                                                     folderPermission.UserID, UserController.GetCurrentUserInfo.UserID)
                    Next
                End If
            End If
        End Sub

#End Region

#Region "ModulePermission Methods"

        Public Overridable Function CanAdminModule(ByVal objModule As ModuleInfo) As Boolean
            Return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(AdminModulePermissionCode))
        End Function

        Public Overridable Function CanDeleteModule(ByVal objModule As ModuleInfo) As Boolean
            Return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(DeleteModulePermissionCode))
        End Function

        Public Overridable Function CanEditModuleContent(ByVal objModule As ModuleInfo) As Boolean
            Return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ContentModulePermissionCode))
        End Function

        Public Overridable Function CanExportModule(ByVal objModule As ModuleInfo) As Boolean
            Return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ExportModulePermissionCode))
        End Function

        Public Overridable Function CanImportModule(ByVal objModule As ModuleInfo) As Boolean
            Return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ImportModulePermissionCode))
        End Function

        Public Overridable Function CanManageModule(ByVal objModule As ModuleInfo) As Boolean
            Return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ManageModulePermissionCode))
        End Function

        Public Overridable Function CanViewModule(ByVal objModule As ModuleInfo) As Boolean
            Return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ViewModulePermissionCode))
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
        Public Overridable Sub DeleteModulePermissionsByUser(ByVal objUser As UserInfo)
            dataProvider.DeleteModulePermissionsByUserID(objUser.PortalID, objUser.UserID)
            DataCache.ClearModulePermissionsCachesByPortal(objUser.PortalID)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModulePermissions gets a ModulePermissionCollection
        ''' </summary>
        ''' <param name="moduleID">The ID of the module</param>
        ''' <param name="tabID">The ID of the tab</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Function GetModulePermissions(ByVal moduleID As Integer, ByVal tabID As Integer) As ModulePermissionCollection
            Dim bFound As Boolean = False

            'Get the Tab ModulePermission Dictionary
            Dim dicModulePermissions As Dictionary(Of Integer, ModulePermissionCollection) = GetModulePermissions(tabID)

            'Get the Collection from the Dictionary
            Dim modulePermissions As ModulePermissionCollection = Nothing
            bFound = dicModulePermissions.TryGetValue(moduleID, modulePermissions)

            If Not bFound Then
                'try the database
                modulePermissions = New ModulePermissionCollection(CBO.FillCollection(dataProvider.GetModulePermissionsByModuleID(moduleID, -1), GetType(ModulePermissionInfo)), moduleID)
            End If

            Return modulePermissions
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HasModulePermission checks whether the current user has a specific Module Permission
        ''' </summary>
        ''' <param name="objModulePermissions">The Permissions for the Module</param>
        ''' <param name="permissionKey">The Permission to check</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Function HasModulePermission(ByVal objModulePermissions As ModulePermissionCollection, ByVal permissionKey As String) As Boolean
            Return PortalSecurity.IsInRoles(objModulePermissions.ToString(permissionKey))
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
        Public Overridable Sub SaveModulePermissions(ByVal objModule As ModuleInfo)
            If Not objModule.ModulePermissions Is Nothing Then
                Dim modulePermissions As ModulePermissionCollection = ModulePermissionController.GetModulePermissions(objModule.ModuleID, objModule.TabID)

                If Not modulePermissions.CompareTo(objModule.ModulePermissions) Then
                    dataProvider.DeleteModulePermissionsByModuleID(objModule.ModuleID)

                    For Each objModulePermission As ModulePermissionInfo In objModule.ModulePermissions
                        If objModule.InheritViewPermissions And objModulePermission.PermissionKey = "VIEW" Then
                            dataProvider.DeleteModulePermission(objModulePermission.ModulePermissionID)
                        Else
                            dataProvider.AddModulePermission(objModule.ModuleID, objModulePermission.PermissionID, _
                                                         objModulePermission.RoleID, objModulePermission.AllowAccess, _
                                                         objModulePermission.UserID, UserController.GetCurrentUserInfo.UserID)
                        End If
                    Next
                End If
            End If
        End Sub

#End Region

#Region "TabPermission Methods"

        Public Overridable Function CanAddContentToPage(ByVal objTab As TabInfo) As Boolean
            Return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ContentPagePermissionCode))
        End Function

        Public Overridable Function CanAddPage(ByVal objTab As TabInfo) As Boolean
            Return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(AddPagePermissionCode))
        End Function

        Public Overridable Function CanAdminPage(ByVal objTab As TabInfo) As Boolean
            Return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(AdminPagePermissionCode))
        End Function

        Public Overridable Function CanCopyPage(ByVal objTab As TabInfo) As Boolean
            Return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(CopyPagePermissionCode))
        End Function

        Public Overridable Function CanDeletePage(ByVal objTab As TabInfo) As Boolean
            Return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(DeletePagePermissionCode))
        End Function

        Public Overridable Function CanExportPage(ByVal objTab As TabInfo) As Boolean
            Return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ExportPagePermissionCode))
        End Function

        Public Overridable Function CanImportPage(ByVal objTab As TabInfo) As Boolean
            Return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ImportPagePermissionCode))
        End Function

        Public Overridable Function CanManagePage(ByVal objTab As TabInfo) As Boolean
            Return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ManagePagePermissionCode))
        End Function

        Public Overridable Function CanNavigateToPage(ByVal objTab As TabInfo) As Boolean
            Return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(NavigatePagePermissionCode))
        End Function

        Public Overridable Function CanViewPage(ByVal objTab As TabInfo) As Boolean
            Return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ViewPagePermissionCode))
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
        Public Overridable Sub DeleteTabPermissionsByUser(ByVal objUser As UserInfo)
            dataProvider.DeleteTabPermissionsByUserID(objUser.PortalID, objUser.UserID)
            DataCache.ClearTabPermissionsCache(objUser.PortalID)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetTabPermissions gets a TabPermissionCollection
        ''' </summary>
        ''' <param name="tabID">The ID of the tab</param>
        ''' <param name="portalID">The ID of the portal</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Function GetTabPermissions(ByVal tabID As Integer, ByVal portalID As Integer) As TabPermissionCollection
            Dim bFound As Boolean = False

            'Get the Portal TabPermission Dictionary
            Dim dicTabPermissions As Dictionary(Of Integer, TabPermissionCollection) = GetTabPermissions(portalID)

            'Get the Collection from the Dictionary
            Dim tabPermissions As TabPermissionCollection = Nothing
            bFound = dicTabPermissions.TryGetValue(tabID, tabPermissions)

            If Not bFound Then
                'try the database
                tabPermissions = New TabPermissionCollection(CBO.FillCollection(dataProvider.GetTabPermissionsByTabID(tabID, -1), GetType(TabPermissionInfo)), tabID)
            End If

            Return tabPermissions
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HasTabPermission checks whether the current user has a specific Tab Permission
        ''' </summary>
        ''' <param name="objTabPermissions">The Permissions for the Tab</param>
        ''' <param name="permissionKey">The Permission to check</param>
        ''' <history>
        ''' 	[cnurse]	04/15/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Function HasTabPermission(ByVal objTabPermissions As Security.Permissions.TabPermissionCollection, ByVal permissionKey As String) As Boolean
            Return PortalSecurity.IsInRoles(objTabPermissions.ToString(permissionKey))
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
        Public Overridable Sub SaveTabPermissions(ByVal objTab As TabInfo)
            Dim objCurrentTabPermissions As TabPermissionCollection = GetTabPermissions(objTab.TabID, objTab.PortalID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController

            If Not objCurrentTabPermissions.CompareTo(objTab.TabPermissions) Then
                dataProvider.DeleteTabPermissionsByTabID(objTab.TabID)
                objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.TABPERMISSION_DELETED)
                If Not objTab.TabPermissions Is Nothing Then
                    For Each objTabPermission As TabPermissionInfo In objTab.TabPermissions
                        dataProvider.AddTabPermission(objTab.TabID, objTabPermission.PermissionID, _
                                                  objTabPermission.RoleID, objTabPermission.AllowAccess, _
                                                  objTabPermission.UserID, UserController.GetCurrentUserInfo.UserID)
                        objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.TABPERMISSION_CREATED)
                    Next
                End If
            End If
        End Sub

#End Region

#End Region

    End Class

End Namespace

