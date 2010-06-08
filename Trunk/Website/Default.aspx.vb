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

Imports System.IO
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Application

Namespace DotNetNuke.Framework

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : CDefault
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[sun1]	1/19/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class DefaultPage
        Inherits DotNetNuke.Framework.CDefault : Implements IClientAPICallbackEventHandler

#Region "Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Property to allow the programmatic assigning of ScrollTop position
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	3/23/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PageScrollTop() As Integer
            Get
                Dim _PageScrollTop As Integer = Null.NullInteger
                If ScrollTop.Value.Length > 0 AndAlso IsNumeric(ScrollTop.Value) Then
                    _PageScrollTop = CInt(ScrollTop.Value)
                End If
                Return _PageScrollTop
            End Get
            Set(ByVal Value As Integer)
                ScrollTop.Value = Value.ToString
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' - Obtain PortalSettings from Current Context
        ''' - redirect to a specific tab based on name
        ''' - if first time loading this page then reload to avoid caching
        ''' - set page title and stylesheet
        ''' - check to see if we should show the Assembly Version in Page Title 
        ''' - set the background image if there is one selected
        ''' - set META tags, copyright, keywords and description
        ''' </remarks>
        ''' <history>
        ''' 	[sun1]	1/19/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub InitializePage()

            Dim objTabs As New TabController
            Dim objTab As TabInfo

            ' redirect to a specific tab based on name
            If Request.QueryString("tabname") <> "" Then
                Dim strURL As String = ""

                objTab = objTabs.GetTabByName(Request.QueryString("TabName"), CType(HttpContext.Current.Items("PortalSettings"), PortalSettings).PortalId)
                If Not objTab Is Nothing Then

                    Dim actualParamCount As Integer = 0
                    Dim params(Request.QueryString.Count - 1) As String 'maximum number of elements
                    For intParam As Integer = 0 To Request.QueryString.Count - 1
                        Select Case Request.QueryString.Keys(intParam).ToLower()
                            Case "tabid", "tabname"
                            Case Else
                                params(actualParamCount) = Request.QueryString.Keys(intParam) + "=" + Request.QueryString(intParam)
                                actualParamCount = actualParamCount + 1
                        End Select
                    Next
                    ReDim Preserve params(actualParamCount - 1) 'redim to remove blank elements

                    Response.Redirect(NavigateURL(objTab.TabID, Null.NullString, params), True)
                Else
                    ''404 Error - Redirect to ErrorPage
                    Throw New HttpException(404, "Not Found")
                End If
            End If

            If Request.IsAuthenticated = True Then
                Select Case Convert.ToString(Host.AuthenticatedCacheability)
                    Case "0" : Response.Cache.SetCacheability(HttpCacheability.NoCache)
                    Case "1" : Response.Cache.SetCacheability(HttpCacheability.Private)
                    Case "2" : Response.Cache.SetCacheability(HttpCacheability.Public)
                    Case "3" : Response.Cache.SetCacheability(HttpCacheability.Server)
                    Case "4" : Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache)
                    Case "5" : Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate)
                End Select
            End If

            ' page comment
            If Host.DisplayCopyright Then
                Comment += String.Concat(vbCrLf, _
                            "<!--**********************************************************************************-->", vbCrLf, _
                            "<!-- DotNetNuke® - http://www.dotnetnuke.com                                          -->", vbCrLf, _
                            "<!-- Copyright (c) 2002-2010                                                          -->", vbCrLf, _
                            "<!-- by DotNetNuke Corporation                                                        -->", vbCrLf, _
                            "<!--**********************************************************************************-->", vbCrLf)
            End If
            Page.Header.Controls.AddAt(0, New LiteralControl(Comment))

            If PortalSettings.ActiveTab.PageHeadText <> Null.NullString AndAlso Not IsAdminControl() Then
                Page.Header.Controls.Add(New LiteralControl(PortalSettings.ActiveTab.PageHeadText))
            End If

            ' set page title
            Dim strTitle As String = PortalSettings.PortalName
            For Each objTab In PortalSettings.ActiveTab.BreadCrumbs
                strTitle += String.Concat(" > ", objTab.TabName)
            Next
            ' tab title override
            If Not String.IsNullOrEmpty(PortalSettings.ActiveTab.Title) Then
                strTitle = PortalSettings.ActiveTab.Title
            End If
            Title = strTitle

            'set the background image if there is one selected
            If Not Me.FindControl("Body") Is Nothing Then
                If Not String.IsNullOrEmpty(PortalSettings.BackgroundFile) Then
                    CType(Me.FindControl("Body"), HtmlGenericControl).Attributes("style") = String.Concat("background-image:url(", PortalSettings.HomeDirectory & PortalSettings.BackgroundFile, ");")
                End If
            End If

            ' META Refresh
            If PortalSettings.ActiveTab.RefreshInterval > 0 _
                    AndAlso Request.QueryString("ctl") Is Nothing Then
                MetaRefresh.Content = PortalSettings.ActiveTab.RefreshInterval.ToString
            Else
                MetaRefresh.Visible = False
            End If

            ' META description
            If Not String.IsNullOrEmpty(PortalSettings.ActiveTab.Description) Then
                Description = PortalSettings.ActiveTab.Description
            Else
                Description = PortalSettings.Description
            End If

            ' META keywords
            If Not String.IsNullOrEmpty(PortalSettings.ActiveTab.KeyWords) Then
                KeyWords = PortalSettings.ActiveTab.KeyWords
            Else
                KeyWords = PortalSettings.KeyWords
            End If
            If Host.DisplayCopyright Then
                KeyWords += ",DotNetNuke,DNN"
            End If

            ' META copyright
            If Not String.IsNullOrEmpty(PortalSettings.FooterText) Then
                Copyright = PortalSettings.FooterText
            Else
                Copyright = String.Concat("Copyright (c) ", Year(Now()), " by ", PortalSettings.PortalName)
            End If

            ' META generator
            If Host.DisplayCopyright Then
                Generator = "DotNetNuke "
            Else
                Generator = ""
            End If

            ' META Robots
            If Request.QueryString("ctl") IsNot Nothing AndAlso _
                    (Request.QueryString("ctl") = "Login" OrElse Request.QueryString("ctl") = "Register") Then
                MetaRobots.Content = "NOINDEX, NOFOLLOW"
            Else
                MetaRobots.Content = "INDEX, FOLLOW"
            End If

            ' NonProduction Label Injection
            If NonProductionVersion() AndAlso Host.DisplayBetaNotice Then
                Dim versionString As String = String.Format(" ({0} Version: {1})", DotNetNukeContext.Current.Application.Status, DotNetNukeContext.Current.Application.Version.ToString)
                Title &= versionString
            End If

            ' register DNN SkinWidgets Inititialization scripts
            If PortalSettings.EnableSkinWidgets Then
                DotNetNuke.Framework.jQuery.RequestRegistration()
                ClientAPI.RegisterStartUpScript(Page, "initWidgets", String.Format("<script type=""text/javascript"" src=""{0}"" ></script>", ResolveUrl("~/Resources/Shared/scripts/initWidgets.js")))
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Look for skin level doctype configuration file, and inject the value into the top of default.aspx
        ''' when no configuration if found, the doctype for versions prior to 4.4 is used to maintain backwards compatibility with existing skins.
        ''' Adds xmlns and lang parameters when appropiate.
        ''' </summary>
        ''' <param name="Skin">The currently loading skin</param>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[cathal]	11/29/2006	Created
        '''     [cniknet]   05/20/2009  Refactored to use HtmlAttributes collection
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SetSkinDoctype(ByVal Skin As DotNetNuke.UI.Skins.Skin)
            Dim strLang As String = System.Globalization.CultureInfo.CurrentCulture.ToString()

            Dim strDocType As String = PortalSettings.ActiveTab.SkinDoctype

            If strDocType.Contains("XHTML 1.0") Then
                'XHTML 1.0
                HtmlAttributes.Add("xml:lang", strLang)
                HtmlAttributes.Add("lang", strLang)
                HtmlAttributes.Add("xmlns", "http://www.w3.org/1999/xhtml")
            ElseIf strDocType.Contains("XHTML 1.1") Then
                'XHTML 1.1
                HtmlAttributes.Add("xml:lang", strLang)
                HtmlAttributes.Add("xmlns", "http://www.w3.org/1999/xhtml")
            Else
                'other
                HtmlAttributes.Add("lang", strLang)
            End If
               
            'Find the placeholder control and render the doctype
            Dim objDoctype As Control = Me.FindControl("skinDocType")
            CType(objDoctype, System.Web.UI.WebControls.Literal).Text = PortalSettings.ActiveTab.SkinDoctype
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' - manage affiliates
        ''' - log visit to site
        ''' </remarks>
        ''' <history>
        ''' 	[sun1]	1/19/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ManageRequest()

            ' affiliate processing
            Dim AffiliateId As Integer = -1
            If Not Request.QueryString("AffiliateId") Is Nothing Then
                If IsNumeric(Request.QueryString("AffiliateId")) Then
                    AffiliateId = Int32.Parse(Request.QueryString("AffiliateId"))
                    Dim objAffiliates As New Services.Vendors.AffiliateController
                    objAffiliates.UpdateAffiliateStats(AffiliateId, 1, 0)

                    ' save the affiliateid for acquisitions
                    If Request.Cookies("AffiliateId") Is Nothing Then       ' do not overwrite
                        Dim objCookie As HttpCookie = New HttpCookie("AffiliateId")
                        objCookie.Value = AffiliateId.ToString
                        objCookie.Expires = Now.AddYears(1)       ' persist cookie for one year
                        Response.Cookies.Add(objCookie)
                    End If
                End If
            End If

            ' site logging
            If PortalSettings.SiteLogHistory <> 0 Then
                ' get User ID

                ' URL Referrer
                Dim URLReferrer As String = ""
                Try
                    If Not Request.UrlReferrer Is Nothing Then
                        URLReferrer = Request.UrlReferrer.ToString()
                    End If
                Catch ex As Exception
                End Try

                Dim strSiteLogStorage As String = Host.SiteLogStorage
                Dim intSiteLogBuffer As Integer = Host.SiteLogBuffer

                ' log visit
                Dim objSiteLogs As New Services.Log.SiteLog.SiteLogController

                Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo
                objSiteLogs.AddSiteLog(PortalSettings.PortalId, objUserInfo.UserID, URLReferrer, Request.Url.ToString(), Request.UserAgent, Request.UserHostAddress, Request.UserHostName, PortalSettings.ActiveTab.TabID, AffiliateId, intSiteLogBuffer, strSiteLogStorage)
            End If

        End Sub

        Private Sub ManageStyleSheets(ByVal PortalCSS As Boolean, ByVal ctlSkin As DotNetNuke.UI.Skins.Skin)

            ' initialize reference paths to load the cascading style sheets
            Dim ID As String

            Dim objCSSCache As Hashtable = CType(DataCache.GetCache("CSS"), Hashtable)
            If objCSSCache Is Nothing Then
                objCSSCache = New Hashtable
            End If

            If PortalCSS = False Then
                ' default style sheet ( required )
                ID = CreateValidID(Common.Globals.HostPath)
                AddStyleSheet(ID, Common.Globals.HostPath & "default.css")

                ' skin package style sheet
                ID = CreateValidID(PortalSettings.ActiveTab.SkinPath)
                If objCSSCache.ContainsKey(ID) = False Then
                    If File.Exists(Server.MapPath(ctlSkin.SkinPath) & "skin.css") Then
                        objCSSCache(ID) = ctlSkin.SkinPath & "skin.css"
                    Else
                        objCSSCache(ID) = ""
                    End If
                    If Not Host.PerformanceSetting = Common.Globals.PerformanceSettings.NoCaching Then
                        DataCache.SetCache("CSS", objCSSCache)
                    End If
                End If
                If objCSSCache(ID).ToString <> "" Then
                    AddStyleSheet(ID, objCSSCache(ID).ToString)
                End If

                ' skin file style sheet
                ID = CreateValidID(Replace(ctlSkin.SkinSrc, ".ascx", ".css"))
                If objCSSCache.ContainsKey(ID) = False Then
                    If File.Exists(Server.MapPath(Replace(ctlSkin.SkinSrc, ".ascx", ".css"))) Then
                        objCSSCache(ID) = Replace(ctlSkin.SkinSrc, ".ascx", ".css")
                    Else
                        objCSSCache(ID) = ""
                    End If
                    If Not Host.PerformanceSetting = Common.Globals.PerformanceSettings.NoCaching Then
                        DataCache.SetCache("CSS", objCSSCache)
                    End If
                End If
                If objCSSCache(ID).ToString <> "" Then
                    AddStyleSheet(ID, objCSSCache(ID).ToString)
                End If
            Else
                If File.Exists(PortalSettings.HomeDirectoryMapPath & "portal.css") Then
                    ' portal style sheet
                    ID = CreateValidID(PortalSettings.HomeDirectory)
                    AddStyleSheet(ID, PortalSettings.HomeDirectory & "portal.css")
                End If
            End If
        End Sub

        Private Sub ManageFavicon()
            Dim strFavicon As String = CType(DataCache.GetCache("FAVICON" & PortalSettings.PortalId.ToString), String)
            If strFavicon = "" Then
                If File.Exists(PortalSettings.HomeDirectoryMapPath & "favicon.ico") Then
                    strFavicon = PortalSettings.HomeDirectory & "favicon.ico"
                    If Not Host.PerformanceSetting = Common.Globals.PerformanceSettings.NoCaching Then
                        DataCache.SetCache("FAVICON" & PortalSettings.PortalId.ToString, strFavicon)
                    End If
                End If
            End If
            If strFavicon <> "" Then
                Dim objLink As New HtmlLink()
                objLink.Attributes("rel") = "SHORTCUT ICON"
                objLink.Attributes("href") = strFavicon

                Page.Header.Controls.Add(objLink)
            End If
        End Sub

        'I realize the parsing of this is rather primitive.  A better solution would be to use json serialization
        'unfortunately, I don't have the time to write it.  When we officially adopt MS AJAX, we will get this type of 
        'functionality and this should be changed to utilize it for its plumbing.
        Private Function ParsePageCallBackArgs(ByVal strArg As String) As Generic.Dictionary(Of String, String)
            Dim aryVals() As String = Split(strArg, DotNetNuke.UI.Utilities.ClientAPI.COLUMN_DELIMITER)
            Dim objDict As Generic.Dictionary(Of String, String) = New Generic.Dictionary(Of String, String)
            If aryVals.Length > 0 Then
                objDict.Add("type", aryVals(0))
                Select Case CType(objDict("type"), DNNClientAPI.PageCallBackType)
                    Case DNNClientAPI.PageCallBackType.GetPersonalization
                        objDict.Add("namingcontainer", aryVals(1))
                        objDict.Add("key", aryVals(2))
                    Case DNNClientAPI.PageCallBackType.SetPersonalization
                        objDict.Add("namingcontainer", aryVals(1))
                        objDict.Add("key", aryVals(2))
                        objDict.Add("value", aryVals(3))
                End Select
            End If
            Return objDict
        End Function

