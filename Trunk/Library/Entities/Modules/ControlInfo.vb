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
    ''' Class	 : ControlInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ControlInfo provides a base class for Module Controls and SkinControls
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	03/28/2008   Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public MustInherit Class ControlInfo
        Inherits BaseEntityInfo

#Region "Private Members"

        Private _ControlKey As String
        Private _ControlSrc As String
        Private _SupportsPartialRendering As Boolean = Null.NullBoolean

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Control Key
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ControlKey() As String
            Get
                Return _ControlKey
            End Get
            Set(ByVal Value As String)
                _ControlKey = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Control Source
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ControlSrc() As String
            Get
                Return _ControlSrc
            End Get
            Set(ByVal Value As String)
                _ControlSrc = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a flag that determines whether the control support the AJAX
        ''' Update Panel
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SupportsPartialRendering() As Boolean
            Get
                Return _SupportsPartialRendering
            End Get
            Set(ByVal value As Boolean)
                _SupportsPartialRendering = value
            End Set
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a ControlInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	03/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub FillInternal(ByVal dr As System.Data.IDataReader)
            'Call EntityBaseInfo's implementation
            MyBase.FillInternal(dr)

            ControlKey = Null.SetNullString(dr("ControlKey"))
            ControlSrc = Null.SetNullString(dr("ControlSrc"))
            SupportsPartialRendering = Null.SetNullBoolean(dr("SupportsPartialRendering"))
        End Sub

        Protected Sub ReadXmlInternal(ByVal reader As XmlReader)
            Select Case reader.Name
                Case "controlKey"
                    ControlKey = reader.ReadElementContentAsString()
                Case "controlSrc"
                    ControlSrc = reader.ReadElementContentAsString()
                Case "supportsPartialRendering"
                    Dim elementvalue As String = reader.ReadElementContentAsString()
                    If Not String.IsNullOrEmpty(elementvalue) Then
                        SupportsPartialRendering = Boolean.Parse(elementvalue)
                    End If
            End Select
        End Sub

        Protected Sub WriteXmlInternal(ByVal writer As XmlWriter)
            'write out properties
            writer.WriteElementString("controlKey", ControlKey)
            writer.WriteElementString("controlSrc", ControlSrc)
            writer.WriteElementString("supportsPartialRendering", SupportsPartialRendering.ToString())
        End Sub

#End Region

    End Class

End Namespace

