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
Imports DotNetNuke.Modules.Taxonomy.Views
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Web.Mvp.Framework
Imports DotNetNuke.Entities.Content.Data
Imports DotNetNuke.Web.Validators
Imports DotNetNuke.Entities.Users

Namespace DotNetNuke.Modules.Taxonomy.Presenters

    Public Class EditVocabularyPresenter
        Inherits Presenter(Of IEditVocabularyView, EditVocabularyPresenterModel)

#Region "Private Members"

        Private _TermController As ITermController
        Private _Term As Term
        Private _Terms As List(Of Term)
        Private _Vocabulary As Vocabulary
        Private _VocabularyController As IVocabularyController

#End Region

#Region "Public Constants"

        Public Const Name As String = "EditVocabulary"

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New(New VocabularyController(New DataService()), _
                   New TermController(New DataService()))
        End Sub

        Public Sub New(ByVal vocabularyController As IVocabularyController, _
                       ByVal termController As ITermController)
            Arg.NotNull("vocabularyController", vocabularyController)
            Arg.NotNull("termController", termController)

            _VocabularyController = vocabularyController
            _TermController = termController
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property IsEditEnabled() As Boolean
            Get
                Dim user As UserInfo = UserController.GetCurrentUserInfo()
                Return Vocabulary.ScopeType.ScopeType = "Portal" OrElse (user IsNot Nothing AndAlso user.IsSuperUser)
            End Get
        End Property

        Public ReadOnly Property IsHeirarchical() As Boolean
            Get
                Dim _IsHeirarchical As Boolean = Null.NullBoolean
                If Vocabulary IsNot Nothing Then
                    _IsHeirarchical = (Vocabulary.Type = VocabularyType.Hierarchy)
                End If
                Return _IsHeirarchical
            End Get
        End Property

        Public ReadOnly Property TermController() As ITermController
            Get
                Return _TermController
            End Get
        End Property

        <ViewState()> _
        Public Property Term() As Term
            Get
                Return _Term
            End Get
            Set(ByVal value As Term)
                _Term = value
            End Set
        End Property

        <ViewState()> _
        Public Property Terms() As List(Of Term)
            Get
                Return _Terms
            End Get
            Set(ByVal value As List(Of Term))
                _Terms = value
            End Set
        End Property

        <ViewState()> _
        Public Property Vocabulary() As Vocabulary
            Get
                Return _Vocabulary
            End Get
            Set(ByVal value As Vocabulary)
                _Vocabulary = value
            End Set
        End Property

        Public ReadOnly Property VocabularyController() As IVocabularyController
            Get
                Return _VocabularyController
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub RefreshTerms()
            'Refresh Terms
            Terms = TermController.GetTermsByVocabulary(Model.VocabularyId).ToList()
            View.BindTerms(Terms, IsHeirarchical, True)

            'Clear Selected Term
            Term = Nothing
            View.ClearSelectedTerm()

            'Hide Term Editor
            View.ShowTermEditor(False)
        End Sub

#End Region

#Region "Public Methods"

        Public Function AddTerm() As Boolean
            'Set term to be a new term
            Term = New Term(Vocabulary.VocabularyId)

            'Bind Term
            View.BindTerm(Term, Terms, IsHeirarchical, False, IsEditEnabled)

            'Display Term Editor
            View.ShowTermEditor(True)

            'Set Term Editor's mode
            View.SetTermEditorMode(True, Null.NullInteger)
        End Function

        Public Function Cancel() As Boolean
            Environment.RedirectToPresenter(New VocabularyListPresenterModel())
        End Function

        Public Function CancelTerm() As Boolean
            'Clear Selected Term
            Term = Nothing
            View.ClearSelectedTerm()

            'Hide Term Editor
            View.ShowTermEditor(False)
        End Function

        Public Function DeleteTerm() As Boolean
            'Delete Term
            TermController.DeleteTerm(Term)

            'Refresh Terms
            RefreshTerms()
        End Function

        Public Function DeleteVocabulary() As Boolean
            'Delete Vocabulary
            VocabularyController.DeleteVocabulary(Vocabulary)

            'Redirect to List
            Environment.RedirectToPresenter(New VocabularyListPresenterModel())
        End Function

        Public Overrides Function Load() As Boolean
            If Not Model.IsPostBack Then
                'Get Vocabulary
                Vocabulary = VocabularyController.GetVocabularies() _
                                                .Where(Function(v) v.VocabularyId = Model.VocabularyId) _
                                                .SingleOrDefault
            End If

            Dim user As UserInfo = UserController.GetCurrentUserInfo()

            'Bind Vocabulary to View
            View.BindVocabulary(Vocabulary, IsEditEnabled, Me.Model.IsSuperUser)

            'Get Terms for this Vocabulary and bind to terms list
            Terms = TermController.GetTermsByVocabulary(Model.VocabularyId).ToList()
            View.BindTerms(Terms, IsHeirarchical, Not Model.IsPostBack)
        End Function

        Public Function SaveTerm() As Boolean
            'First Bind the term so we can get the current values from the View
            View.BindTerm(Term, Terms, IsHeirarchical, True, IsEditEnabled)

            Dim result As ValidationResult = Validator.ValidateObject(Term)
            If result.IsValid Then
                'Save Term
                If Term.TermId = Null.NullInteger Then
                    'Add
                    TermController.AddTerm(Term)
                Else
                    'Update
                    TermController.UpdateTerm(Term)
                End If

                'Refresh Terms
                RefreshTerms()
            Else
                View.ShowMessage("TermValidationError", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If

        End Function

        Public Function SaveVocabulary() As Boolean
            Dim result As ValidationResult = Validator.ValidateObject(Vocabulary)
            If result.IsValid Then
                'Save Vocabulary
                VocabularyController.UpdateVocabulary(Vocabulary)

                'Redirect to Vocabulary List
                Environment.RedirectToPresenter(New VocabularyListPresenterModel())
            Else
                View.ShowMessage("VocabularyValidationError", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If

        End Function

        Public Function SelectTerm(ByVal term As Term) As Boolean
            Me.Term = term

            'Bind Term
            View.BindTerm(term, Terms, IsHeirarchical, False, IsEditEnabled)

            'Display Term Editor
            View.ShowTermEditor(True)

            'Set Term Editor's mode
            View.SetTermEditorMode(False, term.TermId)
        End Function

#End Region

    End Class

End Namespace
