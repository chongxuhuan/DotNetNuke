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
    class ProcessSolutionFilesFixture
    {
        private string[] SolutionFileNames = new string[] { "DotNetNuke_Enterprise_UnitTests.sln", "DotNetNuke_Community_UnitTests.sln" };
        private string[] NewSolutionFileNames = new string[] { "DotNetNuke_Professional.sln", "DotNetNuke_Community.sln" };
        private string[] NewVS2008SolutionFileNames = new string[] { "DotNetNuke_Professional.VS2008.sln", "DotNetNuke_Community.VS2008.sln" };

        [Test]
        public void ProcessSolutionFiles_Returns_False_If_No_FileName_Set()
        {
            var processSolutionFiles = new ProcessSolutionFiles();
            Assert.AreEqual(false, processSolutionFiles.Execute());
        }

        [Test]
        public void ProcessSolutionFiles_Returns_False_If_No_File_Loads()
        {
            var processSolutionFiles = new ProcessSolutionFiles { FileNames = new string[] { "NonExistentFile.sln" } };
            Assert.AreEqual(false, processSolutionFiles.Execute());
        }

        [Test]
        public void ProcessSolutionFiles_Sets_ReadOnly_To_False()
        {
            var processSolutionFiles = new ProcessSolutionFiles { FileNames = SolutionFileNames };
            var solutionFileInfo = new FileInfo(SolutionFileNames[0]) { IsReadOnly = true };
            processSolutionFiles.Execute();
            Assert.AreEqual(false, solutionFileInfo.IsReadOnly);
        }

        [Test]
        public void ProcessSolutionFiles_New_FileName_Correct()
        {
            var processSolutionFiles = new ProcessSolutionFiles { FileNames = SolutionFileNames };
            processSolutionFiles.Execute();
            Assert.IsTrue(File.Exists(NewSolutionFileNames[0]));
            Assert.IsTrue(File.Exists(NewVS2008SolutionFileNames[0]));
        }
    }
}
