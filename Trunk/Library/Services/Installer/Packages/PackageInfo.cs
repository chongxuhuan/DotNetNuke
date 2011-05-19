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
        public PackageInfo(InstallerInfo info) : this()
        {
            AttachInstallerInfo(info);
        }

        public PackageInfo()
        {
            PackageID = Null.NullInteger;
            PortalID = Null.NullInteger;
            Version = new Version(0, 0, 0);
            IsValid = true;
            InstalledVersion = new Version(0, 0, 0);
        }

        public int PackageID { get; set; }

        public int PortalID { get; set; }

        public string Owner { get; set; }

        public string Organization { get; set; }

        public string Url { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }

        [XmlIgnore]
        public Dictionary<string, InstallFile> Files
        {
            get
            {
                return InstallerInfo.Files;
            }
        }

        public string FriendlyName { get; set; }

        public string FolderName { get; set; }

        public string IconFile { get; set; }

        [XmlIgnore]
        public InstallerInfo InstallerInfo { get; private set; }

        public Version InstalledVersion { get; set; }

        public InstallMode InstallMode
        {
            get
            {
                return InstallerInfo.InstallMode;
            }
        }

        public bool IsSystemPackage { get; set; }

        [XmlIgnore]
        public bool IsValid { get; private set; }

        public string License { get; set; }

        [XmlIgnore]
        public Logger Log
        {
            get
            {
                return InstallerInfo.Log;
            }
        }

        public string Manifest { get; set; }

        public string Name { get; set; }

        public string PackageType { get; set; }

        public string ReleaseNotes { get; set; }

        public Version Version { get; set; }

        public void AttachInstallerInfo(InstallerInfo installer)
        {
            InstallerInfo = installer;
        }
    }
}
