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

Imports System.Web
Imports System.Text.RegularExpressions

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Url.FriendlyUrl
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.HttpModules
Imports System.Collections.Generic


Namespace DotNetNuke.Services.Url.FriendlyUrl

    Public Class DNNFriendlyUrlProvider

        Inherits FriendlyUrlProvider

#Region " Constants "

        Private Const ProviderType As String = "friendlyUrl"
        Private Const RegexMatchExpression As String = "[^a-zA-Z0-9 ]"

#End Region

#Region " Private Members "

        Private _providerConfiguration As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private _includePageName As Boolean
        Private _regexMatch As String
        Private _fileExtension As String
        Private _urlFormat As UrlFormatType = UrlFormatType.SearchFriendly

#End Region

#Region " Constructors "

        Public Sub New()

            ' Read the configuration specific information for this provider
            Dim objProvider As Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Provider)

            ' Read the attributes for this provider

            If Convert.ToString(objProvider.Attributes("includePageName")) <> "" Then
                _includePageName = Boolean.Parse(objProvider.Attributes("includePageName"))
            Else
                _includePageName = True
            End If

            If Convert.ToString(objProvider.Attributes("regexMatch")) <> "" Then
                _regexMatch = objProvider.Attributes("regexMatch")
            Else
                _regexMatch = RegexMatchExpression
            End If

            If Convert.ToString(objProvider.Attributes("fileExtension")) <> "" Then
                _fileExtension = objProvider.Attributes("fileExtension")
            Else
                _fileExtension = ".aspx"
            End If

            If Convert.ToString(objProvider.Attributes("urlFormat")) <> "" Then
                Select Case objProvider.Attributes("urlFormat").ToLower()
                    Case "searchfriendly"
                        _urlFormat = UrlFormatType.SearchFriendly

                    Case "humanfriendly"
                        _urlFormat = UrlFormatType.HumanFriendly

                    Case Else
                        _urlFormat = UrlFormatType.SearchFriendly
                End Select
            End If

        End Sub

#End Region

#Region " Public Properties "

        Public ReadOnly Property FileExtension() As String
            Get
                Return _fileExtension
            End Get
        End Property

        Public ReadOnly Property IncludePageName() As Boolean
            Get
                Return _includePageName
            End Get
        End Property

        Public ReadOnly Property RegexMatch() As String
            Get
                Return _regexMatch
            End Get
        End Property

        Public ReadOnly Property UrlFormat() As UrlFormatType
            Get
                Return _urlFormat
            End Get
        End Property

#End Region

#Region " Public Methods "

        Public Overloads Overrides Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String) As String
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Return FriendlyUrl(tab, path, glbDefaultPage, _portalSettings)
        End Function

        Public Overloads Overrides Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String, ByVal pageName As String) As String
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Return FriendlyUrl(tab, path, pageName, _portalSettings)
        End Function

        Public Overloads Overrides Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String, ByVal pageName As String, ByVal settings As PortalSettings) As String

            Return FriendlyUrl(tab, path, pageName, settings.PortalAlias.HTTPAlias)

        End Function

        Public Overloads Overrides Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String, ByVal pageName As String, ByVal portalAlias As String) As String

            Dim friendlyPath As String = path
            Dim matchString As String = ""
            Dim isPagePath As Boolean = tab IsNot Nothing

            If (UrlFormat = UrlFormatType.HumanFriendly) Then
                If (tab IsNot Nothing) Then
                    Dim queryStringDic As Dictionary(Of String, String) = GetQueryStringDictionary(path)
                    If (queryStringDic.Count = 0 OrElse (queryStringDic.Count = 1 AndAlso queryStringDic.ContainsKey("tabid"))) Then
                        'Return AddHTTP(portalAlias & "/" & tab.TabPath.Replace("//", "/").TrimStart(Convert.ToChar("/")) + ".aspx")
                        friendlyPath = GetFriendlyAlias("~/" & tab.TabPath.Replace("//", "/").TrimStart("/"c) + ".aspx", portalAlias, isPagePath)
                    ElseIf (queryStringDic.Count = 2 AndAlso queryStringDic.ContainsKey("tabid") AndAlso _
                            queryStringDic.ContainsKey("language")) Then
                        If Not tab.IsNeutralCulture Then
                            friendlyPath = GetFriendlyAlias("~/" & tab.CultureCode & "/" & tab.TabPath.Replace("//", "/").TrimStart("/"c) + ".aspx", portalAlias, isPagePath).ToLower
                        Else
                            friendlyPath = GetFriendlyAlias("~/" & queryStringDic("language") & "/" & tab.TabPath.Replace("//", "/").TrimStart("/"c) + ".aspx", portalAlias, isPagePath).ToLower
                        End If
                    Else
                        If (queryStringDic.ContainsKey("ctl")) AndAlso Not (queryStringDic.ContainsKey("language")) Then
                            Select Case queryStringDic("ctl")
                                Case "terms"
                                    friendlyPath = GetFriendlyAlias("~/terms.aspx", portalAlias, isPagePath)
                                Case "privacy"
                                    friendlyPath = GetFriendlyAlias("~/privacy.aspx", portalAlias, isPagePath)
                                Case "login"
                                    If (queryStringDic.ContainsKey("returnurl")) Then
                                        friendlyPath = GetFriendlyAlias("~/login.aspx?ReturnUrl=" & queryStringDic("returnurl"), portalAlias, isPagePath)
                                    Else
                                        friendlyPath = GetFriendlyAlias("~/login.aspx", portalAlias, isPagePath)
                                    End If
                                Case "register"
                                    If (queryStringDic.ContainsKey("returnurl")) Then
                                        friendlyPath = GetFriendlyAlias("~/register.aspx?returnurl=" & queryStringDic("returnurl"), portalAlias, isPagePath)
                                    Else
                                        friendlyPath = GetFriendlyAlias("~/register.aspx", portalAlias, isPagePath)
                                    End If
                                Case Else
                                    'Return Search engine friendly version
                                    Return GetFriendlyQueryString(tab, GetFriendlyAlias(path, portalAlias, isPagePath), pageName)
                            End Select
                        Else
                            'Return Search engine friendly version
                            Return GetFriendlyQueryString(tab, GetFriendlyAlias(path, portalAlias, isPagePath), pageName)
                        End If
                    End If
                End If
            Else
                'Return Search engine friendly version
                friendlyPath = GetFriendlyQueryString(tab, GetFriendlyAlias(path, portalAlias, isPagePath), pageName)
            End If

            friendlyPath = CheckPathLength(friendlyPath, path)

            Return friendlyPath

        End Function

