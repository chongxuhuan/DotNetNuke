using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class WmlUi
    {
        public KnownDevice Device { get; set; }
        public bool IconsOnMenuItemsSupport { get { return Device.Capability<bool>("icons_on_menu_items_support"); } }
        public bool OpwvWmlExtensionsSupport { get { return Device.Capability<bool>("opwv_wml_extensions_support"); } }
        public bool BuiltInBackButtonSupport { get { return Device.Capability<bool>("built_in_back_button_support"); } }
        public bool ProportionalFont { get { return Device.Capability<bool>("proportional_font"); } }
        public bool InsertBrElementAfterWidgetRecommended { get { return Device.Capability<bool>("insert_br_element_after_widget_recommended"); } }
        public bool WizardsRecommended { get { return Device.Capability<bool>("wizards_recommended"); } }
        public bool CanDisplayImagesAndTextOnSameLine { get { return Device.Capability<bool>("wml_can_display_images_and_text_on_same_line"); } }
        public bool SoftkeySupport { get { return Device.Capability<bool>("softkey_support"); } }
        public string MakePhoneCallString { get { return Device.Capability<string>("wml_make_phone_call_string"); } }
        public bool DeckPrefetchSupport { get { return Device.Capability<bool>("deck_prefetch_support"); } }
        public bool MenuWithSelectElementRecommended { get { return Device.Capability<bool>("menu_with_select_element_recommended"); } }
        public bool NumberedMenus { get { return Device.Capability<bool>("numbered_menus"); } }
        public bool CardTitleSupport { get { return Device.Capability<bool>("card_title_support"); } }
        public bool ImageAsLinkSupport { get { return Device.Capability<bool>("image_as_link_support"); } }
        public bool WrapModeSupport { get { return Device.Capability<bool>("wrap_mode_support"); } }
        public bool TableSupport { get { return Device.Capability<bool>("table_support"); } }
        public bool AccessKeySupport { get { return Device.Capability<bool>("access_key_support"); } }
        public bool DisplaysImageInCenter { get { return Device.Capability<bool>("wml_displays_image_in_center"); } }
        public bool ElectiveFormsRecommended { get { return Device.Capability<bool>("elective_forms_recommended"); } }
        public bool TimesSquareModeSupport { get { return Device.Capability<bool>("times_square_mode_support"); } }
        public bool BreakListOfLinksWithBrElementRecommended { get { return Device.Capability<bool>("break_list_of_links_with_br_element_recommended"); } }
        public bool MenuWithListOfLinksRecommended { get { return Device.Capability<bool>("menu_with_list_of_links_recommended"); } }

    }
}