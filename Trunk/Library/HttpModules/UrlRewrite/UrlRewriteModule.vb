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

Imports System
Imports System.Text.RegularExpressions
Imports System.Web
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Host
Imports System.Collections.Generic

Namespace DotNetNuke.HttpModules

    Public Class UrlRewriteModule
        Implements IHttpModule

#Region "Private Methods"

        Private Function FormatDomain(ByVal URL As String, ByVal ReplaceDomain As String, ByVal WithDomain As String) As String
            If ReplaceDomain <> "" And WithDomain <> "" Then
                If URL.IndexOf(ReplaceDomain) <> -1 Then
                    URL = URL.Replace(ReplaceDomain, WithDomain)
                End If
            End If
            Return URL
        End Function

        Private Sub RewriteUrl(ByVal app As HttpApplication)
            Dim Request As HttpRequest = app.Request
            Dim Response As HttpResponse = app.Response
            Dim requestedPath As String = app.Request.Url.AbsoluteUri

            ' save original url in context
            app.Context.Items.Add("UrlRewrite:OriginalUrl", app.Request.Url.AbsoluteUri)

            ' Friendly URLs are exposed externally using the following format
            ' http://www.domain.com/tabid/###/mid/###/ctl/xxx/default.aspx
            ' and processed internally using the following format
            ' http://www.domain.com/default.aspx?tabid=###&mid=###&ctl=xxx
            ' The system for accomplishing this is based on an extensible Regex rules definition stored in /SiteUrls.config
            Dim sendTo As String = ""

            ' save and remove the querystring as it gets added back on later
            ' path parameter specifications will take precedence over querystring parameters
            Dim strQueryString As String = ""
            If (app.Request.Url.Query <> "") Then
                strQueryString = Request.QueryString.ToString()
                requestedPath = requestedPath.Replace(app.Request.Url.Query, "")
            End If

            ' get url rewriting rules 
            Dim rules As Config.RewriterRuleCollection = Config.RewriterConfiguration.GetConfig().Rules

            ' iterate through list of rules
            Dim intMatch As Integer = -1
            For intRule As Integer = 0 To rules.Count - 1
                ' check for the existence of the LookFor value 
                Dim strLookFor As String = "^" & RewriterUtils.ResolveUrl(app.Context.Request.ApplicationPath, rules(intRule).LookFor) & "$"
                Dim objMatch As Match = Regex.Match(requestedPath, strLookFor, RegexOptions.IgnoreCase)

                ' if there is a match
                If (objMatch.Success) Then
                    ' create a new URL using the SendTo regex value
                    sendTo = RewriterUtils.ResolveUrl(app.Context.Request.ApplicationPath, Regex.Replace(requestedPath, strLookFor, rules(intRule).SendTo, RegexOptions.IgnoreCase))

                    Dim strParameters As String = objMatch.Groups(2).Value
                    ' process the parameters
                    If (strParameters.Trim().Length > 0) Then
                        ' split the value into an array based on "/" ( ie. /tabid/##/ )
                        strParameters = strParameters.Replace("\", "/")
                        Dim arrParameters As String() = strParameters.Split("/"c)
                        Dim strParameterDelimiter As String
                        Dim strParameterName As String
                        Dim strParameterValue As String
                        ' icreate a well formed querystring based on the array of parameters
                        For intParameter As Integer = 1 To arrParameters.Length - 1
                            ' ignore the page name 
                            If arrParameters(intParameter).ToLower.IndexOf(".aspx") = -1 Then
                                ' get parameter name
                                strParameterName = arrParameters(intParameter).Trim()
                                If strParameterName.Length > 0 Then
                                    ' add parameter to SendTo if it does not exist already  
                                    If sendTo.ToLower.IndexOf("?" & strParameterName.ToLower & "=") = -1 And sendTo.ToLower.IndexOf("&" & strParameterName.ToLower & "=") = -1 Then
                                        ' get parameter delimiter
                                        If sendTo.IndexOf("?") <> -1 Then
                                            strParameterDelimiter = "&"
                                        Else
                                            strParameterDelimiter = "?"
                                        End If
                                        sendTo = sendTo & strParameterDelimiter & strParameterName
                                        ' get parameter value
                                        strParameterValue = ""
                                        If (intParameter < (arrParameters.Length - 1)) Then
                                            intParameter += 1
                                            If (arrParameters(intParameter).Trim <> "") Then
                                                strParameterValue = arrParameters(intParameter).Trim()
                                            End If
                                        End If
                                        ' add the parameter value
                                        If strParameterValue.Length > 0 Then
                                            sendTo = sendTo & "=" & strParameterValue
                                        End If
                                    End If
                                End If
                            End If
                        Next
                    End If
                    intMatch = intRule
                    Exit For ' exit as soon as it processes the first match
                End If
            Next

            ' add querystring parameters back to SendTo
            If strQueryString <> "" Then
                Dim arrParameters As String() = strQueryString.Split("&"c)
                Dim strParameterName As String
                ' iterate through the array of parameters
                For intParameter As Integer = 0 To arrParameters.Length - 1
                    ' get parameter name
                    strParameterName = arrParameters(intParameter)
                    If strParameterName.IndexOf("=") <> -1 Then
                        strParameterName = strParameterName.Substring(0, strParameterName.IndexOf("="))
                    End If
                    ' check if parameter already exists
                    If sendTo.ToLower.IndexOf("?" & strParameterName.ToLower & "=") = -1 And sendTo.ToLower.IndexOf("&" & strParameterName.ToLower & "=") = -1 Then
                        ' add parameter to SendTo value
                        If sendTo.IndexOf("?") <> -1 Then
                            sendTo = sendTo & "&" & arrParameters(intParameter)
                        Else
                            sendTo = sendTo & "?" & arrParameters(intParameter)
                        End If
                    End If
                Next
            End If

            ' if a match was found to the urlrewrite rules
            If intMatch <> -1 Then
                If rules(intMatch).SendTo.StartsWith("~") Then
                    ' rewrite the URL for internal processing
                    RewriterUtils.RewriteUrl(app.Context, sendTo)
                Else
                    ' it is not possible to rewrite the domain portion of the URL so redirect to the new URL
                    Response.Redirect(sendTo, True)
                End If
            Else

                ' Try to rewrite by TabPath
                Dim domain As String = ""
                Dim url As String = app.Request.Url.Host & app.Request.Url.LocalPath

                Dim splitUrl() As String = url.Split(Convert.ToChar("/"))

                Dim myAlias As String = ""
                If (splitUrl.Length > 0) Then
                    For Each urlPart As String In splitUrl

                        If (myAlias = "") Then
                            myAlias = urlPart
                        Else
                            myAlias = myAlias & "/" & urlPart
                        End If

                        Dim objPortalAlias As PortalAliasInfo = PortalAliasController.GetPortalAliasInfo(myAlias)
                        If Not objPortalAlias Is Nothing Then
                            Dim portalID As Integer = objPortalAlias.PortalID
                            ' Identify Tab Name 
                            Dim tabPath As String = url.Replace(myAlias, "")

                            ' Default Page has been Requested
                            If (tabPath = "/" & glbDefaultPage.ToLower()) Then
                                Return
                            End If

                            ' Check to see if the tab exists
                            Dim tabID As Integer = TabController.GetTabByTabPath(portalID, tabPath.Replace("/", "//").Replace(".aspx", ""))

                            If (tabID <> Null.NullInteger) Then
                                If (app.Request.Url.Query <> "") Then
                                    RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?TabID=" & tabID.ToString() & "&" & app.Request.Url.Query.TrimStart("?"c))
                                Else
                                    RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?TabID=" & tabID.ToString())
                                End If
                                Return
                            End If

                            tabPath = tabPath.ToLower()

                            If (tabPath.IndexOf("?"c) <> -1) Then
                                tabPath = tabPath.Substring(0, tabPath.IndexOf("?"c))
                            End If

                            If (tabPath = "/login.aspx") Then
                                'Get the Portal
                                Dim portal As PortalInfo = New PortalController().GetPortal(portalID)

                                If portal.LoginTabId > Null.NullInteger AndAlso ValidateLoginTabID(portal.LoginTabId) Then
                                    If (app.Request.Url.Query <> "") Then
                                        RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?TabID=" & portal.LoginTabId.ToString() & app.Request.Url.Query.TrimStart("?"c))
                                    Else
                                        RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?TabID=" & portal.LoginTabId.ToString())
                                    End If
                                Else
                                    If (app.Request.Url.Query <> "") Then
                                        RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?portalid=" & portalID.ToString() & "&ctl=login&" & app.Request.Url.Query.TrimStart("?"c))
                                    Else
                                        RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?portalid=" & portalID.ToString() & "&ctl=login")
                                    End If
                                End If
                                Return
                            End If

                            If (tabPath = "/register.aspx") Then

                                If (app.Request.Url.Query <> "") Then
                                    RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?portalid=" & portalID.ToString() & "&ctl=Register&" & app.Request.Url.Query.TrimStart("?"c))
                                Else
                                    RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?portalid=" & portalID.ToString() & "&ctl=Register")
                                End If
                                Return
                            End If

                            If (tabPath = "/terms.aspx") Then
                                If (app.Request.Url.Query <> "") Then
                                    RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?portalid=" & portalID.ToString() & "&ctl=Terms&" & app.Request.Url.Query.TrimStart("?"c))
                                Else
                                    RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?portalid=" & portalID.ToString() & "&ctl=Terms")
                                End If
                                Return
                            End If

                            If (tabPath = "/privacy.aspx") Then
                                If (app.Request.Url.Query <> "") Then
                                    RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?portalid=" & portalID.ToString() & "&ctl=Privacy&" & app.Request.Url.Query.TrimStart("?"c))
                                Else
                                    RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?portalid=" & portalID.ToString() & "&ctl=Privacy")
                                End If
                                Return
                            End If

                            tabPath = tabPath.Replace("/", "//")
                            tabPath = tabPath.Replace(".aspx", "")

                            Dim objTabController As New TabController
                            Dim objTabs As TabCollection
                            If tabPath.StartsWith("//host") Then
                                objTabs = objTabController.GetTabsByPortal(Null.NullInteger)
                            Else
                                objTabs = objTabController.GetTabsByPortal(portalID)
                            End If
                            For Each kvp As KeyValuePair(Of Integer, TabInfo) In objTabs
                                If (kvp.Value.IsDeleted = False AndAlso kvp.Value.TabPath.ToLower() = tabPath) Then
                                    If (app.Request.Url.Query <> "") Then
                                        RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?portalid=" & portalID.ToString() & "&" & app.Request.Url.Query.TrimStart("?"c))
                                    Else
                                        RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage & "?TabID=" & kvp.Value.TabID.ToString())
                                    End If
                                    Return
                                End If
                            Next
                        Else

                        End If

                    Next
                Else
                    ' Should always resolve to something
                    ' RewriterUtils.RewriteUrl(app.Context, "~/" & glbDefaultPage)
                    Return
                End If
            End If
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property ModuleName() As String
            Get
                Return "UrlRewriteModule"
            End Get
        End Property

