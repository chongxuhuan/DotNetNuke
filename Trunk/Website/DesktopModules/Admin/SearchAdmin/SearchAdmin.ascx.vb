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
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.UI.Modules

Namespace DotNetNuke.Modules.Admin.Search
    Partial Class SearchAdmin
        Inherits ModuleUserControlBase

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/16/2004 created
        '''     [cnurse]    01/10/2005 added UrlReferrer code so Cancel returns to previous page
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Not Page.IsPostBack Then
                If Me.ModuleContext.PortalSettings.ActiveTab.ParentId = ModuleContext.PortalSettings.SuperTabId Then
                    txtMaxWordLength.Text = Host.SearchMaxWordlLength
                    txtMinWordLength.Text = Host.SearchMinWordlLength
                    chkIncludeCommon.Checked = Host.SearchIncludeCommon
                    chkIncludeNumeric.Checked = Host.SearchIncludeNumeric
                Else
                    txtMaxWordLength.Text = ModuleContext.PortalSettings.SearchMaxWordlLength
                    txtMinWordLength.Text = ModuleContext.PortalSettings.SearchMinWordlLength
                    chkIncludeCommon.Checked = ModuleContext.PortalSettings.SearchIncludeCommon
                    chkIncludeNumeric.Checked = ModuleContext.PortalSettings.SearchIncludeNumeric
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdReIndex_Click runs when the ReIndex LinkButton is clicked.  It re-indexes the
        ''' site (or application if run on Host page)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/16/2004 created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdReIndex_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdReIndex.Click
            Try
                Dim se As New Services.Search.SearchEngine
                If ModuleContext.PortalSettings.ActiveTab.ParentId = ModuleContext.PortalSettings.SuperTabId Then
                    se.IndexContent()
                Else
                    se.IndexContent(ModuleContext.PortalId)
                End If

                Skin.AddModuleMessage(Me, Localization.GetString("Indexed", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Update LinkButton is clicked.
        ''' It saves the current Search Settings
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/9/2004	Modified
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try

                If ModuleContext.PortalSettings.ActiveTab.ParentId = ModuleContext.PortalSettings.SuperTabId Then
                    Dim objHostSettings As New Entities.Host.HostSettingsController
                    objHostSettings.UpdateHostSetting("MaxSearchWordLength", txtMaxWordLength.Text)
                    objHostSettings.UpdateHostSetting("MinSearchWordLength", txtMinWordLength.Text)
                    objHostSettings.UpdateHostSetting("SearchIncludeCommon", CType(IIf(chkIncludeCommon.Checked, "Y", "N"), String))
                    objHostSettings.UpdateHostSetting("SearchIncludeNumeric", CType(IIf(chkIncludeNumeric.Checked, "Y", "N"), String))

                    ' clear host settings cache
                    DataCache.ClearHostCache(False)

                Else
                    PortalController.UpdatePortalSetting(ModuleContext.PortalId, "MaxSearchWordLength", txtMaxWordLength.Text)
                    PortalController.UpdatePortalSetting(ModuleContext.PortalId, "MinSearchWordLength", txtMinWordLength.Text)
                    PortalController.UpdatePortalSetting(ModuleContext.PortalId, "SearchIncludeCommon", CType(IIf(chkIncludeCommon.Checked, "Y", "N"), String))
                    PortalController.UpdatePortalSetting(ModuleContext.PortalId, "SearchIncludeNumeric", CType(IIf(chkIncludeNumeric.Checked, "Y", "N"), String))
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class
End Namespace