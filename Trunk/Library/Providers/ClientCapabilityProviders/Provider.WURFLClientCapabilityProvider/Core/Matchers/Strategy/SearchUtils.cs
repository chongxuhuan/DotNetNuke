/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;
using DotNetNuke.Services.ClientCapability.Matchers.Strategy;

namespace DotNetNuke.Services.ClientCapability.Matchers
{
    public sealed class SearchUtils
    {
        private static readonly IDistance<string> LD_DISTANCE = new LDDistance();
        private static readonly IDistance<string> RIS_DISTANCE = new RISDistance();


        /// <summary>
        /// RISs the search.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="key">The key.</param>
        /// <param name="tollerance">The tollerance.</param>
        /// <returns></returns>
        internal static string RISSearch(IList<string> list, string key, int tollerance)
        {
            string match = null;
            double bestDistance = -1;

            int low = 0;
            int high = list.Count - 1;
            while (low <= high && bestDistance < key.Length)
            {
                int mid = (low + high)/2;
                string midValue = list[mid];
                double distance = RIS_DISTANCE.Distance(key, midValue);


                if (distance > bestDistance)
                {
                    match = midValue;
                    bestDistance = distance;
                }

                int cmp = String.Compare(midValue, key, StringComparison.Ordinal);

                if (cmp < 0)
                {
                    low = mid + 1;
                }
                else if (cmp > 0)
                {
                    high = mid - 1;
                }
                else
                {
                    break;
                }
            }


            if (bestDistance < tollerance)
            {
                return null;
            }

            return match;
        }


        /// <summary>
        /// LDs the search.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="key">The key.</param>
        /// <param name="tollerance">The tollerance.</param>
        /// <returns></returns>
        internal static string LDSearch(IList<string> list, string key, int tollerance)
        {
            string match = null;
            double best = tollerance + 1;
            double current = 0;

            foreach (string element in list)
            {
                current = LD_DISTANCE.Distance(element, key);

                if (current <= best)
                {
                    best = current - 1;
                    match = element;
                }
            }

            return match;
        }

        /// <summary>
        /// Tokenses the serach.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="key">The key.</param>
        /// <param name="tokensProvider">The tokens provider.</param>
        /// <param name="tollerance">The tollerance.</param>
        /// <returns></returns>
        internal static string TokensSerach(IList<string> list, string key, ITokensProvider<Token> tokensProvider,
                                            int tollerance)
        {
            WeightedTokensDistance weightedDistance = new WeightedTokensDistance(tokensProvider);

            double bestDistance = tollerance + 1;
            string bestMatch = null;

            foreach (string element in list)
            {
                double distance = weightedDistance.Distance(element, key);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestMatch = element;
                }
            }

            if (bestDistance <= tollerance)
            {
                return bestMatch;
            }

            return null;
        }
    }
}