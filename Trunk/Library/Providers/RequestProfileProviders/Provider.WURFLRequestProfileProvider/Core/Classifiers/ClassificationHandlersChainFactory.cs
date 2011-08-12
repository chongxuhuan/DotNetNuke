/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using DotNetNuke.Services.Devices.Core.Hanldlers;

namespace DotNetNuke.Services.Devices.Core.Classifiers
{
    public class ClassificationHandlersChainFactory : IChainFactory<IClassificationHandler<string>>
    {
        private ICollection<IClassificationHandler<string>> _classifications;
        private ICollection<IHandler<string>> _handlers;


        public ClassificationHandlersChainFactory(ICollection<IHandler<string>> handlers)
        {
            _handlers = handlers;
        }

        public ICollection<IClassificationHandler<string>> Classifiers
        {
            get { return _classifications; }
        }

        public ICollection<IHandler<string>> Handlers
        {
            get
            {
                if (_handlers == null)
                {
                    _handlers = new List<IHandler<string>>();
                }
                return _handlers;
            }
        }

        #region IChainFactory<IClassificationHandler<string>> Members

        public ICollection<IClassificationHandler<string>> Create()
        {
            if (_classifications == null)
            {
                Initialize();
            }

            return Classifiers;
        }

        #endregion

        private void Initialize()
        {
            _classifications = new List<IClassificationHandler<string>>();
            foreach (IHandler<string> handler in Handlers)
            {
                _classifications.Add(new ClassificationHandler(handler));
            }
        }
    }
}