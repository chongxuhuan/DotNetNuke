
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.ClientCapability;

namespace DotNetNuke.Services.ClientCapability
{
    public class XhtmlUi
    {
        public KnownDevice Device { get; set; }
        public string PreferredCharset { get { return Device.Capability<string>("xhtml_preferred_charset"); } }
        public bool SupportsCssCellTableColoring { get { return Device.Capability<bool>("xhtml_supports_css_cell_table_coloring"); } }
        public bool SelectAsRadiobutton { get { return Device.Capability<bool>("xhtml_select_as_radiobutton"); } }
        public bool AutoexpandSelect { get { return Device.Capability<bool>("xhtml_autoexpand_select"); } }
        public bool AvoidAccesskeys { get { return Device.Capability<bool>("xhtml_avoid_accesskeys"); } }
        public bool AcceptThirdPartyCookie { get { return Device.Capability<bool>("accept_third_party_cookie"); } }
        public string MakePhoneCallString { get { return Device.Capability<string>("xhtml_make_phone_call_string"); } }
        public bool AllowsDisabledFormElements { get { return Device.Capability<bool>("xhtml_allows_disabled_form_elements"); } }
        public bool SupportsInvisibleText { get { return Device.Capability<bool>("xhtml_supports_invisible_text"); } }
        public bool SelectAsDropdown { get { return Device.Capability<bool>("xhtml_select_as_dropdown"); } }
        public bool CookieSupport { get { return Device.Capability<bool>("cookie_support"); } }
        public string SendMmsString { get { return Device.Capability<string>("xhtml_send_mms_string"); } }
        public bool TableSupport { get { return Device.Capability<bool>("xhtml_table_support"); } }
        public bool DisplayAccesskey { get { return Device.Capability<bool>("xhtml_display_accesskey"); } }
        public string CanEmbedVideo { get { return Device.Capability<string>("xhtml_can_embed_video"); } }
        public string SupportsIframe { get { return Device.Capability<string>("xhtml_supports_iframe"); } }
        public string PreferredMimeType { get { return Device.Capability<string>("xhtmlmp_preferred_mime_type"); } }
        public bool SupportsMonospaceFont { get { return Device.Capability<bool>("xhtml_supports_monospace_font"); } }
        public bool SupportsInlineInput { get { return Device.Capability<bool>("xhtml_supports_inline_input"); } }
        public bool SupportsFormsInTable { get { return Device.Capability<bool>("xhtml_supports_forms_in_table"); } }
        public bool DocumentTitleSupport { get { return Device.Capability<bool>("xhtml_document_title_support"); } }
        public bool SupportWml2Namespace { get { return Device.Capability<bool>("xhtml_support_wml2_namespace"); } }
        public string ReadableBackgroundColor1 { get { return Device.Capability<string>("xhtml_readable_background_color1"); } }
        public bool FormatAsAttribute { get { return Device.Capability<bool>("xhtml_format_as_attribute"); } }
        public bool SupportsTableForLayout { get { return Device.Capability<bool>("xhtml_supports_table_for_layout"); } }
        public string ReadableBackgroundColor2 { get { return Device.Capability<string>("xhtml_readable_background_color2"); } }
        public bool SelectAsPopup { get { return Device.Capability<bool>("xhtml_select_as_popup"); } }
        public string SendSmsString { get { return Device.Capability<string>("xhtml_send_sms_string"); } }
        public bool FormatAsCssProperty { get { return Device.Capability<bool>("xhtml_format_as_css_property"); } }
        public string FileUpload { get { return Device.Capability<string>("xhtml_file_upload"); } }
        public bool HonorsBgcolor { get { return Device.Capability<bool>("xhtml_honors_bgcolor"); } }
        public bool OpwvXhtmlExtensionsSupport { get { return Device.Capability<bool>("opwv_xhtml_extensions_support"); } }
        public bool MarqueeAsCssProperty { get { return Device.Capability<bool>("xhtml_marquee_as_css_property"); } }
        public bool NowrapMode { get { return Device.Capability<bool>("xhtml_nowrap_mode"); } }

    }
}