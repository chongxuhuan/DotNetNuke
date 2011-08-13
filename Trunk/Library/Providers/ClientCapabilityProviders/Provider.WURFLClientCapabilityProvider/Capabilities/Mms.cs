using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class Mms
    {
        public KnownDevice Device { get; set; }
        public bool Mms3Gpp { get { return Device.Capability<bool>("mms_3gpp"); } }
        public bool Wbxml { get { return Device.Capability<bool>("mms_wbxml"); } }
        public bool SymbianInstall { get { return Device.Capability<bool>("mms_symbian_install"); } }
        public bool Png { get { return Device.Capability<bool>("mms_png"); } }
        public bool MaxSize { get { return Device.Capability<bool>("mms_max_size"); } }
        public bool Rmf { get { return Device.Capability<bool>("mms_rmf"); } }
        public bool NokiaOperatorlogo { get { return Device.Capability<bool>("mms_nokia_operatorlogo"); } }
        public int MaxWidth { get { return Device.Capability<int>("mms_max_width"); } }
        public int MaxFrameRate { get { return Device.Capability<int>("mms_max_frame_rate"); } }
        public bool Wml { get { return Device.Capability<bool>("mms_wml"); } }
        public bool Evrc { get { return Device.Capability<bool>("mms_evrc"); } }
        public bool Spmidi { get { return Device.Capability<bool>("mms_spmidi"); } }
        public bool GifStatic { get { return Device.Capability<bool>("mms_gif_static"); } }
        public int MaxHeight { get { return Device.Capability<int>("mms_max_height"); } }
        public bool Sender { get { return Device.Capability<bool>("sender"); } }
        public bool Video { get { return Device.Capability<bool>("mms_video"); } }
        public bool Vcard { get { return Device.Capability<bool>("mms_vcard"); } }
        public bool Nokia3Dscreensaver { get { return Device.Capability<bool>("mms_nokia_3dscreensaver"); } }
        public bool Qcelp { get { return Device.Capability<bool>("mms_qcelp"); } }
        public bool MidiPolyphonic { get { return Device.Capability<bool>("mms_midi_polyphonic"); } }
        public bool Wav { get { return Device.Capability<bool>("mms_wav"); } }
        public bool JpegProgressive { get { return Device.Capability<bool>("mms_jpeg_progressive"); } }
        public bool Jad { get { return Device.Capability<bool>("mms_jad"); } }
        public bool NokiaRingingtone { get { return Device.Capability<bool>("mms_nokia_ringingtone"); } }
        public bool BuiltInRecorder { get { return Device.Capability<bool>("built_in_recorder"); } }
        public bool MidiMonophonic { get { return Device.Capability<bool>("mms_midi_monophonic"); } }
        public bool Mms3Gpp2 { get { return Device.Capability<bool>("mms_3gpp2"); } }
        public bool Wmlc { get { return Device.Capability<bool>("mms_wmlc"); } }
        public bool NokiaWallpaper { get { return Device.Capability<bool>("mms_nokia_wallpaper"); } }
        public bool Bmp { get { return Device.Capability<bool>("mms_bmp"); } }
        public bool Vcalendar { get { return Device.Capability<bool>("mms_vcalendar"); } }
        public bool Jar { get { return Device.Capability<bool>("mms_jar"); } }
        public bool OtaBitmap { get { return Device.Capability<bool>("mms_ota_bitmap"); } }
        public bool Mp3 { get { return Device.Capability<bool>("mms_mp3"); } }
        public bool Mmf { get { return Device.Capability<bool>("mms_mmf"); } }
        public bool Amr { get { return Device.Capability<bool>("mms_amr"); } }
        public bool Wbmp { get { return Device.Capability<bool>("mms_wbmp"); } }
        public bool BuiltInCamera { get { return Device.Capability<bool>("built_in_camera"); } }
        public bool Receiver { get { return Device.Capability<bool>("receiver"); } }
        public bool Mp4 { get { return Device.Capability<bool>("mms_mp4"); } }
        public bool Xmf { get { return Device.Capability<bool>("mms_xmf"); } }
        public bool JpegBaseline { get { return Device.Capability<bool>("mms_jpeg_baseline"); } }
        public int MidiPolyphonicVoices { get { return Device.Capability<int>("mms_midi_polyphonic_voices"); } }
        public bool GifAnimated { get { return Device.Capability<bool>("mms_gif_animated"); } }

    }
}