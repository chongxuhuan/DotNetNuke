using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Mms
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool Mms3Gpp { get { return this.CapabilityProfile.Capability<bool>("mms_3gpp"); } }
        public bool Wbxml { get { return this.CapabilityProfile.Capability<bool>("mms_wbxml"); } }
        public bool SymbianInstall { get { return this.CapabilityProfile.Capability<bool>("mms_symbian_install"); } }
        public bool Png { get { return this.CapabilityProfile.Capability<bool>("mms_png"); } }
        public bool MaxSize { get { return this.CapabilityProfile.Capability<bool>("mms_max_size"); } }
        public bool Rmf { get { return this.CapabilityProfile.Capability<bool>("mms_rmf"); } }
        public bool NokiaOperatorlogo { get { return this.CapabilityProfile.Capability<bool>("mms_nokia_operatorlogo"); } }
        public int MaxWidth { get { return this.CapabilityProfile.Capability<int>("mms_max_width"); } }
        public int MaxFrameRate { get { return this.CapabilityProfile.Capability<int>("mms_max_frame_rate"); } }
        public bool Wml { get { return this.CapabilityProfile.Capability<bool>("mms_wml"); } }
        public bool Evrc { get { return this.CapabilityProfile.Capability<bool>("mms_evrc"); } }
        public bool Spmidi { get { return this.CapabilityProfile.Capability<bool>("mms_spmidi"); } }
        public bool GifStatic { get { return this.CapabilityProfile.Capability<bool>("mms_gif_static"); } }
        public int MaxHeight { get { return this.CapabilityProfile.Capability<int>("mms_max_height"); } }
        public bool Sender { get { return this.CapabilityProfile.Capability<bool>("sender"); } }
        public bool Video { get { return this.CapabilityProfile.Capability<bool>("mms_video"); } }
        public bool Vcard { get { return this.CapabilityProfile.Capability<bool>("mms_vcard"); } }
        public bool Nokia3Dscreensaver { get { return this.CapabilityProfile.Capability<bool>("mms_nokia_3dscreensaver"); } }
        public bool Qcelp { get { return this.CapabilityProfile.Capability<bool>("mms_qcelp"); } }
        public bool MidiPolyphonic { get { return this.CapabilityProfile.Capability<bool>("mms_midi_polyphonic"); } }
        public bool Wav { get { return this.CapabilityProfile.Capability<bool>("mms_wav"); } }
        public bool JpegProgressive { get { return this.CapabilityProfile.Capability<bool>("mms_jpeg_progressive"); } }
        public bool Jad { get { return this.CapabilityProfile.Capability<bool>("mms_jad"); } }
        public bool NokiaRingingtone { get { return this.CapabilityProfile.Capability<bool>("mms_nokia_ringingtone"); } }
        public bool BuiltInRecorder { get { return this.CapabilityProfile.Capability<bool>("built_in_recorder"); } }
        public bool MidiMonophonic { get { return this.CapabilityProfile.Capability<bool>("mms_midi_monophonic"); } }
        public bool Mms3Gpp2 { get { return this.CapabilityProfile.Capability<bool>("mms_3gpp2"); } }
        public bool Wmlc { get { return this.CapabilityProfile.Capability<bool>("mms_wmlc"); } }
        public bool NokiaWallpaper { get { return this.CapabilityProfile.Capability<bool>("mms_nokia_wallpaper"); } }
        public bool Bmp { get { return this.CapabilityProfile.Capability<bool>("mms_bmp"); } }
        public bool Vcalendar { get { return this.CapabilityProfile.Capability<bool>("mms_vcalendar"); } }
        public bool Jar { get { return this.CapabilityProfile.Capability<bool>("mms_jar"); } }
        public bool OtaBitmap { get { return this.CapabilityProfile.Capability<bool>("mms_ota_bitmap"); } }
        public bool Mp3 { get { return this.CapabilityProfile.Capability<bool>("mms_mp3"); } }
        public bool Mmf { get { return this.CapabilityProfile.Capability<bool>("mms_mmf"); } }
        public bool Amr { get { return this.CapabilityProfile.Capability<bool>("mms_amr"); } }
        public bool Wbmp { get { return this.CapabilityProfile.Capability<bool>("mms_wbmp"); } }
        public bool BuiltInCamera { get { return this.CapabilityProfile.Capability<bool>("built_in_camera"); } }
        public bool Receiver { get { return this.CapabilityProfile.Capability<bool>("receiver"); } }
        public bool Mp4 { get { return this.CapabilityProfile.Capability<bool>("mms_mp4"); } }
        public bool Xmf { get { return this.CapabilityProfile.Capability<bool>("mms_xmf"); } }
        public bool JpegBaseline { get { return this.CapabilityProfile.Capability<bool>("mms_jpeg_baseline"); } }
        public int MidiPolyphonicVoices { get { return this.CapabilityProfile.Capability<int>("mms_midi_polyphonic_voices"); } }
        public bool GifAnimated { get { return this.CapabilityProfile.Capability<bool>("mms_gif_animated"); } }

    }
}