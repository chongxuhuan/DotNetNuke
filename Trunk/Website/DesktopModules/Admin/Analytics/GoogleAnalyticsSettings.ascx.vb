'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2007
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

Imports DotNetNuke
Imports DotNetNuke.Services.Analytics.Config

Namespace DotNetNuke.Modules.Admin.Analytics

    Partial Public Class GoogleAnalyticsSettings
        Inherits Entities.Modules.PortalModuleBase

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Page.IsPostBack = False Then
                    Dim config As AnalyticsConfiguration = AnalyticsConfiguration.GetConfig("GoogleAnalytics")
                    If Not config Is Nothing Then
                        Dim trackingId As String = ""
                        Dim urlParameter As String = ""

                        For Each setting As AnalyticsSetting In config.Settings
                            Select Case setting.SettingName.ToLower
                                Case "trackingid"
                                    trackingId = setting.SettingValue
                                Case "urlparameter"
                                    urlParameter = setting.SettingValue
                            End Select
                        Next

                        txtTrackingId.Text = trackingId
                        txtUrlParameter.Text = urlParameter
                    End If
                End If

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the update button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                Dim config As New AnalyticsConfiguration
                Dim setting As New AnalyticsSetting()
                config.Settings = New AnalyticsSettingCollection()

                setting.SettingName = "TrackingId"
                setting.SettingValue = txtTrackingId.Text
                config.Settings.Add(setting)

                setting = New AnalyticsSetting()
                setting.SettingName = "UrlParameter"
                setting.SettingValue = txtUrlParameter.Text
                config.Settings.Add(setting)

                AnalyticsConfiguration.SaveConfig("GoogleAnalytics", config)

                Skins.Skin.AddModuleMessage(Me, Localization.GetString("Updated", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
