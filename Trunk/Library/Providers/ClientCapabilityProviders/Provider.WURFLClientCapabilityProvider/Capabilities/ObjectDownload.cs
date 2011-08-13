using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class ObjectDownload
    {
        public KnownDevice Device { get; set; }
        public bool Video { get { return Device.Capability<bool>("video"); } }
        public bool PictureBmp { get { return Device.Capability<bool>("picture_bmp"); } }
        public bool Picture { get { return Device.Capability<bool>("picture"); } }
        public int WallpaperDfSizeLimit { get { return Device.Capability<int>("wallpaper_df_size_limit"); } }
        public int PicturePreferredWidth { get { return Device.Capability<int>("picture_preferred_width"); } }
        public int WallpaperOmaSizeLimit { get { return Device.Capability<int>("wallpaper_oma_size_limit"); } }
        public bool PictureGreyscale { get { return Device.Capability<bool>("picture_greyscale"); } }
        public bool InlineSupport { get { return Device.Capability<bool>("inline_support"); } }
        public bool RingtoneQcelp { get { return Device.Capability<bool>("ringtone_qcelp"); } }
        public int ScreensaverOmaSizeLimit { get { return Device.Capability<int>("screensaver_oma_size_limit"); } }
        public bool ScreensaverWbmp { get { return Device.Capability<bool>("screensaver_wbmp"); } }
        public bool PictureResize { get { return Device.Capability<bool>("picture_resize"); } }
        public int PicturePreferredHeight { get { return Device.Capability<int>("picture_preferred_height"); } }
        public bool RingtoneRmf { get { return Device.Capability<bool>("ringtone_rmf"); } }
        public bool WallpaperWbmp { get { return Device.Capability<bool>("wallpaper_wbmp"); } }
        public bool WallpaperJpg { get { return Device.Capability<bool>("wallpaper_jpg"); } }
        public bool ScreensaverBmp { get { return Device.Capability<bool>("screensaver_bmp"); } }
        public int ScreensaverMaxWidth { get { return Device.Capability<int>("screensaver_max_width"); } }
        public int PictureInlineSizeLimit { get { return Device.Capability<int>("picture_inline_size_limit"); } }
        public int PictureColors { get { return Device.Capability<int>("picture_colors"); } }
        public bool RingtoneMidiPolyphonic { get { return Device.Capability<bool>("ringtone_midi_polyphonic"); } }
        public bool RingtoneMidiMonophonic { get { return Device.Capability<bool>("ringtone_midi_monophonic"); } }
        public int ScreensaverPreferredHeight { get { return Device.Capability<int>("screensaver_preferred_height"); } }
        public int RingtoneVoices { get { return Device.Capability<int>("ringtone_voices"); } }
        public bool Ringtone3Gpp { get { return Device.Capability<bool>("ringtone_3gpp"); } }
        public bool OmaSupport { get { return Device.Capability<bool>("oma_support"); } }
        public int RingtoneInlineSizeLimit { get { return Device.Capability<int>("ringtone_inline_size_limit"); } }
        public int WallpaperPreferredWidth { get { return Device.Capability<int>("wallpaper_preferred_width"); } }
        public bool WallpaperGreyscale { get { return Device.Capability<bool>("wallpaper_greyscale"); } }
        public int ScreensaverPreferredWidth { get { return Device.Capability<int>("screensaver_preferred_width"); } }
        public int WallpaperPreferredHeight { get { return Device.Capability<int>("wallpaper_preferred_height"); } }
        public int PictureMaxWidth { get { return Device.Capability<int>("picture_max_width"); } }
        public bool PictureJpg { get { return Device.Capability<bool>("picture_jpg"); } }
        public bool RingtoneAac { get { return Device.Capability<bool>("ringtone_aac"); } }
        public int RingtoneOmaSizeLimit { get { return Device.Capability<int>("ringtone_oma_size_limit"); } }
        public int WallpaperDirectdownloadSizeLimit { get { return Device.Capability<int>("wallpaper_directdownload_size_limit"); } }
        public int ScreensaverInlineSizeLimit { get { return Device.Capability<int>("screensaver_inline_size_limit"); } }
        public bool RingtoneXmf { get { return Device.Capability<bool>("ringtone_xmf"); } }
        public int PictureMaxHeight { get { return Device.Capability<int>("picture_max_height"); } }
        public int ScreensaverMaxHeight { get { return Device.Capability<int>("screensaver_max_height"); } }
        public bool RingtoneMp3 { get { return Device.Capability<bool>("ringtone_mp3"); } }
        public bool WallpaperPng { get { return Device.Capability<bool>("wallpaper_png"); } }
        public bool ScreensaverJpg { get { return Device.Capability<bool>("screensaver_jpg"); } }
        public int RingtoneDirectdownloadSizeLimit { get { return Device.Capability<int>("ringtone_directdownload_size_limit"); } }
        public int WallpaperMaxWidth { get { return Device.Capability<int>("wallpaper_max_width"); } }
        public int WallpaperMaxHeight { get { return Device.Capability<int>("wallpaper_max_height"); } }
        public bool Screensaver { get { return Device.Capability<bool>("screensaver"); } }
        public bool RingtoneWav { get { return Device.Capability<bool>("ringtone_wav"); } }
        public bool WallpaperGif { get { return Device.Capability<bool>("wallpaper_gif"); } }
        public int ScreensaverDirectdownloadSizeLimit { get { return Device.Capability<int>("screensaver_directdownload_size_limit"); } }
        public int PictureDfSizeLimit { get { return Device.Capability<int>("picture_df_size_limit"); } }
        public bool WallpaperTiff { get { return Device.Capability<bool>("wallpaper_tiff"); } }
        public int ScreensaverDfSizeLimit { get { return Device.Capability<int>("screensaver_df_size_limit"); } }
        public bool RingtoneAwb { get { return Device.Capability<bool>("ringtone_awb"); } }
        public bool Ringtone { get { return Device.Capability<bool>("ringtone"); } }
        public int WallpaperInlineSizeLimit { get { return Device.Capability<int>("wallpaper_inline_size_limit"); } }
        public int PictureDirectdownloadSizeLimit { get { return Device.Capability<int>("picture_directdownload_size_limit"); } }
        public bool PicturePng { get { return Device.Capability<bool>("picture_png"); } }
        public bool WallpaperBmp { get { return Device.Capability<bool>("wallpaper_bmp"); } }
        public bool PictureWbmp { get { return Device.Capability<bool>("picture_wbmp"); } }
        public int RingtoneDfSizeLimit { get { return Device.Capability<int>("ringtone_df_size_limit"); } }
        public int PictureOmaSizeLimit { get { return Device.Capability<int>("picture_oma_size_limit"); } }
        public bool PictureGif { get { return Device.Capability<bool>("picture_gif"); } }
        public bool ScreensaverPng { get { return Device.Capability<bool>("screensaver_png"); } }
        public bool WallpaperResize { get { return Device.Capability<bool>("wallpaper_resize"); } }
        public bool ScreensaverGreyscale { get { return Device.Capability<bool>("screensaver_greyscale"); } }
        public bool RingtoneMmf { get { return Device.Capability<bool>("ringtone_mmf"); } }
        public bool RingtoneAmr { get { return Device.Capability<bool>("ringtone_amr"); } }
        public bool Wallpaper { get { return Device.Capability<bool>("wallpaper"); } }
        public bool RingtoneDigiplug { get { return Device.Capability<bool>("ringtone_digiplug"); } }
        public bool RingtoneSpmidi { get { return Device.Capability<bool>("ringtone_spmidi"); } }
        public bool RingtoneCompactmidi { get { return Device.Capability<bool>("ringtone_compactmidi"); } }
        public bool RingtoneImelody { get { return Device.Capability<bool>("ringtone_imelody"); } }
        public string ScreensaverResize { get { return Device.Capability<string>("screensaver_resize"); } }
        public int WallpaperColors { get { return Device.Capability<int>("wallpaper_colors"); } }
        public bool DirectdownloadSupport { get { return Device.Capability<bool>("directdownload_support"); } }
        public bool DownloadfunSupport { get { return Device.Capability<bool>("downloadfun_support"); } }
        public int ScreensaverColors { get { return Device.Capability<int>("screensaver_colors"); } }
        public bool ScreensaverGif { get { return Device.Capability<bool>("screensaver_gif"); } }

    }
}