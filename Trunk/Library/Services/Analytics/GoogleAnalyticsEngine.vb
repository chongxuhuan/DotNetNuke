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

Imports System
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.Analytics.Config

Namespace DotNetNuke.Services.Analytics

    Public Class GoogleAnalyticsEngine
        Inherits AnalyticsEngineBase

        Public Overrides ReadOnly Property EngineName() As String
            Get
                Return "GoogleAnalytics"
            End Get
        End Property

        Public Overrides Function RenderScript(ByVal scriptTemplate As String) As String

            Dim config As AnalyticsConfiguration = Me.GetConfig()
            If config Is Nothing Then
                Return ""
            End If
 
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

            If trackingId = "" Then
                Return ""
            End If

            scriptTemplate = scriptTemplate.Replace("[TRACKING_ID]", trackingId)
            If (urlParameter <> "") Then
                scriptTemplate = scriptTemplate.Replace("[PAGE_URL]", urlParameter)
            Else
                scriptTemplate = scriptTemplate.Replace("[PAGE_URL]", "")
            End If

            scriptTemplate = scriptTemplate.Replace("[CUSTOM_SCRIPT]", RenderCustomScript(config))

            Return scriptTemplate

        End Function

    End Class

End Namespace
