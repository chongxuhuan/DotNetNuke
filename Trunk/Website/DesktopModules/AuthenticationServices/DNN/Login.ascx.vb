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


Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Authentication

Namespace DotNetNuke.Modules.Admin.Authentication

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Login AuthenticationLoginBase is used to provide a login for a registered user
    ''' portal.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/24/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    '''     [cnurse]    08/07/2007  Ported to new Authentication Framework
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Login
        Inherits AuthenticationLoginBase

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Captcha control is used to validate the login
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/17/2006  Created
        '''     [cnurse]    07/03/2007  Moved from Sign.ascx.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property UseCaptcha() As Boolean
            Get
                Return AuthenticationConfig.GetConfig(PortalId).UseCaptcha
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Check if the Auth System is Enabled (for the Portal)
        ''' </summary>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[cnurse]	07/04/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides ReadOnly Property Enabled() As Boolean
            Get
                Return AuthenticationConfig.GetConfig(PortalId).Enabled
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/8/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            DotNetNuke.UI.Utilities.ClientAPI.RegisterKeyCapture(Me.Parent, Me.cmdLogin, Asc(vbCr))

            If Not Request.IsAuthenticated Then
                If Page.IsPostBack = False Then
                    Try
                        If Not Request.QueryString("username") Is Nothing Then
                            txtUsername.Text = Request.QueryString("username")
                        End If
                        If Not Request.QueryString("verificationcode") Is Nothing Then
                            If PortalSettings.UserRegistration = PortalRegistrationType.VerifiedRegistration Then
                                'Display Verification Rows 
                                rowVerification1.Visible = True
                                rowVerification2.Visible = True
                                txtVerification.Text = Request.QueryString("verificationcode")
                            End If
                        End If

                    Catch
                        'control not there 
                    End Try
                End If

                Try
                    If String.IsNullOrEmpty(txtUsername.Text) Then
                        SetFormFocus(txtUsername)
                    Else
                        SetFormFocus(txtPassword)
                    End If
                Catch
                    'Not sure why this Try/Catch may be necessary, logic was there in old setFormFocus location stating the following
                    'control not there or error setting focus
                End Try
            End If

            trCaptcha1.Visible = UseCaptcha
            trCaptcha2.Visible = UseCaptcha

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            txtPassword.Attributes.Add("value", txtPassword.Text)
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdLogin_Click runs when the login button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/24/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        '''     [cnurse]    12/11/2005  Updated to reflect abstraction of Membership
        '''     [cnurse]    07/03/2007  Moved from Sign.ascx.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdLogin.Click

            If (UseCaptcha And ctlCaptcha.IsValid) OrElse (Not UseCaptcha) Then
                Dim loginStatus As UserLoginStatus = UserLoginStatus.LOGIN_FAILURE
                Dim objUser As UserInfo = UserController.ValidateUser(PortalId, txtUsername.Text, txtPassword.Text, "DNN", txtVerification.Text, PortalSettings.PortalName, IPAddress, loginStatus)
                Dim authenticated As Boolean = Null.NullBoolean
                Dim message As String = Null.NullString

                If loginStatus = UserLoginStatus.LOGIN_USERNOTAPPROVED Then
                    'Check if its the first time logging in to a verified site
                    If PortalSettings.UserRegistration = PortalRegistrationType.VerifiedRegistration Then
                        If Not rowVerification1.Visible Then
                            'Display Verification Rows so User can enter verification code
                            rowVerification1.Visible = True
                            rowVerification2.Visible = True
                            message = "EnterCode"
                        Else
                            If txtVerification.Text <> "" Then
                                message = "InvalidCode"
                            Else
                                message = "EnterCode"
                            End If
                        End If
                    Else
                        message = "UserNotAuthorized"
                    End If
                Else
                    authenticated = (loginStatus <> UserLoginStatus.LOGIN_FAILURE)
                End If

                'Raise UserAuthenticated Event
                Dim eventArgs As UserAuthenticatedEventArgs = New UserAuthenticatedEventArgs(objUser, txtUsername.Text, loginStatus, "DNN")
                eventArgs.Authenticated = authenticated
                eventArgs.Message = message
                OnUserAuthenticated(eventArgs)
            End If

        End Sub


#End Region

    End Class

End Namespace
