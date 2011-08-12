using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class ChtmlUi
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public string ImodeRegion { get { return this.CapabilityProfile.Capability<string>("imode_region"); } }
        public string MakePhoneCallString { get { return this.CapabilityProfile.Capability<string>("chtml_make_phone_call_string"); } }
        public bool CanDisplayImagesAndTextOnSameLine { get { return this.CapabilityProfile.Capability<bool>("chtml_can_display_images_and_text_on_same_line"); } }
        public bool DisplaysImageInCenter { get { return this.CapabilityProfile.Capability<bool>("chtml_displays_image_in_center"); } }
        public bool TableSupport { get { return this.CapabilityProfile.Capability<bool>("chtml_table_support"); } }
        public bool DisplayAccesskey { get { return this.CapabilityProfile.Capability<bool>("chtml_display_accesskey"); } }
        public bool Emoji { get { return this.CapabilityProfile.Capability<bool>("emoji"); } }

    }
}