using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class J2me
    {
        public KnownDevice Device { get; set; }
        public bool Doja15 { get { return Device.Capability<bool>("doja_1_5"); } }
        public bool DatefieldBroken { get { return Device.Capability<bool>("j2me_datefield_broken"); } }
        public int ClearKeyCode { get { return Device.Capability<int>("j2me_clear_key_code"); } }
        public int RightSoftkeyCode { get { return Device.Capability<int>("j2me_right_softkey_code"); } }
        public int HeapSize { get { return Device.Capability<int>("j2me_heap_size"); } }
        public int CanvasWidth { get { return Device.Capability<int>("j2me_canvas_width"); } }
        public bool MotorolaLwt { get { return Device.Capability<bool>("j2me_motorola_lwt"); } }
        public bool Doja35 { get { return Device.Capability<bool>("doja_3_5"); } }
        public bool Wbmp { get { return Device.Capability<bool>("j2me_wbmp"); } }
        public bool Rmf { get { return Device.Capability<bool>("j2me_rmf"); } }
        public bool Wma { get { return Device.Capability<bool>("j2me_wma"); } }
        public int LeftSoftkeyCode { get { return Device.Capability<int>("j2me_left_softkey_code"); } }
        public bool Jtwi { get { return Device.Capability<bool>("j2me_jtwi"); } }
        public bool Jpg { get { return Device.Capability<bool>("j2me_jpg"); } }
        public int ReturnKeyCode { get { return Device.Capability<int>("j2me_return_key_code"); } }
        public bool Real8 { get { return Device.Capability<bool>("j2me_real8"); } }
        public int MaxRecordStoreSize { get { return Device.Capability<int>("j2me_max_record_store_size"); } }
        public bool Realmedia { get { return Device.Capability<bool>("j2me_realmedia"); } }
        public bool Midp10 { get { return Device.Capability<bool>("j2me_midp_1_0"); } }
        public bool Bmp3 { get { return Device.Capability<bool>("j2me_bmp3"); } }
        public bool Midi { get { return Device.Capability<bool>("j2me_midi"); } }
        public bool Btapi { get { return Device.Capability<bool>("j2me_btapi"); } }
        public bool Locapi { get { return Device.Capability<bool>("j2me_locapi"); } }
        public bool SiemensExtension { get { return Device.Capability<bool>("j2me_siemens_extension"); } }
        public bool H263 { get { return Device.Capability<bool>("j2me_h263"); } }
        public bool AudioCaptureEnabled { get { return Device.Capability<bool>("j2me_audio_capture_enabled"); } }
        public bool Midp20 { get { return Device.Capability<bool>("j2me_midp_2_0"); } }
        public bool DatefieldNoAcceptsNullDate { get { return Device.Capability<bool>("j2me_datefield_no_accepts_null_date"); } }
        public bool Aac { get { return Device.Capability<bool>("j2me_aac"); } }
        public string CaptureImageFormats { get { return Device.Capability<string>("j2me_capture_image_formats"); } }
        public int SelectKeyCode { get { return Device.Capability<int>("j2me_select_key_code"); } }
        public bool Xmf { get { return Device.Capability<bool>("j2me_xmf"); } }
        public bool PhotoCaptureEnabled { get { return Device.Capability<bool>("j2me_photo_capture_enabled"); } }
        public bool Realaudio { get { return Device.Capability<bool>("j2me_realaudio"); } }
        public bool Realvideo { get { return Device.Capability<bool>("j2me_realvideo"); } }
        public bool Mp3 { get { return Device.Capability<bool>("j2me_mp3"); } }
        public bool Png { get { return Device.Capability<bool>("j2me_png"); } }
        public bool Au { get { return Device.Capability<bool>("j2me_au"); } }
        public int ScreenWidth { get { return Device.Capability<int>("j2me_screen_width"); } }
        public bool Mp4 { get { return Device.Capability<bool>("j2me_mp4"); } }
        public bool Mmapi10 { get { return Device.Capability<bool>("j2me_mmapi_1_0"); } }
        public bool Http { get { return Device.Capability<bool>("j2me_http"); } }
        public bool Imelody { get { return Device.Capability<bool>("j2me_imelody"); } }
        public bool Socket { get { return Device.Capability<bool>("j2me_socket"); } }
        public bool J2me3Dapi { get { return Device.Capability<bool>("j2me_3dapi"); } }
        public int BitsPerPixel { get { return Device.Capability<int>("j2me_bits_per_pixel"); } }
        public bool Mmapi11 { get { return Device.Capability<bool>("j2me_mmapi_1_1"); } }
        public bool Udp { get { return Device.Capability<bool>("j2me_udp"); } }
        public bool Wav { get { return Device.Capability<bool>("j2me_wav"); } }
        public int MiddleSoftkeyCode { get { return Device.Capability<int>("j2me_middle_softkey_code"); } }
        public bool Svgt { get { return Device.Capability<bool>("j2me_svgt"); } }
        public bool Gif { get { return Device.Capability<bool>("j2me_gif"); } }
        public bool SiemensColorGame { get { return Device.Capability<bool>("j2me_siemens_color_game"); } }
        public int MaxJarSize { get { return Device.Capability<int>("j2me_max_jar_size"); } }
        public bool Wmapi10 { get { return Device.Capability<bool>("j2me_wmapi_1_0"); } }
        public bool NokiaUi { get { return Device.Capability<bool>("j2me_nokia_ui"); } }
        public int ScreenHeight { get { return Device.Capability<int>("j2me_screen_height"); } }
        public bool Wmapi11 { get { return Device.Capability<bool>("j2me_wmapi_1_1"); } }
        public bool Wmapi20 { get { return Device.Capability<bool>("j2me_wmapi_2_0"); } }
        public bool Doja10 { get { return Device.Capability<bool>("doja_1_0"); } }
        public bool Serial { get { return Device.Capability<bool>("j2me_serial"); } }
        public bool Doja20 { get { return Device.Capability<bool>("doja_2_0"); } }
        public bool Bmp { get { return Device.Capability<bool>("j2me_bmp"); } }
        public bool Amr { get { return Device.Capability<bool>("j2me_amr"); } }
        public bool Gif89a { get { return Device.Capability<bool>("j2me_gif89a"); } }
        public bool Cldc10 { get { return Device.Capability<bool>("j2me_cldc_1_0"); } }
        public bool Doja21 { get { return Device.Capability<bool>("doja_2_1"); } }
        public bool Doja30 { get { return Device.Capability<bool>("doja_3_0"); } }
        public bool Cldc11 { get { return Device.Capability<bool>("j2me_cldc_1_1"); } }
        public bool Doja22 { get { return Device.Capability<bool>("doja_2_2"); } }
        public bool Doja40 { get { return Device.Capability<bool>("doja_4_0"); } }
        public bool J2me3Gpp { get { return Device.Capability<bool>("j2me_3gpp"); } }
        public bool VideoCaptureEnabled { get { return Device.Capability<bool>("j2me_video_capture_enabled"); } }
        public int CanvasHeight { get { return Device.Capability<int>("j2me_canvas_height"); } }
        public bool Https { get { return Device.Capability<bool>("j2me_https"); } }
        public bool Mpeg4 { get { return Device.Capability<bool>("j2me_mpeg4"); } }
        public int StorageSize { get { return Device.Capability<int>("j2me_storage_size"); } }

    }
}
