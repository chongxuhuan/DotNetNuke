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
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Services.Installer
Imports DotNetNuke.Security.Permissions

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
        Implements Entities.Modules.IActionable


#Region "Private members"

        Private _ViewType As String = ""

#End Region

#Region "Private Methods"

        Private Sub BindViewType()
            rbViewType.SelectedValue = viewType
        End Sub

        Private Sub bindGrid()
            Me.dgLanguages.DataSource = Localization.GetLocales(Null.NullInteger).Values
            Me.dgLanguages.DataBind()

            dgLanguages.Columns(2).Visible = False
            If Not UserInfo.IsSuperUser Then
                dgLanguages.Columns(5).Visible = False
                dgLanguages.Columns(6).Visible = False
            End If

        End Sub

        Protected Sub DisplayDirtyStatus()
            If IsDirty Then
                litStatus.Text = "<div class=""le_changesPending"">There are changes pending to be saved</div>"
                cmdUpdate.Enabled = True
                cmdCancel.Enabled = True
            Else
                litStatus.Text = ""
                cmdUpdate.Enabled = False
                cmdCancel.Enabled = False
            End If
        End Sub


        Private Sub bindDefaultLanguageSelector()
            Localization.LoadCultureDropDownList(ddlPortalDefaultLanguage, DisplayType, PortalSettings.DefaultLanguage, True)
        End Sub

#End Region

#Region "Private Properties"


        Private ReadOnly Property DisplayType() As CultureDropDownTypes
            Get
                Select Case viewType
                    Case "NATIVE"
                        Return CultureDropDownTypes.NativeName
                    Case "ENGLISH"
                        Return CultureDropDownTypes.EnglishName
                End Select
            End Get
        End Property

        Protected ReadOnly Property GridviewEditMode(ByVal Code As String) As Boolean
            Get
                Dim returnvalue As Boolean = False
                If UserInfo.IsSuperUser OrElse Me.IsEditable Then
                    If Code = WorkingPortalDefaultLanguage Then
                        returnvalue = False
                    Else
                        returnvalue = True
                    End If
                End If
                Return returnvalue
            End Get
        End Property

        Private ReadOnly Property viewType() As String
            Get
                If _ViewType = "" Then
                    _ViewType = Convert.ToString(Personalization.Personalization.GetProfile("LanguageEnabler", "ViewType" & PortalId.ToString))
                End If
                If _ViewType = "" Then _ViewType = "NATIVE"
                Return _ViewType
            End Get
        End Property


        Private ReadOnly Property DefaultLanguageChanging() As Boolean
            Get
                Return PortalSettings.DefaultLanguage <> PortalDefault
            End Get
        End Property

        Private ReadOnly Property WorkingPortalDefaultLanguage() As String
            Get
                If DefaultLanguageChanging Then
                    Return PortalDefault
                Else
                    Return PortalSettings.DefaultLanguage
                End If
            End Get
        End Property

        ''' <summary>
        ''' Checks whether or not the page has been edited
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        '''     [erikvb]    20100224  created
        ''' </history>
        Private ReadOnly Property IsDirty() As Boolean
            Get
                Dim returnValue As Boolean = DefaultLanguageChanging
                If Not returnValue Then
                    For Each di As DataGridItem In dgLanguages.Items
                        If (di.ItemType = ListItemType.Item Or di.ItemType = ListItemType.AlternatingItem) Then
                            Dim lblLanguageCode As Label = CType(di.Cells(0).FindControl("lblLanguageCode"), Label)
                            Dim chkEnabled As CheckBox = CType(di.Cells(3).FindControl("chkEnabled"), CheckBox)
                            If (chkEnabled IsNot Nothing) AndAlso (lblLanguageCode IsNot Nothing) Then
                                Dim language As Locale = Localization.GetLocale(lblLanguageCode.Text)
                                If (isLanguageEnabled(language.Code, False) <> chkEnabled.Checked) Or (language.Fallback <> LanguageFallbacks(language.Code)) Then
                                    returnValue = True
                                    Exit For
                                End If
                            End If

                        End If
                    Next
                End If
                Return returnValue
            End Get
        End Property

        Private ReadOnly Property LanguageSettings() As Dictionary(Of String, Boolean)
            Get
                Dim returnValue As Dictionary(Of String, Boolean) = Nothing
                If ViewState("LanguageSettings") Is Nothing Then
                    returnValue = New Dictionary(Of String, Boolean)
                    ViewState("LanguageSettings") = returnValue
                Else
                    returnValue = CType(ViewState("LanguageSettings"), Dictionary(Of String, Boolean))
                End If
                If returnValue.Count = 0 Then
                    For Each LocaleKVP As KeyValuePair(Of String, Locale) In Localization.GetLocales(Null.NullInteger)
                        returnValue(LocaleKVP.Key) = isLanguageEnabled(LocaleKVP.Key)
                    Next
                End If
                Return returnValue
            End Get
        End Property

        Private ReadOnly Property LanguageFallbacks() As Dictionary(Of String, String)
            Get
                Dim returnValue As Dictionary(Of String, String) = Nothing
                If ViewState("LanguageFallbacks") Is Nothing Then
                    returnValue = New Dictionary(Of String, String)
                    ViewState("LanguageFallbacks") = returnValue
                Else
                    returnValue = CType(ViewState("LanguageFallbacks"), Dictionary(Of String, String))
                End If
                If returnValue.Count = 0 Then
                    For Each LocaleKVP As KeyValuePair(Of String, Locale) In Localization.GetLocales(Null.NullInteger)
                        returnValue(LocaleKVP.Key) = LocaleKVP.Value.Fallback
                    Next
                End If
                Return returnValue
            End Get
        End Property

        Private Property PortalDefault() As String
            Get
                If ViewState("PortalDefault") Is Nothing Then
                    ViewState("PortalDefault") = ddlPortalDefaultLanguage.SelectedValue
                End If
                Return CType(ViewState("PortalDefault"), String)
            End Get
            Set(ByVal value As String)
                ViewState("PortalDefault") = value
            End Set
        End Property

