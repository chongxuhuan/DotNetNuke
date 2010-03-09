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
Imports DotNetNuke.Web.Mvp
Imports DotNetNuke.Modules.Messaging.Views.Models

Namespace DotNetNuke.Modules.Messaging.Presenters

    Public Class ViewMessagePresenter
        Inherits ModulePresenter(Of IViewMessageView)

#Region "Private Members"

        Private _MessagingController As IMessagingController

#End Region

#Region "Constructors"

        Public Sub New(ByVal viewView As IViewMessageView)
            Me.New(viewView, New MessagingController(New MessagingDataService()))
        End Sub

        Public Sub New(ByVal viewView As IViewMessageView, ByVal messagingController As IMessagingController)
            MyBase.New(viewView)
            Arg.NotNull("messagingController", messagingController)

            _MessagingController = messagingController

            AddHandler View.Cancel, AddressOf Cancel
            AddHandler View.Delete, AddressOf DeleteMessage
            AddHandler View.Load, AddressOf Load
            AddHandler View.Reply, AddressOf Reply
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property IndexId() As Integer
            Get
                Dim _IndexId As Integer = Null.NullInteger
                If Not String.IsNullOrEmpty(Request.Params("MessageId")) Then
                    _IndexId = Int32.Parse(Request.Params("MessageId"))
                End If
                Return _IndexId
            End Get
        End Property

#End Region



#Region "Private Methods"

        Private Function GetInboxUrl() As String
            Return NavigateURL(TabId, "", String.Format("userId={0}", UserId))
        End Function

#End Region

#Region "Public Methods"

        Public Function Cancel() As Boolean
            Response.Redirect(GetInboxUrl())
        End Function

        Public Sub DeleteMessage(ByVal sender As Object, ByVal e As EventArgs)
            View.BindMessage(View.Model.Message)

            View.Model.Message.Status = MessageStatusType.Deleted
            _MessagingController.UpdateMessage(View.Model.Message)

            'Redirect to List
            Response.Redirect(GetInboxUrl())
        End Sub

        Public Sub Load(ByVal sender As Object, ByVal e As EventArgs)
            If Not IsPostBack Then
                View.Model.Message = _MessagingController.GetMessageByID(PortalId, UserId, IndexId)
                If View.Model.Message Is Nothing Then
                    'Redirect - message does not belong to user
                    Response.Redirect(AccessDeniedURL())
                End If
                If View.Model.Message.Status = MessageStatusType.Unread Then
                    View.Model.Message.Status = MessageStatusType.Read
                    _MessagingController.UpdateMessage(View.Model.Message)
                End If
                View.BindMessage(View.Model.Message)
            End If

        End Sub

        Public Function Reply() As Boolean
            Response.Redirect(NavigateURL(TabId, "EditMessage", String.Format("mid={0}", ModuleId), String.Format("MessageId={0}", View.Model.Message.MessageID), "IsReply=true"))
        End Function


#End Region
    End Class

End Namespace
