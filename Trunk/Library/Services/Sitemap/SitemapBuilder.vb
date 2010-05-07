Imports System.Collections.Generic
Imports System.Xml
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports DotNetNuke.ComponentModel
Imports DotNetNuke
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions.Exceptions
Imports System.Web.Configuration
Imports System.Configuration.Provider
Imports System.Web

Namespace DotNetNuke.Services.Sitemap

    Public Class SitemapBuilder

        Private Const SITEMAP_MAXURLS As Integer = 50000
        Private Const SITEMAP_VERSION As String = "0.9"

        Private PortalSettings As PortalSettings
        Private writer As XmlWriter

        ''' <summary>
        ''' Creates an instance of the sitemap builder class
        ''' </summary>
        ''' <param name="ps">Current PortalSettings for the portal being processed</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ps As PortalSettings)

            Me.PortalSettings = ps

            LoadProviders()

        End Sub

#Region "Sitemap Building"
        ''' <summary>
        ''' Builds the complete portal sitemap
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub BuildSiteMap(ByVal output As TextWriter)

            Dim cacheDays As Integer = Integer.Parse(PortalController.GetPortalSetting("SitemapCacheDays", PortalSettings.PortalId, "1"))
            Dim cached As Boolean = cacheDays > 0

            If cached AndAlso CacheIsValid() Then
                WriteSitemapFileToOutput("sitemap.xml", output)
                Return
            End If

            Dim allUrls As New List(Of SitemapUrl)

            ' excluded urls by priority
            Dim excludePriority As Single
            excludePriority = Single.Parse(PortalController.GetPortalSetting("SitemapExcludePriority", PortalSettings.PortalId, "0"), NumberFormatInfo.InvariantInfo())

            ' get all urls
            Dim isProviderEnabled As Boolean
            Dim isProviderPriorityOverrided As Boolean
            Dim providerPriorityValue As Single

            For Each _provider As SitemapProvider In Providers

                isProviderEnabled = Boolean.Parse(PortalController.GetPortalSetting(_provider.Name + "Enabled", PortalSettings.PortalId, "True"))
                If isProviderEnabled Then

                    ' check if we should override the priorities
                    isProviderPriorityOverrided = Boolean.Parse(PortalController.GetPortalSetting(_provider.Name + "Override", PortalSettings.PortalId, "False"))
                    ' stored as an integer (pr * 100) to prevent from translating errors with the decimal point
                    providerPriorityValue = Single.Parse(PortalController.GetPortalSetting(_provider.Name + "Value", PortalSettings.PortalId, "50")) / 100

                    ' Get all urls from provider
                    Dim urls As List(Of SitemapUrl) = _provider.GetUrls(PortalSettings.PortalId, PortalSettings, SITEMAP_VERSION)
                    For Each url As SitemapUrl In urls
                        If isProviderPriorityOverrided Then
                            url.Priority = providerPriorityValue
                        End If
                        If url.Priority >= excludePriority Then
                            allUrls.Add(url)
                        End If
                    Next
                End If

            Next

            If allUrls.Count > SITEMAP_MAXURLS Then
                ' create a sitemap index file

                ' enabled cache if it's not already
                If Not cached Then
                    cached = True
                    PortalController.UpdatePortalSetting(PortalSettings.PortalId, "SitemapCacheDays", "1")
                End If

                ' create all the files
                Dim index As Integer
                Dim numFiles As Integer = (allUrls.Count \ SITEMAP_MAXURLS) + 1
                Dim elementsInFile As Integer = allUrls.Count \ numFiles

                For index = 1 To numFiles
                    Dim lowerIndex As Integer = elementsInFile * (index - 1)
                    Dim elements As Integer
                    If index = numFiles Then
                        ' last file
                        elements = allUrls.Count - (elementsInFile * (numFiles - 1))
                    Else
                        elements = elementsInFile
                    End If

                    WriteSitemap(cached, output, index, allUrls.GetRange(lowerIndex, elements))
                Next

                ' create the sitemap index
                WriteSitemapIndex(output, index - 1)
            Else
                ' create a regular sitemap file
                WriteSitemap(cached, output, 0, allUrls)
            End If


            If cached Then
                WriteSitemapFileToOutput("sitemap.xml", output)
            End If

        End Sub

        ''' <summary>
        ''' Returns the sitemap file that is part of a sitemapindex.
        ''' </summary>
        ''' <param name="index">Index of the sitemap to return</param>
        ''' <param name="output">The output stream</param>
        ''' <remarks>The file should already exist since when using sitemapindexes the files are all cached to disk</remarks>
        Public Sub GetSitemapIndexFile(ByVal index As String, ByVal output As TextWriter)

            WriteSitemapFileToOutput("sitemap_" + index + ".xml", output)

        End Sub

        ''' <summary>
        ''' Generates a sitemap file
        ''' </summary>
        ''' <param name="cached">Wheter the generated file should be cached or not</param>
        ''' <param name="output">The output stream</param>
        ''' <param name="index">For sitemapindex files the number of the file being generated, 0 otherwise</param>
        ''' <param name="allUrls">The list of urls to be included in the file</param>
        ''' <remarks>
        ''' If the output should be cached it will generate a file under the portal directory (portals\[portalid]\sitemaps\) with 
        ''' the result of the generation. If the file is part of a sitemap, <paramref name="index">index</paramref> will be appended to the
        ''' filename cached on disk ("sitemap_1.xml")
        ''' </remarks>
        Private Sub WriteSitemap(ByVal cached As Boolean, ByVal output As TextWriter, ByVal index As Integer, ByVal allUrls As List(Of SitemapUrl))

            ' sitemap Output: can be a file is cache is enabled
            Dim sitemapOutput As TextWriter = output
            If cached Then
                If Not Directory.Exists(PortalSettings.HomeDirectoryMapPath + "Sitemap") Then
                    Directory.CreateDirectory(PortalSettings.HomeDirectoryMapPath + "Sitemap")
                End If
                Dim cachedFile As String = "sitemap.xml"
                If index > 0 Then
                    cachedFile = "sitemap_" + index.ToString() + ".xml"
                End If
                sitemapOutput = New StreamWriter(PortalSettings.HomeDirectoryMapPath + "Sitemap\" + cachedFile, False, Encoding.UTF8)
            End If

            ' Initialize writer
            Dim settings As New XmlWriterSettings()
            settings.Indent = True
            settings.Encoding = Encoding.UTF8
            settings.OmitXmlDeclaration = False

            writer = XmlWriter.Create(sitemapOutput, settings)

            ' build header
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/" + SITEMAP_VERSION)
            writer.WriteAttributeString("xmlns", "xsi", Nothing, "http://www.w3.org/2001/XMLSchema-instance")
            writer.WriteAttributeString("xsi", "schemaLocation", Nothing, "http://www.sitemaps.org/schemas/sitemap/" + SITEMAP_VERSION)

            ' write urls to output
            For Each url As SitemapUrl In allUrls
                AddURL(url)
            Next

            writer.WriteEndElement()
            writer.Close()

            If cached Then
                sitemapOutput.Flush()
                sitemapOutput.Close()
            End If
        End Sub

        ''' <summary>
        ''' Generates a sitemapindex file
        ''' </summary>
        ''' <param name="output">The output stream</param>
        ''' <param name="totalFiles">Number of files that are included in the sitemap index</param>
        Private Sub WriteSitemapIndex(ByVal output As TextWriter, ByVal totalFiles As Integer)

            Dim sitemapOutput As TextWriter
            sitemapOutput = New StreamWriter(PortalSettings.HomeDirectoryMapPath + "Sitemap\sitemap.xml", False, Encoding.UTF8)

            ' Initialize writer
            Dim settings As New XmlWriterSettings()
            settings.Indent = True
            settings.Encoding = Encoding.UTF8
            settings.OmitXmlDeclaration = False

            writer = XmlWriter.Create(sitemapOutput, settings)

            ' build header
            writer.WriteStartElement("sitemapindex", "http://www.sitemaps.org/schemas/sitemap/" + SITEMAP_VERSION)

            ' write urls to output
            For index As Integer = 1 To totalFiles
                Dim url As String

                url = "~/Sitemap.aspx?i=" + index.ToString()
                If IsChildPortal(PortalSettings, HttpContext.Current) Then
                    url += "&portalid=" + PortalSettings.PortalId.ToString()
                End If

                writer.WriteStartElement("sitemap")
                writer.WriteElementString("loc", AddHTTP(HttpContext.Current.Request.Url.Host + ResolveUrl(url)))
                writer.WriteElementString("lastmod", DateTime.Now.ToString("yyyy-MM-dd"))
                writer.WriteEndElement()
            Next
            writer.WriteEndElement()
            writer.Close()

            sitemapOutput.Flush()
            sitemapOutput.Close()
        End Sub

#End Region

#Region "Helper methods"
        ''' <summary>
        ''' Adds a new url to the sitemap
        ''' </summary>
        ''' <param name="sitemapUrl">The url to be included in the sitemap</param>
        ''' <remarks></remarks>
        Private Sub AddURL(ByVal sitemapUrl As SitemapUrl)

            writer.WriteStartElement("url")
            writer.WriteElementString("loc", sitemapUrl.Url)
            writer.WriteElementString("lastmod", sitemapUrl.LastModified.ToString("yyyy-MM-dd"))
            writer.WriteElementString("changefreq", sitemapUrl.ChangeFrequency.ToString().ToLower())
            writer.WriteElementString("priority", sitemapUrl.Priority.ToString("F01", CultureInfo.InvariantCulture))
            writer.WriteEndElement()

        End Sub

        ''' <summary>
        ''' Is sitemap is cached, verifies is the cached file exists and is still valid
        ''' </summary>
        ''' <returns>True is the cached file exists and is still valid, false otherwise</returns>
        Private Function CacheIsValid() As Boolean
            Dim cacheDays As Integer = Integer.Parse(PortalController.GetPortalSetting("SitemapCacheDays", PortalSettings.PortalId, "1"))
            Dim isValid As Boolean = True

            If Not File.Exists(PortalSettings.HomeDirectoryMapPath + "Sitemap\sitemap.xml") Then
                isValid = False
            End If
            If isValid Then
                Dim lastmod As DateTime = File.GetLastWriteTime(PortalSettings.HomeDirectoryMapPath + "/Sitemap/sitemap.xml")
                If lastmod.AddDays(cacheDays) < DateTime.Now Then
                    isValid = False
                End If
            End If

            Return isValid
        End Function

        ''' <summary>
        ''' When the sitemap is cached, reads the sitemap file and writes to the output stream
        ''' </summary>
        ''' <param name="output">The output stream</param>
        Private Sub WriteSitemapFileToOutput(ByVal file As String, ByVal output As TextWriter)

            If System.IO.File.Exists(PortalSettings.HomeDirectoryMapPath + "Sitemap\" + file) Then
                ' write the cached file to output
                Dim reader As New StreamReader(PortalSettings.HomeDirectoryMapPath + "/Sitemap/" + file, Encoding.UTF8)
                output.Write(reader.ReadToEnd())

                reader.Close()
            End If

        End Sub


        Private Function IsChildPortal(ByVal ps As PortalSettings, ByVal context As HttpContext) As Boolean
            Dim isChild As Boolean = False
            Dim portalName As String
            Dim aliasController As New PortalAliasController
            Dim arr As ArrayList = aliasController.GetPortalAliasArrayByPortalID(ps.PortalId)
            Dim serverPath As String = DotNetNuke.Common.Globals.GetAbsoluteServerPath(context.Request)

            If arr.Count > 0 Then
                Dim portalAlias As PortalAliasInfo = CType(arr(0), PortalAliasInfo)
                portalName = DotNetNuke.Common.Globals.GetPortalDomainName(ps.PortalAlias.HTTPAlias)
                If Convert.ToBoolean(InStr(1, portalAlias.HTTPAlias, "/")) Then
                    portalName = Mid(portalAlias.HTTPAlias, InStrRev(portalAlias.HTTPAlias, "/") + 1)
                End If
                If portalName <> "" AndAlso System.IO.Directory.Exists(serverPath & portalName) Then
                    isChild = True
                End If
            End If
            Return isChild
        End Function

#End Region

#Region "Provider configuration and setup"

        Private Shared _providers As List(Of SitemapProvider) = Nothing
        Private Shared _lock As Object = New Object()

        Public ReadOnly Property Providers() As List(Of SitemapProvider)
            Get
                Return _providers
            End Get
        End Property

        Private Shared Sub LoadProviders()

            ' Avoid claiming lock if providers are already loaded
            If _providers Is Nothing Then
                SyncLock _lock

                    _providers = New List(Of SitemapProvider)()

                    For Each comp As KeyValuePair(Of String, SitemapProvider) In ComponentModel.ComponentFactory.GetComponents(Of SitemapProvider)()

                        comp.Value.Name = comp.Key
                        comp.Value.Description = comp.Value.Description
                        _providers.Add(comp.Value)

                    Next


                    ''ProvidersHelper.InstantiateProviders(section.Providers, _providers, GetType(SiteMapProvider))
                End SyncLock
            End If

        End Sub

#End Region

    End Class
End Namespace


