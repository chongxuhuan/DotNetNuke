using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class Rss
    {
        public KnownDevice Device { get; set; }
        public bool RssSupport { get { return Device.Capability<bool>("rss_support"); } }

    }
}