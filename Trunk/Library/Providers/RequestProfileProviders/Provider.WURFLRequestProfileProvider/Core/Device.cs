/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.Devices.Core.Resource;

namespace DotNetNuke.Services.Devices.Core
{
    public class Device : IDevice
    {
        private readonly ICapabilitiesHolder _capabilitiesHolder;
        private readonly IMarkUpResolver _markUpResolver;
        private readonly ModelDevice _modelDevice;


        public Device(ModelDevice modelDevice, ICapabilitiesHolder capabilitiesHolder)
            : this(modelDevice, capabilitiesHolder, new MarkUpResolver())
        {
        }

        public Device(ModelDevice modelDevice, ICapabilitiesHolder capabilitiesHolder, IMarkUpResolver markUpResolver)
        {
            _modelDevice = modelDevice;
            _capabilitiesHolder = capabilitiesHolder;
            _markUpResolver = markUpResolver;
        }

        #region IDevice Members

        public string ID
        {
            get { return _modelDevice.ID; }
        }

        public string UserAgent
        {
            get { return _modelDevice.UserAgent; }
        }

        public string FallBack
        {
            get { return _modelDevice.FallBack; }
        }

        public bool IsActualDeviceRoot
        {
            get { return _modelDevice.IsActualDeviceRoot; }
        }

        public string GetCapability(string capabilityName)
        {
            return _capabilitiesHolder.GetCapabilityValue(capabilityName);
        }

        public IDictionary<string, string> Capabilities
        {
            get { return _capabilitiesHolder.Capabilities; }
        }

        public MarkUp MarkUp
        {
            get { return _markUpResolver.ResolveMarkUp(this); }
        }

        #endregion
    }
}