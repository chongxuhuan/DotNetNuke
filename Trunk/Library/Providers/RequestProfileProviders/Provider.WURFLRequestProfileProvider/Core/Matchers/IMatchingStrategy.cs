/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */

using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core.Matchers
{
    /// <summary>
    /// 
    /// </summary>
    internal interface IMatchingStrategy
    {
        /// <summary>
        /// Searches in the collection for a Match of the specified strings.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <param name="target">The target.</param>
        /// <param name="tollerance">The tollerance.</param>
        /// <returns></returns>
        string Match(IList<string> strings, string target, int tollerance);
    }
}