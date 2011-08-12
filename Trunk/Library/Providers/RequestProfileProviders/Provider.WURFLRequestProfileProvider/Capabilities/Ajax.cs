using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Ajax
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public string PreferredGeolocApi { get { return this.CapabilityProfile.Capability<string>("ajax_preferred_geoloc_api"); } }
        public string XhrType { get { return this.CapabilityProfile.Capability<string>("ajax_xhr_type"); } }
        public bool SupportGetelementbyid { get { return this.CapabilityProfile.Capability<bool>("ajax_support_getelementbyid"); } }
        public bool SupportEventListener { get { return this.CapabilityProfile.Capability<bool>("ajax_support_event_listener"); } }
        public bool ManipulateDom { get { return this.CapabilityProfile.Capability<bool>("ajax_manipulate_dom"); } }
        public bool SupportJavascript { get { return this.CapabilityProfile.Capability<bool>("ajax_support_javascript"); } }
        public bool SupportInnerHtml { get { return this.CapabilityProfile.Capability<bool>("ajax_support_inner_html"); } }
        public bool ManipulateCss { get { return this.CapabilityProfile.Capability<bool>("ajax_manipulate_css"); } }
        public bool SupportEvents { get { return this.CapabilityProfile.Capability<bool>("ajax_support_events"); } }

    }
}