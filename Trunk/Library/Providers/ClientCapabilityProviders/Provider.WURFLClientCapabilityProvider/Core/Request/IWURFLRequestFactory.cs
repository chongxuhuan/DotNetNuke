/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Web;

namespace DotNetNuke.Services.ClientCapability.Request
{
    public interface IWURFLRequestFactory
    {
        /// <summary>
        /// Creates the wurfl request from HTTP request.
        /// </summary>
        /// <param name="reqeuest">The reqeuest.</param>
        /// <returns></returns>
        IWURFLRequest CreateRequestFromHttpRequest(HttpRequest reqeuest);

        /// <summary>
        /// Creates the wurfl request from user agent.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        IWURFLRequest CreateRequestFromUserAgent(string userAgent);
    }
}