using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.RequestProfile;

namespace DotNetNuke.Services.Devices.Core.Capabilities
{
    public class Pdf
    {
        public ClientCapabilityProfile CapabilityProfile { get; set; }
        public bool PdfSupport { get { return this.CapabilityProfile.Capability<bool>("pdf_support"); } }

    }
}