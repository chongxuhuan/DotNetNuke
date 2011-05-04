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
using System.Xml.XPath;

using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The LanguageInstaller installs Language Packs to a DotNetNuke site
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	01/29/2008  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class LanguageInstaller : FileInstaller
    {
        private readonly LanguagePackType LanguagePackType;
        private LanguagePackInfo InstalledLanguagePack;
        private Locale Language;
        private LanguagePackInfo LanguagePack;
        private Locale TempLanguage;

        public LanguageInstaller(LanguagePackType type)
        {
            LanguagePackType = type;
        }

        protected override string CollectionNodeName
        {
            get
            {
                return "languageFiles";
            }
        }

        protected override string ItemNodeName
        {
            get
            {
                return "languageFile";
            }
        }

        public override string AllowableFiles
        {
            get
            {
                return "resx, xml";
            }
        }

        private void DeleteLanguage()
        {
            try
            {
                LanguagePackInfo tempLanguagePack = LanguagePackController.GetLanguagePackByPackage(Package.PackageID);
                Locale language = LocaleController.Instance.GetLocale(tempLanguagePack.LanguageID);
                if (tempLanguagePack != null)
                {
                    LanguagePackController.DeleteLanguagePack(tempLanguagePack);
                }
                if (language != null && tempLanguagePack.PackageType == LanguagePackType.Core)
                {
                    Localization.Localization.DeleteLanguage(language);
                }
                Log.AddInfo(string.Format(Util.LANGUAGE_UnRegistered, language.Text));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }

        protected override void ProcessFile(InstallFile file, XPathNavigator nav)
        {
            base.ProcessFile(file, nav);
        }

        protected override void ReadCustomManifest(XPathNavigator nav)
        {
            Language = new Locale();
            LanguagePack = new LanguagePackInfo();
            Language.Code = Util.ReadElement(nav, "code");
            Language.Text = Util.ReadElement(nav, "displayName");
            Language.Fallback = Util.ReadElement(nav, "fallback");
            if (LanguagePackType == LanguagePackType.Core)
            {
                LanguagePack.DependentPackageID = -2;
            }
            else
            {
                string packageName = Util.ReadElement(nav, "package");
                PackageInfo package = PackageController.GetPackageByName(packageName);
                LanguagePack.DependentPackageID = package.PackageID;
            }
            base.ReadCustomManifest(nav);
        }

        public override void Commit()
        {
        }

        public override void Install()
        {
            try
            {
                InstalledLanguagePack = LanguagePackController.GetLanguagePackByPackage(Package.PackageID);
                if (InstalledLanguagePack != null)
                {
                    LanguagePack.LanguagePackID = InstalledLanguagePack.LanguagePackID;
                }
                TempLanguage = LocaleController.Instance.GetLocale(Language.Code);
                if (TempLanguage != null)
                {
                    Language.LanguageId = TempLanguage.LanguageId;
                }
                if (LanguagePack.PackageType == LanguagePackType.Core)
                {
                    Localization.Localization.SaveLanguage(Language);
                }
                PortalSettings _settings = PortalController.GetCurrentPortalSettings();
                if (_settings != null)
                {
                    Locale enabledLanguage = null;
                    if (!LocaleController.Instance.GetLocales(_settings.PortalId).TryGetValue(Language.Code, out enabledLanguage))
                    {
                        //Add language to portal
                        Localization.Localization.AddLanguageToPortal(_settings.PortalId, Language.LanguageId, true);
                    }
                }

                LanguagePack.PackageID = Package.PackageID;
                LanguagePack.LanguageID = Language.LanguageId;
                LanguagePackController.SaveLanguagePack(LanguagePack);
                Log.AddInfo(string.Format(Util.LANGUAGE_Registered, Language.Text));
                base.Install();
                Completed = true;
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }

        public override void Rollback()
        {
            if (TempLanguage == null)
            {
                DeleteLanguage();
            }
            else
            {
                Localization.Localization.SaveLanguage(TempLanguage);
            }
            base.Rollback();
        }

        public override void UnInstall()
        {
            DeleteLanguage();
            base.UnInstall();
        }
    }
}
