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
    class GenerateSolutionFilesFixture
    {
        private string[] Projects = new string[] { "DotNetNuke.Enterprise.vbproj", "DotNetNuke.Enterprise.ContentStaging.vbproj", "DotNetNuke.Tests.ContentStaging.csproj", "Enterprise Projects" };
        private string[] Replacements = new string[] { "DotNetNuke_Enterprise", "TEST" };

        [Test]
        public void CreatePESolutionFiles_Returns_True_If_No_Projects_Set()
        {
            var processSolutionFiles = new GenerateSolutionFiles();
            Assert.AreEqual(true, processSolutionFiles.Execute());
        }

        [Test]
        public void CreatePESolutionFiles_Returns_False_If_No_File_Loads()
        {
            var processSolutionFiles = new GenerateSolutionFiles { ProjectNames = new string[] { "NonExistentFile.sln" } };
            Assert.AreEqual(false, processSolutionFiles.Execute());
        }

        [Test]
        public void CreatePESolutionFiles_Removes_TFS_Section()
        {
            var processSolutionFiles = new GenerateSolutionFiles { ProjectNames = Projects, OriginalFile = "DotNetNuke_Enterprise_UnitTests.sln", CreatedFile = "DNNTest.txt", Replacements = Replacements };
            processSolutionFiles.Execute();
        }

        [Test]
        public void CreatePESolutionFiles_No_Replacements()
        {
            var processSolutionFiles = new GenerateSolutionFiles { ProjectNames = Projects, OriginalFile = "DotNetNuke_Enterprise_UnitTests.sln", CreatedFile = "DNNTest.txt" };
            processSolutionFiles.Execute();
        }


    }
}
