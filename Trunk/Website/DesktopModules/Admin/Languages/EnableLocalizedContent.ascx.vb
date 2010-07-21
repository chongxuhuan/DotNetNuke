'
' DotNetNuke® - http://www.dotnetnuke.com
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
Imports System.Collections.Generic

Imports DotNetNuke
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports System.Globalization
Imports System.Linq
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Services.Installer
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Entities.Modules
Imports Telerik.Web.UI
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Services.Personalization

Namespace DotNetNuke.Modules.Admin.Languages

    Partial Class EnableLocalizedContent
        Inherits PortalModuleBase

#Region "Private Members"

        Private _PortalDefault As String = ""
        Private MyFileName As String = "EnableLocalizedContent.ascx"

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property PortalDefault As String
            Get
                Return _PortalDefault
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Function GetLocalizedPageCount(ByVal code As String, ByVal includeNeutral As Boolean) As Integer
            Dim tabCtrl As New TabController
            Return tabCtrl.GetTabsByPortal(PortalId).WithCulture(code, includeNeutral).Count
        End Function

        Protected Function IsDefaultLanguage(ByVal code As String) As Boolean
            Dim returnValue As Boolean = False
            If code = PortalDefault Then
                returnValue = True
            End If
            Return returnValue
        End Function

        Protected Function IsLanguageEnabled(ByVal Code As String) As Boolean
            Dim enabledLanguage As Locale = Nothing
            Return LocaleController.Instance().GetLocales(Me.ModuleContext.PortalId).TryGetValue(Code, enabledLanguage)
        End Function

        Private Sub PublishLanguage(ByVal cultureCode As String, ByVal publish As Boolean)
            Dim enabledLanguages As Dictionary(Of String, Locale) = LocaleController.Instance().GetLocales(PortalId)
            Dim enabledlanguage As Locale = Nothing
            If enabledLanguages.TryGetValue(cultureCode, enabledlanguage) Then
                enabledlanguage.IsPublished = publish
                LocaleController.Instance().UpdatePortalLocale(enabledlanguage)
            End If
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.LocalResourceFile = Services.Localization.Localization.GetResourceFile(Me, "EnableLocalizedContent.ascx")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            _PortalDefault = PortalSettings.DefaultLanguage
            defaultLanguageLabel.Language = PortalSettings.DefaultLanguage
            defaultLanguageLabel.Visible = True
        End Sub

        Protected Sub cancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cancelButton.Click
            'Redirect to refresh page (and skinobjects)
            Response.Redirect(NavigateURL(), True)
        End Sub

        Protected Sub updateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles updateButton.Click
            PortalController.UpdatePortalSetting(PortalId, "ContentLocalizationEnabled", "True")

            Dim tabCtrl As New TabController
            If GetLocalizedPageCount(PortalDefault, False) = 0 Then
                tabCtrl.LocalizeTabs(PortalId, PortalDefault)
            End If
            PublishLanguage(PortalDefault, True)

            ' populate other languages
            For Each locale As Locale In LocaleController.Instance().GetLocales(PortalSettings.PortalId).Values
                If LocaleController.Instance.IsEnabled(locale.Code, PortalId) _
                        AndAlso Not IsDefaultLanguage(locale.Code) _
                        AndAlso GetLocalizedPageCount(locale.Code, False) = 0 Then
                    tabCtrl.LocalizeTabs(PortalId, locale.Code)
                End If
            Next

            'Redirect to refresh page (and skinobjects)
            Response.Redirect(NavigateURL(), True)
        End Sub

#End Region

    End Class
End Namespace