using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace DotNetNuke.Services.ClientCapability
{
    public class Ajax
    {
        public KnownDevice Device { get; set; }
        public string PreferredGeolocApi { get { return Device.Capability<string>("ajax_preferred_geoloc_api"); } }
        public string XhrType { get { return Device.Capability<string>("ajax_xhr_type"); } }
        public bool SupportGetelementbyid { get { return Device.Capability<bool>("ajax_support_getelementbyid"); } }
        public bool SupportEventListener { get { return Device.Capability<bool>("ajax_support_event_listener"); } }
        public bool ManipulateDom { get { return Device.Capability<bool>("ajax_manipulate_dom"); } }
        public bool SupportJavascript { get { return Device.Capability<bool>("ajax_support_javascript"); } }
        public bool SupportInnerHtml { get { return Device.Capability<bool>("ajax_support_inner_html"); } }
        public bool ManipulateCss { get { return Device.Capability<bool>("ajax_manipulate_css"); } }
        public bool SupportEvents { get { return Device.Capability<bool>("ajax_support_events"); } }

    }
}