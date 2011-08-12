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
    /// Cretate a Chain of T elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IChainsFactory<T>
    {
        ICollection<T> Create();
    }
}