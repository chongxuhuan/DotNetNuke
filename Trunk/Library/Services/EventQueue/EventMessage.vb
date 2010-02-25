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
Imports System.Collections.Specialized
Imports System.Text
Imports System.Xml
Imports System.IO

Namespace DotNetNuke.Services.EventQueue

#Region "Enums"

    Public Enum MessagePriority
        High
        Medium
        Low
    End Enum

#End Region

    <Serializable()> Public Class EventMessage

#Region "Private Members"

        Private _eventMessageID As Integer = Null.NullInteger
        Private _processorType As String = Null.NullString
        Private _processorCommand As String = Null.NullString
        Private _body As String = Null.NullString
        Private _priority As MessagePriority = MessagePriority.Low
        Private _attributes As NameValueCollection
        Private _sender As String = Null.NullString
        Private _subscribers As String = Null.NullString
        Private _authorizedRoles As String = Null.NullString
        Private _expirationDate As DateTime
        Private _exceptionMessage As String = Null.NullString
        Private _sentDate As DateTime

#End Region

#Region "Constructors"

        Public Sub New()
            _attributes = New NameValueCollection
        End Sub

#End Region

#Region "Public Properties"

        Public Property EventMessageID() As Integer
            Get
                Return _eventMessageID
            End Get
            Set(ByVal Value As Integer)
                _eventMessageID = Value
            End Set
        End Property

        Public Property ProcessorType() As String
            Get
                If _processorType Is Nothing Then
                    Return String.Empty
                Else
                    Return _processorType
                End If
            End Get
            Set(ByVal Value As String)
                _processorType = Value
            End Set
        End Property

        Public Property ProcessorCommand() As String
            Get
                If _processorCommand Is Nothing Then
                    Return String.Empty
                Else
                    Return _processorCommand
                End If
            End Get
            Set(ByVal Value As String)
                _processorCommand = Value
            End Set
        End Property

        Public Property Body() As String
            Get
                If _body Is Nothing Then
                    Return String.Empty
                Else
                    Return _body
                End If
            End Get
            Set(ByVal Value As String)
                _body = Value
            End Set
        End Property

        Public Property Sender() As String
            Get
                If _sender Is Nothing Then
                    Return String.Empty
                Else
                    Return _sender
                End If
            End Get
            Set(ByVal Value As String)
                _sender = Value
            End Set
        End Property

        Public Property Subscribers() As String
            Get
                If _subscribers Is Nothing Then
                    Return String.Empty
                Else
                    Return _subscribers
                End If
            End Get
            Set(ByVal Value As String)
                _subscribers = Value
            End Set
        End Property

        Public Property AuthorizedRoles() As String
            Get
                If _authorizedRoles Is Nothing Then
                    Return String.Empty
                Else
                    Return _authorizedRoles
                End If
            End Get
            Set(ByVal Value As String)
                _authorizedRoles = Value
            End Set
        End Property

        Public Property Priority() As MessagePriority
            Get
                Return _priority
            End Get
            Set(ByVal Value As MessagePriority)
                _priority = Value
            End Set
        End Property

        Public Property ExceptionMessage() As String
            Get
                If _exceptionMessage Is Nothing Then
                    Return String.Empty
                Else
                    Return _exceptionMessage
                End If
            End Get
            Set(ByVal Value As String)
                _exceptionMessage = Value
            End Set
        End Property

        Public Property SentDate() As DateTime
            Get
                Return _sentDate.ToLocalTime
            End Get
            Set(ByVal Value As DateTime)
                _sentDate = Value.ToUniversalTime
            End Set
        End Property

        Public Property ExpirationDate() As DateTime
            Get
                Return _expirationDate.ToLocalTime
            End Get
            Set(ByVal Value As DateTime)
                _expirationDate = Value.ToUniversalTime
            End Set
        End Property

        Public Property Attributes() As NameValueCollection
            Get
                Return _attributes
            End Get
            Set(ByVal Value As NameValueCollection)
                _attributes = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        Public Sub DeserializeAttributes(ByVal configXml As String)
            Dim attName As String = Null.NullString
            Dim attValue As String = Null.NullString
            Dim settings As New XmlReaderSettings()
            settings.ConformanceLevel = ConformanceLevel.Fragment

            Dim reader As XmlReader = XmlReader.Create(New StringReader(configXml))
            reader.ReadStartElement("Attributes")
            If Not reader.IsEmptyElement Then
                'Loop throug the Attributes
                Do
                    reader.ReadStartElement("Attribute")

                    'Load it from the Xml
                    reader.ReadStartElement("Name")
                    attName = reader.ReadString()
                    reader.ReadEndElement()
                    If Not reader.IsEmptyElement Then
                        reader.ReadStartElement("Value")
                        attValue = reader.ReadString()
                        reader.ReadEndElement()
                    Else
                        reader.Read()
                    End If

                    'Add attribute to the collection
                    _attributes.Add(attName, attValue)

                Loop While reader.ReadToNextSibling("Attribute")
            End If

        End Sub

        Public Function SerializeAttributes() As String
            Dim settings As New XmlWriterSettings()
            settings.ConformanceLevel = ConformanceLevel.Fragment
            settings.OmitXmlDeclaration = True

            Dim sb As New StringBuilder()

            Dim writer As XmlWriter = XmlWriter.Create(sb, settings)
            writer.WriteStartElement("Attributes")

            For Each key As String In Me.Attributes.Keys
                writer.WriteStartElement("Attribute")

                'Write the Name element
                writer.WriteElementString("Name", key)

                'Write the Value element
                If Me.Attributes(key).IndexOfAny("<'>""&".ToCharArray) > -1 Then
                    'Write value as CDATA
                    writer.WriteStartElement("Value")
                    writer.WriteCData(Me.Attributes(key))
                    writer.WriteEndElement()
                Else
                    'Write value
                    writer.WriteElementString("Value", Me.Attributes(key))
                End If

                'Close the Attribute node
                writer.WriteEndElement()
            Next

            'Close the Attributes node
            writer.WriteEndElement()

            'Close Writer
            writer.Close()

            Return sb.ToString()

        End Function

#End Region

    End Class
End Namespace

