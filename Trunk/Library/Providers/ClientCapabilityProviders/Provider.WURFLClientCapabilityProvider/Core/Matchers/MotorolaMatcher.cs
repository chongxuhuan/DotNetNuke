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
    internal class MotorolaMatcher : AbstractMatcher
    {
        private const int MOTOROLA_LD_TOLLERANCE = 5;

        public MotorolaMatcher(Classification classification, IHandler<string> handler)
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
            if (userAgent.StartsWith("Mot-") || userAgent.StartsWith("MOT-")
                || userAgent.StartsWith("Motorola"))
            {
                match = base.LookForMatchingUserAgent(userAgentsSet, userAgent);
            }
            else
            {
                match = SearchUtils.LDSearch(userAgentsSet, userAgent, MOTOROLA_LD_TOLLERANCE);
            }

            return match;
        }


        /// <summary>
        /// If UA contains "MIB/2.2" or "MIB/BER2.2", return "mot_mib22_generic"
        /// </summary>
        /// <param name="classifiedData">The classified data.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected override string ApplyRecoveryMatch(IDictionary<string, string> classifiedData, IWURFLRequest request)
        {
            string userAgent = request.UserAgent;

            if (userAgent.Contains("MIB/2.2") || userAgent.Contains("MIB/BER2.2"))
                return "mot_mib22_generic";

            return Constants.GENERIC;
        }
    }
}