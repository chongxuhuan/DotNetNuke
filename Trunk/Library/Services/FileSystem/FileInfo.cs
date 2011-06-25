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
using System.Web;
using System.Xml.Serialization;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;

#endregion

namespace DotNetNuke.Services.FileSystem
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Class	 : FileInfo
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   Represents the File object and holds the Properties of that object
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///   [DYNST]     02/01/2004   Created
    ///   [vnguyen]   04/28/2010   Modified: Added GUID and Version GUID properties
    /// </history>
    /// -----------------------------------------------------------------------------
    [XmlRoot("file", IsNullable = false)]
    [Serializable]
    public class FileInfo : IFileInfo
    {
        private string _folder;
        private FileAttributes? _fileAttributes;
        private bool? _supportsFileAttributes;
        private DateTime? _lastModificationTime;
        private int _folderMappingID;

        #region "Constructors"

        public FileInfo()
        {
            UniqueId = Guid.NewGuid();
            VersionGuid = Guid.NewGuid();
        }

        public FileInfo(int portalId, string filename, string extension, int filesize, int width, int height, string contentType, string folder, int folderId, int storageLocation, bool cached)
            : this(portalId, filename, extension, filesize, width, height, contentType, folder, folderId, storageLocation, cached, Null.NullString)
        {
        }

        public FileInfo(int portalId, string filename, string extension, int filesize, int width, int height, string contentType, string folder, int folderId, int storageLocation, bool cached,
                        string hash)
            : this(Guid.NewGuid(), Guid.NewGuid(), portalId, filename, extension, filesize, width, height, contentType, folder, folderId, storageLocation, cached, hash)
        {
        }

        public FileInfo(Guid uniqueId, Guid versionGuid, int portalId, string filename, string extension, int filesize, int width, int height, string contentType, string folder, int folderId,
                        int storageLocation, bool cached, string hash)
        {
            UniqueId = uniqueId;
            VersionGuid = versionGuid;
            PortalId = portalId;
            FileName = filename;
            Extension = extension;
            Size = filesize;
            Width = width;
            Height = height;
            ContentType = contentType;
            Folder = folder;
            FolderId = folderId;
            StorageLocation = storageLocation;
            IsCached = cached;
            SHA1Hash = hash;
        }

        #endregion

        #region "Properties"

        [XmlElement("contenttype")]
        public string ContentType { get; set; }

        [XmlElement("extension")]
        public string Extension { get; set; }

        [XmlElement("fileid")]
        public int FileId { get; set; }

        [XmlElement("uniqueid")]
        public Guid UniqueId { get; set; }

        [XmlElement("versionguid")]
        public Guid VersionGuid { get; set; }

        [XmlElement("filename")]
        public string FileName { get; set; }

        [XmlElement("folder")]
        public string Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                //Make sure folder name ends with /
                if (!string.IsNullOrEmpty(value) && !value.EndsWith("/"))
                {
                    value = value + "/";
                }

                _folder = value;
            }
        }

        [XmlElement("folderid")]
        public int FolderId { get; set; }

        [XmlElement("height")]
        public int Height { get; set; }

        [XmlElement("iscached")]
        public bool IsCached { get; set; }

        [XmlElement("physicalpath")]
        public string PhysicalPath
        {
            get
            {
                string physicalPath = Null.NullString;
                PortalSettings portalSettings = null;
                if ((HttpContext.Current != null))
                {
                    portalSettings = PortalController.GetCurrentPortalSettings();
                }

                if (PortalId == Null.NullInteger)
                {
                    physicalPath = Globals.HostMapPath + RelativePath;
                }
                else
                {
                    if (portalSettings == null || portalSettings.PortalId != PortalId)
                    {
                        //Get the PortalInfo  based on the Portalid
                        var objPortals = new PortalController();
                        PortalInfo objPortal = objPortals.GetPortal(PortalId);
                        if ((objPortal != null))
                        {
                            physicalPath = objPortal.HomeDirectoryMapPath + RelativePath;
                        }
                    }
                    else
                    {
                        physicalPath = portalSettings.HomeDirectoryMapPath + RelativePath;
                    }
                }

                if ((!string.IsNullOrEmpty(physicalPath)))
                {
                    physicalPath = physicalPath.Replace("/", "\\");
                }

                return physicalPath;
            }
        }

        [XmlIgnore]
        public int PortalId { get; set; }

        public string RelativePath
        {
            get
            {
                return Folder + FileName;
            }
        }

        [XmlElement("size")]
        public int Size { get; set; }

        [XmlElement("storagelocation")]
        public int StorageLocation { get; set; }

        [XmlElement("width")]
        public int Width { get; set; }

        [XmlElement("sha1hash")]
        public string SHA1Hash { get; set; }

        public Nullable<FileAttributes> FileAttributes
        {
            get
            {
                if (SupportsFileAttributes)
                {
                    var folderMapping = FolderMappingController.Instance.GetFolderMapping(FolderMappingID);
                    _fileAttributes = FolderProvider.Instance(folderMapping.FolderProviderType).GetFileAttributes(this);
                }

                return _fileAttributes;
            }
            set
            {
                if (SupportsFileAttributes)
                {
                    _fileAttributes = value;

                    if (value.HasValue)
                    {
                        var folderMapping = FolderMappingController.Instance.GetFolderMapping(FolderMappingID);
                        FolderProvider.Instance(folderMapping.FolderProviderType).SetFileAttributes(this, value.Value);
                    }
                }
            }
        }

        public bool SupportsFileAttributes
        {
            get
            {
                if (!_supportsFileAttributes.HasValue)
                {
                    var folderMapping = FolderMappingController.Instance.GetFolderMapping(FolderMappingID);

                    try
                    {
                        _supportsFileAttributes = FolderProvider.Instance(folderMapping.FolderProviderType).SupportsFileAttributes();
                    }
                    catch
                    {
                        _supportsFileAttributes = false;
                    }
                }

                return _supportsFileAttributes.Value;
            }
        }

        public DateTime LastModificationTime
        {
            get
            {
                if (!_lastModificationTime.HasValue)
                {
                    var folderMapping = FolderMappingController.Instance.GetFolderMapping(FolderMappingID);

                    try
                    {
                        _lastModificationTime = FolderProvider.Instance(folderMapping.FolderProviderType).GetLastModificationTime(this);
                    }
                    catch
                    {
                        _lastModificationTime = Null.NullDate;
                    }
                }

                return _lastModificationTime.Value;
            }
        }

        public int FolderMappingID
        {
            get
            {
                if (_folderMappingID == 0)
                {
                    if (FolderId > 0)
                    {
                        var folder = FolderManager.Instance.GetFolder(FolderId);
                        
                        if (folder != null)
                        {
                            _folderMappingID = folder.FolderMappingID;
                            return _folderMappingID;
                        }
                    }

                    switch (StorageLocation)
                    {
                        case (int)FolderController.StorageLocationTypes.InsecureFileSystem:
                            _folderMappingID = FolderMappingController.Instance.GetFolderMapping(PortalId, "Standard").FolderMappingID;
                            break;
                        case (int)FolderController.StorageLocationTypes.SecureFileSystem:
                            _folderMappingID = FolderMappingController.Instance.GetFolderMapping(PortalId, "Secure").FolderMappingID;
                            break;
                        case (int)FolderController.StorageLocationTypes.DatabaseSecure:
                            _folderMappingID = FolderMappingController.Instance.GetFolderMapping(PortalId, "Database").FolderMappingID;
                            break;
                        default:
                            _folderMappingID = FolderMappingController.Instance.GetDefaultFolderMapping(PortalId).FolderMappingID;
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

        #endregion
    }
}
