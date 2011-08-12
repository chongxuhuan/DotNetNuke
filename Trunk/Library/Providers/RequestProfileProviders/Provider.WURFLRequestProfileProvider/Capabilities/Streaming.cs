using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Streaming
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public string VcodecMpeg4Asp { get { return this.CapabilityProfile.Capability<string>("streaming_vcodec_mpeg4_asp"); } }
        public int VideoSizeLimit { get { return this.CapabilityProfile.Capability<int>("streaming_video_size_limit"); } }
        public bool Mov { get { return this.CapabilityProfile.Capability<bool>("streaming_mov"); } }
        public bool Wmv { get { return this.CapabilityProfile.Capability<bool>("streaming_wmv"); } }
        public string AcodecAac { get { return this.CapabilityProfile.Capability<string>("streaming_acodec_aac"); } }
        public string VcodecH2630 { get { return this.CapabilityProfile.Capability<string>("streaming_vcodec_h263_0"); } }
        public string RealMedia { get { return this.CapabilityProfile.Capability<string>("streaming_real_media"); } }
        public bool Streaming3G2 { get { return this.CapabilityProfile.Capability<bool>("streaming_3g2"); } }
        public bool Streaming3Gpp { get { return this.CapabilityProfile.Capability<bool>("streaming_3gpp"); } }
        public string AcodecAmr { get { return this.CapabilityProfile.Capability<string>("streaming_acodec_amr"); } }
        public string VcodecH264Bp { get { return this.CapabilityProfile.Capability<string>("streaming_vcodec_h264_bp"); } }
        public string VcodecH2633 { get { return this.CapabilityProfile.Capability<string>("streaming_vcodec_h263_3"); } }
        public string PreferredProtocol { get { return this.CapabilityProfile.Capability<string>("streaming_preferred_protocol"); } }
        public string VcodecMpeg4Sp { get { return this.CapabilityProfile.Capability<string>("streaming_vcodec_mpeg4_sp"); } }
        public bool Flv { get { return this.CapabilityProfile.Capability<bool>("streaming_flv"); } }
        public bool Video { get { return this.CapabilityProfile.Capability<bool>("streaming_video"); } }
        public bool Mp4 { get { return this.CapabilityProfile.Capability<bool>("streaming_mp4"); } }

    }
}