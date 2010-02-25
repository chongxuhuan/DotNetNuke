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
Imports System.Xml.Serialization
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Security.Permissions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Security.Permissions
    ''' Class	 : TabPermissionInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' TabPermissionInfo provides the Entity Layer for Tab Permissions
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class TabPermissionInfo
        Inherits PermissionInfoBase
        Implements IHydratable

#Region "Private Members"

        ' local property declarations
        Dim _TabPermissionID As Integer
        Dim _TabID As Integer

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new TabPermissionInfo
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
            _TabPermissionID = Null.NullInteger
            _TabID = Null.NullInteger
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new TabPermissionInfo
        ''' </summary>
        ''' <param name="permission">A PermissionInfo object</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal permission As PermissionInfo)
            Me.New()

            Me.ModuleDefID = permission.ModuleDefID
            Me.PermissionCode = permission.PermissionCode
            Me.PermissionID = permission.PermissionID
            Me.PermissionKey = permission.PermissionKey
            Me.PermissionName = permission.PermissionName
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Tab Permission ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("tabpermissionid")> Public Property TabPermissionID() As Integer
            Get
                Return _TabPermissionID
            End Get
            Set(ByVal Value As Integer)
                _TabPermissionID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Tab ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("tabid")> Public Property TabID() As Integer
            Get
                Return _TabID
            End Get
            Set(ByVal Value As Integer)
                _TabID = Value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a TabPermissionInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill

            'Call the base classes fill method to ppoulate base class proeprties
            MyBase.FillInternal(dr)

            TabPermissionID = Null.SetNullInteger(dr("TabPermissionID"))
            TabID = Null.SetNullInteger(dr("TabID"))
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
        <XmlIgnore()> Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return TabPermissionID
            End Get
            Set(ByVal value As Integer)
                TabPermissionID = value
            End Set
        End Property

#End Region

    End Class

End Namespace
