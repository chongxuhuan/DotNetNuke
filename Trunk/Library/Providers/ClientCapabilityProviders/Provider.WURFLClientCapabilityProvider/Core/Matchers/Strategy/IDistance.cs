/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.ClientCapability.Matchers
{
    /// <summary>
    /// Returns the distance between the two objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDistance<T>
    {
        double Distance(T t1, T t2);
    }
}