using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class FlashLite
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public string  FlashLiteVersion { get { return this.CapabilityProfile.Capability<string>("flash_lite_version"); } }
        public bool  FlWallpaper { get { return this.CapabilityProfile.Capability<bool>("fl_wallpaper"); } }
        public bool  FlBrowser { get { return this.CapabilityProfile.Capability<bool>("fl_browser"); } }
        public bool  FlScreensaver { get { return this.CapabilityProfile.Capability<bool>("fl_screensaver"); } }
        public bool  FlStandalone { get { return this.CapabilityProfile.Capability<bool>("fl_standalone"); } }
        public bool  FullFlashSupport { get { return this.CapabilityProfile.Capability<bool>("full_flash_support"); } }
        public bool  FlSubLcd { get { return this.CapabilityProfile.Capability<bool>("fl_sub_lcd"); } }

    }
}