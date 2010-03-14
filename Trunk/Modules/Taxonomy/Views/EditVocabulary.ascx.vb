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

Imports DotNetNuke.Modules.Taxonomy.Presenters
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Modules.Taxonomy.Views.Models
Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Web.Mvp
Imports WebFormsMvp

Namespace DotNetNuke.Modules.Taxonomy.Views

    <PresenterBinding(GetType(EditVocabularyPresenter))> _
    Partial Public Class EditVocabulary
        Inherits ModuleView(Of EditVocabularyModel)
        Implements IEditVocabularyView

#Region "IEditVocabularyView Implementation"

        Public Event AddTerm(ByVal sender As Object, ByVal e As EventArgs) Implements IEditVocabularyView.AddTerm
        Public Event Cancel(ByVal sender As Object, ByVal e As EventArgs) Implements IEditVocabularyView.Cancel
        Public Event CancelTerm(ByVal sender As Object, ByVal e As EventArgs) Implements IEditVocabularyView.CancelTerm
        Public Event Delete(ByVal sender As Object, ByVal e As EventArgs) Implements IEditVocabularyView.Delete
        Public Event DeleteTerm(ByVal sender As Object, ByVal e As EventArgs) Implements IEditVocabularyView.DeleteTerm
        Public Event Save(ByVal sender As Object, ByVal e As EventArgs) Implements IEditVocabularyView.Save
        Public Event SaveTerm(ByVal sender As Object, ByVal e As EventArgs) Implements IEditVocabularyView.SaveTerm
        Public Event SelectTerm(ByVal sender As Object, ByVal e As TermsEventArgs) Implements IEditVocabularyView.SelectTerm


        Public Sub BindTerm(ByVal term As Term, ByVal terms As IEnumerable(Of Term), ByVal isHeirarchical As Boolean, ByVal loadFromControl As Boolean, ByVal editEnabled As Boolean) Implements IEditVocabularyView.BindTerm
            editTermControl.BindTerm(term, terms, isHeirarchical, loadFromControl, editEnabled)
        End Sub

        Public Sub BindTerms(ByVal terms As IEnumerable(Of Term), ByVal isHeirarchical As Boolean, ByVal dataBind As Boolean) Implements IEditVocabularyView.BindTerms
            termsList.BindTerms(terms.ToList(), isHeirarchical, dataBind)
        End Sub

        Public Sub BindVocabulary(ByVal vocabulary As Vocabulary, ByVal editEnabled As Boolean, ByVal showScope As Boolean) Implements IEditVocabularyView.BindVocabulary
            editVocabularyControl.BindVocabulary(vocabulary, editEnabled, showScope)
            saveVocabulary.Enabled = editEnabled
            deleteVocabulary.Enabled = editEnabled
            addTermButton.Enabled = editEnabled
            saveTermButton.Enabled = editEnabled
            deleteTermButton.Enabled = editEnabled
        End Sub

        Public Sub ClearSelectedTerm() Implements IEditVocabularyView.ClearSelectedTerm
            termsList.ClearSelectedTerm()
            termEditor.Visible = False
        End Sub

        Public Sub SetTermEditorMode(ByVal isAddMode As Boolean, ByVal termId As Integer) Implements IEditVocabularyView.SetTermEditorMode
            If isAddMode Then
                termLabel.Text = Localization.GetString("NewTerm", LocalResourceFile)
                saveTermButton.Text = "CreateTerm"
            Else
                termLabel.Text = Localization.GetString("CurrentTerm", LocalResourceFile)
                saveTermButton.Text = "SaveTerm"
            End If

            deleteTermPlaceHolder.Visible = Not isAddMode
        End Sub

        Public Sub ShowTermEditor(ByVal showEditor As Boolean) Implements IEditVocabularyView.ShowTermEditor
            termEditor.Visible = showEditor
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub addTermButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles addTermButton.Click
            RaiseEvent AddTerm(Me, e)
        End Sub

        Private Sub cancelEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cancelEdit.Click
            RaiseEvent Cancel(Me, e)
        End Sub

        Private Sub cancelTermButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cancelTermButton.Click
            RaiseEvent CancelTerm(Me, e)
        End Sub

        Private Sub deleteTermButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles deleteTermButton.Click
            RaiseEvent DeleteTerm(Me, e)
        End Sub

        Private Sub deleteVocabulary_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles deleteVocabulary.Click
            RaiseEvent Delete(Me, e)
        End Sub

        Private Sub saveTermButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles saveTermButton.Click
            RaiseEvent SaveTerm(Me, e)
        End Sub

        Private Sub saveVocabulary_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles saveVocabulary.Click
            RaiseEvent Save(Me, e)
        End Sub

        Private Sub termsList_SelectedTermChanged(ByVal sender As Object, ByVal e As TermsEventArgs) Handles termsList.SelectedTermChanged
            RaiseEvent SelectTerm(Me, e)
        End Sub

#End Region

    End Class

End Namespace
