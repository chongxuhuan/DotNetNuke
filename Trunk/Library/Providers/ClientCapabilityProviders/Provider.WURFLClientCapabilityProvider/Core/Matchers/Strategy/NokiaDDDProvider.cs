/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.ClientCapability.Matchers.Strategy
{
    internal class NokiaDDDProvider : RegExTokensProvider
    {
        private const string regex = @"(.*)\s+(Nokia.*)/(\d+)\.(\d+)\.(\d+)(.*)";

        public NokiaDDDProvider()
            : base(regex, new int[] {10, 10, 4, 2, 1, 1})
        {
        }
    }
}