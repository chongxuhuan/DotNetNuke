#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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
using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Web.Services
{
    public class DnnAuthorizeAttribute : AuthorizeAttribute
    {
        protected string[] UsersSplitBackingField;
        protected string[] RolesSplitBackingField;

        //this method must be thread-safe
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //do not call base implementation
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;
            if (!AllowAnonymous)
            {
                if (user == null || !user.Identity.IsAuthenticated)
                {
                    return false;
                }
            }

            if (UsersSplit.Length > 0 && (user == null || !UsersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase)))
            {
                return false;
            }


            if (RequiresHost || RolesSplit.Length > 0)
            {
                var userInfo = PortalController.GetCurrentPortalSettings().UserInfo;

                if(RequiresHost && !userInfo.IsSuperUser)
                {
                    return false;
                }

                if (!RolesSplit.Any(userInfo.IsInRole))
                {
                    return false;
                }
            }

            return true;
        }

        protected string[] RolesSplit
        {
            get
            {
                if (RolesSplitBackingField == null)
                {
                    //no locking don't worry about running this twice in a race
                    RolesSplitBackingField = SplitString(Roles);
                }
                return RolesSplitBackingField;
            }
        }

        protected string[] UsersSplit
        {
            get
            {
                if (UsersSplitBackingField == null)
                {
                    //no locking don't worry about running this twice in a race
                    UsersSplitBackingField = SplitString(Users);
                }
                return UsersSplitBackingField;
            }
        }

        /// <summary>
        /// Indicates that Host level credentials are required
        /// </summary>
        public bool RequiresHost { get; set; }

        /// <summary>
        /// Allows authorization of anonymous users
        /// <remarks>
        /// AllowAnonymous is used to allow anonymous access to specific methods in a controller that has a higher DefaultAuthLevel
        /// This setting will be ignored if any of User, Roles or Host level access are also specified
        /// </remarks>
        /// </summary>
        public bool AllowAnonymous { get; set; }

        protected string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }
}