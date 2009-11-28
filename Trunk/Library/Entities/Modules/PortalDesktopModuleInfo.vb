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

Imports System
Imports System.Configuration
Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Xml
Imports DotNetNuke.Security.Permissions
Imports System.Xml.Serialization

Namespace DotNetNuke.Entities.Modules

    <Serializable()> Public Class PortalDesktopModuleInfo
        Inherits BaseEntityInfo

#Region "Private Members"

        Private _PortalDesktopModuleID As Integer
        Private _DesktopModuleID As Integer
        Private _FriendlyName As String
        Private _Permissions As DesktopModulePermissionCollection
        Private _PortalID As Integer
        Private _PortalName As String

#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"

        <XmlIgnore()> Public Property PortalDesktopModuleID() As Integer
            Get
                Return _PortalDesktopModuleID
            End Get
            Set(ByVal Value As Integer)
                _PortalDesktopModuleID = Value
            End Set
        End Property

        <XmlIgnore()> Public Property DesktopModuleID() As Integer
            Get
                Return _DesktopModuleID
            End Get
            Set(ByVal Value As Integer)
                _DesktopModuleID = Value
            End Set
        End Property

        Public Property FriendlyName() As String
            Get
                Return _FriendlyName
            End Get
            Set(ByVal Value As String)
                _FriendlyName = Value
            End Set
        End Property

        Public ReadOnly Property Permissions() As DesktopModulePermissionCollection
            Get
                If _Permissions Is Nothing Then
                    _Permissions = New DesktopModulePermissionCollection(DesktopModulePermissionController.GetDesktopModulePermissions(PortalDesktopModuleID))
                End If
                Return _Permissions
            End Get
        End Property

        <XmlIgnore()> Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

        <XmlIgnore()> Public Property PortalName() As String
            Get
                Return _PortalName
            End Get
            Set(ByVal Value As String)
                _PortalName = Value
            End Set
        End Property

#End Region

    End Class

End Namespace

