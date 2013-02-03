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
#region Usings

using System;
using System.Data;
using System.Web;
using System.Xml.Serialization;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;

#endregion

namespace DotNetNuke.Services.FileSystem
{

    #region "FolderInfo"

    [XmlRoot("folder", IsNullable = false)]
    [Serializable]
    public class FolderInfo : IHydratable, IFolderInfo
    {
        // local property declarations
        private string _displayName;
        private string _displayPath;
        private FolderPermissionCollection _folderPermissions;
        private int _folderMappingID;

        #region "Constructors"

        public FolderInfo()
        {
            FolderID = Null.NullInteger;
            UniqueId = Guid.NewGuid();
            VersionGuid = Guid.NewGuid();
        }

        public FolderInfo(int portalId, string folderpath, int storageLocation, bool isProtected, bool isCached, DateTime lastUpdated)
            : this(Guid.NewGuid(), portalId, folderpath, storageLocation, isProtected, isCached, lastUpdated)
        {
        }

        public FolderInfo(Guid uniqueID, int portalId, string folderpath, int storageLocation, bool isProtected, bool isCached, DateTime lastUpdated)
        {
            FolderID = Null.NullInteger;
            UniqueId = uniqueID;
            VersionGuid = Guid.NewGuid();

            PortalID = portalId;
            FolderPath = folderpath;
            StorageLocation = storageLocation;
            IsProtected = isProtected;
            IsCached = isCached;
            LastUpdated = lastUpdated;
        }

        #endregion

        #region "Public Properties"

        [XmlElement("folderid")]
        public int FolderID { get; set; }

        [XmlElement("uniqueid")]
        public Guid UniqueId { get; set; }

        [XmlElement("versionguid")]
        public Guid VersionGuid { get; set; }

        [XmlElement("foldername")]
        public string FolderName
        {
            get
            {
                string folderName = PathUtils.Instance.RemoveTrailingSlash(FolderPath);
                if (folderName.Length > 0 && folderName.LastIndexOf("/") > -1)
                {
                    folderName = folderName.Substring(folderName.LastIndexOf("/") + 1);
                }
                return folderName;
            }
        }

        [XmlElement("displayname")]
        public string DisplayName
        {
            get
            {
                if ((!string.IsNullOrEmpty(_displayName)))
                {
                    _displayName = FolderName;
                }
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        [XmlElement("folderpath")]
        public string FolderPath { get; set; }

        [XmlElement("displaypath")]
        public string DisplayPath
        {
            get
            {
                if ((string.IsNullOrEmpty(_displayPath)))
                {
                    _displayPath = FolderPath;
                }
                return _displayPath;
            }
            set
            {
                _displayPath = value;
            }
        }

        [XmlElement("iscached")]
        public bool IsCached { get; set; }

        [XmlElement("isprotected")]
        public bool IsProtected { get; set; }

        [XmlIgnore]
        public DateTime LastUpdated { get; set; }

        [XmlElement("physicalpath")]
        public string PhysicalPath
        {
            get
            {
                string physicalPath;
                PortalSettings portalSettings = null;
                if ((HttpContext.Current != null))
                {
                    portalSettings = PortalController.GetCurrentPortalSettings();
                }

                if (PortalID == Null.NullInteger)
                {
                    physicalPath = Globals.HostMapPath + FolderPath;
                }
                else
                {
                    if (portalSettings == null || portalSettings.PortalId != PortalID)
                    {
                        //Get the PortalInfo  based on the Portalid
                        var objPortals = new PortalController();
                        PortalInfo objPortal = objPortals.GetPortal(PortalID);

                        physicalPath = objPortal.HomeDirectoryMapPath + FolderPath;
                    }
                    else
                    {
                        physicalPath = portalSettings.HomeDirectoryMapPath + FolderPath;
                    }
                }

                return physicalPath.Replace("/", "\\");
            }
        }

        [XmlElement("portalid")]
        public int PortalID { get; set; }

        [XmlElement("storagelocation")]
        public int StorageLocation { get; set; }

        [XmlElement("folderpermissions")]
        public FolderPermissionCollection FolderPermissions
        {
            get
            {
                if (_folderPermissions == null)
                {
                    _folderPermissions = new FolderPermissionCollection(FolderPermissionController.GetFolderPermissionsCollectionByFolder(PortalID, FolderPath));
                }
                return _folderPermissions;
            }
        }

        public int FolderMappingID
        {
            get
            {
                if (_folderMappingID == 0)
                {
                    switch (StorageLocation)
                    {
                        case (int)FolderController.StorageLocationTypes.InsecureFileSystem:
                            _folderMappingID = FolderMappingController.Instance.GetFolderMapping(PortalID, "Standard").FolderMappingID;
                            break;
                        case (int)FolderController.StorageLocationTypes.SecureFileSystem:
                            _folderMappingID = FolderMappingController.Instance.GetFolderMapping(PortalID, "Secure").FolderMappingID;
                            break;
                        case (int)FolderController.StorageLocationTypes.DatabaseSecure:
                            _folderMappingID = FolderMappingController.Instance.GetFolderMapping(PortalID, "Database").FolderMappingID;
                            break;
                        default:
                            _folderMappingID = FolderMappingController.Instance.GetDefaultFolderMapping(PortalID).FolderMappingID;
                            break;
                    }
                }

                return _folderMappingID;
            }
            set
            {
                _folderMappingID = value;
            }
        }

        public bool IsStorageSecure
        {
            get
            {
                var folderMapping = FolderMappingController.Instance.GetFolderMapping(FolderMappingID);
                return FolderProvider.Instance(folderMapping.FolderProviderType).IsStorageSecure;
            }   
        }

        #endregion

        #region "IHydratable Implementation"

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Fills a FolderInfo from a Data Reader
        /// </summary>
        /// <param name = "dr">The Data Reader to use</param>
        /// <history>
        ///   [cnurse]	07/14/2008   Documented
        ///   [vnguyen]   30/04/2010   Modified: Added VersionGuid
        /// </history>
        /// -----------------------------------------------------------------------------
        public void Fill(IDataReader dr)
        {
            FolderID = Null.SetNullInteger(dr["FolderID"]);
            UniqueId = Null.SetNullGuid(dr["UniqueId"]);
            VersionGuid = Null.SetNullGuid(dr["VersionGuid"]);
            PortalID = Null.SetNullInteger(dr["PortalID"]);
            FolderPath = Null.SetNullString(dr["FolderPath"]);
            IsCached = Null.SetNullBoolean(dr["IsCached"]);
            IsProtected = Null.SetNullBoolean(dr["IsProtected"]);
            StorageLocation = Null.SetNullInteger(dr["StorageLocation"]);
            LastUpdated = Null.SetNullDateTime(dr["LastUpdated"]);
            FolderMappingID = Null.SetNullInteger(dr["FolderMappingID"]);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets and sets the Key ID
        /// </summary>
        /// <returns>An Integer</returns>
        /// <history>
        ///   [cnurse]	07/14/2008   Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        [XmlIgnore]
        public int KeyID
        {
            get
            {
                return FolderID;
            }
            set
            {
                FolderID = value;
            }
        }

        #endregion
    }

    #endregion
}
