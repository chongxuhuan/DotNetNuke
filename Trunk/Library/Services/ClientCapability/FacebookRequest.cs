using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;

using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// Make modules that are aware of Facebook’s signed_request – a parameter that is POSTed to the web page being loaded in the iFrame, 
    /// giving it variables such as if the Page has been Liked, and the age range of the user.
    /// 
    /// </summary>
    public class FacebookRequest
    {
        #region Public Properties
        public string Algorithm { get; set; }
        public string AccessToken{ get; set; }
        public DateTime Expires { get; set; }
        public long UserID { get; set; }        
        public long ProfileId { get; set; }
        public string RawSignedRequest { get; set; }
        public bool IsValid { get; set; }
        #endregion

        #region Public Methods    
        public bool IsValidSignature (string secretKey)
        {
            return FacebookRequestController.IsValidSignature(RawSignedRequest,secretKey);
        }
        #endregion        
    }
    
}
