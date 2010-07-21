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
Imports DotNetNuke.ComponentModel
Imports DotNetNuke.Services.Cache


Namespace DotNetNuke.Entities.Controllers


    Public Class HostController
        Inherits ComponentBase(Of IHostController, HostController)
        Implements IHostController

        Friend Sub New()

        End Sub

        Public Sub Update(ByVal config As ConfigurationSetting) Implements IHostController.Update
            Update(config, True)
        End Sub

        Public Sub Update(ByVal config As ConfigurationSetting, ByVal clearCache As Boolean) Implements IHostController.Update

            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Try
                If GetSettings.ContainsKey(config.Key) Then
                    DataProvider.Instance().UpdateHostSetting(config.Key, config.Value, config.IsSecure, UserController.GetCurrentUserInfo.UserID)
                    objEventLog.AddLog(config.Key, config.Value, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.HOST_SETTING_UPDATED)
                Else
                    DataProvider.Instance().AddHostSetting(config.Key, config.Value, config.IsSecure, UserController.GetCurrentUserInfo.UserID)
                    objEventLog.AddLog(config.Key, config.Value, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.HOST_SETTING_CREATED)
                End If
            Catch ex As Exception
                LogException(ex)

            End Try

            If (clearCache) Then
                DataCache.ClearHostCache(False)
            End If

        End Sub

        Public Sub Update(ByVal key As String, ByVal value As String, ByVal clearCache As Boolean) Implements IHostController.Update
            Update(New ConfigurationSetting() With {.Key = key, .Value = value}, clearCache)
        End Sub

        Public Sub Update(ByVal key As String, ByVal value As String) Implements IHostController.Update
            Update(key, value, True)
        End Sub

        Public Function GetSettings() As Dictionary(Of String, ConfigurationSetting) Implements IHostController.GetSettings
            Return CBO.GetCachedObject(Of Dictionary(Of String, ConfigurationSetting))(New CacheItemArgs(DataCache.HostSettingsCacheKey, _
                                                                                DataCache.HostSettingsCacheTimeOut, _
                                                                                DataCache.HostSettingsCachePriority), _
                                                              AddressOf GetSettingsDictionaryCallBack, True)
        End Function

        Public Function GetSettingsDictionary() As Dictionary(Of String, String) Implements IHostController.GetSettingsDictionary
            Return GetSettings.ToDictionary(Function(c) c.Key, Function(c) c.Value.Value)
        End Function

        Public Function GetBoolean(ByVal key As String) As Boolean Implements IHostController.GetBoolean
            Return GetBoolean(key, Null.NullBoolean)
        End Function

        Public Function GetBoolean(ByVal key As String, ByVal defaultValue As Boolean) As Boolean Implements IHostController.GetBoolean
            Requires.NotNullOrEmpty("key", key)

            Dim retValue As Boolean
            Try
                Dim setting As String = String.Empty
                If (GetSettings.ContainsKey(key)) Then
                    setting = GetSettings(key).Value
                End If

                If String.IsNullOrEmpty(setting) Then
                    retValue = defaultValue
                Else
                    retValue = (setting.ToUpperInvariant().StartsWith("Y") OrElse setting.ToUpperInvariant = "TRUE")
                End If

            Catch ex As Exception
                'we just want to trap the error as we may not be installed so there will be no Settings
            End Try
            Return retValue
        End Function

        Public Function GetDouble(ByVal key As String) As Double Implements IHostController.GetDouble
            Return GetDouble(key, Null.NullDouble)
        End Function

        Public Function GetDouble(ByVal key As String, ByVal defaultValue As Double) As Double Implements IHostController.GetDouble
            Requires.NotNullOrEmpty("key", key)

            Dim retValue As Double

            If (Not GetSettings.ContainsKey(key) OrElse Not Double.TryParse(GetSettings(key).Value, retValue)) Then
                retValue = defaultValue
            End If

            Return retValue
        End Function

        Public Function GetInteger(ByVal key As String) As Integer Implements IHostController.GetInteger
            Return GetInteger(key, Null.NullInteger)
        End Function

        Public Function GetInteger(ByVal key As String, ByVal defaultValue As Integer) As Integer Implements IHostController.GetInteger
            Requires.NotNullOrEmpty("key", key)

            Dim retValue As Integer

            If (Not GetSettings.ContainsKey(key) OrElse Not Integer.TryParse(GetSettings(key).Value, retValue)) Then
                retValue = defaultValue
            End If

            Return retValue
        End Function

        Public Function GetString(ByVal key As String) As String Implements IHostController.GetString
            Return GetString(key, String.Empty)
        End Function

        Public Function GetString(ByVal key As String, ByVal defaultValue As String) As String Implements IHostController.GetString
            Requires.NotNullOrEmpty("key", key)

            If Not GetSettings.ContainsKey(key) OrElse GetSettings(key).Value Is Nothing Then
                Return defaultValue
            End If

            Return GetSettings(key).Value
        End Function

        Private Function GetSettingsDictionaryCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim dicSettings As New Dictionary(Of String, ConfigurationSetting)
            Dim dr As IDataReader
            Try
                dr = DataProvider.Instance().GetHostSettings
                While dr.Read()
                    Dim key As String = dr.GetString(0)
                    Dim config As New ConfigurationSetting()
                    config.Key = key
                    config.IsSecure = Convert.ToBoolean(dr(2))
                    If dr.IsDBNull(1) Then
                        config.Value = String.Empty
                    Else
                        config.Value = dr.GetString(1)
                    End If

                    dicSettings.Add(key, config)
                End While
            Catch ex As Exception
                LogException(ex)
            Finally
                CBO.CloseDataReader(dr, True)
            End Try
            Return dicSettings
        End Function
    End Class

End Namespace