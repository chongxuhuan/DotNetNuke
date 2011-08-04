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
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Tokens;

#endregion

namespace DotNetNuke.UI.Skins.Controls
{
    public class LanguageTokenReplace : TokenReplace
    {
        //see http://support.dotnetnuke.com/issue/ViewIssue.aspx?id=6505
		public LanguageTokenReplace() : base(Scope.NoSettings)
        {
            UseObjectLessExpression = true;
            PropertySource[ObjectLessToken] = new LanguagePropertyAccess(this, Globals.GetPortalSettings());
        }

        public string resourceFile { get; set; }
    }

    public class LanguagePropertyAccess : IPropertyAccess
    {
        private readonly PortalSettings objPortal;
        public LanguageTokenReplace objParent;

        public LanguagePropertyAccess(LanguageTokenReplace parent, PortalSettings settings)
        {
            objPortal = settings;
            objParent = parent;
        }

        #region IPropertyAccess Members

        public string GetProperty(string propertyName, string format, CultureInfo formatProvider, UserInfo AccessingUser, Scope CurrentScope, ref bool PropertyNotFound)
        {
            switch (propertyName.ToLowerInvariant())
            {
                case "url":
                    return newUrl(objParent.Language);
                case "flagsrc":
                    return "/" + objParent.Language + ".gif";
                case "selected":
                    return (objParent.Language == CultureInfo.CurrentCulture.Name).ToString();
                case "label":
                    return Localization.GetString("Label", objParent.resourceFile);
                case "i":
                    return Globals.ResolveUrl("~/images/Flags");
                case "p":
                    return Globals.ResolveUrl(PathUtils.Instance.RemoveTrailingSlash(objPortal.HomeDirectory));
                case "s":
                    return Globals.ResolveUrl(PathUtils.Instance.RemoveTrailingSlash(objPortal.ActiveTab.SkinPath));
                case "g":
                    return Globals.ResolveUrl("~/portals/" + Globals.glbHostSkinFolder);
                default:
                    PropertyNotFound = true;
                    return string.Empty;
            }
        }

        public CacheLevel Cacheability
        {
            get
            {
                return CacheLevel.fullyCacheable;
            }
        }

        #endregion

        /// <summary>
        /// getQSParams builds up a new querystring. This is necessary
        /// in order to prep for navigateUrl.
        /// we don't ever want a tabid, a ctl and a language parameter in the qs
        /// also, the portalid param is not allowed when the tab is a supertab
        /// (because NavigateUrl adds the portalId param to the qs)
        /// </summary>
        /// <history>
        ///     [erikvb]   20070814    added
        /// </history>
        private string[] getQSParams(string newLanguage, bool isLocalized)
        {
            string returnValue = "";
            NameValueCollection coll = HttpContext.Current.Request.QueryString;
            string[] arrKeys;
            string[] arrValues;
            PortalSettings settings = PortalController.GetCurrentPortalSettings();
            arrKeys = coll.AllKeys;

            for (int i = 0; i <= arrKeys.GetUpperBound(0); i++)
            {
                if (arrKeys[i] != null)
                {
                    switch (arrKeys[i].ToLowerInvariant())
                    {
                        case "tabid":
                        case "ctl":
                        case "language": //skip parameter
                            break;
                        case "mid":
                        case "moduleid": //start of patch (Manzoni Fausto)
                            if (isLocalized)
                            {
                                string ModuleIdKey = arrKeys[i].ToLowerInvariant();
                                int ModuleID = 0;
                                int tabid = 0;

                                int.TryParse(coll[ModuleIdKey], out ModuleID);
                                int.TryParse(coll["tabid"], out tabid);
                                ModuleInfo localizedModule = new ModuleController().GetModuleByCulture(ModuleID, tabid, settings.PortalId, LocaleController.Instance.GetLocale(newLanguage));
                                if (localizedModule != null)
                                {
                                    if (!string.IsNullOrEmpty(returnValue))
                                    {
                                        returnValue += "&";
                                    }
                                    returnValue += ModuleIdKey + "=" + localizedModule.ModuleID;
                                }
                            }
                            break;
                        default:
                            if ((arrKeys[i].ToLowerInvariant() == "portalid") && objPortal.ActiveTab.IsSuperTab)
                            {
								//skip parameter
                                //navigateURL adds portalid to querystring if tab is superTab
                            }
                            else
                            {
                                arrValues = coll.GetValues(i);
                                for (int j = 0; j <= arrValues.GetUpperBound(0); j++)
                                {
                                    if (!String.IsNullOrEmpty(returnValue))
                                    {
                                        returnValue += "&";
                                    }
                                    returnValue += arrKeys[i] + "=" + HttpUtility.HtmlEncode(arrValues[j]);
                                }
                            }
                            break;
                    }
                }
            }

            if (!settings.ContentLocalizationEnabled && LocaleController.Instance.GetLocales(settings.PortalId).Count > 1 && !settings.EnableUrlLanguage)
            {
                //because useLanguageInUrl is false, navigateUrl won't add a language param, so we need to do that ourselves
                if (returnValue != "")
                {
                    returnValue += "&";
                }
                returnValue += "language=" + newLanguage.ToLower();
            }

            //return the new querystring as a string array
            return returnValue.Split('&');
        }

        /// <summary>
        /// newUrl returns the new URL based on the new language.
        /// Basically it is just a call to NavigateUrl, with stripped qs parameters
        /// </summary>
        /// <param name="newLanguage"></param>
        /// <history>
        ///     [erikvb]   20070814    added
        /// </history>
        private string newUrl(string newLanguage)
        {
            var objSecurity = new PortalSecurity();
            Locale newLocale = LocaleController.Instance.GetLocale(newLanguage);

            //Ensure that the current ActiveTab is the culture of the new language
            int tabId = objPortal.ActiveTab.TabID;
            bool islocalized = false;

            TabInfo localizedTab = new TabController().GetTabByCulture(tabId, objPortal.PortalId, newLocale);
			if (localizedTab != null)
			{
				islocalized = true;
				tabId = localizedTab.TabID;
			}

        	return
                objSecurity.InputFilter(
                    Globals.NavigateURL(tabId, objPortal.ActiveTab.IsSuperTab, objPortal, HttpContext.Current.Request.QueryString["ctl"], newLanguage, getQSParams(newLocale.Code, islocalized)),
                    PortalSecurity.FilterFlag.NoScripting);
        }
    }
}
