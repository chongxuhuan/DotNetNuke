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

Imports DotNetNuke.Services.Messaging.Data
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Services.Mail

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' SendTokenizedBulkEmail Class is a class to manage the sending of bulk mails
    ''' that contains tokens, which might be replaced with individual user properties
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [sleupold]	8/15/2007	created to support tokens and localisation
    '''     [sleupold]  9/09/2007   refactored interface for enhanced type safety
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SendTokenizedBulkEmail

#Region " Public Members "

        ''' <summary>
        ''' Addressing Methods (personalized or hidden)
        ''' </summary>
        Public Enum AddressMethods
            Send_TO = 1
            Send_BCC = 2
            Send_Relay = 3
        End Enum


        ''' <summary>
        ''' Priority of emails to be sent
        ''' </summary>
        Public Property Priority() As MailPriority
            Get
                Return _priority
            End Get
            Set(ByVal value As MailPriority)
                _priority = value
            End Set
        End Property


        ''' <summary>
        ''' Subject of the emails to be sent
        ''' </summary>
        ''' <remarks>may contain tokens</remarks>
        Public Property Subject() As String
            Get
                Return _strSubject
            End Get
            Set(ByVal value As String)
                _strSubject = value
            End Set
        End Property


        ''' <summary>
        ''' body text of the email to be sent
        ''' </summary>
        ''' <remarks>may contain HTML tags and tokens. Side effect: sets BodyFormat autmatically</remarks>
        Public Property Body() As String
            Get
                Return _strBody
            End Get
            Set(ByVal value As String)
                _strBody = value
                If HtmlUtils.IsHtml(_strBody) Then
                    _bodyFormat = MailFormat.Html
                Else
                    _bodyFormat = MailFormat.Text
                End If
            End Set
        End Property


        ''' <summary>format of body text for the email to be sent.</summary>
        ''' <remarks>by default activated, if tokens are found in Body and subject.</remarks>
        Public Property BodyFormat() As MailFormat
            Get
                Return _bodyFormat
            End Get
            Set(ByVal value As MailFormat)
                _bodyFormat = value
            End Set
        End Property


        ''' <summary>address method for the email to be sent (TO or BCC)</summary>
        ''' <remarks>TO is default value</remarks>
        Public Property AddressMethod() As AddressMethods
            Get
                Return _addressMethod
            End Get
            Set(ByVal value As AddressMethods)
                _addressMethod = value
            End Set
        End Property


        ''' <summary>portal alias http path to be used for links to images, ...</summary>
        Public Property PortalAlias() As String
            Get
                Return _PortalAlias
            End Get
            Set(ByVal value As String)
                _PortalAlias = value
            End Set
        End Property


        ''' <summary>UserInfo of the user sending the mail</summary>
        ''' <remarks>if not set explicitely, currentuser will be used</remarks>
        Public Property SendingUser() As UserInfo
            Get
                Return _sendingUser
            End Get
            Set(ByVal value As UserInfo)
                _sendingUser = value
                If Not _sendingUser.Profile.PreferredLocale Is Nothing Then
                    _strSenderLanguage = _sendingUser.Profile.PreferredLocale
                Else
                    Dim _portalSettings As DotNetNuke.Entities.Portals.PortalSettings = PortalController.GetCurrentPortalSettings
                    _strSenderLanguage = _portalSettings.DefaultLanguage
                End If
            End Set
        End Property

        ''' <summary>email of the user to be shown in the mail as replyTo address</summary>
        ''' <remarks>if not set explicitely, sendingUser will be used</remarks>
        Public Property ReplyTo() As UserInfo
            Get
                If Not _ReplyTo Is Nothing Then
                    Return _ReplyTo
                Else
                    Return SendingUser
                End If
            End Get
            Set(ByVal value As UserInfo)
                _ReplyTo = value
            End Set
        End Property

        ''' <summary>shall duplicate email addresses be ignored? (default value: false)</summary>
        ''' <remarks>Duplicate Users (e.g. from multiple role selections) will always be ignored.</remarks>
        Public Property RemoveDuplicates() As Boolean
            Get
                Return _bolRemoveDuplicates
            End Get
            Set(ByVal value As Boolean)
                _bolRemoveDuplicates = value
            End Set
        End Property


        ''' <summary>Shall automatic TokenReplace be prohibited?</summary>
        ''' <remarks>default value: false</remarks>
        Public Property SuppressTokenReplace() As Boolean
            Get
                Return _bolSuppressTR
            End Get
            Set(ByVal value As Boolean)
                _bolSuppressTR = value
            End Set
        End Property


        ''' <summary>Shall List of recipients appended to confirmation report?</summary>
        ''' <remarks>enabled by default.</remarks>
        Public Property ReportRecipients() As Boolean
            Get
                Return _bolReportRecipients
            End Get
            Set(ByVal value As Boolean)
                _bolReportRecipients = value
            End Set
        End Property

        Public Property RelayEmailAddress() As String
            Get
                If _addressMethod = AddressMethods.Send_Relay Then Return _strRelayEmail Else Return String.Empty
            End Get
            Set(ByVal value As String)
                _strRelayEmail = value
            End Set
        End Property

        Public Property LanguageFilter() As String()
            Get
                Return _languageFilter
            End Get
            Set(ByVal value As String())
                _languageFilter = value
            End Set
        End Property
#End Region

#Region " Private Members "
        Private _addressedRoles As New System.Collections.Generic.List(Of String)
        Private _addressedUsers As New System.Collections.Generic.List(Of UserInfo)
        Private _languageFilter() As String = Nothing
        Private _bolRemoveDuplicates As Boolean
        Private _priority As MailPriority = MailPriority.Normal
        Private _addressMethod As AddressMethods = AddressMethods.Send_TO
        Private _strRelayEmail As String
        Private _strSubject As String = ""
        Private _strBody As String = ""
        Private _bodyFormat As MailFormat = MailFormat.Text
        Private _attachments As New System.Collections.Generic.List(Of String)
        Private _bolSuppressTR As Boolean = False
        Private _bolReportRecipients As Boolean = True

        Private _strSMTPServer As String = ""
        Private _strSMTPAuthentication As String = ""
        Private _strSMTPUsername As String = ""
        Private _strSMTPPassword As String = ""
        Private _bolSMTPEnableSSL As Boolean  ' inited in initObjects

        Private _portalSettings As DotNetNuke.Entities.Portals.PortalSettings ' inited in initObjects
        Private _PortalAlias As String ' inited in initObjects
        Private _strSenderLanguage As String
        Private _strConfBodyText As String ' inited in initObjects
        Private _strConfBodyHTML As String ' inited in initObjects
        Private _strConfSubject As String ' inited in initObjects
        Private _strNoError As String ' inited in initObjects
        Private _sendingUser As UserInfo ' inited in initObjects
        Private _ReplyTo As UserInfo = Nothing

        Private _objTR As TokenReplace

        Private Shared _messagingController As New Messaging.MessagingController()
#End Region

#Region " Private Methods "

        ''' <summary>internal method to initialize used objects, depending on parameters of New() method</summary>
        Private Sub initObjects()
            _portalSettings = PortalController.GetCurrentPortalSettings
            _PortalAlias = _portalSettings.PortalAlias.HTTPAlias
            SendingUser = CType(HttpContext.Current.Items("UserInfo"), UserInfo)
            _objTR = New TokenReplace
            _strConfBodyHTML = Localization.Localization.GetString("EMAIL_BulkMailConf_Html_Body", DotNetNuke.Services.Localization.Localization.GlobalResourceFile, _strSenderLanguage)
            _strConfBodyText = Localization.Localization.GetString("EMAIL_BulkMailConf_Text_Body", DotNetNuke.Services.Localization.Localization.GlobalResourceFile, _strSenderLanguage)
            _strConfSubject = Localization.Localization.GetString("EMAIL_BulkMailConf_Subject", DotNetNuke.Services.Localization.Localization.GlobalResourceFile, _strSenderLanguage)
            _strNoError = Localization.Localization.GetString("NoErrorsSending", DotNetNuke.Services.Localization.Localization.GlobalResourceFile, _strSenderLanguage)
            _bolSMTPEnableSSL = Host.EnableSMTPSSL
        End Sub


        ''' <summary>Send bulkmail confirmation to admin</summary>
        ''' <param name="numRecipients">number of email recipients</param>
        ''' <param name="numMessages">number of messages sent, -1 if not determinable</param>
        ''' <param name="numErrors">number of emails not sent</param>
        ''' <param name="Subject">Subject of BulkMail sent (to be used as reference)</param>
        ''' <param name="StartedAt">date/time, sendout started</param>
        ''' <param name="MailErrors">mail error texts</param>
        ''' <param name="RecipientList">List of recipients as formatted string</param>
        ''' <remarks></remarks>
        Private Sub SendConfirmationMail(ByVal numRecipients As Integer, ByVal numMessages As Integer, ByVal numErrors As Integer, ByVal Subject As String, ByVal StartedAt As String, ByVal MailErrors As String, ByVal RecipientList As String)
            ' send confirmation, use resource string like:
            'Operation started at: [Custom:0]<br>
            'EmailRecipients:      [Custom:1]<b
            'EmailMessages sent:   [Custom:2]<br>
            'Operation Completed:  [Custom:3]<br>
            'Number of Errors:     [Custom:4]<br>
            'Error Report:<br>
            '[Custom:5]
            '--------------------------------------
            'Recipients:
            '[custom:6]
            Dim params As New ArrayList
            params.Add(StartedAt)
            params.Add(numRecipients.ToString)
            If numMessages >= 0 Then params.Add(numMessages.ToString) Else params.Add("***")
            params.Add(Now.ToString)
            If numErrors > 0 Then params.Add(numErrors.ToString) Else params.Add("")
            If MailErrors <> String.Empty Then params.Add(MailErrors) Else params.Add(_strNoError)
            If _bolReportRecipients Then params.Add(RecipientList) Else params.Add("")
            _objTR.User = _sendingUser
            Dim strbody As String
            If _bodyFormat = MailFormat.Html Then
                strbody = _objTR.ReplaceEnvironmentTokens(_strConfBodyHTML, params, "Custom")
            Else
                strbody = _objTR.ReplaceEnvironmentTokens(_strConfBodyText, params, "Custom")
            End If

            Dim strSubject As String = String.Format(_strConfSubject, Subject)
            If Not _bolSuppressTR Then strSubject = _objTR.ReplaceEnvironmentTokens(strSubject)
            ''Mail.SendMail(_sendingUser.Email, _sendingUser.Email, "", "", _priority, strSubject, _bodyFormat, System.Text.Encoding.UTF8, strbody, "", _strSMTPServer, _strSMTPAuthentication, _strSMTPUsername, _strSMTPPassword, _bolSMTPEnableSSL)

            Dim _message As New Message()
            _message.FromUserID = _sendingUser.UserID
            _message.ToUserID = _sendingUser.UserID
            _message.Subject = strSubject
            _message.Body = strbody
            _message.Status = MessageStatusType.Unread

            '_messagingController.SaveMessage(_message)
            Services.Mail.Mail.SendEmail(_sendingUser.Email, _sendingUser.Email, _message.Subject, _message.Body)

        End Sub

        ''' <summary>check, if the user's language matches the current language filter</summary>
        ''' <param name="UserLanguage">Language of the user</param>
        ''' <returns>userlanguage matches current languageFilter</returns>
        ''' <remarks>if userlanguage is empty or filter not set, true is returned</remarks>
        Private Function matchLanguageFilter(ByVal UserLanguage As String) As Boolean
            If UserLanguage = "" Then
                Return True
            ElseIf _languageFilter Is Nothing OrElse _languageFilter.Length = 0 Then
                Return True
            Else
                For Each s As String In _languageFilter
                    If UserLanguage.ToLowerInvariant Like s.ToLowerInvariant & "*" Then Return True
                Next s
            End If
            Return False
        End Function

        ''' <summary>add a user to the userlist, if it is not already in there</summary>
        ''' <param name="user">user to add</param>
        ''' <param name="keyList">list of key (either email addresses or userid's)</param>
        ''' <param name="userList">List of users</param>
        ''' <remarks>for use by Recipients method only</remarks>
        Private Sub conditionallyAddUser(ByRef user As UserInfo, ByRef keyList As System.Collections.Generic.List(Of String), ByRef userList As System.Collections.Generic.List(Of UserInfo))
            If (user.UserID > 0 AndAlso Not user.Membership.Approved) _
            OrElse user.Email = String.Empty _
            OrElse Not matchLanguageFilter(user.Profile.PreferredLocale) Then
                'skip
            Else
                Dim key As String = String.Empty
                If _bolRemoveDuplicates Or user.UserID = Null.NullInteger Then
                    key = user.Email
                Else
                    key = user.UserID.ToString
                End If
                If key <> String.Empty AndAlso Not keyList.Contains(key) Then
                    userList.Add(user)
                    keyList.Add(key)
                End If
            End If
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>create a new object with default settings only.</summary>
        ''' <remarks>not all properties can be set individually, use other overload, if you need to set ReplaceTokens to False or another portal alias in image paths.</remarks>
        Public Sub New()
            initObjects()
        End Sub


        ''' <summary>create a new object with individual settings.</summary>
        ''' <param name="AddressedRoles">List of role names, to address members</param>
        ''' <param name="AddressedUsers">List of individual recipients</param>
        ''' <param name="removeDuplicates">Shall duplicate emails be ignored?</param>
        ''' <param name="Subject">Subject for Emails to be sent</param>
        ''' <param name="Body">Email Body (plain Text or HTML)</param>
        ''' <remarks>Body and Subjects may contain tokens to be replaced</remarks>
        Public Sub New(ByVal AddressedRoles As System.Collections.Generic.List(Of String), ByVal AddressedUsers As System.Collections.Generic.List(Of UserInfo), ByVal removeDuplicates As Boolean, _
                       ByVal Subject As String, ByVal Body As String)

            _addressedRoles = AddressedRoles
            _addressedUsers = AddressedUsers
            _bolRemoveDuplicates = removeDuplicates
            _strSubject = Subject
            Me.Body = Body
            _bolSuppressTR = SuppressTokenReplace
            initObjects()
        End Sub


        ''' <summary>Specify SMTP server to be used</summary>
        ''' <param name="SMTPServer">name of the SMTP server</param>
        ''' <param name="SMTPAuthentication">authentication string (0: anonymous, 1: basic, 2: NTLM)</param>
        ''' <param name="SMTPUsername">username to log in SMTP server</param>
        ''' <param name="SMTPPassword">password to log in SMTP server</param>
        ''' <param name="SMTPEnableSSL">SSL used to connect tp SMTP server</param>
        ''' <returns>always true</returns>
        ''' <remarks>if not called, values will be taken from host settings</remarks>
        Public Function SetSMTPServer(ByVal SMTPServer As String, ByVal SMTPAuthentication As String, ByVal SMTPUsername As String, ByVal SMTPPassword As String, ByVal SMTPEnableSSL As Boolean) As Boolean
            _strSMTPServer = SMTPServer
            _strSMTPAuthentication = SMTPAuthentication
            _strSMTPUsername = SMTPUsername
            _strSMTPPassword = SMTPPassword
            _bolSMTPEnableSSL = SMTPEnableSSL
            Return True 'TODO: replace by validataion of parameters
        End Function


        ''' <summary>Add a single attachment file to the email</summary>
        ''' <param name="pAttachment">path to file to attach</param>
        ''' <remarks>secure storages are currently not supported</remarks>
        Public Sub AddAttachment(ByVal pAttachment As String)
            _attachments.Add(pAttachment)
        End Sub


        ''' <summary>Add a single recipient</summary>
        ''' <param name="recipient">userinfo of user to add</param>
        ''' <remarks>emaiol will be used for addressing, other properties might be used for TokenReplace</remarks>
        Public Sub AddAddressedUser(ByVal recipient As UserInfo)
            _addressedUsers.Add(recipient)
        End Sub


        ''' <summary>Add all members of a role to recipient list</summary>
        ''' <param name="roleName">name of a role, whose members shall be added to recipients</param>
        ''' <remarks>emaiol will be used for addressing, other properties might be used for TokenReplace</remarks>
        Public Sub AddAddressedRole(ByVal roleName As String)
            _addressedRoles.Add(roleName)
        End Sub


        ''' <summary>All bulk mail recipients, derived from role names and individual adressees </summary>
        ''' <returns>List of userInfo objects, who receive the bulk mail </returns>
        ''' <remarks>user.Email used for sending, other properties might be used for TokenReplace</remarks>
        Public Function Recipients() As System.Collections.Generic.List(Of UserInfo)
            Dim objUserList As New System.Collections.Generic.List(Of UserInfo)
            Dim objKeyList As New System.Collections.Generic.List(Of String)
            Dim objRoles As New RoleController
            For Each strRolename As String In _addressedRoles
                Dim roleInfo As RoleInfo
                roleInfo = objRoles.GetRoleByName(_portalSettings.PortalId, strRolename)

                For Each objUser As UserInfo In objRoles.GetUsersByRoleName(_portalSettings.PortalId, strRolename)
                    DotNetNuke.Entities.Profile.ProfileController.GetUserProfile(objUser) 'no autoHydrate :(
                    Dim userRole As UserRoleInfo
                    userRole = objRoles.GetUserRole(_portalSettings.PortalId, objUser.UserID, roleInfo.RoleID)
                    'only add if user role has not expired and effectivedate has been passed
                    If (userRole.EffectiveDate <= Now Or Null.IsNull(userRole.EffectiveDate) = True) And (userRole.ExpiryDate >= Now Or Null.IsNull(userRole.ExpiryDate) = True) Then
                        conditionallyAddUser(objUser, objKeyList, objUserList)
                    End If
                Next
            Next
            For Each objUser As UserInfo In _addressedUsers
                conditionallyAddUser(objUser, objKeyList, objUserList)
            Next
            Return objUserList
        End Function


        ''' <summary>Send bulkmail to all recipients according to settings</summary>
        ''' <returns>Number of emails sent, null.integer if not determinable</returns>
        ''' <remarks>Detailed status report is sent by email to sending user</remarks>
        Public Function SendMails() As Integer
            Dim intRecipients As Integer = 0
            Dim intMessagesSent As Integer = 0
            Dim intErrors As Integer = 0
            Try
                ' send to recipients
                Dim strBody As String = _strBody
                If _bodyFormat = MailFormat.Html Then ' Add Base Href for any images inserted in to the email.
                    strBody = "<base href='" & AddHTTP(_PortalAlias) & "' />" & strBody
                End If
                Dim strSubject As String = _strSubject
                Dim myUser As UserInfo = Nothing
                Dim startedAt As String = Now().ToString

                Dim _bolReplaceTokens As Boolean = True
                _bolReplaceTokens = Not _bolSuppressTR AndAlso (_objTR.ContainsTokens(_strSubject) OrElse _objTR.ContainsTokens(_strBody))
                Dim IndividualSubj As Boolean = False
                Dim IndividualBody As Boolean = False

                Dim stbMailErrors As New System.Text.StringBuilder
                Dim StbRecipients As New System.Text.StringBuilder

                Select Case _addressMethod
                    Case AddressMethods.Send_TO, AddressMethods.Send_Relay
                        'optimization:
                        If _bolReplaceTokens Then
                            IndividualBody = (_objTR.Cacheability(_strBody) = CacheLevel.notCacheable)
                            IndividualSubj = (_objTR.Cacheability(_strSubject) = CacheLevel.notCacheable)
                            If Not IndividualBody Then strBody = _objTR.ReplaceEnvironmentTokens(strBody)
                            If Not IndividualSubj Then strSubject = _objTR.ReplaceEnvironmentTokens(strSubject)
                        End If
                        For Each myUser In Recipients()
                            intRecipients += 1
                            If IndividualBody Or IndividualSubj Then
                                _objTR.User = myUser
                                _objTR.AccessingUser = myUser
                                If IndividualBody Then strBody = _objTR.ReplaceEnvironmentTokens(_strBody)
                                If IndividualSubj Then strSubject = _objTR.ReplaceEnvironmentTokens(_strSubject)
                            End If

                            Dim strRecipient As String
                            If _addressMethod = AddressMethods.Send_TO Then
                                strRecipient = myUser.Email.ToString
                            Else
                                strRecipient = RelayEmailAddress.ToString
                            End If
                            Dim strMailError As String = Mail.SendMail(_sendingUser.Email, strRecipient, "", "", Me.ReplyTo.Email, _priority, strSubject, _bodyFormat, _
                                           System.Text.Encoding.UTF8, strBody, _attachments.ToArray(), _
                                           _strSMTPServer, _strSMTPAuthentication, _strSMTPUsername, _strSMTPPassword, _bolSMTPEnableSSL)
                            If Not String.IsNullOrEmpty(strMailError) Then
                                stbMailErrors.Append(strMailError)
                                stbMailErrors.AppendLine()
                                intErrors += 1
                            Else
                                StbRecipients.Append(myUser.Email)
                                If _bodyFormat = MailFormat.Html Then
                                    StbRecipients.Append("<br />")
                                Else
                                    StbRecipients.Append(vbCrLf)
                                End If
                                intMessagesSent += 1
                            End If
                        Next myUser
                    Case AddressMethods.Send_BCC
                        Dim stbDistributionList As New System.Text.StringBuilder
                        intMessagesSent = Null.NullInteger
                        For Each myUser In Recipients()
                            intRecipients += 1
                            stbDistributionList.Append(myUser.Email & "; ")
                            StbRecipients.Append(myUser.Email)
                            If _bodyFormat = MailFormat.Html Then
                                StbRecipients.Append("<br />")
                            Else
                                StbRecipients.Append(vbCrLf)
                            End If
                        Next
                        If stbDistributionList.Length > 2 Then
                            If _bolReplaceTokens Then
                                'no access to User properties possible!
                                Dim tr As New TokenReplace(Scope.Configuration)
                                strBody = tr.ReplaceEnvironmentTokens(_strBody)
                                strSubject = tr.ReplaceEnvironmentTokens(_strSubject)
                            Else
                                strBody = _strBody
                                strSubject = _strSubject
                            End If
                            Dim err As String = Mail.SendMail(_sendingUser.Email, _sendingUser.Email, "", stbDistributionList.ToString(0, stbDistributionList.Length - 2), Me.ReplyTo.Email, _priority, strSubject, _bodyFormat, _
                                           System.Text.Encoding.UTF8, strBody, _attachments.ToArray(), _
                                           _strSMTPServer, _strSMTPAuthentication, _strSMTPUsername, _strSMTPPassword, _bolSMTPEnableSSL)
                            If err = String.Empty Then
                                intMessagesSent = 1
                            Else
                                stbMailErrors.Append(err)
                                intErrors += 1
                            End If
                        End If
                End Select
                If stbMailErrors.Length > 0 Then StbRecipients = New System.Text.StringBuilder
                SendConfirmationMail(intRecipients, intMessagesSent, intErrors, strSubject, startedAt, stbMailErrors.ToString, StbRecipients.ToString)
            Catch exc As Exception
                ' send mail failure
                System.Diagnostics.Debug.Write(exc.Message)
            End Try
            Return intMessagesSent
        End Function

        ''' <summary>Wrapper for Function SendMails</summary>
        Public Sub Send()
            Dim void As Integer = SendMails()
        End Sub
#End Region
    End Class

End Namespace
