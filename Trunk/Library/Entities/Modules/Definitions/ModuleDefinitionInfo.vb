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
Imports System.Collections.Generic
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Entities.Modules.Definitions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Entities.Modules.Definitions
    ''' Class	 : ModuleDefinitionInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModuleDefinitionInfo provides the Entity Layer for Module Definitions
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/11/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class ModuleDefinitionInfo
        Implements IXmlSerializable
        Implements IHydratable

#Region "Private Members"

        Private _DefaultCacheTime As Integer = 0
        Private _DesktopModuleID As Integer = Null.NullInteger
        Private _FriendlyName As String
        Private _ModuleControls As Dictionary(Of String, ModuleControlInfo)
        Private _ModuleDefID As Integer = Null.NullInteger
        Private _Permissions As Dictionary(Of String, PermissionInfo) = New Dictionary(Of String, PermissionInfo)

        Private _TempModuleID As Integer

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Module Definition ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
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
        ''' Gets and sets the Default Cache Time
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DefaultCacheTime() As Integer
            Get
                Return _DefaultCacheTime
            End Get
            Set(ByVal Value As Integer)
                _DefaultCacheTime = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the associated Desktop Module ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DesktopModuleID() As Integer
            Get
                Return _DesktopModuleID
            End Get
            Set(ByVal Value As Integer)
                _DesktopModuleID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Friendly Name
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FriendlyName() As String
            Get
                Return _FriendlyName
            End Get
            Set(ByVal Value As String)
                _FriendlyName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Dictionary of ModuleControls that are part of this definition
        ''' </summary>
        ''' <returns>A Dictionary(Of String, ModuleControlInfo)</returns>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleControls() As Dictionary(Of String, ModuleControlInfo)
            Get
                If _ModuleControls Is Nothing Then
                    LoadControls()
                End If
                Return _ModuleControls
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Dictionary of Permissions that are part of this definition
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Permissions() As Dictionary(Of String, PermissionInfo)
            Get
                Return _Permissions
            End Get
        End Property

#End Region

#Region "Public Methods"

        Public Sub LoadControls()
            If ModuleDefID > Null.NullInteger Then
                _ModuleControls = ModuleControlController.GetModuleControlsByModuleDefinitionID(ModuleDefID)
            Else
                _ModuleControls = New Dictionary(Of String, ModuleControlInfo)
            End If
        End Sub

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a ModuleDefinitionInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            ModuleDefID = Null.SetNullInteger(dr("ModuleDefID"))
            DesktopModuleID = Null.SetNullInteger(dr("DesktopModuleID"))
            DefaultCacheTime = Null.SetNullInteger(dr("DefaultCacheTime"))
            FriendlyName = Null.SetNullString(dr("FriendlyName"))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return ModuleDefID
            End Get
            Set(ByVal value As Integer)
                ModuleDefID = value
            End Set
        End Property

#End Region

#Region "IXmlSerializable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an XmlSchema for the ModuleDefinitionInfo
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
        ''' Reads the ModuleControls from an XmlReader
        ''' </summary>
        ''' <param name="reader">The XmlReader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ReadModuleControls(ByVal reader As XmlReader)
            reader.ReadStartElement("moduleControls")
            Do
                reader.ReadStartElement("moduleControl")

                'Create new ModuleControl object
                Dim moduleControl As New ModuleControlInfo

                'Load it from the Xml
                moduleControl.ReadXml(reader)

                'Add to the collection
                ModuleControls.Add(moduleControl.ControlKey, moduleControl)

            Loop While reader.ReadToNextSibling("moduleControl")
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Reads a ModuleDefinitionInfo from an XmlReader
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
                ElseIf reader.NodeType = XmlNodeType.Element And reader.Name = "moduleControls" Then
                    ReadModuleControls(reader)
                Else
                    Select Case reader.Name
                        Case "friendlyName"
                            FriendlyName = reader.ReadElementContentAsString()
                        Case "defaultCacheTime"
                            Dim elementvalue As String = reader.ReadElementContentAsString()
                            If Not String.IsNullOrEmpty(elementvalue) Then
                                DefaultCacheTime = Integer.Parse(elementvalue)
                            End If
                    End Select
                End If
            End While
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Writes a ModuleDefinitionInfo to an XmlWriter
        ''' </summary>
        ''' <param name="writer">The XmlWriter to use</param>
        ''' <history>
        ''' 	[cnurse]	01/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
            'Write start of main elemenst
            writer.WriteStartElement("moduleDefinition")

            'write out properties
            writer.WriteElementString("friendlyName", FriendlyName)
            writer.WriteElementString("defaultCacheTime", DefaultCacheTime.ToString())

            'Write start of Module Controls
            writer.WriteStartElement("moduleControls")

            'Iterate through controls
            For Each control As ModuleControlInfo In ModuleControls.Values
                control.WriteXml(writer)
            Next

            'Write end of Module Controls
            writer.WriteEndElement()

            'Write end of main element
            writer.WriteEndElement()
        End Sub

#End Region

#Region "Obsolete"

        <Obsolete("No longer used in DotNetNuke 5.0 as new Installer does not need this.")> _
        Public Property TempModuleID() As Integer
            Get
                Return _TempModuleID
            End Get
            Set(ByVal Value As Integer)
                _TempModuleID = Value
            End Set
        End Property

#End Region

    End Class

End Namespace

