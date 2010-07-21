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
using System.Data.SqlClient;
using DotNetNuke.Tests.Utilities.Mocks;
using MbUnit.Framework;

namespace DotNetNuke.Tests.Data
{
    /// <summary>
    /// Summary description for TestSteup
    /// </summary>
    [AssemblyFixture]
    public class TestSetup
    {
        #region Public Methods

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            MockComponentProvider.ResetContainer();
        }


        [FixtureSetUp]
        public static void SetupDatabase()
        {
            // Connect to the server to create the database
            using (SqlConnection connection = new SqlConnection(DataTestHelper.AdminConnectionString))
            {
                connection.Open();

                //Create database
                DataUtil.CreateDatabase(connection, DataTestHelper.DatabaseName);
            }
        }

        [FixtureTearDown]
        public static void DropDatabase()
        {
            SqlConnection.ClearAllPools();

            // Connect to the server
            using (SqlConnection connection = new SqlConnection(DataTestHelper.AdminConnectionString))
            {
                connection.Open();

                //Drop Database
                DataUtil.DropDatabase(connection, DataTestHelper.DatabaseName);
            }
        }

        #endregion
    }
}