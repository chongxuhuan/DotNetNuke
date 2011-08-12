using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Security
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool PhoneIdProvided { get { return this.CapabilityProfile.Capability<bool>("phone_id_provided"); } }
        public string HttpsSupport { get { return this.CapabilityProfile.Capability<string>("https_support"); } }
        public bool HttpsVerisignClass3 { get { return this.CapabilityProfile.Capability<bool>("https_verisign_class3"); } }

    }
}