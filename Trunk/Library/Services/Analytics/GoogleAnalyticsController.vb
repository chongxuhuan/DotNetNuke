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

Imports System.IO
Imports System.Security
Imports System.Collections.Generic
Imports System.Linq
Imports DotNetNuke.Common

Namespace DotNetNuke.Services.Analytics

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Services.Analytics
    ''' Module:     GoogleAnalytics
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Controller class definition for GoogleAnalytics which handles upgrades
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''    [vnguyen]   10/08/2010   Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class GoogleAnalyticsController

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Handles module upgrades includes a new Google Analytics Asychronous script.
        ''' </summary>
        ''' <param name="Version"></param>
        ''' <remarks></remarks>
        ''' <history>
        '''    [vnguyen]   10/08/2010   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpgradeModule(ByVal Version As String)
            ' MD5 Hash value of the old synchronous script config file (from previous module versions)
            Dim TRADITIONAL_FILEHASHES() As String = {"aRUf9NsElvrpiASJHHlmZg==", "+R2k5mvFvVhWsCm4WinyAA=="}

            Select Case Version
                Case "05.06.00" 'previous module versions
                    Dim fileReader As StreamReader = GetConfigFile()

                    If fileReader IsNot Nothing Then
                        Dim fileEncoding As New System.Text.ASCIIEncoding()
                        Dim md5 As New Cryptography.MD5CryptoServiceProvider()
                        Dim currFileHashValue As String = ""

                        'calculate md5 hash of existing file
                        currFileHashValue = Convert.ToBase64String(md5.ComputeHash(fileEncoding.GetBytes(fileReader.ReadToEnd)))
                        fileReader.Close()

                        Dim result As IEnumerable(Of String) = (From h In TRADITIONAL_FILEHASHES Where h = currFileHashValue Select h)

                        'compare md5 hash
                        If result.Count > 0 Then
                            'Copy new config file from \Config
                            DotNetNuke.Common.Utilities.Config.GetPathToFile(ConfigFileType.SiteAnalytics, True) 'True causes .config to be overwritten
                        End If

                    End If
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Retrieves the Google Analytics config file, "SiteAnalytics.config".
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        '''    [vnguyen]   10/08/2010   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetConfigFile() As System.IO.StreamReader
            Dim fileReader As StreamReader = Nothing
            Dim filePath As String = ""
            Try
                filePath = ApplicationMapPath & "\SiteAnalytics.config"

                If File.Exists(filePath) Then
                    fileReader = New StreamReader(filePath)
                End If
            Catch ex As Exception
                'log it
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.AddProperty("GoogleAnalytics.UpgradeModule", "GetConfigFile Failed")
                objEventLogInfo.AddProperty("FilePath", filePath)
                objEventLogInfo.AddProperty("ExceptionMessage", ex.Message)
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString
                objEventLog.AddLog(objEventLogInfo)
                fileReader.Close()
            End Try

            Return fileReader
        End Function

    End Class

End Namespace