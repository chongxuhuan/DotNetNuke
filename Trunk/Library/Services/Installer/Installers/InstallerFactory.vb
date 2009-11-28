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

Imports System.Xml.XPath

Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Services.Installer.Packages

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The InstallerFactory is a factory class that is used to instantiate the
    ''' appropriate Component Installer
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/24/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class InstallerFactory

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The GetInstaller method instantiates the relevant Component Installer
        ''' </summary>
        ''' <param name="installerType">The type of Installer</param>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetInstaller(ByVal installerType As String) As ComponentInstallerBase
            Dim installer As ComponentInstallerBase = Nothing

            Select Case installerType
                Case "File"
                    installer = New FileInstaller()
                Case "Assembly"
                    installer = New AssemblyInstaller()
                Case "ResourceFile"
                    installer = New ResourceFileInstaller()
                Case "AuthenticationSystem", "Auth_System"
                    installer = New AuthenticationInstaller()
                Case "DashboardControl"
                    installer = New DashboardInstaller()
                Case "Script"
                    installer = New ScriptInstaller()
                Case "Config"
                    installer = New ConfigInstaller()
                Case "Cleanup"
                    installer = New CleanupInstaller()
                Case "Skin"
                    installer = New SkinInstaller()
                Case "Container"
                    installer = New ContainerInstaller()
                Case "Module"
                    installer = New ModuleInstaller()
                Case "CoreLanguage"
                    installer = New LanguageInstaller(LanguagePackType.Core)
                Case "ExtensionLanguage"
                    installer = New LanguageInstaller(LanguagePackType.Extension)
                Case "Provider"
                    installer = New ProviderInstaller()
                Case "SkinObject"
                    installer = New SkinControlInstaller()
                Case "Widget"
                    installer = New WidgetInstaller()
                Case Else
                    'Installer type is defined in the List
                    Dim listController As New Lists.ListController()
                    Dim entry As ListEntryInfo = listController.GetListEntryInfo("Installer", installerType)

                    If entry IsNot Nothing AndAlso Not String.IsNullOrEmpty(entry.Text) Then
                        'The class for the Installer is specified in the Text property
                        installer = CType(Reflection.CreateObject(entry.Text, "Installer_" + entry.Value), ComponentInstallerBase)
                    End If
            End Select

            Return installer

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The GetInstaller method instantiates the relevant Component Installer
        ''' </summary>
        ''' <param name="manifestNav">The manifest (XPathNavigator) for the component</param>
        ''' <param name="package">The associated PackageInfo instance</param>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetInstaller(ByVal manifestNav As XPathNavigator, ByVal package As PackageInfo) As ComponentInstallerBase
            Dim installerType As String = Util.ReadAttribute(manifestNav, "type")
            Dim componentVersion As String = Util.ReadAttribute(manifestNav, "version")

            Dim installer As ComponentInstallerBase = GetInstaller(installerType)

            If installer IsNot Nothing Then
                'Set package
                installer.Package = package

                'Set type
                installer.Type = installerType

                If Not String.IsNullOrEmpty(componentVersion) Then
                    installer.Version = New Version(componentVersion)
                Else
                    installer.Version = package.Version
                End If

                'Read Manifest
                If package.InstallerInfo.InstallMode <> InstallMode.ManifestOnly OrElse installer.SupportsManifestOnlyInstall Then
                    installer.ReadManifest(manifestNav)
                End If
            End If

            Return installer

        End Function

#End Region

    End Class

End Namespace
