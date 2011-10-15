#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework.Providers;
using DotNetNuke.HttpModules;

#endregion

namespace DotNetNuke.Services.Url.FriendlyUrl
{
    public class DNNFriendlyUrlProvider : FriendlyUrlProvider
    {
        private const string ProviderType = "friendlyUrl";
        private const string RegexMatchExpression = "[^a-zA-Z0-9 ]";
        private readonly string _fileExtension;

        private readonly bool _includePageName;

        private readonly ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);

        private readonly string _regexMatch;
        private readonly UrlFormatType _urlFormat = UrlFormatType.SearchFriendly;

        public DNNFriendlyUrlProvider()
        {
            //Read the configuration specific information for this provider
            var objProvider = (Provider) _providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            //Read the attributes for this provider
            if (!String.IsNullOrEmpty(objProvider.Attributes["includePageName"]))
            {
                _includePageName = bool.Parse(objProvider.Attributes["includePageName"]);
            }
            else
            {
                _includePageName = true;
            }
            if (!String.IsNullOrEmpty(objProvider.Attributes["regexMatch"]))
            {
                _regexMatch = objProvider.Attributes["regexMatch"];
            }
            else
            {
                _regexMatch = RegexMatchExpression;
            }
            if (!String.IsNullOrEmpty(objProvider.Attributes["fileExtension"]))
            {
                _fileExtension = objProvider.Attributes["fileExtension"];
            }
            else
            {
                _fileExtension = ".aspx";
            }
            if (!String.IsNullOrEmpty(objProvider.Attributes["urlFormat"]))
            {
                switch (objProvider.Attributes["urlFormat"].ToLower())
                {
                    case "searchfriendly":
                        _urlFormat = UrlFormatType.SearchFriendly;
                        break;
                    case "humanfriendly":
                        _urlFormat = UrlFormatType.HumanFriendly;
                        break;
                    default:
                        _urlFormat = UrlFormatType.SearchFriendly;
                        break;
                }
            }
        }

        public string FileExtension
        {
            get
            {
                return _fileExtension;
            }
        }

        public bool IncludePageName
        {
            get
            {
                return _includePageName;
            }
        }

        public string RegexMatch
        {
            get
            {
                return _regexMatch;
            }
        }

        public UrlFormatType UrlFormat
        {
            get
            {
                return _urlFormat;
            }
        }

