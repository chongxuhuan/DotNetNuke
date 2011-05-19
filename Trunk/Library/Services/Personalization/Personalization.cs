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

using System.Web;

using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

#endregion

namespace DotNetNuke.Services.Personalization
{
    public class Personalization
    {
		#region "Private Methods"

        private static PersonalizationInfo LoadProfile()
        {
            HttpContext context = HttpContext.Current;

            //First try and load Personalization object from the Context
            var objPersonalization = (PersonalizationInfo) context.Items["Personalization"];

            //If the Personalization object is nothing load it and store it in the context for future calls
            if (objPersonalization == null)
            {
                var _portalSettings = (PortalSettings) context.Items["PortalSettings"];

                //load the user info object
                UserInfo UserInfo = UserController.GetCurrentUserInfo();

                //get the personalization object
                var objPersonalizationController = new PersonalizationController();
                objPersonalization = objPersonalizationController.LoadProfile(UserInfo.UserID, _portalSettings.PortalId);

                //store it in the context
                context.Items.Add("Personalization", objPersonalization);
            }
            return objPersonalization;
        }
		
		#endregion

		#region "Public Shared Methods"
        public static object GetProfile(string NamingContainer, string Key)
        {
            return GetProfile(LoadProfile(), NamingContainer, Key);
        }

        public static object GetProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key)
        {
            if (objPersonalization != null)
            {
                return objPersonalization.Profile[NamingContainer + ":" + Key];
            }
            else
            {
                return "";
            }
        }

        public static void RemoveProfile(string NamingContainer, string Key)
        {
            RemoveProfile(LoadProfile(), NamingContainer, Key);
        }

        public static void RemoveProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key)
        {
            if (objPersonalization != null)
            {
                (objPersonalization.Profile).Remove(NamingContainer + ":" + Key);
                objPersonalization.IsModified = true;
            }
        }

        public static void SetProfile(string NamingContainer, string Key, object Value)
        {
            SetProfile(LoadProfile(), NamingContainer, Key, Value);
        }

        public static void SetProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key, object value)
        {
            if (objPersonalization != null)
            {
                objPersonalization.Profile[NamingContainer + ":" + Key] = value;
                objPersonalization.IsModified = true;
            }
        }
		
		#endregion
    }
}