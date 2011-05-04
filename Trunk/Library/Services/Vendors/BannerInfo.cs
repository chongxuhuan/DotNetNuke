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
    public class BannerInfo
    {
        private int _BannerId;
        private string _BannerName;
        private int _BannerTypeId;
        private double _CPM;
        private int _ClickThroughs;
        private string _CreatedByUser;
        private DateTime _CreatedDate;
        private int _Criteria;
        private string _Description;
        private DateTime _EndDate;
        private string _GroupName;
        private int _Height;
        private string _ImageFile;
        private int _Impressions;
        private DateTime _StartDate;
        private string _URL;
        private int _VendorId;
        private int _Views;
        private int _Width;

        public int BannerId
        {
            get
            {
                return _BannerId;
            }
            set
            {
                _BannerId = value;
            }
        }

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

        public string ImageFile
        {
            get
            {
                return _ImageFile;
            }
            set
            {
                _ImageFile = value;
            }
        }

        public string BannerName
        {
            get
            {
                return _BannerName;
            }
            set
            {
                _BannerName = value;
            }
        }

        public string URL
        {
            get
            {
                return _URL;
            }
            set
            {
                _URL = value;
            }
        }

        public int Impressions
        {
            get
            {
                return _Impressions;
            }
            set
            {
                _Impressions = value;
            }
        }

        public double CPM
        {
            get
            {
                return _CPM;
            }
            set
            {
                _CPM = value;
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

        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
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

        public int BannerTypeId
        {
            get
            {
                return _BannerTypeId;
            }
            set
            {
                _BannerTypeId = value;
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                _GroupName = value;
            }
        }

        public int Criteria
        {
            get
            {
                return _Criteria;
            }
            set
            {
                _Criteria = value;
            }
        }

        public int Width
        {
            get
            {
                return _Width;
            }
            set
            {
                _Width = value;
            }
        }

        public int Height
        {
            get
            {
                return _Height;
            }
            set
            {
                _Height = value;
            }
        }
    }
}