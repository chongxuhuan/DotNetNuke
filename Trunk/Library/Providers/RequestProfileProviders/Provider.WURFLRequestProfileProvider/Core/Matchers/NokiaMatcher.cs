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
using DotNetNuke.Services.Devices.Core.Request;
using DotNetNuke.Services.Devices.Core.Utils;

namespace DotNetNuke.Services.Devices.Core.Matchers
{
    internal class NokiaMatcher : AbstractMatcher
    {
        private const int NOKIA_LD_TOLLERANCE = 4;
        private static readonly RegExTokensProvider nokiaDDDTokensProvider = new NokiaDDDProvider();

        public NokiaMatcher(Classification classification, IHandler<string> handler)
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
            if (userAgent.StartsWith("Nokia"))
            {
                int tollerance = StringUtils.FirstSlash(userAgent);
                logger.Debug("Applying RIS(FS) UA: " + userAgent);                
                match = SearchUtils.RISSearch(userAgentsSet, userAgent, tollerance);
            }
            else if (nokiaDDDTokensProvider.CanApply(userAgent))
            {
                logger.Debug("Applying TK(" + 9 + ") UA:" + userAgent);                
                match = SearchUtils.TokensSerach(userAgentsSet, userAgent, nokiaDDDTokensProvider, 9);
            }
            else
            {                
                logger.Debug("Applying LD with threshold: " + NOKIA_LD_TOLLERANCE);                
                match = SearchUtils.LDSearch(userAgentsSet, userAgent, NOKIA_LD_TOLLERANCE);
            }

            return match;
        }


        protected override string ApplyRecoveryMatch(IDictionary<string, string> classifiedData, IWURFLRequest request)
        {
            string userAgent = request.UserAgent;

            if (userAgent.Contains("Series60"))
            {
                return "nokia_generic_series60";
            }
            if (userAgent.Contains("Series80"))
            {
                return "nokia_generic_series80";
            }
            return Constants.GENERIC;
        }
    }
}