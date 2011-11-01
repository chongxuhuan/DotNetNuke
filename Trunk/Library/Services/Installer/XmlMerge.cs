#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
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
		#region "Private Methods"
		
        private readonly string _Sender;
        private readonly XmlDocument _SourceConfig;
        private readonly string _Version;
        private string _TargetFileName;
        private string _TargetProductName;

		#endregion

		#region "Constructors"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the XmlMerge class.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="sender"></param>
        /// <param name="sourceFileName"></param>
        /// <history>
        /// 	[cnurse]	08/03/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public XmlMerge(string sourceFileName, string version, string sender)
        {
            _Version = version;
            _Sender = sender;
            _SourceConfig = new XmlDocument();
            _SourceConfig.Load(sourceFileName);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the XmlMerge class.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="sender"></param>
        /// <param name="sourceStream"></param>
        /// <history>
        /// 	[cnurse]	08/03/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public XmlMerge(Stream sourceStream, string version, string sender)
        {
            _Version = version;
            _Sender = sender;
            _SourceConfig = new XmlDocument();
            _SourceConfig.Load(sourceStream);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the XmlMerge class.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="sender"></param>
        /// <param name="sourceReader"></param>
        /// <history>
        /// 	[cnurse]	08/03/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public XmlMerge(TextReader sourceReader, string version, string sender)
        {
            _Version = version;
            _Sender = sender;
            _SourceConfig = new XmlDocument();
            _SourceConfig.Load(sourceReader);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the XmlMerge class.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="sender"></param>
        /// <param name="sourceDoc"></param>
        /// <history>
        /// 	[cnurse]	08/03/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public XmlMerge(XmlDocument sourceDoc, string version, string sender)
        {
            _Version = version;
            _Sender = sender;
            _SourceConfig = sourceDoc;
        }
		
		#endregion

		#region "Public Properties"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Source for the Config file
        /// </summary>
        /// <value>An XmlDocument</value>
        /// <history>
        /// 	[cnurse]	08/03/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public XmlDocument SourceConfig
        {
            get
            {
                return _SourceConfig;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Sender (source) of the changes to be merged
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	08/03/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string Sender
        {
            get
            {
                return _Sender;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Target Config file
        /// </summary>
        /// <value>An XmlDocument</value>
        /// <history>
        /// 	[cnurse]	08/03/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public XmlDocument TargetConfig { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the File Name of the Target Config file
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	08/04/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string TargetFileName
        {
            get
            {
                return _TargetFileName;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Version of the changes to be merged
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	08/03/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string Version
        {
            get
            {
                return _Version;
            }
        }
		
		#endregion

		#region "Private Methods"

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
				//Use Namespace Manager
                string xmlNameSpace = node.Attributes["nameSpace"].Value;
                string xmlNameSpacePrefix = node.Attributes["nameSpacePrefix"].Value;
                var nsmgr = new XmlNamespaceManager(TargetConfig.NameTable);
                nsmgr.AddNamespace(xmlNameSpacePrefix, xmlNameSpace);
                rootNode = TargetConfig.SelectSingleNode(rootNodePath, nsmgr);
            }

			string nodeAction = node.Attributes["action"].Value.ToLowerInvariant();

            if (rootNode == null)
            {
                return;
            }

            switch (nodeAction)
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
			//The nodes definition is not correct so skip changes
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
				//Get Parent
                XmlNode parentNode = node.ParentNode;

                //Remove current Node
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
								//Use Namespace Manager
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
                        //Since there is no collision we can just add the node
                        rootNode.AppendChild(TargetConfig.ImportNode(child, true));
                        continue;
                    }
                    else
                    {
						//There is a collision so we need to determine what to do.
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
		
		#endregion

		#region "Public Methods"


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The UpdateConfig method processes the source file and updates the Target
        /// Config Xml Document.
        /// </summary>
        /// <param name="target">An Xml Document represent the Target Xml File</param>
        /// <history>
        /// 	[cnurse]	08/04/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void UpdateConfig(XmlDocument target)
        {
            TargetConfig = target;
            if (TargetConfig != null)
            {
                ProcessNodes(SourceConfig.SelectNodes("/configuration/nodes/node"), false);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The UpdateConfig method processes the source file and updates the Target
        /// Config file.
        /// </summary>
        /// <param name="target">An Xml Document represent the Target Xml File</param>
        /// <param name="fileName">The fileName for the Target Xml File - relative to the webroot</param>
        /// <history>
        /// 	[cnurse]	08/04/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void UpdateConfig(XmlDocument target, string fileName)
        {
            _TargetFileName = fileName;
            TargetConfig = target;
            if (TargetConfig != null)
            {
                ProcessNodes(SourceConfig.SelectNodes("/configuration/nodes/node"), true);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The UpdateConfigs method processes the source file and updates the various config 
        /// files
        /// </summary>
        /// <history>
        /// 	[cnurse]	08/03/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void UpdateConfigs()
        {
            foreach (XmlNode configNode in SourceConfig.SelectNodes("/configuration/nodes"))
            {
				//Attempt to load TargetFile property from configFile Atribute
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
                //The nodes definition is not correct so skip changes
				if (TargetConfig != null && IsAppliedToProduct)
                {
                    ProcessNodes(configNode.SelectNodes("node"), true);
                }
            }
        }
		
		#endregion
    }
}
