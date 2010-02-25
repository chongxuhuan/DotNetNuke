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
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Security.Permissions
Imports System.Xml
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Services.EventQueue

Namespace DotNetNuke.Entities.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Entities.Modules
    ''' Class	 : DesktopModuleController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' DesktopModuleController provides the Busines Layer for Desktop Modules
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/11/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DesktopModuleController

#Region "Private Shared Members"

        Private Shared dataProvider As DataProvider = dataProvider.Instance()

#End Region

#Region "Private Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetDesktopModulesByPortalCallBack gets a Dictionary of Desktop Modules by 
        ''' Portal from the the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	01/13/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetDesktopModulesByPortalCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim portalId As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Return CBO.FillDictionary(Of Integer, DesktopModuleInfo)("DesktopModuleID", dataProvider.GetDesktopModulesByPortal(portalId), _
                                                                     New Dictionary(Of Integer, DesktopModuleInfo))
        End Function

        Private Shared Function GetPortalDesktopModulesByPortalIDCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim portalId As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Return CBO.FillDictionary(Of Integer, PortalDesktopModuleInfo)("PortalDesktopModuleID", dataProvider.Instance().GetPortalDesktopModules(portalId, Null.NullInteger), _
                                                                     New Dictionary(Of Integer, PortalDesktopModuleInfo))
        End Function

#End Region

