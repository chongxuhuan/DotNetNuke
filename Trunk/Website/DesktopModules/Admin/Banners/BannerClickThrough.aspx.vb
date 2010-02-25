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
Imports DotNetNuke.Services.Vendors

Namespace DotNetNuke.Modules.Admin.Vendors

    Partial Class BannerClickThrough

        Inherits Framework.PageBase

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'exit without incrementing count if page is indexed by crawler
            If Request.Browser.Crawler = True Then Exit Sub

            Try
                If (Not Request.QueryString("vendorid") Is Nothing) And (Not Request.QueryString("bannerid") Is Nothing) Then
                    Dim intVendorId As Integer = -1
                    If IsNumeric(Request.QueryString("vendorid")) Then
                        intVendorId = Integer.Parse(Request.QueryString("vendorid"))
                    End If
                    Dim intBannerId As Integer = -1
                    If IsNumeric(Request.QueryString("bannerid")) Then
                        intBannerId = Integer.Parse(Request.QueryString("bannerid"))
                    End If

                    Dim intPortalId As Integer = -1
                    If (Not Request.QueryString("portalid") Is Nothing) Then
                        If IsNumeric(Request.QueryString("portalid")) Then
                            intPortalId = Integer.Parse(Request.QueryString("portalid"))
                        End If
                    Else
                        intPortalId = GetPortalSettings.PortalId
                    End If

                    If intBannerId <> -1 And intVendorId <> -1 And intPortalId <> -1 Then
                        Dim strURL As String = "~/" & glbDefaultPage

                        Dim objBanners As New BannerController
                        Dim objBanner As BannerInfo = objBanners.GetBanner(intBannerId, intVendorId, intPortalId)
                        If Not objBanner Is Nothing Then
                            If objBanners.IsBannerActive(objBanner) Then
                                If Not Null.IsNull(objBanner.URL) Then
                                    strURL = Common.Globals.LinkClick(objBanner.URL, -1, -1, False)
                                Else
                                    Dim objVendors As New VendorController
                                    Dim objVendor As VendorInfo = objVendors.GetVendor(objBanner.VendorId, intPortalId)
                                    If objVendor Is Nothing Then
                                        objVendor = objVendors.GetVendor(objBanner.VendorId, Null.NullInteger)
                                    End If
                                    If Not objVendor Is Nothing Then
                                        If objVendor.Website <> "" Then
                                            strURL = AddHTTP(objVendor.Website)
                                        End If
                                    End If
                                End If

                                objBanners.UpdateBannerClickThrough(intBannerId, intVendorId)
                            End If
                        Else
                            If Not Request.UrlReferrer Is Nothing Then
                                strURL = Request.UrlReferrer.ToString
                            End If
                        End If

                        Response.Redirect(strURL, True)
                    End If
                End If

            Catch exc As Exception    'Page failed to load
                ProcessPageLoadException(exc)
            End Try
        End Sub

    End Class

End Namespace
