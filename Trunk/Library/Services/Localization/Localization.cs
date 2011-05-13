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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;

using DotNetNuke.Collections.Internal;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Tokens;
using DotNetNuke.UI.Modules;

#endregion

namespace DotNetNuke.Services.Localization
{
	/// <summary>
	/// Enumeration of Culture display type in drop down list.
	/// </summary>
    [Serializable]
    public enum CultureDropDownTypes
    {
		/// <summary>
		/// Display CultureInfo.DisplayName, this will be the default value for culture drop down list.
		/// </summary>
        DisplayName,
		/// <summary>
		/// Display CultureInfo.EnglishgName with in title case.
		/// </summary>
        EnglishName,
		/// <summary>
		/// Display CultureInfo.LCID
		/// </summary>
        Lcid,
		/// <summary>
		/// Display CultureInfo.Name
		/// </summary>
        Name,
		/// <summary>
		/// Display CultureInfo.NativeName
		/// </summary>
        NativeName,
		/// <summary>
		/// Display CultureInfo.TwoLetterISOLanguageName
		/// </summary>
        TwoLetterIsoCode,
		/// <summary>
		/// Display CultureInfo.ThreeLetterISOLanguageName
		/// </summary>
        ThreeLetterIsoCode
    }

	/// <summary>
	/// Localization class support localization in system.
	/// </summary>
	/// <remarks>
	/// <para>As DNN is used in more and more countries it is very important to provide modules with 
    /// good support for international users. Otherwise we are limiting our potential user base to 
    /// that using English as their base language.</para>
    /// <para>
    /// You can store the muti language content in resource files and use the api below to get localization content.
    /// Resouces files named as: Control(Page)Name + Extension (.aspx/.ascx ) + Language + ".resx"
	/// e.g: Installwizard.aspx.de-DE.resx
    /// </para>
	/// </remarks>
	/// <example>
	/// <code lang="C#">
	/// pageCreationProgressArea.Localization.Total = Localization.GetString("TotalLanguages", LocalResourceFile);
    /// pageCreationProgressArea.Localization.TotalFiles = Localization.GetString("TotalPages", LocalResourceFile);
    /// pageCreationProgressArea.Localization.Uploaded = Localization.GetString("TotalProgress", LocalResourceFile);
    /// pageCreationProgressArea.Localization.UploadedFiles = Localization.GetString("Progress", LocalResourceFile);
    /// pageCreationProgressArea.Localization.CurrentFileName = Localization.GetString("Processing", LocalResourceFile);
	/// </code>
	/// </example>
    public class Localization
    {
        private static string _defaultKeyName = "resourcekey";
        private static string strShowMissingKeys = "";

        public static string ApplicationResourceDirectory
        {
            get
            {
                return "~/App_GlobalResources";
            }
        }

        public static string ExceptionsResourceFile
        {
            get
            {
                return ApplicationResourceDirectory + "/Exceptions.resx";
            }
        }

        public static string GlobalResourceFile
        {
            get
            {
                return ApplicationResourceDirectory + "/GlobalResources.resx";
            }
        }

        public static string LocalResourceDirectory
        {
            get
            {
                return "App_LocalResources";
            }
        }

        public static string LocalSharedResourceFile
        {
            get
            {
                return "SharedResources.resx";
            }
        }

        public static string SharedResourceFile
        {
            get
            {
                return ApplicationResourceDirectory + "/SharedResources.resx";
            }
        }

        public static string SupportedLocalesFile
        {
            get
            {
                return ApplicationResourceDirectory + "/Locales.xml";
            }
        }

        public static string SystemLocale
        {
            get
            {
                return "en-US";
            }
        }

        public static string SystemTimeZone
        {
            get
            {
                return "Pacific Standard Time";
            }
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The CurrentCulture returns the current Culture being used
        /// is 'key'.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/06/2004	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public string CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.ToString();
            }
        }

