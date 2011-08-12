using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.CapabilityProfiles.Core.Capabilities
{
    public class ObjectDownload
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool Video { get { return this.CapabilityProfile.Capability<bool>("video"); } }
        public bool PictureBmp { get { return this.CapabilityProfile.Capability<bool>("picture_bmp"); } }
        public bool Picture { get { return this.CapabilityProfile.Capability<bool>("picture"); } }
        public int WallpaperDfSizeLimit { get { return this.CapabilityProfile.Capability<int>("wallpaper_df_size_limit"); } }
        public int PicturePreferredWidth { get { return this.CapabilityProfile.Capability<int>("picture_preferred_width"); } }
        public int WallpaperOmaSizeLimit { get { return this.CapabilityProfile.Capability<int>("wallpaper_oma_size_limit"); } }
        public bool PictureGreyscale { get { return this.CapabilityProfile.Capability<bool>("picture_greyscale"); } }
        public bool InlineSupport { get { return this.CapabilityProfile.Capability<bool>("inline_support"); } }
        public bool RingtoneQcelp { get { return this.CapabilityProfile.Capability<bool>("ringtone_qcelp"); } }
        public int ScreensaverOmaSizeLimit { get { return this.CapabilityProfile.Capability<int>("screensaver_oma_size_limit"); } }
        public bool ScreensaverWbmp { get { return this.CapabilityProfile.Capability<bool>("screensaver_wbmp"); } }
        public bool PictureResize { get { return this.CapabilityProfile.Capability<bool>("picture_resize"); } }
        public int PicturePreferredHeight { get { return this.CapabilityProfile.Capability<int>("picture_preferred_height"); } }
        public bool RingtoneRmf { get { return this.CapabilityProfile.Capability<bool>("ringtone_rmf"); } }
        public bool WallpaperWbmp { get { return this.CapabilityProfile.Capability<bool>("wallpaper_wbmp"); } }
        public bool WallpaperJpg { get { return this.CapabilityProfile.Capability<bool>("wallpaper_jpg"); } }
        public bool ScreensaverBmp { get { return this.CapabilityProfile.Capability<bool>("screensaver_bmp"); } }
        public int ScreensaverMaxWidth { get { return this.CapabilityProfile.Capability<int>("screensaver_max_width"); } }
        public int PictureInlineSizeLimit { get { return this.CapabilityProfile.Capability<int>("picture_inline_size_limit"); } }
        public int PictureColors { get { return this.CapabilityProfile.Capability<int>("picture_colors"); } }
        public bool RingtoneMidiPolyphonic { get { return this.CapabilityProfile.Capability<bool>("ringtone_midi_polyphonic"); } }
        public bool RingtoneMidiMonophonic { get { return this.CapabilityProfile.Capability<bool>("ringtone_midi_monophonic"); } }
        public int ScreensaverPreferredHeight { get { return this.CapabilityProfile.Capability<int>("screensaver_preferred_height"); } }
        public int RingtoneVoices { get { return this.CapabilityProfile.Capability<int>("ringtone_voices"); } }
        public bool Ringtone3Gpp { get { return this.CapabilityProfile.Capability<bool>("ringtone_3gpp"); } }
        public bool OmaSupport { get { return this.CapabilityProfile.Capability<bool>("oma_support"); } }
        public int RingtoneInlineSizeLimit { get { return this.CapabilityProfile.Capability<int>("ringtone_inline_size_limit"); } }
        public int WallpaperPreferredWidth { get { return this.CapabilityProfile.Capability<int>("wallpaper_preferred_width"); } }
        public bool WallpaperGreyscale { get { return this.CapabilityProfile.Capability<bool>("wallpaper_greyscale"); } }
        public int ScreensaverPreferredWidth { get { return this.CapabilityProfile.Capability<int>("screensaver_preferred_width"); } }
        public int WallpaperPreferredHeight { get { return this.CapabilityProfile.Capability<int>("wallpaper_preferred_height"); } }
        public int PictureMaxWidth { get { return this.CapabilityProfile.Capability<int>("picture_max_width"); } }
        public bool PictureJpg { get { return this.CapabilityProfile.Capability<bool>("picture_jpg"); } }
        public bool RingtoneAac { get { return this.CapabilityProfile.Capability<bool>("ringtone_aac"); } }
        public int RingtoneOmaSizeLimit { get { return this.CapabilityProfile.Capability<int>("ringtone_oma_size_limit"); } }
        public int WallpaperDirectdownloadSizeLimit { get { return this.CapabilityProfile.Capability<int>("wallpaper_directdownload_size_limit"); } }
        public int ScreensaverInlineSizeLimit { get { return this.CapabilityProfile.Capability<int>("screensaver_inline_size_limit"); } }
        public bool RingtoneXmf { get { return this.CapabilityProfile.Capability<bool>("ringtone_xmf"); } }
        public int PictureMaxHeight { get { return this.CapabilityProfile.Capability<int>("picture_max_height"); } }
        public int ScreensaverMaxHeight { get { return this.CapabilityProfile.Capability<int>("screensaver_max_height"); } }
        public bool RingtoneMp3 { get { return this.CapabilityProfile.Capability<bool>("ringtone_mp3"); } }
        public bool WallpaperPng { get { return this.CapabilityProfile.Capability<bool>("wallpaper_png"); } }
        public bool ScreensaverJpg { get { return this.CapabilityProfile.Capability<bool>("screensaver_jpg"); } }
        public int RingtoneDirectdownloadSizeLimit { get { return this.CapabilityProfile.Capability<int>("ringtone_directdownload_size_limit"); } }
        public int WallpaperMaxWidth { get { return this.CapabilityProfile.Capability<int>("wallpaper_max_width"); } }
        public int WallpaperMaxHeight { get { return this.CapabilityProfile.Capability<int>("wallpaper_max_height"); } }
        public bool Screensaver { get { return this.CapabilityProfile.Capability<bool>("screensaver"); } }
        public bool RingtoneWav { get { return this.CapabilityProfile.Capability<bool>("ringtone_wav"); } }
        public bool WallpaperGif { get { return this.CapabilityProfile.Capability<bool>("wallpaper_gif"); } }
        public int ScreensaverDirectdownloadSizeLimit { get { return this.CapabilityProfile.Capability<int>("screensaver_directdownload_size_limit"); } }
        public int PictureDfSizeLimit { get { return this.CapabilityProfile.Capability<int>("picture_df_size_limit"); } }
        public bool WallpaperTiff { get { return this.CapabilityProfile.Capability<bool>("wallpaper_tiff"); } }
        public int ScreensaverDfSizeLimit { get { return this.CapabilityProfile.Capability<int>("screensaver_df_size_limit"); } }
        public bool RingtoneAwb { get { return this.CapabilityProfile.Capability<bool>("ringtone_awb"); } }
        public bool Ringtone { get { return this.CapabilityProfile.Capability<bool>("ringtone"); } }
        public int WallpaperInlineSizeLimit { get { return this.CapabilityProfile.Capability<int>("wallpaper_inline_size_limit"); } }
        public int PictureDirectdownloadSizeLimit { get { return this.CapabilityProfile.Capability<int>("picture_directdownload_size_limit"); } }
        public bool PicturePng { get { return this.CapabilityProfile.Capability<bool>("picture_png"); } }
        public bool WallpaperBmp { get { return this.CapabilityProfile.Capability<bool>("wallpaper_bmp"); } }
        public bool PictureWbmp { get { return this.CapabilityProfile.Capability<bool>("picture_wbmp"); } }
        public int RingtoneDfSizeLimit { get { return this.CapabilityProfile.Capability<int>("ringtone_df_size_limit"); } }
        public int PictureOmaSizeLimit { get { return this.CapabilityProfile.Capability<int>("picture_oma_size_limit"); } }
        public bool PictureGif { get { return this.CapabilityProfile.Capability<bool>("picture_gif"); } }
        public bool ScreensaverPng { get { return this.CapabilityProfile.Capability<bool>("screensaver_png"); } }
        public bool WallpaperResize { get { return this.CapabilityProfile.Capability<bool>("wallpaper_resize"); } }
        public bool ScreensaverGreyscale { get { return this.CapabilityProfile.Capability<bool>("screensaver_greyscale"); } }
        public bool RingtoneMmf { get { return this.CapabilityProfile.Capability<bool>("ringtone_mmf"); } }
        public bool RingtoneAmr { get { return this.CapabilityProfile.Capability<bool>("ringtone_amr"); } }
        public bool Wallpaper { get { return this.CapabilityProfile.Capability<bool>("wallpaper"); } }
        public bool RingtoneDigiplug { get { return this.CapabilityProfile.Capability<bool>("ringtone_digiplug"); } }
        public bool RingtoneSpmidi { get { return this.CapabilityProfile.Capability<bool>("ringtone_spmidi"); } }
        public bool RingtoneCompactmidi { get { return this.CapabilityProfile.Capability<bool>("ringtone_compactmidi"); } }
        public bool RingtoneImelody { get { return this.CapabilityProfile.Capability<bool>("ringtone_imelody"); } }
        public string ScreensaverResize { get { return this.CapabilityProfile.Capability<string>("screensaver_resize"); } }
        public int WallpaperColors { get { return this.CapabilityProfile.Capability<int>("wallpaper_colors"); } }
        public bool DirectdownloadSupport { get { return this.CapabilityProfile.Capability<bool>("directdownload_support"); } }
        public bool DownloadfunSupport { get { return this.CapabilityProfile.Capability<bool>("downloadfun_support"); } }
        public int ScreensaverColors { get { return this.CapabilityProfile.Capability<int>("screensaver_colors"); } }
        public bool ScreensaverGif { get { return this.CapabilityProfile.Capability<bool>("screensaver_gif"); } }

    }
}