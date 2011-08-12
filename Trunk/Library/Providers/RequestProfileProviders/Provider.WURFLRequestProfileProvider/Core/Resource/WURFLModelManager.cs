/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Devices.Core.Resource.Config;

using Provider.WURFLRequestProfileProvider.Core.Resource;
using Provider.WURFLRequestProfileProvider.Core.Resource.Config;

namespace DotNetNuke.Services.Devices.Core.Resource
{
    public class WURFLModelManager
    {
        private readonly IModelDevicesProvider _patchProvider;
        private readonly IWURFLModel _wurflModel;
        private readonly IWURFLPatcher<ModelDevice> _wurflPatcher;
        private readonly IModelDevicesProvider _wurflProvider;
        protected readonly DnnLogger logger = DnnLogger.GetLogger(typeof(WURFLModelManager).FullName);
        private WURFLConfig _config;


        public WURFLModelManager(IModelDevicesProvider wurflProvider, IModelDevicesProvider patchProvider,
                                 IWURFLPatcher<ModelDevice> wurflPatcher, WURFLConfig config)
        {
            _wurflProvider = wurflProvider;
            _patchProvider = patchProvider;
            _wurflPatcher = wurflPatcher;
            _config = config;

            _wurflModel = CreateModel(config);
        }

        public IWURFLModel WURFLModel
        {
            get { return _wurflModel; }
        }

        protected IWURFLModel CreateModel(WURFLConfig config)
        {
            IList<ModelDevice> devices = _wurflProvider.GetModelDevices(config.MainWURFLResource);
            
            logger.Debug("Parsing WURFL: " + config.WURFLFile + ".");            
            WURFLConsistencyVerifier.Verify(devices);

            logger.Debug(devices.Count + " devices found in " + config.WURFLFile);
            


            bool hasPatches = HasPatches(config);

            if (hasPatches)
            {
                logger.Debug("Applying patches: " + config.WURFLFilePatches + ".");
                devices = ApplyPatches(devices, config.PatchWURFLResources);
                logger.Debug("Total device after applying the paches: " + devices.Count + ".");
                
            }

            List<string> patches = config.WURFLFilePatches;
            logger.Info("WURFL managing " + devices.Count + " devices.");
            
            return new WURFLModel(devices);
        }

        private bool HasPatches(WURFLConfig config)
        {
            return config.PatchWURFLResources.Count > 0;
        }


        private IList<ModelDevice> ApplyPatches(IList<ModelDevice> devices, List<WURFLResource> patches)
        {
            IList<ModelDevice> patchedDevices = devices;


            foreach (WURFLResource patchingResource in patches)
            {
                logger.Debug("Appling patch: " + patchingResource.Path);                

                patchedDevices = ApplyPatch(patchedDevices, patchingResource);
                logger.Debug("Patch : " + patchingResource.Path + " applied.");                
            }

            return patchedDevices;
        }


        private IList<ModelDevice> ApplyPatch(IList<ModelDevice> devices, WURFLResource patchingResource)
        {
            IList<ModelDevice> patchingDevices = _patchProvider.GetModelDevices(patchingResource);

            IList<ModelDevice> patchedDevices = _wurflPatcher.PatchDevices(devices, patchingDevices);

            WURFLConsistencyVerifier.Verify(patchedDevices);
            logger.Debug("Patch: " + patchingResource.Path + ", patched devices : " + patchedDevices.Count);
            
            return patchedDevices;
        }
    }
}