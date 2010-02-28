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
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Web.Mvp.Framework
Imports DotNetNuke.Modules.Messaging.Views
Imports DotNetNuke.Services.Messaging.Providers
Imports DotNetNuke.Services.Messaging
Imports DotNetNuke.Services.Messaging.Data

Namespace DotNetNuke.Modules.Messaging.Presenters

    Public Class MessagesListPresenter
        Inherits Presenter(Of IMessagesListView, MessagesListPresenterModel)

#Region "Private Methods"

        Private _Messages As List(Of Message)
        Private _MessagingController As IMessagingController

#End Region

#Region "Public Constants"

        Public Const Name As String = ""

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

        Public Property Messages() As List(Of Message)
            Get
                Return _Messages
            End Get
            Set(ByVal value As List(Of Message))
                _Messages = value
            End Set
        End Property

#End Region

        Public Function AddMessage(ByVal sender As Object, ByVal e As EventArgs) As Boolean
            Environment.RedirectToPresenter(New EditMessagePresenterModel())
        End Function

        Public Overrides Function Load() As Boolean
            Messages = (From pm In _MessagingController.GetMessagesForUser(Me.Model.PortalId, Me.Model.UserId) _
                                Where (pm.Status = "Draft" AndAlso pm.FromUserID = Me.Model.UserId) _
                                    OrElse (pm.Status = "Unread" AndAlso pm.ToUserID = Me.Model.UserId) _
                                    OrElse (pm.Status = "Read" AndAlso pm.ToUserID = Me.Model.UserId) _
                                Select pm) _
                                .ToList()

            View.ShowMessages(Messages)
        End Function

        Public Function MessageDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs) As Boolean
            Dim item As DataGridItem = e.Item

            If item.ItemType = ListItemType.Item Or _
                    item.ItemType = ListItemType.AlternatingItem Or _
                    item.ItemType = ListItemType.SelectedItem Then

                Dim message As Message = TryCast(item.DataItem, Message)

                Dim hyperLinkColumn As HyperLink = TryCast(item.Controls(0).Controls(0), HyperLink)

                If hyperLinkColumn IsNot Nothing Then
                    If message.ToUserID = Me.Model.UserId Then
                        'Message is to me
                        hyperLinkColumn.NavigateUrl = NavigateURL(Me.Model.TabId, _
                                                                    "ViewMessage", _
                                                                    "mid=" + Me.Model.ModuleId.ToString(), _
                                                                    "IndexID=" + message.IndexID.ToString())
                        hyperLinkColumn.Text = "View"
                    ElseIf message.Status = "Draft" Then
                        'Message is from me
                        hyperLinkColumn.NavigateUrl = NavigateURL(Me.Model.TabId, _
                                                                    "EditMessage", _
                                                                    "mid=" + Me.Model.ModuleId.ToString(), _
                                                                    "IndexID=" + message.IndexID.ToString())
                        hyperLinkColumn.Text = "Edit"
                    End If
                End If

            End If

        End Function

    End Class
End Namespace

