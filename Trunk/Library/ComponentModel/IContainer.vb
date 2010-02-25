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
Imports System.Collections.Generic

Namespace DotNetNuke.ComponentModel

    Public Interface IContainer

        Sub RegisterComponent(Of TComponent As Class)()
        Sub RegisterComponent(Of TComponent As Class)(ByVal name As String)
        Sub RegisterComponent(Of TComponent As Class)(ByVal name As String, ByVal lifestyle As ComponentLifeStyleType)

        Sub RegisterComponent(Of TContract, TComponent As Class)()
        Sub RegisterComponent(Of TContract, TComponent As Class)(ByVal name As String)
        Sub RegisterComponent(Of TContract, TComponent As Class)(ByVal name As String, ByVal lifestyle As ComponentLifeStyleType)

        Sub RegisterComponent(ByVal componentType As Type)
        Sub RegisterComponent(ByVal contractType As Type, ByVal componentType As Type)
        Sub RegisterComponent(ByVal contractType As Type, ByVal componentType As Type, ByVal lifestyle As ComponentLifeStyleType)

        Sub RegisterComponent(ByVal name As String, ByVal componentType As Type)
        Sub RegisterComponent(ByVal name As String, ByVal contractType As Type, ByVal componentType As Type)
        Sub RegisterComponent(ByVal name As String, ByVal contractType As Type, ByVal componentType As Type, ByVal lifestyle As ComponentLifeStyleType)

        Sub RegisterComponentInstance(ByVal name As String, ByVal instance As Object)
        Sub RegisterComponentInstance(ByVal name As String, ByVal contractType As Type, ByVal instance As Object)

        Sub RegisterComponentInstance(Of TContract)(ByVal instance As Object)
        Sub RegisterComponentInstance(Of TContract)(ByVal name As String, ByVal instance As Object)

        Sub RegisterComponentSettings(ByVal name As String, ByVal dependencies As IDictionary)
        Sub RegisterComponentSettings(ByVal component As Type, ByVal dependencies As IDictionary)
        Sub RegisterComponentSettings(Of TComponent)(ByVal dependencies As IDictionary)

        Function GetComponent(ByVal name As String) As Object

        Function GetComponent(Of TContract)() As TContract
        Function GetComponent(ByVal contractType As Type) As Object

        Function GetComponent(Of TContract)(ByVal name As String) As TContract
        Function GetComponent(ByVal name As String, ByVal contractType As Type) As Object

        Function GetComponentList(Of TContract)() As String()
        Function GetComponentList(ByVal contractType As Type) As String()

        Function GetComponentSettings(ByVal name As String) As IDictionary
        Function GetComponentSettings(ByVal component As System.Type) As IDictionary
        Function GetComponentSettings(Of TComponent)() As IDictionary

        ReadOnly Property Name() As String

    End Interface

End Namespace
