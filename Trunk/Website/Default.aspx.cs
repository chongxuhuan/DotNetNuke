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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Application;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.SiteLog;
using DotNetNuke.Services.Personalization;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Internals;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;

using DataCache = DotNetNuke.UI.Utilities.DataCache;
using Globals = DotNetNuke.Common.Globals;

#endregion  

namespace DotNetNuke.Framework
{
    public partial class DefaultPage : CDefault, IClientAPICallbackEventHandler
    {
        public int PageScrollTop
        {
            get
            {
                int pageScrollTop = Null.NullInteger;
                if (ScrollTop != null && !String.IsNullOrEmpty(ScrollTop.Value) && Regex.IsMatch(ScrollTop.Value, "^\\d+$"))
                {
                    pageScrollTop = Convert.ToInt32(ScrollTop.Value);
                }
                return pageScrollTop;
            }
            set { ScrollTop.Value = value.ToString(); }
        }

        protected string HtmlAttributeList
        {
            get
            {
                if ((HtmlAttributes != null) && (HtmlAttributes.Count > 0))
                {
                    var attr = new StringBuilder("");
                    foreach (string attributeName in HtmlAttributes.Keys)
                    {
                        if ((!String.IsNullOrEmpty(attributeName)) && (HtmlAttributes[attributeName] != null))
                        {
                            string attributeValue = HtmlAttributes[attributeName];
                            if ((attributeValue.IndexOf(",") > 0))
                            {
                                var attributeValues = attributeValue.Split(',');
                                for (var attributeCounter = 0;
                                     attributeCounter <= attributeValues.Length - 1;
                                     attributeCounter++)
                                {
                                    attr.Append(" " + attributeName + "=\"" + attributeValues[attributeCounter] + "\"");
                                }
                            }
                            else
                            {
                                attr.Append(" " + attributeName + "=\"" + attributeValue + "\"");
                            }
                        }
                    }
                    return attr.ToString();
                }
                return "";
            }
        }

        #region IClientAPICallbackEventHandler Members

        public string RaiseClientAPICallbackEvent(string eventArgument)
        {
            var dict = ParsePageCallBackArgs(eventArgument);
            if (dict.ContainsKey("type"))
            {
                if (DNNClientAPI.IsPersonalizationKeyRegistered(dict["namingcontainer"] + ClientAPI.CUSTOM_COLUMN_DELIMITER + dict["key"]) == false)
                {
                    throw new Exception(string.Format("This personalization key has not been enabled ({0}:{1}).  Make sure you enable it with DNNClientAPI.EnableClientPersonalization", dict["namingcontainer"], dict["key"]));
                }
                switch ((DNNClientAPI.PageCallBackType) Enum.Parse(typeof (DNNClientAPI.PageCallBackType), dict["type"]))
                {
                    case DNNClientAPI.PageCallBackType.GetPersonalization:
                        return Personalization.GetProfile(dict["namingcontainer"], dict["key"]).ToString();
                    case DNNClientAPI.PageCallBackType.SetPersonalization:
                        Personalization.SetProfile(dict["namingcontainer"], dict["key"], dict["value"]);
                        return dict["value"];
                    default:
                        throw new Exception("Unknown Callback Type");
                }
            }
            return "";
        }

        #endregion

