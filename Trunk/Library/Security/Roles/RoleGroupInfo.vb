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
Imports System.Collections
Imports System.Configuration
Imports System.Data
Imports System.Xml.Serialization
Imports System.Xml
Imports System.Collections.Generic
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Security.Roles

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Security.Roles
    ''' Class:      RoleGroupInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The RoleGroupInfo class provides the Entity Layer RoleGroup object
    ''' </summary>
    ''' <history>
    '''     [cnurse]    01/03/2006  made compatible with .NET 2.0
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class RoleGroupInfo
        Inherits DotNetNuke.Entities.BaseEntityInfo
        Implements IHydratable
        Implements IXmlSerializable

#Region "Private Members"

        Private _RoleGroupID As Integer = Null.NullInteger
        Private _PortalID As Integer = Null.NullInteger
        Private _RoleGroupName As String
        Private _Description As String
        Private _Roles As Dictionary(Of String, RoleInfo)

#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

        Public Sub New(ByVal roleGroupID As Integer, ByVal portalID As Integer, ByVal loadRoles As Boolean)
            _PortalID = portalID
            _RoleGroupID = roleGroupID
            If loadRoles Then
                GetRoles()
            End If
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the RoleGroup Id
        ''' </summary>
        ''' <value>An Integer representing the Id of the RoleGroup</value>
        ''' -----------------------------------------------------------------------------
        Public Property RoleGroupID() As Integer
            Get
                Return _RoleGroupID
            End Get
            Set(ByVal Value As Integer)
                _RoleGroupID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Portal Id for the RoleGroup
        ''' </summary>
        ''' <value>An Integer representing the Id of the Portal</value>
        ''' -----------------------------------------------------------------------------
        Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the RoleGroup Name
        ''' </summary>
        ''' <value>A string representing the Name of the RoleGroup</value>
        ''' -----------------------------------------------------------------------------
        Public Property RoleGroupName() As String
            Get
                Return _RoleGroupName
            End Get
            Set(ByVal Value As String)
                _RoleGroupName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an sets the Description of the RoleGroup
        ''' </summary>
        ''' <value>A string representing the description of the RoleGroup</value>
        ''' -----------------------------------------------------------------------------
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Roles for this Role Group
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	01/11/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Roles() As Dictionary(Of String, RoleInfo)
            Get
                If _Roles Is Nothing AndAlso RoleGroupID > Null.NullInteger Then
                    GetRoles()
                End If
                Return _Roles
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub GetRoles()
            _Roles = New Dictionary(Of String, RoleInfo)
            For Each role As RoleInfo In New RoleController().GetRolesByGroup(PortalID, RoleGroupID)
                _Roles(role.RoleName) = role
            Next
        End Sub

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a RoleGroupInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	03/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements Entities.Modules.IHydratable.Fill
            RoleGroupID = Null.SetNullInteger(dr("RoleGroupId"))
            PortalID = Null.SetNullInteger(dr("PortalID"))
            RoleGroupName = Null.SetNullString(dr("RoleGroupName"))
            Description = Null.SetNullString(dr("Description"))

            'Fill base class fields
            FillInternal(dr)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	03/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property KeyID() As Integer Implements Entities.Modules.IHydratable.KeyID
            Get
                Return RoleGroupID
            End Get
            Set(ByVal value As Integer)
                RoleGroupID = value
            End Set
        End Property

#End Region

#Region "IXmlSerializable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an XmlSchema for the RoleGroupInfo
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements System.Xml.Serialization.IXmlSerializable.GetSchema
            Return Nothing
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Reads a Roles from an XmlReader
        ''' </summary>
        ''' <param name="reader">The XmlReader to use</param>
        ''' <history>
        ''' 	[cnurse]	03/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ReadRoles(ByVal reader As XmlReader)
            reader.ReadStartElement("roles")
            _Roles = New Dictionary(Of String, RoleInfo)
            Do
                reader.ReadStartElement("role")

                'Create new role object
                Dim role As New RoleInfo

                'Load it from the Xml
                role.ReadXml(reader)

                'Add to the collection
                _Roles.Add(role.RoleName, role)

            Loop While reader.ReadToNextSibling("role")
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Reads a RoleGroupInfo from an XmlReader
        ''' </summary>
        ''' <param name="reader">The XmlReader to use</param>
        ''' <history>
        ''' 	[cnurse]	03/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ReadXml(ByVal reader As System.Xml.XmlReader) Implements System.Xml.Serialization.IXmlSerializable.ReadXml
            While reader.Read()
                If reader.NodeType = XmlNodeType.EndElement Then
                    Exit While
                ElseIf reader.NodeType = XmlNodeType.Whitespace Then
                    Continue While
                ElseIf reader.NodeType = XmlNodeType.Element Then
                    Select Case reader.Name.ToLowerInvariant()
                        Case "roles"
                            If Not reader.IsEmptyElement Then
                                ReadRoles(reader)
                            End If
                        Case "rolegroupname"
                            RoleGroupName = reader.ReadElementContentAsString()
                        Case "description"
                            Description = reader.ReadElementContentAsString()
                    End Select

                End If
            End While
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Writes a RoleGroupInfo to an XmlWriter
        ''' </summary>
        ''' <param name="writer">The XmlWriter to use</param>
        ''' <history>
        ''' 	[cnurse]	03/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter) Implements System.Xml.Serialization.IXmlSerializable.WriteXml
            'Write start of main elemenst
            writer.WriteStartElement("rolegroup")

            'write out properties
            writer.WriteElementString("rolegroupname", RoleGroupName)
            writer.WriteElementString("description", Description)

            'Write start of roles
            writer.WriteStartElement("roles")

            'Iterate through roles
            If Roles IsNot Nothing Then
                For Each role As RoleInfo In Roles.Values
                    role.WriteXml(writer)
                Next
            End If


            'Write end of Roles
            writer.WriteEndElement()

            'Write end of main element
            writer.WriteEndElement()
        End Sub

#End Region

    End Class

End Namespace
