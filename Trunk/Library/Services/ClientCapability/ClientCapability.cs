using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotNetNuke.Services.ClientCapability;


namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// Default Implementation of IClientCapability
    /// </summary>
    public class ClientCapability : IClientCapability
    {
        /// <summary>
        ///   Default Constructor.
        /// </summary>
        public ClientCapability()
        {
            Capabilities = new Dictionary<string, string>();
        }
    
        #region Implementation of IClientCapability

        /// <summary>
        ///   Unique ID of the client making request.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        ///   User Agent of the client making request
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        ///   Is request coming from a mobile device.
        /// </summary>
        public bool IsMobile { get; set; }

        /// <summary>
        ///   Is request coming from a tablet device.
        /// </summary>
        public bool IsTablet { get; set; }        

        /// <summary>
        ///   Does the requesting device supports touch screen.
        /// </summary>
        public bool IsTouchScreen { get; set; }

        /// <summary>
        ///   FacebookRequest property is filled when request is coming though Facebook iFrame (e.g. fan pages).
        /// </summary>
        /// <remarks>
        ///   FacebookRequest property is populated based on data in "signed_request" headers coming from Facebook.  
        ///   In order to ensure request is coming from Facebook, FacebookRequest.IsValidSignature method should be called with the secrety key provided by Facebook.
        /// </remarks>         
        public FacebookRequest FacebookRequest { get; set; }

        /// <summary>
        ///   ScreenResolution Width of the requester in Pixels.
        /// </summary>
        public int ScreenResolutionWidthInPixels { get; set; }

        /// <summary>
        ///   ScreenResolution Height of the requester in Pixels.
        /// </summary>
        public int ScreenResolutionHeightInPixels { get; set; }

        /// <summary>
        ///   Does requester support Flash.
        /// </summary>
        public bool SupportsFlash { get; set; }

        /// <summary>
        /// A key-value collection containing all capabilities supported by requester
        /// </summary>        
        public IDictionary<string, string> Capabilities { get; set; }

        /// <summary>
        /// Represents the name of the broweser in the request
        /// </summary>
        public string BrowserName { get; set; }

        /// <summary>
        /// Returns the request prefered HTML DTD
        /// </summary>
        public string HtmlPreferedDTD { get; set; }

        /// <summary>
        ///   Http server variable used for SSL offloading - if this value is empty offloading is not enabled
        /// </summary>
        public string SSLOffload { get; set; }

        #endregion
    }
}
