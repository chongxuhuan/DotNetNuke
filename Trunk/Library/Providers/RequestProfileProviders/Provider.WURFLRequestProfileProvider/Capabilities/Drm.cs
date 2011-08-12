using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Drm
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool OmaV10CombinedDelivery { get { return this.CapabilityProfile.Capability<bool>("oma_v_1_0_combined_delivery"); } }
        public bool OmaV10SeparateDelivery { get { return this.CapabilityProfile.Capability<bool>("oma_v_1_0_separate_delivery"); } }
        public bool OmaV10Forwardlock { get { return this.CapabilityProfile.Capability<bool>("oma_v_1_0_forwardlock"); } }

    }
}