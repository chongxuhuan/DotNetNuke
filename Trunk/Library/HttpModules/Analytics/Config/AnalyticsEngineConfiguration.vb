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
    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.HttpModules.Analytics
    ''' Project:    HttpModules
    ''' Module:     AnalyticsEngineConfiguration
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Class definition for AnalyticsEngineConfiguration which is used to create
    ''' an AnalyticsEngineCollection
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cniknet]	05/03/2009	created
    ''' </history>
    ''' -----------------------------------------------------------------------------

    <Serializable(), XmlRoot("AnalyticsEngineConfig")> _
    Public Class AnalyticsEngineConfiguration

#Region "Private Members"

        Private _analyticsEngines As AnalyticsEngineCollection

#End Region

#Region "Public Properties"

        Public Property AnalyticsEngines() As AnalyticsEngineCollection
            Get
                Return _analyticsEngines
            End Get
            Set(ByVal Value As AnalyticsEngineCollection)
                _analyticsEngines = Value
            End Set
        End Property

#End Region

#Region "Shared Methods"

        Public Shared Function GetConfig() As AnalyticsEngineConfiguration
            Dim Config As AnalyticsEngineConfiguration = New AnalyticsEngineConfiguration()
            Config.AnalyticsEngines = New AnalyticsEngineCollection()
            Dim fileReader As FileStream = Nothing
            Dim filePath As String = ""
            Try
                Config = CType(DataCache.GetCache("AnalyticsEngineConfig"), AnalyticsEngineConfiguration)

                If (Config Is Nothing) Then
                    filePath = DotNetNuke.Common.Utilities.Config.GetPathToFile(ConfigFileType.SiteAnalytics)

                    'Create a FileStream for the Config file
                    fileReader = New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)

                    Dim doc As XPathDocument = New XPathDocument(fileReader)
                    Config = New AnalyticsEngineConfiguration()
                    Config.AnalyticsEngines = New AnalyticsEngineCollection()

                    For Each nav As XPathNavigator In doc.CreateNavigator.Select("AnalyticsEngineConfig/Engines/AnalyticsEngine")
                        Dim analyticsEngine As New AnalyticsEngine()
                        analyticsEngine.EngineType = nav.SelectSingleNode("EngineType").Value
                        analyticsEngine.ElementId = nav.SelectSingleNode("ElementId").Value
                        analyticsEngine.InjectTop = CType(nav.SelectSingleNode("InjectTop").Value, Boolean)
                        analyticsEngine.ScriptTemplate = nav.SelectSingleNode("ScriptTemplate").Value

                        Config.AnalyticsEngines.Add(analyticsEngine)
                    Next


                    If File.Exists(filePath) Then
                        ' Set back into Cache
                        DataCache.SetCache("AnalyticsEngineConfig", Config, New DNNCacheDependency(filePath))
                    End If

                End If
            Catch ex As Exception
                'log it
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.AddProperty("Analytics.AnalyticsEngineConfiguration", "GetConfig Failed")
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


#End Region

    End Class

End Namespace
