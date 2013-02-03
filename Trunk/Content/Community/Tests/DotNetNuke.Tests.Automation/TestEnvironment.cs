#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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
using System.Configuration;
using System.IO;

namespace DotNetNuke.Tests.UI.WatiN.Common
{
    /// <summary>
    /// A collection of properties to use for tests. 
    /// Requires an App.config containing Key Value pairs for many of the properties. 
    /// </summary>
    public static class TestEnvironment
    {
        #region Variables
        public readonly static string SiteUrl = ConfigurationManager.AppSettings["SiteURL"];
        public readonly static string TestRoot = ConfigurationManager.AppSettings["TestRoot"];
        public readonly static string DatabaseServer = ConfigurationManager.AppSettings["DatabaseServer"];
        #endregion

        #region Properties

        public static string DatabasePath
        {
            get
            {
                string databasePath = ConfigurationManager.AppSettings["DatabasePath"];
                return Path.IsPathRooted(databasePath) ? databasePath : Path.Combine(TestRoot, databasePath);

            }
        }

        public static string PackagePath
        {
            get
            {
                string packagePath = ConfigurationManager.AppSettings["PackagePath"];
                return Path.IsPathRooted(packagePath) ? packagePath : Path.Combine(TestRoot, packagePath);
            }
        }

        public static string InstallorUpgrade
        {
            get { return ConfigurationManager.AppSettings["InstallorUpgrade"]; }
        }

        public static string SiteType
        {
            get
            {
                if(InstallorUpgrade.Equals("Install"))
                {
                    return ConfigurationManager.AppSettings["InstallType"];
                }
                else if (InstallorUpgrade.Equals("Upgrade"))
                {
                    return ConfigurationManager.AppSettings["UpgradePackageType"];
                }
                else
                {
                    return "Error";
                }
            }
        }

        public static string UpgradeSitePackage
        {
            get
            {
                return ConfigurationManager.AppSettings["UpgradeSitePackage"];
            }
        }

        public static string ScreenCapturePath
        {
            get
            {
                string capturePath = ConfigurationManager.AppSettings["ScreenCapturePath"];
                return Path.IsPathRooted(capturePath) ? capturePath : Path.Combine(TestRoot, capturePath);
            }
        }

        public static string TestFilesPath
        {
            get
            {
                string projectPath = ConfigurationManager.AppSettings["TestFilesPath"];
                if (!projectPath.EndsWith("Test Files"))
                {
                    projectPath = Path.Combine(projectPath, "Test Files");
                }
                return projectPath;
            }
        }
        
        public static string WebsitePath
        {
            get
            {
                string websitePath = ConfigurationManager.AppSettings["WebsitePath"];
                return Path.IsPathRooted(websitePath) ? websitePath : Path.Combine(TestRoot, websitePath);
            }
        }

        public static string ExtensionLanguagePath
        {
            get
            {
                string extensionLanguagePath = ConfigurationManager.AppSettings["ExtensionLanguagePackage"];
                if (string.IsNullOrEmpty(extensionLanguagePath))
                {
                    extensionLanguagePath = "";
                }
                extensionLanguagePath = Path.Combine(TestFilesPath, extensionLanguagePath);
                return extensionLanguagePath;
            }
        }

        public static string LanguagePath
        {
            get
            {
                string languagePath = ConfigurationManager.AppSettings["LanguagePackage"];
                if (string.IsNullOrEmpty(languagePath))
                {
                    languagePath = "";
                }
                languagePath = Path.Combine(TestFilesPath, languagePath);
                return languagePath;
            }
        }

        public static string LocaleName
        {
            get
            {
                string localeName = ConfigurationManager.AppSettings["NativeLocaleName"];
                return localeName;
            }
        }

        public static string EnglishLocaleName
        {
            get
            {
                string localeName = ConfigurationManager.AppSettings["EnglishLocaleName"];
                return localeName;
            }
        }

        public static string LanguageCode
        {
            get
            {
                string languageCode = ConfigurationManager.AppSettings["LanguageCode"];
                return languageCode;
            }
        }

        public static string ImagePath
        {
            get
            {
                string imagePath = ConfigurationManager.AppSettings["ImageName"];
                return imagePath;
            }

        }

        public static string SkinPath
        {
            get
            {
                string skinPath = ConfigurationManager.AppSettings["SkinFileName"];
                return skinPath;
            }
        }

        public static string SkinName
        {
            get
            {
                string skinName = ConfigurationManager.AppSettings["SkinName"];
                return skinName;
            }
        }

        public static string SkinUsed
        {
            get
            {
                string skinUsed = ConfigurationManager.AppSettings["SkinUsed"];
                return skinUsed;
            }
        }

        public static string ContainerUsed
        {
            get
            {
                string containerUsed = ConfigurationManager.AppSettings["ContainerUsed"];
                return containerUsed;
            }
        }

        public static string ForumPackageName
        {
            get
            {
                string packageName = ConfigurationManager.AppSettings["ForumPackageName"];
                return packageName;
            }
        }

        public static string TestEmailPath
        {
            get
            {
                string testEmailPath = ConfigurationManager.AppSettings["TestEmailPath"];
                testEmailPath = Path.Combine(TestRoot, testEmailPath);
                return testEmailPath;
            }
        }

        public static string FilePath
        {
            get
            {
                string testFilePath = ConfigurationManager.AppSettings["TestFileName"];
                if (string.IsNullOrEmpty(testFilePath))
                {
                    testFilePath = "";
                }
                testFilePath = Path.Combine(TestFilesPath, testFilePath);
                return testFilePath;
            }
        }

        public static string TestFileName
        {
            get
            {
                return ConfigurationManager.AppSettings["TestFileName"];
            }
        }

        #endregion

    }
}