        public string CurrentUICulture
        {
            // _CurrentCulture
            get
            {
                return Thread.CurrentThread.CurrentUICulture.ToString();
            }
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The KeyName property returns and caches the name of the key attribute used to lookup resources.
        /// This can be configured by setting ResourceManagerKey property in the web.config file. The default value for this property
        /// is 'key'.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/06/2004	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string KeyName
        {
            get
            {
                return _defaultKeyName;
            }
            set
            {
                _defaultKeyName = value;
                if (_defaultKeyName == null || _defaultKeyName == string.Empty)
                {
                    _defaultKeyName = "resourcekey";
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The ShowMissingKeys property returns the web.config setting that determines
        /// whether to render a visual indicator that a key is missing
        /// is 'key'.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	11/20/2006	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool ShowMissingKeys
        {
            get
            {
                if (string.IsNullOrEmpty(strShowMissingKeys))
                {
                    if (Config.GetSetting("ShowMissingKeys") == null)
                    {
                        strShowMissingKeys = "false";
                    }
                    else
                    {
                        strShowMissingKeys = Config.GetSetting("ShowMissingKeys".ToLower());
                    }
                }
                return bool.Parse(strShowMissingKeys);
            }
        }

        [Obsolete("Deprecated in DNN 6.0. Replaced by SystemTimeZone and use of .NET TimeZoneInfo class")]
        public static int SystemTimeZoneOffset
        {
            get
            {
                return -480;
            }
        }

        [Obsolete("Deprecated in DNN 6.0. Replaced by SystemTimeZone and use of .NET TimeZoneInfo class")]
        public static string TimezonesFile
        {
            get
            {
                return ApplicationResourceDirectory + "/TimeZones.xml";
            }
        }

        private static object GetResourceFileLookupDictionaryCallback(CacheItemArgs cacheItemArgs)
        {
            return new SharedDictionary<string, bool>();
        }

        private static SharedDictionary<string, bool> GetResourceFileLookupDictionary()
        {
            return
                CBO.GetCachedObject<SharedDictionary<string, bool>>(
                    new CacheItemArgs(DataCache.ResourceFileLookupDictionaryCacheKey, DataCache.ResourceFileLookupDictionaryTimeOut, DataCache.ResourceFileLookupDictionaryCachePriority),
                    GetResourceFileLookupDictionaryCallback,
                    true);
        }

        private static object GetResourceFileCallBack(CacheItemArgs cacheItemArgs)
        {
            string cacheKey = cacheItemArgs.CacheKey;
            Dictionary<string, string> resources = null;
            SharedDictionary<string, bool> resourceFileExistsLookup = GetResourceFileLookupDictionary();

            if(ResourceFileMayExist(resourceFileExistsLookup, cacheKey))
            {
                string filePath = null;
                if (cacheKey.Contains(":\\") && Path.IsPathRooted(cacheKey))
                {
                    if (File.Exists(cacheKey))
                    {
                        filePath = cacheKey;
                    }
                }
                if (filePath == null)
                {
                    filePath = HostingEnvironment.MapPath(Globals.ApplicationPath + cacheKey);
                }
                if (File.Exists(filePath))
                {
                    XPathDocument doc = null;
                    doc = new XPathDocument(filePath);
                    resources = new Dictionary<string, string>();
                    foreach (XPathNavigator nav in doc.CreateNavigator().Select("root/data"))
                    {
                        if (nav.NodeType != XPathNodeType.Comment)
                        {
                            resources[nav.GetAttribute("name", string.Empty)] = nav.SelectSingleNode("value").Value;
                        }
                    }
                    cacheItemArgs.CacheDependency = new DNNCacheDependency(filePath);

                    using (resourceFileExistsLookup.GetWriteLock())
                    {
                        resourceFileExistsLookup[cacheKey] = true;    
                    }
                }
                else
                {
                    using (resourceFileExistsLookup.GetWriteLock())
                    {
                        resourceFileExistsLookup[cacheKey] = false;
                    }
                }
            }
            return resources;
        }

	    private static bool ResourceFileMayExist(SharedDictionary<string, bool> resourceFileExistsLookup, string cacheKey)
	    {
	        bool mayExist;
	        using(resourceFileExistsLookup.GetReadLock())
	        {
	            if( ! resourceFileExistsLookup.ContainsKey(cacheKey) )
	            {
	                mayExist = true;
	            }
	            else
	            {
	                mayExist = resourceFileExistsLookup[cacheKey];
	            }
	        }
	        return mayExist;
	    }

	    private static Dictionary<string, string> GetResourceFile(string resourceFile)
        {
            return CBO.GetCachedObject<Dictionary<string, string>>(new CacheItemArgs(resourceFile, DataCache.ResourceFilesCacheTimeOut, DataCache.ResourceFilesCachePriority),
                                                                   GetResourceFileCallBack,
                                                                   true);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetResourceFileName is used to build the resource file name according to the
        /// language
        /// </summary>
        /// <param name="language">The language</param>
        /// <param name="ResourceFileRoot">The resource file root</param>
        /// <returns>The language specific resource file</returns>
        /// <history>
        /// 	[cnurse]	10/06/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static string GetResourceFileName(string ResourceFileRoot, string language)
        {
            string ResourceFile;
            language = language.ToLower();
            if (ResourceFileRoot != null)
            {
                if (language == SystemLocale.ToLower() || String.IsNullOrEmpty(language))
                {
                    switch (ResourceFileRoot.Substring(ResourceFileRoot.Length - 5, 5).ToLower())
                    {
                        case ".resx":
                            ResourceFile = ResourceFileRoot;
                            break;
                        case ".ascx":
                            ResourceFile = ResourceFileRoot + ".resx";
                            break;
                        case ".aspx":
                            ResourceFile = ResourceFileRoot + ".resx";
                            break;
                        default:
                            ResourceFile = ResourceFileRoot + ".ascx.resx";
                            break;
                    }
                }
                else
                {
                    switch (ResourceFileRoot.Substring(ResourceFileRoot.Length - 5, 5).ToLower())
                    {
                        case ".resx":
                            ResourceFile = ResourceFileRoot.Replace(".resx", "." + language + ".resx");
                            break;
                        case ".ascx":
                            ResourceFile = ResourceFileRoot.Replace(".ascx", ".ascx." + language + ".resx");
                            break;
                        case ".aspx":
                            ResourceFile = ResourceFileRoot.Replace(".aspx", ".aspx." + language + ".resx");
                            break;
                        default:
                            ResourceFile = ResourceFileRoot + ".ascx." + language + ".resx";
                            break;
                    }
                }
            }
            else
            {
                if (language == SystemLocale.ToLower() || String.IsNullOrEmpty(language))
                {
                    ResourceFile = SharedResourceFile;
                }
                else
                {
                    ResourceFile = SharedResourceFile.Replace(".resx", "." + language + ".resx");
                }
            }
            return ResourceFile;
        }

        private static string GetStringInternal(string key, string userLanguage, string resourceFileRoot, PortalSettings objPortalSettings, bool disableShowMissngKeys)
        {
            if (key.IndexOf(".") < 1)
            {
                key += ".Text";
            }
            string resourceValue = Null.NullString;
            bool bFound = TryGetStringInternal(key, userLanguage, resourceFileRoot, objPortalSettings, ref resourceValue);
            if (ShowMissingKeys && !disableShowMissngKeys)
            {
                if (bFound)
                {
                    resourceValue = "[L]" + resourceValue;
                }
                else
                {
                    resourceValue = "RESX:" + key;
                }
            }
            return resourceValue;
        }

        private static Dictionary<string, string> LoadResourceFileCallback(CacheItemArgs cacheItemArgs)
        {
            var fileName = (string) cacheItemArgs.ParamList[0];
            string filePath = HttpContext.Current.Server.MapPath(fileName);
            var dicResources = new Dictionary<string, string>();
            if (File.Exists(filePath))
            {
                XPathDocument doc = null;
                try
                {
                    doc = new XPathDocument(filePath);
                    foreach (XPathNavigator nav in doc.CreateNavigator().Select("root/data"))
                    {
                        if (nav.NodeType != XPathNodeType.Comment)
                        {
                            dicResources[nav.GetAttribute("name", string.Empty)] = nav.SelectSingleNode("value").Value;
                        }
                    }
                }
				catch (Exception ex)
				{
					DnnLog.Error(ex);
				}
            }
            return dicResources;
        }

        /// <summary>
        /// Provides localization support for DataControlFields used in DetailsView and GridView controls
        /// </summary>
        /// <param name="controlField">The field control to localize</param>
        /// <param name="resourceFile">The root name of the Resource File where the localized
        ///   text can be found</param>
        /// <remarks>
        /// The header of the DataControlField is localized.
        /// It also localizes text for following controls: ButtonField, CheckBoxField, CommandField, HyperLinkField, ImageField
        /// </remarks>
        private static void LocalizeDataControlField(DataControlField controlField, string resourceFile)
        {
            string localizedText;
            if (!string.IsNullOrEmpty(controlField.HeaderText))
            {
                localizedText = GetString((controlField.HeaderText + ".Header"), resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    controlField.HeaderText = localizedText;
                    controlField.AccessibleHeaderText = controlField.HeaderText;
                }
            }
            if (controlField is TemplateField)
            {
            }
            else if (controlField is ButtonField)
            {
                var button = (ButtonField) controlField;
                localizedText = GetString(button.Text, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    button.Text = localizedText;
                }
            }
            else if (controlField is CheckBoxField)
            {
                var checkbox = (CheckBoxField) controlField;
                localizedText = GetString(checkbox.Text, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    checkbox.Text = localizedText;
                }
            }
            else if (controlField is CommandField)
            {
                var commands = (CommandField) controlField;
                localizedText = GetString(commands.CancelText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    commands.CancelText = localizedText;
                }
                localizedText = GetString(commands.DeleteText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    commands.DeleteText = localizedText;
                }
                localizedText = GetString(commands.EditText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    commands.EditText = localizedText;
                }
                localizedText = GetString(commands.InsertText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    commands.InsertText = localizedText;
                }
                localizedText = GetString(commands.NewText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    commands.NewText = localizedText;
                }
                localizedText = GetString(commands.SelectText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    commands.SelectText = localizedText;
                }
                localizedText = GetString(commands.UpdateText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    commands.UpdateText = localizedText;
                }
            }
            else if (controlField is HyperLinkField)
            {
                var link = (HyperLinkField) controlField;
                localizedText = GetString(link.Text, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    link.Text = localizedText;
                }
            }
            else if (controlField is ImageField)
            {
                var image = (ImageField) controlField;
                localizedText = GetString(image.AlternateText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    image.AlternateText = localizedText;
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Returns the TimeZone file name for a given resource and language
        /// </summary>
        /// <param name="filename">Resource File</param>
        /// <param name="language">Language</param>
        /// <returns>Localized File Name</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	04/10/2004	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private string TimeZoneFile(string filename, string language)
        {
            if (language == SystemLocale)
            {
                return filename;
            }
            else
            {
                return filename.Substring(0, filename.Length - 4) + "." + language + ".xml";
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// TryGetFromResourceFile is used to get the string from a resource file
        /// </summary>
        /// <remarks>This method searches a resource file for the key.  It first checks the
        /// user's language, then the fallback language and finally the default language.
        /// </remarks>
        /// <param name="key">The resource key</param>
        /// <param name="resourceFile">The resource file to search</param>
        /// <param name="userLanguage">The user's language</param>
        /// <param name="fallbackLanguage">The fallback language for the user's language</param>
        /// <param name="defaultLanguage">The portal's default language</param>
        /// <param name="portalID">The id of the portal</param>
        /// <param name="resourceValue">The resulting resource value - returned by reference</param>
        /// <returns>True if successful, false if not found</returns>
        /// <history>
        /// 	[cnurse]	01/30/2008	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static bool TryGetFromResourceFile(string key, string resourceFile, string userLanguage, string fallbackLanguage, string defaultLanguage, int portalID, ref string resourceValue)
        {
            bool bFound = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, userLanguage), portalID, ref resourceValue);
            if (!bFound && fallbackLanguage != userLanguage)
            {
                bFound = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, fallbackLanguage), portalID, ref resourceValue);
            }
            if (!bFound && !(defaultLanguage == userLanguage || defaultLanguage == fallbackLanguage))
            {
                bFound = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, defaultLanguage), portalID, ref resourceValue);
            }
            return bFound;
        }

        private static bool TryGetFromResourceFile(string key, string resourceFile, int portalID, ref string resourceValue)
        {
            bool bFound = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.Portal, ref resourceValue);
            if (!bFound)
            {
                bFound = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.Host, ref resourceValue);
            }
            if (!bFound)
            {
                bFound = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.None, ref resourceValue);
            }
            return bFound;
        }

        private static bool TryGetFromResourceFile(string key, string resourceFile, int portalID, CustomizedLocale resourceType, ref string resourceValue)
        {
            Dictionary<string, string> dicResources = null;
            bool bFound = Null.NullBoolean;
            string resourceFileName = resourceFile;
            switch (resourceType)
            {
                case CustomizedLocale.Host:
                    resourceFileName = resourceFile.Replace(".resx", ".Host.resx");
                    break;
                case CustomizedLocale.Portal:
                    resourceFileName = resourceFile.Replace(".resx", ".Portal-" + portalID + ".resx");
                    break;
            }
            if (resourceFileName.StartsWith("desktopmodules", StringComparison.InvariantCultureIgnoreCase) || resourceFileName.StartsWith("admin", StringComparison.InvariantCultureIgnoreCase))
            {
                resourceFileName = "~/" + resourceFileName;
            }
            string cacheKey = resourceFileName.Replace("~/", "/").ToLowerInvariant();
            if (!string.IsNullOrEmpty(Globals.ApplicationPath))
            {
                if (Globals.ApplicationPath != "/portals")
                {
                    if (cacheKey.StartsWith(Globals.ApplicationPath))
                    {
                        cacheKey = cacheKey.Substring(Globals.ApplicationPath.Length);
                    }
                }
                else
                {
                    cacheKey = "~" + cacheKey;
                    if (cacheKey.StartsWith("~" + Globals.ApplicationPath))
                    {
                        cacheKey = cacheKey.Substring(Globals.ApplicationPath.Length + 1);
                    }
                }
            }
            
            SharedDictionary<string, bool> resourceFileExistsLookup = GetResourceFileLookupDictionary();
            if(ResourceFileMayExist(resourceFileExistsLookup, cacheKey))
            {
                dicResources = GetResourceFile(cacheKey);
                if (dicResources != null)
                {
                    bFound = dicResources.TryGetValue(key, out resourceValue);
                }
            }
            
            return bFound;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// TryGetStringInternal is used to get the string from a resource file
        /// </summary>
        /// <remarks>This method searches a resource file for the key.  It first checks the
        /// user's language, then the fallback language and finally the default language.
        /// </remarks>
        /// <param name="key">The resource key</param>
        /// <param name="userLanguage">The user's language</param>
        /// <param name="resourceFile">The resource file to search</param>
        /// <param name="objPortalSettings">The portal settings</param>
        /// <param name="resourceValue">The resulting resource value - returned by reference</param>
        /// <returns>True if successful, false if not found</returns>
        /// <history>
        /// 	[cnurse]	01/30/2008	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static bool TryGetStringInternal(string key, string userLanguage, string resourceFile, PortalSettings objPortalSettings, ref string resourceValue)
        {
            string defaultLanguage = Null.NullString;
            string fallbackLanguage = SystemLocale;
            int portalId = Null.NullInteger;
            if (objPortalSettings != null)
            {
                defaultLanguage = objPortalSettings.DefaultLanguage;
                portalId = objPortalSettings.PortalId;
            }
            if (string.IsNullOrEmpty(userLanguage))
            {
                userLanguage = Thread.CurrentThread.CurrentUICulture.ToString();
            }
            if (string.IsNullOrEmpty(userLanguage))
            {
                userLanguage = defaultLanguage;
            }
            Locale userLocale = null;
            try
            {
                userLocale = LocaleController.Instance.GetLocale(userLanguage);
            }
			catch (Exception ex)
			{
				DnnLog.Error(ex);
			}

            if (userLocale != null && !string.IsNullOrEmpty(userLocale.Fallback))
            {
                fallbackLanguage = userLocale.Fallback;
            }
            if (string.IsNullOrEmpty(resourceFile))
            {
                resourceFile = SharedResourceFile;
            }
            bool bFound = TryGetFromResourceFile(key, resourceFile, userLanguage, fallbackLanguage, defaultLanguage, portalId, ref resourceValue);
            if (!bFound)
            {
                if (!(SharedResourceFile.ToLowerInvariant() == resourceFile.ToLowerInvariant()))
                {
                    string localSharedFile = resourceFile.Substring(0, resourceFile.LastIndexOf("/") + 1) + LocalSharedResourceFile;
                    if (!(localSharedFile.ToLowerInvariant() == resourceFile.ToLowerInvariant()))
                    {
                        bFound = TryGetFromResourceFile(key, localSharedFile, userLanguage, fallbackLanguage, defaultLanguage, portalId, ref resourceValue);
                    }
                }
            }
            if (!bFound)
            {
                if (!(SharedResourceFile.ToLowerInvariant() == resourceFile.ToLowerInvariant()))
                {
                    bFound = TryGetFromResourceFile(key, SharedResourceFile, userLanguage, fallbackLanguage, defaultLanguage, portalId, ref resourceValue);
                }
            }
            return bFound;
        }

        public string GetFixedCurrency(decimal Expression, string Culture, int NumDigitsAfterDecimal)
        {
            string oldCurrentCulture = CurrentUICulture;
            var newCulture = new CultureInfo(Culture);
            Thread.CurrentThread.CurrentUICulture = newCulture;
            string currencyStr = Expression.ToString(newCulture.NumberFormat.CurrencySymbol);
            var oldCulture = new CultureInfo(oldCurrentCulture);
            Thread.CurrentThread.CurrentUICulture = oldCulture;
            return currencyStr;
        }

        public string GetFixedDate(DateTime Expression, string Culture)
        {
            string oldCurrentCulture = CurrentUICulture;
            var newCulture = new CultureInfo(Culture);
            Thread.CurrentThread.CurrentUICulture = newCulture;
            string dateStr = Expression.ToString(newCulture.DateTimeFormat.FullDateTimePattern);
            var oldCulture = new CultureInfo(oldCurrentCulture);
            Thread.CurrentThread.CurrentUICulture = oldCulture;
            return dateStr;
        }

        public static int ActiveLanguagesByPortalID(int portalID)
        {
            //Default to 1 (maybe called during portal creation before languages are enabled for portal)

            int count = 1;
            Dictionary<string, Locale> locales = LocaleController.Instance.GetLocales(portalID);
            if (locales != null)
            {
                count = locales.Count;
            }
            return count;
        }

        public static void AddLanguageToPortal(int portalID, int languageID, bool clearCache)
        {
            //We need to add a translator role for the language
            Locale language = LocaleController.Instance.GetLocale(languageID);
            bool contentLocalizationEnabled = PortalController.GetPortalSettingAsBoolean("ContentLocalizationEnabled", portalID, false);
            if (language != null && contentLocalizationEnabled)
            {
                //Create new Translator Role
                AddTranslatorRole(portalID, language);
            }

            DataProvider.Instance().AddPortalLanguage(portalID, languageID, false, UserController.GetCurrentUserInfo().UserID);
            var objEventLog = new EventLogController();
            objEventLog.AddLog("portalID/languageID",
                               portalID + "/" + languageID,
                               PortalController.GetCurrentPortalSettings(),
                               UserController.GetCurrentUserInfo().UserID,
                               EventLogController.EventLogType.LANGUAGETOPORTAL_CREATED);

            //'check a valid portal record exists for that language
            //Dim pc As New DotNetNuke.Entities.Portals.PortalController
            //pc.GetPortal(portalID, language.Code)
            //Dim p As PortalInfo = pc.GetPortal(portalID, language.Code)
            //If p Is Nothing Then
            //    Dim fallback As PortalInfo = pc.GetPortal(portalID, language.Fallback)
            //    fallback.CultureCode = language.Code
            //End If


            if (clearCache)
            {
                DataCache.ClearPortalCache(portalID, false);
            }
        }

        public static void AddLanguagesToPortal(int portalID)
        {
            foreach (Locale language in LocaleController.Instance.GetLocales(Null.NullInteger).Values)
            {
                AddLanguageToPortal(portalID, language.LanguageId, false);
            }
            DataCache.RemoveCache(String.Format(DataCache.LocalesCacheKey, portalID));
        }

        public static void AddLanguageToPortals(int languageID)
        {
            var controller = new PortalController();
            foreach (PortalInfo portal in controller.GetPortals())
            {
                AddLanguageToPortal(portal.PortalID, languageID, false);
                DataCache.RemoveCache(String.Format(DataCache.LocalesCacheKey, portal.PortalID));
            }
        }

        public static void AddTranslatorRole(int portalID, Locale language)
        {
            //Create new Translator Role
            var roleController = new RoleController();
            string roleName = string.Format("Translator ({0})", language.Code);
            RoleInfo role = roleController.GetRoleByName(portalID, roleName);

            if (role == null)
            {
                role = new RoleInfo();
                role.RoleGroupID = Null.NullInteger;
                role.PortalID = portalID;
                role.RoleName = roleName;
                role.Description = string.Format("A role for {0} translators", language.EnglishName);
                roleController.AddRole(role);
            }

            string roles = string.Format("Administrators;{0}", string.Format("Translator ({0})", language.Code));

            PortalController.UpdatePortalSetting(portalID, string.Format("DefaultTranslatorRoles-{0}", language.Code), roles);
        }

        public static void DeleteLanguage(Locale language)
        {
            DataProvider.Instance().DeleteLanguage(language.LanguageId);
            var objEventLog = new EventLogController();
            objEventLog.AddLog(language, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.LANGUAGE_DELETED);
            DataCache.ClearHostCache(true);
        }

        public static CultureInfo GetBrowserCulture(int portalId)
        {
            CultureInfo browserCulture = null;
            Dictionary<string, Locale> enabledLocales = null;
            if (portalId > Null.NullInteger)
            {
                enabledLocales = LocaleController.Instance.GetLocales(portalId);
            }

            // use Request.UserLanguages to get the preferred language
            if ((HttpContext.Current != null))
            {
                if ((HttpContext.Current.Request.UserLanguages != null))
                {
                    try
                    {
                        foreach (string userLang in HttpContext.Current.Request.UserLanguages)
                        {
                            //split userlanguage by ";"... all but the first language will contain a preferrence index eg. ;q=.5
                            string userlanguage = userLang.Split(';')[0];
                            if (LocaleController.Instance.IsEnabled(ref userlanguage, portalId))
                            {
                                browserCulture = new CultureInfo(userlanguage);
                            }
                            else if (userLang.Split(';')[0].IndexOf("-") != -1)
                            {
                                //if userLang is neutral we don't need to do this part since
                                //it has already been done in LocaleIsEnabled( )
                                string templang = userLang.Split(';')[0];
                                foreach (string _localeCode in enabledLocales.Keys)
                                {
                                    if (_localeCode.Split('-')[0] == templang.Split('-')[0])
                                    {
                                        //the preferredLanguage was found in the enabled locales collection, so we are going to use this one
                                        //eg, requested locale is en-GB, requested language is en, enabled locale is en-US, so en is a match for en-US
                                        browserCulture = new CultureInfo(_localeCode);
                                        break;
                                    }
                                }
                            }
                            if ((browserCulture != null))
                            {
                                break;
                            }
                        }
                    }
					catch (Exception ex)
					{
						DnnLog.Error(ex);
					}
                }
            }

            return browserCulture;
        }


        public static string GetExceptionMessage(string key, string defaultValue)
        {
            if (HttpContext.Current == null)
            {
                return defaultValue;
            }
            else
            {
                return GetString(key, ExceptionsResourceFile);
            }
        }

        public static string GetExceptionMessage(string key, string defaultValue, params object[] @params)
        {
            if (HttpContext.Current == null)
            {
                return string.Format(defaultValue, @params);
            }
            else
            {
                return string.Format(GetString(key, ExceptionsResourceFile), @params);
            }
        }

        public static string GetLanguageDisplayMode(int PortalId)
        {
            string _ViewTypePersonalizationKey = "ViewType" + PortalId;
            string _ViewType = Convert.ToString(Personalization.Personalization.GetProfile("LanguageDisplayMode", _ViewTypePersonalizationKey));
            if (string.IsNullOrEmpty(_ViewType))
            {
                _ViewType = "NATIVE";
            }
            return _ViewType;
        }

        public static string GetResourceFileName(string resourceFileName, string language, string mode, int portalId)
        {
            if (!resourceFileName.EndsWith(".resx"))
            {
                resourceFileName += ".resx";
            }
            if (language != SystemLocale)
            {
                resourceFileName = resourceFileName.Substring(0, resourceFileName.Length - 5) + "." + language + ".resx";
            }
            if (mode == "Host")
            {
                resourceFileName = resourceFileName.Substring(0, resourceFileName.Length - 5) + "." + "Host.resx";
            }
            else if (mode == "Portal")
            {
                resourceFileName = resourceFileName.Substring(0, resourceFileName.Length - 5) + "." + "Portal-" + portalId + ".resx";
            }
            return resourceFileName;
        }

        public static string GetResourceFile(Control Ctrl, string FileName)
        {
            return Ctrl.TemplateSourceDirectory + "/" + LocalResourceDirectory + "/" + FileName;
        }

        public static CultureInfo GetPageLocale(PortalSettings portalSettings)
        {
            CultureInfo pageCulture = null;
            Dictionary<string, Locale> enabledLocales = null;
            if (portalSettings != null)
            {
                enabledLocales = LocaleController.Instance.GetLocales(portalSettings.PortalId);
            }
            string preferredLocale = "";
            string preferredLanguage = "";
            if (HttpContext.Current != null && portalSettings != null)
            {
                try
                {
                    if (portalSettings.ContentLocalizationEnabled && HttpContext.Current.Request.IsAuthenticated && portalSettings.UserMode == PortalSettings.Mode.Edit)
                    {
                        //Check Cookie only
                        preferredLocale = HttpContext.Current.Request.Cookies["language"].Value;
                    }
                    else
                    {
                        //Check Cookie or Qs
                        preferredLocale = HttpContext.Current.Request["language"];
                    }
                    if (!String.IsNullOrEmpty(preferredLocale))
                    {
                        if (LocaleController.Instance.IsEnabled(ref preferredLocale, portalSettings.PortalId))
                        {
                            pageCulture = new CultureInfo(preferredLocale);
                        }
                        else
                        {
                            preferredLanguage = preferredLocale.Split('-')[0];
                        }
                    }
                }
				catch (Exception ex)
				{
					DnnLog.Error(ex);
				}
            }
            if (pageCulture == null && portalSettings != null)
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                if (objUserInfo.UserID != -1)
                {
                    if (!String.IsNullOrEmpty(objUserInfo.Profile.PreferredLocale))
                    {
                        if (LocaleController.Instance.IsEnabled(ref preferredLocale, portalSettings.PortalId))
                        {
                            pageCulture = new CultureInfo(objUserInfo.Profile.PreferredLocale);
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(preferredLanguage))
                            {
                                preferredLanguage = objUserInfo.Profile.PreferredLocale.Split('-')[0];
                            }
                        }
                    }
                }
            }
            if (pageCulture == null && portalSettings != null && portalSettings.EnableBrowserLanguage)
            {
                pageCulture = GetBrowserCulture(portalSettings.PortalId);
            }
            if (pageCulture == null && !String.IsNullOrEmpty(preferredLanguage))
            {
                foreach (string _localeCode in enabledLocales.Keys)
                {
                    if (_localeCode.Split('-')[0] == preferredLanguage)
                    {
                        pageCulture = new CultureInfo(_localeCode);
                        break;
                    }
                }
            }
            if (pageCulture == null && portalSettings != null)
            {
                if (String.IsNullOrEmpty(portalSettings.DefaultLanguage))
                {
                    if (enabledLocales.Count > 0)
                    {
                        foreach (string _localeCode in enabledLocales.Keys)
                        {
                            pageCulture = new CultureInfo(_localeCode);
                            break;
                        }
                    }
                    else
                    {
                        pageCulture = new CultureInfo(SystemLocale);
                    }
                }
                else
                {
                    pageCulture = new CultureInfo(portalSettings.DefaultLanguage);
                }
            }
            if (pageCulture == null)
            {
                pageCulture = new CultureInfo(SystemLocale);
            }
            SetLanguage(pageCulture.Name);
            return pageCulture;
        }

