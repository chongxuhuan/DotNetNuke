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
Imports DotNetNuke.Services.Authentication


Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The AuthenticationPackageWriter class 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/30/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AuthenticationPackageWriter
        Inherits PackageWriterBase

#Region "Private Members"

        Private _AuthSystem As AuthenticationInfo

#End Region

#Region "Constructors"

        Public Sub New(ByVal package As PackageInfo)
            MyBase.New(package)
            _AuthSystem = AuthenticationController.GetAuthenticationServiceByPackageID(package.PackageID)
            Initialize()
        End Sub

        Public Sub New(ByVal authSystem As AuthenticationInfo, ByVal package As PackageInfo)
            MyBase.New(package)
            _AuthSystem = authSystem
            Initialize()
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the associated Authentication System
        ''' </summary>
        ''' <value>An AuthenticationInfo object</value>
        ''' <history>
        ''' 	[cnurse]	01/30/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AuthSystem() As AuthenticationInfo
            Get
                Return _AuthSystem
            End Get
            Set(ByVal value As AuthenticationInfo)
                _AuthSystem = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Sub Initialize()
            BasePath = Path.Combine("DesktopModules\AuthenticationServices", AuthSystem.AuthenticationType)
            AppCodePath = Path.Combine("App_Code\AuthenticationServices", AuthSystem.AuthenticationType)
            AssemblyPath = "bin"
        End Sub

        Private Sub WriteAuthenticationComponent(ByVal writer As XmlWriter)
            'Start component Element
            writer.WriteStartElement("component")
            writer.WriteAttributeString("type", "AuthenticationSystem")

            'Start authenticationService Element
            writer.WriteStartElement("authenticationService")

            writer.WriteElementString("type", AuthSystem.AuthenticationType)
            writer.WriteElementString("settingsControlSrc", AuthSystem.SettingsControlSrc)
            writer.WriteElementString("loginControlSrc", AuthSystem.LoginControlSrc)
            writer.WriteElementString("logoffControlSrc", AuthSystem.LogoffControlSrc)

            'End authenticationService Element
            writer.WriteEndElement()

            'End component Element
            writer.WriteEndElement()
        End Sub

#End Region

        Protected Overrides Sub WriteManifestComponent(ByVal writer As System.Xml.XmlWriter)
            'Write Authentication Component
            WriteAuthenticationComponent(writer)
        End Sub

    End Class

End Namespace

