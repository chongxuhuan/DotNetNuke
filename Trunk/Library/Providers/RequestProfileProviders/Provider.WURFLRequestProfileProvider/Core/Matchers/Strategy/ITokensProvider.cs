/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core.Matchers.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITokensProvider<T> where T : Token
    {
        /// <summary>
        /// Creates the tokens.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        IList<T> CreateTokens(string source);

        /// <summary>
        /// Determines whether this instance can apply the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can apply the specified s; otherwise, <c>false</c>.
        /// </returns>
        bool CanApply(string s);
    }
}