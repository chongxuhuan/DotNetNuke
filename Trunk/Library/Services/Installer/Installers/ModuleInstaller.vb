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

Imports System.IO
Imports System.Xml.XPath

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Services.EventQueue
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ModuleInstaller installs Module Components to a DotNetNuke site
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/15/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ModuleInstaller
        Inherits ComponentInstallerBase

#Region "Private Properties"

        Private InstalledDesktopModule As DesktopModuleInfo
        Private DesktopModule As DesktopModuleInfo
        Private EventMessage As EventMessage

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a list of allowable file extensions (in addition to the Host's List)
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides ReadOnly Property AllowableFiles() As String
            Get
                Return "cshtml, vbhtml, ashx, aspx, ascx, vb, cs, resx, css, js, resources, config, vbproj, csproj, sln, htm, html, xml, psd"
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DeleteModule method deletes the Module from the data Store.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/15/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteModule()
            Try
                'Attempt to get the Desktop Module
                Dim tempDesktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByPackageID(Package.PackageID)

                If tempDesktopModule IsNot Nothing Then
                    'Remove CodeSubDirectory
                    If (DesktopModule IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(DesktopModule.CodeSubDirectory)) Then
                        DotNetNuke.Common.Utilities.Config.RemoveCodeSubDirectory(DesktopModule.CodeSubDirectory)
                    End If

                    Dim controller As New DesktopModuleController
                    controller.DeleteDesktopModule(tempDesktopModule)
                End If
                Log.AddInfo(String.Format(Util.MODULE_UnRegistered, tempDesktopModule.ModuleName))
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Commit method finalises the Install and commits any pending changes.
        ''' </summary>
        ''' <remarks>In the case of Modules this is not neccessary</remarks>
        ''' <history>
        ''' 	[cnurse]	01/15/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Commit()
            'Add CodeSubDirectory
            If Not String.IsNullOrEmpty(DesktopModule.CodeSubDirectory) Then
                DotNetNuke.Common.Utilities.Config.AddCodeSubDirectory(DesktopModule.CodeSubDirectory)
            End If

            If DesktopModule.SupportedFeatures = Null.NullInteger Then
                'Set an Event Message so the features are loaded by reflection on restart
                Dim oAppStartMessage As New EventQueue.EventMessage
                oAppStartMessage.Priority = MessagePriority.High
                oAppStartMessage.ExpirationDate = Now.AddYears(-1)
                oAppStartMessage.SentDate = System.DateTime.Now
                oAppStartMessage.Body = ""
                oAppStartMessage.ProcessorType = "DotNetNuke.Entities.Modules.EventMessageProcessor, DotNetNuke"
                oAppStartMessage.ProcessorCommand = "UpdateSupportedFeatures"

                'Add custom Attributes for this message
                oAppStartMessage.Attributes.Add("BusinessControllerClass", DesktopModule.BusinessControllerClass)
                oAppStartMessage.Attributes.Add("desktopModuleID", DesktopModule.DesktopModuleID.ToString())

                'send it to occur on next App_Start Event
                EventQueueController.SendMessage(oAppStartMessage, "Application_Start_FirstRequest")
            End If

            'Add Event Message
            If EventMessage IsNot Nothing Then
                EventMessage.Attributes.Set("desktopModuleID", DesktopModule.DesktopModuleID.ToString())
                EventQueueController.SendMessage(EventMessage, "Application_Start_FirstRequest")
            End If

            'Add DesktopModule to all portals
            If Not DesktopModule.IsPremium Then
                DesktopModuleController.AddDesktopModuleToPortals(DesktopModule.DesktopModuleID)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Install method installs the Module component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/15/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Install()
            Try
                'Attempt to get the Desktop Module
                InstalledDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName(DesktopModule.ModuleName, Package.InstallerInfo.PortalID)

                If InstalledDesktopModule IsNot Nothing Then
                    DesktopModule.DesktopModuleID = InstalledDesktopModule.DesktopModuleID
                End If

                'Clear ModuleControls and Module Definitions caches in case script has modifed the contents
                DataCache.RemoveCache(DataCache.ModuleDefinitionCacheKey)
                DataCache.RemoveCache(DataCache.ModuleControlsCacheKey)

                'Save DesktopModule and child objects to database
                DesktopModule.PackageID = Package.PackageID
                DesktopModule.DesktopModuleID = DesktopModuleController.SaveDesktopModule(DesktopModule, True, False)

                Completed = True
                Log.AddInfo(String.Format(Util.MODULE_Registered, DesktopModule.ModuleName))
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadManifest method reads the manifest file for the Module compoent.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/15/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub ReadManifest(ByVal manifestNav As XPathNavigator)

            'Load the Desktop Module from the manifest
            DesktopModule = CBO.DeserializeObject(Of DesktopModuleInfo)(New StringReader(manifestNav.InnerXml))

            DesktopModule.FriendlyName = Package.FriendlyName
            DesktopModule.Description = Package.Description
            DesktopModule.Version = FormatVersion(Package.Version)
            'DesktopModule.IsPremium = False
            'DesktopModule.IsAdmin = False
            DesktopModule.CompatibleVersions = Null.NullString
            DesktopModule.Dependencies = Null.NullString
            DesktopModule.Permissions = Null.NullString
            If String.IsNullOrEmpty(DesktopModule.BusinessControllerClass) Then
                DesktopModule.SupportedFeatures = 0
            End If

            Dim eventMessageNav As XPathNavigator = manifestNav.SelectSingleNode("eventMessage")
            If eventMessageNav IsNot Nothing Then
                EventMessage = New EventQueue.EventMessage
                EventMessage.Priority = MessagePriority.High
                EventMessage.ExpirationDate = Now.AddYears(-1)
                EventMessage.SentDate = System.DateTime.Now
                EventMessage.Body = ""
                EventMessage.ProcessorType = Util.ReadElement(eventMessageNav, "processorType", Log, Util.EVENTMESSAGE_TypeMissing)
                EventMessage.ProcessorCommand = Util.ReadElement(eventMessageNav, "processorCommand", Log, Util.EVENTMESSAGE_CommandMissing)

                For Each attributeNav As XPathNavigator In eventMessageNav.Select("attributes/*")
                    EventMessage.Attributes.Add(attributeNav.Name, attributeNav.Value)
                Next
            End If

            'Load permissions (to add)
            For Each moduleDefinitionNav As XPathNavigator In manifestNav.Select("desktopModule/moduleDefinitions/moduleDefinition")
                Dim friendlyName As String = Util.ReadElement(moduleDefinitionNav, "friendlyName")
                For Each permissionNav As XPathNavigator In moduleDefinitionNav.Select("permissions/permission")
                    Dim permission As New PermissionInfo
                    permission.PermissionCode = Util.ReadAttribute(permissionNav, "code")
                    permission.PermissionKey = Util.ReadAttribute(permissionNav, "key")
                    permission.PermissionName = Util.ReadAttribute(permissionNav, "name")

                    Dim moduleDefinition As ModuleDefinitionInfo = DesktopModule.ModuleDefinitions(friendlyName)
                    If moduleDefinition IsNot Nothing Then
                        moduleDefinition.Permissions.Add(permission.PermissionKey, permission)
                    End If
                Next
            Next


            If Log.Valid Then
                Log.AddInfo(Util.MODULE_ReadSuccess)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Rollback method undoes the installation of the component in the event 
        ''' that one of the other components fails
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/15/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Rollback()
            'If Temp Module exists then we need to update the DataStore with this 
            If InstalledDesktopModule Is Nothing Then
                'No Temp Module - Delete newly added module
                DeleteModule()
            Else
                'Temp Module - Rollback to Temp
                DesktopModuleController.SaveDesktopModule(InstalledDesktopModule, True, False)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstall method uninstalls the Module component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/15/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UnInstall()
            DeleteModule()
        End Sub

#End Region

    End Class

End Namespace
