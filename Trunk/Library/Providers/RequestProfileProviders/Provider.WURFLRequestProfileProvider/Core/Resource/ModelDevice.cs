/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core.Resource
{
    /// <summary>
    /// 
    /// </summary>
    public class ModelDevice
    {
        private readonly bool _actualDeviceRoot;
        private readonly IDictionary<string, string> _capabilities;
        private readonly IDictionary<string, string> _capabilitiesByGroup;
        private readonly string _fallBack;
        private readonly string _id;
        private readonly string _userAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDevice"/> class.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="id">The id.</param>
        /// <param name="fallBack">The fall back.</param>
        /// <param name="actualDeviceRoot">if set to <c>true</c> [actual device start].</param>
        public ModelDevice(string userAgent, string id, String fallBack,
                           bool actualDeviceRoot)
        {
            _userAgent = userAgent;
            _id = id;
            _fallBack = fallBack;
            _actualDeviceRoot = actualDeviceRoot;
            _capabilities = new Dictionary<string, string>();
            _capabilitiesByGroup = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDevice"/> class.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="id">The id.</param>
        /// <param name="fallBack">The fall back.</param>
        /// <param name="actualDeviceRoot">if set to <c>true</c> [actual device start].</param>
        /// <param name="capabilities">The capabilities.</param>
        /// <param name="capabilitiesByGroup">The capabilities by group.</param>
        public ModelDevice(string userAgent, string id, String fallBack,
                           bool actualDeviceRoot, IDictionary<string, string> capabilities,
                           IDictionary<string, string> capabilitiesByGroup)
            : this(userAgent, id, fallBack, actualDeviceRoot)
        {
            _capabilities = capabilities;
            _capabilitiesByGroup = capabilitiesByGroup;
        }

        public IDictionary<string, string> Capabilities
        {
            get { return _capabilities; }
        }

        public IDictionary<string, string> CapabilitiesByGroup
        {
            get { return _capabilitiesByGroup; }
        }


        public string UserAgent
        {
            get { return _userAgent; }
        }

        public string ID
        {
            get { return _id; }
        }

        public string FallBack
        {
            get { return _fallBack; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is actual device start.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is actual device start; otherwise, <c>false</c>.
        /// </value>
        public bool IsActualDeviceRoot
        {
            get { return _actualDeviceRoot; }
        }


        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public IList<string> Groups
        {
            get { return new List<string>(_capabilitiesByGroup.Values); }
        }

        /// <summary>
        /// Gets the group for capability.
        /// </summary>
        /// <param name="capabilityName">Name of the capability.</param>
        /// <returns></returns>
        internal string GetGroupForCapability(string capabilityName)
        {
            string groupID = null;
            bool found = _capabilitiesByGroup.TryGetValue(capabilityName, out groupID);

            if (!found)
            {
                throw new CapabilityNotFoundException(capabilityName);
            }

            return groupID;
        }

        /// <summary>
        /// Determines whether the specified capability is defined in this device.
        /// </summary>
        /// <param name="capability">The capability.</param>
        /// <returns>
        /// 	<c>true</c> if [is capability defined] [the specified capability]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsCapabilityDefined(string capability)
        {
            return _capabilities.ContainsKey(capability);
        }

        /// <summary>
        /// Determines whether the specified group id is defined in this device.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <returns>
        /// 	<c>true</c> if [is group defined] [the specified group id]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsGroupDefined(string groupId)
        {
            return _capabilitiesByGroup.Values.Contains(groupId);
        }
    }
}