using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class SoundFormat
    {
        public KnownDevice Device { get; set; }
        public bool Rmf { get { return Device.Capability<bool>("rmf"); } }
        public bool Qcelp { get { return Device.Capability<bool>("qcelp"); } }
        public bool Awb { get { return Device.Capability<bool>("awb"); } }
        public bool Smf { get { return Device.Capability<bool>("smf"); } }
        public bool Wav { get { return Device.Capability<bool>("wav"); } }
        public bool NokiaRingtone { get { return Device.Capability<bool>("nokia_ringtone"); } }
        public bool Aac { get { return Device.Capability<bool>("aac"); } }
        public bool Digiplug { get { return Device.Capability<bool>("digiplug"); } }
        public bool SpMidi { get { return Device.Capability<bool>("sp_midi"); } }
        public bool Compactmidi { get { return Device.Capability<bool>("compactmidi"); } }
        public int Voices { get { return Device.Capability<int>("voices"); } }
        public bool Mp3 { get { return Device.Capability<bool>("mp3"); } }
        public bool Mld { get { return Device.Capability<bool>("mld"); } }
        public bool Evrc { get { return Device.Capability<bool>("evrc"); } }
        public bool Amr { get { return Device.Capability<bool>("amr"); } }
        public bool Xmf { get { return Device.Capability<bool>("xmf"); } }
        public bool Mmf { get { return Device.Capability<bool>("mmf"); } }
        public bool Imelody { get { return Device.Capability<bool>("imelody"); } }
        public bool MidiMonophonic { get { return Device.Capability<bool>("midi_monophonic"); } }
        public bool Au { get { return Device.Capability<bool>("au"); } }
        public bool MidiPolyphonic { get { return Device.Capability<bool>("midi_polyphonic"); } }

    }
}