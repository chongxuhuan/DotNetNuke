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

using DotNetNuke.Entities.Controllers;
using DotNetNuke.Tests.UI.WatiN.Utilities;
using DotNetNuke.Tests.Utilities;

using TechTalk.SpecFlow;

namespace DotNetNuke.Tests.Steps
{
    public partial class BaseSteps
    {
        /// <summary>
        /// Sets up the site to send any emails sent by the website to a local folder.
        /// Any tests involving emails should have this tag. 
        /// Steps that involve emails expect for any email files they need to be in a specific local folder.  
        /// </summary>
        [BeforeScenario("MustHaveEmailSetUpForSiteDumpToFolder")]
        public void MustHaveEmailSetUpForSiteDumpToFolder()
        {
            MailManager.SetUpMailDumpFolder();
        }

        [BeforeScenario("ClearSmtpSettings")]
        public void ClearSmtpSetings()
        {
            HostController.Instance.Update("SMTPServer", "", false);
            WebConfigManager.TouchConfig(PhysicalPath);
        }

        /// <summary>
        /// Deletes any of the email files that are currently in the local email folder. 
        /// Using this tag with any test involving emails will ensure that the test will only find relevant emails that were sent during the current test.
        /// </summary>
        [BeforeScenario("MustHaveEmptyEmailFolder")]
        public void MustHaveEmptyEmailFolder()
        {
            MailManager.ClearDumpFolder();
        }

        /// <summary>
        /// Changes the sites web.config file so that the site will run in full trust. 
        /// The site must run in full trust to send emails to a local folder. 
        /// Use this tag if a test will be sending emails to a folder and checking for those emails. 
        /// </summary>
        [BeforeScenario("SiteMustRunInFullTrust")]
        public void SiteMustRunInFullTrust()
        {
            WebConfigManager.UpdateConfigForFullTrust();
        }

        /// <summary>
        /// Changes the sites web.config file so that the site will run in medium trust. 
        /// </summary>
        [BeforeScenario("SiteMustRunInMediumTrust")]
        public void SiteMustRunInMediumTrust()
        {
            WebConfigManager.UpdateConfigForMediumTrust();
        }
    }
}
