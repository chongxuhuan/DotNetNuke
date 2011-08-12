using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class ProductInfo
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public string MobileBrowser { get { return this.CapabilityProfile.Capability<string>("mobile_browser"); } }
        public int NokiaFeaturePack { get { return this.CapabilityProfile.Capability<int>("nokia_feature_pack"); } }
        public string DeviceOs { get { return this.CapabilityProfile.Capability<string>("device_os"); } }
        public int NokiaSeries { get { return this.CapabilityProfile.Capability<int>("nokia_series"); } }
        public bool HasQwertyKeyboard { get { return this.CapabilityProfile.Capability<bool>("has_qwerty_keyboard"); } }
        public string PointingMethod { get { return this.CapabilityProfile.Capability<string>("pointing_method"); } }
        public string MobileBrowserVersion { get { return this.CapabilityProfile.Capability<string>("mobile_browser_version"); } }
        public bool IsTablet { get { return this.CapabilityProfile.Capability<bool>("is_tablet"); } }
        public int NokiaEdition { get { return this.CapabilityProfile.Capability<int>("nokia_edition"); } }
        public string Uaprof { get { return this.CapabilityProfile.Capability<string>("uaprof"); } }
        public bool CanSkipAlignedLinkRow { get { return this.CapabilityProfile.Capability<bool>("can_skip_aligned_link_row"); } }
        public bool DeviceClaimsWebSupport { get { return this.CapabilityProfile.Capability<bool>("device_claims_web_support"); } }
        public string UnuniquenessHandler { get { return this.CapabilityProfile.Capability<string>("ununiqueness_handler"); } }
        public string ModelName { get { return this.CapabilityProfile.Capability<string>("model_name"); } }
        public string DeviceOsVersion { get { return this.CapabilityProfile.Capability<string>("device_os_version"); } }
        public string Uaprof2 { get { return this.CapabilityProfile.Capability<string>("uaprof2"); } }
        public bool IsWirelessDevice { get { return this.CapabilityProfile.Capability<bool>("is_wireless_device"); } }
        public string Uaprof3 { get { return this.CapabilityProfile.Capability<string>("uaprof3"); } }
        public string BrandName { get { return this.CapabilityProfile.Capability<string>("brand_name"); } }
        public string ModelExtraInfo { get { return this.CapabilityProfile.Capability<string>("model_extra_info"); } }
        public string MarketingName { get { return this.CapabilityProfile.Capability<string>("marketing_name"); } }
        public bool CanAssignPhoneNumber { get { return this.CapabilityProfile.Capability<bool>("can_assign_phone_number"); } }
        public string ReleaseDate { get { return this.CapabilityProfile.Capability<string>("release_date"); } }
        public bool Unique { get { return this.CapabilityProfile.Capability<bool>("unique"); } }

    }
}