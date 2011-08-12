using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Bugs
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool Emptyok { get { return this.CapabilityProfile.Capability<bool>("emptyok"); } }
        public bool EmptyOptionValueSupport { get { return this.CapabilityProfile.Capability<bool>("empty_option_value_support"); } }
        public bool BasicAuthenticationSupport { get { return this.CapabilityProfile.Capability<bool>("basic_authentication_support"); } }
        public bool PostMethodSupport { get { return this.CapabilityProfile.Capability<bool>("post_method_support"); } }

    }
}