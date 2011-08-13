#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

#region Usings
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace DotNetNuke.Services.ClientCapability
{
    public class KnownDevice : IDevice
    {
        public KnownDevice(IDevice Device)
        {
            this._Device = Device;
        }

        public bool IsMobileDevice { get { return !string.IsNullOrEmpty(Capability<string>("mobile_browser")); } }

        private Dictionary<string, string> Caps = new Dictionary<string, string>();
        public T Capability<T>(string Name)
        {
            string ret = "";
            if (Caps.ContainsKey(Name))
                ret = Caps[Name];
            else
            {
                ret = _Device.GetCapability(Name);
                Caps.Add(Name, ret);
            }

            return (T)Convert.ChangeType(ret, typeof(T));
        }

        public ProductInfo ProductInfo { get { return new ProductInfo() { Device = this }; } }
        public WmlUi WmlUi { get { return new WmlUi() { Device = this }; } }
        public ChtmlUi ChtmlUi { get { return new ChtmlUi() { Device = this }; } }
        public XhtmlUi XhtmlUi { get { return new XhtmlUi() { Device = this }; } }
        public Ajax Ajax { get { return new Ajax() { Device = this }; } }
        public Markup Markup { get { return new Markup() { Device = this }; } }
        public Cache Cache { get { return new Cache() { Device = this }; } }
        public Display Display { get { return new Display() { Device = this }; } }
        public ImageFormat ImageFormat { get { return new ImageFormat() { Device = this }; } }
        public Bugs Bugs { get { return new Bugs() { Device = this }; } }
        public Wta Wta { get { return new Wta() { Device = this }; } }
        public Security Security { get { return new Security() { Device = this }; } }
        public Bearer Bearer { get { return new Bearer() { Device = this }; } }
        public Storage Storage { get { return new Storage() { Device = this }; } }
        public ObjectDownload ObjectDownload { get { return new ObjectDownload() { Device = this }; } }
        public Drm Drm { get { return new Drm() { Device = this }; } }
        public Streaming Streaming { get { return new Streaming() { Device = this }; } }
        public WapPush WapPush { get { return new WapPush() { Device = this }; } }
        public J2me J2me { get { return new J2me() { Device = this }; } }
        public Mms Mms { get { return new Mms() { Device = this }; } }
        public Sms Sms { get { return new Sms() { Device = this }; } }
        public SoundFormat SoundFormat { get { return new SoundFormat() { Device = this }; } }
        public FlashLite FlashLite { get { return new FlashLite() { Device = this }; } }
        public Css Css { get { return new Css() { Device = this }; } }
        public Transcoding Transcoding { get { return new Transcoding() { Device = this }; } }
        public Rss Rss { get { return new Rss() { Device = this }; } }
        public Pdf Pdf { get { return new Pdf() { Device = this }; } }
        public Playback Playback { get { return new Playback() { Device = this }; } }
        public HtmlUi HtmlUi { get { return new HtmlUi() { Device = this }; } }



        private IDevice _Device { get; set; }

        public string ID
        {
            get { return _Device.ID; }
        }

        public string UserAgent
        {
            get { return _Device.UserAgent; }
        }

        public string FallBack
        {
            get { return _Device.UserAgent; }
        }

        public bool IsActualDeviceRoot
        {
            get { return _Device.IsActualDeviceRoot; }
        }


        public IDictionary<string, string> Capabilities
        {
            get { return _Device.Capabilities; }
        }

        public MarkUp MarkUp
        {
            get { return _Device.MarkUp; }
        }

        public string GetCapability(string capabilityName)
        {
            return this.Capability<string>(capabilityName);
        }
    }
    
}
