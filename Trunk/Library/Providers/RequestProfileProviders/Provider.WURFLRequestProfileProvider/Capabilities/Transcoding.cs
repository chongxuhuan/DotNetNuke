using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Transcoding
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool IsTranscoder { get { return this.CapabilityProfile.Capability<bool>("is_transcoder"); } }
        public string TranscoderUaHeader { get { return this.CapabilityProfile.Capability<string>("transcoder_ua_header"); } }

    }
}