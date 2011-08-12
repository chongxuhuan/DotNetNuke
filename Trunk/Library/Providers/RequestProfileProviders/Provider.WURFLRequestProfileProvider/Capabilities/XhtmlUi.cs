
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class XhtmlUi
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public string PreferredCharset { get { return this.CapabilityProfile.Capability<string>("xhtml_preferred_charset"); } }
        public bool SupportsCssCellTableColoring { get { return this.CapabilityProfile.Capability<bool>("xhtml_supports_css_cell_table_coloring"); } }
        public bool SelectAsRadiobutton { get { return this.CapabilityProfile.Capability<bool>("xhtml_select_as_radiobutton"); } }
        public bool AutoexpandSelect { get { return this.CapabilityProfile.Capability<bool>("xhtml_autoexpand_select"); } }
        public bool AvoidAccesskeys { get { return this.CapabilityProfile.Capability<bool>("xhtml_avoid_accesskeys"); } }
        public bool AcceptThirdPartyCookie { get { return this.CapabilityProfile.Capability<bool>("accept_third_party_cookie"); } }
        public string MakePhoneCallString { get { return this.CapabilityProfile.Capability<string>("xhtml_make_phone_call_string"); } }
        public bool AllowsDisabledFormElements { get { return this.CapabilityProfile.Capability<bool>("xhtml_allows_disabled_form_elements"); } }
        public bool SupportsInvisibleText { get { return this.CapabilityProfile.Capability<bool>("xhtml_supports_invisible_text"); } }
        public bool SelectAsDropdown { get { return this.CapabilityProfile.Capability<bool>("xhtml_select_as_dropdown"); } }
        public bool CookieSupport { get { return this.CapabilityProfile.Capability<bool>("cookie_support"); } }
        public string SendMmsString { get { return this.CapabilityProfile.Capability<string>("xhtml_send_mms_string"); } }
        public bool TableSupport { get { return this.CapabilityProfile.Capability<bool>("xhtml_table_support"); } }
        public bool DisplayAccesskey { get { return this.CapabilityProfile.Capability<bool>("xhtml_display_accesskey"); } }
        public string CanEmbedVideo { get { return this.CapabilityProfile.Capability<string>("xhtml_can_embed_video"); } }
        public string SupportsIframe { get { return this.CapabilityProfile.Capability<string>("xhtml_supports_iframe"); } }
        public string PreferredMimeType { get { return this.CapabilityProfile.Capability<string>("xhtmlmp_preferred_mime_type"); } }
        public bool SupportsMonospaceFont { get { return this.CapabilityProfile.Capability<bool>("xhtml_supports_monospace_font"); } }
        public bool SupportsInlineInput { get { return this.CapabilityProfile.Capability<bool>("xhtml_supports_inline_input"); } }
        public bool SupportsFormsInTable { get { return this.CapabilityProfile.Capability<bool>("xhtml_supports_forms_in_table"); } }
        public bool DocumentTitleSupport { get { return this.CapabilityProfile.Capability<bool>("xhtml_document_title_support"); } }
        public bool SupportWml2Namespace { get { return this.CapabilityProfile.Capability<bool>("xhtml_support_wml2_namespace"); } }
        public string ReadableBackgroundColor1 { get { return this.CapabilityProfile.Capability<string>("xhtml_readable_background_color1"); } }
        public bool FormatAsAttribute { get { return this.CapabilityProfile.Capability<bool>("xhtml_format_as_attribute"); } }
        public bool SupportsTableForLayout { get { return this.CapabilityProfile.Capability<bool>("xhtml_supports_table_for_layout"); } }
        public string ReadableBackgroundColor2 { get { return this.CapabilityProfile.Capability<string>("xhtml_readable_background_color2"); } }
        public bool SelectAsPopup { get { return this.CapabilityProfile.Capability<bool>("xhtml_select_as_popup"); } }
        public string SendSmsString { get { return this.CapabilityProfile.Capability<string>("xhtml_send_sms_string"); } }
        public bool FormatAsCssProperty { get { return this.CapabilityProfile.Capability<bool>("xhtml_format_as_css_property"); } }
        public string FileUpload { get { return this.CapabilityProfile.Capability<string>("xhtml_file_upload"); } }
        public bool HonorsBgcolor { get { return this.CapabilityProfile.Capability<bool>("xhtml_honors_bgcolor"); } }
        public bool OpwvXhtmlExtensionsSupport { get { return this.CapabilityProfile.Capability<bool>("opwv_xhtml_extensions_support"); } }
        public bool MarqueeAsCssProperty { get { return this.CapabilityProfile.Capability<bool>("xhtml_marquee_as_css_property"); } }
        public bool NowrapMode { get { return this.CapabilityProfile.Capability<bool>("xhtml_nowrap_mode"); } }

    }
}