using MbUnit.Framework;
using Gallio.Framework;

namespace DotNetNuke.MSBuild.Tasks.Tests
{
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Linq;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    class IncrementBuildFixture
    {
        private string[] SolutionFileNames = new string[] { "DotNetNuke_Enterprise_UnitTests.sln", "DotNetNuke_Community_UnitTests.sln" };
        private string[] NewSolutionFileNames = new string[] { "DotNetNuke_Professional.sln", "DotNetNuke_Community.sln" };
        private string[] NewVS2008SolutionFileNames = new string[] { "DotNetNuke_Professional.VS2008.sln", "DotNetNuke_Community.VS2008.sln" };

        [Test]
        public void IncrementBuild_Returns_False_If_No_FileName_Set()
        {
            var incrementBuild = new IncrementBuild {};
            Assert.AreEqual(false, incrementBuild.Execute());
        }

        [Test]
        public void IncrementBuild_Does_Not_Increment_AutoIncrementVersion_False()
        {
            var incrementBuild = new IncrementBuild { FilePath = "AssemblyInfo.cs", AutoIncrementVersion = false };
            incrementBuild.Execute();
            Assert.AreEqual("6.0.0.0", incrementBuild.BuildVersion);
        }

        [Test]
        public void IncrementBuild_Does_Increment_AutoIncrementVersion_True()
        {
            var incrementBuild = new IncrementBuild { FilePath = "AssemblyInfo.cs", AutoIncrementVersion = true };
            incrementBuild.Execute();
            Assert.AreEqual("6.0.0.1", incrementBuild.BuildVersion);
        }

        [Test]
        public void IncrementBuild_Sets_To_Default_Value()
        {
            var incrementBuild = new IncrementBuild { FilePath = "AssemblyInfo.cs", DefaultVersion="6.0.0.25" };
            incrementBuild.Execute();
            Assert.AreEqual("6.0.0.25", incrementBuild.BuildVersion);
        }

    }
}
