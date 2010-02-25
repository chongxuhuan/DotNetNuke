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
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Xml.XPath

Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Services.Installer.Log
Imports DotNetNuke.Services.Installer.Packages

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ComponentInstallerBase is a base class for all Component Installers
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/24/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class ComponentInstallerBase

#Region "Private Members"

        Private _Completed As Boolean = Null.NullBoolean
        Private _PackageInfo As PackageInfo
        Private _SecurityAccessLevel As SecurityAccessLevel = SecurityAccessLevel.Host
        Private _Type As String
        Private _Version As System.Version

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
        Public Overridable ReadOnly Property AllowableFiles() As String
            Get
                Return Null.NullString
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Completed flag
        ''' </summary>
        ''' <value>A Boolean value</value>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Completed() As Boolean
            Get
                Return _Completed
            End Get
            Set(ByVal value As Boolean)
                _Completed = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the InstallMode
        ''' </summary>
        ''' <value>An InstallMode value</value>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property InstallMode() As InstallMode
            Get
                Return Package.InstallMode
            End Get
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
                Return _PackageInfo
            End Get
            Set(ByVal value As PackageInfo)
                _PackageInfo = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Dictionary of Files that are included in the Package
        ''' </summary>
        ''' <value>A Dictionary(Of String, InstallFile)</value>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property PackageFiles() As Dictionary(Of String, InstallFile)
            Get
                Return Package.Files
            End Get
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
                Return Package.InstallerInfo.PhysicalSitePath
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Installer supports Manifest only installs
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/29/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable ReadOnly Property SupportsManifestOnlyInstall() As Boolean
            Get
                Return True
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Type of the component
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Type() As String
            Get
                Return _Type
            End Get
            Set(ByVal value As String)
                _Type = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Version of the Component
        ''' </summary>
        ''' <value>A System.Version</value>
        ''' <history>
        ''' 	[cnurse]	02/29/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Version() As System.Version
            Get
                Return _Version
            End Get
            Set(ByVal value As System.Version)
                _Version = value
            End Set
        End Property

#End Region

#Region "Abstract Methods"

        Public MustOverride Sub Commit()

        Public MustOverride Sub Install()

        Public MustOverride Sub ReadManifest(ByVal manifestNav As XPathNavigator)

        Public MustOverride Sub Rollback()

        Public MustOverride Sub UnInstall()

#End Region

    End Class

End Namespace