#End Region

#Region "Protected Methods"

        ' Enables addition of multiple attributes to HTML element
        ' This approach is needed because adding id/runat attribute to HTML 
        ' would cause XHTML validation to fail -- NK
        Protected ReadOnly Property HtmlAttributeList() As String
            Get
                If Not (HtmlAttributes Is Nothing) AndAlso (HtmlAttributes.Count > 0) Then
                    Dim attr As New StringBuilder("")
                    For Each attributeName As String In HtmlAttributes.Keys
                        If (attributeName <> "") AndAlso Not (HtmlAttributes(attributeName) Is Nothing) Then
                            Dim attributeValue As String = HtmlAttributes(attributeName)
                            ' Duplicate keys are returned as a comma-separated list
                            ' Split into individual attributeName=attributeValue pairs
                            If (attributeValue.IndexOf(",") > 0) Then
                                Dim attributeValues As String() = attributeValue.Split(",")
                                For attributeCounter As Integer = 0 To attributeValues.Length - 1
                                    attr.Append(" " & attributeName & "=""" & attributeValues(attributeCounter) & """")
                                Next
                            Else
                                attr.Append(" " & attributeName & "=""" & attributeValue & """")
                            End If
                        End If
                    Next
                    Return attr.ToString()
                Else
                    Return ""
                End If
            End Get
        End Property

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

        Protected Function NonProductionVersion() As Boolean
            Return DotNetNukeContext.Current.Application.Status <> ReleaseMode.Stable
        End Function
#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Contains the functionality to populate the Root aspx page with controls
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' - obtain PortalSettings from Current Context
        ''' - set global page settings.
        ''' - initialise reference paths to load the cascading style sheets
        ''' - add skin control placeholder.  This holds all the modules and content of the page.
        ''' </remarks>
        ''' <history>
        ''' 	[sun1]	1/19/2004	Created
        '''		[jhenning] 8/24/2005 Added logic to look for post originating from a ClientCallback
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            ' set global page settings
            InitializePage()

            ' load skin control
            Dim ctlSkin As DotNetNuke.UI.Skins.Skin = DotNetNuke.UI.Skins.Skin.GetSkin(Me)

            'check for and read skin package level doctype
            SetSkinDoctype(ctlSkin)

            'Manage disabled pages
            If PortalSettings.ActiveTab.DisableLink Then
                If TabPermissionController.CanAdminPage() Then
                    Dim heading As String = Localization.GetString("PageDisabled.Header")
                    Dim message As String = Localization.GetString("PageDisabled.Text")

                    DotNetNuke.UI.Skins.Skin.AddPageMessage(ctlSkin, heading, message, Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                Else
                    If PortalSettings.HomeTabId > 0 Then
                        Response.Redirect(NavigateURL(PortalSettings.HomeTabId), True)
                    Else
                        Response.Redirect(GetPortalDomainName(PortalSettings.PortalAlias.HTTPAlias, Request), True)
                    End If
                End If
            End If

            'check if running with known account defaults
            Dim messageText As String = ""
            If Request.IsAuthenticated = True AndAlso String.IsNullOrEmpty(Request.QueryString("runningDefault")) = False Then
                Dim userInfo As UserInfo = HttpContext.Current.Items("UserInfo")
                'only show message to default users
                If (userInfo.Username.ToLower = "admin") OrElse (userInfo.Username.ToLower = "host") Then
                    messageText = RenderDefaultsWarning()
                    Dim messageTitle As String = Services.Localization.Localization.GetString("InsecureDefaults.Title", Services.Localization.Localization.GlobalResourceFile)
                    UI.Skins.Skin.AddPageMessage(ctlSkin, messageTitle.ToString, messageText.ToString, Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            End If

            ' add CSS links
            ManageStyleSheets(False, ctlSkin)

            ' add skin to page
            SkinPlaceHolder.Controls.Add(ctlSkin)

            ' add CSS links
            ManageStyleSheets(True, ctlSkin)

            ' add Favicon
            ManageFavicon()

            ' ClientCallback Logic 
            DotNetNuke.UI.Utilities.ClientAPI.HandleClientAPICallbackEvent(Me)

            'add viewstateuserkey to protect against CSRF attacks
            If User.Identity.IsAuthenticated Then
                ViewStateUserKey = User.Identity.Name
            End If

        End Sub

        ''' <summary>
        ''' check if a warning about account defaults needs to be rendered
        ''' </summary>
        ''' <returns>localised error message</returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[cathal]	2/28/2007	Created
        ''' </history>
        Private Function RenderDefaultsWarning() As String
            Dim warningLevel As String = Request.QueryString("runningDefault").ToString
            Dim warningMessage As String = String.Empty
            Select Case warningLevel
                Case "1"
                    warningMessage = Services.Localization.Localization.GetString("InsecureAdmin.Text", Services.Localization.Localization.GlobalResourceFile)
                Case "2"
                    warningMessage = Services.Localization.Localization.GetString("InsecureHost.Text", Services.Localization.Localization.GlobalResourceFile)
                Case "3"
                    warningMessage = Services.Localization.Localization.GetString("InsecureDefaults.Text", Services.Localization.Localization.GlobalResourceFile)
            End Select

            Return warningMessage
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the Scrolltop html control which controls the open / closed nature of each module 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[sun1]	1/19/2004	Created
        '''		[jhenning] 3/23/2005 No longer passing in parameter to __dnn_setScrollTop, instead pulling value from textbox on client
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim Scrolltop As HtmlControls.HtmlInputHidden = CType(Page.FindControl("ScrollTop"), HtmlControls.HtmlInputHidden)
            If Scrolltop.Value <> "" Then
                DotNetNuke.UI.Utilities.DNNClientAPI.AddBodyOnloadEventHandler(Page, "__dnn_setScrollTop();")
                Scrolltop.Value = Scrolltop.Value
            End If

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender

            ' process the current request
            If Not IsAdminControl() Then
                ManageRequest()
            End If

            'Set the Head tags
            Page.Header.Title = Title

            MetaGenerator.Content = Generator
            MetaGenerator.Visible = (Generator <> "")

            MetaAuthor.Content = PortalSettings.PortalName

            MetaCopyright.Content = Copyright
            MetaCopyright.Visible = (Copyright <> "")

            MetaKeywords.Content = KeyWords
            MetaKeywords.Visible = (KeyWords <> "")

            MetaDescription.Content = Description
            MetaDescription.Visible = (Description <> "")

            ' Because we have delayed registration of the jQuery script,
            ' Modules can override the standard behavior by including their own script on the page.
            ' The module must register the script with the "jQuery" key and should notify user
            ' of potential version conflicts with core jQuery support.
            If jQuery.IsRequested Then jQuery.RegisterScript(Page)

        End Sub

        Public Function HandleCallbackEvent(ByVal eventArgument As String) As String Implements IClientAPICallbackEventHandler.RaiseClientAPICallbackEvent
            Dim objDict As Generic.Dictionary(Of String, String) = ParsePageCallBackArgs(eventArgument)
            If objDict.ContainsKey("type") Then

                'in order to limit the keys that can be accessed and written we are storing 
                'the enabled keys in a shared hash table
                If DNNClientAPI.IsPersonalizationKeyRegistered(objDict("namingcontainer") & ClientAPI.CUSTOM_COLUMN_DELIMITER & objDict("key")) = False Then
                    Throw New Exception(String.Format("This personalization key has not been enabled ({0}:{1}).  Make sure you enable it with DNNClientAPI.EnableClientPersonalization", objDict("namingcontainer"), objDict("key")))
                End If
                Select Case CType(objDict("type"), DNNClientAPI.PageCallBackType)
                    Case DNNClientAPI.PageCallBackType.GetPersonalization
                        Return Personalization.Personalization.GetProfile(objDict("namingcontainer"), objDict("key"))
                    Case DNNClientAPI.PageCallBackType.SetPersonalization
                        Personalization.Personalization.SetProfile(objDict("namingcontainer"), objDict("key"), objDict("value"))
                        Return objDict("value")
                    Case Else
                        Throw New Exception("Unknown Callback Type")
                End Select
            End If
            Return ""
        End Function

#End Region

    End Class

End Namespace
