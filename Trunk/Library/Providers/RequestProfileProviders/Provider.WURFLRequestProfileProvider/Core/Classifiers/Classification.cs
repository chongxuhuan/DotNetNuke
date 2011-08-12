/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core.Classifiers
{
    internal class Classification
    {
        private readonly IDictionary<string, string> _classifiedData =
            new SortedDictionary<string, string>(StringComparer.Ordinal);

        private readonly string _id;


        public Classification(string id)
        {
            _id = id;
        }

        public string ID
        {
            get { return _id; }
        }


        public IDictionary<string, string> ClassifiedData
        {
            get { return _classifiedData; }
        }


        public IList<string> UserAgents
        {
            get { return new List<string>(_classifiedData.Keys).AsReadOnly(); }
        }


        public IList<string> DeviceIDs
        {
            get { return new List<string>(_classifiedData.Values).AsReadOnly(); }
        }


        internal void Put(string userAgent, string deviceID)
        {
            _classifiedData.Add(userAgent, deviceID);
        }
    }
}