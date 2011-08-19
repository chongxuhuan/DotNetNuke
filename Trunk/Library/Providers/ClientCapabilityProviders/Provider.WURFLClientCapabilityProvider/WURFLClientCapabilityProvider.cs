#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

#region Usings
using System;
using System.Linq;

using WURFL;
using WURFL.Config;

#endregion

namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    ///   WURFL implementation of ClientCapabilityProvider
    /// </summary>
    public class WURFLClientCapabilityProvider : ClientCapabilityProvider
    {
        #region Attributes

        /// <summary>
        /// WURFL data paths
        /// </summary>
        private static String _wurflDataFile;
        private static String _wurflPatchFile;

        #endregion

        #region Constructors
        /// <summary>
        /// Default Contructor
        /// </summary>
        public WURFLClientCapabilityProvider()
        {
            _wurflDataFile = Common.Globals.ApplicationMapPath + "/App_Data/WURFLDeviceDatabase/wurfl-latest.zip";
            _wurflPatchFile = Common.Globals.ApplicationMapPath + "/App_Data/WURFLDeviceDatabase/web_browsers_patch.xml";
        }

        /// <summary>
        /// Constructotr injecting the path to data source.  The client code is responsible for passing the full path for the data source xml files 
        /// </summary>
        /// <param name="wurflDataFilePath"></param>
        /// <param name="tWurflPatchFilePath"></param>
        public WURFLClientCapabilityProvider(String wurflDataFilePath, String tWurflPatchFilePath)
        {
            _wurflDataFile = wurflDataFilePath;
            _wurflPatchFile = tWurflPatchFilePath;
        }

        #endregion

        #region static methods
        static object _Managerlock = new object();
        static IWURFLManager _wurflManager;
        private static IWURFLManager Manager
        {
            get
            {
                lock (_Managerlock)
                {
                    if (_wurflManager != null) 
                        return _wurflManager;

                    // Initializes the WURFL infrastructure
                    var configurer = new InMemoryConfigurer()
                        .MainFile(_wurflDataFile)
                        .PatchFile(_wurflPatchFile);
                    _wurflManager = WURFLManagerBuilder.Build(configurer);

                    return _wurflManager;
                }
            }
        }
        #endregion

        #region ClientCapabilityProvider Methods

        /// <summary>
        ///   Returns ClientCapability based on HttpRequest
        /// </summary>
        public override IClientCapability GetClientCapability(string userAgent)
        {            
            var device = Manager.GetDeviceForRequest(userAgent);
            if (device != null)
                return new WURLClientCapability(device);
            return null;
        }

        /// <summary>
        ///   Returns ClientCapability based on ClientCapabilityId
        /// </summary>
        public override IClientCapability GetClientCapabilityById(string clientId)
        {
            var device = Manager.GetDeviceById(clientId);
            if (device != null)
                return new WURLClientCapability(device);
            return null;
        }

        /// <summary>
        ///   Returns All ClientCapabilitys available
        /// </summary>
        public override IQueryable<IClientCapability> GeAllClientCapabilitys()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
