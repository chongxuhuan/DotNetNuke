using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class WapPush
    {
        public KnownDevice Device { get; set; }
        public bool ExpirationDate { get { return Device.Capability<bool>("expiration_date"); } }
        public bool Utf8Support { get { return Device.Capability<bool>("utf8_support"); } }
        public bool ConnectionlessCacheOperation { get { return Device.Capability<bool>("connectionless_cache_operation"); } }
        public bool ConnectionlessServiceLoad { get { return Device.Capability<bool>("connectionless_service_load"); } }
        public bool Iso8859Support { get { return Device.Capability<bool>("iso8859_support"); } }
        public bool ConnectionorientedConfirmedServiceIndication { get { return Device.Capability<bool>("connectionoriented_confirmed_service_indication"); } }
        public bool ConnectionlessServiceIndication { get { return Device.Capability<bool>("connectionless_service_indication"); } }
        public bool AsciiSupport { get { return Device.Capability<bool>("ascii_support"); } }
        public bool ConnectionorientedConfirmedCacheOperation { get { return Device.Capability<bool>("connectionoriented_confirmed_cache_operation"); } }
        public bool ConnectionorientedConfirmedServiceLoad { get { return Device.Capability<bool>("connectionoriented_confirmed_service_load"); } }
        public bool WapPushSupport { get { return Device.Capability<bool>("wap_push_support"); } }
        public bool ConnectionorientedUnconfirmedCacheOperation { get { return Device.Capability<bool>("connectionoriented_unconfirmed_cache_operation"); } }
        public bool ConnectionorientedUnconfirmedServiceLoad { get { return Device.Capability<bool>("connectionoriented_unconfirmed_service_load"); } }
        public bool ConnectionorientedUnconfirmedServiceIndication { get { return Device.Capability<bool>("connectionoriented_unconfirmed_service_indication"); } }

    }
}