using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Xml;

namespace DotNetNuke.MSBuild.Tasks
{
    using System.Xml.XPath;
    using System.IO;

    public class RemoveNode : Task
    {
        public string FileName { get; set; }
        public string XPath { get; set; }
        public string Attribute { get; set; }
        public string AttributeValue { get; set; }

        public override bool Execute()
        {
            XmlDocument projectFile;
            if (FileName == string.Empty)
            {
                return false;
            }

            if (XPath == string.Empty)
            {
                return false;
            }

            var xpathExpression = this.Attribute == string.Empty ? string.Format("descendant::dnn:{0}", this.XPath) : string.Format("descendant::dnn:{0}[@{1}='{2}']", this.XPath, this.Attribute, this.AttributeValue);
            try
            {
                var projectFileInfo = new FileInfo(FileName) { IsReadOnly = false };
                projectFile = new XmlDocument();
                projectFile.Load(FileName);
                var nsmgr = new XmlNamespaceManager(projectFile.NameTable);
                nsmgr.AddNamespace("dnn", "http://schemas.microsoft.com/developer/msbuild/2003");
                var root = projectFile.DocumentElement;
                var node = root.SelectSingleNode(xpathExpression, nsmgr);
                if (node == null)
                {
                    return false;
                }

                node.ParentNode.RemoveChild(node);
                root.SelectSingleNode("descendant::dnn:Target[@Name='AfterBuild']", nsmgr).Attributes["DependsOnTargets"].Value = "DebugProvider";
                projectFile.Save(FileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
