Imports System
Imports System.Web
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Search
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Services.Syndication
    Public Class RssHandler : Inherits SyndicationHandlerBase

        ''' <summary>
        ''' This method
        ''' </summary>
        ''' <param name="channelName"></param>
        ''' <param name="userName"></param>
        ''' <remarks></remarks>
        Protected Overrides Sub PopulateChannel(ByVal channelName As String, ByVal userName As String)
            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo

            If Request Is Nothing OrElse Settings Is Nothing OrElse Settings.ActiveTab Is Nothing OrElse ModuleId = Null.NullInteger Then
                Exit Sub
            End If
            Channel("title") = Settings.PortalName
            Channel("link") = AddHTTP(GetDomainName(Request))
            If Settings.Description <> "" Then
                Channel("description") = Settings.Description
            Else
                Channel("description") = Settings.PortalName
            End If
            Channel("language") = Settings.DefaultLanguage
            Channel("copyright") = Settings.FooterText
            Channel("webMaster") = Settings.Email

            Dim searchResults As SearchResultsInfoCollection = Nothing
            Try
                searchResults = SearchDataStoreProvider.Instance.GetSearchItems(Settings.PortalId, TabId, ModuleId)
            Catch ex As Exception
                LogException(ex)
            End Try

            If searchResults IsNot Nothing Then
                For Each objResult As SearchResultsInfo In searchResults
                    If TabPermissionController.CanViewPage() Then
                        If Settings.ActiveTab.StartDate < Now AndAlso Settings.ActiveTab.EndDate > Now Then
                            objModule = objModules.GetModule(objResult.ModuleId, objResult.TabId)
                            If objModule IsNot Nothing AndAlso objModule.DisplaySyndicate = True AndAlso objModule.IsDeleted = False Then
                                If ModulePermissionController.CanViewModule(objModule) Then
                                    If CType(IIf(objModule.StartDate = Null.NullDate, Date.MinValue, objModule.StartDate), Date) < Now AndAlso CType(IIf(objModule.EndDate = Null.NullDate, Date.MaxValue, objModule.EndDate), Date) > Now Then
                                        Channel.Items.Add(GetRssItem(objResult))
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next
            End If

        End Sub

        ''' <summary>
        ''' Creates an RSS Item
        ''' </summary>
        ''' <param name="SearchItem"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetRssItem(ByVal SearchItem As SearchResultsInfo) As GenericRssElement
            Dim item As GenericRssElement = New GenericRssElement()
            Dim URL As String = NavigateURL(SearchItem.TabId)
            If URL.ToLower.IndexOf(HttpContext.Current.Request.Url.Host.ToLower) = -1 Then
                URL = AddHTTP(HttpContext.Current.Request.Url.Host) & URL
            End If

            item("title") = SearchItem.Title
            item("description") = SearchItem.Description
            'TODO:  JMB: We need to figure out how to persist the dc prefix in the XML output.  See the Render method below.
            'item("dc:creator") = SearchItem.AuthorName
            item("pubDate") = SearchItem.PubDate.ToUniversalTime.ToString("r")

            If Not String.IsNullOrEmpty(SearchItem.Guid) Then
                If URL.Contains("?") Then
                    URL += "&" & SearchItem.Guid
                Else
                    URL += "?" & SearchItem.Guid
                End If
            End If
            item("link") = URL
            item("guid") = URL
            Return item
        End Function

        ''' <summary>
        ''' The PreRender event is used to set the Caching Policy for the Feed.  This mimics the behavior from the 
        ''' OutputCache directive in the old Rss.aspx file.  @OutputCache Duration="60" VaryByParam="moduleid" 
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub RssHandler_PreRender(ByVal source As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(60))
            Context.Response.Cache.SetCacheability(HttpCacheability.Public)
            Context.Response.Cache.VaryByParams("moduleid") = True
        End Sub

    End Class
End Namespace