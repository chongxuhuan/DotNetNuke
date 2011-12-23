using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Web.Services
{
    public class DNNAuthorizeAttribute : AuthorizeAttribute
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
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            if (UsersSplit.Length > 0 && !UsersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }


            if (RolesSplit.Length > 0)
            {
                var userInfo = PortalController.GetCurrentPortalSettings().UserInfo;

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