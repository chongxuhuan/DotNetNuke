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
Option Strict On
Option Explicit On 

Imports System
Imports System.Web
Imports DotNetNuke
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Services.FileSystem

    Public Class FileServerHandler
        Implements IHttpHandler

        Public Sub New()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This handler handles requests for LinkClick.aspx, but only those specifc
        ''' to file serving
        ''' </summary>
        ''' <param name="context">System.Web.HttpContext)</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cpaterra]	4/19/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest

            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            Dim TabId As Integer = -1
            Dim ModuleId As Integer = -1
            Try
                ' get TabId
                If Not IsNothing(context.Request.QueryString("tabid")) Then
                    TabId = Int32.Parse(context.Request.QueryString("tabid"))
                End If

                ' get ModuleId
                If Not IsNothing(context.Request.QueryString("mid")) Then
                    ModuleId = Int32.Parse(context.Request.QueryString("mid"))
                End If
            Catch ex As Exception
                'The TabId or ModuleId are incorrectly formatted (potential DOS)
                Throw New HttpException(404, "Not Found")
            End Try

            ' get Language
            Dim Language As String = _portalSettings.DefaultLanguage
            If Not context.Request.QueryString("language") Is Nothing Then
                Language = context.Request.QueryString("language")
            Else
                If Not context.Request.Cookies("language") Is Nothing Then
                    Language = context.Request.Cookies("language").Value
                End If
            End If
            If Localization.Localization.LocaleIsEnabled(Language) Then
                Threading.Thread.CurrentThread.CurrentCulture = New CultureInfo(Language)
                Localization.Localization.SetLanguage(Language)
            End If
            ' get the URL
            Dim URL As String = ""
            Dim blnClientCache As Boolean = True
            Dim blnForceDownload As Boolean = False

            If Not context.Request.QueryString("fileticket") Is Nothing Then
                URL = "FileID=" & UrlUtils.DecryptParameter(context.Request.QueryString("fileticket"))
            End If
            If Not context.Request.QueryString("userticket") Is Nothing Then
                URL = "UserId=" & UrlUtils.DecryptParameter(context.Request.QueryString("userticket"))
            End If
            If Not context.Request.QueryString("link") Is Nothing Then
                URL = context.Request.QueryString("link")
                If URL.ToLowerInvariant.StartsWith("fileid=") Then
                    URL = "" ' restrict direct access by FileID
                End If
            End If
            If URL <> "" Then

                ' update clicks, this must be done first, because the url tracker works with unmodified urls, like tabid, fileid etc
                Dim objUrls As New UrlController
                objUrls.UpdateUrlTracking(_portalSettings.PortalId, URL, ModuleId, -1)

                Dim UrlType As TabType = GetURLType(URL)

                If UrlType <> TabType.File Then
                    URL = Common.Globals.LinkClick(URL, TabId, ModuleId, False)
                End If

                If UrlType = TabType.File And URL.ToLowerInvariant.StartsWith("fileid=") = False Then
                    ' to handle legacy scenarios before the introduction of the FileServerHandler
                    Dim objFiles As New FileController
                    URL = "FileID=" & objFiles.ConvertFilePathToFileId(URL, _portalSettings.PortalId)
                End If

                ' get optional parameters
                If Not context.Request.QueryString("clientcache") Is Nothing Then
                    blnClientCache = Boolean.Parse(context.Request.QueryString("clientcache"))
                End If

                If (Not context.Request.QueryString("forcedownload") Is Nothing) Or (Not context.Request.QueryString("contenttype") Is Nothing) Then
                    blnForceDownload = Boolean.Parse(context.Request.QueryString("forcedownload"))
                End If

                ' clear the current response
                context.Response.Clear()

                Try
                    Select Case UrlType
                        Case TabType.File
                            ' serve the file
                            If TabId = Null.NullInteger Then
                                If Not FileSystemUtils.DownloadFile(_portalSettings.PortalId, Integer.Parse(UrlUtils.GetParameterValue(URL)), blnClientCache, blnForceDownload) Then
                                    'context.Response.Write(Services.Localization.Localization.GetString("FilePermission.Error"))
                                    Throw New HttpException(404, "Not Found")
                                End If
                            Else
                                If Not FileSystemUtils.DownloadFile(_portalSettings, Integer.Parse(UrlUtils.GetParameterValue(URL)), blnClientCache, blnForceDownload) Then
                                    'context.Response.Write(Services.Localization.Localization.GetString("FilePermission.Error"))
                                    Throw New HttpException(404, "Not Found")
                                End If
                            End If
                        Case TabType.Url
                            ' prevent phishing by verifying that URL exists in URLs table for Portal
                            If Not objUrls.GetUrl(_portalSettings.PortalId, URL) Is Nothing Then
                                ' redirect to URL
                                context.Response.Redirect(URL, True)
                            End If
                        Case Else
                            ' redirect to URL
                            context.Response.Redirect(URL, True)
                    End Select
                Catch ex As Exception
                    Throw New HttpException(404, "Not Found")
                End Try
            Else
                Throw New HttpException(404, "Not Found")
            End If

        End Sub

        Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

    End Class

End Namespace