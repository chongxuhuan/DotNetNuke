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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.UI.WebControls
Imports System.Collections.Generic

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

#Region "Private Methods"

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
                    _Language = Localization.GetLocale(Request.QueryString("locale"))
                Else
                    _Language = New Locale
                End If
                Return _Language
            End Get
        End Property

        Protected ReadOnly Property ReturnUrl() As String
            Get
                If Language Is Nothing OrElse String.IsNullOrEmpty(Language.Code) Then
                    Return NavigateURL()
                Else
                    Return NavigateURL("", "Locale=" & Language.Code)
                End If
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
            If (Not Me.UserInfo.IsSuperUser) OrElse (Not IsAddMode) Then
                ctlLanguage.EditMode = UI.WebControls.PropertyEditorMode.View
            End If
            If Not Page.IsPostBack AndAlso Not String.IsNullOrEmpty(Language.Code) Then
                Dim enabledLanguages As Dictionary(Of String, Locale) = Localization.GetLocales(Me.ModuleContext.PortalId)
                Dim enabledLanguage As Locale = Nothing
                chkEnabled.Checked = enabledLanguages.TryGetValue(Language.Code, enabledLanguage)
            End If

            If Language IsNot Nothing Then
                ctlLanguage.LocalResourceFile = LocalResourceFile
                ctlLanguage.DataSource = Language
                ctlLanguage.DataBind()

                Dim attributes(-1) As Object
                ReDim attributes(0)
                attributes(0) = New LanguagesListTypeAttribute(LanguagesListType.All)
                fldCode.Editor.CustomAttributes = attributes
                fldFallback.Editor.CustomAttributes = attributes
            End If

            cmdDelete.Visible = (Not IsAddMode AndAlso Language.Code.ToLowerInvariant <> "en-us")
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem"))

            BindLanguage()
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(ReturnUrl, True)
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
            Try
                Localization.DeleteLanguage(Language)
                Response.Redirect(ReturnUrl, True)
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Try
                Dim code As String = Language.Code
                Dim languageId As Integer = Language.LanguageID
                If Me.UserInfo.IsSuperUser Then
                    'Update Language
                    If ctlLanguage.IsValid AndAlso ctlLanguage.IsDirty Then
                        Dim language As Locale = TryCast(ctlLanguage.DataSource, Locale)
                        If language IsNot Nothing Then
                            language.Text = CultureInfo.CreateSpecificCulture(language.Code).NativeName
                            Localization.SaveLanguage(language)
                            languageId = language.LanguageID
                            code = language.Code
                        End If
                    End If
                End If
                If chkEnabled.Checked Then
                    Dim enabledLanguages As Dictionary(Of String, Locale) = Localization.GetLocales(Me.ModuleContext.PortalId)
                    Dim enabledLanguage As Locale = Nothing
                    If Not enabledLanguages.TryGetValue(code, enabledLanguage) Then
                        'Add language to portal
                        Localization.AddLanguageToPortal(Me.ModuleContext.PortalId, languageId, True)
                    End If
                Else
                    'remove language from portal
                    Localization.RemoveLanguageFromPortal(Me.ModuleContext.PortalId, languageId)
                End If
                Response.Redirect(ReturnUrl, True)
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace