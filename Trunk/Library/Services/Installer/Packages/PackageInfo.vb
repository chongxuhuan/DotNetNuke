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

Imports DotNetNuke.Services.Installer.Log
Imports DotNetNuke.Entities
Imports System.Xml.Serialization

Namespace DotNetNuke.Services.Installer.Packages

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PackageInfo class represents a single Installer Package
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/24/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class PackageInfo
        Inherits BaseEntityInfo
#Region "Private Members"

        Private _PackageID As Integer = Null.NullInteger
        Private _PortalID As Integer = Null.NullInteger
        Private _Description As String
        Private _FriendlyName As String
        Private _IsValid As Boolean = True
        Private _InstalledVersion As System.Version = New Version(0, 0, 0)
        Private _InstallerInfo As InstallerInfo
        Private _IsSystemPackage As Boolean
        Private _License As String
        Private _Manifest As String
        Private _Name As String
        Private _PackageType As String
        Private _ReleaseNotes As String
        Private _Version As System.Version = New Version(0, 0, 0)

        Private _Owner As String
        Private _Organization As String
        Private _Url As String
        Private _Email As String

        Private InstalledPackage As PackageInfo

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new InstallPackage instance as defined by the
        ''' Parameters
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal info As InstallerInfo)
            AttachInstallerInfo(info)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new InstallPackage instance
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ID of this package
        ''' </summary>
        ''' <value>An Integer</value>
        ''' <history>
        ''' 	[cnurse]	07/26/2007  created
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
        ''' Gets the ID of this portal
        ''' </summary>
        ''' <value>An Integer</value>
        ''' <history>
        ''' 	[cnurse]	09/11/2008  created
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
        ''' Gets the Owner of this package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	03/26/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Owner() As String
            Get
                Return _Owner
            End Get
            Set(ByVal value As String)
                _Owner = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Organisation for this package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	03/26/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Organization() As String
            Get
                Return _Organization
            End Get
            Set(ByVal value As String)
                _Organization = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Url for this package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	03/26/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Url() As String
            Get
                Return _Url
            End Get
            Set(ByVal value As String)
                _Url = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Email for this package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	03/26/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal value As String)
                _Email = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Description of this package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/26/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
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
        <XmlIgnore()> Public ReadOnly Property Files() As Dictionary(Of String, InstallFile)
            Get
                Return InstallerInfo.Files
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the FriendlyName of this package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FriendlyName() As String
            Get
                Return _FriendlyName
            End Get
            Set(ByVal value As String)
                _FriendlyName = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the associated InstallerInfo
        ''' </summary>
        ''' <value>An InstallerInfo object</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public ReadOnly Property InstallerInfo() As InstallerInfo
            Get
                Return _InstallerInfo
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Installed Version of the Package
        ''' </summary>
        ''' <value>A System.Version</value>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property InstalledVersion() As System.Version
            Get
                Return _InstalledVersion
            End Get
            Set(ByVal value As System.Version)
                _InstalledVersion = value
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
                Return InstallerInfo.InstallMode
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets whether this package is a "system" Package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/19/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsSystemPackage() As Boolean
            Get
                Return _IsSystemPackage
            End Get
            Set(ByVal value As Boolean)
                _IsSystemPackage = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Package is Valid
        ''' </summary>
        ''' <value>A Boolean value</value>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsValid() As Boolean
            Get
                Return _IsValid
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the License of this package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property License() As String
            Get
                Return _License
            End Get
            Set(ByVal value As String)
                _License = value
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
        <XmlIgnore()> Public ReadOnly Property Log() As Logger
            Get
                Return InstallerInfo.Log
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Manifest of this package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/26/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Manifest() As String
            Get
                Return _Manifest
            End Get
            Set(ByVal value As String)
                _Manifest = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Name of this package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the ReleaseNotes of this package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/07/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ReleaseNotes() As String
            Get
                Return _ReleaseNotes
            End Get
            Set(ByVal value As String)
                _ReleaseNotes = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Type of this package
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PackageType() As String
            Get
                Return _PackageType
            End Get
            Set(ByVal value As String)
                _PackageType = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Version of this package
        ''' </summary>
        ''' <value>A System.Version</value>
        ''' <history>
        ''' 	[cnurse]	07/26/2007  created
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

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The AttachInstallerInfo method attachs an InstallerInfo instance to the Package
        ''' </summary>
        ''' <param name="installer">The InstallerInfo instance to attach</param>
        ''' <history>
        ''' 	[cnurse]	08/01/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AttachInstallerInfo(ByVal installer As InstallerInfo)
            _InstallerInfo = installer
        End Sub

#End Region

    End Class

End Namespace
