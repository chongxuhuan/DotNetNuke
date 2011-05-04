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
using System.Xml.XPath;

using DotNetNuke.Common.Utilities;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The FileInstaller installs File Components to a DotNetNuke site
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	07/24/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class FileInstaller : ComponentInstallerBase
    {
        private readonly List<InstallFile> _Files = new List<InstallFile>();
        private string _BasePath;
        private bool _DeleteFiles = Null.NullBoolean;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the BasePath for the files
        /// </summary>
        /// <remarks>The Base Path is relative to the WebRoot</remarks>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	07/25/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected string BasePath
        {
            get
            {
                return _BasePath;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the name of the Collection Node ("files")
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	08/07/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected virtual string CollectionNodeName
        {
            get
            {
                return "files";
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Dictionary of Files that are included in this component
        /// </summary>
        /// <value>A Dictionary(Of String, InstallFile)</value>
        /// <history>
        /// 	[cnurse]	07/25/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected List<InstallFile> Files
        {
            get
            {
                return _Files;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the default Path for the file - if not present in the manifest
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	08/10/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected virtual string DefaultPath
        {
            get
            {
                return Null.NullString;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the name of the Item Node ("file")
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	08/07/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected virtual string ItemNodeName
        {
            get
            {
                return "file";
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the PhysicalBasePath for the files
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	07/25/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected virtual string PhysicalBasePath
        {
            get
            {
                string _PhysicalBasePath = PhysicalSitePath + "\\" + BasePath;
                if (!_PhysicalBasePath.EndsWith("\\"))
                {
                    _PhysicalBasePath += "\\";
                }
                return _PhysicalBasePath.Replace("/", "\\");
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether the Packages files are deleted when uninstalling the
        /// package
        /// </summary>
        /// <value>A Boolean value</value>
        /// <history>
        /// 	[cnurse]	01/31/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool DeleteFiles
        {
            get
            {
                return _DeleteFiles;
            }
            set
            {
                _DeleteFiles = value;
            }
        }

        public override bool SupportsManifestOnlyInstall
        {
            get
            {
                return Null.NullBoolean;
            }
        }

        protected virtual void CommitFile(InstallFile insFile)
        {
        }

        protected virtual void DeleteFile(InstallFile insFile)
        {
            if (DeleteFiles)
            {
                Util.DeleteFile(insFile, PhysicalBasePath, Log);
            }
        }

        protected virtual bool InstallFile(InstallFile insFile)
        {
            try
            {
                if ((Package.InstallerInfo.IgnoreWhiteList || Util.IsFileValid(insFile, Package.InstallerInfo.AllowableFiles)))
                {
                    if (File.Exists(PhysicalBasePath + insFile.FullName))
                    {
                        Util.BackupFile(insFile, PhysicalBasePath, Log);
                    }
                    Util.CopyFile(insFile, PhysicalBasePath, Log);
                    return true;
                }
                else
                {
                    Log.AddFailure(string.Format(Util.FILE_NotAllowed, insFile.FullName));
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
                return false;
            }
        }

        protected virtual bool IsCorrectType(InstallFileType type)
        {
            return true;
        }

        protected virtual void ProcessFile(InstallFile file, XPathNavigator nav)
        {
            if (file != null && IsCorrectType(file.Type))
            {
                Files.Add(file);
                Package.InstallerInfo.Files[file.FullName.ToLower()] = file;
            }
        }

        protected virtual void ReadCustomManifest(XPathNavigator nav)
        {
        }

        protected virtual InstallFile ReadManifestItem(XPathNavigator nav, bool checkFileExists)
        {
            string fileName = Null.NullString;
            XPathNavigator pathNav = nav.SelectSingleNode("path");
            if (pathNav == null)
            {
                fileName = DefaultPath;
            }
            else
            {
                fileName = pathNav.Value + "\\";
            }
            XPathNavigator nameNav = nav.SelectSingleNode("name");
            if (nameNav != null)
            {
                fileName += nameNav.Value;
            }
            string sourceFileName = Util.ReadElement(nav, "sourceFileName");
            var file = new InstallFile(fileName, sourceFileName, Package.InstallerInfo);
            if ((!string.IsNullOrEmpty(BasePath)) && (BasePath.ToLowerInvariant().StartsWith("app_code") && file.Type == InstallFileType.Other))
            {
                file.Type = InstallFileType.AppCode;
            }
            if (file != null)
            {
                string strVersion = XmlUtils.GetNodeValue(nav, "version");
                if (!string.IsNullOrEmpty(strVersion))
                {
                    file.SetVersion(new Version(strVersion));
                }
                else
                {
                    file.SetVersion(Package.Version);
                }
                string strAction = XmlUtils.GetAttributeValue(nav, "action");
                if (!string.IsNullOrEmpty(strAction))
                {
                    file.Action = strAction;
                }
                if (InstallMode == InstallMode.Install && checkFileExists && file.Action != "UnRegister")
                {
                    if (File.Exists(file.TempFileName))
                    {
                        Log.AddInfo(string.Format(Util.FILE_Found, file.Path, file.Name));
                    }
                    else
                    {
                        Log.AddFailure(Util.FILE_NotFound + " - " + file.TempFileName);
                    }
                }
            }
            return file;
        }

        protected virtual void RollbackFile(InstallFile installFile)
        {
            if (File.Exists(installFile.BackupFileName))
            {
                Util.RestoreFile(installFile, PhysicalBasePath, Log);
            }
            else
            {
                DeleteFile(installFile);
            }
        }

        protected virtual void UnInstallFile(InstallFile unInstallFile)
        {
            DeleteFile(unInstallFile);
        }

        public override void Commit()
        {
            try
            {
                foreach (InstallFile file in Files)
                {
                    CommitFile(file);
                }
                Completed = true;
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
        }

        public override void Install()
        {
            try
            {
                bool bSuccess = true;
                foreach (InstallFile file in Files)
                {
                    bSuccess = InstallFile(file);
                    if (!bSuccess)
                    {
                        break;
                    }
                }
                Completed = bSuccess;
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
        }

        public override void ReadManifest(XPathNavigator manifestNav)
        {
            XPathNavigator rootNav = manifestNav.SelectSingleNode(CollectionNodeName);
            if (rootNav != null)
            {
                XPathNavigator baseNav = rootNav.SelectSingleNode("basePath");
                if (baseNav != null)
                {
                    _BasePath = baseNav.Value;
                }
                ReadCustomManifest(rootNav);
                foreach (XPathNavigator nav in rootNav.Select(ItemNodeName))
                {
                    ProcessFile(ReadManifestItem(nav, true), nav);
                }
            }
        }

        public override void Rollback()
        {
            try
            {
                foreach (InstallFile file in Files)
                {
                    RollbackFile(file);
                }
                Completed = true;
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
        }

        public override void UnInstall()
        {
            try
            {
                foreach (InstallFile file in Files)
                {
                    UnInstallFile(file);
                }
                Completed = true;
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
        }
    }
}
