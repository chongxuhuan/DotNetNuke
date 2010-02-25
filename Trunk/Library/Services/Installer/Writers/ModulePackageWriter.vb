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
Imports System.IO
Imports System.Xml.XPath

Imports DotNetNuke.Services.Localization
Imports System.Xml
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports System.Collections.Generic


Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ModulePackageWriter class 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/30/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ModulePackageWriter
        Inherits PackageWriterBase

#Region "Private Members"

        Private _DesktopModule As DesktopModuleInfo

#End Region

#Region "Constructors"

        Public Sub New(ByVal manifestNav As XPathNavigator, ByVal installer As InstallerInfo)
            _DesktopModule = New DesktopModuleInfo

            'Create a Package
            Package = New PackageInfo(installer)

            ReadLegacyManifest(manifestNav, True)

            Package.Name = DesktopModule.ModuleName
            Package.FriendlyName = DesktopModule.FriendlyName
            Package.Description = DesktopModule.Description
            If Not String.IsNullOrEmpty(DesktopModule.Version) Then
                Package.Version = New Version(DesktopModule.Version)
            End If
            Package.PackageType = "Module"

            LegacyUtil.ParsePackageName(Package)

            Initialize(DesktopModule.FolderName)
        End Sub

        Public Sub New(ByVal desktopModule As DesktopModuleInfo, ByVal manifestNav As XPathNavigator, ByVal package As PackageInfo)
            MyBase.New(package)

            _DesktopModule = desktopModule

            Initialize(desktopModule.FolderName)

            If manifestNav IsNot Nothing Then
                ReadLegacyManifest(manifestNav.SelectSingleNode("folders/folder"), False)
            End If

            Dim physicalFolderPath As String = Path.Combine(ApplicationMapPath, BasePath)
            ProcessModuleFolders(physicalFolderPath, physicalFolderPath)
        End Sub

        Public Sub New(ByVal package As PackageInfo)
            MyBase.New(package)
            _DesktopModule = DesktopModuleController.GetDesktopModuleByPackageID(package.PackageID)
            Initialize(DesktopModule.FolderName)
        End Sub

        Public Sub New(ByVal desktopModule As DesktopModuleInfo, ByVal package As PackageInfo)
            MyBase.New(package)
            _DesktopModule = desktopModule
            Initialize(desktopModule.FolderName)
        End Sub

#End Region

