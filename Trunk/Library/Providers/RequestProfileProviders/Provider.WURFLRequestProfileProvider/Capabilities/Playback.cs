using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Playback
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public int OmaSizeLimit { get { return this.CapabilityProfile.Capability<int>("playback_oma_size_limit"); } }
        public string AcodecAac { get { return this.CapabilityProfile.Capability<string>("playback_acodec_aac"); } }
        public string VcodecH2633 { get { return this.CapabilityProfile.Capability<string>("playback_vcodec_h263_3"); } }
        public string VcodecMpeg4Asp { get { return this.CapabilityProfile.Capability<string>("playback_vcodec_mpeg4_asp"); } }
        public bool Mp4 { get { return this.CapabilityProfile.Capability<bool>("playback_mp4"); } }
        public bool Playback3Gpp { get { return this.CapabilityProfile.Capability<bool>("playback_3gpp"); } }
        public int DfSizeLimit { get { return this.CapabilityProfile.Capability<int>("playback_df_size_limit"); } }
        public string AcodecAmr { get { return this.CapabilityProfile.Capability<string>("playback_acodec_amr"); } }
        public bool Mov { get { return this.CapabilityProfile.Capability<bool>("playback_mov"); } }
        public string Wmv { get { return this.CapabilityProfile.Capability<string>("playback_wmv"); } }
        public bool AcodecQcelp { get { return this.CapabilityProfile.Capability<bool>("playback_acodec_qcelp"); } }
        public bool ProgressiveDownload { get { return this.CapabilityProfile.Capability<bool>("progressive_download"); } }
        public int DirectdownloadSizeLimit { get { return this.CapabilityProfile.Capability<int>("playback_directdownload_size_limit"); } }
        public string RealMedia { get { return this.CapabilityProfile.Capability<string>("playback_real_media"); } }
        public bool Playback3G2 { get { return this.CapabilityProfile.Capability<bool>("playback_3g2"); } }
        public bool PlaybackFlv { get { return this.CapabilityProfile.Capability<bool>("playback_flv"); } }
        public string VcodecMpeg4Sp { get { return this.CapabilityProfile.Capability<string>("playback_vcodec_mpeg4_sp"); } }
        public string VcodecH2630 { get { return this.CapabilityProfile.Capability<string>("playback_vcodec_h263_0"); } }
        public int InlineSizeLimit { get { return this.CapabilityProfile.Capability<int>("playback_inline_size_limit"); } }
        public bool HintedProgressiveDownload { get { return this.CapabilityProfile.Capability<bool>("hinted_progressive_download"); } }
        public string VcodecH264Bp { get { return this.CapabilityProfile.Capability<string>("playback_vcodec_h264_bp"); } }

    }
}