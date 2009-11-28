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
Imports System.Xml.XPath

Imports DotNetNuke.Services.Installer.Log
Imports System.Xml
Imports DotNetNuke.Services.Installer.Packages
Imports System.Collections.Generic
Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Text
Imports System.Text.RegularExpressions


Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PackageWriter class 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/30/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PackageWriterBase

#Region "Private Members"

        Private _AppCodeFiles As New Dictionary(Of String, InstallFile)
        Private _AppCodePath As String
        Private _Assemblies As New Dictionary(Of String, InstallFile)
        Private _AssemblyPath As String
        Private _BasePath As String = Null.NullString
        Private _CleanUpFiles As New SortedList(Of String, InstallFile)
        Private _Files As New Dictionary(Of String, InstallFile)
        Private _HasProjectFile As Boolean
        Private _LegacyError As String
        Private _Package As PackageInfo
        Private _Resources As New Dictionary(Of String, InstallFile)
        Private _Scripts As New Dictionary(Of String, InstallFile)
        Private _Versions As New List(Of String)


#End Region

#Region "Constructors"

        Protected Sub New()

        End Sub

        Public Sub New(ByVal package As PackageInfo)
            _Package = package
            _Package.AttachInstallerInfo(New InstallerInfo)
        End Sub

#End Region

#Region "Protected Properties"

        Protected Overridable ReadOnly Property Dependencies() As Dictionary(Of String, String)
            Get
                Return New Dictionary(Of String, String)
            End Get
        End Property
