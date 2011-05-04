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
using System.Collections;

#endregion

namespace DotNetNuke.Services.Search
{
    [Serializable]
    public class SearchItemInfoCollection : CollectionBase
    {
        public SearchItemInfoCollection()
        {
        }

        public SearchItemInfoCollection(SearchItemInfoCollection value)
        {
            AddRange(value);
        }

        public SearchItemInfoCollection(SearchItemInfo[] value)
        {
            AddRange(value);
        }

        public SearchItemInfoCollection(ArrayList value)
        {
            AddRange(value);
        }

        public SearchItemInfo this[int index]
        {
            get
            {
                return (SearchItemInfo) List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(SearchItemInfo value)
        {
            return List.Add(value);
        }

        public int IndexOf(SearchItemInfo value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, SearchItemInfo value)
        {
            List.Insert(index, value);
        }

        public void Remove(SearchItemInfo value)
        {
            List.Remove(value);
        }

        public bool Contains(SearchItemInfo value)
        {
            return List.Contains(value);
        }

        public void AddRange(SearchItemInfo[] value)
        {
            for (int i = 0; i <= value.Length - 1; i++)
            {
                Add(value[i]);
            }
        }

        public void AddRange(ArrayList value)
        {
            foreach (object obj in value)
            {
                if (obj is SearchItemInfo)
                {
                    Add((SearchItemInfo) obj);
                }
            }
        }

        public void AddRange(SearchItemInfoCollection value)
        {
            for (int i = 0; i <= value.Count - 1; i++)
            {
                Add((SearchItemInfo) value.List[i]);
            }
        }

        public void CopyTo(SearchItemInfo[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public SearchItemInfo[] ToArray()
        {
            var arr = new SearchItemInfo[Count];
            CopyTo(arr, 0);
            return arr;
        }

        public SearchItemInfoCollection ModuleItems(int ModuleId)
        {
            var retValue = new SearchItemInfoCollection();
            foreach (SearchItemInfo info in this)
            {
                if (info.ModuleId == ModuleId)
                {
                    retValue.Add(info);
                }
            }
            return retValue;
        }
    }
}
