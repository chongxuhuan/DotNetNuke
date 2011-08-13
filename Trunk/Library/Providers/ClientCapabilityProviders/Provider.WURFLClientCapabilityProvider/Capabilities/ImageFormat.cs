using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class ImageFormat
    {
        public KnownDevice Device { get; set; }
        public bool Greyscale { get { return Device.Capability<bool>("greyscale"); } }
        public bool Jpg { get { return Device.Capability<bool>("jpg"); } }
        public bool Gif { get { return Device.Capability<bool>("gif"); } }
        public bool TransparentPngIndex { get { return Device.Capability<bool>("transparent_png_index"); } }
        public bool EpocBmp { get { return Device.Capability<bool>("epoc_bmp"); } }
        public bool Bmp { get { return Device.Capability<bool>("bmp"); } }
        public bool Wbmp { get { return Device.Capability<bool>("wbmp"); } }
        public bool GifAnimated { get { return Device.Capability<bool>("gif_animated"); } }
        public int Colors { get { return Device.Capability<int>("colors"); } }
        public bool Svgt11Plus { get { return Device.Capability<bool>("svgt_1_1_plus"); } }
        public bool Svgt11 { get { return Device.Capability<bool>("svgt_1_1"); } }
        public bool TransparentPngAlpha { get { return Device.Capability<bool>("transparent_png_alpha"); } }
        public bool Png { get { return Device.Capability<bool>("png"); } }
        public bool Tiff { get { return Device.Capability<bool>("tiff"); } }

    }
}