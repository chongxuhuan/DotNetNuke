/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;
using System.Text;
using DotNetNuke.Instrumentation;

namespace DotNetNuke.Services.ClientCapability.Resource
{
    internal static class WURFLConsistencyVerifier
    {
        /// <summary>
        /// Verifies : 
        /// - Null falback( orphan hierarchy) 
        /// - Circular hierarchy 
        /// - Double user-agent
        /// </summary>
        /// <param name="devices">The devices.</param>
        internal static void Verify(IList<ModelDevice> devices)
        {
            IDictionary<string, ModelDevice> devicesMapByID = new Dictionary<string, ModelDevice>();
            IDictionary<string, ModelDevice> devicesMapByUserAgent = new Dictionary<string, ModelDevice>();

            IList<string> hierhierarchyVerifiedDevices = new List<string>();

            foreach (ModelDevice device in devices)
            {
                devicesMapByID.Add(device.ID, device);
            }

            VerifyGeneric(devicesMapByID);


            foreach (KeyValuePair<string, ModelDevice> entry in devicesMapByID)
            {
                ModelDevice device = entry.Value;

                VerifyUserAgent(entry.Value, devicesMapByUserAgent);
                devicesMapByUserAgent.Add(device.UserAgent, device);

                DnnLog.Debug("Verifing " + device.ID + " hierarchy...");
                
                VerifyHierarchy(device, devicesMapByID, hierhierarchyVerifiedDevices);

                hierhierarchyVerifiedDevices.Add(device.ID);

                DnnLog.Debug("Verifing " + device.ID + " groups...");                

                VerifyGroups(device, devicesMapByID);
                DnnLog.Debug("Verifing " + device.ID + " capabilities...");                

                VerifyCapabilities(device, devicesMapByID);
            }
        }

        /// <summary>
        /// Verifies the existance of the generic device.
        /// </summary>
        /// <param name="devicesMap">The devices map.</param>
        private static void VerifyGeneric(IDictionary<string, ModelDevice> devicesMap)
        {
            if (!devicesMap.ContainsKey(Constants.GENERIC))
            {
                throw new Exception("Generic Device is not Found!!!");
            }
        }

        /// <summary>
        /// Verifies the uniqueness of the user agent.
        /// </summary>
        /// <param name="modelDevice">The model device.</param>
        /// <param name="devicesMapByUserAgent">The devices map by user agent.</param>
        private static void VerifyUserAgent(ModelDevice modelDevice,
                                            IDictionary<string, ModelDevice> devicesMapByUserAgent)
        {
            ModelDevice definingDevice = null;

            bool found = devicesMapByUserAgent.TryGetValue(modelDevice.UserAgent, out definingDevice);

            if (found)
            {
                throw new UserAgentNotUniqueException(modelDevice, definingDevice);
            }
        }


        /// <summary>
        /// Verifies the hierarchy.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="devicesMapByID">The devices map by ID.</param>
        /// <param name="hierarchyVerifiedDevices">The hierarchy verified devices.</param>
        private static void VerifyHierarchy(ModelDevice device, IDictionary<string, ModelDevice> devicesMapByID,
                                            IList<string> hierarchyVerifiedDevicesId)
        {
            IList<string> hierarchy = new List<string>(10);
            if (String.IsNullOrEmpty(device.ID))
            {
                throw new Exception("device id is null or empty");
            }


            hierarchy.Add(device.ID);


            string deviceID = device.ID;

            while (!String.Equals(Constants.GENERIC, deviceID))
            {
                ModelDevice examineDevice = devicesMapByID[deviceID];
                string fallBack = examineDevice.FallBack;

                if (hierarchyVerifiedDevicesId.Contains(fallBack))
                {
                    // OK
                    return;
                }

                if (!devicesMapByID.ContainsKey(fallBack))
                {
                    throw new Exception("Orphan Hierarchy");
                }

                int hierarchyIndex = hierarchy.IndexOf(fallBack);
                if (hierarchyIndex != -1)
                {
                    throw new Exception("circularHierarchy");
                }

                hierarchy.Add(fallBack);

                deviceID = fallBack;
            }
        }


        /// <summary>
        /// Verifies the groups.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="devicesMapByID">The devices map by ID.</param>
        private static void VerifyGroups(ModelDevice device, IDictionary<string, ModelDevice> devicesMapByID)
        {
            ModelDevice genericDevice = devicesMapByID[Constants.GENERIC];

            IList<string> genericDeviceGroups = genericDevice.Groups;
            IList<string> deviceGroups = device.Groups;

            foreach (string groupID in deviceGroups)
            {
                if (!genericDeviceGroups.Contains(groupID))
                {
                    StringBuilder message =
                        new StringBuilder().AppendFormat(
                            "The device {0} defines a group id {1} not defined by the generic device ", device.ID,
                            groupID);
                    throw new Exception(message.ToString());
                }
            }
        }


        /// <summary>
        /// Verifies the capabilities.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="devicesMapByID">The devices map by ID.</param>
        private static void VerifyCapabilities(ModelDevice device, IDictionary<string, ModelDevice> devicesMapByID)
        {
            ModelDevice genericDevice = devicesMapByID[Constants.GENERIC];

            IDictionary<string, string> genericDeviceCapabilities = genericDevice.Capabilities;
            IDictionary<string, string> deviceCapabilities = device.Capabilities;


            foreach (KeyValuePair<string, string> deviceCapabilitiy in deviceCapabilities)
            {
                if (!genericDeviceCapabilities.ContainsKey(deviceCapabilitiy.Key))
                {
                    StringBuilder message =
                        new StringBuilder().AppendFormat(
                            "The Device {0} defines a capability {1} not defined in generic", device.ID,
                            deviceCapabilitiy.Key);
                    throw new Exception(message.ToString());
                }
                //if (device.GetGroupForCapability(deviceCapabilitiy.Key).Equals(genericDevice.GetGroupForCapability(deviceCapabilitiy.Key)))
                //{
                //    throw new Exception("Bad Group Capability");
                //}
            }
        }
    }
}