/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Configuration;

namespace DotNetNuke.Services.Devices.Core.Resource.Config
{
    public class WURFLConfigSectionHandler : ConfigurationSection
    {
        [ConfigurationProperty("wurflLocation")]
        public FileLocationConfigElement FileLocationSection
        {
            get { return (FileLocationConfigElement) this["wurflLocation"]; }
        }

        [ConfigurationProperty("patches")]
        public PatchesConfigElementCollection PatchesLocation
        {
            get { return (PatchesConfigElementCollection) this["patches"]; }
        }
    }
}