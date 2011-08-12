/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core.Resource
{
    /// <summary>
    /// 
    /// </summary>
    public interface IModelDevicesProvider
    {
        /// <summary>
        /// Gets the model devices.
        /// </summary>
        /// <param name="wurflResource">The wurfl resource.</param>
        /// <returns></returns>
        IList<ModelDevice> GetModelDevices(WURFLResource wurflResource);
    }
}