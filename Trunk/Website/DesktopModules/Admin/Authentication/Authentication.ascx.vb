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
Imports System.Collections.Generic
Imports System.IO

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Authentication
Imports DotNetNuke.UI.UserControls

Namespace DotNetNuke.Modules.Admin.Authentication

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : Authentication
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Manages the Authentication settings
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]        06/29/2007   Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Authentication
        Inherits PortalModuleBase

        Private settingControls As New List(Of AuthenticationSettingsBase)

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim authSystems As List(Of AuthenticationInfo) = AuthenticationController.GetEnabledAuthenticationServices()

            For Each authSystem As AuthenticationInfo In authSystems
                'Add a Section Header
                Dim sectionHeadControl As SectionHeadControl = CType(LoadControl("~/controls/SectionHeadControl.ascx"), SectionHeadControl)
                sectionHeadControl.IncludeRule = True
                sectionHeadControl.CssClass = "Head"

                'Create a <div> to hold the control
                Dim container As HtmlGenericControl = New HtmlGenericControl()
                container.ID = authSystem.AuthenticationType

                Dim authSettingsControl As AuthenticationSettingsBase = CType(LoadControl("~/" & authSystem.SettingsControlSrc), AuthenticationSettingsBase)

                ' set the control ID to the resource file name ( ie. controlname.ascx = controlname )
                ' this is necessary for the Localization in PageBase
                authSettingsControl.ID = Path.GetFileNameWithoutExtension(authSystem.SettingsControlSrc) + "_" + authSystem.AuthenticationType

                'Add Settings Control to Container
                container.Controls.Add(authSettingsControl)
                settingControls.Add(authSettingsControl)

                'Add Section Head Control to Container
                pnlSettings.Controls.Add(sectionHeadControl)

                'Add Container to Controls
                pnlSettings.Controls.Add(container)

                'Attach Settings Control's container to Section Head Control
                sectionHeadControl.Section = container.ID

                'Get Section Head Text from the setting controls LocalResourceFile
                authSettingsControl.LocalResourceFile = authSettingsControl.TemplateSourceDirectory & "/" & Services.Localization.Localization.LocalResourceDirectory & "/" & Path.GetFileNameWithoutExtension(authSystem.SettingsControlSrc)
                sectionHeadControl.Text = Localization.GetString("Title", authSettingsControl.LocalResourceFile)

                'Add LineBreak
                pnlSettings.Controls.Add(New LiteralControl("<br/>"))

                cmdUpdate.Visible = Me.IsEditable

            Next

        End Sub

        Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click

            For Each settingControl As AuthenticationSettingsBase In settingControls
                settingControl.UpdateSettings()
            Next

            'Validate Enabled
            Dim enabled As Boolean = False
            Dim authSystems As List(Of AuthenticationInfo) = AuthenticationController.GetEnabledAuthenticationServices()
            For Each authSystem As AuthenticationInfo In authSystems
                Dim authLoginControl As AuthenticationLoginBase = CType(LoadControl("~/" & authSystem.LoginControlSrc), AuthenticationLoginBase)

                'Check if AuthSystem is Enabled
                If authLoginControl.Enabled Then
                    enabled = True
                    Exit For
                End If
            Next

            If Not enabled Then
                'Display warning
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("NoProvidersEnabled", LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End If
        End Sub

    End Class

End Namespace

