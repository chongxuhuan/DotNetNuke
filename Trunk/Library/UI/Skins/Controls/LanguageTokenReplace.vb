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


Namespace DotNetNuke.UI.Skins.Controls

    Public Class LanguageTokenReplace
        Inherits DotNetNuke.Services.Tokens.TokenReplace

        Private _resourceFile As String

        Public Property resourceFile() As String
            Get
                Return _resourceFile
            End Get
            Set(ByVal value As String)
                _resourceFile = value
            End Set
        End Property

        ' see http://support.dotnetnuke.com/issue/ViewIssue.aspx?id=6505
        Public Sub New()
            MyBase.new(Services.Tokens.Scope.NoSettings)
            Me.UseObjectLessExpression = True
            Me.PropertySource(ObjectLessToken) = New LanguagePropertyAccess(Me, GetPortalSettings)
        End Sub
    End Class

    Public Class LanguagePropertyAccess
        Implements DotNetNuke.Services.Tokens.IPropertyAccess

        Public objParent As LanguageTokenReplace = Nothing
        Private objPortal As PortalSettings

        Public Sub New(ByVal parent As LanguageTokenReplace, ByVal settings As PortalSettings)
            objPortal = settings
            objParent = parent
        End Sub

        ''' <summary>
        ''' getQSParams builds up a new querystring. This is necessary
        ''' in order to prep for navigateUrl.
        ''' we don't ever want a tabid, a ctl and a language parameter in the qs
        ''' also, the portalid param is not allowed when the tab is a supertab 
        ''' (because NavigateUrl adds the portalId param to the qs)
        ''' </summary>
        ''' <history>
        '''     [erikvb]   20070814    added
        ''' </history>
        Private Function getQSParams(ByVal newLanguage As String) As String()
            Dim returnValue As String = ""
            Dim coll As NameValueCollection = HttpContext.Current.Request.QueryString
            Dim arrKeys(), arrValues() As String
            arrKeys = coll.AllKeys
            For i As Integer = 0 To arrKeys.GetUpperBound(0)
                If arrKeys(i) IsNot Nothing Then
                    Select Case arrKeys(i).ToLowerInvariant()
                        Case "tabid", "ctl", "language"
                            'skip parameter
                        Case Else
                            If (arrKeys(i).ToLowerInvariant = "portalid") And objPortal.ActiveTab.IsSuperTab Then
                                'skip parameter
                                'navigateURL adds portalid to querystring if tab is superTab
                            Else
                                arrValues = coll.GetValues(i)
                                For j As Integer = 0 To arrValues.GetUpperBound(0)
                                    If returnValue <> "" Then returnValue += "&"
                                    returnValue += arrKeys(i) + "=" + HttpUtility.HtmlEncode(arrValues(j))
                                Next
                            End If
                    End Select
                End If
            Next

            Dim _Settings As PortalSettings = PortalController.GetCurrentPortalSettings
            If Localization.GetLocales(_Settings.PortalId).Count > 1 AndAlso (_Settings.EnableUrlLanguage = False) Then
                'because useLanguageInUrl is false, navigateUrl won't add a language param, so we need to do that ourselves
                If returnValue <> "" Then returnValue += "&"
                returnValue += "language=" + newLanguage
            End If

            'return the new querystring as a string array
            Return returnValue.Split("&"c)
        End Function


        ''' <summary>
        ''' newUrl returns the new URL based on the new language. 
        ''' Basically it is just a call to NavigateUrl, with stripped qs parameters
        ''' </summary>
        ''' <param name="newLanguage"></param>
        ''' <history>
        '''     [erikvb]   20070814    added
        ''' </history>
        Private Function newUrl(ByVal newLanguage As String) As String
            Dim objSecurity As New PortalSecurity
            Return objSecurity.InputFilter(NavigateURL(objPortal.ActiveTab.TabID, objPortal.ActiveTab.IsSuperTab, objPortal, HttpContext.Current.Request.QueryString("ctl"), newLanguage, getQSParams(newLanguage)), PortalSecurity.FilterFlag.NoScripting)
        End Function


        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As Entities.Users.UserInfo, ByVal CurrentScope As Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements Services.Tokens.IPropertyAccess.GetProperty
            Select Case strPropertyName.ToLowerInvariant()
                Case "url"
                    Return newUrl(objParent.Language)
                Case "flagsrc"
                    Return "/" + objParent.Language + ".gif"
                Case "selected"
                    Return (objParent.Language = CultureInfo.CurrentCulture.Name).ToString
                Case "label"
                    Return Localization.GetString("Label", objParent.resourceFile)
                Case "i"
                    Return ResolveUrl("~/images/Flags")
                Case "p"
                    Return ResolveUrl(FileSystemUtils.RemoveTrailingSlash(objPortal.HomeDirectory))
                Case "s"
                    Return ResolveUrl(FileSystemUtils.RemoveTrailingSlash(objPortal.ActiveTab.SkinPath))
                Case "g"
                    Return ResolveUrl("~/portals/" + Common.glbHostSkinFolder)

                Case Else
                    PropertyNotFound = True : Return String.Empty
            End Select
        End Function

        Public ReadOnly Property Cacheability() As Services.Tokens.CacheLevel Implements Services.Tokens.IPropertyAccess.Cacheability
            Get
                Return Services.Tokens.CacheLevel.fullyCacheable
            End Get
        End Property
    End Class

End Namespace
