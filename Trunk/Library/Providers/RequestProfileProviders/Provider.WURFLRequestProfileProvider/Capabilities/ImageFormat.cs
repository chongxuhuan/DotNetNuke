using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class ImageFormat
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool Greyscale { get { return this.CapabilityProfile.Capability<bool>("greyscale"); } }
        public bool Jpg { get { return this.CapabilityProfile.Capability<bool>("jpg"); } }
        public bool Gif { get { return this.CapabilityProfile.Capability<bool>("gif"); } }
        public bool TransparentPngIndex { get { return this.CapabilityProfile.Capability<bool>("transparent_png_index"); } }
        public bool EpocBmp { get { return this.CapabilityProfile.Capability<bool>("epoc_bmp"); } }
        public bool Bmp { get { return this.CapabilityProfile.Capability<bool>("bmp"); } }
        public bool Wbmp { get { return this.CapabilityProfile.Capability<bool>("wbmp"); } }
        public bool GifAnimated { get { return this.CapabilityProfile.Capability<bool>("gif_animated"); } }
        public int Colors { get { return this.CapabilityProfile.Capability<int>("colors"); } }
        public bool Svgt11Plus { get { return this.CapabilityProfile.Capability<bool>("svgt_1_1_plus"); } }
        public bool Svgt11 { get { return this.CapabilityProfile.Capability<bool>("svgt_1_1"); } }
        public bool TransparentPngAlpha { get { return this.CapabilityProfile.Capability<bool>("transparent_png_alpha"); } }
        public bool Png { get { return this.CapabilityProfile.Capability<bool>("png"); } }
        public bool Tiff { get { return this.CapabilityProfile.Capability<bool>("tiff"); } }

    }
}