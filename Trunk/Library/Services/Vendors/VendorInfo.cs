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

#endregion

namespace DotNetNuke.Services.Vendors
{
    [Serializable]
    public class VendorInfo
    {
        private bool _Authorized;
        private int _Banners;
        private string _Cell;
        private string _City;
        private int _ClickThroughs;
        private string _Country;
        private string _CreatedByUser;
        private DateTime _CreatedDate;
        private string _Email;
        private string _Fax;
        private string _FirstName;
        private string _KeyWords;
        private string _LastName;
        private string _LogoFile;
        private int _PortalId;
        private string _PostalCode;
        private string _Region;
        private string _Street;
        private string _Telephone;
        private string _Unit;
        private string _UserName;
        private int _VendorId;
        private string _VendorName;
        private int _Views;
        private string _Website;

        public int VendorId
        {
            get
            {
                return _VendorId;
            }
            set
            {
                _VendorId = value;
            }
        }

        public string VendorName
        {
            get
            {
                return _VendorName;
            }
            set
            {
                _VendorName = value;
            }
        }

        public string Street
        {
            get
            {
                return _Street;
            }
            set
            {
                _Street = value;
            }
        }

        public string City
        {
            get
            {
                return _City;
            }
            set
            {
                _City = value;
            }
        }

        public string Region
        {
            get
            {
                return _Region;
            }
            set
            {
                _Region = value;
            }
        }

        public string Country
        {
            get
            {
                return _Country;
            }
            set
            {
                _Country = value;
            }
        }

        public string PostalCode
        {
            get
            {
                return _PostalCode;
            }
            set
            {
                _PostalCode = value;
            }
        }

        public string Telephone
        {
            get
            {
                return _Telephone;
            }
            set
            {
                _Telephone = value;
            }
        }

        public int PortalId
        {
            get
            {
                return _PortalId;
            }
            set
            {
                _PortalId = value;
            }
        }

        public string Fax
        {
            get
            {
                return _Fax;
            }
            set
            {
                _Fax = value;
            }
        }

        public string Cell
        {
            get
            {
                return _Cell;
            }
            set
            {
                _Cell = value;
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

        public string Website
        {
            get
            {
                return _Website;
            }
            set
            {
                _Website = value;
            }
        }

        public int ClickThroughs
        {
            get
            {
                return _ClickThroughs;
            }
            set
            {
                _ClickThroughs = value;
            }
        }

        public int Views
        {
            get
            {
                return _Views;
            }
            set
            {
                _Views = value;
            }
        }

        public string CreatedByUser
        {
            get
            {
                return _CreatedByUser;
            }
            set
            {
                _CreatedByUser = value;
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                _CreatedDate = value;
            }
        }

        public string LogoFile
        {
            get
            {
                return _LogoFile;
            }
            set
            {
                _LogoFile = value;
            }
        }

        public string KeyWords
        {
            get
            {
                return _KeyWords;
            }
            set
            {
                _KeyWords = value;
            }
        }

        public string Unit
        {
            get
            {
                return _Unit;
            }
            set
            {
                _Unit = value;
            }
        }

        public bool Authorized
        {
            get
            {
                return _Authorized;
            }
            set
            {
                _Authorized = value;
            }
        }

        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                _FirstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                _LastName = value;
            }
        }

        public int Banners
        {
            get
            {
                return _Banners;
            }
            set
            {
                _Banners = value;
            }
        }

        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }
    }
}