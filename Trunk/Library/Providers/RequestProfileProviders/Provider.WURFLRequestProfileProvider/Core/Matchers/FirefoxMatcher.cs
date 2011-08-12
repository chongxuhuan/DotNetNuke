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
    internal class FirefoxMatcher : AbstractMatcher
    {
        private const int FIREFOX_TOLLERANCE = 2;

        private static readonly RegExTokensProvider firefoxTokensProvider = new FirefoxTokensProvider();

        public FirefoxMatcher(Classification classification, IHandler<string> handler)
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
            if (firefoxTokensProvider.CanApply(userAgent))
            {
                logger.Debug("Applying TK(" + FIREFOX_TOLLERANCE + ") UA:" + userAgent);
                

                match = SearchUtils.TokensSerach(userAgentsSet, userAgent, firefoxTokensProvider, FIREFOX_TOLLERANCE);
            }
            else
            {
                match = SearchUtils.LDSearch(userAgentsSet, userAgent, 5);
            }

            return match;
        }
    }
}