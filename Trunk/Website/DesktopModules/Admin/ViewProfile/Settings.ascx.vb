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
Imports DotNetNuke.UI.Utilities.ClientAPI

Namespace DotNetNuke.Modules.Admin.Users
    Partial Class ViewProfileSettings
        Inherits Entities.Modules.ModuleSettingsBase

#Region "Base Method Implementations"
        Public Overrides Sub LoadSettings()
            Try
                AddButtonConfirm(cmdLoadDefault, Localization.GetString("LoadDefault.Confirm", Me.LocalResourceFile))
                cmdLoadDefault.ToolTip = Localization.GetString("LoadDefault.Help", Me.LocalResourceFile)

                If Not Page.IsPostBack Then
                    If CType(TabModuleSettings("ProfileTemplate"), String) <> "" Then
                        txtTemplate.Text = CType(TabModuleSettings("ProfileTemplate"), String)
                    End If
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Public Overrides Sub UpdateSettings()
            Try
                Dim objModules As New Entities.Modules.ModuleController
                objModules.UpdateTabModuleSetting(TabModuleId, "ProfileTemplate", txtTemplate.Text)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

        Protected Sub cmdLoadDefault_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdLoadDefault.Click
            txtTemplate.Text = Localization.GetString("DefaultTemplate", Me.LocalResourceFile)
        End Sub

    End Class
End Namespace

