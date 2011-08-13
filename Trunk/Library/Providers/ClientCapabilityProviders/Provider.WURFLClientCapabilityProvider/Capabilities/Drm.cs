using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class Drm
    {
        public KnownDevice Device { get; set; }
        public bool OmaV10CombinedDelivery { get { return Device.Capability<bool>("oma_v_1_0_combined_delivery"); } }
        public bool OmaV10SeparateDelivery { get { return Device.Capability<bool>("oma_v_1_0_separate_delivery"); } }
        public bool OmaV10Forwardlock { get { return Device.Capability<bool>("oma_v_1_0_forwardlock"); } }

    }
}