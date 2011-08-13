using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace DotNetNuke.Services.ClientCapability
{
    public class Wta
    {
        public KnownDevice Device { get; set; }
        public bool NokiaVoiceCall { get { return Device.Capability<bool>("nokia_voice_call"); } }
        public bool Pdc { get { return Device.Capability<bool>("wta_pdc"); } }
        public bool VoiceCall { get { return Device.Capability<bool>("wta_voice_call"); } }
        public bool Misc { get { return Device.Capability<bool>("wta_misc"); } }
        public bool Phonebook { get { return Device.Capability<bool>("wta_phonebook"); } }

    }
}