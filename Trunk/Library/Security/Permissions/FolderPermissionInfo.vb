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

    <Serializable()> Public Class FolderPermissionInfo
        Inherits PermissionInfoBase
        Implements IHydratable

#Region "Private Members"

        ' local property declarations
        Private _folderPermissionID As Integer
        Private _folderPath As String
        Private _portalID As Integer
        Private _folderID As Integer

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new FolderPermissionInfo
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
            _folderPermissionID = Null.NullInteger
            _folderPath = Null.NullString
            _portalID = Null.NullInteger
            _folderID = Null.NullInteger
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new FolderPermissionInfo
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

        <XmlIgnore()> Public Property FolderPermissionID() As Integer
            Get
                Return _folderPermissionID
            End Get
            Set(ByVal Value As Integer)
                _folderPermissionID = Value
            End Set
        End Property

        <XmlIgnore()> Public Property FolderID() As Integer
            Get
                Return _folderID
            End Get
            Set(ByVal Value As Integer)
                _folderID = Value
            End Set
        End Property

        <XmlIgnore()> Public Property PortalID() As Integer
            Get
                Return _portalID
            End Get
            Set(ByVal Value As Integer)
                _portalID = Value
            End Set
        End Property

        <XmlElement("folderpath")> Public Property FolderPath() As String
            Get
                Return _folderPath
            End Get
            Set(ByVal Value As String)
                _folderPath = Value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a FolderPermissionInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	05/23/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements Entities.Modules.IHydratable.Fill

            'Call the base classes fill method to ppoulate base class proeprties
            MyBase.FillInternal(dr)

            FolderPermissionID = Null.SetNullInteger(dr("FolderPermissionID"))
            FolderID = Null.SetNullInteger(dr("FolderID"))
            PortalID = Null.SetNullInteger(dr("PortalID"))
            FolderPath = Null.SetNullString(dr("FolderPath"))

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	05/23/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public Property KeyID() As Integer Implements Entities.Modules.IHydratable.KeyID
            Get
                Return FolderPermissionID
            End Get
            Set(ByVal value As Integer)
                FolderPermissionID = value
            End Set
        End Property

#End Region

    End Class


End Namespace
