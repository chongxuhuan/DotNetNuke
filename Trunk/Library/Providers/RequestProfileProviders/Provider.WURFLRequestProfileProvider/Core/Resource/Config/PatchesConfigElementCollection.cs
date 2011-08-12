/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Configuration;

namespace DotNetNuke.Services.Devices.Core.Resource.Config
{
    public class PatchesConfigElementCollection : ConfigurationElementCollection
    {
        public FileLocationConfigElement this[int index]
        {
            get { return (FileLocationConfigElement) BaseGet(index); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FileLocationConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FileLocationConfigElement) element).path;
        }
    }
}