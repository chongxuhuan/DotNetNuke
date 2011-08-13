/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;

namespace DotNetNuke.Services.ClientCapability.Hanldlers
{
    internal class HandlersFactory : IChainFactory<IHandler<string>>
    {
        #region IChainFactory<IHandler<string>> Members

        public ICollection<IHandler<string>> Create()
        {
            List<IHandler<string>> handlers = new List<IHandler<string>>();

            handlers.Add(new VodafoneHandler(HandlerConstants.VODAFONE));
            handlers.Add(new NokiaHandler(HandlerConstants.NOKIA));

            handlers.Add(new SonyEricssonHandler(HandlerConstants.SONY_ERICSSON));

            handlers.Add(new MotorolaHandler(HandlerConstants.MOTOROLA));
            handlers.Add(new BlackBerryHandler(HandlerConstants.BLACKBERRY));

            handlers.Add(new SiemensHandler(HandlerConstants.SIEMENS));
            handlers.Add(new SagemHandler(HandlerConstants.SAGEM));

            handlers.Add(new SamsungHandler(HandlerConstants.SAMSUNG));


            handlers.Add(new PanasonicHandler(HandlerConstants.PANASONIC));

            handlers.Add(new NecHandler(HandlerConstants.NEC));
            handlers.Add(new QtekHandler(HandlerConstants.QTEK));

            handlers.Add(new MitsubishiHandler(HandlerConstants.MITSUBISHI));
            handlers.Add(new PhilipsHandler(HandlerConstants.PHILIPS));

            handlers.Add(new LGHandler(HandlerConstants.LG));
            handlers.Add(new AppleHandler(HandlerConstants.APPLE));

            handlers.Add(new KyoceraHandler(HandlerConstants.KYOCERA));
            handlers.Add(new AlcatelHandler(HandlerConstants.ALCATEL));

            handlers.Add(new SharpHandler(HandlerConstants.SHARP));
            handlers.Add(new SanyoHandler(HandlerConstants.SANYO));

            handlers.Add(new BenQHandler(HandlerConstants.BENQ));
            handlers.Add(new PantechHandler(HandlerConstants.PANTECH));

            handlers.Add(new ToshibaHandler(HandlerConstants.TOSHIBA));
            handlers.Add(new GrundigHandler(HandlerConstants.GRUNDIG));

            handlers.Add(new HTCHandler(HandlerConstants.HTC));
            handlers.Add(new SPVHandler(HandlerConstants.SPV));

            handlers.Add(new WindowsCEHandler(HandlerConstants.WINDOWS_CE));
            handlers.Add(new PortalmmmHandler(HandlerConstants.PORTALMMM));

            handlers.Add(new DoCoMoHandler(HandlerConstants.DOCOMO));
            handlers.Add(new KDDIHandler(HandlerConstants.KDDDI));

            handlers.Add(new AOLHandler(HandlerConstants.AOL));
            handlers.Add(new OperaHandler(HandlerConstants.OPERA));
            handlers.Add(new FirefoxHandler(HandlerConstants.FIREFOX));
            handlers.Add(new SafariHandler(HandlerConstants.SAFARI));
            handlers.Add(new MSIEHandler(HandlerConstants.MSIE));


            handlers.Add(new CatchAllHandler(HandlerConstants.CATCH_ALL));

            return handlers.AsReadOnly();
        }

        #endregion
    }
}