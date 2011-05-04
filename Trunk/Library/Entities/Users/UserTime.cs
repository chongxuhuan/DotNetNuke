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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;

#endregion

namespace DotNetNuke.Entities.Users
{
    [Obsolete("Deprecated in DNN 6.0.")]
    public class UserTime
    {
        public DateTime CurrentUserTime
        {
            get
            {
                HttpContext context = HttpContext.Current;
                PortalSettings objSettings = PortalController.GetCurrentPortalSettings();
                if (!context.Request.IsAuthenticated)
                {
                    return DateTime.UtcNow.AddMinutes(objSettings.TimeZoneOffset);
                }
                else
                {
                    return DateTime.UtcNow.AddMinutes(objSettings.TimeZoneOffset).AddMinutes(ClientToServerTimeZoneFactor);
                }
            }
        }

        public double ClientToServerTimeZoneFactor
        {
            get
            {
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                return FromClientToServerFactor(objUserInfo.Profile.TimeZone, _portalSettings.TimeZoneOffset);
            }
        }

        public double ServerToClientTimeZoneFactor
        {
            get
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                return FromServerToClientFactor(objUserInfo.Profile.TimeZone, _portalSettings.TimeZoneOffset);
            }
        }

        public DateTime ConvertToUserTime(DateTime dt, double ClientTimeZone)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return dt.AddMinutes(FromClientToServerFactor(ClientTimeZone, _portalSettings.TimeZoneOffset));
        }

        public DateTime ConvertToServerTime(DateTime dt, double ClientTimeZone)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return dt.AddMinutes(FromServerToClientFactor(ClientTimeZone, _portalSettings.TimeZoneOffset));
        }

        public static DateTime CurrentTimeForUser(UserInfo User)
        {
            if (User == null || User.UserID == -1 || User.Profile.TimeZone == Null.NullInteger)
            {
                int intOffset = 0;
                PortalSettings objSettings = PortalController.GetCurrentPortalSettings();
                if (objSettings != null)
                {
                    intOffset = objSettings.TimeZoneOffset;
                }
                else
                {
                    PortalInfo objPCtr = new PortalController().GetPortal(User.PortalID);
                    intOffset = objPCtr.TimeZoneOffset;
                }
                return DateTime.UtcNow.AddMinutes(intOffset);
            }
            else
            {
                return DateTime.UtcNow.AddMinutes(User.Profile.TimeZone);
            }
        }

        private double FromClientToServerFactor(double Client, double Server)
        {
            return Client - Server;
        }

        private double FromServerToClientFactor(double Client, double Server)
        {
            return Server - Client;
        }
    }
}