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

Imports System.ComponentModel
Imports System.Reflection

Namespace DotNetNuke.UI.WebControls
    Public Class PropertyCategoryComparer
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            If TypeOf x Is PropertyInfo AndAlso TypeOf y Is PropertyInfo Then
                Dim xProp As PropertyInfo = CType(x, PropertyInfo)
                Dim yProp As PropertyInfo = CType(y, PropertyInfo)

                Dim xCategory As Object() = xProp.GetCustomAttributes(GetType(CategoryAttribute), True)
                Dim xCategoryName As String = String.Empty
                If xCategory.Length > 0 Then
                    xCategoryName = DirectCast(xCategory(0), CategoryAttribute).Category
                Else
                    xCategoryName = CategoryAttribute.Default.Category
                End If

                Dim yCategory As Object() = yProp.GetCustomAttributes(GetType(CategoryAttribute), True)
                Dim yCategoryName As String = String.Empty
                If yCategory.Length > 0 Then
                    yCategoryName = DirectCast(yCategory(0), CategoryAttribute).Category
                Else
                    yCategoryName = CategoryAttribute.Default.Category
                End If

                If xCategoryName = yCategoryName Then
                    Return String.Compare(xProp.Name, yProp.Name)
                Else
                    Return String.Compare(xCategoryName, yCategoryName)
                End If
            Else
                Throw New ArgumentException("Object is not of type PropertyInfo")
            End If
        End Function

    End Class
End Namespace
