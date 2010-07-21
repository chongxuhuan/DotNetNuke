// '
// ' DotNetNuke® - http://www.dotnetnuke.com
// ' Copyright (c) 2002-2010
// ' by DotNetNuke Corporation
// '
// ' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// ' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// ' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// ' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// '
// ' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// ' of the Software.
// '
// ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// ' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// ' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// ' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// ' DEALINGS IN THE SOFTWARE.
// '
using System;
using System.Configuration;

namespace DotNetNuke.Tests.Data
{
    /// <summary>
    /// Summary description for DataTestHelper
    /// </summary>
    public class DataTestHelper
    {
        #region Private Members

        private static string adminConnectionString = DataResources.AdminConnectionString;
        private static string connectionString = DataResources.ConnectionString;
        private static string databaseName = "DatabaseName";
        private static string serverName = "DatabaseServer";
        private static string databaseOwner = "DatabaseOwner";
        private static string objectQualifier = "ObjectQualifier";
        private static string filePath = "FilePath";

        private static string ServerName
        {
            get { return ConfigurationManager.AppSettings[serverName]; }
        }

        #endregion

        #region Public Members

        public static string AdminConnectionString = String.Format(adminConnectionString, ServerName, DatabaseName);
        public static string ConnectionString = String.Format(connectionString, ServerName, DatabaseName);

        public static string DatabaseName
        {
            get { return ConfigurationManager.AppSettings[databaseName]; }
        }

        public static string DatabaseOwner
        {
            get { return ConfigurationManager.AppSettings[databaseOwner]; }
        }

        public static string ObjectQualifier
        {
            get { return ConfigurationManager.AppSettings[objectQualifier]; }
        }

        public static string FilePath
        {
            get { return ConfigurationManager.AppSettings[filePath]; }
        }

        #endregion
    }
}