        private void InitializePage()
        {
            var objTabs = new TabController();
            TabInfo objTab;
            if (!String.IsNullOrEmpty(Request.QueryString["tabname"]))
            {
                objTab = objTabs.GetTabByName(Request.QueryString["TabName"],
                                              ((PortalSettings) HttpContext.Current.Items["PortalSettings"]).PortalId);
                if (objTab != null)
                {
                    var parameters = new List<string>();
                    for (int intParam = 0; intParam <= Request.QueryString.Count - 1; intParam++)
                    {
                        switch (Request.QueryString.Keys[intParam].ToLower())
                        {
                            case "tabid":
                            case "tabname":
                                break;
                            default:
                                parameters.Add(
                                    Request.QueryString.Keys[intParam] + "=" + Request.QueryString[intParam]);
                                break;
                        }
                    }
                    Response.Redirect(Globals.NavigateURL(objTab.TabID, Null.NullString, parameters.ToArray()), true);
                }
                else
                {
                    throw new HttpException(404, "Not Found");
                }
            }
            if (Request.IsAuthenticated)
            {
                switch (Host.AuthenticatedCacheability)
                {
                    case "0":
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        break;
                    case "1":
                        Response.Cache.SetCacheability(HttpCacheability.Private);
                        break;
                    case "2":
                        Response.Cache.SetCacheability(HttpCacheability.Public);
                        break;
                    case "3":
                        Response.Cache.SetCacheability(HttpCacheability.Server);
                        break;
                    case "4":
                        Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                        break;
                    case "5":
                        Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
                        break;
                }
            }
            if (Host.DisplayCopyright)
            {
                Comment += string.Concat(Environment.NewLine,
                                         "<!--**********************************************************************************-->",
                                         Environment.NewLine,
                                         "<!-- DotNetNuke - http://www.dotnetnuke.com                                          -->",
                                         Environment.NewLine,
                                         "<!-- Copyright (c) 2002-2011                                                          -->",
                                         Environment.NewLine,
                                         "<!-- by DotNetNuke Corporation                                                        -->",
                                         Environment.NewLine,
                                         "<!--**********************************************************************************-->",
                                         Environment.NewLine);
            }
            Page.Header.Controls.AddAt(0, new LiteralControl(Comment));
            if (PortalSettings.ActiveTab.PageHeadText != Null.NullString && !Globals.IsAdminControl())
            {
                Page.Header.Controls.Add(new LiteralControl(PortalSettings.ActiveTab.PageHeadText));
            }
            string strTitle = PortalSettings.PortalName;
            
            foreach (TabInfo tab in PortalSettings.ActiveTab.BreadCrumbs)
            {
                strTitle += string.Concat(" > ", tab.TabName);
            }
            
            if (!string.IsNullOrEmpty(PortalSettings.ActiveTab.Title))
            {
                strTitle = PortalSettings.ActiveTab.Title;
            }
            Title = strTitle;
            if (FindControl("Body") != null)
            {
                if (!string.IsNullOrEmpty(PortalSettings.BackgroundFile))
                {
                    ((HtmlGenericControl) FindControl("Body")).Attributes["style"] =
                        string.Concat("background-image:url(",
                                      PortalSettings.HomeDirectory + PortalSettings.BackgroundFile, ");");
                }
            }
            if (PortalSettings.ActiveTab.RefreshInterval > 0 && Request.QueryString["ctl"] == null)
            {
                MetaRefresh.Content = PortalSettings.ActiveTab.RefreshInterval.ToString();
            }
            else
            {
                MetaRefresh.Visible = false;
            }
            if (!string.IsNullOrEmpty(PortalSettings.ActiveTab.Description))
            {
                Description = PortalSettings.ActiveTab.Description;
            }
            else
            {
                Description = PortalSettings.Description;
            }
            if (!string.IsNullOrEmpty(PortalSettings.ActiveTab.KeyWords))
            {
                KeyWords = PortalSettings.ActiveTab.KeyWords;
            }
            else
            {
                KeyWords = PortalSettings.KeyWords;
            }
            if (Host.DisplayCopyright)
            {
                KeyWords += ",DotNetNuke,DNN";
            }
            if (!string.IsNullOrEmpty(PortalSettings.FooterText))
            {
                Copyright = PortalSettings.FooterText;
            }
            else
            {
                Copyright = string.Concat("Copyright (c) ", DateTime.Now.Year, " by ", PortalSettings.PortalName);
            }
            if (Host.DisplayCopyright)
            {
                Generator = "DotNetNuke ";
            }
            else
            {
                Generator = "";
            }
            if (Request.QueryString["ctl"] != null &&
                (Request.QueryString["ctl"] == "Login" || Request.QueryString["ctl"] == "Register"))
            {
                MetaRobots.Content = "NOINDEX, NOFOLLOW";
            }
            else
            {
                MetaRobots.Content = "INDEX, FOLLOW";
            }
            if (NonProductionVersion() && Host.DisplayBetaNotice)
            {
                string versionString = string.Format(" ({0} Version: {1})", DotNetNukeContext.Current.Application.Status,
                                                     DotNetNukeContext.Current.Application.Version);
                Title += versionString;
            }
            if (PortalSettings.EnableSkinWidgets)
            {
                jQuery.RequestRegistration();
                ClientAPI.RegisterStartUpScript(Page, "initWidgets",
                                                string.Format(
                                                    "<script type=\"text/javascript\" src=\"{0}\" ></script>",
                                                    ResolveUrl("~/Resources/Shared/scripts/initWidgets.js")));
            }
        }

