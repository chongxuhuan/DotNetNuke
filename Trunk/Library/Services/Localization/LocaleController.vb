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

Imports DotNetNuke.ComponentModel
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Services.Installer.Packages

Namespace DotNetNuke.Services.Localization

    Public Class LocaleController
        Inherits ComponentBase(Of ILocaleController, LocaleController)
        Implements ILocaleController

#Region "Private Shared Methods"

        Private Shared Function GetLocalesCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim portalID As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Dim locales As Dictionary(Of String, Locale)
            If portalID > Null.NullInteger Then
                locales = CBO.FillDictionary(Of String, Locale)("CultureCode", DataProvider.Instance().GetLanguagesByPortal(portalID), New Dictionary(Of String, Locale))
            Else
                locales = CBO.FillDictionary(Of String, Locale)("CultureCode", DataProvider.Instance().GetLanguages(), New Dictionary(Of String, Locale))
            End If
            Return locales
        End Function

#End Region

#Region "Public Methods"

        Public Function CanDeleteLanguage(ByVal languageId As Integer) As Boolean Implements ILocaleController.CanDeleteLanguage
            Dim canDelete As Boolean = True
            For Each package As PackageInfo In PackageController.GetPackagesByType("CoreLanguagePack")
                Dim languagePack As LanguagePackInfo = LanguagePackController.GetLanguagePackByPackage(package.PackageID)
                If languagePack.LanguageID = languageId Then
                    canDelete = False
                    Exit For
                End If
            Next
            Return canDelete
        End Function

        Public Function GetCultures(ByVal locales As Dictionary(Of String, Locale)) As List(Of CultureInfo) Implements ILocaleController.GetCultures
            Dim cultures As New List(Of CultureInfo)
            For Each locale As Locale In locales.Values
                cultures.Add(New CultureInfo(locale.Code))
            Next
            Return cultures
        End Function

        Public Function GetCurrentLocale(ByVal PortalId As Integer) As Locale Implements ILocaleController.GetCurrentLocale
            Dim locale As Locale = Nothing

            If HttpContext.Current IsNot Nothing AndAlso Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("language")) Then
                locale = GetLocale(HttpContext.Current.Request.QueryString("language"))
            End If
            If locale Is Nothing Then
                If PortalId = Null.NullInteger Then
                    locale = GetLocale(Localization.SystemLocale)
                Else
                    locale = GetDefaultLocale(PortalId)
                End If
            End If

            Return locale
        End Function

        Public Function GetDefaultLocale(ByVal portalId As Integer) As Locale Implements ILocaleController.GetDefaultLocale
            Dim portal As PortalInfo = New PortalController().GetPortal(portalId)
            Dim locale As Locale = Nothing
            If portal IsNot Nothing Then
                Dim locales As Dictionary(Of String, Locale) = GetLocales(portal.PortalID)
                If locales IsNot Nothing AndAlso locales.ContainsKey(portal.DefaultLanguage) Then
                    locale = locales.Item(portal.DefaultLanguage)
                End If
            End If
            If locale Is Nothing Then
                'Load System default
                locale = GetLocale(Localization.SystemLocale)
            End If
            Return locale
        End Function

        Public Function GetLocale(ByVal code As String) As Locale Implements ILocaleController.GetLocale
            Return GetLocale(Null.NullInteger, code)
        End Function

        Public Function GetLocale(ByVal portalID As Integer, ByVal code As String) As Locale Implements ILocaleController.GetLocale
            Dim dicLocales As Dictionary(Of String, Locale) = GetLocales(portalID)
            Dim locale As Locale = Nothing

            If dicLocales IsNot Nothing Then
                dicLocales.TryGetValue(code, locale)
            End If

            Return locale
        End Function

        Public Function GetLocale(ByVal languageID As Integer) As Locale Implements ILocaleController.GetLocale
            Dim dicLocales As Dictionary(Of String, Locale) = GetLocales(Null.NullInteger)
            Dim locale As Locale = Nothing

            For Each kvp As KeyValuePair(Of String, Locale) In dicLocales
                If kvp.Value.LanguageId = languageID Then
                    locale = kvp.Value
                    Exit For
                End If
            Next

            Return locale
        End Function

        Public Function GetLocales(ByVal portalID As Integer) As Dictionary(Of String, Locale) Implements ILocaleController.GetLocales
            Dim locales As New Dictionary(Of String, Locale)()

            If (Not Status = UpgradeStatus.Install) Then
                Dim cacheKey As String = String.Format(DataCache.LocalesCacheKey, portalID.ToString())
                locales = CBO.GetCachedObject(Of Dictionary(Of String, Locale))(New CacheItemArgs(cacheKey, DataCache.LocalesCacheTimeOut, DataCache.LocalesCachePriority, portalID), AddressOf GetLocalesCallBack, True)
            End If

            Return locales
        End Function

        Public Function GetPublishedLocales(ByVal portalID As Integer) As Dictionary(Of String, Locale) Implements ILocaleController.GetPublishedLocales
            Dim locales As New Dictionary(Of String, Locale)()

            For Each kvp As KeyValuePair(Of String, Locale) In GetLocales(portalID)
                If kvp.Value.IsPublished Then
                    locales.Add(kvp.Key, kvp.Value)
                End If
            Next

            Return locales
        End Function

        Public Function IsEnabled(ByVal localeCode As String, ByVal portalId As Integer) As Boolean Implements ILocaleController.IsEnabled
            Try
                Dim enabled As Boolean = False
                Dim dicLocales As Dictionary(Of String, Locale) = GetLocales(portalId)

                If (Not dicLocales.ContainsKey(localeCode)) Then
                    enabled = False
                ElseIf dicLocales.Item(localeCode) Is Nothing Then
                    'if localecode is neutral (en, es,...) try to find a locale that has the same language
                    If localeCode.IndexOf("-") = -1 Then
                        For Each strLocale As String In dicLocales.Keys
                            If strLocale.Split("-"c)(0) = localeCode Then
                                'set the requested _localecode to the full locale
                                localeCode = strLocale
                                enabled = True
                                Exit For
                            End If
                        Next
                    End If
                Else
                    enabled = True
                End If
                Return enabled
            Catch ex As Exception
                'item could not be retrieved  or error
                LogException(ex)
                Return False
            End Try
        End Function

        Public Sub UpdatePortalLocale(ByVal locale As Locale) Implements ILocaleController.UpdatePortalLocale
            DataProvider.Instance().UpdatePortalLanguage(locale.PortalId, locale.LanguageId, locale.IsPublished, UserController.GetCurrentUserInfo().UserID)
            DataCache.RemoveCache(String.Format(DataCache.LocalesCacheKey, locale.PortalId))
        End Sub

        Public Function IsDefaultLanguage(ByVal code As String) As Boolean Implements ILocaleController.IsDefaultLanguage
            Dim returnValue As Boolean = False
            If code = PortalController.GetCurrentPortalSettings.DefaultLanguage Then
                returnValue = True
            End If
            Return returnValue
        End Function

        Public Sub PublishLanguage(ByVal portalid As Integer, ByVal cultureCode As String, ByVal publish As Boolean) Implements ILocaleController.PublishLanguage
            Dim enabledLanguages As Dictionary(Of String, Locale) = LocaleController.Instance().GetLocales(portalid)
            Dim enabledlanguage As Locale = Nothing
            If enabledLanguages.TryGetValue(cultureCode, enabledlanguage) Then
                enabledlanguage.IsPublished = publish
                LocaleController.Instance().UpdatePortalLocale(enabledlanguage)

                Dim tabCtrl As New TabController()

                tabCtrl.PublishTabs(TabController.GetTabsBySortOrder(portalid, cultureCode, False))
            End If
        End Sub

#End Region

    End Class

End Namespace

