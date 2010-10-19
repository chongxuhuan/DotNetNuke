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
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Modules.Admin.Languages

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Manage languages for the portal
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [erikvb]    20100224  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class LanguageEnabler
        Inherits Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private _PortalDefault As String = ""
        Private _TabController As New TabController()

#End Region

#Region "Private Properties"

        Private ReadOnly Property ViewType() As String
            Get
                Return Localization.GetLanguageDisplayMode(PortalId)
            End Get
        End Property

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property PortalDefault As String
            Get
                Return _PortalDefault
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub BindDefaultLanguageSelector()
            languagesComboBox.DataBind()
            languagesComboBox.SetLanguage(PortalDefault)
        End Sub

        Private Sub BindGrid()
            languagesGrid.DataSource = LocaleController.Instance().GetLocales(Null.NullInteger).Values
            languagesGrid.DataBind()
        End Sub

        Private Function GetLocalizedPages(ByVal code As String, ByVal includeNeutral As Boolean) As TabCollection
            Return _TabController.GetTabsByPortal(PortalId).WithCulture(code, includeNeutral)
        End Function

#End Region

#Region "Protected Methods"

        Protected Function CanEnableDisable(ByVal code As String) As String
            Dim canEnable As Boolean = True
            If IsLanguageEnabled(code) Then
                canEnable = Not IsDefaultLanguage(code) AndAlso Not IsLanguagePublished(code)
            Else
                canEnable = Not IsDefaultLanguage(code)
            End If
            Return canEnable
        End Function

        Protected Function CanLocalize(ByVal code As String) As Boolean
            Return PortalSettings.ContentLocalizationEnabled AndAlso _
                IsLanguageEnabled(code) AndAlso _
                Not IsDefaultLanguage(code)
        End Function

        Protected Function GetEditUrl(ByVal id As String) As String
            Return NavigateURL(TabId, "Edit", String.Format("mid={0}", ModuleId), String.Format("locale={0}", id))
        End Function

        Protected Function GetEditKeysUrl(ByVal code As String, ByVal mode As String) As String
            Return NavigateURL(TabId, "Editor", String.Format("mid={0}", ModuleId), String.Format("locale={0}", code), String.Format("mode={0}", mode))
        End Function

        Protected Function GetLocalizedPages(ByVal code As String) As String
            Dim status As String = ""
            If Not IsDefaultLanguage(code) AndAlso IsLocalized(code) Then
                status = GetLocalizedPages(code, False).Where(Function(t) Not t.Value.IsDeleted).Count.ToString()
            End If
            Return status
        End Function

        Protected Function GetLocalizedStatus(ByVal code As String) As String
            Dim status As String = ""
            If Not IsDefaultLanguage(code) AndAlso IsLocalized(code) Then
                Dim defaultPageCount As Integer = GetLocalizedPages(PortalSettings.DefaultLanguage, False).Count
                Dim currentPageCount As Integer = GetLocalizedPages(code, False).Count
                status = String.Format("{0:#0%}", currentPageCount / defaultPageCount)
            End If
            Return status
        End Function

        Protected Function GetTranslatedPages(ByVal code As String) As String
            Dim status As String = ""
            If Not IsDefaultLanguage(code) AndAlso IsLocalized(code) Then
                Dim translatedCount As Integer = (From t As TabInfo In New TabController().GetTabsByPortal(PortalId).WithCulture(code, False).Values _
                                        Where t.IsTranslated AndAlso Not t.IsDeleted _
                                        Select t).Count
                status = translatedCount.ToString
            End If
            Return status
        End Function

        Protected Function GetTranslatedStatus(ByVal code As String) As String
            Dim status As String = ""
            If Not IsDefaultLanguage(code) AndAlso IsLocalized(code) Then
                Dim localizedCount As Integer = GetLocalizedPages(code, False).Count
                Dim translatedCount As Integer = (From t As TabInfo In New TabController().GetTabsByPortal(PortalId).WithCulture(code, False).Values _
                                        Where t.IsTranslated _
                                        Select t).Count
                status = String.Format("{0:#0%}", translatedCount / localizedCount)
            End If
            Return status
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

        Protected Function IsLanguagePublished(ByVal Code As String) As Boolean
            Dim isPublished As Boolean = Null.NullBoolean
            Dim enabledLanguage As Locale = Nothing
            If LocaleController.Instance().GetLocales(Me.ModuleContext.PortalId).TryGetValue(Code, enabledLanguage) Then
                isPublished = enabledLanguage.IsPublished
            End If
            Return isPublished
        End Function

        Protected Function IsLocalized(ByVal code As String) As Boolean
            Return GetLocalizedPages(code, False).Count > 0
        End Function

