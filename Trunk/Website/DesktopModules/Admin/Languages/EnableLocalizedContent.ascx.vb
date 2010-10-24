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
Imports Telerik.Web.UI.Upload

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

        Private Sub ProcessLanguage(ByVal pageList As List(Of TabInfo), ByVal locale As Locale, ByVal languageCount As Integer, ByVal totalLanguages As Integer)
            Dim tabCtrl As New TabController
            Dim progress As RadProgressContext = RadProgressContext.Current

            progress.Speed = "N/A"
            progress.PrimaryTotal = totalLanguages
            progress.PrimaryValue = languageCount

            Dim total As Integer = pageList.Count
            For i As Integer = 0 To total - 1
                Dim currentTab As TabInfo = pageList(i)
                Dim stepNo As Integer = i + 1

                progress.SecondaryTotal = total
                progress.SecondaryValue = stepNo
                progress.SecondaryPercent = (stepNo * 100) \ total
                progress.PrimaryPercent = (((languageCount * total) + stepNo) * 100) \ (total * totalLanguages)

                progress.CurrentOperationText = String.Format(Localization.GetString("ProcessingPage", Me.LocalResourceFile), locale.Code, stepNo, total, currentTab.TabName)

                If Not Response.IsClientConnected Then
                    'Cancel button was clicked or the browser was closed, so stop processing
                    Exit For
                End If

                progress.TimeEstimated = (total - stepNo) * 100

                If locale.Code = PortalDefault Then
                    tabCtrl.LocalizeTab(currentTab, locale)
                Else
                    tabCtrl.CreateLocalizedCopy(currentTab, locale)
                End If

                'Add a delay for debug testing
                'Threading.Thread.Sleep(500)
            Next
        End Sub
#End Region

#Region "Event Handlers"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.LocalResourceFile = Services.Localization.Localization.GetResourceFile(Me, "EnableLocalizedContent.ascx")

            'Set AJAX timeout to 1 hr for large sites
            AJAX.GetScriptManager(Me.Page).AsyncPostBackTimeout = "3600"

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            _PortalDefault = PortalSettings.DefaultLanguage
            defaultLanguageLabel.Language = PortalSettings.DefaultLanguage
            defaultLanguageLabel.Visible = True

            If Not IsPostBack Then
                'Do not display SelectedFilesCount progress indicator.
                pageCreationProgressArea.ProgressIndicators = pageCreationProgressArea.ProgressIndicators And Not ProgressIndicators.SelectedFilesCount
            End If
            pageCreationProgressArea.ProgressIndicators = pageCreationProgressArea.ProgressIndicators And Not ProgressIndicators.TimeEstimated
            pageCreationProgressArea.ProgressIndicators = pageCreationProgressArea.ProgressIndicators And Not ProgressIndicators.TransferSpeed

            pageCreationProgressArea.Localization.Total = Localization.GetString("TotalLanguages", Me.LocalResourceFile)
            pageCreationProgressArea.Localization.TotalFiles = Localization.GetString("TotalPages", Me.LocalResourceFile)
            pageCreationProgressArea.Localization.Uploaded = Localization.GetString("TotalProgress", Me.LocalResourceFile)
            pageCreationProgressArea.Localization.UploadedFiles = Localization.GetString("Progress", Me.LocalResourceFile)
            pageCreationProgressArea.Localization.CurrentFileName = Localization.GetString("Processing", Me.LocalResourceFile)
        End Sub

        Protected Sub cancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cancelButton.Click
            'Redirect to refresh page (and skinobjects)
            Response.Redirect(NavigateURL(), True)
        End Sub


        Protected Sub updateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles updateButton.Click
            Dim tabCtrl As New TabController
            Dim portalCtrl As New PortalController
            Dim languageCount As Integer = LocaleController.Instance().GetLocales(PortalSettings.PortalId).Count
            Dim pageList As List(Of TabInfo) = tabCtrl.GetDefaultCultureTabList(PortalId)

            Dim languageCounter As Integer = 0
            ProcessLanguage(pageList, _
                            LocaleController.Instance.GetLocale(PortalDefault), _
                            languageCounter, _
                            languageCount)
            PublishLanguage(PortalDefault, True)

            PortalController.UpdatePortalSetting(PortalId, "ContentLocalizationEnabled", "True")

            ' populate other languages
            For Each locale As Locale In LocaleController.Instance().GetLocales(PortalSettings.PortalId).Values
                If Not IsDefaultLanguage(locale.Code) Then
                    languageCounter += 1
                    pageList = tabCtrl.GetCultureTabList(PortalId)

                    'add translator role
                    Localization.AddTranslatorRole(PortalId, locale)

                    'populate pages
                    ProcessLanguage(pageList, _
                                    locale, _
                                    languageCounter, _
                                    languageCount)
                    'Map special pages
                    portalCtrl.MapLocalizedSpecialPages(PortalSettings.PortalId, locale.Code)

                End If
            Next

            ''Redirect to refresh page (and skinobjects)
            Response.Redirect(NavigateURL(), True)
        End Sub

#End Region

    End Class
End Namespace