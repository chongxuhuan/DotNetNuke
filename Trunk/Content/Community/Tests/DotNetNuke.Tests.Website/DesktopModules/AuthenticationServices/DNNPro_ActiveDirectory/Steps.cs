using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Services.Installer.Packages;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace DotNetNuke.Tests.Website.DesktopModules.AuthenticationServices.DNNPro_ActiveDirectory
{
    [Binding]
    public class Steps : AutomationBase
    {
        [Then(@"The CE AD AUth Provider will be uninstalled")]
        public void ThenTheCEADAUthProviderWillBeUninstalled()
        {
            var package = PackageController.GetPackageByName(PortalId, "DNN_ActiveDirectoryAuthentication");
            Assert.IsNull(package);
        }
    }
}
