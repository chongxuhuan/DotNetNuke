using NUnit.Framework;

namespace DotNetNuke.MSBuild.Tasks.Tests
{
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Linq;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    class ProcessProjectFileVS2008Fixture
    {
        private string[] ProjectFileNames = new string[] { "DotNetNuke.Modules.Html.vbproj", "Provider.DNNScheduler.vbproj", "DotNetNuke.Modules.Taxonomy.vbproj" };
        private string[] NewProjectFileNames = new string[] { "DotNetNuke.Modules.Html.VS2008.vbproj", "Provider.DNNScheduler.VS2008.vbproj", "DotNetNuke.Modules.Taxonomy.VS2008.vbproj" };
        XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";

        [Test]
        public void UpdateNode_Returns_False_If_No_FileName_Set()
        {
            var updateNode = new ProcessProjectFileVS2008();
            Assert.AreEqual(false, updateNode.Execute());
        }

        [Test]
        public void ProcessProjectFileVS2008_Returns_False_If_No_File_Loads()
        {
            var updateNode = new ProcessProjectFileVS2008 { FileNames = new string[] { "NonExistentFile.xml" } };
            Assert.AreEqual(false, updateNode.Execute());
        }

        [Test]
        public void UpdateNode_Sets_ReadOnly_To_False()
        {
            var updateNode = new ProcessProjectFileVS2008 { FileNames = ProjectFileNames };
            var projectFileInfo = new FileInfo(ProjectFileNames[0]) { IsReadOnly = true };
            updateNode.Execute();
            Assert.AreEqual(false, projectFileInfo.IsReadOnly);
        }

        [Test]
        public void ProcessProjectFileVS2008_New_FileName_Correct()
        {
            var processProjectFileVS2008 = new ProcessProjectFileVS2008 { FileNames = ProjectFileNames };
            processProjectFileVS2008.Execute();
            var projectFile = new XmlDocument();
            projectFile.Load(NewProjectFileNames[0]);
            Assert.IsNotNull(projectFile);
        }

        [Test]
        public void ProcessProjectFileVS2008_Updates_WebApplication_Targets()
        {
            var processProjectFileVS2008 = new ProcessProjectFileVS2008 { FileNames = ProjectFileNames };
            processProjectFileVS2008.Execute();
            var projectFile = new XmlDocument();
            projectFile.Load(NewProjectFileNames[0]);
            var sr = new StringReader(projectFile.OuterXml);
            var xRoot = XDocument.Load(sr);

            IEnumerable<string> vsVersion = xRoot
                .Element(msbuild + "Project")
                .Elements(msbuild + "Import").Where(x => x.Attribute("Project").Value == @"$(MSBuildExtensionsPath)\Microsoft\VisualStudio\v9.0\WebApplications\Microsoft.WebApplication.targets")
                .Single()
                .Attributes("Project")
                .Select(element => element.Value);
            Assert.AreEqual(@"$(MSBuildExtensionsPath)\Microsoft\VisualStudio\v9.0\WebApplications\Microsoft.WebApplication.targets", vsVersion.First());
        }

        [Test]
        public void ProcessProjectFileVS2008_Updates_ToolsVersion()
        {
            var processProjectFileVS2008 = new ProcessProjectFileVS2008 { FileNames = ProjectFileNames };
            processProjectFileVS2008.Execute();
            var projectFile = new XmlDocument();
            projectFile.Load(NewProjectFileNames[0]);
            var sr = new StringReader(projectFile.OuterXml);
            var xRoot = XDocument.Load(sr);

            IEnumerable<string> vsVersion = xRoot
                .Element(msbuild + "Project")
                .Attributes("ToolsVersion")
                .Select(element => element.Value);
            Assert.AreEqual("3.5", vsVersion.First());
        }

        [Test]
        public void ProcessProjectFileVS2008_Removes_MSBuildCommunityTasks_Import()
        {
            var processProjectFileVS2008 = new ProcessProjectFileVS2008 { FileNames = ProjectFileNames };
            processProjectFileVS2008.Execute();
            var projectFile = new XmlDocument();
            projectFile.Load(NewProjectFileNames[0]);
            var a = projectFile.OuterXml;
            var sr = new StringReader(projectFile.OuterXml);
            var xRoot = XDocument.Load(sr);

            IEnumerable<string> import = xRoot.Element(msbuild + "Project")
                .Elements(msbuild + "Import")
                .Where(x => x.Attribute("Project").Value.Contains("BuildScripts"))
                .Select(element => element.Value);
            Assert.IsTrue(import.Count() == 0);
        }

        [Test]
        public void ProcessProjectFileVS2008_Sets_AfterBuild_To_DebugProject()
        {
            var processProjectFileVS2008 = new ProcessProjectFileVS2008 { FileNames = ProjectFileNames };
            processProjectFileVS2008.Execute();
            var projectFile = new XmlDocument();
            projectFile.Load(NewProjectFileNames[0]);
            var sr = new StringReader(projectFile.OuterXml);
            var xRoot = XDocument.Load(sr);

            IEnumerable<string> target = xRoot.Element(msbuild + "Project")
                .Elements(msbuild + "Target")
                .Where(x => x.Attribute("Name").Value == "AfterBuild")
                .Attributes("DependsOnTargets")
                .Select(element => element.Value);

            Assert.AreEqual("DebugProject", target.First());
        }

        [Test]
        public void ProcessProjectFileVS2008_Removes_Source_Control()
        {
            var processProjectFileVS2008 = new ProcessProjectFileVS2008 { FileNames = ProjectFileNames };
            processProjectFileVS2008.Execute();
            var projectFile = new XmlDocument();
            projectFile.Load(NewProjectFileNames[0]);
            var sr = new StringReader(projectFile.OuterXml);
            var xRoot = XDocument.Load(sr);

            IEnumerable<string> scc = xRoot.Element(msbuild + "Project")
                .Elements(msbuild + "PropertyGroup")
                .Elements(msbuild + "SccProvider")
                .Select(element => element.Value);

            Assert.IsTrue(scc.Count() == 0);
        }
    }
}
