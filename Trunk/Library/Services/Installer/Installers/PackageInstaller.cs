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

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Installer.Dependencies;
using DotNetNuke.Services.Installer.Packages;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The PackageInstaller class is an Installer for Packages
    /// </summary>
    /// <history>
    /// 	[cnurse]	01/16/2008  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class PackageInstaller : ComponentInstallerBase
    {
        private readonly SortedList<int, ComponentInstallerBase> _componentInstallers = new SortedList<int, ComponentInstallerBase>();
        private PackageInfo _installedPackage;
        private bool _deleteFiles = Null.NullBoolean;
        private bool _isValid = true;

        public PackageInstaller(PackageInfo package)
        {
            Package = package;
            if (!string.IsNullOrEmpty(package.Manifest))
            {
                var doc = new XPathDocument(new StringReader(package.Manifest));
                XPathNavigator nav = doc.CreateNavigator().SelectSingleNode("package");
                ReadComponents(nav);
            }
            else
            {
                ComponentInstallerBase installer = InstallerFactory.GetInstaller(package.PackageType);
                if (installer != null)
                {
                    installer.Package = package;
                    installer.Type = package.PackageType;
                    _componentInstallers.Add(0, installer);
                }
            }
        }

        public PackageInstaller(string packageManifest, InstallerInfo info)
        {
            Package = new PackageInfo(info);
            Package.Manifest = packageManifest;
            if (!string.IsNullOrEmpty(packageManifest))
            {
                var doc = new XPathDocument(new StringReader(packageManifest));
                XPathNavigator nav = doc.CreateNavigator().SelectSingleNode("package");
                ReadManifest(nav);
            }
        }

        public bool DeleteFiles
        {
            get
            {
                return _deleteFiles;
            }
            set
            {
                _deleteFiles = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
        }

        private void CheckSecurity()
        {
            PackageType type = PackageController.GetPackageType(Package.PackageType);
            if (type == null)
            {
                Log.Logs.Clear();
                Log.AddFailure(Util.SECURITY_NotRegistered + " - " + Package.PackageType);
                _isValid = false;
            }
            else
            {
                if (type.SecurityAccessLevel > Package.InstallerInfo.SecurityAccessLevel)
                {
                    Log.Logs.Clear();
                    Log.AddFailure(Util.SECURITY_Installer);
                    _isValid = false;
                }
            }
        }

        private void ReadComponents(XPathNavigator manifestNav)
        {
            foreach (XPathNavigator componentNav in manifestNav.CreateNavigator().Select("components/component"))
            {
                int order = _componentInstallers.Count;
                string type = componentNav.GetAttribute("type", "");
                if (InstallMode == InstallMode.Install)
                {
                    string installOrder = componentNav.GetAttribute("installOrder", "");
                    if (!string.IsNullOrEmpty(installOrder))
                    {
                        order = int.Parse(installOrder);
                    }
                }
                else
                {
                    string unInstallOrder = componentNav.GetAttribute("unInstallOrder", "");
                    if (!string.IsNullOrEmpty(unInstallOrder))
                    {
                        order = int.Parse(unInstallOrder);
                    }
                }
                if (Package.InstallerInfo != null)
                {
                    Log.AddInfo(Util.DNN_ReadingComponent + " - " + type);
                }
                ComponentInstallerBase installer = InstallerFactory.GetInstaller(componentNav, Package);
                if (installer == null)
                {
                    Log.AddFailure(Util.EXCEPTION_InstallerCreate);
                }
                else
                {
                    _componentInstallers.Add(order, installer);
                    Package.InstallerInfo.AllowableFiles += ", " + installer.AllowableFiles;
                }
            }
        }

        private string ReadTextFromFile(string source)
        {
            string strText = Null.NullString;
            if (Package.InstallerInfo.InstallMode != InstallMode.ManifestOnly)
            {
                strText = FileSystemUtils.ReadFile(Package.InstallerInfo.TempInstallFolder + "\\" + source);
            }
            return strText;
        }

        private void ValidateVersion(string strVersion)
        {
            if (string.IsNullOrEmpty(strVersion))
            {
                _isValid = false;
                return;
            }
            Package.Version = new Version(strVersion);
            if (_installedPackage != null)
            {
                Package.InstalledVersion = _installedPackage.Version;
                if (Package.InstalledVersion > Package.Version)
                {
                    Log.AddFailure(Util.INSTALL_Version + " - " + Package.InstalledVersion.ToString(3));
                    _isValid = false;
                }
                else if (Package.InstalledVersion == Package.Version)
                {
                    Package.InstallerInfo.Installed = true;
                    Package.InstallerInfo.PortalID = _installedPackage.PortalID;
                }
            }
        }

        public override void Commit()
        {
            for (int index = 0; index <= _componentInstallers.Count - 1; index++)
            {
                ComponentInstallerBase compInstaller = _componentInstallers.Values[index];
                if (compInstaller.Version >= Package.InstalledVersion && compInstaller.Completed)
                {
                    compInstaller.Commit();
                }
            }
            if (Log.Valid)
            {
                Log.AddInfo(Util.INSTALL_Committed);
            }
            else
            {
                Log.AddFailure(Util.INSTALL_Aborted);
            }
            Package.InstallerInfo.PackageID = Package.PackageID;
        }

        public override void Install()
        {
            bool isCompleted = true;
            try
            {
                if (_installedPackage != null)
                {
                    Package.PackageID = _installedPackage.PackageID;
                }
                PackageController.SavePackage(Package);
                for (int index = 0; index <= _componentInstallers.Count - 1; index++)
                {
                    ComponentInstallerBase compInstaller = _componentInstallers.Values[index];
                    if ((_installedPackage == null) || (compInstaller.Version > Package.InstalledVersion) || (Package.InstallerInfo.RepairInstall))
                    {
                        Log.AddInfo(Util.INSTALL_Start + " - " + compInstaller.Type);
                        compInstaller.Install();
                        if (compInstaller.Completed)
                        {
                            Log.AddInfo(Util.COMPONENT_Installed + " - " + compInstaller.Type);
                        }
                        else
                        {
                            Log.AddFailure(Util.INSTALL_Failed + " - " + compInstaller.Type);
                            isCompleted = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Log.AddFailure(Util.INSTALL_Aborted + " - " + Package.Name);
            }
            if (isCompleted)
            {
                Commit();
            }
            else
            {
                Rollback();
            }
        }

        public override void ReadManifest(XPathNavigator manifestNav)
        {
            Package.Name = Util.ReadAttribute(manifestNav, "name", Log, Util.EXCEPTION_NameMissing);
            Package.PackageType = Util.ReadAttribute(manifestNav, "type", Log, Util.EXCEPTION_TypeMissing);
            if (Package.PackageType == "Skin" || Package.PackageType == "Container")
            {
                Package.PortalID = Package.InstallerInfo.PortalID;
            }
            CheckSecurity();
            if (!IsValid)
            {
                return;
            }
            _installedPackage = PackageController.GetPackageByName(Package.PortalID, Package.Name);
            Package.IsSystemPackage = bool.Parse(Util.ReadAttribute(manifestNav, "isSystem", false, Log, "", bool.FalseString));
            string strVersion = Util.ReadAttribute(manifestNav, "version", Log, Util.EXCEPTION_VersionMissing);
            ValidateVersion(strVersion);
            if (!IsValid)
            {
                return;
            }
            Log.AddInfo(Util.DNN_ReadingPackage + " - " + Package.PackageType + " - " + Package.Name);
            Package.FriendlyName = Util.ReadElement(manifestNav, "friendlyName", Package.Name);
            Package.Description = Util.ReadElement(manifestNav, "description");

            XPathNavigator foldernameNav = null;
            switch (Package.PackageType)
            {
                case "Module":
                case "Auth_System":
                    foldernameNav = manifestNav.SelectSingleNode("components/component/files");
                    if (foldernameNav != null) Package.FolderName = Util.ReadElement(foldernameNav, "basePath").Replace('\\', '/');
                    break;
                case "Container":
                    foldernameNav = manifestNav.SelectSingleNode("components/component/containerFiles");
                    if (foldernameNav != null) Package.FolderName = Globals.glbContainersPath +  Util.ReadElement(foldernameNav, "containerName").Replace('\\', '/');
                    break;
                case "Skin":
                    foldernameNav = manifestNav.SelectSingleNode("components/component/skinFiles");
                    if (foldernameNav != null) Package.FolderName = Globals.glbSkinsPath + Util.ReadElement(foldernameNav, "skinName").Replace('\\', '/');
                    break;
                default:
                    break;
            }

            XPathNavigator iconFileNav= manifestNav.SelectSingleNode("iconFile");
            if (Package.FolderName != string.Empty && iconFileNav != null)
            {
                
                if ((iconFileNav.Value != string.Empty) && (Package.PackageType == "Module" || Package.PackageType == "Auth_System" || Package.PackageType == "Container" || Package.PackageType == "Skin"))
                {
                    Package.IconFile = Package.FolderName + "/" + iconFileNav.Value;
                    Package.IconFile = (!Package.IconFile.StartsWith("~/")) ? "~/" + Package.IconFile : Package.IconFile;
                }
            }

            XPathNavigator authorNav = manifestNav.SelectSingleNode("owner");
            if (authorNav != null)
            {
                Package.Owner = Util.ReadElement(authorNav, "name");
                Package.Organization = Util.ReadElement(authorNav, "organization");
                Package.Url = Util.ReadElement(authorNav, "url");
                Package.Email = Util.ReadElement(authorNav, "email");
            }
            XPathNavigator licenseNav = manifestNav.SelectSingleNode("license");
            if (licenseNav != null)
            {
                string licenseSrc = Util.ReadAttribute(licenseNav, "src");
                if (string.IsNullOrEmpty(licenseSrc))
                {
                    Package.License = licenseNav.Value;
                }
                else
                {
                    Package.License = ReadTextFromFile(licenseSrc);
                }
            }
            if (string.IsNullOrEmpty(Package.License))
            {
                Package.License = Util.PACKAGE_NoLicense;
            }
            XPathNavigator relNotesNav = manifestNav.SelectSingleNode("releaseNotes");
            if (relNotesNav != null)
            {
                string relNotesSrc = Util.ReadAttribute(relNotesNav, "src");
                if (string.IsNullOrEmpty(relNotesSrc))
                {
                    Package.ReleaseNotes = relNotesNav.Value;
                }
                else
                {
                    Package.ReleaseNotes = ReadTextFromFile(relNotesSrc);
                }
            }
            if (string.IsNullOrEmpty(Package.ReleaseNotes))
            {
				Package.ReleaseNotes = Util.PACKAGE_NoReleaseNotes;
            }
            IDependency dependency = null;
            foreach (XPathNavigator dependencyNav in manifestNav.CreateNavigator().Select("dependencies/dependency"))
            {
                dependency = DependencyFactory.GetDependency(dependencyNav);
                if (!dependency.IsValid)
                {
                    Log.AddFailure(dependency.ErrorMessage);
                    return;
                }
            }
            ReadComponents(manifestNav);
        }

        public override void Rollback()
        {
            for (int index = 0; index <= _componentInstallers.Count - 1; index++)
            {
                ComponentInstallerBase compInstaller = _componentInstallers.Values[index];
                if (compInstaller.Version > Package.InstalledVersion && compInstaller.Completed)
                {
                    Log.AddInfo(Util.COMPONENT_RollingBack + " - " + compInstaller.Type);
                    compInstaller.Rollback();
                    Log.AddInfo(Util.COMPONENT_RolledBack + " - " + compInstaller.Type);
                }
            }
            if (_installedPackage == null)
            {
                PackageController.DeletePackage(Package);
            }
            else
            {
                PackageController.SavePackage(_installedPackage);
            }
        }

        public override void UnInstall()
        {
            for (int index = 0; index <= _componentInstallers.Count - 1; index++)
            {
                ComponentInstallerBase compInstaller = _componentInstallers.Values[index];
                var fileInstaller = compInstaller as FileInstaller;
                if (fileInstaller != null)
                {
                    fileInstaller.DeleteFiles = DeleteFiles;
                }
                Log.ResetFlags();
                Log.AddInfo(Util.UNINSTALL_StartComp + " - " + compInstaller.Type);
                compInstaller.UnInstall();
                Log.AddInfo(Util.COMPONENT_UnInstalled + " - " + compInstaller.Type);
                if (Log.Valid)
                {
                    Log.AddInfo(Util.UNINSTALL_SuccessComp + " - " + compInstaller.Type);
                }
                else
                {
                    Log.AddWarning(Util.UNINSTALL_WarningsComp + " - " + compInstaller.Type);
                }
            }
            PackageController.DeletePackage(Package);
        }
    }
}
