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

using System.Collections.Generic;
using System.Xml;

using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.Services.Installer.Writers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The LanguageComponentWriter class handles creating the manifest for Language
    /// Component(s)
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	02/08/2008	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class LanguageComponentWriter : FileComponentWriter
    {
        private readonly int _DependentPackageID;
        private readonly Locale _Language;
        private readonly LanguagePackType _PackageType;

        public LanguageComponentWriter(Locale language, string basePath, Dictionary<string, InstallFile> files, PackageInfo package) : base(basePath, files, package)
        {
            _Language = language;
            _PackageType = LanguagePackType.Core;
        }

        public LanguageComponentWriter(LanguagePackInfo languagePack, string basePath, Dictionary<string, InstallFile> files, PackageInfo package) : base(basePath, files, package)
        {
            _Language = LocaleController.Instance.GetLocale(languagePack.LanguageID);
            _PackageType = languagePack.PackageType;
            _DependentPackageID = languagePack.DependentPackageID;
        }

        protected override string CollectionNodeName
        {
            get
            {
                return "languageFiles";
            }
        }

        protected override string ComponentType
        {
            get
            {
                if (_PackageType == LanguagePackType.Core)
                {
                    return "CoreLanguage";
                }
                else
                {
                    return "ExtensionLanguage";
                }
            }
        }

        protected override string ItemNodeName
        {
            get
            {
                return "languageFile";
            }
        }

        protected override void WriteCustomManifest(XmlWriter writer)
        {
            writer.WriteElementString("code", _Language.Code);
            if (_PackageType == LanguagePackType.Core)
            {
                writer.WriteElementString("displayName", _Language.Text);
                writer.WriteElementString("fallback", _Language.Fallback);
            }
            else
            {
                PackageInfo package = PackageController.GetPackage(_DependentPackageID);
                writer.WriteElementString("package", package.Name);
            }
        }
    }
}
