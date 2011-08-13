using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class FlashLite
    {
        public KnownDevice Device { get; set; }
        public string  FlashLiteVersion { get { return Device.Capability<string>("flash_lite_version"); } }
        public bool  FlWallpaper { get { return Device.Capability<bool>("fl_wallpaper"); } }
        public bool  FlBrowser { get { return Device.Capability<bool>("fl_browser"); } }
        public bool  FlScreensaver { get { return Device.Capability<bool>("fl_screensaver"); } }
        public bool  FlStandalone { get { return Device.Capability<bool>("fl_standalone"); } }
        public bool  FullFlashSupport { get { return Device.Capability<bool>("full_flash_support"); } }
        public bool  FlSubLcd { get { return Device.Capability<bool>("fl_sub_lcd"); } }

    }
}