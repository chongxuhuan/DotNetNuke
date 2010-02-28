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

Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Modules.Taxonomy.Presenters
Imports DotNetNuke.Web.Mvp.Framework

Namespace DotNetNuke.Modules.Taxonomy.Views

    Partial Public Class CreateVocabulary
        Inherits ViewBase(Of ICreateVocabularyView, CreateVocabularyPresenter, CreateVocabularyPresenterModel)
        Implements ICreateVocabularyView

#Region "Protected Properties"

        Protected Overrides ReadOnly Property View() As ICreateVocabularyView
            Get
                Return Me
            End Get
        End Property

#End Region

#Region "ViewBase Virtual/Abstract Method Overrides"

        Protected Overrides Sub ConnectEvents()
            MyBase.ConnectEvents()

            AddHandler cancelCreate.Click, CreateSimpleHandler(Function(p) p.Cancel())
            AddHandler saveVocabulary.Click, CreateSaveHandler(Function(p) p.SaveVocabulary())
        End Sub

        Protected Overrides Sub Localize()
        End Sub

#End Region

#Region "ICreateVocabularyView Members"

        Public Sub BindVocabulary(ByVal vocabulary As Vocabulary, ByVal showScope As Boolean) Implements ICreateVocabularyView.BindVocabulary
            editVocabularyControl.BindVocabulary(vocabulary, True, showScope)
        End Sub

#End Region

    End Class

End Namespace
