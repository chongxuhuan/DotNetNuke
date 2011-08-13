/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetNuke.Services.ClientCapability.Request
{
    /// <summary>
    /// Replace serial numbers 
    /// /SN12345678 -> /SNXXXXXXXX 
    /// /SN0987654321 -> /SNXXXXXXXXXX
    /// </summary>
    internal class VodafoneNormalizer : INormalizer<string>
    {
        //private static final Pattern snPattern = Pattern.compile("/SN\\d+\\s");

        private const string matchPattern = @"/SN\d+\s";

        #region INormalizer<string> Members

        public string Normalize(string value)
        {
            string normalizedValue = value;

            Regex regEx = new Regex(matchPattern);


            StringBuilder replace = new StringBuilder("/SN");
            string stringToReplace = null;

            if (regEx.IsMatch(value))
            {
                Match firstMath = regEx.Matches(value)[0];
                stringToReplace = firstMath.Groups[0].Value;
                for (int i = 3; i < stringToReplace.Length - 1; i++)
                {
                    replace.Append("X");
                }
                replace.Append(" ");

                normalizedValue = value.Replace(stringToReplace, replace.ToString());
            }

            return normalizedValue;
        }

        #endregion
    }
}