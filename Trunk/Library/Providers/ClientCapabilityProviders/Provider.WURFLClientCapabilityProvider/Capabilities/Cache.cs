using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace DotNetNuke.Services.ClientCapability
{
    public class Cache
    {
        public KnownDevice Device { get; set; }
        public bool TimeToLiveSupport { get { return Device.Capability<bool>("time_to_live_support"); } }
        public bool TotalCacheDisableSupport { get { return Device.Capability<bool>("total_cache_disable_support"); } }

    }
}