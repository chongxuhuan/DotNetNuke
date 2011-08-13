/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;

namespace DotNetNuke.Services.ClientCapability.Matchers
{
    /// <summary>
    /// 
    /// </summary>
    internal class RISDistance : IDistance<string>
    {
        #region IDistance<string> Members

        /// <summary>
        /// Return the number of common chars between t1 and t2.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns></returns>
        public double Distance(string t1, string t2)
        {
            int t = Math.Min(t1.Length, t2.Length);
            int i = 0;
            while (i < t && t1[i] == t2[i])
            {
                i++;
            }
            return i;
        }

        #endregion
    }
}