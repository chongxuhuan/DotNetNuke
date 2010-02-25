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
Imports DotNetNuke.Entities.Modules
Imports System.Xml.Serialization

Namespace DotNetNuke.Security.Permissions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Security.Permissions
    ''' Class	 : DesktopModulePermissionInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' DesktopModulePermissionInfo provides the Entity Layer for DesktopModulePermissionInfo 
    ''' Permissions
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/15/2008   Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> _
    Public Class DesktopModulePermissionInfo
        Inherits PermissionInfoBase
        Implements IHydratable

#Region "Private Members"

        ' local property declarations
        Dim _DesktopModulePermissionID As Integer
        Dim _PortalDesktopModuleID As Integer

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new DesktopModulePermissionInfo
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
            _DesktopModulePermissionID = Null.NullInteger
            _PortalDesktopModuleID = Null.NullInteger
        End Sub 'New

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new DesktopModulePermissionInfo
        ''' </summary>
        ''' <param name="permission">A PermissionInfo object</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
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
        ''' Gets and sets the DesktopModule Permission ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DesktopModulePermissionID() As Integer
            Get
                Return _DesktopModulePermissionID
            End Get
            Set(ByVal Value As Integer)
                _DesktopModulePermissionID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the PortalDesktopModule ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PortalDesktopModuleID() As Integer
            Get
                Return _PortalDesktopModuleID
            End Get
            Set(ByVal Value As Integer)
                _PortalDesktopModuleID = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Compares if two DesktopModulePermissionInfo objects are equivalent/equal
        ''' </summary>
        ''' <param name="obj">a DesktopModulePermissionObject</param>
        ''' <returns>true if the permissions being passed represents the same permission
        ''' in the current object
        ''' </returns>
        ''' <remarks>
        ''' This function is needed to prevent adding duplicates to the DesktopModulePermissionCollection.
        ''' DesktopModulePermissionCollection.Contains will use this method to check if a given permission
        ''' is already included in the collection.
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Nothing Or Not Me.GetType() Is obj.GetType() Then
                Return False
            End If
            Dim perm As DesktopModulePermissionInfo = CType(obj, DesktopModulePermissionInfo)
            Return (Me.AllowAccess = perm.AllowAccess) And (Me.PortalDesktopModuleID = perm.PortalDesktopModuleID) And _
                (Me.RoleID = perm.RoleID) And (Me.PermissionID = perm.PermissionID)
        End Function

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a DesktopModulePermissionInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements Entities.Modules.IHydratable.Fill

            'Call the base classes fill method to populate base class proeprties
            MyBase.FillInternal(dr)

            DesktopModulePermissionID = Null.SetNullInteger(dr("DesktopModulePermissionID"))
            PortalDesktopModuleID = Null.SetNullInteger(dr("PortalDesktopModuleID"))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property KeyID() As Integer Implements Entities.Modules.IHydratable.KeyID
            Get
                Return DesktopModulePermissionID
            End Get
            Set(ByVal value As Integer)
                DesktopModulePermissionID = value
            End Set
        End Property

#End Region

    End Class

End Namespace
