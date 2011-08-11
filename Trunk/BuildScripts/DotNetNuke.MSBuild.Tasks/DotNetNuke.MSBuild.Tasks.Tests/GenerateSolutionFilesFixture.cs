
using NUnit.Framework;

namespace DotNetNuke.MSBuild.Tasks.Tests
{
    using System.IO;

    [TestFixture]
    class GenerateSolutionFilesFixture
    {
        private string[] Projects = new string[] { "DotNetNuke.Enterprise", "DotNetNuke.Enterprise.ContentStaging", "DotNetNuke.Modules.Html", "DotNetNuke.Enterprise.SharePoint", "Enterprise Projects", "DotNetNuke.Enterprise.SharePoint.Contracts" };
        private string[] Replacements = new string[] { "DotNetNuke_Enterprise", "DotNetNuke_Replaced" };

        [Test]
        public void GenerateSolutionFiles_Returns_True_If_No_Projects_Set()
        {
            var generateSolutionFiles = new GenerateSolutionFiles();
            Assert.AreEqual(true, generateSolutionFiles.Execute());
        }

        [Test]
        public void GenerateSolutionFiles_Returns_False_If_No_File_Loads()
        {
            var generateSolutionFiles = new GenerateSolutionFiles { ProjectNames = new string[] { "NonExistentFile.sln" } };
            Assert.AreEqual(false, generateSolutionFiles.Execute());
        }

        [Test]
        public void GenerateSolutionFiles_Removes_TFS_Section()
        {
            var generateSolutionFiles = new GenerateSolutionFiles { ProjectNames = Projects, OriginalFile = "DotNetNuke_Enterprise_UnitTests.sln", CreatedFile = "DNNTest.txt", Replacements = Replacements, RemoveTfsBindings=true };
            generateSolutionFiles.Execute();

            var tr = new StreamReader("DNNTest.txt", true);
            var content = tr.ReadToEnd();
            tr.Close();
            Assert.IsFalse(content.Contains("TeamFoundationVersionControl"));
        }

        [Test]
        public void GenerateSolutionFiles_Does_Replacements()
        {
            var generateSolutionFiles = new GenerateSolutionFiles { ProjectNames = Projects, OriginalFile = "DotNetNuke_Enterprise_UnitTests.sln", CreatedFile = "DNNTest.txt", Replacements = Replacements };
            generateSolutionFiles.Execute();
            var tr = new StreamReader("DNNTest.txt", true);
            var content = tr.ReadToEnd();
            tr.Close();
            Assert.IsFalse(content.Contains("DotNetNuke_Enterprise"));
            Assert.IsTrue(content.Contains("DotNetNuke_Replaced"));
      }

        [Test]
        public void GenerateSolutionFiles_Removes_Listed_Projects()
        {
            var generateSolutionFiles = new GenerateSolutionFiles { ProjectNames = Projects, OriginalFile = "DotNetNuke_Enterprise_UnitTests.sln", CreatedFile = "DNNTest.txt", Replacements = Replacements, RemoveTfsBindings = true };
            generateSolutionFiles.Execute();
            var tr = new StreamReader("DNNTest.txt", true);
            var content = tr.ReadToEnd();
            tr.Close();
            Assert.IsFalse(content.Contains("DotNetNuke.Enterprise.SharePoint.Contracts"));
        }

        [Test]
        public void GenerateSolutionFiles_Removes_Listed_Projects_Not_TFS()
        {
            var generateSolutionFiles = new GenerateSolutionFiles { ProjectNames = Projects, OriginalFile = "DotNetNuke_Enterprise_UnitTests.sln", CreatedFile = "DNNTest.txt", Replacements = Replacements, RemoveTfsBindings = false };
            generateSolutionFiles.Execute();
            var tr = new StreamReader("DNNTest.txt", true);
            var content = tr.ReadToEnd();
            tr.Close();
            Assert.IsFalse(content.Contains("DotNetNuke.Enterprise.SharePoint.Contracts"));
        }

    }
}
