/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.Devices.Core.Hanldlers
{
    public class MotorolaHandler : AbstractHandler
    {
        public MotorolaHandler(string id) : base(id)
        {
        }

        /// <summary>
        /// Intercept all UAs starting with "Mot-", or containing "MOT-" or "Motorola".
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can handle the specified user agent; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandle(string userAgent)
        {
            return (userAgent.StartsWith("Mot-") || userAgent.Contains("MOT-") || userAgent
                                                                                      .Contains("Motorola"));
        }
    }
}