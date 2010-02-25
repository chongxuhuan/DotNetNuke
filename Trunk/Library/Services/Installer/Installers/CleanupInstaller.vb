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

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The CleanupInstaller cleans up (removes) files from previous versions
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	09/05/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CleanupInstaller
        Inherits FileInstaller

#Region "Private Members"

        Private _FileName As String

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
                Return "*"
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Function ProcessCleanupFile() As Boolean
            Log.AddInfo(String.Format(Util.CLEANUP_Processing, Version.ToString(3)))
            Dim bSuccess As Boolean = True
            Try
                Dim strListFile As String = Path.Combine(Me.Package.InstallerInfo.TempInstallFolder, _FileName)

                If File.Exists(strListFile) Then
                    FileSystemUtils.DeleteFiles(FileSystemUtils.ReadFile(strListFile).Split(ControlChars.CrLf.ToCharArray()))
                End If

                Log.AddInfo(String.Format(Util.CLEANUP_ProcessComplete, Version.ToString(3)))
            Catch ex As Exception
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message)
                bSuccess = False
            End Try
            Return bSuccess
        End Function

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The CleanupFile method cleansup a single file.
        ''' </summary>
        ''' <param name="insFile">The InstallFile to clean up</param>
        ''' <history>
        ''' 	[cnurse]	09/05/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function CleanupFile(ByVal insFile As InstallFile) As Boolean
            Try
                'Backup File
                If File.Exists(PhysicalBasePath + insFile.FullName) Then
                    Util.BackupFile(insFile, PhysicalBasePath, Log)
                End If

                'Delete file
                Util.DeleteFile(insFile, PhysicalBasePath, Log)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ProcessFile method determines what to do with parsed "file" node
        ''' </summary>
        ''' <param name="file">The file represented by the node</param>
        ''' <param name="nav">The XPathNavigator representing the node</param>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub ProcessFile(ByVal file As InstallFile, ByVal nav As System.Xml.XPath.XPathNavigator)
            If file IsNot Nothing Then
                Files.Add(file)
            End If
        End Sub

        Protected Overrides Function ReadManifestItem(ByVal nav As System.Xml.XPath.XPathNavigator, ByVal checkFileExists As Boolean) As InstallFile
            Return MyBase.ReadManifestItem(nav, False)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The RollbackFile method rolls back the cleanup of a single file.
        ''' </summary>
        ''' <param name="installFile">The InstallFile to commit</param>
        ''' <history>
        ''' 	[cnurse]	09/05/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RollbackFile(ByVal installFile As InstallFile)
            'Check for Backups
            If File.Exists(installFile.BackupFileName) Then
                Util.RestoreFile(installFile, PhysicalBasePath, Log)
            End If
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Commit method finalises the Install and commits any pending changes.
        ''' </summary>
        ''' <remarks>In the case of Clenup this is not neccessary</remarks>
        ''' <history>
        ''' 	[cnurse]	09/05/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Commit()
            'Do nothing
            MyBase.Commit()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Install method cleansup the files
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	09/05/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Install()
            Try
                Dim bSuccess As Boolean = True
                If String.IsNullOrEmpty(_FileName) Then
                    For Each file As InstallFile In Files
                        bSuccess = CleanupFile(file)
                        If Not bSuccess Then
                            Exit For
                        End If
                    Next
                Else
                    bSuccess = ProcessCleanupFile()
                End If
                Completed = bSuccess
            Catch ex As Exception
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message)
            End Try
        End Sub

        Public Overrides Sub ReadManifest(ByVal manifestNav As System.Xml.XPath.XPathNavigator)
            _FileName = Util.ReadAttribute(manifestNav, "fileName")

            MyBase.ReadManifest(manifestNav)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstall method uninstalls the file component
        ''' </summary>
        ''' <remarks>There is no uninstall for this component</remarks>
        ''' <history>
        ''' 	[cnurse]	09/05/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UnInstall()
        End Sub

#End Region

    End Class

End Namespace
