using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.Services.ClientCapability
{
    public class Bearer
    {
        public KnownDevice Device { get; set; }
        public bool Sdio { get { return Device.Capability<bool>("sdio"); } }
        public bool Wifi { get { return Device.Capability<bool>("wifi"); } }
        public bool HasCellularRadio { get { return Device.Capability<bool>("has_cellular_radio"); } }
        public int MaxDataRate { get { return Device.Capability<int>("max_data_rate"); } }
        public bool Vpn { get { return Device.Capability<bool>("vpn"); } }

    }
}