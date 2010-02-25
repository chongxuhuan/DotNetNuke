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
Imports DotNetNuke.Application
Imports DotNetNuke.Common
Imports DotNetNuke.Framework.Providers
Imports System.Collections
Imports DotNetNuke.Framework
Imports System.Xml.Serialization
Imports DotNetNuke.Services.Scheduling
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Modules.Dashboard.Components.Host

    <XmlRoot("hostInfo")> _
    Public Class HostInfo
        Implements IXmlSerializable

#Region "Public Properties"

        Public ReadOnly Property CachingProvider() As String
            Get
                Return ProviderConfiguration.GetProviderConfiguration("caching").DefaultProvider
            End Get
        End Property

        Public ReadOnly Property DataProvider() As String
            Get
                Return ProviderConfiguration.GetProviderConfiguration("data").DefaultProvider
            End Get
        End Property

        Public ReadOnly Property FriendlyUrlProvider() As String
            Get
                Return ProviderConfiguration.GetProviderConfiguration("friendlyUrl").DefaultProvider
            End Get
        End Property

        Public ReadOnly Property FriendlyUrlEnabled() As String
            Get
                Return DotNetNuke.Entities.Host.Host.UseFriendlyUrls.ToString
            End Get
        End Property

        Public ReadOnly Property FriendlyUrlType() As String
            Get
                Dim urlprovider As Provider = DirectCast(ProviderConfiguration.GetProviderConfiguration("friendlyUrl").Providers(FriendlyUrlProvider), Provider)
                Return IIf((urlprovider.Attributes("urlformat") = "humanfriendly"), "humanfriendly", "searchfriendly").ToString
            End Get
        End Property

        Public ReadOnly Property HostGUID() As String
            Get
                Return DotNetNuke.Entities.Host.Host.GUID
            End Get
        End Property

        Public ReadOnly Property HtmlEditorProvider() As String
            Get
                Return ProviderConfiguration.GetProviderConfiguration("htmlEditor").DefaultProvider
            End Get
        End Property

        Public ReadOnly Property LoggingProvider() As String
            Get
                Return ProviderConfiguration.GetProviderConfiguration("logging").DefaultProvider
            End Get
        End Property

        Public ReadOnly Property Permissions() As String
            Get
                Return SecurityPolicy.Permissions
            End Get
        End Property

        Public ReadOnly Property Product() As String
            Get
                Return DotNetNukeContext.Current.Application.Description
            End Get
        End Property

        Public ReadOnly Property SchedulerMode() As String
            Get
                Return DotNetNuke.Entities.Host.Host.SchedulerMode.ToString
            End Get
        End Property

        Public ReadOnly Property Version() As String
            Get
                Return DotNetNukeContext.Current.Application.Version.ToString(3)
            End Get
        End Property

        Public ReadOnly Property WebFarmEnabled() As String
            Get
                Return DotNetNuke.Services.Cache.CachingProvider.Instance().IsWebFarm.ToString
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
            writer.WriteElementString("hostGUID", HostGUID)
            writer.WriteElementString("version", Version)
            writer.WriteElementString("permissions", Permissions)
            writer.WriteElementString("dataProvider", DataProvider)
            writer.WriteElementString("cachingProvider", CachingProvider)
            writer.WriteElementString("friendlyUrlProvider", FriendlyUrlProvider)
            writer.WriteElementString("friendlyUrlEnabled", FriendlyUrlEnabled)
            writer.WriteElementString("friendlyUrlType", FriendlyUrlType)
            writer.WriteElementString("htmlEditorProvider", HtmlEditorProvider)
            writer.WriteElementString("loggingProvider", LoggingProvider)
            writer.WriteElementString("schedulerMode", SchedulerMode)
            writer.WriteElementString("webFarmEnabled", WebFarmEnabled)

        End Sub
#End Region

    End Class

End Namespace
