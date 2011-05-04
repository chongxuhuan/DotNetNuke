#region Copyright

// 
// DotNetNuke� - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

#region Usings

using System;
using System.IO;
using System.Xml;

using DotNetNuke.Application;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Instrumentation;

#endregion

namespace DotNetNuke.Services.Installer
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The XmlMerge class is a utility class for XmlSplicing config files
    /// </summary>
    /// <history>
    /// 	[cnurse]	08/03/2007	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class XmlMerge
    {
        private readonly string _Sender;
        private readonly XmlDocument _SourceConfig;
        private readonly string _Version;
        private string _TargetFileName;
        private string _TargetProductName;

        public XmlMerge(string sourceFileName, string version, string sender)
        {
            _Version = version;
            _Sender = sender;
            _SourceConfig = new XmlDocument();
            _SourceConfig.Load(sourceFileName);
        }

        public XmlMerge(Stream sourceStream, string version, string sender)
        {
            _Version = version;
            _Sender = sender;
            _SourceConfig = new XmlDocument();
            _SourceConfig.Load(sourceStream);
        }

        public XmlMerge(TextReader sourceReader, string version, string sender)
        {
            _Version = version;
            _Sender = sender;
            _SourceConfig = new XmlDocument();
            _SourceConfig.Load(sourceReader);
        }

        public XmlMerge(XmlDocument sourceDoc, string version, string sender)
        {
            _Version = version;
            _Sender = sender;
            _SourceConfig = sourceDoc;
        }

        public XmlDocument SourceConfig
        {
            get
            {
                return _SourceConfig;
            }
        }

        public string Sender
        {
            get
            {
                return _Sender;
            }
        }

        public XmlDocument TargetConfig { get; set; }

        public string TargetFileName
        {
            get
            {
                return _TargetFileName;
            }
        }

        public string Version
        {
            get
            {
                return _Version;
            }
        }

        private void AddNode(XmlNode rootNode, XmlNode actionNode)
        {
            foreach (XmlNode child in actionNode.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element || child.NodeType == XmlNodeType.Comment)
                {
                    rootNode.AppendChild(TargetConfig.ImportNode(child, true));
                }
            }
        }

        private void InsertNode(XmlNode childRootNode, XmlNode actionNode, NodeInsertType mode)
        {
            XmlNode rootNode = childRootNode.ParentNode;
            foreach (XmlNode child in actionNode.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element || child.NodeType == XmlNodeType.Comment)
                {
                    switch (mode)
                    {
                        case NodeInsertType.Before:
                            rootNode.InsertBefore(TargetConfig.ImportNode(child, true), childRootNode);
                            break;
                        case NodeInsertType.After:
                            rootNode.InsertAfter(TargetConfig.ImportNode(child, true), childRootNode);
                            break;
                    }
                }
            }
        }

        private void ProcessNode(XmlNode node)
        {
            string rootNodePath = node.Attributes["path"].Value;
            XmlNode rootNode;
            if (node.Attributes["nameSpace"] == null)
            {
                rootNode = TargetConfig.SelectSingleNode(rootNodePath);
            }
            else
            {
                string xmlNameSpace = node.Attributes["nameSpace"].Value;
                string xmlNameSpacePrefix = node.Attributes["nameSpacePrefix"].Value;
                var nsmgr = new XmlNamespaceManager(TargetConfig.NameTable);
                nsmgr.AddNamespace(xmlNameSpacePrefix, xmlNameSpace);
                rootNode = TargetConfig.SelectSingleNode(rootNodePath, nsmgr);
            }
            if (rootNode == null)
            {
				//TODO: what happens if Node can't be found
				DnnLog.Error("Can't found the root path: {0} defined in '{1}'", rootNodePath, node.OuterXml);
            }
            string nodeAction = node.Attributes["action"].Value;
            switch (nodeAction.ToLowerInvariant())
            {
                case "add":
                    AddNode(rootNode, node);
                    break;
                case "insertbefore":
                    InsertNode(rootNode, node, NodeInsertType.Before);
                    break;
                case "insertafter":
                    InsertNode(rootNode, node, NodeInsertType.After);
                    break;
                case "remove":
                    RemoveNode(rootNode);
                    break;
                case "removeattribute":
                    RemoveAttribute(rootNode, node);
                    break;
                case "update":
                    UpdateNode(rootNode, node);
                    break;
                case "updateattribute":
                    UpdateAttribute(rootNode, node);
                    break;
            }
        }

        private void ProcessNodes(XmlNodeList nodes, bool saveConfig)
        {
            if (TargetConfig != null)
            {
                foreach (XmlNode node in nodes)
                {
                    ProcessNode(node);
                }
                if (saveConfig)
                {
                    Config.Save(TargetConfig, TargetFileName);
                }
            }
        }

        private void RemoveAttribute(XmlNode rootNode, XmlNode actionNode)
        {
            string AttributeName = Null.NullString;
            if (actionNode.Attributes["name"] != null)
            {
                AttributeName = actionNode.Attributes["name"].Value;
                if (!string.IsNullOrEmpty(AttributeName))
                {
                    if (rootNode.Attributes[AttributeName] != null)
                    {
                        rootNode.Attributes.Remove(rootNode.Attributes[AttributeName]);
                    }
                }
            }
        }

        private void RemoveNode(XmlNode node)
        {
            if (node != null)
            {
                XmlNode parentNode = node.ParentNode;
                parentNode.RemoveChild(node);
            }
        }

        private void UpdateAttribute(XmlNode rootNode, XmlNode actionNode)
        {
            string AttributeName = Null.NullString;
            string AttributeValue = Null.NullString;
            if (rootNode != null && actionNode.Attributes["name"] != null && actionNode.Attributes["value"] != null)
            {
                AttributeName = actionNode.Attributes["name"].Value;
                AttributeValue = actionNode.Attributes["value"].Value;
                if (!string.IsNullOrEmpty(AttributeName))
                {
                    if (rootNode.Attributes[AttributeName] == null)
                    {
                        rootNode.Attributes.Append(TargetConfig.CreateAttribute(AttributeName));
                    }
                    rootNode.Attributes[AttributeName].Value = AttributeValue;
                }
            }
        }

        private void UpdateNode(XmlNode rootNode, XmlNode actionNode)
        {
            string keyAttribute = Null.NullString;
            string targetPath = Null.NullString;
            if (actionNode.Attributes["key"] != null)
            {
                keyAttribute = actionNode.Attributes["key"].Value;
            }
            foreach (XmlNode child in actionNode.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    XmlNode targetNode = null;
                    if (!string.IsNullOrEmpty(keyAttribute))
                    {
                        if (child.Attributes[keyAttribute] != null)
                        {
                            string path = string.Format("{0}[@{1}='{2}']", child.LocalName, keyAttribute, child.Attributes[keyAttribute].Value);
                            targetNode = rootNode.SelectSingleNode(path);
                        }
                    }
                    else
                    {
                        if (actionNode.Attributes["targetpath"] != null)
                        {
                            string path = actionNode.Attributes["targetpath"].Value;
                            if (actionNode.Attributes["nameSpace"] == null)
                            {
                                targetNode = rootNode.SelectSingleNode(path);
                            }
                            else
                            {
                                string xmlNameSpace = actionNode.Attributes["nameSpace"].Value;
                                string xmlNameSpacePrefix = actionNode.Attributes["nameSpacePrefix"].Value;
                                var nsmgr = new XmlNamespaceManager(TargetConfig.NameTable);
                                nsmgr.AddNamespace(xmlNameSpacePrefix, xmlNameSpace);
                                targetNode = rootNode.SelectSingleNode(path, nsmgr);
                            }
                        }
                    }
                    if (targetNode == null)
                    {
                        rootNode.AppendChild(TargetConfig.ImportNode(child, true));
                        continue;
                    }
                    else
                    {
                        string collisionAction = actionNode.Attributes["collision"].Value;
                        switch (collisionAction.ToLowerInvariant())
                        {
                            case "overwrite":
                                rootNode.RemoveChild(targetNode);
                                rootNode.InnerXml = rootNode.InnerXml + child.OuterXml;
                                break;
                            case "save":
                                string commentHeaderText = string.Format(Localization.Localization.GetString("XMLMERGE_Upgrade", Localization.Localization.SharedResourceFile),
                                                                         Environment.NewLine,
                                                                         Sender,
                                                                         Version,
                                                                         DateTime.Now);
                                XmlComment commentHeader = TargetConfig.CreateComment(commentHeaderText);
                                XmlComment commentNode = TargetConfig.CreateComment(targetNode.OuterXml);
                                rootNode.RemoveChild(targetNode);
                                rootNode.InnerXml = rootNode.InnerXml + commentHeader.OuterXml + commentNode.OuterXml + child.OuterXml;
                                break;
                            case "ignore":
                                break;
                        }
                    }
                }
            }
        }

        public void UpdateConfig(XmlDocument target)
        {
            TargetConfig = target;
            if (TargetConfig != null)
            {
                ProcessNodes(SourceConfig.SelectNodes("/configuration/nodes/node"), false);
            }
        }

        public void UpdateConfig(XmlDocument target, string fileName)
        {
            _TargetFileName = fileName;
            TargetConfig = target;
            if (TargetConfig != null)
            {
                ProcessNodes(SourceConfig.SelectNodes("/configuration/nodes/node"), true);
            }
        }

        public void UpdateConfigs()
        {
            foreach (XmlNode configNode in SourceConfig.SelectNodes("/configuration/nodes"))
            {
                _TargetFileName = configNode.Attributes["configfile"].Value;
                if (configNode.Attributes["productName"] != null)
                {
                    _TargetProductName = configNode.Attributes["productName"].Value;
                }
                bool IsAppliedToProduct = false;
                TargetConfig = Config.Load(TargetFileName);
                if (String.IsNullOrEmpty(_TargetProductName) || _TargetProductName == "All")
                {
                    IsAppliedToProduct = true;
                }
                else
                {
                    IsAppliedToProduct = DotNetNukeContext.Current.Application.ApplyToProduct(_TargetProductName);
                }
                if (TargetConfig != null && IsAppliedToProduct)
                {
                    ProcessNodes(configNode.SelectNodes("node"), true);
                }
            }
        }
    }
}