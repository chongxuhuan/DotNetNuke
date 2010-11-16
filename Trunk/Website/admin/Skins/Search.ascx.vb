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
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Entities.Modules.Definitions

Namespace DotNetNuke.UI.Skins.Controls
    Partial Class Search
        Inherits UI.Skins.SkinObjectBase

#Region "Private Members"

        Private _siteIconURL As String
        Private _submit As String
        Private _cssClass As String
        Private _showSite As Boolean = True
        Private _siteText As String
        Private _siteToolTip As String
        Private _siteURL As String
        Private _showWeb As Boolean = True
        Private _useDropDownList As Boolean = False
        Private _useWebForSite As Boolean = False
        Private _webIconURL As String
        Private _webText As String
        Private _webToolTip As String
        Private _webURL As String

        Const MyFileName As String = "Search.ascx"

#End Region

#Region "Public Members"
        ''' <summary>
        ''' Gets or sets the CSS class for the option buttons and search button
        ''' </summary>
        ''' <remarks>If you are using the DropDownList option then you can style the search
        ''' elements without requiring a custom CssClass.</remarks>
        Public Property CssClass() As String
            Get
                Return _cssClass
            End Get
            Set(ByVal Value As String)
                _cssClass = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the visibility setting for the radio button corresponding to site based searchs.
        ''' </summary>
        ''' <remarks>Set this value to false to hide the "Site" radio button.  This setting has no effect
        ''' if UseDropDownList is true.</remarks>
        Public Property ShowSite() As Boolean
            Get
                Return _showSite
            End Get
            Set(ByVal Value As Boolean)
                _showSite = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the visibility setting for the radio button corresponding to web based searchs.
        ''' </summary>
        ''' <remarks>Set this value to false to hide the "Web" radio button.  This setting has no effect
        ''' if UseDropDownList is true.</remarks>
        Public Property ShowWeb() As Boolean
            Get
                Return _showWeb
            End Get
            Set(ByVal Value As Boolean)
                _showWeb = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the site icon URL.
        ''' </summary>
        ''' <value>The site icon URL.</value>
        ''' <remarks>If the SiteIconURL is not set or is an empty string then this will return a site relative URL for the 
        ''' dotnetnuke-icon.gif image in the images/search subfolder.  SiteIconURL supports using 
        ''' app relative virtual paths designated by the use of the tilde (~).</remarks>
        Public Property SiteIconURL() As String
            Get
                If String.IsNullOrEmpty(_siteIconURL) Then
                    Return "~/images/Search/dotnetnuke-icon.gif"
                End If
                Return _siteIconURL
            End Get
            Set(ByVal Value As String)
                _siteIconURL = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text for the "site" radio button or option list item.
        ''' </summary>
        ''' <value>The site text.</value>
        ''' <remarks>If the value is not set or is an empty string, then the localized value from
        ''' /admin/skins/app_localresources/Search.ascx.resx localresource file is used.</remarks>
        Public Property SiteText() As String
            Get
                If String.IsNullOrEmpty(_siteText) Then
                    Return Localization.GetString("Site", Localization.GetResourceFile(Me, MyFileName))
                End If
                Return _siteText
            End Get
            Set(ByVal Value As String)
                _siteText = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the tooltip text for the "site" radio button.
        ''' </summary>
        ''' <value>The site tool tip.</value>
        ''' <remarks>If the value is not set or is an empty string, then the localized value from
        ''' /admin/skins/app_localresources/Search.ascx.resx localresource file is used.</remarks>
        Public Property SiteToolTip() As String
            Get
                If String.IsNullOrEmpty(_siteToolTip) Then
                    Return Localization.GetString("Site.ToolTip", Localization.GetResourceFile(Me, MyFileName))
                End If
                Return _siteToolTip
            End Get
            Set(ByVal Value As String)
                _siteToolTip = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the URL for doing web based site searches.
        ''' </summary>
        ''' <value>The site URL.</value>
        ''' <remarks>If the value is not set or is an empty string, then the localized value from
        ''' /admin/skins/app_localresources/Search.ascx.resx localresource file is used.
        ''' <para>The site URL is a template for an external search engine, which by default, uses Google.com.  The siteURL should
        ''' include the tokens [TEXT] and [DOMAIN] to be replaced automatically by the search text and the current site domain.</para></remarks>
        Public Property SiteURL() As String
            Get
                If String.IsNullOrEmpty(_siteURL) Then
                    Return Localization.GetString("URL", Localization.GetResourceFile(Me, MyFileName))
                End If
                Return _siteURL
            End Get
            Set(ByVal Value As String)
                _siteURL = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the html for the submit button.
        ''' </summary>
        ''' <remarks>If the value is not set or is an empty string, then the localized value from
        ''' /admin/skins/app_localresources/Search.ascx.resx localresource file is used.
        ''' <para>If you set the value to an hmtl img tag, then the src attribute will be made relative
        ''' to the current skinpath.</para></remarks>
        Public Property Submit() As String
            Get
                Return _submit
            End Get
            Set(ByVal Value As String)
                _submit = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether to use the web search engine for site searches.
        ''' </summary>
        ''' <remarks>Set this value to true to perform a domain limited search using the search engine defined by <see cref="SiteURL">SiteURL</see>.</remarks>
        Public Property UseWebForSite() As Boolean
            Get
                Return _useWebForSite
            End Get
            Set(ByVal Value As Boolean)
                _useWebForSite = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether to display the site/web options using a drop down list.
        ''' </summary>
        ''' <remarks>If true, then the site and web options are displayed in a drop down list.  If the 
        ''' drop down list is used, then the <see cref="ShowWeb">ShowWeb</see> and <see cref="ShowSite">ShowSite</see>
        ''' properties are not used.</remarks>
        Public Property UseDropDownList() As Boolean
            Get
                Return _useDropDownList
            End Get
            Set(ByVal Value As Boolean)
                _useDropDownList = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the web icon URL.
        ''' </summary>
        ''' <value>The web icon URL.</value>
        ''' <remarks>If the WebIconURL is not set or is an empty string then this will return a site relative URL for the 
        ''' google-icon.gif image in the images/search subfolder.  WebIconURL supports using 
        ''' app relative virtual paths designated by the use of the tilde (~).</remarks>
        Public Property WebIconURL() As String
            Get
                If String.IsNullOrEmpty(_webIconURL) Then
                    Return "~/images/Search/google-icon.gif"
                End If
                Return _webIconURL
            End Get
            Set(ByVal Value As String)
                _webIconURL = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text for the "web" radio button or option list item.
        ''' </summary>
        ''' <value>The web text.</value>
        ''' <remarks>If the value is not set or is an empty string, then the localized value from
        ''' /admin/skins/app_localresources/Search.ascx.resx localresource file is used.</remarks>
        Public Property WebText() As String
            Get
                If String.IsNullOrEmpty(_webText) Then
                    Return Localization.GetString("Web", Localization.GetResourceFile(Me, MyFileName))
                End If
                Return _webText
            End Get
            Set(ByVal Value As String)
                _webText = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the tooltip text for the "web" radio button.
        ''' </summary>
        ''' <value>The web tool tip.</value>
        ''' <remarks>If the value is not set or is an empty string, then the localized value from
        ''' /admin/skins/app_localresources/Search.ascx.resx localresource file is used.</remarks>
        Public Property WebToolTip() As String
            Get
                If String.IsNullOrEmpty(_webToolTip) Then
                    Return Localization.GetString("Web.ToolTip", Localization.GetResourceFile(Me, MyFileName))
                End If
                Return _webToolTip
            End Get
            Set(ByVal Value As String)
                _webToolTip = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the URL for doing web based searches.
        ''' </summary>
        ''' <value>The web URL.</value>
        ''' <remarks>If the value is not set or is an empty string, then the localized value from
        ''' /admin/skins/app_localresources/Search.ascx.resx localresource file is used.
        ''' <para>The web URL is a template for an external search engine, which by default, uses Google.com.  The WebURL should
        ''' include the token [TEXT] to be replaced automatically by the search text.  The [DOMAIN] token, if present, will be
        ''' replaced by an empty string.</para></remarks>
        Public Property WebURL() As String
            Get
                If String.IsNullOrEmpty(_webURL) Then
                    Return Localization.GetString("URL", Localization.GetResourceFile(Me, MyFileName))
                End If
                Return _webURL
            End Get
            Set(ByVal Value As String)
                _webURL = Value
            End Set
        End Property

        ''' <summary>
        ''' Executes the search.
        ''' </summary>
        ''' <param name="searchText">The text which will be used to perform the search.</param>
        ''' <param name="searchType">The type of the search. Use "S" for a site search, and "W" for a web search.</param>
        ''' <remarks>All web based searches will open in a new window, while site searches will open in the current window.  A site search uses the built
        ''' in search engine to perform the search, while both web based search variants will use an external search engine to perform a search.</remarks>
        Protected Sub ExecuteSearch(ByVal searchText As String, ByVal searchType As String)
            If Not String.IsNullOrEmpty(searchText) Then
                Select Case searchType
                    Case "S" ' site
                        If UseWebForSite Then
                            Dim strURL As String = SiteURL
                            If Not String.IsNullOrEmpty(strURL) Then
                                strURL = strURL.Replace("[TEXT]", Server.UrlEncode(searchText))
                                strURL = strURL.Replace("[DOMAIN]", Request.Url.Host)
                                UrlUtils.OpenNewWindow(Me.Page, Me.GetType(), strURL)
                            End If
                        Else
                            Dim objModules As New ModuleController
                            Dim searchTabId As Integer = Null.NullInteger
                            Dim arrModules As ArrayList = objModules.GetModulesByDefinition(PortalSettings.PortalId, "Search Results")
                            If arrModules.Count > 1 Then
                                For Each SearchModule As ModuleInfo In arrModules
                                    If SearchModule.CultureCode = PortalSettings.CultureCode Then
                                        searchTabId = SearchModule.TabID
                                    End If
                                Next
                            ElseIf arrModules.Count = 1 Then
                                searchTabId = DirectCast(arrModules(0), ModuleInfo).TabID
                            End If

                            If searchTabId = Null.NullInteger Then
                                Exit Sub
                            End If

                            If Host.UseFriendlyUrls Then
                                Response.Redirect(NavigateURL(searchTabId) & "?Search=" & Server.UrlEncode(searchText))
                            Else
                                Response.Redirect(NavigateURL(searchTabId) & "&Search=" & Server.UrlEncode(searchText))
                            End If
                        End If
                    Case "W" ' web
                        Dim strURL As String = WebURL
                        If Not String.IsNullOrEmpty(strURL) Then
                            strURL = strURL.Replace("[TEXT]", Server.UrlEncode(searchText))
                            strURL = strURL.Replace("[DOMAIN]", "")
                            UrlUtils.OpenNewWindow(Me.Page, Me.GetType(), strURL)
                        End If
                End Select
            End If
        End Sub

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Not Page.IsPostBack Then
                optWeb.Text = WebText
                optWeb.ToolTip = WebToolTip
                optSite.Text = SiteText
                optSite.ToolTip = SiteToolTip
                optWeb.Visible = ShowWeb
                optSite.Visible = ShowSite
                downArrow.AlternateText = Localization.GetString("DropDownGlyph.AltText", Localization.GetResourceFile(Me, MyFileName))
                downArrow.ToolTip = Localization.GetString("DropDownGlyph.AltText", Localization.GetResourceFile(Me, MyFileName))

                If optWeb.Visible Then
                    optWeb.Checked = True
                End If
                If optSite.Visible Then
                    optSite.Checked = True
                End If

                ClientAPI.RegisterKeyCapture(Me.txtSearch, Me.cmdSearch, Asc(vbCr))
                ClientAPI.RegisterKeyCapture(Me.txtSearchNew, Me.cmdSearchNew, Asc(vbCr))

                If Not Request.QueryString("Search") Is Nothing Then
                    txtSearch.Text = Request.QueryString("Search").ToString
                End If

                If Submit <> "" Then
                    If Submit.IndexOf("src=") <> -1 Then
                        Submit = Replace(Submit, "src=""", "src=""" & PortalSettings.ActiveTab.SkinPath)
                        Submit = Replace(Submit, "src='", "src='" & PortalSettings.ActiveTab.SkinPath)
                    End If
                Else
                    Submit = Localization.GetString("Search", Localization.GetResourceFile(Me, MyFileName))
                End If
                cmdSearch.Text = Submit
                cmdSearchNew.Text = Submit

                If CssClass <> "" Then
                    optWeb.CssClass = CssClass
                    optSite.CssClass = CssClass
                    cmdSearch.CssClass = CssClass
                    cmdSearchNew.CssClass = CssClass
                End If
            End If

        End Sub

        ''' <summary>
        ''' Handles the Click event of the cmdSearch control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        ''' <remarks>This event is only used when <see cref="UseDropDownList">UseDropDownList</see> is false.</remarks>
        Private Sub cmdSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSearch.Click
            Dim SearchType As String = "S"
            If optWeb.Visible Then
                If optWeb.Checked Then
                    SearchType = "W"
                End If
            End If

            ExecuteSearch(txtSearch.Text.Trim(), SearchType)
        End Sub

        ''' <summary>
        ''' Handles the Click event of the cmdSearchNew control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        ''' <remarks>This event is only used when <see cref="UseDropDownList">UseDropDownList</see> is true.</remarks>
        Protected Sub cmdSearchNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSearchNew.Click
            ExecuteSearch(txtSearchNew.Text.Trim(), ClientAPI.GetClientVariable(Page, "SearchIconSelected"))
        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        ''' <remarks>This event performs final initialization tasks for the search object UI.</remarks>
        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ClassicSearch.Visible = Not UseDropDownList
            DropDownSearch.Visible = UseDropDownList

            If UseDropDownList Then
                ' Client Variables will survive a postback so there is no reason to register them.
                If Not Page.IsPostBack Then
                    ClientAPI.RegisterClientVariable(Page, "SearchIconWebUrl", String.Format("url({0})", ResolveUrl(WebIconURL)), True)
                    ClientAPI.RegisterClientVariable(Page, "SearchIconSiteUrl", String.Format("url({0})", ResolveUrl(SiteIconURL)), True)

                    ' We are going to use a dnn client variable to store which search option (web/site) is selected.
                    ClientAPI.RegisterClientVariable(Page, "SearchIconSelected", "S", True)
                End If

                Dim script As String = String.Format(glbScriptFormat, ResolveUrl("~/Resources/Search/Search.js"))

                ClientAPI.RegisterStartUpScript(Page, "initSearch", script)
            End If
        End Sub

#End Region

    End Class

End Namespace
