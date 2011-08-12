/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core.Matchers.Strategy
{
    internal class WeightedTokensDistance : IDistance<string>
    {
        private const double MAX_DISTANCE = 100;

        private readonly ITokensProvider<Token> _tokensProvider;

        public WeightedTokensDistance(ITokensProvider<Token> tokensProvider)
        {
            _tokensProvider = tokensProvider;
        }

        #region IDistance<string> Members

        public double Distance(string s, string t)
        {
            if (!_tokensProvider.CanApply(s) || !_tokensProvider.CanApply(t))
            {
                return MAX_DISTANCE;
            }

            IList<Token> sTokens = _tokensProvider.CreateTokens(s);
            IList<Token> tTokens = _tokensProvider.CreateTokens(t);

            int distance = 0;
            for (int i = 0; i < sTokens.Count; i++)
            {
                if (!String.Equals(sTokens[i].Content, tTokens[i].Content))
                {
                    distance += sTokens[i].Weight;
                }
            }

            return distance;
        }

        #endregion
    }
}