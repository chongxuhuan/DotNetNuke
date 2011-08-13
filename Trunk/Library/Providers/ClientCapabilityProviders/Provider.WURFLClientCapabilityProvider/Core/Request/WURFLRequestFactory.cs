/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Web;
using DotNetNuke.Services.ClientCapability.Utils;

namespace DotNetNuke.Services.ClientCapability.Request
{
    public class WURFLRequestFactory : IWURFLRequestFactory
    {
        private readonly INormalizer<string> _normalizer;

        public WURFLRequestFactory(INormalizer<string> normalizer)
        {
            _normalizer = normalizer;
        }

        #region IWURFLRequestFactory Members

        public IWURFLRequest CreateRequestFromHttpRequest(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            string normalizedUserAgent = request.UserAgent;

            if (normalizedUserAgent != null)
            {
                normalizedUserAgent = _normalizer.Normalize(normalizedUserAgent);
            }
            else
            {
                normalizedUserAgent = "";
            }


            String uaProf = UserAgentUtils.GetUAProfile(request);
            bool xhtmlDevice = UserAgentUtils.IsXhtmlRequester(request);

            return new WURFLRequest(normalizedUserAgent, uaProf, xhtmlDevice);
        }

        public IWURFLRequest CreateRequestFromUserAgent(string userAgent)
        {
            string normalizedUserAgent = userAgent;
            if (userAgent != null)
            {
                normalizedUserAgent = _normalizer.Normalize(userAgent);
            }
            else
            {
                normalizedUserAgent = "";
            }

            return new WURFLRequest(normalizedUserAgent, null, false);
        }

        #endregion
    }
}