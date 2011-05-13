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
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Entities.Host;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Framework
{
    public class jQuery
    {
        private const string jQueryDebugFile = "~/Resources/Shared/Scripts/jquery/jquery.js";
        private const string jQueryMinFile = "~/Resources/Shared/Scripts/jquery/jquery.min.js";
        private const string jQueryVersionKey = "jQueryVersionKey";
        private const string jQueryVersionMatch = "(?<=jquery:\\s\")(.*)(?=\")";
 /// <summary>
 /// Returns the default URL for a hosted version of the jQuery script
 /// </summary>
 /// <remarks>
 /// Google hosts versions of many popular javascript libraries on their CDN.
 /// Using the hosted version increases the likelihood that the file is already
 /// cached in the users browser.
 /// </remarks>
        public const string DefaultHostedUrl = "http://ajax.googleapis.com/ajax/libs/jquery/1/jquery.min.js";

        private const string jQueryUIDebugFile = "~/Resources/Shared/Scripts/jquery/jquery-ui.js";
        private const string jQueryUIMinFile = "~/Resources/Shared/Scripts/jquery/jquery-ui.min.js";
        private const string jQueryUIVersionKey = "jQueryUIVersionKey";
        private const string jQueryUIVersionMatch = "(?<=version:\\s\")(.*)(?=\")";
        public const string DefaultUIHostedUrl = "http://ajax.googleapis.com/ajax/libs/jqueryui/1/jquery-ui.min.js";

        #region Public Properties

        public static string HostedUrl
        {
            get
            {
                return Host.jQueryUrl;
            }
        }

        public static string HostedUIUrl
        {
            get
            {
                return Host.jQueryUIUrl;
            }
        }

        /// <summary>
        /// Checks whether the jQuery core script file exists locally.
        /// </summary>
        /// <remarks>
        /// This property checks for both the minified version and the full uncompressed version of jQuery.
        /// These files should exist in the /Resources/Shared/Scripts/jquery directory.
        /// </remarks>
        public static bool IsInstalled
        {
            get
            {
                string minFile = JQueryFileMapPath(true);
                string dbgFile = JQueryFileMapPath(false);
                return File.Exists(minFile) || File.Exists(dbgFile);
            }
        }

        public static bool IsUIInstalled
        {
            get
            {
                string minFile = JQueryUIFileMapPath(true);
                string dbgFile = JQueryUIFileMapPath(false);
                return File.Exists(minFile) || File.Exists(dbgFile);
            }
        }
        public static bool IsRequested
        {
            get
            {
                return GetSettingAsBoolean("jQueryRequested", false);
            }
        }

        public static bool IsUIRequested
        {
            get
            {
                return GetSettingAsBoolean("jQueryUIRequested", false);
            }
        }

        public static bool AreDnnPluginsRequested
        {
            get
            {
                return GetSettingAsBoolean("jQueryDnnPluginsRequested", false);
            }
        }

        /// <summary>
        /// Gets the HostSetting to determine if we should use the standard jQuery script or the minified jQuery script.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>This is a simple wrapper around the Host.jQueryDebug property</remarks>
        public static bool UseDebugScript
        {
            get
            {
                return Host.jQueryDebug;
            }
        }

        /// <summary>
        /// Gets the HostSetting to determine if we should use a hosted version of the jQuery script.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>This is a simple wrapper around the Host.jQueryHosted property</remarks>
        public static bool UseHostedScript
        {
            get
            {
                return Host.jQueryHosted;
            }
        }

        public static string Version
        {
            get
            {
                string ver = Convert.ToString(DataCache.GetCache(jQueryVersionKey));
                if (string.IsNullOrEmpty(ver))
                {
                    if (IsInstalled)
                    {
                        string jqueryFileName = JQueryFileMapPath(false);
                        string jfiletext = File.ReadAllText(jqueryFileName);
                        Match verMatch = Regex.Match(jfiletext, jQueryVersionMatch);
                        if (verMatch != null)
                        {
                            ver = verMatch.Value;
                            DataCache.SetCache(jQueryVersionKey, ver, new CacheDependency(jqueryFileName));
                        }
                        else
                        {
                            ver = Localization.GetString("jQuery.UnknownVersion.Text");
                        }
                    }
                    else
                    {
                        ver = Localization.GetString("jQuery.NotInstalled.Text");
                    }
                }
                return ver;
            }
        }

        public static string UIVersion
        {
            get
            {
                string ver = Convert.ToString(DataCache.GetCache(jQueryUIVersionKey));
                if (string.IsNullOrEmpty(ver))
                {
                    if (IsUIInstalled)
                    {
                        string jqueryUIFileName = JQueryUIFileMapPath(false);
                        string jfiletext = File.ReadAllText(jqueryUIFileName);
                        Match verMatch = Regex.Match(jfiletext, jQueryUIVersionMatch);
                        if (verMatch != null)
                        {
                            ver = verMatch.Value;
                            DataCache.SetCache(jQueryUIVersionKey, ver, new CacheDependency(jqueryUIFileName));
                        }
                        else
                        {
                            ver = Localization.GetString("jQueryUI.UnknownVersion.Text");
                        }
                    }
                    else
                    {
                        ver = Localization.GetString("jQueryUI.NotInstalled.Text");
                    }
                }
                return ver;
            }
        }
        #endregion

        #region Private Methods

        private static bool GetSettingAsBoolean(string key, bool defaultValue)
        {
            bool retValue = defaultValue;
            try
            {
                object setting = HttpContext.Current.Items[key];
                if (setting != null)
                {
                    retValue = Convert.ToBoolean(setting);
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            return retValue;
        }

        private static bool IsScriptRegistered(string key)
        {
            return HttpContext.Current.Items[key] != null;
        }

        private static void RegisterScript(Page page, string key, string script)
        {
            if (!IsScriptRegistered(key))
            {
                HttpContext.Current.Items[key] = true;
                var headscript = new Literal { Text = script };
                PlaceHolder placeHolder;
                if (key=="jQuery")
                {
                    //load in Head
                    placeHolder = page.Header.FindControl("SCRIPTS") as PlaceHolder;
                    if (placeHolder != null)
                    {
                        placeHolder.Controls.AddAt(0, headscript);
                    }
                }
                else
                {
                    //load in Body
                    placeHolder = page.FindControl("BodySCRIPTS") as PlaceHolder;
                    if (placeHolder != null)
                    {
                        placeHolder.Controls.AddAt(0, headscript);
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public static string JQueryFileMapPath(bool getMinFile)
        {
            return HttpContext.Current.Server.MapPath(JQueryFile(getMinFile));
        }

        public static string JQueryUIFileMapPath(bool getMinFile)
        {
            return HttpContext.Current.Server.MapPath(JQueryUIFile(getMinFile));
        }

        public static string JQueryFile(bool getMinFile)
        {
            string jfile = jQueryDebugFile;
            if (getMinFile)
            {
                jfile = jQueryMinFile;
            }
            return Globals.ResolveUrl(jfile);
        }

        public static string JQueryUIFile(bool getMinFile)
        {
            string jfile = jQueryUIDebugFile;
            if (getMinFile)
            {
                jfile = jQueryUIMinFile;
            }
            return Globals.ResolveUrl(jfile);
        }

        public static string GetJQueryScriptReference()
        {
            string scriptsrc = HostedUrl;
            if (!UseHostedScript)
            {
                scriptsrc = JQueryFile(!UseDebugScript);
            }
            return string.Format(Globals.glbScriptFormat, scriptsrc);
        }

        public static string GetJQueryUIScriptReference()
        {
            string scriptsrc = HostedUIUrl;
            if (!UseHostedScript)
            {
                scriptsrc = JQueryUIFile(!UseDebugScript);
            }
            return string.Format(Globals.glbScriptFormat, scriptsrc);
        }

        public static void RegisterJQuery(Page page)
        {
            RegisterScript(page, "jQuery", GetJQueryScriptReference());
        }

        public static void RegisterJQueryUI(Page page)
        {
            //Ensure jQuery is registered first
            RegisterJQuery(page);

            RegisterScript(page, "jQueryUI", GetJQueryUIScriptReference());
        }

        public static void RegisterDnnJQueryPlugins(Page page)
        {
            RegisterJQueryUI(page);
            page.ClientScript.RegisterClientScriptInclude("dnnJqueryPlugins", page.ResolveUrl("~/js/dnn.jquery.js"));
        }

        public static void RequestRegistration()
        {
            HttpContext.Current.Items["jQueryRequested"] = true;
        }

        public static void RequestUIRegistration()
        {
            HttpContext.Current.Items["jQueryUIRequested"] = true;
        }

        public static void RequestDnnPluginsRegistration()
        {
            HttpContext.Current.Items["jQueryDnnPluginsRequested"] = true;
        }

        #endregion

        #region Obsolete Members

        [Obsolete("Deprecated in DNN 5.1. Replaced by IsRequested.")]
        public static bool IsRquested
        {
            get
            {
                return IsRequested;
            }
        }

        [Obsolete("Deprecated in DNN 6.0 Replaced by RegisterJQuery.")]
        public static void RegisterScript(Page page)
        {
            RegisterScript(page, GetJQueryScriptReference());
        }

        [Obsolete("Deprecated in DNN 6.0 Replaced by RegisterJQuery.")]
        public static void RegisterScript(Page page, string script)
        {
            RegisterScript(page, "jQuery", script);
        }

        #endregion
    }
}
