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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Permissions;

#endregion

namespace DotNetNuke.Entities.Modules.Definitions
{
    [Serializable]
    public class ModuleDefinitionInfo : IXmlSerializable, IHydratable
    {
        private Dictionary<string, ModuleControlInfo> _ModuleControls;
        private int _TempModuleID;

        public ModuleDefinitionInfo()
        {
            Permissions = new Dictionary<string, PermissionInfo>();
            DesktopModuleID = Null.NullInteger;
            ModuleDefID = Null.NullInteger;
        }

        public int ModuleDefID { get; set; }

        public int DefaultCacheTime { get; set; }

        public int DesktopModuleID { get; set; }

        public string FriendlyName { get; set; }

        public Dictionary<string, ModuleControlInfo> ModuleControls
        {
            get
            {
                if (_ModuleControls == null)
                {
                    LoadControls();
                }
                return _ModuleControls;
            }
        }

        public Dictionary<string, PermissionInfo> Permissions { get; private set; }

        #region IHydratable Members

        public void Fill(IDataReader dr)
        {
            ModuleDefID = Null.SetNullInteger(dr["ModuleDefID"]);
            DesktopModuleID = Null.SetNullInteger(dr["DesktopModuleID"]);
            DefaultCacheTime = Null.SetNullInteger(dr["DefaultCacheTime"]);
            FriendlyName = Null.SetNullString(dr["FriendlyName"]);
        }

        public int KeyID
        {
            get
            {
                return ModuleDefID;
            }
            set
            {
                ModuleDefID = value;
            }
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
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "moduleControls")
                {
                    ReadModuleControls(reader);
                }
                else
                {
                    switch (reader.Name)
                    {
                        case "friendlyName":
                            FriendlyName = reader.ReadElementContentAsString();
                            break;
                        case "defaultCacheTime":
                            string elementvalue = reader.ReadElementContentAsString();
                            if (!string.IsNullOrEmpty(elementvalue))
                            {
                                DefaultCacheTime = int.Parse(elementvalue);
                            }
                            break;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("moduleDefinition");
            writer.WriteElementString("friendlyName", FriendlyName);
            writer.WriteElementString("defaultCacheTime", DefaultCacheTime.ToString());
            writer.WriteStartElement("moduleControls");
            foreach (ModuleControlInfo control in ModuleControls.Values)
            {
                control.WriteXml(writer);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        #endregion

        public void LoadControls()
        {
            _ModuleControls = ModuleDefID > Null.NullInteger ? ModuleControlController.GetModuleControlsByModuleDefinitionID(ModuleDefID) : new Dictionary<string, ModuleControlInfo>();
        }

        private void ReadModuleControls(XmlReader reader)
        {
            reader.ReadStartElement("moduleControls");
            do
            {
                reader.ReadStartElement("moduleControl");
                var moduleControl = new ModuleControlInfo();
                moduleControl.ReadXml(reader);
                ModuleControls.Add(moduleControl.ControlKey, moduleControl);
            } while (reader.ReadToNextSibling("moduleControl"));
        }

        #region Obsolete Members

        [Obsolete("No longer used in DotNetNuke 5.0 as new Installer does not need this.")]
        public int TempModuleID
        {
            get
            {
                return _TempModuleID;
            }
            set
            {
                _TempModuleID = value;
            }
        }

        #endregion
    }
}
