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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Collections.Internal;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Modules;

#endregion

namespace DotNetNuke.Framework
{
    public abstract class PageBase : Page
    {
        private readonly NameValueCollection _htmlAttributes = new NameValueCollection();
        private readonly ArrayList _localizedControls;
        private CultureInfo _pageCulture;
        private string _localResourceFile;

        protected PageBase()
        {
            _localizedControls = new ArrayList();
        }

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                PageStatePersister persister = base.PageStatePersister;
                switch (Host.PageStatePersister)
                {
                    case "M":
                        persister = new CachePageStatePersister(this);
                        break;
                    case "D":
                        persister = new DiskPageStatePersister(this);
                        break;
                }
                return persister;
            }
        }

        public PortalSettings PortalSettings
        {
            get
            {
                return PortalController.GetCurrentPortalSettings();
            }
        }

        public NameValueCollection HtmlAttributes
        {
            get
            {
                return _htmlAttributes;
            }
        }

        public CultureInfo PageCulture
        {
            get
            {
                return _pageCulture ?? (_pageCulture = Localization.GetPageLocale(PortalSettings));
            }
        }

        public string LocalResourceFile
        {
            get
            {
                string fileRoot;
                string[] page = Request.ServerVariables["SCRIPT_NAME"].Split('/');
                if (String.IsNullOrEmpty(_localResourceFile))
                {
                    fileRoot = TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/" + page[page.GetUpperBound(0)] + ".resx";
                }
                else
                {
                    fileRoot = _localResourceFile;
                }
                return fileRoot;
            }
            set
            {
                _localResourceFile = value;
            }
        }

        private static void AddStyleSheetInternal(Page page, string key, bool isFirst)
        {
            var styleSheetDictionary = CBO.GetCachedObject<SharedDictionary<string, string>>(
                                                new CacheItemArgs("CSS", 200, CacheItemPriority.NotRemovable),
                                                cacheItemArgs => new SharedDictionary<string, string>()
                                            );

            using (ISharedCollectionLock readLock = styleSheetDictionary.GetReadLock())
            {
                if (styleSheetDictionary.ContainsKey(key) && !String.IsNullOrEmpty(styleSheetDictionary[key]))
                {
                    string styleSheet = styleSheetDictionary[key];
                    string id = Globals.CreateValidID(key);

                    Control objCSS = page.FindControl("CSS");
                    if (objCSS != null)
                    {
                        Control objCtrl = page.Header.FindControl(id);
                        if (objCtrl == null)
                        {
                            var objLink = new HtmlLink { ID = id };
                            objLink.Attributes["rel"] = "stylesheet";
                            objLink.Attributes["type"] = "text/css";
                            objLink.Href = styleSheet;
                            if (isFirst)
                            {
                                int iLink;
                                for (iLink = 0; iLink <= objCSS.Controls.Count - 1; iLink++)
                                {
                                    if (objCSS.Controls[iLink] is HtmlLink)
                                    {
                                        break;
                                    }
                                }
                                objCSS.Controls.AddAt(iLink, objLink);
                            }
                            else
                            {
                                objCSS.Controls.Add(objLink);
                            }
                        }
                    }
                }
            }
        }

        private static bool IsStyleSheetRegistered(string key)
        {
            var styleSheetDictionary = CBO.GetCachedObject<SharedDictionary<string, string>>(
                                            new CacheItemArgs("CSS", 200, CacheItemPriority.NotRemovable),
                                                (CacheItemArgs cacheItemArgs) => new SharedDictionary<string, string>());

            bool idFound = Null.NullBoolean;
            using (ISharedCollectionLock readLock = styleSheetDictionary.GetReadLock())
            {
                if (styleSheetDictionary.ContainsKey(key))
                {
                    //Return value
                    idFound = true;
                }
            }

            return idFound;
        }

        private void IterateControls(ControlCollection controls, ArrayList affectedControls, string resourceFileRoot)
        {
            foreach (Control c in controls)
            {
                ProcessControl(c, affectedControls, true, resourceFileRoot);
            }
        }

        private static void RegisterStyleSheet(Page page, string key, string styleSheet)
        {
            var styleSheetDictionary = CBO.GetCachedObject<SharedDictionary<string, string>>(
                                                new CacheItemArgs("CSS", 200, CacheItemPriority.NotRemovable),
                                                cacheItemArgs => new SharedDictionary<string, string>()
                                            );

            using (ISharedCollectionLock writeLock = styleSheetDictionary.GetWriteLock())
            {
                if (!styleSheetDictionary.ContainsKey(key))
                {
                    if (File.Exists(page.Server.MapPath(styleSheet)))
                    {
                        styleSheetDictionary[key] = styleSheet;
                    }
                    else
                    {
                        styleSheetDictionary[key] = "";
                    }
                }
            }
        }

        protected override void OnError(EventArgs e)
        {
            base.OnError(e);
            Exception exc = Server.GetLastError();
            DnnLog.Fatal("An error has occurred while loading page.", exc);

            string strURL = Globals.ApplicationURL();
            if ((exc != null) && (ReferenceEquals(exc.GetType(), typeof(HttpException))))
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Server.Transfer("~/ErrorPage.aspx");
            }
            if (Request.QueryString["error"] != null)
            {
                strURL += (strURL.IndexOf("?") == -1 ? "?" : "&") + "error=terminate";
            }
            else
            {
                strURL += (strURL.IndexOf("?") == -1 ? "?" : "&") + "error=" +  (exc == null ? "": Server.UrlEncode(exc.Message));
                if (!Globals.IsAdminControl())
                {
                    strURL += "&content=0";
                }
            }
            Exceptions.ProcessPageLoadException(exc, strURL);
        }

        protected override void OnInit(EventArgs e)
        {
            if (!HttpContext.Current.Request.Url.LocalPath.ToLower().EndsWith("installwizard.aspx"))
            {
                Localization.SetThreadCultures(PageCulture, PortalSettings);
            }


            AJAX.AddScriptManager(this);
            Page.ClientScript.RegisterClientScriptInclude("dnncore", ResolveUrl("~/js/dnncore.js"));
            base.OnInit(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            IterateControls(Controls, _localizedControls, LocalResourceFile);
            RemoveKeyAttribute(_localizedControls);
            AJAX.RemoveScriptManager(this);
            base.Render(writer);
        }

        internal static string GetControlAttribute(Control c, ArrayList affectedControls)
        {
            AttributeCollection ac = null;
            string key = null;
            if (!(c is LiteralControl))
            {
                if (c is WebControl)
                {
                    var w = (WebControl)c;
                    ac = w.Attributes;
                    key = ac[Localization.KeyName];
                }
                else
                {
                    if (c is HtmlControl)
                    {
                        var h = (HtmlControl)c;
                        ac = h.Attributes;
                        key = ac[Localization.KeyName];
                    }
                    else
                    {
                        if (c is UserControl)
                        {
                            var u = (UserControl)c;
                            ac = u.Attributes;
                            key = ac[Localization.KeyName];
                        }
                        else
                        {
                            Type controlType = c.GetType();
                            PropertyInfo attributeProperty = controlType.GetProperty("Attributes", typeof(AttributeCollection));
                            if (attributeProperty != null)
                            {
                                ac = (AttributeCollection)attributeProperty.GetValue(c, null);
                                key = ac[Localization.KeyName];
                            }
                        }
                    }
                }
            }
            if (key != null && affectedControls != null)
            {
                affectedControls.Add(ac);
            }
            return key;
        }

        internal void ProcessControl(Control c, ArrayList affectedControls, bool includeChildren, string ResourceFileRoot)
        {
            string key = GetControlAttribute(c, affectedControls);
            if (!string.IsNullOrEmpty(key))
            {
                string value;
                value = Localization.GetString(key, ResourceFileRoot);
                if (c is Label)
                {
                    Label ctrl;
                    ctrl = (Label)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is LinkButton)
                {
                    LinkButton ctrl;
                    ctrl = (LinkButton)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        MatchCollection imgMatches = Regex.Matches(value, "<(a|link|img|script|input|form).[^>]*(href|src|action)=(\\\"|'|)(.[^\\\"']*)(\\\"|'|)[^>]*>", RegexOptions.IgnoreCase);
                        foreach (Match _match in imgMatches)
                        {
                            if ((_match.Groups[_match.Groups.Count - 2].Value.IndexOf("~") != -1))
                            {
                                string resolvedUrl = Page.ResolveUrl(_match.Groups[_match.Groups.Count - 2].Value);
                                value = value.Replace(_match.Groups[_match.Groups.Count - 2].Value, resolvedUrl);
                            }
                        }
                        ctrl.Text = value;
                        if (string.IsNullOrEmpty(ctrl.ToolTip))
                        {
                            ctrl.ToolTip = value;
                        }
                    }
                }
                if (c is HyperLink)
                {
                    HyperLink ctrl;
                    ctrl = (HyperLink)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is ImageButton)
                {
                    ImageButton ctrl;
                    ctrl = (ImageButton)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.AlternateText = value;
                    }
                }
                if (c is Button)
                {
                    Button ctrl;
                    ctrl = (Button)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is HtmlImage)
                {
                    HtmlImage ctrl;
                    ctrl = (HtmlImage)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Alt = value;
                    }
                }
                if (c is CheckBox)
                {
                    CheckBox ctrl;
                    ctrl = (CheckBox)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is BaseValidator)
                {
                    BaseValidator ctrl;
                    ctrl = (BaseValidator)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.ErrorMessage = value;
                    }
                }
                if (c is Image)
                {
                    Image ctrl;
                    ctrl = (Image)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.AlternateText = value;
                        ctrl.ToolTip = value;
                    }
                }
            }
            if (c is RadioButtonList)
            {
                RadioButtonList ctrl;
                ctrl = (RadioButtonList)c;
                int i;
                for (i = 0; i <= ctrl.Items.Count - 1; i++)
                {
                    AttributeCollection ac = null;
                    ac = ctrl.Items[i].Attributes;
                    key = ac[Localization.KeyName];
                    if (key != null)
                    {
                        string value = Localization.GetString(key, ResourceFileRoot);
                        if (!String.IsNullOrEmpty(value))
                        {
                            ctrl.Items[i].Text = value;
                        }
                    }
                    if (key != null && affectedControls != null)
                    {
                        affectedControls.Add(ac);
                    }
                }
            }
            if (c is DropDownList)
            {
                DropDownList ctrl;
                ctrl = (DropDownList)c;
                int i;
                for (i = 0; i <= ctrl.Items.Count - 1; i++)
                {
                    AttributeCollection ac = null;
                    ac = ctrl.Items[i].Attributes;
                    key = ac[Localization.KeyName];
                    if (key != null)
                    {
                        string value = Localization.GetString(key, ResourceFileRoot);
                        if (!String.IsNullOrEmpty(value))
                        {
                            ctrl.Items[i].Text = value;
                        }
                    }
                    if (key != null && affectedControls != null)
                    {
                        affectedControls.Add(ac);
                    }
                }
            }
            if (c is Image)
            {
                Image ctrl;
                ctrl = (Image)c;
                if ((ctrl.ImageUrl.IndexOf("~") != -1))
                {
                    ctrl.ImageUrl = Page.ResolveUrl(ctrl.ImageUrl);
                }
            }
            if (c is HtmlImage)
            {
                HtmlImage ctrl;
                ctrl = (HtmlImage)c;
                if ((ctrl.Src.IndexOf("~") != -1))
                {
                    ctrl.Src = Page.ResolveUrl(ctrl.Src);
                }
            }
            if (c is HyperLink)
            {
                HyperLink ctrl;
                ctrl = (HyperLink)c;
                if ((ctrl.NavigateUrl.IndexOf("~") != -1))
                {
                    ctrl.NavigateUrl = Page.ResolveUrl(ctrl.NavigateUrl);
                }
                if ((ctrl.ImageUrl.IndexOf("~") != -1))
                {
                    ctrl.ImageUrl = Page.ResolveUrl(ctrl.ImageUrl);
                }
            }
            if (includeChildren && c.HasControls())
            {
                var objModuleControl = c as IModuleControl;
                if (objModuleControl == null)
                {
                    PropertyInfo pi = c.GetType().GetProperty("LocalResourceFile");
                    if (pi != null && pi.GetValue(c, null) != null)
                    {
                        IterateControls(c.Controls, affectedControls, pi.GetValue(c, null).ToString());
                    }
                    else
                    {
                        IterateControls(c.Controls, affectedControls, ResourceFileRoot);
                    }
                }
                else
                {
                    IterateControls(c.Controls, affectedControls, objModuleControl.LocalResourceFile);
                }
            }
        }

        public static void RegisterStyleSheet(Page page, string styleSheet)
        {
            RegisterStyleSheet(page, styleSheet, false);
        }

        public static void RegisterStyleSheet(Page page, string styleSheet, bool isFirst)
        {
            if (!IsStyleSheetRegistered(styleSheet))
            {
                RegisterStyleSheet(page, styleSheet, styleSheet);
            }

            AddStyleSheetInternal(page, styleSheet, isFirst);
        }

        public static void RemoveKeyAttribute(ArrayList affectedControls)
        {
            if (affectedControls == null)
            {
                return;
            }
            int i;
            for (i = 0; i <= affectedControls.Count - 1; i++)
            {
                var ac = (AttributeCollection)affectedControls[i];
                ac.Remove(Localization.KeyName);
            }
        }


    }
}
