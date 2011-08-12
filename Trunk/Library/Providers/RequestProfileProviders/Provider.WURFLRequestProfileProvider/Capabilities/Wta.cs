using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Wta
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool NokiaVoiceCall { get { return this.CapabilityProfile.Capability<bool>("nokia_voice_call"); } }
        public bool Pdc { get { return this.CapabilityProfile.Capability<bool>("wta_pdc"); } }
        public bool VoiceCall { get { return this.CapabilityProfile.Capability<bool>("wta_voice_call"); } }
        public bool Misc { get { return this.CapabilityProfile.Capability<bool>("wta_misc"); } }
        public bool Phonebook { get { return this.CapabilityProfile.Capability<bool>("wta_phonebook"); } }

    }
}