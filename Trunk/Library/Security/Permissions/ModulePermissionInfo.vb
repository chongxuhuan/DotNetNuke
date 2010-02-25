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
Imports DotNetNuke
Imports System.Xml.Serialization
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Security.Permissions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Security.Permissions
    ''' Class	 : ModulePermissionInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModulePermissionInfo provides the Entity Layer for Module Permissions
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class ModulePermissionInfo
        Inherits PermissionInfoBase
        Implements IHydratable

#Region "Private Members"

        ' local property declarations
        Dim _ModulePermissionID As Integer
        Dim _ModuleID As Integer

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new ModulePermissionInfo
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
            _ModulePermissionID = Null.NullInteger
            _ModuleID = Null.NullInteger
        End Sub 'New

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new ModulePermissionInfo
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
        ''' Gets and sets the Module Permission ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("modulepermissionid")> Public Property ModulePermissionID() As Integer
            Get
                Return _ModulePermissionID
            End Get
            Set(ByVal Value As Integer)
                _ModulePermissionID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Module ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlElement("moduleid")> Public Property ModuleID() As Integer
            Get
                Return _ModuleID
            End Get
            Set(ByVal Value As Integer)
                _ModuleID = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Compares if two ModulePermissionInfo objects are equivalent/equal
        ''' </summary>
        ''' <param name="obj">a ModulePermissionObject</param>
        ''' <returns>true if the permissions being passed represents the same permission
        ''' in the current object
        ''' </returns>
        ''' <remarks>
        ''' This function is needed to prevent adding duplicates to the ModulePermissionCollection.
        ''' ModulePermissionCollection.Contains will use this method to check if a given permission
        ''' is already included in the collection.
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	09/01/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Nothing Or Not Me.GetType() Is obj.GetType() Then
                Return False
            End If
            Dim perm As ModulePermissionInfo = CType(obj, ModulePermissionInfo)
            Return (Me.AllowAccess = perm.AllowAccess) And (Me.ModuleID = perm.ModuleID) And _
                (Me.RoleID = perm.RoleID) And (Me.PermissionID = perm.PermissionID)
        End Function

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a ModulePermissionInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements Entities.Modules.IHydratable.Fill

            'Call the base classes fill method to ppoulate base class proeprties
            MyBase.FillInternal(dr)

            ModulePermissionID = Null.SetNullInteger(dr("ModulePermissionID"))
            ModuleID = Null.SetNullInteger(dr("ModuleID"))
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
        <XmlIgnore()> Public Property KeyID() As Integer Implements Entities.Modules.IHydratable.KeyID
            Get
                Return ModulePermissionID
            End Get
            Set(ByVal value As Integer)
                ModulePermissionID = value
            End Set
        End Property

#End Region

    End Class

End Namespace
