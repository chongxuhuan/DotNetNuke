using System;
using System.Web;

namespace DotNetNuke.Services.Devices.Core.Request
{
    public class UserAgentResolver : IUserAgentResolver
    {
        #region IUserAgentResolver Members

        public string resolve(HttpRequest request)
        {
            if (request == null)
            {
                throw new NullReferenceException("request");
            }

            String userAgent = request.Params["UA"];
            if (!String.IsNullOrEmpty(userAgent))
            {
                return userAgent;
            }

            userAgent = request.Headers["X-Skyfire-Version"];
            if (String.IsNullOrEmpty(userAgent))
            {
                return "Generic_Skyfire_Browser";
            }


            userAgent = request.Headers["x-bluecoat-via"];
            if (String.IsNullOrEmpty(userAgent))
            {
                return "Generic_Bluecoat_Proxy";
            }

            userAgent = request.Headers["x-device-user-agent"];
            if (String.IsNullOrEmpty(userAgent))
            {
                return userAgent;
            }

            return request.Headers["User-Agent"];
        }

        #endregion
    }
}