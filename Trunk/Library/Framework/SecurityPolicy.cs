#region Copyright

// 
// DotNetNuke� - http://www.dotnetnuke.com
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
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Web;

#endregion

namespace DotNetNuke.Framework
{
    public class SecurityPolicy
    {
        public const string ReflectionPermission = "ReflectionPermission";
        public const string WebPermission = "WebPermission";
        public const string AspNetHostingPermission = "AspNetHostingPermission";
        private static bool m_Initialized;
        private static bool m_ReflectionPermission;
        private static bool m_WebPermission;
        private static bool m_AspNetHostingPermission;

        public static string Permissions
        {
            get
            {
                string strPermissions = "";
                if (HasReflectionPermission())
                {
                    strPermissions += ", " + ReflectionPermission;
                }
                if (HasWebPermission())
                {
                    strPermissions += ", " + WebPermission;
                }
                if (HasAspNetHostingPermission())
                {
                    strPermissions += ", " + AspNetHostingPermission;
                }
                if (!String.IsNullOrEmpty(strPermissions))
                {
                    strPermissions = strPermissions.Substring(2);
                }
                return strPermissions;
            }
        }

        private static void GetPermissions()
        {
            if (!m_Initialized)
            {
                CodeAccessPermission securityTest;
                try
                {
                    securityTest = new ReflectionPermission(PermissionState.Unrestricted);
                    securityTest.Demand();
                    m_ReflectionPermission = true;
                }
                catch
                {
                    m_ReflectionPermission = false;
                }
                try
                {
                    securityTest = new WebPermission(PermissionState.Unrestricted);
                    securityTest.Demand();
                    m_WebPermission = true;
                }
                catch
                {
                    m_WebPermission = false;
                }
                try
                {
                    securityTest = new AspNetHostingPermission(AspNetHostingPermissionLevel.Unrestricted);
                    securityTest.Demand();
                    m_AspNetHostingPermission = true;
                }
                catch
                {
                    m_AspNetHostingPermission = false;
                }
                m_Initialized = true;
            }
        }

        public static bool HasAspNetHostingPermission()
        {
            GetPermissions();
            return m_AspNetHostingPermission;
        }

        public static bool HasReflectionPermission()
        {
            GetPermissions();
            return m_ReflectionPermission;
        }

        public static bool HasWebPermission()
        {
            GetPermissions();
            return m_WebPermission;
        }

        public static bool HasPermissions(string permissions, ref string permission)
        {
            bool _HasPermission = true;
            if (!String.IsNullOrEmpty(permissions))
            {
                foreach (string per in (permissions + ";").Split(Convert.ToChar(";")))
                {
                    permission = per;
                    if (!String.IsNullOrEmpty(permission.Trim()))
                    {
                        switch (permission)
                        {
                            case AspNetHostingPermission:
                                if (HasAspNetHostingPermission() == false)
                                {
                                    _HasPermission = false;
                                }
                                break;
                            case ReflectionPermission:
                                if (HasReflectionPermission() == false)
                                {
                                    _HasPermission = false;
                                }
                                break;
                            case WebPermission:
                                if (HasWebPermission() == false)
                                {
                                    _HasPermission = false;
                                }
                                break;
                        }
                    }
                }
            }
            return _HasPermission;
        }

        [Obsolete("Replaced by correctly spelt method")]
        public static bool HasRelectionPermission()
        {
            GetPermissions();
            return m_ReflectionPermission;
        }
    }
}