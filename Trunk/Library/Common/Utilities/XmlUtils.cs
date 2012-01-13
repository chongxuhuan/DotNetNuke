#region Copyright
// 
// DotNetNuke� - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Permissions;

#endregion

namespace DotNetNuke.Common.Utilities
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The XmlUtils class provides Shared/Static methods for manipulating xml files
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	11/08/2004	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class XmlUtils
    {
        public static void AppendElement(ref XmlDocument objDoc, XmlNode objNode, string attName, string attValue, bool includeIfEmpty)
        {
            AppendElement(ref objDoc, objNode, attName, attValue, includeIfEmpty, false);
        }

        public static void AppendElement(ref XmlDocument objDoc, XmlNode objNode, string attName, string attValue, bool includeIfEmpty, bool cdata)
        {
            if (String.IsNullOrEmpty(attValue) && !includeIfEmpty)
            {
                return;
            }
            if (cdata)
            {
                objNode.AppendChild(CreateCDataElement(objDoc, attName, attValue));
            }
            else
            {
                objNode.AppendChild(CreateElement(objDoc, attName, attValue));
            }
        }

        public static XmlAttribute CreateAttribute(XmlDocument objDoc, string attName, string attValue)
        {
            XmlAttribute attribute = objDoc.CreateAttribute(attName);
            attribute.Value = attValue;
            return attribute;
        }

        public static void CreateAttribute(XmlDocument objDoc, XmlNode objNode, string attName, string attValue)
        {
            XmlAttribute attribute = objDoc.CreateAttribute(attName);
            attribute.Value = attValue;
            objNode.Attributes.Append(attribute);
        }

        [Obsolete("Removed in DotNetNuke 5.5")]
        public static XmlElement CreateElement(XmlDocument document, string nodeName)
        {
            return document.CreateElement(nodeName);
        }

        public static XmlElement CreateElement(XmlDocument document, string nodeName, string nodeValue)
        {
            XmlElement element = document.CreateElement(nodeName);
            element.InnerText = nodeValue;
            return element;
        }

        public static XmlElement CreateCDataElement(XmlDocument document, string nodeName, string nodeValue)
        {
            XmlElement element = document.CreateElement(nodeName);
            element.AppendChild(document.CreateCDataSection(nodeValue));
            return element;
        }

        [Obsolete("Replaced in DotNetNuke 5.5 with CBO.DeserializeObject")]
        public static object Deserialize(string xmlObject, Type type)
        {
            var ser = new XmlSerializer(type);
            var sr = new StringReader(xmlObject);
            object obj = ser.Deserialize(sr);
            sr.Close();
            return obj;
        }

        public static object Deserialize(Stream objStream, Type type)
        {
            object obj = Activator.CreateInstance(type);
            var tabDic = obj as Dictionary<int, TabInfo>;
            if (tabDic != null)
            {
                obj = DeSerializeDictionary<TabInfo>(objStream, "dictionary");
                return obj;
            }
            var moduleDic = obj as Dictionary<int, ModuleInfo>;
            if (moduleDic != null)
            {
                obj = DeSerializeDictionary<ModuleInfo>(objStream, "dictionary");
                return obj;
            }
            var tabPermDic = obj as Dictionary<int, TabPermissionCollection>;
            if (tabPermDic != null)
            {
                obj = DeSerializeDictionary<TabPermissionCollection>(objStream, "dictionary");
                return obj;
            }
            var modPermDic = obj as Dictionary<int, ModulePermissionCollection>;
            if (modPermDic != null)
            {
                obj = DeSerializeDictionary<ModulePermissionCollection>(objStream, "dictionary");
                return obj;
            }
            var serializer = new XmlSerializer(type);
            TextReader tr = new StreamReader(objStream);
            obj = serializer.Deserialize(tr);
            tr.Close();
            return obj;
        }

        public static Dictionary<int, TValue> DeSerializeDictionary<TValue>(Stream objStream, string rootname)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(objStream);

            var objDictionary = new Dictionary<int, TValue>();

            foreach (XmlElement xmlItem in xmlDoc.SelectNodes(rootname + "/item"))
            {
                int key = Convert.ToInt32(xmlItem.GetAttribute("key"));

                var objValue = Activator.CreateInstance<TValue>();

                //Create the XmlSerializer
                var xser = new XmlSerializer(objValue.GetType());

                //A reader is needed to read the XML document.
                var reader = new XmlTextReader(new StringReader(xmlItem.InnerXml));

                //Use the Deserialize method to restore the object's state, and store it
                //in the Hashtable
                objDictionary.Add(key, (TValue)xser.Deserialize(reader));
            }
            return objDictionary;
        }

        public static Hashtable DeSerializeHashtable(string xmlSource, string rootname)
        {
            var hashTable  = new Hashtable();

            if (!String.IsNullOrEmpty(xmlSource))
            {
                try
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlSource);

                    foreach (XmlElement xmlItem in xmlDoc.SelectNodes(rootname + "/item"))
                    {
                        string key = xmlItem.GetAttribute("key");
                        string typeName = xmlItem.GetAttribute("type");

                        //Create the XmlSerializer
                        var xser = new XmlSerializer(Type.GetType(typeName));

                        //A reader is needed to read the XML document.
                        var reader = new XmlTextReader(new StringReader(xmlItem.InnerXml));

                        //Use the Deserialize method to restore the object's state, and store it
                        //in the Hashtable
                        hashTable.Add(key, xser.Deserialize(reader));
                    }
                }
                catch (Exception ex)
                {
                    //DnnLog.Error(ex); /*Ignore Log because if failed on profile this will log on every request.*/
                }
            }

            return hashTable;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of an attribute
        /// </summary>
        /// <param name="nav">Parent XPathNavigator</param>
        /// <param name="attributeName">Thename of the Attribute</param>
        /// <returns></returns>
        /// <history>
        /// 	[cnurse]	05/14/2008	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetAttributeValue(XPathNavigator nav, string attributeName)
        {
            return nav.GetAttribute(attributeName, "");
        }

        public static bool GetAttributeValueAsBoolean(XPathNavigator navigator, string attributeName, bool defaultValue)
        {
            bool boolValue = defaultValue;
            string strValue = GetAttributeValue(navigator, attributeName);
            if (!string.IsNullOrEmpty(strValue))
            {
                boolValue = Convert.ToBoolean(strValue);
            }
            return boolValue;
        }

        public static int GetAttributeValueAsInteger(XPathNavigator navigator, string attributeName, int defaultValue)
        {
            int intValue = defaultValue;
            string strValue = GetAttributeValue(navigator, attributeName);
            if (!string.IsNullOrEmpty(strValue))
            {
                intValue = Convert.ToInt32(strValue);
            }
            return intValue;
        }

        public static long GetAttributeValueAsLong(XPathNavigator navigator, string attributeName, long defaultValue)
        {
            long intValue = defaultValue;

            string strValue = GetAttributeValue(navigator, attributeName);
            if (!string.IsNullOrEmpty(strValue))
            {
                intValue = Convert.ToInt64(strValue);
            }

            return intValue;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of a node
        /// </summary>
        /// <param name="navigator">Parent XPathNavigator</param>
        /// <param name="path">The Xpath expression to the value</param>
        /// <returns></returns>
        /// <history>
        /// 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetNodeValue(XPathNavigator navigator, string path)
        {
            string strValue = Null.NullString;
            XPathNavigator elementNav = navigator.SelectSingleNode(path);
            if (elementNav != null)
            {
                strValue = elementNav.Value;
            }
            return strValue;
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="objNode">Parent node</param>
        /// <param name="nodeName">Child node to look for</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value will be returned.
        /// </remarks>
        public static string GetNodeValue(XmlNode objNode, string nodeName)
        {
            return GetNodeValue(objNode, nodeName, String.Empty);
        }
        
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="objNode">Parent node</param>
        /// <param name="nodeName">Child node to look for</param>
        /// <param name="defaultValue">Default value to return</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value will be returned.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	09/09/2004	Created
        /// 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetNodeValue(XmlNode objNode, string nodeName, string defaultValue)
        {
            string strValue = defaultValue;
            if ((objNode[nodeName] != null))
            {
                strValue = objNode[nodeName].InnerText;
                if (String.IsNullOrEmpty(strValue) && !String.IsNullOrEmpty(defaultValue))
                {
                    strValue = defaultValue;
                }
            }
            return strValue;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="objNode">Parent node</param>
        /// <param name="nodeName">Child node to look for</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value (False) will be returned.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	09/09/2004	Added new method to return converted values
        /// 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool GetNodeValueBoolean(XmlNode objNode, string nodeName)
        {
            return GetNodeValueBoolean(objNode, nodeName, false);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="objNode">Parent node</param>
        /// <param name="nodeName">Child node to look for</param>
        /// <param name="defaultValue">Default value to return</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value will be returned.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	09/09/2004	Added new method to return converted values
        /// 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool GetNodeValueBoolean(XmlNode objNode, string nodeName, bool defaultValue)
        {
            bool bValue = defaultValue;
            if ((objNode[nodeName] != null))
            {
                string strValue = objNode[nodeName].InnerText;
                if (!string.IsNullOrEmpty(strValue))
                {
                    bValue = Convert.ToBoolean(strValue);
                }
            }
            return bValue;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="objNode">Parent node</param>
        /// <param name="nodeName">Child node to look for</param>
        /// <param name="defaultValue">Default value to return</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value will be returned.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	09/09/2004	Added new method to return converted values
        /// 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        /// </history>
        /// -----------------------------------------------------------------------------
        public static DateTime GetNodeValueDate(XmlNode objNode, string nodeName, DateTime defaultValue)
        {
            DateTime dateValue = defaultValue;
            if ((objNode[nodeName] != null))
            {
                string strValue = objNode[nodeName].InnerText;
                if (!string.IsNullOrEmpty(strValue))
                {
                    dateValue = Convert.ToDateTime(strValue);
                    if (dateValue.Date.Equals(Null.NullDate.Date))
                    {
                        dateValue = Null.NullDate;
                    }
                }
            }
            return dateValue;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="nodeName">Child node to look for</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value (0) will be returned.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	09/09/2004	Added new method to return converted values
        /// 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int GetNodeValueInt(XmlNode node, string nodeName)
        {
            return GetNodeValueInt(node, nodeName, 0);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="nodeName">Child node to look for</param>
        /// <param name="defaultValue">Default value to return</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value will be returned.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	09/09/2004	Added new method to return converted values
        /// 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int GetNodeValueInt(XmlNode node, string nodeName, int defaultValue)
        {
            int intValue = defaultValue;
            if ((node[nodeName] != null))
            {
                string strValue = node[nodeName].InnerText;
                if (!string.IsNullOrEmpty(strValue))
                {
                    intValue = Convert.ToInt32(strValue);
                }
            }
            return intValue;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="nodeName">Child node to look for</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value (0) will be returned.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	09/09/2004	Added new method to return converted values
        /// 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        /// </history>
        /// -----------------------------------------------------------------------------
        public static float GetNodeValueSingle(XmlNode node, string nodeName)
        {
            return GetNodeValueSingle(node, nodeName, 0);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of node
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="nodeName">Child node to look for</param>
        /// <param name="defaultValue">Default value to return</param>
        /// <returns></returns>
        /// <remarks>
        /// If the node does not exist or it causes any error the default value will be returned.
        /// </remarks>
        /// <history>
        /// 	[VMasanas]	09/09/2004	Added new method to return converted values
        /// 	[cnurse]	11/08/2004	moved from PortalController and made Public Shared
        /// </history>
        /// -----------------------------------------------------------------------------
        public static float GetNodeValueSingle(XmlNode node, string nodeName, float defaultValue)
        {
            float sValue = defaultValue;
            if ((node[nodeName] != null))
            {
                string strValue = node[nodeName].InnerText;
                if (!string.IsNullOrEmpty(strValue))
                {
                    sValue = Convert.ToSingle(strValue, CultureInfo.InvariantCulture);
                }
            }
            return sValue;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets an XmlWriterSettings object
        /// </summary>
        /// <param name="conformance">Conformance Level</param>
        /// <returns>An XmlWriterSettings</returns>
        /// <history>
        /// 	[cnurse]	08/22/2008	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static XmlWriterSettings GetXmlWriterSettings(ConformanceLevel conformance)
        {
            var settings = new XmlWriterSettings();
            settings.ConformanceLevel = conformance;
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            return settings;
        }

        public static string SerializeDictionary(IDictionary source, string rootName)
        {
            string strString;
            if (source.Count != 0)
            {
                XmlSerializer xser;
                StringWriter sw;

                var xmlDoc = new XmlDocument();
                XmlElement xmlRoot = xmlDoc.CreateElement(rootName);
                xmlDoc.AppendChild(xmlRoot);

                foreach (var key in source.Keys)
                {
                    //Create the item Node
                    XmlElement xmlItem = xmlDoc.CreateElement("item");

                    //Save the key name and the object type
                    xmlItem.SetAttribute("key", Convert.ToString(key));
                    xmlItem.SetAttribute("type", source[key].GetType().AssemblyQualifiedName);

                    //Serialize the object
                    var xmlObject = new XmlDocument();
                    xser = new XmlSerializer(source[key].GetType());
                    sw = new StringWriter();
                    xser.Serialize(sw, source[key]);
                    xmlObject.LoadXml(sw.ToString());

                    //import and append the node to the root
                    xmlItem.AppendChild(xmlDoc.ImportNode(xmlObject.DocumentElement, true));
                    xmlRoot.AppendChild(xmlItem);
                }
                //Return the OuterXml of the profile
                strString = xmlDoc.OuterXml;
            }
            else
            {
                strString = "";
            }
            return strString;
        }

        public static void SerializeHashtable(Hashtable hashtable, XmlDocument xmlDocument, XmlNode rootNode, string elementName, string keyField, string valueField)
        {
            XmlNode nodeSetting;
            XmlNode nodeSettingName;
            XmlNode nodeSettingValue;

            string outerElementName = elementName + "s";
            string innerElementName = elementName;

            XmlNode nodeSettings = rootNode.AppendChild(xmlDocument.CreateElement(outerElementName));
            foreach (string key in hashtable.Keys)
            {
                nodeSetting = nodeSettings.AppendChild(xmlDocument.CreateElement(innerElementName));
                nodeSettingName = nodeSetting.AppendChild(xmlDocument.CreateElement(keyField));
                nodeSettingName.InnerText = key;
                nodeSettingValue = nodeSetting.AppendChild(xmlDocument.CreateElement(valueField));
                nodeSettingValue.InnerText = hashtable[key].ToString();
            }
        }


        public static void UpdateAttribute(XmlNode node, string attName, string attValue)
        {
            if ((node != null))
            {
                XmlAttribute attrib = node.Attributes[attName];
                attrib.InnerText = attValue;
            }
        }

        ///-----------------------------------------------------------------------------
        ///<summary>
        ///  Xml Encodes HTML
        ///</summary>
        ///<param name = "html">The HTML to encode</param>
        ///<returns></returns>
        ///<history>
        ///  [cnurse]	09/29/2005	moved from Globals
        ///</history>
        ///-----------------------------------------------------------------------------
        public static string XMLEncode(string html)
        {
            return "<![CDATA[" + html + "]]>";
        }


        public static void XSLTransform(XmlDocument doc, ref StreamWriter writer, string xsltUrl)
        {
            var xslt = new XslCompiledTransform();
            xslt.Load(xsltUrl);
            //Transform the file.
            xslt.Transform(doc, null, writer);
        }

        public static string Serialize(object obj)
        {
            string xmlObject;
            var dic = obj as IDictionary;
            if ((dic != null))
            {
                xmlObject = SerializeDictionary(dic, "dictionary");
            }
            else
            {
                var xmlDoc = new XmlDocument();
                var xser = new XmlSerializer(obj.GetType());
                var sw = new StringWriter();

                xser.Serialize(sw, obj);

                xmlDoc.LoadXml(sw.GetStringBuilder().ToString());
                XmlNode xmlDocEl = xmlDoc.DocumentElement;
                xmlDocEl.Attributes.Remove(xmlDocEl.Attributes["xmlns:xsd"]);
                xmlDocEl.Attributes.Remove(xmlDocEl.Attributes["xmlns:xsi"]);

                xmlObject = xmlDocEl.OuterXml;
            }
            return xmlObject;
        }

        [Obsolete("This method is obsolete.")]
        public static XmlDocument GetXMLContent(string contentUrl)
        {
            //This function reads an Xml document via a Url and returns it as an XmlDocument object

            var functionReturnValue = new XmlDocument();
            var req = WebRequest.Create(contentUrl);
            var result = req.GetResponse();
            var objXmlReader = new XmlTextReader(result.GetResponseStream());
            functionReturnValue.Load(objXmlReader);
            return functionReturnValue;
        }
    }
}
