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
Imports System.IO

Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.UI.Skins

Namespace DotNetNuke.Entities.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 :  DotNetNuke
    ''' Namespace:  DotNetNuke.Entities.Modules
    ''' Class	 :  UserUserControlBase
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The UserUserControlBase class defines a custom base class for the User Control.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	03/02/2007
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UserUserControlBase
        Inherits UserModuleBase

#Region "Delegates"

        Public Delegate Sub UserCreatedEventHandler(ByVal sender As Object, ByVal e As UserCreatedEventArgs)
        Public Delegate Sub UserDeletedEventHandler(ByVal sender As Object, ByVal e As UserDeletedEventArgs)
        Public Delegate Sub UserUpdateErrorEventHandler(ByVal sender As Object, ByVal e As UserUpdateErrorArgs)

#End Region

#Region "Events"

        Public Event UserCreated As UserCreatedEventHandler
        Public Event UserCreateCompleted As UserCreatedEventHandler
        Public Event UserDeleted As UserDeletedEventHandler
        Public Event UserDeleteError As UserUpdateErrorEventHandler
        Public Event UserUpdated As EventHandler
        Public Event UserUpdateCompleted As EventHandler
        Public Event UserUpdateError As UserUpdateErrorEventHandler

#End Region

#Region "Event Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the UserCreateCompleted Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/13/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnUserCreateCompleted(ByVal e As UserCreatedEventArgs)
            RaiseEvent UserCreateCompleted(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the UserCreated Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnUserCreated(ByVal e As UserCreatedEventArgs)
            RaiseEvent UserCreated(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the UserDeleted Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnUserDeleted(ByVal e As UserDeletedEventArgs)
            RaiseEvent UserDeleted(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the UserDeleteError Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	11/30/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnUserDeleteError(ByVal e As UserUpdateErrorArgs)
            RaiseEvent UserDeleteError(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the UserUpdated Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnUserUpdated(ByVal e As EventArgs)
            RaiseEvent UserUpdated(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the UserUpdated Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnUserUpdateCompleted(ByVal e As EventArgs)
            RaiseEvent UserUpdateCompleted(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the UserUpdateError Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/07/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnUserUpdateError(ByVal e As UserUpdateErrorArgs)
            RaiseEvent UserUpdateError(Me, e)
        End Sub

#End Region

#Region "Event Args"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The BaseUserEventArgs class provides a base for User EventArgs classes
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class BaseUserEventArgs

            Private _userName As String
            Private _userId As Integer

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Constructs a new BaseUserEventArgs
            ''' </summary>
            ''' <history>
            ''' 	[cnurse]	02/07/2007  created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New()
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Gets and sets the Id of the User
            ''' </summary>
            ''' <history>
            ''' 	[cnurse]	02/07/2007  created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property UserId() As Integer
                Get
                    Return _userId
                End Get
                Set(ByVal Value As Integer)
                    _userId = Value
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Gets and sets the username of the User
            ''' </summary>
            ''' <history>
            ''' 	[cnurse]	02/07/2007  created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property UserName() As String
                Get
                    Return _userName
                End Get
                Set(ByVal Value As String)
                    _userName = Value
                End Set
            End Property

        End Class

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UserCreatedEventArgs class provides a customised EventArgs class for
        ''' the UserCreated Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/08/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class UserCreatedEventArgs

            Private _NewUser As UserInfo
            Private _CreateStatus As UserCreateStatus = UserCreateStatus.Success
            Private _Notify As Boolean = False

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Constructs a new UserCreatedEventArgs
            ''' </summary>
            ''' <param name="newUser">The newly Created User</param>
            ''' <history>
            ''' 	[cnurse]	03/08/2006  Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal newUser As UserInfo)
                _NewUser = newUser
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Gets and sets the Create Status
            ''' </summary>
            ''' <history>
            ''' 	[cnurse]	03/08/2006  Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property CreateStatus() As UserCreateStatus
                Get
                    Return _CreateStatus
                End Get
                Set(ByVal Value As UserCreateStatus)
                    _CreateStatus = Value
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Gets and sets the New User
            ''' </summary>
            ''' <history>
            ''' 	[cnurse]	03/08/2006  Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property NewUser() As UserInfo
                Get
                    Return _NewUser
                End Get
                Set(ByVal Value As UserInfo)
                    _NewUser = Value
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Gets and sets a flag whether to Notify the new User of the Creation
            ''' </summary>
            ''' <history>
            ''' 	[cnurse]	03/08/2006  Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property Notify() As Boolean
                Get
                    Return _Notify
                End Get
                Set(ByVal Value As Boolean)
                    _Notify = Value
                End Set
            End Property

        End Class

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UserDeletedEventArgs class provides a customised EventArgs class for
        ''' the UserDeleted Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/08/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class UserDeletedEventArgs
            Inherits BaseUserEventArgs

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Constructs a new UserDeletedEventArgs
            ''' </summary>
            ''' <param name="id">The Id of the User</param>
            ''' <param name="name">The user name of the User</param>
            ''' <history>
            ''' 	[cnurse]	02/07/2007  created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal id As Integer, ByVal name As String)
                UserId = id
                UserName = name
            End Sub

        End Class

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UserUpdateErrorArgs class provides a customised EventArgs class for
        ''' the UserUpdateError Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class UserUpdateErrorArgs
            Inherits BaseUserEventArgs

            Private _message As String

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Constructs a new UserUpdateErrorArgs
            ''' </summary>
            ''' <param name="id">The Id of the User</param>
            ''' <param name="name">The user name of the User</param>
            ''' <param name="message">The error message</param>
            ''' <history>
            ''' 	[cnurse]	02/07/2007  created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal id As Integer, ByVal name As String, ByVal message As String)
                UserId = id
                UserName = name
                _message = message
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Gets and sets the error message
            ''' </summary>
            ''' <history>
            ''' 	[cnurse]	02/07/2007  created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property Message() As String
                Get
                    Return _message
                End Get
                Set(ByVal Value As String)
                    _message = Value
                End Set
            End Property

        End Class

#End Region


    End Class

End Namespace
