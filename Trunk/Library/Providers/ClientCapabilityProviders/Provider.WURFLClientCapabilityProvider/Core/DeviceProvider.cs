/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using DotNetNuke.Services.ClientCapability.Resource;

namespace DotNetNuke.Services.ClientCapability
{
    internal class DeviceProvider : IDeviceProvider<IDevice>
    {
        private readonly IWURFLModel _wurflModel;

        public DeviceProvider(IWURFLModel wurflModel)
        {
            _wurflModel = wurflModel;
        }

        #region IDeviceProvider<IDevice> Members

        public IDevice GetDevice(string deviceID)
        {
            // Load modelDevice
            ModelDevice modelDevice = _wurflModel.GetDeviceByID(deviceID);


            ICapabilitiesHolder capabilitiesHolder = new CapabilitiesHolder(
                new CapabilitiesLoader(_wurflModel, modelDevice));

            Device device = new Device(modelDevice, capabilitiesHolder);


            return device;
        }

        #endregion
    }
}