using System.Web.Mvc;
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
        protected override bool AuthorizeCore(AuthorizationContext context)
        {
            //by using ActiveModule the request must specify both moduleid and tabid
            //it is possible to validate permissions when only moduleid is included
            //but is it a desirable thing to do?
            var controller = context.Controller as DnnController;
            if (controller != null && controller.ActiveModule != null)
            {
                return ModulePermissionController.HasModuleAccess(AccessLevel, PermissionKey, controller.ActiveModule);
            }

            return false;
        }
    }
}