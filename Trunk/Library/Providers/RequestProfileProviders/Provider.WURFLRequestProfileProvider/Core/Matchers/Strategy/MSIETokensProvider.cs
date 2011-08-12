/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DotNetNuke.Services.Devices.Core.Matchers.Strategy
{
    /// <summary>
    /// FIXME
    /// </summary>
    internal class MSIETokensProvider : ITokensProvider<Token>
    {
        private static readonly Regex MSIE_VERSION_REGEX = new Regex(@"MSIE ([\.\w]+)", RegexOptions.Compiled);

        private static readonly Regex NET_VERSION_REGEX = new Regex(@"NET CLR ([0-9]{1,}[\.\w]{0,})",
                                                                    RegexOptions.Compiled);

        private static readonly Regex PLATFORM_REGEX = new Regex(@"Windows ([\.\w\s]{0,})", RegexOptions.Compiled);

        #region ITokensProvider<Token> Members

        /// <summary>
        /// Creates the tokens.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public IList<Token> CreateTokens(string source)
        {
            IList<Token> tokens = new List<Token>(3);
            tokens.Add(new Token(getMatch(source, MSIE_VERSION_REGEX), 2));
            tokens.Add(new Token(getMatch(source, PLATFORM_REGEX), 2));
            tokens.Add(new Token(getMatch(source, NET_VERSION_REGEX), 1));

            return tokens;
        }

        public bool CanApply(string userAgent)
        {
            return true;
        }

        #endregion

        private string getMatch(string source, Regex regex)
        {
            return regex.Match(source).Groups[0].Value;
        }
    }
}