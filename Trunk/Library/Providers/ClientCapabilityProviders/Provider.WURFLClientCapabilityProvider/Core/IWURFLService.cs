/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using DotNetNuke.Services.ClientCapability.Request;

namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWURFLService
    {
        /// <summary>
        /// Gets the device for request.
        /// </summary>
        /// <param name="wurflRequest">The wurfl request.</param>
        /// <returns></returns>
        IDevice GetDeviceForRequest(IWURFLRequest wurflRequest);
    }
}