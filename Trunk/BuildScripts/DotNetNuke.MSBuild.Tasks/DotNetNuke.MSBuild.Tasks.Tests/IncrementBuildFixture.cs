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
        private const string CsVersionStart = "[assembly: AssemblyVersion";
        private const string CsVersionEnd = ")]";

        [SetUp]
        private void Setup()
        {
            if (File.Exists("Community/AssemblyInfoTest.cs"))
            {
                var communityAssemblyFile = new FileInfo("Community/AssemblyInfoTest.cs") { IsReadOnly = false };
                File.Delete("Community/AssemblyInfoTest.cs");
            }
            File.Copy("../Community/AssemblyInfo.cs", "Community/AssemblyInfoTest.cs");
            if (File.Exists("Professional/AssemblyInfoTest.cs"))
            {
                var professionalAssemblyFile = new FileInfo("Community/AssemblyInfoTest.cs") { IsReadOnly = false };
                File.Delete("Professional/AssemblyInfoTest.cs");
            }
            File.Copy("../Professional/AssemblyInfo.cs", "Professional/AssemblyInfoTest.cs");
            if (File.Exists("Enterprise/AssemblyInfoTest.cs"))
            {
                var enterpriseAssemblyFile = new FileInfo("Community/AssemblyInfoTest.cs") { IsReadOnly = false };
                File.Delete("Enterprise/AssemblyInfoTest.cs");
            }
            File.Copy("../Enterprise/AssemblyInfo.cs", "Enterprise/AssemblyInfoTest.cs");
        }

        [Test]
        public void IncrementBuild_Returns_False_If_No_FileName_Set()
        {
            var incrementBuild = new IncrementBuild { };
            Assert.AreEqual(false, incrementBuild.Execute());
        }

        [Test]
        public void IncrementBuild_Does_Not_Increment_AutoIncrementVersion_False()
        {
            var incrementBuild = new IncrementBuild { FilePath = "Community/AssemblyInfoTest.cs", AutoIncrementVersion = false };
            incrementBuild.Execute();
            Assert.AreEqual("6.0.0.0", incrementBuild.BuildVersion);
        }

        [Test]
        public void IncrementBuild_Does_Increment_BuildVersion_AutoIncrementVersion_True()
        {
            var incrementBuild = new IncrementBuild { FilePath = "Community/AssemblyInfoTest.cs", AutoIncrementVersion = true };
            incrementBuild.Execute();
            Assert.AreEqual("6.0.0.1", incrementBuild.BuildVersion);
        }

        [Test]
        public void IncrementBuild_Sets_BuildVersion_To_Default_Value()
        {
            var incrementBuild = new IncrementBuild { FilePath = "Community/AssemblyInfoTest.cs", DefaultVersion = "6.0.0.25" };
            incrementBuild.Execute();
            Assert.AreEqual("6.0.0.25", incrementBuild.BuildVersion);
        }

        [Test]
        public void IncrementBuild_Updates_File_To_Default_Value()
        {
            var incrementBuild = new IncrementBuild { FilePath = "Community/AssemblyInfoTest.cs", DefaultVersion = "6.0.0.25" };
            incrementBuild.Execute();
            var content = File.ReadAllText("Community/AssemblyInfoTest.cs");
            var indexStart = content.IndexOf(CsVersionStart) + 28;
            var indexEnd = content.IndexOf(CsVersionEnd, indexStart);
            var versionNumber = content.Substring(indexStart, indexEnd - indexStart - 1);

            Assert.AreEqual("6.0.0.25", versionNumber);
        }

    }
}
