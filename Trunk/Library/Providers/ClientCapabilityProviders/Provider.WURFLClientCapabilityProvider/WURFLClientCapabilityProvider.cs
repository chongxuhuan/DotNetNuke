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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Caching;

using DotNetNuke.Collections.Internal;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;

using FiftyOne.Foundation.Mobile.Detection;
using FiftyOne.Foundation.Mobile.Detection.Wurfl;

#endregion

namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    ///   WURFL implementation of ClientCapabilityProvider
    /// </summary>
    public class WURFLClientCapabilityProvider : ClientCapabilityProvider
    {
        #region Attributes

        /// <summary>
        /// WURFL data paths
        /// </summary>
        private static String _wurflDataFile;
        private static String _wurflPatchFile;

        #endregion

        #region Constructors
        /// <summary>
        /// Default Contructor
        /// </summary>
        public WURFLClientCapabilityProvider()
        {
            _wurflDataFile = Common.Globals.ApplicationMapPath + "/App_Data/WURFLDeviceDatabase/wurfl.xml.gz";
            _wurflPatchFile = Common.Globals.ApplicationMapPath + "/App_Data/WURFLDeviceDatabase/web_browsers_patch.xml";
        }

        /// <summary>
        /// Constructotr injecting the path to data source.  The client code is responsible for passing the full path for the data source xml files 
        /// </summary>
        /// <param name="wurflDataFilePath"></param>
        /// <param name="tWurflPatchFilePath"></param>
        public WURFLClientCapabilityProvider(String wurflDataFilePath, String tWurflPatchFilePath)
        {
            _wurflDataFile = wurflDataFilePath;
            _wurflPatchFile = tWurflPatchFilePath;
        }

        #endregion

        #region Static Methods

        static object _providerLock = new object();
        static Provider _wurflProvider;
        private static Provider WurflProvider
        {
            get
            {
                if (_wurflProvider != null)
                    return _wurflProvider;

                lock (_providerLock)
                {
                    // Initializes the WURFL infrastructure
                    string[] wurflFiles = { _wurflDataFile, _wurflPatchFile };
                    _wurflProvider = new Provider(wurflFiles, null, false);

                    return _wurflProvider;
                }
            }
        }

        static object _capabiliyValueLock = new object();        
        static IDictionary<string, List<string>> _capabilityValues;
        private static IDictionary<string, List<string>> CapabilityValues
        {
            get
            {
                if (_capabilityValues != null)
                    return _capabilityValues;

                lock (_capabiliyValueLock)
                {
                    var capabilityValues = new Dictionary<string, List<string>>();
                    
                    foreach (var device in AllDevices)
                    {                    	
                        foreach (var capability in device.Capabilities)
                        {
							//check for empty capability.Value
							if (string.IsNullOrEmpty((capability.Value)))
							{
								continue;
							}

							//if the capability is high piority item, add it to high piority list for later add them in top of list.
							if (HighPiorityCapabilityValues.ContainsKey(capability.Key))
							{
								if (!HighPiorityCapabilityValues[capability.Key].Contains(capability.Value))
								{
									HighPiorityCapabilityValues[capability.Key].Add(capability.Value);
								}
							}
							else
							{
								if (capabilityValues.ContainsKey(capability.Key))
								{
									if (!capabilityValues[capability.Key].Contains(capability.Value))
									{
										capabilityValues[capability.Key].Add(capability.Value);
									}
								}
								else
								{
									capabilityValues.Add(capability.Key, new List<string>() { capability.Value });
								}
							}
                        }
                    }

					//order the capability list
					var sortedCapabilityValues = capabilityValues.OrderBy(c => c.Key);
					//add high piority items into top of the list
					_capabilityValues = HighPiorityCapabilityValues.Concat(sortedCapabilityValues).ToDictionary(c => c.Key, c => c.Value);

                	return _capabilityValues;
                }
            }
        }

    	private static IDictionary<string, List<string>> _highPiorityCapabilityValues;
		private static IDictionary<string, List<string>> HighPiorityCapabilityValues
    	{
    		get
    		{
    			if(_highPiorityCapabilityValues == null)
    			{
    				//add is_wireless_device,is_tablet,device_os,mobile_browser,mobile_browser_version,
					//pointing_method,device_os_version,resolution_width,resolution_height,brand_name
					//as high piority capability values, it will appear at the top of capability values list.
					_highPiorityCapabilityValues = new Dictionary<string, List<string>>();

					_highPiorityCapabilityValues.Add("is_wireless_device", new List<string>());
					_highPiorityCapabilityValues.Add("is_tablet", new List<string>());
					_highPiorityCapabilityValues.Add("device_os", new List<string>());
					_highPiorityCapabilityValues.Add("mobile_browser", new List<string>());
					_highPiorityCapabilityValues.Add("mobile_browser_version", new List<string>());
					_highPiorityCapabilityValues.Add("pointing_method", new List<string>());
					_highPiorityCapabilityValues.Add("device_os_version", new List<string>());
					_highPiorityCapabilityValues.Add("resolution_width", new List<string>());
					_highPiorityCapabilityValues.Add("resolution_height", new List<string>());
					_highPiorityCapabilityValues.Add("brand_name", new List<string>());
    			}

    			return _highPiorityCapabilityValues;
    		}
    	}

        static object _allCapabilitiesLock = new object();
        static IQueryable<IClientCapability> _allCapabilities;
        private static IQueryable<IClientCapability> AllCapabilities
        {
            get
            {
                if (_allCapabilities != null)
                    return _allCapabilities;

                lock (_allCapabilitiesLock)
                {
                    var capabilities = new List<IClientCapability>();
                    
                    foreach (var device in AllDevices)
                    {
                        capabilities.Add(new WURFLClientCapability(device));
                    }

                    _allCapabilities = capabilities.AsQueryable();
                    return _allCapabilities;
                }
            }
        }

        static IQueryable<DeviceInfoClientCapability> _allDevices;
        private static IQueryable<DeviceInfoClientCapability> AllDevices
        {
            get
            {
				const string cacheKey = "WURFLClientCapability_AllDevices";
            	const int cacheTimeout = 1440;
				var cacheArg = new CacheItemArgs(cacheKey, cacheTimeout, CacheItemPriority.Default);
				return CBO.GetCachedObject<IQueryable<DeviceInfoClientCapability>>(cacheArg, GetAllDevicesCallBack);
            }
        }

		private static IQueryable<DeviceInfoClientCapability> GetAllDevicesCallBack(CacheItemArgs cacheItem)
		{
			var devices = new List<DeviceInfoClientCapability>();
			var allDevicesField = WurflProvider.GetType().GetField("AllDevices", BindingFlags.Instance | BindingFlags.NonPublic);
			var devicesInProvider = allDevicesField.GetValue(WurflProvider) as IDictionary<int, BaseDeviceInfo>;

			foreach (var device in devicesInProvider.Values)
			{
				if (!string.IsNullOrEmpty(device.UserAgent))
				{
					devices.Add(new DeviceInfoClientCapability(device as DeviceInfo));
				}
			}

			_allDevices = devices.AsQueryable();
			return _allDevices;
		}
        #endregion

        #region ClientCapabilityProvider Methods

    	private const string UserAgentsCacheKey = "WurflUserAgents"; //user agents cache key
    	private const int UserAgentsCacheTimeout = 60; //user agents cache expire time.(minutes)

        /// <summary>
        ///   Returns ClientCapability based on HttpRequest
        /// </summary>
        public override IClientCapability GetClientCapability(string userAgent)
        {
            DeviceInfoClientCapability deviceInfoClientCapability = null;

			if (!string.IsNullOrEmpty(userAgent))
			{
				bool found = false;

				//try to get content from cache
                var cachedUserAgents = DataCache.GetCache<SharedDictionary<string, DeviceInfoClientCapability>>(UserAgentsCacheKey);
				if (cachedUserAgents != null)
                {
                    using (cachedUserAgents.GetReadLock())
                    {
                        if (cachedUserAgents.ContainsKey(userAgent))
                        {
                            deviceInfoClientCapability = cachedUserAgents[userAgent];
                            found = true;
                        }                        
                    }
                }

				if (!found)
				{
					var deviceInfo = WurflProvider.GetDeviceInfo(userAgent);
					if (deviceInfo != null)
					{
						deviceInfoClientCapability = new DeviceInfoClientCapability(deviceInfo);

						//update cache content
						if(cachedUserAgents == null)
						{
                            cachedUserAgents = new SharedDictionary<string, DeviceInfoClientCapability>();
						}
                        using (cachedUserAgents.GetWriteLock())
                        {
                            cachedUserAgents[userAgent] = deviceInfoClientCapability;
                        }
					    DataCache.SetCache(UserAgentsCacheKey, cachedUserAgents, TimeSpan.FromMinutes(UserAgentsCacheTimeout));
					}
				}
			}

        	var wurflClientCapability =  new WURFLClientCapability(deviceInfoClientCapability);
            wurflClientCapability.UserAgent = userAgent;
            return wurflClientCapability;
        }

        /// <summary>
        ///   Returns ClientCapability based on ClientCapabilityId
        /// </summary>
        public override IClientCapability GetClientCapabilityById(string clientId)
        {
			Requires.NotNullOrEmpty("clientId", clientId);

        	var device = AllDevices.FirstOrDefault(c => c.DeviceInfo.DeviceId == clientId);
            
			if(device == null)
			{
				throw new Exception(string.Format("Can't get client capability for the id '{0}'", clientId));
			}
            
			return new WURFLClientCapability(device);
        }

        /// <summary>
        /// Returns available Capability Values for every  Capability Name
        /// </summary>
        /// <returns>
        /// Dictionary of Capability Name along with List of possible values of the Capability
        /// </returns>
        /// <example>Capability Name = mobile_browser, value = Safari, Andriod Webkit </example>
        public override IDictionary<string, List<string>> GetAllClientCapabilityValues()
        {
            return CapabilityValues;       
        }

        /// <summary>
        /// Returns All available Client Capabilities present
        /// </summary>
        /// <returns>
        /// List of IClientCapability present
        /// </returns>        
        public override IQueryable<IClientCapability> GetAllClientCapabilities()
        {
            return AllCapabilities;
        }

        #endregion
    }
}
