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
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Entities.Modules.Definitions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Entities.Modules.Definitions
    ''' Class	 : ModuleDefinitionController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModuleDefinitionController provides the Business Layer for Module Definitions
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/11/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ModuleDefinitionController

#Region "Private Members"

        Private Shared key As String = "ModuleDefID"
        Private Shared dataProvider As DataProvider = dataProvider.Instance()

#End Region

#Region "Private Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleDefinitionsCallBack gets a Dictionary of Module Definitions from 
        ''' the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	01/13/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetModuleDefinitionsCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return CBO.FillDictionary(Of Integer, ModuleDefinitionInfo)(key, dataProvider.GetModuleDefinitions(), New Dictionary(Of Integer, ModuleDefinitionInfo))
        End Function

#End Region

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleDefinitionByID gets a Module Definition by its ID
        ''' </summary>
        ''' <param name="moduleDefID">The ID of the Module Definition</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModuleDefinitionByID(ByVal moduleDefID As Integer) As ModuleDefinitionInfo
            Dim moduleDefinition As ModuleDefinitionInfo = Nothing

            'Try Cached Dictionary first
            Dim moduleDefinitions As Dictionary(Of Integer, ModuleDefinitionInfo) = GetModuleDefinitions()
            If Not moduleDefinitions.TryGetValue(moduleDefID, moduleDefinition) Then
                'Not in Cached Dictionary so get DesktopModule from Data Base
                moduleDefinition = CBO.FillObject(Of ModuleDefinitionInfo)(dataProvider.GetModuleDefinition(moduleDefID))
            End If

            Return moduleDefinition
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleDefinitionByFriendlyName gets a Module Definition by its Friendly
        ''' Name
        ''' </summary>
        ''' <param name="friendlyName">The friendly name</param>
        ''' <history>
        ''' 	[cnurse]	10/30/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModuleDefinitionByFriendlyName(ByVal friendlyName As String) As ModuleDefinitionInfo
            Dim moduleDefinition As ModuleDefinitionInfo = Nothing

            'Iterate through cached Dictionary to get all Module Definitions with the correct DesktopModuleID
            For Each kvp As KeyValuePair(Of Integer, ModuleDefinitionInfo) In GetModuleDefinitions()
                If kvp.Value.FriendlyName = friendlyName Then
                    moduleDefinition = kvp.Value
                    Exit For
                End If
            Next

            Return moduleDefinition
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleDefinitionByFriendlyName gets a Module Definition by its Friendly
        ''' Name (and DesktopModuleID)
        ''' </summary>
        ''' <param name="friendlyName">The friendly name</param>
        ''' <param name="desktopModuleID">The ID of the Dekstop Module</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModuleDefinitionByFriendlyName(ByVal friendlyName As String, ByVal desktopModuleID As Integer) As ModuleDefinitionInfo
            Dim moduleDefinition As ModuleDefinitionInfo = Nothing

            'Try Cached Dictionary first
            Dim moduleDefinitions As Dictionary(Of String, ModuleDefinitionInfo) = GetModuleDefinitionsByDesktopModuleID(desktopModuleID)
            If Not moduleDefinitions.TryGetValue(friendlyName, moduleDefinition) Then
                'Not in Cached Dictionary so get DesktopModule from Data Base
                moduleDefinition = CBO.FillObject(Of ModuleDefinitionInfo)(dataProvider.GetModuleDefinitionByName(desktopModuleID, friendlyName))
            End If

            Return moduleDefinition
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleDefinitions gets a Dictionary of Module Definitions.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModuleDefinitions() As Dictionary(Of Integer, ModuleDefinitionInfo)
            Return CBO.GetCachedObject(Of Dictionary(Of Integer, ModuleDefinitionInfo))(New CacheItemArgs(DataCache.ModuleDefinitionCacheKey, DataCache.ModuleDefinitionCachePriority), _
                                                                                                  AddressOf GetModuleDefinitionsCallBack)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleDefinitionsByDesktopModuleID gets a Dictionary of Module Definitions
        ''' with a particular DesktopModuleID, keyed by the FriendlyName.
        ''' </summary>
        ''' <param name="desktopModuleID">The ID of the Desktop Module</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModuleDefinitionsByDesktopModuleID(ByVal desktopModuleID As Integer) As Dictionary(Of String, ModuleDefinitionInfo)
            Dim moduleDefinitions As New Dictionary(Of String, ModuleDefinitionInfo)

            'Iterate through cached Dictionary to get all Module Definitions with the correct DesktopModuleID
            For Each kvp As KeyValuePair(Of Integer, ModuleDefinitionInfo) In GetModuleDefinitions()
                If kvp.Value.DesktopModuleID = desktopModuleID Then
                    moduleDefinitions.Add(kvp.Value.FriendlyName, kvp.Value)
                End If
            Next

            Return moduleDefinitions
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SaveModuleDefinition saves the Module Definition to the database
        ''' </summary>
        ''' <param name="moduleDefinition">The Module Definition to save</param>
        ''' <param name="saveChildren">A flag that determines whether the child objects are also saved</param>
        ''' <param name="clearCache">A flag that determines whether to clear the host cache</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function SaveModuleDefinition(ByVal moduleDefinition As ModuleDefinitionInfo, ByVal saveChildren As Boolean, ByVal clearCache As Boolean) As Integer
            Dim moduleDefinitionID As Integer = moduleDefinition.ModuleDefID

            If moduleDefinitionID = Null.NullInteger Then
                'Add new Module Definition
                moduleDefinitionID = dataProvider.AddModuleDefinition(moduleDefinition.DesktopModuleID, moduleDefinition.FriendlyName, moduleDefinition.DefaultCacheTime, UserController.GetCurrentUserInfo.UserID)
            Else
                'Upgrade Module Definition
                dataProvider.UpdateModuleDefinition(moduleDefinition.ModuleDefID, moduleDefinition.FriendlyName, moduleDefinition.DefaultCacheTime, UserController.GetCurrentUserInfo.UserID)
            End If

            If saveChildren Then
                For Each kvp As KeyValuePair(Of String, PermissionInfo) In moduleDefinition.Permissions
                    kvp.Value.ModuleDefID = moduleDefinitionID

                    'check if permission exists
                    Dim permissionController As New PermissionController()
                    Dim permissions As ArrayList = permissionController.GetPermissionByCodeAndKey(kvp.Value.PermissionCode, kvp.Value.PermissionKey)

                    If permissions IsNot Nothing AndAlso permissions.Count = 1 Then
                        Dim permission As PermissionInfo = CType(permissions(0), PermissionInfo)
                        kvp.Value.PermissionID = permission.PermissionID
                        permissionController.UpdatePermission(kvp.Value)
                    Else
                        permissionController.AddPermission(kvp.Value)
                    End If
                Next

                For Each kvp As KeyValuePair(Of String, ModuleControlInfo) In moduleDefinition.ModuleControls
                    kvp.Value.ModuleDefID = moduleDefinitionID

                    ' check if definition exists
                    Dim moduleControl As ModuleControlInfo = ModuleControlController.GetModuleControlByControlKey(kvp.Value.ControlKey, kvp.Value.ModuleDefID)

                    If moduleControl IsNot Nothing Then
                        kvp.Value.ModuleControlID = moduleControl.ModuleControlID
                    End If

                    ModuleControlController.SaveModuleControl(kvp.Value, clearCache)
                Next
            End If

            If clearCache Then
                DataCache.ClearHostCache(True)
            End If

            Return moduleDefinitionID
        End Function

