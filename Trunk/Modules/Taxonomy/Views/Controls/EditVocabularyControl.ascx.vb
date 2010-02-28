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

Imports DotNetNuke.Entities.Content.Taxonomy

Namespace DotNetNuke.Modules.Taxonomy.Views.Controls

    Partial Public Class EditVocabularyControl
        Inherits UserControl

#Region "Private Members"

        Private _IsAddMode As Boolean
        Private _LocalResourceFile As String

#End Region

#Region "Public Properties"

        Public Property IsAddMode() As Boolean
            Get
                Return _IsAddMode
            End Get
            Set(ByVal value As Boolean)
                _IsAddMode = value
            End Set
        End Property

        Public Property LocalResourceFile() As String
            Get
                Return _LocalResourceFile
            End Get
            Set(ByVal value As String)
                _LocalResourceFile = value
            End Set
        End Property

#End Region

        Public Sub BindVocabulary(ByVal vocabulary As Vocabulary, ByVal editEnabled As Boolean, ByVal showScope As Boolean)
            If IsPostBack Then
                vocabulary.Name = nameTextBox.Text
                vocabulary.Description = descriptionTextBox.Text

                Dim scopeTypeController As New ScopeTypeController
                Dim scopeType As ScopeType
                scopeType = scopeTypeController.GetScopeTypes() _
                                                    .Where(Function(s) s.ScopeType = scopeList.SelectedValue) _
                                                    .SingleOrDefault
                vocabulary.ScopeTypeId = scopeType.ScopeTypeId

                If typeList.SelectedValue = "Simple" Then
                    vocabulary.Type = VocabularyType.Simple
                Else
                    vocabulary.Type = VocabularyType.Hierarchy
                End If
            Else
                nameTextBox.Text = vocabulary.Name
                nameLabel.Text = vocabulary.Name
                descriptionTextBox.Text = vocabulary.Description
                typeList.Items.FindByValue(vocabulary.Type.ToString()).Selected = True
                If vocabulary.ScopeType IsNot Nothing Then
                    scopeLabel.Text = vocabulary.ScopeType.ScopeType
                    scopeList.Items.FindByValue(vocabulary.ScopeType.ScopeType).Selected = True
                End If
                typeLabel.Text = vocabulary.Type.ToString()
            End If

            nameTextBox.Visible = IsAddMode
            nameLabel.Visible = Not IsAddMode
            descriptionTextBox.Enabled = editEnabled
            scopeList.Visible = (IsAddMode AndAlso showScope)
            scopeLabel.Visible = Not (IsAddMode AndAlso showScope)
            typeList.Visible = IsAddMode
            typeLabel.Visible = Not IsAddMode

        End Sub

    End Class

End Namespace
