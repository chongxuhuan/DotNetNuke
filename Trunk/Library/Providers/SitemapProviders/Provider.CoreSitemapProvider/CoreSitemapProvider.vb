Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Security.Permissions
Imports System.Web
Imports DotNetNuke.Services.Localization
Imports System.Collections.Generic
Imports DotNetNuke.Services.Sitemap
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.SitemapProviders

    Public Class CoreSitemapProvider
        Inherits DotNetNuke.Services.Sitemap.SitemapProvider

        Private useLevelBasedPagePriority As Boolean
        Private minPagePriority As Single
        Private includeHiddenPages As Boolean
        Private ps As PortalSettings

        ''' <summary>
        ''' Includes page urls on the sitemap
        ''' </summary>
        ''' <remarks>
        ''' Pages that are included:
        ''' - are not deleted
        ''' - are not disabled
        ''' - are normal pages (not links,...)
        ''' - are visible (based on date and permissions)
        ''' </remarks>
        Public Overrides Function GetUrls(ByVal portalId As Integer, ByVal ps As PortalSettings, ByVal version As String) As List(Of SitemapUrl)

            Dim objTabs As New TabController
            Dim pageUrl As SitemapUrl
            Dim urls As New List(Of SitemapUrl)

            useLevelBasedPagePriority = Boolean.Parse(PortalController.GetPortalSetting("SitemapLevelMode", portalId, "False"))
            minPagePriority = Single.Parse(PortalController.GetPortalSetting("SitemapMinPriority", portalId, "0.1"), Globalization.NumberFormatInfo.InvariantInfo())
            includeHiddenPages = Boolean.Parse(PortalController.GetPortalSetting("SitemapIncludeHidden", portalId, "True"))

            Me.ps = ps

            For Each objTab As TabInfo In objTabs.GetTabsByPortal(portalId).Values
                If Not objTab.IsDeleted AndAlso _
                   Not objTab.DisableLink AndAlso _
                   objTab.TabType = TabType.Normal AndAlso _
                   (Null.IsNull(objTab.StartDate) OrElse objTab.StartDate < Now) AndAlso _
                   (Null.IsNull(objTab.EndDate) OrElse objTab.EndDate > Now) AndAlso _
                   IsTabPublic(objTab.TabPermissions) Then

                    If includeHiddenPages Or objTab.IsVisible Then
                        For Each languageCode As String In LocaleController.Instance().GetLocales(portalId).Keys
                            pageUrl = GetPageUrl(objTab, languageCode)
                            urls.Add(pageUrl)
                        Next
                    End If
                End If
            Next

            Return urls
        End Function

        ''' <summary>
        ''' Return the sitemap url node for the page
        ''' </summary>
        ''' <param name="objTab">The page being indexed</param>
        ''' <returns>A SitemapUrl object for the current page</returns>
        ''' <remarks></remarks>
        Private Function GetPageUrl(ByVal objTab As TabInfo, ByVal language As String) As SitemapUrl

            Dim pageUrl As New SitemapUrl
            pageUrl.Url = DotNetNuke.Common.Globals.NavigateURL(objTab.TabID, objTab.IsSuperTab, ps, "", language)

            If pageUrl.Url.ToLower.IndexOf(ps.PortalAlias.HTTPAlias.ToLower()) = -1 Then
                ' code to fix a bug in dnn5.1.2 for navigateurl
                If Not HttpContext.Current Is Nothing Then
                    pageUrl.Url = AddHTTP(HttpContext.Current.Request.Url.Host + pageUrl.Url)
                Else
                    ' try to use the portalalias
                    pageUrl.Url = AddHTTP(ps.PortalAlias.HTTPAlias.ToLower()) + pageUrl.Url
                End If
            End If

            pageUrl.Priority = GetPriority(objTab)

            pageUrl.LastModified = objTab.LastModifiedOnDate
            Dim modCtrl As New ModuleController
            For Each m As ModuleInfo In modCtrl.GetTabModules(objTab.TabID).Values
                If m.LastModifiedOnDate > objTab.LastModifiedOnDate Then
                    pageUrl.LastModified = m.LastModifiedOnDate
                End If
            Next

            pageUrl.ChangeFrequency = SitemapChangeFrequency.Daily

            Return pageUrl
        End Function

        ''' <summary>
        ''' When page level priority is used, the priority for each page will be computed from 
        ''' the hierarchy level of the page. 
        ''' Top level pages will have a value of 1, second level 0.9, third level 0.8, ... 
        ''' </summary>
        ''' <param name="objTab">The page being indexed</param>
        ''' <returns>The priority assigned to the page</returns>
        ''' <remarks></remarks>
        Protected Function GetPriority(ByVal objTab As TabInfo) As Single

            Dim priority As Single = objTab.SiteMapPriority

            If useLevelBasedPagePriority Then
                If objTab.Level >= 9 Then
                    priority = 0.1
                Else
                    priority = CSng(1 - (objTab.Level * 0.1))
                End If

                If priority < minPagePriority Then
                    priority = minPagePriority
                End If
            End If

            Return priority
        End Function

#Region "Security Check"
        Public Overridable Function IsTabPublic(ByVal objTabPermissions As DotNetNuke.Security.Permissions.TabPermissionCollection) As Boolean
            Dim roles As String = objTabPermissions.ToString("VIEW")
            Dim hasPublicRole As Boolean = False

            If Not roles Is Nothing Then

                ' permissions strings are encoded with Deny permissions at the beginning and Grant permissions at the end for optimal performance
                For Each role As String In roles.Split(New Char() {";"c})
                    If Not String.IsNullOrEmpty(role) Then
                        ' Deny permission
                        If role.StartsWith("!") Then
                            Dim denyRole As String = role.Replace("!", "")
                            If (denyRole = glbRoleUnauthUserName OrElse _
                                denyRole = glbRoleAllUsersName) Then
                                hasPublicRole = False
                                Exit For
                            End If
                        Else ' Grant permission
                            If (role = glbRoleUnauthUserName OrElse _
                                role = glbRoleAllUsersName) Then
                                hasPublicRole = True
                                Exit For
                            End If
                        End If
                    End If
                Next

            End If

            Return hasPublicRole
        End Function
#End Region

    End Class
End Namespace
