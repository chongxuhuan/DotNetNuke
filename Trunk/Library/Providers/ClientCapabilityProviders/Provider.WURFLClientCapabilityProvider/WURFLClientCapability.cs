using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotNetNuke.Services.ClientCapability;

namespace DotNetNuke.Services.ClientCapability
{
    public class WURLClientCapability : KnownDevice, IClientCapability
    {

        public WURLClientCapability(IDevice device)
            : base(device)
        {
            this.ID = base.ID;
            this.UserAgent = base.UserAgent;
            this.IsMobile = base.IsMobileDevice;
            //this.IsTablet = base.IsMobileDevice;
            this.Capabilities = base.Capabilities;
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
        ///   Is request coming from Facebook iframe.
        /// </summary>
        /// <remarks>
        ///   Pesence of "signed_request" in the headers is used to detect of request is coming from facebook.
        ///   No further analysis is performed on the value of "signed_request".
        /// </remarks>         
        public bool IsFacebook { get; set; }

        /// <summary>
        ///   Screen Width of the requester.
        /// </summary>
        /// <remarks>
        ///   If IsFacebook is true, then this value represents iframe width, otherwise this value is device width
        /// </remarks>          
        public int Width { get; set; }

        /// <summary>
        ///   Screen Height of the requester.
        /// </summary>
        /// <remarks>
        ///   If IsFacebook is true, then this value represents iframe height, otherwise this value is device height
        /// </remarks>                  
        public int Height { get; set; }

        /// <summary>
        ///   Does requester supports Flash.
        /// </summary>
        public bool SupportsFlash { get; set; }

        /// <summary>
        /// A key-value collection containing all capabilities supported by requester
        /// </summary>        
        public IDictionary<string, string> Capabilities { get; set; }

        #endregion
    }
}
