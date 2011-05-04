#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
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
using System.ComponentModel;

using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Entities.Users
{
    [Serializable]
    public class UserMembership
    {
        private readonly UserInfo _user;
        private bool _objectHydrated;

        public UserMembership(UserInfo user)
        {
            Approved = true;
            _user = user;
        }

        public bool Approved { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsOnLine { get; set; }

        public DateTime LastActivityDate { get; set; }

        public DateTime LastLockoutDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime LastPasswordChangeDate { get; set; }

        public bool LockedOut { get; set; }

        public string Password { get; set; }

        public string PasswordAnswer { get; set; }

        public string PasswordQuestion { get; set; }

        public bool UpdatePassword { get; set; }

        [Obsolete("Deprecated in DNN 5.1")]
        public UserMembership()
        {
            Approved = true;
            _user = new UserInfo();
        }

        [Obsolete("Deprecated in DNN 5.1")]
        [Browsable(false)]
        public string Email
        {
            get
            {
                return _user.Email;
            }
            set
            {
                _user.Email = value;
            }
        }

        [Obsolete("Deprecated in DNN 5.1")]
        [Browsable(false)]
        public bool ObjectHydrated
        {
            get
            {
                return _objectHydrated;
            }
            set
            {
                _objectHydrated = true;
            }
        }

        [Obsolete("Deprecated in DNN 5.1")]
        [Browsable(false)]
        public string Username
        {
            get
            {
                return _user.Username;
            }
            set
            {
                _user.Username = value;
            }
        }
    }
}
