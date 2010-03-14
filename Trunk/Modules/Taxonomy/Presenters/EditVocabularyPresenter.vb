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
Imports DotNetNuke.Entities.Content.Data
Imports DotNetNuke.Web.Validators
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Web.Mvp
Imports DotNetNuke.Modules.Taxonomy.WebControls
Imports DotNetNuke.Web.UI.WebControls

Namespace DotNetNuke.Modules.Taxonomy.Presenters

    Public Class EditVocabularyPresenter
        Inherits ModulePresenter(Of IEditVocabularyView)

#Region "Private Members"

        Private _TermController As ITermController
        Private _VocabularyController As IVocabularyController

#End Region

#Region "Constructors"

        Public Sub New(ByVal editView As IEditVocabularyView)
            Me.New(editView, _
                   New VocabularyController(New DataService()), _
                   New TermController(New DataService()))
        End Sub

        Public Sub New(ByVal editView As IEditVocabularyView, _
                       ByVal vocabularyController As IVocabularyController, _
                       ByVal termController As ITermController)
            MyBase.New(editView)
            Arg.NotNull("vocabularyController", vocabularyController)
            Arg.NotNull("termController", termController)

            _VocabularyController = vocabularyController
            _TermController = termController

            AddHandler View.AddTerm, AddressOf AddTerm
            AddHandler View.Cancel, AddressOf Cancel
            AddHandler View.CancelTerm, AddressOf CancelTerm
            AddHandler View.Delete, AddressOf DeleteVocabulary
            AddHandler View.DeleteTerm, AddressOf DeleteTerm
            AddHandler View.Load, AddressOf Load
            AddHandler View.Save, AddressOf SaveVocabulary
            AddHandler View.SaveTerm, AddressOf SaveTerm
            AddHandler View.SelectTerm, AddressOf SelectTerm

        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property IsEditEnabled() As Boolean
            Get
                Dim user As UserInfo = UserController.GetCurrentUserInfo()
                Return String.Compare(View.Model.Vocabulary.ScopeType.ScopeType, "Portal", False) = 0 OrElse (user IsNot Nothing AndAlso user.IsSuperUser)
            End Get
        End Property

        Public ReadOnly Property IsHeirarchical() As Boolean
            Get
                Dim _IsHeirarchical As Boolean = Null.NullBoolean
                If View.Model.Vocabulary IsNot Nothing Then
                    _IsHeirarchical = (View.Model.Vocabulary.Type = VocabularyType.Hierarchy)
                End If
                Return _IsHeirarchical
            End Get
        End Property

        Public ReadOnly Property TermController() As ITermController
            Get
                Return _TermController
            End Get
        End Property

        Public ReadOnly Property VocabularyController() As IVocabularyController
            Get
                Return _VocabularyController
            End Get
        End Property

        Public ReadOnly Property VocabularyId() As Integer
            Get
                Dim _VocabularyId As Integer = Null.NullInteger
                If Not String.IsNullOrEmpty(Request.Params("VocabularyId")) Then
                    _VocabularyId = Int32.Parse(Request.Params("VocabularyId"))
                End If
                Return _VocabularyId
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub RefreshTerms()
            'Refresh Terms
            View.Model.Terms = TermController.GetTermsByVocabulary(VocabularyId).ToList()
            View.BindTerms(View.Model.Terms, IsHeirarchical, True)

            'Clear Selected Term
            View.Model.Term = Nothing
            View.ClearSelectedTerm()

            'Hide Term Editor
            View.ShowTermEditor(False)
        End Sub

#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnInit()
            MyBase.OnInit()

            If View.Model.Vocabulary Is Nothing Then
                View.Model.Vocabulary = VocabularyController.GetVocabularies() _
                                        .Where(Function(v) v.VocabularyId = VocabularyId) _
                                        .SingleOrDefault
                View.Model.Terms = TermController.GetTermsByVocabulary(VocabularyId).ToList()
            End If
        End Sub

#End Region

#Region "Public Methods"

        Public Sub AddTerm(ByVal sender As Object, ByVal e As EventArgs)
            'Set term to be a new term
            View.Model.Term = New Term(View.Model.Vocabulary.VocabularyId)

            'Bind Term
            View.BindTerm(View.Model.Term, View.Model.Terms, IsHeirarchical, False, IsEditEnabled)

            'Display Term Editor
            View.ShowTermEditor(True)

            'Set Term Editor's mode
            View.SetTermEditorMode(True, Null.NullInteger)
        End Sub

        Public Sub Cancel(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect(NavigateURL(TabId))
        End Sub

        Public Sub CancelTerm(ByVal sender As Object, ByVal e As EventArgs)
            'Clear Selected Term
            View.Model.Term = Nothing
            View.ClearSelectedTerm()

            'Hide Term Editor
            View.ShowTermEditor(False)
        End Sub

        Public Sub DeleteTerm(ByVal sender As Object, ByVal e As EventArgs)
            'Delete Term
            TermController.DeleteTerm(View.Model.Term)

            'Refresh Terms
            RefreshTerms()
        End Sub

        Public Sub DeleteVocabulary(ByVal sender As Object, ByVal e As EventArgs)
            'Delete Vocabulary
            VocabularyController.DeleteVocabulary(View.Model.Vocabulary)

            'Redirect to List
            Response.Redirect(NavigateURL(TabId))
        End Sub

        Public Sub Load(ByVal sender As Object, ByVal e As EventArgs)
            Dim user As UserInfo = UserController.GetCurrentUserInfo()

            'Bind Vocabulary to View
            View.BindVocabulary(View.Model.Vocabulary, IsEditEnabled, IsSuperUser)

            'Bind Terms to View
            View.BindTerms(View.Model.Terms, IsHeirarchical, Not IsPostBack)
        End Sub

        Public Sub SaveTerm(ByVal sender As Object, ByVal e As EventArgs)
            'First Bind the term so we can get the current values from the View
            View.BindTerm(View.Model.Term, View.Model.Terms, IsHeirarchical, True, IsEditEnabled)

            Dim result As ValidationResult = Validator.ValidateObject(View.Model.Term)
            If result.IsValid Then
                'Save Term
                If View.Model.Term.TermId = Null.NullInteger Then
                    'Add
                    TermController.AddTerm(View.Model.Term)
                Else
                    'Update
                    TermController.UpdateTerm(View.Model.Term)
                End If

                'Refresh Terms
                RefreshTerms()
            Else
                'View.ShowMessage("TermValidationError", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If

        End Sub

        Public Sub SaveVocabulary(ByVal sender As Object, ByVal e As EventArgs)
            'Bind Vocabulary to View
            View.BindVocabulary(View.Model.Vocabulary, IsEditEnabled, IsSuperUser)

            Dim result As ValidationResult = Validator.ValidateObject(View.Model.Vocabulary)
            If result.IsValid Then
                'Save Vocabulary
                VocabularyController.UpdateVocabulary(View.Model.Vocabulary)

                'Redirect to Vocabulary List
                Response.Redirect(NavigateURL(TabId))
            Else
                'View.ShowMessage("VocabularyValidationError", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If

        End Sub

        Public Sub SelectTerm(ByVal sender As Object, ByVal e As TermsEventArgs)
            View.Model.Term = e.SelectedTerm

            'Bind Term
            View.BindTerm(View.Model.Term, View.Model.Terms, IsHeirarchical, False, IsEditEnabled)

            'Display Term Editor
            View.ShowTermEditor(True)

            'Set Term Editor's mode
            View.SetTermEditorMode(False, View.Model.Term.TermId)
        End Sub

#End Region

    End Class

End Namespace