#Region "Public Shared Methods"

        Public Shared Function AddDesktopModuleToPortal(ByVal portalID As Integer, ByVal desktopModule As DesktopModuleInfo, ByVal permissions As DesktopModulePermissionCollection, ByVal clearCache As Boolean) As Integer
            Dim portalDesktopModuleID As Integer = AddDesktopModuleToPortal(portalID, desktopModule.DesktopModuleID, False, clearCache)

            If portalDesktopModuleID > Null.NullInteger Then
                'First remove any existing permissions
                DesktopModulePermissionController.DeleteDesktopModulePermissionsByPortalDesktopModuleID(portalDesktopModuleID)

                'Add new permissions
                For Each permission As DesktopModulePermissionInfo In permissions
                    permission.PortalDesktopModuleID = portalDesktopModuleID

                    DesktopModulePermissionController.AddDesktopModulePermission(permission)
                Next
            End If
            Return portalDesktopModuleID
        End Function

        Public Shared Function AddDesktopModuleToPortal(ByVal portalID As Integer, ByVal desktopModuleID As Integer, ByVal addPermissions As Boolean, ByVal clearCache As Boolean) As Integer
            Dim portalDesktopModuleID As Integer = Null.NullInteger
            Dim portalDesktopModule As PortalDesktopModuleInfo = GetPortalDesktopModule(portalID, desktopModuleID)

            If portalDesktopModule Is Nothing Then
                portalDesktopModuleID = dataProvider.Instance().AddPortalDesktopModule(portalID, desktopModuleID, UserController.GetCurrentUserInfo.UserID)
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                objEventLog.AddLog("PortalDesktopModuleID", portalDesktopModuleID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALDESKTOPMODULE_CREATED)

                If addPermissions Then
                    Dim permissions As ArrayList = PermissionController.GetPermissionsByPortalDesktopModule()

                    If permissions.Count > 0 Then
                        Dim permission As PermissionInfo = TryCast(permissions(0), PermissionInfo)
                        Dim objPortal As PortalInfo = New PortalController().GetPortal(portalID)

                        'Add default permissions for Administrator
                        If permission IsNot Nothing AndAlso objPortal IsNot Nothing Then
                            Dim desktopModulePermission As New DesktopModulePermissionInfo(permission)
                            desktopModulePermission.RoleID = objPortal.AdministratorRoleId
                            desktopModulePermission.AllowAccess = True
                            desktopModulePermission.PortalDesktopModuleID = portalDesktopModuleID
                            DesktopModulePermissionController.AddDesktopModulePermission(desktopModulePermission)
                        End If
                    End If
                End If
            Else
                portalDesktopModuleID = portalDesktopModule.PortalDesktopModuleID
            End If

            If clearCache Then
                DataCache.ClearPortalCache(portalID, True)
            End If

            Return portalDesktopModuleID
        End Function

        Public Shared Sub AddDesktopModuleToPortals(ByVal desktopModuleID As Integer)
            Dim controller As New PortalController
            For Each portal As PortalInfo In controller.GetPortals()
                'Add Portal/Module to PortalDesktopModules
                AddDesktopModuleToPortal(portal.PortalID, desktopModuleID, True, False)
            Next

            DataCache.ClearHostCache(True)
        End Sub

        Public Shared Sub AddDesktopModulesToPortal(ByVal portalID As Integer)
            For Each desktopModule As DesktopModuleInfo In DesktopModuleController.GetDesktopModules(Null.NullInteger).Values
                If Not desktopModule.IsPremium Then
                    'Add Portal/Module to PortalDesktopModules
                    AddDesktopModuleToPortal(portalID, desktopModule.DesktopModuleID, Not desktopModule.IsAdmin, False)
                End If
            Next

            DataCache.ClearPortalCache(portalID, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteDesktopModule deletes a Desktop Module
        ''' </summary>
        ''' <param name="moduleName">The Name of the Desktop Module to delete</param>
        ''' <history>
        ''' 	[cnurse]	05/14/2009   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteDesktopModule(ByVal moduleName As String)
            Dim desktopModule As DesktopModuleInfo = GetDesktopModuleByModuleName(moduleName, Null.NullInteger)

            If desktopModule IsNot Nothing Then
                Dim controller As New DesktopModuleController
                controller.DeleteDesktopModule(desktopModule.DesktopModuleID)

                'Delete the Package
                PackageController.DeletePackage(desktopModule.PackageID)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetDesktopModule gets a Desktop Module by its ID
        ''' </summary>
        ''' <remarks>This method uses the cached Dictionary of DesktopModules.  It first checks
        ''' if the DesktopModule is in the cache.  If it is not in the cache it then makes a call
        ''' to the Dataprovider.</remarks>
        ''' <param name="desktopModuleID">The ID of the Desktop Module to get</param>
        ''' <param name="portalID">The ID of the portal</param>
        ''' <history>
        ''' 	[cnurse]	01/13/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetDesktopModule(ByVal desktopModuleID As Integer, ByVal portalID As Integer) As DesktopModuleInfo
            Dim desktopModule As DesktopModuleInfo = Nothing

            'Try Cached Dictionary first
            Dim desktopModules As Dictionary(Of Integer, DesktopModuleInfo) = GetDesktopModules(portalID)
            If Not desktopModules.TryGetValue(desktopModuleID, desktopModule) Then
                'Not in Cached Dictionary so get DesktopModule from Data Base
                desktopModule = CBO.FillObject(Of DesktopModuleInfo)(dataProvider.GetDesktopModule(desktopModuleID))
            End If

            Return desktopModule
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetDesktopModuleByPackageID gets a Desktop Module by its Package ID
        ''' </summary>
        ''' <param name="packageID">The ID of the Package</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetDesktopModuleByPackageID(ByVal packageID As Integer) As DesktopModuleInfo
            Return CBO.FillObject(Of DesktopModuleInfo)(dataProvider.GetDesktopModuleByPackageID(packageID))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetDesktopModuleByModuleName gets a Desktop Module by its Name
        ''' </summary>
        ''' <remarks>This method uses the cached Dictionary of DesktopModules.  It first checks
        ''' if the DesktopModule is in the cache.  If it is not in the cache it then makes a call
        ''' to the Dataprovider.</remarks>
        ''' <param name="moduleName">The name of the Desktop Module to get</param>
        ''' <param name="portalID">The ID of the portal</param>
        ''' <history>
        ''' 	[cnurse]	01/13/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetDesktopModuleByModuleName(ByVal moduleName As String, ByVal portalID As Integer) As DesktopModuleInfo
            Dim desktopModule As DesktopModuleInfo = Nothing

            'Try Cached Dictionary first
            For Each kvp As KeyValuePair(Of Integer, DesktopModuleInfo) In GetDesktopModules(portalID)
                If kvp.Value.ModuleName = moduleName Then
                    desktopModule = kvp.Value
                    Exit For
                End If
            Next

            If desktopModule Is Nothing Then
                'Not in Cached Dictionary so get DesktopModule from Data Base
                desktopModule = CBO.FillObject(Of DesktopModuleInfo)(dataProvider.GetDesktopModuleByModuleName(moduleName))
            End If

            Return desktopModule
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetDesktopModules gets a Dictionary of Desktop Modules
        ''' </summary>
        ''' <param name="portalID">The ID of the Portal (Use PortalID = Null.NullInteger (-1) to get
        ''' all the DesktopModules including Modules not allowed for the current portal</param>
        ''' <history>
        ''' 	[cnurse]	01/13/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetDesktopModules(ByVal portalID As Integer) As Dictionary(Of Integer, DesktopModuleInfo)
            Dim desktopModules As New Dictionary(Of Integer, DesktopModuleInfo)
            If portalID = Null.NullInteger Then
                desktopModules = CBO.FillDictionary(Of Integer, DesktopModuleInfo)("DesktopModuleID", dataProvider.GetDesktopModules, New Dictionary(Of Integer, DesktopModuleInfo))
            Else
                Dim cacheKey As String = String.Format(DataCache.DesktopModuleCacheKey, portalID.ToString())
                desktopModules = CBO.GetCachedObject(Of Dictionary(Of Integer, DesktopModuleInfo))(New CacheItemArgs(cacheKey, DataCache.DesktopModuleCacheTimeOut, DataCache.DesktopModuleCachePriority, portalID), _
                                                                                                             AddressOf GetDesktopModulesByPortalCallBack)
            End If
            Return desktopModules
        End Function

        Public Shared Function GetPortalDesktopModule(ByVal portalID As Integer, ByVal desktopModuleID As Integer) As PortalDesktopModuleInfo
            Return CBO.FillObject(Of PortalDesktopModuleInfo)(dataProvider.Instance().GetPortalDesktopModules(portalID, desktopModuleID))
        End Function

        Public Shared Function GetDesktopModuleByFriendlyName(ByVal friendlyName As String) As DesktopModuleInfo
            Dim desktopModule As DesktopModuleInfo = Nothing
            For Each kvp As KeyValuePair(Of Integer, DesktopModuleInfo) In GetDesktopModules(Null.NullInteger)
                If kvp.Value.FriendlyName = friendlyName Then
                    desktopModule = kvp.Value
                    Exit For
                End If
            Next
            Return desktopModule
        End Function

        Public Shared Function GetPortalDesktopModulesByDesktopModuleID(ByVal desktopModuleID As Integer) As Dictionary(Of Integer, PortalDesktopModuleInfo)
            Return CBO.FillDictionary(Of Integer, PortalDesktopModuleInfo)("PortalDesktopModuleID", dataProvider.Instance().GetPortalDesktopModules(Null.NullInteger, desktopModuleID))
        End Function

        Public Shared Function GetPortalDesktopModulesByPortalID(ByVal portalID As Integer) As Dictionary(Of Integer, PortalDesktopModuleInfo)
            Dim cacheKey As String = String.Format(DataCache.PortalDesktopModuleCacheKey, portalID.ToString())
            Return CBO.GetCachedObject(Of Dictionary(Of Integer, PortalDesktopModuleInfo))(New CacheItemArgs(cacheKey, DataCache.PortalDesktopModuleCacheTimeOut, DataCache.PortalDesktopModuleCachePriority, portalID), _
                                                                                                             AddressOf GetPortalDesktopModulesByPortalIDCallBack)
        End Function

        Public Shared Function GetPortalDesktopModules(ByVal portalID As Integer) As SortedList(Of String, PortalDesktopModuleInfo)
            'First get all the DesktopModules for the Portal
            Dim dicModules As Dictionary(Of Integer, PortalDesktopModuleInfo) = GetPortalDesktopModulesByPortalID(portalID)
            Dim lstModules As New SortedList(Of String, PortalDesktopModuleInfo)

            For Each desktopModule As PortalDesktopModuleInfo In dicModules.Values
                If DesktopModulePermissionController.HasDesktopModulePermission(desktopModule.Permissions, "DEPLOY") Then
                    lstModules.Add(desktopModule.FriendlyName, desktopModule)
                End If
            Next

            Return lstModules
        End Function

        Public Shared Sub RemoveDesktopModuleFromPortal(ByVal portalID As Integer, ByVal desktopModuleID As Integer, ByVal clearCache As Boolean)
            dataProvider.Instance().DeletePortalDesktopModules(portalID, desktopModuleID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("DesktopModuleID", desktopModuleID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALDESKTOPMODULE_DELETED)
            If clearCache Then
                DataCache.ClearPortalCache(portalID, False)
            End If
        End Sub

        Public Shared Sub RemoveDesktopModuleFromPortals(ByVal desktopModuleID As Integer)
            dataProvider.Instance().DeletePortalDesktopModules(Null.NullInteger, desktopModuleID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("DesktopModuleID", desktopModuleID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALDESKTOPMODULE_DELETED)
            DataCache.ClearHostCache(True)
        End Sub

        Public Shared Sub RemoveDesktopModulesFromPortal(ByVal portalID As Integer)
            dataProvider.Instance().DeletePortalDesktopModules(portalID, Null.NullInteger)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("PortalID", portalID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALDESKTOPMODULE_DELETED)
            DataCache.ClearPortalCache(portalID, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SaveDesktopModule saves the Desktop Module to the database
        ''' </summary>
        ''' <param name="desktopModule">The Desktop Module to save</param>
        ''' <param name="saveChildren">A flag that determines whether the child objects are also saved</param>
        ''' <param name="clearCache">A flag that determines whether to clear the host cache</param>
        ''' <history>
        ''' 	[cnurse]	01/13/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function SaveDesktopModule(ByVal desktopModule As DesktopModuleInfo, ByVal saveChildren As Boolean, ByVal clearCache As Boolean) As Integer
            Dim desktopModuleID As Integer = desktopModule.DesktopModuleID
            Dim objEventLog As New Services.Log.EventLog.EventLogController

            If desktopModuleID = Null.NullInteger Then
                'Add new DesktopModule
                desktopModuleID = dataProvider.AddDesktopModule(desktopModule.PackageID, desktopModule.ModuleName, desktopModule.FolderName, desktopModule.FriendlyName, _
                                                            desktopModule.Description, desktopModule.Version, desktopModule.IsPremium, desktopModule.IsAdmin, _
                                                            desktopModule.BusinessControllerClass, desktopModule.SupportedFeatures, desktopModule.CompatibleVersions, _
                                                            desktopModule.Dependencies, desktopModule.Permissions, UserController.GetCurrentUserInfo.UserID)
                objEventLog.AddLog(desktopModule, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_CREATED)
            Else
                'Upgrade Module
                dataProvider.UpdateDesktopModule(desktopModule.DesktopModuleID, desktopModule.PackageID, desktopModule.ModuleName, desktopModule.FolderName, _
                                                 desktopModule.FriendlyName, desktopModule.Description, desktopModule.Version, desktopModule.IsPremium, _
                                                 desktopModule.IsAdmin, desktopModule.BusinessControllerClass, desktopModule.SupportedFeatures, _
                                                 desktopModule.CompatibleVersions, desktopModule.Dependencies, desktopModule.Permissions, UserController.GetCurrentUserInfo.UserID)
                objEventLog.AddLog(desktopModule, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_UPDATED)
            End If

            If saveChildren Then
                For Each definition As ModuleDefinitionInfo In desktopModule.ModuleDefinitions.Values
                    definition.DesktopModuleID = desktopModuleID

                    ' check if definition exists
                    Dim moduleDefinition As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(definition.FriendlyName, desktopModuleID)

                    If moduleDefinition IsNot Nothing Then
                        definition.ModuleDefID = moduleDefinition.ModuleDefID
                    End If

                    'Save ModuleDefinition and child objects to database
                    ModuleDefinitionController.SaveModuleDefinition(definition, saveChildren, clearCache)
                Next
            End If

            If clearCache Then
                DataCache.ClearHostCache(True)
            End If

            Return desktopModuleID
        End Function

        Public Shared Sub SerializePortalDesktopModules(ByVal writer As XmlWriter, ByVal portalID As Integer)
            'Serialize Portal DesktopModules
            writer.WriteStartElement("portalDesktopModules")
            For Each portalDesktopModule As PortalDesktopModuleInfo In GetPortalDesktopModulesByPortalID(portalID).Values
                writer.WriteStartElement("portalDesktopModule")
                writer.WriteElementString("friendlyname", portalDesktopModule.FriendlyName)
                writer.WriteStartElement("portalDesktopModulePermissions")
                For Each permission As DesktopModulePermissionInfo In portalDesktopModule.Permissions
                    writer.WriteStartElement("portalDesktopModulePermission")

                    writer.WriteElementString("permissioncode", permission.PermissionCode)
                    writer.WriteElementString("permissionkey", permission.PermissionKey)
                    writer.WriteElementString("allowaccess", permission.AllowAccess.ToString().ToLower())
                    writer.WriteElementString("rolename", permission.RoleName)

                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()

                writer.WriteEndElement()
            Next
            writer.WriteEndElement()
        End Sub

#End Region

#Region "DesktopModuleInfo Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddDesktopModule adds a new Desktop Module to the database
        ''' </summary>
        ''' <param name="objDesktopModule">The Desktop Module to save</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function AddDesktopModule(ByVal objDesktopModule As DesktopModuleInfo) As Integer
            Return SaveDesktopModule(objDesktopModule, False, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteDesktopModule deletes a Desktop Module
        ''' </summary>
        ''' <param name="objDesktopModule">The Desktop Module to delete</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteDesktopModule(ByVal objDesktopModule As DesktopModuleInfo)
            dataProvider.DeleteDesktopModule(objDesktopModule.DesktopModuleID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objDesktopModule, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_DELETED)
            DataCache.ClearHostCache(True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteDesktopModule deletes a Desktop Module By ID
        ''' </summary>
        ''' <param name="desktopModuleID">The ID of the Desktop Module to delete</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteDesktopModule(ByVal desktopModuleID As Integer)
            dataProvider.DeleteDesktopModule(desktopModuleID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("DesktopModuleID", desktopModuleID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_DELETED)
            DataCache.ClearHostCache(True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateDesktopModule saves the Desktop Module to the database
        ''' </summary>
        ''' <param name="objDesktopModule">The Desktop Module to save</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateDesktopModule(ByVal objDesktopModule As DesktopModuleInfo)
            SaveDesktopModule(objDesktopModule, False, True)
        End Sub

		Public Sub UpdateModuleInterfaces(ByRef desktopModuleInfo As DesktopModuleInfo)
			If (UserController.GetCurrentUserInfo() Is Nothing) Then
				UpdateModuleInterfaces(desktopModuleInfo, "", True)
			Else
				UpdateModuleInterfaces(desktopModuleInfo, UserController.GetCurrentUserInfo().Username, True)
			End If
		End Sub

		Public Sub UpdateModuleInterfaces(ByRef desktopModuleInfo As DesktopModuleInfo, ByVal sender As String, ByVal forceAppRestart As Boolean)
			'this cannot be done directly at this time because 
			'the module may not be loaded into the app domain yet
			'So send an EventMessage that will process the update 
			'after the App recycles
			Dim oAppStartMessage As New EventQueue.EventMessage
			oAppStartMessage.Sender = sender
			oAppStartMessage.Priority = MessagePriority.High
			oAppStartMessage.ExpirationDate = Now.AddYears(-1)
			oAppStartMessage.SentDate = System.DateTime.Now
			oAppStartMessage.Body = ""
			oAppStartMessage.ProcessorType = "DotNetNuke.Entities.Modules.EventMessageProcessor, DotNetNuke"
			oAppStartMessage.ProcessorCommand = "UpdateSupportedFeatures"

			'Add custom Attributes for this message
			oAppStartMessage.Attributes.Add("BusinessControllerClass", desktopModuleInfo.BusinessControllerClass)
			oAppStartMessage.Attributes.Add("DesktopModuleId", desktopModuleInfo.DesktopModuleID.ToString())

			'send it to occur on next App_Start Event
			EventQueueController.SendMessage(oAppStartMessage, "Application_Start")

			'force an app restart
			If (forceAppRestart) Then
				DotNetNuke.Common.Utilities.Config.Touch()
			End If
		End Sub

#End Region

#Region "Obsolete"

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method AddDesktopModuleToPortal(Integer, Integer)")> _
        Public Sub AddPortalDesktopModule(ByVal portalID As Integer, ByVal desktopModuleID As Integer)
            dataProvider.Instance().AddPortalDesktopModule(portalID, desktopModuleID, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("DesktopModuleID", desktopModuleID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALDESKTOPMODULE_CREATED)
        End Sub

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method RemoveDesktopModulesFromPortal(Integer)")> _
        Public Sub DeletePortalDesktopModules(ByVal PortalID As Integer, ByVal desktopModuleID As Integer)
            dataProvider.Instance().DeletePortalDesktopModules(PortalID, desktopModuleID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("DesktopModuleID", desktopModuleID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALDESKTOPMODULE_DELETED)
            DataCache.ClearPortalCache(PortalID, True)
        End Sub

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetDesktopModule(Integer, Integer)")> _
        Public Function GetDesktopModule(ByVal desktopModuleId As Integer) As DesktopModuleInfo
            Return CBO.FillObject(Of DesktopModuleInfo)(dataProvider.GetDesktopModule(desktopModuleId))
        End Function

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetDesktopModuleByModuleName(String, Integer)")> _
        Public Function GetDesktopModuleByName(ByVal FriendlyName As String) As DesktopModuleInfo
            Return CType(CBO.FillObject(dataProvider.Instance().GetDesktopModuleByFriendlyName(FriendlyName), GetType(DesktopModuleInfo)), DesktopModuleInfo)
        End Function

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetDesktopModuleByModuleName(String, Integer)")> _
        Public Function GetDesktopModuleByModuleName(ByVal moduleName As String) As DesktopModuleInfo
            Return CBO.FillObject(Of DesktopModuleInfo)(dataProvider.GetDesktopModuleByModuleName(moduleName))
        End Function

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetDesktopModules(Integer)")> _
        Public Function GetDesktopModules() As ArrayList
            Return CBO.FillCollection(dataProvider.GetDesktopModules(), GetType(DesktopModuleInfo))
        End Function

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetDesktopModules(Integer)")> _
        Public Function GetDesktopModulesByPortal(ByVal portalID As Integer) As ArrayList
            Return CBO.FillCollection(dataProvider.GetDesktopModulesByPortal(portalID), GetType(DesktopModuleInfo))
        End Function

        <Obsolete("This method replaced in DotNetNuke 5.0 by Shared method GetPortalDesktopModulesByPortalID(Integer) and GetPortalDesktopModulesByDesktopModuleID(Integer) And GetPortalDesktopModule(Integer, Integer)")> _
        Public Function GetPortalDesktopModules(ByVal portalID As Integer, ByVal desktopModuleID As Integer) As ArrayList
            Return CBO.FillCollection(dataProvider.Instance().GetPortalDesktopModules(portalID, desktopModuleID), GetType(PortalDesktopModuleInfo))
        End Function


#End Region

    End Class

End Namespace

