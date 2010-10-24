Imports System.Web
Imports System.Web.Services
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Services.Localization

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

        Dim _portalAliasController As New PortalAliasController
        Dim _urlController As New UrlController
        Dim _fileController As New FileController

#Region "Private Functions"

        Private Function GetLinkClickURL(ByRef params As DialogParams, ByRef link As String) As String
            link = GetLinkUrl(params, link)
            Return "http://" + Me.Context.Request.Url.Host + DotNetNuke.Common.Globals.LinkClick(link, params.TabId, params.ModuleId, True, False, params.PortalId, params.EnableUrlLanguage, params.PortalGuid)

        End Function

        Private Function GetLinkUrl(ByRef params As DialogParams, ByVal link As String) As String
            Dim aliasList As ArrayList = _portalAliasController.GetPortalAliasArrayByPortalID(params.PortalId)

            If params.LinkUrl.Contains(params.HomeDirectory) Then
                Dim filePath As String = params.LinkUrl.Substring(params.LinkUrl.IndexOf(params.HomeDirectory)).Replace(params.HomeDirectory, "")
                Dim linkedFileId As String = _fileController.ConvertFilePathToFileId(filePath, params.PortalId)
                link = String.Format("fileID={0}", linkedFileId)
            Else
                For Each portalAlias As PortalAliasInfo In aliasList
                    params.LinkUrl = params.LinkUrl.Replace(portalAlias.HTTPAlias, "")
                Next

                Dim tabPath As String = params.LinkUrl.Replace("http://", "").Replace("/", "//").Replace(".aspx", "")
                Dim cultureCode As String = Localization.SystemLocale

                'Try HumanFriendlyUrl TabPath
                link = TabController.GetTabByTabPath(params.PortalId, tabPath, cultureCode)

                If link = Null.NullInteger Then
                    'Try getting the tabId from the querystring
                    Dim arrParams As String() = params.LinkUrl.Split("/"c)
                    For i As Integer = 0 To arrParams.Length - 1
                        If arrParams(i).ToLowerInvariant() = "tabid" Then
                            link = arrParams(i + 1)
                            Exit For
                        End If
                    Next
                    If link = Null.NullInteger Then
                        link = params.LinkUrl
                    End If
                End If

            End If

            Return link

        End Function

        Private Function GetURLType(ByVal tabType As TabType) As String
            Select Case tabType
                Case Entities.Tabs.TabType.File
                    Return "F"
                Case Entities.Tabs.TabType.Member
                    Return "M"
                Case Entities.Tabs.TabType.Normal, Entities.Tabs.TabType.Tab
                    Return "T"
                Case Else
                    Return "U"
            End Select
        End Function

        Private Function GetUrlLoggingInfo(ByVal urlLog As ArrayList) As String
            Dim fullTableContent As New StringBuilder
            Dim maxCount As Integer = IIf(urlLog.Count > 100, 100, urlLog.Count)

            If (urlLog.Count > 100) Then
                fullTableContent.Append("<span>Your search returned <strong>" + urlLog.Count.ToString() + "</strong> results. Showing only the first 100 records ordered by date.</span><br /><br />")
            End If

            fullTableContent.Append("<table><tr><td>Date</td><td>User</td></tr>")

            For x = 0 To maxCount - 1
                Dim log As UrlLogInfo = urlLog(x)
                fullTableContent.Append("<tr><td>" + log.ClickDate.ToString() + "</td><td>" + log.FullName + "</td></tr>")
            Next

            fullTableContent.Append("</table>")

            Return fullTableContent.ToString()
        End Function

#End Region

