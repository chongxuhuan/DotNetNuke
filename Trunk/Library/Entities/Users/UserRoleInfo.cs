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
using DotNetNuke.Security.Roles;

#endregion

namespace DotNetNuke.Entities.Users
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Users
    /// Class:      UserRoleInfo
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The UserRoleInfo class provides Business Layer model for a User/Role
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	01/03/2006	documented
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class UserRoleInfo : RoleInfo
    {
        private DateTime _EffectiveDate;
        private string _Email;
        private DateTime _ExpiryDate;
        private string _FullName;
        private bool _IsTrialUsed;
        private bool _Subscribed;
        private int _UserID;
        private int _UserRoleID;

        public int UserRoleID
        {
            get
            {
                return _UserRoleID;
            }
            set
            {
                _UserRoleID = value;
            }
        }

        public int UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                _UserID = value;
            }
        }

        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
            }
        }

        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;
            }
        }

        public DateTime EffectiveDate
        {
            get
            {
                return _EffectiveDate;
            }
            set
            {
                _EffectiveDate = value;
            }
        }

        public DateTime ExpiryDate
        {
            get
            {
                return _ExpiryDate;
            }
            set
            {
                _ExpiryDate = value;
            }
        }

        public bool IsTrialUsed
        {
            get
            {
                return _IsTrialUsed;
            }
            set
            {
                _IsTrialUsed = value;
            }
        }

        public bool Subscribed
        {
            get
            {
                return _Subscribed;
            }
            set
            {
                _Subscribed = value;
            }
        }

        public override void Fill(IDataReader dr)
        {
			//Fill base class properties
            base.Fill(dr);

			//Fill this class properties
            UserRoleID = Null.SetNullInteger(dr["UserRoleID"]);
            UserID = Null.SetNullInteger(dr["UserID"]);
            FullName = Null.SetNullString(dr["DisplayName"]);
            Email = Null.SetNullString(dr["Email"]);
            EffectiveDate = Null.SetNullDateTime(dr["EffectiveDate"]);
            ExpiryDate = Null.SetNullDateTime(dr["ExpiryDate"]);
            IsTrialUsed = Null.SetNullBoolean(dr["IsTrialUsed"]);
            if (UserRoleID > Null.NullInteger)
            {
                Subscribed = true;
            }
        }
    }
}
