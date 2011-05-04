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
using System.Xml;
using System.Xml.XPath;

using DotNetNuke.Common.Utilities;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ConfigInstaller installs Config changes
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	08/03/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class ConfigInstaller : ComponentInstallerBase
    {
        private string _FileName = Null.NullString;
        private string _InstallConfig = Null.NullString;
        private XmlDocument _TargetConfig;
        private InstallFile _TargetFile;
        private string _UnInstallConfig = Null.NullString;
        private string _UninstallFileName = Null.NullString;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Install config changes
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	08/03/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string InstallConfig
        {
            get
            {
                return _InstallConfig;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Target Config XmlDocument
        /// </summary>
        /// <value>An XmlDocument</value>
        /// <history>
        /// 	[cnurse]	08/04/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public XmlDocument TargetConfig
        {
            get
            {
                return _TargetConfig;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Target Config file to change
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	08/04/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public InstallFile TargetFile
        {
            get
            {
                return _TargetFile;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the UnInstall config changes
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	08/03/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string UnInstallConfig
        {
            get
            {
                return _UnInstallConfig;
            }
        }

        public override void Commit()
        {
            try
            {
                Config.Save(TargetConfig, TargetFile.FullName);
                Log.AddInfo(Util.CONFIG_Committed + " - " + TargetFile.Name);
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }

        public override void Install()
        {
            try
            {
                if (string.IsNullOrEmpty(_FileName))
                {
                    Util.BackupFile(TargetFile, PhysicalSitePath, Log);
                    _TargetConfig = new XmlDocument();
                    TargetConfig.Load(Path.Combine(PhysicalSitePath, TargetFile.FullName));
                    var merge = new XmlMerge(new StringReader(InstallConfig), Package.Version.ToString(), Package.Name);
                    merge.UpdateConfig(TargetConfig);
                    Completed = true;
                    Log.AddInfo(Util.CONFIG_Updated + " - " + TargetFile.Name);
                }
                else
                {
                    string strConfigFile = Path.Combine(Package.InstallerInfo.TempInstallFolder, _FileName);
                    if (File.Exists(strConfigFile))
                    {
                        StreamReader stream = File.OpenText(strConfigFile);
                        var merge = new XmlMerge(stream, Package.Version.ToString(3), Package.Name + " Install");
                        merge.UpdateConfigs();
                        stream.Close();
                        Completed = true;
                        Log.AddInfo(Util.CONFIG_Updated);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
        }

        public override void ReadManifest(XPathNavigator manifestNav)
        {
            _FileName = Util.ReadAttribute(manifestNav, "fileName");
            _UninstallFileName = Util.ReadAttribute(manifestNav, "unInstallFileName");
            if (string.IsNullOrEmpty(_FileName))
            {
                XPathNavigator nav = manifestNav.SelectSingleNode("config");
                XPathNavigator nodeNav = nav.SelectSingleNode("configFile");
                string targetFileName = nodeNav.Value;
                if (!string.IsNullOrEmpty(targetFileName))
                {
                    _TargetFile = new InstallFile(targetFileName, "", Package.InstallerInfo);
                }
                nodeNav = nav.SelectSingleNode("install");
                _InstallConfig = nodeNav.InnerXml;
                nodeNav = nav.SelectSingleNode("uninstall");
                _UnInstallConfig = nodeNav.InnerXml;
            }
        }

        public override void Rollback()
        {
            Log.AddInfo(Util.CONFIG_RolledBack + " - " + TargetFile.Name);
        }

        public override void UnInstall()
        {
            if (string.IsNullOrEmpty(_UninstallFileName))
            {
                _TargetConfig = new XmlDocument();
                TargetConfig.Load(Path.Combine(PhysicalSitePath, TargetFile.FullName));
                var merge = new XmlMerge(new StringReader(UnInstallConfig), Package.Version.ToString(), Package.Name);
                merge.UpdateConfig(TargetConfig, TargetFile.FullName);
            }
            else
            {
                string strConfigFile = Path.Combine(Package.InstallerInfo.TempInstallFolder, _UninstallFileName);
                if (File.Exists(strConfigFile))
                {
                    StreamReader stream = File.OpenText(strConfigFile);
                    var merge = new XmlMerge(stream, Package.Version.ToString(3), Package.Name + " UnInstall");
                    merge.UpdateConfigs();
                    stream.Close();
                }
            }
        }
    }
}
