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

Imports ICSharpCode.SharpZipLib.Zip
Imports System.IO
Imports System.Text
Imports DotNetNuke.Services.Installer.Log
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Services.Installer.Installers
Imports System.Collections.Generic
Imports System.Xml.XPath
Imports System.Xml
Imports DotNetNuke.Services.Installer.Writers

Namespace DotNetNuke.Services.Installer

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Installer class provides a single entrypoint for Package Installation
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	07/24/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Installer

#Region "Private Members"

        Private _InstallerInfo As InstallerInfo
        Private _LegacyError As String
        Private _Packages As New SortedList(Of Integer, PackageInstaller)

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new Installer instance from a string representing
        ''' the physical path to the temporary install folder and a string representing 
        ''' the physical path to the root of the site
        ''' </summary>
        ''' <param name="tempFolder">The physical path to the zip file containg the package</param>
        ''' <param name="manifest">The manifest filename</param>
        ''' <param name="physicalSitePath">The physical path to the root of the site</param>
        ''' <param name="loadManifest">Flag that determines whether the manifest will be loaded</param>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal tempFolder As String, ByVal manifest As String, ByVal physicalSitePath As String, ByVal loadManifest As Boolean)
            _InstallerInfo = New InstallerInfo(tempFolder, manifest, physicalSitePath)

            'Called from Interactive installer - default IgnoreWhiteList to false
            _InstallerInfo.IgnoreWhiteList = False
            If loadManifest Then
                ReadManifest(True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new Installer instance from a Stream and a
        ''' string representing the physical path to the root of the site
        ''' </summary>
        ''' <param name="inputStream">The Stream to use to create this InstallerInfo instance</param>
        ''' <param name="physicalSitePath">The physical path to the root of the site</param>
        ''' <param name="loadManifest">Flag that determines whether the manifest will be loaded</param>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal inputStream As Stream, ByVal physicalSitePath As String, ByVal loadManifest As Boolean)
            Me.New(inputStream, physicalSitePath, loadManifest, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new Installer instance from a Stream and a
        ''' string representing the physical path to the root of the site
        ''' </summary>
        ''' <param name="inputStream">The Stream to use to create this InstallerInfo instance</param>
        ''' <param name="physicalSitePath">The physical path to the root of the site</param>
        ''' <param name="loadManifest">Flag that determines whether the manifest will be loaded</param>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal inputStream As Stream, ByVal physicalSitePath As String, ByVal loadManifest As Boolean, ByVal deleteTemp As Boolean)
            _InstallerInfo = New InstallerInfo(inputStream, physicalSitePath)

            'Called from Batch installer - default IgnoreWhiteList to true
            _InstallerInfo.IgnoreWhiteList = True

            If loadManifest Then
                ReadManifest(deleteTemp)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new Installer instance from a PackageInfo object
        ''' </summary>
        ''' <param name="package">The PackageInfo instance</param>
        ''' <param name="physicalSitePath">The physical path to the root of the site</param>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal package As PackageInfo, ByVal physicalSitePath As String)
            _InstallerInfo = New InstallerInfo(package, physicalSitePath)

            Packages.Add(Packages.Count, New PackageInstaller(package))
        End Sub

        Public Sub New(ByVal manifest As String, ByVal physicalSitePath As String, ByVal loadManifest As Boolean)
            _InstallerInfo = New InstallerInfo(physicalSitePath, InstallMode.ManifestOnly)
            If loadManifest Then
                ReadManifest(New FileStream(manifest, FileMode.Open, FileAccess.Read))
            End If
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the associated InstallerInfo object
        ''' </summary>
        ''' <value>An InstallerInfo</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property InstallerInfo() As InstallerInfo
            Get
                Return _InstallerInfo
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the associated InstallerInfo is valid
        ''' </summary>
        ''' <value>True - if valid, False if not</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsValid() As Boolean
            Get
                Return InstallerInfo.IsValid
            End Get
        End Property

        ''' <summary>
        ''' Gets and sets whether there are any errors in parsing legacy packages
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LegacyError() As String
            Get
                Return _LegacyError
            End Get
            Set(ByVal value As String)
                _LegacyError = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a SortedList of Packages that are included in the Package Zip
        ''' </summary>
        ''' <value>A SortedList(Of Integer, PackageInstaller)</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Packages() As SortedList(Of Integer, PackageInstaller)
            Get
                Return _Packages
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets 
        ''' </summary>
        ''' <value>A Dictionary(Of String, PackageInstaller)</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property TempInstallFolder() As String
            Get
                Return InstallerInfo.TempInstallFolder
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The InstallPackages method installs the packages
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub InstallPackages()
            'Iterate through all the Packages
            For index As Integer = 0 To Packages.Count - 1
                Dim installer As PackageInstaller = Packages.Values(index)
                'Check if package is valid
                If installer.Package.IsValid Then
                    InstallerInfo.Log.AddInfo(Util.INSTALL_Start + " - " + installer.Package.Name)
                    installer.Install()
                    If InstallerInfo.Log.Valid Then
                        InstallerInfo.Log.AddInfo(Util.INSTALL_Success + " - " + installer.Package.Name)
                    Else
                        InstallerInfo.Log.AddInfo(Util.INSTALL_Failed + " - " + installer.Package.Name)
                    End If
                Else
                    InstallerInfo.Log.AddFailure(Util.INSTALL_Aborted + " - " + installer.Package.Name)
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Logs the Install event to the Event Log
        ''' </summary>
        ''' <param name="package">The name of the package</param>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LogInstallEvent(ByVal package As String, ByVal eventType As String)
            Try
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString
                objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo(eventType + " " + package + ":", InstallerInfo.ManifestFile.Name.Replace(".dnn", "")))
                Dim objLogEntry As LogEntry
                For Each objLogEntry In InstallerInfo.Log.Logs
                    objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("Info:", objLogEntry.Description))
                Next
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                objEventLog.AddLog(objEventLogInfo)
            Catch ex As Exception
                ' error
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ProcessPackages method processes the packages nodes in the manifest
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProcessPackages(ByVal rootNav As XPathNavigator)
            'Parse the package nodes
            For Each nav As XPathNavigator In rootNav.Select("packages/package")
                Dim order As Integer = Packages.Count
                Dim name As String = Util.ReadAttribute(nav, "name")
                Dim installOrder As String = Util.ReadAttribute(nav, "installOrder")
                If Not String.IsNullOrEmpty(installOrder) Then
                    order = Integer.Parse(installOrder)
                End If
                Packages.Add(order, New PackageInstaller(nav.OuterXml, InstallerInfo))
            Next
        End Sub

        Private Sub ReadManifest(ByVal stream As Stream)
            Dim doc As New XPathDocument(stream)

            'Read the root node to determine what version the manifest is
            Dim rootNav As XPathNavigator = doc.CreateNavigator()
            rootNav.MoveToFirstChild()
            Dim packageType As String = Null.NullString
            If rootNav.Name = "dotnetnuke" Then
                packageType = Util.ReadAttribute(rootNav, "type")
            ElseIf rootNav.Name.ToLower = "languagepack" Then
                packageType = "LanguagePack"
            Else
                InstallerInfo.Log.AddFailure(Util.PACKAGE_UnRecognizable)
            End If

            Select Case packageType.ToLower()
                Case "package"
                    InstallerInfo.IsLegacyMode = False
                    'Parse the package nodes
                    ProcessPackages(rootNav)
                Case "module"
                    InstallerInfo.IsLegacyMode = True

                    'Create an xml writer to create the processed manifest
                    Dim sb As New StringBuilder
                    Dim writer As XmlWriter = XmlWriter.Create(sb, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment))

                    'Write manifest start element
                    PackageWriterBase.WriteManifestStartElement(writer)

                    'Legacy Module - Process each folder
                    For Each folderNav As XPathNavigator In rootNav.Select("folders/folder")
                        Dim modulewriter As New ModulePackageWriter(folderNav, InstallerInfo)
                        modulewriter.WriteManifest(writer, True)
                    Next

                    'Write manifest end element
                    PackageWriterBase.WriteManifestEndElement(writer)

                    'Close XmlWriter
                    writer.Close()

                    'Load manifest into XPathDocument for processing
                    Dim legacyDoc As New XPathDocument(New StringReader(sb.ToString()))

                    'Parse the package nodes
                    ProcessPackages(legacyDoc.CreateNavigator().SelectSingleNode("dotnetnuke"))
                Case "languagepack"
                    InstallerInfo.IsLegacyMode = True
                    'Legacy Language Pack
                    Dim languageWriter As New LanguagePackWriter(rootNav, InstallerInfo)
                    LegacyError = languageWriter.LegacyError

                    If String.IsNullOrEmpty(LegacyError) Then
                        Dim legacyManifest As String = languageWriter.WriteManifest(False)
                        Dim legacyDoc As New XPathDocument(New StringReader(legacyManifest))

                        'Parse the package nodes
                        ProcessPackages(legacyDoc.CreateNavigator().SelectSingleNode("dotnetnuke"))
                    End If
                Case "skinobject"
                    InstallerInfo.IsLegacyMode = True
                    'Legacy Skin Object
                    Dim skinControlwriter As New SkinControlPackageWriter(rootNav, InstallerInfo)
                    Dim legacyManifest As String = skinControlwriter.WriteManifest(False)
                    Dim legacyDoc As New XPathDocument(New StringReader(legacyManifest))

                    'Parse the package nodes
                    ProcessPackages(legacyDoc.CreateNavigator().SelectSingleNode("dotnetnuke"))
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstallPackages method uninstalls the packages
        ''' </summary>
        ''' <param name="deleteFiles">A flag that indicates whether the files should be
        ''' deleted</param>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        '''     [cnurse]    01/31/2008  added deleteFiles parameter
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UnInstallPackages(ByVal deleteFiles As Boolean)
            'Iterate through all the Packages
            For index As Integer = 0 To Packages.Count - 1
                Dim installer As PackageInstaller = Packages.Values(index)
                InstallerInfo.Log.AddInfo(Util.UNINSTALL_Start + " - " + installer.Package.Name)
                installer.DeleteFiles = deleteFiles
                installer.UnInstall()
                If InstallerInfo.Log.HasWarnings Then
                    InstallerInfo.Log.AddWarning(Util.UNINSTALL_Warnings + " - " + installer.Package.Name)
                Else
                    InstallerInfo.Log.AddInfo(Util.UNINSTALL_Success + " - " + installer.Package.Name)
                End If
            Next
        End Sub

