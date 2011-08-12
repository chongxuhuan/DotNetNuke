using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class WapPush
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool ExpirationDate { get { return this.CapabilityProfile.Capability<bool>("expiration_date"); } }
        public bool Utf8Support { get { return this.CapabilityProfile.Capability<bool>("utf8_support"); } }
        public bool ConnectionlessCacheOperation { get { return this.CapabilityProfile.Capability<bool>("connectionless_cache_operation"); } }
        public bool ConnectionlessServiceLoad { get { return this.CapabilityProfile.Capability<bool>("connectionless_service_load"); } }
        public bool Iso8859Support { get { return this.CapabilityProfile.Capability<bool>("iso8859_support"); } }
        public bool ConnectionorientedConfirmedServiceIndication { get { return this.CapabilityProfile.Capability<bool>("connectionoriented_confirmed_service_indication"); } }
        public bool ConnectionlessServiceIndication { get { return this.CapabilityProfile.Capability<bool>("connectionless_service_indication"); } }
        public bool AsciiSupport { get { return this.CapabilityProfile.Capability<bool>("ascii_support"); } }
        public bool ConnectionorientedConfirmedCacheOperation { get { return this.CapabilityProfile.Capability<bool>("connectionoriented_confirmed_cache_operation"); } }
        public bool ConnectionorientedConfirmedServiceLoad { get { return this.CapabilityProfile.Capability<bool>("connectionoriented_confirmed_service_load"); } }
        public bool WapPushSupport { get { return this.CapabilityProfile.Capability<bool>("wap_push_support"); } }
        public bool ConnectionorientedUnconfirmedCacheOperation { get { return this.CapabilityProfile.Capability<bool>("connectionoriented_unconfirmed_cache_operation"); } }
        public bool ConnectionorientedUnconfirmedServiceLoad { get { return this.CapabilityProfile.Capability<bool>("connectionoriented_unconfirmed_service_load"); } }
        public bool ConnectionorientedUnconfirmedServiceIndication { get { return this.CapabilityProfile.Capability<bool>("connectionoriented_unconfirmed_service_indication"); } }

    }
}