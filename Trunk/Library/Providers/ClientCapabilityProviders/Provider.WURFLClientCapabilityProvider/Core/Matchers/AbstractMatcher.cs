/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;
using DotNetNuke.Services.ClientCapability.Core.Classifiers;
using DotNetNuke.Services.ClientCapability.Hanldlers;
using DotNetNuke.Services.ClientCapability.Request;
using DotNetNuke.Services.ClientCapability.Utils;
using DotNetNuke.Instrumentation;

namespace DotNetNuke.Services.ClientCapability.Matchers
{
    /// <summary>
    /// Base Class for all matchers
    /// </summary>
    internal abstract class AbstractMatcher : IMatcher<IWURFLRequest>
    {
        protected static readonly DnnLogger UNDETECTED_WURFL_DEVICES = DnnLogger.GetLogger(Constants.UNDETECTED_WURFL_DEVICES.ToString());

        private readonly Classification _classification;
        private readonly IHandler<string> _handler;

        protected readonly DnnLogger logger =DnnLogger.GetLogger(typeof(AbstractMatcher).FullName);

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMatcher"/> class.
        /// </summary>
        /// <param name="classification">The classification.</param>
        /// <param name="handler">The handler.</param>
        public AbstractMatcher(Classification classification, IHandler<string> handler)
        {
            _classification = classification;
            _handler = handler;
        }

        public IHandler<string> Handler
        {
            get { return _handler; }
        }


        public Classification Classificaion
        {
            get { return _classification; }
        }

        #region IMatcher<IWURFLRequest> Members

        /// <summary>
        /// Determines whether this instance can handle the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can handle the specified request; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(IWURFLRequest request)
        {
            return _handler.CanHandle(request.UserAgent);
        }

        /// <summary>
        /// Matches the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public string Match(IWURFLRequest request)
        {
            IDictionary<string, string> classifiedData = _classification.ClassifiedData;

            // 1) Try Direct Match
            string deviceID = null;

            classifiedData.TryGetValue(request.UserAgent, out deviceID);

            if (!IsNullOrGeneric(deviceID))
            {
                return deviceID;
            }

            // 2) Apply Conclusive Match
            deviceID = ApplyConclusiveMatch(classifiedData, request);
            if (!IsNullOrGeneric(deviceID))
            {
                return deviceID;
            }
            else
            {
                
                // LOG The the UA Profile
                UNDETECTED_WURFL_DEVICES.DebugFormat("{0} : {1}", request.UserAgent, request.UserAgentProfile);
            }

            // 3) Apply Recovery Match
            deviceID = ApplyRecoveryMatch(classifiedData, request);
            if (!IsNullOrGeneric(deviceID))
            {
                return deviceID;
            }

            // 4) Apply Catch All Recovery
            deviceID = ApplyCatchAllRecoveryMatch(classifiedData, request);

            return deviceID;
        }

        #endregion

        /// <summary>
        /// Applies the conclusive match.
        /// </summary>
        /// <param name="classifiedData">The classified data.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected virtual string ApplyConclusiveMatch(IDictionary<string, string> classifiedData, IWURFLRequest request)
        {
            logger.Debug("Matching UA: " + request.UserAgent + " against devices: " + classifiedData.Keys);
            
            string match = LookForMatchingUserAgent(new List<string>(classifiedData.Keys), request.UserAgent);

            // Log undetected

            String deviceID = Constants.GENERIC;

            if (!String.IsNullOrEmpty(match))
            {
                classifiedData.TryGetValue(match, out deviceID);
            }

            if (String.IsNullOrEmpty(deviceID))
            {
                deviceID = Constants.GENERIC;
            }

            return deviceID;
        }


        /// <summary>
        /// Looks for matching user agent.
        /// </summary>
        /// <param name="userAgentsSet">The user agents set.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        protected virtual string LookForMatchingUserAgent(IList<string> userAgentsSet, string userAgent)
        {
            int tollerance = StringUtils.FirstSlash(userAgent);
            logger.Debug("Applying RIS(FS) UA: " + userAgent);            
            return SearchUtils.RISSearch(userAgentsSet, userAgent, tollerance);
        }


        /// <summary>
        /// Applies the recovery match.
        /// </summary>
        /// <param name="classifiedData">The classified data.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected virtual string ApplyRecoveryMatch(IDictionary<string, string> classifiedData, IWURFLRequest request)
        {
            return Constants.GENERIC;
        }

