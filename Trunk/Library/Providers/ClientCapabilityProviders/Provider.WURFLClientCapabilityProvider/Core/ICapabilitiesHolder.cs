/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */

using System.Collections.Generic;

namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICapabilitiesHolder
    {
        /// <summary>
        /// Capabilitieses this instance.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, string> Capabilities { get; }

        /// <summary>
        /// Gets the capability value.
        /// </summary>
        /// <param name="capabilityName">Name of the capability.</param>
        /// <returns></returns>
        string GetCapabilityValue(string capabilityName);
    }
}