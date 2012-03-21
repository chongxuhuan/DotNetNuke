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

using System;
using System.Globalization;

using DotNetNuke.Entities.Portals;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Services.Authentication.OAuth
{
    /// <summary>
    /// The Config class provides a central area for management of Module Configuration Settings.
    /// </summary>
    [Serializable]
    public class oAuthConfigBase : AuthenticationConfigBase
    {
        private static readonly string _cacheKey = "Authentication";

        protected oAuthConfigBase(string service, int portalId)
            : base(portalId)
        {
            Service = service;

            APIKey = PortalController.GetPortalSetting(Service + "_APIKey", portalId, "");
            APISecret = PortalController.GetPortalSetting(Service + "_APISecret", portalId, "");
            Enabled = PortalController.GetPortalSettingAsBoolean(Service + "_Enabled", portalId, false);
            SiteURL = PortalController.GetPortalSetting(Service + "_SiteURL", portalId, "");
        }

        protected string Service { get; set; }

        public string APIKey { get; set; }

        public string APISecret { get; set; }

        public bool Enabled { get; set; }

        public string SiteURL { get; set; }

        private static string GetCacheKey(string service, int portalId)
        {
            return _cacheKey + "." + service + "_" + portalId;
        }

        public static void ClearConfig(string service, int portalId)
        {
            DataCache.RemoveCache(GetCacheKey(service, portalId));
        }

        public static oAuthConfigBase GetConfig(string service, int portalId)
        {
            string key = GetCacheKey(service, portalId);
            var config = (oAuthConfigBase)DataCache.GetCache(key);
            if (config == null)
            {
                config = new oAuthConfigBase(service, portalId);
                DataCache.SetCache(key, config);
            }
            return config;
        }

        public static void UpdateConfig(oAuthConfigBase config)
        {
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_APIKey", config.APIKey);
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_APISecret", config.APISecret);
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_Enabled", config.Enabled.ToString(CultureInfo.InvariantCulture));
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_SiteURL", config.SiteURL);
            ClearConfig(config.Service, config.PortalID);
        }
    }
}
