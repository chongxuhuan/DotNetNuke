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

using DotNetNuke.Instrumentation;

#endregion

namespace DotNetNuke.Common.Lists
{
    [Serializable]
    public class ListEntryInfoCollection : CollectionBase
    {
        private readonly Hashtable mKeyIndexLookup = new Hashtable();

        public ListEntryInfo this[int index]
        {
            get
            {
                try
                {
                    return (ListEntryInfo) base.List[index];
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);
                    return null;
                }
            }
        }

        public ListEntryInfo this[string key]
        {
            get
            {
                int index;
            	//<tam:note key to be lowercase for appropiated seeking>
                try
                {
                    if (mKeyIndexLookup[key.ToLower()] == null)
                    {
                        return null;
                    }
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);
                    return null;
                }
                index = Convert.ToInt32(mKeyIndexLookup[key.ToLower()]);
                return (ListEntryInfo) base.List[index];
            }
        }

        public ListEntryInfo GetChildren(string ParentName)
        {
            return this[ParentName];
        }

        internal new void Clear()
        {
            mKeyIndexLookup.Clear();
            base.Clear();
        }

        public void Add(string key, ListEntryInfo value)
        {
            int index;
            try //Do validation first
            {
                index = base.List.Add(value);
                mKeyIndexLookup.Add(key.ToLower(), index);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
            }
        }
    }
}