#Region "Protected Properties"

        Protected Overrides ReadOnly Property Dependencies() As Dictionary(Of String, String)
            Get
                Dim _Dependencies As New Dictionary(Of String, String)
                If Not String.IsNullOrEmpty(DesktopModule.Dependencies) Then
                    _Dependencies("type") = DesktopModule.Dependencies
                End If
                If Not String.IsNullOrEmpty(DesktopModule.Permissions) Then
                    _Dependencies("permission") = DesktopModule.Permissions
                End If
                Return _Dependencies
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the associated Desktop Module
        ''' </summary>
        ''' <value>A DesktopModuleInfo object</value>
        ''' <history>
        ''' 	[cnurse]	02/01/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DesktopModule() As DesktopModuleInfo
            Get
                Return _DesktopModule
            End Get
            Set(ByVal value As DesktopModuleInfo)
                _DesktopModule = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Sub Initialize(ByVal folder As String)
            BasePath = Path.Combine("DesktopModules", folder).Replace("/", "\")
            AppCodePath = Path.Combine("App_Code", folder).Replace("/", "\")
            AssemblyPath = "bin"
        End Sub

        Private Shared Sub ProcessControls(ByVal controlNav As XPathNavigator, ByVal moduleFolder As String, ByVal definition As ModuleDefinitionInfo)
            Dim moduleControl As New ModuleControlInfo()

            moduleControl.ControlKey = Util.ReadElement(controlNav, "key")
            moduleControl.ControlTitle = Util.ReadElement(controlNav, "title")

            'Write controlSrc
            Dim ControlSrc As String = Util.ReadElement(controlNav, "src")
            If Not (ControlSrc.ToLower.StartsWith("desktopmodules") OrElse Not ControlSrc.ToLower.EndsWith(".ascx")) Then
                ' this code allows a developer to reference an ASCX file in a different folder than the module folder ( good for ASCX files shared between modules where you want only a single copy )
                'or it allows the developer to use webcontrols rather than usercontrols
                ControlSrc = Path.Combine("DesktopModules", Path.Combine(moduleFolder, ControlSrc))
            End If
            ControlSrc = ControlSrc.Replace("\"c, "/")
            moduleControl.ControlSrc = ControlSrc

            moduleControl.IconFile = Util.ReadElement(controlNav, "iconfile")

            Dim controlType As String = Util.ReadElement(controlNav, "type")
            If Not String.IsNullOrEmpty(controlType) Then
                Try
                    moduleControl.ControlType = CType(TypeDescriptor.GetConverter(GetType(SecurityAccessLevel)).ConvertFromString(controlType), SecurityAccessLevel)
                Catch ex As Exception
                    Throw New Exception(Util.EXCEPTION_Type)
                End Try
            End If

            Dim viewOrder As String = Util.ReadElement(controlNav, "vieworder")
            If Not String.IsNullOrEmpty(viewOrder) Then
                moduleControl.ViewOrder = Integer.Parse(viewOrder)
            End If
            moduleControl.HelpURL = Util.ReadElement(controlNav, "helpurl")

            Dim supportsPartialRendering As String = Util.ReadElement(controlNav, "supportspartialrendering")
            If Not String.IsNullOrEmpty(supportsPartialRendering) Then
                moduleControl.SupportsPartialRendering = Boolean.Parse(supportsPartialRendering)
            End If

            definition.ModuleControls.Item(moduleControl.ControlKey) = moduleControl
        End Sub

        Private Sub ProcessModuleFiles(ByVal folder As String, ByVal basePath As String)
            'we are going to drill down through the folders to add the files
            For Each fileName As String In Directory.GetFiles(folder)
                Dim name As String = fileName.Replace(basePath + "\", "")

                AddFile(name, name)
            Next
        End Sub

        Private Sub ProcessModuleFolders(ByVal folder As String, ByVal basePath As String)
            'Process Folders recursively
            For Each directoryName As String In Directory.GetDirectories(folder)
                ProcessModuleFolders(directoryName, basePath)
            Next

            'process files
            ProcessModuleFiles(folder, basePath)
        End Sub

        Private Sub ProcessModules(ByVal moduleNav As XPathNavigator, ByVal moduleFolder As String)
            Dim definition As New ModuleDefinitionInfo()

            definition.FriendlyName = Util.ReadElement(moduleNav, "friendlyname")
            Dim cacheTime As String = Util.ReadElement(moduleNav, "cachetime")
            If Not String.IsNullOrEmpty(cacheTime) Then
                definition.DefaultCacheTime = Integer.Parse(cacheTime)
            End If

            'Process legacy controls Node
            For Each controlNav As XPathNavigator In moduleNav.Select("controls/control")
                ProcessControls(controlNav, moduleFolder, definition)
            Next

            DesktopModule.ModuleDefinitions.Item(definition.FriendlyName) = definition
        End Sub

        Private Sub ReadLegacyManifest(ByVal folderNav As XPathNavigator, ByVal processModule As Boolean)

            If processModule Then
                'Version 2 of legacy manifest
                Dim name As String = Util.ReadElement(folderNav, "name")
                DesktopModule.FolderName = name
                DesktopModule.ModuleName = name
                DesktopModule.FriendlyName = name

                Dim folderName As String = Util.ReadElement(folderNav, "foldername")
                If Not String.IsNullOrEmpty(folderName) Then
                    DesktopModule.FolderName = folderName
                End If
                If String.IsNullOrEmpty(DesktopModule.FolderName) Then
                    DesktopModule.FolderName = "MyModule"
                End If

                Dim friendlyname As String = Util.ReadElement(folderNav, "friendlyname")
                If Not String.IsNullOrEmpty(friendlyname) Then
                    DesktopModule.FriendlyName = friendlyname
                    DesktopModule.ModuleName = friendlyname
                End If

                Dim modulename As String = Util.ReadElement(folderNav, "modulename")
                If Not String.IsNullOrEmpty(modulename) Then
                    DesktopModule.ModuleName = modulename
                End If

                Dim permissions As String = Util.ReadElement(folderNav, "permissions")
                If Not String.IsNullOrEmpty(Permissions) Then
                    DesktopModule.Permissions = permissions
                End If

                Dim dependencies As String = Util.ReadElement(folderNav, "dependencies")
                If Not String.IsNullOrEmpty(dependencies) Then
                    DesktopModule.Dependencies = dependencies
                End If

                DesktopModule.Version = Util.ReadElement(folderNav, "version", "01.00.00")
                DesktopModule.Description = Util.ReadElement(folderNav, "description")
                DesktopModule.BusinessControllerClass = Util.ReadElement(folderNav, "businesscontrollerclass")

                'Process legacy modules Node
                For Each moduleNav As XPathNavigator In folderNav.Select("modules/module")
                    ProcessModules(moduleNav, DesktopModule.FolderName)
                Next
            End If

            'Process legacy files Node
            For Each fileNav As XPathNavigator In folderNav.Select("files/file")
                Dim fileName As String = Util.ReadElement(fileNav, "name")
                Dim filePath As String = Util.ReadElement(fileNav, "path")

                'In Legacy Modules the file can be physically located in the Root of the zip folder - or in the path/file location
                'First test the folder
                Dim sourceFileName As String
                If filePath.Contains("[app_code]") Then
                    'Special case for App_code - files can be in App_Code\ModuleName or root
                    sourceFileName = Path.Combine(filePath, fileName).Replace("[app_code]", "App_Code\" + DesktopModule.FolderName)
                Else
                    sourceFileName = Path.Combine(filePath, fileName)
                End If
                Dim tempFolder As String = Package.InstallerInfo.TempInstallFolder
                If Not File.Exists(Path.Combine(tempFolder, sourceFileName)) Then
                    sourceFileName = fileName
                End If

                'In Legacy Modules the assembly is always in "bin" - ignore the path element
                If fileName.ToLower.EndsWith(".dll") Then
                    AddFile("bin/" & fileName, sourceFileName)
                Else
                    AddFile(Path.Combine(filePath, fileName), sourceFileName)
                End If
            Next

            'Process resource file Node
            If Not String.IsNullOrEmpty(Util.ReadElement(folderNav, "resourcefile")) Then
                AddResourceFile(New InstallFile(Util.ReadElement(folderNav, "resourcefile"), Package.InstallerInfo))
            End If

        End Sub

        Private Sub WriteEventMessage(ByVal writer As XmlWriter)
            'Start Start eventMessage
            writer.WriteStartElement("eventMessage")

            'Write Processor Type
            writer.WriteElementString("processorType", "DotNetNuke.Entities.Modules.EventMessageProcessor, DotNetNuke")

            'Write Processor Type
            writer.WriteElementString("processorCommand", "UpgradeModule")

            'Write Event Message Attributes
            writer.WriteStartElement("attributes")

            'Write businessControllerClass Attribute
            writer.WriteElementString("businessControllerClass", DesktopModule.BusinessControllerClass)

            'Write businessControllerClass Attribute
            writer.WriteElementString("desktopModuleID", "[DESKTOPMODULEID]")

            'Write upgradeVersionsList Attribute
            Dim upgradeVersions As String = Null.NullString
            Versions.Sort()
            For Each version As String In Versions
                upgradeVersions += version + ","
            Next
            If upgradeVersions.Length > 1 Then
                upgradeVersions = upgradeVersions.Remove(upgradeVersions.Length - 1, 1)
            End If
            writer.WriteElementString("upgradeVersionsList", upgradeVersions)

            'End end of Event Message Attribues
            writer.WriteEndElement()

            'End component Element
            writer.WriteEndElement()
        End Sub

        Private Sub WriteModuleComponent(ByVal writer As XmlWriter)
            'Start component Element
            writer.WriteStartElement("component")
            writer.WriteAttributeString("type", "Module")

            'Write Module Manifest
            If AppCodeFiles.Count > 0 Then
                DesktopModule.CodeSubDirectory = DesktopModule.FolderName
            End If
            CBO.SerializeObject(DesktopModule, writer)

            'Write EventMessage
            If Not String.IsNullOrEmpty(DesktopModule.BusinessControllerClass) Then
                WriteEventMessage(writer)
            End If

            'End component Element
            writer.WriteEndElement()
        End Sub

#End Region

#Region "Protected Methods"

        Protected Overrides Sub WriteManifestComponent(ByVal writer As System.Xml.XmlWriter)
            'Write Module Component
            WriteModuleComponent(writer)
        End Sub

#End Region

    End Class

End Namespace

