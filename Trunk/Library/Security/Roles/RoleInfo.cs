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
using System.Data;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;

#endregion

namespace DotNetNuke.Security.Roles
{
    public enum RoleType
    {
        Administrator,
        Subscriber,
        RegisteredUser,
        None
    }

    [Serializable]
    public class RoleInfo : BaseEntityInfo, IHydratable, IXmlSerializable
    {
        private bool _AutoAssignment;
        private string _BillingFrequency = "N";
        private int _BillingPeriod;
        private string _Description;
        private string _IconFile;
        private bool _IsPublic;
        private int _PortalID;
        private string _RSVPCode;
        private int _RoleGroupID;
        private int _RoleID = Null.NullInteger;
        private string _RoleName;
        private RoleType _RoleType = RoleType.None;
        private bool _RoleTypeSet = Null.NullBoolean;
        private float _ServiceFee;
        private float _TrialFee;
        private string _TrialFrequency = "N";
        private int _TrialPeriod;

        [XmlIgnore]
        public int RoleID
        {
            get
            {
                return _RoleID;
            }
            set
            {
                _RoleID = value;
            }
        }

        [XmlIgnore]
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

        [XmlIgnore]
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

        public string RoleName
        {
            get
            {
                return _RoleName;
            }
            set
            {
                _RoleName = value;
            }
        }

