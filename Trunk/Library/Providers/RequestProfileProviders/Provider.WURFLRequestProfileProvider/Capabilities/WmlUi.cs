using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class WmlUi
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool IconsOnMenuItemsSupport { get { return this.CapabilityProfile.Capability<bool>("icons_on_menu_items_support"); } }
        public bool OpwvWmlExtensionsSupport { get { return this.CapabilityProfile.Capability<bool>("opwv_wml_extensions_support"); } }
        public bool BuiltInBackButtonSupport { get { return this.CapabilityProfile.Capability<bool>("built_in_back_button_support"); } }
        public bool ProportionalFont { get { return this.CapabilityProfile.Capability<bool>("proportional_font"); } }
        public bool InsertBrElementAfterWidgetRecommended { get { return this.CapabilityProfile.Capability<bool>("insert_br_element_after_widget_recommended"); } }
        public bool WizardsRecommended { get { return this.CapabilityProfile.Capability<bool>("wizards_recommended"); } }
        public bool CanDisplayImagesAndTextOnSameLine { get { return this.CapabilityProfile.Capability<bool>("wml_can_display_images_and_text_on_same_line"); } }
        public bool SoftkeySupport { get { return this.CapabilityProfile.Capability<bool>("softkey_support"); } }
        public string MakePhoneCallString { get { return this.CapabilityProfile.Capability<string>("wml_make_phone_call_string"); } }
        public bool DeckPrefetchSupport { get { return this.CapabilityProfile.Capability<bool>("deck_prefetch_support"); } }
        public bool MenuWithSelectElementRecommended { get { return this.CapabilityProfile.Capability<bool>("menu_with_select_element_recommended"); } }
        public bool NumberedMenus { get { return this.CapabilityProfile.Capability<bool>("numbered_menus"); } }
        public bool CardTitleSupport { get { return this.CapabilityProfile.Capability<bool>("card_title_support"); } }
        public bool ImageAsLinkSupport { get { return this.CapabilityProfile.Capability<bool>("image_as_link_support"); } }
        public bool WrapModeSupport { get { return this.CapabilityProfile.Capability<bool>("wrap_mode_support"); } }
        public bool TableSupport { get { return this.CapabilityProfile.Capability<bool>("table_support"); } }
        public bool AccessKeySupport { get { return this.CapabilityProfile.Capability<bool>("access_key_support"); } }
        public bool DisplaysImageInCenter { get { return this.CapabilityProfile.Capability<bool>("wml_displays_image_in_center"); } }
        public bool ElectiveFormsRecommended { get { return this.CapabilityProfile.Capability<bool>("elective_forms_recommended"); } }
        public bool TimesSquareModeSupport { get { return this.CapabilityProfile.Capability<bool>("times_square_mode_support"); } }
        public bool BreakListOfLinksWithBrElementRecommended { get { return this.CapabilityProfile.Capability<bool>("break_list_of_links_with_br_element_recommended"); } }
        public bool MenuWithListOfLinksRecommended { get { return this.CapabilityProfile.Capability<bool>("menu_with_list_of_links_recommended"); } }

    }
}