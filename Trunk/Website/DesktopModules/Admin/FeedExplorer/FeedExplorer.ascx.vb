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
Imports System.IO
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Modules.Admin.Syndication
    Partial Class FeedExplorer
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim contentSource As String = ""
            Dim opmlUrlFile As String = ""
            Dim opmlText As String = ""
            Dim theme As String = ""

            Try
                contentSource = CType(Settings("ContentSource"), String).Trim()
                opmlUrlFile = CType(Settings("OpmlUrlFile"), String).Trim()
                opmlText = CType(Settings("OpmlText"), String).Trim()
                theme = CType(Settings("Theme"), String).Trim()
            Catch ex As Exception
                contentSource = "SolutionsExplorer"
            End Try

            Try
                If (contentSource <> "") Then
                    Select Case contentSource
                        Case "NewsExplorer"
                            Dim newsOpmlPath As String = Path.Combine(PortalSettings.HomeDirectoryMapPath, "News.opml")
                            If (Not File.Exists(newsOpmlPath)) Then
                                File.Copy(Server.MapPath("~/DesktopModules/Admin/FeedExplorer/News.opml"), newsOpmlPath)
                            End If
                            Feeds.OpmlText = ""
                            Feeds.OpmlUrl = ""
                            Feeds.OpmlFile = newsOpmlPath
                        Case "OpmlUrlFile"
                            If (opmlUrlFile <> "") Then
                                Feeds.OpmlText = ""
                                If (opmlUrlFile.StartsWith("http") Or opmlUrlFile.StartsWith("/")) Then
                                    Feeds.OpmlFile = ""
                                    Feeds.OpmlUrl = opmlUrlFile
                                Else
                                    If (File.Exists(opmlUrlFile)) Then
                                        Feeds.OpmlUrl = ""
                                        Feeds.OpmlFile = opmlUrlFile
                                    End If
                                End If
                            End If
                        Case "OpmlText"
                            If (opmlText <> "") Then
                                Feeds.OpmlFile = ""
                                Feeds.OpmlUrl = ""
                                Feeds.OpmlText = opmlText
                            End If
                    End Select
                End If

                If (theme <> "") Then Feeds.Theme = theme

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

    End Class

End Namespace
