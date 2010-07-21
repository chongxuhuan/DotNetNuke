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
Imports System.IO
Imports System.Web.Caching
Imports System.Threading
Imports System.Resources
Imports System.Collections.Specialized
Imports System.Diagnostics
Imports System.Xml
Imports System.Xml.Serialization

Namespace DotNetNuke.Services.Localization

     Public Class CultureInfoComparer
        Implements IComparer

        Private _compare As String
        Public Sub New(ByVal compareBy As String)
            _compare = compareBy
        End Sub

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            Select Case _compare.ToUpperInvariant
                Case "ENGLISH"
                    Return (CType(x, CultureInfo)).EnglishName.CompareTo((CType(y, CultureInfo)).EnglishName)
                Case "NATIVE"
                    Return (CType(x, CultureInfo)).NativeName.CompareTo((CType(y, CultureInfo)).NativeName)
                Case Else
                    Return (CType(x, CultureInfo)).Name.CompareTo((CType(y, CultureInfo)).Name)
            End Select
        End Function

    End Class

End Namespace
