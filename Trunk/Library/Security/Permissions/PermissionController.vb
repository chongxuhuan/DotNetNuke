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
Imports System.Text
Imports System.Collections.Generic

Namespace DotNetNuke.Security.Permissions

    Public Class PermissionController

#Region "Public Methods"

        Private Shared provider As DataProvider = DataProvider.Instance()

        Public Function AddPermission(ByVal objPermission As PermissionInfo) As Integer
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objPermission, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.PERMISSION_CREATED)
            Return CType(provider.AddPermission(objPermission.PermissionCode, objPermission.ModuleDefID, objPermission.PermissionKey, objPermission.PermissionName, UserController.GetCurrentUserInfo.UserID), Integer)
        End Function

        Public Sub DeletePermission(ByVal permissionID As Integer)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("PermissionID", permissionID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PERMISSION_DELETED)
            provider.DeletePermission(permissionID)
        End Sub

        Public Function GetPermission(ByVal permissionID As Integer) As PermissionInfo
            Return CBO.FillObject(Of PermissionInfo)(provider.GetPermission(permissionID))
        End Function

        Public Function GetPermissionByCodeAndKey(ByVal permissionCode As String, ByVal permissionKey As String) As ArrayList
            Return CBO.FillCollection(provider.GetPermissionByCodeAndKey(permissionCode, permissionKey), GetType(PermissionInfo))
        End Function

        Public Function GetPermissionsByModuleDefID(ByVal moduleDefID As Integer) As ArrayList
            Return CBO.FillCollection(provider.GetPermissionsByModuleDefID(moduleDefID), GetType(PermissionInfo))
        End Function


        Public Function GetPermissionsByModuleID(ByVal moduleID As Integer) As ArrayList
            Return CBO.FillCollection(provider.GetPermissionsByModuleID(moduleID), GetType(PermissionInfo))
        End Function

        Public Sub UpdatePermission(ByVal objPermission As PermissionInfo)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objPermission, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.PERMISSION_UPDATED)
            provider.UpdatePermission(objPermission.PermissionID, objPermission.PermissionCode, objPermission.ModuleDefID, objPermission.PermissionKey, objPermission.PermissionName, UserController.GetCurrentUserInfo.UserID)
        End Sub

#End Region

#Region "Shared Methods"

        Public Shared Function BuildPermissions(ByVal Permissions As IList, ByVal PermissionKey As String) As String
            Dim strPrefix As String = ""
            Dim strPermission As String = ""
            Dim objPermissions As New StringBuilder()

            For Each permission As PermissionInfoBase In Permissions
                If permission.PermissionKey = PermissionKey Then
                    ' Deny permissions are prefixed with a "!"
                    If Not permission.AllowAccess Then
                        strPrefix = "!"
                    Else
                        strPrefix = ""
                    End If

                    ' encode permission
                    If Null.IsNull(permission.UserID) Then
                        strPermission = strPrefix + permission.RoleName + ";"
                    Else
                        strPermission = strPrefix + "[" + permission.UserID.ToString + "];"
                    End If

                    ' build permissions string ensuring that Deny permissions are inserted at the beginning and Grant permissions at the end
                    If strPrefix = "!" Then
                        objPermissions.Insert(0, strPermission)
                    Else
                        objPermissions.Append(strPermission)
                    End If
                End If
            Next

            ' get string
            Dim strPermissions As String = objPermissions.ToString()

            ' ensure leading delimiter
            If Not strPermissions.StartsWith(";") Then
                strPermissions.Insert(0, ";")
            End If

            Return strPermissions
        End Function

        Public Shared Function GetPermissionsByFolder() As ArrayList
            Return CBO.FillCollection(provider.GetPermissionsByFolder(), GetType(PermissionInfo))
        End Function

        Public Shared Function GetPermissionsByPortalDesktopModule() As ArrayList
            Return CBO.FillCollection(provider.GetPermissionsByPortalDesktopModule(), GetType(PermissionInfo))
        End Function

        Public Shared Function GetPermissionsByTab() As ArrayList
            Return CBO.FillCollection(provider.GetPermissionsByTab(), GetType(PermissionInfo))
        End Function

#End Region

        <Obsolete("Deprecated in DNN 5.0.1. Replaced by GetPermissionsByFolder()")> _
        Public Function GetPermissionsByFolder(ByVal portalID As Integer, ByVal folder As String) As ArrayList
            Return CBO.FillCollection(provider.GetPermissionsByFolder(), GetType(PermissionInfo))
        End Function
        <Obsolete("Deprecated in DNN 5.0.1. Replaced by GetPermissionsByTab()")> _
        Public Function GetPermissionsByTabID(ByVal tabID As Integer) As ArrayList
            Return CBO.FillCollection(provider.GetPermissionsByTab(), GetType(PermissionInfo))
        End Function
        <Obsolete("Deprecated in DNN 5.0.1. Replaced by GetPermissionsByPortalDesktopModule()")> _
        Public Function GetPermissionsByPortalDesktopModuleID() As ArrayList
            Return CBO.FillCollection(provider.GetPermissionsByPortalDesktopModule(), GetType(PermissionInfo))
        End Function


    End Class


End Namespace
