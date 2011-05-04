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
using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Security.Roles
{
    [Serializable]
    public class RoleGroupInfo : BaseEntityInfo, IHydratable, IXmlSerializable
    {
        private string _Description;
        private int _PortalID = Null.NullInteger;
        private int _RoleGroupID = Null.NullInteger;
        private string _RoleGroupName;
        private Dictionary<string, RoleInfo> _Roles;

        public RoleGroupInfo()
        {
        }

        public RoleGroupInfo(int roleGroupID, int portalID, bool loadRoles)
        {
            _PortalID = portalID;
            _RoleGroupID = roleGroupID;
            if (loadRoles)
            {
                GetRoles();
            }
        }

        public int RoleGroupID
        {
            get
            {
                return _RoleGroupID;
            }
            set
            {
                _RoleGroupID = value;
            }
        }

        public int PortalID
        {
            get
            {
                return _PortalID;
            }
            set
            {
                _PortalID = value;
            }
        }

        public string RoleGroupName
        {
            get
            {
                return _RoleGroupName;
            }
            set
            {
                _RoleGroupName = value;
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        public Dictionary<string, RoleInfo> Roles
        {
            get
            {
                if (_Roles == null && RoleGroupID > Null.NullInteger)
                {
                    GetRoles();
                }
                return _Roles;
            }
        }

        #region IHydratable Members

        public void Fill(IDataReader dr)
        {
            RoleGroupID = Null.SetNullInteger(dr["RoleGroupId"]);
            PortalID = Null.SetNullInteger(dr["PortalID"]);
            RoleGroupName = Null.SetNullString(dr["RoleGroupName"]);
            Description = Null.SetNullString(dr["Description"]);
            FillInternal(dr);
        }

        public int KeyID
        {
            get
            {
                return RoleGroupID;
            }
            set
            {
                RoleGroupID = value;
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
                else if (reader.NodeType == XmlNodeType.Whitespace)
                {
                    continue;
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLowerInvariant())
                    {
                        case "roles":
                            if (!reader.IsEmptyElement)
                            {
                                ReadRoles(reader);
                            }
                            break;
                        case "rolegroupname":
                            RoleGroupName = reader.ReadElementContentAsString();
                            break;
                        case "description":
                            Description = reader.ReadElementContentAsString();
                            break;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("rolegroup");
            writer.WriteElementString("rolegroupname", RoleGroupName);
            writer.WriteElementString("description", Description);
            writer.WriteStartElement("roles");
            if (Roles != null)
            {
                foreach (RoleInfo role in Roles.Values)
                {
                    role.WriteXml(writer);
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        #endregion

        private void GetRoles()
        {
            _Roles = new Dictionary<string, RoleInfo>();
            foreach (RoleInfo role in new RoleController().GetRolesByGroup(PortalID, RoleGroupID))
            {
                _Roles[role.RoleName] = role;
            }
        }

        private void ReadRoles(XmlReader reader)
        {
            reader.ReadStartElement("roles");
            _Roles = new Dictionary<string, RoleInfo>();
            do
            {
                reader.ReadStartElement("role");
                var role = new RoleInfo();
                role.ReadXml(reader);
                _Roles.Add(role.RoleName, role);
            } while (reader.ReadToNextSibling("role"));
        }
    }
}
