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
using System.Xml.XPath;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Installer.Log;
using DotNetNuke.Services.Installer.Packages;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    public abstract class ComponentInstallerBase
    {
        private bool _Completed = Null.NullBoolean;

        public virtual string AllowableFiles
        {
            get
            {
                return Null.NullString;
            }
        }

        public bool Completed
        {
            get
            {
                return _Completed;
            }
            set
            {
                _Completed = value;
            }
        }

        public InstallMode InstallMode
        {
            get
            {
                return Package.InstallMode;
            }
        }

        public Logger Log
        {
            get
            {
                return Package.Log;
            }
        }

        public PackageInfo Package { get; set; }

        public Dictionary<string, InstallFile> PackageFiles
        {
            get
            {
                return Package.Files;
            }
        }

        public string PhysicalSitePath
        {
            get
            {
                return Package.InstallerInfo.PhysicalSitePath;
            }
        }

        public virtual bool SupportsManifestOnlyInstall
        {
            get
            {
                return true;
            }
        }

        public string Type { get; set; }

        public Version Version { get; set; }

        public abstract void Commit();

        public abstract void Install();

        public abstract void ReadManifest(XPathNavigator manifestNav);

        public abstract void Rollback();

        public abstract void UnInstall();
    }
}
