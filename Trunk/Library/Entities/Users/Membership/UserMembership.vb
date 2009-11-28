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

Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.Entities.Users

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Entities.Users
    ''' Class:      UserMembership
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The UserMembership class provides Business Layer model for the Users Membership
    ''' related properties
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	12/22/2005	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class UserMembership

#Region "Private Members"

        Private _Approved As Boolean = True
        Private _CreatedDate As Date
        Private _IsOnLine As Boolean
        Private _LastActivityDate As Date
        Private _LastLockoutDate As Date
        Private _LastLoginDate As Date
        Private _LastPasswordChangeDate As Date
        Private _LockedOut As Boolean = False
        Private _ObjectHydrated As Boolean
        Private _Password As String
        Private _PasswordAnswer As String
        Private _PasswordQuestion As String
        Private _UpdatePassword As Boolean

        Private IsSuperUser As Boolean

        Private _User As UserInfo

#End Region

#Region "Constructors"

        Public Sub New(ByVal user As UserInfo)
            _User = user
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the User is Approved
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/27/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(9)> Public Property Approved() As Boolean
            Get
                Return _Approved
            End Get
            Set(ByVal Value As Boolean)
                _Approved = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the User's Creation Date
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/27/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(1), IsReadOnly(True)> Public Property CreatedDate() As Date
            Get
                Return _CreatedDate
            End Get
            Set(ByVal Value As Date)
                _CreatedDate = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the User Is Online
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/27/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(7)> Public Property IsOnLine() As Boolean
            Get
                Return _IsOnLine
            End Get
            Set(ByVal Value As Boolean)
                _IsOnLine = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Last Activity Date of the User
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/27/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(3), IsReadOnly(True)> Public Property LastActivityDate() As Date
            Get
                Return _LastActivityDate
            End Get
            Set(ByVal Value As Date)
                _LastActivityDate = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Last Lock Out Date of the User
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/27/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(5), IsReadOnly(True)> Public Property LastLockoutDate() As Date
            Get
                Return _LastLockoutDate
            End Get
            Set(ByVal Value As Date)
                _LastLockoutDate = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Last Login Date of the User
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/27/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(2), IsReadOnly(True)> Public Property LastLoginDate() As Date
            Get
                Return _LastLoginDate
            End Get
            Set(ByVal Value As Date)
                _LastLoginDate = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Last Password Change Date of the User
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/27/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(4), IsReadOnly(True)> Public Property LastPasswordChangeDate() As Date
            Get
                Return _LastPasswordChangeDate
            End Get
            Set(ByVal Value As Date)
                _LastPasswordChangeDate = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the user is locked out
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/27/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(8)> Public Property LockedOut() As Boolean
            Get
                Return _LockedOut
            End Get
            Set(ByVal Value As Boolean)
                _LockedOut = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the User's Password
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/27/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> Public Property Password() As String
            Get
                Return _Password
            End Get
            Set(ByVal Value As String)
                _Password = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the User's Password Answer
        ''' </summary>
        ''' <history>
        '''     [cnurse]	08/04/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> Public Property PasswordAnswer() As String
            Get
                Return _PasswordAnswer
            End Get
            Set(ByVal Value As String)
                _PasswordAnswer = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the User's Password Question
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/27/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> Public Property PasswordQuestion() As String
            Get
                Return _PasswordQuestion
            End Get
            Set(ByVal Value As String)
                _PasswordQuestion = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a flag that determines whether the password should be updated
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(10)> Public Property UpdatePassword() As Boolean
            Get
                Return _UpdatePassword
            End Get
            Set(ByVal Value As Boolean)
                _UpdatePassword = Value
            End Set
        End Property

#End Region

#Region "Deprecated Members"

        <Obsolete("Deprecated in DNN 5.1")> _
        Public Sub New()
            _User = New UserInfo()
        End Sub

        <Obsolete("Deprecated in DNN 5.1")> _
        <Browsable(False)> Public Property Email() As String
            Get
                Return _User.Email
            End Get
            Set(ByVal Value As String)
                _User.Email = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.1")> _
        <Browsable(False)> Public Property ObjectHydrated() As Boolean
            Get
                Return _ObjectHydrated
            End Get
            Set(ByVal Value As Boolean)
                _ObjectHydrated = True
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.1")> _
        <Browsable(False)> Public Property Username() As String
            Get
                Return _User.Username
            End Get
            Set(ByVal Value As String)
                _User.Username = Value
            End Set
        End Property

#End Region

    End Class

End Namespace
