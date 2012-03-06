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
using System.IO;
using System.Web;

using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Instrumentation;
using DotNetNuke.Professional.Application;

using FiftyOne.Foundation.Mobile.Configuration;
using FiftyOne.Foundation.Mobile.Detection;

#endregion

namespace DotNetNuke.Provider.Professional.FiftyOnePremiumProvider
{
    /// <summary>
    /// FiftyOne Premium Detector Module
    /// </summary>
    public class DetectModule : DetectorModule
    {
        #region Properties

        private const string TrialKey = "88DAAAEZBAYDSAJABJAGAWEKXGKV5TU5B9TKJCJAV2UMZAEG8PKKB3BW948T7T5L2UQZHC7YJ6X7SPS229KT8EC";

        #endregion

        #region Override Methods

        public override void Init(HttpApplication application)
        {
            //call license check.
            try
            {

                var proApplication = DotNetNukeContext.Current.Application as ProApplication;
                if(proApplication == null)
                {
                    return;
                }
                
                var storedKey = GetLicense();
                if (!proApplication.HasActiveLicenseOrIsTrial && string.IsNullOrEmpty(storedKey))
                {
                    RemovePremiumData();
                }
                else
                {
                    LicenceKey.AddKey(TrialKey);
                }
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
            }

            base.Init(application);
        }

        #endregion

        #region Private Methods

        private void RemovePremiumData()
        {
            //remove the premium data file if product doesn't have valid license.
            var binaryFilePath = GetBinaryFilePath();

            if (binaryFilePath.StartsWith("~/"))
            {
                binaryFilePath = binaryFilePath.Substring(2);
            }
            binaryFilePath = Path.Combine(Globals.ApplicationMapPath, binaryFilePath);
            if (File.Exists(binaryFilePath))
            {
                File.Delete(binaryFilePath);
            }
        }

        private string GetBinaryFilePath()
        {
            var binaryFilePath = string.Empty;
            var section = Support.GetWebApplicationSection("fiftyOne/detection", false);
            if (section != null)
            {
                var property = section.ElementInformation.Properties["binaryFilePath"];
                if (property != null && property.Value != null)
                {
                    binaryFilePath = property.Value.ToString();
                }
            }

            return binaryFilePath;
        }

        private string GetLicense()
        {
            var licenseKey = string.Empty;
            var licenseFile = Path.Combine(Globals.ApplicationMapPath, "bin\\" + LicenceKey.LicenceKeyFileName);
            if (File.Exists(licenseFile))
            {
                licenseKey = File.ReadAllText(licenseFile);
            }

            return licenseKey;
        }

        #endregion
    }
}