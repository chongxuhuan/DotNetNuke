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

Namespace DotNetNuke.Services.Localization

    Public Interface ILocaleController

        Function CanDeleteLanguage(ByVal languageId As Integer) As Boolean
        Function GetCultures(ByVal locales As Dictionary(Of String, Locale)) As List(Of CultureInfo)

        Function GetCurrentLocale(ByVal PortalId As Integer) As Locale
        Function GetDefaultLocale(ByVal portalId As Integer) As Locale
        Function GetLocale(ByVal code As String) As Locale
        Function GetLocale(ByVal portalID As Integer, ByVal code As String) As Locale
        Function GetLocale(ByVal languageID As Integer) As Locale
        Function GetLocales(ByVal portalID As Integer) As Dictionary(Of String, Locale)
        Function GetPublishedLocales(ByVal portalID As Integer) As Dictionary(Of String, Locale)

        Function IsEnabled(ByVal localeCode As String, ByVal portalId As Integer) As Boolean

        Sub UpdatePortalLocale(ByVal locale As Locale)
        Function IsDefaultLanguage(ByVal code As String) As Boolean
        Sub PublishLanguage(ByVal portalid As Integer, ByVal cultureCode As String, ByVal publish As Boolean)
       

    End Interface

End Namespace
