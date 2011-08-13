using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class Markup
    {
        public KnownDevice Device { get; set; }
        public bool HtmlWeb32 { get { return Device.Capability<bool>("html_web_3_2"); } }
        public bool HtmlWiImodeHtmlx1 { get { return Device.Capability<bool>("html_wi_imode_htmlx_1"); } }
        public bool HtmlWiImodeHtml1 { get { return Device.Capability<bool>("html_wi_imode_html_1"); } }
        public bool HtmlWiOmaXhtmlmp10 { get { return Device.Capability<bool>("html_wi_oma_xhtmlmp_1_0"); } }
        public bool HtmlWiImodeHtml2 { get { return Device.Capability<bool>("html_wi_imode_html_2"); } }
        public bool HtmlWiW3Xhtmlbasic { get { return Device.Capability<bool>("html_wi_w3_xhtmlbasic"); } }
        public bool HtmlWiImodeCompactGeneric { get { return Device.Capability<bool>("html_wi_imode_compact_generic"); } }
        public bool HtmlWiImodeHtml3 { get { return Device.Capability<bool>("html_wi_imode_html_3"); } }
        public bool Wml11 { get { return Device.Capability<bool>("wml_1_1"); } }
        public bool HtmlWiImodeHtml4 { get { return Device.Capability<bool>("html_wi_imode_html_4"); } }
        public bool Wml12 { get { return Device.Capability<bool>("wml_1_2"); } }
        public bool HtmlWiImodeHtml5 { get { return Device.Capability<bool>("html_wi_imode_html_5"); } }
        public bool Wml13 { get { return Device.Capability<bool>("wml_1_3"); } }
        public string PreferredMarkup { get { return Device.Capability<string>("preferred_markup"); } }
        public string XhtmlSupportLevel { get { return Device.Capability<string>("xhtml_support_level"); } }
        public bool Voicexml { get { return Device.Capability<bool>("voicexml"); } }
        public bool HtmlWiImodeHtmlx11 { get { return Device.Capability<bool>("html_wi_imode_htmlx_1_1"); } }
        public bool MultipartSupport { get { return Device.Capability<bool>("multipart_support"); } }
        public bool HtmlWeb40 { get { return Device.Capability<bool>("html_web_4_0"); } }

    }
}