        public static string GetString(string key, Control ctrl)
        {
            Control parentControl = ctrl.Parent;
            string localizedText;
            var moduleControl = parentControl as IModuleControl;
            if (moduleControl == null)
            {
                PropertyInfo pi = parentControl.GetType().GetProperty("LocalResourceFile");
                if (pi != null)
                {
                    localizedText = GetString(key, pi.GetValue(parentControl, null).ToString());
                }
                else
                {
                    localizedText = GetString(key, parentControl);
                }
            }
            else
            {
                localizedText = GetString(key, moduleControl.LocalResourceFile);
            }
            return localizedText;
        }

        public static string GetString(string name)
        {
            return GetString(name, null, PortalController.GetCurrentPortalSettings(), null, false);
        }

        public static string GetString(string name, PortalSettings objPortalSettings)
        {
            return GetString(name, null, objPortalSettings, null, false);
        }

        public static string GetString(string name, string ResourceFileRoot, bool disableShowMissingKeys)
        {
            return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), null, disableShowMissingKeys);
        }

		/// -----------------------------------------------------------------------------
		/// <overloads>One of six overloads</overloads>
		/// <summary>
		/// GetString gets the localized string corresponding to the resourcekey
		/// </summary>
		/// <param name="name">The resourcekey to find</param>
		/// <param name="ResourceFileRoot">The Resource File Name.</param>
		/// <returns>The localized Text</returns>
		/// <history>
		/// 	[cnurse]	10/06/2004	Documented
		/// </history>
		/// -----------------------------------------------------------------------------
        public static string GetString(string name, string ResourceFileRoot)
        {
            return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), null, false);
        }

        public static string GetString(string name, string ResourceFileRoot, string strlanguage)
        {
            return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), strlanguage, false);
        }

        public static string GetString(string name, string ResourceFileRoot, PortalSettings objPortalSettings, string strLanguage)
        {
            return GetString(name, ResourceFileRoot, objPortalSettings, strLanguage, false);
        }

        public static string GetString(string key, string resourceFileRoot, PortalSettings objPortalSettings, string userLanguage, bool disableShowMissingKeys)
        {
            return GetStringInternal(key, userLanguage, resourceFileRoot, objPortalSettings, disableShowMissingKeys);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetStringUrl gets the localized string corresponding to the resourcekey
        /// </summary>
        /// <param name="name">The resourcekey to find</param>
        /// <param name="ResourceFileRoot">The Local Resource root</param>
        /// <returns>The localized Text</returns>
        /// <remarks>
        /// This function should be used to retrieve strings to be used on URLs.
        /// It is the same as <see cref="GetString(string, string)">GetString(name,ResourceFileRoot)</see> method
        /// but it disables the ShowMissingKey flag, so even it testing scenarios, the correct string
        /// is returned
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	11/21/2006	Added
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetStringUrl(string name, string ResourceFileRoot)
        {
            return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), null, true);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a SystemMessage.
        /// </summary>
        /// <param name="objPortal">The portal settings for the portal to which the message will affect.</param>
        /// <param name="MessageName">The message tag which identifies the SystemMessage.</param>
        /// <returns>The message body with all tags replaced.</returns>
        /// <remarks>
        /// Supported tags:
        /// - All fields from HostSettings table in the form of: [Host:<b>field</b>]
        /// - All properties defined in <see cref="T:DotNetNuke.PortalInfo" /> in the form of: [Portal:<b>property</b>]
        /// - [Portal:URL]: The base URL for the portal
        /// - All properties defined in <see cref="T:DotNetNuke.UserInfo" /> in the form of: [User:<b>property</b>]
        /// - All values stored in the user profile in the form of: [Profile:<b>key</b>]
        /// - [User:VerificationCode]: User verification code for verified registrations
        /// - [Date:Current]: Current date
        /// </remarks>
        /// <history>
        /// 	[Vicenç]	05/07/2004	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetSystemMessage(PortalSettings objPortal, string MessageName)
        {
            return GetSystemMessage(null, objPortal, MessageName, null, GlobalResourceFile, null);
        }

        public static string GetSystemMessage(PortalSettings objPortal, string MessageName, UserInfo objUser)
        {
            return GetSystemMessage(null, objPortal, MessageName, objUser, GlobalResourceFile, null);
        }

        public static string GetSystemMessage(string strLanguage, PortalSettings objPortal, string MessageName, UserInfo objUser)
        {
            return GetSystemMessage(strLanguage, objPortal, MessageName, objUser, GlobalResourceFile, null);
        }

        public static string GetSystemMessage(PortalSettings objPortal, string MessageName, string ResourceFile)
        {
            return GetSystemMessage(null, objPortal, MessageName, null, ResourceFile, null);
        }

        public static string GetSystemMessage(PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile)
        {
            return GetSystemMessage(null, objPortal, MessageName, objUser, ResourceFile, null);
        }

        public static string GetSystemMessage(PortalSettings objPortal, string MessageName, string ResourceFile, ArrayList Custom)
        {
            return GetSystemMessage(null, objPortal, MessageName, null, ResourceFile, Custom);
        }

        public static string GetSystemMessage(PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile, ArrayList Custom)
        {
            return GetSystemMessage(null, objPortal, MessageName, objUser, ResourceFile, Custom);
        }

        public static string GetSystemMessage(string strLanguage, PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile, ArrayList Custom)
        {
            return GetSystemMessage(strLanguage, objPortal, MessageName, objUser, ResourceFile, Custom, null, "", -1);
        }

        public static string GetSystemMessage(string strLanguage, PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile, ArrayList Custom, string CustomCaption,
                                              int AccessingUserID)
        {
            return GetSystemMessage(strLanguage, objPortal, MessageName, objUser, ResourceFile, Custom, null, CustomCaption, AccessingUserID);
        }

        public static string GetSystemMessage(string strLanguage, PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile, ArrayList CustomArray,
                                              IDictionary CustomDictionary, string CustomCaption, int AccessingUserID)
        {
            string strMessageValue;
            strMessageValue = GetString(MessageName, ResourceFile, objPortal, strLanguage);
            if (!String.IsNullOrEmpty(strMessageValue))
            {
                if (String.IsNullOrEmpty(CustomCaption))
                {
                    CustomCaption = "Custom";
                }
                var objTokenReplace = new TokenReplace(Scope.SystemMessages, strLanguage, objPortal, objUser);
                if ((AccessingUserID != -1) && (objUser != null))
                {
                    if (objUser.UserID != AccessingUserID)
                    {
                        objTokenReplace.AccessingUser = new UserController().GetUser(objPortal.PortalId, AccessingUserID);
                    }
                }
                if (CustomArray != null)
                {
                    strMessageValue = objTokenReplace.ReplaceEnvironmentTokens(strMessageValue, CustomArray, CustomCaption);
                }
                else
                {
                    strMessageValue = objTokenReplace.ReplaceEnvironmentTokens(strMessageValue, CustomDictionary, CustomCaption);
                }
            }
            return strMessageValue;
        }

        [Obsolete("Deprecated in DNN 6.0. Replaced by SystemTimeZone and use of .NET TimeZoneInfo class")]
        public static NameValueCollection GetTimeZones(string language)
        {
            language = language.ToLower();
            string cacheKey = "dotnetnuke-" + language + "-timezones";
            string TranslationFile;
            if (language == SystemLocale.ToLower())
            {
                TranslationFile = TimezonesFile;
            }
            else
            {
                TranslationFile = TimezonesFile.Replace(".xml", "." + language + ".xml");
            }
            var timeZones = (NameValueCollection) DataCache.GetCache(cacheKey);
            if (timeZones == null)
            {
                string filePath = HttpContext.Current.Server.MapPath(TranslationFile);
                timeZones = new NameValueCollection();
                if (File.Exists(filePath) == false)
                {
                    return timeZones;
                }
                var dp = new DNNCacheDependency(filePath);
                try
                {
                    var d = new XmlDocument();
                    d.Load(filePath);
                    foreach (XmlNode n in d.SelectSingleNode("root").ChildNodes)
                    {
                        if (n.NodeType != XmlNodeType.Comment)
                        {
                            timeZones.Add(n.Attributes["name"].Value, n.Attributes["key"].Value);
                        }
                    }
                }
                catch (Exception exc)
                {
                    Instrumentation.DnnLog.Error(exc);

                }
                if (Host.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
                {
                    DataCache.SetCache(cacheKey, timeZones, dp);
                }
            }
            return timeZones;
        }

        /// <summary>
        ///   <para>LoadCultureDropDownList loads a DropDownList with the list of supported cultures
        ///     based on the languages defined in the supported locales file, for the current portal</para>
        /// </summary>
        /// <param name = "list">DropDownList to load</param>
        /// <param name = "displayType">Format of the culture to display. Must be one the CultureDropDownTypes values.
        ///   <see cref = "CultureDropDownTypes" /> for list of allowable values</param>
        /// <param name = "selectedValue">Name of the default culture to select</param>
        public static void LoadCultureDropDownList(DropDownList list, CultureDropDownTypes displayType, string selectedValue)
        {
            LoadCultureDropDownList(list, displayType, selectedValue, "", false);
        }

        /// <summary>
        ///   <para>LoadCultureDropDownList loads a DropDownList with the list of supported cultures
        ///     based on the languages defined in the supported locales file. </para>
        ///   <para>This overload allows us to display all installed languages. To do so, pass the value True to the Host parameter</para>
        /// </summary>
        /// <param name = "list">DropDownList to load</param>
        /// <param name = "displayType">Format of the culture to display. Must be one the CultureDropDownTypes values.
        ///   <see cref = "CultureDropDownTypes" /> for list of allowable values</param>
        /// <param name = "selectedValue">Name of the default culture to select</param>
        /// <param name = "Host">Boolean that defines wether or not to load host (ie. all available) locales</param>
        public static void LoadCultureDropDownList(DropDownList list, CultureDropDownTypes displayType, string selectedValue, bool Host)
        {
            LoadCultureDropDownList(list, displayType, selectedValue, "", Host);
        }

        public static string GetLocaleName(string code, CultureDropDownTypes displayType)
        {
            string name = null;

            // Create a CultureInfo class based on culture
            CultureInfo info = CultureInfo.CreateSpecificCulture(code);

            // Based on the display type desired by the user, select the correct property
            switch (displayType)
            {
                case CultureDropDownTypes.EnglishName:
                    name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.EnglishName);
                    break;
                case CultureDropDownTypes.Lcid:
                    name = info.LCID.ToString();
                    break;
                case CultureDropDownTypes.Name:
                    name = info.Name;
                    break;
                case CultureDropDownTypes.NativeName:
                    name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.NativeName);
                    break;
                case CultureDropDownTypes.TwoLetterIsoCode:
                    name = info.TwoLetterISOLanguageName;
                    break;
                case CultureDropDownTypes.ThreeLetterIsoCode:
                    name = info.ThreeLetterISOLanguageName;
                    break;
                default:
                    name = info.DisplayName;
                    break;
            }

            return name;
        }


        /// <summary>
        ///   <para>LoadCultureDropDownList loads a DropDownList with the list of supported cultures
        ///     based on the languages defined in the supported locales file</para>
        ///   <para>This overload allows us to filter a language from the dropdown. To do so pass a language code to the Filter parameter</para>
        ///   <para>This overload allows us to display all installed languages. To do so, pass the value True to the Host parameter</para>
        /// </summary>
        /// <param name = "list">DropDownList to load</param>
        /// <param name = "displayType">Format of the culture to display. Must be one the CultureDropDownTypes values.
        ///   <see cref = "CultureDropDownTypes" /> for list of allowable values</param>
        /// <param name = "selectedValue">Name of the default culture to select</param>
        /// <param name = "Filter">Stringvalue that allows for filtering out a specifiec language</param>
        /// <param name = "Host">Boolean that defines wether or not to load host (ie. all available) locales</param>
        public static void LoadCultureDropDownList(DropDownList list, CultureDropDownTypes displayType, string selectedValue, string Filter, bool Host)
        {
            PortalSettings objPortalSettings = PortalController.GetCurrentPortalSettings();
            Dictionary<string, Locale> enabledLanguages;
            if (Host)
            {
                enabledLanguages = LocaleController.Instance.GetLocales(Null.NullInteger);
            }
            else
            {
                enabledLanguages = LocaleController.Instance.GetLocales(objPortalSettings.PortalId);
            }
            var _cultureListItems = new ListItem[enabledLanguages.Count];
            CultureDropDownTypes _cultureListItemsType = displayType;
            int intAdded = 0;
            foreach (KeyValuePair<string, Locale> kvp in enabledLanguages)
            {
                if (kvp.Value.Code != Filter)
                {
                    var item = new ListItem();
                    item.Value = kvp.Value.Code;
                    item.Text = GetLocaleName(item.Value, displayType);
                    _cultureListItems[intAdded] = item;
                    intAdded += 1;
                }
            }
            if (list.Items.Count > 0)
            {
                list.Items.Clear();
            }
            Array.Resize(ref _cultureListItems, intAdded);
            list.Items.AddRange(_cultureListItems);
            if (selectedValue != null)
            {
                ListItem item = list.Items.FindByValue(selectedValue);
                if (item != null)
                {
                    list.SelectedIndex = -1;
                    item.Selected = true;
                }
            }
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Localizes ModuleControl Titles
        /// </summary>
        /// <param name="moduleControl">ModuleControl</param>
        /// <returns>
        /// Localized control title if found
        /// </returns>
        /// <remarks>
        /// Resource keys are: ControlTitle_[key].Text
        /// Key MUST be lowercase in the resource file
        /// </remarks>
        /// <history>
        /// 	[vmasanas]	08/11/2004	Created
        ///     [cnurse]    11/28/2008  Modified Signature
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string LocalizeControlTitle(IModuleControl moduleControl)
        {
            string controlTitle = moduleControl.ModuleContext.Configuration.ModuleTitle;
            string controlKey = moduleControl.ModuleContext.Configuration.ModuleControl.ControlKey.ToLower();

            if (!string.IsNullOrEmpty(controlTitle) && !string.IsNullOrEmpty(controlKey))
            {
                controlTitle = moduleControl.ModuleContext.Configuration.ModuleControl.ControlTitle;
            }

            if (!string.IsNullOrEmpty(controlKey))
            {
                string reskey;
                reskey = "ControlTitle_" + moduleControl.ModuleContext.Configuration.ModuleControl.ControlKey.ToLower() + ".Text";
                string localizedvalue = GetString(reskey, moduleControl.LocalResourceFile);
                if (string.IsNullOrEmpty(localizedvalue))
                {
                    controlTitle = moduleControl.ModuleContext.Configuration.ModuleControl.ControlTitle;
                }
                else
                {
                    controlTitle = localizedvalue;
                }
            }
            return controlTitle;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LocalizeDataGrid creates localized Headers for a DataGrid
        /// </summary>
        /// <param name="grid">Grid to localize</param>
        /// <param name="ResourceFile">The root name of the Resource File where the localized
        ///   text can be found</param>
        /// <history>
        /// 	[cnurse]	9/10/2004	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void LocalizeDataGrid(ref DataGrid grid, string ResourceFile)
        {
            string localizedText;
            foreach (DataGridColumn col in grid.Columns)
            {
                if (!string.IsNullOrEmpty(col.HeaderText))
                {
                    localizedText = GetString(col.HeaderText + ".Header", ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                    {
                        col.HeaderText = localizedText;
                    }
                }
                if (col is EditCommandColumn)
                {
                    var editCol = (EditCommandColumn) col;
                    localizedText = GetString(editCol.EditText + ".EditText", ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                    {
                        editCol.EditText = localizedText;
                    }
                    localizedText = GetString(editCol.EditText, ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                    {
                        editCol.EditText = localizedText;
                    }
                    localizedText = GetString(editCol.CancelText, ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                    {
                        editCol.CancelText = localizedText;
                    }
                    localizedText = GetString(editCol.UpdateText, ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                    {
                        editCol.UpdateText = localizedText;
                    }
                }
                else if (col is ButtonColumn)
                {
                    var buttonCol = (ButtonColumn) col;
                    localizedText = GetString(buttonCol.Text, ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                    {
                        buttonCol.Text = localizedText;
                    }
                }
            }
        }

        /// <summary>
        /// Localizes headers and fields on a DetailsView control
        /// </summary>
        /// <param name="detailsView"></param>
        /// <param name="resourceFile">The root name of the resource file where the localized
        ///  texts can be found</param>
        /// <remarks></remarks>
        public static void LocalizeDetailsView(ref DetailsView detailsView, string resourceFile)
        {
            foreach (DataControlField field in detailsView.Fields)
            {
                LocalizeDataControlField(field, resourceFile);
            }
        }

        /// <summary>
        /// Localizes headers and fields on a GridView control
        /// </summary>
        /// <param name="gridView">Grid to localize</param>
        /// <param name="resourceFile">The root name of the resource file where the localized
        ///  texts can be found</param>
        /// <remarks></remarks>
        public static void LocalizeGridView(ref GridView gridView, string resourceFile)
        {
            foreach (DataControlField column in gridView.Columns)
            {
                LocalizeDataControlField(column, resourceFile);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Localizes the "Built In" Roles
        /// </summary>
        /// <remarks>
        /// Localizes:
        /// -DesktopTabs
        /// -BreadCrumbs
        /// </remarks>
        /// <history>
        /// 	[cnurse]	02/01/2005	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string LocalizeRole(string role)
        {
            string localRole;
            switch (role)
            {
                case Globals.glbRoleAllUsersName:
                case Globals.glbRoleSuperUserName:
                case Globals.glbRoleUnauthUserName:
                    string roleKey = role.Replace(" ", "");
                    localRole = GetString(roleKey);
                    break;
                default:
                    localRole = role;
                    break;
            }
            return localRole;
        }

        public static void RemoveLanguageFromPortal(int portalID, int languageID)
        {
            //Remove Translator Role from Portal
            Locale language = LocaleController.Instance.GetLocale(languageID);
            if (language != null)
            {
                //Get Translator Role
                var roleController = new RoleController();
                string roleName = string.Format("Translator ({0})", language.Code);
                RoleInfo role = roleController.GetRoleByName(portalID, roleName);

                if (role != null)
                {
                    roleController.DeleteRole(role.RoleID, portalID);
                }
            }

            DataProvider.Instance().DeletePortalLanguages(portalID, languageID);
            var objEventLog = new EventLogController();
            objEventLog.AddLog("portalID/languageID",
                               portalID + "/" + languageID,
                               PortalController.GetCurrentPortalSettings(),
                               UserController.GetCurrentUserInfo().UserID,
                               EventLogController.EventLogType.LANGUAGETOPORTAL_DELETED);

            DataCache.RemoveCache(string.Format(DataCache.LocalesCacheKey, portalID));
        }

        public static void RemoveLanguageFromPortals(int languageID)
        {
            var controller = new PortalController();
            foreach (PortalInfo portal in controller.GetPortals())
            {
                RemoveLanguageFromPortal(portal.PortalID, languageID);
            }
        }

        public static void RemoveLanguagesFromPortal(int portalID)
        {
            foreach (Locale locale in LocaleController.Instance.GetLocales(portalID).Values)
            {
                RemoveLanguageFromPortal(portalID, locale.LanguageId);
            }
        }

        public static void SaveLanguage(Locale locale)
        {
            var objEventLog = new EventLogController();
            if (locale.LanguageId == Null.NullInteger)
            {
                locale.LanguageId = DataProvider.Instance().AddLanguage(locale.Code, locale.Text, locale.Fallback, UserController.GetCurrentUserInfo().UserID);
                objEventLog.AddLog(locale, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.LANGUAGE_CREATED);
            }
            else
            {
                DataProvider.Instance().UpdateLanguage(locale.LanguageId, locale.Code, locale.Text, locale.Fallback, UserController.GetCurrentUserInfo().UserID);
                objEventLog.AddLog(locale, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.LANGUAGE_UPDATED);
            }
            DataCache.ClearHostCache(true);
        }

        public static void SetLanguage(string value)
        {
            try
            {
                HttpResponse Response = HttpContext.Current.Response;
                if (Response == null)
                {
                    return;
                }
                HttpCookie cookie = null;
                cookie = Response.Cookies.Get("language");
                if ((cookie == null))
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        cookie = new HttpCookie("language", value);
                        Response.Cookies.Add(cookie);
                    }
                }
                else
                {
                    cookie.Value = value;
                    if (!String.IsNullOrEmpty(value))
                    {
                        Response.Cookies.Set(cookie);
                    }
                    else
                    {
                        Response.Cookies.Remove("language");
                    }
                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        ///   Sets the culture codes on the current Thread
        /// </summary>
        /// <param name = "cultureInfo">Culture Info for the current page</param>
        /// <param name = "portalSettings">The current portalSettings</param>
        /// <remarks>
        ///   This method will configure the Thread culture codes.  Any page which does not derive from PageBase should
        ///   be sure to call this method in OnInit to ensure localiztion works correctly.  See the TelerikDialogHandler for an example.
        /// </remarks>
        public static void SetThreadCultures(CultureInfo cultureInfo, PortalSettings portalSettings)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException("cultureInfo");
            }

            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            if (portalSettings != null && portalSettings.ContentLocalizationEnabled && 
                        HttpContext.Current.Request.IsAuthenticated && 
                        portalSettings.UserMode == PortalSettings.Mode.Edit)
            {
                Locale locale = LocaleController.Instance.GetCurrentLocale(portalSettings.PortalId);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(locale.Code);
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = cultureInfo;
            }
        }

        [Obsolete("Deprecated in DNN 5.0. Replaced by GetLocales().")]
        public static LocaleCollection GetEnabledLocales()
        {
            PortalSettings objPortalSettings = PortalController.GetCurrentPortalSettings();
            var enabledLocales = new LocaleCollection();
            foreach (KeyValuePair<string, Locale> kvp in LocaleController.Instance.GetLocales(objPortalSettings.PortalId))
            {
                enabledLocales.Add(kvp.Key, kvp.Value);
            }
            return enabledLocales;
        }

        [Obsolete("Deprecated in DNN 5.0. Replaced by GetLocales().")]
        public static LocaleCollection GetSupportedLocales()
        {
            var supportedLocales = new LocaleCollection();
            foreach (KeyValuePair<string, Locale> kvp in LocaleController.Instance.GetLocales(Null.NullInteger))
            {
                supportedLocales.Add(kvp.Key, kvp.Value);
            }
            return supportedLocales;
        }

        [Obsolete("Deprecated in DNN 5.5. Replaced by LocaleController.GetLocale(code).")]
        public static Locale GetLocale(string code)
        {
            return GetLocale(Null.NullInteger, code);
        }

        [Obsolete("Deprecated in DNN 5.5. Replaced by LocaleController.GetLocale(portalID, code).")]
        public static Locale GetLocale(int portalID, string code)
        {
            Dictionary<string, Locale> dicLocales = LocaleController.Instance.GetLocales(portalID);
            Locale language = null;

            if (dicLocales != null)
            {
                dicLocales.TryGetValue(code, out language);
            }

            return language;
        }

        [Obsolete("Deprecated in DNN 5.5. Replaced by LocaleController.GetLocale(languageID).")]
        public static Locale GetLocaleByID(int languageID)
        {
            Dictionary<string, Locale> dicLocales = LocaleController.Instance.GetLocales(Null.NullInteger);
            Locale language = null;

            foreach (KeyValuePair<string, Locale> kvp in dicLocales)
            {
                if (kvp.Value.LanguageId == languageID)
                {
                    language = kvp.Value;
                    break;
                }
            }

            return language;
        }

        [Obsolete("Deprecated in DNN 5.5. Replaced by LocaleController.GetLocales(portalID).")]
        public static Dictionary<string, Locale> GetLocales(int portalID)
        {
            return LocaleController.Instance.GetLocales(portalID);
        }

        [Obsolete("Deprecated in DNN 5.5.  Replcaed by LocaleController.IsEnabled()")]
        public static bool LocaleIsEnabled(Locale locale)
        {
            return LocaleIsEnabled(locale.Code);
        }

        [Obsolete("Deprecated in DNN 5.5.  Replcaed by LocaleController.IsEnabled()")]
        public static bool LocaleIsEnabled(string localeCode)
        {
            PortalSettings _Settings = PortalController.GetCurrentPortalSettings();

            return LocaleController.Instance.IsEnabled(ref localeCode, _Settings.PortalId);
        }


        [Obsolete("Deprecated in DNN 5.0. Replaced by LocalizeControlTitle(IModuleControl).")]
        public static string LocalizeControlTitle(string ControlTitle, string ControlSrc, string Key)
        {
            string reskey;
            reskey = "ControlTitle_" + Key.ToLower() + ".Text";
            string ResFile = ControlSrc.Substring(0, ControlSrc.LastIndexOf("/") + 1) + LocalResourceDirectory +
                             ControlSrc.Substring(ControlSrc.LastIndexOf("/"), ControlSrc.LastIndexOf(".") - ControlSrc.LastIndexOf("/"));
            if (ResFile.StartsWith("DesktopModules"))
            {
                ResFile = "~/" + ResFile;
            }
            string localizedvalue = GetString(reskey, ResFile);
            if (localizedvalue != null)
            {
                return localizedvalue;
            }
            else
            {
                return ControlTitle;
            }
        }

        [Obsolete("Deprecated in DNN 5.0. This does nothing now as the Admin Tabs are treated like any other tab.")]
        public static void LocalizePortalSettings()
        {
        }

        [Obsolete("Deprecated in DNN 5.0. Replaced by Host.EnableBrowserLanguage OR PortalSettings.EnableBrowserLanguage")]
        public static bool UseBrowserLanguage()
        {
            return PortalController.GetCurrentPortalSettings().EnableBrowserLanguage;
        }

        [Obsolete("Deprecated in DNN 5.0. Replaced by Host.EnableUrlLanguage OR PortalSettings.EnableUrlLanguage")]
        public static bool UseLanguageInUrl()
        {
            return PortalController.GetCurrentPortalSettings().EnableUrlLanguage;
        }

        [Obsolete("Deprecated in DNN 6.0. Replaced by new DnnTimeZoneComboBox control and use of .NET TimeZoneInfo class")]
        public static void LoadTimeZoneDropDownList(DropDownList list, string language, string selectedValue)
        {
            NameValueCollection timeZones = GetTimeZones(language);
            if (timeZones.Count == 0)
            {
                timeZones = GetTimeZones(SystemLocale.ToLower());
            }
            int i;
            for (i = 0; i <= timeZones.Keys.Count - 1; i++)
            {
                list.Items.Add(new ListItem(timeZones.GetKey(i), timeZones.Get(i)));
            }
            if (selectedValue != null)
            {
                ListItem item = list.Items.FindByValue(selectedValue);
                if (item == null)
                {
                    item = list.Items.FindByValue(SystemTimeZoneOffset.ToString());
                }
                if (item != null)
                {
                    list.SelectedIndex = -1;
                    item.Selected = true;
                }
            }
        }

        /// <summary>
        /// Converts old TimeZoneOffset to new TimeZoneInfo. 
        /// </summary>
        /// <param name="timeZoneOffsetInMinutes">An offset in minutes, e.g. -480 (-8 times 60) for Pasicif Time Zone</param>        
        /// <returns>TimeZoneInfo is returned if timeZoneOffsetInMinutes is valid, otherwise TimeZoneInfo.Local is returned.</returns>
        /// <remarks>Initial mapping is based on hard-coded rules. These rules are hard-coded from old standard TimeZones.xml data.
        /// When offset is not found hard-coded mapping, a lookup is performed in timezones defined in the system. The first found entry is returned.
        /// When mapping is not found, a default TimeZoneInfo.Local us returned.</remarks>
        public static TimeZoneInfo ConvertLegacyTimeZoneOffsetToTimeZoneInfo(int timeZoneOffsetInMinutes)
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;

            //lookup existing mapping
            switch (timeZoneOffsetInMinutes)
            {
                case -720:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Dateline Standard Time");
                    break;
                case -660:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Samoa Standard Time");
                    break;
                case -600:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time");
                    break;
                case -540:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time");
                    break;
                case -480:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                    break;
                case -420:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
                    break;
                case -360:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                    break;
                case -300:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    break;
                case -240:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time");
                    break;
                case -210:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Newfoundland Standard Time");
                    break;
                case -180:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
                    break;
                case -120:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Mid-Atlantic Standard Time");
                    break;
                case -60:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Cape Verde Standard Time");
                    break;
                case 0:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                    break;
                case 60:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
                    break;
                case 120:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");
                    break;
                case 180:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
                    break;
                case 210:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
                    break;
                case 240:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
                    break;
                case 270:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Afghanistan Standard Time");
                    break;
                case 300:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    break;
                case 330:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    break;
                case 345:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
                    break;
                case 360:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time");
                    break;
                case 390:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Myanmar Standard Time");
                    break;
                case 420:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    break;
                case 480:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                    break;
                case 540:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                    break;
                case 570:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Cen. Australia Standard Time");
                    break;
                case 600:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
                    break;
                case 660:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Magadan Standard Time");
                    break;
                case 720:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
                    break;
                case 780:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Tonga Standard Time");
                    break;
                default:
                    foreach (TimeZoneInfo timeZone in TimeZoneInfo.GetSystemTimeZones())
                    {
                        if (timeZone.BaseUtcOffset.TotalMinutes == timeZoneOffsetInMinutes)
                        {
                            timeZoneInfo = timeZone;
                            break;
                        }
                    }
                    break;
            }

            return timeZoneInfo;
        }

        #region Nested type: CustomizedLocale

        private enum CustomizedLocale
        {
            None = 0,
            Portal = 1,
            Host = 2
        }

        #endregion
    }
}
