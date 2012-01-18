#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
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


namespace DotNetNuke.Security.Roles
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Roles
    /// Class:      RoleInfo
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The RoleInfo class provides the Entity Layer Role object
    /// </summary>
    /// <history>
    ///     [cnurse]    05/23/2005  made compatible with .NET 2.0
    ///     [cnurse]    01/03/2006  added RoleGroupId property
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class RoleInfo : BaseEntityInfo, IHydratable, IXmlSerializable
    {
        #region Private Members

        private RoleType _RoleType = RoleType.None;
        private bool _RoleTypeSet = Null.NullBoolean;

        public RoleInfo()
        {
            TrialFrequency = "N";
            BillingFrequency = "N";
            RoleID = Null.NullInteger;
        }

        #endregion
		
		#region Public Properties

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Role Id
        /// </summary>
        /// <value>An Integer representing the Id of the Role</value>
        /// -----------------------------------------------------------------------------
        [XmlIgnore]
        public int RoleID { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Portal Id for the Role
        /// </summary>
        /// <value>An Integer representing the Id of the Portal</value>
        /// -----------------------------------------------------------------------------
        [XmlIgnore]
        public int PortalID { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the RoleGroup Id
        /// </summary>
        /// <value>An Integer representing the Id of the RoleGroup</value>
        /// -----------------------------------------------------------------------------
        [XmlIgnore]
        public int RoleGroupID { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Role Name
        /// </summary>
        /// <value>A string representing the name of the role</value>
        /// -----------------------------------------------------------------------------
        public string RoleName { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Role Type
        /// </summary>
        /// <value>A enum representing the type of the role</value>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets an sets the Description of the Role
        /// </summary>
        /// <value>A string representing the description of the role</value>
        /// -----------------------------------------------------------------------------
        public string Description { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Billing Frequency for the role
        /// </summary>
        /// <value>A String representing the Billing Frequency of the Role<br/>
        /// <ul>
        /// <list>N - None</list>
        /// <list>O - One time fee</list>
        /// <list>D - Daily</list>
        /// <list>W - Weekly</list>
        /// <list>M - Monthly</list>
        /// <list>Y - Yearly</list>
        /// </ul>
        /// </value>
        /// -----------------------------------------------------------------------------
        public string BillingFrequency { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the fee for the role
        /// </summary>
        /// <value>A single number representing the fee for the role</value>
        /// -----------------------------------------------------------------------------
        public float ServiceFee { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Trial Frequency for the role
        /// </summary>
        /// <value>A String representing the Trial Frequency of the Role<br/>
        /// <ul>
        /// <list>N - None</list>
        /// <list>O - One time fee</list>
        /// <list>D - Daily</list>
        /// <list>W - Weekly</list>
        /// <list>M - Monthly</list>
        /// <list>Y - Yearly</list>
        /// </ul>
        /// </value>
        /// -----------------------------------------------------------------------------
        public string TrialFrequency { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the length of the trial period
        /// </summary>
        /// <value>An integer representing the length of the trial period</value>
        /// -----------------------------------------------------------------------------
        public int TrialPeriod { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the length of the billing period
        /// </summary>
        /// <value>An integer representing the length of the billing period</value>
        /// -----------------------------------------------------------------------------
        public int BillingPeriod { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the trial fee for the role
        /// </summary>
        /// <value>A single number representing the trial fee for the role</value>
        /// -----------------------------------------------------------------------------
        public float TrialFee { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether the role is public
        /// </summary>
        /// <value>A boolean (True/False)</value>
        /// -----------------------------------------------------------------------------
        public bool IsPublic { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether users are automatically assigned to the role
        /// </summary>
        /// <value>A boolean (True/False)</value>
        /// -----------------------------------------------------------------------------
        public bool AutoAssignment { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the RSVP Code for the role
        /// </summary>
        /// <value>A string representing the RSVP Code for the role</value>
        /// -----------------------------------------------------------------------------
        public string RSVPCode { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Icon File for the role
        /// </summary>
        /// <value>A string representing the Icon File for the role</value>
        /// -----------------------------------------------------------------------------
        public string IconFile { get; set; }

        #endregion

        #region IHydratable Members

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fills a RoleInfo from a Data Reader
        /// </summary>
        /// <param name="dr">The Data Reader to use</param>
        /// <history>
        /// 	[cnurse]	03/17/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
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

            //Fill base class fields
            FillInternal(dr);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Key ID
        /// </summary>
        /// <returns>An Integer</returns>
        /// <history>
        /// 	[cnurse]	03/17/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets an XmlSchema for the RoleInfo
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/14/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Reads a RoleInfo from an XmlReader
        /// </summary>
        /// <param name="reader">The XmlReader to use</param>
        /// <history>
        /// 	[cnurse]	03/14/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
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
                if (reader.NodeType == XmlNodeType.Element)
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Writes a RoleInfo to an XmlWriter
        /// </summary>
        /// <param name="writer">The XmlWriter to use</param>
        /// <history>
        /// 	[cnurse]	03/14/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void WriteXml(XmlWriter writer)
        {
            //Write start of main elemenst
            writer.WriteStartElement("role");

            //write out properties
            writer.WriteElementString("rolename", RoleName);
            writer.WriteElementString("description", Description);
            writer.WriteElementString("billingfrequency", BillingFrequency);
            writer.WriteElementString("billingperiod", BillingPeriod.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("servicefee", ServiceFee.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("trialfrequency", TrialFrequency);
            writer.WriteElementString("trialperiod", TrialPeriod.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("trialfee", TrialFee.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("ispublic", IsPublic.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
            writer.WriteElementString("autoassignment", AutoAssignment.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
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
			
            //Write end of main element
            writer.WriteEndElement();
        }

        #endregion
    }
}