        /// <summary>
        /// Applies the catch all recovery match.
        /// </summary>
        /// <param name="classifiedData">The classified data.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected string ApplyCatchAllRecoveryMatch(IDictionary<string, string> classifiedData, IWURFLRequest request)
        {
            String userAgent = request.UserAgent;

            // Openwave            
            if (userAgent.IndexOf("UP.Browser/7.2") != -1)
            {
                return "opwv_v72_generic";
            }
            if (userAgent.IndexOf("UP.Browser/7") != -1)
            {
                return "opwv_v7_generic";
            }
            if (userAgent.IndexOf("UP.Browser/6.2") != -1)
            {
                return "opwv_v62_generic";
            }
            if (userAgent.IndexOf("UP.Browser/6") != -1)
            {
                return "opwv_v6_generic";
            }
            if (userAgent.IndexOf("UP.Browser/5") != -1)
            {
                return "upgui_generic";
            }
            if (userAgent.IndexOf("UP.Browser/4") != -1)
            {
                return "uptext_generic";
            }
            if (userAgent.IndexOf("UP.Browser/3") != -1)
            {
                return "uptext_generic";
            }

            // Series 60
            if (userAgent.IndexOf("Series60") != -1)
            {
                return "nokia_generic_series60";
            }

            // Access/Net Front
            if (userAgent.IndexOf("NetFront/3.0") != -1
                || userAgent.IndexOf("ACS-NF/3.0") != -1)
            {
                return "netfront_ver3";
            }
            if (userAgent.IndexOf("NetFront/3.1") != -1
                || userAgent.IndexOf("ACS-NF/3.1") != -1)
            {
                return "netfront_ver3_1";
            }
            if (userAgent.IndexOf("NetFront/3.2") != -1
                || userAgent.IndexOf("ACS-NF/3.2") != -1)
            {
                return "netfront_ver3_2";
            }
            if (userAgent.IndexOf("NetFront/3.3") != -1
                || userAgent.IndexOf("ACS-NF/3.3") != -1)
            {
                return "netfront_ver3_3";
            }
            if (userAgent.IndexOf("NetFront/3.4") != -1)
            {
                return "netfront_ver3_4";
            }
            if (userAgent.IndexOf("NetFront/3.5") != -1)
            {
                return "netfront_ver3_5";
            }

            // Windows CE

            if (userAgent.IndexOf("Windows CE") != -1)
            {
                return "ms_mobile_browser_ver1";
            }

            if (userAgent.IndexOf("Mozilla/") != -1)
            {
                return Constants.GENERIC_XHTML;
            }

            if ((userAgent.IndexOf("ObigoInternetBrowser/Q03C") != -1)
                || userAgent.IndexOf("AU-MIC/2") != -1
                || userAgent.IndexOf("AU-MIC-") != -1
                || userAgent.IndexOf("AU-OBIGO/") != -1
                || userAgent.IndexOf("Obigo/Q03") != -1
                || userAgent.IndexOf("Obigo/Q04") != -1
                || userAgent.IndexOf("ObigoInternetBrowser/2") != -1
                || userAgent.IndexOf("Teleca Q03B1") != -1)
            {
                return Constants.GENERIC_XHTML;
            }

            // web browsers?
            if (userAgent.IndexOf("Mozilla/4.0") != -1)
            {
                return "generic_web_browser";
            }
            if (userAgent.IndexOf("Mozilla/5.0") != -1)
            {
                return "generic_web_browser";
            }
            if (userAgent.IndexOf("Mozilla/6.0") != -1)
            {
                return "generic_web_browser";
            }

            // Opera Mini
            if (userAgent.IndexOf("Opera Mini/1") != -1)
            {
                return "opera_mini_ver1";
            }
            if (userAgent.IndexOf("Opera Mini/2") != -1)
            {
                return "opera_mini_ver2";
            }
            if (userAgent.IndexOf("Opera Mini/3") != -1)
            {
                return "opera_mini_ver3";
            }
            if (userAgent.IndexOf("Opera Mini/4") != -1)
            {
                return "opera_mini_ver4";
            }

            // DoCoMo
            if (userAgent.StartsWith("DoCoMo") || userAgent.StartsWith("KDDI"))
            {
                return "docomo_generic_jap_ver1";
            }


            // Try if the the requester is xhtml device
            if (request.IsXHTMLDevice)
            {
                return Constants.GENERIC_XHTML;
            }

            return Constants.GENERIC;
        }


        private bool IsNullOrGeneric(string deviceID)
        {
            return String.IsNullOrEmpty(deviceID) || String.Equals("generic", deviceID);
        }
    }
}