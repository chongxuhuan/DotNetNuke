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

Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.Security.Membership

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Security.Membership
    ''' Class:      MembershipProviderConfig
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The MembershipProviderConfig class provides a wrapper to the Membership providers
    ''' configuration
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	03/02/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class MembershipProviderConfig

#Region "Private Shared Members"

        Private Shared memberProvider As DotNetNuke.Security.Membership.MembershipProvider = DotNetNuke.Security.Membership.MembershipProvider.Instance()

#End Region

#Region "Public Shared Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Provider Properties can be edited
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> Public Shared ReadOnly Property CanEditProviderProperties() As Boolean
            Get
                Return memberProvider.CanEditProviderProperties
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the maximum number of invlaid attempts to login are allowed
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(8), Category("Password")> Public Shared Property MaxInvalidPasswordAttempts() As Integer
            Get
                Return memberProvider.MaxInvalidPasswordAttempts
            End Get
            Set(ByVal Value As Integer)
                memberProvider.MaxInvalidPasswordAttempts = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Mimimum no of Non AlphNumeric characters required
        ''' </summary>
        ''' <returns>An Integer.</returns>
        ''' <history>
        '''     [cnurse]	02/07/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(5), Category("Password")> Public Shared Property MinNonAlphanumericCharacters() As Integer
            Get
                Return memberProvider.MinNonAlphanumericCharacters
            End Get
            Set(ByVal Value As Integer)
                memberProvider.MinNonAlphanumericCharacters = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Mimimum Password Length
        ''' </summary>
        ''' <returns>An Integer.</returns>
        ''' <history>
        '''     [cnurse]	02/07/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(4), Category("Password")> Public Shared Property MinPasswordLength() As Integer
            Get
                Return memberProvider.MinPasswordLength
            End Get
            Set(ByVal Value As Integer)
                memberProvider.MinPasswordLength = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the window in minutes that the maxium attempts are tracked for
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(9), Category("Password")> Public Shared Property PasswordAttemptWindow() As Integer
            Get
                Return memberProvider.PasswordAttemptWindow
            End Get
            Set(ByVal Value As Integer)
                memberProvider.PasswordAttemptWindow = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Password Format
        ''' </summary>
        ''' <returns>A PasswordFormat enumeration.</returns>
        ''' <history>
        '''     [cnurse]	02/07/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(1), Category("Password")> Public Shared Property PasswordFormat() As PasswordFormat
            Get
                Return memberProvider.PasswordFormat
            End Get
            Set(ByVal Value As PasswordFormat)
                memberProvider.PasswordFormat = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Users's Password can be reset
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(3), Category("Password")> Public Shared Property PasswordResetEnabled() As Boolean
            Get
                Return memberProvider.PasswordResetEnabled
            End Get
            Set(ByVal Value As Boolean)
                memberProvider.PasswordResetEnabled = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Users's Password can be retrieved
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(2), Category("Password")> Public Shared Property PasswordRetrievalEnabled() As Boolean
            Get
                Dim enabled As Boolean = memberProvider.PasswordRetrievalEnabled

                'If password format is hashed the password cannot be retrieved
                If memberProvider.PasswordFormat = PasswordFormat.Hashed Then
                    enabled = False
                End If
                Return enabled
            End Get
            Set(ByVal Value As Boolean)
                memberProvider.PasswordRetrievalEnabled = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a Regular Expression that deermines the strength of the password
        ''' </summary>
        ''' <returns>A String.</returns>
        ''' <history>
        '''     [cnurse]	02/07/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(7), Category("Password")> Public Shared Property PasswordStrengthRegularExpression() As String
            Get
                Return memberProvider.PasswordStrengthRegularExpression
            End Get
            Set(ByVal Value As String)
                memberProvider.PasswordStrengthRegularExpression = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether a Question/Answer is required for Password retrieval
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	02/07/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(6), Category("Password")> Public Shared Property RequiresQuestionAndAnswer() As Boolean
            Get
                Return memberProvider.RequiresQuestionAndAnswer
            End Get
            Set(ByVal Value As Boolean)
                memberProvider.RequiresQuestionAndAnswer = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether a Unique Email is required
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	02/06/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(0), Category("User")> Public Shared Property RequiresUniqueEmail() As Boolean
            Get
                Return memberProvider.RequiresUniqueEmail
            End Get
            Set(ByVal Value As Boolean)
                memberProvider.RequiresUniqueEmail = Value
            End Set
        End Property

#End Region

    End Class


End Namespace
