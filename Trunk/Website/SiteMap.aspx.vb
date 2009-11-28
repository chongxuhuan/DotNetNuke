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
Imports System.Text
Imports DotNetNuke.Entities.Tabs
Imports System.Xml
Imports System.IO
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Common.Utilities

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The LinkClick Page processes links
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class SiteMap
        Inherits Framework.PageBase

        Const SITEMAP_CHANGEFREQ As String = "daily"
        Const SITEMAP_PRIORITY As String = "0.5"
        Const SITEMAP_MAXURLS As Integer = 50000

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try
                Response.ContentType = "text/xml"
                Response.ContentEncoding = Encoding.UTF8
                BuildSiteMap(Response.Output, PortalSettings.PortalId)

            Catch exc As Exception

            End Try

        End Sub

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds SiteMap
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BuildSiteMap(ByVal textWriter As TextWriter, ByVal PortalID As Integer)
            Dim settings As New XmlWriterSettings()
            settings.Indent = 2
            settings.Encoding = Encoding.UTF8
            settings.OmitXmlDeclaration = False
            Dim writer As XmlWriter = XmlWriter.Create(textWriter, settings)

            ' build header
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9")
            writer.WriteAttributeString("xmlns", "xsi", Nothing, "http://www.w3.org/2001/XMLSchema-instance")
            writer.WriteAttributeString("xsi", "schemaLocation", Nothing, "http://www.sitemaps.org/schemas/sitemap/0.9")
            'writer.WriteAttributeString("", "url", Nothing, "http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd")

            ' add urls
            Dim intURLs As Integer = 0
            Dim URL As String
            Dim objTabs As New TabController
            For Each objTab As TabInfo In objTabs.GetTabsByPortal(PortalID).Values
                If objTab.IsDeleted = False AndAlso objTab.DisableLink = False AndAlso objTab.IsVisible AndAlso objTab.TabType = TabType.Normal AndAlso ((Null.IsNull(objTab.StartDate) = True OrElse objTab.StartDate < Now) AndAlso (Null.IsNull(objTab.EndDate) = True OrElse objTab.EndDate > Now)) Then
                    ' the crawler is an anonymous user therefore the site map will only contain publicly accessible pages
                    If TabPermissionController.CanViewPage(objTab) Then
                        If intURLs < SITEMAP_MAXURLS Then
                            intURLs += 1
                            URL = objTab.FullUrl
                            If URL.ToLower.IndexOf(Request.Url.Host.ToLower) = -1 Then
                                URL = AddHTTP(Request.Url.Host) & URL
                            End If
                            BuildURL(writer, URL, objTab.SiteMapPriority)
                        End If
                    End If
                End If
            Next

            writer.WriteEndElement()
            writer.Close()
        End Sub

        Private Sub BuildURL(ByVal writer As XmlWriter, ByVal URL As String, ByVal priority As Single)
            writer.WriteStartElement("url")
            writer.WriteElementString("loc", URL)
            writer.WriteElementString("lastmod", DateTime.Now.ToString("yyyy-MM-dd"))
            writer.WriteElementString("changefreq", SITEMAP_CHANGEFREQ)
            writer.WriteElementString("priority", priority.ToString("F02", CultureInfo.InvariantCulture))

            writer.WriteEndElement()
        End Sub

#End Region

    End Class

End Namespace
