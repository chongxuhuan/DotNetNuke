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
using System.IO;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace DotNetNuke.UI.WebControls
{
    [ToolboxData("<{0}:CountryListBox runat=server></{0}:CountryListBox>")]
    public class CountryListBox : DropDownList
    {
        private bool _CacheGeoIPData = true;
        private string _GeoIPFile;
        private string _LocalhostCountryCode;
        private string _TestIP;

        [Bindable(true), Category("Caching"), DefaultValue(true)]
        public bool CacheGeoIPData
        {
            get
            {
                return _CacheGeoIPData;
            }
            set
            {
                _CacheGeoIPData = value;
                if (value == false)
                {
                    Context.Cache.Remove("GeoIPData");
                }
            }
        }

        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public string GeoIPFile
        {
            get
            {
                return _GeoIPFile;
            }
            set
            {
                _GeoIPFile = value;
            }
        }

        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public string TestIP
        {
            get
            {
                return _TestIP;
            }
            set
            {
                _TestIP = value;
            }
        }

        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public string LocalhostCountryCode
        {
            get
            {
                return _LocalhostCountryCode;
            }
            set
            {
                _LocalhostCountryCode = value;
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            bool IsLocal = false;
            string IP;
            if (!Page.IsPostBack)
            {
                if (String.IsNullOrEmpty(_GeoIPFile))
                {
                    _GeoIPFile = "controls/CountryListBox/Data/GeoIP.dat";
                }
                EnsureChildControls();
                if (!String.IsNullOrEmpty(_TestIP))
                {
                    IP = _TestIP;
                }
                else if (Page.Request.UserHostAddress == "127.0.0.1")
                {
                    IsLocal = true;
                    IP = Page.Request.UserHostAddress;
                }
                else
                {
                    IP = Page.Request.UserHostAddress;
                }
                if (Context.Cache.Get("GeoIPData") == null && _CacheGeoIPData)
                {
                    Context.Cache.Insert("GeoIPData", CountryLookup.FileToMemory(Context.Server.MapPath(_GeoIPFile)), new CacheDependency(Context.Server.MapPath(_GeoIPFile)));
                }
                if (IsLocal && !String.IsNullOrEmpty(_LocalhostCountryCode))
                {
                    base.OnDataBinding(e);
                    if (Items.FindByValue(_LocalhostCountryCode) != null)
                    {
                        Items.FindByValue(_LocalhostCountryCode).Selected = true;
                    }
                }
                else
                {
                    CountryLookup _CountryLookup;
                    if (_CacheGeoIPData)
                    {
                        _CountryLookup = new CountryLookup((MemoryStream) Context.Cache.Get("GeoIPData"));
                    }
                    else
                    {
                        _CountryLookup = new CountryLookup(Context.Server.MapPath(_GeoIPFile));
                    }
                    string _UserCountryCode = _CountryLookup.LookupCountryCode(IP);
                    base.OnDataBinding(e);
                    if (Items.FindByValue(_UserCountryCode) != null)
                    {
                        Items.FindByValue(_UserCountryCode).Selected = true;
                    }
                    else
                    {
                        string _UserCountry = _CountryLookup.LookupCountryName(IP);
                        if (_UserCountry != "N/A")
                        {
                            var newItem = new ListItem();
                            newItem.Value = _UserCountryCode;
                            newItem.Text = _UserCountry;
                            Items.Insert(0, newItem);
                            Items.FindByValue(_UserCountryCode).Selected = true;
                        }
                    }
                }
            }
        }
    }
}