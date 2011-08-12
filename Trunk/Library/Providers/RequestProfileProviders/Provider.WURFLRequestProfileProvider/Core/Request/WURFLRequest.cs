/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.Devices.Core.Request
{
    internal class WURFLRequest : IWURFLRequest
    {
        private readonly string _userAgent;
        private readonly string _userAgentProfile;
        private readonly bool _xhtmlDevice;

        public WURFLRequest(string userAgent, string userAgentProfile, bool xhtmlDevice)
        {
            _userAgent = userAgent;
            _userAgentProfile = userAgentProfile;
            _xhtmlDevice = xhtmlDevice;
        }

        #region IWURFLRequest Members

        public string UserAgent
        {
            get { return _userAgent; }
        }

        public string UserAgentProfile
        {
            get { return _userAgentProfile; }
        }

        public bool IsXHTMLDevice
        {
            get { return _xhtmlDevice; }
        }

        #endregion
    }
}