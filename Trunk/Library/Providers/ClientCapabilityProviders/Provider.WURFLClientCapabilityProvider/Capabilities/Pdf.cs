using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DotNetNuke.Services.ClientCapability
{
    public class Pdf
    {
        public KnownDevice Device { get; set; }
        public bool PdfSupport { get { return Device.Capability<bool>("pdf_support"); } }

    }
}