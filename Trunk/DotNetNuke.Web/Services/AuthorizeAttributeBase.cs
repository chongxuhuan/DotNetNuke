using System;
using System.Web;
using System.Web.Mvc;

namespace DotNetNuke.Web.Services
{
    /// <summary>
    /// A base class for authorize attributes
    /// <remarks>
    /// This is based on the ASP.Net MVC AuthorizeAttribute, with an enhancemtn to pass AuthorizationContext to AuthorizeCore.
    /// The code to deal with the cache and set the result should be common to all AuthorizationFilters</remarks>
    /// </summary>
    public abstract class AuthorizeAttributeBase : FilterAttribute, IAuthorizationFilter
    {
        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (AuthorizeCore(filterContext))
            {
                // ** IMPORTANT **
                // Since we're performing authorization at the action level, the authorization code runs
                // after the output caching module. In the worst case this could allow an authorized user
                // to cause the page to be cached, then an unauthorized user would later be served the
                // cached page. We work around this by telling proxies not to cache the sensitive page,
                // then we hook our custom authorization code into the caching mechanism so that we have
                // the final say on whether a page should be served from the cache.

                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, filterContext);
            }
            else
            {
                // auth failed, redirect to login page
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        #endregion

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization((AuthorizationContext)data);
        }

        private HttpValidationStatus OnCacheAuthorization(AuthorizationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            bool isAuthorized = AuthorizeCore(context);
            return (isAuthorized) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
        }

        protected abstract bool AuthorizeCore(AuthorizationContext context);
    }
}