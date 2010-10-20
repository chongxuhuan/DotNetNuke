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

Imports DotNetNuke.Entities.Controllers
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Modules.Admin.Languages

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : LanguageSettings
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Supplies LanguageSettings functionality for the Extensions module
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]   04/03/2008    Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class LanguageSettings
        Inherits ModuleUserControlBase

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
                Me.ModuleContext.Configuration.ModuleTitle = Services.Localization.Localization.GetString("HostSettings.Title", Me.LocalResourceFile)
            Else
                Me.ModuleContext.Configuration.ModuleTitle = Services.Localization.Localization.GetString("PortalSettings.Title", Me.LocalResourceFile)
            End If

            valPageSize.MinimumValue = 1
            valPageSize.MaximumValue = Int32.MaxValue
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Page.IsPostBack Then
                If Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
                    chkBrowser.Checked = DotNetNuke.Entities.Host.Host.EnableBrowserLanguage
                    chkUrl.Checked = DotNetNuke.Entities.Host.Host.EnableUrlLanguage
                Else
                    chkBrowser.Checked = Me.ModuleContext.PortalSettings.EnableBrowserLanguage
                    chkUrl.Checked = Me.ModuleContext.PortalSettings.EnableUrlLanguage
                End If
                chkUsePaging.Checked = CType(Me.ModuleContext.Settings("UsePaging"), Boolean)
                Dim _PageSize As Integer = 10 'default page size
                If CType(Me.ModuleContext.Settings("PageSize"), Integer) = 0 Then
                    txtPageSize.Text = _PageSize
                Else
                    txtPageSize.Text = CType(Me.ModuleContext.Settings("PageSize"), Integer)
                End If
            End If

            urlRow.Visible = Not PortalSettings.Current.ContentLocalizationEnabled

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates the settings
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	05/09/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Try
                If Page.IsValid Then
                    If Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
                        HostController.Instance.Update("EnableBrowserLanguage", chkBrowser.Checked.ToString())
                        HostController.Instance.Update("EnableUrlLanguage", chkUrl.Checked.ToString())
                    Else
                        PortalController.UpdatePortalSetting(Me.ModuleContext.PortalId, "EnableBrowserLanguage", chkBrowser.Checked.ToString())
                        PortalController.UpdatePortalSetting(Me.ModuleContext.PortalId, "EnableUrlLanguage", chkUrl.Checked.ToString())
                    End If
                    Dim modController As New ModuleController
                    modController.UpdateModuleSetting(Me.ModuleContext.ModuleId, "UsePaging", chkUsePaging.Checked)
                    modController.UpdateModuleSetting(Me.ModuleContext.ModuleId, "PageSize", txtPageSize.Text)

                    Response.Redirect(NavigateURL())
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns to main control
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	05/09/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL())
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

    End Class

End Namespace


