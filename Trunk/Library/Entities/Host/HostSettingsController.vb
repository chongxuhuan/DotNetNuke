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
Imports System.Configuration
Imports System.Data
Imports System.Globalization
Imports DotNetNuke.Entities.Controllers

Namespace DotNetNuke.Entities.Host

    <Obsolete("Replaced in DotNetNuke 5.5 by HostController")> _
    Public Class HostSettingsController
        
        <Obsolete("Replaced in DotNetNuke 5.5 by HostController.GetHostSetting()")> _
        Public Function GetHostSetting(ByVal SettingName As String) As IDataReader
            Return DataProvider.Instance().GetHostSetting(SettingName)
        End Function

        <Obsolete("Replaced in DotNetNuke 5.5 by HostController.GetHostSettings()")> _
        Public Function GetHostSettings() As IDataReader
            Return DataProvider.Instance().GetHostSettings()
        End Function

        <Obsolete("Replaced in DotNetNuke 5.5 by HostController.UpdateHostSetting()")> _
        Public Sub UpdateHostSetting(ByVal SettingName As String, ByVal SettingValue As String)
            HostController.Instance.Update(New ConfigurationSetting() With {.Key = SettingName, .Value = SettingValue, .IsSecure = False})
        End Sub

        <Obsolete("Replaced in DotNetNuke 5.5 by HostController.UpdateHostSetting()")> _
        Public Sub UpdateHostSetting(ByVal SettingName As String, ByVal SettingValue As String, ByVal SettingIsSecure As Boolean)
            HostController.Instance.Update(New ConfigurationSetting() With {.Key = SettingName, .Value = SettingValue, .IsSecure = SettingIsSecure})
        End Sub

        <Obsolete("Replaced in DotNetNuke 5.5 by HostController.UpdateHostSetting()")> _
        Public Sub UpdateHostSetting(ByVal SettingName As String, ByVal SettingValue As String, ByVal SettingIsSecure As Boolean, ByVal clearCache As Boolean)
            HostController.Instance.Update(New ConfigurationSetting() With {.Key = SettingName, .Value = SettingValue, .IsSecure = SettingIsSecure})
        End Sub

    End Class

End Namespace
