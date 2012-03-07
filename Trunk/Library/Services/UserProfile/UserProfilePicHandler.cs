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
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System.IO;

#endregion

namespace DotNetNuke.Services.UserProfile
{
    public class UserProfilePicHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            int UserId = -1;
            if (!String.IsNullOrEmpty(context.Request.QueryString["userid"]))
            {
                UserId = Convert.ToInt32(context.Request.QueryString["userid"].ToString());
            }
            int Width = 55;
            int Height = 55;
            if (!String.IsNullOrEmpty(context.Request.QueryString["w"]))
            {
                Width = Convert.ToInt32(context.Request.QueryString["w"].ToString());
            }
            if (!String.IsNullOrEmpty(context.Request.QueryString["h"]))
            {
                Height = Convert.ToInt32(context.Request.QueryString["h"].ToString());
            }
            int PortalId = 0;
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
            UserController uc = new UserController();
            UserInfo ui = uc.GetUser(PortalId, UserId);
            FileInfo fi = new FileInfo(context.Request.MapPath(ui.Profile.PhotoURL));
            String ext = ".png";
            String sizedPhoto;
            if (fi.Exists)
            {
                ext = fi.Extension;
                //sizedPhoto = fi.FullName; // fi.FullName.Replace(ext, Width.ToString() + "x" + Height.ToString() + ext);
                sizedPhoto = fi.FullName.Replace(ext, Width.ToString() + "x" + Height.ToString() + ext);
            }
            else
            {
                fi = new FileInfo(context.Request.MapPath("~/images/no_avatar.gif"));
                //sizedPhoto = fi.FullName; // fi.FullName.Replace(ext, Width.ToString() + "x" + Height.ToString() + ext);
                sizedPhoto = fi.FullName.Replace(ext, Width.ToString() + "x" + Height.ToString() + ext);
            }

            if (!File.Exists(sizedPhoto))
            {
                //need to create the photo
                File.Copy(fi.FullName, sizedPhoto);
                sizedPhoto = ImageUtils.CreateImage(sizedPhoto, Height, Width);
            }

            byte[] bindata = null;
            bindata = System.IO.File.ReadAllBytes(sizedPhoto);
            if (ext == ".png")
            {
                context.Response.ContentType = "image/png";
            }
            else if (ext == ".jpg" || ext == ".jpeg")
            {
                context.Response.ContentType = "image/jpeg";
            }
            else if (ext == ".gif")
            {
                context.Response.ContentType = "image/gif";
            }
            context.Response.WriteFile(sizedPhoto);
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        #endregion

        
    }
}
