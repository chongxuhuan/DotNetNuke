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
    Public Class CulturePropertyAccess
        Implements IPropertyAccess

        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As Entities.Users.UserInfo, ByVal AccessLevel As Scope, ByRef PropertyNotFound As Boolean) As String Implements IPropertyAccess.GetProperty
            Dim ci As CultureInfo = formatProvider
            Select Case strPropertyName.ToLower
                Case CultureDropDownTypes.EnglishName.ToString.ToLowerInvariant
                    Return PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.EnglishName), strFormat)
                Case CultureDropDownTypes.Lcid.ToString.ToLowerInvariant
                    Return ci.LCID.ToString()
                Case CultureDropDownTypes.Name.ToString.ToLowerInvariant
                    Return PropertyAccess.FormatString(ci.Name, strFormat)
                Case CultureDropDownTypes.NativeName.ToString.ToLowerInvariant
                    Return PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.NativeName), strFormat)
                Case CultureDropDownTypes.TwoLetterIsoCode.ToString.ToLowerInvariant
                    Return PropertyAccess.FormatString(ci.TwoLetterISOLanguageName, strFormat)
                Case CultureDropDownTypes.ThreeLetterIsoCode.ToString.ToLowerInvariant
                    Return PropertyAccess.FormatString(ci.ThreeLetterISOLanguageName, strFormat)
                Case CultureDropDownTypes.DisplayName.ToString.ToLowerInvariant
                    Return PropertyAccess.FormatString(ci.DisplayName, strFormat)
            End Select
            PropertyNotFound = True : Return String.Empty
        End Function

        Public ReadOnly Property Cacheability() As CacheLevel Implements Services.Tokens.IPropertyAccess.Cacheability
            Get
                Return CacheLevel.fullyCacheable
            End Get
        End Property
    End Class
End Namespace
