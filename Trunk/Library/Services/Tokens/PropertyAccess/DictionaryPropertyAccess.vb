'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009 by DotNetNuke Corp. 
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

Imports DotNetNuke
Imports DotNetNuke.Services.Localization
Imports System.Reflection

Namespace DotNetNuke.Services.Tokens
    Public Class DictionaryPropertyAccess
        Implements IPropertyAccess
        Dim NameValueCollection As IDictionary

        Public Sub New(ByVal list As IDictionary)
            NameValueCollection = list
        End Sub

        Public Overridable Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As Entities.Users.UserInfo, ByVal AccessLevel As Scope, ByRef PropertyNotFound As Boolean) As String Implements IPropertyAccess.GetProperty
            If NameValueCollection Is Nothing Then Return String.Empty
            Dim valueObject As Object = NameValueCollection.Item(strPropertyName)
            Dim OutputFormat As String = strFormat
            If String.IsNullOrEmpty(strFormat) Then OutputFormat = "g"
            If Not valueObject Is Nothing Then
                Select Case valueObject.GetType.Name()
                    Case "String"
                        Return PropertyAccess.FormatString(CStr(valueObject), strFormat)
                    Case "Boolean"
                        Return (PropertyAccess.Boolean2LocalizedYesNo(CBool(valueObject), formatProvider))
                    Case "DateTime", "Double", "Single", "Int32", "Int64"
                        Return (CType(valueObject, IFormattable).ToString(OutputFormat, formatProvider))
                    Case Else
                        Return PropertyAccess.FormatString(valueObject.ToString(), strFormat)
                End Select
            Else
                PropertyNotFound = True : Return String.Empty
            End If
        End Function

        Public ReadOnly Property Cacheability() As CacheLevel Implements Services.Tokens.IPropertyAccess.Cacheability
            Get
                Return CacheLevel.notCacheable
            End Get
        End Property
    End Class

End Namespace
