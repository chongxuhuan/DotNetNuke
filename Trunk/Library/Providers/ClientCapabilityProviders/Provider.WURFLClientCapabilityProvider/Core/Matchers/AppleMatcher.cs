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
    internal class AppleMatcher : AbstractMatcher
    {
        private const int APPLE_LD_TOLLERANCE = 5;

        public AppleMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }

        protected override string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            logger.Debug("Applying LD(" + APPLE_LD_TOLLERANCE + ") UA: "
                             + userAgent);            

            return SearchUtils.LDSearch(userAgentsSet, userAgent, APPLE_LD_TOLLERANCE);
        }

        /// <summary>
        /// if the UA contains "iPhone" return "apple_iphone_ver1" if the UA contains
        /// "iPod" return "apple_ipod_touch_ver1"
        /// </summary>
        /// <param name="classifiedData">The classified data.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected override string ApplyRecoveryMatch(IDictionary<string, string> classifiedData, IWURFLRequest request)
        {
            string match = "apple_ipod_touch_ver1";

            if (request.UserAgent.Contains("iPhone"))
            {
                match = "apple_iphone_ver1";
            }
            return match;
        }
    }
}