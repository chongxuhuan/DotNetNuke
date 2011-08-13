/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;

namespace DotNetNuke.Services.ClientCapability.Resource
{
    internal class UserAgentNotUniqueException : Exception
    {
        public UserAgentNotUniqueException()
        {
        }

        public UserAgentNotUniqueException(ModelDevice device, ModelDevice definingDevice)
        {
        }
    }
}