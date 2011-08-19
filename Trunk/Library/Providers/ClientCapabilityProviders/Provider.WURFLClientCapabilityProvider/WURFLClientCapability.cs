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
    public class WURLClientCapability : IClientCapability
    {

        /// <summary>
        /// WURLClientCapability constructor
        /// </summary>
        public WURLClientCapability(IDevice device)            
        {
            this.ID = device.Id;
            this.UserAgent = device.UserAgent;
            this.IsMobile = !string.IsNullOrEmpty(device.GetCapability("mobile_browser"));

            this.IsTablet = Capability<bool>(device, "is_tablet");
            this.IsTouchScreen = Capability<String>(device, "pointing_method").Equals("touchscreen");
            this.ScreenResolutionWidthInPixels = Capability<int>(device, "resolution_width");
            this.ScreenResolutionHeightInPixels = Capability<int>(device, "resolution_height");
            this.SupportsFlash = Capability<bool>(device, "full_flash_support");
            this.BrowserName = Capability<string>(device, "mobile_browser");
            
            this.Capabilities = device.GetCapabilities();
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

        #region Implementation of IClientCapability

        /// <summary>
        ///   Unique ID of the client making request.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        ///   User Agent of the client making request
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        ///   Is request coming from a mobile device.
        /// </summary>
        public bool IsMobile { get; set; }

        /// <summary>
        ///   Is request coming from a tablet device.
        /// </summary>
        public bool IsTablet { get; set; }        

        /// <summary>
        ///   Does the requesting device supports touch screen.
        /// </summary>
        public bool IsTouchScreen { get; set; }

        /// <summary>
        ///   FacebookRequest property is filled when request is coming though Facebook iFrame (e.g. fan pages).
        /// </summary>
        /// <remarks>
        ///   FacebookRequest property is populated based on data in "signed_request" headers coming from Facebook.  
        ///   In order to ensure request is coming from Facebook, FacebookRequest.IsValidSignature method should be called with the secrety key provided by Facebook.
        /// </remarks>         
        public FacebookRequest FacebookRequest { get; set; }

        /// <summary>
        ///   ScreenResolution Width of the requester in Pixels.
        /// </summary>
        public int ScreenResolutionWidthInPixels { get; set; }

        /// <summary>
        ///   ScreenResolution Height of the requester in Pixels.
        /// </summary>
        public int ScreenResolutionHeightInPixels { get; set; }

        /// <summary>
        ///   Does requester support Flash.
        /// </summary>
        public bool SupportsFlash { get; set; }

        /// <summary>
        /// A key-value collection containing all capabilities supported by requester
        /// </summary>        
        public IDictionary<string, string> Capabilities { get; set; }

        /// <summary>
        /// Represents the name of the broweser in the request
        /// </summary>
        public string BrowserName { get; set; }
        #endregion
    }
}
