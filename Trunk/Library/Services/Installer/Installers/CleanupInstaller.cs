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
using System.Text.RegularExpressions;
using System.Xml.XPath;

using DotNetNuke.Common.Utilities;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The CleanupInstaller cleans up (removes) files from previous versions
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	09/05/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class CleanupInstaller : FileInstaller
    {
        private string _FileName;

        public override string AllowableFiles
        {
            get
            {
                return "*";
            }
        }

        private bool ProcessCleanupFile()
        {
            Log.AddInfo(string.Format(Util.CLEANUP_Processing, Version.ToString(3)));
            bool bSuccess = true;
            try
            {
                string strListFile = Path.Combine(Package.InstallerInfo.TempInstallFolder, _FileName);
                if (File.Exists(strListFile))
                {
                    FileSystemUtils.DeleteFiles(Regex.Split(FileSystemUtils.ReadFile(strListFile), Environment.NewLine));
                }
                Log.AddInfo(string.Format(Util.CLEANUP_ProcessComplete, Version.ToString(3)));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
                bSuccess = false;
            }
            return bSuccess;
        }

        protected bool CleanupFile(InstallFile insFile)
        {
            try
            {
                if (File.Exists(PhysicalBasePath + insFile.FullName))
                {
                    Util.BackupFile(insFile, PhysicalBasePath, Log);
                }
                Util.DeleteFile(insFile, PhysicalBasePath, Log);
                return true;
            }
            catch (Exception exc)
            {
                Instrumentation.DnnLog.Error(exc);

                return false;
            }
        }

        protected override void ProcessFile(InstallFile file, XPathNavigator nav)
        {
            if (file != null)
            {
                Files.Add(file);
            }
        }

        protected override InstallFile ReadManifestItem(XPathNavigator nav, bool checkFileExists)
        {
            return base.ReadManifestItem(nav, false);
        }

        protected override void RollbackFile(InstallFile installFile)
        {
            if (File.Exists(installFile.BackupFileName))
            {
                Util.RestoreFile(installFile, PhysicalBasePath, Log);
            }
        }

        public override void Commit()
        {
            base.Commit();
        }

        public override void Install()
        {
            try
            {
                bool bSuccess = true;
                if (string.IsNullOrEmpty(_FileName))
                {
                    foreach (InstallFile file in Files)
                    {
                        bSuccess = CleanupFile(file);
                        if (!bSuccess)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    bSuccess = ProcessCleanupFile();
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
            _FileName = Util.ReadAttribute(manifestNav, "fileName");
            base.ReadManifest(manifestNav);
        }

        public override void UnInstall()
        {
        }
    }
}
