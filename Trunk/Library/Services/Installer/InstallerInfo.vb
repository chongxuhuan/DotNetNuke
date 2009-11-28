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

Imports System.Collections.Generic
Imports System.IO
Imports System.Xml.XPath

Imports ICSharpCode.SharpZipLib.Zip
Imports DotNetNuke.Services.Installer.Log
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Services.Installer.Installers

Namespace DotNetNuke.Services.Installer

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The InstallerInfo class holds all the information associated with a
    ''' Installation.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/24/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class InstallerInfo

#Region "Private Members"

        Private _AllowableFiles As String
        Private _Files As New Dictionary(Of String, InstallFile)
        Private _IgnoreWhiteList As Boolean = Null.NullBoolean
        Private _Installed As Boolean = Null.NullBoolean
        Private _InstallMode As InstallMode = InstallMode.Install
        Private _IsLegacyMode As Boolean = Null.NullBoolean
        Private _Log As New Logger
        Private _ManifestFile As InstallFile
        Private _PackageID As Integer = Null.NullInteger
        Private _PhysicalSitePath As String = Null.NullString
        Private _PortalID As Integer = Null.NullInteger
        Private _RepairInstall As Boolean = Null.NullBoolean
        Private _SecurityAccessLevel As SecurityAccessLevel = SecurityAccessLevel.Host
        Private _TempInstallFolder As String = Null.NullString

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new InstallerInfo instance
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/26/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new InstallerInfo instance from a 
        ''' string representing the physical path to the root of the site
        ''' </summary>
        ''' <param name="sitePath">The physical path to the root of the site</param>
        ''' <history>
        ''' 	[cnurse]	02/29/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal sitePath As String, ByVal mode As InstallMode)
            _PhysicalSitePath = sitePath
            _InstallMode = mode
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new InstallerInfo instance from a Stream and a
        ''' string representing the physical path to the root of the site
        ''' </summary>
        ''' <param name="inputStream">The Stream to use to create this InstallerInfo instance</param>
        ''' <param name="sitePath">The physical path to the root of the site</param>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal inputStream As Stream, ByVal sitePath As String)
            _TempInstallFolder = DotNetNuke.Common.InstallMapPath + "Temp\" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName)
            _PhysicalSitePath = sitePath
            _InstallMode = InstallMode.Install

            'Read the Zip file into its component entries
            ReadZipStream(inputStream, False)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new InstallerInfo instance from a string representing
        ''' the physical path to the temporary install folder and a string representing 
        ''' the physical path to the root of the site
        ''' </summary>
        ''' <param name="tempFolder">The physical path to the zip file containg the package</param>
        ''' <param name="manifest">The manifest filename</param>
        ''' <param name="sitePath">The physical path to the root of the site</param>
        ''' <history>
        ''' 	[cnurse]	08/13/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal tempFolder As String, ByVal manifest As String, ByVal sitePath As String)
            _TempInstallFolder = tempFolder
            _PhysicalSitePath = sitePath
            _InstallMode = InstallMode.Install

            If Not String.IsNullOrEmpty(manifest) Then
                _ManifestFile = New InstallFile(manifest, Me)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new InstallerInfo instance from a PackageInfo object
        ''' </summary>
        ''' <param name="package">The PackageInfo instance</param>
        ''' <param name="sitePath">The physical path to the root of the site</param>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal package As PackageInfo, ByVal sitePath As String)
            _PhysicalSitePath = sitePath
            _TempInstallFolder = DotNetNuke.Common.InstallMapPath + "Temp\" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName)
            _InstallMode = InstallMode.UnInstall

            package.AttachInstallerInfo(Me)
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a list of allowable file extensions (in addition to the Host's List)
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AllowableFiles() As String
            Get
                Return _AllowableFiles
            End Get
            Set(ByVal value As String)
                _AllowableFiles = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Dictionary of Files that are included in the Package
        ''' </summary>
        ''' <value>A Dictionary(Of String, InstallFile)</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Files() As Dictionary(Of String, InstallFile)
            Get
                Return _Files
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the package contains Valid Files
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	09/24/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property HasValidFiles() As Boolean
            Get
                Dim _HasValidFiles As Boolean = True
                For Each file As InstallFile In Files.Values
                    If Not Util.IsFileValid(file, AllowableFiles) Then
                        _HasValidFiles = Null.NullBoolean
                        Exit For
                    End If
                Next
                Return _HasValidFiles
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Package is installed
        ''' </summary>
        ''' <value>A Boolean value</value>
        ''' <history>
        ''' 	[cnurse]	09/24/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Installed() As Boolean
            Get
                Return _Installed
            End Get
            Set(ByVal value As Boolean)
                _Installed = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the InstallMode
        ''' </summary>
        ''' <value>A InstallMode value</value>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property InstallMode() As InstallMode
            Get
                Return _InstallMode
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Invalid File Extensions
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/12/2009  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property InvalidFileExtensions() As String
            Get
                Dim _InvalidFileExtensions As String = Null.NullString
                For Each file As InstallFile In Files.Values
                    If Not Util.IsFileValid(file, AllowableFiles) Then
                        _InvalidFileExtensions += ", " + file.Extension
                    End If
                Next
                If Not String.IsNullOrEmpty(_InvalidFileExtensions) Then
                    _InvalidFileExtensions = _InvalidFileExtensions.Substring(2)
                End If
                Return _InvalidFileExtensions
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the File Extension WhiteList is ignored
        ''' </summary>
        ''' <value>A Boolean value</value>
        ''' <history>
        ''' 	[cnurse]	05/06/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IgnoreWhiteList() As Boolean
            Get
                Return _IgnoreWhiteList
            End Get
            Set(ByVal value As Boolean)
                _IgnoreWhiteList = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Installer is in legacy mode
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/20/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsLegacyMode() As Boolean
            Get
                Return _IsLegacyMode
            End Get
            Set(ByVal value As Boolean)
                _IsLegacyMode = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the InstallerInfo instance is Valid
        ''' </summary>
        ''' <value>A Boolean value</value>
        ''' <history>
        ''' 	[cnurse]	08/13/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsValid() As Boolean
            Get
                Return Log.Valid
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the associated Logger
        ''' </summary>
        ''' <value>A Logger</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Log() As Logger
            Get
                Return _Log
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the Manifest File for the Package
        ''' </summary>
        ''' <value>An InstallFile</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ManifestFile() As InstallFile
            Get
                Return _ManifestFile
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Id of the package after installation (-1 if fail)
        ''' </summary>
        ''' <value>An Integer</value>
        ''' <history>
        ''' 	[cnurse]	08/22/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PackageID() As Integer
            Get
                Return _PackageID
            End Get
            Set(ByVal value As Integer)
                _PackageID = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Physical Path to the root of the Site (eg D:\Websites\DotNetNuke")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property PhysicalSitePath() As String
            Get
                Return _PhysicalSitePath
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Id of the current portal (-1 if Host)
        ''' </summary>
        ''' <value>An Integer</value>
        ''' <history>
        ''' 	[cnurse]	08/22/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal value As Integer)
                _PortalID = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Package Install is being repaird
        ''' </summary>
        ''' <value>A Boolean value</value>
        ''' <history>
        ''' 	[cnurse]	09/24/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property RepairInstall() As Boolean
            Get
                Return _RepairInstall
            End Get
            Set(ByVal value As Boolean)
                _RepairInstall = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the security Access Level of the user that is calling the INstaller
        ''' </summary>
        ''' <value>A SecurityAccessLevel enumeration</value>
        ''' <history>
        ''' 	[cnurse]	08/22/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SecurityAccessLevel() As SecurityAccessLevel
            Get
                Return _SecurityAccessLevel
            End Get
            Set(ByVal value As SecurityAccessLevel)
                _SecurityAccessLevel = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Temporary Install Folder used to unzip the archive (and to place the 
        ''' backups of existing files) during InstallMode
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/01/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property TempInstallFolder() As String
            Get
                Return _TempInstallFolder
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadZipStream reads a zip stream, and loads the Files Dictionary
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ReadZipStream(ByVal inputStream As Stream, ByVal isEmbeddedZip As Boolean)
            Log.StartJob(Util.FILES_Reading)

            Dim unzip As New ZipInputStream(inputStream)
            Dim entry As ZipEntry = unzip.GetNextEntry()

            While Not (entry Is Nothing)
                If Not entry.IsDirectory Then
                    ' Add file to list
                    Dim file As New InstallFile(unzip, entry, Me)

                    If file.Type = InstallFileType.Resources AndAlso (file.Name.ToLowerInvariant() = "containers.zip" OrElse file.Name.ToLowerInvariant() = "skins.zip") Then
                        'Temporarily save the TempInstallFolder
                        Dim tmpInstallFolder As String = TempInstallFolder

                        'Create Zip Stream from File
                        Dim zipStream As New FileStream(file.TempFileName, FileMode.Open, FileAccess.Read)

                        'Set TempInstallFolder
                        _TempInstallFolder = Path.Combine(TempInstallFolder, Path.GetFileNameWithoutExtension(file.Name))

                        'Extract files from zip
                        ReadZipStream(zipStream, True)

                        'Restore TempInstallFolder
                        _TempInstallFolder = tmpInstallFolder

                        'Delete zip file
                        Dim zipFile As New FileInfo(file.TempFileName)
                        zipFile.Delete()
                    Else
                        Files(file.FullName.ToLower) = file

                        If file.Type = InstallFileType.Manifest AndAlso Not isEmbeddedZip Then
                            If ManifestFile Is Nothing Then
                                _ManifestFile = file
                            Else
                                If ManifestFile.Extension = "dnn" AndAlso file.Extension = "dnn5" Then
                                    _ManifestFile = file
                                Else
                                    Log.AddFailure((Util.EXCEPTION_MultipleDnn + ManifestFile.Name + " and " + file.Name))
                                End If
                            End If
                        End If
                    End If
                    Log.AddInfo(String.Format(Util.FILE_ReadSuccess, file.FullName))
                End If
                entry = unzip.GetNextEntry
            End While

            If ManifestFile Is Nothing Then
                Log.AddFailure(Util.EXCEPTION_MissingDnn)
            End If

            If Log.Valid Then
                Log.EndJob(Util.FILES_ReadingEnd)
            Else
                Log.AddFailure(New Exception(Util.EXCEPTION_FileLoad))
                Log.EndJob(Util.FILES_ReadingEnd)
            End If

            'Close the Zip Input Stream as we have finished with it
            inputStream.Close()
        End Sub

#End Region

    End Class

End Namespace