#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Dictionary of AppCodeFiles that should be included in the Package
        ''' </summary>
        ''' <value>A Dictionary(Of String, InstallFile)</value>
        ''' <history>
        ''' 	[cnurse]	02/12/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property AppCodeFiles() As Dictionary(Of String, InstallFile)
            Get
                Return _AppCodeFiles
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Path for the Package's app code files
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/12/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AppCodePath() As String
            Get
                Return _AppCodePath
            End Get
            Set(ByVal value As String)
                _AppCodePath = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Dictionary of Assemblies that should be included in the Package
        ''' </summary>
        ''' <value>A Dictionary(Of String, InstallFile)</value>
        ''' <history>
        ''' 	[cnurse]	01/31/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Assemblies() As Dictionary(Of String, InstallFile)
            Get
                Return _Assemblies
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Path for the Package's assemblies
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/31/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AssemblyPath() As String
            Get
                Return _AssemblyPath
            End Get
            Set(ByVal value As String)
                _AssemblyPath = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Base Path for the Package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/31/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property BasePath() As String
            Get
                Return _BasePath
            End Get
            Set(ByVal value As String)
                _BasePath = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Dictionary of CleanUpFiles that should be included in the Package
        ''' </summary>
        ''' <value>A Dictionary(Of String, InstallFile)</value>
        ''' <history>
        ''' 	[cnurse]	02/21/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property CleanUpFiles() As SortedList(Of String, InstallFile)
            Get
                Return _CleanUpFiles
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Dictionary of Files that should be included in the Package
        ''' </summary>
        ''' <value>A Dictionary(Of String, InstallFile)</value>
        ''' <history>
        ''' 	[cnurse]	01/31/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Files() As Dictionary(Of String, InstallFile)
            Get
                Return _Files
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether a project file is found in the folder
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/31/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property HasProjectFile() As Boolean
            Get
                Return _HasProjectFile
            End Get
            Set(ByVal value As Boolean)
                _HasProjectFile = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to include Assemblies
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/06/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable ReadOnly Property IncludeAssemblies() As Boolean
            Get
                Return True
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
        ''' Gets the Logger
        ''' </summary>
        ''' <value>An Logger object</value>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Log() As Logger
            Get
                Return Package.Log
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the associated Package
        ''' </summary>
        ''' <value>An PackageInfo object</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Package() As PackageInfo
            Get
                Return _Package
            End Get
            Set(ByVal value As PackageInfo)
                _Package = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Dictionary of Resources that should be included in the Package
        ''' </summary>
        ''' <value>A Dictionary(Of String, InstallFile)</value>
        ''' <history>
        ''' 	[cnurse]	02/11/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Resources() As Dictionary(Of String, InstallFile)
            Get
                Return _Resources
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Dictionary of Scripts that should be included in the Package
        ''' </summary>
        ''' <value>A Dictionary(Of String, InstallFile)</value>
        ''' <history>
        ''' 	[cnurse]	01/31/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Scripts() As Dictionary(Of String, InstallFile)
            Get
                Return _Scripts
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a List of Versions that should be included in the Package
        ''' </summary>
        ''' <value>A List(Of String)</value>
        ''' <history>
        ''' 	[cnurse]	01/31/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Versions() As List(Of String)
            Get
                Return _Versions
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub AddFilesToZip(ByVal stream As ZipOutputStream, ByVal files As IDictionary(Of String, InstallFile), ByVal basePath As String)
            For Each packageFile As InstallFile In files.Values
                Dim filepath As String
                If String.IsNullOrEmpty(basePath) Then
                    filepath = Path.Combine(ApplicationMapPath, packageFile.FullName)
                Else
                    filepath = Path.Combine(Path.Combine(ApplicationMapPath, basePath), packageFile.FullName.Replace(basePath + "\", ""))
                End If

                If File.Exists(filepath) Then
                    Dim packageFilePath As String = packageFile.Path
                    If Not String.IsNullOrEmpty(basePath) Then
                        packageFilePath = packageFilePath.Replace(basePath + "\", "")
                    End If
                    FileSystemUtils.AddToZip(stream, filepath, packageFile.Name, packageFilePath)
                    Log.AddInfo(String.Format(Util.WRITER_SavedFile, packageFile.FullName))
                End If
            Next
        End Sub

        Private Sub CreateZipFile(ByVal zipFileName As String)
            Dim CompressionLevel As Integer = 9
            Dim zipFile As New FileInfo(zipFileName)

            Dim ZipFileShortName As String = zipFile.Name

            Dim strmZipFile As FileStream = Nothing
            Log.StartJob(Util.WRITER_CreatingPackage)

            Try
                Log.AddInfo(String.Format(Util.WRITER_CreateArchive, ZipFileShortName))
                strmZipFile = File.Create(zipFileName)
                Dim strmZipStream As ZipOutputStream = Nothing
                Try
                    strmZipStream = New ZipOutputStream(strmZipFile)
                    strmZipStream.SetLevel(CompressionLevel)

                    'Add Files To zip
                    AddFilesToZip(strmZipStream, _Assemblies, "")
                    AddFilesToZip(strmZipStream, _AppCodeFiles, AppCodePath)
                    AddFilesToZip(strmZipStream, _Files, BasePath)
                    AddFilesToZip(strmZipStream, _CleanUpFiles, BasePath)
                    AddFilesToZip(strmZipStream, _Resources, BasePath)
                    AddFilesToZip(strmZipStream, _Scripts, BasePath)
                Catch ex As Exception
                    LogException(ex)
                    Log.AddFailure(String.Format(Util.WRITER_SaveFileError, ex))
                Finally
                    If Not strmZipStream Is Nothing Then
                        strmZipStream.Finish()
                        strmZipStream.Close()
                    End If
                End Try
                Log.EndJob(Util.WRITER_CreatedPackage)
            Catch ex As Exception
                LogException(ex)
                Log.AddFailure(String.Format(Util.WRITER_SaveFileError, ex))
            Finally
                If Not strmZipFile Is Nothing Then
                    strmZipFile.Close()
                End If
            End Try
        End Sub

        Private Sub WritePackageEndElement(ByVal writer As XmlWriter)
            'Close components Element
            writer.WriteEndElement()

            'Close package Element
            writer.WriteEndElement()

        End Sub

        Private Sub WritePackageStartElement(ByVal writer As XmlWriter)
            'Start package Element
            writer.WriteStartElement("package")
            writer.WriteAttributeString("name", Package.Name)
            writer.WriteAttributeString("type", Package.PackageType)
            writer.WriteAttributeString("version", Package.Version.ToString(3))

            'Write FriendlyName
            writer.WriteElementString("friendlyName", Package.FriendlyName)

            'Write Description
            writer.WriteElementString("description", Package.Description)

            'Write Author
            writer.WriteStartElement("owner")

            writer.WriteElementString("name", Package.Owner)
            writer.WriteElementString("organization", Package.Organization)
            writer.WriteElementString("url", Package.Url)
            writer.WriteElementString("email", Package.Email)

            'Write Author End
            writer.WriteEndElement()

            'Write License
            writer.WriteElementString("license", Package.License)

            'Write Release Notes
            writer.WriteElementString("releaseNotes", Package.ReleaseNotes)

            'Write Dependencies
            If Dependencies.Count > 0 Then
                writer.WriteStartElement("dependencies")
                For Each kvp As KeyValuePair(Of String, String) In Dependencies
                    writer.WriteStartElement("dependency")
                    writer.WriteAttributeString("type", kvp.Key)
                    writer.WriteString(kvp.Value)
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
            End If

            'Write components Element
            writer.WriteStartElement("components")
        End Sub

#End Region

#Region "Protected Methods"

        Protected Overridable Sub AddFile(ByVal fileName As String)
            AddFile(New InstallFile(fileName, Package.InstallerInfo))
        End Sub

        Protected Overridable Sub AddFile(ByVal fileName As String, ByVal sourceFileName As String)
            AddFile(New InstallFile(fileName, sourceFileName, Package.InstallerInfo))
        End Sub

        Protected Overridable Sub ConvertLegacyManifest(ByVal legacyManifest As XPathNavigator, ByVal writer As XmlWriter)
        End Sub

        Protected Overridable Sub GetFiles(ByVal includeSource As Boolean, ByVal includeAppCode As Boolean)
            Dim baseFolder As String = Path.Combine(ApplicationMapPath, BasePath)

            If Directory.Exists(baseFolder) Then
                'Create the DirectoryInfo object
                Dim folderInfo As New DirectoryInfo(baseFolder)

                'Get the Project File in the folder
                Dim files As FileInfo() = folderInfo.GetFiles("*.??proj")

                If files.Length = 0 Then 'Assume Dynamic (App_Code based) Module
                    'Add the files in the DesktopModules Folder
                    ParseFolder(baseFolder, baseFolder)

                    'Add the files in the AppCode Folder
                    If includeAppCode Then
                        Dim appCodeFolder As String = Path.Combine(ApplicationMapPath, AppCodePath)
                        ParseFolder(appCodeFolder, appCodeFolder)
                    End If
                Else 'WAP Project File is present
                    HasProjectFile = True

                    'Parse the Project files (probably only one)
                    For Each projFile As FileInfo In files
                        ParseProjectFile(projFile, includeSource)
                    Next
                End If
            End If
        End Sub

        Protected Overridable Sub ParseFiles(ByVal folder As DirectoryInfo, ByVal rootPath As String)
            'Add the Files in the Folder
            Dim files As FileInfo() = folder.GetFiles()
            For Each file As FileInfo In files
                Dim filePath As String = folder.FullName.Replace(rootPath, "")
                If filePath.StartsWith("\") Then
                    filePath = filePath.Substring(1)
                End If
                If folder.FullName.ToLowerInvariant.Contains("app_code") Then
                    filePath = "[app_code]" + filePath
                End If
                If file.Extension.ToLowerInvariant() <> ".dnn" AndAlso (file.Attributes And FileAttributes.Hidden) = 0 Then
                    AddFile(Path.Combine(filePath, file.Name))
                End If
            Next
        End Sub

        Protected Overridable Sub ParseFolder(ByVal folderName As String, ByVal rootPath As String)

            If Directory.Exists(folderName) Then
                Dim folder As DirectoryInfo = New DirectoryInfo(folderName)

                'Recursively parse the subFolders
                Dim subFolders As DirectoryInfo() = folder.GetDirectories()
                For Each subFolder As DirectoryInfo In subFolders
                    If (subFolder.Attributes And FileAttributes.Hidden) = 0 Then
                        ParseFolder(subFolder.FullName, rootPath)
                    End If
                Next

                'Add the Files in the Folder
                ParseFiles(folder, rootPath)
            End If
        End Sub

        Protected Sub ParseProjectFile(ByVal projFile As FileInfo, ByVal includeSource As Boolean)
            Dim fileName As String = ""

            'Create an XPathDocument from the Xml
            Dim doc As New XPathDocument(New FileStream(projFile.FullName, FileMode.Open, FileAccess.Read))
            Dim rootNav As XPathNavigator = doc.CreateNavigator()
            Dim manager As XmlNamespaceManager = New XmlNamespaceManager(rootNav.NameTable)
            manager.AddNamespace("proj", "http://schemas.microsoft.com/developer/msbuild/2003")
            rootNav.MoveToFirstChild()

            Dim assemblyNav As XPathNavigator = rootNav.SelectSingleNode("proj:PropertyGroup/proj:AssemblyName", manager)
            fileName = assemblyNav.Value
            Dim buildPathNav As XPathNavigator = rootNav.SelectSingleNode("proj:PropertyGroup/proj:OutputPath", manager)
            Dim buildPath As String = buildPathNav.Value.Replace("..\", "")
            buildPath = buildPath.Replace(AssemblyPath & "\", "")
            AddFile(Path.Combine(buildPath, fileName & ".dll"))

            'Check for referenced assemblies
            For Each itemNav As XPathNavigator In rootNav.Select("proj:ItemGroup/proj:Reference", manager)
                fileName = Util.ReadAttribute(itemNav, "Include")
                If fileName.IndexOf(",") > -1 Then
                    fileName = fileName.Substring(0, fileName.IndexOf(","))
                End If
                If Not (fileName.ToLowerInvariant.StartsWith("system") OrElse fileName.ToLowerInvariant().StartsWith("microsoft") OrElse _
                        fileName.ToLowerInvariant = "dotnetnuke" OrElse fileName.ToLowerInvariant = "dotnetnuke.webutility" OrElse _
                        fileName.ToLowerInvariant = "dotnetnuke.webcontrols") Then
                    AddFile(fileName & ".dll")
                End If
            Next

            'Add all the files that are classified as None
            For Each itemNav As XPathNavigator In rootNav.Select("proj:ItemGroup/proj:None", manager)
                fileName = Util.ReadAttribute(itemNav, "Include")
                AddFile(fileName)
            Next

            'Add all the files that are classified as Content
            For Each itemNav As XPathNavigator In rootNav.Select("proj:ItemGroup/proj:Content", manager)
                fileName = Util.ReadAttribute(itemNav, "Include")
                AddFile(fileName)
            Next

            'Add all the files that are classified as Compile
            If includeSource Then
                For Each itemNav As XPathNavigator In rootNav.Select("proj:ItemGroup/proj:Compile", manager)
                    fileName = Util.ReadAttribute(itemNav, "Include")
                    AddFile(fileName)
                Next
            End If
        End Sub

        Protected Overridable Sub WriteFilesToManifest(ByVal writer As XmlWriter)
            Dim fileWriter As New FileComponentWriter(BasePath, Files, Package)
            fileWriter.WriteManifest(writer)
        End Sub

        Protected Overridable Sub WriteManifestComponent(ByVal writer As XmlWriter)
        End Sub

#End Region

#Region "Public Methods"

        Public Overridable Sub AddFile(ByVal file As InstallFile)
            Select Case file.Type
                Case InstallFileType.AppCode
                    _AppCodeFiles(file.FullName.ToLower) = file
                Case InstallFileType.Assembly
                    _Assemblies(file.FullName.ToLower) = file
                Case InstallFileType.CleanUp
                    _CleanUpFiles(file.FullName.ToLower) = file
                Case InstallFileType.Script
                    _Scripts(file.FullName.ToLower) = file
                Case Else
                    _Files(file.FullName.ToLower) = file
            End Select

            If (file.Type = InstallFileType.CleanUp OrElse file.Type = InstallFileType.Script) AndAlso Regex.IsMatch(file.Name, Util.REGEX_Version) Then
                Dim version As String = Path.GetFileNameWithoutExtension(file.Name)
                If Not _Versions.Contains(version) Then
                    _Versions.Add(version)
                End If
            End If
        End Sub

        Public Sub AddResourceFile(ByVal file As InstallFile)
            _Resources(file.FullName.ToLower) = file
        End Sub

        Public Sub CreatePackage(ByVal archiveName As String, ByVal manifestName As String, ByVal manifest As String, ByVal createManifest As Boolean)
            If createManifest Then
                WriteManifest(manifestName, manifest)
            End If
            AddFile(manifestName)
            CreateZipFile(archiveName)
        End Sub

        Public Sub GetFiles(ByVal includeSource As Boolean)
            'Call protected method that does the work
            GetFiles(includeSource, True)
        End Sub

        ''' <summary>
        ''' WriteManifest writes an existing manifest
        ''' </summary>
        ''' <param name="manifestName">The name of the manifest file</param>
        ''' <param name="manifest">The manifest</param>
        ''' <remarks>This overload takes a package manifest and writes it to a file</remarks>
        Public Sub WriteManifest(ByVal manifestName As String, ByVal manifest As String)
            Dim writer As XmlWriter = XmlWriter.Create(Path.Combine(ApplicationMapPath, Path.Combine(BasePath, manifestName)), XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment))

            Log.StartJob(Util.WRITER_CreatingManifest)
            WriteManifest(writer, manifest)
            Log.EndJob(Util.WRITER_CreatedManifest)
        End Sub

        ''' <summary>
        ''' WriteManifest writes a package manifest to an XmlWriter
        ''' </summary>
        ''' <param name="writer">The XmlWriter</param>
        ''' <param name="manifest">The manifest</param>
        ''' <remarks>This overload takes a package manifest and writes it to a Writer</remarks>
        Public Sub WriteManifest(ByVal writer As XmlWriter, ByVal manifest As String)
            WriteManifestStartElement(writer)

            'Inject Manifest String
            'Dim doc As XPathDocument = New XPathDocument(New StringReader(manifest))
            'Dim nav As XPathNavigator = doc.CreateNavigator()
            'nav.WriteSubtree(writer)
            writer.WriteRaw(manifest)

            'Close Dotnetnuke Element
            WriteManifestEndElement(writer)

            'Close Writer
            writer.Close()
        End Sub

        ''' <summary>
        ''' WriteManifest writes the manifest assoicated with this PackageWriter to a string
        ''' </summary>
        ''' <param name="packageFragment">A flag that indicates whether to return the package element
        ''' as a fragment (True) or whether to add the outer dotnetnuke and packages elements (False)</param>
        ''' <returns>The manifest as a string</returns>
        ''' <remarks></remarks>
        Public Function WriteManifest(ByVal packageFragment As Boolean) As String
            'Create a writer to create the processed manifest
            Dim sb As New StringBuilder
            Dim writer As XmlWriter = XmlWriter.Create(sb, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment))

            WriteManifest(writer, packageFragment)

            'Close XmlWriter
            writer.Close()

            'Return new manifest
            Return sb.ToString()

        End Function

        Public Sub WriteManifest(ByVal writer As XmlWriter, ByVal packageFragment As Boolean)
            Log.StartJob(Util.WRITER_CreatingManifest)

            If Not packageFragment Then
                'Start dotnetnuke element
                WriteManifestStartElement(writer)
            End If

            'Start package Element
            WritePackageStartElement(writer)

            'write Script Component
            If Scripts.Count > 0 Then
                Dim scriptWriter As New ScriptComponentWriter(BasePath, Scripts, Package)
                scriptWriter.WriteManifest(writer)
            End If

            'write Clean Up Files Component
            If CleanUpFiles.Count > 0 Then
                Dim cleanupFileWriter As New CleanupComponentWriter(BasePath, CleanUpFiles)
                cleanupFileWriter.WriteManifest(writer)
            End If

            'Write the Custom Component
            WriteManifestComponent(writer)

            'Write Assemblies Component
            If Assemblies.Count > 0 Then
                Dim assemblyWriter As New AssemblyComponentWriter(AssemblyPath, Assemblies, Package)
                assemblyWriter.WriteManifest(writer)
            End If

            'Write AppCode Files Component
            If AppCodeFiles.Count > 0 Then
                Dim fileWriter As New FileComponentWriter(AppCodePath, AppCodeFiles, Package)
                fileWriter.WriteManifest(writer)
            End If

            'write Files Component
            If Files.Count > 0 Then
                WriteFilesToManifest(writer)
            End If

            'write ResourceFiles Component
            If Resources.Count > 0 Then
                Dim fileWriter As New ResourceFileComponentWriter(BasePath, Resources, Package)
                fileWriter.WriteManifest(writer)
            End If

            'Close Package
            WritePackageEndElement(writer)

            If Not packageFragment Then
                'Close Dotnetnuke Element
                WriteManifestEndElement(writer)
            End If

            Log.EndJob(Util.WRITER_CreatedManifest)
        End Sub

        Public Shared Sub WriteManifestEndElement(ByVal writer As XmlWriter)
            'Close packages Element
            writer.WriteEndElement()

            'Close root Element
            writer.WriteEndElement()
        End Sub

        Public Shared Sub WriteManifestStartElement(ByVal writer As XmlWriter)
            'Start the new Root Element
            writer.WriteStartElement("dotnetnuke")
            writer.WriteAttributeString("type", "Package")
            writer.WriteAttributeString("version", "5.0")

            'Start packages Element
            writer.WriteStartElement("packages")
        End Sub

#End Region

    End Class

End Namespace

