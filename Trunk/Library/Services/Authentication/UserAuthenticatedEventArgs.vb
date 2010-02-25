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
Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Data
Imports System.Windows.Forms

Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security.Membership

Namespace DotNetNuke.Services.Authentication

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The UserAuthenticatedEventArgs class provides a custom EventArgs object for the
    ''' UserAuthenticated event
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	07/10/2007  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UserAuthenticatedEventArgs
        Inherits EventArgs

#Region "Private Members"

        Private _Authenticated As Boolean = True
        Private _AuthenticationType As String = Null.NullString
        Private _AutoRegister As Boolean = Null.NullBoolean
        Private _LoginStatus As UserLoginStatus = UserLoginStatus.LOGIN_FAILURE
        Private _Message As String = Null.NullString
        Private _Profile As NameValueCollection = New NameValueCollection()
        Private _User As UserInfo = Nothing
        Private _UserToken As String = Null.NullString

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' All properties Constructor.
        ''' </summary>
        ''' <param name="user">The user being authenticated.</param>
        ''' <param name="token">The user token</param>
        ''' <param name="status">The login status.</param>
        ''' <param name="type">The type of Authentication</param>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal user As UserInfo, ByVal token As String, ByVal status As UserLoginStatus, ByVal type As String)
            _User = user
            _LoginStatus = status
            _UserToken = token
            _AuthenticationType = type
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a flag that determines whether the User was authenticated
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/11/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Authenticated() As Boolean
            Get
                Return _Authenticated
            End Get
            Set(ByVal value As Boolean)
                _Authenticated = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Authentication Type
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AuthenticationType() As String
            Get
                Return _AuthenticationType
            End Get
            Set(ByVal value As String)
                _AuthenticationType = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a flag that determines whether the user should be automatically registered
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/16/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AutoRegister() As Boolean
            Get
                Return _AutoRegister
            End Get
            Set(ByVal value As Boolean)
                _AutoRegister = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Login Status
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LoginStatus() As UserLoginStatus
            Get
                Return _LoginStatus
            End Get
            Set(ByVal value As UserLoginStatus)
                _LoginStatus = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Message
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/11/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Message() As String
            Get
                Return _Message
            End Get
            Set(ByVal value As String)
                _Message = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Profile
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/16/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Profile() As NameValueCollection
            Get
                Return _Profile
            End Get
            Set(ByVal value As NameValueCollection)
                _Profile = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the User
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property User() As UserInfo
            Get
                Return _User
            End Get
            Set(ByVal value As UserInfo)
                _User = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the UserToken (the userid or authenticated id)
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property UserToken() As String
            Get
                Return _UserToken
            End Get
            Set(ByVal value As String)
                _UserToken = value
            End Set
        End Property

#End Region

    End Class

End Namespace

