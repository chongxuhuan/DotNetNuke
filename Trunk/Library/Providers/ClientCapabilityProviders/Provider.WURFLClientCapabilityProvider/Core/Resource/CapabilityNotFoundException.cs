/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;

namespace DotNetNuke.Services.ClientCapability.Resource
{
    public class CapabilityNotFoundException : Exception
    {
        public CapabilityNotFoundException()
        {
        }

        public CapabilityNotFoundException(string capabilityName)
            : base(
                string.Format("The Capability name : {0} specified is not defined in wurfl repository.", capabilityName)
                )
        {
        }
    }
}