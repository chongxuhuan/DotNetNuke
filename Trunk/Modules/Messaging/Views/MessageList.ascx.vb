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

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Web.Mvp
Imports DotNetNuke.Modules.Messaging.Views.Models
Imports Telerik.Web.UI
Imports DotNetNuke.Web.UI.WebControls
Imports WebFormsMvp
Imports DotNetNuke.Modules.Messaging.Presenters

Namespace DotNetNuke.Modules.Messaging.Views

    <PresenterBinding(GetType(MessageListPresenter))> _
    Partial Public Class MessageList
        Inherits ModuleView(Of MessageListModel)
        Implements IMessageListView
        Implements IProfileModule

#Region "IProfileModule Implementation"

        Public ReadOnly Property DisplayModule() As Boolean Implements IProfileModule.DisplayModule
            Get
                Return (ProfileUserId = ModuleContext.PortalSettings.UserId)
            End Get
        End Property

        Public ReadOnly Property ProfileUserId() As Integer Implements IProfileModule.ProfileUserId
            Get
                Dim _ProfileUserId As Integer = Null.NullInteger
                If Not String.IsNullOrEmpty(Request.Params("UserId")) Then
                    _ProfileUserId = Int32.Parse(Request.Params("UserId"))
                End If
                Return _ProfileUserId
            End Get
        End Property

#End Region

#Region "IMessageListView Implementation"

        Public Event AddMessage(ByVal sender As Object, ByVal e As EventArgs) Implements IMessageListView.AddMessage
        Public Event DeleteSelectedMessages(ByVal sender As Object, ByVal e As DnnGridItemSelectedEventArgs) Implements IMessageListView.DeleteSelectedMessages
        Public Event MarkSelectedMessagesRead(ByVal sender As Object, ByVal e As DnnGridItemSelectedEventArgs) Implements IMessageListView.MarkSelectedMessagesRead
        Public Event MarkSelectedMessagesUnread(ByVal sender As Object, ByVal e As DnnGridItemSelectedEventArgs) Implements IMessageListView.MarkSelectedMessagesUnread
        Public Event MessageDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Implements IMessageListView.MessageDataBound
        Public Event MessagesNeedDataSource(ByVal sender As Object, ByVal e As GridNeedDataSourceEventArgs) Implements IMessageListView.MessagesNeedDataSource

#End Region

#Region "Event Handlers"

        Private Sub addMessageButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles addMessageButton.Click
            RaiseEvent AddMessage(Me, e)
        End Sub

        Private Sub delete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles delete.Click
            RaiseEvent DeleteSelectedMessages(Me, New DnnGridItemSelectedEventArgs(messagesGrid.SelectedItems))
            messagesGrid.Rebind()
        End Sub

        Private Sub markAsRead_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles markAsRead.Click
            RaiseEvent MarkSelectedMessagesRead(Me, New DnnGridItemSelectedEventArgs(messagesGrid.SelectedItems))
            messagesGrid.Rebind()
        End Sub

        Private Sub markAsUnread_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles markAsUnread.Click
            RaiseEvent MarkSelectedMessagesUnread(Me, New DnnGridItemSelectedEventArgs(messagesGrid.SelectedItems))
            messagesGrid.Rebind()
        End Sub

        Private Sub messagesGrid_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles messagesGrid.ItemDataBound
            RaiseEvent MessageDataBound(messagesGrid, e)
        End Sub

        Private Sub messagesGrid_NeedDataSource(ByVal source As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles messagesGrid.NeedDataSource
            RaiseEvent MessagesNeedDataSource(messagesGrid, e)
        End Sub

#End Region

    End Class
End Namespace
