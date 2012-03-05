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
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Web.Services
{
    public sealed class DnnAuthorizeAttribute : AuthorizeAttributeBase, IOverrideDefaultAuthLevel
    {
        private string _roles;
        private string[] _rolesSplit = new string[0];

        public string Roles
        {
            get { return _roles; }
            set
            {
                _roles = value;
                _rolesSplit = SplitString(_roles);
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

        protected override bool AuthorizeCore(HttpContextBase context)
        {
            //do not call base implementation
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            IPrincipal user = context.User;
            if (!AllowAnonymous)
            {
                if (user == null || !user.Identity.IsAuthenticated)
                {
                    return false;
                }
            }

            if (RequiresHost)
            {
                if (!CurrentUser.IsSuperUser)
                {
                    return false;
                }
            }

            if(_rolesSplit.Any())
            {
                if (!_rolesSplit.Any(CurrentUser.IsInRole))
                {
                    return false;
                }
            }

            return true;
        }

        private static UserInfo CurrentUser
        {
            get { return PortalController.GetCurrentPortalSettings().UserInfo; }
        }

        private string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            IEnumerable<string> split = from piece in original.Split(',')
                                        let trimmed = piece.Trim()
                                        where !String.IsNullOrEmpty(trimmed)
                                        select trimmed;
            return split.ToArray();
        }
    }
}