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
using System.Data;
using System.Xml.Serialization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;

#endregion

namespace DotNetNuke.Entities
{
    [Serializable]
    public abstract class BaseEntityInfo
    {
        protected BaseEntityInfo()
        {
            CreatedByUserID = Null.NullInteger;
            LastModifiedByUserID = Null.NullInteger;
        }

        [Browsable(false), XmlIgnore]
        public int CreatedByUserID { get; private set; }

        [Browsable(false), XmlIgnore]
        public DateTime CreatedOnDate { get; private set; }

        [Browsable(false), XmlIgnore]
        public int LastModifiedByUserID { get; private set; }

        [Browsable(false), XmlIgnore]
        public DateTime LastModifiedOnDate { get; private set; }

        public UserInfo CreatedByUser(int portalId)
        {
            if (CreatedByUserID > Null.NullInteger)
            {
                UserInfo user = UserController.GetUserById(portalId, CreatedByUserID);
                return user;
            }
            return null;
        }

        public UserInfo LastModifiedByUser(int portalId)
        {
            if (LastModifiedByUserID > Null.NullInteger)
            {
                UserInfo user = UserController.GetUserById(portalId, LastModifiedByUserID);
                return user;
            }
            return null;
        }

        protected virtual void FillInternal(IDataReader dr)
        {
            CreatedByUserID = Null.SetNullInteger(dr["CreatedByUserID"]);
            CreatedOnDate = Null.SetNullDateTime(dr["CreatedOnDate"]);
            LastModifiedByUserID = Null.SetNullInteger(dr["LastModifiedByUserID"]);
            LastModifiedOnDate = Null.SetNullDateTime(dr["LastModifiedOnDate"]);
        }

        internal void FillBaseProperties(IDataReader dr)
        {
            FillInternal(dr);
        }
    }
}
