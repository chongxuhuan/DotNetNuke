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

Namespace DotNetNuke.Modules.Admin.Features

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The LanguagePackEditor ModuleUserControlBase is used to edit a Language
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	02/14/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class LanguagePackEditor
        Inherits PackageEditorBase

#Region "Private Methods"

        Private _LanguagesModule As ModuleInfo = Nothing
        Private _LanguagePack As LanguagePackInfo = Nothing

#End Region

#Region "Protected Properties"

        Protected Overrides ReadOnly Property EditorID() As String
            Get
                Return "LanguagePackEditor"
            End Get
        End Property

        Protected ReadOnly Property Language() As Locale
            Get
                Return Localization.GetLocaleByID(LanguagePack.LanguageID)
            End Get
        End Property

        Protected ReadOnly Property LanguagePack() As LanguagePackInfo
            Get
                If _LanguagePack Is Nothing Then
                    _LanguagePack = LanguagePackController.GetLanguagePackByPackage(PackageID)
                End If
                Return _LanguagePack
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub BindLanguagePack()
            cboLanguage.DataSource = Localization.GetLocales(Null.NullInteger).Values
            cboLanguage.DataBind()

            If cboLanguage.Items.FindByValue(LanguagePack.LanguageID) IsNot Nothing Then
                cboLanguage.Items.FindByValue(LanguagePack.LanguageID).Selected = True
            End If

            If LanguagePack IsNot Nothing Then
                If LanguagePack.PackageType = LanguagePackType.Extension Then
                    'Get all the packages but only bind to combo if not a language package
                    Dim packages As List(Of PackageInfo) = New List(Of PackageInfo)
                    For Each package As PackageInfo In PackageController.GetPackages()
                        If package.PackageType <> "CoreLanguagePack" AndAlso package.PackageType <> "ExtensionLanguagePack" Then
                            packages.Add(package)
                        End If
                    Next
                    cboPackage.DataSource = packages
                    cboPackage.DataBind()

                    If cboPackage.Items.FindByValue(LanguagePack.DependentPackageID) IsNot Nothing Then
                        cboPackage.Items.FindByValue(LanguagePack.DependentPackageID).Selected = True
                    End If

                    trPackage.Visible = True
                Else
                    trPackage.Visible = False
                End If
            End If
        End Sub

#End Region

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            cmdEdit.Visible = Not IsWizard
        End Sub

#Region "Public Methods"

        Public Overrides Sub Initialize()
            BindLanguagePack()
        End Sub

        Public Overrides Sub UpdatePackage()
            LanguagePack.LanguageID = Integer.Parse(cboLanguage.SelectedValue)
            If LanguagePack.PackageType = LanguagePackType.Extension Then
                LanguagePack.DependentPackageID = Integer.Parse(cboPackage.SelectedValue)
            End If
            LanguagePackController.SaveLanguagePack(LanguagePack)
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub cmdEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEdit.Click
            Dim languagesTab As Integer = TabController.GetTabByTabPath(Me.ModuleContext.PortalId, "//Admin//Languages")
            Response.Redirect(NavigateURL(languagesTab, "", "Locale=" & Language.Code), True)
        End Sub

#End Region

    End Class

End Namespace