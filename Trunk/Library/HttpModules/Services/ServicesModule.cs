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
            var response = app.Response;

            if ((bool?)HttpContext.Current.Items["DnnReal401"] ?? false)
            {
                response.ClearContent();
                response.StatusCode = 401;
            }
        }

        public void Dispose()
        {
        }
    }
}