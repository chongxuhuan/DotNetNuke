/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Text.RegularExpressions;

namespace DotNetNuke.Services.Devices.Core.Request
{
    internal class YesWAPNormalizer : INormalizer<string>
    {
        private readonly Regex YES_WAP_REGEX = new Regex(@"\s*Mozilla/4\.0 \(YesWAP mobile phone proxy\)");

        #region INormalizer<string> Members

        public string Normalize(string value)
        {
            string normalizedValue = value;

            if (YES_WAP_REGEX.IsMatch(value))
            {
                normalizedValue = YES_WAP_REGEX.Replace(value, "");
            }
            return normalizedValue;
        }

        #endregion
    }
}