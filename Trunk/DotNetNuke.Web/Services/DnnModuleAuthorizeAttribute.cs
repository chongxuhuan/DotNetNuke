using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Internal;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;

namespace DotNetNuke.Web.Services
{
    public sealed class DnnModuleAuthorizeAttribute : AuthorizeAttributeBase
    {
        public DnnModuleAuthorizeAttribute()
        {
            AccessLevel = SecurityAccessLevel.Host;
        }

        public string PermissionKey { get; set; }
        public SecurityAccessLevel AccessLevel { get; set; }

        // This method must be thread-safe since it is called by the caching module.
        protected override bool AuthorizeCore(HttpContextBase context)
        {
            var activeModule = context.FindModuleInfo();

            if (activeModule != null)
            {
                return ModulePermissionController.HasModuleAccess(AccessLevel, PermissionKey, activeModule);
            }

            return false;
        }
    }
}