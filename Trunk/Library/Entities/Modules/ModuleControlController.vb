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
Imports System.Data
Imports System.Collections.Generic

Namespace DotNetNuke.Entities.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Entities.Modules
    ''' Class	 : ModuleControlController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModuleControlController provides the Business Layer for Module Controls
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ModuleControlController

#Region "Private Members"

        Private Shared key As String = "ModuleControlID"
        Private Shared dataProvider As DataProvider = DataProvider.Instance()

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleControls gets a Dictionary of Module Controls from 
        ''' the Cache.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetModuleControls() As Dictionary(Of Integer, ModuleControlInfo)
            Return CBO.GetCachedObject(Of Dictionary(Of Integer, ModuleControlInfo))(New CacheItemArgs(DataCache.ModuleControlsCacheKey, DataCache.ModuleControlsCacheTimeOut, DataCache.ModuleControlsCachePriority), _
                                                                                               AddressOf GetModuleControlsCallBack)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleControlsCallBack gets a Dictionary of Module Controls from 
        ''' the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetModuleControlsCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return CBO.FillDictionary(Of Integer, ModuleControlInfo)(key, dataProvider.GetModuleControls(), New Dictionary(Of Integer, ModuleControlInfo))
        End Function

#End Region

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleControl adds a new Module Control to the database
        ''' </summary>
        ''' <param name="objModuleControl">The Module Control to save</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddModuleControl(ByVal objModuleControl As ModuleControlInfo)
            SaveModuleControl(objModuleControl, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteModuleControl deletes a Module Control in the database
        ''' </summary>
        ''' <param name="moduleControlID">The ID of the Module Control to delete</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteModuleControl(ByVal moduleControlID As Integer)
            dataProvider.DeleteModuleControl(moduleControlID)
            DataCache.ClearHostCache(True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleControl gets a single Module Control from the database
        ''' </summary>
        ''' <param name="moduleControlID">The ID of the Module Control to fetch</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModuleControl(ByVal moduleControlID As Integer) As ModuleControlInfo
            Dim moduleControl As ModuleControlInfo = Nothing

            'Try Cached Dictionary first
            Dim moduleControls As Dictionary(Of Integer, ModuleControlInfo) = GetModuleControls()
            If Not moduleControls.TryGetValue(moduleControlID, moduleControl) Then
                'Not in Cached Dictionary so get ModuleControl from Data Base
                moduleControl = CBO.FillObject(Of ModuleControlInfo)(dataProvider.GetModuleControl(moduleControlID))
            End If

            Return moduleControl
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleControl gets a Dictionary of Module Controls by Module Definition
        ''' </summary>
        ''' <param name="moduleDefID">The ID of the Module Definition</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModuleControlsByModuleDefinitionID(ByVal moduleDefID As Integer) As Dictionary(Of String, ModuleControlInfo)
            Dim moduleControls As New Dictionary(Of String, ModuleControlInfo)

            'Iterate through cached Dictionary to get all Module Controls with the correct ModuleDefID
            For Each moduleControl As ModuleControlInfo In GetModuleControls().Values
                If moduleControl.ModuleDefID = moduleDefID Then
                    moduleControls(moduleControl.ControlKey) = moduleControl
                End If
            Next

            Return moduleControls
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleControlByControlKey gets a single Module Control from the database
        ''' </summary>
        ''' <param name="controlKey">The key for the control</param>
        ''' <param name="moduleDefID">The ID of the Module Definition</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModuleControlByControlKey(ByVal controlKey As String, ByVal moduleDefId As Integer) As ModuleControlInfo
            Dim moduleControl As ModuleControlInfo = Nothing

            'Try Cache
            For Each kvp As KeyValuePair(Of Integer, ModuleControlInfo) In GetModuleControls()
                If kvp.Value.ModuleDefID = moduleDefId AndAlso kvp.Value.ControlKey.ToLowerInvariant() = controlKey.ToLowerInvariant() Then
                    moduleControl = kvp.Value
                End If
            Next

            'Try Database
            If moduleControl Is Nothing Then
                moduleControl = CBO.FillObject(Of ModuleControlInfo)(dataProvider.GetModuleControlsByKey(controlKey, moduleDefId))
            End If

            Return moduleControl
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SaveModuleControl updates a Module Control in the database
        ''' </summary>
        ''' <param name="moduleControl">The Module Control to save</param>
        ''' <param name="clearCache">A flag that determines whether to clear the host cache</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function SaveModuleControl(ByVal moduleControl As ModuleControlInfo, ByVal clearCache As Boolean) As Integer
            Dim moduleControlID As Integer = moduleControl.ModuleControlID

            If moduleControlID = Null.NullInteger Then
                'Add new Module Definition
                moduleControlID = dataProvider.AddModuleControl(moduleControl.ModuleDefID, moduleControl.ControlKey, moduleControl.ControlTitle, moduleControl.ControlSrc, _
                                          moduleControl.IconFile, CType(moduleControl.ControlType, Integer), moduleControl.ViewOrder, moduleControl.HelpURL, _
                                          moduleControl.SupportsPartialRendering, UserController.GetCurrentUserInfo.UserID)
            Else
                'Upgrade Module Control
                dataProvider.UpdateModuleControl(moduleControl.ModuleControlID, moduleControl.ModuleDefID, moduleControl.ControlKey, moduleControl.ControlTitle, _
                                             moduleControl.ControlSrc, moduleControl.IconFile, CType(moduleControl.ControlType, Integer), moduleControl.ViewOrder, _
                                             moduleControl.HelpURL, moduleControl.SupportsPartialRendering, UserController.GetCurrentUserInfo.UserID)
            End If

            If clearCache Then
                DataCache.ClearHostCache(True)
            End If

            Return moduleControlID
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateModuleControl updates a Module Control in the database
        ''' </summary>
        ''' <param name="objModuleControl">The Module Control to save</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub UpdateModuleControl(ByVal objModuleControl As ModuleControlInfo)
            SaveModuleControl(objModuleControl, True)
        End Sub

#End Region

#Region "Obsolete"

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetModuleControlsByModuleDefinitionID(Integer)")> _
        Public Shared Function GetModuleControls(ByVal moduleDefID As Integer) As ArrayList
            Dim arrControls As New ArrayList()
            arrControls.AddRange(GetModuleControlsByModuleDefinitionID(moduleDefID).Values())
            Return arrControls
        End Function

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetModuleControlByControlKey(String, Integer)")> _
        Public Shared Function GetModuleControlsByKey(ByVal controlKey As String, ByVal moduleDefID As Integer) As ArrayList
            Dim arrControls As New ArrayList()
            arrControls.Add(GetModuleControlByControlKey(controlKey, moduleDefID))
            Return arrControls
        End Function

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetModuleControlByControlKey(String, Integer)")> _
        Public Shared Function GetModuleControlByKeyAndSrc(ByVal moduleDefID As Integer, ByVal controlKey As String, ByVal controlSrc As String) As ModuleControlInfo
            Return GetModuleControlByControlKey(controlKey, moduleDefID)
        End Function

#End Region

    End Class


End Namespace

