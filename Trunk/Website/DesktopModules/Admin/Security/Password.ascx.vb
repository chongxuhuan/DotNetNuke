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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Utilities

Namespace DotNetNuke.Modules.Admin.Users

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Password UserModuleBase is used to manage Users Passwords
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	03/03/2006  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Password
        Inherits UserModuleBase

#Region "Delegates"

        Public Delegate Sub PasswordUpdatedEventHandler(ByVal sender As Object, ByVal e As PasswordUpdatedEventArgs)

#End Region

#Region "Events"

        Public Event PasswordUpdated As PasswordUpdatedEventHandler
        Public Event PasswordQuestionAnswerUpdated As PasswordUpdatedEventHandler

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the UserMembership associated with this control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/03/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Membership() As UserMembership
            Get
                Dim _Membership As UserMembership = Nothing
                If Not User Is Nothing Then
                    _Membership = User.Membership
                End If
                Return _Membership
            End Get
        End Property

#End Region

#Region "Event Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the PasswordUpdated Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/08/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnPasswordUpdated(ByVal e As PasswordUpdatedEventArgs)

            RaiseEvent PasswordUpdated(Me, e)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the PasswordQuestionAnswerUpdated Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnPasswordQuestionAnswerUpdated(ByVal e As PasswordUpdatedEventArgs)

            RaiseEvent PasswordQuestionAnswerUpdated(Me, e)

        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DataBind binds the data to the controls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/03/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub DataBind()

            If IsAdmin Then
                lblTitle.Text = String.Format(Localization.GetString("PasswordTitle.Text", LocalResourceFile), User.Username, User.UserID.ToString)
            Else
                trTitle.Visible = False
            End If

            lblLastChanged.Text = User.Membership.LastPasswordChangeDate.ToLongDateString

            'Set Password Expiry Label
            If User.Membership.UpdatePassword Then
                lblExpires.Text = Localization.GetString("ForcedExpiry", Me.LocalResourceFile)
            Else
                If PasswordConfig.PasswordExpiry > 0 Then
                    lblExpires.Text = User.Membership.LastPasswordChangeDate.AddDays(PasswordConfig.PasswordExpiry).ToLongDateString
                Else
                    lblExpires.Text = Localization.GetString("NoExpiry", Me.LocalResourceFile)
                End If
            End If

            'If Password retrieval is not supported then only the user can change
            'their password, an Admin must Reset
            If ((Not MembershipProviderConfig.PasswordRetrievalEnabled) AndAlso IsAdmin AndAlso (Not IsUser)) Then
                'OrElse (MembershipProviderConfig.RequiresQuestionAndAnswer AndAlso IsAdmin) Then
                pnlChange.Visible = False
            Else
                pnlChange.Visible = True

                'Set up Change Password
                If IsAdmin And Not IsUser Then
                    lblChangeHelp.Text = Localization.GetString("AdminChangeHelp", Me.LocalResourceFile)
                    trOldPassword.Visible = False
                Else
                    lblChangeHelp.Text = Localization.GetString("UserChangeHelp", Me.LocalResourceFile)
                End If
            End If

            'If Password Reset is not enabled then only the Admin can reset the 
            'Password, a User must Update
            If Not MembershipProviderConfig.PasswordResetEnabled Then
                pnlReset.Visible = False
            Else
                pnlReset.Visible = True

                'Set up Reset Password
                If IsAdmin And Not IsUser Then
                    If MembershipProviderConfig.RequiresQuestionAndAnswer Then
                        pnlReset.Visible = False
                    Else
                        lblResetHelp.Text = Localization.GetString("AdminResetHelp", Me.LocalResourceFile)
                    End If
                    trQuestion.Visible = False
                    trAnswer.Visible = False
                Else
                    If MembershipProviderConfig.RequiresQuestionAndAnswer And IsUser Then
                        lblResetHelp.Text = Localization.GetString("UserResetHelp", Me.LocalResourceFile)
                        lblQuestion.Text = User.Membership.PasswordQuestion
                        trQuestion.Visible = True
                        trAnswer.Visible = True
                    Else
                        pnlReset.Visible = False
                    End If
                End If
            End If

            'Set up Edit Question and Answer area
            If MembershipProviderConfig.RequiresQuestionAndAnswer And IsUser Then
                pnlQA.Visible = True
            Else
                pnlQA.Visible = False
            End If

        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/03/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/03/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            ClientAPI.RegisterKeyCapture(Me.Parent, Me.cmdUpdate.Controls(0), Asc(vbCr))
            ClientAPI.RegisterKeyCapture(Me, Me.cmdUpdate.Controls(0), Asc(vbCr))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdReset_Click runs when the Reset Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/03/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdReset.Click

            Dim answer As String = ""
            If MembershipProviderConfig.RequiresQuestionAndAnswer And Not IsAdmin Then
                If txtAnswer.Text = "" Then
                    OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.InvalidPasswordAnswer))
                    Exit Sub
                End If
                answer = txtAnswer.Text
            End If

            Try
                UserController.ResetPassword(User, answer)
                OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.Success))
            Catch exc As ArgumentException
                OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.InvalidPasswordAnswer))
            Catch ex As Exception
                OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordResetFailed))
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Update  Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/03/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click

            '1. Check New Password and Confirm are the same
            If txtNewPassword.Text <> txtNewConfirm.Text Then
                OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordMismatch))
                Exit Sub
            End If

            '2. Check New Password is Valid
            If Not UserController.ValidatePassword(txtNewPassword.Text) Then
                OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordInvalid))
                Exit Sub
            End If

            '3. Check old Password is Provided
            If Not IsAdmin And txtOldPassword.Text = "" Then
                OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordMissing))
                Exit Sub
            End If

            '4. Check New Password is ddifferent
            If Not IsAdmin And txtNewPassword.Text = txtOldPassword.Text Then
                OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordNotDifferent))
                Exit Sub
            End If

            Try
                If UserController.ChangePassword(User, txtOldPassword.Text, txtNewPassword.Text) Then
                    'Success
                    OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.Success))
                Else
                    'Fail
                    OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordResetFailed))
                End If
            Catch mex As MembershipPasswordException
                'Password Answer missing
                OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.InvalidPasswordAnswer))
            Catch ex As Exception
                'Fail
                OnPasswordUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordResetFailed))
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Update Question and Answer  Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/09/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdateQA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdateQA.Click

            If txtQAPassword.Text = "" Then
                OnPasswordQuestionAnswerUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordInvalid))
                Exit Sub
            End If

            If txtEditQuestion.Text = "" Then
                OnPasswordQuestionAnswerUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.InvalidPasswordQuestion))
                Exit Sub
            End If

            If txtEditAnswer.Text = "" Then
                OnPasswordQuestionAnswerUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.InvalidPasswordAnswer))
                Exit Sub
            End If

            'Try and set password Q and A
            Dim objUser As UserInfo = UserController.GetUserById(PortalId, UserId)
            If UserController.ChangePasswordQuestionAndAnswer(objUser, txtQAPassword.Text, txtEditQuestion.Text, txtEditAnswer.Text) Then
                'Success
                OnPasswordQuestionAnswerUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.Success))
            Else
                'Fail
                OnPasswordQuestionAnswerUpdated(New PasswordUpdatedEventArgs(PasswordUpdateStatus.PasswordResetFailed))
            End If
        End Sub

#End Region

#Region "Event Args"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The PasswordUpdatedEventArgs class provides a customised EventArgs class for
        ''' the PasswordUpdated Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/08/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class PasswordUpdatedEventArgs

            Private _UpdateStatus As PasswordUpdateStatus

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Constructs a new PasswordUpdatedEventArgs
            ''' </summary>
            ''' <param name="status">The Password Update Status</param>
            ''' <history>
            ''' 	[cnurse]	03/08/2006  Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal status As PasswordUpdateStatus)
                _UpdateStatus = status
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Gets and sets the Update Status
            ''' </summary>
            ''' <history>
            ''' 	[cnurse]	03/08/2006  Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property UpdateStatus() As PasswordUpdateStatus
                Get
                    Return _UpdateStatus
                End Get
                Set(ByVal Value As PasswordUpdateStatus)
                    _UpdateStatus = Value
                End Set
            End Property

        End Class

#End Region

    End Class

End Namespace