'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Messaging.Views
Imports DotNetNuke.Services.Messaging
Imports DotNetNuke.Services.Messaging.Data
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Web.Mvp

Namespace DotNetNuke.Modules.Messaging.Presenters

    Public Class EditMessagePresenter
        Inherits ModulePresenter(Of IEditMessageView)

#Region "Private Members"

        Private _MessagingController As IMessagingController

#End Region

#Region "Constructors"

        Public Sub New(ByVal editView As IEditMessageView)
            Me.New(editView, New MessagingController(New MessagingDataService()))
        End Sub

        Public Sub New(ByVal editView As IEditMessageView, ByVal messagingController As IMessagingController)
            MyBase.New(editView)
            Arg.NotNull("messagingController", messagingController)

            _MessagingController = messagingController

            AddHandler View.Cancel, AddressOf Cancel
            AddHandler View.Delete, AddressOf DeleteMessage
            AddHandler View.Load, AddressOf Load
            AddHandler View.SaveDraft, AddressOf SaveDraft
            AddHandler View.SendMessage, AddressOf SendMessage
            AddHandler View.ValidateUser, AddressOf ValidateUser
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property MessageId() As Long
            Get
                Dim _IndexId As Long = Null.NullInteger
                If Not String.IsNullOrEmpty(Request.Params("MessageId")) Then
                    _IndexId = Int32.Parse(Request.Params("MessageId"))
                End If
                Return _IndexId
            End Get
        End Property


        Public ReadOnly Property IsReplyMode() As Boolean
            Get
                Dim _isReply As Boolean
                If Not String.IsNullOrEmpty(Request.Params("IsReply")) Then
                    Boolean.TryParse(Request.Params("IsReply"), _isReply)
                End If
                Return _isReply
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Function GetInboxUrl() As String
            Return NavigateURL(TabId, "", String.Format("userId={0}", UserId))
        End Function

#End Region

#Region "Public Methods"

        Public Sub Cancel(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect(GetInboxUrl())
        End Sub


        Public Sub DeleteMessage(ByVal sender As Object, ByVal e As EventArgs)
            View.BindMessage(View.Model.Message)

            View.Model.Message.Status = MessageStatusType.Deleted
            _MessagingController.UpdateMessage(View.Model.Message)

            'Redirect to List
            Response.Redirect(GetInboxUrl())
        End Sub

        Public Sub Load(ByVal sender As Object, ByVal e As EventArgs)
            If Not IsPostBack Then

                If (MessageId > 0) Then

                    If IsReplyMode Then
                        View.Model.Message = _MessagingController.GetMessageByID(PortalId, UserId, MessageId).GetReplyMessage()
                        View.HideDeleteButton()
                    Else
                        View.Model.Message = _MessagingController.GetMessageByID(PortalId, UserId, MessageId)
                    End If
                Else
                    View.Model.Message = New Message()
                End If

                View.BindMessage(View.Model.Message)
            End If
        End Sub

        Public Sub SaveDraft(ByVal sender As Object, ByVal e As EventArgs)
            SubmitMessage(MessageStatusType.Draft)
        End Sub

        Public Function SendMessage(ByVal sender As Object, ByVal e As EventArgs) As Boolean
            SubmitMessage(MessageStatusType.Unread)
        End Function

        Private Sub SubmitMessage(ByVal status As MessageStatusType)

            View.BindMessage(View.Model.Message)

            View.Model.Message.ToUserID = ValidateUserName(View.Model.UserName)

            If View.Model.Message.ToUserID > Null.NullInteger Then

                View.Model.Message.FromUserID = UserId
                View.Model.Message.MessageDate = DateTime.Now


                View.Model.Message.Status = status

                'Save Message
                If (View.Model.Message.MessageID = 0) Then
                    _MessagingController.SaveMessage(View.Model.Message)
                Else
                    _MessagingController.UpdateMessage(View.Model.Message)
                End If

                'Redirect to Message List
                Response.Redirect(GetInboxUrl())
            End If
        End Sub

        Public Function ValidateUser(ByVal sender As Object, ByVal e As EventArgs) As Boolean
            ' validate username
            If (ValidateUserName(View.Model.UserName) > 0) Then
                View.ShowValidUserMessage()
            End If
        End Function

#End Region

        Private Function ValidateUserName(ByVal userName As String) As Integer
            Dim userId As Integer = Null.NullInteger
            If Not String.IsNullOrEmpty(userName) Then
                ' validate username
                Dim objUser As UserInfo = UserController.GetUserByName(PortalId, userName)
                If objUser IsNot Nothing Then
                    userId = objUser.UserID
                End If
            End If

            If (userId = Null.NullInteger) Then
                View.ShowInvalidUserError()
            End If

            Return userId
        End Function

    End Class

End Namespace