#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                _PortalDefault = PortalSettings.DefaultLanguage
                If Not Page.IsPostBack Then
                    BindDefaultLanguageSelector()
                    BindGrid()
                    chkBrowser.Checked = Me.ModuleContext.PortalSettings.EnableBrowserLanguage
                End If

                If Not UserInfo.IsSuperUser Then
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("HostOnlyMessage", LocalResourceFile), _
                                                              Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo)
                End If

                systemDefaultLanguageLabel.Language = Localization.SystemLocale
                If PortalSettings.ContentLocalizationEnabled Then
                    defaultLanguageLabel.Language = PortalSettings.DefaultLanguage
                    defaultLanguageLabel.Visible = True
                    languagesComboBox.Visible = False
                    updateButton.Visible = False
                    enableLocalizedContentButton.Visible = False
                    defaultPortalMessage.Text = Localization.GetString("PortalDefaultPublished.Text", Me.LocalResourceFile)
                    enabledPublishedPlaceHolder.Visible = True
                Else
                    defaultLanguageLabel.Visible = False
                    languagesComboBox.Visible = True
                    updateButton.Visible = True
                    enableLocalizedContentButton.Visible = Host.EnableContentLocalization
                    defaultPortalMessage.Text = Localization.GetString("PortalDefaultEnabled.Text", Me.LocalResourceFile)
                    enabledPublishedPlaceHolder.Visible = False
                End If

                addLanguageButton.Visible = UserInfo.IsSuperUser
                createLanguagePackButton.Visible = UserInfo.IsSuperUser
                verifyLanguageResourcesButton.Visible = UserInfo.IsSuperUser
                installLanguagePackButton.Visible = UserInfo.IsSuperUser
                timeZonesButton.Visible = UserInfo.IsSuperUser

                'Add the enable content Localization Button to the ToolTip Manager
                toolTipManager.TargetControls.Add(enableLocalizedContentButton.ID)
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub actionButton_Command(ByVal sender As Object, ByVal e As CommandEventArgs) _
            Handles addLanguageButton.Command, createLanguagePackButton.Command, verifyLanguageResourcesButton.Command, _
                    timeZonesButton.Command

            Response.Redirect(ModuleContext.EditUrl(e.CommandName), True)
        End Sub

        Protected Sub enabledCheckbox_CheckChanged(ByVal sender As Object, ByVal e As EventArgs)
            Try
                If TypeOf (sender) Is DnnCheckBox Then
                    Dim enabledCheckbox As DnnCheckBox = CType(sender, DnnCheckBox)
                    Dim languageId As Integer = Integer.Parse(enabledCheckbox.CommandArgument)
                    Dim locale As Locale = LocaleController.Instance().GetLocale(languageId)
                    Dim defaultLocale As Locale = LocaleController.Instance.GetDefaultLocale(PortalId)
                    Dim tabPath As String = ""

                    Dim enabledLanguages As Dictionary(Of String, Locale) = LocaleController.Instance().GetLocales(PortalId)
                    If enabledCheckbox.Enabled Then
                        ' do not touch default language
                        If enabledCheckbox.Checked = True Then
                            If Not enabledLanguages.ContainsKey(locale.Code) Then
                                'Add language to portal
                                Localization.AddLanguageToPortal(PortalId, languageId, True)

                                If PortalSettings.ContentLocalizationEnabled AndAlso GetLocalizedPages(locale.Code, False).Count < GetLocalizedPages(defaultLocale.Code, False).Count Then
                                    'Create Missing Localized Pages
                                    For Each t As TabInfo In _TabController.GetCultureTabList(PortalId)
                                        tabPath = GenerateTabPath(t.ParentId, t.TabName + " (" + locale.Code + ")")
                                        'if tabpath does not already exists...create it.
                                        If TabController.GetTabByTabPath(PortalId, tabPath, locale.Code) = Null.NullInteger Then
                                            _TabController.CreateLocalizedCopy(t, locale)
                                        End If
                                    Next
                                End If

                            End If
                        Else
                            'remove language from portal
                            Localization.RemoveLanguageFromPortal(PortalId, languageId)
                        End If
                    End If

                    'Redirect to refresh page (and skinobjects)
                    Response.Redirect(NavigateURL(), True)
                End If
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Protected Sub installLanguagePackButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles installLanguagePackButton.Click
            Response.Redirect(Util.InstallURL(ModuleContext.TabId, ""), True)
        End Sub

        Protected Sub languagesComboBox_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles languagesComboBox.ModeChanged
            BindGrid()
        End Sub

        Protected Sub languagesGrid_ItemCreated(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles languagesGrid.ItemCreated
            Dim gridItem As GridDataItem = TryCast(e.Item, GridDataItem)
            If gridItem IsNot Nothing Then
                Dim languge As Locale = TryCast(gridItem.DataItem, Locale)
                If languge IsNot Nothing Then
                    Dim localizeButton As ImageButton = gridItem.FindControl("localizeButton")
                    If localizeButton IsNot Nothing Then
                        Dim defaultLocale As Locale = LocaleController.Instance().GetLocale(PortalDefault)
                        Dim DisplayType As CultureDropDownTypes
                        Dim _ViewType As String = Convert.ToString(Personalization.GetProfile("LanguageDisplayMode", "ViewType" & PortalId.ToString))
                        Select Case _ViewType
                            Case "NATIVE"
                                DisplayType = CultureDropDownTypes.NativeName
                            Case "ENGLISH"
                                DisplayType = CultureDropDownTypes.EnglishName
                            Case Else
                                DisplayType = CultureDropDownTypes.DisplayName
                        End Select

                        Dim msg As String = String.Format(Localization.GetString("Localize.Confirm", Me.LocalResourceFile), Localization.GetLocaleName(languge.Code, DisplayType), Localization.GetLocaleName(defaultLocale.Code, DisplayType))
                        localizeButton.OnClientClick = DotNetNuke.Web.UI.Utilities.GetOnClientClickConfirm(localizeButton, msg)
                    End If
                End If
            End If
        End Sub

        Protected Sub languagesGrid_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles languagesGrid.PreRender
            For Each column As GridColumn In languagesGrid.Columns
                If (column.UniqueName = "ContentLocalization") Then
                    column.Visible = PortalSettings.ContentLocalizationEnabled
                End If
            Next
            languagesGrid.Rebind()

        End Sub

        Protected Sub localizePages(ByVal sender As Object, ByVal e As CommandEventArgs)
            Dim cultureCode As String = DirectCast(e.CommandArgument, String)

            If PortalSettings.ContentLocalizationEnabled AndAlso GetLocalizedPages(cultureCode, False).Count = 0 Then
                'Create Localized Pages
                Dim locale As Locale = LocaleController.Instance.GetLocale(cultureCode)
                For Each t As TabInfo In _TabController.GetCultureTabList(PortalId)
                    _TabController.CreateLocalizedCopy(t, locale)
                Next
            End If

            'Redirect to refresh page (and skinobjects)
            Response.Redirect(NavigateURL(), True)
        End Sub

        Protected Sub publishedCheckbox_CheckChanged(ByVal sender As Object, ByVal e As EventArgs)
            Try
                If TypeOf (sender) Is DnnCheckBox Then
                    Dim publishedCheckbox As DnnCheckBox = CType(sender, DnnCheckBox)
                    Dim languageId As Integer = Integer.Parse(publishedCheckbox.CommandArgument)
                    Dim locale As Locale = LocaleController.Instance().GetLocale(languageId)

                    If publishedCheckbox.Enabled Then
                        LocaleController.Instance().PublishLanguage(PortalId, locale.Code, publishedCheckbox.Checked)
                    End If

                    'Redirect to refresh page (and skinobjects)
                    Response.Redirect(NavigateURL(), True)
                End If
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Protected Sub toolTipManager_AjaxUpdate(ByVal sender As Object, ByVal e As Telerik.Web.UI.ToolTipUpdateEventArgs) Handles toolTipManager.AjaxUpdate
            Dim ctrl As Control = Page.LoadControl("~/desktopmodules/admin/languages/EnableLocalizedContent.ascx")
            e.UpdatePanel.ContentTemplateContainer.Controls.Add(ctrl)
        End Sub

        Protected Sub updateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles updateButton.Click
            Dim language As Locale

            PortalController.UpdatePortalSetting(Me.ModuleContext.PortalId, "EnableBrowserLanguage", chkBrowser.Checked.ToString())

            ' first check whether or not portal default language has changed
            Dim newDefaultLanguage As String = languagesComboBox.SelectedValue
            If newDefaultLanguage <> PortalSettings.DefaultLanguage Then
                If Not IsLanguageEnabled(newDefaultLanguage) Then
                    language = LocaleController.Instance().GetLocale(newDefaultLanguage)
                    Localization.AddLanguageToPortal(Me.ModuleContext.PortalId, language.LanguageId, True)
                End If

                ' update portal default language
                Dim objPortalController As New PortalController
                Dim objPortal As PortalInfo = objPortalController.GetPortal(PortalId)
                objPortal.DefaultLanguage = newDefaultLanguage
                objPortalController.UpdatePortalInfo(objPortal)

                _PortalDefault = newDefaultLanguage
            End If

            BindDefaultLanguageSelector()
            BindGrid()

        End Sub


#End Region

    End Class

End Namespace
