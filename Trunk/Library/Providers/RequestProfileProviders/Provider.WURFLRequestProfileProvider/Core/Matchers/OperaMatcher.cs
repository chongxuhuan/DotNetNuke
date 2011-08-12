/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.Devices.Core.Classifiers;
using DotNetNuke.Services.Devices.Core.Hanldlers;

namespace DotNetNuke.Services.Devices.Core.Matchers
{
    internal class OperaMatcher : AbstractMatcher
    {
        private const int OPERA_LD_TOLLERANCE = 5;

        public OperaMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }

        /// <summary>
        /// Looks for matching user agent.
        /// </summary>
        /// <param name="userAgentsSet">The user agents set.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        protected override string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            logger.Debug("Applying LD with threshold: " + OPERA_LD_TOLLERANCE);
            
            return SearchUtils.LDSearch(userAgentsSet, userAgent, OPERA_LD_TOLLERANCE);
        }
    }
}