#End Region

#Region "protected Functions"

        Protected Function isLanguageEnabled(ByVal Code As String) As Boolean
            Return isLanguageEnabled(Code, True)
        End Function

        Protected Function isLanguageEnabled(ByVal Code As String, ByVal checkDefaultLanguage As Boolean) As Boolean
            Return isLanguageEnabled(Code, True, False)
        End Function

        Protected Function isLanguageEnabled(ByVal Code As String, ByVal checkDefaultLanguage As Boolean, ByVal useCache As Boolean) As Boolean
            Dim returnValue As Boolean = False
            Dim enabledLanguage As Locale = Nothing
            If checkDefaultLanguage AndAlso DefaultLanguageChanging AndAlso (Code = WorkingPortalDefaultLanguage) Then
                returnValue = True
            Else
                If useCache Then
                    returnValue = LanguageSettings(Code)
                Else
                    returnValue = Localization.GetLocales(Me.ModuleContext.PortalId).TryGetValue(Code, enabledLanguage)
                End If
            End If
            Return returnValue
        End Function

        Protected Function isLanguageDefault(ByVal Code As String) As Boolean
            Return Code = WorkingPortalDefaultLanguage
        End Function

        Protected Function GetLanguageName(ByVal code As String) As String
            Dim returnValue As String = "unknown"
            Dim culture As New CultureInfo(code)
            If Not culture Is Nothing Then
                Select Case viewType
                    Case "NATIVE"
                        returnValue = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(culture.NativeName)
                    Case "ENGLISH"
                        returnValue = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(culture.EnglishName)
                End Select
            End If
            Return returnValue
        End Function

        Protected Function GetLanguageHint(ByVal code As String) As String
            Dim returnValue As String = ""
            If code = WorkingPortalDefaultLanguage Then
                returnValue = Localization.GetString("PortalDefault", LocalResourceFile)
            End If
            Return returnValue
        End Function
