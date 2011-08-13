using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class ProductInfo
    {
        public KnownDevice Device { get; set; }
        public string MobileBrowser { get { return Device.Capability<string>("mobile_browser"); } }
        public int NokiaFeaturePack { get { return Device.Capability<int>("nokia_feature_pack"); } }
        public string DeviceOs { get { return Device.Capability<string>("device_os"); } }
        public int NokiaSeries { get { return Device.Capability<int>("nokia_series"); } }
        public bool HasQwertyKeyboard { get { return Device.Capability<bool>("has_qwerty_keyboard"); } }
        public string PointingMethod { get { return Device.Capability<string>("pointing_method"); } }
        public string MobileBrowserVersion { get { return Device.Capability<string>("mobile_browser_version"); } }
        public bool IsTablet { get { return Device.Capability<bool>("is_tablet"); } }
        public int NokiaEdition { get { return Device.Capability<int>("nokia_edition"); } }
        public string Uaprof { get { return Device.Capability<string>("uaprof"); } }
        public bool CanSkipAlignedLinkRow { get { return Device.Capability<bool>("can_skip_aligned_link_row"); } }
        public bool DeviceClaimsWebSupport { get { return Device.Capability<bool>("device_claims_web_support"); } }
        public string UnuniquenessHandler { get { return Device.Capability<string>("ununiqueness_handler"); } }
        public string ModelName { get { return Device.Capability<string>("model_name"); } }
        public string DeviceOsVersion { get { return Device.Capability<string>("device_os_version"); } }
        public string Uaprof2 { get { return Device.Capability<string>("uaprof2"); } }
        public bool IsWirelessDevice { get { return Device.Capability<bool>("is_wireless_device"); } }
        public string Uaprof3 { get { return Device.Capability<string>("uaprof3"); } }
        public string BrandName { get { return Device.Capability<string>("brand_name"); } }
        public string ModelExtraInfo { get { return Device.Capability<string>("model_extra_info"); } }
        public string MarketingName { get { return Device.Capability<string>("marketing_name"); } }
        public bool CanAssignPhoneNumber { get { return Device.Capability<bool>("can_assign_phone_number"); } }
        public string ReleaseDate { get { return Device.Capability<string>("release_date"); } }
        public bool Unique { get { return Device.Capability<bool>("unique"); } }

    }
}