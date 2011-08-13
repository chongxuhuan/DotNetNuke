/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.ClientCapability.Request;
using DotNetNuke.Services.ClientCapability.Resource;

namespace DotNetNuke.Services.ClientCapability.Matchers
{
    internal class Matcher : IMatcher<IWURFLRequest>
    {
        private readonly ICollection<IMatcher<IWURFLRequest>> _matchers;
        private IWURFLModel _wurflModel;

        public Matcher(IWURFLModel wurflModel, ICollection<IMatcher<IWURFLRequest>> matchers)
        {
            _wurflModel = wurflModel;
            _matchers = matchers;
        }

        #region IMatcher<IWURFLRequest> Members

        /// <summary>
        /// Matches the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public string Match(IWURFLRequest request)
        {
            string deviceID = Constants.GENERIC;

            foreach (IMatcher<IWURFLRequest> matcher in _matchers)
            {
                if (matcher.CanHandle(request))
                {
                    deviceID = matcher.Match(request);
                    break;
                }
            }

            return deviceID;
        }

        public bool CanHandle(IWURFLRequest request)
        {
            return true;
        }

        #endregion
    }
}