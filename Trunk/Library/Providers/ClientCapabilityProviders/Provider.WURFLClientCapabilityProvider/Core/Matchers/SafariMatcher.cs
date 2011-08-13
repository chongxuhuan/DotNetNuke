/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.ClientCapability.Core.Classifiers;
using DotNetNuke.Services.ClientCapability.Hanldlers;
using DotNetNuke.Services.ClientCapability.Matchers.Strategy;

namespace DotNetNuke.Services.ClientCapability.Matchers
{
    internal class SafariMatcher : AbstractMatcher
    {
        private const int LONG_TOLLERANCE = 24;
        private const string regex = @"(.*)\s+(Nokia.*)/(\d+)\.(\d+)\.(\d+)(.*)";
        private const int SHORT_TOLLERANCE = 23;

        private readonly ITokensProvider<Token> LONG_TOKENS_PROVIDER = new SafariLongTokensProvider();
        private readonly ITokensProvider<Token> SHORT_TOKENS_PROVIDER = new SafariShortTokensProvider();


        public SafariMatcher(Classification classification, IHandler<string> handler)
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

            if (LONG_TOKENS_PROVIDER.CanApply(userAgent))
            {
                logger.Debug("Applying TK(" + LONG_TOLLERANCE + ") UA:" + userAgent);                
                match = SearchUtils.TokensSerach(userAgentsSet, userAgent, LONG_TOKENS_PROVIDER, LONG_TOLLERANCE);
            }
            else if (SHORT_TOKENS_PROVIDER.CanApply(userAgent))
            {
                logger.Debug("Applying TK(" + SHORT_TOLLERANCE + ") UA:" + userAgent);                
                match = SearchUtils.TokensSerach(userAgentsSet, userAgent, SHORT_TOKENS_PROVIDER, SHORT_TOLLERANCE);
            }

            return match;
        }
    }
}