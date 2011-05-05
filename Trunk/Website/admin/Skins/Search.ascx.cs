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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.UI.Skins.Controls
{
    public partial class Search : SkinObjectBase
    {
        private const string MyFileName = "Search.ascx";
        private bool _showSite = true;
        private bool _showWeb = true;
        private string _siteIconURL;
        private string _siteText;
        private string _siteToolTip;
        private string _siteURL;
        private string _webIconURL;
        private string _webText;
        private string _webToolTip;
        private string _webURL;
        public string CssClass { get; set; }

        public bool ShowSite
        {
            get
            {
                return _showSite;
            }
            set
            {
                _showSite = value;
            }
        }

        public bool ShowWeb
        {
            get
            {
                return _showWeb;
            }
            set
            {
                _showWeb = value;
            }
        }

        public string SiteIconURL
        {
            get
            {
                if (string.IsNullOrEmpty(_siteIconURL))
                {
                    return "~/images/Search/dotnetnuke-icon.gif";
                }
                return _siteIconURL;
            }
            set
            {
                _siteIconURL = value;
            }
        }

        public string SiteText
        {
            get
            {
                if (string.IsNullOrEmpty(_siteText))
                {
                    return Localization.GetString("Site", Localization.GetResourceFile(this, MyFileName));
                }
                return _siteText;
            }
            set
            {
                _siteText = value;
            }
        }

        public string SiteToolTip
        {
            get
            {
                if (string.IsNullOrEmpty(_siteToolTip))
                {
                    return Localization.GetString("Site.ToolTip", Localization.GetResourceFile(this, MyFileName));
                }
                return _siteToolTip;
            }
            set
            {
                _siteToolTip = value;
            }
        }

        public string SiteURL
        {
            get
            {
                if (string.IsNullOrEmpty(_siteURL))
                {
                    return Localization.GetString("URL", Localization.GetResourceFile(this, MyFileName));
                }
                return _siteURL;
            }
            set
            {
                _siteURL = value;
            }
        }

        public string Submit { get; set; }

        public bool UseWebForSite { get; set; }

        public bool UseDropDownList { get; set; }

        public string WebIconURL
        {
            get
            {
                if (string.IsNullOrEmpty(_webIconURL))
                {
                    return "~/images/Search/google-icon.gif";
                }
                return _webIconURL;
            }
            set
            {
                _webIconURL = value;
            }
        }

        public string WebText
        {
            get
            {
                if (string.IsNullOrEmpty(_webText))
                {
                    return Localization.GetString("Web", Localization.GetResourceFile(this, MyFileName));
                }
                return _webText;
            }
            set
            {
                _webText = value;
            }
        }

        public string WebToolTip
        {
            get
            {
                if (string.IsNullOrEmpty(_webToolTip))
                {
                    return Localization.GetString("Web.ToolTip", Localization.GetResourceFile(this, MyFileName));
                }
                return _webToolTip;
            }
            set
            {
                _webToolTip = value;
            }
        }

        public string WebURL
        {
            get
            {
                if (string.IsNullOrEmpty(_webURL))
                {
                    return Localization.GetString("URL", Localization.GetResourceFile(this, MyFileName));
                }
                return _webURL;
            }
            set
            {
                _webURL = value;
            }
        }

        private int GetSearchTabId()
        {
            int searchTabId = PortalSettings.SearchTabId;
            if (searchTabId == Null.NullInteger)
            {
                var objModules = new ModuleController();
                ArrayList arrModules = objModules.GetModulesByDefinition(PortalSettings.PortalId, "Search Results");
                if (arrModules.Count > 1)
                {
                    foreach (ModuleInfo SearchModule in arrModules)
                    {
                        if (SearchModule.CultureCode == PortalSettings.CultureCode)
                        {
                            searchTabId = SearchModule.TabID;
                        }
                    }
                }
                else if (arrModules.Count == 1)
                {
                    searchTabId = ((ModuleInfo) arrModules[0]).TabID;
                }
            }

            return searchTabId;
        }

        /// <summary>
        ///   Executes the search.
        /// </summary>
        /// <param name = "searchText">The text which will be used to perform the search.</param>
        /// <param name = "searchType">The type of the search. Use "S" for a site search, and "W" for a web search.</param>
        /// <remarks>
        ///   All web based searches will open in a new window, while site searches will open in the current window.  A site search uses the built
        ///   in search engine to perform the search, while both web based search variants will use an external search engine to perform a search.
        /// </remarks>
        protected void ExecuteSearch(string searchText, string searchType)
        {
            int searchTabId = GetSearchTabId();

            if (searchTabId == Null.NullInteger)
            {
                return;
            }
            string strURL;
            if (!string.IsNullOrEmpty(searchText))
            {
                switch (searchType)
                {
                    case "S":
                        // site
                        if (UseWebForSite)
                        {
                            strURL = SiteURL;
                            if (!string.IsNullOrEmpty(strURL))
                            {
                                strURL = strURL.Replace("[TEXT]", Server.UrlEncode(searchText));
                                strURL = strURL.Replace("[DOMAIN]", Request.Url.Host);
                                UrlUtils.OpenNewWindow(Page, GetType(), strURL);
                            }
                        }
                        else
                        {
                            if (Host.UseFriendlyUrls)
                            {
                                Response.Redirect(Globals.NavigateURL(searchTabId) + "?Search=" + Server.UrlEncode(searchText));
                            }
                            else
                            {
                                Response.Redirect(Globals.NavigateURL(searchTabId) + "&Search=" + Server.UrlEncode(searchText));
                            }
                        }
                        break;
                    case "W":
                        // web
                        strURL = WebURL;
                        if (!string.IsNullOrEmpty(strURL))
                        {
                            strURL = strURL.Replace("[TEXT]", Server.UrlEncode(searchText));
                            strURL = strURL.Replace("[DOMAIN]", "");
                            UrlUtils.OpenNewWindow(Page, GetType(), strURL);
                        }
                        break;
                }
            }
            else
            {
                if (Host.UseFriendlyUrls)
                {
                    Response.Redirect(Globals.NavigateURL(searchTabId));
                }
                else
                {
                    Response.Redirect(Globals.NavigateURL(searchTabId));
                }
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdSearch.Click += cmdSearch_Click;
            cmdSearchNew.Click += cmdSearchNew_Click;

            if (!Page.IsPostBack)
            {
                optWeb.Text = WebText;
                optWeb.ToolTip = WebToolTip;
                optSite.Text = SiteText;
                optSite.ToolTip = SiteToolTip;
                optWeb.Visible = ShowWeb;
                optSite.Visible = ShowSite;
                downArrow.AlternateText = Localization.GetString("DropDownGlyph.AltText", Localization.GetResourceFile(this, MyFileName));
                downArrow.ToolTip = Localization.GetString("DropDownGlyph.AltText", Localization.GetResourceFile(this, MyFileName));
                if (optWeb.Visible)
                {
                    optWeb.Checked = true;
                }
                if (optSite.Visible)
                {
                    optSite.Checked = true;
                }
                ClientAPI.RegisterKeyCapture(txtSearch, cmdSearch, 13);
                ClientAPI.RegisterKeyCapture(txtSearchNew, cmdSearchNew, 13);

                if (!String.IsNullOrEmpty(Submit))
                {
                    if (Submit.IndexOf("src=") != -1)
                    {
                        Submit = Submit.Replace("src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath);
                        Submit = Submit.Replace("src='", "src='" + PortalSettings.ActiveTab.SkinPath);
                    }
                }
                else
                {
                    Submit = Localization.GetString("Search", Localization.GetResourceFile(this, MyFileName));
                }
                cmdSearch.Text = Submit;
                cmdSearchNew.Text = Submit;
                if (!String.IsNullOrEmpty(CssClass))
                {
                    optWeb.CssClass = CssClass;
                    optSite.CssClass = CssClass;
                    cmdSearch.CssClass = CssClass;
                    cmdSearchNew.CssClass = CssClass;
                }

                if (Request.QueryString["Search"] != null)
                {
                    txtSearch.Text = Request.QueryString["Search"];
                    txtSearchNew.Text = Request.QueryString["Search"];
                }
            }
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            string SearchType = "S";
            if (optWeb.Visible)
            {
                if (optWeb.Checked)
                {
                    SearchType = "W";
                }
            }
            ExecuteSearch(txtSearch.Text.Trim(), SearchType);
        }

        protected void cmdSearchNew_Click(object sender, EventArgs e)
        {
            ExecuteSearch(txtSearchNew.Text.Trim(), ClientAPI.GetClientVariable(Page, "SearchIconSelected"));
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ClassicSearch.Visible = !UseDropDownList;
            DropDownSearch.Visible = UseDropDownList;
            if (UseDropDownList)
            {
                if (!Page.IsPostBack)
                {
                    ClientAPI.RegisterClientVariable(Page, "SearchIconWebUrl", string.Format("url({0})", ResolveUrl(WebIconURL)), true);
                    ClientAPI.RegisterClientVariable(Page, "SearchIconSiteUrl", string.Format("url({0})", ResolveUrl(SiteIconURL)), true);
                    ClientAPI.RegisterClientVariable(Page, "SearchIconSelected", "S", true);
                }
                string script = string.Format(Globals.glbScriptFormat, ResolveUrl("~/Resources/Search/Search.js"));
                ClientAPI.RegisterStartUpScript(Page, "initSearch", script);
            }
        }
    }
}