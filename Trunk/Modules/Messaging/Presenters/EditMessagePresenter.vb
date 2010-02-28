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
Imports DotNetNuke.Web.Mvp.Framework
Imports DotNetNuke.Web.Validators
Imports DotNetNuke.Modules.Messaging.Views
Imports DotNetNuke.Services.Messaging.Providers
Imports DotNetNuke.Services.Messaging
Imports DotNetNuke.Services.Messaging.Data
Imports DotNetNuke.Entities.Users

Namespace DotNetNuke.Modules.Messaging.Presenters

    Public Class EditMessagePresenter
        Inherits Presenter(Of IEditMessageView, EditMessagePresenterModel)

#Region "Private Members"

        Private _Message As Message
        Private _MessagingController As IMessagingController

#End Region

#Region "Public Constants"

        Public Const Name As String = "EditMessage"

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New(New MessagingController(New MessagingDataService()))
        End Sub

        Public Sub New(ByVal messagingController As IMessagingController)
            Arg.NotNull("messagingController", messagingController)

            _MessagingController = messagingController
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property IsAddMode() As Boolean
            Get
                Return (Me.Model.IndexId = Null.NullInteger)
            End Get
        End Property

        Public ReadOnly Property IsReplyMode() As Boolean
            Get
                Return (Me.Model.OriginalIndexId > Null.NullInteger)
            End Get
        End Property

        <ViewState()> _
        Public Property Message() As Message
            Get
                Return _Message
            End Get
            Set(ByVal value As Message)
                _Message = value
            End Set
        End Property

#End Region

#Region "Public Methods"

        Public Function Cancel() As Boolean
            Environment.RedirectToPresenter(New MessagesListPresenterModel())
        End Function

        Public Function DeleteMessage() As Boolean
            'Delete(Message)
            _MessagingController.DeleteMessage(Me.Model.PortalId, Me.Model.IndexId)

            'Redirect to List
            Environment.RedirectToPresenter(New MessagesListPresenterModel())
        End Function

        Public Overrides Function Load() As Boolean
            If Not Model.IsPostBack Then
                If IsAddMode Then
                    If IsReplyMode Then
                        'Get the original message
                        Message = _MessagingController.GetMessageByID(Me.Model.PortalId, Me.Model.UserId, Me.Model.OriginalIndexId)

                        'Convert to Reply
                        Message.ConvertToNewReply()
                    Else
                        Message = New Message()
                    End If
                    View.HideDeleteButton()
                Else
                    Message = _MessagingController.GetMessageByID(Me.Model.PortalId, Me.Model.UserId, Me.Model.IndexId)
                End If
            End If

            View.BindMessage(Message)
        End Function

        Public Function SaveMessage(ByVal userName As String, ByVal isDraft As Boolean) As Boolean
            'Rebind message to load values from controls
            View.BindMessage(Message)

            Message.ToUserID = ValidateUserName(userName)

            If Message.ToUserID > Null.NullInteger Then
                Message.FromUserID = Me.Model.UserId
                Message.PendingSend = Not isDraft
                If isDraft Then
                    Message.Status = "Draft"
                Else
                    Message.Status = "Unread"
                End If

                'Save Message
                _MessagingController.SaveMessage(Message)

                'Redirect to Message List
                Environment.RedirectToPresenter(New MessagesListPresenterModel())
            Else
                View.ShowMessage("Validation.Error", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If
        End Function

        Public Function ValidateUser(ByVal userName As String) As Boolean
            ' validate username
            If ValidateUserName(userName) < 0 Then
                View.ClearToField()
            End If
        End Function

#End Region

        Private Function ValidateUserName(ByVal userName As String) As Integer
            Dim userId As Integer = Null.NullInteger
            If Not String.IsNullOrEmpty(userName) Then
                ' validate username
                Dim objUser As UserInfo = UserController.GetUserByName(Me.Model.PortalId, userName)
                If objUser IsNot Nothing Then
                    userId = objUser.UserID
                End If
            End If
            Return userId
        End Function

    End Class

End Namespace
