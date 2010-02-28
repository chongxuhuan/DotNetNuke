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
Imports DotNetNuke.Modules.Taxonomy.Presenters
Imports DotNetNuke.Modules.Taxonomy.ViewModels
Imports DotNetNuke.Modules.Taxonomy.WebControls
Imports DotNetNuke.Web.Mvp.Framework
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Content.Taxonomy

Namespace DotNetNuke.Modules.Taxonomy.Views

    Partial Public Class EditVocabulary
        Inherits ViewBase(Of IEditVocabularyView, EditVocabularyPresenter, EditVocabularyPresenterModel)
        Implements IEditVocabularyView

#Region "Protected Properties"

        Protected Overrides ReadOnly Property View() As IEditVocabularyView
            Get
                Return Me
            End Get
        End Property

#End Region

#Region "ViewBase Virtual/Abstract Method Overrides"

        Protected Overrides Sub ConnectEvents()
            MyBase.ConnectEvents()

            AddHandler deleteVocabulary.Click, CreateSimpleHandler(Function(p) p.DeleteVocabulary())
            AddHandler cancelEdit.Click, CreateSimpleHandler(Function(p) p.Cancel())
            AddHandler saveVocabulary.Click, CreateSaveHandler(Function(p) p.SaveVocabulary())

            AddHandler termsList.SelectedTermChanged, CreateSimpleHandler(Of TermsListEventArgs)( _
                                                                Function(p, a) _
                                                                    p.SelectTerm(a.SelectedTerm) _
                                                                )
            AddHandler addTerm.Click, CreateSimpleHandler(Function(p) p.AddTerm())
            AddHandler deleteTerm.Click, CreateSimpleHandler(Function(p) p.DeleteTerm())
            AddHandler cancelTerm.Click, CreateSimpleHandler(Function(p) p.CancelTerm())
            AddHandler saveTerm.Click, CreateSaveHandler(Function(p) p.SaveTerm())
        End Sub

        Protected Overrides Sub Localize()
        End Sub

#End Region

#Region "IEditVocabularyView Members"

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
            addTerm.Enabled = editEnabled
            saveTerm.Enabled = editEnabled
            deleteTerm.Enabled = editEnabled

        End Sub

        Public Sub ClearSelectedTerm() Implements IEditVocabularyView.ClearSelectedTerm
            termsList.ClearSelectedTerm()
            termEditor.Visible = False
        End Sub

        Public Sub SetTermEditorMode(ByVal isAddMode As Boolean, ByVal termId As Integer) Implements IEditVocabularyView.SetTermEditorMode
            If isAddMode Then
                termLabel.Text = Localization.GetString("NewTerm", LocalResourceFile)
                saveTerm.Text = Localization.GetString("CreateTerm", LocalResourceFile)
            Else
                termLabel.Text = Localization.GetString("CurrentTerm", LocalResourceFile)
                saveTerm.Text = Localization.GetString("SaveTerm", LocalResourceFile)
            End If

            deleteTermPlaceHolder.Visible = Not isAddMode
        End Sub

        Public Sub ShowTermEditor(ByVal showEditor As Boolean) Implements IEditVocabularyView.ShowTermEditor
            termEditor.Visible = showEditor
        End Sub

#End Region

    End Class

End Namespace
