/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core
{
    public class CapabilitiesHolder : ICapabilitiesHolder
    {
        private readonly ICapabilitiesLoader _capabilitiesLoader;
        private IDictionary<string, string> _capabilities;

        public CapabilitiesHolder(ICapabilitiesLoader capabilitiesLoader)
        {
            _capabilitiesLoader = capabilitiesLoader;
        }

        #region ICapabilitiesHolder Members

        public string GetCapabilityValue(string capabilityName)
        {
            if (capabilityName == null)
            {
                throw new ArgumentNullException("capabilityName must not be null!");
            }

            string capabilityValue = null;
            if (!Capabilities.TryGetValue(capabilityName, out capabilityValue))
            {
                throw new ArgumentException("The Capability Name is not defined");
            }

            return capabilityValue;
        }

        /// <summary>
        /// Reuturns All the capabilities
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public IDictionary<string, string> Capabilities
        {
            get
            {
                if (_capabilities == null)
                {
                    _capabilities = _capabilitiesLoader.LoadCapabilities();
                }

                return _capabilities;
            }
        }

        #endregion
    }
}