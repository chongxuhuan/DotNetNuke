using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class HtmlUi
    {
        public KnownDevice Device { get; set; }
        public string CanvasSupport { get { return Device.Capability<string>("canvas_support"); } }
        public string ViewportWidth { get { return Device.Capability<string>("viewport_width"); } }
        public string HtmlPreferredDtd { get { return Device.Capability<string>("html_preferred_dtd"); } }
        public bool ViewportSupported { get { return Device.Capability<bool>("viewport_supported"); } }
        public string ViewportMinimumScale { get { return Device.Capability<string>("viewport_minimum_scale"); } }
        public string ViewportInitialScale { get { return Device.Capability<string>("viewport_initial_scale"); } }
        public bool Mobileoptimized { get { return Device.Capability<bool>("mobileoptimized"); } }
        public string ViewportMaximumScale { get { return Device.Capability<string>("viewport_maximum_scale"); } }
        public string ViewportUserscalable { get { return Device.Capability<string>("viewport_userscalable"); } }
        public bool ImageInlining { get { return Device.Capability<bool>("image_inlining"); } }
        public bool Handheldfriendly { get { return Device.Capability<bool>("handheldfriendly"); } }

    }
}