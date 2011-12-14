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
using System.Web;
using System.Web.Security;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Personalization;

#endregion

namespace DotNetNuke.HttpModules.Membership
{
    public class MembershipModule : IHttpModule
    {
        public string ModuleName
        {
            get
            {
                return "DNNMembershipModule";
            }
        }

        #region IHttpModule Members

        public void Init(HttpApplication application)
        {
            application.AuthenticateRequest += OnAuthenticateRequest;
        }

        public void Dispose()
        {
        }

        #endregion

        public void OnAuthenticateRequest(object s, EventArgs e)
        {
            HttpContext context = ((HttpApplication) s).Context;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            //First check if we are upgrading/installing
            if (request.Url.LocalPath.ToLower().EndsWith("install.aspx")
                    || request.Url.LocalPath.ToLower().EndsWith("upgradewizard.aspx")
                    || request.Url.LocalPath.ToLower().EndsWith("installwizard.aspx"))
            {
                return;
            }
			
            //exit if a request for a .net mapping that isn't a content page is made i.e. axd
            if (request.Url.LocalPath.ToLower().EndsWith(".aspx") == false 
                && request.Url.LocalPath.ToLower().EndsWith(".asmx") == false 
                && request.Url.LocalPath.ToLower().EndsWith(".ashx") == false)
            {
                return;
            }
			
            //Obtain PortalSettings from Current Context
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();

            if (request.IsAuthenticated && portalSettings != null)
            {
                var roleController = new RoleController();
                UserInfo user = UserController.GetCachedUser(portalSettings.PortalId, context.User.Identity.Name);
				
                //authenticate user and set last login ( this is necessary for users who have a permanent Auth cookie set ) 
                if (user == null || user.IsDeleted || user.Membership.LockedOut 
                        || user.Membership.Approved == false 
                        || user.Username.ToLower() != context.User.Identity.Name.ToLower())
                {
                    var portalSecurity = new PortalSecurity();
                    portalSecurity.SignOut();
					
                    //Remove user from cache
                    if (user != null)
                    {
                        DataCache.ClearUserCache(portalSettings.PortalId, context.User.Identity.Name);
                    }
					
                    //Redirect browser back to home page
                    response.Redirect(request.RawUrl, true);
                    return;
                }

                //if users LastActivityDate is outside of the UsersOnlineTimeWindow then record user activity
                if (DateTime.Compare(user.Membership.LastActivityDate.AddMinutes(Host.UsersOnlineTimeWindow), DateTime.Now) < 0)
                {
                    //update LastActivityDate and IP Address for user
                    user.Membership.LastActivityDate = DateTime.Now;
                    user.LastIPAddress = request.UserHostAddress;
                    UserController.UpdateUser(portalSettings.PortalId, user, false);
                }
					
                //check for RSVP code
                if (request.QueryString["rsvp"] != null && !string.IsNullOrEmpty(request.QueryString["rsvp"]))
                {
                    foreach (RoleInfo role in roleController.GetPortalRoles(portalSettings.PortalId))
                    {
                        if (role.RSVPCode == request.QueryString["rsvp"])
                        {
                            roleController.UpdateUserRole(portalSettings.PortalId, user.UserID, role.RoleID);
                            user.ClearRoles();
                        }
                    }
                }

                //save userinfo object in context
                context.Items.Add("UserInfo", user);

                //load the personalization object
                var personalizationController = new PersonalizationController();
                personalizationController.LoadProfile(context, user.UserID, user.PortalID);

                //Localization.SetLanguage also updates the user profile, so this needs to go after the profile is loaded
                Localization.SetLanguage(user.Profile.PreferredLocale);
            }
            if (HttpContext.Current.Items["UserInfo"] == null)
            {
                context.Items.Add("UserInfo", new UserInfo());
            }
        }
    }
}