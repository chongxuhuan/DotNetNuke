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

Imports DotNetNuke.ComponentModel
Imports DotNetNuke.Entities.Content.Data
Imports DotNetNuke.Entities.Content.Taxonomy

Namespace DotNetNuke.Entities.Content.Common

    Public Module Util

        Public Function GetDataService() As IDataService
            Dim ds As IDataService = ComponentFactory.GetComponent(Of IDataService)()

            If ds Is Nothing Then
                ds = New DataService()
                ComponentFactory.RegisterComponentInstance(Of IDataService)(ds)
            End If
            Return ds
        End Function

        Public Function GetContentController() As IContentController
            Dim ctl As IContentController = ComponentFactory.GetComponent(Of IContentController)()

            If ctl Is Nothing Then
                ctl = New ContentController()
                ComponentFactory.RegisterComponentInstance(Of IContentController)(ctl)
            End If
            Return ctl
        End Function

        Public Function GetScopeTypeController() As IScopeTypeController
            Dim ctl As IScopeTypeController = ComponentFactory.GetComponent(Of IScopeTypeController)()

            If ctl Is Nothing Then
                ctl = New ScopeTypeController()
                ComponentFactory.RegisterComponentInstance(Of IScopeTypeController)(ctl)
            End If
            Return ctl

        End Function

        Public Function GetTermController() As ITermController
            Dim ctl As ITermController = ComponentFactory.GetComponent(Of ITermController)()

            If ctl Is Nothing Then
                ctl = New TermController()
                ComponentFactory.RegisterComponentInstance(Of ITermController)(ctl)
            End If
            Return ctl

        End Function

        Public Function GetVocabularyController() As IVocabularyController
            Dim ctl As IVocabularyController = ComponentFactory.GetComponent(Of IVocabularyController)()

            If ctl Is Nothing Then
                ctl = New VocabularyController()
                ComponentFactory.RegisterComponentInstance(Of IVocabularyController)(ctl)
            End If
            Return ctl

        End Function

    End Module

End Namespace


