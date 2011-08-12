/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.Devices.Core.Classifiers;
using DotNetNuke.Services.Devices.Core.Hanldlers;
using DotNetNuke.Services.Devices.Core.Matchers.Strategy;

namespace DotNetNuke.Services.Devices.Core.Matchers
{
    internal class VodafoneMatcher : AbstractMatcher
    {
        private const int VODAFONE_LD_TOLLERANCE = 5;
        private static readonly RegExTokensProvider nokiaDDDTokensProvider = new NokiaDDDProvider();

        public VodafoneMatcher(Classification classification, IHandler<string> handler)
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
            string match = null;
            if (userAgent.Contains("Nokia"))
            {
                logger.Debug("Applying TK(" + 9 + ") UA:" + userAgent);                
                match = SearchUtils.TokensSerach(userAgentsSet, userAgent, nokiaDDDTokensProvider, 9);
            }
            else
            {
                match = SearchUtils.LDSearch(userAgentsSet, userAgent, VODAFONE_LD_TOLLERANCE);
            }


            return match;
        }
    }
}