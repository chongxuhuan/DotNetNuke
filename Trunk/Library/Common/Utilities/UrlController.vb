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

Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Services.FileSystem

Namespace DotNetNuke.Common.Utilities

    Public Class UrlController

        Public Function GetUrls(ByVal PortalID As Integer) As ArrayList

            Return CBO.FillCollection(DataProvider.Instance().GetUrls(PortalID), GetType(UrlInfo))

        End Function

        Public Function GetUrl(ByVal PortalID As Integer, ByVal Url As String) As UrlInfo

            Return CType(CBO.FillObject(DataProvider.Instance().GetUrl(PortalID, Url), GetType(UrlInfo)), UrlInfo)

        End Function

        Public Function GetUrlTracking(ByVal PortalID As Integer, ByVal Url As String, ByVal ModuleId As Integer) As UrlTrackingInfo

            Return CType(CBO.FillObject(DataProvider.Instance().GetUrlTracking(PortalID, Url, ModuleId), GetType(UrlTrackingInfo)), UrlTrackingInfo)

        End Function

        Public Sub UpdateUrl(ByVal PortalID As Integer, ByVal Url As String, ByVal UrlType As String, ByVal LogActivity As Boolean, ByVal TrackClicks As Boolean, ByVal ModuleID As Integer, ByVal NewWindow As Boolean)
            UpdateUrl(PortalID, Url, UrlType, 0, Null.NullDate, Null.NullDate, LogActivity, TrackClicks, ModuleID, NewWindow)
        End Sub

        Public Sub UpdateUrl(ByVal PortalID As Integer, ByVal Url As String, ByVal UrlType As String, ByVal Clicks As Integer, ByVal LastClick As Date, ByVal CreatedDate As Date, ByVal LogActivity As Boolean, ByVal TrackClicks As Boolean, ByVal ModuleID As Integer, ByVal NewWindow As Boolean)

            If Url <> "" Then
                If UrlType = "U" Then
                    If GetUrl(PortalID, Url) Is Nothing Then
                        DataProvider.Instance().AddUrl(PortalID, Url)
                    End If
                End If

                Dim objURLTracking As UrlTrackingInfo = GetUrlTracking(PortalID, Url, ModuleID)
                If objURLTracking Is Nothing Then
                    DataProvider.Instance().AddUrlTracking(PortalID, Url, UrlType, LogActivity, TrackClicks, ModuleID, NewWindow)
                Else
                    DataProvider.Instance().UpdateUrlTracking(PortalID, Url, LogActivity, TrackClicks, ModuleID, NewWindow)
                End If
            End If

        End Sub

        Public Sub DeleteUrl(ByVal PortalID As Integer, ByVal Url As String)

            DataProvider.Instance().DeleteUrl(PortalID, Url)

        End Sub

        Public Sub UpdateUrlTracking(ByVal PortalID As Integer, ByVal Url As String, ByVal ModuleId As Integer, ByVal UserID As Integer)

            Dim UrlType As TabType = GetURLType(Url)
            If UrlType = TabType.File And Url.ToLower.StartsWith("fileid=") = False Then
                ' to handle legacy scenarios before the introduction of the FileServerHandler
                Dim objFiles As New FileController
                Url = "FileID=" & objFiles.ConvertFilePathToFileId(Url, PortalID)
            End If

            Dim objUrlTracking As UrlTrackingInfo = GetUrlTracking(PortalID, Url, ModuleId)
            If Not objUrlTracking Is Nothing Then
                If objUrlTracking.TrackClicks Then
                    DataProvider.Instance().UpdateUrlTrackingStats(PortalID, Url, ModuleId)
                    If objUrlTracking.LogActivity Then
                        If UserID = -1 Then
                            UserID = UserController.GetCurrentUserInfo.UserID
                        End If
                        DataProvider.Instance().AddUrlLog(objUrlTracking.UrlTrackingID, UserID)
                    End If
                End If
            End If

        End Sub

        Public Function GetUrlLog(ByVal PortalID As Integer, ByVal Url As String, ByVal ModuleId As Integer, ByVal StartDate As Date, ByVal EndDate As Date) As ArrayList

            Dim arrUrlLog As ArrayList = Nothing

            Dim objUrlTracking As UrlTrackingInfo = GetUrlTracking(PortalID, Url, ModuleId)
            If Not objUrlTracking Is Nothing Then
                arrUrlLog = CBO.FillCollection(DataProvider.Instance().GetUrlLog(objUrlTracking.UrlTrackingID, StartDate, EndDate), GetType(UrlLogInfo))
            End If

            Return arrUrlLog

        End Function

    End Class

End Namespace