using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class Sms
    {
        public KnownDevice Device { get; set; }
        public bool Ems { get { return Device.Capability<bool>("ems"); } }
        public bool TextImelody { get { return Device.Capability<bool>("text_imelody"); } }
        public bool Nokiaring { get { return Device.Capability<bool>("nokiaring"); } }
        public int SiemensLogoHeight { get { return Device.Capability<int>("siemens_logo_height"); } }
        public bool EmsVariablesizedpictures { get { return Device.Capability<bool>("ems_variablesizedpictures"); } }
        public bool ScklGroupgraphic { get { return Device.Capability<bool>("sckl_groupgraphic"); } }
        public bool SiemensOta { get { return Device.Capability<bool>("siemens_ota"); } }
        public bool SagemV1 { get { return Device.Capability<bool>("sagem_v1"); } }
        public bool Largeoperatorlogo { get { return Device.Capability<bool>("largeoperatorlogo"); } }
        public bool SagemV2 { get { return Device.Capability<bool>("sagem_v2"); } }
        public int EmsVersion { get { return Device.Capability<int>("ems_version"); } }
        public bool EmsOdi { get { return Device.Capability<bool>("ems_odi"); } }
        public bool Nokiavcal { get { return Device.Capability<bool>("nokiavcal"); } }
        public bool Operatorlogo { get { return Device.Capability<bool>("operatorlogo"); } }
        public int SiemensLogoWidth { get { return Device.Capability<int>("siemens_logo_width"); } }
        public bool EmsImelody { get { return Device.Capability<bool>("ems_imelody"); } }
        public bool ScklVcard { get { return Device.Capability<bool>("sckl_vcard"); } }
        public int SiemensScreensaverWidth { get { return Device.Capability<int>("siemens_screensaver_width"); } }
        public bool ScklOperatorlogo { get { return Device.Capability<bool>("sckl_operatorlogo"); } }
        public bool Panasonic { get { return Device.Capability<bool>("panasonic"); } }
        public bool EmsUpi { get { return Device.Capability<bool>("ems_upi"); } }
        public bool Nokiavcard { get { return Device.Capability<bool>("nokiavcard"); } }
        public bool Callericon { get { return Device.Capability<bool>("callericon"); } }
        public bool SmsEnabled { get { return Device.Capability<bool>("sms_enabled"); } }
        public bool Gprtf { get { return Device.Capability<bool>("gprtf"); } }
        public int SiemensScreensaverHeight { get { return Device.Capability<int>("siemens_screensaver_height"); } }
        public bool ScklRingtone { get { return Device.Capability<bool>("sckl_ringtone"); } }
        public bool Picturemessage { get { return Device.Capability<bool>("picturemessage"); } }
        public bool ScklVcalendar { get { return Device.Capability<bool>("sckl_vcalendar"); } }

    }
}