using System;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;

namespace DotNetNuke.Web.Services
{
    public sealed class DnnModuleAuthorizeAttribute : AuthorizeAttributeBase
    {
        public DnnModuleAuthorizeAttribute()
        {
            ModuleIdKey = "ModuleId";
        }

        //todo is SAL appropriate some values don't really fit
        public SecurityAccessLevel Requires { get; set; }
        public string ModuleIdKey { get; set; }

        // This method must be thread-safe since it is called by the caching module.

        // This method must be thread-safe since it is called by the caching module.
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (Requires == SecurityAccessLevel.Anonymous)
            {
                return true;
            }

            int moduleId = FindModuleId(httpContext.Request);
            if (moduleId > Null.NullInteger)
            {
                ModuleInfo module = new ModuleController().GetModule(moduleId);
                return ModulePermissionController.HasModuleAccess(Requires, "EDIT", module);
            }

            return false;
        }

        private int FindModuleId(HttpRequestBase request)
        {
            string value = request.QueryString[ModuleIdKey];

            if (String.IsNullOrEmpty(value))
            {
                value = request.Headers[ModuleIdKey];
            }

            if (String.IsNullOrEmpty(value))
            {
                HttpCookie cookie = request.Cookies[ModuleIdKey];
                if (cookie != null)
                {
                    value = cookie.Value;
                }
            }

            int id;
            if (Int32.TryParse(value, out id))
            {
                return id;
            }

            return Null.NullInteger;
        }
    }
}