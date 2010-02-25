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

Namespace DotNetNuke.Security.Permissions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Security.Permissions
    ''' Class	 : PermissionInfoBase
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' PermissionInfoBase provides a base class for PermissionInfo classes
    ''' </summary>
    ''' <remarks>All Permission calsses have  a common set of properties
    '''   - AllowAccess
    '''   - RoleID
    '''   - RoleName
    '''   - UserID
    '''   - Username
    '''   - DisplayName
    ''' 
    ''' and these are implemented in this base class
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public MustInherit Class PermissionInfoBase
        Inherits PermissionInfo

#Region "Private Members"

        Private _AllowAccess As Boolean
        Private _DisplayName As String
        Private _RoleID As Integer
        Private _RoleName As String
        Private _UserID As Integer
        Private _Username As String

#End Region

#Region "Constructors"

        Public Sub New()
            MyBase.New()
            _RoleID = Integer.Parse(glbRoleNothing)
            _AllowAccess = False
            _RoleName = Null.NullString
            _UserID = Null.NullInteger
            _Username = Null.NullString
            _DisplayName = Null.NullString
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets  aflag that indicates whether the user or role has permission
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("allowaccess")> Public Property AllowAccess() As Boolean
            Get
                Return _AllowAccess
            End Get
            Set(ByVal Value As Boolean)
                _AllowAccess = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the User's DisplayName
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("displayname")> Public Property DisplayName() As String
            Get
                Return _DisplayName
            End Get
            Set(ByVal Value As String)
                _DisplayName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Role ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("roleid")> Public Property RoleID() As Integer
            Get
                Return _RoleID
            End Get
            Set(ByVal Value As Integer)
                _RoleID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Role Name
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("rolename")> Public Property RoleName() As String
            Get
                Return _RoleName
            End Get
            Set(ByVal Value As String)
                _RoleName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the User ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("userid")> Public Property UserID() As Integer
            Get
                Return _UserID
            End Get
            Set(ByVal Value As Integer)
                _UserID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the User Name
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("username")> Public Property Username() As String
            Get
                Return _Username
            End Get
            Set(ByVal Value As String)
                _Username = Value
            End Set
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FillInternal fills the PermissionInfoBase from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub FillInternal(ByVal dr As System.Data.IDataReader)

            'Call the base classes fill method to populate base class proeprties
            MyBase.FillInternal(dr)

            UserID = Null.SetNullInteger(dr("UserID"))
            Username = Null.SetNullString(dr("Username"))
            DisplayName = Null.SetNullString(dr("DisplayName"))
            If UserID = Null.NullInteger Then
                RoleID = Null.SetNullInteger(dr("RoleID"))
                RoleName = Null.SetNullString(dr("RoleName"))
            Else
                RoleID = Integer.Parse(glbRoleNothing)
                RoleName = ""
            End If
            AllowAccess = Null.SetNullBoolean(dr("AllowAccess"))
        End Sub

#End Region

    End Class

End Namespace
