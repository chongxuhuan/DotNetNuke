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

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Xml

Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Syndication
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Framework
Imports DotNetNuke.Common

Namespace DotNetNuke.UI.WebControls

    <DefaultProperty("RssProxyUrl"), ToolboxData("<{0}:FeedBrowser runat=server></{0}:FeedBrowser>")> _
    Public Class FeedBrowser
        Inherits WebControlBase

        Private output As New StringBuilder("")
#Region "Private Members"
        Private _defaultTemplate As String = ""
        Private _rssProxyUrl As String = ""
        Private _opmlUrl As String = ""
        Private _opmlFile As String = ""
        Private _opmlText As String = ""
        Private _allowHtmlDescription As Boolean = True
#End Region

#Region "Public Properties"

        Property DefaultTemplate() As String
            Get
                Return _defaultTemplate
            End Get
            Set(ByVal value As String)
                _defaultTemplate = value
            End Set
        End Property


        Property RssProxyUrl() As String
            Get
                Return _rssProxyUrl
            End Get
            Set(ByVal value As String)
                _rssProxyUrl = value
            End Set
        End Property


        Property AllowHtmlDescription() As Boolean
            Get
                Return _allowHtmlDescription
            End Get
            Set(ByVal value As Boolean)
                _allowHtmlDescription = value
            End Set
        End Property


        Property OpmlUrl() As String
            Get
                Return _opmlUrl
            End Get
            Set(ByVal value As String)
                _opmlUrl = value
            End Set
        End Property

        Property OpmlFile() As String
            Get
                Return _opmlFile
            End Get
            Set(ByVal value As String)
                _opmlFile = value
            End Set
        End Property

        Property OpmlText() As String
            Get
                Return _opmlText
            End Get
            Set(ByVal value As String)
                _opmlText = value
            End Set
        End Property

