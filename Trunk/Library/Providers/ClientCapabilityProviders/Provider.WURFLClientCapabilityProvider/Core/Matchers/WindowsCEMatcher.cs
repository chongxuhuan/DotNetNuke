/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.ClientCapability.Core.Classifiers;
using DotNetNuke.Services.ClientCapability.Hanldlers;
using DotNetNuke.Services.ClientCapability.Request;

namespace DotNetNuke.Services.ClientCapability.Matchers
{
    internal class WindowsCEMatcher : AbstractMatcher
    {
        private const int WINDOWS_CE_LD_TOLLERANCE = 3;

        public WindowsCEMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }


        /// <summary>
        /// Apply LD with a threshold of 3
        /// </summary>
        /// <param name="userAgentsSet">The user agents set.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        protected override string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            logger.Debug("Applying LD(" + WINDOWS_CE_LD_TOLLERANCE + ") UA: " + userAgent);            
            return SearchUtils.LDSearch(userAgentsSet, userAgent, WINDOWS_CE_LD_TOLLERANCE);
        }

        /// <summary>
        /// return "ms_mobile_browser_ver1"
        /// </summary>
        /// <param name="classifiedData">The classified data.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected override string ApplyRecoveryMatch(IDictionary<string, string> classifiedData, IWURFLRequest request)
        {
            return "ms_mobile_browser_ver1";
        }
    }
}