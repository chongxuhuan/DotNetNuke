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

Imports System.Net
Imports System.Web
Imports System.IO

Imports DotNetNuke.Data
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.Services.Upgrade

Namespace DotNetNuke.Common.Utilities

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Common.Utilities
    ''' Project:    DotNetNuke
    ''' Class:      HtmlUtils
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' HtmlUtils is a Utility class that provides Html Utility methods
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/16/2004	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class HtmlUtils

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Clean removes any HTML Tags, Entities (and optionally any punctuation) from
        ''' a string
        ''' </summary>
        ''' <remarks>
        ''' Encoded Tags are getting decoded, as they are part of the content!
        ''' </remarks>
        ''' <param name="HTML">The Html to clean</param>
        ''' <param name="RemovePunctuation">A flag indicating whether to remove punctuation</param>
        ''' <returns>The cleaned up string</returns>
        ''' <history>
        '''		[cnurse]	11/16/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function Clean(ByVal HTML As String, ByVal RemovePunctuation As Boolean) As String

            'First remove any HTML Tags ("<....>")
            HTML = StripTags(HTML, True)

            'Second replace any HTML entities (&nbsp; &lt; etc) through their char symbol
            HTML = System.Web.HttpUtility.HtmlDecode(HTML)

            'Thirdly remove any punctuation
            If RemovePunctuation Then
                HTML = StripPunctuation(HTML, True)
            End If

            'Finally remove extra whitespace
            HTML = StripWhiteSpace(HTML, True)

            Return HTML

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Formats an Email address
        ''' </summary>
        ''' <param name="Email">The email address to format</param>
        ''' <returns>The formatted email address</returns>
        ''' <history>
        '''		[cnurse]	09/29/2005	moved from Globals
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FormatEmail(ByVal Email As String) As String
            Return FormatEmail(Email, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Formats an Email address
        ''' </summary>
        ''' <param name="Email">The email address to format</param>
        ''' <param name="cloak">A flag that indicates whether the text should be cloaked</param>
        ''' <returns>The formatted email address</returns>
        ''' <history>
        '''		[cnurse]	09/29/2005	moved from Globals
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FormatEmail(ByVal Email As String, ByVal cloak As Boolean) As String
            FormatEmail = ""

            If Not String.IsNullOrEmpty(Email) AndAlso Not String.IsNullOrEmpty(Email.Trim) Then
                If Email.IndexOf("@") <> -1 Then
                    FormatEmail = "<a href=""mailto:" & Email & """>" & Email & "</a>"
                Else
                    FormatEmail = Email
                End If
            End If

            If cloak Then
                FormatEmail = CloakText(FormatEmail)
            End If

            Return FormatEmail
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatText replaces <br/> tags by LineFeed characters
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="HTML">The HTML content to clean up</param>
        ''' <returns>The cleaned up string</returns>
        ''' <history>
        '''		[cnurse]	12/13/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FormatText(ByVal HTML As String, ByVal RetainSpace As Boolean) As String

            'Match all variants of <br> tag (<br>, <BR>, <br/>, including embedded space
            Dim brMatch As String = "\s*<\s*[bB][rR]\s*/\s*>\s*"

            'Set up Replacement String
            Dim RepString As String
            If RetainSpace Then
                RepString = " "
            Else
                RepString = ""
            End If

            'Replace Tags by replacement String and return mofified string
            Return System.Text.RegularExpressions.Regex.Replace(HTML, brMatch, ControlChars.Lf)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Format a domain name including link
        ''' </summary>
        ''' <param name="Website">The domain name to format</param>
        ''' <returns>The formatted domain name</returns>
        ''' <history>
        '''		[cnurse]	09/29/2005	moved from Globals
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FormatWebsite(ByVal Website As Object) As String

            FormatWebsite = ""

            If Not IsDBNull(Website) Then
                If Trim(Website.ToString()) <> "" Then
                    If Convert.ToBoolean(InStr(1, Website.ToString(), ".")) Then
                        FormatWebsite = "<a href=""" & IIf(Convert.ToBoolean(InStr(1, Website.ToString(), "://")), "", "http://").ToString & Website.ToString() & """>" & Website.ToString() & "</a>"
                    Else
                        FormatWebsite = Website.ToString()
                    End If
                End If
            End If

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Shorten returns the first (x) characters of a string
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="txt">The text to reduces</param>
        ''' <param name="length">The max number of characters to return</param>
        ''' <param name="suffix">An optional suffic to append to the shortened string</param>
        ''' <returns>The shortened string</returns>
        ''' <history>
        '''		[cnurse]	11/16/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function Shorten(ByVal txt As String, ByVal length As Integer, ByVal suffix As String) As String
            Dim results As String
            If txt.Length > length Then
                results = txt.Substring(0, length) & suffix
            Else
                results = txt
            End If
            Return results
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StripEntities removes the HTML Entities from the content
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="HTML">The HTML content to clean up</param>
        ''' <param name="RetainSpace">Indicates whether to replace the Entity by a space (true) or nothing (false)</param>
        ''' <returns>The cleaned up string</returns>
        ''' <history>
        '''		[cnurse]	11/16/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("This method has been deprecated. Please use System.Web.HtmlUtility.HtmlDecode")> _
        Public Shared Function StripEntities(ByVal HTML As String, ByVal RetainSpace As Boolean) As String

            'Set up Replacement String
            Dim RepString As String
            If RetainSpace Then
                RepString = " "
            Else
                RepString = ""
            End If

            'Replace Entities by replacement String and return mofified string
            Return System.Text.RegularExpressions.Regex.Replace(HTML, "&[^;]*;", RepString)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StripTags removes the HTML Tags from the content
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="HTML">The HTML content to clean up</param>
        ''' <param name="RetainSpace">Indicates whether to replace the Tag by a space (true) or nothing (false)</param>
        ''' <returns>The cleaned up string</returns>
        ''' <history>
        '''		[cnurse]	11/16/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function StripTags(ByVal HTML As String, ByVal RetainSpace As Boolean) As String

            'Set up Replacement String
            Dim RepString As String
            If RetainSpace Then
                RepString = " "
            Else
                RepString = ""
            End If

            'Replace Tags by replacement String and return mofified string
            Return System.Text.RegularExpressions.Regex.Replace(HTML, "<[^>]*>", RepString)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StripPunctuation removes the Punctuation from the content
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="HTML">The HTML content to clean up</param>
        ''' <param name="RetainSpace">Indicates whether to replace the Punctuation by a space (true) or nothing (false)</param>
        ''' <returns>The cleaned up string</returns>
        ''' <history>
        '''		[cnurse]	11/16/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function StripPunctuation(ByVal HTML As String, ByVal RetainSpace As Boolean) As String

            'Create Regular Expression objects
            Dim punctuationMatch As String = "[~!#\$%\^&*\(\)-+=\{\[\}\]\|;:\x22'<,>\.\?\\\t\r\v\f\n]"
            Dim afterRegEx As New System.Text.RegularExpressions.Regex(punctuationMatch & "\s")
            Dim beforeRegEx As New System.Text.RegularExpressions.Regex("\s" & punctuationMatch)

            'Define return string
            Dim retHTML As String = HTML & " "  'Make sure any punctuation at the end of the String is removed

            'Set up Replacement String
            Dim RepString As String
            If RetainSpace Then
                RepString = " "
            Else
                RepString = ""
            End If

            While beforeRegEx.IsMatch(retHTML)
                'Strip punctuation from beginning of word
                retHTML = beforeRegEx.Replace(retHTML, RepString)
            End While

            While afterRegEx.IsMatch(retHTML)
                'Strip punctuation from end of word
                retHTML = afterRegEx.Replace(retHTML, RepString)
            End While

            ' Return modified string
            Return retHTML
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StripWhiteSpace removes the WhiteSpace from the content
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="HTML">The HTML content to clean up</param>
        ''' <param name="RetainSpace">Indicates whether to replace the WhiteSpace by a space (true) or nothing (false)</param>
        ''' <returns>The cleaned up string</returns>
        ''' <history>
        '''		[cnurse]	12/13/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function StripWhiteSpace(ByVal HTML As String, ByVal RetainSpace As Boolean) As String

            'Set up Replacement String
            Dim RepString As String
            If RetainSpace Then
                RepString = " "
            Else
                RepString = ""
            End If

            'Replace Tags by replacement String and return mofified string
            If HTML = Null.NullString Then
                Return Null.NullString
            Else
                Return System.Text.RegularExpressions.Regex.Replace(HTML, "\s+", RepString)
            End if
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StripNonWord removes any Non-Word Character from the content
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="HTML">The HTML content to clean up</param>
        ''' <param name="RetainSpace">Indicates whether to replace the Non-Word Character by a space (true) or nothing (false)</param>
        ''' <returns>The cleaned up string</returns>
        ''' <history>
        '''		[cnurse]	1/28/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function StripNonWord(ByVal HTML As String, ByVal RetainSpace As Boolean) As String

            'Set up Replacement String
            Dim RepString As String
            If RetainSpace Then
                RepString = " "
            Else
                RepString = ""
            End If

            'Replace Tags by replacement String and return mofified string
            If HTML Is Nothing Then
                Return HTML
            Else
                Return System.Text.RegularExpressions.Regex.Replace(HTML, "\W*", RepString)
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' WriteError outputs an Error Message during Install/Upgrade etc
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="response">The ASP.Net Response object</param>
        ''' <param name="file">The filename where the Error Occurred</param>
        ''' <param name="message">The error message</param>
        ''' <history>
        '''		[cnurse]	02/21/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub WriteError(ByVal response As HttpResponse, ByVal file As String, ByVal message As String)

            response.Write("<h2>Error Details</h2>")
            response.Write("<table cellspacing=0 cellpadding=0 border=0>")
            response.Write("<tr><td><b>File</b></td><td><b>" & file & "</b></td></tr>")
            response.Write("<tr><td><b>Error</b>&nbsp;&nbsp;</td><td><b>" & message & "</b></td></tr>")
            response.Write("</table>")
            response.Write("<br><br>")
            response.Flush()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' WriteFeedback outputs a Feedback Line during Install/Upgrade etc
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="response">The ASP.Net Response object</param>
        ''' <param name="indent">The indent for this feedback message</param>
        ''' <param name="message">The feedback message</param>
        ''' <history>
        '''		[cnurse]	02/21/2005	created
        '''     [gve] 	    07/14/2006	added extra overload (showtime) to show or hide the upgrade runtime
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub WriteFeedback(ByVal response As HttpResponse, ByVal indent As Int32, ByVal message As String)

            WriteFeedback(response, indent, message, True)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' WriteFeedback outputs a Feedback Line during Install/Upgrade etc
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="response">The ASP.Net Response object</param>
        ''' <param name="indent">The indent for this feedback message</param>
        ''' <param name="message">The feedback message</param>
        ''' <param name="showtime">Show the timespan before the message</param>
        ''' <history>
        '''		[cnurse]	02/21/2005	created
        '''     [gve] 	    07/14/2006	added extra overload (showtime) to show or hide the upgrade runtime
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub WriteFeedback(ByVal response As HttpResponse, ByVal indent As Int32, ByVal message As String, ByVal showtime As Boolean)

            Dim showInstallationMessages As Boolean = True
            Dim ConfigSetting As String = Config.GetSetting("ShowInstallationMessages")
            If Not ConfigSetting Is Nothing Then
                showInstallationMessages = Boolean.Parse(ConfigSetting)
            End If
            If showInstallationMessages Then
                'Get the time of the feedback
                Dim timeElapsed As TimeSpan = Upgrade.RunTime

                Dim strMessage As String = ""
                If showtime = True Then
                    strMessage += timeElapsed.ToString.Substring(0, timeElapsed.ToString.LastIndexOf(".") + 4) & " -"
                End If

                For i As Integer = 0 To indent
                    strMessage += "&nbsp;"
                Next
                strMessage += message
                response.Write(strMessage)
                response.Flush()
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' WriteFooter outputs the Footer during Install/Upgrade etc
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="response">The ASP.Net Response object</param>
        ''' <history>
        '''		[cnurse]	02/21/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub WriteFooter(ByVal response As HttpResponse)

            response.Write("</body>")
            response.Write("</html>")
            response.Flush()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' WriteHeader outputs the Header during Install/Upgrade etc
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="response">The ASP.Net Response object</param>
        ''' <param name="mode">The mode Install/Upgrade etc</param>
        ''' <history>
        '''		[cnurse]	02/21/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub WriteHeader(ByVal response As HttpResponse, ByVal mode As String)
            'Set Response buffer to False
            response.Buffer = False

            ' create an install page if it does not exist already
            If Not File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.htm")) Then
                If File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.template.htm")) Then
                    File.Copy(System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.template.htm"), System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.htm"))
                End If
            End If

            ' read install page and insert into response stream
            If File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.htm")) Then
                response.Write(FileSystemUtils.ReadFile(System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.htm")))
            End If

            Select Case mode
                Case "install"
                    response.Write("<h1>Installing DotNetNuke</h1>")
                Case "upgrade"
                    response.Write("<h1>Upgrading DotNetNuke</h1>")
                Case "addPortal"
                    response.Write("<h1>Adding New Portal</h1>")
                Case "installResources"
                    response.Write("<h1>Installing Resources</h1>")
                Case "executeScripts"
                    response.Write("<h1>Executing Scripts</h1>")
                Case "none"
                    response.Write("<h1>Nothing To Install At This Time</h1>")
                Case "noDBVersion"
                    response.Write("<h1>New DotNetNuke Database</h1>")
                Case "error"
                    response.Write("<h1>Error Installing DotNetNuke</h1>")
                Case Else
                    response.Write("<h1>" & mode & "</h1>")
            End Select
            response.Flush()
        End Sub

        Public Shared Sub WriteSuccessError(ByVal response As HttpResponse, ByVal bSuccess As Boolean)
            If bSuccess Then
                WriteFeedback(response, 0, "<font color='green'>Success</font><br>", False)
            Else
                WriteFeedback(response, 0, "<font color='red'>Error!</font><br>", False)
            End If
        End Sub

        Public Shared Sub WriteScriptSuccessError(ByVal response As HttpResponse, ByVal bSuccess As Boolean, ByVal strLogFile As String)
            If bSuccess Then
                WriteFeedback(response, 0, "<font color='green'>Success</font><br>", False)
            Else
                WriteFeedback(response, 0, "<font color='red'>Error! (see " & strLogFile & " for more information)</font><br>", False)
            End If
        End Sub

    End Class
End Namespace