#End Region

#Region " Private Methods "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddPage adds the page to the friendly url
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="path">The path to format.</param>
        ''' <param name="pageName">The page name.</param>
        ''' <returns>The formatted url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function AddPage(ByVal path As String, ByVal pageName As String) As String
            Dim friendlyPath As String = path

            If (friendlyPath.EndsWith("/")) Then
                friendlyPath = friendlyPath & pageName
            Else
                friendlyPath = friendlyPath & "/" & pageName
            End If

            Return friendlyPath
        End Function

        Private Function CheckPathLength(ByVal friendlyPath As String, ByVal originalpath As String) As String

            If friendlyPath.Length >= 260 Then '248 + "/default.aspx"
                Return ResolveUrl(originalpath)
            Else
                Return friendlyPath
            End If

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetFriendlyAlias gets the Alias root of the friendly url
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="path">The path to format.</param>
        ''' <param name="portalAlias">The portal alias of the site.</param>
        ''' <returns>The formatted url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetFriendlyAlias(ByVal path As String, ByVal portalAlias As String, ByVal IsPagePath As Boolean) As String
            Dim friendlyPath As String = path
            Dim matchString As String = ""

            If Not (portalAlias = Null.NullString) Then

                If Not (HttpContext.Current.Items("UrlRewrite:OriginalUrl") Is Nothing) Then
                    Dim httpAlias As String = AddHTTP(portalAlias).ToLowerInvariant()
                    Dim originalUrl As String = HttpContext.Current.Items("UrlRewrite:OriginalUrl").ToString().ToLowerInvariant()

                    httpAlias = AddPort(httpAlias, originalUrl)

                    If (originalUrl.StartsWith(httpAlias)) Then
                        matchString = httpAlias
                    End If

                    If (matchString = "") Then
                        'Manage the special case where original url contains the alias as
                        'http://www.domain.com/Default.aspx?alias=www.domain.com/child"
                        Dim portalMatch As Match = Regex.Match(originalUrl, "^?alias=" & portalAlias, RegexOptions.IgnoreCase)
                        If Not (portalMatch Is Match.Empty) Then
                            matchString = httpAlias
                        End If
                    End If

                    If (matchString = "") Then
                        'Manage the special case of child portals 
                        'http://www.domain.com/child/default.aspx
                        Dim tempurl As String = HttpContext.Current.Request.Url.Host & ResolveUrl(friendlyPath)
                        If Not tempurl.Contains(portalAlias) Then
                            matchString = httpAlias
                        End If
                    End If

                    If (matchString = "") Then
                        ' manage the case where the current hostname is www.domain.com and the portalalias is domain.com
                        ' (this occurs when www.domain.com is not listed as portal alias for the portal, but domain.com is)
                        Dim wwwHttpAlias As String = AddHTTP("www." & portalAlias)
                        If (originalUrl.StartsWith(wwwHttpAlias)) Then
                            matchString = wwwHttpAlias
                        End If
                    End If

                End If
            End If

            If (matchString <> "") Then
                If (path.IndexOf("~") <> -1) Then
                    If matchString.EndsWith("/") Then
                        friendlyPath = friendlyPath.Replace("~/", matchString)
                    Else
                        friendlyPath = friendlyPath.Replace("~", matchString)
                    End If
                Else
                    friendlyPath = matchString & friendlyPath
                End If
            Else
                friendlyPath = ResolveUrl(friendlyPath)
            End If

            If friendlyPath.StartsWith("//") And IsPagePath Then
                friendlyPath = friendlyPath.Substring(1)
            End If
            Return friendlyPath
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetFriendlyQueryString gets the Querystring part of the friendly url
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="tab">The tab whose url is being formatted.</param>
        ''' <param name="path">The path to format.</param>
        ''' <returns>The formatted url</returns>
        ''' <history>
        '''		[cnurse]	12/16/2004	created
        '''		[smcculloch]10/10/2005	Regex update for rewritten characters
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetFriendlyQueryString(ByVal tab As TabInfo, ByVal path As String, ByVal pageName As String) As String
            Dim friendlyPath As String = path
            Dim queryStringMatch As Match = Regex.Match(friendlyPath, "(.[^\\?]*)\\?(.*)", RegexOptions.IgnoreCase)
            Dim queryStringSpecialChars As String = ""

            If Not (queryStringMatch Is Match.Empty) Then
                friendlyPath = queryStringMatch.Groups(1).Value
                friendlyPath = Regex.Replace(friendlyPath, glbDefaultPage, "", RegexOptions.IgnoreCase)

                Dim queryString As String = queryStringMatch.Groups(2).Value.Replace("&amp;", "&")
                If (queryString.StartsWith("?")) Then
                    queryString = queryString.TrimStart(Convert.ToChar("?"))
                End If

                Dim nameValuePairs As String() = queryString.Split(Convert.ToChar("&"))
                For i As Integer = 0 To nameValuePairs.Length - 1
                    Dim pathToAppend As String = ""
                    Dim pair As String() = nameValuePairs(i).Split(Convert.ToChar("="))

                    'Add name part of name/value pair
                    If (friendlyPath.EndsWith("/")) Then
                        pathToAppend = pathToAppend & pair(0)
                    Else
                        pathToAppend = pathToAppend & "/" & pair(0)
                    End If

                    If (pair.Length > 1) Then
                        If (pair(1).Length > 0) Then

                            If (Regex.IsMatch(pair(1), _regexMatch) = False) Then

                                ' Contains Non-AlphaNumeric Characters
                                If (pair(0).ToLower() = "tabid") Then
                                    If (IsNumeric(pair(1))) Then
                                        If Not (tab Is Nothing) Then
                                            Dim tabId As Integer = Convert.ToInt32(pair(1))
                                            If (tab.TabID = tabId) Then
                                                If (tab.TabPath <> Null.NullString) And IncludePageName Then
                                                    pathToAppend = tab.TabPath.Replace("//", "/").TrimStart("/"c) & "/" & pathToAppend
                                                End If
                                            End If
                                        End If
                                    End If
                                End If

                                pathToAppend = pathToAppend & "/" & System.Web.HttpUtility.UrlPathEncode(pair(1))

                            Else

                                ' Rewrite into URL, contains only alphanumeric and the % or space
                                If (queryStringSpecialChars.Length = 0) Then
                                    queryStringSpecialChars = pair(0) & "=" & pair(1)
                                Else
                                    queryStringSpecialChars = queryStringSpecialChars & "&" & pair(0) & "=" & pair(1)
                                End If

                                pathToAppend = ""

                            End If
                        Else
                            pathToAppend = pathToAppend & "/" & System.Web.HttpUtility.UrlPathEncode((Chr(32)).ToString())
                        End If
                    End If

                    friendlyPath = friendlyPath & pathToAppend
                Next
            End If

            If (queryStringSpecialChars.Length > 0) Then
                Return AddPage(friendlyPath, pageName) & "?" & queryStringSpecialChars
            Else
                Return AddPage(friendlyPath, pageName)
            End If
        End Function

        Private Function GetQueryStringDictionary(ByVal path As String) As Dictionary(Of String, String)
            Dim parts As String() = path.ToLowerInvariant().Split("?"c)
            Dim results As New Dictionary(Of String, String)

            If (parts.Count = 2) Then

                For Each part As String In parts(1).Split("&"c)
                    Dim keyvalue As String() = part.Split("="c)
                    If (keyvalue.Length = 2) Then
                        results(keyvalue(0)) = keyvalue(1)
                    End If
                Next part

            End If

            Return results

        End Function


#End Region

    End Class

End Namespace

