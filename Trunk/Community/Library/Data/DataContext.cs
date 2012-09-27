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

using System.Configuration;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data.PetaPoco;

#endregion

namespace DotNetNuke.Data
{
    public class DataContext
    {
        #region Private Members

        private static bool _isInitialized;
        private static readonly object Lock = new object();

        #endregion

        #region Private Methods

        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                lock (Lock)
                {
                    if (!_isInitialized)
                    {
                        for (int i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
                        {
                            var connectionString = ConfigurationManager.ConnectionStrings[i];
                            var tablePrefix = string.Empty;
                            if (connectionString.ConnectionString == DataProvider.Instance().ConnectionString)
                            {
                                tablePrefix = DataProvider.Instance().ObjectQualifier;
                            }

                            if (i == 0)
                            {
                                ComponentFactory.RegisterComponentInstance<IDataContext>(new PetaPocoDataContext(connectionString.Name, tablePrefix));
                            }
                            else
                            {
                                ComponentFactory.RegisterComponentInstance<IDataContext>(connectionString.Name, new PetaPocoDataContext(connectionString.Name, tablePrefix));
                            }
                        }
                        _isInitialized = true;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public static IDataContext Instance()
        {
            EnsureInitialized();
            return ComponentFactory.GetComponent<IDataContext>();
        }

        public static IDataContext Instance(string connectionStringName)
        {
            EnsureInitialized();
            return ComponentFactory.GetComponent<IDataContext>(connectionStringName);
        }

        #endregion
    }
}