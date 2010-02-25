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

Imports System.Collections.Specialized
Imports System.Text.RegularExpressions
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Common.Utilities

    Public Class UrlUtils

#Region "Public Methods"

        Public Shared Function DecryptParameter(ByVal Value As String) As String
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Return DecryptParameter(Value, _portalSettings.GUID.ToString)
        End Function

        Public Shared Function DecryptParameter(ByVal Value As String, ByVal encryptionKey As String) As String
            Dim objSecurity As New PortalSecurity

            '[DNN-8257] - Can't do URLEncode/URLDecode as it introduces issues on decryption (with / = %2f), so we use a modifed Base64
            Value = Value.Replace("_", "/")
            Value = Value.Replace("-", "+")
            Value = Value.Replace("%3d", "=")

            Return objSecurity.Decrypt(encryptionKey, Value)
        End Function

        Public Shared Function EncryptParameter(ByVal Value As String) As String
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Return EncryptParameter(Value, _portalSettings.GUID.ToString)
        End Function

        Public Shared Function EncryptParameter(ByVal Value As String, ByVal encryptionKey As String) As String
            Dim objSecurity As New PortalSecurity
            Dim strParameter As String = objSecurity.Encrypt(encryptionKey, Value)

            '[DNN-8257] - Can't do URLEncode/URLDecode as it introduces issues on decryption (with / = %2f), so we use a modifed Base64
            strParameter = strParameter.Replace("/", "_")
            strParameter = strParameter.Replace("+", "-")
            strParameter = strParameter.Replace("=", "%3d")
            Return strParameter
        End Function

        Public Shared Function GetParameterName(ByVal Pair As String) As String
            Dim arrNameValue As String() = Pair.Split(Convert.ToChar("="))
            Return arrNameValue(0)
        End Function

        Public Shared Function GetParameterValue(ByVal Pair As String) As String
            Dim arrNameValue As String() = Pair.Split(Convert.ToChar("="))
            If arrNameValue.Length > 1 Then
                Return arrNameValue(1)
            Else
                Return ""
            End If
        End Function

        ''' <summary>
        ''' getQSParamsForNavigateURL builds up a new querystring. This is necessary
        ''' in order to prep for navigateUrl.
        ''' we don't ever want a tabid, a ctl and a language parameter in the qs
        ''' either, the portalid param is not allowed when the tab is a supertab 
        ''' (because NavigateUrl adds the portalId param to the qs)
        ''' </summary>
        ''' <history>
        '''     [erikvb]   20070814    added
        ''' </history>
        Public Shared Function GetQSParamsForNavigateURL() As String()
            Dim returnValue As String = ""
            Dim coll As NameValueCollection = HttpContext.Current.Request.QueryString
            Dim arrKeys(), arrValues() As String
            arrKeys = coll.AllKeys
            For i As Integer = 0 To arrKeys.GetUpperBound(0)
                If arrKeys(i) IsNot Nothing Then
                    Select Case arrKeys(i).ToLower
                        Case "tabid", "ctl", "language"
                            'skip parameter
                        Case Else
                            If (arrKeys(i).ToLower = "portalid") And GetPortalSettings.ActiveTab.IsSuperTab Then
                                'skip parameter
                                'navigateURL adds portalid to querystring if tab is superTab
                            Else
                                arrValues = coll.GetValues(i)
                                For j As Integer = 0 To arrValues.GetUpperBound(0)
                                    If returnValue <> "" Then returnValue += "&"
                                    returnValue += arrKeys(i) + "=" + arrValues(j)
                                Next
                            End If
                    End Select
                End If
            Next

            'return the new querystring as a string array
            Return returnValue.Split("&"c)
        End Function

        Public Shared Sub OpenNewWindow(ByVal page As Page, ByVal type As Type, ByVal url As String)
            page.ClientScript.RegisterStartupScript(type, "DotNetNuke.NewWindow", String.Format("<script>window.open('{0}','new')</script>", url))
        End Sub

        Public Shared Function ReplaceQSParam(ByVal strURL As String, ByVal strParam As String, ByVal strNewValue As String) As String
            If Host.UseFriendlyUrls Then
                Return Regex.Replace(strURL, "(.*)(" + strParam + "/)([^/]+)(/.*)", "$1$2" + strNewValue + "$4", RegexOptions.IgnoreCase)
            Else
                Return Regex.Replace(strURL, "(.*)(&|\?)(" + strParam + "=)([^&\?]+)(.*)", "$1$2$3" + strNewValue + "$5", RegexOptions.IgnoreCase)
            End If
        End Function

        Public Shared Function StripQSParam(ByVal strURL As String, ByVal strParam As String) As String
            If Host.UseFriendlyUrls Then
                'Dim r As New Regex("(.*)(language/[^/]+/)(.*)", RegexOptions.IgnoreCase)
                Return Regex.Replace(strURL, "(.*)(" + strParam + "/[^/]+/)(.*)", "$1$3", RegexOptions.IgnoreCase)
            Else
                Return Regex.Replace(Regex.Replace(strURL, "(.*)(&|\?)(" + strParam + "=)([^&\?]+)([&\?])?(.*)", "$1$2$6", RegexOptions.IgnoreCase), "(.*)([&\?]$)", "$1")
            End If
        End Function

        Public Shared Function EncodeParameter(ByVal Value As String) As String
            Dim arrBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(Value)
            Value = System.Convert.ToBase64String(arrBytes)
            Value = Value.Replace("+", "-").Replace("/", "_").Replace("=", "$")
            Return Value
        End Function

        Public Shared Function DecodeParameter(ByVal Value As String) As String
            Value = Value.Replace("-", "+").Replace("_", "/").Replace("$", "=")
            Dim arrBytes As Byte() = Convert.FromBase64String(Value)
            Return System.Text.Encoding.UTF8.GetString(arrBytes)
        End Function

#End Region

        <Obsolete("Replaced in DNN 5.0.1 by OpenNewWindow(Page, Type, String)")> _
        Public Shared Sub OpenNewWindow(ByVal Url As String)
            HttpContext.Current.Response.Write("<script>window.open('" & Url & "', 'new');</script>")
        End Sub

    End Class

End Namespace
