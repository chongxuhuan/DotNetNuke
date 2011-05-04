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
            HttpContext Context = ((HttpApplication) s).Context;
            HttpRequest Request = Context.Request;
            HttpResponse Response = Context.Response;
            if (Request.Url.LocalPath.ToLower().EndsWith("install.aspx") || Request.Url.LocalPath.ToLower().EndsWith("installwizard.aspx"))
            {
                return;
            }
            if (Request.Url.LocalPath.ToLower().EndsWith(".aspx") == false && Request.Url.LocalPath.ToLower().EndsWith(".asmx") == false && Request.Url.LocalPath.ToLower().EndsWith(".ashx") == false)
            {
                return;
            }
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            if (Request.IsAuthenticated && _portalSettings != null)
            {
                string[] arrPortalRoles;
                var objRoleController = new RoleController();
                UserInfo objUser = UserController.GetCachedUser(_portalSettings.PortalId, Context.User.Identity.Name);
                if (Request.Cookies["portalaliasid"] != null)
                {
                    FormsAuthenticationTicket PortalCookie = FormsAuthentication.Decrypt(Context.Request.Cookies["portalaliasid"].Value);
                    if (_portalSettings.PortalAlias.PortalAliasID != Int32.Parse(PortalCookie.UserData))
                    {
                        Response.Cookies["portalaliasid"].Value = null;
                        Response.Cookies["portalaliasid"].Path = "/";
                        Response.Cookies["portalaliasid"].Expires = DateTime.Now.AddYears(-30);
                        Response.Cookies["portalroles"].Value = null;
                        Response.Cookies["portalroles"].Path = "/";
                        Response.Cookies["portalroles"].Expires = DateTime.Now.AddYears(-30);
                    }
                }
                if (objUser == null || objUser.IsDeleted || objUser.Membership.LockedOut || objUser.Membership.Approved == false || objUser.Username.ToLower() != Context.User.Identity.Name.ToLower())
                {
                    var objPortalSecurity = new PortalSecurity();
                    objPortalSecurity.SignOut();
                    if (objUser != null)
                    {
                        DataCache.ClearUserCache(_portalSettings.PortalId, Context.User.Identity.Name);
                    }
                    Response.Redirect(Request.RawUrl, true);
                    return;
                }
                else
                {
                    if (DateTime.Compare(objUser.Membership.LastActivityDate.AddMinutes(Host.UsersOnlineTimeWindow), DateTime.Now) < 0)
                    {
                        objUser.Membership.LastActivityDate = DateTime.Now;
                        objUser.LastIPAddress = Request.UserHostAddress;
                        UserController.UpdateUser(_portalSettings.PortalId, objUser, false);
                    }
                    bool refreshCookies = objUser.RefreshRoles;
                    if (!objUser.RefreshRoles && Request.QueryString["rsvp"] != null && !string.IsNullOrEmpty(Request.QueryString["rsvp"]))
                    {
                        foreach (RoleInfo objRole in objRoleController.GetPortalRoles(_portalSettings.PortalId))
                        {
                            if (objRole.RSVPCode == Request.QueryString["rsvp"])
                            {
                                objRoleController.UpdateUserRole(_portalSettings.PortalId, objUser.UserID, objRole.RoleID);
                                refreshCookies = true;
                            }
                        }
                    }
                    if (Request.Cookies["portalroles"] == null || refreshCookies)
                    {
                        DateTime CurrentDateTime = DateTime.Now;
                        var PortalTicket = new FormsAuthenticationTicket(1,
                                                                         Context.User.Identity.Name,
                                                                         CurrentDateTime,
                                                                         CurrentDateTime.AddHours(1),
                                                                         false,
                                                                         _portalSettings.PortalAlias.PortalAliasID.ToString());
                        string strPortalAliasID = FormsAuthentication.Encrypt(PortalTicket);
                        Response.Cookies["portalaliasid"].Value = strPortalAliasID;
                        Response.Cookies["portalaliasid"].Path = "/";
                        Response.Cookies["portalaliasid"].Expires = CurrentDateTime.AddMinutes(1);
                        arrPortalRoles = objRoleController.GetRolesByUser(objUser.UserID, _portalSettings.PortalId);
                        string strPortalRoles = _portalSettings.PortalId + "!!" + String.Join(";", arrPortalRoles);
                        var RolesTicket = new FormsAuthenticationTicket(1, Context.User.Identity.Name, CurrentDateTime, CurrentDateTime.AddHours(1), false, strPortalRoles);
                        string strRoles = FormsAuthentication.Encrypt(RolesTicket);
                        Response.Cookies["portalroles"].Value = strRoles;
                        Response.Cookies["portalroles"].Path = "/";
                        Response.Cookies["portalroles"].Expires = CurrentDateTime.AddMinutes(1);
                        if (refreshCookies)
                        {
                            Context.Request.Cookies["portalroles"].Value = strRoles;
                        }
                    }
                    if (Request.Cookies["portalroles"] != null)
                    {
                        if (!String.IsNullOrEmpty(Request.Cookies["portalroles"].Value))
                        {
                            FormsAuthenticationTicket RoleTicket = FormsAuthentication.Decrypt(Context.Request.Cookies["portalroles"].Value);
                            if (RoleTicket != null)
                            {
                                // get the role data and split it into portalid and a string array of role data
                                string rolesdata = RoleTicket.UserData;
                                char[] mySplit = "!!".ToCharArray();
                                //need to use StringSplitOptions.None to preserve case where superuser has no roles
                                string[] RolesParts = rolesdata.Split(mySplit, StringSplitOptions.None);

                                //if cookie is for a different portal than current force a refresh of roles else used cookie cached version
                                if (Convert.ToInt32(RolesParts[0]) != _portalSettings.PortalId)
                                {
                                    objUser.Roles = objRoleController.GetRolesByUser(objUser.UserID, _portalSettings.PortalId);
                                }
                                else
                                {
                                    objUser.Roles = RolesParts[2].Split(';');
                                }
                            }
                            else
                            {
                                objUser.Roles = objRoleController.GetRolesByUser(objUser.UserID, _portalSettings.PortalId);
                            }
                            if (objUser.RefreshRoles)
                            {
                                objUser.RefreshRoles = false;
                                UserController.UpdateUser(_portalSettings.PortalId, objUser, false);
                            }
                        }
                        Context.Items.Add("UserInfo", objUser);
                        var objPersonalizationController = new PersonalizationController();
                        objPersonalizationController.LoadProfile(Context, objUser.UserID, objUser.PortalID);
                        Localization.SetLanguage(objUser.Profile.PreferredLocale);
                    }
                }
            }
            if (HttpContext.Current.Items["UserInfo"] == null)
            {
                Context.Items.Add("UserInfo", new UserInfo());
            }
        }
    }
}