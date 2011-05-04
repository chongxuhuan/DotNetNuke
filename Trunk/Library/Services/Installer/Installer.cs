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
using System.Text;
using System.Xml;
using System.Xml.XPath;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Installer.Installers;
using DotNetNuke.Services.Installer.Log;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Services.Installer.Writers;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Services.Installer
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Installer class provides a single entrypoint for Package Installation
    /// </summary>
    /// <history>
    /// 	[cnurse]	07/24/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class Installer
    {
        private readonly InstallerInfo _InstallerInfo;
        private readonly SortedList<int, PackageInstaller> _Packages = new SortedList<int, PackageInstaller>();

        public Installer(string tempFolder, string manifest, string physicalSitePath, bool loadManifest)
        {
            _InstallerInfo = new InstallerInfo(tempFolder, manifest, physicalSitePath);
            _InstallerInfo.IgnoreWhiteList = false;
            if (loadManifest)
            {
                ReadManifest(true);
            }
        }

        public Installer(Stream inputStream, string physicalSitePath, bool loadManifest) : this(inputStream, physicalSitePath, loadManifest, true)
        {
        }

        public Installer(Stream inputStream, string physicalSitePath, bool loadManifest, bool deleteTemp)
        {
            _InstallerInfo = new InstallerInfo(inputStream, physicalSitePath);
            _InstallerInfo.IgnoreWhiteList = true;
            if (loadManifest)
            {
                ReadManifest(deleteTemp);
            }
        }

        public Installer(PackageInfo package, string physicalSitePath)
        {
            _InstallerInfo = new InstallerInfo(package, physicalSitePath);
            Packages.Add(Packages.Count, new PackageInstaller(package));
        }

        public Installer(string manifest, string physicalSitePath, bool loadManifest)
        {
            _InstallerInfo = new InstallerInfo(physicalSitePath, InstallMode.ManifestOnly);
            if (loadManifest)
            {
                ReadManifest(new FileStream(manifest, FileMode.Open, FileAccess.Read));
            }
        }

        public InstallerInfo InstallerInfo
        {
            get
            {
                return _InstallerInfo;
            }
        }

        public bool IsValid
        {
            get
            {
                return InstallerInfo.IsValid;
            }
        }


        public SortedList<int, PackageInstaller> Packages
        {
            get
            {
                return _Packages;
            }
        }

        public string TempInstallFolder
        {
            get
            {
                return InstallerInfo.TempInstallFolder;
            }
        }

        private void InstallPackages()
        {
            for (int index = 0; index <= Packages.Count - 1; index++)
            {
                PackageInstaller installer = Packages.Values[index];
                if (installer.Package.IsValid)
                {
                    InstallerInfo.Log.AddInfo(Util.INSTALL_Start + " - " + installer.Package.Name);
                    installer.Install();
                    if (InstallerInfo.Log.Valid)
                    {
                        InstallerInfo.Log.AddInfo(Util.INSTALL_Success + " - " + installer.Package.Name);
                    }
                    else
                    {
                        InstallerInfo.Log.AddInfo(Util.INSTALL_Failed + " - " + installer.Package.Name);
                    }
                }
                else
                {
                    InstallerInfo.Log.AddFailure(Util.INSTALL_Aborted + " - " + installer.Package.Name);
                }
            }
        }

        private void LogInstallEvent(string package, string eventType)
        {
            try
            {
                var objEventLogInfo = new LogInfo();
                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString();
                objEventLogInfo.LogProperties.Add(new LogDetailInfo(eventType + " " + package + ":", InstallerInfo.ManifestFile.Name.Replace(".dnn", "")));
                foreach (LogEntry objLogEntry in InstallerInfo.Log.Logs)
                {
                    objEventLogInfo.LogProperties.Add(new LogDetailInfo("Info:", objLogEntry.Description));
                }
                var objEventLog = new EventLogController();
                objEventLog.AddLog(objEventLogInfo);
            }
            catch (Exception exc)
            {
                Instrumentation.DnnLog.Error(exc);

            }
        }

        private void ProcessPackages(XPathNavigator rootNav)
        {
            foreach (XPathNavigator nav in rootNav.Select("packages/package"))
            {
                int order = Packages.Count;
                string name = Util.ReadAttribute(nav, "name");
                string installOrder = Util.ReadAttribute(nav, "installOrder");
                if (!string.IsNullOrEmpty(installOrder))
                {
                    order = int.Parse(installOrder);
                }
                Packages.Add(order, new PackageInstaller(nav.OuterXml, InstallerInfo));
            }
        }

        private void ReadManifest(Stream stream)
        {
            var doc = new XPathDocument(stream);
            XPathNavigator rootNav = doc.CreateNavigator();
            rootNav.MoveToFirstChild();
            string packageType = Null.NullString;

            if (rootNav.Name == "dotnetnuke")
            {
                packageType = Util.ReadAttribute(rootNav, "type");
            }
            else if (rootNav.Name.ToLower() == "languagepack")
            {
                packageType = "LanguagePack";
            }
            else
            {
                InstallerInfo.Log.AddFailure(Util.PACKAGE_UnRecognizable);
            }
            switch (packageType.ToLower())
            {
                case "package":
                    InstallerInfo.IsLegacyMode = false;
                    ProcessPackages(rootNav);
                    break;
                case "module":
                case "languagepack":
                case "skinobject":
                    InstallerInfo.IsLegacyMode = true;
                    ProcessPackages(ConvertLegacyNavigator(rootNav, InstallerInfo));
                    break;
            }
        }

        private void UnInstallPackages(bool deleteFiles)
        {
            for (int index = 0; index <= Packages.Count - 1; index++)
            {
                PackageInstaller installer = Packages.Values[index];
                InstallerInfo.Log.AddInfo(Util.UNINSTALL_Start + " - " + installer.Package.Name);
                installer.DeleteFiles = deleteFiles;
                installer.UnInstall();
                if (InstallerInfo.Log.HasWarnings)
                {
                    InstallerInfo.Log.AddWarning(Util.UNINSTALL_Warnings + " - " + installer.Package.Name);
                }
                else
                {
                    InstallerInfo.Log.AddInfo(Util.UNINSTALL_Success + " - " + installer.Package.Name);
                }
            }
        }

        public static XPathNavigator ConvertLegacyNavigator(XPathNavigator rootNav, InstallerInfo info)
        {
            XPathNavigator nav = null;

            var packageType = Null.NullString;
            if (rootNav.Name == "dotnetnuke")
            {
                packageType = Util.ReadAttribute(rootNav, "type");
            }
            else if (rootNav.Name.ToLower() == "languagepack")
            {
                packageType = "LanguagePack";
            }

            XPathDocument legacyDoc;
            string legacyManifest;
            switch (packageType.ToLower())
            {
                case "module":
                    var sb = new StringBuilder();
                    var writer = XmlWriter.Create(sb, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment));
                    PackageWriterBase.WriteManifestStartElement(writer);
                    foreach (XPathNavigator folderNav in rootNav.Select("folders/folder"))
                    {
                        var modulewriter = new ModulePackageWriter(folderNav, info);
                        modulewriter.WriteManifest(writer, true);
                    }

                    PackageWriterBase.WriteManifestEndElement(writer);
                    writer.Close();
                    legacyDoc = new XPathDocument(new StringReader(sb.ToString()));
                    nav = legacyDoc.CreateNavigator().SelectSingleNode("dotnetnuke");
                    break;
                case "languagepack":
                    var languageWriter = new LanguagePackWriter(rootNav, info);
                    info.LegacyError = languageWriter.LegacyError;
                    if (string.IsNullOrEmpty(info.LegacyError))
                    {
                        legacyManifest = languageWriter.WriteManifest(false);
                        legacyDoc = new XPathDocument(new StringReader(legacyManifest));
                        nav = legacyDoc.CreateNavigator().SelectSingleNode("dotnetnuke");
                    }
                    break;
                case "skinobject":
                    var skinControlwriter = new SkinControlPackageWriter(rootNav, info);
                    legacyManifest = skinControlwriter.WriteManifest(false);
                    legacyDoc = new XPathDocument(new StringReader(legacyManifest));
                    nav = legacyDoc.CreateNavigator().SelectSingleNode("dotnetnuke");
                    break;
            }

            return nav;
        }

        public void DeleteTempFolder()
        {
            if (!string.IsNullOrEmpty(TempInstallFolder))
            {
                Directory.Delete(TempInstallFolder, true);
            }
        }

        public bool Install()
        {
            InstallerInfo.Log.StartJob(Util.INSTALL_Start);
            bool bStatus = true;
            try
            {
                InstallPackages();
            }
            catch (Exception ex)
            {
            
                InstallerInfo.Log.AddFailure(ex);
                bStatus = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(TempInstallFolder))
                {
                    Globals.DeleteFolderRecursive(TempInstallFolder);
                }
                InstallerInfo.Log.AddInfo(Util.FOLDER_DeletedBackup);
            }
            if (InstallerInfo.Log.Valid)
            {
                InstallerInfo.Log.EndJob(Util.INSTALL_Success);
            }
            else
            {
                InstallerInfo.Log.EndJob(Util.INSTALL_Failed);
                bStatus = false;
            }
            LogInstallEvent("Package", "Install");
            DataCache.ClearHostCache(true);
            return bStatus;
        }

        public void ReadManifest(bool deleteTemp)
        {
            InstallerInfo.Log.StartJob(Util.DNN_Reading);
            if (InstallerInfo.ManifestFile != null)
            {
                ReadManifest(new FileStream(InstallerInfo.ManifestFile.TempFileName, FileMode.Open, FileAccess.Read));
            }
            if (InstallerInfo.Log.Valid)
            {
                InstallerInfo.Log.EndJob(Util.DNN_Success);
            }
            else if (deleteTemp)
            {
                DeleteTempFolder();
            }
        }

        public bool UnInstall(bool deleteFiles)
        {
            InstallerInfo.Log.StartJob(Util.UNINSTALL_Start);
            try
            {
                UnInstallPackages(deleteFiles);
            }
            catch (Exception ex)
            {

                InstallerInfo.Log.AddFailure(ex);
                return false;
            }
            if (InstallerInfo.Log.HasWarnings)
            {
                InstallerInfo.Log.EndJob(Util.UNINSTALL_Warnings);
            }
            else
            {
                InstallerInfo.Log.EndJob(Util.UNINSTALL_Success);
            }
            LogInstallEvent("Package", "UnInstall");
            return true;
        }
    }
}
