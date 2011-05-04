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
using System.IO;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using DotNetNuke.Services.Installer.Log;
using DotNetNuke.Services.Installer.Packages;

using ICSharpCode.SharpZipLib.Zip;

#endregion

namespace DotNetNuke.Services.Installer
{
    [Serializable]
    public class InstallerInfo
    {
        private readonly Dictionary<string, InstallFile> _Files = new Dictionary<string, InstallFile>();
        private readonly InstallMode _InstallMode = InstallMode.Install;
        private readonly Logger _Log = new Logger();
        private readonly string _PhysicalSitePath = Null.NullString;
        private string _AllowableFiles;
        private bool _IgnoreWhiteList = Null.NullBoolean;
        private bool _Installed = Null.NullBoolean;
        private bool _IsLegacyMode = Null.NullBoolean;
        private InstallFile _ManifestFile;
        private int _PackageID = Null.NullInteger;
        private int _PortalID = Null.NullInteger;
        private bool _RepairInstall = Null.NullBoolean;
        private SecurityAccessLevel _SecurityAccessLevel = SecurityAccessLevel.Host;
        private string _TempInstallFolder = Null.NullString;

        public InstallerInfo()
        {
        }

        public InstallerInfo(string sitePath, InstallMode mode)
        {
            _TempInstallFolder = Globals.InstallMapPath + "Temp\\" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            _PhysicalSitePath = sitePath;
            _InstallMode = mode;
        }

        public InstallerInfo(Stream inputStream, string sitePath)
        {
            _TempInstallFolder = Globals.InstallMapPath + "Temp\\" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            _PhysicalSitePath = sitePath;
            _InstallMode = InstallMode.Install;
            ReadZipStream(inputStream, false);
        }

        public InstallerInfo(string tempFolder, string manifest, string sitePath)
        {
            _TempInstallFolder = tempFolder;
            _PhysicalSitePath = sitePath;
            _InstallMode = InstallMode.Install;
            if (!string.IsNullOrEmpty(manifest))
            {
                _ManifestFile = new InstallFile(manifest, this);
            }
        }

        public InstallerInfo(PackageInfo package, string sitePath)
        {
            _PhysicalSitePath = sitePath;
            _TempInstallFolder = Globals.InstallMapPath + "Temp\\" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            _InstallMode = InstallMode.UnInstall;
            package.AttachInstallerInfo(this);
        }

        public string AllowableFiles
        {
            get
            {
                return _AllowableFiles;
            }
            set
            {
                _AllowableFiles = value;
            }
        }

        public Dictionary<string, InstallFile> Files
        {
            get
            {
                return _Files;
            }
        }

        public bool HasValidFiles
        {
            get
            {
                bool _HasValidFiles = true;
                foreach (InstallFile file in Files.Values)
                {
                    if (!Util.IsFileValid(file, AllowableFiles))
                    {
                        _HasValidFiles = Null.NullBoolean;
                        break;
                    }
                }
                return _HasValidFiles;
            }
        }

        public bool Installed
        {
            get
            {
                return _Installed;
            }
            set
            {
                _Installed = value;
            }
        }

        public InstallMode InstallMode
        {
            get
            {
                return _InstallMode;
            }
        }

        public string InvalidFileExtensions
        {
            get
            {
                string _InvalidFileExtensions = Null.NullString;
                foreach (InstallFile file in Files.Values)
                {
                    if (!Util.IsFileValid(file, AllowableFiles))
                    {
                        _InvalidFileExtensions += ", " + file.Extension;
                    }
                }
                if (!string.IsNullOrEmpty(_InvalidFileExtensions))
                {
                    _InvalidFileExtensions = _InvalidFileExtensions.Substring(2);
                }
                return _InvalidFileExtensions;
            }
        }

        public bool IgnoreWhiteList
        {
            get
            {
                return _IgnoreWhiteList;
            }
            set
            {
                _IgnoreWhiteList = value;
            }
        }

        public bool IsLegacyMode
        {
            get
            {
                return _IsLegacyMode;
            }
            set
            {
                _IsLegacyMode = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return Log.Valid;
            }
        }

        public string LegacyError { get; set; }

        public Logger Log
        {
            get
            {
                return _Log;
            }
        }

        public InstallFile ManifestFile
        {
            get
            {
                return _ManifestFile;
            }
        }

        public int PackageID
        {
            get
            {
                return _PackageID;
            }
            set
            {
                _PackageID = value;
            }
        }

        public string PhysicalSitePath
        {
            get
            {
                return _PhysicalSitePath;
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

        public bool RepairInstall
        {
            get
            {
                return _RepairInstall;
            }
            set
            {
                _RepairInstall = value;
            }
        }

        public SecurityAccessLevel SecurityAccessLevel
        {
            get
            {
                return _SecurityAccessLevel;
            }
            set
            {
                _SecurityAccessLevel = value;
            }
        }

        public string TempInstallFolder
        {
            get
            {
                return _TempInstallFolder;
            }
        }

        private void ReadZipStream(Stream inputStream, bool isEmbeddedZip)
        {
            Log.StartJob(Util.FILES_Reading);
            var unzip = new ZipInputStream(inputStream);
            ZipEntry entry = unzip.GetNextEntry();
            while (entry != null)
            {
                if (!entry.IsDirectory)
                {
                    var file = new InstallFile(unzip, entry, this);
                    if (file.Type == InstallFileType.Resources && (file.Name.ToLowerInvariant() == "containers.zip" || file.Name.ToLowerInvariant() == "skins.zip"))
                    {
                        string tmpInstallFolder = TempInstallFolder;
                        var zipStream = new FileStream(file.TempFileName, FileMode.Open, FileAccess.Read);
                        _TempInstallFolder = Path.Combine(TempInstallFolder, Path.GetFileNameWithoutExtension(file.Name));
                        ReadZipStream(zipStream, true);
                        _TempInstallFolder = tmpInstallFolder;
                        var zipFile = new FileInfo(file.TempFileName);
                        zipFile.Delete();
                    }
                    else
                    {
                        Files[file.FullName.ToLower()] = file;
                        if (file.Type == InstallFileType.Manifest && !isEmbeddedZip)
                        {
                            if (ManifestFile == null)
                            {
                                _ManifestFile = file;
                            }
                            else
                            {
                                if (ManifestFile.Extension == "dnn" && file.Extension == "dnn5")
                                {
                                    _ManifestFile = file;
                                }
                                else
                                {
                                    Log.AddFailure((Util.EXCEPTION_MultipleDnn + ManifestFile.Name + " and " + file.Name));
                                }
                            }
                        }
                    }
                    Log.AddInfo(string.Format(Util.FILE_ReadSuccess, file.FullName));
                }
                entry = unzip.GetNextEntry();
            }
            if (ManifestFile == null)
            {
                Log.AddFailure(Util.EXCEPTION_MissingDnn);
            }
            if (Log.Valid)
            {
                Log.EndJob(Util.FILES_ReadingEnd);
            }
            else
            {
                Log.AddFailure(new Exception(Util.EXCEPTION_FileLoad));
                Log.EndJob(Util.FILES_ReadingEnd);
            }
            inputStream.Close();
        }
    }
}
