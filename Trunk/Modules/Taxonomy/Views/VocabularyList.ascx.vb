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

Imports DotNetNuke.Web.Mvp
Imports DotNetNuke.Modules.Taxonomy.Views.Models
Imports WebFormsMvp
Imports DotNetNuke.Modules.Taxonomy.Presenters
Imports Telerik.Web.UI
Imports DotNetNuke.Web.UI.WebControls

Namespace DotNetNuke.Modules.Taxonomy.Views

    <PresenterBinding(GetType(VocabularyListPresenter))> _
    Partial Public Class VocabularyList
        Inherits ModuleView(Of VocabularyListModel)
        Implements IVocabularyListView

#Region "IVocabularyListView Implementation"

        Public Event AddVocabulary(ByVal sender As Object, ByVal e As EventArgs) Implements IVocabularyListView.AddVocabulary

        Public Sub ShowAddButton(ByVal showButton As Boolean) Implements IVocabularyListView.ShowAddButton
            addVocabularyButton.Visible = showButton
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub addVocabularyButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles addVocabularyButton.Click
            RaiseEvent AddVocabulary(Me, e)
        End Sub

        Private Sub vocabulariesGrid_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles vocabulariesGrid.PreRender
            Dim hyperlinkColumn As DnnGridHyperlinkColumn = TryCast(vocabulariesGrid.Columns(0), DnnGridHyperlinkColumn)
            If hyperlinkColumn IsNot Nothing Then
                hyperlinkColumn.Visible = Model.IsEditable
                hyperlinkColumn.DataNavigateUrlFormatString = Model.NavigateUrlFormatString
            End If

        End Sub

#End Region

    End Class

End Namespace
