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

Imports System
Imports System.Configuration
Imports System.Data
Imports System.Globalization

Namespace DotNetNuke.Entities.Host

	Public Class HostSettingsController

		Public Function GetHostSetting(ByVal SettingName As String) As IDataReader
            Return DataProvider.Instance().GetHostSetting(SettingName)
        End Function

		Public Function GetHostSettings() As IDataReader
            Return DataProvider.Instance().GetHostSettings()
        End Function

		Public Sub UpdateHostSetting(ByVal SettingName As String, ByVal SettingValue As String)
            UpdateHostSetting(SettingName, SettingValue, False, True)
		End Sub

        Public Sub UpdateHostSetting(ByVal SettingName As String, ByVal SettingValue As String, ByVal SettingIsSecure As Boolean)
            UpdateHostSetting(SettingName, SettingValue, SettingIsSecure, True)
        End Sub

        Public Sub UpdateHostSetting(ByVal SettingName As String, ByVal SettingValue As String, ByVal SettingIsSecure As Boolean, ByVal clearCache As Boolean)

            Dim dr As IDataReader = Nothing
            Try
                dr = DataProvider.Instance().GetHostSetting(SettingName)
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                If dr.Read Then
                    DataProvider.Instance().UpdateHostSetting(SettingName, SettingValue, SettingIsSecure, UserController.GetCurrentUserInfo.UserID)
                    objEventLog.AddLog(SettingName.ToString, SettingValue.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.HOST_SETTING_UPDATED)
                Else
                    DataProvider.Instance().AddHostSetting(SettingName, SettingValue, SettingIsSecure, UserController.GetCurrentUserInfo.UserID)
                    objEventLog.AddLog(SettingName.ToString, SettingValue.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.HOST_SETTING_CREATED)
                End If
            Catch ex As Exception
                LogException(ex)
            Finally
                CBO.CloseDataReader(dr, True)
            End Try

            ' clear host cache
            If clearCache Then
                DataCache.ClearHostCache(False)
            End If
        End Sub

    End Class

End Namespace
