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
Imports System.Configuration
Imports System.Data
Imports System.Globalization
Imports System.Xml.Serialization
Imports System.IO

Namespace DotNetNuke.Services.FileSystem

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : FileInfo
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Represents the File object and holds the Properties of that object
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[DYNST]	2/1/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <XmlRoot("file", IsNullable:=False)> <Serializable()> Public Class FileInfo

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Primary Key ID of the current File, as represented within the Database table named "Files"
        ''' </summary>
        ''' <remarks>
        ''' This Integer Property is passed to the FileInfo Collection
        ''' </remarks>
        ''' <history>
        ''' 	[DYNST]	2/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private _FileId As Integer
        Private _PortalId As Integer
        Private _FileName As String
        Private _Extension As String
        Private _Size As Integer
        Private _Width As Integer
        Private _Height As Integer
        Private _ContentType As String
        Private _Folder As String
        Private _FolderId As Integer
        Private _StorageLocation As Integer
        Private _IsCached As Boolean = False

        Public Sub New()
        End Sub

        <XmlElement("contenttype")> Public Property ContentType() As String
            Get
                Return _ContentType
            End Get
            Set(ByVal Value As String)
                _ContentType = Value
            End Set
        End Property

        <XmlElement("extension")> Public Property Extension() As String
            Get
                Return _Extension
            End Get
            Set(ByVal Value As String)
                _Extension = Value
            End Set
        End Property

        <XmlIgnore()> Public Property FileId() As Integer
            Get
                Return _FileId
            End Get
            Set(ByVal Value As Integer)
                _FileId = Value
            End Set
        End Property

        <XmlElement("filename")> Public Property FileName() As String
            Get
                Return _FileName
            End Get
            Set(ByVal Value As String)
                _FileName = Value
            End Set
        End Property

        <XmlIgnore()> Public Property Folder() As String
            Get
                Return _Folder
            End Get
            Set(ByVal Value As String)
                _Folder = Value
            End Set
        End Property

        <XmlIgnore()> Public Property FolderId() As Integer
            Get
                Return _FolderId
            End Get
            Set(ByVal Value As Integer)
                _FolderId = Value
            End Set
        End Property

        <XmlElement("height")> Public Property Height() As Integer
            Get
                Return _Height
            End Get
            Set(ByVal Value As Integer)
                _Height = Value
            End Set
        End Property

        <XmlIgnore()> Public Property IsCached() As Boolean
            Get
                Return _IsCached
            End Get
            Set(ByVal Value As Boolean)
                _IsCached = Value
            End Set
        End Property

        <XmlIgnore()> Public ReadOnly Property PhysicalPath() As String
            Get
                Dim _PhysicalPath As String
                Dim PortalSettings As PortalSettings = Nothing
                If Not HttpContext.Current Is Nothing Then
                    PortalSettings = PortalController.GetCurrentPortalSettings()
                End If

                If PortalId = Null.NullInteger Then
                    _PhysicalPath = DotNetNuke.Common.Globals.HostMapPath + RelativePath
                Else
                    If PortalSettings Is Nothing OrElse PortalSettings.PortalId <> PortalId Then
                        'Get the PortalInfo  based on the Portalid
                        Dim objPortals As New PortalController()
                        Dim objPortal As PortalInfo = objPortals.GetPortal(PortalId)

                        _PhysicalPath = objPortal.HomeDirectoryMapPath + RelativePath
                    Else
                        _PhysicalPath = PortalSettings.HomeDirectoryMapPath + RelativePath
                    End If
                End If

                Return _PhysicalPath.Replace("/", "\")
            End Get
        End Property

        <XmlIgnore()> Public Property PortalId() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal Value As Integer)
                _PortalId = Value
            End Set
        End Property

        Public ReadOnly Property RelativePath() As String
            Get
                Return Folder + FileName
            End Get
        End Property

        <XmlElement("size")> Public Property Size() As Integer
            Get
                Return _Size
            End Get
            Set(ByVal Value As Integer)
                _Size = Value
            End Set
        End Property

        <XmlIgnore()> Public Property StorageLocation() As Integer
            Get
                Return _StorageLocation
            End Get
            Set(ByVal Value As Integer)
                _StorageLocation = Value
            End Set
        End Property

        <XmlElement("width")> Public Property Width() As Integer
            Get
                Return _Width
            End Get
            Set(ByVal Value As Integer)
                _Width = Value
            End Set
        End Property

    End Class

End Namespace