        public override string FriendlyUrl(TabInfo tab, string path)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return FriendlyUrl(tab, path, Globals.glbDefaultPage, _portalSettings);
        }

        public override string FriendlyUrl(TabInfo tab, string path, string pageName)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return FriendlyUrl(tab, path, pageName, _portalSettings);
        }

        public override string FriendlyUrl(TabInfo tab, string path, string pageName, PortalSettings settings)
        {
            return FriendlyUrl(tab, path, pageName, settings.PortalAlias.HTTPAlias);
        }

        public override string FriendlyUrl(TabInfo tab, string path, string pageName, string portalAlias)
        {
            string friendlyPath = path;
            bool isPagePath = (tab != null);

            if ((UrlFormat == UrlFormatType.HumanFriendly))
            {
                if ((tab != null))
                {
                    var queryStringDic = GetQueryStringDictionary(path);
                    if ((queryStringDic.Count == 0 || (queryStringDic.Count == 1 && queryStringDic.ContainsKey("tabid"))))
                    {
                        friendlyPath = GetFriendlyAlias("~/" + tab.TabPath.Replace("//", "/").TrimStart('/') + ".aspx", portalAlias, isPagePath);
                    }
                    else if ((queryStringDic.Count == 2 && queryStringDic.ContainsKey("tabid") && queryStringDic.ContainsKey("language")))
                    {
                        if (!tab.IsNeutralCulture)
                        {
                            friendlyPath = GetFriendlyAlias("~/" + tab.CultureCode + "/" + tab.TabPath.Replace("//", "/").TrimStart('/') + ".aspx", portalAlias, isPagePath).ToLower();
                        }
                        else
                        {
                            friendlyPath = GetFriendlyAlias("~/" + queryStringDic["language"] + "/" + tab.TabPath.Replace("//", "/").TrimStart('/') + ".aspx", portalAlias, isPagePath).ToLower();
                        }
                    }
                    else
                    {
                        if (queryStringDic.ContainsKey("ctl") && !queryStringDic.ContainsKey("language"))
                        {
                            switch (queryStringDic["ctl"].ToLowerInvariant())
                            {
                                case "terms":
                                    friendlyPath = GetFriendlyAlias("~/terms.aspx", portalAlias, isPagePath);
                                    break;
                                case "privacy":
                                    friendlyPath = GetFriendlyAlias("~/privacy.aspx", portalAlias, isPagePath);
                                    break;
                                case "login":
                                    if ((queryStringDic.ContainsKey("returnurl")))
                                    {
                                        friendlyPath = GetFriendlyAlias("~/login.aspx?ReturnUrl=" + queryStringDic["returnurl"], portalAlias, isPagePath);
                                    }
                                    else
                                    {
                                        friendlyPath = GetFriendlyAlias("~/login.aspx", portalAlias, isPagePath);
                                    }
                                    break;
                                case "register":
                                    if ((queryStringDic.ContainsKey("returnurl")))
                                    {
                                        friendlyPath = GetFriendlyAlias("~/register.aspx?returnurl=" + queryStringDic["returnurl"], portalAlias, isPagePath);
                                    }
                                    else
                                    {
                                        friendlyPath = GetFriendlyAlias("~/register.aspx", portalAlias, isPagePath);
                                    }
                                    break;
                                default:
                                    //Return Search engine friendly version
                                    return GetFriendlyQueryString(tab, GetFriendlyAlias(path, portalAlias, isPagePath), pageName);
                            }
                        }
                        else
                        {
                            //Return Search engine friendly version
                            return GetFriendlyQueryString(tab, GetFriendlyAlias(path, portalAlias, isPagePath), pageName);
                        }
                    }
                }
            }
            else
            {
                //Return Search engine friendly version
                friendlyPath = GetFriendlyQueryString(tab, GetFriendlyAlias(path, portalAlias, isPagePath), pageName);
            }

			friendlyPath = CheckPathLength(Globals.ResolveUrl(friendlyPath), path);

            return friendlyPath;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddPage adds the page to the friendly url
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="path">The path to format.</param>
        /// <param name="pageName">The page name.</param>
        /// <returns>The formatted url</returns>
        /// <history>
        ///		[cnurse]	12/16/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private string AddPage(string path, string pageName)
        {
            string friendlyPath = path;
            if ((friendlyPath.EndsWith("/")))
            {
                friendlyPath = friendlyPath + pageName;
            }
            else
            {
                friendlyPath = friendlyPath + "/" + pageName;
            }
            return friendlyPath;
        }

        private string CheckPathLength(string friendlyPath, string originalpath)
        {
            if (friendlyPath.Length >= 260)
            {
                return Globals.ResolveUrl(originalpath);
            }
            else
            {
                return friendlyPath;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetFriendlyAlias gets the Alias root of the friendly url
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="path">The path to format.</param>
        /// <param name="portalAlias">The portal alias of the site.</param>
        /// <param name="isPagePath">Whether is a relative page path.</param>
        /// <returns>The formatted url</returns>
        /// <history>
        ///		[cnurse]	12/16/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private string GetFriendlyAlias(string path, string portalAlias, bool isPagePath)
        {
            string friendlyPath = path;
            string matchString = "";
            if (portalAlias != Null.NullString)
            {
                if (HttpContext.Current.Items["UrlRewrite:OriginalUrl"] != null)
                {
                    string httpAlias = Globals.AddHTTP(portalAlias).ToLowerInvariant();
                    string originalUrl = HttpContext.Current.Items["UrlRewrite:OriginalUrl"].ToString().ToLowerInvariant();
                    httpAlias = Globals.AddPort(httpAlias, originalUrl);
                    if (originalUrl.StartsWith(httpAlias))
                    {
                        matchString = httpAlias;
                    }
                    if ((String.IsNullOrEmpty(matchString)))
                    {
                        //Manage the special case where original url contains the alias as
                        //http://www.domain.com/Default.aspx?alias=www.domain.com/child"
                        Match portalMatch = Regex.Match(originalUrl, "^?alias=" + portalAlias, RegexOptions.IgnoreCase);
                        if (!ReferenceEquals(portalMatch, Match.Empty))
                        {
                            matchString = httpAlias;
                        }
                    }

                    if ((String.IsNullOrEmpty(matchString)))
                    {
                        //Manage the special case of child portals 
                        //http://www.domain.com/child/default.aspx
                        string tempurl = HttpContext.Current.Request.Url.Host + Globals.ResolveUrl(friendlyPath);
                        if( ! tempurl.Contains(portalAlias) )
                        {
                            matchString = httpAlias;
                        }
                    }

                    if ((String.IsNullOrEmpty(matchString)))
                    {
                        // manage the case where the current hostname is www.domain.com and the portalalias is domain.com
                        // (this occurs when www.domain.com is not listed as portal alias for the portal, but domain.com is)
                        string wwwHttpAlias = Globals.AddHTTP("www." + portalAlias);
                        if (originalUrl.StartsWith(wwwHttpAlias))
                        {
                            matchString = wwwHttpAlias;
                        }
                    }
                }
            }
            if ((!String.IsNullOrEmpty(matchString)))
            {
                if ((path.IndexOf("~") != -1))
                {
                    if (matchString.EndsWith("/"))
                    {
                        friendlyPath = friendlyPath.Replace("~/", matchString);
                    }
                    else
                    {
                        friendlyPath = friendlyPath.Replace("~", matchString);
                    }
                }
                else
                {
                    friendlyPath = matchString + friendlyPath;
                }
            }
            else
            {
                friendlyPath = Globals.ResolveUrl(friendlyPath);
            }
            if (friendlyPath.StartsWith("//") && isPagePath)
            {
                friendlyPath = friendlyPath.Substring(1);
            }
            return friendlyPath;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetFriendlyQueryString gets the Querystring part of the friendly url
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="tab">The tab whose url is being formatted.</param>
        /// <param name="path">The path to format.</param>
        /// <param name="pageName">The Page name.</param>
        /// <returns>The formatted url</returns>
        /// <history>
        ///		[cnurse]	12/16/2004	created
        ///		[smcculloch]10/10/2005	Regex update for rewritten characters
        /// </history>
        /// -----------------------------------------------------------------------------
        private string GetFriendlyQueryString(TabInfo tab, string path, string pageName)
        {
            string friendlyPath = path;
            Match queryStringMatch = Regex.Match(friendlyPath, "(.[^\\\\?]*)\\\\?(.*)", RegexOptions.IgnoreCase);
            string queryStringSpecialChars = "";
            if (!ReferenceEquals(queryStringMatch, Match.Empty))
            {
                friendlyPath = queryStringMatch.Groups[1].Value;
                friendlyPath = Regex.Replace(friendlyPath, Globals.glbDefaultPage, "", RegexOptions.IgnoreCase);
                string queryString = queryStringMatch.Groups[2].Value.Replace("&amp;", "&");
                if ((queryString.StartsWith("?")))
                {
                    queryString = queryString.TrimStart(Convert.ToChar("?"));
                }
                string[] nameValuePairs = queryString.Split(Convert.ToChar("&"));
                for (int i = 0; i <= nameValuePairs.Length - 1; i++)
                {
                    string pathToAppend = "";
                    string[] pair = nameValuePairs[i].Split(Convert.ToChar("="));

                    //Add name part of name/value pair
                    if ((friendlyPath.EndsWith("/")))
                    {
                        pathToAppend = pathToAppend + pair[0];
                    }
                    else
                    {
                        pathToAppend = pathToAppend + "/" + pair[0];
                    }
                    if ((pair.Length > 1))
                    {
                        if ((!String.IsNullOrEmpty(pair[1])))
                        {
                            if ((Regex.IsMatch(pair[1], _regexMatch) == false))
                            {
                                //Contains Non-AlphaNumeric Characters
                                if ((pair[0].ToLower() == "tabid"))
                                {
                                    if ((Regex.IsMatch(pair[1], "^\\d+$")))
                                    {
                                        if (tab != null)
                                        {
                                            int tabId = Convert.ToInt32(pair[1]);
                                            if ((tab.TabID == tabId))
                                            {
                                                if ((tab.TabPath != Null.NullString) && IncludePageName)
                                                {
                                                    pathToAppend = tab.TabPath.Replace("//", "/").TrimStart('/') + "/" + pathToAppend;
                                                }
                                            }
                                        }
                                    }
                                }
                                pathToAppend = pathToAppend + "/" + HttpUtility.UrlPathEncode(pair[1]);
                            }
                            else
                            {
                                //Rewrite into URL, contains only alphanumeric and the % or space
                                if (String.IsNullOrEmpty(queryStringSpecialChars))
                                {
                                    queryStringSpecialChars = pair[0] + "=" + pair[1];
                                }
                                else
                                {
                                    queryStringSpecialChars = queryStringSpecialChars + "&" + pair[0] + "=" + pair[1];
                                }
                                pathToAppend = "";
                            }
                        }
                        else
                        {
                            pathToAppend = pathToAppend + "/" + HttpUtility.UrlPathEncode((' ').ToString());
                        }
                    }
                    friendlyPath = friendlyPath + pathToAppend;
                }
            }
            if ((!String.IsNullOrEmpty(queryStringSpecialChars)))
            {
                return AddPage(friendlyPath, pageName) + "?" + queryStringSpecialChars;
            }
            else
            {
                return AddPage(friendlyPath, pageName);
            }
        }

        private Dictionary<string, string> GetQueryStringDictionary(string path)
        {
            string[] parts = path.Split('?');
            var results = new Dictionary<string, string>();


            if ((parts.Length == 2))
            {
                foreach (string part in parts[1].Split('&'))
                {
                    string[] keyvalue = part.Split('=');
                    if ((keyvalue.Length == 2))
                    {
                        results[keyvalue[0].ToLowerInvariant()] = keyvalue[1];
                    }
                }
            }

            return results;
        }
    }
}
