/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System;
using System.Collections.Generic;
using DotNetNuke.Services.ClientCapability.Resource;

namespace DotNetNuke.Services.ClientCapability.Core.Classifiers
{
    /// <summary>
    /// 
    /// </summary>
    internal class Classifier<T, C> : IClassifier<ModelDevice, Classification>
    {
        private readonly ICollection<IClassificationHandler<string>> _classificationHandlers;


        /// <summary>
        /// Initializes a new instance of the <see cref="Classifier&lt;T, C&gt;"/> class.
        /// </summary>
        /// <param name="classificationHandlers">The classification handlers.</param>
        public Classifier(ICollection<IClassificationHandler<string>> classificationHandlers)
        {
            if (classificationHandlers == null)
            {
                _classificationHandlers = new List<IClassificationHandler<string>>();
            }

            _classificationHandlers = classificationHandlers;
        }

        #region IClassifier<ModelDevice,Classification> Members

        /// <summary>
        /// Classifies the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public ICollection<Classification> Classify(ICollection<ModelDevice> data)
        {
            IDictionary<string, Classification> classifications = new Dictionary<string, Classification>();

            foreach (IClassificationHandler<string> classificationHandler in _classificationHandlers)
            {
                classifications.Add(classificationHandler.ID, new Classification(classificationHandler.ID));
            }

            string classID = null;
            Classification classification;

            foreach (ModelDevice modelDevice in data)
            {
                foreach (IClassificationHandler<string> classificationHandler in _classificationHandlers)
                {
                    classID = classificationHandler.GetClassID(modelDevice.UserAgent);

                    if (!String.IsNullOrEmpty(classID))
                    {
                        if (classifications.TryGetValue(classID, out classification))
                        {
                            classification.Put(modelDevice.UserAgent, modelDevice.ID);
                            break;
                        }
                    }
                }
            }

            return classifications.Values;
        }

        #endregion
    }
}