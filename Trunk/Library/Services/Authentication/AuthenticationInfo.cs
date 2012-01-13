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
using System.Data;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Services.Authentication
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The AuthenticationInfo class provides the Entity Layer for the 
    /// Authentication Systems.
    /// </summary>
    /// <history>
    /// 	[cnurse]	07/10/2007  Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class AuthenticationInfo : BaseEntityInfo, IHydratable
    {
		#region "Private Members"

        private int _AuthenticationID = Null.NullInteger;
        private string _AuthenticationType = Null.NullString;
        private bool _IsEnabled;
        private string _LoginControlSrc = Null.NullString;
        private string _LogoffControlSrc = Null.NullString;
        private int _PackageID;
        private string _SettingsControlSrc = Null.NullString;

		#endregion

		#region "Public Properties"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the ID of the Authentication System
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int AuthenticationID
        {
            get
            {
                return _AuthenticationID;
            }
            set
            {
                _AuthenticationID = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the PackageID for the Authentication System
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/31/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int PackageID
        {
            get
            {
                return _PackageID;
            }
            set
            {
                _PackageID = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets a flag that determines whether the Authentication System is enabled
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                _IsEnabled = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the type (name) of the Authentication System (eg DNN, OpenID, LiveID)
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
        /// Gets and Sets the url for the Settings Control
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string SettingsControlSrc
        {
            get
            {
                return _SettingsControlSrc;
            }
            set
            {
                _SettingsControlSrc = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the url for the Login Control
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/10/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string LoginControlSrc
        {
            get
            {
                return _LoginControlSrc;
            }
            set
            {
                _LoginControlSrc = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the url for the Logoff Control
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/23/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string LogoffControlSrc
        {
            get
            {
                return _LogoffControlSrc;
            }
            set
            {
                _LogoffControlSrc = value;
            }
        }
		
		#endregion

        #region IHydratable Members

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fills a RoleInfo from a Data Reader
        /// </summary>
        /// <param name="dr">The Data Reader to use</param>
        /// <history>
        /// 	[cnurse]	03/17/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public virtual void Fill(IDataReader dr)
        {
            AuthenticationID = Null.SetNullInteger(dr["AuthenticationID"]);
            PackageID = Null.SetNullInteger(dr["PackageID"]);
            IsEnabled = Null.SetNullBoolean(dr["IsEnabled"]);
            AuthenticationType = Null.SetNullString(dr["AuthenticationType"]);
            SettingsControlSrc = Null.SetNullString(dr["SettingsControlSrc"]);
            LoginControlSrc = Null.SetNullString(dr["LoginControlSrc"]);
            LogoffControlSrc = Null.SetNullString(dr["LogoffControlSrc"]);

            //Fill base class fields
            FillInternal(dr);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Key ID
        /// </summary>
        /// <returns>An Integer</returns>
        /// <history>
        /// 	[cnurse]	03/17/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public virtual int KeyID
        {
            get
            {
                return AuthenticationID;
            }
            set
            {
                AuthenticationID = value;
            }
        }

        #endregion
    }
}
