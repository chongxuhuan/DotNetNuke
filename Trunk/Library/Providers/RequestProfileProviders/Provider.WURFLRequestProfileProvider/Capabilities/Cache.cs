using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Cache
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool TimeToLiveSupport { get { return this.CapabilityProfile.Capability<bool>("time_to_live_support"); } }
        public bool TotalCacheDisableSupport { get { return this.CapabilityProfile.Capability<bool>("total_cache_disable_support"); } }

    }
}