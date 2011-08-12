/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */

using System;
using System.Collections.Generic;
using DotNetNuke.Instrumentation;


namespace DotNetNuke.Services.Devices.Core.Resource
{
    public class WURFLPatcher : IWURFLPatcher<ModelDevice>
    {        
        #region IWURFLPatcher<ModelDevice> Members

        /// <summary>
        /// Patches the devices.
        /// </summary>
        /// <param name="devicesToPatch">The devices to patch.</param>
        /// <param name="patchingDevices">The patching devices.</param>
        /// <returns></returns>
        public IList<ModelDevice> PatchDevices(IList<ModelDevice> devicesToPatch, IList<ModelDevice> patchingDevices)
        {
            IDictionary<string, ModelDevice> devices = new Dictionary<string, ModelDevice>();


            foreach (ModelDevice modelDevice in devicesToPatch)
            {
                devices.Add(modelDevice.ID, modelDevice);
            }

            IDictionary<string, ModelDevice> patchedDevices = new Dictionary<string, ModelDevice>(devices);


            foreach (ModelDevice patchingDevice in patchingDevices)
            {
                ModelDevice patchedDevice = null;

                if (devices.ContainsKey(patchingDevice.ID))
                {
                    // The Device Exist so we need to apply the patch on it
                    DnnLog.Debug("Patching device: " + patchingDevice.ID);
                    
                    ModelDevice deviceToPatch = devices[patchingDevice.ID];

                    // Return a new instance
                    patchedDevice = PatchDevice(deviceToPatch, patchingDevice);
                }
                else
                {
                    // New Device, just add it to the list
                    DnnLog.Debug("Adding device: " + patchingDevice.ID);
                    
                    patchedDevice = patchingDevice;
                }

                patchedDevices[patchedDevice.ID] = patchedDevice;
            }

            return new List<ModelDevice>(patchedDevices.Values);
        }

        #endregion

        /// <summary>
        /// Patches the device.
        /// </summary>
        /// <param name="deviceToPatch">The device to patch.</param>
        /// <param name="patchingDevice">The patching device.</param>
        /// <returns></returns>
        private ModelDevice PatchDevice(ModelDevice deviceToPatch, ModelDevice patchingDevice)
        {
            // Returning device
            ModelDevice patchedDevice = null;

            // Patch capabilities
            IDictionary<string, string> patchedDeviceCapabilities =
                new Dictionary<string, string>(deviceToPatch.Capabilities);

            foreach (KeyValuePair<string, string> capability in patchingDevice.Capabilities)
            {
                patchedDeviceCapabilities[capability.Key] = capability.Value;
            }

            // Patch capabilities structure
            IDictionary<string, string> patchedDeviceCapabilitiesByGroup =
                new Dictionary<string, string>(deviceToPatch.CapabilitiesByGroup);
            foreach (KeyValuePair<string, string> capabilityByGroup in patchingDevice.CapabilitiesByGroup)
            {
                patchedDeviceCapabilitiesByGroup[capabilityByGroup.Key] = capabilityByGroup.Value;
            }


            if (!patchingDevice.UserAgent.Equals(deviceToPatch.UserAgent))
            {
                throw new Exception("You are not allowed to averride the User Agent of a device");
            }

            patchedDevice =
                new ModelDevice(deviceToPatch.UserAgent, deviceToPatch.ID, patchingDevice.FallBack,
                                patchingDevice.IsActualDeviceRoot, patchingDevice.Capabilities,
                                patchingDevice.CapabilitiesByGroup);

            return patchedDevice;
        }
    }
}