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
Imports Telerik.Web.UI
Imports WebFormsMvp
Imports DotNetNuke.Modules.Taxonomy.Views.Models

Namespace DotNetNuke.Modules.Taxonomy.Presenters

    Public Class VocabularyListPresenter
        Inherits ModulePresenter(Of IVocabularyListView)

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
            AddHandler View.VocabularyDataBound, AddressOf VocabularyDataBound
            View.Model.Vocabularies = _VocabularyController.GetVocabularies().ToList()
        End Sub

#End Region

#Region "Public Methods"

        Public Sub AddVocabulary(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect(NavigateURL(ModuleContext.TabId, _
                                          "CreateVocabulary", _
                                          String.Format("mid={0}", ModuleContext.ModuleId)))
        End Sub

        Public Function VocabularyDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) As Boolean
            Dim item As GridItem = e.Item

            If item.ItemType = GridItemType.Item Or _
                    item.ItemType = GridItemType.AlternatingItem Or _
                    item.ItemType = GridItemType.SelectedItem Then

                Dim vocabulary As Vocabulary = TryCast(item.DataItem, Vocabulary)

                Dim hyperLinkColumn As HyperLink = TryCast(item.Controls(2).Controls(0), HyperLink)

                If hyperLinkColumn IsNot Nothing Then
                    hyperLinkColumn.NavigateUrl = NavigateURL(ModuleContext.TabId, _
                                                                "EditVocabulary", _
                                                                String.Format("mid={0}", ModuleContext.ModuleId), _
                                                                String.Format("VocabularyId={0}", vocabulary.VocabularyId))
                End If

            End If

        End Function

#End Region

    End Class

End Namespace
