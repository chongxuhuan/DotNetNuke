' 
'' DotNetNuke® - http://www.dotnetnuke.com 
'' Copyright (c) 2002-2010 
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


Imports System
Imports System.Web
Imports System.Net
Imports System.Xml.Serialization
Imports DotNetNuke.Common

Namespace DotNetNuke.Modules.Dashboard.Components.Server
    ''' <summary> 
    ''' This class manages the Server Information for the site 
    ''' </summary> 
    <XmlRoot("serverInfo")> _
    Public Class ServerInfo
        Implements IXmlSerializable

#Region "Public Properties"

        Public ReadOnly Property Framework() As String
            Get
                Return System.Environment.Version.ToString()
            End Get
        End Property

        Public ReadOnly Property HostName() As String
            Get
                Return Dns.GetHostName()
            End Get
        End Property

        Public ReadOnly Property Identity() As String
            Get
                Return System.Security.Principal.WindowsIdentity.GetCurrent().Name
            End Get
        End Property

        Public ReadOnly Property IISVersion() As String
            Get
                Return HttpContext.Current.Request.ServerVariables("SERVER_SOFTWARE")
            End Get
        End Property

        Public ReadOnly Property OSVersion() As String
            Get
                Return System.Environment.OSVersion.ToString()
            End Get
        End Property

        Public ReadOnly Property PhysicalPath() As String
            Get
                Return DotNetNuke.Common.Globals.ApplicationMapPath
            End Get
        End Property

        Public ReadOnly Property Url() As String
            Get
                Return Globals.GetDomainName(HttpContext.Current.Request)
            End Get
        End Property

        Public ReadOnly Property RelativePath() As String
            Get
                Dim path As String
                If String.IsNullOrEmpty(DotNetNuke.Common.Globals.ApplicationPath) Then
                    path = "/"
                Else
                    path = DotNetNuke.Common.Globals.ApplicationPath
                End If
                Return path
            End Get
        End Property

        Public ReadOnly Property ServerTime() As String
            Get
                Return System.DateTime.Now.ToString()
            End Get
        End Property

#End Region

#Region "IXmlSerializable Members"

        Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements IXmlSerializable.GetSchema
            Throw New NotImplementedException()
        End Function

        Public Sub ReadXml(ByVal reader As System.Xml.XmlReader) Implements IXmlSerializable.ReadXml
            Throw New NotImplementedException()
        End Sub

        Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter) Implements IXmlSerializable.WriteXml
            writer.WriteElementString("osVersion", OSVersion)
            writer.WriteElementString("iisVersion", IISVersion)
            writer.WriteElementString("framework", Framework)
            writer.WriteElementString("identity", Identity)
            writer.WriteElementString("hostName", HostName)
            writer.WriteElementString("physicalPath", PhysicalPath)
            writer.WriteElementString("url", Url)
            writer.WriteElementString("relativePath", RelativePath)
        End Sub
#End Region

    End Class

End Namespace