/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.Devices.Core.Request
{
    /// <summary>
    /// Trims the String before the BlackBerry string
    /// </summary>
    internal class BlackBerryNormalizer : INormalizer<string>
    {
        #region INormalizer<string> Members

        public string Normalize(string value)
        {
            string normalizedValue = value;

            int index = value.IndexOf("BlackBerry");

            if (index != -1 && index > 0)
            {
                normalizedValue = value.Substring(index);
            }

            return normalizedValue;
        }

        #endregion
    }
}