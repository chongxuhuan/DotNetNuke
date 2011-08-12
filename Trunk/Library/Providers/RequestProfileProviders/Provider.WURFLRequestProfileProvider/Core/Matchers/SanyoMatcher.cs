/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using DotNetNuke.Services.Devices.Core.Classifiers;
using DotNetNuke.Services.Devices.Core.Hanldlers;

namespace DotNetNuke.Services.Devices.Core.Matchers
{
    internal class SanyoMatcher : AbstractMatcher
    {
        public SanyoMatcher(Classification classification, IHandler<string> handler)
            : base(classification, handler)
        {
        }
    }
}