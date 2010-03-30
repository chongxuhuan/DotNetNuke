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

Imports DotNetNuke.Modules.Messaging.Presenters
Imports DotNetNuke.Services.Messaging.Data
Imports DotNetNuke.Modules.Messaging.Views.Models
Imports DotNetNuke.Web.Mvp
Imports DotNetNuke.Common.Utilities
Imports WebFormsMvp

Namespace DotNetNuke.Modules.Messaging.Views

    <PresenterBinding(GetType(ViewMessagePresenter))> _
    Partial Public Class ViewMessage
        Inherits ModuleView(Of ViewMessageModel)
        Implements IViewMessageView


#Region "IViewMessageView Implementation"

        Public Event Cancel(ByVal sender As Object, ByVal e As EventArgs) Implements IViewMessageView.Cancel
        Public Event Delete(ByVal sender As Object, ByVal e As EventArgs) Implements IViewMessageView.Delete
        Public Event Reply(ByVal sender As Object, ByVal e As EventArgs) Implements IViewMessageView.Reply

        Public Sub BindMessage(ByVal message As Message) Implements IViewMessageView.BindMessage
            fromLabel.Text = message.FromUserName
            subjectLabel.Text = message.Subject
            messageLabel.Text = HtmlUtils.ConvertToHtml(message.Body)
        End Sub

#End Region

        Private Sub cancelView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cancelView.Click
            RaiseEvent Cancel(Me, e)
        End Sub

        Private Sub deleteMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles deleteMessage.Click
            RaiseEvent Delete(Me, e)
        End Sub

        Private Sub ReplyMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles replyMessage.Click
            RaiseEvent Reply(Me, e)
        End Sub
    End Class

End Namespace
