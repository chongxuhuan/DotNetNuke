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

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.HtmlEditor.TelerikEditorProvider
Imports System.Web
Imports DotNetNuke.Services

Namespace DotNetNuke.HtmlEditor.TelerikEditorProvider.Dialogs

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    Partial Class RenderTemplate
        Inherits System.Web.UI.Page

#Region "Event Handlers"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try
                Dim renderUrl As String = Request.QueryString("rurl")

                If (Not String.IsNullOrEmpty(renderUrl)) Then
                    Dim fileContents As String = String.Empty
                    Dim fileCtrl As FileSystem.FileController = New FileSystem.FileController()
                    Dim fileInfo As FileSystem.FileInfo = Nothing
                    Dim portalID As Integer = PortalController.GetCurrentPortalSettings().PortalId

                    If (renderUrl.ToLower().Contains("linkclick.aspx") AndAlso renderUrl.ToLower().Contains("fileticket")) Then
                        'File Ticket
                        Dim fileID As Integer = GetFileIDFromURL(renderUrl)

                        If (fileID > -1) Then
                            fileInfo = fileCtrl.GetFileById(fileID, portalID)
                        End If
                    Else
                        'File URL
                        Dim dbPath As String = FileSystemValidation.ToDBPath(renderUrl)
                        Dim fileName As String = System.IO.Path.GetFileName(renderUrl)
                        Dim cssUrl As String = System.IO.Path.ChangeExtension(renderUrl, ".css")

                        If (Not String.IsNullOrEmpty(fileName)) Then
                            Dim dnnFolder As FileSystem.FolderInfo = GetDNNFolder(dbPath)

                            If (Not IsNothing(dnnFolder)) Then
                                fileInfo = fileCtrl.GetFile(fileName, portalID, dnnFolder.FolderID)
                            End If

                            ' Add the CSS file with a matching name to the TemplateCSS HtmlLink Control 
                            If (Not IsNothing(fileInfo)) Then
                                TemplateCSS.Href = cssUrl
                            End If
                        End If
                    End If

                    If (Not IsNothing(fileInfo)) Then
                        If (CanViewFile(fileInfo.Folder) AndAlso fileInfo.Extension.ToLower() = "htmtemplate") Then
                            Dim fileBytes As Byte() = FileSystemUtils.GetFileContent(fileInfo)
                            fileContents = System.Text.Encoding.ASCII.GetString(fileBytes)
                        End If
                    End If

                    If (Not String.IsNullOrEmpty(fileContents)) Then
                        Content.Text = Server.HtmlEncode(fileContents)
                    End If
                End If
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.LogException(ex)
                Content.Text = String.Empty
            End Try
        End Sub

#End Region

#Region "Methods"

        Private Function GetFileIDFromURL(ByVal url As String) As Integer
            Dim returnValue As Integer = -1
            'add http
            If (Not url.ToLower().StartsWith("http")) Then
                If (url.ToLower().StartsWith("/")) Then
                    url = "http:/" + url
                Else
                    url = "http://" + url
                End If
            End If

            Dim u As Uri = New Uri(url)

            If (Not IsNothing(u) AndAlso Not IsNothing(u.Query)) Then
                Dim params As NameValueCollection = HttpUtility.ParseQueryString(u.Query)

                If (Not IsNothing(params) AndAlso params.Count > 0) Then
                    Dim fileTicket As String = params.Get("fileticket")

                    If (Not String.IsNullOrEmpty(fileTicket)) Then
                        Dim strFileID = UrlUtils.DecryptParameter(fileTicket)

                        Try
                            returnValue = Integer.Parse(strFileID)
                        Catch ex As Exception
                            returnValue = -1
                        End Try
                    End If
                End If
            End If

            Return returnValue
        End Function

        Protected Function CanViewFile(ByVal dbPath As String) As Boolean
            Return DotNetNuke.Security.Permissions.FolderPermissionController.CanViewFolder(GetDNNFolder(dbPath))
        End Function

        Private Function GetDNNFolder(ByVal dbPath As String) As DotNetNuke.Services.FileSystem.FolderInfo
            Return New DotNetNuke.Services.FileSystem.FolderController().GetFolder(PortalController.GetCurrentPortalSettings().PortalId, dbPath, False)
        End Function

        Private ReadOnly Property DNNHomeDirectory() As String
            Get
                'todo: host directory
                Dim homeDir As String = PortalController.GetCurrentPortalSettings().HomeDirectory
                homeDir = homeDir.Replace("\", "/")

                If (homeDir.EndsWith("/")) Then
                    homeDir = homeDir.Remove(homeDir.Length - 1, 1)
                End If

                Return homeDir
            End Get
        End Property

#End Region

    End Class

End Namespace
