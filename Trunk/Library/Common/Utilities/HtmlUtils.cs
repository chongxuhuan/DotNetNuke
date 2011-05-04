#region Copyright

// 
// DotNetNuke� - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

#region Usings

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using DotNetNuke.Services.Upgrade;
using System.Collections.Generic;

#endregion

namespace DotNetNuke.Common.Utilities
{
    /// -----------------------------------------------------------------------------
    /// Namespace:  DotNetNuke.Common.Utilities
    /// Project:    DotNetNuke
    /// Class:      HtmlUtils
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// HtmlUtils is a Utility class that provides Html Utility methods
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/16/2004	documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class HtmlUtils
    {
        private static readonly Regex HtmlDetectionRegex = new Regex("<(.*\\s*)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Clean removes any HTML Tags, Entities (and optionally any punctuation) from
        /// a string
        /// </summary>
        /// <remarks>
        /// Encoded Tags are getting decoded, as they are part of the content!
        /// </remarks>
        /// <param name="HTML">The Html to clean</param>
        /// <param name="RemovePunctuation">A flag indicating whether to remove punctuation</param>
        /// <returns>The cleaned up string</returns>
        /// <history>
        ///		[cnurse]	11/16/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string Clean(string HTML, bool RemovePunctuation)
        {
            HTML = StripTags(HTML, true);
            HTML = HttpUtility.HtmlDecode(HTML);
            if (RemovePunctuation)
            {
                HTML = StripPunctuation(HTML, true);
            }
            HTML = StripWhiteSpace(HTML, true);
            return HTML;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   CleanWithTagInfo removes unspecified HTML Tags, Entities (and optionally any punctuation) from a string.
        /// </summary>
        /// <param name = "html"></param>
        /// <param name="tagsFilter"></param>
        /// <param name = "removePunctuation"></param>
        /// <returns>The cleaned up string</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [vnguyen]   09/02/2010   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string CleanWithTagInfo(string html, string tagsFilter, bool removePunctuation)
        {
            //First remove unspecified HTML Tags ("<....>")
            html = StripUnspecifiedTags(html, tagsFilter, true);

            //Second replace any HTML entities (&nbsp; &lt; etc) through their char symbol
            html = HttpUtility.HtmlDecode(html);

            //Thirdly remove any punctuation
            if (removePunctuation)
            {
                html = StripPunctuation(html, true);
            }

            //Finally remove extra whitespace
            html = StripWhiteSpace(html, true);

            return html;
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Formats an Email address
        /// </summary>
        /// <param name="Email">The email address to format</param>
        /// <returns>The formatted email address</returns>
        /// <history>
        ///		[cnurse]	09/29/2005	moved from Globals
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string FormatEmail(string Email)
        {
            return FormatEmail(Email, true);
        }

        public static string FormatEmail(string Email, bool cloak)
        {
            string formatEmail = "";
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Email.Trim()))
            {
                if (Email.IndexOf("@") != -1)
                {
                    formatEmail = "<a href=\"mailto:" + Email + "\">" + Email + "</a>";
                }
                else
                {
                    formatEmail = Email;
                }
            }
            if (cloak)
            {
                formatEmail = Globals.CloakText(formatEmail);
            }
            return formatEmail;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// FormatText replaces <br/> tags by LineFeed characters
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="HTML">The HTML content to clean up</param>
        /// <param name="RetainSpace">Whether ratain Space</param>
        /// <returns>The cleaned up string</returns>
        /// <history>
        ///		[cnurse]	12/13/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string FormatText(string HTML, bool RetainSpace)
        {
            string brMatch = "\\s*<\\s*[bB][rR]\\s*/\\s*>\\s*";
            return Regex.Replace(HTML, brMatch, Environment.NewLine);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Formats String as Html by replacing linefeeds by <br />
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "strText">Text to format</param>
        /// <returns>The formatted html</returns>
        /// <history>
        ///   [cnurse]	12/13/2004	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string ConvertToHtml(string strText)
        {
            string strHtml = strText;

            if (!string.IsNullOrEmpty(strHtml))
            {
                strHtml = strHtml.Replace("\n", "");
                strHtml = strHtml.Replace("\r", "<br />");
            }

            return strHtml;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Formats Html as text by removing <br /> tags and replacing by linefeeds
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "strHtml">Html to format</param>
        /// <returns>The formatted text</returns>
        /// <history>
        ///   [cnurse]	12/13/2004	Documented and modified to use HtmlUtils methods
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string ConvertToText(string strHtml)
        {
            string strText = strHtml;

            if (!string.IsNullOrEmpty(strText))
            {
                //First remove white space (html does not render white space anyway and it screws up the conversion to text)
                //Replace it by a single space
                strText = StripWhiteSpace(strText, true);

                //Replace all variants of <br> by Linefeeds
                strText = FormatText(strText, false);
            }


            return strText;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Format a domain name including link
        /// </summary>
        /// <param name="Website">The domain name to format</param>
        /// <returns>The formatted domain name</returns>
        /// <history>
        ///		[cnurse]	09/29/2005	moved from Globals
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string FormatWebsite(object Website)
        {
            string formatWebsite = "";
            if (Website != DBNull.Value)
            {
                if (!String.IsNullOrEmpty(Website.ToString().Trim()))
                {
                    if (Website.ToString().IndexOf(".") > -1)
                    {
                        formatWebsite = "<a href=\"" + (Website.ToString().IndexOf("://") > -1 ? "" : "http://") + Website + "\">" + Website + "</a>";
                    }
                    else
                    {
                        formatWebsite = Website.ToString();
                    }
                }
            }
            return formatWebsite;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Shorten returns the first (x) characters of a string
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="txt">The text to reduces</param>
        /// <param name="length">The max number of characters to return</param>
        /// <param name="suffix">An optional suffic to append to the shortened string</param>
        /// <returns>The shortened string</returns>
        /// <history>
        ///		[cnurse]	11/16/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string Shorten(string txt, int length, string suffix)
        {
            string results;
            if (txt.Length > length)
            {
                results = txt.Substring(0, length) + suffix;
            }
            else
            {
                results = txt;
            }
            return results;
        }

        [Obsolete("This method has been deprecated. Please use System.Web.HtmlUtility.HtmlDecode")]
        public static string StripEntities(string HTML, bool RetainSpace)
        {
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }
            return Regex.Replace(HTML, "&[^;]*;", RepString);
        }

        public static string StripTags(string HTML, bool RetainSpace)
        {
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }
            return Regex.Replace(HTML, "<[^>]*>", RepString);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   StripUnspecifiedTags removes the HTML tags from the content -- leaving behind the info 
        ///   for the specified HTML tags.
        /// </summary>
        /// <param name = "html"></param>
        /// <param name="specifiedTags"></param>
        /// <param name = "retainSpace"></param>
        /// <returns>The cleaned up string</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [vnguyen]   09/02/2010   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string StripUnspecifiedTags(string html, string specifiedTags, bool retainSpace)
        {
            var result = new StringBuilder();

            //Set up Replacement String
            string RepString = null;
            if (retainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }

            //Stripped HTML
            result.Append(Regex.Replace(html, "<[^>]*>", RepString));

            //Adding Tag info from specified tags
            foreach (Match m in Regex.Matches(html, "(?<=(" + specifiedTags + ")=)\"(?<a>.*?)\""))
            {
                if (m.Value.Length > 0)
                {
                    result.Append(" " + m.Value);
                }
            }

            return result.ToString();
        }


        public static string StripPunctuation(string HTML, bool RetainSpace)
        {
            string punctuationMatch = "[~!#\\$%\\^&*\\(\\)-+=\\{\\[\\}\\]\\|;:\\x22'<,>\\.\\?\\\\\\t\\r\\v\\f\\n]";
            var afterRegEx = new Regex(punctuationMatch + "\\s");
            var beforeRegEx = new Regex("\\s" + punctuationMatch);
            string retHTML = HTML + " ";
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }
            while (beforeRegEx.IsMatch(retHTML))
            {
                retHTML = beforeRegEx.Replace(retHTML, RepString);
            }
            while (afterRegEx.IsMatch(retHTML))
            {
                retHTML = afterRegEx.Replace(retHTML, RepString);
            }
            // Return modified string after trimming leading and ending quotation marks
            return retHTML.Trim('"');
        }

        public static string StripWhiteSpace(string HTML, bool RetainSpace)
        {
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }
            if (HTML == Null.NullString)
            {
                return Null.NullString;
            }
            else
            {
                return Regex.Replace(HTML, "\\s+", RepString);
            }
        }

        public static string StripNonWord(string HTML, bool RetainSpace)
        {
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }
            if (HTML == null)
            {
                return HTML;
            }
            else
            {
                return Regex.Replace(HTML, "\\W*", RepString);
            }
        }

        /// <summary>
        ///   Determines wether or not the passed in string contains any HTML tags
        /// </summary>
        /// <param name = "text">Text to be inspected</param>
        /// <returns>True for HTML and False for plain text</returns>
        /// <remarks>
        /// </remarks>
        public static bool IsHtml(string text)
        {
            if ((string.IsNullOrEmpty(text)))
            {
                return false;
            }

            return HtmlDetectionRegex.IsMatch(text);
        }

        public static void WriteError(HttpResponse response, string file, string message)
        {
            response.Write("<h2>Error Details</h2>");
            response.Write("<table cellspacing=0 cellpadding=0 border=0>");
            response.Write("<tr><td><b>File</b></td><td><b>" + file + "</b></td></tr>");
            response.Write("<tr><td><b>Error</b>&nbsp;&nbsp;</td><td><b>" + message + "</b></td></tr>");
            response.Write("</table>");
            response.Write("<br><br>");
            response.Flush();
        }

        public static void WriteFeedback(HttpResponse response, Int32 indent, string message)
        {
            WriteFeedback(response, indent, message, true);
        }

        public static void WriteFeedback(HttpResponse response, Int32 indent, string message, bool showtime)
        {
            bool showInstallationMessages = true;
            string ConfigSetting = Config.GetSetting("ShowInstallationMessages");
            if (ConfigSetting != null)
            {
                showInstallationMessages = bool.Parse(ConfigSetting);
            }
            if (showInstallationMessages)
            {
                TimeSpan timeElapsed = Upgrade.RunTime;
                string strMessage = "";
                if (showtime)
                {
                    strMessage += timeElapsed.ToString().Substring(0, timeElapsed.ToString().LastIndexOf(".") + 4) + " -";
                }
                for (int i = 0; i <= indent; i++)
                {
                    strMessage += "&nbsp;";
                }
                strMessage += message;
                response.Write(strMessage);
                response.Flush();
            }
        }

        public static void WriteFooter(HttpResponse response)
        {
            response.Write("</body>");
            response.Write("</html>");
            response.Flush();
        }

        public static void WriteHeader(HttpResponse response, string mode)
        {
            response.Buffer = false;
            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Install/Install.htm")))
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/Install/Install.template.htm")))
                {
                    File.Copy(HttpContext.Current.Server.MapPath("~/Install/Install.template.htm"), HttpContext.Current.Server.MapPath("~/Install/Install.htm"));
                }
            }
            if (File.Exists(HttpContext.Current.Server.MapPath("~/Install/Install.htm")))
            {
                response.Write(FileSystemUtils.ReadFile(HttpContext.Current.Server.MapPath("~/Install/Install.htm")));
            }
            switch (mode)
            {
                case "install":
                    response.Write("<h1>Installing DotNetNuke</h1>");
                    break;
                case "upgrade":
                    response.Write("<h1>Upgrading DotNetNuke</h1>");
                    break;
                case "addPortal":
                    response.Write("<h1>Adding New Portal</h1>");
                    break;
                case "installResources":
                    response.Write("<h1>Installing Resources</h1>");
                    break;
                case "executeScripts":
                    response.Write("<h1>Executing Scripts</h1>");
                    break;
                case "none":
                    response.Write("<h1>Nothing To Install At This Time</h1>");
                    break;
                case "noDBVersion":
                    response.Write("<h1>New DotNetNuke Database</h1>");
                    break;
                case "error":
                    response.Write("<h1>Error Installing DotNetNuke</h1>");
                    break;
                default:
                    response.Write("<h1>" + mode + "</h1>");
                    break;
            }
            response.Flush();
        }

        public static void WriteSuccessError(HttpResponse response, bool bSuccess)
        {
            if (bSuccess)
            {
                WriteFeedback(response, 0, "<font color='green'>Success</font><br>", false);
            }
            else
            {
                WriteFeedback(response, 0, "<font color='red'>Error!</font><br>", false);
            }
        }

        public static void WriteScriptSuccessError(HttpResponse response, bool bSuccess, string strLogFile)
        {
            if (bSuccess)
            {
                WriteFeedback(response, 0, "<font color='green'>Success</font><br>", false);
            }
            else
            {
                WriteFeedback(response, 0, "<font color='red'>Error! (see " + strLogFile + " for more information)</font><br>", false);
            }
        }

        /// <summary>
        /// Searches the provided html for absolute hrefs that match the provided aliases and converts them to relative urls
        /// </summary>
        /// <param name="html">The input html</param>
        /// <param name="aliases">a list of aliases that should be made into relative urls</param>
        /// <returns>html string</returns>
        public static string AbsoluteToRelativeUrls(string html, IEnumerable<string> aliases)
        {
            foreach (string portalAlias in aliases)
            {
                string searchAlias = portalAlias;
                if (portalAlias.Contains("/"))
                {
                    searchAlias = portalAlias.Substring(0, portalAlias.IndexOf("/"));
                }
                Regex exp = new Regex(string.Format("(href=&quot;)https?://{0}(.*?&quot;)", searchAlias));

                html = exp.Replace(html, "$1$2");
            }

            return html;
        }

    }
}
