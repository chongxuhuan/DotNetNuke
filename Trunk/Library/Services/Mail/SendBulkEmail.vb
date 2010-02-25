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
Imports System.Threading

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.FileSystem

Namespace DotNetNuke.Services.Mail

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SendBulkEMail Class is a helper class tht manages the sending of Email
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' 	[sleupold]	8/15/2007	add localization of confirmation email 
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Obsolete("This class has been deprecated, please use SendTokenizedBulkMail instead")> _
    Public Class SendBulkEmail

        Public Recipients As ArrayList
        Public Priority As MailPriority
        Public Subject As String
        Public BodyFormat As MailFormat
        Public Body As String
        Public Attachment As String
        Public SendMethod As String
        Public SMTPServer As String
        Public SMTPAuthentication As String
        Public SMTPUsername As String
        Public SMTPPassword As String
        Public Administrator As String
        Public Heading As String

        'internal:
        Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings
        Dim SendingUser As DotNetNuke.Entities.Users.UserInfo
        Dim SenderLanguage As String
        Dim strConfirmationBody As String
        Dim strConfirmationSubj As String
        Dim objTR As DotNetNuke.Services.Tokens.TokenReplace


        Private Sub InitObjects()
            portalSettings = PortalController.GetCurrentPortalSettings
            SendingUser = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
            If Not SendingUser.Profile.PreferredLocale Is Nothing Then
                SenderLanguage = SendingUser.Profile.PreferredLocale
            Else
                SenderLanguage = portalSettings.DefaultLanguage
            End If
            objTR = New DotNetNuke.Services.Tokens.TokenReplace
            Dim messageKey As String
            If BodyFormat = MailFormat.Html Then
                messageKey = "EMAIL_BulkMailConf_Html_Body"
            Else
                messageKey = "EMAIL_BulkMailConf_Text_Body"
            End If
            strConfirmationBody = Localization.Localization.GetString(messageKey, DotNetNuke.Services.Localization.Localization.GlobalResourceFile, SenderLanguage)
            strConfirmationSubj = Localization.Localization.GetString("EMAIL_BulkMailConf_Subject", DotNetNuke.Services.Localization.Localization.GlobalResourceFile, SenderLanguage)
        End Sub

        Public Sub New()
            InitObjects()
        End Sub

        Public Sub New(ByVal recipients As ArrayList, ByVal priority As String, ByVal format As String, ByVal portalAlias As String)

            Me.Recipients = recipients
            Select Case priority
                Case "1"
                    Me.Priority = MailPriority.High
                Case "2"
                    Me.Priority = MailPriority.Normal
                Case "3"
                    Me.Priority = MailPriority.Low
            End Select
            If format = "BASIC" Then
                Me.BodyFormat = MailFormat.Text
            Else
                Me.BodyFormat = MailFormat.Html
                ' Add Base Href for any images inserted in to the email.
                Me.Body = "<Base Href='" & portalAlias & "'>"
            End If
            InitObjects()
        End Sub

        Public Function SendMails() As Integer
            Dim intRecipients As Integer = 0
            Dim intMessages As Integer = 0
            Dim intErrors As Integer = 0
            Try
                ' send to recipients
                Dim strBody As String
                Dim strDistributionList As String = ""
                Dim objRecipient As ListItem
                Dim startedAt As String = Now().ToString

                Dim strbMailErrors As New System.Text.StringBuilder
                Dim strMailError As String
                Select Case SendMethod
                    Case "TO"
                        Dim endDelimit As String
                        If BodyFormat = MailFormat.Html Then
                            endDelimit = "<br>"
                        Else
                            endDelimit = vbCrLf
                        End If
                        For Each objRecipient In Recipients
                            If objRecipient.Text <> "" Then
                                intRecipients += 1
                                strBody = Heading & objRecipient.Value & "," & endDelimit & endDelimit & Body

                                strMailError = Mail.SendMail(Administrator, objRecipient.Text, "", "", Priority, Subject, BodyFormat, System.Text.Encoding.UTF8, strBody, Attachment, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword)
                                If Not String.IsNullOrEmpty(strMailError) Then
                                    strbMailErrors.Append(strMailError)
                                    strbMailErrors.AppendLine()
                                    intErrors += 1
                                Else
                                    intMessages += 1
                                End If
                            End If
                        Next
                    Case "BCC"
                        For Each objRecipient In Recipients
                            If objRecipient.Text <> "" Then
                                intRecipients += 1
                                strDistributionList += objRecipient.Text & "; "
                            End If
                        Next

                        ' [DNN-4786] The loop above leaves a single ";" and a space at the end
                        ' but this causes the ASP.Net Mail system to reject the address list as
                        ' invalid, so remove the last two characters
                        If strDistributionList.Length > 2 Then
                            strDistributionList = strDistributionList.Substring(0, strDistributionList.Length - 2)
                        Else
                            strDistributionList = String.Empty
                        End If

                        intMessages = 1
                        strBody = Body

                        ' [DNN-4786] Placed From field in to field as well so that BCC sending will work properly
                        strMailError = Mail.SendMail(Administrator, Administrator, "", strDistributionList, Priority, Subject, BodyFormat, System.Text.Encoding.UTF8, strBody, Attachment, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword)
                        If Not String.IsNullOrEmpty(strMailError) Then
                            strbMailErrors.Append(strMailError)
                        End If
                End Select

                If strbMailErrors.Length = 0 Then
                    strbMailErrors.Append(Localization.Localization.GetString("NoErrorsSending", DotNetNuke.Services.Localization.Localization.GlobalResourceFile, SenderLanguage))
                End If
                ' send confirmation, use resource string like:
                'Operation started at: [Custom:0]<br>
                'EmailRecipients:      [Custom:1]<br>
                'EmailMessages:        [Custom:2]<br>
                'Operation Completed:  [Custom:3]<br>
                'Number of Errors:     [Custom:4]<br>
                'Error Report:<br>
                '[Custom:5]
                Dim params As New ArrayList
                params.Add(startedAt)
                params.Add(intRecipients.ToString)
                params.Add(intMessages.ToString)
                params.Add(Now.ToString)
                params.Add(intErrors.ToString)
                params.Add(strbMailErrors.ToString)
                'Use message resource string appropriate for BodyFormat
                strBody = objTR.ReplaceEnvironmentTokens(strConfirmationBody, params, "Custom")
                strConfirmationSubj = String.Format(strConfirmationSubj, Subject)
                Mail.SendMail(SendingUser.Email, SendingUser.Email, "", "", Priority, strConfirmationSubj, BodyFormat, System.Text.Encoding.UTF8, strBody, "", SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword)
            Catch exc As Exception
                ' send mail failure
                System.Diagnostics.Debug.Write(exc.Message)
            End Try
            Return intMessages
        End Function

        Public Sub Send()
            Dim i As Integer = SendMails()
        End Sub

    End Class

End Namespace
