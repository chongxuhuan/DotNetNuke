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
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Entities.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Entities.Modules
    ''' Class	 : SkinControlInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' SkinControlInfo provides the Entity Layer for Skin Controls (SkinObjects)
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	03/28/2008   Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class SkinControlInfo
        Inherits ControlInfo
        Implements IXmlSerializable
        Implements IHydratable

#Region "Private Members"

        Private _SkinControlID As Integer = Null.NullInteger
        Private _PackageID As Integer = Null.NullInteger

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the SkinControl ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SkinControlID() As Integer
            Get
                Return _SkinControlID
            End Get
            Set(ByVal Value As Integer)
                _SkinControlID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ID of the Package for this Desktop Module
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PackageID() As Integer
            Get
                Return _PackageID
            End Get
            Set(ByVal Value As Integer)
                _PackageID = Value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a SkinControlInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            SkinControlID = Null.SetNullInteger(dr("SkinControlID"))
            PackageID = Null.SetNullInteger(dr("PackageID"))
            FillInternal(dr)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return SkinControlID
            End Get
            Set(ByVal value As Integer)
                SkinControlID = value
            End Set
        End Property

#End Region

#Region "IXmlSerializable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an XmlSchema for the SkinControlInfo
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Reads a SkinControlInfo from an XmlReader
        ''' </summary>
        ''' <param name="reader">The XmlReader to use</param>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
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
                End If
            End While
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Writes a SkinControlInfo to an XmlWriter
        ''' </summary>
        ''' <param name="writer">The XmlWriter to use</param>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
            'Write start of main elemenst
            writer.WriteStartElement("moduleControl")

            'write out properties
            WriteXmlInternal(writer)

            'Write end of main element
            writer.WriteEndElement()
        End Sub

#End Region

    End Class

End Namespace

