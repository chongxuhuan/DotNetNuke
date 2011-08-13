using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class Transcoding
    {
        public KnownDevice Device { get; set; }
        public bool IsTranscoder { get { return Device.Capability<bool>("is_transcoder"); } }
        public string TranscoderUaHeader { get { return Device.Capability<string>("transcoder_ua_header"); } }

    }
}