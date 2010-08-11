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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.UI.WebControls
Imports System.Collections.Generic
Imports DotNetNuke.Security.Roles

Namespace DotNetNuke.Modules.Admin.Languages

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditLanguage ModuleUserControlBase is used to edit a Language
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	02/14/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class EditLanguage
        Inherits PortalModuleBase

#Region "Private Members"

        Private _Language As Locale

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property IsAddMode() As Boolean
            Get
                Return String.IsNullOrEmpty(Request.QueryString("locale"))
            End Get
        End Property

        Protected ReadOnly Property Language() As Locale
            Get
                If Not IsAddMode Then
                    _Language = LocaleController.Instance().GetLocale(Integer.Parse(Request.QueryString("locale")))
                End If
                Return _Language
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine Binds the Language
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/14/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindLanguage()
            languageLanguageLabel.Visible = Not Language Is Nothing
            languageComboBox.Visible = Language Is Nothing
            languageComboBox.IncludeNoneSpecified = False
            languageComboBox.HideLanguagesList = LocaleController.Instance().GetLocales(Null.NullInteger)
            languageComboBox.DataBind()

            fallbackLanguageLabel.Visible = Not Me.UserInfo.IsSuperUser
            fallBackComboBox.Visible = Me.UserInfo.IsSuperUser
            fallBackComboBox.IncludeNoneSpecified = True
            If Language IsNot Nothing Then
                Dim hideLanguagesList As New Dictionary(Of String, Locale)
                hideLanguagesList.Add(Language.Code, Language)
                fallBackComboBox.HideLanguagesList = hideLanguagesList
            End If
            fallBackComboBox.DataBind()
            If Not IsPostBack AndAlso Language IsNot Nothing Then
                fallbackLanguageLabel.Language = Language.Fallback
                languageLanguageLabel.Language = Language.Code
                languageComboBox.SetLanguage(Language.Code)
                fallBackComboBox.SetLanguage(Language.Fallback)
            End If

            If Language Is Nothing OrElse Language.Code = PortalSettings.DefaultLanguage Then
                translatorsRow.Visible = False
            Else
                Dim defaultRoles As String = PortalController.GetPortalSetting(String.Format("DefaultTranslatorRoles-{0}", Language.Code), PortalId, "Administrators")

                translatorRoles.SelectedRoleNames = New ArrayList(defaultRoles.Split(";"c))

                translatorsRow.Visible = True
            End If

            Dim isEnabled As Boolean = Null.NullBoolean
            If Not IsAddMode Then
                Dim enabledLanguage As Locale = Nothing
                isEnabled = LocaleController.Instance().GetLocales(Me.ModuleContext.PortalId).TryGetValue(Language.Code, enabledLanguage)
            End If

            cmdDelete.Visible = (Me.UserInfo.IsSuperUser AndAlso Not IsAddMode AndAlso _
                                 Not isEnabled AndAlso Not Language.IsPublished AndAlso _
                                 LocaleController.Instance().CanDeleteLanguage(Language.LanguageId) AndAlso _
                                 Language.Code.ToLowerInvariant <> "en-us")
        End Sub

        Private Function IsLanguageEnabled(ByVal Code As String) As Boolean
            Dim enabledLanguage As Locale = Nothing
            Return LocaleController.Instance().GetLocales(Me.ModuleContext.PortalId).TryGetValue(Code, enabledLanguage)
        End Function

#End Region

#Region "Event Handlers"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem"))

            BindLanguage()
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
            Try
                Localization.DeleteLanguage(Language)
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Try
                If Me.UserInfo.IsSuperUser Then
                    'Update Language
                    If Language Is Nothing Then
                        _Language = LocaleController.Instance().GetLocale(languageComboBox.SelectedValue)
                        If _Language Is Nothing Then
                            _Language = New Locale()
                            Language.Code = languageComboBox.SelectedValue
                        End If
                    End If
                    Language.Fallback = fallBackComboBox.SelectedValue
                    Language.Text = CultureInfo.CreateSpecificCulture(Language.Code).NativeName
                    Localization.SaveLanguage(Language)
                End If

                If Not IsLanguageEnabled(Language.Code) Then
                    'Add language to portal
                    Localization.AddLanguageToPortal(PortalId, Language.LanguageId, True)
                End If

                Dim roles As String = Null.NullString
                If IsAddMode Then
                    roles = String.Format("Administrators;{0}", String.Format("Translator ({0})", Language.Code))
                Else
                    For Each role As String In translatorRoles.SelectedRoleNames
                        roles += role + ";"
                    Next
                End If

                PortalController.UpdatePortalSetting(Me.PortalId, String.Format("DefaultTranslatorRoles-{0}", Language.Code), roles)

                Dim tabCtrl As New TabController
                Dim tabs As TabCollection = tabCtrl.GetTabsByPortal(PortalId).WithCulture(Language.Code, False)
                If PortalSettings.ContentLocalizationEnabled AndAlso tabs.Count = 0 Then
                    'Create Localized Pages
                    For Each t As TabInfo In tabCtrl.GetCultureTabList(PortalId)
                        tabCtrl.CreateLocalizedCopy(t, Language)
                    Next
                End If

                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace