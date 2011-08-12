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
    internal class MSIEMatcher : AbstractMatcher
    {
        private const int MSIE_TOLLERANCE = 0;

        private static readonly ITokensProvider<Token> msieTokensProvider = new MSIETokensProvider();

        public MSIEMatcher(Classification classification, IHandler<string> handler)
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
            logger.Debug("Applying TK(" + MSIE_TOLLERANCE + ") UA:" + userAgent);            
            return SearchUtils.TokensSerach(userAgentsSet, userAgent, msieTokensProvider, MSIE_TOLLERANCE);
        }
    }
}