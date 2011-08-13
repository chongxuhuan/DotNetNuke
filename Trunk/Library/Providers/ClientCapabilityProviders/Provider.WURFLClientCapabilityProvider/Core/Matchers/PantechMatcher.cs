/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.ClientCapability.Core.Classifiers;
using DotNetNuke.Services.ClientCapability.Hanldlers;

namespace DotNetNuke.Services.ClientCapability.Matchers
{
    internal class PantechMatcher : AbstractMatcher
    {
        private const int PANTECH_LD_TOLLERANCE = 4;

        public PantechMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }

        /// <summary>
        /// If starts with "PT-", "PG-" or "PANTECH", use RIS with FS Otherwise LD
        /// with threshold 4
        /// </summary>
        /// <param name="userAgentsSet">The user agents set.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        protected override string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            string match = null;

            if (userAgent.StartsWith("Pantech"))
            {
                logger.Debug("Applying LD(" + PANTECH_LD_TOLLERANCE + ") UA: " + userAgent);                
                match = SearchUtils.LDSearch(userAgentsSet, userAgent, PANTECH_LD_TOLLERANCE);
            }
            else
            {
                match = base.LookForMatchingUserAgent(userAgentsSet, userAgent);
            }

            return match;
        }
    }
}