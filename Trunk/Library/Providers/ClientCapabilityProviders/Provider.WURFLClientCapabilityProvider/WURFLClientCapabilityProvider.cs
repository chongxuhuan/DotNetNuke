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
    public class WURFLClientCapabilityProvider : ClientCapabilityProvider
    {

        #region Constructors
        /// <summary>
        /// Default Contructor
        /// </summary>
        public WURFLClientCapabilityProvider()
        {
            DefaultWurflDataFilePath = "/App_Data/WURFLDeviceDatabase/wurfl-latest.zip";
            DefaultWurflPatchFilePath = "/App_Data/WURFLDeviceDatabase/web_browsers_patch.xml";
        }

        /// <summary>
        /// Constructotr injecting the path to data source 
        /// </summary>
        /// <param name="wurflDataFilePath"></param>
        /// <param name="tWurflPatchFilePath"></param>
        public WURFLClientCapabilityProvider(String wurflDataFilePath, String tWurflPatchFilePath)
        {
            DefaultWurflDataFilePath = wurflDataFilePath;
            DefaultWurflPatchFilePath = tWurflPatchFilePath;
        }

        #endregion


        #region Attributes

        /// <summary>
        /// WURFL data paths
        /// </summary>
        private static String DefaultWurflDataFilePath;
        private static String DefaultWurflPatchFilePath;

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
                    // Get the absolute path of required data files
                    var wurflDataFile = Common.Globals.ApplicationMapPath + DefaultWurflDataFilePath;
                    var wurflPatchFile = Common.Globals.ApplicationMapPath + DefaultWurflPatchFilePath;

                    // Initializes the WURFL infrastructure
                    var configurer = new InMemoryConfigurer()
                        .MainFile(wurflDataFile)
                        .PatchFile(wurflPatchFile);
                    _wurflManager = WURFLManagerBuilder.Build(configurer);

                    return _wurflManager;
                }
            }
        }
        #endregion

        public override IClientCapability GetClientCapability(string userAgent)
        {            
            IDevice device = Manager.GetDeviceForRequest(userAgent);
            if (device != null)
                return new WURLClientCapability(device);
            return null;
        }

        public override IClientCapability GetClientCapabilityById(string clientId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<IClientCapability> GeAllClientCapabilitys()
        {
            throw new NotImplementedException();
        }
    }
}
