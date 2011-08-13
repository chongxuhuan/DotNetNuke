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
    internal class NecMatcher : AbstractMatcher
    {
        private const int NEC_LD_TOLLERANCE = 2;

        public NecMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }

        protected override string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            string match = null;
            if (userAgent.StartsWith("NEC"))
            {
                match = base.LookForMatchingUserAgent(userAgentsSet, userAgent);
            }
            else
            {
                logger.Debug("Applying LD(" + NEC_LD_TOLLERANCE + " UA: " + userAgent);                
                match = SearchUtils.LDSearch(userAgentsSet, userAgent, NEC_LD_TOLLERANCE);
            }

            return match;
        }
    }
}