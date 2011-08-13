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
    internal class MatchersChainFactory : IChainFactory<IMatcher<IWURFLRequest>>
    {
        private readonly ICollection<Classification> _classifications;
        private readonly ICollection<IHandler<string>> _handlers;
        private readonly ICollection<IMatcher<IWURFLRequest>> _matchers = new List<IMatcher<IWURFLRequest>>();


        /// <summary>
        /// Initializes a new instance of the <see cref="MatchersChainFactory"/> class.
        /// </summary>
        /// <param name="classifications">The classifications.</param>
        /// <param name="handlers">The handlers.</param>
        public MatchersChainFactory(ICollection<Classification> classifications, ICollection<IHandler<string>> handlers)
        {
            _classifications = classifications;
            _handlers = handlers;
        }

        #region IChainFactory<IMatcher<IWURFLRequest>> Members

        public ICollection<IMatcher<IWURFLRequest>> Create()
        {
            if (_matchers.Count == 0)
            {
                Initialize();
            }

            return new List<IMatcher<IWURFLRequest>>(_matchers).AsReadOnly();
        }

        #endregion

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            IDictionary<string, IHandler<string>> handlersMap = new Dictionary<string, IHandler<string>>();
            IDictionary<string, Classification> classificationsMap = new Dictionary<string, Classification>();

            foreach (IHandler<string> handler in _handlers)
            {
                handlersMap.Add(handler.ID, handler);
            }

            foreach (Classification classification in _classifications)
            {
                classificationsMap.Add(classification.ID, classification);
            }

            _matchers.Add(new VodafoneMatcher(classificationsMap[HandlerConstants.VODAFONE],
                                              handlersMap[HandlerConstants.VODAFONE]));
            _matchers.Add(new NokiaMatcher(classificationsMap[HandlerConstants.NOKIA],
                                           handlersMap[HandlerConstants.NOKIA]));
            _matchers.Add(new SonyEricssonMatcher(classificationsMap[HandlerConstants.SONY_ERICSSON],
                                                  handlersMap[HandlerConstants.SONY_ERICSSON]));

            _matchers.Add(new MotorolaMatcher(classificationsMap[HandlerConstants.MOTOROLA],
                                              handlersMap[HandlerConstants.MOTOROLA]));
            _matchers.Add(new BlackBerryMatcher(classificationsMap[HandlerConstants.BLACKBERRY],
                                                handlersMap[HandlerConstants.BLACKBERRY]));
            _matchers.Add(new SiemensMatcher(classificationsMap[HandlerConstants.SIEMENS],
                                             handlersMap[HandlerConstants.SIEMENS]));


            _matchers.Add(new SagemMatcher(classificationsMap[HandlerConstants.SAGEM],
                                           handlersMap[HandlerConstants.SAGEM]));
            _matchers.Add(new SamsungMatcher(classificationsMap[HandlerConstants.SAMSUNG],
                                             handlersMap[HandlerConstants.SAMSUNG]));


            //--------------------------------------------
            _matchers.Add(new PanasonicMatcher(classificationsMap[HandlerConstants.PANASONIC],
                                               handlersMap[HandlerConstants.PANASONIC]));
            _matchers.Add(new NecMatcher(classificationsMap[HandlerConstants.NEC], handlersMap[HandlerConstants.NEC]));
            _matchers.Add(new QtekMatcher(classificationsMap[HandlerConstants.QTEK], handlersMap[HandlerConstants.QTEK]));


            _matchers.Add(new MitsubishiMatcher(classificationsMap[HandlerConstants.MITSUBISHI],
                                                handlersMap[HandlerConstants.MITSUBISHI]));
            _matchers.Add(new PhilipsMatcher(classificationsMap[HandlerConstants.PHILIPS],
                                             handlersMap[HandlerConstants.PHILIPS]));
            _matchers.Add(new LGMatcher(classificationsMap[HandlerConstants.LG], handlersMap[HandlerConstants.LG]));

            _matchers.Add(new AppleMatcher(classificationsMap[HandlerConstants.APPLE],
                                           handlersMap[HandlerConstants.APPLE]));


            _matchers.Add(new KyoceraMatcher(classificationsMap[HandlerConstants.KYOCERA],
                                             handlersMap[HandlerConstants.KYOCERA]));
            _matchers.Add(new AlcatelMatcher(classificationsMap[HandlerConstants.ALCATEL],
                                             handlersMap[HandlerConstants.ALCATEL]));
            _matchers.Add(new SharpMatcher(classificationsMap[HandlerConstants.SHARP],
                                           handlersMap[HandlerConstants.SHARP]));
            _matchers.Add(new SanyoMatcher(classificationsMap[HandlerConstants.SANYO],
                                           handlersMap[HandlerConstants.SANYO]));

            _matchers.Add(new BenQMatcher(classificationsMap[HandlerConstants.BENQ], handlersMap[HandlerConstants.BENQ]));
            _matchers.Add(new PantechMatcher(classificationsMap[HandlerConstants.PANTECH],
                                             handlersMap[HandlerConstants.PANTECH]));


            //--------------------------------------------
            _matchers.Add(new ToshibaMatcher(classificationsMap[HandlerConstants.TOSHIBA],
                                             handlersMap[HandlerConstants.TOSHIBA]));
            _matchers.Add(new GrundigMatcher(classificationsMap[HandlerConstants.GRUNDIG],
                                             handlersMap[HandlerConstants.GRUNDIG]));

            _matchers.Add(new HTCMatcher(classificationsMap[HandlerConstants.HTC], handlersMap[HandlerConstants.HTC]));


            _matchers.Add(new SPVMatcher(classificationsMap[HandlerConstants.SPV], handlersMap[HandlerConstants.SPV]));
            _matchers.Add(new WindowsCEMatcher(classificationsMap[HandlerConstants.WINDOWS_CE],
                                               handlersMap[HandlerConstants.WINDOWS_CE]));
            _matchers.Add(new PortalmmmMatcher(classificationsMap[HandlerConstants.PORTALMMM],
                                               handlersMap[HandlerConstants.PORTALMMM]));

            _matchers.Add(new DoCoMoMatcher(classificationsMap[HandlerConstants.DOCOMO],
                                            handlersMap[HandlerConstants.DOCOMO]));
            _matchers.Add(new KDDIMatcher(classificationsMap[HandlerConstants.KDDDI],
                                          handlersMap[HandlerConstants.KDDDI]));


            //// WEB Browser Handlers
            _matchers.Add(new AOLMatcher(classificationsMap[HandlerConstants.AOL], handlersMap[HandlerConstants.AOL]));
            _matchers.Add(new OperaMatcher(classificationsMap[HandlerConstants.OPERA],
                                           handlersMap[HandlerConstants.OPERA]));
            _matchers.Add(new FirefoxMatcher(classificationsMap[HandlerConstants.FIREFOX],
                                             handlersMap[HandlerConstants.FIREFOX]));
            _matchers.Add(new SafariMatcher(classificationsMap[HandlerConstants.SAFARI],
                                            handlersMap[HandlerConstants.SAFARI]));
            _matchers.Add(new MSIEMatcher(classificationsMap[HandlerConstants.MSIE], handlersMap[HandlerConstants.MSIE]));

            // Catch All Handler
            _matchers.Add(new CatchAllMatcher(classificationsMap[HandlerConstants.CATCH_ALL],
                                              handlersMap[HandlerConstants.CATCH_ALL]));
        }
    }
}