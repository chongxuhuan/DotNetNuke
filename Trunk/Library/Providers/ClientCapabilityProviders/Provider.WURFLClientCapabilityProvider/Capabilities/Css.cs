using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace DotNetNuke.Services.ClientCapability
{
    public class Css
    {
        public KnownDevice Device { get; set; }
        public string Gradient { get { return Device.Capability<string>("css_gradient"); } }
        public string BorderImage { get { return Device.Capability<string>("css_border_image"); } }
        public string RoundedCorners { get { return Device.Capability<string>("css_rounded_corners"); } }
        public bool Spriting { get { return Device.Capability<bool>("css_spriting"); } }
        public bool SupportsWidthAsPercentage { get { return Device.Capability<bool>("css_supports_width_as_percentage"); } }

    }
}