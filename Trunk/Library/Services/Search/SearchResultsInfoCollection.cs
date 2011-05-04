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
    public class SearchResultsInfoCollection : CollectionBase
    {
        public SearchResultsInfoCollection()
        {
        }

        public SearchResultsInfoCollection(SearchResultsInfoCollection value)
        {
            AddRange(value);
        }

        public SearchResultsInfoCollection(SearchResultsInfo[] value)
        {
            AddRange(value);
        }

        public SearchResultsInfoCollection(ArrayList value)
        {
            AddRange(value);
        }

        public SearchResultsInfo this[int index]
        {
            get
            {
                return (SearchResultsInfo) List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(SearchResultsInfo value)
        {
            return List.Add(value);
        }

        public int IndexOf(SearchResultsInfo value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, SearchResultsInfo value)
        {
            List.Insert(index, value);
        }

        public void Remove(SearchResultsInfo value)
        {
            List.Remove(value);
        }

        public bool Contains(SearchResultsInfo value)
        {
            return List.Contains(value);
        }

        public void AddRange(SearchResultsInfo[] value)
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
                if (obj is SearchResultsInfo)
                {
                    Add((SearchResultsInfo) obj);
                }
            }
        }

        public void AddRange(SearchResultsInfoCollection value)
        {
            for (int i = 0; i <= value.Count - 1; i++)
            {
                Add((SearchResultsInfo) value.List[i]);
            }
        }

        public void CopyTo(SearchResultsInfo[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public SearchResultsInfo[] ToArray()
        {
            var arr = new SearchResultsInfo[Count];
            CopyTo(arr, 0);
            return arr;
        }
    }
}
