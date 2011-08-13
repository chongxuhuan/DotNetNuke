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
using DotNetNuke.Services.ClientCapability.Utils;

namespace DotNetNuke.Services.ClientCapability.Matchers
{
    internal class KDDIMatcher : AbstractMatcher
    {
        public KDDIMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }

        /// <summary>
        /// if UA starts with "KDDI/", apply RIS with Second Slash Otherwise apply
        /// RIS with FS
        /// </summary>
        /// <param name="userAgentsSet">The user agents set.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        protected override string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            string match = null;
            if (userAgent.StartsWith("KDDI/"))
            {
                int tollerance = StringUtils.SecondSlash(userAgent);
                logger.Debug("Applying RIS(SS) UA: " + userAgent);                
                match = SearchUtils.RISSearch(userAgentsSet, userAgent, tollerance);
            }
            else
            {
                match = base.LookForMatchingUserAgent(userAgentsSet, userAgent);
            }

            return match;
        }

        /// <summary>
        /// return "opwv_v62_generic"
        /// </summary>
        /// <param name="classifiedData">The classified data.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected override string ApplyRecoveryMatch(IDictionary<string, string> classifiedData, IWURFLRequest request)
        {
            return "opwv_v62_generic";
        }
    }
}