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
using System.Runtime.Serialization;

#endregion

namespace DotNetNuke.Services.Authentication.OAuth
{
    [DataContract]
    public class UserData
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        //[DataMember(Name = "location")]
        //public string City { get; set; }

        public virtual string DisplayName
        {
            get
            {
                return Name;
            }
            set { }
        }

        [DataMember(Name = "email")]
        public virtual string Email { get; set; }

        public virtual string FirstName
        {
            get
            {
                return (Name.IndexOf(" ") > 0) ? Name.Substring(0, Name.IndexOf(" ")) : String.Empty;
            }
            set { }
        }

        public virtual string LastName
        {
            get
            {
                return (Name.IndexOf(" ") > 0) ? Name.Substring(Name.IndexOf(" ") + 1) : Name;
            }
            set { }
        }

        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "locale")]
        public virtual string Locale { get; set; }

        [DataMember(Name = "name")]
        public virtual string Name { get; set; }

        public virtual string ProfileImage { get; set; }

        [DataMember(Name = "timezone")]
        public string Timezone { get; set; }

        [DataMember(Name = "time_zone")]
        public string TimeZoneInfo { get; set; }

        [DataMember(Name = "username")]
        public virtual string UserName { get; set; }

        [DataMember(Name = "website")]
        public virtual string Website { get; set; }
    }
}