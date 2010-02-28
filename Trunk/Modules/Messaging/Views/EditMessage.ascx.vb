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
Imports DotNetNuke.Web.Mvp.Framework
Imports DotNetNuke.Services.Messaging.Providers
Imports DotNetNuke.UI.UserControls

Namespace DotNetNuke.Modules.Messaging.Views

    Partial Public Class EditMessage
        Inherits ViewBase(Of IEditMessageView, EditMessagePresenter, EditMessagePresenterModel)
        Implements IEditMessageView

#Region "Protected Properties"

        Protected Overrides ReadOnly Property View() As IEditMessageView
            Get
                Return Me
            End Get
        End Property

#End Region

#Region "ViewBase Virtual/Abstract Method Overrides"

        Protected Overrides Sub ConnectEvents()
            MyBase.ConnectEvents()

            AddHandler cancelEdit.Click, CreateSimpleHandler(Function(p) p.Cancel())
            AddHandler deleteMessage.Click, CreateSimpleHandler(Function(p) p.DeleteMessage())
            AddHandler sendMessage.Click, CreateSaveHandler(Function(p) p.SaveMessage(toTextBox.Text, False))
            AddHandler saveDraft.Click, CreateSaveHandler(Function(p) p.SaveMessage(toTextBox.Text, True))
            AddHandler validateUser.Click, CreateSimpleHandler(Function(p) p.ValidateUser(toTextBox.Text))
        End Sub

        Protected Overrides Sub Localize()
        End Sub

#End Region

        Public Sub BindMessage(ByVal message As Message) Implements IEditMessageView.BindMessage
            If Me.IsPostBack Then
                message.Subject = subjectTextBox.Text
                message.LongBody = messageEditor.Text
            Else
                toTextBox.Text = message.ToUsername
                toTextBox.ToolTip = message.ToDisplayName
                subjectTextBox.Text = message.Subject
                messageEditor.Text = message.LongBody
            End If
        End Sub

        Public Sub ClearToField() Implements IEditMessageView.ClearToField
            toTextBox.Text = ""
        End Sub

        Public Sub HideDeleteButton() Implements IEditMessageView.HideDeleteButton
            deleteHolder.Visible = False
        End Sub

    End Class

End Namespace
