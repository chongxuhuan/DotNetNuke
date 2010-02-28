Imports System.Collections.Generic
Imports DotNetNuke.Services.Sitemap

Namespace DotNetNuke.Sitemap

    Public Class BigSitemapProvider
        Inherits SitemapProvider

        Public Overrides Function GetUrls(ByVal portalId As Integer, ByVal ps As DotNetNuke.Entities.Portals.PortalSettings, ByVal version As String) As System.Collections.Generic.List(Of SitemapUrl)

            Dim urls As New List(Of SitemapUrl)
            For i As Integer = 0 To 50
                urls.Add(GetPageUrl(i))
            Next

            Return urls
        End Function


        Private Function GetPageUrl(ByVal index As Integer) As SitemapUrl

            Dim pageUrl As New SitemapUrl
            pageUrl.Url = String.Format("http://mysite/page_{0}.aspx", index)
            pageUrl.Priority = 0.5
            pageUrl.LastModified = DateTime.Now
            pageUrl.ChangeFrequency = SitemapChangeFrequency.Daily

            Return pageUrl
        End Function
    End Class
End Namespace
