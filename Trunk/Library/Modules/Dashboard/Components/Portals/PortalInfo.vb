' 
'' DotNetNuke® - http://www.dotnetnuke.com 
'' Copyright (c) 2002-2009 
'' by DotNetNuke Corporation 
'' 
'' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'' to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
'' 
'' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
'' of the Software. 
'' 
'' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'' DEALINGS IN THE SOFTWARE. 
' 


Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security.Roles
Imports System.Collections
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Dashboard.Components.Portals
    Public Class PortalInfo

#Region "Private Members"
        Private _Pages As Integer = Null.NullInteger
        Private _Roles As Integer = Null.NullInteger
        Private _Users As Integer = Null.NullInteger

#End Region

#Region "Public Properties"

        Private _GUID As System.Guid
        Public Property GUID() As System.Guid
            Get
                Return _GUID
            End Get
            Set(ByVal value As System.Guid)
                _GUID = value
            End Set
        End Property

        Public ReadOnly Property Pages() As Integer
            Get
                If _Pages < 0 Then
                    Dim controller As New TabController()
                    _Pages = controller.GetTabCount(PortalID)
                End If
                Return _Pages
            End Get
        End Property

        Private _PortalID As Integer
        Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal value As Integer)
                _PortalID = value
            End Set
        End Property

        Private _PortalName As String
        Public Property PortalName() As String
            Get
                Return _PortalName
            End Get
            Set(ByVal value As String)
                _PortalName = value
            End Set
        End Property

        Public ReadOnly Property Roles() As Integer
            Get
                If _Roles < 0 Then
                    Dim controller As New RoleController()
                    Dim portalRoles As ArrayList = controller.GetPortalRoles(PortalID)
                    _Roles = portalRoles.Count
                End If
                Return _Roles
            End Get
        End Property

        Public ReadOnly Property Users() As Integer
            Get
                If _Users < 0 Then
                    _Users = UserController.GetUserCountByPortal(PortalID)
                End If
                Return _Users
            End Get
        End Property

#End Region

        Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter)
            'Write start of main elemenst 
            writer.WriteStartElement("portal")

            writer.WriteElementString("portalName", PortalName)
            writer.WriteElementString("GUID", GUID.ToString())
            writer.WriteElementString("pages", Pages.ToString())
            writer.WriteElementString("users", Users.ToString())
            writer.WriteElementString("roles", Roles.ToString())

            'Write end of Host Info 
            writer.WriteEndElement()
        End Sub
    End Class

End Namespace