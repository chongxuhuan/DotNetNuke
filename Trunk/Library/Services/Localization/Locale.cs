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
using System.Globalization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Services.Localization
{
    /// <summary>
    ///   <para>The Locale class is a custom business object that represents a locale, which is the language and country combination.</para>
    /// </summary>
    [Serializable]
    public class Locale : BaseEntityInfo, IHydratable
    {
        private string _Code;
        private string _Fallback;
        private bool _IsPublished = Null.NullBoolean;
        private int _LanguageId = Null.NullInteger;
        private int _PortalId = Null.NullInteger;

        private string _Text;

        #region "Public Properties"

        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
            }
        }

        public CultureInfo Culture
        {
            get
            {
                return CultureInfo.CreateSpecificCulture(Code);
            }
        }

        public string EnglishName
        {
            get
            {
                string _Name = Null.NullString;
                if (Culture != null)
                {
                    _Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Culture.EnglishName);
                }
                return _Name;
            }
        }

        public string Fallback
        {
            get
            {
                return _Fallback;
            }
            set
            {
                _Fallback = value;
            }
        }

        public Locale FallBackLocale
        {
            get
            {
                Locale _FallbackLocale = null;
                if (!string.IsNullOrEmpty(Fallback))
                {
                    _FallbackLocale = LocaleController.Instance.GetLocale(PortalId, Fallback);
                }
                return _FallbackLocale;
            }
        }

        public bool IsPublished
        {
            get
            {
                return _IsPublished;
            }
            set
            {
                _IsPublished = value;
            }
        }

        public int LanguageId
        {
            get
            {
                return _LanguageId;
            }
            set
            {
                _LanguageId = value;
            }
        }

        public string NativeName
        {
            get
            {
                string _Name = Null.NullString;
                if (Culture != null)
                {
                    _Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Culture.NativeName);
                }
                return _Name;
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

        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
            }
        }

        #endregion

        #region "IHydratable Implementation"

        public void Fill(IDataReader dr)
        {
            LanguageId = Null.SetNullInteger(dr["LanguageID"]);
            Code = Null.SetNullString(dr["CultureCode"]);
            Text = Null.SetNullString(dr["CultureName"]);
            Fallback = Null.SetNullString(dr["FallbackCulture"]);

            try
            {
                //These fields may not be populated (for Host level locales)
                IsPublished = Null.SetNullBoolean(dr["IsPublished"]);
                PortalId = Null.SetNullInteger(dr["PortalID"]);
            }
            catch (IndexOutOfRangeException)
            {
            }

            //Call the base classes fill method to populate base class proeprties
            base.FillInternal(dr);
        }

        public int KeyID
        {
            get
            {
                return LanguageId;
            }
            set
            {
                LanguageId = value;
            }
        }

        #endregion
    }
}
