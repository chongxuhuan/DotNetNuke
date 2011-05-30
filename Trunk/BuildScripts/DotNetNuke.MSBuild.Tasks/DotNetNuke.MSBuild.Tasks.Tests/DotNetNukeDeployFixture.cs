using MbUnit.Framework;
using Gallio.Framework;
using DotNetNuke.MSBuild.Tasks;

namespace DotNetNuke.MSBuild.Tasks.Tests
{
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Linq;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    class DotNetNukeDeployFixture
    {

        [Test]
        public void CIBuildDeploy_Works()
        {
            var ci = new DotNetNukeDeploy() { PhysicalPath = @"D:\DotNetNuke\Releases\EE\6.0.0\DotNetNuke_Enterprise_6.0.0.1190_Source\Website", AppPool = "DotNetNukeAppPool", WebsiteName = "DotNetNuke_Enterprise_6001190_Source" };
            ci.Execute();
            //Assert.IsTrue();
        }
    }
}
