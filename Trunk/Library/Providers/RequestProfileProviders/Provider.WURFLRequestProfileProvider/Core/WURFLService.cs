/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using DotNetNuke.Services.Devices.Core.Cache;
using DotNetNuke.Services.Devices.Core.Matchers;
using DotNetNuke.Services.Devices.Core.Request;

namespace DotNetNuke.Services.Devices.Core
{
    internal class WURFLService : IWURFLService
    {
        private readonly ICache<string, IDevice> _cache;
        private readonly IDeviceProvider<IDevice> _deviceProvider;


        private readonly IMatcher<IWURFLRequest> _matcher;

        public WURFLService(IDeviceProvider<IDevice> deviceProvider, IMatcher<IWURFLRequest> matcher,
                            ICache<string, IDevice> cache)
        {
            _deviceProvider = deviceProvider;
            _matcher = matcher;
            _cache = cache;
        }

        #region IWURFLService Members

        public IDevice GetDeviceForRequest(IWURFLRequest wurflRequest)
        {
            // Get device from cache
            string key = CreateDeviceKey(wurflRequest);
            IDevice device = _cache.Get(key);

            if (device == null)
            {
                // Matching device
                string deviceId = _matcher.Match(wurflRequest);
                device = _deviceProvider.GetDevice(deviceId);

                device.Capabilities.Add("user_agent_profile", wurflRequest.UserAgentProfile);
                device.Capabilities.Add("user_agent", wurflRequest.UserAgent);
                device.Capabilities.Add("is_xhtml_device", wurflRequest.IsXHTMLDevice.ToString());

                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpBrowserCapabilities browser = System.Web.HttpContext.Current.Request.Browser;

                    foreach (var k in browser.Capabilities.Keys)
                    {
                        if (browser.Capabilities[k]!=null)
                            device.Capabilities.Add("ms_" + k.ToString(), browser.Capabilities[k].ToString());
                        else
                            device.Capabilities.Add("ms_" + k.ToString(), "");
                    }
                    
                }

                _cache.Put(key, device);
            } // if (device == null)


            return device;
        }

        #endregion

        private static string CreateDeviceKey(IWURFLRequest request)
        {
            return request.UserAgent;
        }
    }
}