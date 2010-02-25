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
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Exceptions

Namespace DotNetNuke.Security.Permissions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Security.Permissions
    ''' Class	 : WorkflowStatePermissionController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' WorkflowStatePermissionController provides the Business Layer for DesktopModule Permissions
    ''' </summary>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class WorkflowStatePermissionController

#Region "Private Members"

        Private Shared provider As DotNetNuke.Modules.HTML.DataProvider = DotNetNuke.Modules.HTML.DataProvider.Instance()

        Public Const WorkflowStatePermissionCacheKey As String = "WorkflowStatePermissions"
        Public Const WorkflowStatePermissionCachePriority As CacheItemPriority = CacheItemPriority.Normal
        Public Const WorkflowStatePermissionCacheTimeOut As Integer = 20

#End Region

#Region "Private Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetWorkflowStatePermissions gets a Dictionary of WorkflowStatePermissionCollections by 
        ''' WorkflowState.
        ''' </summary>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetWorkflowStatePermissions() As Dictionary(Of Integer, WorkflowStatePermissionCollection)
            Return CBO.GetCachedObject(Of Dictionary(Of Integer, WorkflowStatePermissionCollection))(New CacheItemArgs(WorkflowStatePermissionCacheKey, WorkflowStatePermissionCachePriority), _
                                                                                                        AddressOf GetWorkflowStatePermissionsCallBack)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetWorkflowStatePermissionsCallBack gets a Dictionary of WorkflowStatePermissionCollections by 
        ''' WorkflowState from the the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters needed for the database call</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetWorkflowStatePermissionsCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return FillWorkflowStatePermissionDictionary(provider.GetWorkflowStatePermissions())
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillWorkflowStatePermissionDictionary fills a Dictionary of WorkflowStatePermissions from a
        ''' dataReader
        ''' </summary>
        ''' <param name="dr">The IDataReader</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function FillWorkflowStatePermissionDictionary(ByVal dr As IDataReader) As Dictionary(Of Integer, WorkflowStatePermissionCollection)
            Dim dic As New Dictionary(Of Integer, WorkflowStatePermissionCollection)
            Try
                Dim obj As WorkflowStatePermissionInfo
                While dr.Read
                    ' fill business object
                    obj = CBO.FillObject(Of WorkflowStatePermissionInfo)(dr, False)

                    ' add WorkflowState Permission to dictionary
                    If dic.ContainsKey(obj.StateID) Then
                        'Add WorkflowStatePermission to WorkflowStatePermission Collection already in dictionary for StateId
                        dic(obj.StateID).Add(obj)
                    Else
                        'Create new WorkflowStatePermission Collection for WorkflowStatePermissionID
                        Dim collection As New WorkflowStatePermissionCollection

                        'Add Permission to Collection
                        collection.Add(obj)

                        'Add Collection to Dictionary
                        dic.Add(obj.StateID, collection)
                    End If
                End While
            Catch exc As Exception
                LogException(exc)
            Finally
                ' close datareader
                If Not dr Is Nothing Then
                    dr.Close()
                End If
            End Try
            Return dic
        End Function

#End Region

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetWorkflowStatePermissions gets a WorkflowStatePermissionCollection
        ''' </summary>
        ''' <param name="StateID">The ID of the State</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetWorkflowStatePermissions(ByVal StateID As Integer) As WorkflowStatePermissionCollection
            Dim bFound As Boolean = False

            'Get the WorkflowStatePermission Dictionary
            Dim dicWorkflowStatePermissions As Dictionary(Of Integer, WorkflowStatePermissionCollection) = GetWorkflowStatePermissions()

            'Get the Collection from the Dictionary
            Dim WorkflowStatePermissions As WorkflowStatePermissionCollection = Nothing
            bFound = dicWorkflowStatePermissions.TryGetValue(StateID, WorkflowStatePermissions)

            If Not bFound Then
                'try the database
                WorkflowStatePermissions = New WorkflowStatePermissionCollection(CBO.FillCollection(provider.GetWorkflowStatePermissionsByStateID(StateID), GetType(WorkflowStatePermissionInfo)), StateID)
            End If

            Return WorkflowStatePermissions
        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HasWorkflowStatePermission checks whether the current user has a specific WorkflowState Permission
        ''' </summary>
        ''' <param name="objWorkflowStatePermissions">The Permissions for the WorkflowState</param>
        ''' <param name="permissionKey">The Permission to check</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function HasWorkflowStatePermission(ByVal objWorkflowStatePermissions As WorkflowStatePermissionCollection, ByVal permissionKey As String) As Boolean
            Return PortalSecurity.IsInRoles(objWorkflowStatePermissions.ToString(permissionKey))
        End Function

#End Region

    End Class



End Namespace