#End Region

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            Dim opmlFeed As Opml = New Opml()
            Dim elementIdPrefix As String = Me.ClientID
            Dim instanceVarName As String = elementIdPrefix + "_feedBrowser"

            If ((OpmlUrl = "") And (OpmlFile = "") And (OpmlText = "")) Then
                opmlFeed = GetDefaultOpmlFeed()
            Else
                If (OpmlText <> "") Then
                    Dim opmlDoc As New XmlDocument()
                    opmlDoc.LoadXml("<?xml version=""1.0"" encoding=""utf-8""?><opml version=""2.0""><head /><body>" + OpmlText + "</body></opml>")
                    opmlFeed = Opml.LoadFromXml(opmlDoc)
                ElseIf (OpmlUrl <> "") Then
                    opmlFeed = Opml.LoadFromUrl(New Uri(OpmlUrl))
                Else
                    opmlFeed = Opml.LoadFromFile(OpmlFile)
                End If
            End If

            Dim script As New StringBuilder()

            Dim tabInstanceVarName As String = instanceVarName + "_tabs"
            script.Append("var " + tabInstanceVarName + " = new DotNetNuke.UI.WebControls.TabStrip.Strip(""" + tabInstanceVarName + """);")
            script.Append("var " + instanceVarName + " = new DotNetNuke.UI.WebControls.FeedBrowser.Browser(""" + instanceVarName + """," + tabInstanceVarName + ");")
            script.Append(tabInstanceVarName + ".setResourcesFolderUrl(""" + ResourcesFolderUrl + """);")
            script.Append(instanceVarName + ".setResourcesFolderUrl(""" + ResourcesFolderUrl + """);")

            script.Append(instanceVarName + ".setElementIdPrefix(""" + elementIdPrefix + """);")
            If (Theme <> "") Then
                script.Append(instanceVarName + ".setTheme(""" + Theme + """);")
            End If

            If (RssProxyUrl <> "") Then
                script.Append(instanceVarName + ".setRssProxyUrl(""" + RssProxyUrl + """);")
            End If

            If (DefaultTemplate <> "") Then
                script.Append(instanceVarName + ".setDefaultTemplate(""" + DefaultTemplate + """);")
            End If

            script.Append(instanceVarName + ".setAllowHtmlDescription(")
            If (AllowHtmlDescription) Then
                script.Append("true")
            Else
                script.Append("false")
            End If
            script.Append(");")


            Dim renderScript As String = GetRenderingScript(instanceVarName, opmlFeed.Outlines)
            Dim includeFallbackScript As Boolean = False

            ' Is there any OPML structure to render?
            If (renderScript = "") Then
                includeFallbackScript = True
                script.Append(instanceVarName + ".setTabs(defaultFeedBrowser());")
            Else
                ' Javascript function that renders the OPML structure
                script.Append("function " & instanceVarName & "customFeedBrowser() ")
                script.Append("{")
                script.Append("     var " & instanceVarName & "tabs = [];")
                script.Append("     with (DotNetNuke.UI.WebControls.FeedBrowser) ")
                script.Append("     {")
                script.Append(renderScript)
                script.Append("     }")
                script.Append("     return(" & instanceVarName & "tabs);")
                script.Append("} ")
                script.Append(instanceVarName + ".setTabs(" & instanceVarName & "customFeedBrowser());")
            End If

            ' NK 11/25/08
            ' This code has a jQuery dependency so it can't be loaded using standard client script registration
            ' It must come later in the page which is why it is inline; also ClientScript buggy when used in webcontrols

            If Not Page.ClientScript.IsClientScriptBlockRegistered("FBHostUrl") Then
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "FBHostUrl", "<script type=""text/javascript"">var $dnn = new Object(); $dnn.hostUrl = '" + Globals.ResolveUrl("~/") + "';</script>")
                output.Append("<script type=""text/javascript"" src=""" & Globals.ResolveUrl("~/Resources/Shared/scripts/init.js") & """></script>")
                output.Append("<script type=""text/javascript"" src=""" & Globals.ResolveUrl("~/Resources/Shared/scripts/DotNetNukeAjaxShared.js") & """></script>")
                output.Append("<script type=""text/javascript"" src=""" & Globals.ResolveUrl("~/Resources/TabStrip/scripts/tabstrip.js") & """></script>")
                output.Append("<script type=""text/javascript"" src=""" & Globals.ResolveUrl("~/Resources/FeedBrowser/scripts/feedbrowser.js") & """></script>")
                output.Append("<script type=""text/javascript"" src=""" & Globals.ResolveUrl("~/Resources/FeedBrowser/scripts/templates.js") & """></script>")
                If (includeFallbackScript) Then
                    output.Append("<script type=""text/javascript"" src=""" & Globals.ResolveUrl("~/Resources/FeedBrowser/scripts/fallback.js") & """></script>")
                End If
                If (StyleSheetUrl <> "") Then
                    script.Append(tabInstanceVarName + ".setStyleSheetUrl(""" + StyleSheetUrl + """);")
                    script.Append(instanceVarName + ".setStyleSheetUrl(""" + StyleSheetUrl + """);")
                End If
            End If

            script.Append(instanceVarName + ".render();")
            output.Append("<script type=""text/javascript"">" + script.ToString() + "</script>")

            MyBase.OnLoad(e)

        End Sub

        Public Overrides ReadOnly Property HtmlOutput() As String
            Get
                Return (output.ToString())
            End Get
        End Property


#Region "Private Methods"

        Private Function GetDefaultOpmlFeed() As Opml

            Dim opmlFeed As Opml = New Opml()
            Dim filePath As String = Config.GetPathToFile(ConfigFileType.SolutionsExplorer)

            If File.Exists(filePath) Then
                opmlFeed = Opml.LoadFromFile(filePath)
            End If

            Return opmlFeed
        End Function

        Private Function GetRenderingScript(ByVal instanceVarName As String, ByVal _outlines As OpmlOutlines) As String

            Dim script As String = ""

            ' First fetch any linked OPML files
            ' Only one level of link fetching is supported so
            ' no recursion
            Dim expandedOutlines As New OpmlOutlines
            For Each item As OpmlOutline In _outlines
                If (item.Type = "link") Then
                    Dim linkedFeed As Opml = Opml.LoadFromUrl(item.XmlUrl)
                    If (item.Category.StartsWith("Tab")) Then
                        expandedOutlines.Add(item)
                        For Each linkedOutline As OpmlOutline In linkedFeed.Outlines
                            item.Outlines.Add(linkedOutline)
                        Next
                    Else
                        For Each linkedOutline As OpmlOutline In linkedFeed.Outlines
                            expandedOutlines.Add(linkedOutline)
                        Next
                    End If
                Else
                    expandedOutlines.Add(item)
                End If
            Next

            script = GetTabsScript(instanceVarName, expandedOutlines)

            Return script
        End Function

        Private Function GetTabsScript(ByVal instanceVarName As String, ByVal _outlines As OpmlOutlines) As String

            Dim js As New StringBuilder("")
            Dim tabCounter As Integer = -1
            For Each item As OpmlOutline In _outlines
                If (item.Category.StartsWith("Tab")) Then
                    tabCounter = tabCounter + 1
                    Dim tabVarName As String = instanceVarName & "tab" + tabCounter.ToString()

                    ' Create a call to the "addTab" method
                    ' addTab accepts one parameter -- a TabInfo object
                    ' Here the TabInfo object is dynamically created
                    ' with the parameters  Label, Url and Template
                    js.Append("var " + tabVarName + " = new TabInfo('" + item.Text + "',")

                    If (item.Type = "none") Then
                        js.Append("''")
                    Else
                        js.Append("'" + item.XmlUrl.AbsoluteUri + "'")
                    End If

                    ' Template detection
                    ' The category field indicates if the outline node is a tab, section or category
                    ' If the field value includes a / character, then portion of the value to the right of /
                    ' contains the name of the template that should be used for that tab/section/category
                    ' and its children
                    If (item.Category.IndexOf("/") > 0) Then
                        js.Append(",'" + item.Category.Substring(item.Category.IndexOf("/") + 1) + "'")
                    End If

                    js.Append(");")

                    If (Not item.Outlines Is Nothing) Then
                        js.Append(GetSectionsScript(item.Outlines, tabVarName))

                    End If
                    js.Append(instanceVarName & "tabs[" & instanceVarName & "tabs.length] = " + tabVarName + ";")
                End If
            Next

            Return js.ToString()
        End Function

        Private Function GetSectionsScript(ByVal _outlines As OpmlOutlines, ByVal tabVarName As String) As String

            Dim js As New StringBuilder("")
            Dim sectionCounter As Integer = -1
            For Each item As OpmlOutline In _outlines
                If (item.Category.StartsWith("Section")) Then
                    sectionCounter = sectionCounter + 1
                    Dim sectionVarName As String = tabVarName + "_" + sectionCounter.ToString()
                    Dim sectionUrl As String = ""

                    If (Not item.XmlUrl Is Nothing) Then
                        sectionUrl = ", '" + item.XmlUrl.AbsoluteUri + "'"
                    End If
                    ' Create a call to the addSection method
                    ' addSection accepts one parameter -- a SectionInfo object
                    ' Here the SectionInfo object is dyncamically created
                    ' with the parameters Label, Url and Template
                    ' A section Url is the Url called for obtaining search results
                    ' If the Url contains a [KEYWORD] token, the user's search keyword
                    ' is substituted for the token. If no token exists then &keyword={keyword value}
                    ' is appended to the Url
                    js.Append("var " + sectionVarName + " = " + tabVarName + ".addSection(new SectionInfo('" + item.Text + "'" + sectionUrl)

                    ' Template detection
                    ' The category field indicates if the outline node is a tab, section or category
                    ' If the field value includes a / character, then portion of the value to the right of /
                    ' contains the name of the template that should be used for that section/category
                    ' and its children
                    If (item.Category.IndexOf("/") > 0) Then
                        js.Append(",'" + item.Category.Substring(item.Category.IndexOf("/") + 1) + "'")
                    End If

                    js.Append("));")

                    If (Not item.Outlines Is Nothing) Then
                        Dim counter As Integer = -1
                        js.Append(GetCategoriesScript(item.Outlines, sectionVarName, -1, counter))
                    End If
                End If
            Next

            Return js.ToString()
        End Function

        Private Function GetCategoriesScript(ByVal _outlines As OpmlOutlines, ByVal sectionVarName As String, ByVal depth As Integer, ByRef counter As Integer) As String

            Dim js As New StringBuilder("")
            depth = depth + 1
            For Each item As OpmlOutline In _outlines
                If (item.Category.StartsWith("Category")) Then
                    counter = counter + 1
                    js.Append(sectionVarName + ".addCategory(new CategoryInfo('" + item.Text + "','" + item.XmlUrl.AbsoluteUri + "'," + depth.ToString())

                    ' Template detection
                    ' The category field indicates if the outline node is a tab, section or category
                    ' If the field value includes a / character, then portion of the value to the right of /
                    ' contains the name of the template that should be used for that category
                    If (item.Category.IndexOf("/") > 0) Then
                        js.Append(",'" + item.Category.Substring(item.Category.IndexOf("/") + 1) + "'")
                    End If

                    js.Append("));")

                    ' If the Category field includes "Default" in its list of values,
                    ' the item is marked as the default category
                    If (item.Category.IndexOf("Default") > -1) Then
                        js.Append(sectionVarName + ".setDefaultCategory(" + counter.ToString() + ");")
                    End If

                    If (Not item.Outlines Is Nothing) Then
                        js.Append(GetCategoriesScript(item.Outlines, sectionVarName, depth, counter))
                    End If
                End If

            Next

            Return js.ToString()
        End Function

#End Region

    End Class
End Namespace
