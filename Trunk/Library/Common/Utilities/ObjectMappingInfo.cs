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
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace DotNetNuke.Common.Utilities
{
    [Serializable]
    public class ObjectMappingInfo
    {
        private const string RootCacheKey = "ObjectCache_";
        private readonly Dictionary<string, string> _ColumnNames;
        private readonly Dictionary<string, PropertyInfo> _Properties;
        private string _CacheByProperty;
        private int _CacheTimeOutMultiplier;
        private string _ObjectType;
        private string _PrimaryKey;
        private string _TableName;

        public ObjectMappingInfo()
        {
            _Properties = new Dictionary<string, PropertyInfo>();
            _ColumnNames = new Dictionary<string, string>();
        }

        public string CacheKey
        {
            get
            {
                string _CacheKey = RootCacheKey + TableName + "_";
                if (!string.IsNullOrEmpty(CacheByProperty))
                {
                    _CacheKey += CacheByProperty + "_";
                }
                return _CacheKey;
            }
        }

        public string CacheByProperty
        {
            get
            {
                return _CacheByProperty;
            }
            set
            {
                _CacheByProperty = value;
            }
        }

        public int CacheTimeOutMultiplier
        {
            get
            {
                return _CacheTimeOutMultiplier;
            }
            set
            {
                _CacheTimeOutMultiplier = value;
            }
        }

        public Dictionary<string, string> ColumnNames
        {
            get
            {
                return _ColumnNames;
            }
        }

        public string ObjectType
        {
            get
            {
                return _ObjectType;
            }
            set
            {
                _ObjectType = value;
            }
        }

        public string PrimaryKey
        {
            get
            {
                return _PrimaryKey;
            }
            set
            {
                _PrimaryKey = value;
            }
        }

        public Dictionary<string, PropertyInfo> Properties
        {
            get
            {
                return _Properties;
            }
        }

        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value;
            }
        }
    }
}
