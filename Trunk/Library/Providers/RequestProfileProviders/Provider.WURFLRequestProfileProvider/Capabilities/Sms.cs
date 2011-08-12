using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Sms
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool Ems { get { return this.CapabilityProfile.Capability<bool>("ems"); } }
        public bool TextImelody { get { return this.CapabilityProfile.Capability<bool>("text_imelody"); } }
        public bool Nokiaring { get { return this.CapabilityProfile.Capability<bool>("nokiaring"); } }
        public int SiemensLogoHeight { get { return this.CapabilityProfile.Capability<int>("siemens_logo_height"); } }
        public bool EmsVariablesizedpictures { get { return this.CapabilityProfile.Capability<bool>("ems_variablesizedpictures"); } }
        public bool ScklGroupgraphic { get { return this.CapabilityProfile.Capability<bool>("sckl_groupgraphic"); } }
        public bool SiemensOta { get { return this.CapabilityProfile.Capability<bool>("siemens_ota"); } }
        public bool SagemV1 { get { return this.CapabilityProfile.Capability<bool>("sagem_v1"); } }
        public bool Largeoperatorlogo { get { return this.CapabilityProfile.Capability<bool>("largeoperatorlogo"); } }
        public bool SagemV2 { get { return this.CapabilityProfile.Capability<bool>("sagem_v2"); } }
        public int EmsVersion { get { return this.CapabilityProfile.Capability<int>("ems_version"); } }
        public bool EmsOdi { get { return this.CapabilityProfile.Capability<bool>("ems_odi"); } }
        public bool Nokiavcal { get { return this.CapabilityProfile.Capability<bool>("nokiavcal"); } }
        public bool Operatorlogo { get { return this.CapabilityProfile.Capability<bool>("operatorlogo"); } }
        public int SiemensLogoWidth { get { return this.CapabilityProfile.Capability<int>("siemens_logo_width"); } }
        public bool EmsImelody { get { return this.CapabilityProfile.Capability<bool>("ems_imelody"); } }
        public bool ScklVcard { get { return this.CapabilityProfile.Capability<bool>("sckl_vcard"); } }
        public int SiemensScreensaverWidth { get { return this.CapabilityProfile.Capability<int>("siemens_screensaver_width"); } }
        public bool ScklOperatorlogo { get { return this.CapabilityProfile.Capability<bool>("sckl_operatorlogo"); } }
        public bool Panasonic { get { return this.CapabilityProfile.Capability<bool>("panasonic"); } }
        public bool EmsUpi { get { return this.CapabilityProfile.Capability<bool>("ems_upi"); } }
        public bool Nokiavcard { get { return this.CapabilityProfile.Capability<bool>("nokiavcard"); } }
        public bool Callericon { get { return this.CapabilityProfile.Capability<bool>("callericon"); } }
        public bool SmsEnabled { get { return this.CapabilityProfile.Capability<bool>("sms_enabled"); } }
        public bool Gprtf { get { return this.CapabilityProfile.Capability<bool>("gprtf"); } }
        public int SiemensScreensaverHeight { get { return this.CapabilityProfile.Capability<int>("siemens_screensaver_height"); } }
        public bool ScklRingtone { get { return this.CapabilityProfile.Capability<bool>("sckl_ringtone"); } }
        public bool Picturemessage { get { return this.CapabilityProfile.Capability<bool>("picturemessage"); } }
        public bool ScklVcalendar { get { return this.CapabilityProfile.Capability<bool>("sckl_vcalendar"); } }

    }
}