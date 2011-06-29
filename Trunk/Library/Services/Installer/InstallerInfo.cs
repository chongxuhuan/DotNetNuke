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
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The InstallerInfo class holds all the information associated with a
    /// Installation.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	07/24/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class InstallerInfo
    {
		#region "Private Members"
		
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
		
		#endregion

		#region "Constructors"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This Constructor creates a new InstallerInfo instance
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/26/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public InstallerInfo()
        {
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This Constructor creates a new InstallerInfo instance from a 
        /// string representing the physical path to the root of the site
        /// </summary>
        /// <param name="sitePath">The physical path to the root of the site</param>
        /// <param name="mode">Install Mode.</param>
        /// <history>
        /// 	[cnurse]	02/29/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public InstallerInfo(string sitePath, InstallMode mode)
        {
            _TempInstallFolder = Globals.InstallMapPath + "Temp\\" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            _PhysicalSitePath = sitePath;
            _InstallMode = mode;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This Constructor creates a new InstallerInfo instance from a Stream and a
        /// string representing the physical path to the root of the site
        /// </summary>
        /// <param name="inputStream">The Stream to use to create this InstallerInfo instance</param>
        /// <param name="sitePath">The physical path to the root of the site</param>
        /// <history>
        /// 	[cnurse]	07/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public InstallerInfo(Stream inputStream, string sitePath)
        {
            _TempInstallFolder = Globals.InstallMapPath + "Temp\\" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            _PhysicalSitePath = sitePath;
            _InstallMode = InstallMode.Install;

            //Read the Zip file into its component entries
            ReadZipStream(inputStream, false);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This Constructor creates a new InstallerInfo instance from a string representing
        /// the physical path to the temporary install folder and a string representing 
        /// the physical path to the root of the site
        /// </summary>
        /// <param name="tempFolder">The physical path to the zip file containg the package</param>
        /// <param name="manifest">The manifest filename</param>
        /// <param name="sitePath">The physical path to the root of the site</param>
        /// <history>
        /// 	[cnurse]	08/13/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This Constructor creates a new InstallerInfo instance from a PackageInfo object
        /// </summary>
        /// <param name="package">The PackageInfo instance</param>
        /// <param name="sitePath">The physical path to the root of the site</param>
        /// <history>
        /// 	[cnurse]	07/31/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public InstallerInfo(PackageInfo package, string sitePath)
        {
            _PhysicalSitePath = sitePath;
            _TempInstallFolder = Globals.InstallMapPath + "Temp\\" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            _InstallMode = InstallMode.UnInstall;
            package.AttachInstallerInfo(this);
        }
		
		#endregion

		#region "Public Properties"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets a list of allowable file extensions (in addition to the Host's List)
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	03/28/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Dictionary of Files that are included in the Package
        /// </summary>
        /// <value>A Dictionary(Of String, InstallFile)</value>
        /// <history>
        /// 	[cnurse]	07/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public Dictionary<string, InstallFile> Files
        {
            get
            {
                return _Files;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the package contains Valid Files
        /// </summary>
        /// <value>A Boolean</value>
        /// <history>
        /// 	[cnurse]	09/24/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the Package is installed
        /// </summary>
        /// <value>A Boolean value</value>
        /// <history>
        /// 	[cnurse]	09/24/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the InstallMode
        /// </summary>
        /// <value>A InstallMode value</value>
        /// <history>
        /// 	[cnurse]	07/31/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public InstallMode InstallMode
        {
            get
            {
                return _InstallMode;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Invalid File Extensions
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	01/12/2009  created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether the File Extension WhiteList is ignored
        /// </summary>
        /// <value>A Boolean value</value>
        /// <history>
        /// 	[cnurse]	05/06/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the Installer is in legacy mode
        /// </summary>
        /// <history>
        /// 	[cnurse]	08/20/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the InstallerInfo instance is Valid
        /// </summary>
        /// <value>A Boolean value</value>
        /// <history>
        /// 	[cnurse]	08/13/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool IsValid
        {
            get
            {
                return Log.Valid;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the associated Logger
        /// </summary>
        /// <value>A Logger</value>
        /// <history>
        /// 	[cnurse]	07/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string LegacyError { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the associated Logger
        /// </summary>
        /// <value>A Logger</value>
        /// <history>
        /// 	[cnurse]	07/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public Logger Log
        {
            get
            {
                return _Log;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the Manifest File for the Package
        /// </summary>
        /// <value>An InstallFile</value>
        /// <history>
        /// 	[cnurse]	07/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public InstallFile ManifestFile
        {
            get
            {
                return _ManifestFile;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Id of the package after installation (-1 if fail)
        /// </summary>
        /// <value>An Integer</value>
        /// <history>
        /// 	[cnurse]	08/22/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Physical Path to the root of the Site (eg D:\Websites\DotNetNuke")
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	07/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string PhysicalSitePath
        {
            get
            {
                return _PhysicalSitePath;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Id of the current portal (-1 if Host)
        /// </summary>
        /// <value>An Integer</value>
        /// <history>
        /// 	[cnurse]	08/22/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether the Package Install is being repaird
        /// </summary>
        /// <value>A Boolean value</value>
        /// <history>
        /// 	[cnurse]	09/24/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the security Access Level of the user that is calling the INstaller
        /// </summary>
        /// <value>A SecurityAccessLevel enumeration</value>
        /// <history>
        /// 	[cnurse]	08/22/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Temporary Install Folder used to unzip the archive (and to place the 
        /// backups of existing files) during InstallMode
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	08/01/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string TempInstallFolder
        {
            get
            {
                return _TempInstallFolder;
            }
        }
		
		#endregion

		#region "Private Methods"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The ReadZipStream reads a zip stream, and loads the Files Dictionary
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ReadZipStream(Stream inputStream, bool isEmbeddedZip)
        {
            Log.StartJob(Util.FILES_Reading);
            var unzip = new ZipInputStream(inputStream);
            ZipEntry entry = unzip.GetNextEntry();
            while (entry != null)
            {
                if (!entry.IsDirectory)
                {
					//Add file to list
                    var file = new InstallFile(unzip, entry, this);
                    if (file.Type == InstallFileType.Resources && (file.Name.ToLowerInvariant() == "containers.zip" || file.Name.ToLowerInvariant() == "skins.zip"))
                    {
						//Temporarily save the TempInstallFolder
                        string tmpInstallFolder = TempInstallFolder;

                        //Create Zip Stream from File
                        var zipStream = new FileStream(file.TempFileName, FileMode.Open, FileAccess.Read);

                        //Set TempInstallFolder
                        _TempInstallFolder = Path.Combine(TempInstallFolder, Path.GetFileNameWithoutExtension(file.Name));

                        //Extract files from zip
                        ReadZipStream(zipStream, true);

                        //Restore TempInstallFolder
                        _TempInstallFolder = tmpInstallFolder;

                        //Delete zip file
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
                                if (file.Extension == "dnn6" && (ManifestFile.Extension == "dnn" || ManifestFile.Extension == "dnn5"))
                                {
                                   _ManifestFile = file; 
                                }
                                else if (file.Extension == "dnn5" && ManifestFile.Extension == "dnn")
                                {
                                    _ManifestFile = file;
                                }
                                else if (file.Extension == ManifestFile.Extension)
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
			
            //Close the Zip Input Stream as we have finished with it
            inputStream.Close();
        }
		
		#endregion
    }
}
