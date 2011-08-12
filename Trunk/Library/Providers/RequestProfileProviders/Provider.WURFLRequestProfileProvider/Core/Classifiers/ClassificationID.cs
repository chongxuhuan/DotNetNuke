/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
namespace DotNetNuke.Services.Devices.Core.Classifiers
{
    public class ClassificationID
    {
        private readonly string _id;

        public ClassificationID(string id)
        {
            _id = id;
        }

        public string ID
        {
            get { return _id; }
        }
    }
}