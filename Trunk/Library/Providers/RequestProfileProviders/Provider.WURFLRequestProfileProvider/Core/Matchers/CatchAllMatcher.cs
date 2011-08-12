/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.Devices.Core.Classifiers;
using DotNetNuke.Services.Devices.Core.Hanldlers;

namespace DotNetNuke.Services.Devices.Core.Matchers
{
    internal class CatchAllMatcher : AbstractMatcher
    {
        private const string MOZILLA = "Mozilla";
        private const string MOZILLA_4 = "Mozilla/4";
        private const string MOZILLA_5 = "Mozilla/5";
        private const int MOZILLA_LD_TOLLERANCE = 5;

        private const string NON_MOZILLA = "NON_MOZZILA";

        private readonly Classification _mozilla = new Classification(MOZILLA);
        private readonly Classification _mozilla4 = new Classification(MOZILLA_4);
        private readonly Classification _mozilla5 = new Classification(MOZILLA_5);

        private readonly Classification _nonMozilla = new Classification(NON_MOZILLA);

        public CatchAllMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
            ClassifyMozillaData(classification);
        }

        private void ClassifyMozillaData(Classification classification)
        {
            foreach (KeyValuePair<string, string> entry in classification.ClassifiedData)
            {
                if (entry.Key.StartsWith(MOZILLA))
                {
                    if (entry.Key.StartsWith(MOZILLA_4))
                    {
                        _mozilla4.Put(entry.Key, entry.Value);
                    }
                    else if (entry.Key.StartsWith(MOZILLA_5))
                    {
                        _mozilla5.Put(entry.Key, entry.Value);
                    }
                    else
                    {
                        _mozilla5.Put(entry.Key, entry.Value);
                    }
                }
                else
                {
                    _nonMozilla.Put(entry.Key, entry.Value);
                }
            }
        }


        protected override string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            string match = null;
            if (userAgent.StartsWith(MOZILLA))
            {
                if (userAgent.StartsWith(MOZILLA_4))
                {
                    match = SearchUtils
                        .LDSearch(_mozilla4.UserAgents, userAgent, MOZILLA_LD_TOLLERANCE);
                }
                else if (userAgent.StartsWith(MOZILLA_5))
                {
                    match = SearchUtils
                        .LDSearch(_mozilla5.UserAgents, userAgent, MOZILLA_LD_TOLLERANCE);
                }
                else
                {
                    match = SearchUtils.LDSearch(_mozilla.UserAgents, userAgent, MOZILLA_LD_TOLLERANCE);
                }
            }
            else
            {
                match = base.LookForMatchingUserAgent(_nonMozilla.UserAgents, userAgent);
            }

            return match;
        }

        private IList<string> GetMozillaData(IList<string> userAgentsSet, string p)
        {
            return new List<string>();
        }
    }
}