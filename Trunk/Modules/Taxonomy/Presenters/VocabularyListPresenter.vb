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
Imports DotNetNuke.Modules.Taxonomy.Views
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Web.Mvp
Imports DotNetNuke.Entities.Content.Data
Imports DotNetNuke.Modules.Taxonomy.Views.Models

Namespace DotNetNuke.Modules.Taxonomy.Presenters

    Public Class VocabularyListPresenter
        Inherits ModulePresenter(Of IVocabularyListView, VocabularyListModel)

#Region "Private Members"

        Private _VocabularyController As IVocabularyController

#End Region

#Region "Constructors"

        Public Sub New(ByVal view As IVocabularyListView)
            Me.New(view, New VocabularyController(New DataService()))
        End Sub

        Public Sub New(ByVal listView As IVocabularyListView, ByVal vocabularyController As IVocabularyController)
            MyBase.New(listView)
            Arg.NotNull("vocabularyController", vocabularyController)

            _VocabularyController = vocabularyController

            AddHandler View.AddVocabulary, AddressOf AddVocabulary

        End Sub

#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnInit()
            MyBase.OnInit()

            View.Model.Vocabularies = (From v In _VocabularyController.GetVocabularies() _
                                      Where v.ScopeType.ScopeType = "Application" _
                                      OrElse v.ScopeType.ScopeType = "Portal" AndAlso v.ScopeId = PortalId _
                                      Select v) _
                                      .ToList()

            View.Model.IsEditable = IsEditable
            View.Model.NavigateUrlFormatString = NavigateURL(TabId, _
                                                                "EditVocabulary", _
                                                                String.Format("mid={0}", ModuleId), _
                                                                "VocabularyId={0}")
        End Sub

        Protected Overrides Sub OnLoad()
            MyBase.OnLoad()

            View.ShowAddButton(IsEditable)
        End Sub

#End Region

#Region "Public Methods"

        Public Sub AddVocabulary(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect(NavigateURL(TabId, _
                                          "CreateVocabulary", _
                                          String.Format("mid={0}", ModuleId)))
        End Sub

#End Region

    End Class

End Namespace
