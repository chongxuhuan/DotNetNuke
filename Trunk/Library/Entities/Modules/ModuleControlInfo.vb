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
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Namespace DotNetNuke.Entities.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Entities.Modules
    ''' Class	 : ModuleControlInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModuleControlInfo provides the Entity Layer for Module Controls
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class ModuleControlInfo
        Inherits ControlInfo
        Implements IXmlSerializable
        Implements IHydratable

#Region "Private Members"

        Private _ModuleControlID As Integer = Null.NullInteger
        Private _ModuleDefID As Integer = Null.NullInteger
        Private _ControlTitle As String
        Private _ControlType As SecurityAccessLevel = SecurityAccessLevel.Anonymous
        Private _IconFile As String
        Private _HelpURL As String
        Private _ViewOrder As Integer

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Module Control ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ModuleControlID() As Integer
            Get
                Return _ModuleControlID
            End Get
            Set(ByVal Value As Integer)
                _ModuleControlID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Module Definition ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ModuleDefID() As Integer
            Get
                Return _ModuleDefID
            End Get
            Set(ByVal Value As Integer)
                _ModuleDefID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Control Title
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ControlTitle() As String
            Get
                Return _ControlTitle
            End Get
            Set(ByVal Value As String)
                _ControlTitle = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Control Type
        ''' </summary>
        ''' <returns>A SecurityAccessLevel</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ControlType() As SecurityAccessLevel
            Get
                Return _ControlType
            End Get
            Set(ByVal Value As SecurityAccessLevel)
                _ControlType = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Icon  Source
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IconFile() As String
            Get
                Return _IconFile
            End Get
            Set(ByVal Value As String)
                _IconFile = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Help URL
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property HelpURL() As String
            Get
                Return _HelpURL
            End Get
            Set(ByVal Value As String)
                _HelpURL = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the View Order
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ViewOrder() As Integer
            Get
                Return _ViewOrder
            End Get
            Set(ByVal Value As Integer)
                _ViewOrder = Value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a ModuleControlInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            ModuleControlID = Null.SetNullInteger(dr("ModuleControlID"))
            FillInternal(dr)
            ModuleDefID = Null.SetNullInteger(dr("ModuleDefID"))
            ControlTitle = Null.SetNullString(dr("ControlTitle"))
            IconFile = Null.SetNullString(dr("IconFile"))
            HelpURL = Null.SetNullString(dr("HelpUrl"))
            ControlType = CType([Enum].Parse(GetType(SecurityAccessLevel), Null.SetNullString(dr("ControlType"))), SecurityAccessLevel)
            ViewOrder = Null.SetNullInteger(dr("ViewOrder"))
            'Call the base classes fill method to populate base class proeprties
            MyBase.FillInternal(dr)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return ModuleControlID
            End Get
            Set(ByVal value As Integer)
                ModuleControlID = value
            End Set
        End Property

#End Region

#Region "IXmlSerializable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an XmlSchema for the ModuleControlInfo
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Reads a ModuleControlInfo from an XmlReader
        ''' </summary>
        ''' <param name="reader">The XmlReader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
            While reader.Read()
                If reader.NodeType = XmlNodeType.EndElement Then
                    Exit While
                ElseIf reader.NodeType = XmlNodeType.Whitespace Then
                    Continue While
                Else
                    ReadXmlInternal(reader)
                    Select Case reader.Name
                        Case "controlTitle"
                            ControlTitle = reader.ReadElementContentAsString()
                        Case "controlType"
                            ControlType = CType([Enum].Parse(GetType(SecurityAccessLevel), reader.ReadElementContentAsString()), SecurityAccessLevel)
                        Case "iconFile"
                            IconFile = reader.ReadElementContentAsString()
                        Case "helpUrl"
                            HelpURL = reader.ReadElementContentAsString()
                        Case "viewOrder"
                            Dim elementvalue As String = reader.ReadElementContentAsString()
                            If Not String.IsNullOrEmpty(elementvalue) Then
                                ViewOrder = Integer.Parse(elementvalue)
                            End If
                    End Select
                End If
            End While
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Writes a ModuleControlInfo to an XmlWriter
        ''' </summary>
        ''' <param name="writer">The XmlWriter to use</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
            'Write start of main elemenst
            writer.WriteStartElement("moduleControl")

            'write out properties
            WriteXmlInternal(writer)
            writer.WriteElementString("controlTitle", ControlTitle)
            writer.WriteElementString("controlType", ControlType.ToString())
            writer.WriteElementString("iconFile", IconFile)
            writer.WriteElementString("helpUrl", HelpURL)
            If ViewOrder > Null.NullInteger Then
                writer.WriteElementString("viewOrder", ViewOrder.ToString)
            End If

            'Write end of main element
            writer.WriteEndElement()
        End Sub

#End Region

    End Class

End Namespace

