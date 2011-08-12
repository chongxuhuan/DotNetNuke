using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Markup
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool HtmlWeb32 { get { return this.CapabilityProfile.Capability<bool>("html_web_3_2"); } }
        public bool HtmlWiImodeHtmlx1 { get { return this.CapabilityProfile.Capability<bool>("html_wi_imode_htmlx_1"); } }
        public bool HtmlWiImodeHtml1 { get { return this.CapabilityProfile.Capability<bool>("html_wi_imode_html_1"); } }
        public bool HtmlWiOmaXhtmlmp10 { get { return this.CapabilityProfile.Capability<bool>("html_wi_oma_xhtmlmp_1_0"); } }
        public bool HtmlWiImodeHtml2 { get { return this.CapabilityProfile.Capability<bool>("html_wi_imode_html_2"); } }
        public bool HtmlWiW3Xhtmlbasic { get { return this.CapabilityProfile.Capability<bool>("html_wi_w3_xhtmlbasic"); } }
        public bool HtmlWiImodeCompactGeneric { get { return this.CapabilityProfile.Capability<bool>("html_wi_imode_compact_generic"); } }
        public bool HtmlWiImodeHtml3 { get { return this.CapabilityProfile.Capability<bool>("html_wi_imode_html_3"); } }
        public bool Wml11 { get { return this.CapabilityProfile.Capability<bool>("wml_1_1"); } }
        public bool HtmlWiImodeHtml4 { get { return this.CapabilityProfile.Capability<bool>("html_wi_imode_html_4"); } }
        public bool Wml12 { get { return this.CapabilityProfile.Capability<bool>("wml_1_2"); } }
        public bool HtmlWiImodeHtml5 { get { return this.CapabilityProfile.Capability<bool>("html_wi_imode_html_5"); } }
        public bool Wml13 { get { return this.CapabilityProfile.Capability<bool>("wml_1_3"); } }
        public string PreferredMarkup { get { return this.CapabilityProfile.Capability<string>("preferred_markup"); } }
        public string XhtmlSupportLevel { get { return this.CapabilityProfile.Capability<string>("xhtml_support_level"); } }
        public bool Voicexml { get { return this.CapabilityProfile.Capability<bool>("voicexml"); } }
        public bool HtmlWiImodeHtmlx11 { get { return this.CapabilityProfile.Capability<bool>("html_wi_imode_htmlx_1_1"); } }
        public bool MultipartSupport { get { return this.CapabilityProfile.Capability<bool>("multipart_support"); } }
        public bool HtmlWeb40 { get { return this.CapabilityProfile.Capability<bool>("html_web_4_0"); } }

    }
}