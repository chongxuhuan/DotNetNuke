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
Option Strict On

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Services.Mail

Namespace DotNetNuke.Modules.Admin.Security

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SendPassword UserModuleBase is used to allow a user to retrieve their password
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	03/21/2006  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class SendPassword
        Inherits UserModuleBase

#Region "Private Members"

        Private ipAddress As String
        Private _User As UserInfo
        Private _UserCount As Integer = Null.NullInteger

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Redirect URL (after successful sending of password)
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/11/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property RedirectURL() As String
            Get
                Dim _RedirectURL As String = ""

                If Not Request.QueryString("returnurl") Is Nothing Then
                    ' return to the url passed
                    _RedirectURL = HttpUtility.UrlDecode(Request.QueryString("returnurl"))
                    ' redirect url should never contain a protocol ( if it does, it is likely a cross-site request forgery attempt )
                    If _RedirectURL.Contains("://") Then
                        _RedirectURL = ""
                    End If

                    If _RedirectURL.Contains("?returnurl") Then
                        Dim baseURL As String = _RedirectURL.Substring(0, _RedirectURL.IndexOf("?returnurl"))
                        Dim returnURL As String = _RedirectURL.Substring(_RedirectURL.IndexOf("?returnurl") + 11)

                        _RedirectURL = String.Concat(baseURL, "?returnurl", HttpUtility.UrlEncode(returnURL))
                    End If
                End If
                If _RedirectURL = "" Then
                    ' redirect to current page 
                    _RedirectURL = NavigateURL()
                End If

                Return _RedirectURL
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Captcha control is used to validate the login
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/21/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property UseCaptcha() As Boolean
            Get
                Dim setting As Object = UserModuleBase.GetSetting(PortalId, "Security_CaptchaLogin")
                Return CType(setting, Boolean)
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub GetUser()
            Dim arrUsers As ArrayList

            If MembershipProviderConfig.RequiresUniqueEmail AndAlso _
                        Trim(txtEmail.Text) <> "" AndAlso Trim(txtUsername.Text) = "" Then
                arrUsers = UserController.GetUsersByEmail(PortalSettings.PortalId, txtEmail.Text, 0, Int32.MaxValue, _UserCount)
                If Not arrUsers Is Nothing AndAlso arrUsers.Count = 1 Then
                    _User = DirectCast(arrUsers(0), UserInfo)
                End If
            Else
                _User = UserController.GetUserByName(PortalSettings.PortalId, txtUsername.Text)
            End If
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim isEnabled As Boolean = True

            If MembershipProviderConfig.PasswordRetrievalEnabled Then
                lblHelp.Text = Localization.GetString("SendPasswordHelp", LocalResourceFile)
                cmdSendPassword.Text = Localization.GetString("SendPassword", LocalResourceFile)
                cmdSendPassword.ImageUrl = "~/images/password.gif"
            ElseIf MembershipProviderConfig.PasswordResetEnabled Then
                lblHelp.Text = Localization.GetString("ResetPasswordHelp", LocalResourceFile)
                cmdSendPassword.Text = Localization.GetString("ResetPassword", LocalResourceFile)
                cmdSendPassword.ImageUrl = "~/images/reset.gif"
            Else
                isEnabled = False
                lblHelp.Text = Localization.GetString("DisabledPasswordHelp", LocalResourceFile)
                tblSendPassword.Visible = False
            End If

            If MembershipProviderConfig.RequiresUniqueEmail AndAlso isEnabled Then
                lblHelp.Text += Localization.GetString("RequiresUniqueEmail", LocalResourceFile)
            End If

            If MembershipProviderConfig.RequiresQuestionAndAnswer AndAlso isEnabled Then
                lblHelp.Text += Localization.GetString("RequiresQuestionAndAnswer", LocalResourceFile)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/21/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If Not Request.UserHostAddress Is Nothing Then
                ipAddress = Request.UserHostAddress
            End If

            rowEmailLabel.Visible = MembershipProviderConfig.RequiresUniqueEmail
            rowEmailText.Visible = MembershipProviderConfig.RequiresUniqueEmail

            trCaptcha1.Visible = UseCaptcha
            trCaptcha2.Visible = UseCaptcha

            If UseCaptcha Then
                ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", Me.LocalResourceFile)
                ctlCaptcha.Text = Localization.GetString("CaptchaText", Me.LocalResourceFile)
            End If

        End Sub

        Protected Sub cmdLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdLogin.Click
            Response.Redirect(RedirectURL, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdSendPassword_Click runs when the Password Reminder button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/21/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdSendPassword_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdSendPassword.Click

            Dim strMessage As String = Null.NullString
            Dim strLogMessage As String = Null.NullString
            Dim canSend As Boolean = True

            If MembershipProviderConfig.RequiresQuestionAndAnswer And txtAnswer.Text = "" Then
                GetUser()
                If Not _User Is Nothing Then
                    lblQuestion.Text = _User.Membership.PasswordQuestion
                End If
                tblQA.Visible = True
                Exit Sub
            End If

            If (UseCaptcha And ctlCaptcha.IsValid) OrElse (Not UseCaptcha) Then
                If Trim(txtUsername.Text) = "" Then
                    'No UserName provided
                    If MembershipProviderConfig.RequiresUniqueEmail Then
                        If Trim(txtEmail.Text) = "" Then
                            'No email address either (cannot retrieve password)
                            canSend = False
                            strMessage = Services.Localization.Localization.GetString("EnterUsernameEmail", Me.LocalResourceFile)
                        End If
                    Else
                        'Cannot retrieve password
                        canSend = False
                        strMessage = Services.Localization.Localization.GetString("EnterUsername", Me.LocalResourceFile)
                    End If
                End If

                If canSend Then
                    Dim objSecurity As New PortalSecurity
                    GetUser()

                    If Not _User Is Nothing Then
                        If _User.IsDeleted Then
                            canSend = False
                            strMessage = Localization.GetString("UsernameError", Me.LocalResourceFile)
                        Else
                            If MembershipProviderConfig.PasswordRetrievalEnabled Then
                                Try
                                    _User.Membership.Password = UserController.GetPassword(_User, txtAnswer.Text)
                                Catch ex As Exception
                                    canSend = False
                                    strMessage = Localization.GetString("PasswordRetrievalError", Me.LocalResourceFile)
                                End Try
                            Else
                                Try
                                    _User.Membership.Password = UserController.GeneratePassword()
                                    UserController.ResetPassword(_User, txtAnswer.Text)
                                Catch ex As Exception
                                    canSend = False
                                    strMessage = Localization.GetString("PasswordResetError", Me.LocalResourceFile)
                                End Try
                            End If
                            If canSend Then
                                If Mail.SendMail(_User, MessageType.PasswordReminder, PortalSettings) <> String.Empty Then
                                    strMessage = Localization.GetString("SendMailError", Me.LocalResourceFile)
                                    canSend = False
                                Else
                                    strMessage = Localization.GetString("PasswordSent", Me.LocalResourceFile)
                                End If
                            End If
                        End If
                    Else
                        If _UserCount > 1 Then
                            strMessage = Localization.GetString("MultipleUsers", Me.LocalResourceFile)
                        ElseIf MembershipProviderConfig.RequiresUniqueEmail AndAlso _
                                    Trim(txtEmail.Text) <> "" AndAlso Trim(txtUsername.Text) = "" Then
                            strMessage = Localization.GetString("EmailError", Me.LocalResourceFile)
                        Else
                            strMessage = Localization.GetString("UsernameError", Me.LocalResourceFile)
                        End If

                        canSend = False
                    End If

                    If canSend Then
                        Dim objEventLog As New Services.Log.EventLog.EventLogController
                        Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                        objEventLogInfo.AddProperty("IP", ipAddress)
                        objEventLogInfo.LogPortalID = PortalSettings.PortalId
                        objEventLogInfo.LogPortalName = PortalSettings.PortalName
                        objEventLogInfo.LogUserID = UserId
                        objEventLogInfo.LogUserName = objSecurity.InputFilter(txtUsername.Text, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
                        objEventLogInfo.LogTypeKey = "PASSWORD_SENT_SUCCESS"
                        objEventLog.AddLog(objEventLogInfo)

                        UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                    Else
                        Dim objEventLog As New Services.Log.EventLog.EventLogController
                        Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                        objEventLogInfo.AddProperty("IP", ipAddress)
                        objEventLogInfo.LogPortalID = PortalSettings.PortalId
                        objEventLogInfo.LogPortalName = PortalSettings.PortalName
                        objEventLogInfo.LogUserID = UserId
                        objEventLogInfo.LogUserName = objSecurity.InputFilter(txtUsername.Text, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
                        objEventLogInfo.LogTypeKey = "PASSWORD_SENT_FAILURE"
                        objEventLogInfo.LogProperties.Add(New LogDetailInfo("Cause", strMessage))
                        objEventLog.AddLog(objEventLogInfo)

                        UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    End If

                    cmdLogin.Visible = True
                Else
                    UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            End If
        End Sub

#End Region

    End Class

End Namespace
