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
Imports DotNetNuke.Entities.Content.Data
Imports DotNetNuke.Web.Validators
Imports DotNetNuke.Web.Mvp

Namespace DotNetNuke.Modules.Taxonomy.Presenters

    Public Class CreateVocabularyPresenter
        Inherits ModulePresenter(Of ICreateVocabularyView)

#Region "Private Members"

        Private _VocabularyController As IVocabularyController
        Private _ScopeTypeController As IScopeTypeController

#End Region

#Region "Constructors"

        Public Sub New(ByVal createView As ICreateVocabularyView)
            Me.New(createView, New VocabularyController(New DataService()), New ScopeTypeController(New DataService()))
        End Sub

        Public Sub New(ByVal createView As ICreateVocabularyView, ByVal vocabularyController As IVocabularyController, ByVal scopeTypeController As IScopeTypeController)
            MyBase.New(createView)
            Arg.NotNull("vocabularyController", vocabularyController)
            Arg.NotNull("scopeTypeController", scopeTypeController)

            _VocabularyController = vocabularyController
            _ScopeTypeController = scopeTypeController

            AddHandler View.Cancel, AddressOf Cancel
            AddHandler View.Load, AddressOf Load
            AddHandler View.Save, AddressOf Save
            View.Model.Vocabulary = GetVocabulary()
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property VocabularyController() As IVocabularyController
            Get
                Return _VocabularyController
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Function GetVocabulary() As Vocabulary
            Dim vocabulary As New Vocabulary()
            Dim scopeType As ScopeType
            If IsSuperUser Then
                scopeType = _ScopeTypeController.GetScopeTypes() _
                                                    .Where(Function(s) s.ScopeType = "Application") _
                                                    .SingleOrDefault
            Else
                scopeType = _ScopeTypeController.GetScopeTypes() _
                                                    .Where(Function(s) s.ScopeType = "Portal") _
                                                    .SingleOrDefault
            End If

            If scopeType IsNot Nothing Then
                vocabulary.ScopeTypeId = scopeType.ScopeTypeId
            End If
            vocabulary.Type = VocabularyType.Simple

            Return vocabulary
        End Function

#End Region

#Region "Public Methods"

        Public Sub Cancel(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect(NavigateURL(TabId))
        End Sub

        Public Sub Load(ByVal sender As Object, ByVal e As EventArgs)
            View.BindVocabulary(View.Model.Vocabulary, IsSuperUser)
        End Sub

        Public Sub Save(ByVal sender As Object, ByVal e As EventArgs)
            'Bind Model
            View.BindVocabulary(View.Model.Vocabulary, IsSuperUser)
            If View.Model.Vocabulary.ScopeType.ScopeType = "Portal" Then
                View.Model.Vocabulary.ScopeId = PortalId
            End If

            'Validate Model
            Dim result As ValidationResult = Validator.ValidateObject(View.Model.Vocabulary)

            If result.IsValid Then
                'Save Vocabulary
                VocabularyController.AddVocabulary(View.Model.Vocabulary)

                'Redirect to Vocabulary List
                Response.Redirect(NavigateURL(TabId))
            Else
                ShowMessage("Validation.Error", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If
        End Sub

#End Region

    End Class

End Namespace
