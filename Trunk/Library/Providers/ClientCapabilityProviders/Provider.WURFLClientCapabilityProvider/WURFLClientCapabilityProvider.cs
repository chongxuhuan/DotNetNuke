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
                lock (_capabiliyValueLock)
                {
                    if (_capabilityValues != null)
                        return _capabilityValues;

                    _capabilityValues = new Dictionary<string, List<string>>();

                    var devices = Manager.GetAllDevices();
                    foreach (var device in devices)
                    {
                        foreach (var capability in device.GetCapabilities())
                        {
                            if (_capabilityValues.ContainsKey(capability.Key))
                            {
                                if (!_capabilityValues[capability.Key].Contains(capability.Value))
                                {
                                    //check for empty capability.Value
                                    if (!String.IsNullOrEmpty((capability.Value)))_capabilityValues[capability.Key].Add(capability.Value);
                                }
                            }
                            else
                            {
                                //check for empty capability.Value
                                if (!String.IsNullOrEmpty((capability.Value)))_capabilityValues.Add(capability.Key, new List<string>() { capability.Value });
                            }
                        }
                    }

                    return _capabilityValues;
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
                return new WURLClientCapability(device);
            return null;
        }

        /// <summary>
        ///   Returns ClientCapability based on ClientCapabilityId
        /// </summary>
        public override IClientCapability GetClientCapabilityById(string clientId)
        {
            var device = Manager.GetDeviceById(clientId);
            if (device != null)
                return new WURLClientCapability(device);
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
            throw new NotImplementedException();
            var devices = Manager.GetAllDevices();
            foreach (var device in devices)
            {
                
            }            
        }

        #endregion
    }
}
