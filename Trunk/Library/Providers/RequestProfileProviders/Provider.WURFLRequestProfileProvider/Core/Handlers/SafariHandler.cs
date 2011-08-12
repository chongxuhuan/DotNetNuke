/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using DotNetNuke.Services.Devices.Core.Utils;

namespace DotNetNuke.Services.Devices.Core.Hanldlers
{
    internal class SafariHandler : AbstractHandler
    {
        private const string SAFARI = "Safari";

        public SafariHandler(string id) : base(id)
        {
        }

        /// <summary>
        /// Intercept all UAs containing Safari
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can handle the specified user agent; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandle(string userAgent)
        {
            if (UserAgentUtils.IsMobileBrowser(userAgent))
            {
                return false;
            }

            return userAgent.Contains(SAFARI);
        }
    }
}