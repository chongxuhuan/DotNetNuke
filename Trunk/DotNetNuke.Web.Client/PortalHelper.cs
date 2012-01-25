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
namespace DotNetNuke.Web.Client
{
    using System.Reflection;
    using System.Web;

    class PortalHelper
    {
        public bool? IsCompositeFilesOptionSetForPortal()
        {
            var portalSettings = HttpContext.Current.Items["PortalSettings"];
            if (portalSettings == null)
            {
                // portal settings not available
                return null;
            }

            var portalSetting = GetPropValue(portalSettings, "EnableCompositeFiles");
            bool enableCompositeFiles;
            if (portalSetting != null && bool.TryParse(portalSetting.ToString(), out enableCompositeFiles))
            {
                // a valid setting was found
                return enableCompositeFiles;
            }

            // no valid portal setting was found
            return null;
        }

        public int? GetPortalVersion(HttpContextBase http)
        {
            var portalSettings = HttpContext.Current.Items["PortalSettings"];
            if (portalSettings == null)
            {
                // portal settings not available
                return null;
            }

            var portalSetting = GetPropValue(portalSettings, "CdfVersion");
            int version;
            if (portalSetting != null && int.TryParse(portalSetting.ToString(), out version))
            {
                if (version > -1)
                {
                    // a valid setting was found
                    return version;
                }
            }

            // no valid portal setting was found
            return null;
        }

        private static object GetPropValue(object src, string propName)
        {
            PropertyInfo propertyInfo = src.GetType().GetProperty(propName);
            return propertyInfo != null ? propertyInfo.GetValue(src, null) : null;
        }
    }
}
