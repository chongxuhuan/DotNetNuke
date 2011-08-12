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
    /// <typeparam name="T"></typeparam>
    public interface IWURFLPatcher<T> where T : ModelDevice
    {
        /// <summary>
        /// Patches the devices.
        /// </summary>
        /// <param name="devicesToPatch">The devices to patch.</param>
        /// <param name="patchingDevices">The patching devices.</param>
        /// <returns></returns>
        IList<T> PatchDevices(IList<T> devicesToPatch, IList<T> patchingDevices);
    }
}