        private void SetSkinDoctype()
        {
            string strLang = CultureInfo.CurrentCulture.ToString();
            string strDocType = PortalSettings.ActiveTab.SkinDoctype;
            if (strDocType.Contains("XHTML 1.0"))
            {
                HtmlAttributes.Add("xml:lang", strLang);
                HtmlAttributes.Add("lang", strLang);
                HtmlAttributes.Add("xmlns", "http://www.w3.org/1999/xhtml");
            }
            else if (strDocType.Contains("XHTML 1.1"))
            {
                HtmlAttributes.Add("xml:lang", strLang);
                HtmlAttributes.Add("xmlns", "http://www.w3.org/1999/xhtml");
            }
            else
            {
                HtmlAttributes.Add("lang", strLang);
            }
            skinDocType.Text = PortalSettings.ActiveTab.SkinDoctype;
        }

        private void ManageRequest()
        {
            int affiliateId = -1;
            if (Request.QueryString["AffiliateId"] != null)
            {
                if (Regex.IsMatch(Request.QueryString["AffiliateId"], "^\\d+$"))
                {
                    affiliateId = Int32.Parse(Request.QueryString["AffiliateId"]);
                    var objAffiliates = new AffiliateController();
                    objAffiliates.UpdateAffiliateStats(affiliateId, 1, 0);
                    if (Request.Cookies["AffiliateId"] == null)
                    {
                        var objCookie = new HttpCookie("AffiliateId");
                        objCookie.Value = affiliateId.ToString();
                        objCookie.Expires = DateTime.Now.AddYears(1);
                        Response.Cookies.Add(objCookie);
                    }
                }
            }
            if (PortalSettings.SiteLogHistory != 0)
            {
                string urlReferrer = "";
                try
                {
                    if (Request.UrlReferrer != null)
                    {
                        urlReferrer = Request.UrlReferrer.ToString();
                    }
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);

                }
                string strSiteLogStorage = Host.SiteLogStorage;
                int intSiteLogBuffer = Host.SiteLogBuffer;
                var objSiteLogs = new SiteLogController();
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                objSiteLogs.AddSiteLog(PortalSettings.PortalId, objUserInfo.UserID, urlReferrer, Request.Url.ToString(),
                                       Request.UserAgent, Request.UserHostAddress, Request.UserHostName,
                                       PortalSettings.ActiveTab.TabID, affiliateId, intSiteLogBuffer,
                                       strSiteLogStorage);
            }
        }

        private void ManageFavicon()
        {
            string headerLink = FavIcon.GetHeaderLink(PortalSettings.PortalId);

            if (!String.IsNullOrEmpty(headerLink))
            {
                Page.Header.Controls.Add(new Literal{Text = headerLink});
            }
        }

        private Dictionary<string, string> ParsePageCallBackArgs(string strArg)
        {
            string[] aryVals = strArg.Split(new[] {ClientAPI.COLUMN_DELIMITER}, StringSplitOptions.None);
            var objDict = new Dictionary<string, string>();
            if (aryVals.Length > 0)
            {
                objDict.Add("type", aryVals[0]);
                switch (
                    (DNNClientAPI.PageCallBackType) Enum.Parse(typeof (DNNClientAPI.PageCallBackType), objDict["type"]))
                {
                    case DNNClientAPI.PageCallBackType.GetPersonalization:
                        objDict.Add("namingcontainer", aryVals[1]);
                        objDict.Add("key", aryVals[2]);
                        break;
                    case DNNClientAPI.PageCallBackType.SetPersonalization:
                        objDict.Add("namingcontainer", aryVals[1]);
                        objDict.Add("key", aryVals[2]);
                        objDict.Add("value", aryVals[3]);
                        break;
                }
            }
            return objDict;
        }

        private string RenderDefaultsWarning()
        {
            string warningLevel = Request.QueryString["runningDefault"];
            string warningMessage = string.Empty;
            switch (warningLevel)
            {
                case "1":
                    warningMessage = Localization.GetString("InsecureAdmin.Text", Localization.GlobalResourceFile);
                    break;
                case "2":
                    warningMessage = Localization.GetString("InsecureHost.Text", Localization.GlobalResourceFile);
                    break;
                case "3":
                    warningMessage = Localization.GetString("InsecureDefaults.Text", Localization.GlobalResourceFile);
                    break;
            }
            return warningMessage;
        }

