/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.ClientCapability.Matchers.Strategy
{
    internal class SPVOpVerProvider : RegExTokensProvider
    {
        private const string regex = @"(.*)(SPV\s+.+);(.*)\s+OpVer\s+(\d+)\.(\d+)\.(\d+)\.(\d+)(.*)";

        public SPVOpVerProvider() : base(regex, new int[] {16, 16, 16, 8, 4, 2, 1, 16})
        {
        }
    }
}