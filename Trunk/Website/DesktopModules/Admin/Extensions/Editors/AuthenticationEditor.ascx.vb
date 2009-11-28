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

Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Services.Authentication
Imports DotNetNuke.Services.Installer.Packages
Imports System.IO

Namespace DotNetNuke.Modules.Admin.Authentication

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The AuthenticationEditor.ascx control is used to edit the Authentication Properties
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class AuthenticationEditor
        Inherits PackageEditorBase

#Region "Private Members"

        Private _AuthSystem As AuthenticationInfo
        Private _SettingsControl As AuthenticationSettingsBase

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property AuthSystem() As AuthenticationInfo
            Get
                If _AuthSystem Is Nothing Then
                    _AuthSystem = AuthenticationController.GetAuthenticationServiceByPackageID(PackageID)
                End If
                Return _AuthSystem
            End Get
        End Property

        Protected Overrides ReadOnly Property EditorID() As String
            Get
                Return "AuthenticationEditor"
            End Get
        End Property

        Protected ReadOnly Property SettingsControl() As AuthenticationSettingsBase
            Get
                If _SettingsControl Is Nothing AndAlso Not String.IsNullOrEmpty(AuthSystem.SettingsControlSrc) Then
                    _SettingsControl = CType(LoadControl("~/" & AuthSystem.SettingsControlSrc), AuthenticationSettingsBase)
                End If
                Return _SettingsControl
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine Binds the Authentication System
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/15/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindAuthentication()
            If AuthSystem IsNot Nothing Then
                If AuthSystem.AuthenticationType = "DNN" Then
                    fldIsEnabled.EditMode = UI.WebControls.PropertyEditorMode.View
                End If
                ctlAuthentication.LocalResourceFile = LocalResourceFile
                ctlAuthentication.DataSource = AuthSystem
                ctlAuthentication.DataBind()

                If SettingsControl IsNot Nothing Then
                    ' set the control ID to the resource file name ( ie. controlname.ascx = controlname )
                    ' this is necessary for the Localization in PageBase
                    SettingsControl.ID = Path.GetFileNameWithoutExtension(AuthSystem.SettingsControlSrc)

                    'Add Container to Controls
                    pnlSettings.Controls.AddAt(0, SettingsControl)
                Else
                    cmdUpdate.Visible = False
                End If
            End If
        End Sub

#End Region

#Region "Public Methods"

        Public Overrides Sub Initialize()
            ctlAuthentication.Visible = IsSuperTab
            pnlSettings.Visible = Not IsSuperTab
            If IsSuperTab Then
                lblHelp.Text = Localization.GetString("HostHelp", LocalResourceFile)
            Else
                If SettingsControl Is Nothing Then
                    lblHelp.Text = Localization.GetString("NoSettings", LocalResourceFile)
                Else
                    lblHelp.Text = Localization.GetString("AdminHelp", LocalResourceFile)
                End If
            End If

            BindAuthentication()
        End Sub

        Public Overrides Sub UpdatePackage()
            If ctlAuthentication.IsValid AndAlso ctlAuthentication.IsDirty Then
                Dim authInfo As AuthenticationInfo = TryCast(ctlAuthentication.DataSource, AuthenticationInfo)
                If authInfo IsNot Nothing Then
                    AuthenticationController.UpdateAuthentication(authInfo)
                End If
            End If
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            If SettingsControl IsNot Nothing Then
                SettingsControl.UpdateSettings()
            End If
        End Sub

#End Region

    End Class

End Namespace
