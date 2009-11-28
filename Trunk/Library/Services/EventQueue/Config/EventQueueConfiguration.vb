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
Imports System.collections.generic
Imports System.IO
Imports System.Web
Imports System.Web.Caching
Imports System.Xml
Imports System.Xml.Serialization
Imports DotNetNuke.Services.Cache
Imports DotNetNuke
Imports DotNetNuke.Common
Imports System.Text
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Services.EventQueue.Config

    Friend Class EventQueueConfiguration

        Private _publishedEvents As Dictionary(Of String, PublishedEvent)
        Private _eventQueueSubscribers As Dictionary(Of String, SubscriberInfo)

#Region "Constructors"

        Friend Sub New()
            _publishedEvents = New Dictionary(Of String, PublishedEvent)()
            _eventQueueSubscribers = New Dictionary(Of String, SubscriberInfo)()
        End Sub

#End Region

#Region "Public Properties"

        Friend Property EventQueueSubscribers() As Dictionary(Of String, SubscriberInfo)
            Get
                Return _eventQueueSubscribers
            End Get
            Set(ByVal Value As Dictionary(Of String, SubscriberInfo))
                _eventQueueSubscribers = Value
            End Set
        End Property

        Friend Property PublishedEvents() As Dictionary(Of String, PublishedEvent)
            Get
                Return _publishedEvents
            End Get
            Set(ByVal Value As Dictionary(Of String, PublishedEvent))
                _publishedEvents = Value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Sub Deserialize(ByVal configXml As String)
            If configXml <> "" Then
                Dim xmlDoc As New XmlDocument
                xmlDoc.LoadXml(configXml)
                For Each xmlItem As XmlElement In xmlDoc.SelectNodes("/EventQueueConfig/PublishedEvents/Event")
                    Dim oPublishedEvent As New PublishedEvent
                    oPublishedEvent.EventName = xmlItem.SelectSingleNode("EventName").InnerText
                    oPublishedEvent.Subscribers = xmlItem.SelectSingleNode("Subscribers").InnerText
                    Me.PublishedEvents.Add(oPublishedEvent.EventName, oPublishedEvent)
                Next
                For Each xmlItem As XmlElement In xmlDoc.SelectNodes("/EventQueueConfig/EventQueueSubscribers/Subscriber")
                    Dim oSubscriberInfo As New SubscriberInfo
                    oSubscriberInfo.ID = xmlItem.SelectSingleNode("ID").InnerText
                    oSubscriberInfo.Name = xmlItem.SelectSingleNode("Name").InnerText
                    oSubscriberInfo.Address = xmlItem.SelectSingleNode("Address").InnerText
                    oSubscriberInfo.Description = xmlItem.SelectSingleNode("Description").InnerText
                    oSubscriberInfo.PrivateKey = xmlItem.SelectSingleNode("PrivateKey").InnerText
                    Me.EventQueueSubscribers.Add(oSubscriberInfo.ID, oSubscriberInfo)
                Next
            End If

        End Sub

        Private Function Serialize() As String
            Dim settings As New XmlWriterSettings()
            settings.ConformanceLevel = ConformanceLevel.Document
            settings.Indent = True
            settings.CloseOutput = True
            settings.OmitXmlDeclaration = False

            Dim sb As New StringBuilder()

            Dim writer As XmlWriter = XmlWriter.Create(sb, settings)
            writer.WriteStartElement("EventQueueConfig")

            writer.WriteStartElement("PublishedEvents")
            For Each key As String In Me.PublishedEvents.Keys
                writer.WriteStartElement("Event")

                writer.WriteElementString("EventName", Me.PublishedEvents(key).EventName)
                writer.WriteElementString("Subscribers", Me.PublishedEvents(key).Subscribers)

                writer.WriteEndElement()
            Next
            writer.WriteEndElement()

            writer.WriteStartElement("EventQueueSubscribers")
            For Each key As String In Me.EventQueueSubscribers.Keys
                writer.WriteStartElement("Subscriber")

                writer.WriteElementString("ID", Me.EventQueueSubscribers(key).ID)
                writer.WriteElementString("Name", Me.EventQueueSubscribers(key).Name)
                writer.WriteElementString("Address", Me.EventQueueSubscribers(key).Address)
                writer.WriteElementString("Description", Me.EventQueueSubscribers(key).Description)
                writer.WriteElementString("PrivateKey", Me.EventQueueSubscribers(key).PrivateKey)

                writer.WriteEndElement()
            Next
            writer.WriteEndElement()

            'Close EventQueueConfig
            writer.WriteEndElement()

            writer.Close()

            Return sb.ToString()

        End Function

#End Region

        Friend Shared Function GetConfig() As EventQueueConfiguration

            Dim config As EventQueueConfiguration = CType(DataCache.GetCache("EventQueueConfig"), EventQueueConfiguration)

            If (config Is Nothing) Then

                Dim filePath As String = HostMapPath & "EventQueue\EventQueue.config"
                If File.Exists(filePath) Then
                    config = New EventQueueConfiguration
                    ' Deserialize into EventQueueConfiguration
                    config.Deserialize(FileSystemUtils.ReadFile(filePath))
                Else
                    'make a default config file
                    Dim si As New SubscriberInfo("DNN Core")
                    Dim e As New PublishedEvent
                    e.EventName = "Application_Start"
                    e.Subscribers = si.ID
                    config = New EventQueueConfiguration
                    config.PublishedEvents = New Dictionary(Of String, PublishedEvent)
                    config.PublishedEvents.Add(e.EventName, e)
                    config.EventQueueSubscribers = New Dictionary(Of String, SubscriberInfo)
                    config.EventQueueSubscribers.Add(si.ID, si)
                    Dim oStream As StreamWriter = File.CreateText(filePath)
                    oStream.WriteLine(config.Serialize())
                    oStream.Close()

                End If
                If File.Exists(filePath) Then
                    ' Set back into Cache
                    DataCache.SetCache("EventQueueConfig", config, New DNNCacheDependency(filePath))
                End If

            End If

            Return config

        End Function

    End Class

End Namespace
