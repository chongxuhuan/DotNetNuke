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
Imports System.Xml.Serialization

Namespace DotNetNuke.Security.Permissions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Security.Permissions
    ''' Class	 : PermissionInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' PermissionInfo provides the Entity Layer for Permissions
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class PermissionInfo
        Inherits Entities.BaseEntityInfo


#Region "Private Members"

        Private _ModuleDefID As Integer
        Private _PermissionCode As String
        Private _PermissionID As Integer
        Private _PermissionKey As String
        Private _PermissionName As String

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Mdoule Definition ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public Property ModuleDefID() As Integer
            Get
                Return _ModuleDefID
            End Get
            Set(ByVal Value As Integer)
                _ModuleDefID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Permission Code
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("permissioncode")> Public Property PermissionCode() As String
            Get
                Return _PermissionCode
            End Get
            Set(ByVal Value As String)
                _PermissionCode = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Permission ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("permissionid")> Public Property PermissionID() As Integer
            Get
                Return _PermissionID
            End Get
            Set(ByVal Value As Integer)
                _PermissionID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Permission Key
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("permissionkey")> Public Property PermissionKey() As String
            Get
                Return _PermissionKey
            End Get
            Set(ByVal Value As String)
                _PermissionKey = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Permission Name
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public Property PermissionName() As String
            Get
                Return _PermissionName
            End Get
            Set(ByVal Value As String)
                _PermissionName = Value
            End Set
        End Property
#End Region

#Region "Protected methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillInternal fills a PermissionInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub FillInternal(ByVal dr As System.Data.IDataReader)
            'Call EntityBaseInfo's implementation
            MyBase.FillInternal(dr)

            PermissionID = Null.SetNullInteger(dr("PermissionID"))
            ModuleDefID = Null.SetNullInteger(dr("ModuleDefID"))
            PermissionCode = Null.SetNullString(dr("PermissionCode"))
            PermissionKey = Null.SetNullString(dr("PermissionKey"))
            PermissionName = Null.SetNullString(dr("PermissionName"))
        End Sub

#End Region

    End Class

End Namespace
