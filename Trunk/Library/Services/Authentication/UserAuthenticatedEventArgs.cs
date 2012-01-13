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

using System;
using System.Collections.Specialized;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;

#endregion

namespace DotNetNuke.Services.Authentication
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The UserAuthenticatedEventArgs class provides a custom EventArgs object for the
    /// UserAuthenticated event
    /// </summary>
    /// <history>
    /// 	[cnurse]	07/10/2007  Created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class UserAuthenticatedEventArgs : EventArgs
    {
        private bool _Authenticated = true;
        private string _AuthenticationType = Null.NullString;
        private bool _AutoRegister = Null.NullBoolean;
        private UserLoginStatus _LoginStatus = UserLoginStatus.LOGIN_FAILURE;
        private string _Message = Null.NullString;
        private NameValueCollection _Profile = new NameValueCollection();
        private string _UserToken = Null.NullString;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// All properties Constructor.
        /// </summary>
        /// <param name="user">The user being authenticated.</param>
        /// <param name="token">The user token</param>
        /// <param name="status">The login status.</param>
        /// <param name="type">The type of Authentication</param>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public UserAuthenticatedEventArgs(UserInfo user, string token, UserLoginStatus status, string type)
        {
            User = user;
            _LoginStatus = status;
            _UserToken = token;
            _AuthenticationType = type;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets a flag that determines whether the User was authenticated
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/11/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool Authenticated
        {
            get
            {
                return _Authenticated;
            }
            set
            {
                _Authenticated = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Authentication Type
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
        /// Gets and sets a flag that determines whether the user should be automatically registered
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/16/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool AutoRegister
        {
            get
            {
                return _AutoRegister;
            }
            set
            {
                _AutoRegister = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Login Status
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public UserLoginStatus LoginStatus
        {
            get
            {
                return _LoginStatus;
            }
            set
            {
                _LoginStatus = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Message
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/11/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Profile
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/16/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public NameValueCollection Profile
        {
            get
            {
                return _Profile;
            }
            set
            {
                _Profile = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the User
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public UserInfo User { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the UserToken (the userid or authenticated id)
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string UserToken
        {
            get
            {
                return _UserToken;
            }
            set
            {
                _UserToken = value;
            }
        }
    }
}
