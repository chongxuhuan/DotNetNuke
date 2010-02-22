'
' DotNetNuke - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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

Imports DotNetNuke.Common.Utilities
Imports System.Collections.Specialized
Imports System.Globalization
Imports System.Runtime.CompilerServices

Namespace DotNetNuke.Web.Mvp.Framework

    Public Module NameValueCollectionExtensions

        <Extension()> _
        Public Sub AddIfNotNull(Of T)(ByVal collection As NameValueCollection, ByVal key As String, ByVal value As T)
            If (Not Null.IsNull(value)) Then
                collection(key) = value.ToString()
            End If
        End Sub

        <Extension()> _
        Public Function ToInt32(ByVal collection As NameValueCollection, ByVal key As String) As Integer
            Return ToInt32(collection, key, Null.NullInteger)
        End Function

        <Extension()> _
        Public Function ToInt32(ByVal collection As NameValueCollection, ByVal key As String, ByVal defaultValue As Integer) As Integer
            Dim value As String = collection(key)
            Return If(String.IsNullOrEmpty(value), defaultValue, Int32.Parse(value, CultureInfo.InvariantCulture))
        End Function

    End Module

End Namespace
