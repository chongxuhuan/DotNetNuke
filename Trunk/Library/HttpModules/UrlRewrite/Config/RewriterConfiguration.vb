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
Imports System.IO
Imports System.Web
Imports System.Web.Caching
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Xml.XPath
Imports DotNetNuke.Services.Cache
Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.HttpModules.Config

    <Serializable(), XmlRoot("RewriterConfig")> _
    Public Class RewriterConfiguration

#Region "Private Members"

        Private _rules As RewriterRuleCollection

#End Region

#Region "Public Properties"

        Public Property Rules() As RewriterRuleCollection
            Get
                Return _rules
            End Get
            Set(ByVal Value As RewriterRuleCollection)
                _rules = Value
            End Set
        End Property

#End Region

#Region "Shared Methods"

        Public Shared Function GetConfig() As RewriterConfiguration
            Dim Config As RewriterConfiguration = New RewriterConfiguration()
            Config.Rules = New RewriterRuleCollection()
            Dim fileReader As FileStream = Nothing
            Dim filePath As String = ""
            Try
                Config = CType(DataCache.GetCache("RewriterConfig"), RewriterConfiguration)

                If (Config Is Nothing) Then
                    filePath = DotNetNuke.Common.Utilities.Config.GetPathToFile(ConfigFileType.SiteUrls)

                    'Create a FileStream for the Config file
                    fileReader = New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)

                    Dim doc As XPathDocument = New XPathDocument(fileReader)
                    Config = New RewriterConfiguration()
                    Config.Rules = New RewriterRuleCollection()

                    For Each nav As XPathNavigator In doc.CreateNavigator.Select("RewriterConfig/Rules/RewriterRule")
                        Dim rule As New RewriterRule()
                        rule.LookFor = nav.SelectSingleNode("LookFor").Value
                        rule.SendTo = nav.SelectSingleNode("SendTo").Value
                        Config.Rules.Add(rule)
                    Next


                    If File.Exists(filePath) Then
                        ' Set back into Cache
                        DataCache.SetCache("RewriterConfig", Config, New DNNCacheDependency(filePath))
                    End If

                End If
            Catch ex As Exception
                'log it
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.AddProperty("UrlRewriter.RewriterConfiguration", "GetConfig Failed")
                objEventLogInfo.AddProperty("FilePath", filePath)
                objEventLogInfo.AddProperty("ExceptionMessage", ex.Message)
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString
                objEventLog.AddLog(objEventLogInfo)

            Finally
                If Not fileReader Is Nothing Then
                    'Close the Reader
                    fileReader.Close()
                End If

            End Try

            Return Config

        End Function

        Public Shared Sub SaveConfig(ByVal rules As RewriterRuleCollection)

            If Not rules Is Nothing Then
                Dim config As New RewriterConfiguration
                config.Rules = rules

                ' Create a new Xml Serializer
                Dim ser As XmlSerializer = New XmlSerializer(GetType(RewriterConfiguration))

                'Create a FileStream for the Config file
                Dim filePath As String = ApplicationMapPath & "\SiteUrls.config"
                If File.Exists(filePath) Then
                    ' make sure file is not read-only
                    File.SetAttributes(filePath, FileAttributes.Normal)
                End If
                Dim fileWriter As FileStream = New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write)


                ' Open up the file to serialize
                Dim writer As StreamWriter = New StreamWriter(fileWriter)

                ' Serialize the RewriterConfiguration
                ser.Serialize(writer, config)

                ' Close the Writers
                writer.Close()
                fileWriter.Close()

                ' Set Cache
                DataCache.SetCache("RewriterConfig", config, New DNNCacheDependency(filePath))
            End If

        End Sub

#End Region

    End Class

End Namespace
