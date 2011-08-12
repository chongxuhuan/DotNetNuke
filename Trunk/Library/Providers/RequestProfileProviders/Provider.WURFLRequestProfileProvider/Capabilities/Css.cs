using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Css
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public string Gradient { get { return this.CapabilityProfile.Capability<string>("css_gradient"); } }
        public string BorderImage { get { return this.CapabilityProfile.Capability<string>("css_border_image"); } }
        public string RoundedCorners { get { return this.CapabilityProfile.Capability<string>("css_rounded_corners"); } }
        public bool Spriting { get { return this.CapabilityProfile.Capability<bool>("css_spriting"); } }
        public bool SupportsWidthAsPercentage { get { return this.CapabilityProfile.Capability<bool>("css_supports_width_as_percentage"); } }

    }
}