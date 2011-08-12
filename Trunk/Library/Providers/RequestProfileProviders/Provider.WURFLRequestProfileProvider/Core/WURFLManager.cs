/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */

using System.Web;
using DotNetNuke.Services.Devices.Core.Request;

namespace DotNetNuke.Services.Devices.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class WURFLManager : IWURFLManager
    {
        private readonly IWURFLRequestFactory _wurflRequestFactory;
        private readonly IWURFLService _wurflService;

        public IDevice GetGenericDevice()
        {
            return GetDeviceForRequest("generic");
        }

        public WURFLManager(IWURFLService wurflService, IWURFLRequestFactory wurflRequestFactory)
        {
            _wurflService = wurflService;
            _wurflRequestFactory = wurflRequestFactory;
        }

        #region IWURFLManager Members

        public IDevice GetDeviceForRequest(IWURFLRequest request)
        {
            return _wurflService.GetDeviceForRequest(request);
        }

        public IDevice GetDeviceForRequest(HttpRequest reqeuest)
        {
            IWURFLRequest wurflRequest = _wurflRequestFactory.CreateRequestFromHttpRequest(reqeuest);
            return GetDeviceForRequest(wurflRequest);
        }

        public IDevice GetDeviceForRequest(string userAgent)
        {
            IWURFLRequest wurflRequest = _wurflRequestFactory.CreateRequestFromUserAgent(userAgent);
            return GetDeviceForRequest(wurflRequest);
        }

        #endregion
    }
}