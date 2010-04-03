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
Imports DotNetNuke.Modules.Taxonomy.Views.Models
Imports DotNetNuke.Web.UI.WebControls
Imports WebFormsMvp
Imports DotNetNuke.Web.Mvp

Namespace DotNetNuke.Modules.Taxonomy.Views

    Public Interface IEditVocabularyView
        Inherits IModuleView(Of EditVocabularyModel)

        Sub BindVocabulary(ByVal vocabulary As Vocabulary, ByVal editEnabled As Boolean, ByVal deleteEnabled As Boolean, ByVal showScope As Boolean)
        Sub BindTerms(ByVal terms As IEnumerable(Of Term), ByVal isHeirarchical As Boolean, ByVal dataBind As Boolean)
        Sub BindTerm(ByVal term As Term, ByVal terms As IEnumerable(Of Term), ByVal isHeirarchical As Boolean, ByVal loadFromControl As Boolean, ByVal editEnabled As Boolean)
        Sub ClearSelectedTerm()
        Sub SetTermEditorMode(ByVal isAddMode As Boolean, ByVal termId As Integer)
        Sub ShowTermEditor(ByVal showEditor As Boolean)

        Event AddTerm As EventHandler
        Event Cancel As EventHandler
        Event CancelTerm As EventHandler
        Event Delete As EventHandler
        Event DeleteTerm As EventHandler
        Event Save As EventHandler
        Event SaveTerm As EventHandler
        Event SelectTerm As EventHandler(Of TermsEventArgs)

    End Interface

End Namespace
