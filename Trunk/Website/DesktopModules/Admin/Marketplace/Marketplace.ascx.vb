' 
'' DotNetNuke® - http://www.dotnetnuke.com 
'' Copyright (c) 2002-2010 
'' by DotNetNuke Corporation 
'' 
'' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'' to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
'' 
'' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
'' of the Software. 
'' 
'' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'' DEALINGS IN THE SOFTWARE. 
' 
Imports System.Xml
Imports System.Xml.XPath
Imports System.IO
Imports System.Net

Namespace DotNetNuke.Modules.Admin.Marketplace
    Partial Class Marketplace
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

        Private Const XSL_PATH As String = "RSS91.xsl"
        Private Const PRODUCT_XMLNODE_PATH As String = "//channel/item"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    PopulateControls()
                End If

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub PopulateControls()
            Dim oListItem As ListItem
            Dim oXMLDocument As XmlDocument
            Dim oNodes As XmlNodeList

            Try
                oXMLDocument = New XmlDocument
                oXMLDocument.Load(AppDomain.CurrentDomain.SetupInformation.ApplicationBase & "DesktopModules\Admin\Marketplace\ListItems.xml")

                'load search-by
                oNodes = oXMLDocument.SelectNodes("/listitems/feedtypes/feedtype")

                For Each oNode As XmlNode In oNodes
                    oListItem = New ListItem
                    oListItem.Value = oNode.ChildNodes.Item(0).InnerText 'ID
                    oListItem.Text = oNode.ChildNodes.Item(1).InnerText  'Name
                    cboSearchFor.Items.Add(oListItem)
                Next

                'load categories
                oNodes = oXMLDocument.SelectNodes("/listitems/categories/category")

                For Each oNode As XmlNode In oNodes
                    oListItem = New ListItem
                    oListItem.Value = oNode.ChildNodes.Item(0).InnerText 'ID
                    oListItem.Text = oNode.ChildNodes.Item(1).InnerText  'Name
                    cboCategories.Items.Add(oListItem)
                Next

            Catch oExc As Exception
                Throw oExc
            End Try
        End Sub

        Protected Sub cmdGo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdGo.Click
            Dim oRequest As HttpWebRequest
            Dim oResponse As WebResponse
            Dim oStream As Stream
            Dim oReader As XmlTextReader
            Dim oXMLDocument As XPathDocument

            Dim sFeedType As String
            Dim sCategoryID As String
            Dim sRequest As String
            
            Try
                sFeedType = cboSearchFor.SelectedValue
                sCategoryID = cboCategories.SelectedValue
                sRequest = "http://www.snowcovered.com/Snowcovered2/DesktopModules/PortalStore/rss.aspx?feedtype=" & sFeedType & "&categoryid=" & sCategoryID

                ' make remote request
                Try
                    oRequest = CType(WebRequest.Create(sRequest), HttpWebRequest)
                    oRequest.Timeout = 10000 ' 10 seconds
                    oResponse = oRequest.GetResponse()
                    oStream = oResponse.GetResponseStream()

                Catch oExc As System.Exception
                    Throw oExc
                End Try

                ' load XML document
                oReader = New XmlTextReader(oStream)
                oReader.XmlResolver = Nothing
                oXMLDocument = New XPathDocument(oReader)
                Dim nav As XPathNavigator = oXMLDocument.CreateNavigator()
                xmlRSS.XPathNavigator = nav
                'xmlRSS.Document = oXMLDocument
                xmlRSS.TransformSource = XSL_PATH  'apply transformation to render output

                'display message if no products found
                If nav.Select(PRODUCT_XMLNODE_PATH).Count = 0 Then
                    lblMessage.Text = Localization.GetString("NoProductsFound.Text", Me.LocalResourceFile)
                End If

            Catch oExc As Exception
                Throw oExc
            End Try
        End Sub
    End Class

End Namespace
