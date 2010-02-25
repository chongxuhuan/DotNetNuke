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

Imports System.Text
Imports System.Xml

Namespace DotNetNuke.Services.Log.EventLog

    <Serializable()> Public Class LogDetailInfo

#Region "Private Members"

        Private _PropertyName As String
        Private _PropertyValue As String

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New("", "")
        End Sub

        Public Sub New(ByVal name As String, ByVal value As String)
            _PropertyName = name
            _PropertyValue = value
        End Sub

#End Region

#Region "Public Properties"

        Public Property PropertyName() As String
            Get
                Return _PropertyName
            End Get
            Set(ByVal Value As String)
                _PropertyName = Value
            End Set
        End Property
        Public Property PropertyValue() As String
            Get
                Return _PropertyValue
            End Get
            Set(ByVal Value As String)
                _PropertyValue = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        Public Sub ReadXml(ByVal reader As XmlReader)
            reader.ReadStartElement("PropertyName")
            PropertyName = reader.ReadString()
            reader.ReadEndElement()
            If Not reader.IsEmptyElement Then
                reader.ReadStartElement("PropertyValue")
                PropertyValue = reader.ReadString()
                reader.ReadEndElement()
            Else
                reader.Read()
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder
            sb.Append("<b>")
            sb.Append(PropertyName)
            sb.Append("</b>: ")
            sb.Append(PropertyValue)
            sb.Append(";&nbsp;")
            Return sb.ToString()
        End Function

        Public Sub WriteXml(ByVal writer As XmlWriter)
            writer.WriteStartElement("LogProperty")
            writer.WriteElementString("PropertyName", PropertyName)
            writer.WriteElementString("PropertyValue", PropertyValue)
            writer.WriteEndElement()
        End Sub

#End Region

    End Class

End Namespace




