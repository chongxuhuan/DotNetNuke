using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Display
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public int PhysicalScreenHeight { get { return this.CapabilityProfile.Capability<int>("physical_screen_height"); } }
        public int Columns { get { return this.CapabilityProfile.Capability<int>("columns"); } }
        public bool DualOrientation { get { return this.CapabilityProfile.Capability<bool>("dual_orientation"); } }
        public int PhysicalScreenWidth { get { return this.CapabilityProfile.Capability<int>("physical_screen_width"); } }
        public int Rows { get { return this.CapabilityProfile.Capability<int>("rows"); } }
        public int MaxImageWidth { get { return this.CapabilityProfile.Capability<int>("max_image_width"); } }
        public int ResolutionHeight { get { return this.CapabilityProfile.Capability<int>("resolution_height"); } }
        public int ResolutionWidth { get { return this.CapabilityProfile.Capability<int>("resolution_width"); } }
        public int MaxImageHeight { get { return this.CapabilityProfile.Capability<int>("max_image_height"); } }

    }
}