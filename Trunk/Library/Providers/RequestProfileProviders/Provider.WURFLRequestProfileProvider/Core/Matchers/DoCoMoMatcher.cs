/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.Devices.Core.Classifiers;
using DotNetNuke.Services.Devices.Core.Hanldlers;
using DotNetNuke.Services.Devices.Core.Request;

namespace DotNetNuke.Services.Devices.Core.Matchers
{
    internal class DoCoMoMatcher : AbstractMatcher
    {
        public DoCoMoMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }

        /// <summary>
        /// ONLY DIRECT MATCH
        /// </summary>
        /// <param name="userAgentsSet">The user agents set.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        protected override string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            return null;
        }

        /// <summary>
        /// return "docomo_generic_jap_ver1"
        /// </summary>
        /// <param name="classifiedData">The classified data.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected override string ApplyRecoveryMatch(IDictionary<string, string> classifiedData, IWURFLRequest request)
        {
            return "docomo_generic_jap_ver1";
        }
    }
}