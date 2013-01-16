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
    using System;
    using System.Collections.Generic;
    using System.Reflection;
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
            return IsBooleanSettingEnabled(EnableCompositeFilesKey);
        }

        public bool? EnableCssMinification()
        {
            return IsBooleanSettingEnabled(MinifyCssKey);
        }

        public bool? EnableJsMinification()
        {
            return IsBooleanSettingEnabled(MinifyJsKey);
        }

        private bool? IsBooleanSettingEnabled(string settingKey)
        {
            if(Status != UpgradeStatus.None)
            {
                return false;
            }
            
            var portalEnabled = GetBooleanSetting(PortalSettingsDictionaryKey, settingKey);
            var overrideDefaultSettings = GetBooleanSetting(PortalSettingsDictionaryKey, OverrideDefaultSettingsKey);

            // if portal version is set
            // and the portal "override default settings" flag is set and set to true
            if (portalEnabled.HasValue && overrideDefaultSettings.HasValue && overrideDefaultSettings.Value)
                return portalEnabled.Value;

            // otherwise return the host setting
            var hostEnabled = GetBooleanSetting(HostSettingsDictionaryKey, settingKey);
            if (hostEnabled.HasValue)
                return hostEnabled.Value;

            // otherwise tell the calling method that nothing is set
            return null;
        }

        private static bool? GetBooleanSetting(string dictionaryKey, string settingKey)
        {
            var setting = GetSetting(dictionaryKey, settingKey);
            bool result;
            if (setting != null && bool.TryParse(setting, out result))
            {
                return result;
            }
            return null;
        }

        private static int? GetIntegerSetting(string dictionaryKey, string settingKey)
        {
            var setting = GetSetting(dictionaryKey, settingKey);
            int version;
            if (setting != null && int.TryParse(setting, out version))
            {
                if (version > -1)
                {
                    return version;
                }
            }
            return null;
        }

        private static string GetSetting(string dictionaryKey, string settingKey)
        {
            var settings = HttpContext.Current.Items[dictionaryKey];
            if (settings == null)
            {
                if (dictionaryKey == HostSettingsDictionaryKey)
                    return GetHostSettingThroughReflection(settingKey);

                return GetPortalSettingThroughReflection(settingKey);
            }

            var dictionary = (Dictionary<string, string>)settings;
            if (dictionary.ContainsKey(settingKey))
            {
                return dictionary[settingKey];
            }

            // no valid setting was found
            return null;
        }

        private static string GetPortalSettingThroughReflection(string settingKey)
        {
            try
            {
                int? portalId = GetPortalIdThroughReflection();
                if (!portalId.HasValue)
                    return null;
                
                var type = Type.GetType("DotNetNuke.Entities.Portals.PortalController, DotNetNuke");
                MethodInfo method = type.GetMethod("GetPortalSettingsDictionary");
                var settings = method.Invoke(null, new object[] { portalId.Value });

                var dictionary = (Dictionary<string, string>)settings;
                if (dictionary.ContainsKey(settingKey))
                {
                    return dictionary[settingKey];
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static int? GetPortalIdThroughReflection()
        {
            try
            {
                Type type = Type.GetType("DotNetNuke.Entities.Portals.PortalAliasController, DotNetNuke");
                MethodInfo method = type.GetMethod("GetPortalAliasInfo");
                object portalAliasInfo = method.Invoke(null, new object[] { HttpContext.Current.Request.Url.Host });
                object portalId = portalAliasInfo.GetType().GetProperty("PortalID").GetValue(portalAliasInfo, new object[] {});
                return (int) portalId;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetHostSettingThroughReflection(string settingKey)
        {
            try
            {
                var type = Type.GetType("DotNetNuke.Entities.Controllers.HostController, DotNetNuke");
                
                MethodInfo method = type.GetMethod("GetSettingsDictionary");
                PropertyInfo property = type.BaseType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
                object instance = property.GetValue(null, Type.EmptyTypes);
                var settings = method.Invoke(instance, Type.EmptyTypes);

                var dictionary = (Dictionary<string, string>)settings;
                if (dictionary.ContainsKey(settingKey))
                {
                    return dictionary[settingKey];
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool _statusChecked;
        private UpgradeStatus _status;

        private UpgradeStatus Status
        {
            get
            {
                if (!_statusChecked)
                {
                    _status = GetStatusByReflection();
                    _statusChecked = true;
                }

                return _status;
            }
        }

        private UpgradeStatus GetStatusByReflection()
        {
            try
            {
                var type = Type.GetType("DotNetNuke.Common.Globals, DotNetNuke");

                PropertyInfo property = type.GetProperty("Status", BindingFlags.Static | BindingFlags.Public);
                var status = (UpgradeStatus)property.GetValue(null, null);
                return status;
            }
            catch (Exception)
            {
                return UpgradeStatus.Unknown;
            }
        }

        private enum UpgradeStatus
        {
            /// <summary>
            /// The application need update to a higher version.
            /// </summary>
            Upgrade,
            /// <summary>
            /// The application need to install itself.
            /// </summary>
            Install,
            /// <summary>
            /// The application is normal running.
            /// </summary>
            None,
            /// <summary>
            /// The application occur error when running.
            /// </summary>
            Error,
            /// <summary>
            /// The application status is unknown,
            /// </summary>
            /// <remarks>This status should never be returned. its is only used as a flag that Status hasn't been determined.</remarks>
            Unknown
        }
    }
}