#Region "Public Methods"

        Public Overrides Sub HandleRequest()

            Dim output As String
            Dim params As DialogParams = Content.FromJson(Of DialogParams)()             ' This uses the new JSON Extensions in DotNetNuke.Common.Utilities.JsonExtensionsWeb

            Dim link As String = params.LinkUrl
            params.LinkClickUrl = link

            If (params IsNot Nothing) Then

                If (Not params.LinkAction = "GetLinkInfo") Then
                    If params.Track Then
                        params.LinkClickUrl = GetLinkClickURL(params, params.LinkUrl)
                        Dim linkTrackingInfo As UrlTrackingInfo = _urlController.GetUrlTracking(params.PortalId, params.LinkUrl, params.ModuleId)

                        If (Not linkTrackingInfo Is Nothing) Then
                            params.Track = linkTrackingInfo.TrackClicks
                            params.TrackUser = linkTrackingInfo.LogActivity
                            params.DateCreated = linkTrackingInfo.CreatedDate.ToString()
                            params.LastClick = linkTrackingInfo.LastClick.ToString()
                            params.Clicks = linkTrackingInfo.Clicks
                        Else
                            params.Track = False
                            params.TrackUser = False
                        End If
                        params.LinkUrl = link

                    End If
                End If

                Select Case params.LinkAction
                    Case "GetLoggingInfo" 'also meant for the tracking tab but this is to retrieve the user information
                        Dim logStartDate As Date
                        Dim logEndDate As Date

                        If Not DateTime.TryParse(params.LogStartDate, logStartDate) Then Exit Select
                        If Not DateTime.TryParse(params.LogEndDate, logEndDate) Then Exit Select

                        Dim _urlController As New UrlController
                        Dim urlLog As ArrayList = _urlController.GetUrlLog(params.PortalId, GetLinkUrl(params, params.LinkUrl), params.ModuleId, logStartDate, logEndDate)

                        If (Not urlLog Is Nothing) Then
                            params.TrackingLog = GetUrlLoggingInfo(urlLog)
                        End If

                    Case "GetLinkInfo"
                        If params.Track Then
                            'this section is for when the user clicks ok in the dialog box, we actually create a record for the linkclick urls.
                            If Not params.LinkUrl.ToLower.Contains("linkclick.aspx") Then
                                params.LinkClickUrl = GetLinkClickURL(params, link)
                            End If

                            _urlController.UpdateUrl(params.PortalId, link, GetURLType(DotNetNuke.Common.Globals.GetURLType(link)), params.TrackUser, True, params.ModuleId, False)

                        Else
                            'this section is meant for retrieving/displaying the original links and determining if the links are being tracked(making sure the track checkbox properly checked)
                            Dim linkTrackingInfo As UrlTrackingInfo

                            If params.LinkUrl.Contains("fileticket") Then
                                Dim queryString = params.LinkUrl.Split("=")
                                Dim encryptedFileId = queryString(1).Split("&")(0)

                                Dim fileID As String = UrlUtils.DecryptParameter(encryptedFileId, params.PortalGuid)
                                Dim savedFile As FileInfo = _fileController.GetFileById(fileID, params.PortalId)

                                linkTrackingInfo = _urlController.GetUrlTracking(params.PortalId, String.Format("fileID={0}", fileID), params.ModuleId)
                                params.LinkClickUrl = String.Format("{0}{1}{2}{3}/{4}", "http://", Me.Context.Request.Url.Host, params.HomeDirectory, savedFile.Folder, savedFile.FileName).Replace("//", "/")
                            Else
                                Try
                                    link = params.LinkUrl.Split(Convert.ToChar("?"))(1).Split(Convert.ToChar("&"))(0).Split(Convert.ToChar("="))(1)

                                    Dim tabId As Integer
                                    If Integer.TryParse(link, tabId) Then 'if it's a tabid get the tab path
                                        Dim _tabController As New TabController
                                        params.LinkClickUrl = _tabController.GetTab(tabId, params.PortalId, True).FullUrl
                                        linkTrackingInfo = _urlController.GetUrlTracking(params.PortalId, tabId, params.ModuleId)
                                    Else
                                        params.LinkClickUrl = HttpContext.Current.Server.UrlDecode(link) 'get the actual link
                                        linkTrackingInfo = _urlController.GetUrlTracking(params.PortalId, params.LinkClickUrl, params.ModuleId)
                                    End If

                                Catch ex As Exception
                                    params.LinkClickUrl = params.LinkUrl
                                End Try
                            End If

                            If linkTrackingInfo Is Nothing Then
                                params.Track = False
                                params.TrackUser = False
                            Else
                                params.Track = linkTrackingInfo.TrackClicks
                                params.TrackUser = linkTrackingInfo.LogActivity
                            End If

                        End If
                End Select
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

#End Region

    End Class
End Namespace