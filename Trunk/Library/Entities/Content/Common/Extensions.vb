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
Imports System.Runtime.CompilerServices

Imports DotNetNuke.Entities.Content.Taxonomy
Imports System.Text

Namespace DotNetNuke.Entities.Content.Common

    Public Module Extensions

#Region "Term Extensions"

        <Extension()> _
        Friend Function GetChildTerms(ByVal this As Term, ByVal termId As Integer, ByVal vocabularyId As Integer) As List(Of Term)
            Dim ctl As ITermController = Util.GetTermController()

            Dim terms As IQueryable(Of Term) = From term In ctl.GetTermsByVocabulary(vocabularyId) _
                                               Where term.ParentTermId = termId

            Return terms.ToList()
        End Function

        <Extension()> _
        Friend Function GetVocabulary(ByVal this As Term, ByVal vocabularyId As Integer) As Vocabulary
            Dim ctl As IVocabularyController = Util.GetVocabularyController()

            Return (From v In ctl.GetVocabularies() _
                    Where v.VocabularyId = vocabularyId _
                    Select v) _
                    .SingleOrDefault
        End Function

        <Extension()> _
        Public Function ToDelimittedString(ByVal this As List(Of Term), ByVal delimitter As String) As String
            Dim sb As New StringBuilder
            If this IsNot Nothing Then
                For Each _Term As Term In (From term In this Order By term.Name Ascending Select term)
                    If sb.Length > 0 Then
                        sb.Append(delimitter)
                    End If
                    sb.Append(_Term.Name)
                Next
            End If
            Return sb.ToString()
        End Function

#End Region

#Region "Vocabulary Extensions"

        <Extension()> _
        Friend Function GetScopeType(ByVal this As Vocabulary, ByVal scopeTypeId As Integer) As ScopeType
            Dim ctl As IScopeTypeController = Util.GetScopeTypeController()

            Return ctl.GetScopeTypes() _
                        .Where(Function(s) s.ScopeTypeId = scopeTypeId) _
                        .SingleOrDefault
        End Function

        <Extension()> _
        Friend Function GetTerms(ByVal this As Vocabulary, ByVal vocabularyId As Integer) As List(Of Term)
            Dim ctl As ITermController = Util.GetTermController()

            Return ctl.GetTermsByVocabulary(vocabularyId).ToList()
        End Function

#End Region

        <Extension()> _
        Friend Function GetTerms(ByVal this As ContentItem, ByVal contentItemId As Integer) As List(Of Term)
            Dim ctl As ITermController = Util.GetTermController()

            Dim _Terms As List(Of Term)
            If contentItemId = Null.NullInteger Then
                _Terms = New List(Of Term)
            Else
                _Terms = ctl.GetTermsByContent(contentItemId).ToList()
            End If

            Return _Terms
        End Function

    End Module

End Namespace
