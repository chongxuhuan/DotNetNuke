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
Imports System.Xml
Imports System.Xml.XPath

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ConfigInstaller installs Config changes
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	08/03/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ConfigInstaller
        Inherits ComponentInstallerBase

#Region "Private Members"

        Private _FileName As String = Null.NullString
        Private _InstallConfig As String = Null.NullString
        Private _TargetConfig As XmlDocument
        Private _TargetFile As InstallFile
        Private _UnInstallConfig As String = Null.NullString
        Private _UninstallFileName As String = Null.NullString

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Install config changes
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property InstallConfig() As String
            Get
                Return _InstallConfig
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Target Config XmlDocument
        ''' </summary>
        ''' <value>An XmlDocument</value>
        ''' <history>
        ''' 	[cnurse]	08/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property TargetConfig() As XmlDocument
            Get
                Return _TargetConfig
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Target Config file to change
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property TargetFile() As InstallFile
            Get
                Return _TargetFile
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the UnInstall config changes
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property UnInstallConfig() As String
            Get
                Return _UnInstallConfig
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Commit method finalises the Install and commits any pending changes.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Commit()
            Try
                'Save the XmlDocument
                Config.Save(TargetConfig, TargetFile.FullName)
                Log.AddInfo(Util.CONFIG_Committed + " - " + TargetFile.Name)
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Install method installs the config component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Install()
            Try
                If String.IsNullOrEmpty(_FileName) Then
                    'First backup the config file
                    Util.BackupFile(TargetFile, PhysicalSitePath, Log)

                    'Create an XmlDocument for the config file
                    _TargetConfig = New XmlDocument()
                    TargetConfig.Load(Path.Combine(PhysicalSitePath, TargetFile.FullName))

                    'Create XmlMerge instance from InstallConfig source
                    Dim merge As XmlMerge = New XmlMerge(New StringReader(InstallConfig), Me.Package.Version.ToString(), Me.Package.Name)

                    'Update the Config file - Note that this method does not save the file - we will save it in Commit
                    merge.UpdateConfig(TargetConfig)
                    Completed = True
                    Log.AddInfo(Util.CONFIG_Updated + " - " + TargetFile.Name)
                Else
                    'Process external file
                    Dim strConfigFile As String = Path.Combine(Me.Package.InstallerInfo.TempInstallFolder, _FileName)
                    If File.Exists(strConfigFile) Then
                        'Create XmlMerge instance from config file source
                        Dim stream As StreamReader = File.OpenText(strConfigFile)
                        Dim merge As XmlMerge = New XmlMerge(stream, Package.Version.ToString(3), Package.Name & " Install")

                        'Process merge
                        merge.UpdateConfigs()

                        'Close stream
                        stream.Close()

                        Completed = True
                        Log.AddInfo(Util.CONFIG_Updated)
                    End If
                End If
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadManifest method reads the manifest file for the config compoent.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub ReadManifest(ByVal manifestNav As XPathNavigator)
            _FileName = Util.ReadAttribute(manifestNav, "fileName")
            _UninstallFileName = Util.ReadAttribute(manifestNav, "unInstallFileName")

            If String.IsNullOrEmpty(_FileName) Then
                Dim nav As XPathNavigator = manifestNav.SelectSingleNode("config")

                'Get the name of the target config file to update
                Dim nodeNav As XPathNavigator = nav.SelectSingleNode("configFile")
                Dim targetFileName As String = nodeNav.Value
                If Not String.IsNullOrEmpty(targetFileName) Then
                    _TargetFile = New InstallFile(targetFileName, "", Me.Package.InstallerInfo)
                End If

                'Get the Install config changes
                nodeNav = nav.SelectSingleNode("install")
                _InstallConfig = nodeNav.InnerXml

                'Get the UnInstall config changes
                nodeNav = nav.SelectSingleNode("uninstall")
                _UnInstallConfig = nodeNav.InnerXml
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Rollback method undoes the installation of the file component in the event 
        ''' that one of the other components fails
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Rollback()
            'Do nothing as the changes are all in memory
            Log.AddInfo(Util.CONFIG_RolledBack + " - " + TargetFile.Name)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstall method uninstalls the config component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UnInstall()
            If String.IsNullOrEmpty(_UninstallFileName) Then
                'Create an XmlDocument for the config file
                _TargetConfig = New XmlDocument()
                TargetConfig.Load(Path.Combine(PhysicalSitePath, TargetFile.FullName))

                'Create XmlMerge instance from UnInstallConfig source
                Dim merge As XmlMerge = New XmlMerge(New StringReader(UnInstallConfig), Me.Package.Version.ToString(), Me.Package.Name)

                'Update the Config file - Note that this method does save the file
                merge.UpdateConfig(TargetConfig, TargetFile.FullName)
            Else
                'Process external file
                Dim strConfigFile As String = Path.Combine(Me.Package.InstallerInfo.TempInstallFolder, _UninstallFileName)
                If File.Exists(strConfigFile) Then
                    'Create XmlMerge instance from config file source
                    Dim stream As StreamReader = File.OpenText(strConfigFile)
                    Dim merge As XmlMerge = New XmlMerge(stream, Package.Version.ToString(3), Package.Name & " UnInstall")

                    'Process merge
                    merge.UpdateConfigs()

                    'Close stream
                    stream.Close()
                End If
            End If
        End Sub

#End Region

    End Class

End Namespace
