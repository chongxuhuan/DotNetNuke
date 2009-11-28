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
Imports System.IO
Imports System.Text
Imports System.Xml

Namespace DotNetNuke.Services.Log.EventLog

    <Serializable()> Public Class LogInfo

#Region "Private Members"

        Private _LogGUID As String
        Private _LogFileID As String
        Private _LogTypeKey As String
        Private _LogUserID As Integer
        Private _LogUserName As String
        Private _LogPortalID As Integer
        Private _LogPortalName As String
        Private _LogCreateDate As Date
        Private _LogCreateDateNum As Long
        Private _BypassBuffering As Boolean
        Private _LogServerName As String
        Private _LogProperties As LogProperties
        Private _LogConfigID As String

#End Region

#Region "Constructors"

        Public Sub New()
            _LogGUID = Guid.NewGuid.ToString
            _BypassBuffering = False
            _LogProperties = New LogProperties
            _LogPortalID = -1
            _LogPortalName = ""
            _LogUserID = -1
            _LogUserName = ""
        End Sub

        Public Sub New(ByVal content As String)
            Me.New()
            Me.Deserialize(content)
        End Sub

#End Region

#Region "Properties"

        Public Property LogGUID() As String
            Get
                Return _LogGUID
            End Get
            Set(ByVal Value As String)
                _LogGUID = Value
            End Set
        End Property

        Public Property LogFileID() As String
            Get
                Return _LogFileID
            End Get
            Set(ByVal Value As String)
                _LogFileID = Value
            End Set
        End Property

        Public Property LogTypeKey() As String
            Get
                Return _LogTypeKey
            End Get
            Set(ByVal Value As String)
                _LogTypeKey = Value
            End Set
        End Property

        Public Property LogUserID() As Integer
            Get
                Return _LogUserID
            End Get
            Set(ByVal Value As Integer)
                _LogUserID = Value
            End Set
        End Property

        Public Property LogUserName() As String
            Get
                Return _LogUserName
            End Get
            Set(ByVal Value As String)
                _LogUserName = Value
            End Set
        End Property
        Public Property LogPortalID() As Integer
            Get
                Return _LogPortalID
            End Get
            Set(ByVal Value As Integer)
                _LogPortalID = Value
            End Set
        End Property
        Public Property LogPortalName() As String
            Get
                Return _LogPortalName
            End Get
            Set(ByVal Value As String)
                _LogPortalName = Value
            End Set
        End Property
        Public Property LogCreateDate() As Date
            Get
                Return _LogCreateDate
            End Get
            Set(ByVal Value As Date)
                _LogCreateDate = Value
            End Set
        End Property
        Public Property LogCreateDateNum() As Long
            Get
                Return _LogCreateDateNum
            End Get
            Set(ByVal Value As Long)
                _LogCreateDateNum = Value
            End Set
        End Property
        Public Property LogProperties() As LogProperties
            Get
                Return _LogProperties
            End Get
            Set(ByVal Value As LogProperties)
                _LogProperties = Value
            End Set
        End Property
        Public Property BypassBuffering() As Boolean
            Get
                Return _BypassBuffering
            End Get
            Set(ByVal Value As Boolean)
                _BypassBuffering = Value
            End Set
        End Property

        Public Property LogServerName() As String
            Get
                Return _LogServerName
            End Get
            Set(ByVal Value As String)
                _LogServerName = Value
            End Set
        End Property

        Public Property LogConfigID() As String
            Get
                Return _LogConfigID
            End Get
            Set(ByVal Value As String)
                _LogConfigID = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        Public Sub AddProperty(ByVal PropertyName As String, ByVal PropertyValue As String)
            Try
                If PropertyValue Is Nothing Then
                    PropertyValue = String.Empty
                End If
                If PropertyName.Length > 50 Then
                    PropertyName = Left(PropertyName, 50)
                End If
                If PropertyValue.Length > 500 Then
                    PropertyValue = "(TRUNCATED TO 500 CHARS): " + Left(PropertyValue, 500)
                End If
                Dim objLogDetailInfo As New LogDetailInfo
                objLogDetailInfo.PropertyName = PropertyName
                objLogDetailInfo.PropertyValue = PropertyValue
                _LogProperties.Add(objLogDetailInfo)
            Catch exc As Exception
                LogException(exc)
            End Try
        End Sub

        Public Sub Deserialize(ByVal content As String)
            Using reader As XmlReader = XmlReader.Create(New StringReader(content))
                If reader.Read() Then
                    ReadXml(reader)
                End If
                reader.Close()
            End Using
        End Sub

        Public Sub ReadXml(ByVal reader As XmlReader)
            If reader.HasAttributes() Then
                While reader.MoveToNextAttribute()
                    Select Case reader.Name
                        Case "LogGUID"
                            LogGUID = reader.ReadContentAsString()
                        Case "LogFileID"
                            LogFileID = reader.ReadContentAsString()
                        Case "LogTypeKey"
                            LogTypeKey = reader.ReadContentAsString()
                        Case "LogUserID"
                            LogUserID = reader.ReadContentAsInt()
                        Case "LogUserName"
                            LogUserName = reader.ReadContentAsString()
                        Case "LogPortalID"
                            LogPortalID = reader.ReadContentAsInt()
                        Case "LogPortalName"
                            LogPortalName = reader.ReadContentAsString()
                        Case "LogCreateDate"
                            LogCreateDate = Date.Parse(reader.ReadContentAsString())
                        Case "LogCreateDateNum"
                            LogCreateDateNum = reader.ReadContentAsLong()
                        Case "BypassBuffering"
                            BypassBuffering = Boolean.Parse(reader.ReadContentAsString())
                        Case "LogServerName"
                            LogServerName = reader.ReadContentAsString()
                        Case "LogConfigID"
                            LogConfigID = reader.ReadContentAsString()
                    End Select
                End While
            End If

            'Check for LogProperties child node
            reader.Read()
            If reader.NodeType = XmlNodeType.Element And reader.LocalName = "LogProperties" Then
                reader.ReadStartElement("LogProperties")
                If reader.ReadState <> ReadState.EndOfFile And reader.NodeType <> XmlNodeType.None And reader.LocalName <> "" Then
                    LogProperties.ReadXml(reader)
                End If
            End If
        End Sub

        Public Shared Function IsSystemType(ByVal PropName As String) As Boolean
            Select Case PropName
                Case "LogGUID", "LogFileID", "LogTypeKey", "LogCreateDate", "LogCreateDateNum", "LogPortalID", "LogPortalName", "LogUserID", "LogUserName", "BypassBuffering", "LogServerName"
                    Return True
            End Select

            Return False
        End Function

        Public Function Serialize() As String
            Dim settings As New XmlWriterSettings()
            settings.ConformanceLevel = ConformanceLevel.Fragment
            settings.OmitXmlDeclaration = True

            Dim sb As New StringBuilder()

            Dim writer As XmlWriter = XmlWriter.Create(sb, settings)
            WriteXml(writer)
            writer.Close()

            Return sb.ToString()

        End Function

        Public Overrides Function ToString() As String
            Dim str As New System.Text.StringBuilder
            str.Append("<b>LogGUID:</b> " + LogGUID + "<br>")
            str.Append("<b>LogType:</b> " + LogTypeKey + "<br>")
            str.Append("<b>UserID:</b> " + LogUserID.ToString + "<br>")
            str.Append("<b>Username:</b> " + LogUserName + "<br>")
            str.Append("<b>PortalID:</b> " + LogPortalID.ToString + "<br>")
            str.Append("<b>PortalName:</b> " + LogPortalName + "<br>")
            str.Append("<b>CreateDate:</b> " + LogCreateDate.ToString + "<br>")
            str.Append("<b>ServerName:</b> " + LogServerName + "<br>")
            str.Append(LogProperties.ToString)
            Return str.ToString
        End Function

        Public Sub WriteXml(ByVal writer As XmlWriter)
            writer.WriteStartElement("log")
            writer.WriteAttributeString("LogGUID", LogGUID)
            writer.WriteAttributeString("LogFileID", LogFileID)
            writer.WriteAttributeString("LogTypeKey", LogTypeKey)
            writer.WriteAttributeString("LogUserID", LogUserID.ToString())
            writer.WriteAttributeString("LogUserName", LogUserName)
            writer.WriteAttributeString("LogPortalID", LogPortalID.ToString())
            writer.WriteAttributeString("LogPortalName", LogPortalName)
            writer.WriteAttributeString("LogCreateDate", LogCreateDate.ToString())
            writer.WriteAttributeString("LogCreateDateNum", LogCreateDateNum.ToString())
            writer.WriteAttributeString("BypassBuffering", BypassBuffering.ToString())
            writer.WriteAttributeString("LogServerName", LogServerName)
            writer.WriteAttributeString("LogConfigID", LogConfigID)

            LogProperties.WriteXml(writer)

            writer.WriteEndElement()

        End Sub

#End Region

    End Class

End Namespace




