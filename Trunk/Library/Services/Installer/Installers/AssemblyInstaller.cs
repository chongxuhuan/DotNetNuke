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

using DotNetNuke.Data;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The AssemblyInstaller installs Assembly Components to a DotNetNuke site
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	07/24/2007  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class AssemblyInstaller : FileInstaller
    {
        protected override string CollectionNodeName
        {
            get
            {
                return "assemblies";
            }
        }

        protected override string DefaultPath
        {
            get
            {
                return "bin\\";
            }
        }

        protected override string ItemNodeName
        {
            get
            {
                return "assembly";
            }
        }

        protected override string PhysicalBasePath
        {
            get
            {
                return PhysicalSitePath + "\\";
            }
        }

        public override string AllowableFiles
        {
            get
            {
                return "dll";
            }
        }

        protected override void DeleteFile(InstallFile file)
        {
            if (DataProvider.Instance().UnRegisterAssembly(Package.PackageID, file.Name))
            {
                Log.AddInfo(Util.ASSEMBLY_UnRegistered + " - " + file.FullName);
                base.DeleteFile(file);
            }
            else
            {
                Log.AddInfo(Util.ASSEMBLY_InUse + " - " + file.FullName);
            }
        }

        protected override bool IsCorrectType(InstallFileType type)
        {
            return (type == InstallFileType.Assembly);
        }

        protected override bool InstallFile(InstallFile file)
        {
            bool bSuccess = true;
            if (file.Action == "UnRegister")
            {
                DeleteFile(file);
            }
            else
            {
                int returnCode = DataProvider.Instance().RegisterAssembly(Package.PackageID, file.Name, file.Version.ToString(3));
                switch (returnCode)
                {
                    case 0:
                        Log.AddInfo(Util.ASSEMBLY_Added + " - " + file.FullName);
                        break;
                    case 1:
                        Log.AddInfo(Util.ASSEMBLY_Updated + " - " + file.FullName);
                        break;
                    case 2:
                    case 3:
                        Log.AddInfo(Util.ASSEMBLY_Registered + " - " + file.FullName);
                        break;
                }
                if (returnCode < 2 || (returnCode == 2 && file.InstallerInfo.RepairInstall))
                {
                    bSuccess = base.InstallFile(file);
                }
            }
            return bSuccess;
        }
    }
}
