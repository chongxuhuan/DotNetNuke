'
' DotNetNuke® - http://www.dotnetnuke.com
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

' The NavDataSourceView class encapsulates the
' capabilities of the NavDataSource data source control.

Namespace DotNetNuke.UI.WebControls

    Public Class NavDataSourceView
        Inherits HierarchicalDataSourceView
        Private m_sKey As String
        Private m_sNamespace As String = "MyNS"
        Public Property [Namespace]() As String
            Get
                Return m_sNamespace
            End Get
            Set(ByVal value As String)
                m_sNamespace = value
            End Set
        End Property

        Public Sub New(ByVal viewPath As String)
            If Len(viewPath) = 0 Then
                m_sKey = ""
            ElseIf viewPath.IndexOf("\") > -1 Then
                m_sKey = viewPath.Substring(viewPath.LastIndexOf("\") + 1) ', viewPath.Length - viewPath.LastIndexOf("\") - 1)
            Else
                m_sKey = viewPath
            End If
        End Sub 'New


        ''' <summary>
        ''' Starting with the rootNode, recursively build a list of
        ''' PageInfo nodes, create PageHierarchyData
        ''' objects, add them all to the PageHierarchicalEnumerable,
        ''' and return the list.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function [Select]() As IHierarchicalEnumerable
            Dim objPages As New NavDataPageHierarchicalEnumerable()

            Dim objNodes As DNNNodeCollection
            Dim objNode As DNNNode
            objNodes = DotNetNuke.UI.Navigation.GetNavigationNodes(m_sNamespace)
            If Len(m_sKey) > 0 Then
                objNodes = objNodes.FindNodeByKey(m_sKey).DNNNodes
            End If
            For Each objNode In objNodes
                objPages.Add(New NavDataPageHierarchyData(objNode))
            Next
            Return objPages

        End Function 'Select
    End Class 'NavDataSourceView

End Namespace