#End Region

#Region "Public Methods"

        Public Sub DeleteTempFolder()
            If Not String.IsNullOrEmpty(TempInstallFolder) Then
                Directory.Delete(TempInstallFolder, True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Install method installs the feature.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function Install() As Boolean
            InstallerInfo.Log.StartJob(Util.INSTALL_Start)
            Dim bStatus As Boolean = True
            Try
                InstallPackages()
            Catch ex As Exception
                InstallerInfo.Log.AddFailure(ex)
                bStatus = False
            Finally
                'Delete Temp Folder
                If Not String.IsNullOrEmpty(TempInstallFolder) Then
                    DeleteFolderRecursive(TempInstallFolder)
                End If
                InstallerInfo.Log.AddInfo(Util.FOLDER_DeletedBackup)
            End Try

            If InstallerInfo.Log.Valid Then
                InstallerInfo.Log.EndJob(Util.INSTALL_Success)
            Else
                InstallerInfo.Log.EndJob(Util.INSTALL_Failed)
                bStatus = False
            End If

            ' log installation event
            LogInstallEvent("Package", "Install")

            'Clear Host Cache
            DataCache.ClearHostCache(True)

            Return bStatus
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadManifest method reads the manifest file and parses it into packages.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ReadManifest(ByVal deleteTemp As Boolean)
            InstallerInfo.Log.StartJob(Util.DNN_Reading)

            If InstallerInfo.ManifestFile IsNot Nothing Then
                ReadManifest(New FileStream(InstallerInfo.ManifestFile.TempFileName, FileMode.Open, FileAccess.Read))
            End If

            If InstallerInfo.Log.Valid Then
                InstallerInfo.Log.EndJob(Util.DNN_Success)
            Else
                If deleteTemp Then
                    'Delete Temp Folder
                    DeleteTempFolder()
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstall method uninstalls the feature
        ''' </summary>
        ''' <param name="deleteFiles">A flag that indicates whether the files should be
        ''' deleted</param>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        '''     [cnurse]    01/31/2008  added deleteFiles parameter
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function UnInstall(ByVal deleteFiles As Boolean) As Boolean
            InstallerInfo.Log.StartJob(Util.UNINSTALL_Start)
            Try
                UnInstallPackages(deleteFiles)
            Catch ex As Exception
                InstallerInfo.Log.AddFailure(ex)
                Return False
            End Try

            If InstallerInfo.Log.HasWarnings Then
                InstallerInfo.Log.EndJob(Util.UNINSTALL_Warnings)
            Else
                InstallerInfo.Log.EndJob(Util.UNINSTALL_Success)
            End If

            ' log installation event
            LogInstallEvent("Package", "UnInstall")

            Return True
        End Function

#End Region

    End Class

End Namespace
