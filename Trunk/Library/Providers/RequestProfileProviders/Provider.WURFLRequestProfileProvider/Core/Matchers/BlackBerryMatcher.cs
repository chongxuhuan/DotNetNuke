/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;
using DotNetNuke.Services.Devices.Core.Classifiers;
using DotNetNuke.Services.Devices.Core.Hanldlers;
using DotNetNuke.Services.Devices.Core.Request;

namespace DotNetNuke.Services.Devices.Core.Matchers
{
    internal class BlackBerryMatcher : AbstractMatcher
    {
        public BlackBerryMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }


        /// <summary>
        ///
        /// Parse version number ("2","3.2","3.3","3.5","3.6","3.7","4.") and return
        ///"blackberry_generic_ver2","blackberry_generic_ver3_sub2",
        ///"blackberry_generic_ver3_sub30", "blackberry_generic_ver3_sub50",
        ///"blackberry_generic_ver3_sub60","blackberry_generic_ver3_sub70" or
        ///"blackberry_generic_ver4" respectively	 
        /// </summary>
        /// <param name="classifiedData">The classified data.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected override string ApplyRecoveryMatch(IDictionary<string, string> classifiedData, IWURFLRequest request)
        {
            String userAgent = request.UserAgent;
            if (userAgent.StartsWith("BlackBerry"))
            {
                int position = userAgent.IndexOf('/');

                if (position != -1 && position + 4 <= userAgent.Length)
                {
                    String version = userAgent.Substring(position + 1, position + 4);

                    if (version.StartsWith("2."))
                    {
                        return "blackberry_generic_ver2";
                    }
                    if (version.StartsWith("3.2"))
                    {
                        return "blackberry_generic_ver3_sub2";
                    }
                    if (version.StartsWith("3.3"))
                    {
                        return "blackberry_generic_ver3_sub30";
                    }
                    if (version.StartsWith("3.5"))
                    {
                        return "blackberry_generic_ver3_sub50";
                    }
                    if (version.StartsWith("3.6"))
                    {
                        return "blackberry_generic_ver3_sub60";
                    }
                    if (version.StartsWith("3.7"))
                    {
                        return "blackberry_generic_ver3_sub70";
                    }
                    if (version.StartsWith("4."))
                    {
                        return "blackberry_generic_ver4";
                    }
                    logger.Warn("No version matched, User-Agent: " + userAgent + " version: " + version);
                    
                }
            }
            return Constants.GENERIC;
        }
    }
}