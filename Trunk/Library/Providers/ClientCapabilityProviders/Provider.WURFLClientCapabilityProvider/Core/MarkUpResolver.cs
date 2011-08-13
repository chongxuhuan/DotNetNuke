/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */

using System;

namespace DotNetNuke.Services.ClientCapability
{
    internal class MarkUpResolver : IMarkUpResolver
    {
        private const string XHTML_SUPPORT_LEVEL = "xhtml_support_level";

        #region IMarkUpResolver Members

        public MarkUp ResolveMarkUp(IDevice device)
        {
            int xhtmlSupportLevel = Int32.Parse(device.GetCapability(XHTML_SUPPORT_LEVEL));

            MarkUp markUP = MarkUp.WML;
            switch (xhtmlSupportLevel)
            {
                case -1:
                case 0:
                    markUP = MarkUp.WML;
                    break;
                case 1:
                case 2:
                    markUP = MarkUp.XHTML_SIMPLE;
                    break;
                case 3:
                case 4:
                    markUP = MarkUp.XHTML_ADVANCED;
                    break;
                default:
                    break;
            }

            return markUP;
        }

        #endregion
    }
}