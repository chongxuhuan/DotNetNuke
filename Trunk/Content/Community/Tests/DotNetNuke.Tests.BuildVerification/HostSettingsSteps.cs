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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using NUnit.Framework;
using TechTalk.SpecFlow;
using WatiN.Core;

namespace DotNetNuke.Tests.BuildVerification
{
    [Binding]
    public class HostSettingsSteps :WatiNTest
    {
        public HostSettingsPage HostSettingsPage
        {
            get
            {
                return GetPage<HostSettingsPage>();
            }
        }

        /// <summary>
        /// Enters a valid email (test@dnn.com) into the Host Email field on the Host Settings page.
        /// This step must be performed anytime a test will be updating the Host Settings for the site.
        /// </summary>
        [Given(@"I have updated the Host Email on the Settings page")]
        public void GivenIHaveUpdatedTheHostEmailOnTheSettingsPage()
        {
            HostSettingsPage.HostEmailTextField.Value = "test@dnn.com";
            HostSettingsPage.UpdateLink.Click();
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Enters the server specified into the SMTP server field on the Host Settings page.
        /// If the test will being sending emails to a local folder the server will need to be localhost.
        /// </summary>
        /// <param name="server">The server being used to SMTP.</param>
        [When(@"I enter (.*) into the SMTP server field")]
        public void WhenIEnterIntoTheSMTPServerField(string server)
        {
            HostSettingsPage.SMTPServerField.Value = server;
        }

        /// <summary>
        /// Select Annonymous as the SMTP authentication method. 
        /// </summary>
        [When(@"I select Annonymous SMTP Authentication")]
        public void WhenISelectAnnonymousSMTPAuthentication()
        {
            HostSettingsPage.SMTPAnonymousAuthenticationRadioButton.Checked = true;
        }

        /// <summary>
        /// Clicks the Test SMTP Settings link on the Host Settings page.
        /// </summary>
        [When(@"I click the SMTP Test link")]
        public void WhenIClickTheSMTPTestLink()
        {
            Thread.Sleep(1000);
            HostSettingsPage.SMTPTestLink.Click();
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Searches for a test email that was sent to the host in the local email folder.
        /// Confirms that the email file exists and that it contains some expected text. 
        /// This method expects emails to be sent to the Tests\Packages\TestEmails folder.
        /// </summary>
        [Then(@"A test email should be sent to the host")]
        public void ThenATestEmailShouldBeSentToTheHost()
        {
            string physicalPath = Directory.GetCurrentDirectory();
            string packagePath = physicalPath.Replace("\\Fixtures", "\\Packages");
            string emailPath = packagePath + "\\TestEmails";
            string emailFileName = null;
            string currentEmailContent = null;
            string emailContent = null;
            foreach (string email in Directory.GetFiles(emailPath))
            {
                currentEmailContent = File.ReadAllText(email);
                if (currentEmailContent.Contains("X-Receiver: test@dnn.com") && currentEmailContent.Contains("X-Sender: test@dnn.com"))
                {
                    emailFileName = email;
                    emailContent = currentEmailContent;
                    break;
                }
            }

            //Convert the EmailAddress content
            Assert.IsTrue(emailContent != null, "The test email could not be found. The test was looking in: " + emailPath);
            String emailSubject = FindLineUsingRegex("Subject", emailContent, emailFileName);
            Assert.IsTrue(emailSubject.Contains("DotNetNuke SMTP Configuration Test"), "The test email subject was incorrect. Looking at string: " + emailSubject);
        }

        #region Private Methods  

        /// <summary>
        /// Locates the line with the Prefix specified in the email content.
        /// </summary>
        /// <param name="linePrefix">The prefix for the line that will be returned.</param>
        /// <param name="emailContent">The content of the email file.</param>
        /// <param name="emailFileName">The name of the email file. Used for error reporting if the line isn't found.</param>
        /// <returns>The line found.</returns>
        private String FindLineUsingRegex(string linePrefix, string emailContent, string emailFileName)
        {
            string lineRegex = linePrefix + @": .*";
            Match match = Regex.Match(@emailContent, @lineRegex);
            Assert.IsTrue(match.Success, "Could not find " + linePrefix + " Line! Looking in file: " + emailFileName);
            String line = match.Value;
            return line;
        }

        #endregion

    }
}
