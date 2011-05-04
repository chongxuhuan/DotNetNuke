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

namespace DotNetNuke.Entities.Profile
{
    [Serializable]
    public class ProfilePropertyDefinitionCollection : CollectionBase
    {
        public ProfilePropertyDefinitionCollection()
        {
        }

        public ProfilePropertyDefinitionCollection(ArrayList definitionsList)
        {
            AddRange(definitionsList);
        }

        public ProfilePropertyDefinitionCollection(ProfilePropertyDefinitionCollection collection)
        {
            AddRange(collection);
        }

        public ProfilePropertyDefinition this[int index]
        {
            get
            {
                return (ProfilePropertyDefinition) List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public ProfilePropertyDefinition this[string name]
        {
            get
            {
                return GetByName(name);
            }
        }

        public int Add(ProfilePropertyDefinition value)
        {
            return List.Add(value);
        }

        public void AddRange(ArrayList definitionsList)
        {
            foreach (ProfilePropertyDefinition objProfilePropertyDefinition in definitionsList)
            {
                Add(objProfilePropertyDefinition);
            }
        }

        public void AddRange(ProfilePropertyDefinitionCollection collection)
        {
            foreach (ProfilePropertyDefinition objProfilePropertyDefinition in collection)
            {
                Add(objProfilePropertyDefinition);
            }
        }

        public bool Contains(ProfilePropertyDefinition value)
        {
            return List.Contains(value);
        }

        public ProfilePropertyDefinitionCollection GetByCategory(string category)
        {
            var collection = new ProfilePropertyDefinitionCollection();
            foreach (ProfilePropertyDefinition profileProperty in InnerList)
            {
                if (profileProperty.PropertyCategory == category)
                {
                    collection.Add(profileProperty);
                }
            }
            return collection;
        }

        public ProfilePropertyDefinition GetById(int id)
        {
            ProfilePropertyDefinition profileItem = null;
            foreach (ProfilePropertyDefinition profileProperty in InnerList)
            {
                if (profileProperty.PropertyDefinitionId == id)
                {
                    profileItem = profileProperty;
                }
            }
            return profileItem;
        }

        public ProfilePropertyDefinition GetByName(string name)
        {
            ProfilePropertyDefinition profileItem = null;
            foreach (ProfilePropertyDefinition profileProperty in InnerList)
            {
                if (profileProperty.PropertyName == name)
                {
                    profileItem = profileProperty;
                }
            }
            return profileItem;
        }

        public int IndexOf(ProfilePropertyDefinition value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, ProfilePropertyDefinition value)
        {
            List.Insert(index, value);
        }

        public void Remove(ProfilePropertyDefinition value)
        {
            List.Remove(value);
        }

        public void Sort()
        {
            InnerList.Sort(new ProfilePropertyDefinitionComparer());
        }
    }
}
