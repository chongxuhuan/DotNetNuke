/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICapabilitiesLoader
    {
        /// <summary>
        /// Loads the capabilities.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, string> LoadCapabilities();
    }
}