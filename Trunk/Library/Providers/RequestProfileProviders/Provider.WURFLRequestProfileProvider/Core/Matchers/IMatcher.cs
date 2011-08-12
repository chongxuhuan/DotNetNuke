/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.Devices.Core.Matchers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IMatcher<T>
    {
        /// <summary>
        /// Matches the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        string Match(T request);

        /// <summary>
        /// Determines whether this instance can handle the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can handle the specified request; otherwise, <c>false</c>.
        /// </returns>
        bool CanHandle(T request);
    }
}