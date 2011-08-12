/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DotNetNuke.Services.Devices.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class UserAgentUtils
    {
        //Compiled regex to strip out quotes in UAProfile 

        // Compiled regex to parse UAProfile namespace
        private static readonly Regex NAME_SPACE_REGEX = new Regex(@"ns=(\d*)", RegexOptions.Compiled);
        private static readonly Regex STRIP_QUOTE_REGEX = new Regex(@"""", RegexOptions.Compiled);


        /// <summary>
        /// Gets the UA profile.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        internal static string GetUAProfile(HttpRequest request)
        {
            string headerName = GetHeaderNameForUAProfile(request);

            return GetUAProfileForHeader(request, headerName);
        }

        private static string GetUAProfileForHeader(HttpRequest request, string headerName)
        {
            string uaProfile = null;
            if (headerName != null && headerName.Trim().Length > 0)
            {
                uaProfile = request.Headers[headerName];
            }

            // Strip out quotes from uaProfile
            if (uaProfile != null && uaProfile.Trim().Length > 0)
            {
                STRIP_QUOTE_REGEX.Replace(uaProfile, "");
            }

            return uaProfile;
        }

        private static string GetHeaderNameForUAProfile(HttpRequest request)
        {
            string headerName = null;
            if (request.Headers["x-wap-profile"] != null)
            {
                headerName = "x-wap-profile";
            }
            else if (request.Headers["Profile"] != null)
            {
                headerName = "Profile";
            }
            else if (request.Headers["wap-profile"] != null)
            {
                headerName = "wap-profile";
            }
            else if (request.Headers["Opt"] != null)
            {
                string optHeader = request.Headers["Opt"];
                string namespaceNumber = null;

                if (optHeader != null && optHeader.IndexOf("ns=") != -1)
                {
                    try
                    {
                        namespaceNumber = NAME_SPACE_REGEX.Match(optHeader).Groups[0].Value;
                        StringBuilder sb = new StringBuilder(namespaceNumber);
                        sb.Append("-Profile");
                        headerName = sb.ToString();
                    }
                    catch (Exception e)
                    {
                        headerName = null;
                    }
                }
            }
            return headerName;
        }

        /// <summary>
        /// Gets the user agent.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        internal static string GetUserAgent(HttpRequest request)
        {
            string userAgent = "";
            if (request.Params["UA"] != null)
            {
                userAgent = request.Params["UA"];
            }
            else if (request.Headers["x-device-user-agent"] != null)
            {
                userAgent = request.Headers["x-device-user-agent"];
            }
            else
            {
                userAgent = request.UserAgent;
            }

            return userAgent;
        }

        /// <summary>
        /// Determines whether [is XHTML requester] [the specified request].
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// 	<c>true</c> if [is XHTML requester] [the specified request]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsXhtmlRequester(HttpRequest request)
        {
            string acceptHeader = request.Headers["accept"];

            if (null != acceptHeader)
            {
                if (acceptHeader.Contains(Constants.ACCEPT_HEADER_VND_WAP_XHTML_XML)
                    || acceptHeader.Contains(Constants.ACCEPT_HEADER_VND_WAP_XHTML_XML)
                    || acceptHeader.Contains(Constants.ACCEPT_HEADER_VND_WAP_XHTML_XML))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Determines whether [is mobile browser] [the specified user agent].
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>
        /// 	<c>true</c> if [is mobile browser] [the specified user agent]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsMobileBrowser(string userAgent)
        {
            return userAgent.Contains("Mobile");
        }
    }
}