#region Copyright

// 
// DotNetNuke� - http://www.dotnetnuke.com
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
using System.Globalization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Authentication;

#endregion

namespace DotNetNuke.Authentication.Facebook.Components
{
    /// <summary>
    /// The Config class provides a central area for management of Module Configuration Settings.
    /// </summary>
    [Serializable]
    public class FacebookConfig : AuthenticationConfigBase
    {
        private const string CACHEKEY = "Authentication.Facebook";

        protected FacebookConfig(int portalID) : base(portalID)
        {
            APIKey = PortalController.GetPortalSetting("Facebook_APIKey", portalID, "");
            APISecret = PortalController.GetPortalSetting("Facebook_APISecret", portalID, "");
            Enabled = PortalController.GetPortalSettingAsBoolean("Facebook_Enabled", portalID, true);
            SiteURL = PortalController.GetPortalSetting("Facebook_SiteURL", portalID, "");
        }

        public string APIKey { get; set; }

        public string APISecret { get; set; }

        public bool Enabled { get; set; }

        public string SiteURL { get; set; }

        public static void ClearConfig(int portalId)
        {
            string key = CACHEKEY + "_" + portalId;
            DataCache.RemoveCache(key);
        }

        public static FacebookConfig GetConfig(int portalId)
        {
            string key = CACHEKEY + "_" + portalId;
            var config = (FacebookConfig)DataCache.GetCache(key);
            if (config == null)
            {
                config = new FacebookConfig(portalId);
                DataCache.SetCache(key, config);
            }
            return config;
        }

        public static void UpdateConfig(FacebookConfig config)
        {
            PortalController.UpdatePortalSetting(config.PortalID, "Facebook_APIKey", config.APIKey);
            PortalController.UpdatePortalSetting(config.PortalID, "Facebook_APISecret", config.APISecret);
            PortalController.UpdatePortalSetting(config.PortalID, "Facebook_Enabled", config.Enabled.ToString(CultureInfo.InvariantCulture));
            PortalController.UpdatePortalSetting(config.PortalID, "Facebook_SiteURL", config.SiteURL);
            ClearConfig(config.PortalID);
        }
    }
}