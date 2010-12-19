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

Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Xml.XPath
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Xml

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ResourceFileInstaller installs Resource File Components (zips) to a DotNetNuke site
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/18/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ResourceFileInstaller
        Inherits FileInstaller

#Region "Private Members"

        Private _Manifest As String

#End Region

#Region "Public Contants"

        Public Const DEFAULT_MANIFESTEXT As String = ".manifest"

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Collection Node ("resourceFiles")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/18/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property CollectionNodeName() As String
            Get
                Return "resourceFiles"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Item Node ("resourceFile")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/18/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ItemNodeName() As String
            Get
                Return "resourceFile"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Manifest
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/18/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property Manifest() As String
            Get
                Return _Manifest
            End Get
        End Property


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
                Return "resources, zip"
            End Get
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The CommitFile method commits a single file.
        ''' </summary>
        ''' <param name="insFile">The InstallFile to commit</param>
        ''' <history>
        ''' 	[cnurse]	01/18/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub CommitFile(ByVal insFile As InstallFile)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DeleteFile method deletes a single assembly.
        ''' </summary>
        ''' <param name="file">The InstallFile to delete</param>
        ''' <history>
        ''' 	[cnurse]	01/18/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub DeleteFile(ByVal file As InstallFile)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The InstallFile method installs a single assembly.
        ''' </summary>
        ''' <param name="insFile">The InstallFile to install</param>
        ''' <history>
        ''' 	[cnurse]	01/18/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function InstallFile(ByVal insFile As InstallFile) As Boolean
            Dim fs As FileStream = Nothing
            Dim unzip As ZipInputStream = Nothing
            Dim writer As XmlWriter = Nothing
            Dim retValue As Boolean = True

            Try
                Log.AddInfo(Util.FILES_Expanding)
                unzip = New ZipInputStream(New FileStream(insFile.TempFileName, FileMode.Open))

                'Create a writer to create the manifest for the resource file
                If String.IsNullOrEmpty(Manifest) Then
                    _Manifest = insFile.Name & ".manifest"
                End If
                If Not Directory.Exists(PhysicalBasePath) Then
                    Directory.CreateDirectory(PhysicalBasePath)
                End If
                fs = New FileStream(Path.Combine(PhysicalBasePath, Manifest), FileMode.Create, FileAccess.Write)
                Dim settings As New XmlWriterSettings()
                settings.ConformanceLevel = ConformanceLevel.Fragment
                settings.OmitXmlDeclaration = True
                settings.Indent = True

                writer = XmlWriter.Create(fs, settings)

                'Start the new Root Element
                writer.WriteStartElement("dotnetnuke")
                writer.WriteAttributeString("type", "ResourceFile")
                writer.WriteAttributeString("version", "5.0")

                'Start files Element
                writer.WriteStartElement("files")

                Dim entry As ZipEntry = unzip.GetNextEntry()
                While Not (entry Is Nothing)
                    If Not entry.IsDirectory Then
                        Dim fileName As String = Path.GetFileName(entry.Name)

                        'Start file Element
                        writer.WriteStartElement("file")

                        'Write path
                        writer.WriteElementString("path", entry.Name.Substring(0, entry.Name.IndexOf(fileName)))

                        'Write name
                        writer.WriteElementString("name", fileName)

                        Dim physicalPath As String = Path.Combine(PhysicalBasePath, entry.Name)
                        If File.Exists(physicalPath) Then
                            Util.BackupFile(New InstallFile(entry.Name, Me.Package.InstallerInfo), PhysicalBasePath, Log)
                        End If

                        Util.WriteStream(unzip, physicalPath)

                        'Close files Element
                        writer.WriteEndElement()

                        Log.AddInfo(String.Format(Util.FILE_Created, entry.Name))
                    End If
                    entry = unzip.GetNextEntry
                End While

                'Close files Element
                writer.WriteEndElement()

                Log.AddInfo(Util.FILES_CreatedResources)
            Catch ex As Exception
                retValue = False
            Finally
                'Close XmlWriter
                If writer IsNot Nothing Then writer.Close()

                'Close FileStreams
                If fs IsNot Nothing Then fs.Close()
                If unzip IsNot Nothing Then unzip.Close()
            End Try
            Return retValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a flag that determines what type of file this installer supports
        ''' </summary>
        ''' <param name="type">The type of file being processed</param>
        ''' <history>
        ''' 	[cnurse]	01/18/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function IsCorrectType(ByVal type As InstallFileType) As Boolean
            Return (type = InstallFileType.Resources)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadManifestItem method reads a single node
        ''' </summary>
        ''' <param name="nav">The XPathNavigator representing the node</param>
        ''' <param name="checkFileExists">Flag that determines whether a check should be made</param>
        ''' <history>
        ''' 	[cnurse]	01/18/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function ReadManifestItem(ByVal nav As System.Xml.XPath.XPathNavigator, ByVal checkFileExists As Boolean) As InstallFile

            Dim insFile As InstallFile = MyBase.ReadManifestItem(nav, checkFileExists)

            _Manifest = Util.ReadElement(nav, "manifest")

            If String.IsNullOrEmpty(_Manifest) Then
                _Manifest = insFile.FullName + DEFAULT_MANIFESTEXT
            End If

            'Call base method
            Return MyBase.ReadManifestItem(nav, checkFileExists)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The RollbackFile method rolls back the install of a single file.
        ''' </summary>
        ''' <remarks>For new installs this removes the added file.  For upgrades it restores the
        ''' backup file created during install</remarks>
        ''' <param name="insFile">The InstallFile to commit</param>
        ''' <history>
        ''' 	[cnurse]	01/18/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RollbackFile(ByVal insFile As InstallFile)
            Dim unzip As New ZipInputStream(New FileStream(insFile.InstallerInfo.TempInstallFolder + insFile.FullName, FileMode.Open))
            Dim entry As ZipEntry = unzip.GetNextEntry()
            While Not (entry Is Nothing)
                If Not entry.IsDirectory Then
                    'Check for Backups
                    If File.Exists(insFile.BackupPath + entry.Name) Then
                        'Restore File
                        Util.RestoreFile(New InstallFile(unzip, entry, Me.Package.InstallerInfo), PhysicalBasePath, Log)
                    Else
                        'Delete File
                        Util.DeleteFile(entry.Name, PhysicalBasePath, Log)
                    End If
                End If
                entry = unzip.GetNextEntry
            End While
        End Sub

        Protected Overrides Sub UnInstallFile(ByVal unInstallFile As InstallFile)
            _Manifest = unInstallFile.Name + ".manifest"
            Dim doc As New XPathDocument(Path.Combine(PhysicalBasePath, Manifest))

            For Each fileNavigator As XPathNavigator In doc.CreateNavigator().Select("dotnetnuke/files/file")
                Dim path As String = XmlUtils.GetNodeValue(fileNavigator, "path")
                Dim fileName As String = XmlUtils.GetNodeValue(fileNavigator, "name")
                Dim filePath As String = System.IO.Path.Combine(path, fileName)

                Try
                    If DeleteFiles = True Then
                        Util.DeleteFile(filePath, PhysicalBasePath, Log)
                    End If
                Catch ex As Exception
                    Log.AddFailure(ex)
                End Try
            Next
            If DeleteFiles = True Then
                Util.DeleteFile(Manifest, PhysicalBasePath, Log)
            End If
        End Sub

#End Region

    End Class

End Namespace
