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
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Messaging.Views
Imports DotNetNuke.Services.Messaging
Imports DotNetNuke.Services.Messaging.Data
Imports Telerik.Web.UI
Imports DotNetNuke.Web.Mvp
Imports DotNetNuke.Web.UI.WebControls

Namespace DotNetNuke.Modules.Messaging.Presenters

    Public Class MessageListPresenter
        Inherits ModulePresenter(Of IMessageListView)

#Region "Private Fields"

        Private _MessagingController As IMessagingController

#End Region

#Region "Constructors"

        Public Sub New(ByVal listView As IMessageListView)
            Me.New(listView, New MessagingController(New MessagingDataService()))
        End Sub

        Public Sub New(ByVal listView As IMessageListView, ByVal messagingController As IMessagingController)
            MyBase.New(listView)
            Arg.NotNull("messagingController", messagingController)
            _MessagingController = messagingController

            AddHandler View.AddMessage, AddressOf AddMessage
            AddHandler View.DeleteSelectedMessages, AddressOf DeleteSelectedMessages
            AddHandler View.MarkSelectedMessagesRead, AddressOf MarkSelectedMessagesRead
            AddHandler View.MarkSelectedMessagesUnread, AddressOf MarkSelectedMessagesUnread
            AddHandler View.MessageDataBound, AddressOf MessageDataBound
            AddHandler View.MessagesNeedDataSource, AddressOf MessagesNeedDataSource
        End Sub

#End Region

        Protected Overrides Sub OnInit()
            MyBase.OnInit()

            View.Model.Messages = _MessagingController.GetUserInbox(PortalId, UserId, 1, 999)
        End Sub

        Public Sub AddMessage(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect(NavigateURL(TabId, _
                                          "EditMessage", _
                                          String.Format("mid={0}", ModuleId)))
        End Sub

        Public Sub DeleteSelectedMessages(ByVal sender As Object, ByVal e As DnnGridItemSelectedEventArgs)
            For Each c In e.SelectedItems
                Dim messageID = CType(c.OwnerTableView.DataKeyValues(c.ItemIndex)("MessageID"), Long)
                Dim message = _MessagingController.GetMessageByID(PortalId, UserId, messageID)
                message.Status = MessageStatusType.Deleted
                _MessagingController.UpdateMessage(message)
            Next
        End Sub

        Public Sub MarkSelectedMessagesRead(ByVal sender As Object, ByVal e As DnnGridItemSelectedEventArgs)
            For Each c In e.SelectedItems
                Dim messageID = CType(c.OwnerTableView.DataKeyValues(c.ItemIndex)("MessageID"), Long)
                Dim message = _MessagingController.GetMessageByID(PortalId, UserId, messageID)

                If (message.Status = MessageStatusType.Unread) Then
                    message.Status = MessageStatusType.Read
                    _MessagingController.UpdateMessage(message)
                End If
            Next
        End Sub

        Public Sub MarkSelectedMessagesUnread(ByVal sender As Object, ByVal e As DnnGridItemSelectedEventArgs)
            For Each c In e.SelectedItems
                Dim messageID = CType(c.OwnerTableView.DataKeyValues(c.ItemIndex)("MessageID"), Long)
                Dim message = _MessagingController.GetMessageByID(PortalId, UserId, messageID)

                If (message.Status = MessageStatusType.Read) Then
                    message.Status = MessageStatusType.Unread
                    _MessagingController.UpdateMessage(message)
                End If
                message.Status = MessageStatusType.Unread
                _MessagingController.UpdateMessage(message)
            Next
        End Sub

        Public Sub MessageDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs)
            If e.Item.ItemType = GridItemType.AlternatingItem Or e.Item.ItemType = GridItemType.Item Then
                Dim message As Message = TryCast(e.Item.DataItem, Message)

                Dim item As GridDataItem = e.Item

                Dim hyperLinkColumn As HyperLink = TryCast(item.Controls(4).Controls(0), HyperLink)

                If hyperLinkColumn IsNot Nothing Then
                    If message.Status = MessageStatusType.Draft Then
                        'Message is from me
                        hyperLinkColumn.NavigateUrl = NavigateURL(TabId, _
                                                                    "EditMessage", _
                                                                    String.Format("mid={0}", ModuleId), _
                                                                    String.Format("MessageId={0}", message.MessageID))
                        hyperLinkColumn.Text = String.Format("[Draft] {0}", message.Subject)
                    Else
                        'Message is to me
                        hyperLinkColumn.NavigateUrl = NavigateURL(TabId, _
                                                                    "ViewMessage", _
                                                                    String.Format("mid={0}", ModuleId), _
                                                                    String.Format("MessageId={0}", message.MessageID))
                        hyperLinkColumn.Text = message.Subject
                    End If
                End If

                If (message.Status = MessageStatusType.Unread) Then
                    hyperLinkColumn.Font.Bold = True
                End If
            End If
        End Sub

        Public Sub MessagesNeedDataSource(ByVal sender As Object, ByVal e As GridNeedDataSourceEventArgs)
            Dim mGrid = DirectCast(sender, DnnGrid)
            mGrid.PagerStyle.AlwaysVisible = True
            mGrid.VirtualItemCount = _MessagingController.GetInboxCount(PortalId, UserId)
            mGrid.DataSource = _MessagingController.GetUserInbox(PortalId, UserId, mGrid.CurrentPageIndex + 1, mGrid.PageSize)
        End Sub

    End Class
End Namespace

