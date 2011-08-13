using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class Security
    {
        public KnownDevice Device { get; set; }
        public bool PhoneIdProvided { get { return Device.Capability<bool>("phone_id_provided"); } }
        public string HttpsSupport { get { return Device.Capability<string>("https_support"); } }
        public bool HttpsVerisignClass3 { get { return Device.Capability<bool>("https_verisign_class3"); } }

    }
}