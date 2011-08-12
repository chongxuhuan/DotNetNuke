/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.Devices.Core.Hanldlers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHandler<T>
    {
        string ID { get; }
        bool CanHandle(T userAgent);
    }
}