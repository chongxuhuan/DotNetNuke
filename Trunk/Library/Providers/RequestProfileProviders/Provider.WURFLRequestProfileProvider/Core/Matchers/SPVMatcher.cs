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
    internal class SPVMatcher : AbstractMatcher
    {
        private const int SPV_LD_TOLLERANCE = 5;
        private static readonly RegExTokensProvider spvOpVerTokensProvider = new SPVOpVerProvider();

        public SPVMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }


        /// <summary>
        /// If "OpVer x.x.x.x is present, then apply TokensMatcher with threshold 7,
        /// otherwise apply LD with threshold 5.
        /// </summary>
        /// <param name="userAgentsSet">The user agents set.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        protected override string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            string match = null;
            if (spvOpVerTokensProvider.CanApply(userAgent))
            {
                logger.Debug("Applying TK(" + 9 + ") UA:" + userAgent);                
                match = SearchUtils.TokensSerach(userAgentsSet, userAgent, spvOpVerTokensProvider, 7);
            }
            else
            {
                logger.Debug("Applying LD with threshold: " + SPV_LD_TOLLERANCE);                
                match = SearchUtils.LDSearch(userAgentsSet, userAgent, SPV_LD_TOLLERANCE);
            }

            return match;
        }
    }
}