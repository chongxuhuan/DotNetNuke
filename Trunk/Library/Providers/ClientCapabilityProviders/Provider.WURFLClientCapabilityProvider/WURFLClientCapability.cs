using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotNetNuke.Services.ClientCapability;

using FiftyOne.Foundation.Mobile.Detection.Wurfl;


namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// WURFL Implementation of IClientCapability
    /// </summary>
    public class WURFLClientCapability : ClientCapability
    {

        #region Constructor
        /// <summary>
        /// WURFLClientCapability constructor
        /// </summary>
		public WURFLClientCapability(DeviceInfo device)
			: this(new DeviceInfoClientCapability(device))          
        {
        }

        /// <summary>
        /// WURFLClientCapability constructor
        /// </summary>
        public WURFLClientCapability(DeviceInfoClientCapability deviceInfoClientCapability)
        {

            if (deviceInfoClientCapability != null)
            {
                ID = deviceInfoClientCapability.DeviceInfo.DeviceId;
                IsMobile = deviceInfoClientCapability.IsMobile;
                IsTablet = deviceInfoClientCapability.IsTablet;
                IsTouchScreen = deviceInfoClientCapability.IsTouchScreen;
                ScreenResolutionWidthInPixels = deviceInfoClientCapability.ScreenResolutionWidthInPixels;
                ScreenResolutionHeightInPixels = deviceInfoClientCapability.ScreenResolutionHeightInPixels;
                SupportsFlash = deviceInfoClientCapability.SupportsFlash;
                BrowserName = deviceInfoClientCapability.BrowserName;
                HtmlPreferedDTD = deviceInfoClientCapability.HtmlPreferedDTD;

                Capabilities = deviceInfoClientCapability.Capabilities;
            }
        }
        #endregion
    }
}
