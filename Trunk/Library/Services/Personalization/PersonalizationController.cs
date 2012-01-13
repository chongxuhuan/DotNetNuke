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
#region Usings

using System;
using System.Collections;
using System.Data;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

#endregion

namespace DotNetNuke.Services.Personalization
{
    public class PersonalizationController
    {
        //default implementation relies on HTTPContext
        public void LoadProfile(HttpContext httpContext, int userId, int portalId)
        {
            LoadProfile(new HttpContextWrapper(httpContext), userId, portalId);
        }

        public void LoadProfile(HttpContextBase httpContext, int userId, int portalId)
        {
            if (HttpContext.Current.Items["Personalization"] == null)
            {
                httpContext.Items.Add("Personalization", LoadProfile(userId, portalId));
            }
        }

        //override allows for manipulation of PersonalizationInfo outside of HTTPContext
        public PersonalizationInfo LoadProfile(int UserId, int PortalId)
        {
            var objPersonalization = new PersonalizationInfo();
            objPersonalization.UserId = UserId;
            objPersonalization.PortalId = PortalId;
            objPersonalization.IsModified = false;
            string profileData = Null.NullString;
            if (UserId > Null.NullInteger)
            {
                IDataReader dr = null;
                try
                {
                    dr = DataProvider.Instance().GetProfile(UserId, PortalId);
                    if (dr.Read())
                    {
                        profileData = dr["ProfileData"].ToString();
                    }
                    else //does not exist
                    {
                        DataProvider.Instance().AddProfile(UserId, PortalId);
                    }
                }
                catch (Exception ex)
                {
                    Exceptions.Exceptions.LogException(ex);
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
            }
            else
            {
				//Anon User - so try and use cookie.
                HttpContext context = HttpContext.Current;
                if (context != null && context.Request != null && context.Request.Cookies["DNNPersonalization"] != null)
                {
                    profileData = context.Request.Cookies["DNNPersonalization"].Value;
                }
            }
            if (string.IsNullOrEmpty(profileData))
            {
                objPersonalization.Profile = new Hashtable();
            }
            else
            {
                objPersonalization.Profile = Globals.DeserializeHashTableXml(profileData);
            }
            return objPersonalization;
        }

        public void SaveProfile(PersonalizationInfo objPersonalization)
        {
            SaveProfile(objPersonalization, objPersonalization.UserId, objPersonalization.PortalId);
        }

        //default implementation relies on HTTPContext
        public void SaveProfile(HttpContext objHTTPContext, int UserId, int PortalId)
        {
            var objPersonalization = (PersonalizationInfo) objHTTPContext.Items["Personalization"];
            SaveProfile(objPersonalization, UserId, PortalId);
        }

        //override allows for manipulation of PersonalizationInfo outside of HTTPContext
        public void SaveProfile(PersonalizationInfo objPersonalization, int UserId, int PortalId)
        {
            if (objPersonalization != null)
            {
                if (objPersonalization.IsModified)
                {
                    string ProfileData = Globals.SerializeHashTableXml(objPersonalization.Profile);
                    if (UserId > Null.NullInteger)
                    {
                        DataProvider.Instance().UpdateProfile(UserId, PortalId, ProfileData);
                    }
                    else
                    {
						//Anon User - so try and use cookie.
                        HttpContext context = HttpContext.Current;
                        if (context != null && context.Response != null)
                        {
                            var personalizationCookie = new HttpCookie("DNNPersonalization");
                            personalizationCookie.Value = ProfileData;
                            personalizationCookie.Expires = DateTime.Now.AddDays(30);
                            context.Response.Cookies.Add(personalizationCookie);
                        }
                    }
                }
            }
        }
    }
}