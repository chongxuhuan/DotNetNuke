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

using System.Collections;

#endregion

namespace DotNetNuke.Services.Search
{
    /// -----------------------------------------------------------------------------
    /// Namespace:  DotNetNuke.Services.Search
    /// Project:    DotNetNuke.Search.Index
    /// Class:      SearchContentModuleInfoCollection
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Represents a collection of <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> objects.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	11/15/2004	documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class SearchContentModuleInfoCollection : CollectionBase
    {
        public SearchContentModuleInfoCollection()
        {
        }

        public SearchContentModuleInfoCollection(SearchContentModuleInfoCollection value)
        {
            AddRange(value);
        }

        public SearchContentModuleInfoCollection(SearchContentModuleInfo[] value)
        {
            AddRange(value);
        }

        public SearchContentModuleInfo this[int index]
        {
            get
            {
                return (SearchContentModuleInfo) List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(SearchContentModuleInfo value)
        {
            return List.Add(value);
        }

        public int IndexOf(SearchContentModuleInfo value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, SearchContentModuleInfo value)
        {
            List.Insert(index, value);
        }

        public void Remove(SearchContentModuleInfo value)
        {
            List.Remove(value);
        }

        public bool Contains(SearchContentModuleInfo value)
        {
            return List.Contains(value);
        }

        public void AddRange(SearchContentModuleInfo[] value)
        {
            for (int i = 0; i <= value.Length - 1; i++)
            {
                Add(value[i]);
            }
        }

        public void AddRange(SearchContentModuleInfoCollection value)
        {
            for (int i = 0; i <= value.Count - 1; i++)
            {
                Add((SearchContentModuleInfo) value.List[i]);
            }
        }

        public void CopyTo(SearchContentModuleInfo[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public SearchContentModuleInfo[] ToArray()
        {
            var arr = new SearchContentModuleInfo[Count];
            CopyTo(arr, 0);
            return arr;
        }
    }
}
