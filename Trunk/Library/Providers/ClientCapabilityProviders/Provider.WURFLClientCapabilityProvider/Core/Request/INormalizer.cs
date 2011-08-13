/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.ClientCapability.Request
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INormalizer<T>
    {
        /// <summary>
        /// Normalizes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        T Normalize(T value);
    }
}