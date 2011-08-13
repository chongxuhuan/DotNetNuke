/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.ClientCapability.Hanldlers
{
    internal class SamsungHandler : AbstractHandler
    {
        public SamsungHandler(string id) : base(id)
        {
        }


        /// <summary>
        /// Intercept all UAs containing "Samsung/SGH" or starting with one of the
        /// following "SEC-", "Samsung", "SAMSUNG", "SPH", "SGH", "SCH"
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can handle the specified user agent; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandle(string userAgent)
        {
            return (userAgent.Contains("Samsung/SGH")
                    || userAgent.StartsWith("SEC-")
                    || userAgent.StartsWith("Samsung")
                    || userAgent.StartsWith("SAMSUNG")
                    || userAgent.StartsWith("SPH") || userAgent.StartsWith("SGH") || userAgent
                                                                                         .StartsWith("SCH"));
        }
    }
}