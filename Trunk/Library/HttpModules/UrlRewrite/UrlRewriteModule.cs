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
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.HttpModules.Config;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.EventQueue;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.HttpModules
{
    public class UrlRewriteModule : IHttpModule
    {
        public string ModuleName
        {
            get
            {
                return "UrlRewriteModule";
            }
        }

        #region IHttpModule Members

        public void Init(HttpApplication application)
        {
            application.BeginRequest += OnBeginRequest;
        }

        public void Dispose()
        {
        }

        #endregion

        private string FormatDomain(string url, string replaceDomain, string withDomain)
        {
            if (!String.IsNullOrEmpty(replaceDomain) && !String.IsNullOrEmpty(withDomain))
            {
                if (url.IndexOf(replaceDomain) != -1)
                {
                    url = url.Replace(replaceDomain, withDomain);
                }
            }
            return url;
        }

        private void RewriteUrl(HttpApplication app, out string portalAlias)
        {
            HttpRequest request = app.Request;
            HttpResponse response = app.Response;
            string requestedPath = app.Request.Url.AbsoluteUri;


            portalAlias = "";

            //determine portal alias looking for longest possible match
            String myAlias = Globals.GetDomainName(app.Request, true);
            PortalAliasInfo objPortalAlias;
            do
            {
                objPortalAlias = PortalAliasController.GetPortalAliasInfo(myAlias);

                if(objPortalAlias != null)
                {
                    portalAlias = myAlias;
                    break;
                }

                int slashIndex = myAlias.LastIndexOf('/');
                if(slashIndex > 1)
                {
                    myAlias = myAlias.Substring(0, slashIndex);
                }
                else
                {
                    myAlias = "";
                }
            } while (myAlias.Length > 0);


            app.Context.Items.Add("UrlRewrite:OriginalUrl", app.Request.Url.AbsoluteUri);
            string sendTo = "";
            string strQueryString = "";
            if ((!String.IsNullOrEmpty(app.Request.Url.Query)))
            {
                strQueryString = request.QueryString.ToString();
                requestedPath = requestedPath.Replace(app.Request.Url.Query, "");
            }
            RewriterRuleCollection rules = RewriterConfiguration.GetConfig().Rules;
            int matchIndex = -1;
            for (int ruleIndex = 0; ruleIndex <= rules.Count - 1; ruleIndex++)
            {
                string pattern = "^" + RewriterUtils.ResolveUrl(app.Context.Request.ApplicationPath, rules[ruleIndex].LookFor) + "$";
                Match objMatch = Regex.Match(requestedPath, pattern, RegexOptions.IgnoreCase);
                if ((objMatch.Success))
                {
                    sendTo = RewriterUtils.ResolveUrl(app.Context.Request.ApplicationPath, Regex.Replace(requestedPath, pattern, rules[ruleIndex].SendTo, RegexOptions.IgnoreCase));
                    string parameters = objMatch.Groups[2].Value;
                    if ((parameters.Trim().Length > 0))
                    {
                        parameters = parameters.Replace("\\", "/");
                        string[] splitParameters = parameters.Split('/');
                        string parameterName;
                        string parameterValue;
                        for (int parameterIndex = 0; parameterIndex < splitParameters.Length; parameterIndex++)
                        {
                            if (splitParameters[parameterIndex].IndexOf(".aspx", StringComparison.InvariantCultureIgnoreCase) == -1)
                            {
                                parameterName = splitParameters[parameterIndex].Trim();
                                if (parameterName.Length > 0)
                                {
                                    if (sendTo.IndexOf("?" + parameterName + "=", StringComparison.InvariantCultureIgnoreCase) == -1 &&
                                        sendTo.IndexOf("&" + parameterName + "=", StringComparison.InvariantCultureIgnoreCase) == -1)
                                    {
                                        string parameterDelimiter;
                                        if (sendTo.IndexOf("?") != -1)
                                        {
                                            parameterDelimiter = "&";
                                        }
                                        else
                                        {
                                            parameterDelimiter = "?";
                                        }
                                        sendTo = sendTo + parameterDelimiter + parameterName;
                                        parameterValue = "";
                                        if (parameterIndex < splitParameters.Length - 1)
                                        {
                                            parameterIndex += 1;
                                            if (!String.IsNullOrEmpty(splitParameters[parameterIndex].Trim()))
                                            {
                                                parameterValue = splitParameters[parameterIndex].Trim();
                                            }
                                        }
                                        if (parameterValue.Length > 0)
                                        {
                                            sendTo = sendTo + "=" + parameterValue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    matchIndex = ruleIndex;
                    break;
                }
            }
            if (!String.IsNullOrEmpty(strQueryString))
            {
                string[] parameters = strQueryString.Split('&');
                string parameterName;
                for (int parameterIndex = 0; parameterIndex <= parameters.Length - 1; parameterIndex++)
                {
                    parameterName = parameters[parameterIndex];
                    if (parameterName.IndexOf("=") != -1)
                    {
                        parameterName = parameterName.Substring(0, parameterName.IndexOf("="));
                    }
                    if (sendTo.IndexOf("?" + parameterName + "=", StringComparison.InvariantCultureIgnoreCase) == -1 &&
                        sendTo.IndexOf("&" + parameterName + "=", StringComparison.InvariantCultureIgnoreCase) == -1)
                    {
                        if (sendTo.IndexOf("?") != -1)
                        {
                            sendTo = sendTo + "&" + parameters[parameterIndex];
                        }
                        else
                        {
                            sendTo = sendTo + "?" + parameters[parameterIndex];
                        }
                    }
                }
            }
            if (matchIndex != -1)
            {
                if (rules[matchIndex].SendTo.StartsWith("~"))
                {
                    RewriterUtils.RewriteUrl(app.Context, sendTo);
                }
                else
                {
                    response.Redirect(sendTo, true);
                }
            }
            else
            {
                string url;
                if (Globals.UsePortNumber() && ((app.Request.Url.Port != 80 && !app.Request.IsSecureConnection) || (app.Request.Url.Port != 443 && app.Request.IsSecureConnection)))
                {
                    url = app.Request.Url.Host + ":" + app.Request.Url.Port + app.Request.Url.LocalPath;
                }
                else
                {
                    url = app.Request.Url.Host + app.Request.Url.LocalPath;
                }

                if(!String.IsNullOrEmpty(myAlias))
                {
                        
                    if (objPortalAlias != null)
                    {
                        int portalID = objPortalAlias.PortalID;
                        string tabPath = url;
                        if (tabPath.StartsWith(myAlias))
                        {
                            tabPath = url.Remove(0, myAlias.Length);
                        }
                        if ((tabPath == "/" + Globals.glbDefaultPage.ToLower()))
                        {
                            return;
                        }

                        string cultureCode = string.Empty;

                        Dictionary<string, Locale> dicLocales = LocaleController.Instance.GetLocales(portalID);
                        if (dicLocales.Count > 1)
                        {
                            String[] splitUrl = app.Request.Url.ToString().Split('/');

                            foreach (string culturePart in splitUrl)
                            {
                                if (culturePart.Length.Equals(5))
                                {
                                    foreach (KeyValuePair<string, Locale> key in dicLocales)
                                    {
                                        if (key.Key.ToLower().Equals(culturePart.ToLower()))
                                        {
                                            cultureCode = key.Value.Code;
                                            tabPath = tabPath.Replace("/" + culturePart, "");
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        // Check to see if the tab exists (if localization is enable, check for the specified culture)
                        int tabID = TabController.GetTabByTabPath(portalID, tabPath.Replace("/", "//").Replace(".aspx", ""), cultureCode);

                        // Check to see if neutral culture tab exists
                        if ((tabID == Null.NullInteger && cultureCode.Length > 0))
                        {
                            tabID = TabController.GetTabByTabPath(portalID, tabPath.Replace("/", "//").Replace(".aspx", ""), "");
                        }

                        if ((tabID != Null.NullInteger))
                        {
                            string sendToUrl = "~/" + Globals.glbDefaultPage + "?TabID=" + tabID;
                            if (!cultureCode.Equals(string.Empty))
                            {
                                sendToUrl = sendToUrl + "&language=" + cultureCode;
                            }
                            if ((!String.IsNullOrEmpty(app.Request.Url.Query)))
                            {
                                sendToUrl = sendToUrl + "&" + app.Request.Url.Query.TrimStart('?');
                            }
                            RewriterUtils.RewriteUrl(app.Context, sendToUrl);
                            return;
                        }
                        tabPath = tabPath.ToLower();
                        if ((tabPath.IndexOf('?') != -1))
                        {
                            tabPath = tabPath.Substring(0, tabPath.IndexOf('?'));
                        }
                        if ((tabPath == "/login.aspx"))
                        {
                            PortalInfo portal = new PortalController().GetPortal(portalID);
                            if (portal.LoginTabId > Null.NullInteger && Globals.ValidateLoginTabID(portal.LoginTabId))
                            {
                                if ((!String.IsNullOrEmpty(app.Request.Url.Query)))
                                {
                                    RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?TabID=" + portal.LoginTabId + app.Request.Url.Query.TrimStart('?'));
                                }
                                else
                                {
                                    RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?TabID=" + portal.LoginTabId);
                                }
                            }
                            else
                            {
                                if ((!String.IsNullOrEmpty(app.Request.Url.Query)))
                                {
                                    RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?portalid=" + portalID + "&ctl=login&" + app.Request.Url.Query.TrimStart('?'));
                                }
                                else
                                {
                                    RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?portalid=" + portalID + "&ctl=login");
                                }
                            }
                            return;
                        }
                        if ((tabPath == "/register.aspx"))
                        {
                            if ((!String.IsNullOrEmpty(app.Request.Url.Query)))
                            {
                                RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?portalid=" + portalID + "&ctl=Register&" + app.Request.Url.Query.TrimStart('?'));
                            }
                            else
                            {
                                RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?portalid=" + portalID + "&ctl=Register");
                            }
                            return;
                        }
                        if ((tabPath == "/terms.aspx"))
                        {
                            if ((!String.IsNullOrEmpty(app.Request.Url.Query)))
                            {
                                RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?portalid=" + portalID + "&ctl=Terms&" + app.Request.Url.Query.TrimStart('?'));
                            }
                            else
                            {
                                RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?portalid=" + portalID + "&ctl=Terms");
                            }
                            return;
                        }
                        if ((tabPath == "/privacy.aspx"))
                        {
                            if ((!String.IsNullOrEmpty(app.Request.Url.Query)))
                            {
                                RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?portalid=" + portalID + "&ctl=Privacy&" + app.Request.Url.Query.TrimStart('?'));
                            }
                            else
                            {
                                RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?portalid=" + portalID + "&ctl=Privacy");
                            }
                            return;
                        }
                        tabPath = tabPath.Replace("/", "//");
                        tabPath = tabPath.Replace(".aspx", "");
                        var objTabController = new TabController();
                        TabCollection objTabs;
                        if (tabPath.StartsWith("//host"))
                        {
                            objTabs = objTabController.GetTabsByPortal(Null.NullInteger);
                        }
                        else
                        {
                            objTabs = objTabController.GetTabsByPortal(portalID);
                        }
                        foreach (KeyValuePair<int, TabInfo> kvp in objTabs)
                        {
                            if ((kvp.Value.IsDeleted == false && kvp.Value.TabPath.ToLower() == tabPath))
                            {
                                if ((!String.IsNullOrEmpty(app.Request.Url.Query)))
                                {
                                    RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?TabID=" + kvp.Value.TabID + "&" + app.Request.Url.Query.TrimStart('?'));
                                }
                                else
                                {
                                    RewriterUtils.RewriteUrl(app.Context, "~/" + Globals.glbDefaultPage + "?TabID=" + kvp.Value.TabID);
                                }
                                return;
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        public void OnBeginRequest(object s, EventArgs e)
        {
            var app = (HttpApplication) s;
            var server = app.Server;
            var request = app.Request;
            var response = app.Response;
            var requestedPath = app.Request.Url.AbsoluteUri;
            if (RewriterUtils.OmitFromRewriteProcessing(request.Url.LocalPath))
            {
                return;
            }
            
            Initialize.Init(app);
            if (request.Url.LocalPath.EndsWith("install.aspx", StringComparison.InvariantCultureIgnoreCase) ||
                request.Url.LocalPath.EndsWith("installwizard.aspx", StringComparison.InvariantCultureIgnoreCase) ||
                request.Url.LocalPath.EndsWith("captcha.aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            var strURL = request.Url.AbsolutePath;
            var strDoubleDecodeURL = server.UrlDecode(server.UrlDecode(request.RawUrl));
            if (Regex.Match(strURL, "[\\\\/]\\.\\.[\\\\/]").Success || Regex.Match(strDoubleDecodeURL, "[\\\\/]\\.\\.[\\\\/]").Success)
            {
                throw new HttpException(404, "Not Found");
            }
            try
            {
                if ((request.Path.IndexOf("\\") >= 0 || Path.GetFullPath(request.PhysicalPath) != request.PhysicalPath))
                {
                    throw new HttpException(404, "Not Found");
                }
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);

            }


            String domainName;
            RewriteUrl(app, out domainName);

            //blank DomainName indicates RewriteUrl couldn't locate a current portal
            //reprocess url for portal alias if auto add is an option
            if(domainName == "" && CanAutoAddPortalAlias())
            {
                domainName = Globals.GetDomainName(app.Request, true);
            }

            //from this point on we are dealing with a "standard" querystring ( ie. http://www.domain.com/default.aspx?tabid=## )
            //if the portal/url was succesfully identified

            var tabId=-1;
            var portalId=-1;
            string portalAlias = null;
            PortalAliasInfo portalAliasInfo = null;
            var parsingError = false;

            // get TabId from querystring ( this is mandatory for maintaining portal context for child portals )
            if (!string.IsNullOrEmpty(request.QueryString["tabid"]))
            {
                if (!Int32.TryParse(request.QueryString["tabid"], out tabId))
                {
                    tabId = Null.NullInteger;
                    parsingError = true;
                }
            }

            // get PortalId from querystring ( this is used for host menu options as well as child portal navigation )
            if (!string.IsNullOrEmpty(request.QueryString["portalid"]))
            {
                if (!Int32.TryParse(request.QueryString["portalid"], out portalId))
                {
                    portalId = Null.NullInteger;
                    parsingError = true;
                }
            }

            if (parsingError)
            {
                //The tabId or PortalId are incorrectly formatted (potential DOS)
                throw new HttpException(404, "Not Found");
            }


            try
            {
                if (request.QueryString["alias"] != null)
                {
                    // check if the alias is valid
                    var childAlias = request.QueryString["alias"];
                    if (!Globals.UsePortNumber())
                    {
                        childAlias = childAlias.Replace(":" + request.Url.Port, "");
                    }

                    if (PortalAliasController.GetPortalAliasInfo(childAlias) != null)
                    {
                        if (childAlias.IndexOf(domainName, StringComparison.OrdinalIgnoreCase) == -1)
                        {
                            response.Redirect(Globals.GetPortalDomainName(childAlias, request, true), true);
                        }
                        else
                        {
                            portalAlias = childAlias;
                        }
                    }
                }
                
                domainName = Globals.GetDomainName(request, true);
                if (portalAlias == null)
                {
                    if (portalId != -1)
                    {
                        portalAlias = PortalAliasController.GetPortalAliasByPortal(portalId, domainName);
                    }
                }
                if (portalAlias == null)
                {
                    if (tabId != -1)
                    {
                        portalAlias = PortalAliasController.GetPortalAliasByTab(tabId, domainName);
                        if (String.IsNullOrEmpty(portalAlias))
                        {
                            portalAliasInfo = PortalAliasController.GetPortalAliasInfo(domainName);
                            if (portalAliasInfo != null)
                            {
                                if (app.Request.Url.AbsoluteUri.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    strURL = "https://" + portalAliasInfo.HTTPAlias.Replace("*.", "");
                                }
                                else
                                {
                                    strURL = "http://" + portalAliasInfo.HTTPAlias.Replace("*.", "");
                                }
                                if (strURL.IndexOf(domainName, StringComparison.InvariantCultureIgnoreCase) == -1)
                                {
                                    strURL += app.Request.Url.PathAndQuery;
                                }
                                response.Redirect(strURL, true);
                            }
                        }
                    }
                }
                if (String.IsNullOrEmpty(portalAlias))
                {
                    portalAlias = domainName;
                }
                portalAliasInfo = PortalAliasController.GetPortalAliasInfo(portalAlias);
                if (portalAliasInfo != null)
                {
                    portalId = portalAliasInfo.PortalID;
                }
                
                if (portalId == -1)
                {
                    bool autoAddPortalAlias = CanAutoAddPortalAlias();

                    if (!autoAddPortalAlias && !request.Url.LocalPath.EndsWith(Globals.glbDefaultPage, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // allows requests for aspx pages in custom folder locations to be processed
                        return;
                    }

                    if (autoAddPortalAlias)
                    {
                        var portalAliasController = new PortalAliasController();
                        portalId = Host.HostPortalID;
                        if (portalId > Null.NullInteger)
                        {
                            portalAliasInfo = new PortalAliasInfo();
                            portalAliasInfo.PortalID = portalId;
                            portalAliasInfo.HTTPAlias = portalAlias;
                            portalAliasController.AddPortalAlias(portalAliasInfo);

                            response.Redirect(app.Request.Url.ToString(), true);
                        }
                    }
                }
            }
            catch (ThreadAbortException exc)
            {
                //Do nothing if Thread is being aborted - there are two response.redirect calls in the Try block
                DnnLog.Debug(exc);

            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);

                strURL = "~/ErrorPage.aspx?status=500&error=" + server.UrlEncode(ex.Message);
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Server.Transfer(strURL);
            }
            if (portalId != -1)
            {
                var portalSettings = new PortalSettings(tabId, portalAliasInfo);
                app.Context.Items.Add("PortalSettings", portalSettings);

                if (portalSettings.PortalAliasMappingMode == PortalSettings.PortalAliasMapping.Redirect && 
                    !String.IsNullOrEmpty(portalSettings.DefaultPortalAlias) &&
                    portalAliasInfo != null &&
                    portalAliasInfo.HTTPAlias != portalSettings.DefaultPortalAlias)
                {
                    //Permanently Redirect
                    response.StatusCode = 301;
                    response.AppendHeader("Location", Globals.AddHTTP(portalSettings.DefaultPortalAlias));
                }

                if (!String.IsNullOrEmpty(portalSettings.ActiveTab.Url) && request.QueryString["ctl"] == null && request.QueryString["fileticket"] == null)
                {
                    var redirectUrl = portalSettings.ActiveTab.FullUrl;
                    if (portalSettings.ActiveTab.PermanentRedirect)
                    {
                        response.StatusCode = 301;
                        response.AppendHeader("Location", redirectUrl);
                    }
                    else
                    {
                        response.Redirect(redirectUrl, true);
                    }
                }
                if (request.Url.AbsolutePath.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase))
                {
                    strURL = "";
                    if (portalSettings.SSLEnabled)
                    {
                        if (portalSettings.ActiveTab.IsSecure && !request.IsSecureConnection)
                        {
                            strURL = requestedPath.Replace("http://", "https://");
                            strURL = FormatDomain(strURL, portalSettings.STDURL, portalSettings.SSLURL);
                        }
                    }
                    if (portalSettings.SSLEnforced)
                    {
                        if (!portalSettings.ActiveTab.IsSecure && request.IsSecureConnection)
                        {
                            if (request.QueryString["ssl"] == null)
                            {
                                strURL = requestedPath.Replace("https://", "http://");
                                strURL = FormatDomain(strURL, portalSettings.SSLURL, portalSettings.STDURL);
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(strURL))
                    {
                        if (strURL.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                        {
                            response.Redirect(strURL, true);
                        }
                        else
                        {
                            response.Clear();
                            response.AddHeader("Refresh", "0;URL=" + strURL);
                            response.Write("<html><head><title></title>");
                            response.Write("<!-- <script language=\"javascript\">window.location.replace(\"" + strURL + "\")</script> -->");
                            response.Write("</head><body></body></html>");
                            response.End();
                        }
                    }
                }
            }
            else
            {
                strURL = "~/ErrorPage.aspx?status=404&error=" + domainName;
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Server.Transfer(strURL);
            }

            if (app.Context.Items["FirstRequest"] != null)
            {
                app.Context.Items.Remove("FirstRequest");

                //Process any messages in the EventQueue for the Application_Start_FirstRequest event
                EventQueueController.ProcessMessages("Application_Start_FirstRequest");
            }
        }

        private bool CanAutoAddPortalAlias()
        {
            var autoAddPortalAlias = HostController.Instance.GetBoolean("AutoAddPortalAlias");
            autoAddPortalAlias = autoAddPortalAlias && (new PortalController().GetPortals().Count == 1);
            return autoAddPortalAlias;
        }
    }
}