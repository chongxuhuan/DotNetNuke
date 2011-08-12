/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.Devices.Core.Classifiers;
using DotNetNuke.Services.Devices.Core.Hanldlers;
using DotNetNuke.Services.Devices.Core.Utils;

namespace DotNetNuke.Services.Devices.Core.Matchers
{
    internal class SonyEricssonMatcher : AbstractMatcher
    {
        public SonyEricssonMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }


        /// <summary>
        /// If UA starts with "SonyEricsson", apply RIS with FS as a threshold. If UA
        /// contains "SonyEricsson" somewhere in the middle, apply RIS with threshold
        /// second slash
        /// </summary>
        /// <param name="userAgentsSet">The user agents set.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        protected override string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            string match = null;
            if (userAgent.StartsWith("SonyEricsson"))
            {
                match = base.LookForMatchingUserAgent(userAgentsSet, userAgent);
            }
            else
            {
                int tollerance = StringUtils.SecondSlash(userAgent);
                logger.Debug("Appling RIS(SS) UA: " + userAgent);                
                match = SearchUtils.RISSearch(userAgentsSet, userAgent, tollerance);
            }

            return match;
        }
    }
}