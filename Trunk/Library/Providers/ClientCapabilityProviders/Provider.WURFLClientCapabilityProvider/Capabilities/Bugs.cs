using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class Bugs
    {
        public KnownDevice Device { get; set; }
        public bool Emptyok { get { return Device.Capability<bool>("emptyok"); } }
        public bool EmptyOptionValueSupport { get { return Device.Capability<bool>("empty_option_value_support"); } }
        public bool BasicAuthenticationSupport { get { return Device.Capability<bool>("basic_authentication_support"); } }
        public bool PostMethodSupport { get { return Device.Capability<bool>("post_method_support"); } }
    }
}