#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [erikvb]    20100224  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If UserInfo.IsSuperUser Or ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", Me.ModuleConfiguration) Then

                    If Not Page.IsPostBack Then
                        BindViewType()
                        bindDefaultLanguageSelector()
                        bindGrid()
                        DisplayDirtyStatus()
                    End If
                Else
                    Response.Redirect(NavigateURL("Access Denied"))
                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
            If LanguageSettings.Count > 0 Then
                ViewState("LanguageSettings") = LanguageSettings
            End If
            If LanguageFallbacks.Count > 0 Then
                ViewState("LanguageFallbacks") = LanguageFallbacks
            End If
        End Sub

        ''' <summary>
        ''' cmdUpdate_Click updates the changed values to the database, using API calls
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        ''' <history>
        '''     [erikvb]    20100224  created
        ''' </history>
        Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try

                Dim language As Locale

                ' first check whether or not portal default language has changed
                Dim newDefaultLanguage As String = PortalDefault
                If newDefaultLanguage <> PortalSettings.DefaultLanguage Then
                    If Not isLanguageEnabled(newDefaultLanguage, False, False) Then
                        language = Localization.GetLocale(newDefaultLanguage)
                        Localization.AddLanguageToPortal(Me.ModuleContext.PortalId, language.LanguageID, True)
                    End If

                    ' update portal default language
                    Dim objPortalController As New PortalController
                    Dim objPortal As PortalInfo = objPortalController.GetPortal(PortalId)
                    objPortal.DefaultLanguage = newDefaultLanguage
                    objPortalController.UpdatePortalInfo(objPortal)
                End If

                Dim enabledLanguages As Dictionary(Of String, Locale) = Localization.GetLocales(Me.ModuleContext.PortalId)
                For Each di As DataGridItem In dgLanguages.Items
                    If (di.ItemType = ListItemType.Item Or di.ItemType = ListItemType.AlternatingItem) Then
                        Dim lblLanguageCode As Label = CType(di.Cells(0).FindControl("lblLanguageCode"), Label)
                        Dim chkEnabled As CheckBox = CType(di.Cells(3).FindControl("chkEnabled"), CheckBox)
                        Dim ddlFallback As DropDownList = CType(di.Cells(2).FindControl("ddlFallback"), DropDownList)
                        If (chkEnabled IsNot Nothing) AndAlso (lblLanguageCode IsNot Nothing) AndAlso (ddlFallback IsNot Nothing) Then
                            language = Localization.GetLocale(lblLanguageCode.Text)
                            If chkEnabled.Enabled Then
                                ' do not touch default language
                                If chkEnabled.Checked = True Then
                                    Dim enabledLanguage As Locale = Nothing
                                    If Not enabledLanguages.TryGetValue(lblLanguageCode.Text, enabledLanguage) Then
                                        'Add language to portal
                                        Localization.AddLanguageToPortal(Me.ModuleContext.PortalId, language.LanguageID, True)
                                    End If
                                Else
                                    'remove language from portal
                                    Localization.RemoveLanguageFromPortal(Me.ModuleContext.PortalId, language.LanguageID)
                                End If
                            End If

                            'If UserInfo.IsSuperUser Then
                            '    ' update language fallback, only if user = host user
                            '    If language.Fallback <> ddlFallback.SelectedValue Then
                            '        language.Fallback = ddlFallback.SelectedValue
                            '        Localization.SaveLanguage(language)
                            '    End If
                            'End If
                        End If

                    End If
                Next

                Response.Redirect(NavigateURL())
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Protected Sub ddlFallback_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Try
                If TypeOf (sender) Is DropDownList Then
                    Dim languageCode As String = ""
                    Dim ddlFallback As DropDownList = CType(sender, DropDownList)
                    Dim ParentCell As TableCell = CType(ddlFallback.Parent, TableCell)
                    Dim parentRow As TableRow = CType(ParentCell.Parent, TableRow)
                    Dim lblLanguageCode As Label = CType(parentRow.Cells(0).FindControl("lblLanguageCode"), Label)
                    If Not lblLanguageCode Is Nothing Then
                        languageCode = lblLanguageCode.Text
                        LanguageFallbacks(languageCode) = ddlFallback.SelectedValue
                    End If
                End If
                DisplayDirtyStatus()
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Protected Sub chkEnabled_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            Try
                If TypeOf (sender) Is CheckBox Then
                    Dim languageCode As String = ""
                    Dim chkEnabled As CheckBox = CType(sender, CheckBox)
                    Dim ParentCell As TableCell = CType(chkEnabled.Parent, TableCell)
                    Dim parentRow As TableRow = CType(ParentCell.Parent, TableRow)
                    Dim lblLanguageCode As Label = CType(parentRow.Cells(0).FindControl("lblLanguageCode"), Label)
                    If Not lblLanguageCode Is Nothing Then
                        languageCode = lblLanguageCode.Text
                        LanguageSettings(languageCode) = chkEnabled.Checked
                    End If
                End If
                DisplayDirtyStatus()
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Protected Sub rbViewType_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rbViewType.SelectedIndexChanged
            Personalization.Personalization.SetProfile("LanguageEnabler", "ViewType" & PortalSettings.PortalId.ToString, rbViewType.SelectedValue)
            bindDefaultLanguageSelector()
            bindGrid()
            DisplayDirtyStatus()
        End Sub

        Protected Sub ddlPortalDefaultLanguage_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlPortalDefaultLanguage.SelectedIndexChanged
            PortalDefault = ddlPortalDefaultLanguage.SelectedValue
            bindGrid()
            DisplayDirtyStatus()
        End Sub

        Private Sub dgLanguages_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgLanguages.ItemDataBound

            If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
                Dim ddlfallback As DropDownList = CType(e.Item.FindControl("ddlFallback"), DropDownList)
                Dim rowLanguage As Locale = CType(e.Item.DataItem, Locale)
                If (ddlfallback IsNot Nothing) AndAlso (rowLanguage IsNot Nothing) Then
                    If rowLanguage.Code = Localization.SystemLocale Then
                        ddlfallback.Visible = False
                    Else
                        Localization.LoadCultureDropDownList(ddlfallback, DisplayType, LanguageFallbacks(rowLanguage.Code), rowLanguage.Code, True)
                    End If
                End If
            End If
        End Sub

        Protected Sub cmdEdit_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            If TypeOf (sender) Is ImageButton Then
                Dim cmdEdit As ImageButton = CType(sender, ImageButton)
                Dim mode As String = cmdEdit.CommandName
                Response.Redirect(NavigateURL("Editor", "mid=" + ModuleId.ToString, "locale=" + cmdEdit.CommandArgument, "mode=" + mode))
            End If
        End Sub

        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Response.Redirect(NavigateURL())
        End Sub

#End Region

#Region "Optional Interfaces"

        ''' ----------------------------------------------------------------------------- 
        ''' <summary>
        ''' Registers the module actions required for interfacing with the portal framework
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection

                Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("CreateLanguage.Action", LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", ModuleContext.EditUrl("Edit"), False, SecurityAccessLevel.Host, True, False)

                '' install new LanguagePack
                Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("LanguagePackInstall.Action", LocalResourceFile), ModuleActionType.AddContent, "", "action_import.gif", Util.InstallURL(ModuleContext.TabId, ""), False, SecurityAccessLevel.Host, True, False)

                '' Language Settings
                Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("LanguageSettings.Action", LocalResourceFile), ModuleActionType.AddContent, "", "settings.gif", ModuleContext.EditUrl("LanguageSettings"), False, SecurityAccessLevel.Edit, True, False)

                '' Time Zone Editor
                Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("TimeZone.Action", LocalResourceFile), ModuleActionType.AddContent, "", "icon_language_16px.gif", ModuleContext.EditUrl("TimeZone"), False, SecurityAccessLevel.Host, True, False)

                '' Resource Verifier
                Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("ResourceVerifier.Action", LocalResourceFile), ModuleActionType.AddContent, "", "icon_language_16px.gif", ModuleContext.EditUrl("Verify"), False, SecurityAccessLevel.Host, True, False)

                '' Create new LanguagePack
                Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("CreateLanguagePack.Action", LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", ModuleContext.EditUrl("PackageWriter"), False, SecurityAccessLevel.Host, True, False)

                Return Actions
            End Get
        End Property

#End Region

    End Class

End Namespace
