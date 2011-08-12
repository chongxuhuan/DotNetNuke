/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.Devices.Core.Utils
{
    internal static class StringUtils
    {
        /// <summary>
        /// Returns the index of th first slash char.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        internal static int FirstSlash(string userAgent)
        {
            int position = userAgent.IndexOf('/');
            return position != -1 ? position : userAgent.Length;
        }

        /// <summary>
        /// Returns the index of the second slash char.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        internal static int SecondSlash(string userAgent)
        {
            int first = FirstSlash(userAgent);
            int position = userAgent.IndexOf('/', ++first);
            return position != -1 ? position : userAgent.Length;
        }

        /// <summary>
        /// Returns the index of the first space char.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        internal static int FirstSpace(string userAgent)
        {
            int position = userAgent.IndexOf(' ');
            return position != -1 ? position : userAgent.Length;
        }
    }
}