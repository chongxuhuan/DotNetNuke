Imports System.Text
Imports DotNetNuke.Entities.Tabs
Imports System.Xml
Imports System.IO
Imports DotNetNuke.Security.Permissions
Imports System.Collections.Generic
Imports DotNetNuke.Entities.Portals
Imports System.Web


Namespace DotNetNuke.Services.Sitemap
    Public Class SitemapHandler
        Implements IHttpHandler

        Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

        Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
            Try
                Dim Response As HttpResponse = context.Response
                Dim ps As PortalSettings = PortalController.GetCurrentPortalSettings()

                Response.ContentType = "text/xml"
                Response.ContentEncoding = Encoding.UTF8

                Dim builder As New SitemapBuilder(ps)

                If Not context.Request.QueryString("i") Is Nothing Then
                    ' This is a request for one of the files that build the sitemapindex
                    builder.GetSitemapIndexFile(context.Request.QueryString("i"), Response.Output)
                Else
                    builder.BuildSiteMap(Response.Output)
                End If

            Catch exc As Exception
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc)
            End Try

        End Sub

    End Class
End Namespace
