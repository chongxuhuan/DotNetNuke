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
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Entities.Content.Taxonomy
{
	/// <summary>
	/// Class of ScopeType.
	/// </summary>
	/// <seealso cref="TermController"/>
    [Serializable]
    public class ScopeType : IHydratable
    {
        private string _ScopeType;
        private int _ScopeTypeId;

        #region "Constructors"

        public ScopeType() : this(Null.NullString)
        {
        }

        public ScopeType(string scopeType)
        {
            _ScopeTypeId = Null.NullInteger;
            _ScopeType = scopeType;
        }

        #endregion

        #region "Public Properties"

        public int ScopeTypeId
        {
            get
            {
                return _ScopeTypeId;
            }
            set
            {
                _ScopeTypeId = value;
            }
        }

        public string Type
        {
            get
            {
                return _ScopeType;
            }
            set
            {
                _ScopeType = value;
            }
        }

        #endregion

        #region "IHydratable Implementation"

        public void Fill(IDataReader dr)
        {
            ScopeTypeId = Null.SetNullInteger(dr["ScopeTypeID"]);
            Type = Null.SetNullString(dr["ScopeType"]);
        }

        public int KeyID
        {
            get
            {
                return ScopeTypeId;
            }
            set
            {
                ScopeTypeId = value;
            }
        }

        #endregion

        public override string ToString()
        {
            return Type;
        }
    }
}