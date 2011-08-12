using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Bearer
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool Sdio { get { return this.CapabilityProfile.Capability<bool>("sdio"); } }
        public bool Wifi { get { return this.CapabilityProfile.Capability<bool>("wifi"); } }
        public bool HasCellularRadio { get { return this.CapabilityProfile.Capability<bool>("has_cellular_radio"); } }
        public int MaxDataRate { get { return this.CapabilityProfile.Capability<int>("max_data_rate"); } }
        public bool Vpn { get { return this.CapabilityProfile.Capability<bool>("vpn"); } }

    }
}