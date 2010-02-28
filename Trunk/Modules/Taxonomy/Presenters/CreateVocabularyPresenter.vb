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

Imports System.Linq

Imports DotNetNuke.Common
Imports DotNetNuke.Modules.Taxonomy.Views
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Web.Mvp.Framework
Imports DotNetNuke.Entities.Content.Data
Imports DotNetNuke.Web.Validators

Namespace DotNetNuke.Modules.Taxonomy.Presenters

    Public Class CreateVocabularyPresenter
        Inherits Presenter(Of ICreateVocabularyView, CreateVocabularyPresenterModel)

#Region "Private Members"

        Private _Vocabulary As Vocabulary
        Private _VocabularyController As IVocabularyController
        Private _ScopeTypeController As IScopeTypeController

#End Region

#Region "Public Constants"

        Public Const Name As String = "CreateVocabulary"

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New(New VocabularyController(New DataService()), New ScopeTypeController(New DataService()))
        End Sub

        Public Sub New(ByVal vocabularyController As IVocabularyController, ByVal scopeTypeController As IScopeTypeController)
            Arg.NotNull("vocabularyController", vocabularyController)
            Arg.NotNull("scopeTypeController", scopeTypeController)

            _VocabularyController = vocabularyController
            _ScopeTypeController = scopeTypeController
        End Sub

#End Region

#Region "Public Properties"

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

#Region "Public Methods"

        Public Function Cancel() As Boolean
            Environment.RedirectToPresenter(New VocabularyListPresenterModel())
        End Function

        Public Overrides Function Load() As Boolean
            If Not Model.IsPostBack Then
                Vocabulary = New Vocabulary()
                Dim scopeType As ScopeType
                If Me.Model.IsSuperUser Then
                    scopeType = _ScopeTypeController.GetScopeTypes() _
                                                        .Where(Function(s) s.ScopeType = "Application") _
                                                        .SingleOrDefault
                Else
                    scopeType = _ScopeTypeController.GetScopeTypes() _
                                                        .Where(Function(s) s.ScopeType = "Portal") _
                                                        .SingleOrDefault
                End If

                If scopeType IsNot Nothing Then
                    Vocabulary.ScopeTypeId = scopeType.ScopeTypeId
                End If
                Vocabulary.Type = VocabularyType.Simple
            End If

            View.BindVocabulary(Vocabulary, Me.Model.IsSuperUser)
        End Function

        Public Function SaveVocabulary() As Boolean
            Validator.Validators.Add(New DataAnnotationsObjectValidator())
            Dim result As ValidationResult = Validator.ValidateObject(Vocabulary)
            If result.IsValid Then
                'Save Vocabulary
                VocabularyController.AddVocabulary(Vocabulary)

                'Redirect to Vocabulary List
                Environment.RedirectToPresenter(New VocabularyListPresenterModel())
            Else
                View.ShowMessage("Validation.Error", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If

        End Function

#End Region

    End Class

End Namespace
