using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class SoundFormat
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool Rmf { get { return this.CapabilityProfile.Capability<bool>("rmf"); } }
        public bool Qcelp { get { return this.CapabilityProfile.Capability<bool>("qcelp"); } }
        public bool Awb { get { return this.CapabilityProfile.Capability<bool>("awb"); } }
        public bool Smf { get { return this.CapabilityProfile.Capability<bool>("smf"); } }
        public bool Wav { get { return this.CapabilityProfile.Capability<bool>("wav"); } }
        public bool NokiaRingtone { get { return this.CapabilityProfile.Capability<bool>("nokia_ringtone"); } }
        public bool Aac { get { return this.CapabilityProfile.Capability<bool>("aac"); } }
        public bool Digiplug { get { return this.CapabilityProfile.Capability<bool>("digiplug"); } }
        public bool SpMidi { get { return this.CapabilityProfile.Capability<bool>("sp_midi"); } }
        public bool Compactmidi { get { return this.CapabilityProfile.Capability<bool>("compactmidi"); } }
        public int Voices { get { return this.CapabilityProfile.Capability<int>("voices"); } }
        public bool Mp3 { get { return this.CapabilityProfile.Capability<bool>("mp3"); } }
        public bool Mld { get { return this.CapabilityProfile.Capability<bool>("mld"); } }
        public bool Evrc { get { return this.CapabilityProfile.Capability<bool>("evrc"); } }
        public bool Amr { get { return this.CapabilityProfile.Capability<bool>("amr"); } }
        public bool Xmf { get { return this.CapabilityProfile.Capability<bool>("xmf"); } }
        public bool Mmf { get { return this.CapabilityProfile.Capability<bool>("mmf"); } }
        public bool Imelody { get { return this.CapabilityProfile.Capability<bool>("imelody"); } }
        public bool MidiMonophonic { get { return this.CapabilityProfile.Capability<bool>("midi_monophonic"); } }
        public bool Au { get { return this.CapabilityProfile.Capability<bool>("au"); } }
        public bool MidiPolyphonic { get { return this.CapabilityProfile.Capability<bool>("midi_polyphonic"); } }

    }
}