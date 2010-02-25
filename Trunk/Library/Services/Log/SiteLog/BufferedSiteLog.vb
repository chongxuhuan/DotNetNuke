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
Imports System.Threading

Namespace DotNetNuke.Services.Log.SiteLog

	Public Class BufferedSiteLog

        Public SiteLog As ArrayList
        Public SiteLogStorage As String

		Public Sub AddSiteLog()

			Try

                Dim objSiteLog As SiteLogInfo
                Dim objSiteLogs As New SiteLogController

				' iterate through buffered sitelog items and insert into database
				Dim intIndex As Integer
				For intIndex = 0 To SiteLog.Count - 1
					objSiteLog = CType(SiteLog(intIndex), SiteLogInfo)
                    Select Case SiteLogStorage
                        Case "D" ' database
                            DataProvider.Instance().AddSiteLog(objSiteLog.DateTime, objSiteLog.PortalId, objSiteLog.UserId, objSiteLog.Referrer, objSiteLog.URL, objSiteLog.UserAgent, objSiteLog.UserHostAddress, objSiteLog.UserHostName, objSiteLog.TabId, objSiteLog.AffiliateId)
                        Case "F" ' file system
                            objSiteLogs.W3CExtendedLog(objSiteLog.DateTime, objSiteLog.PortalId, objSiteLog.UserId, objSiteLog.Referrer, objSiteLog.URL, objSiteLog.UserAgent, objSiteLog.UserHostAddress, objSiteLog.UserHostName, objSiteLog.TabId, objSiteLog.AffiliateId)
                    End Select
                Next

            Catch ex As Exception

			End Try

		End Sub

	End Class

End Namespace
