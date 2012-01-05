using System;
using System.Web.Mvc;

namespace DotNetNuke.Web.Services
{
    public class DnnControllerActionInvoker : ControllerActionInvoker
    {
        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var controller = controllerContext.Controller as DnnController;
            if(controller == null)
            {
                throw new ArgumentException("DnnControllerActionInvoker only works with Controllers that are castable to DnnController");
            }
            
            var filters = base.GetFilters(controllerContext, actionDescriptor);

            //always 1 auth filter for the controller itself
            if(filters.AuthorizationFilters.Count == 1 && controller.DefaultAuthLevel != ServiceAuthLevel.Anonymous)
            {
                DnnAuthorizeAttribute authAttribute;
                switch (controller.DefaultAuthLevel)
                {
                    case ServiceAuthLevel.Host:
                        authAttribute = new DnnAuthorizeAttribute {RequiresHost = true};
                        break;

                    case ServiceAuthLevel.Admin:
                        authAttribute = new DnnAuthorizeAttribute {Roles = "Administrator"};
                        break;

                    default: //authenticated
                        authAttribute = new DnnAuthorizeAttribute();
                        break;
                }

                filters.AuthorizationFilters.Add(authAttribute);
            }

            return filters;
        }
    }
}