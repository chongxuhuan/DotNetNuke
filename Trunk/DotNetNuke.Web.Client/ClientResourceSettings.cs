#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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

namespace DotNetNuke.Web.Client
{
    using System.Collections.Generic;
    using System.Web;

    public class ClientResourceSettings
    {
        // public keys used to identify the dictionaries stored in the application context
        public static readonly string HostSettingsDictionaryKey = "HostSettingsDictionary";
        public static readonly string PortalSettingsDictionaryKey = "PortalSettingsDictionary";

        // public keys used to identify the various host and portal level settings
        public static readonly string EnableCompositeFilesKey = "CrmEnableCompositeFiles";
        public static readonly string MinifyCssKey = "CrmMinifyCss";
        public static readonly string MinifyJsKey = "CrmMinifyJs";
        public static readonly string OverrideDefaultSettingsKey = "CrmUseApplicationSettings";
        public static readonly string VersionKey = "CrmVersion";

        public int? GetVersion()
        {
            var portalVersion = GetIntegerSetting(PortalSettingsDictionaryKey, VersionKey);
            var overrideDefaultSettings = GetBooleanSetting(PortalSettingsDictionaryKey, OverrideDefaultSettingsKey);

            // if portal version is set
            // and the portal "override default settings" flag is set and set to true
            if (portalVersion.HasValue && overrideDefaultSettings.HasValue && overrideDefaultSettings.Value)
                return portalVersion.Value;

            // otherwise return the host setting
            var hostVersion = GetIntegerSetting(HostSettingsDictionaryKey, VersionKey);
            if (hostVersion.HasValue)
                return hostVersion.Value;

            // otherwise tell the calling method that nothing is set
            return null;
        }

        public bool? AreCompositeFilesEnabled()
        {
            var portalEnabled = GetBooleanSetting(PortalSettingsDictionaryKey, EnableCompositeFilesKey);
            var overrideDefaultSettings = GetBooleanSetting(PortalSettingsDictionaryKey, OverrideDefaultSettingsKey);

            // if portal version is set
            // and the portal "override default settings" flag is set and set to true
            if (portalEnabled.HasValue && overrideDefaultSettings.HasValue && overrideDefaultSettings.Value)
                return portalEnabled.Value;

            // otherwise return the host setting
            var hostEnabled = GetBooleanSetting(HostSettingsDictionaryKey, EnableCompositeFilesKey);
            if (hostEnabled.HasValue)
                return hostEnabled.Value;

            // otherwise tell the calling method that nothing is set
            return null;
        }

        private static bool? GetBooleanSetting(string dictionaryKey, string settingKey)
        {
            var settings = HttpContext.Current.Items[dictionaryKey];
            if (settings == null)
            {
                // settings not available
                return null;
            }

            var dictionary = (Dictionary<string, string>) settings;
            if (dictionary.ContainsKey(settingKey))
            {
                var setting = dictionary[settingKey];

                bool result;
                if (setting != null && bool.TryParse(setting, out result))
                {
                    // a valid setting was found
                    return result;
                }
            }

            // no valid setting was found
            return null;
        }

        private static int? GetIntegerSetting(string dictionaryKey, string settingKey)
        {
            var settings = HttpContext.Current.Items[dictionaryKey];
            if (settings == null)
            {
                // settings not available
                return null;
            }

            var dictionary = (Dictionary<string, string>) settings;
            if (dictionary.ContainsKey(settingKey))
            {
                var setting = dictionary[settingKey];
                int version;
                if (setting != null && int.TryParse(setting, out version))
                {
                    if (version > -1)
                    {
                        // a valid setting was found
                        return version;
                    }
                }
            }

            // no valid setting was found
            return null;
        }
    }
}