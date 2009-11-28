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
Imports System.Collections
Imports System.Configuration
Imports System.Data
Imports System.Xml.Serialization
Imports System.Xml.Schema
Imports System.Xml
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Security.Roles

    Public Enum RoleType
        Administrator
        Subscriber
        RegisteredUser
        None
    End Enum

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Security.Roles
    ''' Class:      RoleInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The RoleInfo class provides the Entity Layer Role object
    ''' </summary>
    ''' <history>
    '''     [cnurse]    05/23/2005  made compatible with .NET 2.0
    '''     [cnurse]    01/03/2006  added RoleGroupId property
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class RoleInfo
        Inherits DotNetNuke.Entities.BaseEntityInfo
        Implements IHydratable
        Implements IXmlSerializable

#Region "Private Members"

        Private _RoleID As Integer = Null.NullInteger
        Private _PortalID As Integer
        Private _RoleGroupID As Integer
        Private _RoleName As String
        Private _RoleType As RoleType = Roles.RoleType.None
        Private _Description As String
        Private _ServiceFee As Single
        Private _BillingFrequency As String = "N"
        Private _TrialPeriod As Integer
        Private _TrialFrequency As String = "N"
        Private _BillingPeriod As Integer
        Private _TrialFee As Single
        Private _IsPublic As Boolean
        Private _AutoAssignment As Boolean
        Private _RSVPCode As String
        Private _IconFile As String

        Private _RoleTypeSet As Boolean = Null.NullBoolean

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Role Id
        ''' </summary>
        ''' <value>An Integer representing the Id of the Role</value>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public Property RoleID() As Integer
            Get
                Return _RoleID
            End Get
            Set(ByVal Value As Integer)
                _RoleID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Portal Id for the Role
        ''' </summary>
        ''' <value>An Integer representing the Id of the Portal</value>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the RoleGroup Id
        ''' </summary>
        ''' <value>An Integer representing the Id of the RoleGroup</value>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public Property RoleGroupID() As Integer
            Get
                Return _RoleGroupID
            End Get
            Set(ByVal Value As Integer)
                _RoleGroupID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Role Name
        ''' </summary>
        ''' <value>A string representing the name of the role</value>
        ''' -----------------------------------------------------------------------------
        Public Property RoleName() As String
            Get
                Return _RoleName
            End Get
            Set(ByVal Value As String)
                _RoleName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Role Type
        ''' </summary>
        ''' <value>A enum representing the type of the role</value>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property RoleType() As RoleType
            Get
                If Not _RoleTypeSet Then
                    Dim objPortal As PortalInfo = New PortalController().GetPortal(PortalID)
                    If RoleID = objPortal.AdministratorRoleId Then
                        _RoleType = Roles.RoleType.Administrator
                    ElseIf RoleID = objPortal.RegisteredRoleId Then
                        _RoleType = Roles.RoleType.RegisteredUser
                    ElseIf RoleName = "Subscribers" Then
                        _RoleType = Roles.RoleType.Subscriber
                    End If
                    _RoleTypeSet = True
                End If
                Return _RoleType
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an sets the Description of the Role
        ''' </summary>
        ''' <value>A string representing the description of the role</value>
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
        ''' Gets and sets the Billing Frequency for the role
        ''' </summary>
        ''' <value>A String representing the Billing Frequency of the Role<br/>
        ''' <ul>
        ''' <list>N - None</list>
        ''' <list>O - One time fee</list>
        ''' <list>D - Daily</list>
        ''' <list>W - Weekly</list>
        ''' <list>M - Monthly</list>
        ''' <list>Y - Yearly</list>
        ''' </ul>
        ''' </value>
        ''' -----------------------------------------------------------------------------
        Public Property BillingFrequency() As String
            Get
                Return _BillingFrequency
            End Get
            Set(ByVal Value As String)
                _BillingFrequency = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the fee for the role
        ''' </summary>
        ''' <value>A single number representing the fee for the role</value>
        ''' -----------------------------------------------------------------------------
        Public Property ServiceFee() As Single
            Get
                Return _ServiceFee
            End Get
            Set(ByVal Value As Single)
                _ServiceFee = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Trial Frequency for the role
        ''' </summary>
        ''' <value>A String representing the Trial Frequency of the Role<br/>
        ''' <ul>
        ''' <list>N - None</list>
        ''' <list>O - One time fee</list>
        ''' <list>D - Daily</list>
        ''' <list>W - Weekly</list>
        ''' <list>M - Monthly</list>
        ''' <list>Y - Yearly</list>
        ''' </ul>
        ''' </value>
        ''' -----------------------------------------------------------------------------
        Public Property TrialFrequency() As String
            Get
                Return _TrialFrequency
            End Get
            Set(ByVal Value As String)
                _TrialFrequency = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the length of the trial period
        ''' </summary>
        ''' <value>An integer representing the length of the trial period</value>
        ''' -----------------------------------------------------------------------------
        Public Property TrialPeriod() As Integer
            Get
                Return _TrialPeriod
            End Get
            Set(ByVal Value As Integer)
                _TrialPeriod = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the length of the billing period
        ''' </summary>
        ''' <value>An integer representing the length of the billing period</value>
        ''' -----------------------------------------------------------------------------
        Public Property BillingPeriod() As Integer
            Get
                Return _BillingPeriod
            End Get
            Set(ByVal Value As Integer)
                _BillingPeriod = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the trial fee for the role
        ''' </summary>
        ''' <value>A single number representing the trial fee for the role</value>
        ''' -----------------------------------------------------------------------------
        Public Property TrialFee() As Single
            Get
                Return _TrialFee
            End Get
            Set(ByVal Value As Single)
                _TrialFee = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the role is public
        ''' </summary>
        ''' <value>A boolean (True/False)</value>
        ''' -----------------------------------------------------------------------------
        Public Property IsPublic() As Boolean
            Get
                Return _IsPublic
            End Get
            Set(ByVal Value As Boolean)
                _IsPublic = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether users are automatically assigned to the role
        ''' </summary>
        ''' <value>A boolean (True/False)</value>
        ''' -----------------------------------------------------------------------------
        Public Property AutoAssignment() As Boolean
            Get
                Return _AutoAssignment
            End Get
            Set(ByVal Value As Boolean)
                _AutoAssignment = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the RSVP Code for the role
        ''' </summary>
        ''' <value>A string representing the RSVP Code for the role</value>
        ''' -----------------------------------------------------------------------------
        Public Property RSVPCode() As String
            Get
                Return _RSVPCode
            End Get
            Set(ByVal Value As String)
                _RSVPCode = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Icon File for the role
        ''' </summary>
        ''' <value>A string representing the Icon File for the role</value>
        ''' -----------------------------------------------------------------------------
        Public Property IconFile() As String
            Get
                Return _IconFile
            End Get
            Set(ByVal Value As String)
                _IconFile = Value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a RoleInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	03/17/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub Fill(ByVal dr As System.Data.IDataReader) Implements Entities.Modules.IHydratable.Fill
            RoleID = Null.SetNullInteger(dr("RoleId"))
            PortalID = Null.SetNullInteger(dr("PortalID"))
            RoleGroupID = Null.SetNullInteger(dr("RoleGroupId"))
            RoleName = Null.SetNullString(dr("RoleName"))
            Description = Null.SetNullString(dr("Description"))
            ServiceFee = Null.SetNullSingle(dr("ServiceFee"))
            BillingPeriod = Null.SetNullInteger(dr("BillingPeriod"))
            BillingFrequency = Null.SetNullString(dr("BillingFrequency"))
            TrialFee = Null.SetNullSingle(dr("TrialFee"))
            TrialPeriod = Null.SetNullInteger(dr("TrialPeriod"))
            TrialFrequency = Null.SetNullString(dr("TrialFrequency"))
            IsPublic = Null.SetNullBoolean(dr("IsPublic"))
            AutoAssignment = Null.SetNullBoolean(dr("AutoAssignment"))
            RSVPCode = Null.SetNullString(dr("RSVPCode"))
            IconFile = Null.SetNullString(dr("IconFile"))

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
        Public Overridable Property KeyID() As Integer Implements Entities.Modules.IHydratable.KeyID
            Get
                Return RoleID
            End Get
            Set(ByVal value As Integer)
                RoleID = value
            End Set
        End Property

#End Region

#Region "IXmlSerializable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an XmlSchema for the RoleInfo
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
            Return Nothing
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Reads a RoleInfo from an XmlReader
        ''' </summary>
        ''' <param name="reader">The XmlReader to use</param>
        ''' <history>
        ''' 	[cnurse]	03/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ReadXml(ByVal reader As System.Xml.XmlReader) Implements IXmlSerializable.ReadXml
            While reader.Read()
                If reader.NodeType = XmlNodeType.EndElement Then
                    Exit While
                ElseIf reader.NodeType = XmlNodeType.Whitespace Then
                    Continue While
                ElseIf reader.NodeType = XmlNodeType.Element Then
                    Select Case reader.Name.ToLowerInvariant
                        Case "rolename"
                            RoleName = reader.ReadElementContentAsString()
                        Case "description"
                            Description = reader.ReadElementContentAsString()
                        Case "billingfrequency"
                            BillingFrequency = reader.ReadElementContentAsString()
                            If String.IsNullOrEmpty(BillingFrequency) Then BillingFrequency = "N"
                        Case "billingperiod"
                            BillingPeriod = reader.ReadElementContentAsInt()
                        Case "servicefee"
                            ServiceFee = reader.ReadElementContentAsFloat()
                            If ServiceFee < 0 Then ServiceFee = 0
                        Case "trialfrequency"
                            TrialFrequency = reader.ReadElementContentAsString()
                            If String.IsNullOrEmpty(TrialFrequency) Then TrialFrequency = "N"
                        Case "trialperiod"
                            TrialPeriod = reader.ReadElementContentAsInt()
                        Case "trialfee"
                            TrialFee = reader.ReadElementContentAsFloat()
                            If TrialFee < 0 Then TrialFee = 0
                        Case "ispublic"
                            IsPublic = reader.ReadElementContentAsBoolean()
                        Case "autoassignment"
                            AutoAssignment = reader.ReadElementContentAsBoolean()
                        Case "rsvpcode"
                            RSVPCode = reader.ReadElementContentAsString()
                        Case "iconfile"
                            IconFile = reader.ReadElementContentAsString()
                        Case "roletype"
                            Select Case reader.ReadElementContentAsString()
                                Case "adminrole"
                                    _RoleType = Roles.RoleType.Administrator
                                Case "registeredrole"
                                    _RoleType = Roles.RoleType.RegisteredUser
                                Case "subscriberrole"
                                    _RoleType = Roles.RoleType.Subscriber
                                Case Else
                                    _RoleType = Roles.RoleType.None
                            End Select
                            _RoleTypeSet = True
                    End Select

                End If
            End While
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Writes a RoleInfo to an XmlWriter
        ''' </summary>
        ''' <param name="writer">The XmlWriter to use</param>
        ''' <history>
        ''' 	[cnurse]	03/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter) Implements IXmlSerializable.WriteXml
            'Write start of main elemenst
            writer.WriteStartElement("role")

            'write out properties
            writer.WriteElementString("rolename", RoleName)
            writer.WriteElementString("description", Description)
            writer.WriteElementString("billingfrequency", BillingFrequency)
            writer.WriteElementString("billingperiod", BillingPeriod.ToString())
            writer.WriteElementString("servicefee", ServiceFee.ToString(CultureInfo.InvariantCulture))
            writer.WriteElementString("trialfrequency", TrialFrequency)
            writer.WriteElementString("trialperiod", TrialPeriod.ToString())
            writer.WriteElementString("trialfee", TrialFee.ToString(CultureInfo.InvariantCulture))
            writer.WriteElementString("ispublic", IsPublic.ToString().ToLowerInvariant())
            writer.WriteElementString("autoassignment", AutoAssignment.ToString().ToLowerInvariant())
            writer.WriteElementString("rsvpcode", RSVPCode)
            writer.WriteElementString("iconfile", IconFile)

            Select Case RoleType
                Case RoleType.Administrator
                    writer.WriteElementString("roletype", "adminrole")
                Case RoleType.RegisteredUser
                    writer.WriteElementString("roletype", "registeredrole")
                Case RoleType.Subscriber
                    writer.WriteElementString("roletype", "subscriberrole")
                Case RoleType.None
                    writer.WriteElementString("roletype", "none")
            End Select

            'Write end of main element
            writer.WriteEndElement()
        End Sub

#End Region

    End Class

End Namespace
