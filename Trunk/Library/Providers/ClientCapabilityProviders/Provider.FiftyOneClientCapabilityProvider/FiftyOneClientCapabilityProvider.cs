/* *********************************************************************
 * The contents of this file are subject to the Mozilla Public License 
 * Version 1.1 (the "License"); you may not use this file except in 
 * compliance with the License. You may obtain a copy of the License at 
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS" 
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 * See the License for the specific language governing rights and 
 * limitations under the License.
 *
 * The Original Code is named ClientCapabilityProvider, first 
 * released under this licence on 25th January 2012.
 * 
 * The Initial Developer of the Original Code is owned by 
 * 51 Degrees Mobile Experts Limited. Portions created by 51 Degrees
 * Mobile Experts Limited are Copyright (C) 2012. All Rights Reserved.
 * 
 * Contributor(s):
 *     James Rosewell <james@51degrees.mobi>
 * 
 * ********************************************************************* */

#region Usings

using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Common;
using DotNetNuke.Services.Localization;

using FiftyOne.Foundation.Mobile.Detection;
using System.Web;
using FiftyOne.Foundation.Mobile;
using DotNetNuke.Services.ClientCapability;

using FiftyOne.Foundation.UI;

#endregion

namespace FiftyOne.Services.ClientCapability
{
    /// <summary>
    /// 51Degrees.mobi implementation of ClientCapabilityProvider
    /// </summary>
    public class FiftyOneClientCapabilityProvider : DotNetNuke.Services.ClientCapability.ClientCapabilityProvider
    {
        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FiftyOneClientCapabilityProvider()
        {
        }

        #endregion

        #region Static Methods

        static object _allCapabilitiesLock = new object();
        static IQueryable<IClientCapability> _allCapabilities;

        static object _allClientCapabilityValuesLock = new object();
        static Dictionary<string, List<string>> _allClientCapabilityValues;

        private static IQueryable<IClientCapability> AllCapabilities
        {
            get
            {
                if (_allCapabilities == null)
                {
                    lock (_allCapabilitiesLock)
                    {
                        if (_allCapabilities == null)
                        {
                            var capabilities = new List<IClientCapability>();

                            foreach (var device in Factory.ActiveProvider.Devices)
                            {
                                capabilities.Add(new FiftyOneClientCapability(device));
                            }

                            _allCapabilities = capabilities.AsQueryable();
                        }
                    }
                }
                return _allCapabilities;
            }
        }

        private static Dictionary<string, List<string>> ClientCapabilityValues
        {
            get
            {
                if (_allClientCapabilityValues == null)
                {
                    lock (_allClientCapabilityValuesLock)
                    {
                        if (_allClientCapabilityValues == null)
                        {
                            _allClientCapabilityValues = new Dictionary<string, List<string>>();

                            foreach (var property in Factory.ActiveProvider.Properties)
                            {
                                var values = new List<string>();
                                foreach (var value in property.Values)
                                    values.Add(value.Name);
                                _allClientCapabilityValues.Add(property.Name, values);
                            }
                        }
                    }
                }
                return _allClientCapabilityValues;
            }
        }

        #endregion

        #region ClientCapabilityProvider Override Methods

        /// <summary>
        /// Returns ClientCapability based on the user agent provided.
        /// </summary>
        public override IClientCapability GetClientCapability(string userAgent)
        {
            var request = HttpContext.Current != null ? HttpContext.Current.Request : null;
            if (request != null && request.UserAgent == userAgent &&
                request.Browser.Capabilities.Contains(FiftyOne.Foundation.Mobile.Detection.Constants.FiftyOneDegreesProperties))
                // The useragent has already been processed by 51Degrees.mobi when the request
                // was processed by the detector module. Uses the values obtained then.
                return new FiftyOneClientCapability(request.Browser);
            else
            {
                // The useragent has not already been processed. Therefore process it now
                // and then set the properties.
                var deviceInfo = Factory.ActiveProvider.GetDeviceInfo(userAgent);
                if(deviceInfo != null)
                {
                    return new FiftyOneClientCapability(deviceInfo);
                }
                else
                {
                    return new FiftyOneClientCapability(null as SortedList<string, List<string>>);
                }
            }
        }

        /// <summary>
        /// Returns ClientCapability based on device Id provided.
        /// </summary>
        public override IClientCapability GetClientCapabilityById(string deviceId)
        {
            Requires.NotNullOrEmpty("deviceId", deviceId);

            var device = Factory.ActiveProvider.GetDeviceInfoByID(deviceId);
            
			if(device == null)
			{
                throw new MobileException(string.Format("Can't get device capability for the id '{0}'", deviceId));
			}

            return new FiftyOneClientCapability(device.GetAllProperties());
        }

        /// <summary>
        /// Returns available Capability Values for every Capability Name
        /// </summary>
        /// <returns>
        /// Dictionary of Capability Name along with List of possible values of the Capability
        /// </returns>
        /// <example>Capability Name = mobile_browser, value = Safari, Andriod Webkit </example>
        public override IDictionary<string, List<string>> GetAllClientCapabilityValues()
        {
            return ClientCapabilityValues;
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

        #region Override Properties

        public override bool SupportTabletDetect
        {
            get
            {
                return DataProvider.IsPremium;
            }
        }

        #endregion
    }
}
