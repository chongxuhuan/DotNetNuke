/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Configuration;

namespace DotNetNuke.Services.ClientCapability.Resource.Config
{
    public class FileLocationConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true, IsKey = true)]
        public string path
        {
            get { return (string) (this["path"] != null ? this["path"] : base["path"]); }
        }

        public override string ToString()
        {
            return path;
        }
    }
}