        protected bool NonProductionVersion()
        {
            return DotNetNukeContext.Current.Application.Status != ReleaseMode.Stable;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InitializePage();
            UI.Skins.Skin ctlSkin = UI.Skins.Skin.GetSkin(this);
            SetSkinDoctype();
            if (PortalSettings.ActiveTab.DisableLink)
            {
                if (TabPermissionController.CanAdminPage())
                {
                    string heading = Localization.GetString("PageDisabled.Header");
                    string message = Localization.GetString("PageDisabled.Text");
                    UI.Skins.Skin.AddPageMessage(ctlSkin, heading, message,
                                                 ModuleMessage.ModuleMessageType.YellowWarning);
                }
                else
                {
                    if (PortalSettings.HomeTabId > 0)
                    {
                        Response.Redirect(Globals.NavigateURL(PortalSettings.HomeTabId), true);
                    }
                    else
                    {
                        Response.Redirect(
                            Globals.GetPortalDomainName(PortalSettings.PortalAlias.HTTPAlias, Request, true), true);
                    }
                }
            }
            //Manage canonical urls
            if (PortalSettings.PortalAliasMappingMode == PortalSettings.PortalAliasMapping.CanonicalUrl &&
                PortalSettings.PortalAlias.HTTPAlias != PortalSettings.DefaultPortalAlias)
            {
                string originalurl = Context.Items["UrlRewrite:OriginalUrl"].ToString();

                //Add Canonical <link>
                var canonicalLink = new HtmlLink();
                canonicalLink.Href = originalurl.Replace(PortalSettings.PortalAlias.HTTPAlias,
                                                         PortalSettings.DefaultPortalAlias);

                canonicalLink.Attributes.Add("rel", "canonical");

                // Add the HtmlLink to the Head section of the page.
                Page.Header.Controls.Add(canonicalLink);
            }

            string messageText = "";
            if (Request.IsAuthenticated && string.IsNullOrEmpty(Request.QueryString["runningDefault"]) == false)
            {
                var userInfo = HttpContext.Current.Items["UserInfo"] as UserInfo;
                if ((userInfo.Username.ToLower() == "admin") || (userInfo.Username.ToLower() == "host"))
                {
                    messageText = RenderDefaultsWarning();
                    string messageTitle = Localization.GetString("InsecureDefaults.Title",
                                                                 Localization.GlobalResourceFile);
                    UI.Skins.Skin.AddPageMessage(ctlSkin, messageTitle, messageText,
                                                 ModuleMessage.ModuleMessageType.RedError);
                }
            }

            RegisterStyleSheet(this, Globals.HostPath + "default.css");
            RegisterStyleSheet(this, ctlSkin.SkinPath + "skin.css");
            RegisterStyleSheet(this, ctlSkin.SkinSrc.Replace(".ascx", ".css"));

            SkinPlaceHolder.Controls.Add(ctlSkin);

            RegisterStyleSheet(this, PortalSettings.HomeDirectory + "portal.css");
 
            ManageFavicon();
            ClientAPI.HandleClientAPICallbackEvent(this);
            //add viewstateuserkey to protect against CSRF attacks
            if (User.Identity.IsAuthenticated)
            {
                ViewStateUserKey = User.Identity.Name;
            }

            if (PortalSettings.EnablePopUps)
            {                
                jQuery.RegisterJQueryUI(Page);
                ClientScript.RegisterClientScriptInclude("modalPopUp", ResolveUrl("~/js/dnn.modalpopup.js"));
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!String.IsNullOrEmpty(ScrollTop.Value))
            {
                DNNClientAPI.AddBodyOnloadEventHandler(Page, "__dnn_setScrollTop();");
                ScrollTop.Value = ScrollTop.Value;
            }
        }

        protected override void OnPreRender(EventArgs evt)
        {
            base.OnPreRender(evt);

            if (!Globals.IsAdminControl())
            {
                ManageRequest();
            }
            Page.Header.Title = Title;
            MetaGenerator.Content = Generator;
            MetaGenerator.Visible = (!String.IsNullOrEmpty(Generator));
            MetaAuthor.Content = PortalSettings.PortalName;
            MetaCopyright.Content = Copyright;
            MetaCopyright.Visible = (!String.IsNullOrEmpty(Copyright));
            MetaKeywords.Content = KeyWords;
            MetaKeywords.Visible = (!String.IsNullOrEmpty(KeyWords));
            MetaDescription.Content = Description;
            MetaDescription.Visible = (!String.IsNullOrEmpty(Description));
            if (jQuery.IsRequested)
            {
                jQuery.RegisterJQuery(Page);
            }
            if (jQuery.IsUIRequested)
            {
                jQuery.RegisterJQueryUI(Page);
            }
            if (jQuery.AreDnnPluginsRequested)
            {
                jQuery.RegisterDnnJQueryPlugins(Page);
            }
        }
    }
}
