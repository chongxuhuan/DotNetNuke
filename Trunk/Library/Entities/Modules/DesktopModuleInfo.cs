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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules.Definitions;

#endregion

namespace DotNetNuke.Entities.Modules
{
    [Serializable]
    public class DesktopModuleInfo : ContentItem, IXmlSerializable
    {
        private Dictionary<string, ModuleDefinitionInfo> _moduleDefinitions;

        public DesktopModuleInfo()
        {
            IsPremium = Null.NullBoolean;
            IsAdmin = Null.NullBoolean;
            CodeSubDirectory = Null.NullString;
            PackageID = Null.NullInteger;
            DesktopModuleID = Null.NullInteger;
            SupportedFeatures = Null.NullInteger;
        }

        public int DesktopModuleID { get; set; }

        public int PackageID { get; set; }

        public string BusinessControllerClass { get; set; }

        public string Category
        {
            get
            {
                Term term = (from Term t in Terms select t).FirstOrDefault();
                return (term != null) ? term.Name : String.Empty;
            } 
            set
            {
                Terms.Clear();
                ITermController termController = Util.GetTermController();
                var term = (from Term t in termController.GetTermsByVocabulary("Module_Categories") 
                            where t.Name == value 
                            select t)
                            .FirstOrDefault();
                if (term != null)
                {
                    Terms.Add(term);
                }
            }
        }

        public string CodeSubDirectory { get; set; }

        public string CompatibleVersions { get; set; }

        public string Dependencies { get; set; }

        public string Description { get; set; }

        public string FolderName { get; set; }

        public string FriendlyName { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsPortable
        {
            get
            {
                return GetFeature(DesktopModuleSupportedFeature.IsPortable);
            }
            set
            {
                UpdateFeature(DesktopModuleSupportedFeature.IsPortable, value);
            }
        }

        public bool IsPremium { get; set; }

        public bool IsSearchable
        {
            get
            {
                return GetFeature(DesktopModuleSupportedFeature.IsSearchable);
            }
            set
            {
                UpdateFeature(DesktopModuleSupportedFeature.IsSearchable, value);
            }
        }

        public bool IsUpgradeable
        {
            get
            {
                return GetFeature(DesktopModuleSupportedFeature.IsUpgradeable);
            }
            set
            {
                UpdateFeature(DesktopModuleSupportedFeature.IsUpgradeable, value);
            }
        }

        public Dictionary<string, ModuleDefinitionInfo> ModuleDefinitions
        {
            get
            {
                if (_moduleDefinitions == null)
                {
                    if (DesktopModuleID > Null.NullInteger)
                    {
                        _moduleDefinitions = ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(DesktopModuleID);
                    }
                    else
                    {
                        _moduleDefinitions = new Dictionary<string, ModuleDefinitionInfo>();
                    }
                }
                return _moduleDefinitions;
            }
        }

        public string ModuleName { get; set; }

        public string Permissions { get; set; }

        public int SupportedFeatures { get; set; }

        public string Version { get; set; }

        #region IHydratable Members

        public override void Fill(IDataReader dr)
        {
            DesktopModuleID = Null.SetNullInteger(dr["DesktopModuleID"]);
            PackageID = Null.SetNullInteger(dr["PackageID"]);
            ModuleName = Null.SetNullString(dr["ModuleName"]);
            FriendlyName = Null.SetNullString(dr["FriendlyName"]);
            Description = Null.SetNullString(dr["Description"]);
            FolderName = Null.SetNullString(dr["FolderName"]);
            Version = Null.SetNullString(dr["Version"]);
            Description = Null.SetNullString(dr["Description"]);
            IsPremium = Null.SetNullBoolean(dr["IsPremium"]);
            IsAdmin = Null.SetNullBoolean(dr["IsAdmin"]);
            BusinessControllerClass = Null.SetNullString(dr["BusinessControllerClass"]);
            SupportedFeatures = Null.SetNullInteger(dr["SupportedFeatures"]);
            CompatibleVersions = Null.SetNullString(dr["CompatibleVersions"]);
            Dependencies = Null.SetNullString(dr["Dependencies"]);
            Permissions = Null.SetNullString(dr["Permissions"]);
            base.FillInternal(dr);
        }

        #endregion

        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                if (reader.NodeType == XmlNodeType.Whitespace)
                {
                    continue;
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "moduleDefinitions" && !reader.IsEmptyElement)
                {
                    ReadModuleDefinitions(reader);
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "supportedFeatures" && !reader.IsEmptyElement)
                {
                    ReadSupportedFeatures(reader);
                }
                else
                {
                    switch (reader.Name)
                    {
                        case "moduleName":
                            ModuleName = reader.ReadElementContentAsString();
                            break;
                        case "foldername":
                            FolderName = reader.ReadElementContentAsString();
                            break;
                        case "businessControllerClass":
                            BusinessControllerClass = reader.ReadElementContentAsString();
                            break;
                        case "codeSubDirectory":
                            CodeSubDirectory = reader.ReadElementContentAsString();
                            break;
                        case "isAdmin":
                            bool isAdmin;
                            Boolean.TryParse(reader.ReadElementContentAsString(), out isAdmin);
                            IsAdmin = isAdmin;
                            break;
                        case "isPremium":
                            bool isPremium;
                            Boolean.TryParse(reader.ReadElementContentAsString(), out isPremium);
                            IsPremium = isPremium;
                            break;
                    }
                }
            }
        }
		
