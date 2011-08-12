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
    internal class RegExTokensProvider : ITokensProvider<Token>
    {
        private readonly Regex _regex;
        private readonly int[] _weights;

        public RegExTokensProvider(string regEx, int[] weights)
        {
            _regex = new Regex(regEx, RegexOptions.Compiled);
            _weights = weights;
        }


        public RegExTokensProvider(Regex regEx, int[] weights)
        {
            _regex = regEx;
            _weights = weights;
        }

        #region ITokensProvider<Token> Members

        public IList<Token> CreateTokens(string source)
        {
            IList<Token> tokens = new List<Token>();

            MatchCollection matchCollection = _regex.Matches(source);

            foreach (Match match in matchCollection)
            {
                for (int counter = 1;
                     counter < match.Groups.Count;
                     counter++)
                {
                    tokens.Add(new Token(match.Groups[counter].Value, _weights[counter - 1]));
                }
            }

            return tokens;
        }


        public bool CanApply(string userAgent)
        {
            return _regex.IsMatch(userAgent);
        }

        #endregion
    }
}