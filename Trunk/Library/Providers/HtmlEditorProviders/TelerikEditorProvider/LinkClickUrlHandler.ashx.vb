Imports System.Web
Imports System.Web.Services
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Entities.Tabs

Namespace DotNetNuke.HtmlEditor.TelerikEditorProvider
    ''' <summary>
    ''' Returns a LinkClickUrl if provided a tabid and LinkUrl.
    ''' </summary>
    ''' <remarks>This uses the new BaseHttpHandler which encapsulates most common scenarios including the retrieval of AJAX request data.
    ''' See http://blog.theaccidentalgeek.com/post/2008/10/28/An-Updated-Abstract-Boilerplate-HttpHandler.aspx for more information on 
    ''' the BaseHttpHandler.
    ''' </remarks>
    Public Class LinkClickUrlHandler
        Inherits DotNetNuke.Framework.BaseHttpHandler

        Public Overrides Sub HandleRequest()
            Dim output As String

            ' This uses the new JSON Extensions in DotNetNuke.Common.Utilities.JsonExtensionsWeb
            Dim params As DialogParams = Content.FromJson(Of DialogParams)()
            Dim link As String = params.LinkUrl
            params.LinkClickUrl = link
            Dim portal As PortalSettings = PortalController.GetCurrentPortalSettings
            If (params IsNot Nothing) Then
                If params.Track Then
                    If Not params.LinkUrl.ToLower.Contains("linkclick.aspx") Then
                        If params.LinkUrl.Contains(portal.HomeDirectory) Then
                            'File linking
                            Dim fc As New DotNetNuke.Services.FileSystem.FileController
                            Dim filePath As String = params.LinkUrl.Replace(portal.HomeDirectory, "")
                            Dim linkedFile As FileInfo = fc.GetFile(filePath, PortalController.GetCurrentPortalSettings().PortalId)
                            link = String.Format("fileID={0}", linkedFile.FileId)
                        Else
                            Dim tabPath As String = params.LinkUrl.Replace("http://", "").Replace(portal.PortalAlias.HTTPAlias, "").Replace("/", "//").Replace(".aspx", "")
                            link = TabController.GetTabByTabPath(portal.PortalId, tabPath)
                            If link = Null.NullInteger Then
                                link = params.LinkUrl
                            End If
                        End If
                        params.LinkClickUrl = DotNetNuke.Common.Globals.LinkClick(link, PortalController.GetCurrentPortalSettings.ActiveTab.TabID, params.ModuleId, True)
                    End If
                    Dim objUrls As New UrlController
                    objUrls.UpdateUrl(portal.PortalId, params.LinkClickUrl, DotNetNuke.Common.Globals.GetURLType(link), params.TrackUser, True, params.ModuleId, False)
                Else
                    If params.LinkUrl.Contains("fileticket") Then
                        Dim queryString = params.LinkUrl.Split("=")
                        Dim encryptedFileId = queryString(1).Split("&")(0)
                        Dim fileID As String = UrlUtils.DecryptParameter(encryptedFileId)
                        Dim fc As New FileController
                        Dim savedFile As FileInfo = fc.GetFileById(fileID, portal.PortalId)
                        params.LinkClickUrl = String.Format("{0}{1}/{2}", portal.HomeDirectory, savedFile.Folder, savedFile.FileName).Replace("//", "/")
                    Else
                        Try
                            link = params.LinkUrl.Split(Convert.ToChar("?"))(1).Split(Convert.ToChar("&"))(0).Split(Convert.ToChar("="))(1)
                            Dim tabId As Integer
                            If Integer.TryParse(link, tabId) Then
                                Dim tc As New TabController
                                params.LinkClickUrl = tc.GetTab(tabId, portal.PortalId, True).FullUrl
                            Else
                                params.LinkClickUrl = HttpContext.Current.Server.UrlDecode(link)
                            End If
                        Catch ex As Exception
                            params.LinkClickUrl = params.LinkUrl
                        End Try
                    End If
                End If
                output = params.ToJson()
            Else
                output = (New DialogParams()).ToJson
            End If

            Response.Write(output)
        End Sub


        Public Overrides ReadOnly Property ContentMimeType As String
            Get
                'Normally we could use the ContentEncoding property, but because of an IE bug we have to ensure
                'that the UTF-8 is capitalized which requires inclusion in the mimetype property as shown here
                Return "application/json; charset=UTF-8"
            End Get
        End Property

        Public Overrides Function ValidateParameters() As Boolean
            'TODO: This should be updated to validate the Content paramater and return false if the content can't be converted to a DialogParams
            Return True
        End Function

        Public Overrides ReadOnly Property HasPermission() As Boolean
            Get
                'TODO: This should be updated to ensure the user has appropriate permissions for the passed in TabId.
                Return Context.User.Identity.IsAuthenticated
            End Get
        End Property

    End Class
End Namespace