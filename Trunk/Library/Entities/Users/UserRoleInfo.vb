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
Imports System.Configuration
Imports System.Data
Imports DotNetNuke.Security.Roles

Namespace DotNetNuke.Entities.Users

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Entities.Users
    ''' Class:      UserRoleInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The UserRoleInfo class provides Business Layer model for a User/Role
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	01/03/2006	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class UserRoleInfo
        Inherits RoleInfo
        Private _UserRoleID As Integer
        Private _UserID As Integer
        Private _FullName As String
        Private _Email As String
        Private _EffectiveDate As Date
        Private _ExpiryDate As Date
        Private _IsTrialUsed As Boolean
        Private _Subscribed As Boolean

        Public Property UserRoleID() As Integer
            Get
                Return _UserRoleID
            End Get
            Set(ByVal Value As Integer)
                _UserRoleID = Value
            End Set
        End Property

        Public Property UserID() As Integer
            Get
                Return _UserID
            End Get
            Set(ByVal Value As Integer)
                _UserID = Value
            End Set
        End Property

        Public Property FullName() As String
            Get
                Return _FullName
            End Get
            Set(ByVal Value As String)
                _FullName = Value
            End Set
        End Property

        Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal Value As String)
                _Email = Value
            End Set
        End Property

        Public Property EffectiveDate() As Date
            Get
                Return _EffectiveDate
            End Get
            Set(ByVal Value As Date)
                _EffectiveDate = Value
            End Set
        End Property

        Public Property ExpiryDate() As Date
            Get
                Return _ExpiryDate
            End Get
            Set(ByVal Value As Date)
                _ExpiryDate = Value
            End Set
        End Property

        Public Property IsTrialUsed() As Boolean
            Get
                Return _IsTrialUsed
            End Get
            Set(ByVal Value As Boolean)
                _IsTrialUsed = Value
            End Set
        End Property

        Public Property Subscribed() As Boolean
            Get
                Return _Subscribed
            End Get
            Set(ByVal Value As Boolean)
                _Subscribed = Value
            End Set
        End Property

        Public Overrides Sub Fill(ByVal dr As System.Data.IDataReader)
            'Fill base class properties
            MyBase.Fill(dr)

            'Fill this class properties
            UserRoleID = Null.SetNullInteger(dr("UserRoleID"))
            UserID = Null.SetNullInteger(dr("UserID"))
            FullName = Null.SetNullString(dr("DisplayName"))
            Email = Null.SetNullString(dr("Email"))
            EffectiveDate = Null.SetNullDateTime(dr("EffectiveDate"))
            ExpiryDate = Null.SetNullDateTime(dr("ExpiryDate"))
            IsTrialUsed = Null.SetNullBoolean(dr("IsTrialUsed"))

            If UserRoleID > Null.NullInteger Then
                Subscribed = True
            End If
        End Sub

    End Class


End Namespace
