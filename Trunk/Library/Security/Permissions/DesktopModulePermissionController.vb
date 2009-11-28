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

Namespace DotNetNuke.Security.Permissions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Security.Permissions
    ''' Class	 : DesktopModulePermissionController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' DesktopModulePermissionController provides the Business Layer for DesktopModule Permissions
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/15/2008   Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> _
    Public Class DesktopModulePermissionController

#Region "Private Members"

        Private Shared provider As DataProvider = DataProvider.Instance()

#End Region

#Region "Private Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ClearPermissionCache clears the DesktopModule Permission Cache
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub ClearPermissionCache()
            DataCache.ClearDesktopModulePermissionsCache()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillDesktopModulePermissionDictionary fills a Dictionary of DesktopModulePermissions from a
        ''' dataReader
        ''' </summary>
        ''' <param name="dr">The IDataReader</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function FillDesktopModulePermissionDictionary(ByVal dr As IDataReader) As Dictionary(Of Integer, DesktopModulePermissionCollection)
            Dim dic As New Dictionary(Of Integer, DesktopModulePermissionCollection)
            Try
                Dim obj As DesktopModulePermissionInfo
                While dr.Read
                    ' fill business object
                    obj = CBO.FillObject(Of DesktopModulePermissionInfo)(dr, False)

                    ' add DesktopModule Permission to dictionary
                    If dic.ContainsKey(obj.PortalDesktopModuleID) Then
                        'Add DesktopModulePermission to DesktopModulePermission Collection already in dictionary for TabId
                        dic(obj.PortalDesktopModuleID).Add(obj)
                    Else
                        'Create new DesktopModulePermission Collection for DesktopModulePermissionID
                        Dim collection As New DesktopModulePermissionCollection

                        'Add Permission to Collection
                        collection.Add(obj)

                        'Add Collection to Dictionary
                        dic.Add(obj.PortalDesktopModuleID, collection)
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
        ''' GetDesktopModulePermissions gets a Dictionary of DesktopModulePermissionCollections by 
        ''' DesktopModule.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetDesktopModulePermissions() As Dictionary(Of Integer, DesktopModulePermissionCollection)
            Return CBO.GetCachedObject(Of Dictionary(Of Integer, DesktopModulePermissionCollection))(New CacheItemArgs(DataCache.DesktopModulePermissionCacheKey, DataCache.DesktopModulePermissionCachePriority), _
                                                                                                        AddressOf GetDesktopModulePermissionsCallBack)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetDesktopModulePermissionsCallBack gets a Dictionary of DesktopModulePermissionCollections by 
        ''' DesktopModule from the the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetDesktopModulePermissionsCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return FillDesktopModulePermissionDictionary(provider.GetDesktopModulePermissions())
        End Function

#End Region

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddDesktopModulePermission adds a DesktopModule Permission to the Database
        ''' </summary>
        ''' <param name="objDesktopModulePermission">The DesktopModule Permission to add</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function AddDesktopModulePermission(ByVal objDesktopModulePermission As DesktopModulePermissionInfo) As Integer
            Dim Id As Integer = provider.AddDesktopModulePermission(objDesktopModulePermission.PortalDesktopModuleID, objDesktopModulePermission.PermissionID, objDesktopModulePermission.RoleID, _
                                                              objDesktopModulePermission.AllowAccess, objDesktopModulePermission.UserID, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objDesktopModulePermission, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.DESKTOPMODULEPERMISSION_CREATED)
            ClearPermissionCache()
            Return Id
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteDesktopModulePermission deletes a DesktopModule Permission in the Database
        ''' </summary>
        ''' <param name="DesktopModulePermissionID">The ID of the DesktopModule Permission to delete</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteDesktopModulePermission(ByVal DesktopModulePermissionID As Integer)
            provider.DeleteDesktopModulePermission(DesktopModulePermissionID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("DesktopModulePermissionID", DesktopModulePermissionID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.DESKTOPMODULEPERMISSION_DELETED)
            ClearPermissionCache()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteDesktopModulePermissionsByPortalDesktopModuleID deletes a DesktopModule's 
        ''' DesktopModule Permission in the Database
        ''' </summary>
        ''' <param name="portalDesktopModuleID">The ID of the DesktopModule to delete</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteDesktopModulePermissionsByPortalDesktopModuleID(ByVal portalDesktopModuleID As Integer)
            provider.DeleteDesktopModulePermissionsByPortalDesktopModuleID(portalDesktopModuleID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("PortalDesktopModuleID", portalDesktopModuleID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_DELETED)
            ClearPermissionCache()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteDesktopModulePermissionsByUserID deletes a user's DesktopModule Permission in the Database
        ''' </summary>
        ''' <param name="objUser">The user</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteDesktopModulePermissionsByUserID(ByVal objUser As UserInfo)
            provider.DeleteDesktopModulePermissionsByUserID(objUser.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("UserID", objUser.UserID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_DELETED)
            ClearPermissionCache()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetDesktopModulePermission gets a DesktopModule Permission from the Database
        ''' </summary>
        ''' <param name="DesktopModulePermissionID">The ID of the DesktopModule Permission</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetDesktopModulePermission(ByVal DesktopModulePermissionID As Integer) As DesktopModulePermissionInfo
            Return CBO.FillObject(Of DesktopModulePermissionInfo)(provider.GetDesktopModulePermission(DesktopModulePermissionID), True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetDesktopModulePermissions gets a DesktopModulePermissionCollection
        ''' </summary>
        ''' <param name="portalDesktopModuleID">The ID of the DesktopModule</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetDesktopModulePermissions(ByVal portalDesktopModuleID As Integer) As DesktopModulePermissionCollection
            Dim bFound As Boolean = False

            'Get the Tab DesktopModulePermission Dictionary
            Dim dicDesktopModulePermissions As Dictionary(Of Integer, DesktopModulePermissionCollection) = GetDesktopModulePermissions()

            'Get the Collection from the Dictionary
            Dim DesktopModulePermissions As DesktopModulePermissionCollection = Nothing
            bFound = dicDesktopModulePermissions.TryGetValue(portalDesktopModuleID, DesktopModulePermissions)

            If Not bFound Then
                'try the database
                DesktopModulePermissions = New DesktopModulePermissionCollection(CBO.FillCollection(provider.GetDesktopModulePermissionsByPortalDesktopModuleID(portalDesktopModuleID), GetType(DesktopModulePermissionInfo)), portalDesktopModuleID)
            End If

            Return DesktopModulePermissions
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HasDesktopModulePermission checks whether the current user has a specific DesktopModule Permission
        ''' </summary>
        ''' <param name="objDesktopModulePermissions">The Permissions for the DesktopModule</param>
        ''' <param name="permissionKey">The Permission to check</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function HasDesktopModulePermission(ByVal objDesktopModulePermissions As DesktopModulePermissionCollection, ByVal permissionKey As String) As Boolean
            Return PortalSecurity.IsInRoles(objDesktopModulePermissions.ToString(permissionKey))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateDesktopModulePermission updates a DesktopModule Permission in the Database
        ''' </summary>
        ''' <param name="objDesktopModulePermission">The DesktopModule Permission to update</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub UpdateDesktopModulePermission(ByVal objDesktopModulePermission As DesktopModulePermissionInfo)
            provider.UpdateDesktopModulePermission(objDesktopModulePermission.DesktopModulePermissionID, objDesktopModulePermission.PortalDesktopModuleID, objDesktopModulePermission.PermissionID, _
                                             objDesktopModulePermission.RoleID, objDesktopModulePermission.AllowAccess, objDesktopModulePermission.UserID, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objDesktopModulePermission, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.DESKTOPMODULEPERMISSION_UPDATED)
            ClearPermissionCache()
        End Sub

#End Region

    End Class



End Namespace
