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
Imports DotNetNuke.Web.Mvp.Framework
Imports DotNetNuke.Entities.Content.Data

Namespace DotNetNuke.Modules.Taxonomy.Presenters

    Public Class VocabularyListPresenter
        Inherits Presenter(Of IVocabularyListView, VocabularyListPresenterModel)

#Region "Private Members"

        Private _Vocabularies As List(Of Vocabulary)
        Private _VocabularyController As IVocabularyController

#End Region

#Region "Public Constants"

        Public Const Name As String = ""

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New(New VocabularyController(New DataService()))
        End Sub

        Public Sub New(ByVal vocabularyController As IVocabularyController)
            Arg.NotNull("vocabularyController", vocabularyController)

            _VocabularyController = vocabularyController
        End Sub

#End Region

#Region "Public Properties"

        Public Property Vocabularies() As List(Of Vocabulary)
            Get
                Return _Vocabularies
            End Get
            Set(ByVal value As List(Of Vocabulary))
                _Vocabularies = value
            End Set
        End Property

#End Region

#Region "Public Methods"

        Public Sub AddVocabulary(ByVal sender As Object, ByVal e As EventArgs)
            Environment.RedirectToPresenter(New CreateVocabularyPresenterModel())
        End Sub

        Public Overrides Function Load() As Boolean
            Vocabularies = _VocabularyController.GetVocabularies().ToList()
            View.ShowVocabularies(Vocabularies)
        End Function

        Public Function VocabularyDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs) As Boolean
            Dim item As DataGridItem = e.Item

            If item.ItemType = ListItemType.Item Or _
                    item.ItemType = ListItemType.AlternatingItem Or _
                    item.ItemType = ListItemType.SelectedItem Then

                Dim vocabulary As Vocabulary = TryCast(item.DataItem, Vocabulary)

                Dim hyperLinkColumn As HyperLink = TryCast(item.Controls(0).Controls(0), HyperLink)

                If hyperLinkColumn IsNot Nothing Then
                    hyperLinkColumn.NavigateUrl = NavigateURL(Me.Model.TabId, _
                                                                "EditVocabulary", _
                                                                "mid=" + Me.Model.ModuleId.ToString(), _
                                                                "VocabularyId=" + vocabulary.VocabularyId.ToString())
                    hyperLinkColumn.Text = "Edit"
                End If

            End If

        End Function

#End Region

    End Class

End Namespace
