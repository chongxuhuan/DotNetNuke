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
using System;
using System.Collections.Generic;
using System.Linq;

using FiftyOne.Foundation.Mobile.Detection.Wurfl;

namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// WURFL Implementation of IClientCapability
    /// </summary>
    public class DeviceInfoClientCapability
    {
        #region Public Properties
        /// <summary>
        /// DeviceInfo - FifityOne DeviceInfo Object
        /// </summary>
        public DeviceInfo DeviceInfo { get; set; }

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
        /// Capabilities - Collection of available capabilities of the device
        /// </summary>
        public IDictionary<string, string> Capabilities { get; set; }

        /// <summary>
        /// Represents the name of the broweser in the request
        /// </summary>
        public string BrowserName { get; set; }

        /// <summary>
        /// Returns the request prefered HTML DTD
        /// </summary>
        public string HtmlPreferedDTD { get; set; }

        /// <summary>
        ///   Http server variable that indicates that SSL offloading is enabled
        /// </summary>
        public string SSLOffload { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        ///   Default Constructor.
        /// </summary>
        public DeviceInfoClientCapability()
        {
            Capabilities = new Dictionary<string, string>();
        }

        /// <summary>
        ///   Constructor with DeviceInfo as parameter. Use this constructor to build capabailities collection
        /// </summary>
        public DeviceInfoClientCapability(DeviceInfo deviceInfo)
        {
            DeviceInfo = deviceInfo;
            Capabilities = new Dictionary<string, string>();
            foreach (var knownCapability in KnownCapabilities)
            {
                Capabilities.Add(knownCapability, deviceInfo.GetCapability(knownCapability));
            }

            IsMobile = Capability<bool>(deviceInfo, is_wireless_device);
            IsTablet = Capability<bool>(deviceInfo, is_tablet);
            if (IsTablet)
                IsMobile = false;
            IsTouchScreen = Capability<String>(deviceInfo, pointing_method).Equals("touchscreen");
            ScreenResolutionWidthInPixels = Capability<int>(deviceInfo, resolution_width);
            ScreenResolutionHeightInPixels = Capability<int>(deviceInfo, resolution_height);
            SupportsFlash = Capability<bool>(deviceInfo, full_flash_support);
            BrowserName = Capability<string>(deviceInfo, mobile_browser);
            HtmlPreferedDTD = Capability<string>(deviceInfo, html_preferred_dtd);
            SSLOffload = Capability<string>(deviceInfo, ssl_offload);
        }
        #endregion

        #region Private Properties

        private const string is_wireless_device = "is_wireless_device";
        private const string is_tablet = "is_tablet";
        private const string pointing_method = "pointing_method";
        private const string resolution_width = "resolution_width";
        private const string resolution_height = "resolution_height";
        private const string full_flash_support = "full_flash_support";
        private const string mobile_browser = "mobile_browser";
        private const string html_preferred_dtd = "html_preferred_dtd";
        private const string ssl_offload = "ssl_offload";
               

        private static IQueryable<string> _knownCapabilities;
        private static IQueryable<string> KnownCapabilities
        {
            get
            {
                if (_knownCapabilities == null)
                {
                    //add common default capabilities
                    var knownCapabilities = new List<string>();

                    knownCapabilities.Add(is_wireless_device);
                    knownCapabilities.Add(is_tablet);
                    knownCapabilities.Add("device_os");
                    knownCapabilities.Add(mobile_browser);
                    knownCapabilities.Add("mobile_browser_version");
                    knownCapabilities.Add(pointing_method);
                    knownCapabilities.Add("device_os_version");
                    knownCapabilities.Add(resolution_width);
                    knownCapabilities.Add(resolution_height);
                    knownCapabilities.Add("brand_name");
                    knownCapabilities.Add("marketing_name");
                    knownCapabilities.Add("model_name");
                    knownCapabilities.Add("physical_screen_width");
                    knownCapabilities.Add("physical_screen_height");
                    knownCapabilities.Add("max_image_width");
                    knownCapabilities.Add("max_image_height");
                    knownCapabilities.Add("access_key_support");
                    knownCapabilities.Add("built_in_back_button_support");
                    knownCapabilities.Add("colors");
                    knownCapabilities.Add("png");
                    knownCapabilities.Add("gif");
                    knownCapabilities.Add("jpg");                    
                    knownCapabilities.Add("ajax_support_javascript");
                    knownCapabilities.Add("xhtml_support_level");
                    knownCapabilities.Add("cookie_support");
                    knownCapabilities.Add("preferred_markup");
                    knownCapabilities.Add("xhtmlmp_preferred_mime_type");
                    knownCapabilities.Add(full_flash_support);
                    knownCapabilities.Add(html_preferred_dtd);
                    knownCapabilities.Add(ssl_offload); 

                    _knownCapabilities = knownCapabilities.AsQueryable();                    
                }

                return _knownCapabilities;
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        /// <param name="deviceInfo">A DeviceInfo representing the device of interest</param>
        /// <param name="name">A user Agent String</param>
        /// <returns></returns>
        private T Capability<T>(DeviceInfo deviceInfo, string name)
        {
            string ret = deviceInfo.GetCapability(name);            
            return (T)Convert.ChangeType(ret, typeof(T));
        }
        #endregion

    }


}