#End Region

#Region "Public Instance Methods"

        Public Function AddModuleDefinition(ByVal objModuleDefinition As ModuleDefinitionInfo) As Integer
            Return SaveModuleDefinition(objModuleDefinition, False, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteModuleDefinition deletes a Module Definition By ID
        ''' </summary>
        ''' <param name="objModuleDefinition">The Module Definition to delete</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteModuleDefinition(ByVal objModuleDefinition As ModuleDefinitionInfo)
            DeleteModuleDefinition(objModuleDefinition.ModuleDefID)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteModuleDefinition deletes a Module Definition By ID
        ''' </summary>
        ''' <param name="moduleDefinitionId">The ID of the Module Definition to delete</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteModuleDefinition(ByVal moduleDefinitionId As Integer)
            'Delete associated permissions
            Dim permissionController As New PermissionController
            For Each permission As PermissionInfo In permissionController.GetPermissionsByModuleDefID(moduleDefinitionId)
                permissionController.DeletePermission(permission.PermissionID)
            Next
            dataProvider.DeleteModuleDefinition(moduleDefinitionId)
            DataCache.ClearHostCache(True)
        End Sub

        Public Sub UpdateModuleDefinition(ByVal objModuleDefinition As ModuleDefinitionInfo)
            SaveModuleDefinition(objModuleDefinition, False, True)
        End Sub


#End Region

#Region "Obsolete"


        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetModuleDefinitionByID(Integer)")> _
        Public Function GetModuleDefinition(ByVal moduleDefID As Integer) As ModuleDefinitionInfo
            Return GetModuleDefinitionByID(moduleDefID)
        End Function

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetModuleDefinitionByFriendlyName(String,Integer)")> _
        Public Function GetModuleDefinitionByName(ByVal desktopModuleID As Integer, ByVal friendlyName As String) As ModuleDefinitionInfo
            Return GetModuleDefinitionByFriendlyName(friendlyName, desktopModuleID)
        End Function

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetModuleDefinitionsByDesktopModuleID(Integer)")> _
        Public Function GetModuleDefinitions(ByVal DesktopModuleId As Integer) As ArrayList
            Dim arrDefinitions As New ArrayList()
            arrDefinitions.AddRange(GetModuleDefinitionsByDesktopModuleID(DesktopModuleId).Values())
            Return arrDefinitions
        End Function

#End Region

    End Class

End Namespace

