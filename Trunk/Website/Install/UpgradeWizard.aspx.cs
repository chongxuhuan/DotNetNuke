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
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Xml.XPath;

using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Localization.Internal;
using DotNetNuke.Application;
using DotNetNuke.Services.Upgrade.InternalController.Steps;
using DotNetNuke.Services.Upgrade.Internals.Steps;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Services.Install
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The InstallWizard class provides the Installation Wizard for DotNetNuke
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	01/23/2007 Created
    ///     [vnguyen]   07/09/2012 Modified
    /// </history>
    /// -----------------------------------------------------------------------------
    public partial class UpgradeWizard : PageBase
    {
        #region Private Members
        
        private const string LocalesFile = "/Install/App_LocalResources/Locales.xml";
        protected static new string LocalResourceFile = "~/Install/App_LocalResources/UpgradeWizard.aspx.resx";
        private static readonly string InstallLogFile =  Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Portals", "_default", "logs", "Install.log.resources");
        private Version _dataBaseVersion;        
        private static string _culture;
        private static string[] _supportedLanguages;

        private static IInstallationStep _currentStep;
        private static bool _upgradeRunning;
        private static int _upgradeProgress;
        private static string _statusFile;

        #endregion

        #region Protected Members
        protected Version ApplicationVersion
        {
            get
            {
                return DotNetNukeContext.Current.Application.Version;
            }
        }
        protected Version DatabaseVersion
        {
            get
            {
                return _dataBaseVersion ?? (_dataBaseVersion = DataProvider.Instance().GetVersion());
            }
        }
        #endregion

        #region Private Properties
        private static string StatusFile
        {
            get
            {
                if (!string.IsNullOrEmpty(_statusFile))
                    return _statusFile;

                return _statusFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Portals", "_default", "logs", "upgradestat.log.resources");
            }
        }
        #endregion

        #region Private Methods
        private void LocalizePage()
        {
            SetBrowserLanguage();
            versionLabel.Text = string.Format(LocalizeString("Version"), Globals.FormatVersion(ApplicationVersion));
            currentVersionLabel.Text = string.Format(LocalizeString("CurrentVersion"), Globals.FormatVersion(DatabaseVersion));
        }

        private static void GetInstallerLocales()
        {
            var filePath = Globals.ApplicationMapPath + LocalesFile.Replace("/", "\\");

            if (File.Exists(filePath))
            {
                var doc = new XPathDocument(filePath);
                var languages = doc.CreateNavigator().Select("root/language");

                if (languages.Count > 0)
                {
                    _supportedLanguages = new string[languages.Count];
                    var i = 0;
                    foreach (XPathNavigator nav in languages)
                    {
                        if (nav.NodeType != XPathNodeType.Comment)
                        {
                            _supportedLanguages.SetValue(nav.GetAttribute("key", ""), i);
                        }
                        i++;
                    }
                }
                else
                {
                    _supportedLanguages = new string[1];
                    _supportedLanguages.SetValue("en-US", 0);
                }
            }
            else
            {
                _supportedLanguages = new string[1];
                _supportedLanguages.SetValue("en-US",0);
            }
        }

        private void SetBrowserLanguage()
        {
            string cultureCode;
            if (PageLocale.Value == string.Empty)
            {
                cultureCode = TestableLocalization.Instance.BestCultureCodeBasedOnBrowserLanguages(_supportedLanguages);
                PageLocale.Value = cultureCode;
            }
            else
            {
                cultureCode = PageLocale.Value;
            }
            _culture = cultureCode;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
        }

        private static string LocalizeStringStatic(string key)
        {
            return Localization.Localization.GetString(key, LocalResourceFile, _culture);
        }
        
        private static void LaunchUpgrade()
        {
            ResetInstallLog(true);

            //Get current Script time-out
            var scriptTimeOut = HttpContext.Current.Server.ScriptTimeout;

            //Set Script timeout to MAX value
            HttpContext.Current.Server.ScriptTimeout = int.MaxValue;

            if (_culture != null) Thread.CurrentThread.CurrentUICulture = new CultureInfo(_culture);

            //bail out early if upgrade is in progress
            if (_upgradeRunning)
                return;

            var percentForEachStep = 100 / _steps.Count;
            var useGenericPercent = false;
            var totalPercent = _steps.Sum(step => step.Value);
            if (totalPercent != 100) useGenericPercent = true;

            _upgradeRunning = true;
            _upgradeProgress = 0;

            foreach (var step in _steps)
            {
                _currentStep = step.Key;

                try
                {
                    _currentStep.Activity += CurrentStepActivity;
                    _currentStep.Execute();
                }
                catch (Exception ex)
                {
                    CurrentStepActivity(Localization.Localization.GetString("ErrorInStep", LocalResourceFile) + ": " + ex.Message);
                    _upgradeRunning = false;
                    return;
                }
                switch (_currentStep.Status)
                {
                    case StepStatus.AppRestart:
                        _upgradeRunning = false;
                        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, true);
                        break;
                    default:
                        if (_currentStep.Status != StepStatus.Done)
                        {
                            CurrentStepActivity(string.Format(Localization.Localization.GetString("ErrorInStep", LocalResourceFile)
                                                                                                  , _currentStep.Errors.Count > 0 ? string.Join(",", _currentStep.Errors.ToArray()) : _currentStep.Details));
                            _upgradeRunning = false;
                            return;
                        }
                        break;
                }
                if (useGenericPercent)
                    _upgradeProgress += percentForEachStep;
                else
                    _upgradeProgress += step.Value;
            }

            _currentStep = null;
            _upgradeProgress = 100;
            CurrentStepActivity(Localization.Localization.GetString("UpgradeDone", LocalResourceFile));

            //indicate we are done
            _upgradeRunning = false;

            //Sleep for few seconds and then delete the status file. This should not affect UI as it's polling through a different method
            Thread.Sleep(10000);
            try
            {
                File.Delete(StatusFile);
            }
            catch (Exception)
            {
                //TODO - probably do nothing
                //throw;
            }

            //restore Script timeout
            HttpContext.Current.Server.ScriptTimeout = scriptTimeOut;
        }

        private static void CurrentStepActivity(string status)
        {
            var percentage = (_currentStep == null) ? _upgradeProgress : _upgradeProgress + (_currentStep.Percentage / _steps.Count);
            var obj = new
            {
                progress = percentage,
                details = status,
                check0 = upgradeDatabase.Status.ToString() + (upgradeDatabase.Errors.Count == 0 ? "" : " Errors " + upgradeDatabase.Errors.Count),
                check1 = upgradeExtensions.Status.ToString() + (upgradeExtensions.Errors.Count == 0 ? "" : " Errors " + upgradeExtensions.Errors.Count)
            };

            try
            {
                using (var sw = File.CreateText(StatusFile))
                {
                    sw.WriteLine(obj.ToJson());
                }
            }
            catch (Exception)
            {
                //TODO - do something                
            }
        }

        private static void ResetInstallLog(bool recreate)
        {
            if (File.Exists(InstallLogFile))
            {
                var archiveFilename = InstallLogFile.Replace("Install.log.resources", Globals.FormatVersion(DataProvider.Instance().GetVersion()) + ".Install.log.resources");
                int i = 1;
                while (File.Exists(archiveFilename))
                {
                    archiveFilename = InstallLogFile.Replace("Install.log.resources", Globals.FormatVersion(DataProvider.Instance().GetVersion()) + ".Install.log" + " - Copy (" + i.ToString() + ")" + ".resources");
                    i++;
                }

                File.Move(InstallLogFile, archiveFilename);
                if (recreate)
                {
                    Thread.Sleep(3000);
                    File.Create(InstallLogFile);
                }
            }
        }


        #endregion

        #region Protected Methods
        protected string LocalizeString(string key)
        {
            return Localization.Localization.GetString(key, LocalResourceFile, _culture);
        }
        
        protected override void OnError(EventArgs e)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Server.Transfer("~/ErrorPage.aspx");
        }
        #endregion

        #region Event Handlers
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Page_Init runs when the Page is initialised
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	02/14/2007	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            GetInstallerLocales();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Page_Load runs when the Page loads
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	02/09/2007	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LocalizePage();
        }
        #endregion
        
        #region "Web Methods"
        //steps shown in UI
        static IInstallationStep upgradeDatabase = new InstallDatabaseStep();
        static IInstallationStep upgradeExtensions = new InstallExtensionsStep();

        //Ordered List of Steps (and weight in percentage) to be executed
        private static IDictionary<IInstallationStep, int> _steps = new Dictionary<IInstallationStep, int>
                                        { {upgradeDatabase, 50}, {upgradeExtensions, 50} };

        [System.Web.Services.WebMethod()]
        public static Tuple<bool, string> ValidateInput(Dictionary<string, string> accountInfo)
        {
            var result = true;
            var errorMsg = string.Empty;
            
            UserLoginStatus loginStatus = UserLoginStatus.LOGIN_FAILURE;
            UserInfo hostUser = UserController.ValidateUser(-1, accountInfo["username"], accountInfo["password"], "DNN", "", "", AuthenticationLoginBase.GetIPAddress(), ref loginStatus);

            if (loginStatus == UserLoginStatus.LOGIN_FAILURE || !hostUser.IsSuperUser)
            {
                //Response.Redirect("~/Install/UpgradeWizard.aspx");
                result = false;
                errorMsg = LocalizeStringStatic("InvalidCredentials");
            }

            return new Tuple<bool, string>(result, errorMsg);
        }
        
        [System.Web.Services.WebMethod()]
        public static void RunUpgrade()
        {
            _upgradeRunning = false;
            LaunchUpgrade();
        }

        [System.Web.Services.WebMethod()]
        public static object GetUpgradeProgress()
        {
            var data = string.Empty;
            try
            {
                data = File.ReadAllText(StatusFile);
            }
            catch (Exception)
            {
                //TODO - do something
            }

            return data;
        }

        [System.Web.Services.WebMethod()]
        public static object GetInstallationLog(int startRow)
        {
            var data = string.Empty;
            try
            {
                var lines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Portals", "_default", "logs", "install.log.resources"));

                if (lines.Length > startRow)
                {
                    var count = lines.Length - startRow > 500 ? 500 : lines.Length - startRow;
                    var sb = new System.Text.StringBuilder();
                    for (var i = startRow; i < startRow + count; i++)
                    {
                        sb.Append(lines[i]);
                        sb.Append("<br/>");
                    }

                    data = sb.ToString();
                }
            }
            catch (Exception)
            {
            }

            return data;
        }
        #endregion
    }
}