using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class Display
    {
        public KnownDevice Device { get; set; }
        public int PhysicalScreenHeight { get { return Device.Capability<int>("physical_screen_height"); } }
        public int Columns { get { return Device.Capability<int>("columns"); } }
        public bool DualOrientation { get { return Device.Capability<bool>("dual_orientation"); } }
        public int PhysicalScreenWidth { get { return Device.Capability<int>("physical_screen_width"); } }
        public int Rows { get { return Device.Capability<int>("rows"); } }
        public int MaxImageWidth { get { return Device.Capability<int>("max_image_width"); } }
        public int ResolutionHeight { get { return Device.Capability<int>("resolution_height"); } }
        public int ResolutionWidth { get { return Device.Capability<int>("resolution_width"); } }
        public int MaxImageHeight { get { return Device.Capability<int>("max_image_height"); } }

    }
}