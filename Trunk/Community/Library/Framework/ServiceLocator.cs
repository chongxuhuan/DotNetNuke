// // DotNetNuke® - http://www.dotnetnuke.com
// // Copyright (c) 2002-2012
// // by DotNetNuke Corporation
// // 
// // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// // documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// // the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// // to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// // 
// // The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// // of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// // TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// // THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// // CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// // DEALINGS IN THE SOFTWARE.
using System;

namespace DotNetNuke.Framework
{
    /// <summary>
    /// Provides a readily testable way to manage a Singleton
    /// </summary>
    /// <typeparam name="TContract">The interface that the controller provides</typeparam>
    /// <typeparam name="TSelf">The type of the controller itself, used to call the GetFactory override</typeparam>
    public abstract class ServiceLocator<TContract, TSelf> where TSelf : ServiceLocator<TContract, TSelf>, new()
    {
        private static TContract _instance;
// ReSharper disable StaticFieldInGenericType
        private static bool _isInitialized;
        private static readonly object Lock = new object();
// ReSharper restore StaticFieldInGenericType

        protected static Func<TContract> Factory { get; set; }

        /// <summary>
        /// Returns a singleton of T
        /// </summary>
        public static TContract Instance
        {
            get
            {
                if (!_isInitialized)
                {
                    lock(Lock)
                    {
                        if (!_isInitialized)
                        {
                            if (Factory == null)
                            {
                                var controllerInstance = new TSelf();
                                Factory = controllerInstance.GetFactory();
                            }

                            _instance = Factory();
                            _isInitialized = true;
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Registers an instance to use for the Singleton
        /// </summary>
        /// <remarks>Intended for unit testing purposes, the instance must be registered before the snigleton is accessed</remarks>
        /// <param name="instance"></param>
        public static void SetTestableInstance(TContract instance)
        {
            _instance = instance;
            _isInitialized = true;
        }

        protected abstract Func<TContract> GetFactory();
    }
}