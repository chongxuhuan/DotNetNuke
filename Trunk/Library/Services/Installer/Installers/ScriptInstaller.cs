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
using DotNetNuke.Data;
using DotNetNuke.Framework.Providers;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ScriptInstaller installs Script Components to a DotNetNuke site
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	08/07/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class ScriptInstaller : FileInstaller
    {
        private readonly SortedList<Version, InstallFile> _InstallScripts = new SortedList<Version, InstallFile>();
        private readonly SortedList<Version, InstallFile> _UnInstallScripts = new SortedList<Version, InstallFile>();
        private InstallFile _InstallScript;
        private InstallFile _UpgradeScript;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the base Install Script (if present)
        /// </summary>
        /// <value>An InstallFile</value>
        /// <history>
        /// 	[cnurse]	05/20/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected InstallFile InstallScript
        {
            get
            {
                return _InstallScript;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the collection of Install Scripts
        /// </summary>
        /// <value>A List(Of InstallFile)</value>
        /// <history>
        /// 	[cnurse]	08/07/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected SortedList<Version, InstallFile> InstallScripts
        {
            get
            {
                return _InstallScripts;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the collection of UnInstall Scripts
        /// </summary>
        /// <value>A List(Of InstallFile)</value>
        /// <history>
        /// 	[cnurse]	08/07/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected SortedList<Version, InstallFile> UnInstallScripts
        {
            get
            {
                return _UnInstallScripts;
            }
        }

        protected override string CollectionNodeName
        {
            get
            {
                return "scripts";
            }
        }

        protected override string ItemNodeName
        {
            get
            {
                return "script";
            }
        }

        protected ProviderConfiguration ProviderConfiguration
        {
            get
            {
                return ProviderConfiguration.GetProviderConfiguration("data");
            }
        }

        protected InstallFile UpgradeScript
        {
            get
            {
                return _UpgradeScript;
            }
        }

        public override string AllowableFiles
        {
            get
            {
                return "*dataprovider, sql";
            }
        }

        private bool ExecuteSql(InstallFile scriptFile, bool useTransaction)
        {
            bool bSuccess = true;
            Log.AddInfo(string.Format(Util.SQL_BeginFile, scriptFile.Name));
            string strScript = FileSystemUtils.ReadFile(PhysicalBasePath + scriptFile.FullName);
            if (strScript.StartsWith("?"))
            {
                strScript = strScript.Substring(1);
            }
            string strSQLExceptions = Null.NullString;
            strSQLExceptions = DataProvider.Instance().ExecuteScript(strScript);
            if (!String.IsNullOrEmpty(strSQLExceptions))
            {
                if (Package.InstallerInfo.IsLegacyMode)
                {
                    Log.AddWarning(string.Format(Util.SQL_Exceptions, Environment.NewLine, strSQLExceptions));
                }
                else
                {
                    Log.AddFailure(string.Format(Util.SQL_Exceptions, Environment.NewLine, strSQLExceptions));
                    bSuccess = false;
                }
            }
            Log.AddInfo(string.Format(Util.SQL_EndFile, scriptFile.Name));
            return bSuccess;
        }

        private bool InstallScriptFile(InstallFile scriptFile)
        {
            bool bSuccess = InstallFile(scriptFile);

            string fileExtension = Path.GetExtension(scriptFile.Name.ToLower()).Substring(1);
            if (bSuccess && ProviderConfiguration.DefaultProvider.ToLower() == fileExtension)
            {
                Log.AddInfo(Util.SQL_Executing + scriptFile.Name);
                bSuccess = ExecuteSql(scriptFile, false);
            }
            return bSuccess;
        }

        protected override bool IsCorrectType(InstallFileType type)
        {
            return (type == InstallFileType.Script);
        }

        protected override void ProcessFile(InstallFile file, XPathNavigator nav)
        {
            string type = nav.GetAttribute("type", "");
            if (file != null && IsCorrectType(file.Type))
            {
                if (file.Name.ToLower().StartsWith("install."))
                {
                    _InstallScript = file;
                }
                else if (file.Name.ToLower().StartsWith("upgrade."))
                {
                    _UpgradeScript = file;
                }
                else if (type.ToLower() == "install")
                {
                    InstallScripts[file.Version] = file;
                }
                else
                {
                    UnInstallScripts[file.Version] = file;
                }
            }
            base.ProcessFile(file, nav);
        }

        protected override void UnInstallFile(InstallFile scriptFile)
        {
            if (UnInstallScripts.ContainsValue(scriptFile) && ProviderConfiguration.DefaultProvider.ToLower() == Path.GetExtension(scriptFile.Name.ToLower()).Substring(1))
            {
                if (scriptFile.Name.ToLower().StartsWith("uninstall."))
                {
                    Log.AddInfo(Util.SQL_Executing + scriptFile.Name);
                    ExecuteSql(scriptFile, false);
                }
            }
            base.UnInstallFile(scriptFile);
        }

        public override void Commit()
        {
            base.Commit();
        }

        public override void Install()
        {
            Log.AddInfo(Util.SQL_Begin);
            try
            {
                bool bSuccess = true;
                Version installedVersion = Package.InstalledVersion;
                if (installedVersion == new Version(0, 0, 0))
                {
                    if (InstallScript != null)
                    {
                        bSuccess = InstallScriptFile(InstallScript);
                        installedVersion = InstallScript.Version;
                    }
                }
                if (bSuccess)
                {
                    foreach (InstallFile file in InstallScripts.Values)
                    {
                        if (file.Version > installedVersion)
                        {
                            bSuccess = InstallScriptFile(file);
                            if (!bSuccess)
                            {
                                break;
                            }
                        }
                    }
                }
                if (UpgradeScript != null)
                {
                    bSuccess = InstallScriptFile(UpgradeScript);
                    installedVersion = UpgradeScript.Version;
                }
                if (bSuccess)
                {
                    foreach (InstallFile file in UnInstallScripts.Values)
                    {
                        bSuccess = InstallFile(file);
                        if (!bSuccess)
                        {
                            break;
                        }
                    }
                }
                Completed = bSuccess;
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
            Log.AddInfo(Util.SQL_End);
        }

        public override void Rollback()
        {
            base.Rollback();
        }

        public override void UnInstall()
        {
            Log.AddInfo(Util.SQL_BeginUnInstall);
            base.UnInstall();
            Log.AddInfo(Util.SQL_EndUnInstall);
        }
    }
}
