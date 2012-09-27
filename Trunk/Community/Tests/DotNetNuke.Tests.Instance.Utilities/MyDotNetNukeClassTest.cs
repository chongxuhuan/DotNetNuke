using System;
using System.Text;
using System.Collections.Generic;
using DotNetNuke.Data;
using MbUnit.Framework;
using Store.AppController;

namespace Store.Tests.TestingUtilities
{
    /// <summary>
    /// Example DotNetNuke Unit Test Class
    /// </summary>
    [TestFixture]
    public class MyDotNetNukeClassTest : DnnUnitTest 
    {
        public MyDotNetNukeClassTest() : base (1)  //send portal id to base class
        {
        }

        [Test]
        public void MyDotNetNukeSqlTest()
        {
            //SqlDataProvider sqlProvider = new SqlDataProvider();
            //int tabCount = sqlProvider.GetTabCount(this.PortalId);
            //Assert.AreNotEqual(0, tabCount, "Portal has no tabs");
            Assert.IsTrue(true);
        }
    }
}
