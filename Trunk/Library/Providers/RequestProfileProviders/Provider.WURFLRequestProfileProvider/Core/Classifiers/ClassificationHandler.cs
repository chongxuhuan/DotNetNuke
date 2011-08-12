/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using DotNetNuke.Services.Devices.Core.Hanldlers;

namespace DotNetNuke.Services.Devices.Core.Classifiers
{
    internal class ClassificationHandler : IClassificationHandler<string>
    {
        private readonly IHandler<string> _handler;


        public ClassificationHandler(IHandler<string> handler)
        {
            _handler = handler;
        }

        public IHandler<string> Handler
        {
            get { return _handler; }
        }

        #region IClassificationHandler<string> Members

        public string GetClassID(string userAgent)
        {
            if (_handler.CanHandle(userAgent))
            {
                return Handler.ID;
            }

            return null;
        }

        public string ID
        {
            get { return Handler.ID; }
        }

        #endregion
    }
}