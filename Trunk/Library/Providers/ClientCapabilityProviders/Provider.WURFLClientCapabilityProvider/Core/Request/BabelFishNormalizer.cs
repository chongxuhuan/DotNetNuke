/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Text.RegularExpressions;

namespace DotNetNuke.Services.ClientCapability.Request
{
    /// <summary>
    /// Removes the (via babelfish.yahoo.com) string from the user agent
    /// </summary>
    internal class BabelFishNormalizer : INormalizer<string>
    {
        private readonly Regex BABEL_FISH_REGEX = new Regex(@"\s*\(via babelfish.yahoo.com\)\s*");

        #region INormalizer<string> Members

        public string Normalize(string value)
        {
            string normalizedValue = value;

            if (BABEL_FISH_REGEX.IsMatch(value))
            {
                normalizedValue = BABEL_FISH_REGEX.Replace(value, "");
            }
            return normalizedValue;
        }

        #endregion
    }
}