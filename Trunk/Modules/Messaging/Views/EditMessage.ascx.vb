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
Imports DotNetNuke.Web.Mvp
Imports DotNetNuke.Modules.Messaging.Views.Models
Imports WebFormsMvp

Namespace DotNetNuke.Modules.Messaging.Views

    <PresenterBinding(GetType(EditMessagePresenter))> _
    Partial Public Class EditMessage
        Inherits ModuleView(Of EditMessageModel)
        Implements IEditMessageView


#Region "IEditMessageView Implementation"

        Public Event Cancel(ByVal sender As Object, ByVal e As EventArgs) Implements IEditMessageView.Cancel
        Public Event Delete(ByVal sender As Object, ByVal e As EventArgs) Implements IEditMessageView.Delete
        Public Event SaveDraft(ByVal sender As Object, ByVal e As EventArgs) Implements IEditMessageView.SaveDraft
        Public Event SendMessage(ByVal sender As Object, ByVal e As EventArgs) Implements IEditMessageView.SendMessage
        Public Event ValidateUser(ByVal sender As Object, ByVal e As EventArgs) Implements IEditMessageView.ValidateUser

        Public Sub BindMessage(ByVal message As Message) Implements IEditMessageView.BindMessage
            If Me.IsPostBack Then
                message.Subject = subjectTextBox.Text
                message.Body = messageEditor.Text
            Else
                toTextBox.Text = message.ToUserName
                toTextBox.ToolTip = message.ToUserName
                subjectTextBox.Text = message.Subject
                messageEditor.Text = message.Body
            End If
        End Sub

        Public Sub ClearToField() Implements IEditMessageView.ClearToField
            toTextBox.Text = ""
        End Sub

        Public Sub HideDeleteButton() Implements IEditMessageView.HideDeleteButton
            deleteHolder.Visible = False
        End Sub

#End Region

        Private Sub cancelEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cancelEdit.Click
            RaiseEvent Cancel(Me, e)
        End Sub

        Private Sub deleteMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles deleteMessage.Click
            RaiseEvent Delete(Me, e)
        End Sub

        Private Sub saveDraftButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles saveDraftButton.Click
            Model.UserName = toTextBox.Text
            RaiseEvent SaveDraft(Me, e)
        End Sub

        Private Sub sendMessageButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles sendMessageButton.Click
            Model.UserName = toTextBox.Text
            RaiseEvent SendMessage(Me, e)
        End Sub

        Private Sub validateUserButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles validateUserButton.Click
            Model.UserName = toTextBox.Text
            RaiseEvent ValidateUser(Me, e)
        End Sub
    End Class

End Namespace
