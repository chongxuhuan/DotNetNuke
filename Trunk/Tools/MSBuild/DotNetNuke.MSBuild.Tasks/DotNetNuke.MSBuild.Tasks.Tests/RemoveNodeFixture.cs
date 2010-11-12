using MbUnit.Framework;
using Gallio.Framework;

namespace DotNetNuke.MSBuild.Tasks.Tests
{
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Linq;
    using System.Collections.Generic;

    [TestFixture]
    public class RemoveNodeFixture
    {
        private const string ProjectFileName = "Provider.DNNScheduler.vbproj";
        private const string ModuleProjectFileName = "DotNetNuke.Modules.Taxonomy.vbproj";
        private readonly XmlDocument _originalProjectFile = new XmlDocument();

        [SetUp]
        public void TestSetup()
        {
            _originalProjectFile.Load(ProjectFileName);
        }

        [TearDown]
        public void TestTearDown()
        {
            _originalProjectFile.Save(ProjectFileName);
        }

        [Test]
        public void Execute_Returns_False_If_No_FileName_Set()
        {
            var removeNode = new RemoveNode();
            Assert.AreEqual(false, removeNode.Execute());
        }

        [Test]
        public void Execute_Returns_False_If_No_XPath_Set()
        {
            var removeNode = new RemoveNode();
            Assert.AreEqual(false, removeNode.Execute());
        }

        [Test]
        public void Execute_Returns_False_If_No_File_Loads()
        {
            var removeNode = new RemoveNode { FileName = "NonExistentFile.xml" };
            Assert.AreEqual(false, removeNode.Execute());
        }

        [Test]
        public void Execute_Returns_False_If_XPath_Expression_Node_Does_Not_Exist()
        {
            var removeNode = new RemoveNode { FileName = ProjectFileName, XPath = "NonExistentNode" };
            Assert.AreEqual(false, removeNode.Execute());
        }

        [Test]
        public void Execute_Sets_ReadOnly_To_False()
        {
            var removeNode = new RemoveNode { FileName = ProjectFileName, XPath = "NonExistentNode" };
            var projectFileInfo = new FileInfo(ProjectFileName) { IsReadOnly = true };
            removeNode.Execute();
            Assert.AreEqual(false, projectFileInfo.IsReadOnly);
        }

        [Test]
        public void Execute_Removes_Node_XPath_Expression_Exists()
        {
            var removeNode = new RemoveNode { FileName = ProjectFileName, XPath = "Import", Attribute = "Project", AttributeValue = @"..\..\..\..\BuildScripts\ProviderPackage.Targets" };
            removeNode.Execute();
            //Check the file to see if the node is removed.
            var projectFile = new XmlDocument();
            projectFile.Load(ProjectFileName);
            var nsmgr = new XmlNamespaceManager(projectFile.NameTable);
            nsmgr.AddNamespace("dnn", "http://schemas.microsoft.com/developer/msbuild/2003");
            var root = projectFile.DocumentElement;
            var node = root.SelectSingleNode("descendant::dnn:Import[@Project='..\\..\\..\\..\\BuildScripts\\ProviderPackage.Targets']", nsmgr);
            Assert.AreEqual(null, node);
        }

        [Test]
        public void Execute_Updates_AfterBuild()
        {
            var removeNode = new RemoveNode { FileName = ProjectFileName, XPath = "Import", Attribute = "Project", AttributeValue = @"..\..\..\..\BuildScripts\ProviderPackage.Targets", ReplacementAttributeValue = "DebugProvider" };
            removeNode.Execute();
            var projectFile = new XmlDocument();
            projectFile.Load(ProjectFileName);
            var nsmgr = new XmlNamespaceManager(projectFile.NameTable);
            nsmgr.AddNamespace("dnn", "http://schemas.microsoft.com/developer/msbuild/2003");
            var root = projectFile.DocumentElement;
            var node = root.SelectSingleNode("descendant::dnn:Target[@Name='AfterBuild']", nsmgr);
            Assert.AreEqual("DebugProvider", node.Attributes["DependsOnTargets"].Value);
        }

        [Test]
        public void Execute_UpdatesModuleProjFile_AfterBuild()
        {
            var removeNode = new RemoveNode { FileName = ModuleProjectFileName, XPath = "Import", Attribute = "Project", AttributeValue = @"..\..\BuildScripts\ModulePackage.Targets", ReplacementAttributeValue = "DeployModule" };
            removeNode.Execute();
            var projectFile = new XmlDocument();
            projectFile.Load(ModuleProjectFileName);
            var nsmgr = new XmlNamespaceManager(projectFile.NameTable);
            nsmgr.AddNamespace("dnn", "http://schemas.microsoft.com/developer/msbuild/2003");
            var root = projectFile.DocumentElement;
            var node = root.SelectSingleNode("descendant::dnn:Target[@Name='AfterBuild']", nsmgr);
            Assert.AreEqual("DeployModule", node.Attributes["DependsOnTargets"].Value);
        }

        [Test]
        public void Execute_Updates_For_VS2008_Creates_New_File()
        {
            var removeNode = new RemoveNode { FileName = ModuleProjectFileName, XPath = "Import", Attribute = "Project", AttributeValue = @"..\..\BuildScripts\ModulePackage.Targets", ReplacementAttributeValue = "DeployModule" };
            removeNode.Execute();
            var projectFile = new XmlDocument();
            projectFile.Load(ModuleProjectFileName);
        }
    }
}
