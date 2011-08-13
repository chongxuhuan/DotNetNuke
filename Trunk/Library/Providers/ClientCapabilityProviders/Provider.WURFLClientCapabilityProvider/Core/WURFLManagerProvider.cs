/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */

using System.Collections.Generic;
using DotNetNuke.Services.ClientCapability.Core.Cache;
using DotNetNuke.Services.ClientCapability.Core.Classifiers;
using DotNetNuke.Services.ClientCapability.Hanldlers;
using DotNetNuke.Services.ClientCapability.Matchers;
using DotNetNuke.Services.ClientCapability.Request;
using DotNetNuke.Services.ClientCapability.Resource;
using DotNetNuke.Services.ClientCapability.Resource.Config;

using Provider.WURFLRequestProfileProvider.Core.Resource.Config;

namespace DotNetNuke.Services.ClientCapability
{
    public class WURFLManagerProvider : IWURFLManagerProvider<IWURFLManager>
    {
        private readonly IWURFLManager _wurflManager;

        public WURFLManagerProvider()
            : this(new WURFLConfig())
        {
        }


        public WURFLManagerProvider(WURFLConfig config)
        {
            IModelDevicesProvider wurflDevicesProvider = new ModelDevicesProvider();
            IModelDevicesProvider patchDevicesProvider = new ModelDevicesProvider(true);
            IWURFLPatcher<ModelDevice> wurflPatcher = new WURFLPatcher();


            WURFLModelManager modelManager = new WURFLModelManager(
                wurflDevicesProvider, patchDevicesProvider, wurflPatcher, config);

            IWURFLModel wurflModel = modelManager.WURFLModel;

            // Create Handlers
            IChainFactory<IHandler<string>> handlersFactory = new HandlersFactory();
            ICollection<IHandler<string>> handlers = handlersFactory.Create();

            // Create Classifer and ClassificationHandlers
            IChainFactory<IClassificationHandler<string>> classificationHandlersChainFactory =
                new ClassificationHandlersChainFactory(handlers);
            ICollection<IClassificationHandler<string>> classifiers = classificationHandlersChainFactory.Create();

            IClassifier<ModelDevice, Classification> classifier =
                new Classifier<ModelDevice, Classification>(classifiers);
            ICollection<Classification> classifications = classifier.Classify(modelManager.WURFLModel.Devices);

            // Create Matchers
            IChainFactory<IMatcher<IWURFLRequest>> matchersChainFactory = new MatchersChainFactory(classifications,
                                                                                                   handlers);
            ICollection<IMatcher<IWURFLRequest>> matchers = matchersChainFactory.Create();

            IMatcher<IWURFLRequest> matcher = new Matcher(wurflModel, matchers);

            IDeviceProvider<IDevice> deviceProvider = new DeviceProvider(wurflModel);

            ICache<string, IDevice> cache = new DictionaryCache<string, IDevice>();

            IWURFLService wurflService = new WURFLService(deviceProvider, matcher, cache);


            //********* REQUEST FACTORY AND NORMALIZERS
            IChainFactory<INormalizer<string>> normalizersFacotry = new NormalizersFactory();
            INormalizer<string> normalizer = new Normalizer(normalizersFacotry.Create());
            IWURFLRequestFactory requestFactory = new WURFLRequestFactory(normalizer);

            _wurflManager = new WURFLManager(wurflService, requestFactory);
        }

        #region IWURFLManagerProvider<IWURFLManager> Members

        public IWURFLManager WURFLManager
        {
            get { return _wurflManager; }
        }

        #endregion
    }
}