        public RoleType RoleType
        {
            get
            {
                if (!_RoleTypeSet)
                {
                    PortalInfo objPortal = new PortalController().GetPortal(PortalID);
                    if (RoleID == objPortal.AdministratorRoleId)
                    {
                        _RoleType = RoleType.Administrator;
                    }
                    else if (RoleID == objPortal.RegisteredRoleId)
                    {
                        _RoleType = RoleType.RegisteredUser;
                    }
                    else if (RoleName == "Subscribers")
                    {
                        _RoleType = RoleType.Subscriber;
                    }
                    _RoleTypeSet = true;
                }
                return _RoleType;
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

        public string BillingFrequency
        {
            get
            {
                return _BillingFrequency;
            }
            set
            {
                _BillingFrequency = value;
            }
        }

        public float ServiceFee
        {
            get
            {
                return _ServiceFee;
            }
            set
            {
                _ServiceFee = value;
            }
        }

        public string TrialFrequency
        {
            get
            {
                return _TrialFrequency;
            }
            set
            {
                _TrialFrequency = value;
            }
        }

        public int TrialPeriod
        {
            get
            {
                return _TrialPeriod;
            }
            set
            {
                _TrialPeriod = value;
            }
        }

        public int BillingPeriod
        {
            get
            {
                return _BillingPeriod;
            }
            set
            {
                _BillingPeriod = value;
            }
        }

        public float TrialFee
        {
            get
            {
                return _TrialFee;
            }
            set
            {
                _TrialFee = value;
            }
        }

        public bool IsPublic
        {
            get
            {
                return _IsPublic;
            }
            set
            {
                _IsPublic = value;
            }
        }

        public bool AutoAssignment
        {
            get
            {
                return _AutoAssignment;
            }
            set
            {
                _AutoAssignment = value;
            }
        }

        public string RSVPCode
        {
            get
            {
                return _RSVPCode;
            }
            set
            {
                _RSVPCode = value;
            }
        }

        public string IconFile
        {
            get
            {
                return _IconFile;
            }
            set
            {
                _IconFile = value;
            }
        }

        #region IHydratable Members

        public virtual void Fill(IDataReader dr)
        {
            RoleID = Null.SetNullInteger(dr["RoleId"]);
            PortalID = Null.SetNullInteger(dr["PortalID"]);
            RoleGroupID = Null.SetNullInteger(dr["RoleGroupId"]);
            RoleName = Null.SetNullString(dr["RoleName"]);
            Description = Null.SetNullString(dr["Description"]);
            ServiceFee = Null.SetNullSingle(dr["ServiceFee"]);
            BillingPeriod = Null.SetNullInteger(dr["BillingPeriod"]);
            BillingFrequency = Null.SetNullString(dr["BillingFrequency"]);
            TrialFee = Null.SetNullSingle(dr["TrialFee"]);
            TrialPeriod = Null.SetNullInteger(dr["TrialPeriod"]);
            TrialFrequency = Null.SetNullString(dr["TrialFrequency"]);
            IsPublic = Null.SetNullBoolean(dr["IsPublic"]);
            AutoAssignment = Null.SetNullBoolean(dr["AutoAssignment"]);
            RSVPCode = Null.SetNullString(dr["RSVPCode"]);
            IconFile = Null.SetNullString(dr["IconFile"]);
            FillInternal(dr);
        }

        public virtual int KeyID
        {
            get
            {
                return RoleID;
            }
            set
            {
                RoleID = value;
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
                        case "rolename":
                            RoleName = reader.ReadElementContentAsString();
                            break;
                        case "description":
                            Description = reader.ReadElementContentAsString();
                            break;
                        case "billingfrequency":
                            BillingFrequency = reader.ReadElementContentAsString();
                            if (string.IsNullOrEmpty(BillingFrequency))
                            {
                                BillingFrequency = "N";
                            }
                            break;
                        case "billingperiod":
                            BillingPeriod = reader.ReadElementContentAsInt();
                            break;
                        case "servicefee":
                            ServiceFee = reader.ReadElementContentAsFloat();
                            if (ServiceFee < 0)
                            {
                                ServiceFee = 0;
                            }
                            break;
                        case "trialfrequency":
                            TrialFrequency = reader.ReadElementContentAsString();
                            if (string.IsNullOrEmpty(TrialFrequency))
                            {
                                TrialFrequency = "N";
                            }
                            break;
                        case "trialperiod":
                            TrialPeriod = reader.ReadElementContentAsInt();
                            break;
                        case "trialfee":
                            TrialFee = reader.ReadElementContentAsFloat();
                            if (TrialFee < 0)
                            {
                                TrialFee = 0;
                            }
                            break;
                        case "ispublic":
                            IsPublic = reader.ReadElementContentAsBoolean();
                            break;
                        case "autoassignment":
                            AutoAssignment = reader.ReadElementContentAsBoolean();
                            break;
                        case "rsvpcode":
                            RSVPCode = reader.ReadElementContentAsString();
                            break;
                        case "iconfile":
                            IconFile = reader.ReadElementContentAsString();
                            break;
                        case "roletype":
                            switch (reader.ReadElementContentAsString())
                            {
                                case "adminrole":
                                    _RoleType = RoleType.Administrator;
                                    break;
                                case "registeredrole":
                                    _RoleType = RoleType.RegisteredUser;
                                    break;
                                case "subscriberrole":
                                    _RoleType = RoleType.Subscriber;
                                    break;
                                default:
                                    _RoleType = RoleType.None;
                                    break;
                            }
                            _RoleTypeSet = true;
                            break;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("role");
            writer.WriteElementString("rolename", RoleName);
            writer.WriteElementString("description", Description);
            writer.WriteElementString("billingfrequency", BillingFrequency);
            writer.WriteElementString("billingperiod", BillingPeriod.ToString());
            writer.WriteElementString("servicefee", ServiceFee.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("trialfrequency", TrialFrequency);
            writer.WriteElementString("trialperiod", TrialPeriod.ToString());
            writer.WriteElementString("trialfee", TrialFee.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("ispublic", IsPublic.ToString().ToLowerInvariant());
            writer.WriteElementString("autoassignment", AutoAssignment.ToString().ToLowerInvariant());
            writer.WriteElementString("rsvpcode", RSVPCode);
            writer.WriteElementString("iconfile", IconFile);
            switch (RoleType)
            {
                case RoleType.Administrator:
                    writer.WriteElementString("roletype", "adminrole");
                    break;
                case RoleType.RegisteredUser:
                    writer.WriteElementString("roletype", "registeredrole");
                    break;
                case RoleType.Subscriber:
                    writer.WriteElementString("roletype", "subscriberrole");
                    break;
                case RoleType.None:
                    writer.WriteElementString("roletype", "none");
                    break;
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
