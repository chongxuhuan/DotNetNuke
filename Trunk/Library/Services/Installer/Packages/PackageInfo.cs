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
using System.Xml.Serialization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities;
using DotNetNuke.Services.Installer.Log;

#endregion

namespace DotNetNuke.Services.Installer.Packages
{
    [Serializable]
    public class PackageInfo : BaseEntityInfo
    {
        private string _description;
        private string _email;
        private string _friendlyName;
        private string _folderName;
        private string _iconFile;
        private Version _installedVersion = new Version(0, 0, 0);
        private InstallerInfo _installerInfo;
        private bool _isSystemPackage;
        private bool _isValid = true;
        private string _license;
        private string _manifest;
        private string _name;
        private string _organization;
        private string _owner;
        private int _packageID = Null.NullInteger;
        private string _packageType;
        private int _portalID = Null.NullInteger;
        private string _releaseNotes;
        private string _url;
        private Version _version = new Version(0, 0, 0);

        public PackageInfo(InstallerInfo info)
        {
            AttachInstallerInfo(info);
        }

        public PackageInfo()
        {
        }

        public int PackageID
        {
            get
            {
                return _packageID;
            }
            set
            {
                _packageID = value;
            }
        }

        public int PortalID
        {
            get
            {
                return _portalID;
            }
            set
            {
                _portalID = value;
            }
        }

        public string Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }

        public string Organization
        {
            get
            {
                return _organization;
            }
            set
            {
                _organization = value;
            }
        }

        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        [XmlIgnore]
        public Dictionary<string, InstallFile> Files
        {
            get
            {
                return InstallerInfo.Files;
            }
        }

        public string FriendlyName
        {
            get
            {
                return _friendlyName;
            }
            set
            {
                _friendlyName = value;
            }
        }

        public string FolderName
        {
            get
            {
                return _folderName;
            }
            set
            {
                _folderName = value;
            }
        }

        public string IconFile
        {
            get
            {
                return _iconFile;
            }
            set
            {
                _iconFile = value;
            }
        }

        [XmlIgnore]
        public InstallerInfo InstallerInfo
        {
            get
            {
                return _installerInfo;
            }
        }

        public Version InstalledVersion
        {
            get
            {
                return _installedVersion;
            }
            set
            {
                _installedVersion = value;
            }
        }

        public InstallMode InstallMode
        {
            get
            {
                return InstallerInfo.InstallMode;
            }
        }

        public bool IsSystemPackage
        {
            get
            {
                return _isSystemPackage;
            }
            set
            {
                _isSystemPackage = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
        }

        public string License
        {
            get
            {
                return _license;
            }
            set
            {
                _license = value;
            }
        }

        [XmlIgnore]
        public Logger Log
        {
            get
            {
                return InstallerInfo.Log;
            }
        }

        public string Manifest
        {
            get
            {
                return _manifest;
            }
            set
            {
                _manifest = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string ReleaseNotes
        {
            get
            {
                return _releaseNotes;
            }
            set
            {
                _releaseNotes = value;
            }
        }

        public string PackageType
        {
            get
            {
                return _packageType;
            }
            set
            {
                _packageType = value;
            }
        }

        public Version Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        public void AttachInstallerInfo(InstallerInfo installer)
        {
            _installerInfo = installer;
        }
    }
}
