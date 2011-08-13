/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.ClientCapability.Core.Classifiers;
using DotNetNuke.Services.ClientCapability.Hanldlers;
using DotNetNuke.Services.ClientCapability.Utils;

namespace DotNetNuke.Services.ClientCapability.Matchers
{
    internal class SamsungMatcher : AbstractMatcher
    {
        private const int Samsung_LD_TOLLERANCE = 5;

        public SamsungMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }


        protected override string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            string match = null;
            // RIS(FS)
            if (userAgent.StartsWith("SEC-") || userAgent.StartsWith("SAMSUNG-") || userAgent.StartsWith("SCH"))
            {
                match = base.LookForMatchingUserAgent(userAgentsSet, userAgent);
            }

                // RIS(FP)
            else if (userAgent.StartsWith("Samsung") || userAgent.StartsWith("SPH") || userAgent.StartsWith("SGH"))
            {
                int tollerance = StringUtils.FirstSpace(userAgent);
                logger.Debug("Applying RIS(FPi se) UA: " + userAgent);               
                match = SearchUtils.RISSearch(userAgentsSet, userAgent, tollerance);
            }

                // RIS(SS)
            else if (userAgent.StartsWith("SAMSUNG/"))
            {
                int tollerance = StringUtils.SecondSlash(userAgent);
                logger.Debug("Applying RIS(SS) UA: " + userAgent);                
                match = SearchUtils.RISSearch(userAgentsSet, userAgent, tollerance);
            }


            return match;
        }
    }
}