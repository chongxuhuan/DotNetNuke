/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.ClientCapability.Resource;

namespace DotNetNuke.Services.ClientCapability
{
    public class CapabilitiesLoader : ICapabilitiesLoader
    {
        private readonly ModelDevice _modelDevice;
        private readonly IWURFLModel _wurflModel;

        public CapabilitiesLoader(IWURFLModel wurflModel, ModelDevice modelDevice)
        {
            _wurflModel = wurflModel;
            _modelDevice = modelDevice;
        }


        public IWURFLModel WURFLModel
        {
            get { return _wurflModel; }
        }

        #region ICapabilitiesLoader Members

        public IDictionary<string, string> LoadCapabilities()
        {
            IDictionary<string, string> capabilities = new Dictionary<string, string>();
            IList<ModelDevice> deviceHierarcy = _wurflModel.GetDeviceHierarchy(_modelDevice);

            // NOTE: deviceHierarchy is a list of devices generic -> ... -> start
            foreach (ModelDevice modelDevice in deviceHierarcy)
            {
                CopyAll(modelDevice.Capabilities, capabilities);
            }

            return capabilities;
        }

        #endregion

        /// <summary>
        /// FIXME: FIND BETTER IMPLEMENTATION
        /// Copies all.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        private void CopyAll(IDictionary<string, string> from, IDictionary<string, string> to)
        {
            foreach (KeyValuePair<string, string> capability in from)
            {
                if (to.ContainsKey(capability.Key))
                {
                    to[capability.Key] = capability.Value;
                }
                else
                {
                    to.Add(capability);
                }
            }
        }
    }
}