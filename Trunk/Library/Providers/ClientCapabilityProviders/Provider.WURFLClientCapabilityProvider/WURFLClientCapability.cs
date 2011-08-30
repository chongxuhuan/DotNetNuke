using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotNetNuke.Services.ClientCapability;

using WURFL;

namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// WURFL Implementation of IClientCapability
    /// </summary>
    public class WURFLClientCapability : ClientCapability
    {

        /// <summary>
        /// WURFLClientCapability constructor
        /// </summary>
        public WURFLClientCapability(IDevice device)            
        {
            ID = device.Id;
            UserAgent = device.UserAgent;
            IsMobile = !string.IsNullOrEmpty(device.GetCapability("mobile_browser"));
            IsTablet = Capability<bool>(device, "is_tablet");
            if (IsTablet)
                IsMobile = false;   
            IsTouchScreen = Capability<String>(device, "pointing_method").Equals("touchscreen");
            ScreenResolutionWidthInPixels = Capability<int>(device, "resolution_width");
            ScreenResolutionHeightInPixels = Capability<int>(device, "resolution_height");
            SupportsFlash = Capability<bool>(device, "full_flash_support");
            BrowserName = Capability<string>(device, "mobile_browser");
            HtmlPreferedDTD = Capability<string>(device, "html_preferred_dtd");

            Capabilities = device.GetCapabilities();
        }

        #region Private methods
       /// <summary>
       /// 
       /// </summary>
       /// <typeparam name="T"></typeparam>
        /// <param name="device">A <see cref="IDevice"/> IDevice representing the device of interest</param>
       /// <param name="name">A user Agent String</param>
       /// <returns></returns>
        private T Capability<T>(IDevice device, string name)
        {
            string ret = String.Empty;
            if (device.GetCapabilities().ContainsKey(name))
            {
                ret = device.GetCapabilities()[name];
            }
            else
            {
                ret = null;
            }

            return (T)Convert.ChangeType(ret, typeof(T));
        }
        #endregion
    }
}
