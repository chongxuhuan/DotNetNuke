using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Rss
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool RssSupport { get { return this.CapabilityProfile.Capability<bool>("rss_support"); } }

    }
}