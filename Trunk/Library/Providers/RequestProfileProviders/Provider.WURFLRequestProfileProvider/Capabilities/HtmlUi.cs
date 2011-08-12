using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class HtmlUi
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public string CanvasSupport { get { return this.CapabilityProfile.Capability<string>("canvas_support"); } }
        public string ViewportWidth { get { return this.CapabilityProfile.Capability<string>("viewport_width"); } }
        public string HtmlPreferredDtd { get { return this.CapabilityProfile.Capability<string>("html_preferred_dtd"); } }
        public bool ViewportSupported { get { return this.CapabilityProfile.Capability<bool>("viewport_supported"); } }
        public string ViewportMinimumScale { get { return this.CapabilityProfile.Capability<string>("viewport_minimum_scale"); } }
        public string ViewportInitialScale { get { return this.CapabilityProfile.Capability<string>("viewport_initial_scale"); } }
        public bool Mobileoptimized { get { return this.CapabilityProfile.Capability<bool>("mobileoptimized"); } }
        public string ViewportMaximumScale { get { return this.CapabilityProfile.Capability<string>("viewport_maximum_scale"); } }
        public string ViewportUserscalable { get { return this.CapabilityProfile.Capability<string>("viewport_userscalable"); } }
        public bool ImageInlining { get { return this.CapabilityProfile.Capability<bool>("image_inlining"); } }
        public bool Handheldfriendly { get { return this.CapabilityProfile.Capability<bool>("handheldfriendly"); } }

    }
}