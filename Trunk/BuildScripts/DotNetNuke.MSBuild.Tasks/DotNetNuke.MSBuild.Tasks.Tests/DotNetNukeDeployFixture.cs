using NUnit.Framework;

namespace DotNetNuke.MSBuild.Tasks.Tests
{

    [TestFixture]
    class DotNetNukeDeployFixture
    {

        [Test]
        public void CIBuildDeploy_Works()
        {
            var ci = new DotNetNukeDeploy() { PhysicalPath = @"D:\Releases\PE\DotNetNukeProfessional060100Install", AppPool = "ASP.NET v4.0", WebsiteName = "DotNetNuke_DeployTest" };
            //Assert.IsTrue();
        }
    }
}
