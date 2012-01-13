#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
#region Usings

using System.Web;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Services.Authentication
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The AuthenticationLoginBase class provides a bas class for Authentiication 
    /// Login controls
    /// </summary>
    /// <history>
    /// 	[cnurse]	07/10/2007  Created
    /// </history>
    /// -----------------------------------------------------------------------------
    public abstract class AuthenticationLoginBase : UserModuleBase
    {
        #region Delegates

        public delegate void UserAuthenticatedEventHandler(object sender, UserAuthenticatedEventArgs e);

        #endregion

        private string _AuthenticationType = Null.NullString;
        private string _RedirectURL = Null.NullString;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the Type of Authentication associated with this control
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string AuthenticationType
        {
            get
            {
                return _AuthenticationType;
            }
            set
            {
                _AuthenticationType = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets whether the control is Enabled
        /// </summary>
        /// <remarks>This property must be overriden in the inherited class</remarks>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public abstract bool Enabled { get; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the IP address associated with the request
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string IPAddress
        {
            get
            {
                return GetIPAddress();
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the Type of Authentication associated with this control
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string RedirectURL
        {
            get
            {
                return _RedirectURL;
            }
            set
            {
                _RedirectURL = value;
            }
        }

        public event UserAuthenticatedEventHandler UserAuthenticated;

        protected virtual void OnUserAuthenticated(UserAuthenticatedEventArgs ea)
        {
            if (UserAuthenticated != null)
            {
                UserAuthenticated(null, ea);
            }
        }

        public static string GetIPAddress()
        {
            string _IPAddress = Null.NullString;
            if (HttpContext.Current.Request.UserHostAddress != null)
            {
                _IPAddress = HttpContext.Current.Request.UserHostAddress;
            }
            return _IPAddress;
        }
    }
}