#End Region

        Public Sub Init(ByVal application As HttpApplication) Implements IHttpModule.Init
            AddHandler application.BeginRequest, AddressOf Me.OnBeginRequest
        End Sub

        Public Sub OnBeginRequest(ByVal s As Object, ByVal e As EventArgs)

            Dim app As HttpApplication = CType(s, HttpApplication)
            Dim Server As HttpServerUtility = app.Server
            Dim Request As HttpRequest = app.Request
            Dim Response As HttpResponse = app.Response
            Dim requestedPath As String = app.Request.Url.AbsoluteUri

            If Request.Url.LocalPath.ToLower.EndsWith("scriptresource.axd") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("webresource.axd") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("gif") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("jpg") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("css") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("js") Then
                Exit Sub
            End If

            'Carry out first time initialization tasks
            Initialize.Init(app)

            If Request.Url.LocalPath.ToLower.EndsWith("install.aspx") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("installwizard.aspx") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("captcha.aspx") Then
                Exit Sub
            End If

            ' URL validation 
            ' check for ".." escape characters commonly used by hackers to traverse the folder tree on the server
            ' the application should always use the exact relative location of the resource it is requesting
            Dim strURL As String = Request.Url.AbsolutePath
            Dim strDoubleDecodeURL As String = Server.UrlDecode(Server.UrlDecode(Request.RawUrl))
            If Regex.Match(strURL, "[\\/]\.\.[\\/]").Success Or Regex.Match(strDoubleDecodeURL, "[\\/]\.\.[\\/]").Success Then
                Throw New HttpException(404, "Not Found")
            End If
            Try
                'fix for ASP.NET canonicalization issues http://support.microsoft.com/?kbid=887459
                If (Request.Path.IndexOf(Chr(92)) >= 0 Or System.IO.Path.GetFullPath(Request.PhysicalPath) <> Request.PhysicalPath) Then
                    Throw New HttpException(404, "Not Found")
                End If
            Catch ex As Exception
                'DNN 5479
                'request.physicalPath throws an exception when the path of the request exceeds 248 chars.
                'example to test: http://localhost/dotnetnuke_2/xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx/default.aspx
            End Try

            'check if we are upgrading/installing or if this is a captcha request
            RewriteUrl(app)

            ' *Note: from this point on we are dealing with a "standard" querystring ( ie. http://www.domain.com/default.aspx?tabid=## )

            Dim TabId As Integer
            Dim PortalId As Integer
            Dim DomainName As String = Nothing
            Dim PortalAlias As String = Nothing
            Dim objPortalAliasInfo As PortalAliasInfo = Nothing

            ' get TabId from querystring ( this is mandatory for maintaining portal context for child portals )
            Try
                If Not Int32.TryParse(Request.QueryString("tabid"), TabId) Then
                    TabId = Null.NullInteger
                End If
                ' get PortalId from querystring ( this is used for host menu options as well as child portal navigation )
                If Not Int32.TryParse(Request.QueryString("portalid"), PortalId) Then
                    PortalId = Null.NullInteger
                End If
            Catch ex As Exception
                'The tabId or PortalId are incorrectly formatted (potential DOS)
                Throw New HttpException(404, "Not Found")
            End Try

            Try
                ' alias parameter can be used to switch portals
                If Not (Request.QueryString("alias") Is Nothing) Then
                    ' check if the alias is valid
                    If Not PortalAliasController.GetPortalAliasInfo(Request.QueryString("alias")) Is Nothing Then
                        ' check if the domain name contains the alias
                        If InStr(1, Request.QueryString("alias"), DomainName, CompareMethod.Text) = 0 Then
                            ' redirect to the url defined in the alias
                            Response.Redirect(GetPortalDomainName(Request.QueryString("alias"), Request), True)
                        Else ' the alias is the same as the current domain
                            PortalAlias = Request.QueryString("alias")
                        End If
                    End If
                End If

                ' parse the Request URL into a Domain Name token 
                DomainName = GetDomainName(Request, True)

                ' PortalId identifies a portal when set
                If PortalAlias Is Nothing Then
                    If PortalId <> -1 Then
                        PortalAlias = PortalAliasController.GetPortalAliasByPortal(PortalId, DomainName)
                    End If
                End If

                ' TabId uniquely identifies a Portal
                If PortalAlias Is Nothing Then
                    If TabId <> -1 Then
                        ' get the alias from the tabid, but only if it is for a tab in that domain
                        PortalAlias = PortalAliasController.GetPortalAliasByTab(TabId, DomainName)
                        If PortalAlias Is Nothing Or PortalAlias = "" Then
                            'if the TabId is not for the correct domain
                            'see if the correct domain can be found and redirect it 
                            objPortalAliasInfo = PortalAliasController.GetPortalAliasInfo(DomainName)
                            If Not objPortalAliasInfo Is Nothing Then
                                If app.Request.Url.AbsoluteUri.ToLower.StartsWith("https://") Then
                                    strURL = "https://" & objPortalAliasInfo.HTTPAlias.Replace("*.", "")
                                Else
                                    strURL = "http://" & objPortalAliasInfo.HTTPAlias.Replace("*.", "")
                                End If
                                If strURL.ToLower.IndexOf(DomainName.ToLower()) = -1 Then
                                    strURL += app.Request.Url.PathAndQuery
                                End If
                                Response.Redirect(strURL, True)
                            End If
                        End If
                    End If
                End If

                ' else use the domain name
                If PortalAlias Is Nothing Or PortalAlias = "" Then
                    PortalAlias = DomainName
                End If
                'using the DomainName above will find that alias that is the domainname portion of the Url
                'ie. dotnetnuke.com will be found even if zzz.dotnetnuke.com was entered on the Url
                objPortalAliasInfo = PortalAliasController.GetPortalAliasInfo(PortalAlias)
                If Not objPortalAliasInfo Is Nothing Then
                    PortalId = objPortalAliasInfo.PortalID
                End If

                ' if the portalid is not known
                If PortalId = -1 Then
                    If Not Request.Url.LocalPath.ToLower.EndsWith(glbDefaultPage.ToLower) Then
                        ' allows requests for aspx pages in custom folder locations to be processed
                        Exit Sub
                    Else
                        'the domain name was not found so try using the host portal's first alias
                        PortalId = Host.HostPortalID
                        If PortalId > Null.NullInteger Then
                            ' use the host portal
                            Dim objPortalAliasController As New PortalAliasController
                            Dim arrPortalAliases As ArrayList
                            arrPortalAliases = objPortalAliasController.GetPortalAliasArrayByPortalID(PortalId)
                            If arrPortalAliases.Count > 0 Then
                                'Get the first Alias
                                objPortalAliasInfo = CType(arrPortalAliases(0), PortalAliasInfo)
                                If app.Request.Url.AbsoluteUri.ToLower.StartsWith("https://") Then
                                    strURL = "https://" & objPortalAliasInfo.HTTPAlias.Replace("*.", "")
                                Else
                                    strURL = "http://" & objPortalAliasInfo.HTTPAlias.Replace("*.", "")
                                End If
                                If TabId <> -1 Then
                                    strURL += app.Request.Url.Query()
                                End If
                                Response.Redirect(strURL, True)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                ''500 Error - Redirect to ErrorPage
                strURL = "~/ErrorPage.aspx?status=500&error=" & Server.UrlEncode(ex.Message)
                HttpContext.Current.Response.Clear()
                HttpContext.Current.Server.Transfer(strURL)
            End Try

            If PortalId <> -1 Then
                ' load the PortalSettings into current context
                Dim _portalSettings As PortalSettings = New PortalSettings(TabId, objPortalAliasInfo)
                app.Context.Items.Add("PortalSettings", _portalSettings)

                ' manage page URL redirects - that reach here because they bypass the built-in navigation
                ' ie Spiders, saved favorites, hand-crafted urls etc
                If _portalSettings.ActiveTab.Url <> "" AndAlso Request.QueryString("ctl") Is Nothing AndAlso Request.QueryString("fileticket") Is Nothing Then
                    'Target Url
                    Dim redirectUrl As String = _portalSettings.ActiveTab.FullUrl

                    If _portalSettings.ActiveTab.PermanentRedirect Then
                        'Permanently Redirect
                        Response.StatusCode = 301
                        Response.AppendHeader("Location", redirectUrl)
                    Else
                        'Normal Redirect
                        Response.Redirect(redirectUrl, True)
                    End If
                End If

                ' manage secure connections
                If Request.Url.AbsolutePath.ToLower.EndsWith(".aspx") Then
                    ' request is for a standard page
                    strURL = ""
                    ' if SSL is enabled
                    If _portalSettings.SSLEnabled Then
                        ' if page is secure and connection is not secure
                        If _portalSettings.ActiveTab.IsSecure = True And Request.IsSecureConnection = False Then
                            ' switch to secure connection
                            strURL = requestedPath.Replace("http://", "https://")
                            strURL = FormatDomain(strURL, _portalSettings.STDURL, _portalSettings.SSLURL)
                        End If
                    End If
                    ' if SSL is enforced
                    If _portalSettings.SSLEnforced Then
                        ' if page is not secure and connection is secure
                        If _portalSettings.ActiveTab.IsSecure = False And Request.IsSecureConnection = True Then
                            ' check if connection has already been forced to secure
                            If Request.QueryString("ssl") Is Nothing Then
                                ' switch to unsecure connection
                                strURL = requestedPath.Replace("https://", "http://")
                                strURL = FormatDomain(strURL, _portalSettings.SSLURL, _portalSettings.STDURL)
                            End If
                        End If
                    End If
                    ' if a protocol switch is necessary
                    If strURL <> "" Then
                        If strURL.ToLower.StartsWith("https://") Then
                            ' redirect to secure connection
                            Response.Redirect(strURL, True)
                        Else ' when switching to an unsecure page, use a clientside redirector to avoid the browser security warning
                            Response.Clear()
                            ' add a refresh header to the response 
                            Response.AddHeader("Refresh", "0;URL=" & strURL)
                            ' add the clientside javascript redirection script
                            Response.Write("<html><head><title></title>")
                            Response.Write("<!-- <script language=""javascript"">window.location.replace(""" & strURL & """)</script> -->")
                            Response.Write("</head><body></body></html>")
                            ' send the response
                            Response.End()
                        End If
                    End If
                End If

            Else
                ' alias does not exist in database
                ' and all attempts to find another have failed
                'this should only happen if the HostPortal does not have any aliases
                '404 Error - Redirect to ErrorPage
                strURL = "~/ErrorPage.aspx?status=404&error=" & DomainName
                HttpContext.Current.Response.Clear()
                HttpContext.Current.Server.Transfer(strURL)
            End If

        End Sub

        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub

    End Class

End Namespace
