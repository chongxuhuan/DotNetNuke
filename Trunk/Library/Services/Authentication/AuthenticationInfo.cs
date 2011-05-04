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
using System.Data;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Services.Authentication
{
    [Serializable]
    public class AuthenticationInfo : BaseEntityInfo, IHydratable
    {
        private int _AuthenticationID = Null.NullInteger;
        private string _AuthenticationType = Null.NullString;
        private bool _IsEnabled;
        private string _LoginControlSrc = Null.NullString;
        private string _LogoffControlSrc = Null.NullString;
        private int _PackageID;
        private string _SettingsControlSrc = Null.NullString;

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

        #region IHydratable Members

        public virtual void Fill(IDataReader dr)
        {
            AuthenticationID = Null.SetNullInteger(dr["AuthenticationID"]);
            PackageID = Null.SetNullInteger(dr["PackageID"]);
            IsEnabled = Null.SetNullBoolean(dr["IsEnabled"]);
            AuthenticationType = Null.SetNullString(dr["AuthenticationType"]);
            SettingsControlSrc = Null.SetNullString(dr["SettingsControlSrc"]);
            LoginControlSrc = Null.SetNullString(dr["LoginControlSrc"]);
            LogoffControlSrc = Null.SetNullString(dr["LogoffControlSrc"]);
            FillInternal(dr);
        }

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
