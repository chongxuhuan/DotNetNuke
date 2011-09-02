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
using System.Linq;

using DotNetNuke.Collections.Internal;

using WURFL;
using WURFL.Config;

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
            _wurflDataFile = Common.Globals.ApplicationMapPath + "/App_Data/WURFLDeviceDatabase/wurfl-latest.zip";
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

        #region static methods
        static object _managerLock = new object();
        static IWURFLManager _wurflManager;
        private static IWURFLManager Manager
        {
            get
            {
                lock (_managerLock)
                {
                    if (_wurflManager != null) 
                        return _wurflManager;

                    // Initializes the WURFL infrastructure
                    var configurer = new InMemoryConfigurer()
                        .MainFile(_wurflDataFile)
                        .PatchFile(_wurflPatchFile);
                    _wurflManager = WURFLManagerBuilder.Build(configurer);

                    return _wurflManager;
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

                    var devices = Manager.GetAllDevices();
                    foreach (var device in devices)
                    {
                        foreach (var capability in device.GetCapabilities())
                        {
							//if the capability is high piority item, add it to high piority list for later add them in top of list.
							if (HighPiorityCapabilityValues.ContainsKey(capability.Key))
							{
								if (!HighPiorityCapabilityValues[capability.Key].Contains(capability.Value))
								{
									//check for empty capability.Value
									if (!String.IsNullOrEmpty((capability.Value))) HighPiorityCapabilityValues[capability.Key].Add(capability.Value);
								}
							}
							else
							{
								if (capabilityValues.ContainsKey(capability.Key))
								{
									if (!capabilityValues[capability.Key].Contains(capability.Value))
									{
										//check for empty capability.Value
										if (!String.IsNullOrEmpty((capability.Value))) capabilityValues[capability.Key].Add(capability.Value);
									}
								}
								else
								{
									//check for empty capability.Value
									if (!String.IsNullOrEmpty((capability.Value))) capabilityValues.Add(capability.Key, new List<string>() { capability.Value });
								}
							}
                        }
						//order the capability list
						var sortedCapabilityValues = capabilityValues.OrderBy(c => c.Key);
						//add high piority items into top of the list
						_capabilityValues = HighPiorityCapabilityValues.Concat(sortedCapabilityValues).ToDictionary(c => c.Key, c => c.Value);
                    }

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

                    var devices = Manager.GetAllDevices();
                    foreach (var device in devices)
                    {
                        capabilities.Add(new WURFLClientCapability(device));
                    }

                    _allCapabilities = capabilities.AsQueryable();
                    return _allCapabilities;
                }
            }
        }
        #endregion

        #region ClientCapabilityProvider Methods

        /// <summary>
        ///   Returns ClientCapability based on HttpRequest
        /// </summary>
        public override IClientCapability GetClientCapability(string userAgent)
        {            
            var device = Manager.GetDeviceForRequest(userAgent);
            if (device != null)
                return new WURFLClientCapability(device);
            return null;
        }

        /// <summary>
        ///   Returns ClientCapability based on ClientCapabilityId
        /// </summary>
        public override IClientCapability GetClientCapabilityById(string clientId)
        {
            var device = Manager.GetDeviceById(clientId);
            if (device != null)
                return new WURFLClientCapability(device);
            return null;
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
