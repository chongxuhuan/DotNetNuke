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
Imports System.Threading
Imports System.IO
Imports System.Text
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Services.Log.SiteLog

    Public Class SiteLogController

        Public Sub AddSiteLog(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal Referrer As String, ByVal URL As String, ByVal UserAgent As String, ByVal UserHostAddress As String, ByVal UserHostName As String, ByVal TabId As Integer, ByVal AffiliateId As Integer, ByVal SiteLogBuffer As Integer, ByVal SiteLogStorage As String)
            Dim objSecurity As New PortalSecurity
            Try
                If Host.PerformanceSetting = Common.Globals.PerformanceSettings.NoCaching Then
                    SiteLogBuffer = 1
                End If

                Select Case SiteLogBuffer
                    Case 0       ' logging disabled
                    Case 1       ' no buffering
                        Select Case SiteLogStorage
                            Case "D" ' database
                                DataProvider.Instance().AddSiteLog(Now(), PortalId, UserId, objSecurity.InputFilter(Referrer, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(URL, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserAgent, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserHostAddress, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserHostName, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), TabId, AffiliateId)
                            Case "F" ' file system
                                W3CExtendedLog(Now(), PortalId, UserId, objSecurity.InputFilter(Referrer, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(URL, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserAgent, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserHostAddress, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), objSecurity.InputFilter(UserHostName, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup), TabId, AffiliateId)
                        End Select
                    Case Else       ' buffered logging
                        Dim key As String = "SiteLog" & PortalId.ToString
                        Dim arrSiteLog As ArrayList = CType(DataCache.GetCache(key), ArrayList)

                        ' get buffered site log records from the cache
                        If arrSiteLog Is Nothing Then
                            arrSiteLog = New ArrayList
                            DataCache.SetCache(key, arrSiteLog)
                        End If

                        ' create new sitelog object
                        Dim objSiteLog As New SiteLogInfo
                        objSiteLog.DateTime = Now()
                        objSiteLog.PortalId = PortalId
                        objSiteLog.UserId = UserId
                        objSiteLog.Referrer = objSecurity.InputFilter(Referrer, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                        objSiteLog.URL = objSecurity.InputFilter(URL, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                        objSiteLog.UserAgent = objSecurity.InputFilter(UserAgent, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                        objSiteLog.UserHostAddress = objSecurity.InputFilter(UserHostAddress, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                        objSiteLog.UserHostName = objSecurity.InputFilter(UserHostName, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                        objSiteLog.TabId = TabId
                        objSiteLog.AffiliateId = AffiliateId

                        ' add sitelog object to cache
                        arrSiteLog.Add(objSiteLog)

                        If arrSiteLog.Count >= SiteLogBuffer Then
                            ' create the buffered sitelog object
                            Dim objBufferedSiteLog As New BufferedSiteLog
                            objBufferedSiteLog.SiteLogStorage = SiteLogStorage
                            objBufferedSiteLog.SiteLog = arrSiteLog

                            ' clear the current sitelogs from the cache
                            DataCache.RemoveCache(key)

                            ' process buffered sitelogs on a background thread
                            Dim objThread As New Thread(AddressOf objBufferedSiteLog.AddSiteLog)
                            objThread.Start()
                        End If

                End Select

            Catch ex As Exception

            End Try

        End Sub

        Public Function GetSiteLog(ByVal PortalId As Integer, ByVal PortalAlias As String, ByVal ReportType As Integer, ByVal StartDate As Date, ByVal EndDate As Date) As IDataReader

            Return DataProvider.Instance().GetSiteLog(PortalId, PortalAlias, "GetSiteLog" & ReportType.ToString, StartDate, EndDate)

        End Function

        Public Sub DeleteSiteLog(ByVal DateTime As Date, ByVal PortalId As Integer)

            DataProvider.Instance().DeleteSiteLog(DateTime, PortalId)

        End Sub

        Public Sub W3CExtendedLog(ByVal DateTime As Date, ByVal PortalId As Integer, ByVal UserId As Integer, ByVal Referrer As String, ByVal URL As String, ByVal UserAgent As String, ByVal UserHostAddress As String, ByVal UserHostName As String, ByVal TabId As Integer, ByVal AffiliateId As Integer)

            Dim objStream As StreamWriter

            ' create log file path
            Dim LogFilePath As String = ApplicationMapPath & "\Portals\" & PortalId.ToString & "\Logs\"
            Dim LogFileName As String = "ex" & Now.ToString("yyMMdd") & ".log"

            ' check if log file exists
            If Not File.Exists(LogFilePath & LogFileName) Then
                Try
                    ' create log file
                    Directory.CreateDirectory(LogFilePath)

                    ' open log file for append ( position the stream at the end of the file )
                    objStream = File.AppendText(LogFilePath & LogFileName)

                    ' add standard log file headers
                    objStream.WriteLine("#Software: Microsoft Internet Information Services 6.0")
                    objStream.WriteLine("#Version: 1.0")
                    objStream.WriteLine("#Date: " & Now.ToString("yyyy-MM-dd hh:mm:ss"))
                    objStream.WriteLine("#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) sc-status sc-substatus sc-win32-status")

                    ' close stream
                    objStream.Flush()
                    objStream.Close()
                Catch
                    ' can not create file
                End Try
            End If

            Try
                ' open log file for append ( position the stream at the end of the file )
                objStream = File.AppendText(LogFilePath & LogFileName)

                ' declare a string builder
                Dim objStringBuilder As New StringBuilder(1024)

                ' build W3C extended log item
                objStringBuilder.Append(DateTime.ToString("yyyy-MM-dd hh:mm:ss") & " ")
                objStringBuilder.Append(UserHostAddress & " ")
                objStringBuilder.Append("GET" & " ")
                objStringBuilder.Append(URL & " ")
                objStringBuilder.Append("-" & " ")
                objStringBuilder.Append("80" & " ")
                objStringBuilder.Append("-" & " ")
                objStringBuilder.Append(UserHostAddress & " ")
                objStringBuilder.Append(UserAgent.Replace(" ", "+") & " ")
                objStringBuilder.Append("200" & " ")
                objStringBuilder.Append("0" & " ")
                objStringBuilder.Append("0")

                ' write to log file
                objStream.WriteLine(objStringBuilder.ToString)

                ' close stream
                objStream.Flush()
                objStream.Close()
            Catch
                ' can not open file
            End Try

        End Sub

    End Class

End Namespace