		/// -----------------------------------------------------------------------------
        /// <summary>
        /// Writes a DesktopModuleInfo to an XmlWriter
        /// </summary>
        /// <param name="writer">The XmlWriter to use</param>
        /// <history>
        /// 	[cnurse]	01/17/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------

        public void WriteXml(XmlWriter writer)
        {
            //Write start of main elemenst
            writer.WriteStartElement("desktopModule");

            //write out properties
            writer.WriteElementString("moduleName", ModuleName);
            writer.WriteElementString("foldername", FolderName);
            writer.WriteElementString("businessControllerClass", BusinessControllerClass);
            if (!string.IsNullOrEmpty(CodeSubDirectory))
            {
                writer.WriteElementString("codeSubDirectory", CodeSubDirectory);
            }
			
            //Write out Supported Features
            writer.WriteStartElement("supportedFeatures");
            if (IsPortable)
            {
                writer.WriteStartElement("supportedFeature");
                writer.WriteAttributeString("type", "Portable");
                writer.WriteEndElement();
            }
            if (IsSearchable)
            {
                writer.WriteStartElement("supportedFeature");
                writer.WriteAttributeString("type", "Searchable");
                writer.WriteEndElement();
            }
            if (IsUpgradeable)
            {
                writer.WriteStartElement("supportedFeature");
                writer.WriteAttributeString("type", "Upgradeable");
                writer.WriteEndElement();
            }
            //Write end of Supported Features
            writer.WriteEndElement();

            //Write start of Module Definitions
            writer.WriteStartElement("moduleDefinitions");

            //Iterate through definitions
            foreach (ModuleDefinitionInfo definition in ModuleDefinitions.Values)
            {
                definition.WriteXml(writer);
            }
            //Write end of Module Definitions
            writer.WriteEndElement();

            //Write end of main element
            writer.WriteEndElement();
        }

        #endregion

        private void ClearFeature(DesktopModuleSupportedFeature feature)
        {
            SupportedFeatures = SupportedFeatures & (int) feature;
        }

        private bool GetFeature(DesktopModuleSupportedFeature feature)
        {
            bool isSet = false;
            if (SupportedFeatures > Null.NullInteger && (SupportedFeatures & (int) feature) == (int) feature)
            {
                isSet = true;
            }
            return isSet;
        }

        private void SetFeature(DesktopModuleSupportedFeature feature)
        {
            SupportedFeatures = SupportedFeatures ^ (int) feature;
        }

        private void UpdateFeature(DesktopModuleSupportedFeature feature, bool isSet)
        {
            if (isSet)
            {
                SetFeature(feature);
            }
            else
            {
                ClearFeature(feature);
            }
        }

        private void ReadSupportedFeatures(XmlReader reader)
        {
            SupportedFeatures = 0;
            reader.ReadStartElement("supportedFeatures");
            do
            {
                if (reader.HasAttributes)
                {
                    reader.MoveToFirstAttribute();
                    switch (reader.ReadContentAsString())
                    {
                        case "Portable":
                            IsPortable = true;
                            break;
                        case "Searchable":
                            IsSearchable = true;
                            break;
                        case "Upgradeable":
                            IsUpgradeable = true;
                            break;
                    }
                }
            } while (reader.ReadToNextSibling("supportedFeature"));
        }

        private void ReadModuleDefinitions(XmlReader reader)
        {
            reader.ReadStartElement("moduleDefinitions");
            do
            {
                reader.ReadStartElement("moduleDefinition");
                var moduleDefinition = new ModuleDefinitionInfo();
                moduleDefinition.ReadXml(reader);
                ModuleDefinitions.Add(moduleDefinition.FriendlyName, moduleDefinition);
            } while (reader.ReadToNextSibling("moduleDefinition"));
        }
    }
}
