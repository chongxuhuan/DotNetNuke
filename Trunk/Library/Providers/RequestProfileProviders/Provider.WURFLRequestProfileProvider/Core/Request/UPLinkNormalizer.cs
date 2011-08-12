/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.Devices.Core.Request
{
    /// <summary>
    /// Removes the UP.Link string form the UserAgent
    /// </summary>
    internal class UPLinkNormalizer : INormalizer<string>
    {
        private const string UP_LINK = "UP.Link";

        #region INormalizer<string> Members

        public string Normalize(string value)
        {
            return value.Replace(UP_LINK, "");
        }

        #endregion
    }
}