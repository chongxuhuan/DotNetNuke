/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Text.RegularExpressions;

namespace DotNetNuke.Services.ClientCapability.Matchers.Strategy
{
    internal class FirefoxTokensProvider : RegExTokensProvider
    {
        private static readonly string pattern =
            @"Mozilla/5.0\s*(?:\(([^;]*);?\s*U;\s*([^;]*);?\s*([^;]*);?\s*(?:rv.*);?\s*(?:.*)?\))(?:.*)Gecko/(?:[\d]+)?(?:.*)?Firefox/(\d.\w)(?:[\.\w]+)?\s*(.*)?";

        private static readonly Regex regex = new Regex(pattern, RegexOptions.Compiled);

        public FirefoxTokensProvider()
            : base(regex, new int[] {1, 1, 1, 2, 0})
        {
        }
    }
}