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
Imports System.Text
Imports System.Xml.XPath

Imports DotNetNuke.Services.Authentication

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The AuthenticationInstaller installs Authentication Service Components to a DotNetNuke site
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/25/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AuthenticationInstaller
        Inherits ComponentInstallerBase

#Region "Private Properties"

        Private TempAuthSystem As AuthenticationInfo
        Private AuthSystem As AuthenticationInfo

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
                Return "ashx, aspx, ascx, vb, cs, resx, css, js, resources, config, vbproj, csproj, sln, htm, html"
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DeleteAuthentiation method deletes the Authentication System
        ''' from the data Store.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteAuthentiation()
            Try
                'Attempt to get the Authentication Service
                Dim authSystem As AuthenticationInfo = AuthenticationController.GetAuthenticationServiceByPackageID(Package.PackageID)

                If authSystem IsNot Nothing Then
                    AuthenticationController.DeleteAuthentication(authSystem)
                End If
                Log.AddInfo(String.Format(Util.AUTHENTICATION_UnRegistered, authSystem.AuthenticationType))
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
        ''' <remarks>In the case of Authentication systems this is not neccessary</remarks>
        ''' <history>
        ''' 	[cnurse]	08/01/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Commit()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Install method installs the authentication component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Install()
            Dim bAdd As Boolean = Null.NullBoolean

            Try
                'Attempt to get the Authentication Service
                TempAuthSystem = AuthenticationController.GetAuthenticationServiceByType(AuthSystem.AuthenticationType)

                If TempAuthSystem Is Nothing Then
                    'Enable by default
                    AuthSystem.IsEnabled = True
                    bAdd = True
                Else
                    AuthSystem.AuthenticationID = TempAuthSystem.AuthenticationID
                    AuthSystem.IsEnabled = TempAuthSystem.IsEnabled
                End If
                AuthSystem.PackageID = Package.PackageID

                If bAdd Then
                    'Add new service
                    AuthenticationController.AddAuthentication(AuthSystem)
                Else
                    'Update service
                    AuthenticationController.UpdateAuthentication(AuthSystem)
                End If

                Completed = True
                Log.AddInfo(String.Format(Util.AUTHENTICATION_Registered, AuthSystem.AuthenticationType))
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadManifest method reads the manifest file for the Authentication compoent.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub ReadManifest(ByVal manifestNav As XPathNavigator)

            AuthSystem = New AuthenticationInfo

            'Get the type
            AuthSystem.AuthenticationType = Util.ReadElement(manifestNav, "authenticationService/type", Log, Util.AUTHENTICATION_TypeMissing)

            'Get the SettingsSrc
            AuthSystem.SettingsControlSrc = Util.ReadElement(manifestNav, "authenticationService/settingsControlSrc", Log, Util.AUTHENTICATION_SettingsSrcMissing)

            'Get the LoginSrc
            AuthSystem.LoginControlSrc = Util.ReadElement(manifestNav, "authenticationService/loginControlSrc", Log, Util.AUTHENTICATION_LoginSrcMissing)

            'Get the LogoffSrc
            AuthSystem.LogoffControlSrc = Util.ReadElement(manifestNav, "authenticationService/logoffControlSrc")

            If Log.Valid Then
                Log.AddInfo(Util.AUTHENTICATION_ReadSuccess)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Rollback method undoes the installation of the component in the event 
        ''' that one of the other components fails
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Rollback()
            'If Temp Auth System exists then we need to update the DataStore with this 
            If TempAuthSystem Is Nothing Then
                'No Temp Auth System - Delete newly added system
                DeleteAuthentiation()
            Else
                'Temp Auth System - Rollback to Temp
                AuthenticationController.UpdateAuthentication(TempAuthSystem)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstall method uninstalls the authentication component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UnInstall()
            DeleteAuthentiation()
        End Sub

#End Region

    End Class

End Namespace
