using System;
using System.Web;

namespace DotNetNuke.HttpModules.Services
{
    public class ServicesModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += CheckForReal401;
        }

        private static void CheckForReal401(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if(app != null)
            {
                CheckForReal401(new HttpContextWrapper(app.Context));
            }
        }

        internal static void CheckForReal401(HttpContextBase context)
        {
            if(context == null)
            {
                throw new ArgumentNullException("context");
            }

            if ((bool?)context.Items["DnnReal401"] ?? false)
            {
                context.Response.ClearContent();
                context.Response.StatusCode = 401;
                context.Response.Headers.Remove("Location");
            }
        }

        public void Dispose()
        {
        }
    }
}