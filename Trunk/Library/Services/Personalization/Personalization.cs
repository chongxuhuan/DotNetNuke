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

using System.Web;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;

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
        /// <summary>
        /// load users profile and extract value base on naming container and key
        /// </summary>
        /// <param name="NamingContainer">Container for related set of values</param>
        /// <param name="Key">Individual profile key</param>
        /// <returns></returns>
        public static object GetProfile(string NamingContainer, string Key)
        {
            return GetProfile(LoadProfile(), NamingContainer, Key);
        }

        /// <summary>
        /// extract value base on naming container and key from PersonalizationInfo object
        /// </summary>
        /// <param name="objPersonalization">Object containing user personalization info</param>
        /// <param name="NamingContainer">Container for related set of values</param>
        /// <param name="Key">Individual profile key</param>
        /// <returns></returns>
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

       /// <summary>
        /// load users profile and extract secure value base on naming container and key
       /// </summary>
        /// <param name="NamingContainer">Container for related set of values</param>
        /// <param name="Key">Individual profile key</param>
       /// <returns></returns>
        public static object GetSecureProfile(string NamingContainer, string Key)
        {
            return GetSecureProfile(LoadProfile(), NamingContainer, Key);
        }

        /// <summary>
        /// extract value base on naming container and key from PersonalizationInfo object
        /// function will automatically decrypt value to plaintext
        /// </summary>
        /// <param name="objPersonalization">Object containing user personalization info</param>
        /// <param name="NamingContainer">Container for related set of values</param>
        /// <param name="Key">Individual profile key</param>
        /// <returns></returns>
        public static object GetSecureProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key)
        {
            if (objPersonalization != null)
            {
                var ps = new PortalSecurity();
                return ps.DecryptString(objPersonalization.Profile[NamingContainer + ":" + Key].ToString(), Config.GetDecryptionkey());
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// remove value from profile
        /// uses namingcontainer and key to locate approriate value
        /// </summary>
        /// <param name="NamingContainer">Container for related set of values</param>
        /// <param name="Key">Individual profile key</param>
        public static void RemoveProfile(string NamingContainer, string Key)
        {
            RemoveProfile(LoadProfile(), NamingContainer, Key);
        }

        /// <summary>
        /// remove value from users PersonalizationInfo object (if it exists)
        /// uses namingcontainer and key to locate approriate value
        /// </summary>
        /// <param name="objPersonalization">Object containing user personalization info</param>
        /// <param name="NamingContainer">Container for related set of values</param>
        /// <param name="Key">Individual profile key</param>
        public static void RemoveProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key)
        {
            if (objPersonalization != null)
            {
                (objPersonalization.Profile).Remove(NamingContainer + ":" + Key);
                objPersonalization.IsModified = true;
            }
        }

        /// <summary>
        /// persist profile value -use naming container and key to orgainize
        /// </summary>
        /// <param name="NamingContainer">Container for related set of values</param>
        /// <param name="Key">Individual profile key</param>
        /// <param name="Value">Individual profile value</param>
        public static void SetProfile(string NamingContainer, string Key, object Value)
        {
            SetProfile(LoadProfile(), NamingContainer, Key, Value);
        }

        /// <summary>
        /// persist value stored in PersonalizationInfo obhect - use naming container and key to organize
        /// </summary>
        /// <param name="objPersonalization">Object containing user personalization info</param>
        /// <param name="NamingContainer">Container for related set of values</param>
        /// <param name="Key">Individual profile key</param>
        /// <param name="value">Individual profile value</param>
        public static void SetProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key, object value)
        {
            if (objPersonalization != null)
            {
                objPersonalization.Profile[NamingContainer + ":" + Key] = value;
                objPersonalization.IsModified = true;
            }
        }

        /// <summary>
        /// persist profile value -use naming container and key to orgainize
        /// function calls an overload which automatically encrypts the value
        /// </summary>
        /// <param name="NamingContainer">Object containing user personalization info</param>
        /// <param name="Key">Individual profile key</param>
        /// <param name="Value">Individual profile value</param>
        public static void SetSecureProfile(string NamingContainer, string Key, object Value)
        {
            SetSecureProfile(LoadProfile(), NamingContainer, Key, Value);
        }

        /// <summary>
        /// persist profile value from PersonalizationInfo object, using naming container and key to organise 
        /// function will automatically encrypt the value to plaintext
        /// </summary>
        /// <param name="objPersonalization">Object containing user personalization info</param>
        /// <param name="NamingContainer">Container for related set of values</param>
        /// <param name="Key">Individual profile key</param>
        /// <param name="value">Individual profile value</param>
        public static void SetSecureProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key, object value)
        {
            if (objPersonalization != null)
            {
                var ps = new PortalSecurity();
                objPersonalization.Profile[NamingContainer + ":" + Key] = ps.EncryptString(value.ToString(), Config.GetDecryptionkey());
                objPersonalization.IsModified = true;
            }
        }
		#endregion
    }
}