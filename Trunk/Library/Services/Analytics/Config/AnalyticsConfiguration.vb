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
Imports System.IO
Imports System.Web
Imports System.Collections
Imports System.Web.Caching
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Xml.XPath
Imports DotNetNuke.Services.Cache
Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals

Namespace DotNetNuke.Services.Analytics.Config

    <Serializable(), XmlRoot("AnalyticsConfig")> _
    Public Class AnalyticsConfiguration

#Region "Private Members"

        Private _rules As AnalyticsRuleCollection
        Private _settings As AnalyticsSettingCollection

#End Region

#Region "Public Properties"

        Public Property Settings() As AnalyticsSettingCollection
            Get
                Return _settings
            End Get
            Set(ByVal Value As AnalyticsSettingCollection)
                _settings = Value
            End Set
        End Property


        Public Property Rules() As AnalyticsRuleCollection
            Get
                Return _rules
            End Get
            Set(ByVal Value As AnalyticsRuleCollection)
                _rules = Value
            End Set
        End Property

#End Region

#Region "Shared Methods"

        Public Shared Function GetConfig(ByVal analyticsEngineName As String) As AnalyticsConfiguration
            Dim cacheKey As String = analyticsEngineName + "." + PortalSettings.Current.PortalId.ToString()

            Dim Config As AnalyticsConfiguration = New AnalyticsConfiguration()
            Config.Rules = New AnalyticsRuleCollection()
            Config.Settings = New AnalyticsSettingCollection()

            Dim fileReader As FileStream = Nothing
            Dim filePath As String = ""
            Try
                Config = CType(DataCache.GetCache(cacheKey), AnalyticsConfiguration)

                If (Config Is Nothing) Then
                    filePath = PortalSettings.Current.HomeDirectoryMapPath & "\" & analyticsEngineName & ".config"

                    If Not File.Exists(filePath) Then
                        Return Nothing
                    End If

                    'Create a FileStream for the Config file
                    fileReader = New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)

                    Dim doc As XPathDocument = New XPathDocument(fileReader)
                    Config = New AnalyticsConfiguration()
                    Config.Rules = New AnalyticsRuleCollection()
                    Config.Settings = New AnalyticsSettingCollection()

                    Dim allSettings As New Hashtable()
                    For Each nav As XPathNavigator In doc.CreateNavigator.Select("AnalyticsConfig/Settings/AnalyticsSetting")
                        Dim setting As New AnalyticsSetting()
                        setting.SettingName = nav.SelectSingleNode("SettingName").Value
                        setting.SettingValue = nav.SelectSingleNode("SettingValue").Value
                        Config.Settings.Add(setting)
                    Next

                    For Each nav As XPathNavigator In doc.CreateNavigator.Select("AnalyticsConfig/Rules/AnalyticsRule")
                        Dim rule As New AnalyticsRule()
                        rule.RoleId = CType(nav.SelectSingleNode("RoleId").Value, Integer)
                        rule.TabId = CType(nav.SelectSingleNode("TabId").Value, Integer)
                        rule.Label = nav.SelectSingleNode("Label").Value
                        Config.Rules.Add(rule)
                    Next


                    If File.Exists(filePath) Then
                        ' Set back into Cache
                        DataCache.SetCache(cacheKey, Config, New DNNCacheDependency(filePath))
                    End If

                End If
            Catch ex As Exception
                'log it
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.AddProperty("Analytics.AnalyticsConfiguration", "GetConfig Failed")
                objEventLogInfo.AddProperty("FilePath", filePath)
                objEventLogInfo.AddProperty("ExceptionMessage", ex.Message)
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT.ToString
                objEventLog.AddLog(objEventLogInfo)

            Finally
                If Not fileReader Is Nothing Then
                    'Close the Reader
                    fileReader.Close()
                End If

            End Try

            Return Config

        End Function

        Public Shared Sub SaveConfig(ByVal analyticsEngineName As String, ByVal config As AnalyticsConfiguration)

            Dim cacheKey As String = analyticsEngineName + "." + PortalSettings.Current.PortalId.ToString()

            If Not config.Settings Is Nothing Then

                ' Create a new Xml Serializer
                Dim ser As XmlSerializer = New XmlSerializer(GetType(AnalyticsConfiguration))
                Dim filePath As String = ""

                'Create a FileStream for the Config file
                filePath = PortalSettings.Current.HomeDirectoryMapPath & "\" & analyticsEngineName & ".config"
                If File.Exists(filePath) Then
                    ' make sure file is not read-only
                    File.SetAttributes(filePath, FileAttributes.Normal)
                End If
                Dim fileWriter As FileStream = New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write)

                ' Open up the file to serialize
                Dim writer As StreamWriter = New StreamWriter(fileWriter)

                ' Serialize the AnalyticsConfiguration
                ser.Serialize(writer, config)

                ' Close the Writers
                writer.Close()
                fileWriter.Close()

                ' Set Cache
                DataCache.SetCache(cacheKey, config, New DNNCacheDependency(filePath))
            End If

        End Sub

#End Region

    End Class

End Namespace
