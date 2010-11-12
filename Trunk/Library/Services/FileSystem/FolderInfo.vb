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
Imports System.Data
Imports DotNetNuke
Imports System.Xml.Serialization
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Services.FileSystem

#Region "FolderInfo"
    <XmlRoot("folder", IsNullable:=False)> <Serializable()> Public Class FolderInfo
        Implements IHydratable

#Region "Private Members"

        ' local property declarations
        Private _folderID As Integer = Null.NullInteger
        Private _portalID As Integer
        Private _uniqueID As Guid
        Private _versionGuid As Guid
        Private _folderPath As String
        Private _displayName As String
        Private _storageLocation As Integer
        Private _isProtected As Boolean
        Private _isCached As Boolean = False
        Private _lastUpdated As Date
        Private _FolderPermissions As Security.Permissions.FolderPermissionCollection
        Private _displayPath As String

#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

        Public Sub New(ByVal portalId As Integer, ByVal folderpath As String, ByVal storageLocation As Integer, ByVal isProtected As Boolean, ByVal isCached As Boolean, ByVal lastUpdated As Date)
            Me.New(Guid.NewGuid(), portalId, folderpath, storageLocation, isProtected, isCached, lastUpdated)
        End Sub

        Public Sub New(ByVal uniqueID As Guid, ByVal portalId As Integer, ByVal folderpath As String, ByVal storageLocation As Integer, ByVal isProtected As Boolean, ByVal isCached As Boolean, ByVal lastUpdated As Date)
            Me.UniqueId = uniqueID
            Me.VersionGuid = Guid.NewGuid()

            Me.PortalID = portalId
            Me.FolderPath = folderpath
            Me.StorageLocation = storageLocation
            Me.IsProtected = isProtected
            Me.IsCached = isCached
            Me.LastUpdated = lastUpdated
        End Sub
#End Region

#Region "Public Properties"

        <XmlElement("folderid")> Public Property FolderID() As Integer
            Get
                Return _folderID
            End Get
            Set(ByVal Value As Integer)
                _folderID = Value
            End Set
        End Property

        <XmlElement("uniqueid")> Public Property UniqueId() As Guid
            Get
                Return _uniqueID
            End Get
            Set(ByVal Value As Guid)
                _uniqueID = Value
            End Set

        End Property

        <XmlElement("versionguid")> Public Property VersionGuid() As Guid
            Get
                Return _versionGuid
            End Get
            Set(ByVal Value As Guid)
                _versionGuid = Value
            End Set
        End Property

        <XmlElement("foldername")> Public ReadOnly Property FolderName() As String
            Get
                Dim _folderName As String = FileSystemUtils.RemoveTrailingSlash(_folderPath)
                If _folderName.Length > 0 AndAlso _folderName.LastIndexOf("/") > -1 Then
                    _folderName = _folderName.Substring(_folderName.LastIndexOf("/") + 1)
                End If
                Return _folderName
            End Get
        End Property

        <XmlElement("displayname")> Public Property DisplayName() As String
            Get
                If (Not String.IsNullOrEmpty(_displayName)) Then
                    _displayName = FolderName
                End If
                Return _displayName
            End Get
            Set(ByVal value As String)
                _displayName = value
            End Set
        End Property

        <XmlElement("folderpath")> Public Property FolderPath() As String
            Get
                Return _folderPath
            End Get
            Set(ByVal Value As String)
                _folderPath = Value
            End Set
        End Property

        <XmlElement("displaypath")> Public Property DisplayPath() As String
            Get
                If (String.IsNullOrEmpty(_displayPath)) Then
                    _displayPath = FolderPath
                End If
                Return _displayPath
            End Get
            Set(ByVal value As String)
                _displayPath = value
            End Set
        End Property

        <XmlElement("iscached")> Public Property IsCached() As Boolean
            Get
                Return _isCached
            End Get
            Set(ByVal Value As Boolean)
                _isCached = Value
            End Set
        End Property

        <XmlElement("isprotected")> Public Property IsProtected() As Boolean
            Get
                Return _isProtected
            End Get
            Set(ByVal Value As Boolean)
                _isProtected = Value
            End Set
        End Property

        <XmlIgnore()> Public Property LastUpdated() As Date
            Get
                Return _lastUpdated
            End Get
            Set(ByVal Value As Date)
                _lastUpdated = Value
            End Set
        End Property

        <XmlElement("physicalpath")> Public ReadOnly Property PhysicalPath() As String
            Get
                Dim _PhysicalPath As String
                Dim PortalSettings As PortalSettings = Nothing
                If Not HttpContext.Current Is Nothing Then
                    PortalSettings = PortalController.GetCurrentPortalSettings()
                End If

                If PortalID = Null.NullInteger Then
                    _PhysicalPath = DotNetNuke.Common.Globals.HostMapPath + FolderPath
                Else
                    If PortalSettings Is Nothing OrElse PortalSettings.PortalId <> PortalID Then
                        'Get the PortalInfo  based on the Portalid
                        Dim objPortals As New PortalController()
                        Dim objPortal As PortalInfo = objPortals.GetPortal(PortalID)

                        _PhysicalPath = objPortal.HomeDirectoryMapPath + FolderPath
                    Else
                        _PhysicalPath = PortalSettings.HomeDirectoryMapPath + FolderPath
                    End If
                End If

                Return _PhysicalPath.Replace("/", "\")
            End Get
        End Property

        <XmlElement("portalid")> Public Property PortalID() As Integer
            Get
                Return _portalID
            End Get
            Set(ByVal Value As Integer)
                _portalID = Value
            End Set
        End Property

        <XmlElement("storagelocation")> Public Property StorageLocation() As Integer
            Get
                Return _storageLocation
            End Get
            Set(ByVal Value As Integer)
                _storageLocation = Value
            End Set
        End Property

        <XmlElement("folderpermissions")> Public ReadOnly Property FolderPermissions() As FolderPermissionCollection
            Get
                If _FolderPermissions Is Nothing Then
                    _FolderPermissions = New FolderPermissionCollection(FolderPermissionController.GetFolderPermissionsCollectionByFolder(PortalID, FolderPath))
                End If
                Return _FolderPermissions
            End Get
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a FolderInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	07/14/2008   Documented
        '''     [vnguyen]   30/04/2010   Modified: Added VersionGuid
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            FolderID = Null.SetNullInteger(dr("FolderID"))
            UniqueId = Null.SetNullGuid(dr("UniqueId"))
            VersionGuid = Null.SetNullGuid(dr("VersionGuid"))
            PortalID = Null.SetNullInteger(dr("PortalID"))
            FolderPath = Null.SetNullString(dr("FolderPath"))
            IsCached = Null.SetNullBoolean(dr("IsCached"))
            IsProtected = Null.SetNullBoolean(dr("IsProtected"))
            StorageLocation = Null.SetNullInteger(dr("StorageLocation"))
            LastUpdated = Null.SetNullDateTime(dr("LastUpdated"))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	07/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return FolderID
            End Get
            Set(ByVal value As Integer)
                FolderID = value
            End Set
        End Property

#End Region

    End Class
#End Region

End Namespace
